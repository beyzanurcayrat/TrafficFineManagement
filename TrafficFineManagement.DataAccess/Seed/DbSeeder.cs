using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrafficFineManagement.DataAccess.Context;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Identity;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Seed;

public static class DbSeeder
{
    public const string DefaultPassword = "Test123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        // Bekleyen migration'ları uygula (veritabanı yoksa oluşturur)
        await context.Database.MigrateAsync();

        foreach (var role in AppRoles.DisplayNames.Keys)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await EnsureUserAsync(userManager, "personel@test.com", "Ayşe Personel", AppRoles.Personnel);
        await EnsureUserAsync(userManager, "yonetici@test.com", "Mehmet Yönetici", AppRoles.Manager);
        await EnsureUserAsync(userManager, "finans@test.com", "Zeynep Finans", AppRoles.Finance);
        await EnsureUserAsync(userManager, "admin@test.com", "Sistem Yöneticisi", AppRoles.Admin);

        if (!await context.Vehicles.AnyAsync())
        {
            context.Vehicles.AddRange(
                new Vehicle { Plate = "34ABC123", VehicleType = VehicleType.Car, Brand = "Renault", Model = "Megane" },
                new Vehicle { Plate = "41TR4100", VehicleType = VehicleType.Tractor, Brand = "Mercedes", Model = "Actros" },
                new Vehicle { Plate = "41DR201", VehicleType = VehicleType.Trailer, Brand = "Tırsan", Model = "Tenteli" },
                new Vehicle { Plate = "06KR0606", VehicleType = VehicleType.Rental, Brand = "Fiat", Model = "Egea" });
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(UserManager<AppUser> userManager, string email, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new AppUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Seed kullanıcısı oluşturulamadı ({email}): {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        else if (string.IsNullOrWhiteSpace(user.FullName))
        {
            user.FullName = fullName;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }
}