namespace TomasosTre.Models
{
    public class OrderRowIngredient
    {
        public int IngredientId { get; set; }
        public int OrderRowId { get; set; }
        public bool IsExtra { get; set; }
        public bool IsRemoved { get; set; }

        public Ingredient Ingredient { get; set; }
        public OrderRow OrderRow { get; set; }
    }
}
