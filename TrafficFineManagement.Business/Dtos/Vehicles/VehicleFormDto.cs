using System.ComponentModel.DataAnnotations;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Dtos.Vehicles;

public class VehicleFormDto
{
    [Display(Name = "Plaka")]
    [Required(ErrorMessage = "Plaka zorunludur.")]
    [RegularExpression(@"^\s*(0[1-9]|[1-7][0-9]|8[01])\s*[A-Za-z]{1,3}\s*[0-9]{2,4}\s*$",
        ErrorMessage = "Geçerli bir plaka giriniz (örn. 34 ABC 123).")]
    public string Plate { get; set; } = string.Empty;

    [Display(Name = "Araç Tipi")]
    [Required(ErrorMessage = "Araç tipi seçiniz.")]
    public VehicleType? VehicleType { get; set; }

    [Display(Name = "Marka")]
    [Required(ErrorMessage = "Marka zorunludur.")]
    [StringLength(50, ErrorMessage = "Marka en fazla 50 karakter olabilir.")]
    public string Brand { get; set; } = string.Empty;

    [Display(Name = "Model")]
    [Required(ErrorMessage = "Model zorunludur.")]
    [StringLength(50, ErrorMessage = "Model en fazla 50 karakter olabilir.")]
    public string Model { get; set; } = string.Empty;
}