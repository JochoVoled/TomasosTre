using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class UserStateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserStateService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ApplicationUser GetUser(ClaimsPrincipal user)
        {
            return _userManager.GetUserAsync(user).Result;
        }

        public bool IsSignedIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }
    }
}
