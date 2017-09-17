using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class DishService
    {
        private readonly ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dish CreateDish(string name, decimal price, int categoryId)
        {
            //var existingDish = GetExistingDish(dish);
            
            // Add it to the table, if it's a new combination of ingredients
            //if (existingDish != null) return existingDish;

            Dish newDish = new Dish
            {
                Price = price,
                Name = name,
                CategoryId = categoryId
                
            };
            _context.Dishes.Add(newDish);
            _context.SaveChanges();
            return newDish;
        }

        public bool IsDishNew(Dish newDish, List<Ingredient> selected)
        {
            // All customized X are called "custom X", so filter that first for performance
            var filtered = _context.Dishes.Where(x => x.Name == newDish.Name).ToList();
            if (!filtered.Any())
            {
                return true;
            }
            // Check if any dish has the same - no more, no less - ingredants as the new dish.
            
            foreach (var dish in filtered)
            {
                var dishIngredients = _context.DishIngredients.Where(x => x.DishId == dish.Id).Select(x => x.Ingredient).ToList();
                // A except B, and B except A, can only be equal if A = B
                var equals = dishIngredients.Except(selected) == selected.Except(dishIngredients);
                if (equals)
                {
                    return false;
                }
            }
            // if no Dishes remain, there are no duplicates; Dish is new
            return true;
        }
        public Dish GetExistingDish(Dish newDish, List<Ingredient> selected)
        {
            // All customized X are called "custom X", so filter that first for performance
            var filtered = _context.Dishes.Where(x => x.Name == newDish.Name).ToList();
            if (!filtered.Any())
            {
                return null;
            }
            // Check if any dish has the same - no more, no less - ingredants as the new dish.
            foreach (var dish in filtered)
            {
                // A except B, and B except A, can only be equal if A = B
                var dishIngredients = _context.DishIngredients.Where(x => x.DishId == dish.Id).Select(x => x.Ingredient).ToList();
                // A except B, and B except A, can only be equal if A = B
                var equals = dishIngredients.Except(selected) == selected.Except(dishIngredients);
                if (equals)
                {
                    return dish;
                }
            }
            // if no Dishes remain, there are no duplicates; Dish is new
            return null;
        }
    }
}
