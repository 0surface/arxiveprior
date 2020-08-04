using Journal.Domain.SeedWork;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Category
        : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        public string GroupName { get; private set; }

        protected Category() { }
        public Category(string code)
        {
            Code = code;
        }
    }
}
