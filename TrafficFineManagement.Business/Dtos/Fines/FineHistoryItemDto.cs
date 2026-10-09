using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Dtos.Fines;

public record FineHistoryItemDto(
    DateTime Date,
    string UserName,
    ApprovalActionType ActionType,
    FineStatus? FromStatus,
    FineStatus ToStatus,
    string? Note);