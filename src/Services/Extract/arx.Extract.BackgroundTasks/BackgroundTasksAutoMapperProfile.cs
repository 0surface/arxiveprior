using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTasksAutoMapperProfile : Profile
    {
        public BackgroundTasksAutoMapperProfile()
        {
            CreateMap<PublicationItem, PublicationItemEntity>()
                .ForMember(m => m.PartitionKey, opt => opt.MapFrom(n => n.PrimarySubjectCode))
                .ForMember(m => m.RowKey, opt => opt.MapFrom(n => n.ArxivId));
            CreateMap<PublicationItemEntity, PublicationItem>();
            CreateMap<Author, AuthorItem>().ReverseMap();
        }
    }
}
