using API.Interfaces;
using API.Models;
using AutoMapper;
using Images.Interfaces;
using Images.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Users.Identity.Classes;
using static Images.Interfaces.INeuroImageStorage;

namespace API.Controllers
{

	[Route("api/[controller]/{UserName}/{action}")]
	[ApiController]
	public class UserImagesController : ControllerBase
	{

		[BindProperty(SupportsGet = true)]
		public string UserName { get; set; } = string.Empty;

		readonly IMapper _mapper;
		readonly UserManager<User> _userManager;
		readonly INeuroImageStorage _imageStore;
        private ICurrentUserProvider _currentUserProvider;

        public UserImagesController(IMapper mapper, UserManager<User> userManager, INeuroImageStorage imageStore, ICurrentUserProvider currentUserProvider)
        {
            _mapper = mapper;
            _userManager = userManager;
            _imageStore = imageStore;
            _currentUserProvider = currentUserProvider;
        }

        /// <summary>
        /// Вспомогательный метод, возвращающий все изображения пользователя определенного статуса, либо возвращающего
        /// </summary>
        public async Task<IActionResult> GetUserImages(ImageStatus imageStatus)
        {

            User? owner = await _userManager.FindByNameAsync(UserName);
            if (owner is null)
            {
                ModelState.AddModelError("", "User not found");
                return ValidationProblem();
            }

            if (imageStatus is ImageStatus.InHeap)
            {
                User? currentUser = await _currentUserProvider.GetCurrentUser();

                if (currentUser is null || currentUser != owner)
                    return Unauthorized();
            }

            var results = await _imageStore.GetAllOfUserOfStatus(owner.Id, imageStatus);
            var dtos = _mapper.Map<IEnumerable<ImageGetDto>>(results);

            return Ok(new ImagesResultGet { Images = dtos });

        }


        // Получить все изображения в галерее пользователя
        [HttpGet]
        public async Task<IActionResult> InGallery()
		{
            return await GetUserImages(ImageStatus.InGallery);
		}

        // Получить все изображения пользователя, выставленные на продажу.
        [HttpGet]
        public async Task<IActionResult> OnSale()
        {
            return await GetUserImages(ImageStatus.OnSale);
        }

        // Получить все необработанные изображения пользователя. Доступно, только если автором изображений является залогиненный пользователь
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> InHeap()
        {
            return await GetUserImages(ImageStatus.InHeap);
        }

    }
}
