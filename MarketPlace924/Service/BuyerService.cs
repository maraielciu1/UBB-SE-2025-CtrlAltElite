using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
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
            // TODO Handle Buyer Not Found; Exception?
            var buyer = new Buyer();
            buyer.User = user;
            LoadBuyer(buyer, BuyerDataSegments.BasicInfo | BuyerDataSegments.Wishlist | BuyerDataSegments.Linkages);
            return buyer;
        }


        public void CreateBuyer(Buyer buyer)
        {
            //TODO run this atomically. put the A in ACID
            _buyerRepo.CreateBuyer(buyer);
            _userRepo.UpdateContactInfo(buyer.User);
        }

        public void SaveInfo(Buyer buyer)
        {
            //TODO run this atomically. put the A in ACID
            _buyerRepo.SaveInfo(buyer);
            _userRepo.UpdateContactInfo(buyer.User);
        }


        public List<Buyer> FindBuyersWithShippingAddress(Address currentBuyerShippingAddress)
        {
            if (currentBuyerShippingAddress.Country == null) //i.e. its an empty address
            {
                return new List<Buyer>();
            }
            var buyers = _buyerRepo.FindBuyersWithShippingAddress(currentBuyerShippingAddress);
            buyers.ForEach(buyer => LoadBuyer(buyer, BuyerDataSegments.BasicInfo | BuyerDataSegments.User));
            return buyers;
        }

        public void LoadBuyer(Buyer buyer, BuyerDataSegments buyerDataSegments)
        {
            if ((buyerDataSegments & BuyerDataSegments.BasicInfo) == BuyerDataSegments.BasicInfo)
            {
                _buyerRepo.LoadBuyerInfo(buyer);
            }

            if ((buyerDataSegments & BuyerDataSegments.User) == BuyerDataSegments.User)
            {
                _userRepo.LoadUserContactById(buyer.User);
            }

            if ((buyerDataSegments & BuyerDataSegments.Wishlist) == BuyerDataSegments.Wishlist)
            {
                buyer.Wishlist = _buyerRepo.GetWishlist(buyer.Id);
            }

            if ((buyerDataSegments & BuyerDataSegments.Linkages) == BuyerDataSegments.Linkages)
            {
                buyer.Linkages = _buyerRepo.GetBuyerLinkages(buyer.Id);
                buyer.Linkages.Select(linkage => linkage.Buyer).ToList().ForEach(linikedBuyer =>
                    LoadBuyer(linikedBuyer,
                        BuyerDataSegments.BasicInfo | BuyerDataSegments.User | BuyerDataSegments.Wishlist));
            }
        }


        public void CreateLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
        {
            _buyerRepo.CreateLinkageRequest(userBuyer.Id, linkedBuyer.Id);
        }

        public void BreakLinkage(Buyer userBuyer, Buyer linkedBuyer)
        {
            _ = _buyerRepo.DeleteLinkageRequest(userBuyer.Id, linkedBuyer.Id) ||
                _buyerRepo.DeleteLinkageRequest(linkedBuyer.Id, userBuyer.Id);
        }

        public void CancelLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
        {
            _ = _buyerRepo.DeleteLinkageRequest(userBuyer.Id, linkedBuyer.Id);
        }

        public void AcceptLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
        {
            _buyerRepo.UpdateLinkageRequest(linkedBuyer.Id, userBuyer.Id);
        }

        public void RefuseLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
        {
            _buyerRepo.DeleteLinkageRequest(linkedBuyer.Id, userBuyer.Id);
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
        public async Task<List<Seller>> GetFollowedSellers(List<int> followingUsersID)
        {
            return await _buyerRepo.GetFollowedSellers(followingUsersID);
        }

        // Retrieves products of the seller that the buyer is currently viewing.
        public async Task<List<Product>> GetProductsForViewProfile(int sellerId)
        {
            List<Product> allProducts = new List<Product>();
            var products = await _buyerRepo.GetProductsFromSeller(sellerId);
            allProducts.AddRange(products);
            return allProducts;
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
}
