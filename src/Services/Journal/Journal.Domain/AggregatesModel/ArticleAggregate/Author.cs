using Journal.Domain.SeedWork;
using System.Collections.Generic;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Author
        : Entity
    {
        public string Name { get; private set; }
        public string OrcidId { get; private set; }
        public string OrcidLink { get; private set; }   
        public List<AuthorAffiliation> AuthorAffiliations { get; private set; }

        public Author(string name)
        {
            Name = name;
            AuthorAffiliations = new List<AuthorAffiliation>();
        }

        public void AddAffiliation(string affliationName)
        {
            if (AuthorAffiliations.Exists(aa => aa.Author.Name == Name && aa.Affiliation.Name == affliationName) == false)
            {
                var newAffilation = new Affiliation(affliationName);
                AuthorAffiliations.Add(new AuthorAffiliation() { Author = this, Affiliation = newAffilation });
            }
        }

        /// <summary>
        /// Adds orcid_Id  and OrcidLink if and only if 
        /// orcid_id is not empty and has format 0000-0000-0000-0000 i.e. 16 numbers and 3 hyphens.        
        /// See https://support.orcid.org for more information.
        /// </summary>
        /// <param name="orcid_id"></param>
        public void AddOrcid(string orcid_id, string orcidLink)
        {
            if(!string.IsNullOrEmpty(orcid_id) && orcid_id.Trim().Length == 19)
            {
                OrcidId = orcid_id;
                OrcidLink = orcidLink;
            }            
        }

        /* ORCID, when accessed from its website/api gives the following on an Author
         * ScopusAuthorId => accessed from https://www.scopus.com, rich source of data
         * 'Also Known as' Names of Author
         * Country
         
         */
    }
}
