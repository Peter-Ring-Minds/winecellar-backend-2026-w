using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}
        public DbSet<Domain.Cellar> Cellars { get; set; }
        public DbSet<Domain.StorageUnit> StorageUnits { get; set; }
        public DbSet<Domain.Wine> Wines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Domain.Cellar>()
                .HasMany(c => c.StorageUnits)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Domain.StorageUnit>((entity) => 
            {
                entity.HasOne(x => x.Cellar)
                    .WithMany(c => c.StorageUnits)
                    .HasForeignKey(x => x.CellarId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            builder.Entity<Domain.Wine>((entity) =>
            {
                entity.Property(x => x.Name).HasMaxLength(30);
                entity.Property(x => x.Wineyard).HasMaxLength(50);
                entity.Property(x => x.Type).HasMaxLength(30);
                entity.HasOne(x => x.StorageUnit)
                    .WithMany(su => su.Wines)
                    .HasForeignKey(x => x.StorageUnitId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

        }
}
