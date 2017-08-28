using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TomasosTre.Models;
//using static Microsoft.AspNetCore.Http.HttpContext;

namespace TomasosTre.Services
{
    // This is a draft service, written intended to refactor useful methods from controllers
    // However, there are strange issues with HttpContext, so I'll wait fixing those until later
    public class SessionService
    {
        public HttpContext HttpContext { get; }


        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="order">Current Order</param>
        private void Save(List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            HttpContext.Session.SetString("Order", serializedValue);
        }
        /// <summary>
        /// Loads current Order from session variable
        /// </summary>
        /// <returns>Current Order</returns>
        private List<OrderRow> Load()
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
