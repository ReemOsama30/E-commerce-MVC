using E_commerce.Models;
using E_commerce_MVC.Repository;
using E_commerce_MVC.Utility;
using E_commerce_MVC.viewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_MVC.Controllers
{
    // Added by MAi
    public class CartController : Controller
    {

        private readonly ICartRepository _cartRepositry;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProductRepository _productRepository;

        public CartController(ICartRepository cartRepositry, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IProductRepository productRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _productRepository = productRepository;
            _cartRepositry = cartRepositry;
        }

        public IActionResult Index()
        {
            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (checkIfUserSignedInOrNot)
            {
                var userId = _userManager.GetUserId(User);

                IEnumerable<Cart> CartList = _cartRepositry.GetProductsInCart(userId);

                CartIndexViewModel CartViewModel = new CartIndexViewModel();
                foreach (var item in CartList)
                {
                    Cart cart = _cartRepositry.Get(p => p.Product_Id == item.Product_Id);
                    CartViewModel.ProductName.Add(cart.product.Name);
                    CartViewModel.ProductId.Add(cart.product.Id);
                    CartViewModel.ImgUrl.Add(cart.product.Image_Url);
                    CartViewModel.Quantity.Add(cart.Quantity);
                    CartViewModel.Price.Add(cart.product.Price);
                    CartViewModel.Total.Add(cart.product.Price * cart.Quantity);
                }
                var currentUser = _userManager.GetUserAsync(User);
                string username = User.Identity.Name;
                ViewData["Username"] = username;

                return View("CartIndex", CartViewModel);
            }

            return RedirectToAction("login", "Account");

        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var productAddToCart = _productRepository.Get(p => p.Id == productId);
            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (checkIfUserSignedInOrNot)
            {
                // User Signed in
                // Get User Id
                var user = _userManager.GetUserId(User);
                if (user != null)
                {
                    // check if the signed user has any cart or not
                    var getTheCartIfAnyExistForTheUser = _cartRepositry.CountCartForUser(user);
                    if (getTheCartIfAnyExistForTheUser.Count() > 0)
                    {
                        // check if the item is already in the cart or not
                        var getTheQuantity = _cartRepositry.CheckIfTheProductExists(productId, user);
                        if (getTheQuantity != null)
                        {
                            // if the item is already in the cart just increase the quantity by 1 and update the cart
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _cartRepositry.update(getTheQuantity);
                        }
                        else
                        {
                            // user has a cart but adding a new item to the existing cart
                            Cart newItemToCart = new Cart
                            {
                                Product_Id = productId,
                                Customer_Id = user,
                                Quantity = 1
                            };
                            _cartRepositry.insert(newItemToCart);
                        }
                    }
                    else
                    {
                        // user has no cart. Adding a new cart for the user
                        Cart newItemToCart = new Cart
                        {
                            Product_Id = productId,
                            Customer_Id = user,
                            Quantity = 1
                        };
                        _cartRepositry.insert(newItemToCart);
                    }
                    _cartRepositry.save();

                    // Count Items in Cart
                    var count = _cartRepositry.CountItems(user);
                    HttpContext.Session.SetInt32(CartCount.sessionCount, count);
                }
            }
            return RedirectToAction("GetAllproducts", "product");
        }

        public ActionResult Remove(int id)
        {
            Cart cartItem = _cartRepositry.Get(p => p.Product_Id == id); // Get the item to delete
            if (cartItem != null)
            {

                // If quantity is 1 or less, remove the item from the cart
                _cartRepositry.Remove(id);
                _cartRepositry.save(); // Save changes

                // Count Items in Cart
                var user = _userManager.GetUserId(User);
                var count = _cartRepositry.CountItems(user);
                HttpContext.Session.SetInt32(CartCount.sessionCount, count);
            }

            return RedirectToAction("Index", new { id = cartItem?.Customer_Id });
        }

        public ActionResult DecreaseQuantity(int id)
        {
            Cart cartItem = _cartRepositry.Get(p => p.Product_Id == id); // Get the item to delete

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    // Decrease quantity by 1
                    cartItem.Quantity--;
                    _cartRepositry.update(cartItem); // Update the quantity in the repository
                    _cartRepositry.save(); // Save changes
                }
                else
                {
                    // If quantity is 1 or less, remove the item from the cart
                    _cartRepositry.Remove(id);
                    _cartRepositry.save(); // Save changes
                }
            }

            var user = _userManager.GetUserId(User);
            var count = _cartRepositry.CountItems(user);
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);

            return RedirectToAction("Index", new { id = cartItem?.Customer_Id });
        }


        public ActionResult IncreaseQuantity(int id)
        {
            Cart cartItem = _cartRepositry.Get(p => p.Product_Id == id); // Get the item to delete

            if (cartItem != null)
            {
                // Increase quantity by 1
                cartItem.Quantity++;
                _cartRepositry.update(cartItem); // Update the quantity in the repository
                _cartRepositry.save(); // Save changes
            }

            var user = _userManager.GetUserId(User);
            var count = _cartRepositry.CountItems(user);
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);

            return RedirectToAction("Index", new { id = cartItem?.Customer_Id });
        }
        public IActionResult AddToCartFromWishList(int productId)
        {
            var productAddToCart = _productRepository.Get(p => p.Id == productId);
            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (checkIfUserSignedInOrNot)
            {
                // User Signed in
                // Get User Id
                var user = _userManager.GetUserId(User);
                if (user != null)
                {
                    // check if the signed user has any cart or not
                    var getTheCartIfAnyExistForTheUser = _cartRepositry.CountCartForUser(user);
                    if (getTheCartIfAnyExistForTheUser.Count() > 0)
                    {
                        // check if the item is already in the cart or not
                        var getTheQuantity = _cartRepositry.CheckIfTheProductExists(productId, user);
                        if (getTheQuantity != null)
                        {
                            // if the item is already in the cart just increase the quantity by 1 and update the cart
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _cartRepositry.update(getTheQuantity);
                        }
                        else
                        {
                            Cart newItemToCart = new Cart
                            {
                                Product_Id = productId,
                                Customer_Id = user,
                                Quantity = 1
                            };
                            _cartRepositry.insert(newItemToCart);
                        }
                    }
                    else
                    {
                        Cart newItemToCart = new Cart
                        {
                            Product_Id = productId,
                            Customer_Id = user,
                            Quantity = 1
                        };
                        _cartRepositry.insert(newItemToCart);
                    }
                    _cartRepositry.save();
                    var count = _cartRepositry.CountItems(user);
                    HttpContext.Session.SetInt32(CartCount.sessionCount, count);
                }
            }
            return Json(new { success = true });
        }

    }
}

