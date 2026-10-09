using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Dtos.Vehicles;
using TrafficFineManagement.Business.Services.Interfaces;

namespace TrafficFineManagement.Web.Controllers.Api;

[ApiController]
[Route("api/vehicles")]
[Authorize]
[Produces("application/json")]
public class VehiclesApiController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesApiController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>Tüm araçları listeler.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VehicleListItemDto>>> GetAll()
    {
        return Ok(await _vehicleService.GetAllAsync());
    }
}