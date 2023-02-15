﻿using API.Models;
using AutoMapper;
using Images.Models;
using Microsoft.Extensions.Configuration;
using Users.Identity.Classes;

namespace API.Automapper.AutoMapper;

public class MyProfile : Profile
{

	public MyProfile()
    { 

       /* CreateMap<NeuroImageInfo, ImageInfoModelPOST>()
			.ReverseMap()
			    .ForMember(dest => dest.IsInGallery, opt => opt.MapFrom((_, _, _, context) => context.Items["IsInGallery"]));
*/

        /*   CreateMap<NeuroImageInfo, ImageModelGET>();
             CreateMap<NeuroImageResult, ImageModelGET>()
                 .IncludeMembers(src => src.Info);*/

        //Маппинг модели, возвращаемой из бд, в get-модель, возвращаемой из api на фронт.
        //Маппинга из post-модели, биндящейся на api в запросах на обновление инфы о картинках (ImageInfoModelPOST) в 
        //бд-модель NeuroImageInfo отсутствует потому, что этот маппинг происходит вручную в методе api-запроса.

        //Чтобы комплексную модель развернуть в плоскую, необходимо сначала определить мап из дочерней модели в плоскую, а затем из
        //родительской в плоскую с указанием дочерней модели в IncludeMembers
        CreateMap<NeuroImageInfo, ImageGetDto>();
        CreateMap<NeuroImageResult, ImageGetDto>()
            .IncludeMembers(src => src.Info);

        //Пользователь в модель, отправляемую по запросу api
        CreateMap<User, UserGetDto>();

    }


}
