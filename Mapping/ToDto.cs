using AutoMapper;
using TinyUrl.Models;
using TinyUrl.Models.Dto;

namespace TinyUrl.Mapping
{
    public class ToDto : Profile
    {
        public ToDto()
        {
            CreateMap<User, UserDtoOut>();
               
        }
    }
}
