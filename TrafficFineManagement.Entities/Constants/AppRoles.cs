namespace TrafficFineManagement.Entities.Constants;

public static class AppRoles
{
    public const string Personnel = "Personel";
    public const string Manager = "Yonetici";
    public const string Finance = "Finans";
    public const string Admin = "Admin";

    // Ekranda gösterilecek isimler; aynı zamanda geçerli rollerin listesi
    public static readonly IReadOnlyDictionary<string, string> DisplayNames = new Dictionary<string, string>
    {
        [Personnel] = "Personel",
        [Manager] = "Yönetici",
        [Finance] = "Finans",
        [Admin] = "Sistem Yöneticisi"
    };
}