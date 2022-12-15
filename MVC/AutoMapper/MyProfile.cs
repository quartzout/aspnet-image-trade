using AutoMapper;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using RazorPages.Models.Api;
using RazorPages.Models.Classes.UI;
using RazorPages.Models.Implementations;

namespace RazorPages.Models.Classes.AutoMapper;

public class MyProfile : Profile
{

	public MyProfile()
    { 

        CreateMap<NeuroImageInfo, ImageInfoModelPOST>()
			.ReverseMap()
			    .ForMember(dest => dest.IsInGallery, opt => opt.MapFrom((_, _, _, context) => context.Items["IsInGallery"]));
        
        CreateMap<NeuroImageInfo, ImageModelGET>();
        CreateMap<NeuroImageResult, ImageModelGET>()
            .IncludeMembers(src => src.Info);

        CreateMap<NeuroImageInfo, ImageGetDto>();
        CreateMap<NeuroImageResult, ImageGetDto>()
            .IncludeMembers(src => src.Info);

    }


}
