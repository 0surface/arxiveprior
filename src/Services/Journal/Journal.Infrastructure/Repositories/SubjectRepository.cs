using Journal.Domain.AggregatesModel.SubjectAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journal.Infrastructure.Repositories
{
    public interface ISubjectRepository
    {
        List<Subject> GetAllSubjects();
    }
    public class SubjectRepository : ISubjectRepository
    {
        private readonly JournalContext _context;

        public SubjectRepository(JournalContext context)
        {
            _context = context;
        }

        public List<Subject> GetAllSubjects()
        {
            try
            {
                return _context.Subjects.ToList();
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }
    }
}
