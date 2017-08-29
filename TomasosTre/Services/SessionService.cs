using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public static class SessionService
    {
        #region Save Methods
        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="order">Current Order</param>
        public static void Save(HttpContext HttpContext,List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            HttpContext.Session.SetString("Order", serializedValue);
        }
        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="ori">Current Order Row Ingredients</param>
        public static void Save(HttpContext HttpContext, List<OrderRowIngredient> ori)
        {
            var serializedValue = JsonConvert.SerializeObject(ori);
            HttpContext.Session.SetString("ORI", serializedValue);
        }
        #endregion

        #region Load Methods

        /// <summary>
        /// Loads current Order from session variable
        /// </summary>
        /// <returns>Current Order</returns>
        public static List<OrderRow> LoadOrderRows(HttpContext HttpContext)
        {
            List<OrderRow> order;
            if (HttpContext.Session.GetString("Order") == null)
            {
                order = new List<OrderRow>();
            }
            else
            {
                var str = HttpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<List<OrderRow>>(str);
            }

            return order;
        }

        /// <summary>
        /// Loads customized dishes from session variable
        /// </summary>
        /// <returns>Current Order Row Ingredients</returns>
        public static List<OrderRowIngredient> LoadOrderRowIngredients(HttpContext HttpContext)
        {
            List<OrderRowIngredient> order;
            if (HttpContext.Session.GetString("ORI") == null)
            {
                order = new List<OrderRowIngredient>();
            }
            else
            {
                var str = HttpContext.Session.GetString("ORI");
                order = JsonConvert.DeserializeObject<List<OrderRowIngredient>>(str);
            }

            return order;
        }

        #endregion

        #region Clear Methods
        public static void ClearAll(HttpContext HttpContext)
        {
            var order = LoadOrderRows(HttpContext);
            var ori = LoadOrderRowIngredients(HttpContext);
            Clear(HttpContext, order);
            Clear(HttpContext, ori);
        }
        public static void Clear(HttpContext HttpContext, List<OrderRow> order)
        {
            HttpContext.Session.SetString("Order", "");
        }

        public static void Clear(HttpContext HttpContext, List<OrderRowIngredient> ori)
        {
            HttpContext.Session.SetString("ORI", "");
        }
        #endregion
    }
}
