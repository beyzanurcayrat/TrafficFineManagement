using System.Globalization;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Web.Extensions;

public static class FormatExtensions
{
    private static readonly CultureInfo Tr = new("tr-TR");

    public static string ToMoney(this decimal amount) => amount.ToString("N2", Tr) + " ₺";

    public static string ToTrDateTime(this DateTime date) => date.ToString("dd.MM.yyyy HH:mm", Tr);

    public static string GetBadgeClass(this FineStatus status) => status switch
    {
        FineStatus.New => "bg-secondary",
        FineStatus.PendingManagerApproval => "bg-warning text-dark",
        FineStatus.PendingFinanceApproval => "bg-info text-dark",
        FineStatus.Completed => "bg-success",
        FineStatus.Rejected => "bg-danger",
        _ => "bg-light text-dark"
    };
}