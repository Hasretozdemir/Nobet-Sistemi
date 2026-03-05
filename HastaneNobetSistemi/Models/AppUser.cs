// Models/AppUser.cs - OLUŞTURULACAK YENİ DOSYA
using Microsoft.AspNetCore.Identity;

namespace HastaneNobetSistemi.Models;

public class AppUser : IdentityUser
{
    public int? PersonelId { get; set; } // Personel ile ilişki (Yetkili için null)
    public virtual Personel? Personel { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
}