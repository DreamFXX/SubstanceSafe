using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Models;

namespace SubstanceSafe.Services;

public class SubstancesDbContext : DbContext
{
    public SubstancesDbContext(DbContextOptions<SubstancesDbContext> options) : base(options)
    }

    public DbSet<SubstanceUsage> SubstanceUsages { get; set; } = null!;
    public DbSet<SubstanceType> SubstanceTypes { get; set; } = null!;
    public DbSet<SubstanceCategory> SubstanceCategories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships if needed (Fluent API)
        // Example: Ensure Category Name is unique
        modelBuilder.Entity<SubstanceCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

// Example: Ensure Type Name is unique within a Category
        m        odelBuilder.Entity<SubstanceType>()
            .HasIndex(t => new { t.CategoryId, t.Name })
                .IsUnique();

    }
}
