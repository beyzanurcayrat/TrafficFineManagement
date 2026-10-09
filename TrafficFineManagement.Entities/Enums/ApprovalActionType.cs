using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Entities.Enums;

public enum ApprovalActionType
{
    [Display(Name = "Oluşturuldu")] Created = 1,
    [Display(Name = "Onaya Gönderildi")] Submitted = 2,
    [Display(Name = "Onaylandı")] Approved = 3,
    [Display(Name = "Reddedildi")] Rejected = 4,
    [Display(Name = "Güncellendi")] Updated = 5
}