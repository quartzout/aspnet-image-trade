using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Security.Claims;
using Users.Identity.Classes;

namespace RazorPages.Models.Implementations;

/// <summary>
/// Вспомогательный класс с разными полезными методами.
/// </summary>
public class MyHelper 
{ 

    public readonly UserManager<User> _userManager;
    public readonly IHttpContextAccessor _httpContextAccessor;

    public MyHelper(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }


    /// <summary>
    /// Возвращает текущего залогиненного пользователя
    /// </summary>
    public async Task<User?> GetCurrentUser()
    {
        string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(email)) return null;

        return await _userManager.FindByNameAsync(email);
    }

    /// <summary>
    /// Возвращает guid текущего залогиненного пользователя
    /// </summary>
    public async Task<string?> GetCurrentUserId() => (await GetCurrentUser())?.Id;

    /// <summary>
    /// Возвращает username текущего залогиненного пользователя
    /// </summary>
    public async Task<string?> GetCurrentUserName() => (await GetCurrentUser())?.UserName;  
}
