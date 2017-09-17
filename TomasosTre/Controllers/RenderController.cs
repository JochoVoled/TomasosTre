using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TomasosTre.Data;
using TomasosTre.Models;
using TomasosTre.ViewModels.Home;
using Microsoft.AspNetCore.Http;
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
        #region Controller Setup
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RenderController> _logger;

        private readonly SessionService _session;
        private readonly OrderService _order;
        private readonly DishIngredientService _dishIngredientService;
        private readonly AddressService _address;

        public RenderController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RenderController> logger,
            SessionService session,
            OrderService order,
            DishIngredientService dishIngredientService,
            AddressService address)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _session = session;
            _order = order;
            _dishIngredientService = dishIngredientService;
            _address = address;
        }
        #endregion

        /// <summary>
        /// Initialize site
        /// </summary>
        /// <returns>The index page</returns>
        public IActionResult Index()
        {
            // Set up page
            var model = new IndexViewModel();

            // If user is returning from a non-finished purchase
            model.Cart.OrderRows = _session.LoadOrderRows();
            if (model.Cart.OrderRows != null)
            {
                model.Cart.OrderRows.ForEach(x => model.Cart.PriceSum += (x.Dish.Price * x.Amount));
            }
            else
            {
                model.Cart.PriceSum = 0;
            }
            
            return View(model);
        }

        /// <summary>
        /// Sets up the cart div
        /// </summary>
        /// <returns>The Cart partial view</returns>
        public IActionResult CartPartial()
        {
            var cartModel = new CartViewModel {OrderRows = _session.LoadOrderRows()};

            cartModel.OrderRows.ForEach(x => cartModel.PriceSum += (x.Dish.Price * x.Amount));
            return PartialView("Partial/_Cart",cartModel);
        }

        public IActionResult DishCustomizePartial(int id)
        {
            var allIngredients = _context.Ingredients.ToList();
            
            try
            {
                Dish dish = _context.Dishes.First(x => x.Id == id);
                var model = new DishCustomizationViewModel
                {
                    Dish = dish,
                    DishIngredients = _dishIngredientService.GetIngredientsRelatedTo(dish)
                };
                return PartialView("Partial/_DishCustomizer", model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public IActionResult CheckoutPartial(CheckoutViewModel checkout = null)
        {
            CheckoutViewModel data = checkout.Address != null ? checkout : _session.LoadCheckout();
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
            // TODO Uncomment this once I discover what breaks if its commented. Also, see if I can return the page with its validation errors if form is invalid
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            // Abort if card has expired
            DateTime expires = checkout.ExpiryMonth.ToExpiryDate();
            if (expires < DateTime.Now)
            {
                return RedirectToAction("CheckoutPartial", checkout);
            }

            // Apply the user's Address if one is logged in
            ApplicationUser user = _userManager.GetUserAsync(User).Result;
            Address address = user == null
                ? _address.Create(checkout.Address, checkout.Zip, checkout.City)
                : _address.Read(user.Id);
            
            // Load Order, with OrderRows and -Ingredients from session variables, and add them to database
            var order = _order.CreateOrder(user,address);
            //order.AddressId = address.AddressId;

            var or = _session.LoadOrderRows();
            or.ForEach(x => _order.CreateOrderRow(x.DishId, order.Id, x.Amount));
            //_order.CreateOrderRows(order);

            var ori = _session.LoadOrderRowIngredients();
            ori.ForEach(x => _order.CreateOrderRowIngredient(x.OrderRowId, x.IngredientId, x.IsExtra, x.IsRemoved));
            //var ori = _order.CreateOrderRowIngredients(order);

            // Modify the price
            _order.UpdateOrderPrice(order, or, ori);

            // Clear session variables, but add address for future account registration
            if (user == null)
                _session.Save(checkout);
            _session.ClearAll();

            if (checkout.IsRegistrating)
                return RedirectToAction("Register", "Account", routeValues: "/Confirmation");
            else
                return View("Confirmation");
        }

        [Authorize(Roles = "Admin")]
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
