using System;

namespace TomasosTre.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public string ApplicationUserId { get; set; }
        public bool IsDelivered { get; set; }
        public Address Address { get; set; }

        public ApplicationUser Customer { get; set; }
    }
}
