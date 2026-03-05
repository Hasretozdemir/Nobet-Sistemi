using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Identity;

namespace HastaneNobetSistemi.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        // Rolleri oluştur
        string[] roleNames = { "Yetkili", "Personel" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Varsayılan Yetkili kullanıcı
        var adminEmail = "admin@hastane.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                AdSoyad = "Sistem Yöneticisi",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123");
            await userManager.AddToRoleAsync(adminUser, "Yetkili");
        }
    }
}