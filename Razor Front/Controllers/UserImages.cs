﻿using AutoMapper;
using Images.Interfaces;
using Images.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using RazorPages.Models.Implementations;
using Users.Identity.Classes;
using static Images.Interfaces.IImageStorage;

namespace RazorPages.Controllers
{

	[Route("api/{controller}/{userName}")]
	[ApiController]
	public class UserImagesController : ControllerBase
	{
		[BindProperty(SupportsGet = true)]
		public string UserName { get; set; }

		readonly IMapper _mapper;
		readonly UserManager<User> _userManager;
		readonly MyHelper _helper;
		readonly IImageStorage _imageStore;

		public UserImagesController(IMapper mapper, UserManager<User> userManager, MyHelper helper, IImageStorage imageStore)
		{
			_mapper = mapper;
			_userManager = userManager;
			_helper = helper;
			_imageStore = imageStore;
		}


		// GET: api/UserImages/{imageStatus}
		// Получить все изображения пользователя. тип inHeap может получить только владелец изображений.
		[HttpGet("{imageStatus=InGallery}")]
		public async Task<IActionResult> GetAsync(ImageStatus imageStatus)
		{

			User owner = await _userManager.FindByNameAsync(UserName);
			if (owner is null)
				return NotFound("User not found");


            if (imageStatus is ImageStatus.InHeap)
			{
				User? currentUser = await _helper.GetCurrentUser();

				if (currentUser is null || currentUser != owner)
					return Unauthorized();
			}

			var results = await _imageStore.GetAllOfUserOfStatus(owner.Id, imageStatus);
            var dtos = _mapper.Map<IEnumerable<ImageGetDto>>(results);

            return Ok(dtos);

        }

	}
}
