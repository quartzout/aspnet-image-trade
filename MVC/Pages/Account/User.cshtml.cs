using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Identity.Classes;
using RazorPages.Models.Implementations;

namespace RazorPages.Pages.Account
{
    public class UserModel : PageModel
    {
        readonly MyHelper _myHelper;
        readonly UserManager<User> _userManager;

        public UserModel(UserManager<User> userManager, MyHelper myHelper)
        {
            _userManager = userManager;
            _myHelper = myHelper;
        }

        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; } = "";

        public User ViewingUser { get; set; }
        public bool IsPersonal { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(UserName))
                return NotFound("User not found");

            ViewingUser = await _userManager.FindByNameAsync(UserName);

            if (ViewingUser is null)
                return NotFound("User not found");

            User? currentUser = await _myHelper.GetCurrentUser();
            if (currentUser is not null && currentUser == ViewingUser)
                IsPersonal = true;

            return Page();
        }
    }
}
