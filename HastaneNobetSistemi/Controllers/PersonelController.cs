using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Controllers;

[Authorize(Roles = "Personel")]
public class PersonelController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PersonelController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Personel/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user?.PersonelId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var personel = await _context.Personeller.FindAsync(user.PersonelId);

        // Gelecek nöbetler
        var gelecekNobetler = await _context.Nobetler
            .Where(n => n.PersonelId == user.PersonelId && n.Tarih >= DateTime.Today)
            .OrderBy(n => n.Tarih)
            .Take(10)
            .ToListAsync();

        // Geçmiş nöbetler
        var gecmisNobetler = await _context.Nobetler
            .Where(n => n.PersonelId == user.PersonelId && n.Tarih < DateTime.Today)
            .OrderByDescending(n => n.Tarih)
            .Take(5)
            .ToListAsync();

        // Bekleyen takas sayısı
        var bekleyenTakaslar = await _context.NobetTakaslar
            .CountAsync(t => (t.HedefPersonelId == user.PersonelId || t.HedefPersonelId == null)
                          && t.TeklifEdenPersonelId != user.PersonelId
                          && t.Durum == "Beklemede");

        // Bekleyen izin talepleri
        var bekleyenIzinTalepleri = await _context.IzinTalepleri
            .CountAsync(i => i.PersonelId == user.PersonelId && i.Durum == "Beklemede");

        // Onaylanmış ve gelecekteki izinler
        var onaylananIzinler = await _context.IzinTalepleri
            .Where(i => i.PersonelId == user.PersonelId
                     && i.Durum == "Onaylandi"
                     && i.BitisTarihi >= DateTime.Today)
            .OrderBy(i => i.BaslangicTarihi)
            .ToListAsync();

        ViewBag.Personel = personel;
        ViewBag.GelecekNobetler = gelecekNobetler;
        ViewBag.GecmisNobetler = gecmisNobetler;
        ViewBag.BekleyenTakaslar = bekleyenTakaslar;
        ViewBag.BekleyenIzinTalepleri = bekleyenIzinTalepleri;
        ViewBag.OnaylananIzinler = onaylananIzinler;

        return View();
    }

    // GET: /Personel/NobetListesi
    public async Task<IActionResult> NobetListesi(int? ay, int? yil)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var personelId = user.PersonelId.Value;
        var personel = await _context.Personeller.FindAsync(personelId);

        // Yayınlanmış dönemleri getir
        var yayinlar = await _context.NobetYayinlari
            .Where(y => y.YayindaMi)
            .OrderByDescending(y => y.Yil)
            .ThenByDescending(y => y.Ay)
            .ToListAsync();

        // Seçili ay-yıl belirleme
        int seciliAy;
        int seciliYil;

        if (ay.HasValue && yil.HasValue)
        {
            seciliAy = ay.Value;
            seciliYil = yil.Value;
        }
        else if (yayinlar.Any())
        {
            // İlk yayınlanmış dönemi seç
            seciliAy = yayinlar.First().Ay;
            seciliYil = yayinlar.First().Yil;
        }
        else
        {
            // Hiç yayın yoksa mevcut ayı göster
            seciliAy = DateTime.Now.Month;
            seciliYil = DateTime.Now.Year;
        }

        // Seçili dönemin yayında olup olmadığını kontrol et
        var yayindaMi = yayinlar.Any(y => y.Ay == seciliAy && y.Yil == seciliYil);

        // Nöbetleri getir (sadece yayındaysa)
        var nobetlerim = yayindaMi
            ? await _context.Nobetler
                .Where(n => n.PersonelId == personelId
                         && n.Tarih.Month == seciliAy
                         && n.Tarih.Year == seciliYil)
                .OrderBy(n => n.Tarih)
                .ToListAsync()
            : new List<Nobet>();

        ViewBag.Personel = personel;
        ViewBag.Yayinlar = yayinlar;
        ViewBag.SeciliAy = seciliAy;
        ViewBag.SeciliYil = seciliYil;
        ViewBag.YayindaMi = yayindaMi;
        ViewBag.Nobetlerim = nobetlerim;

        return View();
    }

    // GET: /Personel/NobetGecmisim
    public async Task<IActionResult> NobetGecmisim()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null)
            return RedirectToAction("Login", "Account");

        var personelId = user.PersonelId.Value;
        var personel = await _context.Personeller.FindAsync(personelId);

        // Tüm nöbetleri getir
        var tumNobetlerim = await _context.Nobetler
            .Where(n => n.PersonelId == personelId)
            .OrderByDescending(n => n.Tarih)
            .ToListAsync();

        // İstatistikleri hesapla
        var bugun = DateTime.Today;
        var stats = new
        {
            ToplamNobet = tumNobetlerim.Count,
            ToplamHaftaIci = tumNobetlerim.Count(n => n.NobetTipi == "HaftaIci"),
            ToplamHaftaSonu = tumNobetlerim.Count(n => n.NobetTipi == "HaftaSonu"),
            ToplamBayram = tumNobetlerim.Count(n => n.NobetTipi == "Bayram"),
            TamamlananNobet = tumNobetlerim.Count(n => n.Tarih.Date < bugun),
            GelecekNobet = tumNobetlerim.Count(n => n.Tarih.Date >= bugun)
        };

        ViewBag.Personel = personel;
        ViewBag.Stats = stats;

        return View(tumNobetlerim);
    }
}