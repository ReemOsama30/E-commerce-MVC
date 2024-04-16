using E_commerce.Models;

namespace E_commerce_MVC.viewModels
{
    public class CartIndexViewModel
    {
        public CartIndexViewModel()
        {
            ProductName = new List<string>();
            ProductId = new List<int>();
            ImgUrl = new List<string>();
            Price = new List<double>();
            Quantity = new List<int>();
            Total = new List<double>();
        }
        public List<string> ProductName { get; set; }
        public List<int> ProductId { get; set; }
        public List<string> ImgUrl { get; set; }
        public List<double> Price { get; set; }
        public List<int> Quantity { get; set; }
        public List<double> Total { get; set; }
    }
}
