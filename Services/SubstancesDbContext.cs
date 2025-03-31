using Microsoft.EntityFrameworkCore;
using SubstanceSafe.Models;

namespace SubstanceSafe.Services // Assuming this is the correct namespace
{
    public class SubstancesDbContext : DbContext
    {
        public SubstancesDbContext(DbContextOptions<SubstancesDbContext> options) : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<SubstanceUsage> SubstanceUsages { get; set; } = null!; // Initialize with null forgiving operator
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
            modelBuilder.Entity<SubstanceType>()
                .HasIndex(t => new { t.CategoryId, t.Name })
                .IsUnique();

            // Add any other configurations here
        }
    }
}
