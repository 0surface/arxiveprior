﻿using arx.Extract.Data.Entities;
using arx.Extract.Types;
using AutoMapper;
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
        }

        public class AuthorListAggregateResolver : IValueResolver<PublicationItemEntity, PublicationItem, List<string>>
        {
            public List<string> Resolve(PublicationItemEntity source, PublicationItem destination, List<string> destMember, ResolutionContext context)
            {
                if (source?.Authors != null && source.Authors.Count > 0)
                    destination.Authors.AddRange(source.Authors);

                if (source?.AuthorSpillOverListOne != null && source.AuthorSpillOverListOne.Count > 0)
                    destination.Authors.AddRange(source?.AuthorSpillOverListOne);

                if (source?.AuthorSpillOverListOne != null && source.AuthorSpillOverListOne.Count > 0)
                    destination.Authors.AddRange(source.AuthorSpillOverListTwo);

                if (source?.AuthorSpillOverListOne != null && source.AuthorSpillOverListOne.Count > 0)
                    destination.Authors.AddRange(source.AuthorSpillOverListThree);

                return destination.Authors;
            }
        }
    }
}
