using Microsoft.AspNetCore.Identity;
using RazorPages.Identity.Classes;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RazorPages.Models.Implementations;

public class MyHelper 
{ 

    public readonly UserManager<User> _userManager;
    public readonly IHttpContextAccessor _httpContextAccessor;

    public MyHelper(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<User?> GetCurrentUser()
    {
        string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(email)) return null;

        return await _userManager.FindByNameAsync(email);
    } 

    public async Task<string?> GetCurrentUserId() => (await GetCurrentUser())?.Id;

    public async Task<string?> GetCurrentUserName() => (await GetCurrentUser())?.UserName;  
}
