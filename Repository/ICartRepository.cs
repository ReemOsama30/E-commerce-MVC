using E_commerce.Models;
using E_commerce.Repository;

namespace E_commerce_MVC.Repository
{
    public interface ICartRepository : IRepository<Cart>
    {
        List<Cart> CountCartForUser(string userId);

        // check if the item is already in the cart or not
        Cart? CheckIfTheProductExists(int productId, string userId);

        int CountItems(string userId);

        IEnumerable<Cart> GetProductsInCart(string userId);
        public List<Cart> GetAllbyCustomerId(string userId);

        public void Remove(int id);

        public double GetTotalPrice(string customerId);


    }
}
