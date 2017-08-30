using System;

namespace TomasosTre.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int CustomerId { get; set; }
        public bool IsDelivered { get; set; }

        //public Customer Customer { get; set; }
    }
}
