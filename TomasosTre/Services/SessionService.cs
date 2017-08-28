using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public static class SessionService
    {
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
        /// Loads current Order from session variable
        /// </summary>
        /// <returns>Current Order</returns>
        public static List<OrderRow> Load(HttpContext HttpContext)
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
    }
}
