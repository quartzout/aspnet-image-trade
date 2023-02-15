using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Images.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using Microsoft.Extensions.Options;
using Images.Classes.Options;
using System.Xml.Linq;
using AutoMapper;
using static System.Net.Mime.MediaTypeNames;
using static Images.Interfaces.IImageStorage;
using Images.Models;
using static Images.Interfaces.IInfoStorage;
using Users.Identity.Classes;
namespace Images.Classes;


/// <summary>
/// Имплементация <see cref="IImageStorage"/> 
/// Использует <see cref="IInfoStorage"/> для хранения информации об изображении,
/// и <see cref="IFileStorage"/> для хранения файлов изображений.
/// </summary>
public class ImageStorage : IImageStorage
{
    private readonly IInfoStorage _infoStorage;
    private readonly IFileStorage _fileStorage;
    public readonly IMapper _mapper;

    public ImageStorage(IInfoStorage infoStorage, IFileStorage fileStorage)
    {
        _infoStorage = infoStorage;
        _fileStorage = fileStorage;

        var config = new MapperConfiguration(cfg =>
        cfg.CreateMap<PathedImageResult, ImageResult>()
            .ForMember(dest => dest.FullName, opt=> opt.MapFrom(src => _fileStorage.GetPath(src.Filename) ))
        );

        _mapper = config.CreateMapper();
    }


    public async Task<ImageResult> StoreCopy(string copyFrom, ImageInfo info, bool deleteOriginalImage = false)
    {

        //Сохраняем картинку в файловом хранилище, получаем название сохраненного файла
        var fileSaveResult = _fileStorage.StoreCopy(copyFrom, deleteOriginal: deleteOriginalImage);

        //Создаем обьект для передачи в инфо хранилище, название файла берем только что полученное
        PathedImage storedImage = new(fileSaveResult.newFileName, info);

        //Сохраняем в инфо хранилище
        var pathedImageResult = await _infoStorage.Create(storedImage);

        //Маппим полученный обьект в результат
        var result = _mapper.Map<ImageResult>(pathedImageResult);

        return result;
    }


    public async Task Delete(int id)
    {
        var pathedImageResult = await _infoStorage.Get(id);

        if (pathedImageResult == null) throw new ImageNotFoundException();

        _fileStorage.Delete(pathedImageResult.Filename);

        await _infoStorage.Delete(id);
    }


    public async Task<ImageResult> GetById(int id)
    {
       var pathedImageResult = await _infoStorage.Get(id);

        return _mapper.Map<ImageResult>(pathedImageResult);
    }


    /*    public async Task<IEnumerable<ImageResult>> GetAll() {
            var pathedResults = await _infoStorage.GetAll();
            return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
        }

    */
    /*
        public async Task<IEnumerable<ImageResult>> GetAllOnSale()
        {
            var pathedResults = await _infoStorage.GetAllOnSale();
            return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
        }


        public async Task<IEnumerable<ImageResult>> GetInGalleryOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetInGalleryOfUser(userID);
            return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
        }

        public async Task<IEnumerable<ImageResult>> GetInHeapOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetInHeapOfUser(userID);
            return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
        }


        public async Task<IEnumerable<ImageResult>> GetOnSaleOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetOnSaleOfUser(userID);
            return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
        }
    */


    public async Task<IEnumerable<ImageResult>> GetAllOfUserOfStatus(string userID, ImageStatus status)
    {
        var pathedResults = await _infoStorage.GetAllOfUserOfStatus(userID, status);
        return _mapper.Map<IEnumerable<PathedImageResult>, IEnumerable<ImageResult>>(pathedResults);
    }


    public async Task<ImageResult> UpdateInfo(int idToUpdate, ImageInfo newInfo)
    {
        //обновляем информацию, оставляя название файла тем же (чтобы оно указывало на тот же самый файл в файловом хранилище),
        //с файлом ничего не делаем

        var oldImageResult = await _infoStorage.Get(idToUpdate);

        //новый объект, который заменит старый
        PathedImage newStoredImage = new(oldImageResult.Filename, newInfo);
        var pathedResult =  await _infoStorage.Replace(idToUpdate, newStoredImage);

        return _mapper.Map<ImageResult>(pathedResult);
    }

    public async Task<ImageResult> UpdateFile(int idToUpdate, string copyFrom, bool deleteOriginal = false)
    {
        //обновляем файл в файловом хранилище, получаем новое название и обновляем информацию в инфо хранилище,
        //заменив название файла на новое
        var oldImageResult = await _infoStorage.Get(idToUpdate);
        var fileSaveResult = _fileStorage.Replace(copyFrom, oldImageResult.Filename, deleteOriginal);   
        
        PathedImage newStoredImage = new(fileSaveResult.newFileName, oldImageResult.Info);                   
        var pathedResult = await _infoStorage.Replace(idToUpdate, newStoredImage);

        return _mapper.Map<ImageResult>(pathedResult);
    }


}
