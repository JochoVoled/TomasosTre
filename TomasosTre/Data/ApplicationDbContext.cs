using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TomasosTre.Models;

namespace TomasosTre.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set DB keys, including composite keys
            // Set keys
            builder.Entity<DishIngredient>().HasKey(di => new { di.DishId, di.IngredientId });
            // Set composite keys
            builder.Entity<DishIngredient>().HasOne(di => di.Dish).WithMany(di => di.DishIngredients)
                .HasForeignKey(di => di.DishId);
            builder.Entity<DishIngredient>().HasOne(di => di.Ingredient).WithMany(di => di.DishIngredients)
                .HasForeignKey(di => di.IngredientId);

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

        }
        // Set up DbSets
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<DishIngredient> DishIngredientcses { get; set; }
    }
}
