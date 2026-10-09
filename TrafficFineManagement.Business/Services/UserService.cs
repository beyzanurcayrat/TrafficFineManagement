using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Users;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Identity;

namespace TrafficFineManagement.Business.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;

    public UserService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserListItemDto>> GetAllAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();

        var result = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListItemDto(user.Id, user.FullName, user.Email ?? "-", roles.ToList()));
        }

        return result;
    }

    public async Task<ServiceResult> CreateAsync(UserCreateDto dto)
    {
        // Rol dışarıdan gelen metin: sadece tanımlı rollerden biri kabul edilir
        if (!AppRoles.DisplayNames.ContainsKey(dto.Role))
            return ServiceResult.Fail("Geçersiz rol.");

        var email = dto.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
            return ServiceResult.Fail("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten var.");

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = dto.FullName.Trim(),
            EmailConfirmed = true
        };

        // UserManager şifreyi hash'leyerek saklar ve şifre kurallarını kontrol eder
        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return ServiceResult.Fail(string.Join(" ", createResult.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);
        return ServiceResult.Ok();
    }
}