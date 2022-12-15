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
using static DataAccessLibrary.Interfaces.INeuroImageRepository;
using DataAccessLibrary.Models;
using static DataAccessLibrary.Interfaces.INeuroImageStoredInfoRepository;
using RazorPages.Identity.Classes;

namespace DataAccessLibrary;

public class NeuroImageRepository : INeuroImageRepository
{
    private readonly INeuroImageStoredInfoRepository _infoRepository;
    private readonly IFileRepository _fileRepository;
    public readonly IMapper _mapper;

    public class NiNotFoundException : Exception
    {
        public NiNotFoundException() { }
        public NiNotFoundException(string message) : base(message) { }
    }

    public NeuroImageRepository(INeuroImageStoredInfoRepository infoRepository, IFileRepository fileRepository)
    {
        _infoRepository = infoRepository;
        _fileRepository = fileRepository;

        var config = new MapperConfiguration(cfg =>
        cfg.CreateMap<PathedNeuroImageResult, NeuroImageResult>()
            .ForMember(dest => dest.FullName, opt=> opt.MapFrom(src => _fileRepository.GetPath(src.Filename) ))
        );

        _mapper = config.CreateMapper();
    }


    public async Task<NeuroImageResult> StoreCopy(string copyFrom, NeuroImageInfo info, bool deleteOriginalImage = false)
    {

        //Saving to file repository
        var fileSaveResult = _fileRepository.StoreCopy(copyFrom, deleteOriginal: deleteOriginalImage);

        //Create StoredNeuroImage
        PathedNeuroImage storedImage = new(fileSaveResult.newFileName, info);

        //Saving to StoredInfo Repository
        var pathedImageResult = await _infoRepository.Create(storedImage);

        //Building NeuroImageResult
        var result = _mapper.Map<NeuroImageResult>(pathedImageResult);

        return result;
    }


    public async Task Delete(int id)
    {
        var pathedImageResult = await _infoRepository.Get(id);

        if (pathedImageResult == null) throw new NiNotFoundException();

        _fileRepository.Delete(pathedImageResult.Filename);

        await _infoRepository.Delete(id);
    }


    public async Task<NeuroImageResult> GetById(int id)
    {
       var pathedImageResult = await _infoRepository.Get(id);

        return _mapper.Map<NeuroImageResult>(pathedImageResult);
    }


    public async Task<IEnumerable<NeuroImageResult>> GetAll() {
        var pathedResults = await _infoRepository.GetAll();
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }



    public async Task<IEnumerable<NeuroImageResult>> GetAllOnSale()
    {
        var pathedResults = await _infoRepository.GetAllOnSale();
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }


    public async Task<IEnumerable<NeuroImageResult>> GetInGalleryOfUser(string userID)
    {
        var pathedResults = await _infoRepository.GetInGalleryOfUser(userID);
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }

    public async Task<IEnumerable<NeuroImageResult>> GetInHeapOfUser(string userID)
    {
        var pathedResults = await _infoRepository.GetInHeapOfUser(userID);
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }


    public async Task<IEnumerable<NeuroImageResult>> GetOnSaleOfUser(string userID)
    {
        var pathedResults = await _infoRepository.GetOnSaleOfUser(userID);
        return _mapper.Map<IEnumerable<PathedNeuroImageResult>, IEnumerable<NeuroImageResult>>(pathedResults);
    }




    public async Task<NeuroImageResult> UpdateInfo(int idToUpdate, NeuroImageInfo newInfo)
    {
        //Getting record that needs to be replaced
        var oldImageResult = await _infoRepository.Get(idToUpdate);

        //Create new StoredImage
        PathedNeuroImage newStoredImage = new(oldImageResult.Filename, newInfo);

        //Replacing
        var pathedResult =  await _infoRepository.Replace(idToUpdate, newStoredImage);

        //Create result
        return _mapper.Map<NeuroImageResult>(pathedResult);
    }

    public async Task<NeuroImageResult> UpdateFile(int idToUpdate, string copyFrom, bool deleteOriginal = false)
    {
        //Getting record that needs to be replaced
        var oldImageResult = await _infoRepository.Get(idToUpdate);

        //Replacing file in fileRepository
        var fileSaveResult = _fileRepository.Replace(copyFrom, oldImageResult.Filename, deleteOriginal);

        //Create new StoredImage                 
        PathedNeuroImage newStoredImage = new(fileSaveResult.newFileName, oldImageResult.Info);                   

        //Replacing
        var pathedResult = await _infoRepository.Replace(idToUpdate, newStoredImage);

        //Create result
        return _mapper.Map<NeuroImageResult>(pathedResult);
    }


}
