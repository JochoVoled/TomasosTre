using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Extensions;
using TomasosTre.Models;
using TomasosTre.ViewModels;

namespace TomasosTre.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionService _session;
        private readonly AddressService _address;

        public OrderService(ApplicationDbContext context, SessionService session, AddressService address)
        {
            _context = context;
            _session = session;
            _address = address;
        }

        //public Order SetupNewOrder(CheckoutViewModel checkout, ApplicationUser User)
        public Order SetupNewOrder(ApplicationUser User)
        {
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
                order.AddressId = _address.Read(User.Id).AddressId;
            }

            return order;
        }

        public List<OrderRow> SetupNewOrderRows(Order order)
        {
            var OrderRows = _session.LoadOrderRows();
            foreach (var x in OrderRows)
            {
                x.OrderId = order.Id;
                order.Price += x.Dish.Price * x.Amount;
                x.Dish = null;
            }
            return OrderRows;
        }
        public List<OrderRowIngredient> SetupNewOrderRowIngredients(Order order)
        {
            List<OrderRowIngredient> ori = _session.LoadOrderRowIngredients();

            foreach (var x in ori)
            {
                order.Price += x.IsExtra ? x.Ingredient.Price : 0;
                x.OrderRow = null;
                x.Ingredient = null;
            }

            return ori;
        }

        public void SaveNewOrder(Order order, List<OrderRow> orderRows, List<OrderRowIngredient> ori)
        {
            // save readied order
            _context.Orders.Add(order);
            _context.SaveChanges();

            _context.OrderRows.AddRange(orderRows);
            _context.SaveChanges();

            _context.OrderRowIngredients.AddRange(ori);
            _context.SaveChanges();
        }

        public decimal ModifyPrice(int dishId, List<Ingredient> orderedIngredients)
        {
            List<Ingredient> optionIngredients = _context.DishIngredients.Where(x => x.DishId == dishId).Select(x => x.Ingredient).ToList();
            List<Ingredient> subtracted = optionIngredients.Except(orderedIngredients).ToList();
            List<Ingredient> added = orderedIngredients.Except(optionIngredients).ToList();

            decimal newPrice = 0m;
            foreach (var item in added)
            {
                newPrice += item.Price;
            }
            foreach (var item in subtracted)
            {
                newPrice -= item.Price;
            }
            return newPrice;
        }
    }
}
