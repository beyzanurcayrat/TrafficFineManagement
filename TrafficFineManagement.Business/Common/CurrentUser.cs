namespace TrafficFineManagement.Business.Common;

public record CurrentUser(string Id, IReadOnlyCollection<string> Roles)
{
    public bool IsInRole(string role) => Roles.Contains(role);
}