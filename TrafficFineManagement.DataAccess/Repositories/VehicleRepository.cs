using Microsoft.EntityFrameworkCore;
using TrafficFineManagement.DataAccess.Context;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.DataAccess.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<List<Vehicle>> GetAllAsync() =>
        await DbSet.AsNoTracking().OrderBy(v => v.Plate).ToListAsync();

    public Task<bool> PlateExistsAsync(string plate, int? excludeId = null) =>
        DbSet.AnyAsync(v => v.Plate == plate && (excludeId == null || v.Id != excludeId));

    public Task<bool> HasFinesAsync(int vehicleId) =>
        Context.TrafficFines.AnyAsync(f => f.VehicleId == vehicleId);
}