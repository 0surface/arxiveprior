using Journal.Domain.SeedWork;
using System;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Version
        : Entity
    {
        /* On arxiv.org, an Article's version list is titled 'Submission history' 
          SubmissionDate = Article's UpdatedDate */
        public DateTime SubmissionDate { get; private set; }
        public int Number { get; private set; }
        public DateTime CreatedDate { get; private set; }

        protected Version()
        {
            SubmissionDate = DateTime.MinValue;
            CreatedDate = DateTime.MinValue;
        }
        public Version(DateTime updatedDate, int versionNumber)
        {
            SubmissionDate = updatedDate;
            Number = versionNumber;
            CreatedDate = DateTime.UtcNow;
        }
    }
}
