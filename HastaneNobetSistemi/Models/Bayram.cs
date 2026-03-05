using System.ComponentModel.DataAnnotations;

namespace HastaneNobetSistemi.Models
{
    public class Bayram
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BayramAdi { get; set; } = string.Empty;

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }
    }
}