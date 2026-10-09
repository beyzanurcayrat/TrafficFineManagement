using TrafficFineManagement.Entities.Common;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Identity;

namespace TrafficFineManagement.Entities.Models;

// CreatedAt = işlem tarihi
public class FineApprovalHistory : BaseEntity
{
    public int TrafficFineId { get; set; }
    public TrafficFine TrafficFine { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public ApprovalActionType ActionType { get; set; }
    public FineStatus? FromStatus { get; set; }   // "Oluşturuldu"da önceki durum yok
    public FineStatus ToStatus { get; set; }
    public string? Note { get; set; }             // Açıklama / ret nedeni
}