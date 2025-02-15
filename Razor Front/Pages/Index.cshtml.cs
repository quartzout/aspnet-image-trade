﻿using System.Net.Mime;
using AutoMapper;
using Images.Interfaces;
using Images.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mvc.Models;
using RazorPages.Models.Classes.UI;
using RazorPages.Models.Implementations;
using Webapp174.Models.Interfaces;
using Webapp174.Models.Mocks;

namespace Webapp174;

public class ListOfImages : PageModel
{
    private readonly IImageStorage _storage;
    private readonly MyHelper _helper;
    private readonly IMapper _mapper;
    private readonly IPictureGenerator _gen;

    public ListOfImages(IImageStorage storage, MyHelper helper, IMapper mapper, IPictureGenerator gen)
    {
        _storage = storage;
        _helper = helper;
        _mapper = mapper;
        _gen = gen;
    }

    public List<ImageGetDto> GETImages { get; set; } = new();

    public async Task OnGet()
    {
//        var ImageResults = await _storage.GetAllOnSale();

     /*   foreach (var imageResult in ImageResults)
        {
            var infoVisual = _mapper.Map<ImageInfoModelBOTH>(imageResult.Info);
            string fileFullName = _helper.FilesystemToWeb(imageResult.FullName);
            GETImages.Add(new ImageModelGET(infoVisual, fileFullName));
        }
*/
    }


}