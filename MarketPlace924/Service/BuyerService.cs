using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace924.Domain;
using MarketPlace924.Repository;

namespace MarketPlace924.Service;

public class BuyerService
{
    // Repository instances for managing buyer and user data
    private BuyerRepository _buyerRepo;
    private UserRepository _userRepo;

    // Constructor initializing repositories
    public BuyerService(BuyerRepository buyerRepo, UserRepository userRepo)
    {
        _buyerRepo = buyerRepo;
        _userRepo = userRepo;
    }

    // Retrieves a Buyer object associated with the given User.
    public async Task<Buyer> GetBuyerByUser(User user)
    {
        var buyer = new Buyer();
        buyer.User = user;
        await LoadBuyer(buyer, BuyerDataSegments.BasicInfo | BuyerDataSegments.Wishlist | BuyerDataSegments.Linkages);
        return buyer;
    }


    public async Task CreateBuyer(Buyer buyer)
    {
        ValidateBuyerInfo(buyer);
        //TODO run this atomically. put the A in ACID
        await _buyerRepo.CreateBuyer(buyer);
        await _userRepo.UpdateContactInfo(buyer.User);
    }

    public async Task SaveInfo(Buyer buyer)
    {
        ValidateBuyerInfo(buyer);
        //TODO run this atomically. put the A in ACID
        await _buyerRepo.SaveInfo(buyer);
        await _userRepo.UpdateContactInfo(buyer.User);
    }


    public async Task<List<Buyer>> FindBuyersWithShippingAddress(Address currentBuyerShippingAddress)
    {
        if (currentBuyerShippingAddress.Country == null) //i.e. its an empty address
        {
            return new List<Buyer>();
        }

        var buyers = await _buyerRepo.FindBuyersWithShippingAddress(currentBuyerShippingAddress);
        foreach (var buyer in buyers)
        {
            await LoadBuyer(buyer, BuyerDataSegments.BasicInfo | BuyerDataSegments.User);
        }

        return buyers;
    }

    public async Task LoadBuyer(Buyer buyer, BuyerDataSegments buyerDataSegments)
    {
        if ((buyerDataSegments & BuyerDataSegments.BasicInfo) == BuyerDataSegments.BasicInfo)
        {
            await _buyerRepo.LoadBuyerInfo(buyer);
        }

        if ((buyerDataSegments & BuyerDataSegments.User) == BuyerDataSegments.User)
        {
           await _userRepo.LoadUserContactById(buyer.User);
        }

        if ((buyerDataSegments & BuyerDataSegments.Wishlist) == BuyerDataSegments.Wishlist)
        {
            buyer.Wishlist = await _buyerRepo.GetWishlist(buyer.Id);
        }

        if ((buyerDataSegments & BuyerDataSegments.Linkages) == BuyerDataSegments.Linkages)
        {
            buyer.Linkages = await _buyerRepo.GetBuyerLinkages(buyer.Id);
           var linkedBuyers = buyer.Linkages.Select(linkage => linkage.Buyer).ToList();
           foreach (var linkedBuyer in linkedBuyers)
           {
               await LoadBuyer(linkedBuyer,
                   BuyerDataSegments.BasicInfo | BuyerDataSegments.User | BuyerDataSegments.Wishlist);
           }
        }
    }


    public async Task CreateLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
    {
        await _buyerRepo.CreateLinkageRequest(userBuyer.Id, linkedBuyer.Id);
    }

    public async Task BreakLinkage(Buyer userBuyer, Buyer linkedBuyer)
    {
        _ = await _buyerRepo.DeleteLinkageRequest(userBuyer.Id, linkedBuyer.Id) ||
            await _buyerRepo.DeleteLinkageRequest(linkedBuyer.Id, userBuyer.Id);
    }

    public async Task CancelLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
    {
        _ = await _buyerRepo.DeleteLinkageRequest(userBuyer.Id, linkedBuyer.Id);
    }

    public async Task AcceptLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
    {
        await _buyerRepo.UpdateLinkageRequest(linkedBuyer.Id, userBuyer.Id);
    }

    public async Task RefuseLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
    {
        await _buyerRepo.DeleteLinkageRequest(linkedBuyer.Id, userBuyer.Id);
    }


    // My Market Functionalities


    // Retrieves the list of seller IDs that the buyer follows.
    public async Task<List<int>> GetFollowingUsersIDs(int buyerId)
    {
        return await _buyerRepo.GetFollowingUsersIds(buyerId);
    }

    // Retrieves all products from the sellers that the buyer follows.
    public async Task<List<Product>> GetProductsFromFollowedSellers(List<int> followedSellersIDs)
    {
        List<Product> allProducts = new List<Product>();
        foreach (var sellerId in followedSellersIDs)
        {
            var products = await _buyerRepo.GetProductsFromSeller(sellerId);
            allProducts.AddRange(products); // Aggregate products from all followed sellers
        }

        return allProducts;
    }

    // Retrieves the list of followed sellers based on buyer's followed seller IDs.
    public async Task<List<Seller>> GetFollowedSellers(List<int> followingUserIds)
    {
        return await _buyerRepo.GetFollowedSellers(followingUserIds);
    }

    // Retrieves the list of all sellers.
    public async Task<List<Seller>> GetAllSellers()
    {
        return await _buyerRepo.GetAllSellers();
    }

    public async Task UpdateAfterPurchase(Buyer buyer, decimal purchaseAmount)
    {
        buyer.UpdateAfterPurchase(purchaseAmount);
        await _buyerRepo.UpdateAfterPurchase(buyer);
    }

    public async Task RemoveWishilistItem(Buyer buyer, int productId)
    {
        await _buyerRepo.RemoveWishilistItem(buyer.Id, productId);
        var foundItem = buyer.Wishlist.Items.Find(item => item.ProductId == productId);
        if (foundItem != null)
        {
            buyer.Wishlist.Items.Remove(foundItem);
        }
    }

    private void ValidateBuyerInfo(Buyer buyer)
    {
        ValidateMandatoryField("First Name", buyer.FirstName);
        ValidateMandatoryField("Last Name", buyer.LastName);
        if (!UserValidator.ValidatePhone(buyer.PhoneNumber))
        {
            throw new ArgumentException("Invalid Phone Number");
        }

        ValidateAddress(buyer.BillingAddress);
        if (!buyer.UseSameAddress)
        {
            ValidateAddress(buyer.ShippingAddress);
        }
    }

    private void ValidateAddress(Address addr)
    {
        ValidateMandatoryField("Stree Name", addr.StreetLine);
        ValidateMandatoryField("Postal Code", addr.PostalCode);
        ValidateMandatoryField("City", addr.City);
        ValidateMandatoryField("Country", addr.Country);
    }


    private void ValidateMandatoryField(string fieldName, string? fieldValue)
    {
        if (string.IsNullOrWhiteSpace(fieldValue))
        {
            throw new ArgumentException(fieldName + " is required");
        }
    }

    // Retrieves products of the seller that the buyer is currently viewing.
    public async Task<List<Product>> GetProductsForViewProfile(int sellerId)
    {
        List<Product> allProducts = new List<Product>();
        var products = await _buyerRepo.GetProductsFromSeller(sellerId);
        allProducts.AddRange(products);
        return allProducts;
    }

    // Checks if buyer exists in the database
    public async Task<bool> CheckIfBuyerExists(int buyerId)
    {
        return await _buyerRepo.CheckIfBuyerExists(buyerId);
    }


    // Checks if a buyer is following a specific seller.
    public async Task<bool> IsFollowing(int buyerId, int sellerId)
    {
        return await _buyerRepo.IsFollowing(buyerId, sellerId);
    }

    // Adds a seller to the buyer's following list.
    public async Task FollowSeller(int buyerId, int sellerId)
    {
        await _buyerRepo.FollowSeller(buyerId, sellerId);
    }

    // Removes a seller from the buyer's following list.
    public async Task UnfollowSeller(int buyerId, int sellerId)
    {
        await _buyerRepo.UnfollowSeller(buyerId, sellerId);
    }
}