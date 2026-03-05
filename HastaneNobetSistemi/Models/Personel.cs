using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.Models
{
    public class Personel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        public string AdSoyad { get; set; } = string.Empty;


        [Required(ErrorMessage = "Sicil No zorunludur")]
        [StringLength(20)]
        public string SicilNo { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Birim { get; set; }

        [Required]
        public DateTime IseGirisTarihi { get; set; } = DateTime.Now;

        public bool? AktifMi { get; set; } = true;

        // İzin Bilgileri
        public DateTime? IzinBaslangic { get; set; }
        public DateTime? IzinBitis { get; set; }

        // Yedek Mi?
        public bool YedekMi { get; set; } = false;

        // Zorunlu Nöbetçi Mi? (Bayram/Özel günlerde zorunlu nöbet tutan personel)
        public bool ZorunluNobetciMi { get; set; } = false;

        // Nöbet Sırası Bilgileri
        public int HaftaIciSira { get; set; } = 0;
        public int HaftaSonuSira { get; set; } = 0;
        public int BayramSira { get; set; } = 0;

        // Nöbet Sayaçları - Aylık
        public int BuAyHaftaIci { get; set; } = 0;
        public int BuAyHaftaSonu { get; set; } = 0;
        public int BuAyBayram { get; set; } = 0;

        // Nöbet Sayaçları - Toplam
        public int ToplamHaftaIci { get; set; } = 0;
        public int ToplamHaftaSonu { get; set; } = 0;
        public int ToplamBayram { get; set; } = 0;

        public DateTime? SonNobetTarihi { get; set; }

        // Navigation
        public virtual ICollection<Nobet>? Nobetler { get; set; }
    }
}