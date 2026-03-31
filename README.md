# 🏥 Hastane Nöbet Sistemi

Gazi Üniversitesi Sağlık Araştırma ve Uygulama Merkezi (Gazi Hastanesi) Bilgi İşlem Birimi için geliştirilmiş **otomatik nöbet dağıtım ve yönetim sistemi**. ASP.NET Core MVC (.NET 7) altyapısı ile çalışır.

> 🚧 **Proje henüz geliştirme aşamasındadır.** Yeni özellikler eklenmeye devam etmektedir.

---

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Varsayılan Giriş Bilgileri](#-varsayılan-giriş-bilgileri)
- [Proje Yapısı](#-proje-yapısı)
- [Ekran Görüntüleri](#-ekran-görüntüleri)
- [Yapılacaklar](#-yapılacaklar)

---

## ✨ Özellikler

### 🔐 Kimlik Doğrulama & Yetkilendirme
- **ASP.NET Core Identity** tabanlı kullanıcı yönetimi
- **Rol bazlı erişim kontrolü:** `Yetkili` ve `Personel` rolleri
- Otomatik cookie tabanlı oturum yönetimi (8 saat zaman aşımı)
- Varsayılan admin hesabı ile Seed Data desteği

### ⚙️ Otomatik Nöbet Dağıtımı
- Ay bazlı otomatik nöbet dağıtım algoritması
- **Adaletli dağıtım:** En az nöbet tutan personele öncelik
- **7 günlük dinlenme kuralı:** Art arda nöbet yazılmasını engeller
- **Haftalık max 2 nöbet:** 1 hafta içi + 1 hafta sonu limiti
- **Pazartesi döngüsü:** 2 hafta atlayarak Pazartesi nöbeti ataması
- Hafta içi / Hafta sonu / Bayram nöbet tiplerini otomatik algılama
- İzinli personeli nöbetten muaf tutma
- Yedek personel sistemi (son çare olarak devreye girer)

### 👥 Personel Yönetimi (Yetkili Paneli)
- Personel CRUD işlemleri (Ekleme, Düzenleme, Silme)
- Personel ekleme sırasında otomatik kullanıcı hesabı oluşturma
- Toplu hesap oluşturma (eski personeller için)
- Zorunlu nöbetçi listesi yönetimi
- Aktif/Pasif personel takibi
- Nöbet sayaçları (Aylık & Toplam: Hafta İçi, Hafta Sonu, Bayram)

### 📅 Nöbet Çizelgesi & Excel Export
- Aylık nöbet çizelgesi görüntüleme
- **Profesyonel Excel çıktısı** (ClosedXML ile):
  - Kurumsal üst bilgi
  - Matris formatında nöbet gösterimi (Gün adı + Tarih)
  - Hafta sonu ve bayram günleri renk kodlaması
  - Nöbet tiplerine göre renkli hücreler
  - Kurumsal alt bilgi ve imza alanları

### 🔄 Nöbet Takas Sistemi
- Personeller arası nöbet takas teklifi oluşturma
- Hedefli veya herkese açık takas teklifleri
- Karşılık nöbet seçerek takas onaylama
- Takas reddetme ve iptal etme
- Otomatik nöbet değişimi (onay sonrası)

### 🏖️ İzin Yönetimi
- **Personel tarafı:** İzin talebi oluşturma, bekleyen talepleri görme, talep silme
- **Yetkili tarafı:** İzin taleplerini onaylama/reddetme, izin takvimi görüntüleme
- Çakışma kontrolü (aynı tarihlerde çift izin engelleme)
- **Otomatik zorunlu personel ataması:** İzin onaylandığında, izinli personelin nöbetleri zorunlu personellere adil şekilde dağıtılır
- İzin yönetim paneli ile istatistikler (Bekleyen / Onaylanan / Reddedilen)

### 🎉 Bayram Yönetimi
- Resmi tatil ve bayram günlerini tanımlama (başlangıç - bitiş)
- Nöbet dağıtımında bayram günlerini otomatik algılama
- Excel çıktısında bayram günlerini vurgulama

### 📢 Nöbet Yayınlama
- Hazırlanan nöbet çizelgesini personele yayınlama
- Yayını geri çekme
- Personel panelinde sadece yayınlanmış nöbetleri gösterme

### 🏠 Personel Paneli (Dashboard)
- Kişisel nöbet takvimi
- Gelecek ve geçmiş nöbet listesi
- Nöbet geçmişi ve istatistikler
- Bekleyen takas/izin bildirim sayıları
- Yayınlanmış dönemlere göre nöbet görüntüleme

---

## 💻 Teknolojiler

| Teknoloji | Versiyon | Kullanım Alanı |
|---|---|---|
| **.NET** | 7.0 | Uygulama altyapısı |
| **ASP.NET Core MVC** | 7.0 | Web framework |
| **Entity Framework Core** | 7.0.20 | ORM & Veritabanı yönetimi |
| **ASP.NET Core Identity** | 7.0.20 | Kimlik doğrulama & yetkilendirme |
| **SQL Server** | - | Veritabanı |
| **ClosedXML** | 0.105.0 | Excel dosya oluşturma |
| **Bootstrap** | - | Arayüz tasarımı |
| **Razor Views** | - | Sunucu taraflı HTML oluşturma |

---

## 🚀 Kurulum

### Gereksinimler
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB veya Express)
- Visual Studio 2022+ veya VS Code

### Adımlar

1. **Projeyi klonlayın:**
   ```bash
   git clone [https://github.com/Hasretozdemir/Nobet-Sistemi.git](https://github.com/Hasretozdemir/Nobet-Sistemi.git)
   cd Nobet-Sistemi
