using AutoMapper;
using DomainCore.Entities;
using ApplicationServices.DTOs;

namespace ApplicationServices.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.ProfilePicUrl));


            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore()) // Ensure Password is not set
                .ForMember(dest => dest.Salt, opt => opt.Ignore()) // Ensure Salt is not set
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.ProfilePicUrl))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ProfilePicUrl, opt => opt.Ignore()); // Ignoring ProfilePicUrl if it's not part of UserDTO


            CreateMap<UserCreationDTO, User>()
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore()) // Hashing happens in the service
                .ForMember(dest => dest.Salt, opt => opt.Ignore()); // Salt generation happens in the service
            
            CreateMap<UserDTO, UserUpdateDTO>();

            CreateMap<UserUpdateDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id to prevent it from being updated
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore()) // Ignore Password to prevent accidental overwrite
                .ForMember(dest => dest.Salt, opt => opt.Ignore()) // Ignore Salt for the same reason
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Optional: Ignore Email if it's not updatable
                .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.ProfilePicUrl))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));


        }

    }
}
