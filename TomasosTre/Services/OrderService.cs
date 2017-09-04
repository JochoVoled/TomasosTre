using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TomasosTre.Extensions;
using TomasosTre.Models;
using TomasosTre.ViewModels;

namespace TomasosTre.Services
{
    public static class OrderService
    {
        public static Data.ApplicationDbContext _context { get; set; }

        public static Order SetupNewOrder(CheckoutViewModel checkout, ApplicationUser User, HttpContext HttpContext)
        {
            DateTime expires = checkout.ExpiryMonth.ToDateTime();

            // get and prepare data
            //var User = UserStateService.GetUser(User);
            var order = new Order
            {
                Date = DateTime.Now,
                IsDelivered = false,
            };
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (User != null)
            {
                order.ApplicationUserId = User.Id;
                order.Customer = User;
            }
            order.OrderRows = SessionService.LoadOrderRows(HttpContext);
            var ori = SessionService.LoadOrderRowIngredients(HttpContext);

            // connect all orderRows to new order
            foreach (var x in order.OrderRows)
            {
                x.OrderId = order.Id;
                order.Price += x.Dish.Price * x.Amount;
                var relatedOris = ori.Where(y => y.OrderRowId == x.OrderRowId);
                x.OrderRowIngredient.AddRange(relatedOris);
            }
            //order.OrderRows.ForEach(x => x.OrderId = order.Id);
            //order.OrderRows.ForEach(x => order.Price += x.Dish.Price * x.Amount);
            ori.ForEach(x => order.Price += x.IsExtra ? x.Ingredient.Price : 0);

            return order;
        }

        public static void SaveNewOrder(Order order, List<OrderRow> orderRows, List<OrderRowIngredient> ori)
        {
            // save readied order
            _context.Orders.Add(order);
            _context.OrderRows.AddRange(orderRows);
            _context.SaveChanges();

            _context.OrderRowIngredients.AddRange(ori);
            _context.SaveChanges();
        }
    }
}
