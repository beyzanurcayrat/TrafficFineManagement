using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Business.Dtos.Vehicles;
using TrafficFineManagement.Business.Services.Interfaces;
using TrafficFineManagement.DataAccess.Repositories.Interfaces;
using TrafficFineManagement.Entities.Models;

namespace TrafficFineManagement.Business.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<List<VehicleListItemDto>> GetAllAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles
            .Select(v => new VehicleListItemDto(v.Id, v.Plate, v.VehicleType, v.Brand, v.Model))
            .ToList();
    }

    public async Task<VehicleFormDto?> GetForEditAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle is null) return null;

        return new VehicleFormDto
        {
            Plate = vehicle.Plate,
            VehicleType = vehicle.VehicleType,
            Brand = vehicle.Brand,
            Model = vehicle.Model
        };
    }

    public async Task<ServiceResult> CreateAsync(VehicleFormDto dto)
    {
        var plate = NormalizePlate(dto.Plate);

        if (await _vehicleRepository.PlateExistsAsync(plate))
            return ServiceResult.Fail($"{plate} plakalı araç zaten kayıtlı.");

        var vehicle = new Vehicle
        {
            Plate = plate,
            VehicleType = dto.VehicleType!.Value,
            Brand = dto.Brand.Trim(),
            Model = dto.Model.Trim()
        };

        await _vehicleRepository.AddAsync(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> UpdateAsync(int id, VehicleFormDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle is null)
            return ServiceResult.Fail("Araç bulunamadı.");

        var plate = NormalizePlate(dto.Plate);
        if (await _vehicleRepository.PlateExistsAsync(plate, excludeId: id))
            return ServiceResult.Fail($"{plate} plakalı araç zaten kayıtlı.");

        vehicle.Plate = plate;
        vehicle.VehicleType = dto.VehicleType!.Value;
        vehicle.Brand = dto.Brand.Trim();
        vehicle.Model = dto.Model.Trim();

        _vehicleRepository.Update(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle is null)
            return ServiceResult.Fail("Araç bulunamadı.");

        _vehicleRepository.Remove(vehicle);
        await _vehicleRepository.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    // "34 abc 123" -> "34ABC123": aynı plakanın farklı yazımlarla iki kez eklenmesini önler
    private static string NormalizePlate(string plate) =>
        string.Concat(plate.Where(c => !char.IsWhiteSpace(c))).ToUpperInvariant();
}