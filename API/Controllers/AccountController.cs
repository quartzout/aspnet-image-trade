using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Users.Identity.Classes;

namespace API.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginModelPOST loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(
                loginModel.Email, loginModel.Password, loginModel.RememberMe, false);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModelPOST registerModel)
        {

            User user = new()
            {
                Email = registerModel.Email,
                UserName = registerModel.Email,
                DisplayName = registerModel.DisplayName,
                CoinBalance = 100
            };

            var registerResult = await _userManager.CreateAsync(user, registerModel.Password);
            if (!registerResult.Succeeded) return BadRequest(registerResult);

            var loginResult = await _signInManager.PasswordSignInAsync(
                user, registerModel.Password, isPersistent: registerModel.RememberMe, false);

            return loginResult.Succeeded ? Ok(loginResult) : BadRequest(loginResult);
            
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
