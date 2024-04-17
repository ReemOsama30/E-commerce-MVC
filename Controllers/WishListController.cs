using E_commerce.Models;
using E_commerce_MVC.Repository;
using E_commerce_MVC.viewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_MVC.Controllers
{
    public class WishListController : Controller
    {
        private readonly IWishListRepository wishListRepository;
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public WishListController(IWishListRepository wishListRepository, IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            this.wishListRepository = wishListRepository;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }
        //[Authorize]
        public async Task<IActionResult> Index(string id, int page = 1, int pageSize = 5)
        {
            List<WishList> wishLists = wishListRepository.GetAllbyCustomerId(id);
            int totalItems = wishLists.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            wishLists = wishLists.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            WishListViewModel wishListViewModel = new WishListViewModel();
            foreach (WishList item in wishLists)
            {
                Product product = productRepository.Get(p => p.Id == item.Product_Id);
                wishListViewModel.ProductName.Add(product.Name);
                wishListViewModel.ProductId.Add(product.Id);
                wishListViewModel.ImgUrl.Add(product.Image_Url);
                wishListViewModel.Price.Add(product.Price);
                if (product.Quantity != 0)
                {
                    wishListViewModel.Stock.Add("Available");
                }
                else
                {
                    wishListViewModel.Stock.Add("Not Available");
                }
            }

            var currentUser = await userManager.GetUserAsync(User);
            string username = User.Identity.Name;
            ViewData["Username"] = username;

            wishListViewModel.CurrentUserId = id;
            wishListViewModel.Page = page;
            wishListViewModel.TotalPages = totalPages;

            return View("Index", wishListViewModel);
        }

        public async Task<ActionResult> Remove(int id)
        {
            WishList wishList = wishListRepository.Get(p => p.Product_Id == id);
            if (wishList != null)
            {
                wishListRepository.HardDelete(wishList);
                wishListRepository.save();
            }
            return RedirectToAction("Index", new { id = wishList.Customer_Id });
        }



        //public ActionResult Remove(int id)
        //{
        //    WishList wishList = wishListRepository.Get(p => p.Product_Id == id);
        //    wishListRepository.delete(wishList);
        //    wishListRepository.save();
        //    return RedirectToAction("Index", new { id = wishList.Customer_Id });
        //}


    }
}
