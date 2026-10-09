using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Plate).IsRequired().HasMaxLength(15);
        builder.HasIndex(v => v.Plate).IsUnique();

        builder.Property(v => v.Brand).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Model).IsRequired().HasMaxLength(50);

        builder.Property(v => v.VehicleType)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}