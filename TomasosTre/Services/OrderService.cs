using System;
using System.Collections.Generic;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Order CreateOrder(ApplicationUser User, Address address)
        {
            var order = new Order
            {
                Date = DateTime.Now,
                IsDelivered = false,
                AddressId = address.AddressId
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            return order;
        }

        public OrderRow CreateOrderRow(int dishId, int orderId, int amount)
        {
            // For code integrity, test if desired order is already deliver, and abort addition if so
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


        public decimal UpdateOrderPrice(Order order, List<OrderRow> orderRows, List<OrderRowIngredient> extraIngredients)
        {
            decimal priceChange = order.Price;
            foreach (var x in orderRows)
            {
                var dish = _context.Dishes.Find(x.DishId);
                priceChange += dish.Price * x.Amount;
            }
            foreach (var x in extraIngredients)
            {
                priceChange += x.IsExtra ? x.Ingredient.Price : 0;
            }

            _context.Orders.Update(order);
            _context.SaveChanges();

            return priceChange;
        }

        public decimal ModifyCartPrice(int dishId, List<Ingredient> orderedIngredients)
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
