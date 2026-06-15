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
    internal class TrainerConfiguration :GymUserConfiguration<Trainer>, IEntityTypeConfiguration<Trainer>
    {
        public new void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(X => X.CreatedAt)
                .HasColumnName("HireDate")
                .HasDefaultValueSql("GETDATE()");

            //IMPORTANT
            base.Configure(builder);
        }
    }
}
