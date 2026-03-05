using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.ViewModels;

public class PersonelKayitViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [Display(Name = "Ad Soyad")]
    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "İşe giriş tarihi zorunludur")]
    [Display(Name = "İşe Giriş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime IseGirisTarihi { get; set; } = DateTime.Now;

    [Display(Name = "Aktif Mi?")]
    public bool AktifMi { get; set; } = true;

    [Display(Name = "Yedek Personel Mi?")]
    public bool YedekMi { get; set; } = false;

    // ✅ Sistem Giriş Bilgileri
    [Required(ErrorMessage = "E-posta zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    [Display(Name = "E-posta (Giriş için)")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Şifre Tekrar")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;
}