using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;
using fashion_shop.Core.Entities;
using fashion_shop.Core.Extensions;
using Microsoft.AspNetCore.Identity;

namespace fashion_shop.API.Seeders;

public static class UserRoleSeeder
{
    public static async Task Seed(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        foreach (RoleEnum roleEnum in Enum.GetValues(typeof(RoleEnum)))
        {
            var roleName = roleEnum.GetDisplayName();
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var users = new List<(string username, string email, string role)>
        {
            ("admin", "admin@gmail.com", "Admin"),
            ("manager", "manager@gmail.com", "Manager"),
            ("user", "user@gmail.com", "User")
        };

        foreach (var (username, email, role) in users)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var newUser = new User
                {
                    UserName = username,
                    Email = email
                };

                var result = await userManager.CreateAsync(newUser, "180402");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }
            }
        }
    }
}