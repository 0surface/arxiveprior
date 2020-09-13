using arx.Extract.API.Services;
using arx.Extract.Data.Entities;
using arx.Extract.Types;
using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;
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
            CreateMap<PublicationItemEntity, Publication>()
                .ForMember(m => m.PublishedDate, opt => opt.MapFrom(entity => Timestamp.FromDateTime(entity.PublishedDate)))
                .ForMember(m => m.UpdatedDate, opt => opt.MapFrom(entity => Timestamp.FromDateTime(entity.UpdatedDate)))
                .ForMember(m => m.SubjectCodes, opt => opt.MapFrom<GrpcSubjectCodesResolver>())
                .ForMember(m => m.Authors, opt => opt.MapFrom<GrpcAuthorListAggregateResolver>());
                //.ForMember(m => m.MscCodes, opt => opt.MapFrom(e => e.MscCodes));
        }

        public class GrpcSubjectCodesResolver : IValueResolver<PublicationItemEntity, Publication, Google.Protobuf.Collections.RepeatedField<string>>
        {
            public RepeatedField<string> Resolve(PublicationItemEntity source, Publication destination, RepeatedField<string> destMember, ResolutionContext context)
            {
                destination.SubjectCodes.AddRange(source.SubjectCodes);
                return destination.SubjectCodes;
            }
        }

        public class GrpcAuthorListAggregateResolver : IValueResolver<PublicationItemEntity, Publication, Google.Protobuf.Collections.RepeatedField<string>>
        {
            public RepeatedField<string> Resolve(PublicationItemEntity source, Publication destination, RepeatedField<string> destMember, ResolutionContext context)
            {
                if (source?.Authors != null)
                    destination.Authors.AddRange(source.Authors);

                if (source?.AuthorSpillOverList != null)
                    destination.Authors.AddRange(source.AuthorSpillOverList);

                return destination.Authors;
            }
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
    }
}
