using System.ComponentModel.DataAnnotations;

namespace TomasosTre.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }
        [Required]
        [RegularExpression(@"^\d{3}$")]
        public int SecurityNumber { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}$")]
        public string ExpiryMonth { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^\d{5}$")]
        public int Zip { get; set; }
        [Required]
        public string City { get; set; }
        [Phone]
        [RegularExpression(@"^[\d\+\- ]+$")]        
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool IsRegistrating { get; set; }
    }
}