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
    internal class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(X => X.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(X => X.Email)
                .HasColumnType("varchar")
                .HasMaxLength (100);

            builder.HasIndex(X => X.Email).IsUnique();
            builder.HasIndex(X => X.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email Like '_%@_%._%'");
                tb.HasCheckConstraint("PhoneCheck", "Phone like '010%' or Phone like '011%' or Phone like '012%' or Phone like '015%'");
            });

            // address owned
            builder.OwnsOne(X => X.Address, address =>
            {
                address.Property(X => X.Street)
                .HasColumnName("Street")
                .HasColumnType("varchar")
                .HasMaxLength(30);

                address.Property(X => X.City)
                .HasColumnName("City")
                .HasColumnType("varchar")
                .HasMaxLength(30);
            });


        }
    }
}
