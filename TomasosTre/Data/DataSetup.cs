using System.Linq;
using Microsoft.AspNetCore.Identity;
using TomasosTre.Models;

namespace TomasosTre.Data
{
    public static class DataSetup
    {
        private static bool Clean { get; set; } = true;

        public static void Setup(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SetupAccountsAsync(userManager, roleManager);

            // Set up all storange tables
            if (!context.Dishes.Any())
            {
                context.Categories.AddRange(
                    new Category{ Name = "Pizza"}
                    );

                context.Dishes.AddRange(
                    new Dish { Name = "Capricciosa", Price = 79 },
                    new Dish { Name = "Margaritha", Price = 69 },
                    new Dish { Name = "Hawaii", Price = 85 }
                );
                // do stuff
                Clean = false;
            }
            if (!context.Ingredients.Any())
            {
                context.Ingredients.AddRange(
                    new Ingredient { Name = "Champignon", Price = 5m },
                    new Ingredient { Name = "Mozarella", Price = 15m },
                    new Ingredient { Name = "Basil", Price = 10m },
                    new Ingredient { Name = "Pineapple", Price = 10m },
                    new Ingredient { Name = "Ham", Price = 5m }
                );
                Clean = false;
            }
            // repeat pattern for each table

            Save(context);

            // Set up all relationship tables
            if (!context.DishIngredients.Any())
            {
                context.DishIngredients.AddRange(
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Capricciosa"), Ingredient = context.Ingredients.First(x => x.Name == "Champignon") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Capricciosa"), Ingredient = context.Ingredients.First(x => x.Name == "Ham") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Margaritha"), Ingredient = context.Ingredients.First(x => x.Name == "Basil") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Margaritha"), Ingredient = context.Ingredients.First(x => x.Name == "Mozarella") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Hawaii"), Ingredient = context.Ingredients.First(x => x.Name == "Pineapple") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Hawaii"), Ingredient = context.Ingredients.First(x => x.Name == "Ham") }
                );
                Clean = false;
            }

            Save(context);

            // Second-level relationship table (ie TableRowIngredients)
        }

        private static async void SetupAccountsAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Set up accounts
            var aUser = new ApplicationUser
            {
                UserName = "student@example.com",
                Email = "student@example.com",
            };
            await userManager.CreateAsync(aUser, "Pa$$w0rd");

            var adminRole = new IdentityRole { Name = "Admin" };
            await roleManager.CreateAsync(adminRole);

            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                
            };
            var result = await userManager.CreateAsync(adminUser, "Pa$$w0rd");

            await userManager.AddToRoleAsync(adminUser, adminRole.Name);
        }

        private static void Save(ApplicationDbContext context)
        {
            if (!Clean)
            {
                context.SaveChanges();
            }
            Clean = true;
        }
    }
}
