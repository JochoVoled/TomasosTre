using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Ingredient> All()
        {
            return _context.Ingredients.OrderBy(x => x.Name).ToList();
        }
    }
}
