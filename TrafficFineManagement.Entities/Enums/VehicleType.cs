using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Entities.Enums;

public enum VehicleType
{
    [Display(Name = "Binek")] Car = 1,
    [Display(Name = "Çekici")] Tractor = 2,
    [Display(Name = "Dorse")] Trailer = 3,
    [Display(Name = "Kiralık Araç")] Rental = 4
}