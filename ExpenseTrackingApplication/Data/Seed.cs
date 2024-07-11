using Microsoft.AspNetCore.Identity;
using System.Net;
using ExpenseTrackingApplication.Data.Enum;
using ExpenseTrackingApplication.Models;
using static System.Net.WebRequestMethods;

namespace ExpenseTrackingApplication.Data;

public class Seed
{
    public static void SeedData(IApplicationBuilder applicationBuilder)
    {
        // Adding seed data to the database
        // using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        // {
        //     var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        //
        //     context.Database.EnsureCreated();
        //
        //     if (!context.Budgets.Any())
        //     {
        //         context.Budgets.AddRange(new List<Budget>()
        //         {
        //             new Budget()
        //             {
        //                 AppUserId = "1",
        //                 Amount = 1000,
        //                 Limit = 500
        //             },
        //         });
        //         context.SaveChanges();
        //     }
        //
        // }
    }
    
    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        
        //Roles
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    
        //Users
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        string adminUserEmail = "admin@etracking.com";
    
        var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
        if (adminUser == null)
        {
            var newAdminUser = new AppUser()
            {
                UserName = "Admin",
                Email = adminUserEmail,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(newAdminUser, "Coding@1234?");
            await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
        }
    
        string appUserEmail = "user@etracking.com";
    
        var appUser = await userManager.FindByEmailAsync(appUserEmail);
        if (appUser == null)
        {
            var newAppUser = new AppUser()
            {
                UserName = "app-user",
                Email = appUserEmail,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(newAppUser, "Coding@1234?");
            await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
        }
    }
    
}
