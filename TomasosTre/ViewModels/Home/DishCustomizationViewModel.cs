using System.Collections.Generic;
using TomasosTre.Models;

namespace TomasosTre.ViewModels
{
    public class DishCustomizationViewModel
    {
        public Dish Dish { get; set; }
        public List<DishCustomizationStruct> DishIngredients { get; set; } = new List<DishCustomizationStruct>();

    }
    // Not perhaps best practice, but this struct is made exclusively for this ViewModel, so I find it appropriate
    public struct DishCustomizationStruct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public decimal Price { get; set; }
    }
}
