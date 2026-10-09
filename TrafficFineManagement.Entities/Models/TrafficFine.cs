using TrafficFineManagement.Entities.Common;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Identity;

namespace TrafficFineManagement.Entities.Models;

public class TrafficFine : BaseEntity
{
    public string FineNumber { get; set; } = string.Empty;   // Ceza tutanak no
    public DateTime ViolationDate { get; set; }
    public string ViolationType { get; set; } = string.Empty; // Örn. "Hız sınırı aşımı"
    public string? Location { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public FineStatus Status { get; set; } = FineStatus.New;

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string CreatedByUserId { get; set; } = string.Empty;
    public AppUser CreatedByUser { get; set; } = null!;

    public byte[] RowVersion { get; set; } = [];

    public ICollection<FineApprovalHistory> ApprovalHistories { get; set; } = new List<FineApprovalHistory>();
}