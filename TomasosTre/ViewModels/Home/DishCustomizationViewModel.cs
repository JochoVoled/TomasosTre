using System.Collections.Generic;
using TomasosTre.Models;
using TomasosTre.Structs;

namespace TomasosTre.ViewModels
{
    public class DishCustomizationViewModel
    {
        public Dish Dish { get; set; }
        public List<DishIngredientStruct> DishIngredients { get; set; } = new List<DishIngredientStruct>();

    }
}
