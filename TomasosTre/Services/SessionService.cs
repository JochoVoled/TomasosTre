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
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="order">Current Order</param>
        public static void Save(HttpContext httpContext,List<OrderRow> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            httpContext.Session.SetString("Order", serializedValue);
        }

        /// <summary>
        /// Saves current Order to session variable
        /// </summary>
        /// <param name="httpContext">Pass on static HttpContext</param>
        /// <param name="ori">Current Order Row Ingredients</param>
        public static void Save(HttpContext httpContext, List<OrderRowIngredient> ori)
        {
            /*TODO Fix self referencing loop
              Newtonsoft.Json.JsonSerializationException occurred
              HResult=0x80131500
              Message=Self referencing loop detected for property 'Dish' with type 'TomasosTre.Models.Dish'. Path '[0].OrderRow.Dish.DishIngredients[0]'.
              Source=<Cannot evaluate the exception source>
              StackTrace:
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.CheckForCircularReference(JsonWriter writer, Object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.CalculatePropertyValues(JsonWriter writer, Object value, JsonContainerContract contract, JsonProperty member, JsonProperty property, JsonContract& memberContract, Object& memberValue)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeObject(JsonWriter writer, Object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeObject(JsonWriter writer, Object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeObject(JsonWriter writer, Object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeObject(JsonWriter writer, Object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
               at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.Serialize(JsonWriter jsonWriter, Object value, Type objectType)
               at Newtonsoft.Json.JsonSerializer.SerializeInternal(JsonWriter jsonWriter, Object value, Type objectType)
               at Newtonsoft.Json.JsonConvert.SerializeObjectInternal(Object value, Type type, JsonSerializer jsonSerializer)
               at Newtonsoft.Json.JsonConvert.SerializeObject(Object value)
               at TomasosTre.Services.SessionService.Save(HttpContext HttpContext, List`1 ori) in C:\Source\Repos\TomasosTre\TomasosTre\Services\SessionService.cs:line 26
               at TomasosTre.Controllers.ApiController.CustomizedDish(Int32 baseDishId, List`1 isOrderedIngredients) in C:\Source\Repos\TomasosTre\TomasosTre\Controllers\ApiController.cs:line 158
               at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
               at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.<InvokeActionMethodAsync>d__12.MoveNext()
            */
            var serializedValue = JsonConvert.SerializeObject(ori);
            httpContext.Session.SetString("ORI", serializedValue);
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
        #endregion
    }
}
