using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Dtos.Vehicles;
using TrafficFineManagement.Business.Services.Interfaces;

namespace TrafficFineManagement.Web.Controllers;

[Authorize]
public class VehiclesController : Controller
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        return View(vehicles);
    }

    [HttpGet]
    public IActionResult Create() => View(new VehicleFormDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _vehicleService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(dto);
        }

        TempData["Success"] = "Araç başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _vehicleService.GetForEditAsync(id);
        if (dto is null)
            return NotFound();

        ViewData["VehicleId"] = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleFormDto dto)
    {
        ViewData["VehicleId"] = id;

        if (!ModelState.IsValid)
            return View(dto);

        var result = await _vehicleService.UpdateAsync(id, dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(dto);
        }

        TempData["Success"] = "Araç güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _vehicleService.DeleteAsync(id);

        if (result.Success)
            TempData["Success"] = "Araç silindi.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}