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
        #region Controller Setup
        private readonly ApplicationDbContext _context;
        private readonly SessionService _session;
        private readonly DishService _dish;
        private readonly DishIngredientService _dishIngredientService;
        private readonly OrderService _order;
        private readonly OrderRowIngredientService _ori;

        public ApiController(ApplicationDbContext context,
            SessionService session,
            DishService dish,
            DishIngredientService dishIngredientService,
            OrderService order,
            OrderRowIngredientService ori)
        {
            _context = context;
            _session = session;
            _dish = dish;
            _dishIngredientService = dishIngredientService;
            _order = order;
            _ori = ori;
        }
        #endregion

        #region Cart session CRUD
        /// <summary>
        /// Add an OrderRow to Cart if its the first one, or add to Amount if existing
        /// </summary>
        /// <param name="id">The Id of the Dish being added</param>
        /// <returns>Re-directs to the CartPartial</returns>
        //[HttpPut]
        public IActionResult Add(int id)
        {
            _session.Create(id);

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
            _session.Delete(id);

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
            _session.Update(id, amount);

            return RedirectToAction("CartPartial", "Render");
        }
        #endregion

        /// <summary>
        /// Save a custom dish to session variables
        /// </summary>
        /// <param name="baseDishId">Id of the dish being customized</param>
        /// <param name="isOrderedIngredients">List of selected ingredient IDs</param>
        /// <returns>The CartPartial partial view</returns>
        public IActionResult CustomizedDish(int baseDishId, List<int> isOrderedIngredients)
        {
            Dish option = _context.Dishes.First(dish => dish.Id == baseDishId);
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == baseDishId).Select(x => x.Ingredient).ToList();
            List<Ingredient> orderedIngredients = new List<Ingredient>();
            isOrderedIngredients.ForEach(x => orderedIngredients.Add(_context.Ingredients.First(y => y.Id == x)));

            // Create a new dish, to connect this instance to the differing ingredients
            var order = _session.LoadOrderRows();

            // Remove one occurence of base dish
            if (order.Find(x => x.DishId == baseDishId).Amount == 1)
            {
                // TODO Solve Bug: Function does not remove OrderRow
                _session.Delete(baseDishId);
            }
            else
            {
                order.Find(x => x.DishId == baseDishId).Amount -= 1;
            }

            // Set up a new Dish
            Dish newDish = null;
            if (_dish.IsDishNew(option, orderedIngredients))
            {
                var added = _dishIngredientService.GetAdded(option, orderedIngredients);
                System.Text.StringBuilder mods = BuildCustomDishName(option, orderedIngredients, added);

                newDish = _dish.CreateDish($"Custom {option.Name}{mods.ToString()}", option.Price, option.CategoryId);
                _dishIngredientService.CreateMany(newDish, orderedIngredients);
            }
            else
            {
                option = _dish.GetExistingDish(option, orderedIngredients);
            }

            // Create a new OrderRow
            var newOrder = new OrderRow(newDish ?? option, 1);

            order.Add(newOrder);
            _ori.CreateMany(newOrder, orderedIngredients);
            
            // Modify price
            newOrder.Dish.Price += _order.ModifyCartPrice(baseDishId, orderedIngredients);

            _session.Save(order);
            return RedirectToAction("CartPartial", "Render");
        }

        private System.Text.StringBuilder BuildCustomDishName(Dish option, List<Ingredient> orderedIngredients, List<Ingredient> added)
        {
            System.Text.StringBuilder mods = new System.Text.StringBuilder();
            if (added.Count > 0)
            {
                mods.Append(" with");
                added.ForEach(x => mods.Append(" " + x.Name));
            }
            var subtracted = _dishIngredientService.GetSubtracted(option, orderedIngredients);
            if (added.Count > 0 && subtracted.Count > 0)
            {
                mods.Append(", but");
            }

            //System.Text.StringBuilder withouts = new System.Text.StringBuilder();
            if (subtracted.Count > 0)
            {
                mods.Append(" without");
                subtracted.ForEach(x => mods.Append(" " + x.Name));
            }

            return mods;
        }

        // TODO Remove once select2 is designed away
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