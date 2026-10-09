using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Fines;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Web.Controllers.Api;

[ApiController]
[Route("api/fines")]
[Authorize]
[Produces("application/json")]
public class FinesApiController : ControllerBase
{
    private readonly ITrafficFineService _fineService;

    public FinesApiController(ITrafficFineService fineService)
    {
        _fineService = fineService;
    }

    /// <summary>Cezaları listeler. Duruma ve plakaya göre filtrelenebilir.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FineListItemDto>>> GetAll([FromQuery] FineStatus? status, [FromQuery] string? plate)
    {
        return Ok(await _fineService.GetListAsync(status, plate));
    }

    /// <summary>Ceza detayını ve onay geçmişini getirir.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FineDetailDto>> GetById(int id)
    {
        var user = new CurrentUser(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList());

        var fine = await _fineService.GetDetailAsync(id, user);
        return fine is null ? NotFound() : Ok(fine);
    }
}