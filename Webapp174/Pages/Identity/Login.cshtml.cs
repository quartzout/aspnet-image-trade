using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Models.Classes.UI;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RazorPages.Identity.Classes;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authentication.Google;

namespace RazorPages.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //bind returnUrl from get. It is then gets sent to page to become value of hidden form input,
        //and then gets binded again when the form is posted.
        //This way onPost can use this value to redirect user after succesuful login.
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        

        [BindProperty]
        public LoginModelPOST UserLoginPOST { get; set; }

        public async Task<IActionResult> OnPost()
        {

            ModelState.Remove(nameof(ReturnUrl));

            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.PasswordSignInAsync(UserLoginPOST.Email, UserLoginPOST.Password, UserLoginPOST.RememberMe, false);

            return ProcessSignInResult(result);
        }

        IActionResult ProcessSignInResult(SignInResult result)
        {
            if (result.Succeeded) return Redirect(ReturnUrl ?? "/Index");

            if (result.IsNotAllowed)
            {
                //добавление ошибки в ModelState на уровень модели (а не в какой то конкретное свойство),
                //чтобы эта ошибка отобразилась в asp-validation-summary
                //(Пробовал добавить ее в свойство пароля, но таким образом в спане валидации пароля ошибка по какой то причине
                //не отображалась зато отображалась в asp-validation-summary, если поставтьь его на All вместо ModelOnly, магия)
                ModelState.AddModelError(string.Empty, "Wrong password or your email is have not been confirmed");
                return Page();
            }

            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is temporary locked out. Try again later");
                return Page();
            }

            ModelState.AddModelError(string.Empty, "Wrong email");
            return Page();
        }

        public IActionResult OnPostGoogleLogin()
        {

            string redirectUrl = Url.Page(
                pageName:"/Identity/Login",
                pageHandler: "ExternalLogin",
                values: new { ReturnUrl = ReturnUrl })!;

            var props = _signInManager.ConfigureExternalAuthenticationProperties(
                provider: GoogleDefaults.AuthenticationScheme,
                redirectUrl: redirectUrl);
             
            return Challenge(
                properties:  props,
                authenticationSchemes: new string[] { GoogleDefaults.AuthenticationScheme });
        }


        public async Task<IActionResult> OnGetExternalLoginAsync(string? remoteError = null)
        { 

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"remote auth error \"{remoteError}\".");
                return Page();
            }

            //МАГИЧЕСКОЕ доставание информации непонятно откуда
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                ModelState.AddModelError("", "Something went wrong: provider havent provided login info");
                return Page();
            }

            //taking email from principal provided by google
            string email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (email is null)
            {
                ModelState.AddModelError("", "Something went wrong: provider havent provided an email");
                return Page();
            }

            //Look up for user with this email
            var userToLogIn = await _userManager.FindByEmailAsync(
                email: email);
            
            //if there is no, create new user (with already confirmed email)
            if (userToLogIn is null) {

                userToLogIn = new()
                {
                    UserName = email,
                    Email = email,
                    DisplayName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name),
                    CoinBalance = 100,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(userToLogIn);

                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError("", $"Something went wrong: Error while creating new user: {error.Description}");
                    return Page();
                }

             }
            //At this point user is guaranteed to exist in database

            //Look up all logins that are tied to the user
            var userLogins = await _userManager.GetLoginsAsync(userToLogIn);

            //If in these logins there is no logins with the same provider as in logininfo provided by google, create new login 
            if (userLogins.Where(userLoginInfo => userLoginInfo.LoginProvider == loginInfo.LoginProvider).Count() == 0)
            {
                var addLoginResult = await _userManager.AddLoginAsync(userToLogIn, loginInfo);
                
                if (!addLoginResult.Succeeded)
                {
                    foreach (var error in addLoginResult.Errors)
                        ModelState.AddModelError("", $"Something went wrong: Error while adding new login for user: {error.Description}");
                    return Page();
                }
            }
            //At this point user with this login is guaranteed to exist in db

            var result = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: loginInfo.LoginProvider,
                providerKey: loginInfo.ProviderKey,
                isPersistent: true);

            return ProcessSignInResult(result);
        }
    }
}
