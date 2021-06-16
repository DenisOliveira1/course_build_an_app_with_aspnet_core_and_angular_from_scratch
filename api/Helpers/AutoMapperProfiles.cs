using System.Linq;
using api.DTOs;
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
                     opt => opt.MapFrom(str => str.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<PhotoModel, PhotoDto>();
        }
    }
}