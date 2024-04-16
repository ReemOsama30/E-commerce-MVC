using E_commerce.Models;
using E_commerce.Repository;
using E_commerce_MVC.myHubs;
using E_commerce_MVC.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<Context>();

            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));

            });


            builder.Services.AddScoped<ICartRepository, CartRepositry>();
            builder.Services.AddScoped<IWishListRepository, WishListRepository>();
            builder.Services.AddScoped<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<IRepository<Shipment>, Repository<Shipment>>();
            builder.Services.AddScoped<IRepository<Payment>, Repository<Payment>>();

            builder.Services.AddScoped<ICommentRepository, CommentRepository>();


            builder.Services.AddSignalR();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.MapHub<ProductHub>("/productHub");
            app.MapHub<whishlistHub>("/whishlistHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=category}/{action=getallcategory}/{id?}");

            app.Run();
        }
    }
}
