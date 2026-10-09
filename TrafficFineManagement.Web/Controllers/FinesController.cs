using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Fines;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Web.Controllers;

[Authorize]
public class FinesController : Controller
{
    private readonly ITrafficFineService _fineService;
    private readonly IVehicleService _vehicleService;

    public FinesController(ITrafficFineService fineService, IVehicleService vehicleService)
    {
        _fineService = fineService;
        _vehicleService = vehicleService;
    }

    public async Task<IActionResult> Index(FineStatus? status, string? plate)
    {
        ViewData["Status"] = status;
        ViewData["Plate"] = plate;

        var fines = await _fineService.GetListAsync(status, plate);
        return View(fines);
    }

    // Giriş yapan kullanıcının onay aşamasındaki cezalar
    public IActionResult Pending()
    {
        if (User.IsInRole(AppRoles.Manager))
            return RedirectToAction(nameof(Index), new { status = FineStatus.PendingManagerApproval });

        if (User.IsInRole(AppRoles.Finance))
            return RedirectToAction(nameof(Index), new { status = FineStatus.PendingFinanceApproval });

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var fine = await _fineService.GetDetailAsync(id, GetCurrentUser());
        return fine is null ? NotFound() : View(fine);
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Personnel)]
    public async Task<IActionResult> Create()
    {
        await LoadVehicleOptionsAsync();
        return View(new FineFormDto { ViolationDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Personnel)]
    public async Task<IActionResult> Create(FineFormDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadVehicleOptionsAsync();
            return View(dto);
        }

        var result = await _fineService.CreateAsync(dto, GetCurrentUser());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadVehicleOptionsAsync();
            return View(dto);
        }

        TempData["Success"] = "Ceza kaydı oluşturuldu. Onaya göndermeyi unutmayın.";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Personnel)]
    public async Task<IActionResult> Edit(int id)
    {
        var detail = await _fineService.GetDetailAsync(id, GetCurrentUser());
        if (detail is null)
            return NotFound();

        if (!detail.CanEdit)
        {
            TempData["Error"] = "Bu ceza mevcut durumunda düzenlenemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var dto = await _fineService.GetForEditAsync(id);
        ViewData["FineId"] = id;
        await LoadVehicleOptionsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Personnel)]
    public async Task<IActionResult> Edit(int id, FineFormDto dto)
    {
        ViewData["FineId"] = id;

        if (!ModelState.IsValid)
        {
            await LoadVehicleOptionsAsync();
            return View(dto);
        }

        var result = await _fineService.UpdateAsync(id, dto, GetCurrentUser());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadVehicleOptionsAsync();
            return View(dto);
        }

        TempData["Success"] = "Ceza güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Personnel)]
    public Task<IActionResult> Submit(int id) =>
        RunWorkflowActionAsync(id, () => _fineService.SubmitAsync(id, GetCurrentUser()),
            "Ceza yönetici onayına gönderildi.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Manager + "," + AppRoles.Finance)]
    public Task<IActionResult> Approve(int id, string? note) =>
        RunWorkflowActionAsync(id, () => _fineService.ApproveAsync(id, note, GetCurrentUser()),
            "Ceza onaylandı.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Manager + "," + AppRoles.Finance)]
    public Task<IActionResult> Reject(int id, string? reason) =>
        RunWorkflowActionAsync(id, () => _fineService.RejectAsync(id, reason, GetCurrentUser()),
            "Ceza reddedildi.");

    // ---------- Yardımcılar ----------

    private async Task<IActionResult> RunWorkflowActionAsync(int id, Func<Task<ServiceResult>> action, string successMessage)
    {
        var result = await action();

        if (result.Success)
            TempData["Success"] = successMessage;
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Details), new { id });
    }

    private CurrentUser GetCurrentUser() => new(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!,
        User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList());

    private async Task LoadVehicleOptionsAsync()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        ViewData["Vehicles"] = vehicles
            .Select(v => new SelectListItem($"{v.Plate} - {v.Brand} {v.Model}", v.Id.ToString()))
            .ToList();
    }
}