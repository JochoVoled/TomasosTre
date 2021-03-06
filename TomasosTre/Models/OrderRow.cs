﻿using System.Collections.Generic;

namespace TomasosTre.Models
{
    public class OrderRow
    {
        public int OrderRowId { get; set; }
        public int DishId { get; set; }
        public int OrderId { get; set; }
        public int Amount { get; set; }

        public Dish Dish { get; set; }
        public Order Order { get; set; }
        public List<OrderRowIngredient> OrderRowIngredient { get; set; }

        public OrderRow(){}
        public OrderRow(Dish dish, int amount)
        {
            Dish = dish;
            DishId = dish.Id;
            Amount = amount;
        }
    }
}
