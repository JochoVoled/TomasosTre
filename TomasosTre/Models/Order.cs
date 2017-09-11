using System;
using System.Collections.Generic;

namespace TomasosTre.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string ApplicationUserId { get; set; }
        public bool IsDelivered { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ApplicationUser Customer { get; set; }
        public List<OrderRow> OrderRows { get; set; }
    }
}
