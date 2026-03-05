using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Services;

public class NobetDagiticisi
{
    private readonly AppDbContext _context;

    public NobetDagiticisi(AppDbContext context)
    {
        _context = context;
    }

    public async Task OtomatikNobetDagit(DateTime hedefAy)
    {
        // Aktif personelleri getir (HERKESI dahil et)
        var personeller = await _context.Personeller
            .Where(p => (p.AktifMi ?? true) == true)
            .ToListAsync();

        if (!personeller.Any())
            throw new Exception("Aktif personel yok.");

        var ayBas = new DateTime(hedefAy.Year, hedefAy.Month, 1);
        var aySon = ayBas.AddMonths(1).AddDays(-1);

        // ✅ SADECE HEDEF AYI SİL (Geçmiş aylara dokunmaz)
        var hedefAyNobetleri = await _context.Nobetler
            .Where(n => n.Tarih >= ayBas && n.Tarih <= aySon)
            .ToListAsync();

        if (hedefAyNobetleri.Any())
        {
            _context.Nobetler.RemoveRange(hedefAyNobetleri);
        }

        // ✅ AYLIK SAYAÇLARI SIFIRLA (Genel toplamlara dokunma)
        foreach (var p in personeller)
        {
            p.BuAyHaftaIci = 0;
            p.BuAyHaftaSonu = 0;
            p.BuAyBayram = 0;
        }

        var bayramGunleri = await GetBayramDates(hedefAy);
        var yeniNobetler = new List<Nobet>();
        int gunSayisi = DateTime.DaysInMonth(hedefAy.Year, hedefAy.Month);

        for (int gun = 1; gun <= gunSayisi; gun++)
        {
            var tarih = new DateTime(hedefAy.Year, hedefAy.Month, gun);
            string tip = GetNobetTipi(tarih, bayramGunleri);

            // 1. ADIM: TÜM AKTİF PERSONELDEN uygun olanı seç (Zorunlu + Normal)
            var secilen = SecPersonel(personeller, tarih, tip, yeniNobetler);

            // 2. ADIM: Eğer normal seçimde kimse bulunamadıysa ZORUNLU LİSTEDEN zorla seç
            if (secilen == null)
            {
                secilen = SecZorunluPersonel(personeller, tarih, tip, yeniNobetler);
            }

            // 3. ADIM: Hala kimse yoksa yedeklere bak
            if (secilen == null)
            {
                secilen = GetYedekPersonel(personeller, tarih);
            }

            if (secilen != null)
            {
                secilen.SonNobetTarihi = tarih;
                UpdatePersonelCounters(secilen, tip);
                yeniNobetler.Add(new Nobet
                {
                    Tarih = tarih,
                    PersonelId = secilen.Id,
                    NobetTipi = tip
                });
            }
        }

        await _context.Nobetler.AddRangeAsync(yeniNobetler);
        _context.Personeller.UpdateRange(personeller);
        await _context.SaveChangesAsync();
    }

    private string GetNobetTipi(DateTime tarih, List<DateTime> bayramlar)
    {
        if (bayramlar.Contains(tarih.Date)) return "Bayram";
        if (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday) return "HaftaSonu";
        return "HaftaIci";
    }

    private void UpdatePersonelCounters(Personel p, string tip)
    {
        switch (tip)
        {
            case "HaftaIci": p.BuAyHaftaIci++; p.ToplamHaftaIci++; break;
            case "HaftaSonu": p.BuAyHaftaSonu++; p.ToplamHaftaSonu++; break;
            case "Bayram": p.BuAyBayram++; p.ToplamBayram++; break;
        }
    }

    private Personel? SecPersonel(List<Personel> personeller, DateTime tarih, string tip, List<Nobet> mevcutAyNobetleri)
    {
        // İzinli olmayan, işe girmiş TÜM AKTİF PERSONELLER (zorunlu dahil)
        var musaitler = personeller
            .Where(p => p.IseGirisTarihi.Date <= tarih.Date)
            .Where(p => !IsIzinli(p, tarih))
            .ToList();

        if (!musaitler.Any()) return null;

        // ✅ KURAL 1: 7 GÜNLÜK DİNLENME KURALI
        var dinlenmisOlanlar = musaitler
            .Where(p => p.SonNobetTarihi == null || (tarih.Date - p.SonNobetTarihi.Value.Date).TotalDays >= 7)
            .ToList();

        var hedefListe = dinlenmisOlanlar.Any() ? dinlenmisOlanlar : musaitler;

        // ✅ KURAL 2: HAFTALIK MAX 2 NÖBET (1 hafta içi + 1 hafta sonu)
        hedefListe = hedefListe
            .Where(p => GetHaftalikNobetSayisi(p.Id, tarih, mevcutAyNobetleri) < 2)
            .ToList();

        if (!hedefListe.Any()) hedefListe = musaitler; // Zorunlu durumda kural esnet

        // ✅ KURAL 3: PAZARTESİ DÖNGÜSÜ (2 hafta atlayarak)
        if (tip == "HaftaIci" && tarih.DayOfWeek == DayOfWeek.Monday)
        {
            var pazartesiUygunlar = hedefListe
                .Where(p => PazartesiUygunMu(p, tarih, mevcutAyNobetleri))
                .ToList();

            if (pazartesiUygunlar.Any())
                hedefListe = pazartesiUygunlar;
        }

        // ✅ ADALETLİ DAĞITIM: Önce bu ay az tutan, sonra toplamda az tutan
        return tip switch
        {
            "HaftaIci" => hedefListe
                .OrderBy(p => p.BuAyHaftaIci)
                .ThenBy(p => p.ToplamHaftaIci)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            "HaftaSonu" => hedefListe
                .OrderBy(p => p.BuAyHaftaSonu)
                .ThenBy(p => p.ToplamHaftaSonu)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            "Bayram" => hedefListe
                .OrderBy(p => p.BuAyBayram)
                .ThenBy(p => p.ToplamBayram)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            _ => null
        };
    }

    // ✅ ZORUNLU LİSTEDEN SEÇİM (İzinli durumda veya acil durumda kullanılır)
    private Personel? SecZorunluPersonel(List<Personel> personeller, DateTime tarih, string tip, List<Nobet> mevcutAyNobetleri)
    {
        // Sadece zorunlu nöbetçilerden izinli olmayanları seç
        var zorunluMusaitler = personeller
            .Where(p => p.ZorunluNobetciMi)
            .Where(p => p.IseGirisTarihi.Date <= tarih.Date)
            .Where(p => !IsIzinli(p, tarih))
            .ToList();

        if (!zorunluMusaitler.Any()) return null;

        // Zorunlu personeller için de kurallara uygun seçim yap
        var dinlenmisler = zorunluMusaitler
            .Where(p => p.SonNobetTarihi == null || (tarih.Date - p.SonNobetTarihi.Value.Date).TotalDays >= 3)
            .ToList();

        var hedefListe = dinlenmisler.Any() ? dinlenmisler : zorunluMusaitler;

        // Adaletli dağıtım: En az nöbet tutanı seç
        return tip switch
        {
            "HaftaIci" => hedefListe
                .OrderBy(p => p.BuAyHaftaIci)
                .ThenBy(p => p.ToplamHaftaIci)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            "HaftaSonu" => hedefListe
                .OrderBy(p => p.BuAyHaftaSonu)
                .ThenBy(p => p.ToplamHaftaSonu)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            "Bayram" => hedefListe
                .OrderBy(p => p.BuAyBayram)
                .ThenBy(p => p.ToplamBayram)
                .ThenBy(p => p.SonNobetTarihi)
                .FirstOrDefault(),
            _ => null
        };
    }

    // ✅ Haftalık nöbet sayısını hesapla
    private int GetHaftalikNobetSayisi(int personelId, DateTime tarih, List<Nobet> mevcutAyNobetleri)
    {
        // Haftanın başlangıcını bul (Pazartesi)
        var haftaBaslangic = tarih.AddDays(-(int)tarih.DayOfWeek + (int)DayOfWeek.Monday);
        if (tarih.DayOfWeek == DayOfWeek.Sunday) haftaBaslangic = haftaBaslangic.AddDays(-7);

        return mevcutAyNobetleri
            .Count(n => n.PersonelId == personelId && n.Tarih >= haftaBaslangic && n.Tarih <= tarih);
    }

    // ✅ Pazartesi kontrolü: Son 14 gün içinde Pazartesi tuttuysa verme
    private bool PazartesiUygunMu(Personel p, DateTime tarih, List<Nobet> mevcutAyNobetleri)
    {
        var sonPazartesiNobet = mevcutAyNobetleri
            .Where(n => n.PersonelId == p.Id && n.Tarih.DayOfWeek == DayOfWeek.Monday)
            .OrderByDescending(n => n.Tarih)
            .FirstOrDefault();

        // Eğer önceki aylarda Pazartesi tutmuşsa onu da kontrol et
        if (sonPazartesiNobet == null && p.SonNobetTarihi.HasValue && p.SonNobetTarihi.Value.DayOfWeek == DayOfWeek.Monday)
        {
            return (tarih.Date - p.SonNobetTarihi.Value.Date).TotalDays >= 14;
        }

        return sonPazartesiNobet == null || (tarih - sonPazartesiNobet.Tarih).TotalDays >= 14;
    }

    // ✅ Yedek Personel Seçimi (Son Çare)
    private Personel? GetYedekPersonel(List<Personel> tumPersoneller, DateTime tarih)
    {
        // Yedek personellerden izinli olmayanları seç
        var yedekler = tumPersoneller
            .Where(p => p.YedekMi && !IsIzinli(p, tarih))
            .OrderBy(p => p.SonNobetTarihi)
            .ThenBy(p => p.ToplamHaftaIci + p.ToplamHaftaSonu + p.ToplamBayram)
            .ToList();

        if (yedekler.Any())
            return yedekler.First();

        // Hiçbir yedek yoksa en az nöbet tutan aktif personeli seç (İZİNLİ OLSA BİLE)
        return tumPersoneller
            .OrderBy(p => p.ToplamHaftaIci + p.ToplamHaftaSonu + p.ToplamBayram)
            .ThenBy(p => p.SonNobetTarihi)
            .FirstOrDefault();
    }

    private bool IsIzinli(Personel p, DateTime tarih)
    {
        return p.IzinBaslangic.HasValue && p.IzinBitis.HasValue &&
               tarih.Date >= p.IzinBaslangic.Value.Date && tarih.Date <= p.IzinBitis.Value.Date;
    }

    private async Task<List<DateTime>> GetBayramDates(DateTime hedefAy)
    {
        var bayramlar = await _context.Bayramlar.ToListAsync();
        var liste = new List<DateTime>();

        foreach (var b in bayramlar)
        {
            for (var dt = b.BaslangicTarihi.Date; dt <= b.BitisTarihi.Date; dt = dt.AddDays(1))
            {
                if (dt.Month == hedefAy.Month && dt.Year == hedefAy.Year)
                {
                    liste.Add(dt.Date);
                }
            }
        }
        return liste.Distinct().ToList();
    }
}