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
        Task<List<Subject>> GetAllSubjects();
        Task<List<Subject>> GetSubjectsByGroupCode(string groupCode);
        Task<List<Subject>> GetSubjectsByDiscipline(string discipline);
    }
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SubjectContext _context;

        public SubjectRepository(SubjectContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjects()
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
        public async Task<List<Subject>> GetSubjectsByGroupCode(string groupCode)
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
        public async Task<List<Subject>> GetSubjectsByDiscipline(string discipline)
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
