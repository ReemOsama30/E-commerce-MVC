using E_commerce.Models;
using E_commerce_MVC.Repository;
using Microsoft.AspNetCore.SignalR;

namespace E_commerce_MVC.myHubs
{
    public class whishlistHub:Hub
    {
        private readonly IWishListRepository wishListRepository;

        public  whishlistHub(IWishListRepository wishListRepository)
        {
            this.wishListRepository = wishListRepository;
        }


        public async Task RemoveProductFromWishList(string userId, int productId)
        {
         WishList wishList=   wishListRepository.getwishlistByProductId(productId);
            wishListRepository.HardDelete(wishList);
            wishListRepository.save();
            await Clients.All.SendAsync("ProductRemoved", userId, productId);
        }
    }
}
