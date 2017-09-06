using System.Collections.Generic;
using TomasosTre.Models;
using TomasosTre.Structs;

namespace TomasosTre.ViewModels.Admin
{
    public class DishesViewModel
    {
        public Dish Dish { get; set; }
        public List<DishIngredientStruct> Ingredients { get; set; }
    }
}
