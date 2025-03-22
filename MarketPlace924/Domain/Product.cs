using System;

namespace MarketPlace924.Domain
{
    class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int SellerId { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, string description, double price, int stock, int sellerId)
        {
            ID = id;
            Name = name;
            Description = description;
            Price = price;
            SellerId = sellerId;
            Stock = stock;
        } 
    }
}
