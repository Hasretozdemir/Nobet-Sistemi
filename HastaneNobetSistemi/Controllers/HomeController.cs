using System.Diagnostics;
using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneNobetSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Yetkili")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Iletisim()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Iletisim(string Konu, string Kategori, string Mesaj)
        {
            if (string.IsNullOrWhiteSpace(Konu) || string.IsNullOrWhiteSpace(Mesaj))
            {
                TempData["ErrorMessage"] = "Konu ve mesaj alanları zorunludur.";
                return View();
            }

            var gonderen = User.Identity?.Name ?? "Bilinmeyen";
            var mailKonu = $"[Nöbet Sistemi - {Kategori}] {Konu}";
            var mailBody = $"Gönderen: {gonderen}\nKategori: {Kategori}\nKonu: {Konu}\n\n{Mesaj}";

            // mailto linki ile kullanıcının mail uygulamasını aç
            var mailtoLink = $"mailto:hasretozdemir288@gmail.com?subject={Uri.EscapeDataString(mailKonu)}&body={Uri.EscapeDataString(mailBody)}";

            TempData["MailtoLink"] = mailtoLink;
            TempData["SuccessMessage"] = "Mesajınız hazırlandı! Mail uygulamanız açılacaktır.";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}