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

        public Dish NewCustomDish(string name, decimal price)
        {
            //List<Ingredient> list = dishIngredient ?? new List<Ingredient>();
            Dish newDish = new Dish
            {
                Price = price,
                Name = $"Custom {name}",
            };

            // Add it to the table, if it's a new combination of ingredients
            if (IsDishNew(newDish))
            {
                _context.Dishes.Add(newDish);
                _context.SaveChanges();
            }

            return newDish;
        }
        private bool IsDishNew(Dish newDish)
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
                // A except B, and B except A, can only be equal if A = B
                var equals = dish.DishIngredients.Except(newDish.DishIngredients) == newDish.DishIngredients.Except(dish.DishIngredients);
                if (equals)
                {
                    return false;
                }
            }
            // if no Dishes remain, there are no duplicates; Dish is new
            return true;
        }
    }
}
