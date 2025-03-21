using MarketPlace924.Domain;
using MarketPlace924.Repository;

namespace MarketPlace924.Service;

public class BuyerService
{
    private BuyerRepository _buyerRepo;
    private UserRepository _userRepo;

    public BuyerService(BuyerRepository buyerRepo, UserRepository userRepo)
    {
        this._buyerRepo = buyerRepo;
        this._userRepo = userRepo;
    }

    public Buyer GetBuyerByUser(User user)
    {
        // TODO Handle Buyer Not Found; Exception?
        var buyer = _buyerRepo.GetBuyer(user.UserID) ?? new Buyer();
        buyer.User = user;
        return buyer;
    }

    public void SaveInfo(Buyer buyer)
    {
        //TODO run this atomically. put the A in ACID
        _buyerRepo.SaveInfo(buyer);
        _userRepo.UpdateContactInfo(buyer.User);
    }
}