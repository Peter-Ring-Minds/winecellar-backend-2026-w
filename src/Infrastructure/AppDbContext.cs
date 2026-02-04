using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<Cellar> Cellars => Set<Cellar>();
    public DbSet<StorageUnit> StorageUnits => Set<StorageUnit>();
    public DbSet<Wine> Wines => Set<Wine>();
    public DbSet<CellarMembership> CellarMemberships => Set<CellarMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cellar>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<StorageUnit>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.HasOne(x => x.Cellar)
                .WithMany(x => x.StorageUnits)
                .HasForeignKey(x => x.CellarId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Wine>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Producer).HasMaxLength(200);
            entity.HasOne(x => x.StorageUnit)
                .WithMany(x => x.Wines)
                .HasForeignKey(x => x.StorageUnitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CellarMembership>(entity =>
        {
            entity.HasKey(x => new { x.CellarId, x.UserId });
            entity.HasOne(x => x.Cellar)
                .WithMany(x => x.Memberships)
                .HasForeignKey(x => x.CellarId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
