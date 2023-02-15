using API.Interfaces;
using API.Models;
using AutoMapper;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Users.Identity.Classes;
using Webapp174.Models.Interfaces;
using static DataAccessLibrary.Classes.InfoStorage;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace API.Controllers;


/// <summary>
/// Контроллер, реализующий возможные действия залогиненного пользователя с изображениями
/// </summary>
[Route("api/{controller}/{action}")]
[ApiController]
[Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
public class UserActionsController : ControllerBase
{
    private readonly IPictureGenerator _generator;
    private readonly INeuroImageStorage _storage;
    private readonly IMapper _mapper;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly UserManager<User> _userManager;


    public UserActionsController(IPictureGenerator generator, INeuroImageStorage storage, IMapper mapper, UserManager<User> userManager, ICurrentUserProvider currentUserProvider) : base()
    {
        _generator = generator;
        _storage = storage;
        _mapper = mapper;
        _userManager = userManager;
        _currentUserProvider = currentUserProvider;
    }


    [HttpPost]
    //Генерирует новое изображение со стандартными данными, сохраняет его в бд и возвращает
    public async Task<IActionResult> Generate()
    {

        //Берем текущего пользователя
        var user = await _currentUserProvider.GetCurrentUser();
        if (user == null)
            return Unauthorized();

        //Если он банкрот, возаращаем ошибку
        if (user.CoinBalance == 0)
        {
            ModelState.AddModelError("", "Not enough coins on balance");
            return ValidationProblem();
        }

        //Отнимаем монету и сохраняем пользователя
        user.CoinBalance--;
        await _userManager.UpdateAsync(user);

        //Генерируем файл картинки, сохраняем его с помощью INeuroImageStorage с минимальным количеством информации
        string generatedPicturePath = _generator.GeneratePicture();

        NeuroImageInfo newlyGeneratedInfo = new(
            isInGallery: false,
            generationDate: DateTime.Now,
            ownerId: user.Id);
        var imageSaveResult = await _storage.StoreCopy(generatedPicturePath, newlyGeneratedInfo, deleteOriginal: true);

        //Маппим вернувшийся из INeuroImageStorage обьект сохраненного изображения в плоский дто и возвращаем
        return Ok(_mapper.Map<ImageGetDto>(imageSaveResult));
    }



    [HttpPost]
    //Изменяет информацию о существующем изображении пользователя
    public async Task<IActionResult> UpdateImageInfo([FromBody] ImageInfoModelPOST newImageInfo)
    {

        //Берем текущего пользователя
        var user = await _currentUserProvider.GetCurrentUser();
        if (user == null)
            return Unauthorized();

        //Если пользователь установил галочку "выставить на продажу", цена не должна быть нулевой или отсутствовать
        if (newImageInfo.IsOnSale && (newImageInfo.Price is null || newImageInfo.Price == 0))
        {
            ModelState.AddModelError(nameof(newImageInfo.Price), "Price should be set and shouldnt be zero");
            return ValidationProblem();
        }

        // Достаем из бд изображение, которое пользователь редактирует
        NeuroImageResult image;
        try { image = await _storage.GetById(newImageInfo.Id); }
        catch (SqlReadException) { return NotFound("Image not found"); }

        // Проверяем авторство
        if (user.Id != image.Info.OwnerId)
            return Unauthorized();

        //Маппинг из ImageInfoModelPOST в NeuroImageInfo происходит вручную, однако можно вместо этого обьявить мап 
        //из первой модели в последнюю и как-то настроить его так, чтобы он мог совмещать две NeuroImageInfo в одну
        image.Info.Name = newImageInfo.Name;
        image.Info.Description = newImageInfo.Description;
        image.Info.IsOnSale = newImageInfo.IsOnSale;
        image.Info.Price = newImageInfo.Price;
        image.Info.IsInGallery = true;

        await _storage.UpdateInfo(newImageInfo.Id, image.Info);

        return Ok();
    }

}