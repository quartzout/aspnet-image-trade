using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Users.Identity.Classes;

namespace API.Implementations
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private IHttpContextAccessor _httpContextAccessor;
        private UserManager<User> _userManager;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<User?> GetCurrentUser()
        {
            //Достаем email, который является уникальным юзернеймом, из клеймов httpcontext
            string? email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);

            //Находим по взятому email пользователя
            return await _userManager.FindByNameAsync(email ?? "");
        }
    }
}
