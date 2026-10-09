using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}