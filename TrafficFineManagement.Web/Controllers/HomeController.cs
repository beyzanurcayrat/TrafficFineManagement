using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Web.Models;

namespace TrafficFineManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITrafficFineService _fineService;

    public HomeController(ITrafficFineService fineService)
    {
        _fineService = fineService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var summary = await _fineService.GetStatusSummaryAsync();
        return View(summary);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}