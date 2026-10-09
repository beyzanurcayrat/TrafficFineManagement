using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Repositories.Interfaces;

public interface ITrafficFineRepository : IRepository<TrafficFine>
{
    Task<List<TrafficFine>> GetListAsync(FineStatus? status, string? plate);
    Task<TrafficFine?> GetWithDetailsAsync(int id);
    Task<bool> FineNumberExistsAsync(string fineNumber, int? excludeId = null);
    Task<Dictionary<FineStatus, int>> GetStatusCountsAsync();
    Task<bool> IsUserReferencedAsync(string userId);
}