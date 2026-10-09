using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Web.Models;

namespace TrafficFineManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITrafficFineService _fineService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ITrafficFineService fineService, ILogger<HomeController> logger)
    {
        _fineService = fineService;
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var summary = await _fineService.GetStatusSummaryAsync();
        return View(summary);
    }

    // 404, 403 gibi durum kodları için kullanıcı dostu sayfa
    public IActionResult HttpStatus(int id)
    {
        return View(id);
    }

    // Beklenmeyen hatalar (Production): logla ve genel hata sayfası göster
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature?.Error is not null)
        {
            _logger.LogError(exceptionFeature.Error, "Beklenmeyen hata. Adres: {Path}", exceptionFeature.Path);
        }

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}