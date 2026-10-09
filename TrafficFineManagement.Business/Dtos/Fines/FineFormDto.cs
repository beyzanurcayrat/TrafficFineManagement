using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Business.Dtos.Fines;

public class FineFormDto
{
    [Display(Name = "Araç")]
    [Required(ErrorMessage = "Araç seçiniz.")]
    public int? VehicleId { get; set; }

    [Display(Name = "Tutanak No")]
    [Required(ErrorMessage = "Tutanak numarası zorunludur.")]
    [StringLength(30, ErrorMessage = "Tutanak numarası en fazla 30 karakter olabilir.")]
    public string FineNumber { get; set; } = string.Empty;

    [Display(Name = "İhlal Tarihi")]
    [Required(ErrorMessage = "İhlal tarihi zorunludur.")]
    public DateTime? ViolationDate { get; set; }

    [Display(Name = "İhlal Türü")]
    [Required(ErrorMessage = "İhlal türü zorunludur.")]
    [StringLength(100, ErrorMessage = "İhlal türü en fazla 100 karakter olabilir.")]
    public string ViolationType { get; set; } = string.Empty;

    [Display(Name = "Yer")]
    [StringLength(200, ErrorMessage = "Yer en fazla 200 karakter olabilir.")]
    public string? Location { get; set; }

    [Display(Name = "Tutar (₺)")]
    [Required(ErrorMessage = "Tutar zorunludur.")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Tutar 0,01 ile 1.000.000 arasında olmalıdır.")]
    public decimal? Amount { get; set; }

    [Display(Name = "Açıklama")]
    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    public string? Description { get; set; }
}