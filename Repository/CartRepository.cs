using E_commerce.Models;
using E_commerce.Repository;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_MVC.Repository
{
    public class CartRepositry : Repository<Cart>, ICartRepository
    {
        public CartRepositry(Context context) : base(context)
        {

        }

        public Cart? CheckIfTheProductExists(int productId, string userId)
        {
            return CountCartForUser(userId).FirstOrDefault(p => p.Product_Id == productId);
        }

        public List<Cart> CountCartForUser(string userId)
        {
            return context
                          .Carts.Where(u => u.Customer_Id.Contains(userId))
                          .ToList();
        }

        public int CountItems(string userId)
        {
            return context.Carts.Where(u => u.Customer_Id.Contains(userId) && !u.IsDeleted).Count();
        }

        public IEnumerable<Cart> GetProductsInCart(string userId)
        {
            return context
                .Carts
                .Include(u => u.product)
                .Where(u => u.Customer_Id.Contains(userId) && !u.IsDeleted)
                .ToList();
        }

        public List<Cart> GetAllbyCustomerId(string id)
        {
            List<Cart> CartList = context.Carts.Include(c => c.product)
                .Where(item => item.Customer_Id == id && item.IsDeleted == false).ToList();
            return CartList;
        }

        public void Remove(int id)
        {
            var item = context.Carts.FirstOrDefault(p => p.Product_Id == id);
            context.Carts.Remove(item);
        }

        public double GetTotalPrice(string customerId)
        {
            double totalPrice = 0;
            foreach (Cart cartItem in GetAllbyCustomerId(customerId))
            {
                totalPrice += (cartItem.product.Price * cartItem.Quantity);
            }

            return totalPrice;
        }
    }
}
