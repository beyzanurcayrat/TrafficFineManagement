using System.ComponentModel.DataAnnotations;

namespace TrafficFineManagement.Entities.Enums;

public enum FineStatus
{
    [Display(Name = "Yeni")] New = 1,
    [Display(Name = "Yönetici Onayında")] PendingManagerApproval = 2,
    [Display(Name = "Finans Onayında")] PendingFinanceApproval = 3,
    [Display(Name = "Tamamlandı")] Completed = 4,
    [Display(Name = "Reddedildi")] Rejected = 5
}