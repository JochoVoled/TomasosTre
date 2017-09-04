using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class UserStateService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;

        public UserStateService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ApplicationUser GetUser(ClaimsPrincipal User)
        {
            return _userManager.GetUserAsync(User).Result;
        }

        public bool IsSignedIn(ClaimsPrincipal User)
        {
            return _signInManager.IsSignedIn(User);
        }
    }
}
