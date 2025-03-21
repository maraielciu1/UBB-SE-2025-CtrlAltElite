using System;
using System.Collections.Generic;
using System.Linq;
using MarketPlace924.Domain;
using MarketPlace924.Repository;
using MarketPlace924.ViewModel;

namespace MarketPlace924.Service;

public class BuyerService
{
    private BuyerRepository _buyerRepo;
    private UserRepository _userRepo;

    public BuyerService(BuyerRepository buyerRepo, UserRepository userRepo)
    {
        _buyerRepo = buyerRepo;
        _userRepo = userRepo;
    }

    public Buyer GetBuyerByUser(User user)
    {
        // TODO Handle Buyer Not Found; Exception?
        var buyer = new Buyer();
        buyer.User = user;
        LoadBuyer(buyer, BuyerDataSegments.BasicInfo | BuyerDataSegments.Wishlist | BuyerDataSegments.Linkages);
        return buyer;
    }

    public void SaveInfo(Buyer buyer)
    {
        //TODO run this atomically. put the A in ACID
        _buyerRepo.SaveInfo(buyer);
        _userRepo.UpdateContactInfo(buyer.User);
    }


    public List<Buyer> FindBuyersWithShippingAddress(Address currentBuyerShippingAddress)
    {
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
        _buyerRepo.UpdateLinkageRequest( linkedBuyer.Id, userBuyer.Id);
    }

    public void RefuseLinkageRequest(Buyer userBuyer, Buyer linkedBuyer)
    {
        _buyerRepo.DeleteLinkageRequest( linkedBuyer.Id, userBuyer.Id);
    }
}