using DMS.Domain.Entities.Tags;

namespace DMS.Application.DTOs;
using AutoMapper;
using Domain.Entities.DmsDocument;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ReverseMap();
        CreateMap<Tag, CreateTagDto>();
        CreateMap<DmsDocument, DmsDocumentDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
        CreateMap<DmsDocument, UploadDocumentDto>();
        CreateMap<DmsDocument, DocumentSearchResultDto>();
        CreateMap<DmsDocument, CreateDocumentDto>();
        CreateMap<DmsDocument, DocumentContentDto>()
            .ForMember(dest => dest.Content, 
                opt => 
                    opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}