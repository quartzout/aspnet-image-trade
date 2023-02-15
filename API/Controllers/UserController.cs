using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Users.Identity.Classes;

namespace API.Controllers
{
    [Route("api/[controller]/{action}/{email=''}")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ICurrentUserProvider _currentUserProvider;
        private IMapper _mapper;
        private UserManager<User> _userManager;

        public UserController(ICurrentUserProvider currentUserProvider, IMapper mapper, UserManager<User> userManager)
        {
            _currentUserProvider = currentUserProvider;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Возвращает полную информацию о текущем залогиненном пользователе
        /// </summary>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Current()
        {

            var user = await _currentUserProvider.GetCurrentUser();
            if (user == null)
            {
                ModelState.AddModelError("", "Cannot find user");
                return ValidationProblem();
            }

            //Маппим пользователя в плоский дто для отправки и возвращаем
            return Ok(_mapper.Map<User, UserGetDto>(user));

        }

        /// <summary>
        /// Возвращает полную информацию о пользователе по email
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Find(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Cannot find user");
                return ValidationProblem();
            }

            //Маппим пользователя в плоский дто для отправки и возвращаем
            return Ok(_mapper.Map<User, UserGetDto>(user));

        }

        [HttpGet]
        public IActionResult All()
        {
            var users = _userManager.Users.ToList();
            return Ok(_mapper.Map<IEnumerable<User>, IEnumerable<UserGetDto>>(users));
        }
    }
}
