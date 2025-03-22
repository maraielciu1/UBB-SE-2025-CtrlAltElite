using MarketPlace924.Domain;
using MarketPlace924.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MarketPlace924.Service
{
    class SellerService
    {
        private SellerRepository _sellerRepository;
        public SellerService(SellerRepository sellerRepository) 
        {
            _sellerRepository = sellerRepository;
        }

        public Seller? GetSeller(int sellerID)
        {
            return _sellerRepository.GetSeller(sellerID);
        }

        public List<String>? GetNotifications(int sellerID)
        {
            return _sellerRepository.GetNotifications(sellerID);
        }

        public List<Product> GetProducts(int sellerID)
        {
            return _sellerRepository.GetProducts(sellerID);
        }

        public void UpdateSeller(Seller seller)
        {
            _sellerRepository.UpdateSeller(seller);
        }
    }
}
