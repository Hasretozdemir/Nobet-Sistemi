// Models/IzinTalebi.cs - YENİ MODEL
namespace HastaneNobetSistemi.Models;

public class IzinTalebi
{
    public int Id { get; set; }
    public int PersonelId { get; set; }
    public virtual Personel Personel { get; set; }

    public DateTime TalepTarihi { get; set; } = DateTime.Now;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public string Aciklama { get; set; } = string.Empty;

    // Onay Durumu
    public string Durum { get; set; } = "Beklemede"; // "Beklemede", "Onaylandi", "Reddedildi"
    public DateTime? OnayTarihi { get; set; }
    public string? YetkiliNotu { get; set; }
}