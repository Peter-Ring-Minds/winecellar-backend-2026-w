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
}
