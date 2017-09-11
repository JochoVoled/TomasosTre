using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TomasosTre.Models;
using TomasosTre.ViewModels;
using System.Linq;
using TomasosTre.Data;

namespace TomasosTre.Services
{
    public class SessionService
    {
        private IHttpContextAccessor _http { get; }
        private readonly ApplicationDbContext _context;

        public SessionService(IHttpContextAccessor httpContext, ApplicationDbContext context)
        {
            _http = httpContext;
            _context = context;
        }

        #region CRUD Methods
        public void Create(int id)
        {
            Dish option = _context.Dishes.First(dish => dish.Id == id);
            List<OrderRow> order = LoadOrderRows();
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

            Save(order);
        }

        public void Delete(int id)
        {
            List<OrderRow> order = LoadOrderRows();
            OrderRow remove = order.Find(o => o.DishId == id);

            order.Remove(remove);

            Save(order);
        }

        public void Update(int id, int amount)
        {
            List<OrderRow> order = LoadOrderRows();
            OrderRow update = order.Find(o => o.DishId == id);

            update.Amount = amount;

            Save(order);
        }
        #endregion

        #region Save Methods

        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="order">Current Order</param>
        public void Save(List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            _http.HttpContext.Session.SetString("Order", serializedValue);
        }
        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="ori">Current Order Row Ingredients</param>
        public void Save(List<OrderRowIngredient> ori)
        {
            var serializedValue = JsonConvert.SerializeObject(ori, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            _http.HttpContext.Session.SetString("ORI", serializedValue);
        }
        /// <summary>
        /// Saves data fields during Checkout to session variable
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="checkout"></param>
        public void Save(CheckoutViewModel checkout)
        {
            // Clear card credentials to prevent saving
            checkout.CardNumber = "";
            checkout.SecurityNumber = 0;
            checkout.ExpiryMonth = "";

            var serializedValue = JsonConvert.SerializeObject(checkout, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            _http.HttpContext.Session.SetString("Checkout", serializedValue);
        }
        #endregion

        #region Load Methods

        /// <summary>
        /// Loads current Order from session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <returns>Current Order</returns>
        public List<OrderRow> LoadOrderRows()
        {
            List<OrderRow> order;
            if (_http.HttpContext.Session.GetString("Order") == null)
            {
                order = new List<OrderRow>();
            }
            else
            {
                var str = _http.HttpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<List<OrderRow>>(str);
            }

            return order;
        }

        /// <summary>
        /// Loads customized dishes from session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <returns>Current Order Row Ingredients</returns>
        public List<OrderRowIngredient> LoadOrderRowIngredients()
        {
            List<OrderRowIngredient> order;
            if (_http.HttpContext.Session.GetString("ORI") == null)
            {
                order = new List<OrderRowIngredient>();
            }
            else
            {
                var str = _http.HttpContext.Session.GetString("ORI");
                order = JsonConvert.DeserializeObject<List<OrderRowIngredient>>(str);
            }

            return order;
        }
        public CheckoutViewModel LoadCheckout()
        {
            CheckoutViewModel data;
            if (_http.HttpContext.Session.GetString("Checkout") == null)
            {
                data = new CheckoutViewModel();
            }
            else
            {
                var str = _http.HttpContext.Session.GetString("Checkout");
                data = JsonConvert.DeserializeObject<CheckoutViewModel>(str);
            }

            return data;
        }
        #endregion

        #region Clear Methods
        /// <summary>
        /// Clears all session variables
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        public void ClearAll()
        {
            var order = LoadOrderRows();
            var ori = LoadOrderRowIngredients();
            Clear(order);
            Clear(ori);
        }
        /// <summary>
        /// Clears OrderRows from the session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="order">Any List with OrderRow type</param>
        public void Clear(List<OrderRow> order)
        {
            _http.HttpContext.Session.SetString("Order", "");
        }
        /// <summary>
        /// Clears OrderRowIngredients from the session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="ori">Any List with OrderRowIngredient type</param>
        public void Clear(List<OrderRowIngredient> ori)
        {
            _http.HttpContext.Session.SetString("ORI", "");
        }
        public void Clear(CheckoutViewModel checkout)
        {
            _http.HttpContext.Session.SetString("Checkout", "");
        }
        #endregion
    }
}
