using System.Collections.Generic;
using TomasosTre.Models;

namespace TomasosTre.ViewModels.Home
{
    public class IndexViewModel
    {
        public List<Dish> Menu { get; set; }
        public CartViewModel Cart { get; set; }
        public DishCustomizationViewModel DishCustomization { get; set; }

        public IndexViewModel()
        {
            Cart = new CartViewModel();
            DishCustomization = new DishCustomizationViewModel();
        }
    }
}
