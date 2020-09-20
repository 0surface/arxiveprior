using arx.Extract.API.Services;
using arx.Extract.Data.Entities;
using arx.Extract.Types;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;

namespace arx.Extract.API
{
    public class ApiAutoMapperProfile : Profile
    {
        public ApiAutoMapperProfile()
        {
            CreateMap<PublicationItemEntity, PublicationItem>()
                .ForMember(m => m.Authors, opt => opt.MapFrom<AuthorListAggregateResolver>());

            CreateMap<FulfillmentEntity, Fulfillment>();
            CreateMap<FulfillmentItemEntity, FulfillmentItem>();

            CreateMap<PublicationItemEntity, Publication>().ConvertUsing<PublicationItemEntityToPublicationConverter>();

        }



        public class AuthorListAggregateResolver : IValueResolver<PublicationItemEntity, PublicationItem, List<string>>
        {
            public List<string> Resolve(PublicationItemEntity source, PublicationItem destination, List<string> destMember, ResolutionContext context)
            {
                if (source?.Authors != null && source.Authors.Count > 0)
                    destination.Authors.AddRange(source.Authors);

                if (source?.AuthorSpillOverList != null && source.AuthorSpillOverList.Count > 0)
                    destination.Authors.AddRange(source.AuthorSpillOverList);

                return destination.Authors;
            }
        }

        public class PublicationItemEntityToPublicationConverter : ITypeConverter<PublicationItemEntity, Publication>
        {
            public Publication Convert(PublicationItemEntity source, Publication destination, ResolutionContext context)
            {
                var publication = new Publication()
                {
                    Abstract = source.Abstract,
                    AcmCodes = source.AcmCodes,
                    ArxivId = source.ArxivId,
                    AuthorListTruncated = source.AuthorListTruncated,
                    Comment = source.Comment,
                    Doi = source.Doi,
                    DoiLinks = source.DoiLinks,
                    JournalReference = source.JournalReference,
                    MscCodes = source.MscCodes,
                    PrimarySubjectCode = source.PrimarySubjectCode,
                    PublishedDate = Timestamp.FromDateTime(source.PublishedDate),
                    UpdatedDate = Timestamp.FromDateTime(source.UpdatedDate),
                    Title = source.Title
                };

                foreach (var author in source?.Authors ?? new List<string>())
                {
                    publication.Authors.Add(context.Mapper.Map<string>(author));
                }

                if (source.AuthorSpillOverList.Count > 0)
                {
                    foreach (var author in source?.AuthorSpillOverList ?? new List<string>())
                    {
                        publication.Authors.Add(context.Mapper.Map<string>(author));
                    }
                }

                foreach (var code in source?.SubjectCodes ?? new List<string>())
                {
                    publication.SubjectCodes.Add(context.Mapper.Map<string>(code));
                }

                return publication;
            }
        }
    }
}
