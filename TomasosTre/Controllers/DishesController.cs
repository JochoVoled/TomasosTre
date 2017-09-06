using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TomasosTre.Data;
using TomasosTre.Models;
using Microsoft.AspNetCore.Authorization;
using TomasosTre.ViewModels.Admin;
using TomasosTre.Services;

namespace TomasosTre.Controllers
{
    // Disabled while in development. Tested, and it works
    //[Authorize(Roles = "Admin")]
    public class DishesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DishIngredientService _dishIngredientService;

        public DishesController(ApplicationDbContext context, DishIngredientService dishIngredientService)
        {
            _context = context;
            _dishIngredientService = dishIngredientService;
        }

        // GET: Dishes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Dishes.Include(d => d.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = new DishesViewModel();
            model.Dish = await _context.Dishes
                .Include(d => d.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (model.Dish == null)
            {
                return NotFound();
            }
            model.Ingredients = _dishIngredientService.GetIngredientsRelatedTo(model.Dish);

            return View(model);
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");

            var model = new DishesViewModel
            {
                Dish = new Dish(),
                Ingredients = _dishIngredientService.NewDish()
            };

            return View(model);
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId,Price")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dish);
                // TODO Add DishIngredients to corresponding table
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", dish.CategoryId);
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = new DishesViewModel();
            model.Dish = await _context.Dishes.SingleOrDefaultAsync(m => m.Id == id);
            if (model.Dish == null)
            {
                return NotFound();
            }
            model.Ingredients = _dishIngredientService.GetIngredientsRelatedTo(model.Dish);

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", model.Dish.CategoryId);
            return View(model);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Price")] Dish dish)
        {
            if (id != dish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", dish.CategoryId);
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .Include(d => d.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.SingleOrDefaultAsync(m => m.Id == id);
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
