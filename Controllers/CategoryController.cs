using E_commerce.Models;
using E_commerce_MVC.Repository;
using E_commerce_MVC.Utility;
using E_commerce_MVC.viewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_commerce_MVC.Controllers
{

    //[Route("Home")]
    public class CategoryController : Controller
    {

        private readonly IProductRepository ProductRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Context _context;

        public CategoryController(Context context,IProductRepository ProductRepository, ICategoryRepository CategoryRepository,IWebHostEnvironment webHostEnvironment, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.ProductRepository = ProductRepository;
            this.CategoryRepository = CategoryRepository;
            this.webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult GetAllCategory()
        {
            // Added by MAi
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                // if the user signed in
                var userId = _userManager.GetUserId(User);
                var count = _context.Carts.Where(u => u.Customer_Id.Contains(userId) && !u.IsDeleted).Count();

                HttpContext.Session.SetInt32(CartCount.sessionCount, count);
            }
            List<Category> categories = (List<Category>)CategoryRepository.GetAllCategories();
            List<Product> latestProductsInCategories = (List<Product>)ProductRepository.GetLatestProduct();
            ListOfProductAndListOfCategory ListOfProductAndListOfCategory = new ListOfProductAndListOfCategory();
            ListOfProductAndListOfCategory.categories = categories;
            ListOfProductAndListOfCategory.products = latestProductsInCategories;
            return View("GetAllCategory", ListOfProductAndListOfCategory);
        }


        public IActionResult addNewCategory()
        {

            return View();

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult addNewCategory(NewcategoryVM newcategory)
        {
            if (ModelState.IsValid)
            {
                string UploadPath = Path.Combine(webHostEnvironment.WebRootPath, "img/gallery");
                string imageName = Guid.NewGuid().ToString() + "-" + newcategory.imageURL.FileName;
                string filePath = Path.Combine(UploadPath, imageName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newcategory.imageURL.CopyTo(fileStream);
                }

                Category category = new Category();
                category.Name = newcategory.Name;
                category.imageURL = imageName;

                CategoryRepository.insert(category);
                CategoryRepository.save();

        
                return RedirectToAction("GetAllCategory");
            }

       
            return View(newcategory);
        }
    }
}
