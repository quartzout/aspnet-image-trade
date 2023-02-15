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
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentUserController : ControllerBase
    {
        private ICurrentUserProvider _currentUserProvider;
        private IMapper _mapper;


        public CurrentUserController(ICurrentUserProvider currentUserProvider, IMapper mapper)
        {
            _currentUserProvider = currentUserProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Возвращает полную информацию о текущем залогиненном пользователе
        /// </summary>
        [Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
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
    }
}
