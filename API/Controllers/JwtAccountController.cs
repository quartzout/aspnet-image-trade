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
    private IJwtTokenGenerator _jwtTokenGenerator;

    public JwtAccountController(UserManager<User> userManager,  IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
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
        return Ok(new LoginResultGet { Token = _jwtTokenGenerator.GenerateToken(user.Email!, user.Id) });
        
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
        return Ok(new LoginResultGet() { Token = _jwtTokenGenerator.GenerateToken(user.Email!, user.Id) });

    }

}
