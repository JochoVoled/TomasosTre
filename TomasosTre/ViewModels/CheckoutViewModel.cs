using System;

namespace TomasosTre.ViewModels
{
    public class CheckoutViewModel
    {
        public string CardNumber { get; set; }
        public int SecurityNumber { get; set; }
        
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }

        public string Address { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public bool IsRegistrating { get; set; }
    }
}