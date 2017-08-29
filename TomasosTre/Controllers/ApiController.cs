using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Models;
using TomasosTre.Data;
using TomasosTre.Services;

namespace TomasosTre.Controllers
{
    // TODO Move most methods here to services, create an API Controller that directs to services for future cases like these
    /// <inheritdoc />
    /// <summary>
    /// Takes calls from front-end and re-routes to services
    /// </summary>
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add an OrderRow to Cart if its the first one, or add to Amount if existing
        /// </summary>
        /// <param name="id">The Id of the Dish being added</param>
        /// <returns>Re-directs to the CartPartial</returns>
        //[HttpPut]
        public IActionResult Add(int id)
        {
            Dish option = _context.Dishes.First(dish => dish.Id == id);
            List<OrderRow> order = SessionService.LoadOrderRows(HttpContext);
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

            SessionService.Save(HttpContext,order);

            return RedirectToAction("CartPartial", "Home");
        }

        /// <summary>
        /// Remove an OrderRow from Cart
        /// </summary>
        /// <param name="id">The Id of the Dish to remove</param>
        /// <returns>Re-directs to the CartPartial</returns>
        //[HttpDelete]
        public IActionResult Remove(int id)
        {
            List<OrderRow> order = SessionService.LoadOrderRows(HttpContext);
            OrderRow remove = order.Find(o => o.DishId == id);

            order.Remove(remove);

            SessionService.Save(HttpContext,order);
            return RedirectToAction("CartPartial", "Home");
        }
        /// <summary>
        /// Sets a new amount ordered of a Dish, based on its id
        /// </summary>
        /// <param name="id">The Id of the dish whose ordered amount to change</param>
        /// <param name="amount">The new amount</param>
        /// <returns>Re-directs to the CartPartial</returns>
        //[HttpPatch]
        public IActionResult Set(int id, int amount)
        {
            List<OrderRow> order = SessionService.LoadOrderRows(HttpContext);
            OrderRow update = order.Find(o => o.DishId == id);

            update.Amount = amount;

            SessionService.Save(HttpContext,order);
            return RedirectToAction("CartPartial", "Home");
        }
        public IActionResult CustomizedDish(int baseDishId, List<int> isOrderedIngredients)
        {
            // Find the baseDish
            Dish option = _context.Dishes.First(dish => dish.Id == baseDishId);
            // Get a list of all its ingredients (A)
            var optionIngredients = _context.DishIngredientcses.Where(x => x.DishId == baseDishId).Select(x => x.Ingredient).ToList();
            // Get a list the ordered ingredients (B)
            List<Ingredient> orderedIngredients = new List<Ingredient>();
            isOrderedIngredients.ForEach(x => orderedIngredients.Add(_context.Ingredients.First(y => y.Id == x)));

            /* Match A and B
                If item of A is not in B, the customer has de-selected an ingredient
                If item of B is not in A, the customer has added an extra ingredient
                Code for that found through https://stackoverflow.com/questions/3739246/linq-to-sql-not-contains-or-not-in#3740255 */
            //IEnumerable<Ingredient> hasDeselected = optionIngredients.Where(x => !orderedIngredients.Select(y => y.Id).Contains(x.Id)).ToList();
            IEnumerable<Ingredient> hasDeselected = optionIngredients.Except(orderedIngredients).ToList();
            //IEnumerable<Ingredient> hasAdded = orderedIngredients.Where(x => !optionIngredients.Select(y => y.Id).Contains(x.Id)).ToList();
            IEnumerable<Ingredient> hasAdded = orderedIngredients.Except(optionIngredients);

            // Create a new dish, to connect this instance to the differing ingredients
            var order = SessionService.LoadOrderRows(HttpContext);
            var newOrder = new OrderRow
            {
                Dish = option,
                Amount = 1,
            };
            order.Add(newOrder);
            SessionService.Save(HttpContext,order);

            // Set up the diffs in a second session variable, noting if its extra or removed
            var orderRowIngredients = hasDeselected.Select(ingredient => new OrderRowIngredient
            {
                Ingredient = ingredient,
                IngredientId = ingredient.Id,
                IsRemoved = true,
                IsExtra = false,
                OrderRowId = newOrder.OrderRowId,
                OrderRow = newOrder
            })
                .ToList();
            orderRowIngredients.AddRange(hasAdded.Select(ingredient => new OrderRowIngredient
            {
                Ingredient = ingredient,
                IngredientId = ingredient.Id,
                IsRemoved = false,
                IsExtra = true,
                OrderRowId = newOrder.OrderRowId,
                OrderRow = newOrder
            }));
            
            SessionService.Save(HttpContext, orderRowIngredients);

            return RedirectToAction("CartPartial", "Home");
        }
        /// <summary>
        /// Draft method. Save cart values stored in session to DB, and clear session
        /// </summary>
        public void PlaceOrder()
        {
            var order = SessionService.LoadOrderRows(HttpContext);
            _context.OrderRows.AddRange(order);
            var ori = SessionService.LoadOrderRowIngredients(HttpContext);
            _context.OrderRowIngredients.AddRange(ori);

            _context.SaveChanges();

            SessionService.ClearAll(HttpContext);
        }
    }
}