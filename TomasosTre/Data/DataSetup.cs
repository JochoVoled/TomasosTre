using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosTre.Models;

namespace TomasosTre.Data
{
    public static class DataSetup
    {
        private static bool Clean { get; set; } = true;

        public static void Setup(ApplicationDbContext context)
        {
            // Set up all storange tables
            if (!context.Dishes.Any())
            {
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
                    new Ingredient { Name = "Cheese" },
                    new Ingredient { Name = "Tomato" },
                    new Ingredient { Name = "Ham" }
                );
                Clean = false;
            }
            // repeat pattern for each table

            Save(context);

            // Set up all relationship tables
            if (!context.DishIngredientcses.Any())
            {
                context.DishIngredientcses.AddRange(
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Capricciosa"), Ingredient = context.Ingredients.First(x => x.Name == "Cheese") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Capricciosa"), Ingredient = context.Ingredients.First(x => x.Name == "Tomato") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Margaritha"), Ingredient = context.Ingredients.First(x => x.Name == "Cheese") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Margaritha"), Ingredient = context.Ingredients.First(x => x.Name == "Tomato") },
                    new DishIngredient { Dish = context.Dishes.First(x => x.Name == "Hawaii"), Ingredient = context.Ingredients.First(x => x.Name == "Tomato") }
                );
                Clean = false;
            }

            Save(context);

            // Second-level relationship table (ie TableRowIngredients)
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
