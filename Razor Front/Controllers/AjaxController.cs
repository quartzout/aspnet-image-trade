﻿using AutoMapper;
using Images.Interfaces;
using Images.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using RazorPages.Models.Classes.UI;
using RazorPages.Models.Implementations;
using Users.Identity.Classes;
using Webapp174.Models.Interfaces;
using static Images.Classes.InfoStorage;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace RazorPages.Controllers;

[Route("api/{controller}/{action}")]
[ApiController]
public class AjaxController : ControllerBase
{
    private readonly IPictureGenerator _generator;
    private readonly IImageStorage _storage;
    private readonly MyHelper _helper;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;


    public AjaxController(IPictureGenerator generator, IImageStorage storage, MyHelper helper, IMapper mapper, UserManager<User> userManager) : base()
    {
        _generator = generator;
        _storage = storage;
        _helper = helper;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    //Возвращает баланс монет текущего залогиненного пользователя
    public async Task<IActionResult> GetCoinBalance()
    {
        var user = await _helper.GetCurrentUser();

        if (user is null)
            return Unauthorized();

        return Ok(user.CoinBalance);
    }

    [HttpPost]
    //Генерирует новое изображение со стандартными данными, сохраняет его в бд и возвращает
    public async Task<IActionResult> GenerateNewImage()
    {

        var user = await _helper.GetCurrentUser();

        if (user == null)
            return Unauthorized();

        if (user.CoinBalance == 0)
            return BadRequest("Not Enough coins on balance");

        user.CoinBalance--;
        await _userManager.UpdateAsync(user);

        //Generating file and getting a path to it
        string generatedPicturePath = _generator.GeneratePicture();

        ImageInfo newlyGeneratedInfo = new(
            isInGallery: false,
            generationDate: DateTime.Now,
            ownerId: user.Id);

        var imageSaveResult = await _storage.StoreCopy(generatedPicturePath, newlyGeneratedInfo, deleteOriginal: true);

        return Ok(_mapper.Map<ImageGetDto>(imageSaveResult));
    }



    [HttpPost]
    //Сохраняет существующее бд необработанное (inHeap) изображение в галерею текущему пользователю, добавляя к его
    //информации ту, что была передана телом post-запроса
    public async Task<IActionResult> SaveImageToGallery([FromBody] ImageInfoModelPOST imageInfo)
    {
        var user = await _helper.GetCurrentUserId();

        if (user == null)
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (imageInfo.IsOnSale && (imageInfo.Price is null || imageInfo.Price == 0))
        {
            ModelState.AddModelError(nameof(imageInfo.Price), "Price should be set and shouldnt be zero");
            return BadRequest(ModelState);
        }


        ImageResult image;
        try { image = await _storage.GetById(imageInfo.Id); }
        catch (SqlReadException) { return NotFound("Image not found"); }

        var info = image.Info;

        if (user != info.OwnerId)
            return Unauthorized();

        if (info.IsInGallery)
            return BadRequest("Specified image is already in gallery");

        //Маппинг из ImageInfoModelPOST в ImageInfo происходит вручную, однако можно вместо этого обьявить мап 
        //из первой модели в последнюю и как-то настроить его так, чтобы он мог совмещать две ImageInfo в одну
        info.Name = imageInfo.Name;
        info.Description = imageInfo.Description;
        info.IsInGallery = true;
        info.IsOnSale = false;

        await _storage.UpdateInfo(imageInfo.Id, info);

        return Ok();
    }

}