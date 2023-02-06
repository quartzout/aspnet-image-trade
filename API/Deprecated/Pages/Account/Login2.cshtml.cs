using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Models.Classes;
using System.Net;
using System.Security.Claims;

namespace RazorPages.Pages.Account
{
    public class Login2Model : PageModel
    {

        public IActionResult OnGet()
        {

            var claims = new List<Claim>() { new Claim("keytest", "valuetest") };
            var identity = new ClaimsIdentity(claims, "second_cookie_auth_scheme");
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync("second_cookie_auth_scheme", principal, new()
            {
                IsPersistent = true
            });

            var redirectAddress = Request.Query["returnurl"].ToString();

            return RedirectToPage(redirectAddress);
        }
    }
}
