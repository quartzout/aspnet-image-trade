using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using Microsoft.Extensions.Options;
using DataAccessLibrary.Classes.Options;
using System.Xml.Linq;
using AutoMapper;
using static System.Net.Mime.MediaTypeNames;
using static DataAccessLibrary.Interfaces.INeuroImageStorage;
using DataAccessLibrary.Models;
using static DataAccessLibrary.Interfaces.IInfoStorage;
using Users.Identity.Classes;
using DapperImageStore.Models;

namespace DataAccessLibrary;


/// <summary>
/// Имплементация <see cref="INeuroImageStorage"/> 
/// Использует <see cref="IInfoStorage"/> для хранения информации об изображении,
/// и <see cref="IFileStorage"/> для хранения файлов изображений.
/// </summary>
public class NeuroImageStorage : INeuroImageStorage
{
    private readonly IInfoStorage _infoStorage;
    private readonly IFileStorage _fileStorage;
    public readonly IMapper _mapper;

    public NeuroImageStorage(IInfoStorage infoStorage, IFileStorage fileStorage)
    {
        _infoStorage = infoStorage;
        _fileStorage = fileStorage;

        var config = new MapperConfiguration(cfg =>
        cfg.CreateMap<PathedNeuroImageResult, NeuroImageResult>()
            .ForMember(dest => dest.FullName, opt=> opt.MapFrom(src => _fileStorage.GetPath(src.Filename) ))
        );

        _mapper = config.CreateMapper();
    }


    public async Task<NeuroImageResult> StoreCopy(string copyFrom, NeuroImageInfo info, bool deleteOriginalImage = false)
    {

        //Сохраняем картинку в файловом хранилище, получаем название сохраненного файла
        var fileSaveResult = _fileStorage.StoreCopy(copyFrom, deleteOriginal: deleteOriginalImage);

        //Создаем обьект для передачи в инфо хранилище, название файла берем только что полученное
        PathedNeuroImage storedImage = new(fileSaveResult.newFileName, info);

        //Сохраняем в инфо хранилище
        var pathedImageResult = await _infoStorage.Create(storedImage);

        //Маппим полученный обьект в результат
        var result = _mapper.Map<NeuroImageResult>(pathedImageResult);

        return result;
    }


    public async Task Delete(int id)
    {
        var pathedImageResult = await _infoStorage.Get(id);

        if (pathedImageResult == null) throw new NeuroImageNotFoundException();

        _fileStorage.Delete(pathedImageResult.Filename);

        await _infoStorage.Delete(id);
    }


    public async Task<NeuroImageResult> GetById(int id)
    {
       var pathedImageResult = await _infoStorage.Get(id);

        return _mapper.Map<NeuroImageResult>(pathedImageResult);
    }


    /*    public async Task<IEnumerable<NeuroImageResult>> GetAll() {
            var pathedResults = await _infoStorage.GetAll();
            return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
        }

    */
    /*
        public async Task<IEnumerable<NeuroImageResult>> GetAllOnSale()
        {
            var pathedResults = await _infoStorage.GetAllOnSale();
            return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
        }


        public async Task<IEnumerable<NeuroImageResult>> GetInGalleryOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetInGalleryOfUser(userID);
            return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
        }

        public async Task<IEnumerable<NeuroImageResult>> GetInHeapOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetInHeapOfUser(userID);
            return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
        }


        public async Task<IEnumerable<NeuroImageResult>> GetOnSaleOfUser(string userID)
        {
            var pathedResults = await _infoStorage.GetOnSaleOfUser(userID);
            return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
        }
    */


    public async Task<IEnumerable<NeuroImageResult>> GetAllOfUserOfStatus(string userID, ImageStatus status)
    {
        var pathedResults = await _infoStorage.GetAllOfUserOfStatus(userID, status);
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }


    public async Task<NeuroImageResult> UpdateInfo(int idToUpdate, NeuroImageInfo newInfo)
    {
        //обновляем информацию, оставляя название файла тем же (чтобы оно указывало на тот же самый файл в файловом хранилище),
        //с файлом ничего не делаем

        var oldImageResult = await _infoStorage.Get(idToUpdate);

        //новый объект, который заменит старый
        PathedNeuroImage newStoredImage = new(oldImageResult.Filename, newInfo);
        var pathedResult =  await _infoStorage.Replace(idToUpdate, newStoredImage);

        return _mapper.Map<NeuroImageResult>(pathedResult);
    }

    public async Task<NeuroImageResult> UpdateFile(int idToUpdate, string copyFrom, bool deleteOriginal = false)
    {
        //обновляем файл в файловом хранилище, получаем новое название и обновляем информацию в инфо хранилище,
        //заменив название файла на новое
        var oldImageResult = await _infoStorage.Get(idToUpdate);
        var fileSaveResult = _fileStorage.Replace(copyFrom, oldImageResult.Filename, deleteOriginal);   
        
        PathedNeuroImage newStoredImage = new(fileSaveResult.newFileName, oldImageResult.Info);                   
        var pathedResult = await _infoStorage.Replace(idToUpdate, newStoredImage);

        return _mapper.Map<NeuroImageResult>(pathedResult);
    }


}
