using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TomasosTre.Models;
using TomasosTre.ViewModels;

namespace TomasosTre.Services
{
    public static class SessionService
    {
        #region Save Methods

        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="order">Current Order</param>
        public static void Save(HttpContext httpContext,List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            httpContext.Session.SetString("Order", serializedValue);
        }

        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="ori">Current Order Row Ingredients</param>
        public static void Save(HttpContext httpContext, List<OrderRowIngredient> ori)
        {
            var serializedValue = JsonConvert.SerializeObject(ori, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            httpContext.Session.SetString("ORI", serializedValue);
        }
        public static void Save(HttpContext httpContext, CheckoutViewModel checkout)
        {
            var serializedValue = JsonConvert.SerializeObject(checkout, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            httpContext.Session.SetString("Checkout", serializedValue);
        }
        #endregion

        #region Load Methods

        /// <summary>
        /// Loads current Order from session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <returns>Current Order</returns>
        public static List<OrderRow> LoadOrderRows(HttpContext httpContext)
        {
            List<OrderRow> order;
            if (httpContext.Session.GetString("Order") == null)
            {
                order = new List<OrderRow>();
            }
            else
            {
                var str = httpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<List<OrderRow>>(str);
            }

            return order;
        }

        /// <summary>
        /// Loads customized dishes from session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <returns>Current Order Row Ingredients</returns>
        public static List<OrderRowIngredient> LoadOrderRowIngredients(HttpContext httpContext)
        {
            List<OrderRowIngredient> order;
            if (httpContext.Session.GetString("ORI") == null)
            {
                order = new List<OrderRowIngredient>();
            }
            else
            {
                var str = httpContext.Session.GetString("ORI");
                order = JsonConvert.DeserializeObject<List<OrderRowIngredient>>(str);
            }

            return order;
        }
        public static CheckoutViewModel LoadCheckout(HttpContext httpContext)
        {
            CheckoutViewModel data;
            if (httpContext.Session.GetString("Checkout") == null)
            {
                data = new CheckoutViewModel();
            }
            else
            {
                var str = httpContext.Session.GetString("Checkout");
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
        public static void ClearAll(HttpContext httpContext)
        {
            var order = LoadOrderRows(httpContext);
            var ori = LoadOrderRowIngredients(httpContext);
            Clear(httpContext, order);
            Clear(httpContext, ori);
        }
        /// <summary>
        /// Clears OrderRows from the session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="order">Any List with OrderRow type</param>
        public static void Clear(HttpContext httpContext, List<OrderRow> order)
        {
            httpContext.Session.SetString("Order", "");
        }
        /// <summary>
        /// Clears OrderRowIngredients from the session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="ori">Any List with OrderRowIngredient type</param>
        public static void Clear(HttpContext httpContext, List<OrderRowIngredient> ori)
        {
            httpContext.Session.SetString("ORI", "");
        }
        public static void Clear(HttpContext httpContext, CheckoutViewModel checkout)
        {
            httpContext.Session.SetString("Checkout", "");
        }
        #endregion
    }
}
