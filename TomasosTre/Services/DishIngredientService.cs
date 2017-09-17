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

        public List<Ingredient> GetAdded(Dish dish, List<Ingredient> selected)
        {
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == dish.Id).Select(x => x.Ingredient).ToList();
            return selected.Except(optionIngredients).ToList();
        }

        public List<Ingredient> GetSubtracted(Dish dish, List<Ingredient> selected)
        {
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == dish.Id).Select(x => x.Ingredient).ToList();
            return optionIngredients.Except(selected).ToList();
        }

        public void UpdateDishIngredients(Dish dish, List<Ingredient> updatedIngredients)
        {
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == dish.Id).Select(x => x.Ingredient).ToList();
            List<Ingredient> delete = GetSubtracted(dish, updatedIngredients);
            List<Ingredient> add = GetAdded(dish, updatedIngredients);
            DeleteMany(dish, delete);
            CreateMany(dish, add);
            _context.SaveChanges();
        }

        public void CreateMany(Dish dish, List<Ingredient> ingredients)
        {
            dish.DishIngredients = new List<DishIngredient>();
            ingredients.ForEach(x => dish.DishIngredients.Add(new DishIngredient
            {
                DishId = dish.Id,
                IngredientId = x.Id
            }));
            _context.DishIngredients.AddRange(dish.DishIngredients);
            _context.SaveChanges();
        }

        public void DeleteMany(Dish dish, List<Ingredient> ingredients)
        {
            dish.DishIngredients = new List<DishIngredient>();
            foreach (var item in ingredients)
            {
                var dishIngredient = _context.DishIngredients.First(x => x.DishId == dish.Id && x.IngredientId == item.Id);
                if (dishIngredient == null)
                {
                    continue;
                }
                _context.DishIngredients.Remove(dishIngredient);
            }
            _context.SaveChanges();
        }
    }
}
