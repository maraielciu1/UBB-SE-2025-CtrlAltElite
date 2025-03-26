using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Domain
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int SellerId { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, string description, double price, int stock, int sellerId)
        {
            ProductId = id;
            Name = name;
            Description = description;
            Price = price;
            SellerId = sellerId;
            Stock = stock;
        }
    }
}
