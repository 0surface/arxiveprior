using Journal.Domain.SeedWork;
using System;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Category
        : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string GroupCode { get; private set; }
        public string GroupName { get; private set; }
        public SubjectCode SubjectCode { get; private set; }
        public SubjectGroup SubjectGroup { get; private set; }
        public Discipline Discipline { get; private set; }

        protected Category() { }
        public Category(string code)
        {
            Code = code;
        }

        public Category(SubjectCode code, SubjectGroup group, Discipline discipline)
        {
            SubjectCode = code;
            SubjectGroup = group;
            Discipline = discipline;
        }
        public Category FindBySubjectCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
