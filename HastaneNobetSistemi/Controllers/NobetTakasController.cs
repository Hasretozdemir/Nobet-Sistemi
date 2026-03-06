using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Controllers;

[Authorize(Roles = "Personel")]
public class NobetTakasController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public NobetTakasController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Ana Sayfa - Takas Listesi
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var personelId = user.PersonelId.Value;
        var personel = await _context.Personeller.FindAsync(personelId);

        // Kendi tekliflerim
        var benimTekliflerim = await _context.NobetTakaslar
            .Include(t => t.TeklifEdilenNobet).ThenInclude(n => n.Personel)
            .Include(t => t.TeklifEdenPersonel)
            .Include(t => t.HedefPersonel)
            .Include(t => t.KarsilikNobet).ThenInclude(n => n.Personel)
            .Where(t => t.TeklifEdenPersonelId == personelId)
            .OrderByDescending(t => t.OlusturmaTarihi)
            .ToListAsync();

        // Bana gelen teklifler (hedef ben veya herkese açýk, beklemede olanlar)
        var banaGelenTeklifler = await _context.NobetTakaslar
            .Include(t => t.TeklifEdenPersonel)
            .Include(t => t.TeklifEdilenNobet).ThenInclude(n => n.Personel)
            .Include(t => t.KarsilikNobet).ThenInclude(n => n.Personel)
            .Where(t => (t.HedefPersonelId == personelId || t.HedefPersonelId == null)
                     && t.TeklifEdenPersonelId != personelId
                     && t.Durum == "Beklemede")
            .OrderByDescending(t => t.OlusturmaTarihi)
            .ToListAsync();

        // Onaylanmýþ takaslarým (hem teklif eden hem kabul eden olarak)
        var onaylananTakaslarim = await _context.NobetTakaslar
            .Include(t => t.TeklifEdenPersonel)
            .Include(t => t.TeklifEdilenNobet)
            .Include(t => t.KarsilikNobet)
            .Include(t => t.HedefPersonel)
            .Where(t => t.Durum == "Onaylandi"
                     && (t.TeklifEdenPersonelId == personelId || t.KabulEdenPersonelId == personelId))
            .OrderByDescending(t => t.YanitTarihi)
            .ToListAsync();

        ViewBag.Personel = personel;
        ViewBag.BenimTekliflerim = benimTekliflerim;
        ViewBag.BanaGelenTeklifler = banaGelenTeklifler;
        ViewBag.OnaylananTakaslarim = onaylananTakaslarim;

        return View();
    }

    // Yeni Takas Teklifi Oluþtur
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var personelId = user.PersonelId.Value;

        // Personelin hangi iþletmeye ait olduðunu bul
        var personel = await _context.Personeller.FindAsync(personelId);
        var yetkiliUserId = personel?.YetkiliUserId;

        // Gelecekteki nöbetlerimi getir
        var gelecekNobetlerim = await _context.Nobetler
            .Where(n => n.PersonelId == personelId && n.Tarih > DateTime.Today)
            .OrderBy(n => n.Tarih)
            .ToListAsync();

        ViewBag.GelecekNobetlerim = new SelectList(
            gelecekNobetlerim.Select(n => new {
                n.Id,
                Display = $"{n.Tarih:dd MMMM yyyy dddd} - {n.NobetTipi}"
            }), "Id", "Display");

        // Sadece ayný iþletmedeki diðer personelleri göster
        var digerPersoneller = await _context.Personeller
            .Where(p => p.Id != personelId && p.AktifMi == true && p.YetkiliUserId == yetkiliUserId)
            .OrderBy(p => p.AdSoyad)
            .ToListAsync();

        ViewBag.DigerPersoneller = new SelectList(digerPersoneller, "Id", "AdSoyad");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NobetTakas model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        model.TeklifEdenPersonelId = user.PersonelId.Value;
        model.OlusturmaTarihi = DateTime.Now;
        model.Durum = "Beklemede";

        // Ayný nöbet için aktif takas var mý kontrol et
        var mevcutTakas = await _context.NobetTakaslar
            .AnyAsync(t => t.TeklifEdilenNobetId == model.TeklifEdilenNobetId
                        && t.Durum == "Beklemede");

        if (mevcutTakas)
        {
            TempData["ErrorMessage"] = "Bu nöbet için zaten bekleyen bir takas talebi var.";
            return RedirectToAction(nameof(Index));
        }

        _context.NobetTakaslar.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Takas teklifiniz baþarýyla oluþturuldu!";
        return RedirectToAction(nameof(Index));
    }

    // Takas Teklifini Kabul Et - GET
    public async Task<IActionResult> Accept(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var takas = await _context.NobetTakaslar
            .Include(t => t.TeklifEdenPersonel)
            .Include(t => t.TeklifEdilenNobet)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (takas == null || takas.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Bu takas teklifi artýk geçerli deðil.";
            return RedirectToAction(nameof(Index));
        }

        // Karþýlýk verilecek nöbetleri getir (tüm gelecek nöbetler - tip kýsýtlamasý yok)
        var benimGelecekNobetlerim = await _context.Nobetler
            .Where(n => n.PersonelId == user.PersonelId
                     && n.Tarih > DateTime.Today)
            .OrderBy(n => n.Tarih)
            .ToListAsync();

        ViewBag.KarsilikNobetler = new SelectList(
            benimGelecekNobetlerim.Select(n => new {
                n.Id,
                Display = $"{n.Tarih:dd MMMM yyyy dddd} - {n.NobetTipi}"
            }), "Id", "Display");

        return View(takas);
    }

    // Takas Teklifini Kabul Et - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptConfirm(int id, int karsilikNobetId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var takas = await _context.NobetTakaslar
            .Include(t => t.TeklifEdilenNobet)
            .Include(t => t.TeklifEdenPersonel)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (takas == null || takas.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Bu takas teklifi artýk geçerli deðil.";
            return RedirectToAction(nameof(Index));
        }

        var karsilikNobet = await _context.Nobetler.FindAsync(karsilikNobetId);
        if (karsilikNobet == null || karsilikNobet.PersonelId != user.PersonelId)
        {
            TempData["ErrorMessage"] = "Seçilen nöbet geçersiz.";
            return RedirectToAction(nameof(Index));
        }

        // ? NÖBET LÝSTESÝNÝ DEÐÝÞTÝRME! Sadece takas kaydýný güncelle
        takas.KarsilikNobetId = karsilikNobetId;
        takas.KabulEdenPersonelId = user.PersonelId.Value;
        takas.Durum = "Onaylandi";
        takas.YanitTarihi = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Takas anlaþmasý tamamlandý! Nöbet günlerinde birbirinizin yerine nöbet tutacaksýnýz.";
        return RedirectToAction(nameof(Index));
    }

    // Takas Teklifini Reddet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string? redNedeni)
    {
        var takas = await _context.NobetTakaslar.FindAsync(id);
        if (takas == null || takas.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Bu takas teklifi artýk geçerli deðil.";
            return RedirectToAction(nameof(Index));
        }

        takas.Durum = "Reddedildi";
        takas.RedNedeni = redNedeni ?? "Neden belirtilmedi";
        takas.YanitTarihi = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["InfoMessage"] = "Takas teklifi reddedildi.";
        return RedirectToAction(nameof(Index));
    }

    // Kendi Teklifini Ýptal Et
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var takas = await _context.NobetTakaslar.FindAsync(id);

        if (takas == null || takas.TeklifEdenPersonelId != user.PersonelId)
        {
            TempData["ErrorMessage"] = "Takas teklifi bulunamadý.";
            return RedirectToAction(nameof(Index));
        }

        if (takas.Durum != "Beklemede")
        {
            TempData["ErrorMessage"] = "Sadece bekleyen teklifler iptal edilebilir.";
            return RedirectToAction(nameof(Index));
        }

        takas.Durum = "IptalEdildi";
        takas.YanitTarihi = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["InfoMessage"] = "Takas teklifiniz iptal edildi.";
        return RedirectToAction(nameof(Index));
    }
}