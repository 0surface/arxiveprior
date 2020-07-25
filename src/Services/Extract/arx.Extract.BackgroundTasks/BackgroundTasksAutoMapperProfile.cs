using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTasksAutoMapperProfile : Profile
    {
        const int AUTHOR_COUNT_SPILL_OVER_LIMIT = 400;
        public BackgroundTasksAutoMapperProfile()
        {
            CreateMap<PublicationItem, PublicationItemEntity>()
                .ForMember(m => m.RowKey, opt => opt.MapFrom(n => n.ArxivId))
                .ForMember(m => m.Authors, opt => opt.MapFrom(new AuthorListSplitResolver(0, AUTHOR_COUNT_SPILL_OVER_LIMIT)))
                .ForMember(m => m.AuthorSpillOverListOne, opt => opt.MapFrom(new AuthorListSplitResolver(1, AUTHOR_COUNT_SPILL_OVER_LIMIT)))
                .ForMember(m => m.AuthorSpillOverListTwo, opt => opt.MapFrom(new AuthorListSplitResolver(2, AUTHOR_COUNT_SPILL_OVER_LIMIT)))
                .ForMember(m => m.AuthorSpillOverListThree, opt => opt.MapFrom(new AuthorListSplitResolver(3, AUTHOR_COUNT_SPILL_OVER_LIMIT)))
                .ForMember(m => m.AuthorListTruncated, opt => opt.MapFrom(new AuthorListTruncatedResolver(AUTHOR_COUNT_SPILL_OVER_LIMIT)));

            CreateMap<PublicationItemEntity, PublicationItem>();

            CreateMap<Author, AuthorItem>().ReverseMap();
        }
    }

    public class AuthorListTruncatedResolver : IValueResolver<PublicationItem, PublicationItemEntity, bool>
    {
        private readonly int _authorsCountLimit;
        public AuthorListTruncatedResolver(int authorsCountLimit)
        {
            _authorsCountLimit = authorsCountLimit;
        }

        public bool Resolve(PublicationItem source, PublicationItemEntity destination, bool destMember, ResolutionContext context)
        {
            return source?.Authors?.Count() > (4 * _authorsCountLimit);
        }
    }


    public class AuthorListSplitResolver : IValueResolver<PublicationItem, PublicationItemEntity, List<AuthorItem>>
    {        
        private readonly int _collectionIndex;
        private readonly int _authorsCountLimit;

        public AuthorListSplitResolver(int collectionIndex, int authorsCountLimit)
        {
            _collectionIndex = collectionIndex;
            _authorsCountLimit = authorsCountLimit;
        }
        public List<AuthorItem> Resolve(PublicationItem source, PublicationItemEntity destination, List<AuthorItem> destMember, ResolutionContext context)
        {
            if(_collectionIndex == 0 && source.Authors.Count() > 0)
            {
                return source?.Authors?.Take(_authorsCountLimit)?.ToList();
            }
            else
            {
                return source?.Authors
                        ?.Skip(_collectionIndex * _authorsCountLimit)
                        ?.Take(_authorsCountLimit)
                        ?.ToList();
            }
        }
    }
}
