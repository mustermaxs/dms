using AutoMapper;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;

namespace DMS.Application.DTOs;

public class DmsMappingProfile : Profile
{
    public DmsMappingProfile()
    {
        CreateMap<Tag, TagDto>();
        CreateMap<Tag, CreateTagDto>();
        CreateMap<DmsDocument, DmsDocumentDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
        CreateMap<DmsDocument, UploadDocumentDto>();
        CreateMap<DmsDocument, DocumentSearchResultDto>();
        CreateMap<DmsDocument, CreateDocumentDto>();
        CreateMap<DocumentTag, TagDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Tag.Label))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Tag.Value))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Tag.Color));
        CreateMap<DmsDocument, DocumentContentDto>()
            .ForMember(dest => dest.Content, 
                opt => 
                    opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}