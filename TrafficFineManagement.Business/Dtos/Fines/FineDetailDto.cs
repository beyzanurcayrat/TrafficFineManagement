using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Dtos.Fines;

public class FineDetailDto
{
    public int Id { get; init; }
    public string FineNumber { get; init; } = string.Empty;
    public string Plate { get; init; } = string.Empty;
    public string VehicleInfo { get; init; } = string.Empty;
    public string ViolationType { get; init; } = string.Empty;
    public DateTime ViolationDate { get; init; }
    public string? Location { get; init; }
    public decimal Amount { get; init; }
    public string? Description { get; init; }
    public FineStatus Status { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public List<FineHistoryItemDto> History { get; init; } = [];

    // Ekranda hangi butonların görüneceği (asıl kontrol yine serviste)
    public bool CanEdit { get; init; }
    public bool CanSubmit { get; init; }
    public bool CanDecide { get; init; }
}