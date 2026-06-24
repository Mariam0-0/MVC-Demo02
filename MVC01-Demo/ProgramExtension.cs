using GymManagement.DAL.Contexts;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC01_Demo.Contexts;

namespace MVC01_Demo
{
    public static class ProgramExtension
    {
        public static async Task MigrateAndSeedDataAsync(this WebApplication app)
        {

            // scope unmanaged
            using var scope = app.Services.CreateScope();

            // gymdbcontext
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            //logger
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            //check if any pending migration or not
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var pendingMigration = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigration.Any())
            {
                await dbContext.Database.MigrateAsync();
            }

            // folder path
            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);

            await IdentityDataSeeding.SeedIdentityData(roleManager, userManager, logger);
        }
    }
}
