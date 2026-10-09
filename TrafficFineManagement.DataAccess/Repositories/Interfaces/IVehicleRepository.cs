using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Repositories.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<bool> PlateExistsAsync(string plate, int? excludeId = null);
    Task<bool> HasFinesAsync(int vehicleId);
}