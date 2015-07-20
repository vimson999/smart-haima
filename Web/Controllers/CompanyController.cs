using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL;

namespace Web.Controllers
{
    public class CompanyController : ApiController
    {
        [HttpGet]
        public List<Companies> all()
        {
            DataService ds = new DataService();
            return ds.GetCompanies(1, 10);
        }

        Product[] products = new Product[] 
        { 
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 }, 
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M }, 
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M } 
        };

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            return products;
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }
}
