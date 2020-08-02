using Journal.Domain.AggregatesModel.SubjectAggregate;
using Journal.Infrastructure;
using Journal.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace Journal.API.Services
{
    public interface ISubjectService
    {
        List<Subject> GetAll();
    }
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;

        public SubjectService(JournalContext context)
        {
            _repo = new SubjectRepository(context);
        }

        public List<Subject> GetAll()
        {
            try
            {
                var result = _repo.GetAllSubjects();
                return result;
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }
    }
}
