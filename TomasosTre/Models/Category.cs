using System.Collections.Generic;

namespace TomasosTre.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<Dish> Dishes { get; set; }
    }
}
