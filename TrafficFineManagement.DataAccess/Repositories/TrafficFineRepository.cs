using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.DataAccess.Context;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.Entities.Enums;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Repositories;

public class TrafficFineRepository : Repository<TrafficFine>, ITrafficFineRepository
{
    public TrafficFineRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<TrafficFine>> GetListAsync(FineStatus? status, string? plate)
    {
        var query = DbSet.AsNoTracking()
            .Include(f => f.Vehicle)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(f => f.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(plate))
            query = query.Where(f => f.Vehicle.Plate.Contains(plate));

        return await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
    }

    public Task<TrafficFine?> GetWithDetailsAsync(int id) =>
        DbSet
            .Include(f => f.Vehicle)
            .Include(f => f.CreatedByUser)
            .Include(f => f.ApprovalHistories).ThenInclude(h => h.User)
            .FirstOrDefaultAsync(f => f.Id == id);

    public Task<bool> FineNumberExistsAsync(string fineNumber, int? excludeId = null) =>
        DbSet.AnyAsync(f => f.FineNumber == fineNumber && (excludeId == null || f.Id != excludeId));
}