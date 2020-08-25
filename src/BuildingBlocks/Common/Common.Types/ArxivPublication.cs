using System;
using System.Collections.Generic;

namespace Common.Types
{
    public interface IArxivPublication
    {
         string ArxivId { get; set; } 
         DateTime PublishedDate { get; set; }
         DateTime UpdatedDate { get; set; }

         string Title { get; set; }
         string Abstract { get; set; }
         string Comment { get; set; }

         string PrimarySubjectCode { get; set; }
         List<string> SubjectCodes { get; set; }
         string MscCodes { get; set; }
         string AcmCodes { get; set; }
         string JournalReference { get; set; }
         string Doi { get; set; }
         string DoiLinks { get; set; }

         bool AuthorListTruncated { get; set; }
         List<string> Authors { get; set; }
    }

    public class ArxivPublication : IArxivPublication
    {
        public string ArxivId { get; set; } 
        public DateTime PublishedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Comment { get; set; }

        public string PrimarySubjectCode { get; set; }
        public List<string> SubjectCodes { get; set; } = new List<string>();
        public string MscCodes { get; set; }
        public string AcmCodes { get; set; }
        public string JournalReference { get; set; }
        public string Doi { get; set; }
        public string DoiLinks { get; set; }

        public bool AuthorListTruncated { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
    }
}
