# 🏥 Hastane Nöbet Sistemi (ASP.NET Core MVC)

**Modern, Adaletli ve Kullanıcı Dostu Hastane Nöbet Dağıtım ve Takas Sistemi**

Hastanelerde görev yapan sağlık personellerinin (doktor, hemşire vb.) nöbet listelerinin adil bir şekilde dağıtılmasını, personeller arası nöbet takaslarını ve izin süreçlerini dijitalleştiren, web tabanlı kurumsal bir otomasyon sistemidir.

---

## 🎯 Proje Nedir?

Bu sistem, hastanelerin ilgili birimlerindeki karmaşık nöbet yazım süreçlerini optimize eden bir çözümdür. Yetkililer tüm personelin nöbetlerini, bayram günlerini ve icap/uzaktan çalışma durumlarını yönetirken; personeller kendi nöbetlerini takip edebilir, takas talebinde bulunabilir ve izinlerini planlayabilir.

### 🔍 Temel İşleyiş:
1. **Yetkili** → Sisteme personel ekler, bayram/tatil günlerini belirler ve adaletli nöbet dağıtımını gerçekleştirir.
2. **Sistem** → Nöbetleri personelin geçmiş nöbet yüküne, izin durumuna ve yedeklik durumuna göre hesaplayarak atar.
3. **Personel** → Kendi paneline girerek nöbet listesini görüntüler.
4. **Personel** → Gerekli durumlarda başka bir personel ile nöbet takas talebi oluşturur.
5. **Yetkili** → Takas ve izin taleplerini onaylar veya reddeder.

---

## ✨ Temel Özellikler

### 👥 Personel Özellikleri
- ✅ **Kişisel Dashboard**: Kendi nöbet takvimini ve yaklaşan nöbetleri görüntüleme.
- ✅ **Nöbet Takas Sistemi**: Diğer personellerle nöbet değişimi talep etme ve gelen talepleri yanıtlama.
- ✅ **İzin Yönetimi**: Yıllık veya mazeret izni talebi oluşturma ve durumunu takip etme.
- ✅ **Nöbet Geçmişi**: Önceki aylara ait nöbetlerin ve icap/uzaktan çalışma durumlarının dökümü.

### 🎛️ Yetkili (Admin) Paneli Özellikleri
- ✅ **Gelişmiş Dağıtım Algoritması**: Adalet sistemine dayalı, personelin çalışma yükünü dengeleyen otomatik nöbet dağıtıcı (`NobetDagiticisi.cs`).
- ✅ **Birim ve Sicil Yönetimi**: Hastane içi birimleri ve personelleri sisteme tanımlama.
- ✅ **Takas ve İzin Onayı**: Personeller arası nöbet takaslarını ve izin taleplerini inceleyip karara bağlama.
- ✅ **Ücret ve İcap Takibi**: Nöbet ücretleri, icap (nöbete çağrılma) ve uzaktan çalışma durumlarının hesaplanması.
- ✅ **Bayram ve Özel Gün Yönetimi**: Tatil günlerinin sisteme girilerek nöbet zorluk derecelerinin ayarlanması.

### 🎨 Tasarım Özellikleri
- ✅ **Responsive Tasarım**: Mobil, tablet, masaüstü tam uyumlu modern arayüz.
- ✅ **Bootstrap Grid Sistemi**: Temiz ve düzenli sayfa yerleşimleri.
- ✅ **Dinamik Tablolar**: Verilerin anlık ve düzenli bir şekilde listelenmesi.

### 🔒 Güvenlik Özellikleri
- ✅ **ASP.NET Core Identity**: Güvenli rol (Yetkili/Personel) ve kullanıcı yönetimi.
- ✅ **Şifreleme**: Parolaların veritabanında güvenli bir şekilde hashlenerek saklanması.
- ✅ **Yetkilendirme (Authorization)**: Sayfalara ve işlemlere sadece yetkili rollerin erişebilmesi.
- ✅ **Entity Framework Core**: SQL Injection saldırılarına karşı güvenli ORM kullanımı ve ilişkisel veri bütünlüğü.

---

## 📋 Sistem Gereksinimleri

- **.NET SDK**: .NET 8.0 (veya güncel sürüm)
- **Veritabanı**: SQL Server (LocalDB veya SQL Server Express/Developer)
- **IDE**: Visual Studio 2022 veya JetBrains Rider
