using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Contexts
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityData(RoleManager<IdentityRole> roleManager,
                                            UserManager<ApplicationUser> userManager,
                                            
                                            CancellationToken ct = default)
        {
            try
            {
                // check if users or roles exist
                bool hasUsers = await userManager.Users.AnyAsync(ct);
                bool hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasUsers && hasRoles) return;

                // roles
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin")
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                            Console.WriteLine($"Failed to add role {role.Name}");
                    }

                }

                //users
                if (!hasUsers)
                {
                    var mainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mariam",
                        LastName = "Abdelrahman",
                        Email = "mariam@gmail.com",
                        UserName = "Mariam123",
                        PhoneNumber = "0123456789",
                    };
                    await userManager.CreateAsync(mainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(mainAdmin, "SuperAdmin");
                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Sara",
                        LastName = "Ahmed",
                        Email = "sara@gmail.com",
                        UserName = "Sara123",
                        PhoneNumber = "0123450189",
                    };
                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    Console.WriteLine("Identity seeded successfully");


                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
