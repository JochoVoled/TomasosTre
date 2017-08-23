using System.Collections.Generic;

namespace TomasosTre.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public List<DishIngredient> DishIngredients { get; set; }
    }
}
