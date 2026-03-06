# ?? Hastane Nöbet Sistemi

Gazi Üniversitesi Saðlýk Araþtýrma ve Uygulama Merkezi (Gazi Hastanesi) Bilgi Ýþlem Birimi için geliþtirilmiþ **otomatik nöbet daðýtým ve yönetim sistemi**. ASP.NET Core MVC (.NET 7) altyapýsý ile çalýþýr.

> ?? **Proje henüz geliþtirme aþamasýndadýr.** Yeni özellikler eklenmeye devam etmektedir.

---

## ?? Ýçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Varsayýlan Giriþ Bilgileri](#-varsayýlan-giriþ-bilgileri)
- [Proje Yapýsý](#-proje-yapýsý)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Yapýlacaklar](#-yapýlacaklar)

---

## ? Özellikler

### ?? Kimlik Doðrulama & Yetkilendirme
- **ASP.NET Core Identity** tabanlý kullanýcý yönetimi
- **Rol bazlý eriþim kontrolü:** `Yetkili` ve `Personel` rolleri
- Otomatik cookie tabanlý oturum yönetimi (8 saat zaman aþýmý)
- Varsayýlan admin hesabý ile Seed Data desteði

### ?? Otomatik Nöbet Daðýtýmý
- Ay bazlý otomatik nöbet daðýtým algoritmasý
- **Adaletli daðýtým:** En az nöbet tutan personele öncelik
- **7 günlük dinlenme kuralý:** Art arda nöbet yazýlmasýný engeller
- **Haftalýk max 2 nöbet:** 1 hafta içi + 1 hafta sonu limiti
- **Pazartesi döngüsü:** 2 hafta atlayarak Pazartesi nöbeti atamasý
- Hafta içi / Hafta sonu / Bayram nöbet tiplerini otomatik algýlama
- Ýzinli personeli nöbetten muaf tutma
- Yedek personel sistemi (son çare olarak devreye girer)

### ?? Personel Yönetimi (Yetkili Paneli)
- Personel CRUD iþlemleri (Ekleme, Düzenleme, Silme)
- Personel ekleme sýrasýnda otomatik kullanýcý hesabý oluþturma
- Toplu hesap oluþturma (eski personeller için)
- Zorunlu nöbetçi listesi yönetimi
- Aktif/Pasif personel takibi
- Nöbet sayaçlarý (Aylýk & Toplam: Hafta Ýçi, Hafta Sonu, Bayram)

### ?? Nöbet Çizelgesi & Excel Export
- Aylýk nöbet çizelgesi görüntüleme
- **Profesyonel Excel çýktýsý** (ClosedXML ile):
  - Kurumsal üst bilgi
  - Matris formatýnda nöbet gösterimi (Gün adý + Tarih)
  - Hafta sonu ve bayram günleri renk kodlamasý
  - Nöbet tiplerine göre renkli hücreler
  - Kurumsal alt bilgi ve imza alanlarý

### ?? Nöbet Takas Sistemi
- Personeller arasý nöbet takas teklifi oluþturma
- Hedefli veya herkese açýk takas teklifleri
- Karþýlýk nöbet seçerek takas onaylama
- Takas reddetme ve iptal etme
- Otomatik nöbet deðiþimi (onay sonrasý)

### ?? Ýzin Yönetimi
- **Personel tarafý:** Ýzin talebi oluþturma, bekleyen talepleri görme, talep silme
- **Yetkili tarafý:** Ýzin taleplerini onaylama/reddetme, izin takvimi görüntüleme
- Çakýþma kontrolü (ayný tarihlerde çift izin engelleme)
- **Otomatik zorunlu personel atamasý:** Ýzin onaylandýðýnda, izinli personelin nöbetleri zorunlu personellere adil þekilde daðýtýlýr
- Ýzin yönetim paneli ile istatistikler (Bekleyen / Onaylanan / Reddedilen)

### ?? Bayram Yönetimi
- Resmi tatil ve bayram günlerini tanýmlama (baþlangýç - bitiþ)
- Nöbet daðýtýmýnda bayram günlerini otomatik algýlama
- Excel çýktýsýnda bayram günlerini vurgulama

### ?? Nöbet Yayýnlama
- Hazýrlanan nöbet çizelgesini personele yayýnlama
- Yayýný geri çekme
- Personel panelinde sadece yayýnlanmýþ nöbetleri gösterme

### ?? Personel Paneli (Dashboard)
- Kiþisel nöbet takvimi
- Gelecek ve geçmiþ nöbet listesi
- Nöbet geçmiþi ve istatistikler
- Bekleyen takas/izin bildirim sayýlarý
- Yayýnlanmýþ dönemlere göre nöbet görüntüleme

---

## ?? Teknolojiler

| Teknoloji | Versiyon | Kullaným Alaný |
|---|---|---|
| **.NET** | 7.0 | Uygulama altyapýsý |
| **ASP.NET Core MVC** | 7.0 | Web framework |
| **Entity Framework Core** | 7.0.20 | ORM & Veritabaný yönetimi |
| **ASP.NET Core Identity** | 7.0.20 | Kimlik doðrulama & yetkilendirme |
| **SQL Server** | - | Veritabaný |
| **ClosedXML** | 0.105.0 | Excel dosya oluþturma |
| **Bootstrap** | - | Arayüz tasarýmý |
| **Razor Views** | - | Sunucu taraflý HTML oluþturma |

---

## ?? Kurulum

### Gereksinimler
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB veya Express)
- Visual Studio 2022+ veya VS Code

### Adýmlar

1. **Projeyi klonlayýn:**
   ```bash
   git clone https://github.com/Hasretozdemir/Nobet-Sistemi.git
   cd Nobet-Sistemi
   ```

2. **Veritabaný baðlantý dizesini ayarlayýn:**

   `HastaneNobetSistemi/appsettings.json` dosyasýndaki `DefaultConnection` baðlantý dizesini kendi ortamýnýza göre düzenleyin:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HastaneNobetDb;Trusted_Connection=True;"
   }
   ```

3. **Veritabanýný oluþturun (Migration):**
   ```bash
   cd HastaneNobetSistemi
   dotnet ef database update
   ```

4. **Projeyi çalýþtýrýn:**
   ```bash
   dotnet run
   ```

5. Tarayýcýda `https://localhost:5001` adresine gidin.

---

## ?? Varsayýlan Giriþ Bilgileri

| Rol | E-posta | Þifre |
|---|---|---|
| **Yetkili (Admin)** | `admin@hastane.com` | `Admin123` |
| **Personel** | Yetkili tarafýndan oluþturulur | Yetkili tarafýndan belirlenir |

> Toplu hesap oluþturma ile eklenen personellerin varsayýlan þifresi: `Personel123!`

---

## ?? Proje Yapýsý

```
HastaneNobetSistemi/
??? Controllers/
?   ??? AccountController.cs        # Giriþ/Çýkýþ/Yetkilendirme
?   ??? NobetsController.cs         # Nöbet yönetimi (Yetkili)
?   ??? PersonelsController.cs      # Personel CRUD (Yetkili)
?   ??? PersonelController.cs       # Personel paneli (Dashboard)
?   ??? NobetTakasController.cs     # Nöbet takas sistemi
?   ??? IzinController.cs           # Ýzin talep/onay sistemi
?   ??? BayramsController.cs        # Bayram/tatil yönetimi
?   ??? HomeController.cs           # Ana sayfa
??? Models/
?   ??? AppUser.cs                  # Identity kullanýcý modeli
?   ??? Personel.cs                 # Personel modeli
?   ??? Nobet.cs                    # Nöbet modeli
?   ??? NobetTakas.cs               # Takas modeli
?   ??? NobetYayini.cs              # Yayýn durumu modeli
?   ??? IzinTalebi.cs               # Ýzin talebi modeli
?   ??? Bayram.cs                   # Bayram modeli
??? ViewModels/
?   ??? LoginViewModel.cs           # Giriþ formu
?   ??? YetkiliLoginViewModel.cs    # Yetkili giriþ formu
?   ??? PersonelKayitViewModel.cs   # Personel kayýt formu
??? Views/                          # Razor View dosyalarý
??? Services/
?   ??? NobetDagiticisi.cs          # Otomatik nöbet daðýtým algoritmasý
??? Data/
?   ??? AppDbContext.cs             # EF Core veritabaný baðlamý
?   ??? SeedData.cs                 # Baþlangýç verileri (Admin kullanýcý & roller)
??? Migrations/                     # EF Core migration dosyalarý
??? Program.cs                      # Uygulama giriþ noktasý & servis konfigürasyonu
```

---

## ?? Ekran Görüntüleri

> ?? Ekran görüntüleri yakýnda eklenecektir.

---

## ?? Yapýlacaklar

- [ ] Bildirim sistemi (nöbet deðiþikliði, takas sonucu vb.)
- [ ] Raporlama ve istatistik sayfalarý
- [ ] Personel bazlý detaylý nöbet analizi
- [ ] Responsive mobil uyumluluk iyileþtirmeleri
- [ ] Ekran görüntüleri eklenmesi
- [ ] Birim bazlý personel filtreleme
- [ ] Sistem loglarý ve denetim kaydý

---

## ????? Geliþtirici

Bu proje **Gazi Üniversitesi** 2. sýnýf ders projesi kapsamýnda geliþtirilmektedir.

---

## ?? Lisans

Bu proje eðitim amaçlý geliþtirilmiþtir.
