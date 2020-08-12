using Journal.Domain.SeedWork;
using System;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class PaperVersion
        : Entity
    {
        /// <summary>
        /// The 'canonical' arxiv.oeg id withour thee version appended at the end as v1,2,3...
        /// </summary>
        public string ArxivId { get; private set; }
                
        /// <summary>
        ///  On arxiv.org, an Article's version list is titled 'Submission history' 
        ///  SubmissionDate = Article's UpdatedDate
        /// </summary>
        public DateTime SubmissionDate { get; private set; }
        public int Number { get; private set; }

        public Article Article { get; set; }


        protected PaperVersion()
        {
            Number = 1;
            SubmissionDate = DateTime.MinValue;            
        }
        public PaperVersion(string arxivId, DateTime updatedDate, int versionNumber) 
            : this()
        {
            ArxivId = arxivId;
            SubmissionDate = updatedDate;
            Number = versionNumber;
        }
    }
}
