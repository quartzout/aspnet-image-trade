using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Users.Identity.Classes;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace API.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class JwtAccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        private IMapper _mapper;
        private IJwtTokenGenerator _jwtTokenGenerator;

        public JwtAccountController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModelPOST loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "No user found with provided email");
                return ValidationProblem();
            }
            if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                ModelState.AddModelError("Password", "Password doesnt match");
                return ValidationProblem();
            }

            return Ok(new LoginResultGet { Token = _jwtTokenGenerator.GenerateToken(user.Email!) });
            
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModelPOST registerModel)
        {

            User user = new()
            {
                Email = registerModel.Email,
                UserName = registerModel.Email,
                DisplayName = registerModel.DisplayName,
                CoinBalance = 100
            };

            var registerResult = await _userManager.CreateAsync(user, registerModel.Password);
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return ValidationProblem();
            }

            return Ok(new LoginResultGet() { Token = _jwtTokenGenerator.GenerateToken(user.Email!) });

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            string? email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            User? user = await _userManager.FindByNameAsync(email ?? "");
            if (user == null)
            {
                ModelState.AddModelError("", "Cannot find user");
                return ValidationProblem();
            }

            return Ok(_mapper.Map<User, UserGetDto>(user)); 

        }

    }
}
