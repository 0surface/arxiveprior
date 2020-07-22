using arx.Extract.Data.Entities;
using arx.Extract.Types;
using AutoMapper;

namespace arx.Extract.BackgroundTasks
{
    public class BackgroundTasksAutoMapperProfile : Profile
    {
        public BackgroundTasksAutoMapperProfile()
        {
            CreateMap<PublicationItem, PublicationItemEntity>().ReverseMap();
        }
    }
}
