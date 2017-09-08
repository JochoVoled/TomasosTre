namespace TomasosTre.Models
{
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int IngredientId { get; set; }
        //public bool IsIngredient { get; set; }
        //public int BoughtExtra { get; set; }

        public Dish Dish { get; set; }
        public Ingredient Ingredient { get; set; }
    }
}