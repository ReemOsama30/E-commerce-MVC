using E_commerce.Models;
using E_commerce_MVC.Repository;
using E_commerce_MVC.viewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



namespace E_commerce_MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ICommentRepository commentRepository;
        private readonly IWishListRepository wishListRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public ProductController(IProductRepository ProductRepository, ICategoryRepository CategoryRepository, IWebHostEnvironment webHostEnvironment
            , UserManager<ApplicationUser> userManager, ICommentRepository commentRepository, IWishListRepository wishListRepository)
        {
            this.ProductRepository = ProductRepository;
            this.CategoryRepository = CategoryRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.commentRepository = commentRepository;
            this.wishListRepository = wishListRepository;
            this.userManager = userManager;
        }


        //////


        //dina controllers

        public IActionResult GetAllProducts()
        {
            List<Product> products = (List<Product>)ProductRepository.GetAll();
            return View("GetAllProducts", products);
        }

        public IActionResult GetProductsByCategoryId(int CategoryId)
        {
            List<string> productNames = ProductRepository.GetProductNamesByCatId(CategoryId);
            List<string> productImages = ProductRepository.GetProductImagesByCatId(CategoryId);
            List<double> productPrices = ProductRepository.GetProductPricesByCatId(CategoryId);
            List<int> productIds = ProductRepository.GetProductIDsByCatId(CategoryId);

            List<Product> products = ProductRepository.GetProductsByCatgoryId(CategoryId);

            string CategoryName = CategoryRepository.GetName(CategoryId);

            ProductPartViewModel productPartViewModel = new ProductPartViewModel()
            {
                CategoryId = CategoryId,
                CategoryName = CategoryName,
                Products = products,

                Price = productPrices,
                ProductNames = productNames,
                ProductsId = productIds
            };

            return View("GetProductsByCategoryId", productPartViewModel);
        }

        //Get latest product in each category
        //public IActionResult GetLatestProduct()
        //{
        //    List<Product> latestProductsInCategories = (List<Product>)ProductRepository.GetLatestProduct();
        //    ViewBag.Products = latestProductsInCategories;
        //    return View("_GetLatestProduct");

        //}


        //////







        public IActionResult show()
        {
            List<Product> newproduct = ProductRepository.GetAll().ToList();

            return View(newproduct);
        }


        public IActionResult addNewProduct()
        {
            newProductVM newProductVM = new newProductVM();
            newProductVM.Category = CategoryRepository.GetAll().ToList();
            return View(newProductVM);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult addNewProduct(newProductVM newProduct)
        {

            if (ModelState.IsValid)
            {
                string UploadPath = Path.Combine(webHostEnvironment.WebRootPath, "img/gallery");
                string imageName = Guid.NewGuid().ToString() + "-" + newProduct.Image_Url.FileName;
                string filePath = Path.Combine(UploadPath, imageName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newProduct.Image_Url.CopyTo(fileStream);
                }

                Product product = new Product();
                product.Name = newProduct.Name;
                product.Image_Url = imageName;
                product.Price = newProduct.Price;
                product.Description = newProduct.Description;
                product.Category_Id = newProduct.Category_Id;
                product.Quantity = newProduct.Quantity;

                ProductRepository.insert(product);
                ProductRepository.save();


                return RedirectToAction("GetAllProducts");
            }

            newProduct.Category = CategoryRepository.GetAll().ToList();
            return View(newProduct);
        }






        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddToCart(int ProductId)
        {
            Product product = ProductRepository.Get(p => p.Id == ProductId);
            return View("ShoppingCart", product);
        }


        //public IActionResult AddToWishList(int ProductId)
        //{
        //    Product product = ProductRepository.Get(p => p.Id == ProductId);
        //    return View("wishingList", product);
        //}


        public async Task<IActionResult> AddToWishList(int ProductId)
        {
            Product product = ProductRepository.Get(p => p.Id == ProductId);
            var currentUser = await userManager.GetUserAsync(User);
            string CurrentUserId = currentUser.Id;
            WishList wishList = new WishList();
            wishList.Customer_Id = CurrentUserId;
            wishList.Product_Id = product.Id;
            bool found = wishListRepository.ExistOrNot(ProductId);
            if (found == true)
            {
                return RedirectToAction("Index", "WishList", new { id = CurrentUserId });
            }
            wishListRepository.insert(wishList);
            wishListRepository.save();
            return RedirectToAction("Index", "WishList", new { id = CurrentUserId });
        }

        //Get latest product in each category
        //public IActionResult GetLatestProduct()
        //{
        //    List<Product> latestProductsInCategories = (List<Product>)ProductRepository.GetLatestProduct();
        //    ViewBag.Products = latestProductsInCategories;
        //    return View("_GetLatestProduct");

        //}


        public async Task<IActionResult> Details(int id)
        {
            Product product = ProductRepository.Get(p => p.Id == id);

            var commentsWithRatings = commentRepository.GetAllIncludeUser(id);


            var currentUser = await userManager.GetUserAsync(User);
            string username = User.Identity.Name;
            string userid = currentUser.Id;

            ViewData["Username"] = username;
            ViewData["UserId"] = userid;
            ViewData["Product"] = product;
            ViewData["CommentsWithRatings"] = commentsWithRatings;

            return View("Details");
        }

    }
}
