using System.ComponentModel.DataAnnotations;

namespace TomasosTre.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public int Zip { get; set; }

        [Required]
        public string City { get; set; }

        public string StatusMessage { get; set; }
    }
}
