using Journal.Domain.SeedWork;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Category
        : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public SubjectCode SubjectCode { get; private set; }
        public SubjectGroup SubjectGroup { get; private set; }
        public Discipline Discipline { get; private set; }

        protected Category() { }
        public Category(string code) : base()
        {
            SubjectCode = SubjectCode.FindByCode(code);
            Name = SubjectCode.Description;
            SubjectGroup = SubjectCode.SubjectGroup;
            Discipline = SubjectGroup.Discipline;
        }
    }
}
