using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Controllers;

[Authorize]
public class IzinController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public IzinController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ========== PERSONEL TARAFLI İŞLEMLER ==========

    // GET: /Izin/TalepOlustur - Personelin izin talebi oluşturma sayfası
    [Authorize(Roles = "Personel")]
    public async Task<IActionResult> TalepOlustur()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var personel = await _context.Personeller.FindAsync(user.PersonelId);
        ViewBag.Personel = personel;

        return View();
    }

    // POST: /Izin/TalepOlustur
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Personel")]
    public async Task<IActionResult> TalepOlustur(IzinTalebi model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        // Tarih kontrolü
        if (model.BaslangicTarihi < DateTime.Today)
        {
            TempData["ErrorMessage"] = "İzin başlangıç tarihi bugünden önce olamaz!";
            return View(model);
        }

        if (model.BitisTarihi < model.BaslangicTarihi)
        {
            TempData["ErrorMessage"] = "İzin bitiş tarihi başlangıç tarihinden önce olamaz!";
            return View(model);
        }

        // Çakışma kontrolü
        var mevcutIzinler = await _context.IzinTalepleri
            .Where(i => i.PersonelId == user.PersonelId.Value
                     && i.Durum == "Onaylandi"
                     && ((model.BaslangicTarihi >= i.BaslangicTarihi && model.BaslangicTarihi <= i.BitisTarihi)
                      || (model.BitisTarihi >= i.BaslangicTarihi && model.BitisTarihi <= i.BitisTarihi)))
            .AnyAsync();

        if (mevcutIzinler)
        {
            TempData["ErrorMessage"] = "Bu tarihler arasında zaten onaylanmış izniniz var!";
            return View(model);
        }

        model.PersonelId = user.PersonelId.Value;
        model.TalepTarihi = DateTime.Now;
        model.Durum = "Beklemede";

        _context.IzinTalepleri.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İzin talebiniz başarıyla oluşturuldu. Yetkili onayını bekliyor.";
        return RedirectToAction("TaleplerimListesi");
    }

    // GET: /Izin/TaleplerimListesi - Personelin kendi taleplerini görmesi
    [Authorize(Roles = "Personel")]
    public async Task<IActionResult> TaleplerimListesi()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var taleplerim = await _context.IzinTalepleri
            .Where(i => i.PersonelId == user.PersonelId.Value)
            .OrderByDescending(i => i.TalepTarihi)
            .ToListAsync();

        var personel = await _context.Personeller.FindAsync(user.PersonelId);
        ViewBag.Personel = personel;

        return View(taleplerim);
    }

    // GET: /Izin/TalepSil/5 - Beklemedeki talebi silme
    [Authorize(Roles = "Personel")]
    public async Task<IActionResult> TalepSil(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var talep = await _context.IzinTalepleri.FindAsync(id);

        if (talep == null || talep.PersonelId != user.PersonelId.Value)
        {
            TempData["ErrorMessage"] = "Talep bulunamadı veya bu talebi silme yetkiniz yok!";
            return RedirectToAction("TaleplerimListesi");
        }

        if (talep.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Sadece beklemedeki talepler silinebilir!";
            return RedirectToAction("TaleplerimListesi");
        }

        return View(talep);
    }

    // POST: /Izin/TalepSil/5
    [HttpPost, ActionName("TalepSil")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Personel")]
    public async Task<IActionResult> TalepSilConfirmed(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var talep = await _context.IzinTalepleri.FindAsync(id);

        if (talep == null || talep.PersonelId != user.PersonelId.Value || talep.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "İşlem başarısız!";
            return RedirectToAction("TaleplerimListesi");
        }

        _context.IzinTalepleri.Remove(talep);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İzin talebiniz silindi.";
        return RedirectToAction("TaleplerimListesi");
    }

    // ========== YETKİLİ TARAFLI İŞLEMLER ==========

    // GET: /Izin/YonetimPaneli - Tüm izin taleplerini yönetme
    [Authorize(Roles = "Yetkili")]
    public async Task<IActionResult> YonetimPaneli(string durum = "Beklemede")
    {
        var talepler = await _context.IzinTalepleri
            .Include(i => i.Personel)
            .Where(i => i.Durum == durum)
            .OrderByDescending(i => i.TalepTarihi)
            .ToListAsync();

        ViewBag.SeciliDurum = durum;

        // İstatistikler
        ViewBag.BekleyenSayisi = await _context.IzinTalepleri.CountAsync(i => i.Durum == "Beklemede");
        ViewBag.OnaylananSayisi = await _context.IzinTalepleri.CountAsync(i => i.Durum == "Onaylandi");
        ViewBag.RedSayisi = await _context.IzinTalepleri.CountAsync(i => i.Durum == "Reddedildi");

        return View(talepler);
    }

    // GET: /Izin/TalepDetay/5 - Talep detayı
    [Authorize(Roles = "Yetkili")]
    public async Task<IActionResult> TalepDetay(int id)
    {
        var talep = await _context.IzinTalepleri
            .Include(i => i.Personel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (talep == null)
        {
            TempData["ErrorMessage"] = "Talep bulunamadı!";
            return RedirectToAction("YonetimPaneli");
        }

        // Bu tarihte personelin nöbeti var mı kontrol et
        var nobetVarMi = await _context.Nobetler
            .AnyAsync(n => n.PersonelId == talep.PersonelId
                        && n.Tarih >= talep.BaslangicTarihi.Date
                        && n.Tarih <= talep.BitisTarihi.Date);

        ViewBag.NobetCakismasi = nobetVarMi;

        return View(talep);
    }

    // ✅ POST: /Izin/TalebiOnayla/5 - OTOMATİK ZORUNLU PERSONEL ATAMA İLE
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Yetkili")]
    public async Task<IActionResult> TalebiOnayla(int id, string? yetkiliNotu)
    {
        var talep = await _context.IzinTalepleri
            .Include(i => i.Personel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (talep == null)
        {
            TempData["ErrorMessage"] = "Talep bulunamadı!";
            return RedirectToAction("YonetimPaneli");
        }

        if (talep.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Sadece beklemedeki talepler onaylanabilir!";
            return RedirectToAction("YonetimPaneli");
        }

        // Onaylama işlemi
        talep.Durum = "Onaylandi";
        talep.OnayTarihi = DateTime.Now;
        talep.YetkiliNotu = yetkiliNotu;

        // Personel tablosuna da izin tarihlerini yaz
        var personel = await _context.Personeller.FindAsync(talep.PersonelId);
        if (personel != null)
        {
            personel.IzinBaslangic = talep.BaslangicTarihi;
            personel.IzinBitis = talep.BitisTarihi;
            _context.Personeller.Update(personel);
        }

        _context.IzinTalepleri.Update(talep);
        await _context.SaveChangesAsync();

        // ✅ OTOMATİK ZORUNLU PERSONEL ATAMA
        int atananSayisi = await OtomatikZorunluPersonelAta(talep.PersonelId, talep.BaslangicTarihi, talep.BitisTarihi);

        if (atananSayisi > 0)
        {
            TempData["SuccessMessage"] = $"{personel?.AdSoyad} personelinin izin talebi ONAYLANDI! {atananSayisi} nöbet için zorunlu personel otomatik atandı.";
        }
        else
        {
            TempData["SuccessMessage"] = $"{personel?.AdSoyad} personelinin izin talebi ONAYLANDI!";
        }

        return RedirectToAction("YonetimPaneli");
    }

    // ✅ ZORUNLU PERSONEL OTOMATİK ATAMA FONKSİYONU - SIRA SİSTEMİ İLE (ADİL DAĞILIM)
    private async Task<int> OtomatikZorunluPersonelAta(int izinliPersonelId, DateTime baslangic, DateTime bitis)
    {
        int atananSayisi = 0;

        // İzinli personelin bu tarihlerdeki nöbetlerini bul
        var nobetler = await _context.Nobetler
            .Where(n => n.PersonelId == izinliPersonelId
                     && n.Tarih >= baslangic.Date
                     && n.Tarih <= bitis.Date)
            .OrderBy(n => n.Tarih) // Tarihe göre sırala
            .ToListAsync();

        if (!nobetler.Any())
            return 0;

        // Zorunlu personel listesini çek (aktif ve zorunlu olanlar)
        var zorunluPersoneller = await _context.Personeller
            .Where(p => p.ZorunluNobetciMi
                     && (p.AktifMi ?? true)
                     && p.Id != izinliPersonelId) // İzinli olanı hariç tut
            .ToListAsync();

        if (!zorunluPersoneller.Any())
            return 0;

        // Her nöbet için en uygun kişiyi bul (en az nöbet tutan)
        foreach (var nobet in nobetler)
        {
            // Bu tarihte başka izinli var mı kontrol et
            var buTarihtekiIzinliIdler = await _context.Personeller
                .Where(p => p.Id != izinliPersonelId
                         && p.IzinBaslangic.HasValue
                         && p.IzinBitis.HasValue
                         && nobet.Tarih >= p.IzinBaslangic.Value.Date
                         && nobet.Tarih <= p.IzinBitis.Value.Date)
                .Select(p => p.Id)
                .ToListAsync();

            // Müsait zorunlu personeller (izinli olmayanlar)
            var musaitZorunluPersoneller = zorunluPersoneller
                .Where(p => !buTarihtekiIzinliIdler.Contains(p.Id))
                .ToList();

            if (!musaitZorunluPersoneller.Any())
                continue; // Hiç müsait yoksa bu nöbeti atla

            // ✅ EN AZ NÖBET TUTANI BUL (Adil dağılım için)
            Personel enUygun;

            if (nobet.NobetTipi == "HaftaIci")
            {
                // Hafta içi nöbeti için toplam hafta içi nöbeti en az olanı seç
                enUygun = musaitZorunluPersoneller
                    .OrderBy(p => p.ToplamHaftaIci)
                    .ThenBy(p => p.SonNobetTarihi ?? DateTime.MinValue) // Son nöbet tarihi de önemli
                    .ThenBy(p => p.HaftaIciSira)
                    .First();
            }
            else if (nobet.NobetTipi == "HaftaSonu")
            {
                // Hafta sonu nöbeti için toplam hafta sonu nöbeti en az olanı seç
                enUygun = musaitZorunluPersoneller
                    .OrderBy(p => p.ToplamHaftaSonu)
                    .ThenBy(p => p.SonNobetTarihi ?? DateTime.MinValue)
                    .ThenBy(p => p.HaftaSonuSira)
                    .First();
            }
            else // Bayram
            {
                // Bayram nöbeti için toplam bayram nöbeti en az olanı seç
                enUygun = musaitZorunluPersoneller
                    .OrderBy(p => p.ToplamBayram)
                    .ThenBy(p => p.SonNobetTarihi ?? DateTime.MinValue)
                    .ThenBy(p => p.BayramSira)
                    .First();
            }

            // Nöbeti değiştir
            nobet.PersonelId = enUygun.Id;
            _context.Nobetler.Update(nobet);

            // ✅ Atanan personelin sayaçlarını artır
            if (nobet.NobetTipi == "HaftaIci")
            {
                enUygun.BuAyHaftaIci++;
                enUygun.ToplamHaftaIci++;
            }
            else if (nobet.NobetTipi == "HaftaSonu")
            {
                enUygun.BuAyHaftaSonu++;
                enUygun.ToplamHaftaSonu++;
            }
            else // Bayram
            {
                enUygun.BuAyBayram++;
                enUygun.ToplamBayram++;
            }

            enUygun.SonNobetTarihi = nobet.Tarih;
            _context.Personeller.Update(enUygun);

            atananSayisi++;
        }

        await _context.SaveChangesAsync();
        return atananSayisi;
    }

    // POST: /Izin/TalebiReddet/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Yetkili")]
    public async Task<IActionResult> TalebiReddet(int id, string? yetkiliNotu)
    {
        var talep = await _context.IzinTalepleri
            .Include(i => i.Personel)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (talep == null)
        {
            TempData["ErrorMessage"] = "Talep bulunamadı!";
            return RedirectToAction("YonetimPaneli");
        }

        if (talep.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Sadece beklemedeki talepler reddedilebilir!";
            return RedirectToAction("YonetimPaneli");
        }

        talep.Durum = "Reddedildi";
        talep.OnayTarihi = DateTime.Now;
        talep.YetkiliNotu = yetkiliNotu ?? "Red nedeni belirtilmedi.";

        _context.IzinTalepleri.Update(talep);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"{talep.Personel?.AdSoyad} personelinin izin talebi REDDEDİLDİ!";
        return RedirectToAction("YonetimPaneli");
    }

    // GET: /Izin/IzinTakvimi - Tüm personellerin izinlerini takvim görünümü
    [Authorize(Roles = "Yetkili")]
    public async Task<IActionResult> IzinTakvimi(int? ay, int? yil)
    {
        int seciliAy = ay ?? DateTime.Now.Month;
        int seciliYil = yil ?? DateTime.Now.Year;

        var ayBas = new DateTime(seciliYil, seciliAy, 1);
        var aySon = ayBas.AddMonths(1).AddDays(-1);

        // Onaylanan izinler
        var izinler = await _context.IzinTalepleri
            .Include(i => i.Personel)
            .Where(i => i.Durum == "Onaylandi"
                     && ((i.BaslangicTarihi >= ayBas && i.BaslangicTarihi <= aySon)
                      || (i.BitisTarihi >= ayBas && i.BitisTarihi <= aySon)
                      || (i.BaslangicTarihi <= ayBas && i.BitisTarihi >= aySon)))
            .ToListAsync();

        ViewBag.SeciliAy = seciliAy;
        ViewBag.SeciliYil = seciliYil;

        return View(izinler);
    }
}