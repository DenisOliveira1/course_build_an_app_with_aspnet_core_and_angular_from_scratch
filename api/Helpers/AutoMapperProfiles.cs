using System;
using System.Linq;
using api.DTOs;
using api.Extentions;
using api.Models;
using AutoMapper;

namespace api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserModel, MemberDto>()
                .ForMember(dest => dest.PhotoUrl,
                     opt => opt.MapFrom(str => str.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<PhotoModel, PhotoDto>();
            CreateMap<MemberUpdateDto, UserModel>();
            CreateMap<RegisterDto, UserModel>();

            CreateMap<MessageModel, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, 
                    opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, 
                    opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<MessageDto, MessageModel>();
            // CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc)); // Erro
            CreateMap<GroupModel, GroupDto>().ReverseMap();
            CreateMap<ConnectionModel, ConnectionDto>().ReverseMap();

        }
    }
}