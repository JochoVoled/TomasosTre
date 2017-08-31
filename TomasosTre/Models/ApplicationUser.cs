using Microsoft.AspNetCore.Identity;

namespace TomasosTre.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
    }
}
