using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.FluentConfigurations;
using MVC01_Demo.FluentConfigurations;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace MVC01_Demo.Contexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser> 
    {

        public GymDbContext(DbContextOptions<GymDbContext> options): base(options) 
        {
            

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // config identity
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }

        //public DbSet<ApplicationUser> Users { get; set; }
        //public DbSet<IdentityRole> Roles { get; set; }
        //public DbSet<IdentityUserRole<string>> UserRoles { get; set; }

    }
}
