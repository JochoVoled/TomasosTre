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
using TomasosTre.Extensions;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TomasosTre.Controllers
{
    public class RenderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RenderController> _logger;

        private readonly SessionService _session;
        private readonly OrderService _order;

        public RenderController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                ILogger<RenderController> logger, SessionService session, OrderService order)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _session = session;
            _order = order;
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
            var cartModel = new CartViewModel {OrderRows = _session.LoadOrderRows(HttpContext)};

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

        public IActionResult CheckoutPartial(CheckoutViewModel checkout = null)
        {


            CheckoutViewModel data = checkout.Address != null ? checkout : _session.LoadCheckout(HttpContext);
            ApplicationUser user = new ApplicationUser();
            if (_signInManager.IsSignedIn(User))
            {
                user = _userManager.GetUserAsync(User).Result;
            }
            var address = user?.Addresses.FirstOrDefault(x =>
                x.CustomerId == user.Id && x.StartDateTime < DateTime.Now && x.EndDateTime > DateTime.Now);

            var model = new CheckoutViewModel
            {
                Address = address?.Street ?? data.Address,
                City = address?.City ?? data.City,
                Email = user?.Email ?? data.Email,
                Phone = user?.PhoneNumber ?? data.Phone,
                Zip = address?.Zip ?? data.Zip,
            };
            return PartialView("_Checkout",model);
        }

        public IActionResult Order(CheckoutViewModel checkout)
        {
            // Last Validation
            DateTime expires = checkout.ExpiryMonth.ToExpiryDate();

            if (expires < DateTime.Now)
            {
                return RedirectToAction("CheckoutPartial", checkout);
            }
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            
            // TODO Ask how to remove this HttpContext parameter - I don't want to pass it around everywhere
            var order = _order.SetupNewOrder(checkout, user, HttpContext);
            var ori = _session.LoadOrderRowIngredients(HttpContext);

            //// get and prepare data
            //var user = UserStateService.GetUser(User);
            //var order = new Order
            //{
            //    Date = DateTime.Now,
            //    IsDelivered = false,
            //};
            //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //if (user != null)
            //{
            //    order.ApplicationUserId = user.Id;
            //    order.Customer = user;
            //}
            //var orderRows = SessionService.LoadOrderRows();
            //var ori = SessionService.LoadOrderRowIngredients();

            //// connect all orderRows to new order
            //orderRows.ForEach(x => x.OrderId = order.Id);

            //// get price sum, if stored, or calculate sum from stored order rows
            //orderRows.ForEach(x => order.Price += x.Dish.Price * x.Amount);
            //ori.ForEach(x => order.Price += x.IsExtra ? x.Ingredient.Price : 0);

            _order.SaveNewOrder(order, order.OrderRows, ori);
            _session.ClearAll(HttpContext);
            _session.Save(HttpContext, checkout);

            if (checkout.IsRegistrating)
                return RedirectToAction("Register", "Account", routeValues: "/Confirmation");
            else
                return View("Confirmation");
        }
        
        // Disabled while in development. Tested, and it works
        //[Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View("Admin");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
