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
        /// <summary>
        /// Azure Table Storage Cell Size.
        /// String values may be up to 64 KB in size. Note that the maximum number of characters supported is about 32 K or less.
        /// </summary>      
        const int AUTHOR_CELL_CHAR_COUNT_LIMIT = 32000;
        public BackgroundTasksAutoMapperProfile()
        {
            CreateMap<PublicationItem, PublicationItemEntity>()
                .ForMember(m => m.RowKey, opt => opt.MapFrom(n => n.ArxivId))
                .ForMember(m => m.Authors, opt => opt.MapFrom(new AuthorListSplitResolver(0, AUTHOR_CELL_CHAR_COUNT_LIMIT)))
                .ForMember(m => m.AuthorSpillOverList, opt => opt.MapFrom(new AuthorListSplitResolver(1, AUTHOR_CELL_CHAR_COUNT_LIMIT)))
                .ForMember(m => m.AuthorListTruncated, opt => opt.MapFrom(new AuthorListTruncatedResolver()));

            CreateMap<PublicationItemEntity, PublicationItem>();

            CreateMap<Author, string>().ReverseMap();
        }
    }

    public class AuthorListTruncatedResolver : IValueResolver<PublicationItem, PublicationItemEntity, bool>
    {
        public AuthorListTruncatedResolver() { }

        /// <summary>
        /// Return false if there are few authors in PublicationItemEntity's Authors & AuthorSpillOverList Lists.
        /// Otherwise return true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="destMember"></param>
        /// <param name="context"></param>
        /// <returns>bool</returns>
        public bool Resolve(PublicationItem source, PublicationItemEntity destination, bool destMember, ResolutionContext context)
            => (destination?.AuthorSpillOverList?.Count + destination?.Authors?.Count) - source?.Authors?.Count < 0;        
    }


    public class AuthorListSplitResolver : IValueResolver<PublicationItem, PublicationItemEntity, List<string>>
    {
        private readonly int _collectionIndex;
        private readonly int _authorsCellCharCountLimit;

        public AuthorListSplitResolver(int collectionIndex, int authorsCellCharCountLimit)
        {
            _collectionIndex = collectionIndex;
            _authorsCellCharCountLimit = authorsCellCharCountLimit;
        }
        
        public List<string> Resolve(PublicationItem source, PublicationItemEntity destination, List<string> destMember, ResolutionContext context)
        {
            ///Guesstimate, tosave processing resources 
            ///30 Author Names with Affliations can be safely put into one cell
            if (_collectionIndex == 0 && source.Authors.Count < 30)
                return source.Authors;
            else if (_collectionIndex > 0 && source.Authors.Count < 30)
                return new List<string>();

            int _authorsCountLimit
            = CalculateAuthorsPerCell(source.Authors, _authorsCellCharCountLimit);


            if (_collectionIndex == 0)
            {
                ///Avoid Performance hit of Linq.Skip()
                ///https://github.com/dotnet/runtime/blob/master/src/libraries/System.Linq/src/System/Linq/Skip.cs
                return source.Authors.Take(_authorsCountLimit).ToList();
            }
            else
            {
                return source.Authors
                        ?.Skip(_collectionIndex * _authorsCountLimit)
                        ?.Take(_authorsCountLimit)
                        ?.ToList();
            }
        }

        /// <summary>
        /// Precodition  - input list is not null & has elements.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>int</returns>
        private int CalculateAuthorsPerCell(List<string> list, int limit)
        {
            /* Chars Size = Content Count + 
             * (2 Double quotes, 1comma char) per string when json encoded
             * + 2 chars [ ] */
            int acc = 2;
            int i = 0;
            foreach (var item in list)
            {
                i++;
                acc += item.Length + 3;
                if (acc >= limit)
                {
                    return i;
                }
            }
            return list.Count;
        }
    }
}
