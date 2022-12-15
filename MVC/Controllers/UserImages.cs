using AutoMapper;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RazorPages.Identity.Classes;
using RazorPages.Models.Api;
using RazorPages.Models.Implementations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RazorPages.Controllers
{

    public enum ImageType
    {
        InGallery,
        InHeap,
        OnSale
    }

	[Route("api/{controller}/{userName}")]
	[ApiController]
	public class UserImagesController : ControllerBase
	{
		[BindProperty(SupportsGet = true)]
		public string UserName { get; set; }

		readonly IMapper _mapper;
		readonly UserManager<User> _userManager;
		readonly MyHelper _helper;
		readonly INeuroImageRepository _imageStore;

		public UserImagesController(IMapper mapper, UserManager<User> userManager, MyHelper helper, INeuroImageRepository imageStore)
		{
			_mapper = mapper;
			_userManager = userManager;
			_helper = helper;
			_imageStore = imageStore;
		}


		// GET: api/UserImages/{imageType}
		[HttpGet("{imageType=InGallery}")]
		public async Task<IActionResult> GetAsync(ImageType imageType)
		{

			User owner = await _userManager.FindByNameAsync(UserName);
			if (owner is null)
				return NotFound("User not found");

            if (imageType is ImageType.InHeap)
			{
				User? currentUser = await _helper.GetCurrentUser();

				if (currentUser is null || currentUser != owner)
					return Unauthorized();

				var results = await _imageStore.GetInHeapOfUser(owner.Id);
				var dtos = _mapper.Map<IEnumerable<ImageGetDto>>(results);
				var json = new JsonResult(dtos);

                return Ok(json);
			}

			if (imageType == ImageType.OnSale)
			{
                var results = await _imageStore.GetOnSaleOfUser(owner.Id);
                var dtos = _mapper.Map<IEnumerable<ImageGetDto>>(results);
                var json = new JsonResult(dtos);

                return Ok(json);
            }
			else 
			{
				var results = await _imageStore.GetInGalleryOfUser(owner.Id);
				var dtos = _mapper.Map<IEnumerable<ImageGetDto>>(results);
                var json = new JsonResult(dtos);

                return Ok(json);
			}

        }


/*
		// GET api/<UserImages>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<UserImages>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<UserImages>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<UserImages>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}*/
	}
}
