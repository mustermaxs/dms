using AutoMapper;
using DMS.Domain.Entities.Documents;
using DMS.Domain.Entities.Tags;

namespace DMS.Application.DTOs;

public class DmsMappingProfile : Profile
{
    public DmsMappingProfile()
    {
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        CreateMap<TagDto, Tag>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        CreateMap<Tag, CreateTagDto>();
        CreateMap<DmsDocument, DmsDocumentDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FileExtension));
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