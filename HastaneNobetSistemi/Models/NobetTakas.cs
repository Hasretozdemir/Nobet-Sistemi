using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneNobetSistemi.Models
{
    public class NobetTakas
    {
        public int Id { get; set; }

        // Takas teklif eden personel
        [Display(Name = "Teklif Eden Personel")]
        public int TeklifEdenPersonelId { get; set; }
        [ForeignKey("TeklifEdenPersonelId")]
        public Personel? TeklifEdenPersonel { get; set; }

        // Takas edilecek nöbet
        [Display(Name = "Takas Edilecek Nöbet")]
        public int TeklifEdilenNobetId { get; set; }
        [ForeignKey("TeklifEdilenNobetId")]
        public Nobet? TeklifEdilenNobet { get; set; }

        // Karşı taraf (opsiyonel - null ise herkese açık)
        [Display(Name = "Hedef Personel")]
        public int? HedefPersonelId { get; set; }
        [ForeignKey("HedefPersonelId")]
        public Personel? HedefPersonel { get; set; }

        // Karşı taraf tarafından seçilen nöbet (kabul aşamasında)
        [Display(Name = "Karşılık Nöbet")]
        public int? KarsilikNobetId { get; set; }
        [ForeignKey("KarsilikNobetId")]
        public Nobet? KarsilikNobet { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Takas Nedeni")]
        public string Aciklama { get; set; } = string.Empty;

        [Display(Name = "Durum")]
        [StringLength(20)]
        public string Durum { get; set; } = "Beklemede";
        // Beklemede, Onaylandi, Reddedildi, IptalEdildi

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Yanıt Tarihi")]
        public DateTime? YanitTarihi { get; set; }

        [StringLength(500)]
        [Display(Name = "Red/İptal Nedeni")]
        public string? RedNedeni { get; set; }
    }
}