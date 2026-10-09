using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Configurations;

public class TrafficFineConfiguration : IEntityTypeConfiguration<TrafficFine>
{
    public void Configure(EntityTypeBuilder<TrafficFine> builder)
    {
        builder.ToTable("TrafficFines");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FineNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(f => f.FineNumber).IsUnique();

        builder.Property(f => f.ViolationType).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Location).HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.Amount).HasPrecision(18, 2);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(30);
        builder.HasIndex(f => f.Status);

        builder.Property(f => f.RowVersion).IsRowVersion();

        builder.HasOne(f => f.Vehicle)
            .WithMany(v => v.TrafficFines)
            .HasForeignKey(f => f.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(f => f.CreatedByUserId).IsRequired();
        builder.HasOne(f => f.CreatedByUser)
            .WithMany()
            .HasForeignKey(f => f.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}