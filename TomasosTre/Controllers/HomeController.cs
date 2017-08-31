using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TomasosTre.Services;
using TomasosTre.ViewModels;

namespace TomasosTre.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        /// <summary>
        /// Initialize site
        /// </summary>
        /// <returns>The index page</returns>
        public IActionResult Index()
        {
            // Set up page
            var model = new IndexViewModel {
                Cart = new CartViewModel(),
                DishCustomization = new DishCustomizationViewModel()
            };
            
            // If user is returning from a non-finished purchase
            if (HttpContext.Session.GetString("Order") != null)
            {
                string str = HttpContext.Session.GetString("Order");
                model.Cart.OrderRows = JsonConvert.DeserializeObject<List<OrderRow>>(str);
            }
            model.Cart.OrderRows.ForEach(x => model.Cart.PriceSum += (x.Dish.Price * x.Amount));
            return View(model);
        }

        /// <summary>
        /// Sets up the cart div
        /// </summary>
        /// <returns>The Cart partial view</returns>
        public IActionResult CartPartial()
        {
            var cartModel = new CartViewModel {OrderRows = SessionService.LoadOrderRows(HttpContext)};

            cartModel.OrderRows.ForEach(x => cartModel.PriceSum += (x.Dish.Price * x.Amount));
            return PartialView("Partial/_Cart",cartModel);
        }

        public IActionResult DishCustomizePartial(int id)
        {
            var allIngredients = _context.Ingredients.ToList();
            Dish dish = _context.Dishes.FirstOrDefault(x => x.Id == id);
            if (dish == null)
            {
                // Return BadRequest response code (401?)
                
            }
            var model = new DishCustomizationViewModel{
                Dish = dish
            };
            foreach (var i in allIngredients)
            {
                var isChecked = _context.DishIngredients.Where(d => d.DishId == dish.Id).FirstOrDefault(di => di.IngredientId == i.Id) != null;
                model.DishIngredients.Add(new DishCustomizationStruct
                {
                    Id = i.Id,
                    Name = i.Name,
                    IsChecked = isChecked,
                    Price = i.Price
                });
            }

            return PartialView("Partial/_DishCustomizer", model);
        }

        public IActionResult CheckoutPartial()
        {
            CheckoutViewModel data = SessionService.LoadCheckout(HttpContext);
            ApplicationUser user = new ApplicationUser();
            if (_signInManager.IsSignedIn(User))
            {
                user = _userManager.GetUserAsync(User).Result;
            }
            var model = new CheckoutViewModel
            {
                Address = user?.Address ?? data.Address,
                City = user?.City ?? data.City,
                Email = user?.Email ?? data.Email,
                Zip = user?.Zip ?? data.Zip,
            };
            return PartialView("_Checkout",model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
