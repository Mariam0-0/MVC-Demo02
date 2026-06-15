using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC01_Demo.Models;

namespace MVC01_Demo.FluentConfigurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(P => P.Name)
                .HasColumnType("varchar")
                .HasMaxLength(30);

            builder.Property(P => P.Description)
                .HasMaxLength (200);

            builder.Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Property(P => P.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("PlanDurationCheck", "DurationDays Between 1 and 365");
            }); 

        }
    }
}
