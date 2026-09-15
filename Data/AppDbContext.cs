using Microsoft.EntityFrameworkCore;
using appOne.Models;

namespace appOne.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<SubMenu> SubMenus => Set<SubMenu>();
    public DbSet<AccessMenu> AccessMenus => Set<AccessMenu>();   

    public DbSet<AccessSubMenu> AccessSubMenus => Set<AccessSubMenu>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.KodeRole)
            .IsUnique();

        modelBuilder.Entity<SubMenu>()
            .HasOne(s => s.Menu)
            .WithMany()
            .HasForeignKey(s => s.IdMenu)
            .OnDelete(DeleteBehavior.Restrict);

        // tambahan baru: relasi AccessMenu ke Role dan Menu
        modelBuilder.Entity<AccessMenu>()
            .HasOne(a => a.Role)
            .WithMany()
            .HasForeignKey(a => a.IdRole)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccessMenu>()
            .HasOne(a => a.Menu)
            .WithMany()
            .HasForeignKey(a => a.IdMenu)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccessSubMenu>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccessSubMenu>()
            .HasOne(a => a.SubMenu)
            .WithMany()
            .HasForeignKey(a => a.IdSubMenu)
            .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.IdUser)
                .OnDelete(DeleteBehavior.Cascade); // kalau user dihapus, refresh token-nya ikut terhapus

                modelBuilder.Entity<RefreshToken>()
                    .HasIndex(rt => rt.Token)
                    .IsUnique();   
    }
}