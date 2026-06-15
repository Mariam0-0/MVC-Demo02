using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.FluentConfigurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(X => X.CategoryName)
                .HasColumnType("varchar")
                .HasMaxLength(30);

            builder.Property(X => X.CreatedAt)
                .HasDefaultValueSql("GETDATE");

            //seeding
            // cardio, strength, yoga, boxing, crossfit

            // hasdata => must send "id", disable identity id
            builder.HasData(
                new Category { Id = 1, CategoryName = "Cardio" },
                new Category { Id = 2, CategoryName = "Strength" },
                new Category { Id = 3, CategoryName = "Yoga" },
                new Category { Id = 4, CategoryName = "Boxing" },
                new Category { Id = 5, CategoryName = "CrossFit" }
                );
            
        }
    }
}
