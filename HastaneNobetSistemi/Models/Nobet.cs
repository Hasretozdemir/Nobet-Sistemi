using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HastaneNobetSistemi.Models
{
    public class Nobet
    {
        public int Id { get; set; }

        // Personel ile ilişki
        public int PersonelId { get; set; }

        [ForeignKey("PersonelId")]
        public Personel? Personel { get; set; }

        public DateTime Tarih { get; set; }

        public int PuanDegeri { get; set; } // 0 günün zorluk puanı

        [StringLength(20)]
        public string NobetTipi { get; set; } = string.Empty; // 'HaftaIci', 'HaftaSonu', 'Bayram'
    }
}