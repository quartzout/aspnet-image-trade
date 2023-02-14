using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Users.Identity.Classes;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace API.Controllers;
/// <summary>
/// Содержит все, что связано с пользователями: вход, регистрация и прочее. Вход реализован через JWT-токены,
/// содержащие email как единственный клейм, идентифицирующий пользователя.
/// Так как браузер автоматически не вставляет jwt-токены при отправке запросов, нет необходимости делать эндпоинт Logout
/// </summary>
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

    /// <summary>
    /// Логин пользователя с помощью JWT
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody]LoginModelPostJWT loginModel)
    {
        //Пользователь находится по email (являющимся уникальным юзернеймом) и проверяется его пароль.
        //Оба этих действия выполняются SignInManager.PasswordSignInAsync, но этот метот также добавляет
        //к response хедер с куки сессии, поэтому тут его нельзя использовать
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

        //При успешной проверке пароля генерируется и возвращается токен, содержащий только email
        return Ok(new LoginResultGet { Token = _jwtTokenGenerator.GenerateToken(user.Email!) });
        
    }


    /// <summary>
    /// Регистрация пользователя, а затем логин с помощью JWT
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterModelPostJWT registerModel)
    {

        User user = new()
        {
            Email = registerModel.Email,
            UserName = registerModel.Email,
            DisplayName = registerModel.DisplayName,
            CoinBalance = 100
        };

        //Пытаемся сохранить нового пользователя в бд, если не получается, возвращаем проблемы как ошибки валидации
        var registerResult = await _userManager.CreateAsync(user, registerModel.Password);
        if (!registerResult.Succeeded)
        {
            foreach (var error in registerResult.Errors)
                ModelState.AddModelError("", error.Description);
            return ValidationProblem();
        }

        //При успешном сохранении пользователя генерируется и возвращается токен, содержащий только email
        return Ok(new LoginResultGet() { Token = _jwtTokenGenerator.GenerateToken(user.Email!) });

    }

    /// <summary>
    /// Возвращает полную информацию о текущем залогиненном пользователе
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        //Достаем email, который является уникальным юзернеймом, из клеймов httpcontext (клейм отличается от того,
        //который используется для Cookie-логина
        string? email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //Находим по взятому email пользователя
        User? user = await _userManager.FindByNameAsync(email ?? "");
        if (user == null)
        {
            ModelState.AddModelError("", "Cannot find user");
            return ValidationProblem();
        }

        //Маппим пользователя в плоский дто для отправки и возвращаем
        return Ok(_mapper.Map<User, UserGetDto>(user)); 

    }

}
