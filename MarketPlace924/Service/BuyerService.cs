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
        private BuyerRepository _buyerRepository;
        private UserRepository _userRepository;

        // Constructor initializing repositories
        public BuyerService(BuyerRepository buyerRepo, UserRepository userRepo)
        {
            _buyerRepository = buyerRepo;
            _userRepository = userRepo;
        }

        // Retrieves a Buyer object associated with the given User.
        public async Task<Buyer> GetBuyerByUser(User user)
        {
            var buyer = new Buyer(); // Create a new Buyer instance
            buyer.User = user; // Associate the buyer with the given user
            await _buyerRepository.LoadBuyerInfo(buyer); // Load buyer details from repository
            return buyer;
        }

        // Retrieves the list of seller IDs that the buyer follows.
        public async Task<List<int>> GetFollowingUsersIDs(int buyerId)
        {
            return await _buyerRepository.GetFollowingUsersIds(buyerId);
        }

        // Retrieves all products from the sellers that the buyer follows.
        public async Task<List<Product>> GetProductsFromFollowedSellers(List<int> followedSellersIDs)
        {
            List<Product> allProducts = new List<Product>();
            foreach (var sellerId in followedSellersIDs)
            {
                var products = await _buyerRepository.GetProductsFromSeller(sellerId);
                allProducts.AddRange(products); // Aggregate products from all followed sellers
            }
            return allProducts;
        }

        // Retrieves the list of followed sellers based on buyer's followed seller IDs.
        public async Task<List<Seller>> GetFollowedSellers(List<int> followingUsersID)
        {
            return await _buyerRepository.GetFollowedSellers(followingUsersID);
        }

        // Retrieves products of the seller that the buyer is currently viewing.
        public async Task<List<Product>> GetProductsForViewProfile(int sellerId)
        {
            List<Product> allProducts = new List<Product>();
            var products = await _buyerRepository.GetProductsFromSeller(sellerId);
            allProducts.AddRange(products);
            return allProducts;
        }

        // Checks if a buyer is following a specific seller.
        public async Task<bool> IsFollowing(int buyerId, int sellerId)
        {
            return await _buyerRepository.IsFollowing(buyerId, sellerId);
        }

        // Adds a seller to the buyer's following list.
        public async Task FollowSeller(int buyerId, int sellerId)
        {
            await _buyerRepository.FollowSeller(buyerId, sellerId);
        }

        // Removes a seller from the buyer's following list.
        public async Task UnfollowSeller(int buyerId, int sellerId)
        {
            await _buyerRepository.UnfollowSeller(buyerId, sellerId);
        }
    }
}
