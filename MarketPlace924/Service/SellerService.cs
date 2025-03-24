using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
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

        public async Task<Seller?> GetSellerAsync(string username)
        {
            return await _sellerRepository.GetSellerAsync(username);
        }

        public async Task<List<string>?> GetNotificationsAsync(int sellerID)
        {
            return await _sellerRepository.GetNotificationsAsync(sellerID);
        }

        public async Task<List<Product>> GetAllProductsAsync(int sellerID)
        {
            return await _sellerRepository.GetProductsAsync(sellerID);
        }

        public async Task<int> GetSellerIDByUsernameAsync(string username)
        {
            return await _sellerRepository.GetSellerIDByUsernameAsync(username);
        }

        public async Task UpdateSellerAsync(Seller seller)
        {
            await _sellerRepository.UpdateSellerAsync(seller);
        }
    }
}
