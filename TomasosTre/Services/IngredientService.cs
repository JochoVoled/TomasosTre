using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Ingredient> All()
        {
            return _context.Ingredients.OrderBy(x => x.Name).ToList();
        }

        public List<Ingredient>GetIngredientsRelatedTo(Dish dish)
        {
            var data = new List<Ingredient>();
            foreach (var item in _context.DishIngredients.Where(x => x.DishId == dish.Id).ToList())
            {
                data.Add(item.Ingredient);
            }
            return data;
        }

        public List<Ingredient> GetIngredientsRelatedTo(OrderRow or)
        {
            var data = new List<Ingredient>();
            foreach (var item in _context.OrderRowIngredients.Where(x => x.OrderRowId == or.OrderRowId).ToList())
            {
                data.Add(item.Ingredient);
            }
            return data;
        }
    }
}
