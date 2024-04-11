using E_commerce.Models;
using E_commerce_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace E_commerce_MVC.myHubs
{
    public class ProductHub : Hub
    {
        private readonly Context _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductHub(Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task WriteComment(string text, int product_id, int rating)
        {

            var currentUser = await _userManager.GetUserAsync(Context.User);
            string username = currentUser.UserName; // Get the username


            var newComment = new Comments
            {
                Text = text,
                ProductId = product_id,
                Rating = rating,
                applicationUser = currentUser
            };


            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();


            await Clients.All.SendAsync("ReciveNewComment", text, product_id, rating, username);
        }
    }
}
