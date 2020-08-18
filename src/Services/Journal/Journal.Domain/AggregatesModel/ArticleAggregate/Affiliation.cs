using Journal.Domain.SeedWork;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Affiliation
        : Entity
    {
        public string Name { get; private set; }

        public Affiliation(string name)
        {
            Name = name;
        }
    }
}
