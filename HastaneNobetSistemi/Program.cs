using DocumentFormat.OpenXml.Spreadsheet;
using HastaneNobetSistemi.Data;
using HastaneNobetSistemi.Models;
using HastaneNobetSistemi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC (Model-View-Controller) Servislerini Ekle
builder.Services.AddControllersWithViews();

// 2. Veritabaný Baðlantýsýný Tanýmla (appsettings.json'dan DefaultConnection'ý çeker)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? 3. IDENTITY SÝSTEMÝ (Kullanýcý Giriþi ve Yetkilendirme)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Þifre politikalarý (Test için basitleþtirilmiþ - Canlý ortamda güçlendirilmeli)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Hesap kilitleme ayarlarý
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Kullanýcý ayarlarý
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ? 4. COOKIE AYARLARI (Giriþ/Çýkýþ Yönetimi)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriþ yapmamýþ kullanýcý buraya yönlendirilir
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetki yoksa buraya gider
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 saat sonra otomatik çýkýþ
    options.SlidingExpiration = true; // Kullanýcý aktifse süre uzar
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 5. KRÝTÝK SERVÝSLER (Dependency Injection)
builder.Services.AddScoped<NobetDagiticisi>();

var app = builder.Build();

// ? 6. ÝLK KULLANICI VE ROLLERÝ OLUÞTUR (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data oluþturulurken hata oluþtu.");
    }
}

// 7. Hata Yönetimi ve Güvenlik Ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 8. Middleware (Ara Katman) Ayarlarý
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ? 9. KÝMLÝK DOÐRULAMA VE YETKÝLENDÝRME (Sýralama Önemli!)
app.UseAuthentication(); // Kullanýcý kimliðini doðrular
app.UseAuthorization();  // Yetki kontrolü yapar

// ? 10. ROTA YÖNLENDÝRMESÝ (Varsayýlan olarak Login sayfasýna gider)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();