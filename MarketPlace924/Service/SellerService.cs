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

        public Seller? GetSeller(string username)
        {
            return _sellerRepository.GetSeller(username);
        }

        public List<String>? GetNotifications(int sellerID)
        {
            return _sellerRepository.GetNotifications(sellerID);
        }

        public List<Product> GetAllProducts(int sellerID)
        {
            return _sellerRepository.GetProducts(sellerID);
        }

        public int GetSellerIDByUsername(string username)
        {
            return _sellerRepository.GetSellerIDByUsername(username);
        }

        public void UpdateSeller(Seller seller)
        {
            _sellerRepository.UpdateSeller(seller);
        }
    }
}
