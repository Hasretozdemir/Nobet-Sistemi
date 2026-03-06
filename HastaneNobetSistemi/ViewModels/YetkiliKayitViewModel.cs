using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.ViewModels;

public class YetkiliKayitViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [StringLength(100)]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ýþletme adý zorunludur")]
    [StringLength(200)]
    [Display(Name = "Ýþletme Adý")]
    public string IsletmeAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre zorunludur")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr")]
    [DataType(DataType.Password)]
    [Display(Name = "Þifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre tekrarý zorunludur")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Þifreler uyuþmuyor")]
    [Display(Name = "Þifre Tekrar")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
