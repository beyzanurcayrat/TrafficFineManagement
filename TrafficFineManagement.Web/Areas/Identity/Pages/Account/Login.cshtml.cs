using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficFineManagement.Entities.Identity;

namespace TrafficFineManagement.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class LoginInput
    {
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni hatırla")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!ModelState.IsValid)
            return Page();

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("Kullanıcı giriş yaptı: {Email}", Input.Email);
            return LocalRedirect(ReturnUrl);
        }

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "Hesabınız kilitli veya pasif durumda. Lütfen sistem yöneticisiyle iletişime geçin."
            : "E-posta veya şifre hatalı.");

        return Page();
    }
}