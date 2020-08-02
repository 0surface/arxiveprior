using AutoMapper;
using Journal.API.Application.Models.Subject;
using Journal.Domain.AggregatesModel.SubjectAggregate;

namespace Journal.API
{
    public class JournalApiAutoMapperProfile : Profile
    {
        public JournalApiAutoMapperProfile()
        {
            #region Subject

            CreateMap<Subject, SubjectItem>();
            CreateMap<Subject, SubjectSummaryItem>();

            #endregion Subject

        }
    }
}
