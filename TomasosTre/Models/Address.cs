using System;

namespace TomasosTre.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string CustomerId { get; set; }
        public int OrderId { get; set; }
        public string Street { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public ApplicationUser Customer { get; set; }
        public Order Order { get; set; }
    }
}