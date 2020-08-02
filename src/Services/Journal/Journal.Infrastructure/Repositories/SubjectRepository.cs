using Journal.Domain.AggregatesModel.SubjectAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Journal.Infrastructure.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllSubjects();
        Task<IEnumerable<Subject>> GetSubjectsByGroupCode(string groupCode);
        Task<IEnumerable<Subject>> GetSubjectsByDiscipline(string discipline);
    }
    public class SubjectRepository : ISubjectRepository
    {
        private readonly JournalContext _context;

        public SubjectRepository(JournalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjects()
        {
            try
            {
                return await _context.Subjects.ToListAsync();
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByGroupCode(string groupCode)
        {
            try
            {
                return await _context.Subjects
                    .Where(s => s.GroupCode == groupCode)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }
        public async Task<IEnumerable<Subject>> GetSubjectsByDiscipline(string discipline)
        {
            try
            {
                return await _context.Subjects
                  .Where(s => s.Discipline.ToLower() == discipline.ToLower())
                  .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Subject>();
            }
        }
    }
}
