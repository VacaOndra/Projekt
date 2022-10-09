using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pojisteni.Data;
using Pojisteni.Models;

namespace Pojisteni
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("Home") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddIdentity<AccountIdentity, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            //CreateDefaultAdminAndRoles(app);

            app.Run();
        }

        /// <summary>
        /// Create roles and default admin account. Default password = 'Admin123!'
        /// </summary>
        /// <param name="app"></param>
        public static async void CreateDefaultAdminAndRoles(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<AccountIdentity> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AccountIdentity>>();

                roleManager.CreateAsync(new IdentityRole("admin")).Wait();
                roleManager.CreateAsync(new IdentityRole("insured")).Wait();

                AccountIdentity user = new AccountIdentity { UserName = "Admin", Email = "admin@admin.com" };
                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }
        }
    }
}