# 🏥 Hastane Nöbet Dağıtım ve Yönetim Sistemi

Bu proje, hastanelerdeki personelin nöbet yazım işlemlerini, izin taleplerini, nöbet takaslarını ve ödeme süreçlerini adil ve otomatik bir şekilde yönetmek amacıyla geliştirilmiş kapsamlı bir **ASP.NET Core MVC** web uygulamasıdır.

## 🚀 Öne Çıkan Özellikler

* **Otomatik Nöbet Dağıtımı:** Gelişmiş adalet sistemi (`NobetDagiticisi`) sayesinde personelin kıdemi, daha önceki nöbet sayısı ve izin günleri dikkate alınarak adil nöbet yazımı.
* **Rol Tabanlı Erişim Yönetimi:** Yetkili (Admin) ve Personel (Kullanıcı) olmak üzere iki farklı rol yapısı.
* **Nöbet Takas Sistemi:** Personelin kendi aralarında güvenli ve onay mekanizmalı nöbet takası yapabilmesi.
* **İzin Yönetimi:** Personelin izin talebi oluşturabilmesi, yetkililerin bu talepleri onaylayıp/reddedebilmesi. Takvim üzerinden görsel takip.
* **Bayram ve Özel Gün Yönetimi:** Resmi tatillerin sisteme işlenerek bu günlerde tutulan nöbetlerin katsayılarının/ücretlerinin farklı hesaplanması.
* **Ücretlendirme Sistemi:** Nöbet türüne (İcap, Uzaktan vb.) ve birime göre otomatik nöbet ücreti ve ek ödeme hesaplamaları.

## 💻 Kullanılan Teknolojiler

* **Backend:** C#, ASP.NET Core MVC 8.0 (veya ilgili sürüm)
* **Veritabanı ve ORM:** Entity Framework Core, SQL Server
* **Kimlik Doğrulama:** ASP.NET Core Identity
* **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery
* **Mimari:** Katmanlı Mimari Prensibine Uygun Klasörleme

---

## 📸 Ekran Görüntüleri

Aşağıda projenin kullanıcı ve yönetici arayüzlerine ait bazı ekran görüntüleri bulunmaktadır:

### Sisteme Giriş
Kullanıcıların ve yetkililerin sisteme güvenli giriş yaptığı ekran.
![Giriş Ekranı](HastaneNobetSistemi/screenshots/login.png)

### 👨‍⚕️ Yetkili (Admin) Paneli
Yetkililerin nöbetleri dağıttığı, personelleri yönettiği ve izin taleplerini incelediği ana ekranlar.

<details>
  <summary>Yetkili Paneli Görsellerini Göster / Gizle</summary>
  
  **Ana Dashboard:**
  ![Yetkili Dashboard](HastaneNobetSistemi/screenshots/yetkili.png)
  
  **Personel Yönetimi ve Diğer Ekranlar:**
  ![Yetkili Ekranı 1](HastaneNobetSistemi/screenshots/yetkili1.png)
  ![Yetkili Ekranı 2](HastaneNobetSistemi/screenshots/yetkili2.png)
  ![Yetkili Ekranı 3](HastaneNobetSistemi/screenshots/yetkili3.png)
  ![Yetkili Ekranı 4](HastaneNobetSistemi/screenshots/yetkili4.png)
  ![Yetkili Ekranı 5](HastaneNobetSistemi/screenshots/yetkili5.png)
  ![Yetkili Ekranı 6](HastaneNobetSistemi/screenshots/yetkili6.png)
  ![Yetkili Ekranı 7](HastaneNobetSistemi/screenshots/yetkili7.png)
  ![Yetkili Ekranı 8](HastaneNobetSistemi/screenshots/yetkili8.png)
  ![Yetkili Ekranı 9](HastaneNobetSistemi/screenshots/yetkili9.png)
  ![Yetkili Ekranı 10](HastaneNobetSistemi/screenshots/yetkili10.png)
  ![Yetkili Ekranı 11](HastaneNobetSistemi/screenshots/yetkili11.png)
  ![Yetkili Ekranı 12](HastaneNobetSistemi/screenshots/yetkili12.png)
  ![Yetkili Ekranı 13](HastaneNobetSistemi/screenshots/yetkili13.png)
  ![Yetkili Ekranı 14](HastaneNobetSistemi/screenshots/yetkili14.png)
  ![Yetkili Ekranı 15](HastaneNobetSistemi/screenshots/yetkili15.png)
</details>

### 🧑‍💼 Personel Paneli
Personelin kendi nöbetlerini gördüğü, izin talep ettiği ve nöbet takas işlemlerini gerçekleştirdiği ekranlar.

<details>
  <summary>Personel Paneli Görsellerini Göster / Gizle</summary>
  
  **Ana Dashboard:**
  ![Personel Dashboard](HastaneNobetSistemi/screenshots/personel.png)
  
  **Nöbet Listem, Takas ve İzin Ekranları:**
  ![Personel Ekranı 2](HastaneNobetSistemi/screenshots/personel2.png)
  ![Personel Ekranı 3](HastaneNobetSistemi/screenshots/personel3.png)
  ![Personel Ekranı 4](HastaneNobetSistemi/screenshots/personel4.png)
  ![Personel Ekranı 5](HastaneNobetSistemi/screenshots/personel5.png)
  ![Personel Ekranı 6](HastaneNobetSistemi/screenshots/personel6.png)
  ![Personel Ekranı 7](HastaneNobetSistemi/screenshots/personel7.png)
  ![Personel Ekranı 8](HastaneNobetSistemi/screenshots/personel8.png)
  ![Personel Ekranı 9](HastaneNobetSistemi/screenshots/personel9.png)
  ![Personel Ekranı 10](HastaneNobetSistemi/screenshots/personel10.png)
  ![Personel Ekranı 11](HastaneNobetSistemi/screenshots/personel11.png)
</details>

---

## ⚙️ Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyebilirsiniz:

1. **Projeyi Klonlayın:**
   ```bash
   git clone [https://github.com/hasretozdemir/nobet-sistemi.git](https://github.com/hasretozdemir/nobet-sistemi.git)
