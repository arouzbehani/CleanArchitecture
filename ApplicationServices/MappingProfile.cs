using AutoMapper;
using DomainCore.Entities;
using ApplicationServices.DTOs;

namespace ApplicationServices.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Ensure Password is not set
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Optionally ignore Id if it should not be set from DTO
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            
            CreateMap<UserCreationDTO, User>();
            CreateMap<UserDTO,UserUpdateDTO>()
                            .ForMember(dest=>dest.Id,opt=>opt.Ignore());

        }

    }
}
