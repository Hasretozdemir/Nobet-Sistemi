using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using HastaneNobetSistemi.Services;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HastaneNobetSistemi.Controllers
{
    [Authorize(Roles = "Yetkili")]
    public class NobetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NobetDagiticisi _nobetDagiticisi;
        private readonly UserManager<AppUser> _userManager;

        public NobetsController(AppDbContext context, NobetDagiticisi nobetDagiticisi, UserManager<AppUser> userManager)
        {
            _context = context;
            _nobetDagiticisi = nobetDagiticisi;
            _userManager = userManager;
        }

        private async Task<string> GetYetkiliUserId()
        {
            var user = await _userManager.GetUserAsync(User);
            return user!.Id;
        }

        private async Task<List<int>> GetYetkiliPersonelIds()
        {
            var yetkiliUserId = await GetYetkiliUserId();
            return await _context.Personeller
                .Where(p => p.YetkiliUserId == yetkiliUserId)
                .Select(p => p.Id)
                .ToListAsync();
        }

        // GET: Nobets
        public async Task<IActionResult> Index(int? ay, int? yil)
        {
            var personelIds = await GetYetkiliPersonelIds();

            int seciliAy;
            int seciliYil;

            if (ay.HasValue && yil.HasValue)
            {
                seciliAy = ay.Value;
                seciliYil = yil.Value;
            }
            else
            {
                var sonNobet = await _context.Nobetler
                    .Where(n => personelIds.Contains(n.PersonelId))
                    .OrderByDescending(n => n.Tarih)
                    .FirstOrDefaultAsync();

                if (sonNobet != null)
                {
                    seciliAy = sonNobet.Tarih.Month;
                    seciliYil = sonNobet.Tarih.Year;
                }
                else
                {
                    seciliAy = DateTime.Now.Month;
                    seciliYil = DateTime.Now.Year;
                }
            }

            ViewBag.SeciliAy = seciliAy;
            ViewBag.SeciliYil = seciliYil;

            // ✅ Yayın durumu kontrolü
            var yetkiliUserId = await GetYetkiliUserId();
            var yayindaMi = await _context.NobetYayinlari
                .AnyAsync(y => y.Ay == seciliAy && y.Yil == seciliYil && y.YayindaMi);
            ViewBag.YayindaMi = yayindaMi;

            var liste = await _context.Nobetler
                .Include(n => n.Personel)
                .Where(n => n.Tarih.Month == seciliAy && n.Tarih.Year == seciliYil && personelIds.Contains(n.PersonelId))
                .OrderBy(n => n.Tarih)
                .ToListAsync();

            return View(liste);
        }

        // ✅ ZORUNLU NÖBETÇİ LİSTESİ
        public async Task<IActionResult> ZorunluListe()
        {
            var yetkiliUserId = await GetYetkiliUserId();
            var zorunluPersoneller = await _context.Personeller
                .Where(p => p.ZorunluNobetciMi && (p.AktifMi ?? true) && p.YetkiliUserId == yetkiliUserId)
                .OrderBy(p => p.AdSoyad)
                .ToListAsync();

            ViewBag.ToplamZorunlu = zorunluPersoneller.Count;
            return View(zorunluPersoneller);
        }

        // ✅ ZORUNLU DURUM DEĞİŞTİR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZorunluDurumDegistir(int id, bool zorunluMu)
        {
            var yetkiliUserId = await GetYetkiliUserId();
            var personel = await _context.Personeller.FirstOrDefaultAsync(p => p.Id == id && p.YetkiliUserId == yetkiliUserId);
            if (personel == null)
            {
                TempData["ErrorMessage"] = "Personel bulunamadı!";
                return RedirectToAction(nameof(ZorunluListe));
            }

            personel.ZorunluNobetciMi = zorunluMu;
            await _context.SaveChangesAsync();

            if (zorunluMu)
            {
                TempData["SuccessMessage"] = $"{personel.AdSoyad} zorunlu nöbetçi listesine eklendi.";
            }
            else
            {
                TempData["SuccessMessage"] = $"{personel.AdSoyad} zorunlu nöbetçi listesinden çıkarıldı.";
            }

            return RedirectToAction(nameof(ZorunluListe));
        }

        // ✅ NÖBET YAYINLAMA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YayinlaPersonele(int ay, int yil)
        {
            try
            {
                // İlgili aydaki nöbetleri kontrol et
                var nobetler = await _context.Nobetler
                    .Where(n => n.Tarih.Month == ay && n.Tarih.Year == yil)
                    .ToListAsync();

                if (!nobetler.Any())
                {
                    TempData["ErrorMessage"] = $"{ay}/{yil} dönemine ait nöbet bulunamadı! Önce nöbet dağıtımı yapmalısınız.";
                    return RedirectToAction(nameof(Index), new { ay, yil });
                }

                // Daha önce yayınlanmış mı kontrol et
                var mevcutYayin = await _context.NobetYayinlari
                    .FirstOrDefaultAsync(y => y.Ay == ay && y.Yil == yil && y.YayindaMi);

                if (mevcutYayin != null)
                {
                    TempData["InfoMessage"] = $"{ay}/{yil} dönemi zaten yayında!";
                    return RedirectToAction(nameof(Index), new { ay, yil });
                }

                // Önceki yayını pasif yap (varsa)
                var eskiYayin = await _context.NobetYayinlari
                    .FirstOrDefaultAsync(y => y.Ay == ay && y.Yil == yil);

                if (eskiYayin != null)
                {
                    eskiYayin.YayindaMi = true;
                    eskiYayin.YayinlanmaTarihi = DateTime.Now;
                }
                else
                {
                    // Yeni yayın oluştur
                    var yeniYayin = new NobetYayini
                    {
                        Ay = ay,
                        Yil = yil,
                        YayinlanmaTarihi = DateTime.Now,
                        YayindaMi = true
                    };
                    _context.NobetYayinlari.Add(yeniYayin);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"✅ {ay}/{yil} dönemi nöbetleri başarıyla personele yayınlandı!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Yayınlama hatası: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { ay, yil });
        }

        // ✅ YAYINI GERİ ÇEK
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YayiniGeriCek(int ay, int yil)
        {
            var yayin = await _context.NobetYayinlari
                .FirstOrDefaultAsync(y => y.Ay == ay && y.Yil == yil && y.YayindaMi);

            if (yayin != null)
            {
                yayin.YayindaMi = false;
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = $"{ay}/{yil} dönemi yayından kaldırıldı.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bu dönem zaten yayında değil!";
            }

            return RedirectToAction(nameof(Index), new { ay, yil });
        }

        // ✅ GELİŞMİŞ TASARIMLI: Gazi Hastanesi Profesyonel Nöbet Matrisi (Excel) - GÜN VE TARİH İLE
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(int ay, int yil)
        {
            var yetkiliUser = await _userManager.GetUserAsync(User);
            var yetkiliUserId = yetkiliUser!.Id;
            var personelIds = await _context.Personeller
                .Where(p => p.YetkiliUserId == yetkiliUserId)
                .Select(p => p.Id)
                .ToListAsync();

            var aylar = new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };
            var gunler = new[] { "Paz", "Pzt", "Sal", "Çar", "Per", "Cum", "Cmt" };
            int gunSayisi = DateTime.DaysInMonth(yil, ay);

            var nobetler = await _context.Nobetler
                .Include(n => n.Personel)
                .Where(n => n.Tarih.Month == ay && n.Tarih.Year == yil && personelIds.Contains(n.PersonelId))
                .ToListAsync();

            var personeller = nobetler.Select(n => n.Personel).DistinctBy(p => p.Id).OrderBy(p => p.AdSoyad).ToList();
            var bayramlar = await _context.Bayramlar.ToListAsync();

            var isletmeAdi = yetkiliUser.IsletmeAdi ?? "İŞLETME";

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Nöbet Çizelgesi");

                // --- 1. KURUMSAL ÜST BİLGİ ---
                worksheet.Cell(1, 1).Value = isletmeAdi.ToUpper();
                worksheet.Range(1, 1, 1, gunSayisi + 1).Merge().Style.Font.SetBold().Font.SetFontSize(14).Font.SetFontColor(XLColor.FromHtml("#1a3a5a")).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell(2, 1).Value = $"{aylar[ay - 1]} {yil} AYI NÖBET ÇİZELGESİ";
                worksheet.Range(2, 1, 2, gunSayisi + 1).Merge().Style.Font.SetBold().Font.SetFontSize(11).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // --- 2. MATRİS BAŞLIKLARI (Günler - 2 Satırlı: GÜN ADI ve TARİH) ---
                worksheet.Cell(4, 1).Value = "PERSONEL ADI SOYADI";
                worksheet.Range(4, 1, 5, 1).Merge(); // Personel sütunu 2 satır birleştir
                worksheet.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1a3a5a");
                worksheet.Cell(4, 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(4, 1).Style.Font.SetBold();
                worksheet.Cell(4, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                worksheet.Cell(4, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Günleri ve tarihleri 2 satırda göster
                for (int i = 1; i <= gunSayisi; i++)
                {
                    var tarih = new DateTime(yil, ay, i);
                    string gunAdi = gunler[(int)tarih.DayOfWeek];

                    // Satır 4: Gün adı (Pzt, Sal, vb.)
                    var gunCell = worksheet.Cell(4, i + 1);
                    gunCell.Value = gunAdi;
                    gunCell.Style.Font.SetBold();
                    gunCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    gunCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1a3a5a");
                    gunCell.Style.Font.FontColor = XLColor.White;

                    // Satır 5: Tarih (gün numarası)
                    var tarihCell = worksheet.Cell(5, i + 1);
                    tarihCell.Value = i;
                    tarihCell.Style.Font.SetBold();
                    tarihCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    tarihCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1a3a5a");
                    tarihCell.Style.Font.FontColor = XLColor.White;

                    bool isWeekend = tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday;
                    bool isHoliday = bayramlar.Any(b => tarih.Date >= b.BaslangicTarihi.Date && tarih.Date <= b.BitisTarihi.Date);

                    // Hafta sonu veya bayram günlerini vurgula
                    if (isWeekend || isHoliday)
                    {
                        gunCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff6b6b");
                        tarihCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ff6b6b");
                    }
                }

                // --- 3. VERİLER (Matris Dolumu) ---
                int currentRow = 6; // Başlık 2 satır olduğu için veriler 6. satırdan başlar
                foreach (var personel in personeller)
                {
                    worksheet.Cell(currentRow, 1).Value = personel.AdSoyad;
                    worksheet.Cell(currentRow, 1).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    for (int i = 1; i <= gunSayisi; i++)
                    {
                        var tarih = new DateTime(yil, ay, i);
                        var nobet = nobetler.FirstOrDefault(n => n.PersonelId == personel.Id && n.Tarih.Date == tarih.Date);

                        bool isWeekend = tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday;
                        bool isHoliday = bayramlar.Any(b => tarih.Date >= b.BaslangicTarihi.Date && tarih.Date <= b.BitisTarihi.Date);

                        var cell = worksheet.Cell(currentRow, i + 1);

                        if (nobet != null)
                        {
                            cell.Value = "1";
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Font.SetBold();

                            if (nobet.NobetTipi == "HaftaSonu")
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEB3B");
                            else if (nobet.NobetTipi == "Bayram")
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00BCD4");
                            else
                                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#d9e1f2");
                        }
                        else if (isWeekend || isHoliday)
                        {
                            // Nöbet olmayan hafta sonu/bayram günleri için arka plan rengi
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f2f2f2");
                        }
                    }
                    currentRow++;
                }

                // --- 4. ALT BİLGİ VE KURUMSAL NOTLAR ---
                int footRow = currentRow + 2;
                worksheet.Cell(footRow, 1).Value = "Saat 19:00'a kadar bilgi işlem personeli çalışmakta olup 19:00'dan ertesi gün sabah 08:00'a kadar listede yer alan personel icapçı olacaktır. Hafta sonları ve Resmi tatil günlerinde saat 09:00 dan 12:00'a kadar personel nöbet tutacak bu saatler dışında kalan zamanda aynı listedeki personel icapçı olacaktır.";
                worksheet.Range(footRow, 1, footRow + 2, gunSayisi + 1).Merge().Style.Font.SetItalic().Font.SetFontSize(9).Alignment.SetVertical(XLAlignmentVerticalValues.Top).Alignment.SetWrapText(true);

                int signRow = footRow + 5;
                worksheet.Cell(signRow, 2).Value = "Hazırlayan\nBirim Personeli";
                worksheet.Cell(signRow, gunSayisi - 4).Value = "Onaylayan\nBirim Müdürü";
                worksheet.Range(signRow, 1, signRow, gunSayisi + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // --- 5. GENEL BİÇİMLENDİRME ---
                worksheet.Columns(2, gunSayisi + 1).Width = 3.5;
                worksheet.Column(1).Width = 30;
                worksheet.Row(4).Height = 20; // Gün adı satırı yüksekliği
                worksheet.Row(5).Height = 20; // Tarih satırı yüksekliği

                var tableRange = worksheet.Range(4, 1, currentRow - 1, gunSayisi + 1);
                tableRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                tableRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Gazi_Hastanesi_Nobet_{aylar[ay - 1]}_{yil}.xlsx");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OtomatikDagit(int dagitAy, int dagitYil)
        {
            try
            {
                var yetkiliUserId = await GetYetkiliUserId();
                var hedefTarih = new DateTime(dagitYil, dagitAy, 1);
                await _nobetDagiticisi.OtomatikNobetDagit(hedefTarih, yetkiliUserId);
                TempData["SuccessMessage"] = $"{dagitAy}/{dagitYil} dönemi nöbetleri başarıyla hazırlandı. Önceki aylar korundu!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Dağıtım hatası: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { ay = dagitAy, yil = dagitYil });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SistemiTamamenSifirla()
        {
            var yetkiliUserId = await GetYetkiliUserId();
            var personelIds = await _context.Personeller
                .Where(p => p.YetkiliUserId == yetkiliUserId)
                .Select(p => p.Id)
                .ToListAsync();

            var tumNobetler = await _context.Nobetler
                .Where(n => personelIds.Contains(n.PersonelId))
                .ToListAsync();
            if (tumNobetler.Any())
            {
                _context.Nobetler.RemoveRange(tumNobetler);
            }

            var personeller = await _context.Personeller
                .Where(p => p.YetkiliUserId == yetkiliUserId)
                .ToListAsync();
            foreach (var p in personeller)
            {
                p.ToplamHaftaIci = 0;
                p.ToplamHaftaSonu = 0;
                p.ToplamBayram = 0;
                p.BuAyHaftaIci = 0;
                p.BuAyHaftaSonu = 0;
                p.BuAyBayram = 0;
                p.SonNobetTarihi = null;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Sistem fabrika ayarlarına döndürüldü. Tüm veriler silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var personelIds = await GetYetkiliPersonelIds();
            var nobet = await _context.Nobetler.Include(n => n.Personel).FirstOrDefaultAsync(m => m.Id == id && personelIds.Contains(m.PersonelId));
            if (nobet == null) return NotFound();
            return View(nobet);
        }

        public async Task<IActionResult> Create()
        {
            var yetkiliUserId = await GetYetkiliUserId();
            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.AktifMi == true && p.YetkiliUserId == yetkiliUserId), "Id", "AdSoyad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tarih,PersonelId,NobetTipi")] Nobet nobet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nobet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { ay = nobet.Tarih.Month, yil = nobet.Tarih.Year });
            }
            var yetkiliUserId = await GetYetkiliUserId();
            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.YetkiliUserId == yetkiliUserId), "Id", "AdSoyad", nobet.PersonelId);
            return View(nobet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var nobet = await _context.Nobetler.FindAsync(id);
            if (nobet == null) return NotFound();
            var yetkiliUserId = await GetYetkiliUserId();
            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.AktifMi == true && p.YetkiliUserId == yetkiliUserId), "Id", "AdSoyad", nobet.PersonelId);
            return View(nobet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tarih,PersonelId,NobetTipi")] Nobet nobet)
        {
            if (id != nobet.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(nobet); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!NobetExists(nobet.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index), new { ay = nobet.Tarih.Month, yil = nobet.Tarih.Year });
            }
            var yetkiliUserId = await GetYetkiliUserId();
            ViewData["PersonelId"] = new SelectList(_context.Personeller.Where(p => p.YetkiliUserId == yetkiliUserId), "Id", "AdSoyad", nobet.PersonelId);
            return View(nobet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var personelIds = await GetYetkiliPersonelIds();
            var nobet = await _context.Nobetler.Include(n => n.Personel).FirstOrDefaultAsync(m => m.Id == id && personelIds.Contains(m.PersonelId));
            if (nobet == null) return NotFound();
            return View(nobet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nobet = await _context.Nobetler.FindAsync(id);
            if (nobet == null) return NotFound();
            int ay = nobet.Tarih.Month;
            int yil = nobet.Tarih.Year;
            _context.Nobetler.Remove(nobet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { ay = ay, yil = yil });
        }

        private bool NobetExists(int id) => _context.Nobetler.Any(e => e.Id == id);
    }
}