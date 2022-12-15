using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Models.Classes.UI;
using Microsoft.AspNetCore.Identity;
using RazorPages.Identity.Classes;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RazorPages.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RegisterModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        
        [BindProperty(SupportsGet = true)]
        
        [ValidateNever]
        public string ReturnUrl { get; set; }

        [BindProperty]
        public RegisterModelPOST UserPOST { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            //Validation is performed both for UserPOST and, for some reason, for ReturnURL (despite that no validation attributes
            //are applied to it), and furthermore, [ValidateNever] attribute have no effect on ReturnUrl, so the ModelState
            //needs to be modified before checking IsValid to clear all errors.
            ModelState.Remove(nameof(ReturnUrl));
            //ModelState[nameof(ReturnUrl)]!.ValidationState = ModelValidationState.Valid;   //also a way

            if (!ModelState.IsValid) return Page();

            User newUser = new() {
                Email = UserPOST.Email,
                UserName = UserPOST.Email,
                DisplayName = UserPOST.DisplayName,
                CoinBalance = 100
            };

            var result = await _userManager.CreateAsync(newUser, UserPOST.Password);
               
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Code + ": " + error.Description);
                }

                return Page();
            }

            //Aspnet turns all plus signs in spaces when binding from url, so token (which has pluses) needs to be encoded before 
            //feeding to Url.Page
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            string encodedToken = HttpUtility.UrlEncode(token);
            string link = Url.Page(
                    pageName: "/Identity/ConfirmEmail",
                    values: new { 
                        userName = UserPOST.Email, 
                        encodedToken = encodedToken, 
                        returnUrl = ReturnUrl,
                        rememberMe = UserPOST.RememberMe})!;

            string encodedLink = HttpUtility.UrlEncode(link);

            return Redirect("/Identity/EmailConfirmationLink?encodedLink=" + encodedLink);

        }

    }
}
