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

        // Kendi tekliflerim
        var benimTekliflerim = await _context.NobetTakaslar
            .Include(t => t.TeklifEdilenNobet)
            .Include(t => t.HedefPersonel)
            .Include(t => t.KarsilikNobet)
            .Where(t => t.TeklifEdenPersonelId == personelId)
            .OrderByDescending(t => t.OlusturmaTarihi)
            .ToListAsync();

        // Bana gelen teklifler
        var banaGelenTeklifler = await _context.NobetTakaslar
            .Include(t => t.TeklifEdenPersonel)
            .Include(t => t.TeklifEdilenNobet)
            .Include(t => t.KarsilikNobet)
            .Where(t => (t.HedefPersonelId == personelId || t.HedefPersonelId == null) 
                     && t.TeklifEdenPersonelId != personelId 
                     && t.Durum == "Beklemede")
            .OrderByDescending(t => t.OlusturmaTarihi)
            .ToListAsync();

        ViewBag.BenimTekliflerim = benimTekliflerim;
        ViewBag.BanaGelenTeklifler = banaGelenTeklifler;

        return View();
    }

    // Yeni Takas Teklifi Oluþtur
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var personelId = user.PersonelId.Value;

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

        var digerPersoneller = await _context.Personeller
            .Where(p => p.Id != personelId && p.AktifMi == true)
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

    // Takas Teklifini Kabul Et
    public async Task<IActionResult> Accept(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var takas = await _context.NobetTakaslar
            .Include(t => t.TeklifEdilenNobet)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (takas == null || takas.Durum != "Beklemede")
            return NotFound();

        // Karþýlýk verilecek nöbetleri getir
        var benimGelecekNobetlerim = await _context.Nobetler
            .Where(n => n.PersonelId == user.PersonelId 
                     && n.Tarih > DateTime.Today
                     && n.NobetTipi == takas.TeklifEdilenNobet.NobetTipi) // Ayný tip nöbet
            .OrderBy(n => n.Tarih)
            .ToListAsync();

        ViewBag.KarsilikNobetler = new SelectList(
            benimGelecekNobetlerim.Select(n => new { 
                n.Id, 
                Display = $"{n.Tarih:dd MMMM yyyy dddd}" 
            }), "Id", "Display");

        ViewBag.Takas = takas;
        return View(takas);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptConfirm(int id, int karsilikNobetId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.PersonelId == null) return RedirectToAction("Login", "Account");

        var takas = await _context.NobetTakaslar
            .Include(t => t.TeklifEdilenNobet)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (takas == null || takas.Durum != "Beklemede")
            return NotFound();

        var karsilikNobet = await _context.Nobetler.FindAsync(karsilikNobetId);
        if (karsilikNobet == null || karsilikNobet.PersonelId != user.PersonelId)
            return BadRequest();

        // TAKAS ÝÞLEMÝ
        var teklifEdilenNobet = takas.TeklifEdilenNobet;
        
        var eskiTeklifPersonel = teklifEdilenNobet.PersonelId;
        var eskiKarsilikPersonel = karsilikNobet.PersonelId;

        teklifEdilenNobet.PersonelId = eskiKarsilikPersonel;
        karsilikNobet.PersonelId = eskiTeklifPersonel;

        takas.KarsilikNobetId = karsilikNobetId;
        takas.Durum = "Onaylandi";
        takas.YanitTarihi = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Takas baþarýyla tamamlandý! Nöbetler deðiþtirildi.";
        return RedirectToAction(nameof(Index));
    }

    // Takas Teklifini Reddet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string redNedeni)
    {
        var takas = await _context.NobetTakaslar.FindAsync(id);
        if (takas == null || takas.Durum != "Beklemede")
            return NotFound();

        takas.Durum = "Reddedildi";
        takas.RedNedeni = redNedeni;
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
            return NotFound();

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