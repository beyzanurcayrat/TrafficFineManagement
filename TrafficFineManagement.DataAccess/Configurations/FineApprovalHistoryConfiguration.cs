using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Configurations;

public class FineApprovalHistoryConfiguration : IEntityTypeConfiguration<FineApprovalHistory>
{
    public void Configure(EntityTypeBuilder<FineApprovalHistory> builder)
    {
        builder.ToTable("FineApprovalHistories");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.ActionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(h => h.Note).HasMaxLength(500);

        builder.HasOne(h => h.TrafficFine)
            .WithMany(f => f.ApprovalHistories)
            .HasForeignKey(h => h.TrafficFineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(h => h.UserId).IsRequired();
        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => new { h.TrafficFineId, h.CreatedAt });
    }
}