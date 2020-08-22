using Journal.Domain.SeedWork;
using System.Collections.Generic;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Affiliation
        : Entity
    {
        public string Name { get; private set; }

        public List<AuthorAffiliation> AuthorAffiliations { get; private set; }

        protected Affiliation()
        {
            AuthorAffiliations = new List<AuthorAffiliation>();
        }

        public Affiliation(string affiliateName) 
            : base()
        {
            Name = affiliateName;
        }
    }
}
