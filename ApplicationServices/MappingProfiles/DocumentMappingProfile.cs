using AutoMapper;
using DomainCore.Entities;
using ApplicationServices.DTOs;

namespace ApplicationServices.Mapping
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {

            CreateMap<Document, DocumentDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.DateUploaded, opt => opt.MapFrom(src => src.DateUploaded));

            CreateMap<DocumentDTO, Document>()
                .ForMember(dest => dest.Hash, opt => opt.Ignore()) 
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
                .ForMember(dest => dest.User, opt => opt.Ignore()) 
                .ForMember(dest => dest.SavedName, opt => opt.Ignore()) 
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.DateUploaded, opt => opt.MapFrom(src => src.DateUploaded))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url));

            CreateMap<Document, DocumentCreateDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.SavedName, opt => opt.MapFrom(src => src.SavedName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.DateUploaded, opt => opt.MapFrom(src => src.DateUploaded));

            CreateMap<Document, DocumentDownloadDTO>()
                .ForMember(dest => dest.SavedName, opt => opt.MapFrom(src => src.SavedName));


        }

    }
}
