using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.ViewModels;

public class YetkiliLoginViewModel
{
    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
    [Display(Name = "E-posta")]
    public string YetkiliEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string YetkiliPassword { get; set; } = string.Empty;

    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }
}