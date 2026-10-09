using TrafficFineManagement.Business.Common;
using TrafficFineManagement.Entities.Constants;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Business.Workflow;

/// <summary>
/// Trafik cezası onay sürecinin kuralları.
/// Yeni → Yönetici Onayı → Finans Onayı → Tamamlandı. Onay aşamalarından Reddedildi'ye geçilebilir.
/// </summary>
public static class FineWorkflow
{
    // Hangi durumdaki cezayı hangi rol onaylar ve onaydan sonra hangi duruma geçer
    private static readonly IReadOnlyDictionary<FineStatus, (string ApproverRole, FineStatus NextStatus)> ApprovalSteps =
        new Dictionary<FineStatus, (string, FineStatus)>
        {
            [FineStatus.PendingManagerApproval] = (AppRoles.Manager, FineStatus.PendingFinanceApproval),
            [FineStatus.PendingFinanceApproval] = (AppRoles.Finance, FineStatus.Completed)
        };

    // Sadece henüz onaya girmemiş veya reddedilmiş cezalar düzenlenebilir
    public static bool IsEditable(FineStatus status) =>
        status is FineStatus.New or FineStatus.Rejected;

    public static bool CanEdit(FineStatus status, CurrentUser user) =>
        IsEditable(status) && user.IsInRole(AppRoles.Personnel);

    // Reddedilen ceza düzeltilip tekrar onaya gönderilebilir
    public static bool CanSubmit(FineStatus status, CurrentUser user) =>
        CanEdit(status, user);

    // Onay ve ret: sadece cezanın bulunduğu aşamanın yetkilisi
    public static bool CanDecide(FineStatus status, CurrentUser user) =>
        ApprovalSteps.TryGetValue(status, out var step) && user.IsInRole(step.ApproverRole);

    public static FineStatus GetNextStatus(FineStatus status) =>
        ApprovalSteps.TryGetValue(status, out var step)
            ? step.NextStatus
            : throw new InvalidOperationException($"{status} durumundaki ceza onaylanamaz.");
}