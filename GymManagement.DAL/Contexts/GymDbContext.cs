using Microsoft.EntityFrameworkCore;
using MVC01_Demo.FluentConfigurations;
using MVC01_Demo.Models;

namespace MVC01_Demo.Contexts
{
    public class GymDbContext : DbContext 
    {

        public GymDbContext(DbContextOptions<GymDbContext> options): base(options) 
        {
            

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfiguration());
        }

        public DbSet<Plan> Plans { get; set; }
    }
}
