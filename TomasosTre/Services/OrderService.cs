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
        private readonly AddressService _address;
        
        public OrderService(ApplicationDbContext context, AddressService address)
        {
            _context = context;
            _address = address;
        }

        public Order CreateOrder(ApplicationUser User)
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
                order.AddressId = _address.Read(User.Id).AddressId;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

            return order;
        }

        public OrderRow CreateOrderRow(int dishId, int orderId, int amount)
        {
            OrderRow orderRow = new OrderRow
            {
                DishId = dishId,
                Dish = null,
                OrderId = orderId,
                Order = null,
                Amount = amount,
            };

            _context.OrderRows.Add(orderRow);
            _context.SaveChanges();

            return orderRow;
        }
        public OrderRowIngredient CreateOrderRowIngredient(int orderRowId, int ingredientId, bool isExtra = true, bool isRemoved = false)
        {
            OrderRowIngredient ori = new OrderRowIngredient
            {
                OrderRow = null,
                OrderRowId = orderRowId,
                Ingredient = null,
                IngredientId = ingredientId,
                IsExtra = isExtra,
                IsRemoved = isRemoved
            };

            _context.OrderRowIngredients.Add(ori);
            _context.SaveChanges();

            return ori;
        }
        public decimal ModifyOrderPriceOnOrderedDishes(List<OrderRow> orderRows)
        {
            decimal priceChange = 0m;
            foreach (var x in orderRows)
            {
                var dish = _context.Dishes.Find(x.DishId);
                priceChange += dish.Price * x.Amount;
            }
            return priceChange;
        }
        public decimal ModifyOrderPriceOnAddedIngredient(List<OrderRowIngredient> extraIngredients)
        {
            decimal priceChange = 0m;
            foreach (var x in extraIngredients)
            {
                priceChange += x.IsExtra ? x.Ingredient.Price : 0;
            }
            return priceChange;
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
