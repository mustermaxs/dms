using DMS.Domain.ValueObjects;

namespace DMS.Infrastructure;
using AutoMapper;
using Domain.Entities.DmsDocument;
using Domain.Entities.Tags;
using Models;
public class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<Tag, TagModel>()
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ReverseMap();
        
        CreateMap<DmsDocument, DocumentModel>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => new FileTypeModel(src.Title)))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UploadDateTime, opt => opt.MapFrom(src => src.UploadDateTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ModificationDateTime, opt => opt.MapFrom(src => src.ModificationDateTime))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) =>
            {
                var documentId = src.Id;
                var documentModelTags = src.Tags?.Select(tag =>
                    new DocumentTagModel(dest, new TagModel(tag.Id, tag.Label, tag.Value, tag.Color)));
                return documentModelTags;
            }));
        
        CreateMap<DocumentModel, DmsDocument>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.DocumentType, opt => opt.MapFrom((src) => new FileType(src.Title)))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.UploadDateTime, opt => opt.MapFrom(src => src.UploadDateTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ModificationDateTime, opt => opt.MapFrom(src => src.ModificationDateTime))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) =>
            {
                var tags = src.Tags?.Select(tag => new Tag(tag.Tag.Id, tag.Tag.Label, tag.Tag.Value, tag.Tag.Color));
                return tags;
            }));
        
        CreateMap<DocumentTagModel, Tag>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Tag.Value))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Tag.Label))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Tag.Color))
            .ReverseMap();
    }
}