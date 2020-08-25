using arx.Extract.API.Services;
using AutoMapper;
using Journal.BackgroundTasks.Types;
using System.Collections.Generic;

namespace Journal.BackgroundTasks
{
    public class JournalTasksAutoMapperProfile : Profile
    {
        public JournalTasksAutoMapperProfile()
        {
            CreateMap<Publication, PublicationItemDto>()
                .ForMember(m => m.PublishedDate, opt => opt.MapFrom(x => x.PublishedDate.ToDateTime()))
                .ForMember(m => m.UpdatedDate, opt => opt.MapFrom(x => x.UpdatedDate.ToDateTime()))
                .ForMember(m => m.SubjectCodes, opt => opt.MapFrom<GrpcSubjectCodesResolver>())
                .ForMember(m => m.Authors, opt => opt.MapFrom<GrpcAuthorListResolver>());
        }
        public class GrpcSubjectCodesResolver : IValueResolver<Publication, PublicationItemDto, List<string>>
        {
            public List<string> Resolve(Publication source, PublicationItemDto destination, List<string> destMember, ResolutionContext context)
            {
                if (source.SubjectCodes != null && source.SubjectCodes.Count > 0)
                {
                    destination.SubjectCodes.AddRange(source.SubjectCodes);
                }
                return destination.SubjectCodes;
            }
        }

        public class GrpcAuthorListResolver : IValueResolver<Publication, PublicationItemDto, List<string>>
        {
            public List<string> Resolve(Publication source, PublicationItemDto destination, List<string> destMember, ResolutionContext context)
            {
                if (source.Authors != null && source.Authors.Count > 0)
                {
                    destination.Authors.AddRange(source.Authors);
                }
                return destination.Authors;
            }
        }
    }
}
