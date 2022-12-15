using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Models.Classes.UI;
using System.Net;
using System.Security.Claims;
using Identity;

namespace RazorPages.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly Identity.IdentityManager _loginManager;

        public LoginModel(Identity.IdentityManager loginManager)
        {
            _loginManager = loginManager;
        }

        //bind returnUrl from get. It is then gets sent to page to become value of hidden form input,
        //and then gets binded again when the form is posted.
        //This way onPost can use this value to redirect user after succesuful login.
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        

        [BindProperty]
        public POSTUserLoginVisual UserLoginPOST { get; set; }

        public async Task<IActionResult> OnPost()
        {
            UserLoginInfo loginInfo = new UserLoginInfo()
            {
                Email = UserLoginPOST.Email,
                Password = UserLoginPOST.Password
            };

            var result = await _loginManager.Login(loginInfo);

            if (!result.Success) return Redirect("/Account/Login?error="+result.Error!.Replace(" ", "-"));

            return Redirect(ReturnUrl ?? "/Index");



        }
    }
}
