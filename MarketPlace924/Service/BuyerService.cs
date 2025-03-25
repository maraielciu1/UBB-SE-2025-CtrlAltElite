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
        private BuyerRepository _buyerRepository;
        private UserRepository _userRepository;

        public BuyerService(BuyerRepository buyerRepo, UserRepository userRepo)
        {
            _buyerRepository = buyerRepo;
            _userRepository = userRepo;
        }

        // Retrieves a Buyer object associated with the given User.
        public async Task<Buyer> GetBuyerByUser(User user)
        {
            var buyer = new Buyer();
            buyer.User = user;
            await _buyerRepository.LoadBuyerInfo(buyer);
            return buyer;
        }

        // Retrieves the list of seller IDs that the buyer follows.
        public async Task<List<int>> GetFollowingUsersIDs(int buyerID)
        {
            return await _buyerRepository.GetFollowingUsersIds(buyerID);
        }

        // Adds a seller to the buyer's following list.
        public void FollowSeller(int sellerID)
        {
            // TODO: Implement database logic to follow a seller.
        }

        // Removes a seller from the buyer's following list.
        public void UnfollowSeller(int sellerID)
        {
            // TODO: Implement database logic to unfollow a seller.
        }

        // Retrieves the list of followed sellers based on buyer's followed seller IDs.
        public async Task<List<Seller>> GetFollowedSellers(List<int> followingUsersID)
        {
            return await _buyerRepository.GetFollowedSellers(followingUsersID);
        }

        // Retrieves all products from the sellers that the buyer follows.
        public async Task<List<Product>> GetProductsFromFollowedSellers(List<int> followedSellersIDs)
        {
            List<Product> allProducts = new List<Product>();
            foreach (var sellerID in followedSellersIDs)
            {
                var products = await _buyerRepository.GetProductsFromSeller(sellerID);
                allProducts.AddRange(products);
            }
            return allProducts;
        }
    }
}
