namespace TrafficFineManagement.Business.Dtos.Users;

public record UserListItemDto(string Id, string FullName, string Email, IReadOnlyList<string> Roles, bool IsActive);