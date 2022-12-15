using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Identity.Classes;
using System.Web;

namespace RazorPages.Pages.Identity
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ConfirmEmailModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public async Task<IActionResult> OnGetAsync(string userName, string encodedToken, string? returnUrl, bool rememberMe)
        {
            //Token is decoded automatically by parameter binding 
            string token = encodedToken;

            if (userName is null || token is null) return LocalRedirect("/Identity/Register/?error=User-Name-Or-Token-Are-Empty");

            User userToConfirm = await _userManager.FindByNameAsync(userName);
            var result = await _userManager.ConfirmEmailAsync(userToConfirm, token);

            await _signInManager.SignInAsync(userToConfirm, rememberMe);

            if (result.Succeeded) return LocalRedirect(returnUrl ?? "/Index");

            return LocalRedirect("/Identity/Register/?error=" + result.Errors.First().Description.Replace(" ", "-"));
            
        }
    }
}
