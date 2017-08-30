using System.Collections.Generic;
using TomasosTre.Models;

namespace TomasosTre.ViewModels.Home
{
    public class CartViewModel
    {
        public List<OrderRow> OrderRows { get; set; } = new List<OrderRow>();
        public decimal PriceSum { get; set; }
    }
}
