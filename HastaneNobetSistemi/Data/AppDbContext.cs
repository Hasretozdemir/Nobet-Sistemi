using HastaneNobetSistemi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HastaneNobetSistemi.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Personel> Personeller { get; set; }
    public DbSet<Nobet> Nobetler { get; set; }
    public DbSet<Bayram> Bayramlar { get; set; }
    public DbSet<IzinTalebi> IzinTalepleri { get; set; }
    public DbSet<NobetYayini> NobetYayinlari { get; set; }
    public DbSet<NobetTakas> NobetTakaslar { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NobetUcreti decimal hassasiyeti
        modelBuilder.Entity<Personel>()
            .Property(p => p.NobetUcreti)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Personel>()
            .Property(p => p.IcapSaatlikUcret)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Personel>()
            .Property(p => p.IcapSaat)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Personel>()
            .Property(p => p.UzaktanSaatlikUcret)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Personel>()
            .Property(p => p.UzaktanSaat)
            .HasPrecision(18, 2);

        // Personel - Yetkili ilişkisi
        modelBuilder.Entity<Personel>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(p => p.YetkiliUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // AppUser - Personel ilişkisi
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Personel)
            .WithMany()
            .HasForeignKey(u => u.PersonelId)
            .OnDelete(DeleteBehavior.SetNull);

        // ✅ NobetTakas - Personel ilişkileri (TÜM NO ACTION)
        modelBuilder.Entity<NobetTakas>()
            .HasOne(t => t.TeklifEdenPersonel)
            .WithMany()
            .HasForeignKey(t => t.TeklifEdenPersonelId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NobetTakas>()
            .HasOne(t => t.HedefPersonel)
            .WithMany()
            .HasForeignKey(t => t.HedefPersonelId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NobetTakas>()
            .HasOne(t => t.KabulEdenPersonel)
            .WithMany()
            .HasForeignKey(t => t.KabulEdenPersonelId)
            .OnDelete(DeleteBehavior.NoAction);

        // ✅ NobetTakas - Nobet ilişkileri (TÜM NO ACTION)
        modelBuilder.Entity<NobetTakas>()
            .HasOne(t => t.TeklifEdilenNobet)
            .WithMany()
            .HasForeignKey(t => t.TeklifEdilenNobetId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NobetTakas>()
            .HasOne(t => t.KarsilikNobet)
            .WithMany()
            .HasForeignKey(t => t.KarsilikNobetId)
            .OnDelete(DeleteBehavior.NoAction); // ✅ SetNull yerine NoAction


    }
}