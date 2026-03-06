using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.ViewModels;

public class PersonelKayitViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [Display(Name = "Ad Soyad")]
    [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sicil No zorunludur")]
    [Display(Name = "Sicil No")]
    [StringLength(20, ErrorMessage = "Sicil No en fazla 20 karakter olabilir")]
    public string SicilNo { get; set; } = string.Empty;

    [Display(Name = "Birim")]
    [StringLength(100, ErrorMessage = "Birim en fazla 100 karakter olabilir")]
    public string? Birim { get; set; }

    [Display(Name = "Ünvan (Görev)")]
    [StringLength(100, ErrorMessage = "Ünvan en fazla 100 karakter olabilir")]
    public string? Unvan { get; set; }

    [Display(Name = "Nöbet Başına Ücret (₺)")]
    [Range(0, 100000, ErrorMessage = "Ücret 0 ile 100.000 arasında olmalıdır")]
    public decimal NobetUcreti { get; set; } = 0;

    [Display(Name = "Ücret Hesaplama Tipi")]
    public string UcretTipi { get; set; } = "Normal";

    [Display(Name = "İcap Saatlik Ücreti (₺)")]
    [Range(0, 100000)]
    public decimal IcapSaatlikUcret { get; set; } = 0;

    [Display(Name = "İcap Süresi (Saat)")]
    [Range(0, 24)]
    public decimal IcapSaat { get; set; } = 0;

    [Display(Name = "Uzaktan Saatlik Ücreti (₺)")]
    [Range(0, 100000)]
    public decimal UzaktanSaatlikUcret { get; set; } = 0;

    [Display(Name = "Uzaktan Süresi (Saat)")]
    [Range(0, 24)]
    public decimal UzaktanSaat { get; set; } = 0;

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

    [Required(ErrorMessage = "Şifre tekrar zorunludur")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre Tekrar")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;
}