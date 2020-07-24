using arx.Extract.Data.Entities;
using arx.Extract.Types;
using AutoMapper;

namespace arx.Extract.API
{
    public class ApiAutoMapperProfile : Profile
    {
        public ApiAutoMapperProfile()
        {
            CreateMap<PublicationItemEntity, PublicationItem>();
        }
    }
}
