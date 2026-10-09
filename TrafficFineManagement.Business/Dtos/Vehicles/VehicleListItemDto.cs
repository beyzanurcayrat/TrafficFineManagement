using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Dtos.Vehicles;

public record VehicleListItemDto(int Id, string Plate, VehicleType VehicleType, string Brand, string Model);