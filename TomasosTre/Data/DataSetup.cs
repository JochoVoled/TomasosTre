using System.Linq;
using Microsoft.AspNetCore.Identity;
using TomasosTre.Models;
using System.Threading.Tasks;

namespace TomasosTre.Data
{
    public static class DataSetup
    {
        private static bool Clean { get; set; } = true;

        public static async Task Setup(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // As this is a seed method, make sure it only runs if tables are empty
            ApplicationUser studentUser;
            if (context.Users.FirstOrDefault(x => x.Email == "student@example.com") == null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = "student@example.com",
                    Email = "student@example.com",
                };
                await userManager.CreateAsync(studentUser, "Pa$$w0rd");
            }
            else
            {
                studentUser = context.Users.FirstOrDefault(x => x.Email == "student@example.com");
            }

            ApplicationUser adminUser;
            if (context.Users.FirstOrDefault(x => x.Email == "admin@example.com") == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",

                };
                var adminRole = new IdentityRole { Name = "Admin" };

                var debug1 = roleManager.CreateAsync(adminRole).Result;
                var debug2 = userManager.CreateAsync(adminUser, "Pa$$w0rd").Result;
                var debug3 = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;
            }
            else
            {
                adminUser = context.Users.First(x => x.Email == "admin@example.com");
            }

            if (!context.Addresses.Any())
            {
                Address adminAddress = new Address
                {
                    CustomerId = adminUser.Id,
                    Street = "Admin Ave 14",
                    Zip = 12435,
                    City = "Testington"
                };

                Address studentAddress = new Address
                {
                    CustomerId = studentUser.Id,
                    Street = "Storgatan 14",
                    Zip = 12312,
                    City = "Testington"
                };
                context.Addresses.AddRange(adminAddress, studentAddress);
            }
            Category pizzaCategory;
            if (!context.Categories.Any())
            {
                pizzaCategory = new Category { Name = "Pizza" };
                context.Categories.Add(pizzaCategory);
            }
            else if (!context.Dishes.Any())
            {
                pizzaCategory = context.Categories.First(x => x.Name == "Pizza");
            }
            else
            {
                pizzaCategory = new Category();
            }

            Dish capricciosaPizza, margarithaPizza, hawaiiPizza;
            if (!context.Dishes.Any())
            {
                capricciosaPizza = new Dish { Name = "Capricciosa", Price = 79, Category = pizzaCategory };
                margarithaPizza = new Dish { Name = "Margaritha", Price = 69, Category = pizzaCategory };
                hawaiiPizza = new Dish { Name = "Hawaii", Price = 85, Category = pizzaCategory };
                context.AddRange(capricciosaPizza, margarithaPizza, hawaiiPizza);
            }
            else if (!context.DishIngredients.Any())
            {
                capricciosaPizza = context.Dishes.First(x => x.Name == "Capricciosa");
                margarithaPizza = context.Dishes.First(x => x.Name == "Margaritha");
                hawaiiPizza = context.Dishes.First(x => x.Name == "Hawaii");
            }
            else
            {
                capricciosaPizza = new Dish();
                margarithaPizza = new Dish();
                hawaiiPizza = new Dish();
            }

            Ingredient champignonIngredient, mozarellaIngredient, basilIngredient, pineappleIngredient, hamIngredient;
            if (!context.Ingredients.Any())
            {
                champignonIngredient = new Ingredient { Name = "Champignon", Price = 5m };
                mozarellaIngredient = new Ingredient { Name = "Mozarella", Price = 15m };
                basilIngredient = new Ingredient { Name = "Basil", Price = 10m };
                pineappleIngredient = new Ingredient { Name = "Pineapple", Price = 10m };
                hamIngredient = new Ingredient { Name = "Ham", Price = 5m };
                context.AddRange(champignonIngredient, mozarellaIngredient, basilIngredient, pineappleIngredient, hamIngredient);
            }
            else if (!context.DishIngredients.Any())
            {
                champignonIngredient = context.Ingredients.First(x => x.Name == "Champignon"); 
                mozarellaIngredient = context.Ingredients.First(x => x.Name == "Mozarella");
                basilIngredient = context.Ingredients.First(x => x.Name == "Basil");
                pineappleIngredient = context.Ingredients.First(x => x.Name == "Pineapple");
                hamIngredient = context.Ingredients.First(x => x.Name == "Ham");
            }
            else
            {
                champignonIngredient = new Ingredient();
                mozarellaIngredient = new Ingredient();
                basilIngredient = new Ingredient();
                pineappleIngredient = new Ingredient();
                hamIngredient = new Ingredient();
            }

            if (!context.DishIngredients.Any())
            {
                DishIngredient capricciosaHasChampignon = new DishIngredient { Dish = capricciosaPizza, Ingredient = champignonIngredient };
                DishIngredient capricciosaHasHam = new DishIngredient { Dish = capricciosaPizza, Ingredient = hamIngredient };
                DishIngredient margarithaHasBasil = new DishIngredient { Dish = margarithaPizza, Ingredient = basilIngredient };
                DishIngredient margarithaHasMozarella = new DishIngredient { Dish = margarithaPizza, Ingredient = mozarellaIngredient };
                DishIngredient hawaiiHasPineapple = new DishIngredient { Dish = hawaiiPizza, Ingredient = pineappleIngredient };
                DishIngredient hawaiiHasHam = new DishIngredient { Dish = hawaiiPizza, Ingredient = hamIngredient };
                context.DishIngredients.AddRange(capricciosaHasChampignon, capricciosaHasHam, margarithaHasBasil, margarithaHasMozarella, hawaiiHasPineapple, hawaiiHasHam);
            }

            context.SaveChanges();

        }
    }
}
