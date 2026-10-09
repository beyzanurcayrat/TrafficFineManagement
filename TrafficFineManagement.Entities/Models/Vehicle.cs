using TrafficFineManagement.Entities.Common;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Entities.Models;

public class Vehicle : BaseEntity
{
    public string Plate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}