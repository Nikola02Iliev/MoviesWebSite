using Microsoft.EntityFrameworkCore;
using MoviesWebSite.Context;
using Microsoft.AspNetCore.Identity;
using MoviesWebSite.Models;
using MoviesWebSite.Services.Implementations;
using MoviesWebSite.Services.Interfaces;

namespace MoviesWebSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AppDBContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IMoviesService, MoviesService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<IRatingsService, RatingsService>();

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

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movies}/{action=Index}/{id?}");

            app.MapRazorPages();


            app.Run();
        }
    }
}
