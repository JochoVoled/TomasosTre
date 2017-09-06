using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Models;
using TomasosTre.Data;
using TomasosTre.Services;

namespace TomasosTre.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Takes calls from front-end and re-routes to services
    /// </summary>
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _session;

        public ApiController(ApplicationDbContext context, SessionService session)
        {
            _context = context;
            _session = session;
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
            List<OrderRow> order = _session.LoadOrderRows(HttpContext);
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

            _session.Save(HttpContext,order);

            return RedirectToAction("CartPartial", "Render");
        }

        /// <summary>
        /// Remove an OrderRow from Cart
        /// </summary>
        /// <param name="id">The Id of the Dish to remove</param>
        /// <returns>Re-directs to the CartPartial</returns>
        //[HttpDelete]
        public IActionResult Remove(int id)
        {
            List<OrderRow> order = _session.LoadOrderRows(HttpContext);
            OrderRow remove = order.Find(o => o.DishId == id);

            order.Remove(remove);

            _session.Save(HttpContext,order);
            return RedirectToAction("CartPartial", "Render");
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
            List<OrderRow> order = _session.LoadOrderRows(HttpContext);
            OrderRow update = order.Find(o => o.DishId == id);

            update.Amount = amount;

            _session.Save(HttpContext,order);
            return RedirectToAction("CartPartial", "Render");
        }
        /// <summary>
        /// Save a custom dish to session variables
        /// </summary>
        /// <param name="baseDishId">Id of the dish being customized</param>
        /// <param name="isOrderedIngredients">List of selected ingredient IDs</param>
        /// <returns>The CartPartial partial view</returns>
        public IActionResult CustomizedDish(int baseDishId, List<int> isOrderedIngredients)
        {
            Dish option = _context.Dishes.First(dish => dish.Id == baseDishId);
            var optionIngredients = _context.DishIngredients.Where(x => x.DishId == baseDishId).Select(x => x.Ingredient).ToList();
            List<Ingredient> orderedIngredients = new List<Ingredient>();

            isOrderedIngredients.ForEach(x => orderedIngredients.Add(_context.Ingredients.First(y => y.Id == x)));
            IEnumerable<Ingredient> hasDeselected = optionIngredients.Except(orderedIngredients).ToList();
            IEnumerable<Ingredient> hasAdded = orderedIngredients.Except(optionIngredients).ToList();

            // Create a new dish, to connect this instance to the differing ingredients
            var order = _session.LoadOrderRows(HttpContext);

            // Remove one occurence of base dish
            if (order.Find(x => x.DishId == baseDishId).Amount == 1)
            {
                Remove(baseDishId);
            }
            else
            {
                order.Find(x => x.DishId == baseDishId).Amount -= 1;
            }
            
            // Set up a new Dish
            var newDish = new Dish
            {
                Price = option.Price,
                Name = $"Custom {option.Name}",
                DishIngredients = new List<DishIngredient>()
            };
            var newIngredients = optionIngredients.Except(hasDeselected).ToList().Union(hasAdded).ToList();
            newIngredients.ForEach(x => newDish.DishIngredients.Add(new DishIngredient
            {
                DishId = newDish.Id,
                IngredientId = x.Id
            }));
            var newOrder = new OrderRow
            {
                DishId = newDish.Id,
                Dish = newDish,
                Amount = 1,
                
            };

            // Add it to the table, if it's a new combination of ingredients
            if (IsDishNew(newDish))
            {
                _context.Dishes.Add(newDish);
            }
            order.Add(newOrder);

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
            _session.Save(HttpContext, orderRowIngredients);

            // Modify price
            foreach (var addedIngredient in hasAdded.Except(optionIngredients))
            {
                newOrder.Dish.Price += addedIngredient.Price;
            }
            foreach (var removeIngredient in hasDeselected.Except(optionIngredients))
            {
                newOrder.Dish.Price -= removeIngredient.Price;
            }
            _session.Save(HttpContext, order);
            return RedirectToAction("CartPartial", "Render");
        }

        private bool IsDishNew(Dish newDish)
        {
            // All customized X are called "custom X", so filter that first for performance
            var filtered = _context.Dishes.Where(x => x.Name == newDish.Name).ToList();
            if (!filtered.Any())
            {
                return true;
            }
            // Check if any dish has the same - no more, no less - ingredants as the new dish.
            foreach (var dish in filtered)
            {
                // A except B, and B except A, can only be equal if A = B
                var equals = dish.DishIngredients.Except(newDish.DishIngredients) == newDish.DishIngredients.Except(dish.DishIngredients);
                if (equals)
                {
                    return false;
                }
            }
            // if no Dishes remain, there are no duplicates; Dish is new
            return true;
        }

        /// <summary>
        /// Fetches the names and Ids of Dishes to the select2 box
        /// </summary>
        /// <returns>An collection of anon objects with IDs and Names</returns>
        public IActionResult GetDishNames()
        {
            var model = _context.Dishes.Select(x => new
            {
                id = x.Id,
                text = x.Name
            }).ToList();
            return Json(model);
        }
    }
}