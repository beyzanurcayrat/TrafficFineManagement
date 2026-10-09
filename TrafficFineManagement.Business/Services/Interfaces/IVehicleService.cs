using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Vehicles;

namespace TrafficFineManagement.Business.Services.Interfaces;

public interface IVehicleService
{
    Task<List<VehicleListItemDto>> GetAllAsync();
    Task<VehicleFormDto?> GetForEditAsync(int id);
    Task<ServiceResult> CreateAsync(VehicleFormDto dto);
    Task<ServiceResult> UpdateAsync(int id, VehicleFormDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}