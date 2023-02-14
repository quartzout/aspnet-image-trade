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
/// Содержит все, что связано с пользователями: вход, выход, регистрация и прочее. Вход реализован через Cookie с айди сессии. Для управления Cookie браузера
/// используется сервис Identity SignInManager. В Jwt-реализации этого же контроллера этот серсвис не используется.
/// </summary>
[Route("api/[controller]/{action}")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private IHttpContextAccessor _httpContextAccessor;
    private IMapper _mapper;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _signInManager = signInManager;
    }


    /// <summary>
    /// Вспомогательный метод, возвращающий ошибки валидации, сформированные из результата <see cref="SignInManager{TUser}.PasswordSignInAsync"/>
    /// </summary>
    public IActionResult LoginFail(SignInResult result)
    {
        if (result.IsNotAllowed)
        {
            ModelState.AddModelError("", "User Not Allowed");
            return ValidationProblem();
        }
        else if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "User us locked out");
            return ValidationProblem();
        }
       
        ModelState.AddModelError("", "Wrong email or password");
        return ValidationProblem();

    }

    /// <summary>
    /// Логин пользователя с помощью Cookie
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody]LoginModelPOST loginModel)
    {
        //Этот метод при успешном логине добавляет в response хедер set-cookie с session-id внутри
        var result = await _signInManager.PasswordSignInAsync(
            loginModel.Email, loginModel.Password, loginModel.RememberMe, false);

        if (result.Succeeded)
        {
            return Ok();
        }

        return LoginFail(result);            
        
    }


    /// <summary>
    /// Регистрация пользователя, а затем логин с помощью Cookie
    /// </summary>
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

        //Пытаемся сохранить нового пользователя в бд, если не получается, возвращаем проблемы как ошибки валидации
        var registerResult = await _userManager.CreateAsync(user, registerModel.Password);
        if (!registerResult.Succeeded)
        {
            foreach (var error in registerResult.Errors)
                ModelState.AddModelError("", error.Description);
            return ValidationProblem();
        }

        //Логин с помощью Cookie
        var result = await _signInManager.PasswordSignInAsync(
           user, registerModel.Password, registerModel.RememberMe, false);

        if (result.Succeeded)
        {
            return Ok();
        }

        return LoginFail(result);

    }

    /// <summary>
    /// Возвращает полную информацию о текущем залогиненном пользователе
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        //Достаем email, который является уникальным юзернеймом, из клеймов httpcontext
        string? email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);

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

    /// <summary>
    /// Выход пользователя.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        //Убирает из браузера Cookie с айди сессии
        await _signInManager.SignOutAsync();
        return Ok();
    }

}
