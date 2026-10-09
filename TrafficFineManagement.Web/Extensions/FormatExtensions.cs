using System.Globalization;
using System.Text.RegularExpressions;
using TrafficFineManagement.Entities.Enums;

namespace TrafficFineManagement.Web.Extensions;

public static class FormatExtensions
{
    private static readonly CultureInfo Tr = new("tr-TR");

    // "34ABC123" -> "34 ABC 123" (veritabanında boşluksuz saklanır, ekranda okunaklı gösterilir)
    public static string FormatPlate(this string plate)
    {
        var match = Regex.Match(plate, @"^(\d{2})([A-Z]{1,3})(\d{2,4})$");
        return match.Success
            ? $"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}"
            : plate;
    }

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