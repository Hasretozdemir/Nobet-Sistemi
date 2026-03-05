using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using HastaneNobetSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Controllers;

[Authorize(Roles = "Yetkili")]
public class PersonelsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PersonelsController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Personels
    public async Task<IActionResult> Index()
    {
        if (_context.Personeller == null)
        {
            return Problem("Veritabanı bağlantısı kurulamadı (Personeller seti null).");
        }

        var personeller = await _context.Personeller.OrderBy(p => p.AdSoyad).ToListAsync();
        return View(personeller);
    }

    // GET: Personels/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Personeller == null)
        {
            return NotFound();
        }

        var personel = await _context.Personeller.FirstOrDefaultAsync(m => m.Id == id);
        if (personel == null)
        {
            return NotFound();
        }

        return View(personel);
    }

    // GET: Personels/Create
    public IActionResult Create()
    {
        return View();
    }

    // ✅ POST: Personels/Create - YENİ VERSİYON (E-POSTA VE ŞİFRE İLE)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonelKayitViewModel model)
    {
        if (ModelState.IsValid)
        {
            // 1. E-posta kontrolü
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor!");
                return View(model);
            }

            // 2. Personel kaydını oluştur
            var personel = new Personel
            {
                AdSoyad = model.AdSoyad,
                IseGirisTarihi = model.IseGirisTarihi,
                AktifMi = model.AktifMi,
                YedekMi = model.YedekMi
            };

            // Ortalama kuralı (nöbet yığılmasını önler)
            var mevcutMaxSira = await _context.Personeller.AnyAsync();
            if (mevcutMaxSira)
            {
                personel.ToplamHaftaIci = (int)(await _context.Personeller.AverageAsync(p => p.ToplamHaftaIci));
                personel.ToplamHaftaSonu = (int)(await _context.Personeller.AverageAsync(p => p.ToplamHaftaSonu));
                personel.ToplamBayram = (int)(await _context.Personeller.AverageAsync(p => p.ToplamBayram));
            }
            else
            {
                personel.ToplamHaftaIci = 0;
                personel.ToplamHaftaSonu = 0;
                personel.ToplamBayram = 0;
            }

            personel.BuAyHaftaIci = 0;
            personel.BuAyHaftaSonu = 0;
            personel.BuAyBayram = 0;
            personel.SonNobetTarihi = null;

            _context.Add(personel);
            await _context.SaveChangesAsync();

            // 3. Kullanıcı hesabını oluştur
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                AdSoyad = model.AdSoyad,
                EmailConfirmed = true,
                PersonelId = personel.Id
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Personel");

                TempData["SuccessMessage"] = $"✅ {personel.AdSoyad} başarıyla eklendi! Giriş: {model.Email}";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Hata durumunda personeli de sil
                _context.Personeller.Remove(personel);
                await _context.SaveChangesAsync();

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        return View(model);
    }

    // GET: Personels/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Personeller == null)
        {
            return NotFound();
        }

        var personel = await _context.Personeller.FindAsync(id);
        if (personel == null)
        {
            return NotFound();
        }
        return View(personel);
    }

    // POST: Personels/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Personel personel, string Email, string Sifre)
    {
        if (id != personel.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                // 1. Personel Bilgilerini Güncelle
                _context.Update(personel);

                // 2. Kullanıcı Bilgilerini Güncelle
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PersonelId == personel.Id);
                if (user != null)
                {
                    // Email ve UserName güncelleme
                    user.Email = Email;
                    user.UserName = Email;

                    if (!string.IsNullOrEmpty(Sifre))
                    {
                        // ✅ DOĞRU YÖNTEM: Şifreyi güvenli bir şekilde hash'leyerek değiştirme
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, Sifre);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(personel);
                        }
                    }

                    await _userManager.UpdateAsync(user);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Personel ve erişim bilgileri başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonelExists(personel.Id)) return NotFound();
                else throw;
            }
        }
        return View(personel);
    }
    // GET: Personels/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Personeller == null)
        {
            return NotFound();
        }

        var personel = await _context.Personeller.FirstOrDefaultAsync(m => m.Id == id);
        if (personel == null)
        {
            return NotFound();
        }

        return View(personel);
    }

    // POST: Personels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Personeller == null)
        {
            return Problem("Entity set 'AppDbContext.Personeller' is null.");
        }

        var personel = await _context.Personeller.FindAsync(id);
        if (personel != null)
        {
            // 1. Kullanıcı hesabını sil
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PersonelId == id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            // 2. İlişkili nöbetleri sil
            var personeleAitNobetler = _context.Nobetler.Where(n => n.PersonelId == id);
            _context.Nobetler.RemoveRange(personeleAitNobetler);

            // 3. Personeli sil
            _context.Personeller.Remove(personel);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Personel ve kullanıcı hesabı silindi.";
        return RedirectToAction(nameof(Index));
    }

    // ✅ YENİ: Toplu Hesap Oluşturma (Eski personeller için)
    public async Task<IActionResult> CreateAllAccounts()
    {
        var personellerWithoutAccount = await _context.Personeller
            .Where(p => p.AktifMi == true)
            .ToListAsync();

        int olusturulan = 0;

        foreach (var personel in personellerWithoutAccount)
        {
            var email = $"{personel.AdSoyad.Replace(" ", "").ToLower()}@hastane.com";
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    AdSoyad = personel.AdSoyad,
                    EmailConfirmed = true,
                    PersonelId = personel.Id
                };

                var result = await _userManager.CreateAsync(user, "Personel123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Personel");
                    olusturulan++;
                }
            }
        }

        TempData["SuccessMessage"] = $"{olusturulan} personel için varsayılan hesap oluşturuldu! Şifre: Personel123!";
        return RedirectToAction(nameof(Index));
    }

    private bool PersonelExists(int id)
    {
        return (_context.Personeller?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}