using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.Structs;

namespace TomasosTre.Services
{
    public class DishIngredientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IngredientService _ingredientService;


        public DishIngredientService(ApplicationDbContext context, IngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
        }

        public List<DishIngredientStruct> NewDish()
        {
            var data = new List<DishIngredientStruct>();

            foreach (var i in _ingredientService.All())
            {
                data.Add(new DishIngredientStruct
                {
                    Id = i.Id,
                    Name = i.Name,
                    IsChecked = false,
                    Price = i.Price
                });
            }
            return data;
        }

        public List<DishIngredientStruct> GetIngredientsRelatedTo(Dish dish)
        {
            var data = new List<DishIngredientStruct>();

            foreach (var i in _ingredientService.All())
            {
                var isChecked = _context.DishIngredients.Where(d => d.DishId == dish.Id).FirstOrDefault(di => di.IngredientId == i.Id) != null;
                data.Add(new DishIngredientStruct
                {
                    Id = i.Id,
                    Name = i.Name,
                    IsChecked = isChecked,
                    Price = i.Price
                });
            }
            return data;
        }
    }
}
