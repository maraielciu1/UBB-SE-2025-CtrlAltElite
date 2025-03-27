using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    public class SellerService
    {
        private SellerRepository _sellerRepository;

        public SellerService(SellerRepository sellerRepository)
        {
            _sellerRepository = sellerRepository;
        }
        public Seller GetSellerByUser(User user)
        {
            var seller = new Seller();
            seller.User = user;
            _sellerRepository.LoadSellerInfo(seller);
            return seller;
        }

        public async Task<List<Product>> GetAllProductsAsync(int sellerID)
        {
            return await _sellerRepository.GetProductsAsync(sellerID);
        }

        public async Task UpdateSellerAsync(Seller seller)
        {
            await _sellerRepository.UpdateSellerAsync(seller);
        }

        public async Task CreateSeller(Seller seller)
        {
            await _sellerRepository.CreateSeller(seller);
        }

        public async Task<double> CalculateAverageReviewScore(int sellerId)
        {
            return await _sellerRepository.GetTrustScoreBySellerId(sellerId);
        }

        public async Task<List<string>> GetNotifications(int sellerId, int currentFollowerCount)
        {
            return await _sellerRepository.GetNotifications(sellerId, currentFollowerCount);
        }
    }
}
