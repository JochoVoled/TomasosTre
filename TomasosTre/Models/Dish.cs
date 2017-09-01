using System.Collections.Generic;

namespace TomasosTre.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }

        public Category Category { get; set; }
        public List<DishIngredient> DishIngredients { get; set; }
    }
}
