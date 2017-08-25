using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Models;
using TomasosTre.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TomasosTre.Controllers
{
    /// <summary>
    /// Manages the Order state. This controller is imported from the previous ASP .NET course. No point re-inventing the wheel.
    /// </summary>
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add an OrderRow to Cart if its the first one, or add to Amount if existing
        /// </summary>
        /// <param name="id">The Id of the Dish being added</param>
        /// <returns>Re-directs to the CartPartial</returns>
        public IActionResult Add(int id)
        {
            Dish option = _context.Dishes.First(dish => dish.Id == id);
            List<OrderRow> order = Load();
            OrderRow row = order.SingleOrDefault(or => or.DishId == option.Id);
            if (row != null)
            {
                row.Amount += 1;
            }
            else
            {
                row = new OrderRow
                {
                    Dish = option,
                    DishId = option.Id,
                    Amount = 1,
                };
                order.Add(row);
            }

            Save(order);

            return RedirectToAction("CartPartial", "Home");
        }

        /// <summary>
        /// Remove an OrderRow from Cart
        /// </summary>
        /// <param name="id">The Id of the Dish to remove</param>
        /// <returns>Re-directs to the CartPartial</returns>
        public IActionResult Remove(int id)
        {
            List<OrderRow> order = Load();
            OrderRow remove = order.Find(o => o.DishId == id);

            order.Remove(remove);

            Save(order);
            return RedirectToAction("CartPartial", "Home");
        }
        /// <summary>
        /// Sets a new amount ordered of a Dish, based on its id
        /// </summary>
        /// <param name="id">The Id of the dish whose ordered amount to change</param>
        /// <param name="amount">The new amount</param>
        /// <returns>Re-directs to the CartPartial</returns>
        public IActionResult Set(int id, int amount) {
            List<OrderRow> order = Load();
            OrderRow update = order.Find(o => o.DishId == id);

            update.Amount = amount;

            Save(order);
            return RedirectToAction("CartPartial", "Home");
        }
        public IActionResult CustomizedDish(int baseDishId,List<bool> isOrderedIngredients) {
            // Find the baseDish
            Dish option = _context.Dishes.First(dish => dish.Id == baseDishId);
            // Get a list of all its ingredients (A)
            var optionIngredients = _context.DishIngredientcses.Where(x => x.DishId == baseDishId).Select(x => x.Ingredient).ToList();
            // Get a list the ordered ingredients (B)
            List<Ingredient> orderedIngredients = new List<Ingredient>();
            for(int i = 0; i < isOrderedIngredients.Count; i++)
            {
                if (isOrderedIngredients[i]) {
                    var ing = _context.Ingredients.First(ingredient => ingredient.Id == i);
                    optionIngredients.Add(ing);
                }
            }
            /* Match A and B
                If item of A is not in B, the customer has de-selected an ingredient
                If item of B is not in A, the customer has added an extra ingredient */
            
            // Set up the diffs in a second session variable, noting if its extra or removed

            return RedirectToAction("CartPartial", "Home");
        }

        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="order">Current Order</param>
        private void Save(List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            HttpContext.Session.SetString("Order", serializedValue);
        }
        /// <summary>
        /// Loads current Order from session variable
        /// </summary>
        /// <returns>Current Order</returns>
        private List<OrderRow> Load()
        {
            List<OrderRow> order;
            if (HttpContext.Session.GetString("Order") == null)
            {
                order = new List<OrderRow>();
            }
            else
            {
                var str = HttpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<List<OrderRow>>(str);
            }

            return order;
        }
    }
}