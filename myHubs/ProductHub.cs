using E_commerce.Models;
using E_commerce_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace E_commerce_MVC.myHubs
{
    public class ProductHub : Hub
    {
        private readonly Context context;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductHub(Context context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task WriteComment(string text, int product_id, int rating)
        {

            var currentUser = await userManager.GetUserAsync(Context.User);
            string username = currentUser.UserName;


            var newComment = new Comments
            {
                Text = text,
                ProductId = product_id,
                Rating = rating,
                applicationUser = currentUser
            };


            context.Comments.Add(newComment);
            await context.SaveChangesAsync();


            await Clients.All.SendAsync("ReciveNewComment", text, product_id, rating, username);
        }
    }
}
