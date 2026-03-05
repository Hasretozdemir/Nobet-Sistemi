namespace HastaneNobetSistemi.Models;

public class NobetYayini
{
    public int Id { get; set; }
    public int Ay { get; set; }
    public int Yil { get; set; }
    public bool YayindaMi { get; set; } = false;
    public DateTime YayinlanmaTarihi { get; set; } = DateTime.Now;
}