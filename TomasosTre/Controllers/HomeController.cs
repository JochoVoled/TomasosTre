using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.ViewModels.Home;

namespace TomasosTre.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dishes = _context.Dishes.ToList();
            //var model = _context.Ingredients.ToList();
            var model = new IndexViewModel
            {
                Dishes = dishes
            };
            // Set up page
            return View(model);
            //return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Search(string phrase)
        {
            var applicableDishes = _context.Dishes.Where(x => x.Name.Contains(phrase));
            return Json(applicableDishes);
        }

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
