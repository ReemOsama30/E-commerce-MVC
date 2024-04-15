using E_commerce.Models;
using E_commerce.Repository;
using E_commerce.viewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace E_commerce.Controllers
{
    public class AccountController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        IRepository<ApplicationUser> _repository;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IRepository<ApplicationUser> appRpo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repository = appRpo;

        }
        public  IActionResult DeleteUser(string id)
        {
            ApplicationUser user =  _repository.Get(i=>i.Id==id);
            if (user== null)
            {
                return NotFound();
            }

          
           _repository.delete(user);
            user.UserName = "";
            user.NormalizedUserName = "";
            _repository.save();
            return RedirectToAction("RegisteredCustomers");


        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult login()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(loginVm uservm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await _userManager.FindByNameAsync(uservm.userName);

                if (applicationUser != null&&!applicationUser.IsDeleted)
                {
                    bool found = await _userManager.CheckPasswordAsync(applicationUser, uservm.password);

                    if (found)
                    {
                        await _signInManager.SignInAsync(applicationUser, uservm.RememberMe);
                        return RedirectToAction("GetAllCategory", "Category");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password");
            }

            return View("login");
        }

        [HttpGet]
        public IActionResult register()
        {
            return View("register");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> register(RegisterVm model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    UserName = model.userName,
                    PasswordHash = model.password,
                    Email = model.email,
                    Address = model.address,
                };
                IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, model.password);

                if (identityResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, "User");
                    await _signInManager.SignInAsync(applicationUser, false);


                    return RedirectToAction("GetAllCategory","Category");
                }
                else
                {
                    foreach (var item in identityResult.Errors)
                    {

                        ModelState.AddModelError("", item.Description);
                    }

                }
            }
            return View("register", model);
        }

        public async Task<IActionResult> signOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login");
        }



        // for trials only
        [Authorize(Roles ="Admin")]
        public IActionResult RegisteredCustomers()
        {
            List<ApplicationUser> customers = _repository.GetAll().Where(c=>c.IsDeleted==false).ToList();

            return View(customers);
        }





    }
}
