using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Dtos.Fines;

public record FineListItemDto(
    int Id,
    string FineNumber,
    string Plate,
    string ViolationType,
    DateTime ViolationDate,
    decimal Amount,
    FineStatus Status);