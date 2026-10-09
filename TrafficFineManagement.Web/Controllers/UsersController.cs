using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Users;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Entities.Constants;

namespace TrafficFineManagement.Web.Controllers;

// Tüm sayfalar sadece Admin'e açık
[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["CurrentUserId"] = CurrentUserId;
        var users = await _userService.GetAllAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create() => View(new UserCreateDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _userService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(dto);
        }

        TempData["Success"] = $"{dto.FullName} kullanıcısı oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Deactivate(string id) =>
        RunAsync(() => _userService.SetActiveAsync(id, isActive: false, CurrentUserId), "Kullanıcı pasifleştirildi.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Activate(string id) =>
        RunAsync(() => _userService.SetActiveAsync(id, isActive: true, CurrentUserId), "Kullanıcı aktifleştirildi.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Delete(string id) =>
        RunAsync(() => _userService.DeleteAsync(id, CurrentUserId), "Kullanıcı silindi.");

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private async Task<IActionResult> RunAsync(Func<Task<ServiceResult>> action, string successMessage)
    {
        var result = await action();

        if (result.Success)
            TempData["Success"] = successMessage;
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}