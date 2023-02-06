using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Models.Classes.UI;
using Identity;

namespace RazorPages.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly Identity.IdentityManager _loginManager;

        public RegisterModel(Identity.IdentityManager loginManager)
        {
            _loginManager = loginManager;
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty]
        public POSTUserRegisterVisual UserPOST { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            UserRegisterInfo userInfo = new() { 
                Email = UserPOST.Email,
                Name = UserPOST.Name,
                Password = UserPOST.Password
            };

            var result = await _loginManager.Register(userInfo);

            if (!result.Success) return Redirect("/Account/Register?error=" + result.Error!.Replace(" ", "-"));

            return Redirect(ReturnUrl ?? "/Index");

        }

    }
}
