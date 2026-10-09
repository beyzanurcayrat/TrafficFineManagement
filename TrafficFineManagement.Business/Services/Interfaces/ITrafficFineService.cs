using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Fines;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Services.Interfaces;

public interface ITrafficFineService
{
    Task<List<FineListItemDto>> GetListAsync(FineStatus? status, string? plate);
    Task<FineDetailDto?> GetDetailAsync(int id, CurrentUser user);
    Task<FineFormDto?> GetForEditAsync(int id);
    Task<ServiceResult<int>> CreateAsync(FineFormDto dto, CurrentUser user);
    Task<ServiceResult> UpdateAsync(int id, FineFormDto dto, CurrentUser user);
    Task<ServiceResult> SubmitAsync(int id, CurrentUser user);
    Task<ServiceResult> ApproveAsync(int id, string? note, CurrentUser user);
    Task<ServiceResult> RejectAsync(int id, string? reason, CurrentUser user);
}