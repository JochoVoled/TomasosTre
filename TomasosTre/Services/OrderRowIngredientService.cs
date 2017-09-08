using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class OrderRowIngredientService
    {
        private readonly SessionService _session;
        private readonly ApplicationDbContext _context;

        public OrderRowIngredientService(SessionService session, ApplicationDbContext context)
        {
            _session = session;
            _context = context;
        }

        public void CreateMany(OrderRow orderRow, List<Ingredient> orderedIngredients)
        {
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == orderRow.DishId).Select(x => x.Ingredient).ToList();
            //List<Ingredient> orderedIngredients = new List<Ingredient>();
            //isOrderedIngredients.ForEach(x => orderedIngredients.Add(_context.Ingredients.First(y => y.Id == x)));

            List<Ingredient> hasDeselected = optionIngredients.Except(orderedIngredients).ToList();
            List<Ingredient> hasAdded = orderedIngredients.Except(optionIngredients).ToList();

            // Set up the diffs in a second session variable, noting if its extra or removed
            List<OrderRowIngredient> orderRowIngredients = new List<OrderRowIngredient>();
            orderRowIngredients.AddRange(hasDeselected.Select(ingredient => new OrderRowIngredient
            {
                Ingredient = ingredient,
                IngredientId = ingredient.Id,
                IsRemoved = true,
                IsExtra = false,
                OrderRowId = orderRow.OrderRowId,
                OrderRow = orderRow
            }));
            orderRowIngredients.AddRange(hasAdded.Select(ingredient => new OrderRowIngredient
            {
                Ingredient = ingredient,
                IngredientId = ingredient.Id,
                IsRemoved = false,
                IsExtra = true,
                OrderRowId = orderRow.OrderRowId,
                OrderRow = orderRow
            }));
            _session.Save(orderRowIngredients);
        }
    }
}
