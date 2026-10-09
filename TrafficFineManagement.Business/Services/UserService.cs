using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Users;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Identity;

namespace TrafficFineManagement.Business.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITrafficFineRepository _fineRepository;

    public UserService(UserManager<AppUser> userManager, ITrafficFineRepository fineRepository)
    {
        _userManager = userManager;
        _fineRepository = fineRepository;
    }

    public async Task<List<UserListItemDto>> GetAllAsync()
    {
        var users = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();

        var result = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListItemDto(user.Id, user.FullName, user.Email ?? "-", roles.ToList(), IsActive(user)));
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

    // Pasif kullanıcı silinmez, giriş yapması engellenir; onay geçmişindeki adı korunur
    public async Task<ServiceResult> SetActiveAsync(string userId, bool isActive, string currentUserId)
    {
        if (userId == currentUserId)
            return ServiceResult.Fail("Kendi hesabınızın durumunu değiştiremezsiniz.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ServiceResult.Fail("Kullanıcı bulunamadı.");

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, isActive ? null : DateTimeOffset.MaxValue);

        // Pasifleştirilen kullanıcının açık oturumu da sonlansın
        if (!isActive)
            await _userManager.UpdateSecurityStampAsync(user);

        return ServiceResult.Ok();
    }

    // Sadece hiç işlem yapmamış kullanıcılar silinebilir (izlenebilirlik)
    public async Task<ServiceResult> DeleteAsync(string userId, string currentUserId)
    {
        if (userId == currentUserId)
            return ServiceResult.Fail("Kendi hesabınızı silemezsiniz.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ServiceResult.Fail("Kullanıcı bulunamadı.");

        if (await _fineRepository.IsUserReferencedAsync(userId))
            return ServiceResult.Fail($"{user.FullName} kullanıcısının ceza veya onay geçmişinde kaydı olduğu için silinemez. Bunun yerine pasifleştirebilirsiniz.");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? ServiceResult.Ok()
            : ServiceResult.Fail(string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    private static bool IsActive(AppUser user) =>
        user.LockoutEnd is null || user.LockoutEnd <= DateTimeOffset.UtcNow;
}