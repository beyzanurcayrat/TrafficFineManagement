using Microsoft.AspNetCore.Identity;

namespace TrafficFineManagement.Entities.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}