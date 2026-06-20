using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVC01_Demo.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagement.DAL.Contexts
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFolderPath, ILogger logger)
        {
            // get data from json file
            // check if table empty

            try
            {

                if(!await dbContext.Plans.AnyAsync())
                {
                    // seed

                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                    if(plans.Any() )
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Plans seeded with count = {plans.Count}");
                    }

                    // save changes
                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation("Plan already seeded"); 
                }
            }
            catch (Exception ex) 
            {
                logger.LogInformation(ex, "Seeding Failed");
                throw;
            }
        }
        public static List<T> LoadDataFromJsonFile<T>( string folderPath, string filename)
        {
            // filepath
            var filePath = Path.Combine(folderPath, filename);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File data not found");

            // read data
            var data = File.ReadAllText(filePath);

            // options => ignore case sensitive
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            // json => list
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
