using DMS.Domain.Entities.Documents;
using DMS.Domain.Entities.Tags;
using DMS.Infrastructure.ValueObjects;
using AutoMapper;
using DMS.Domain.Entities;
using DMS.Infrastructure.Entities;
using DMS.Infrastructure.ValueObjects;
using AutoMapper.Extensions.EnumMapping;
using DMS.Domain.ValueObjects;

namespace DMS.Infrastructure.Mapping
{

    public class InfrastructureApplicationMappingProfile : Profile
    {
        public InfrastructureApplicationMappingProfile()
        {
            // Domain <-> Infrastructure Mapping
            CreateMap<ProcessingStatusModel, Domain.ValueObjects.ProcessingStatus>()
                .ConvertUsing((value, destination) =>
                {
                    switch (value)
                    {
                        case ProcessingStatusModel.Pending:
                            return ProcessingStatus.Pending;
                        case ProcessingStatusModel.Failed:
                            return ProcessingStatus.Failed;
                        case ProcessingStatusModel.Finished:
                            return ProcessingStatus.Finished;
                        case ProcessingStatusModel.NotStarted:
                            return ProcessingStatus.NotStarted;
                        default:
                            return ProcessingStatus.Failed;
                    }
                });

            CreateMap<DmsDocument, DocumentModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.UploadDateTime, opt => opt.MapFrom(src => src.UploadDateTime))
                .ForMember(dest => dest.ModificationDateTime, opt => opt.MapFrom(src => src.ModificationDateTime))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) =>
                {
                    var documentId = src.Id;
                    var docModelTags = src.Tags?.Select(tag =>
                        new DocumentTagModel { DocumentId = documentId, Id = Guid.NewGuid(), TagId = tag.Id });
                    return docModelTags;
                }))
                .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FileExtension))
                .ForMember(dest => dest.StatusModel, opt => opt.MapFrom(src => src.Status));

            CreateMap<DocumentModel, DmsDocument>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.UploadDateTime, opt => opt.MapFrom(src => src.UploadDateTime))
                .ForMember(dest => dest.ModificationDateTime, opt => opt.MapFrom(src => src.ModificationDateTime))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) =>
                {
                    var tags = src.Tags?.Select(tag => new Tag(tag.TagId, tag.TagModels.Label, tag.TagModels.Value, tag.TagModels.Color)).ToList();
                    return tags;
                }))
                .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FileExtension))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StatusModel));

            CreateMap<Tag, TagModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                .ReverseMap();
        }
    }
}
