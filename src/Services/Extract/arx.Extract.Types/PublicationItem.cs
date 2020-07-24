using System;
using System.Collections.Generic;

namespace arx.Extract.Types
{
    public interface IPublicationItem
    {
        string ArxivId { get; set; } //T
        string CanonicalArxivId { get; set; }
        string VersionTag { get; set; } //T
        DateTime PublishedDate { get; set; }
        DateTime UpdatedDate { get; set; }

        string Title { get; set; }
        string Abstract { get; set; }
        string Comment { get; set; }

        string PrimarySubjectCode { get; set; }
        List<string> SubjectCodes { get; set; }
        string JournalReference { get; set; }
        string Doi { get; set; }

        string PdfLink { get; set; }
        string DoiLink { get; set; }

        List<AuthorItem> Authors { get; set; }
    }
    public class PublicationItem : IPublicationItem
    {
        public string ArxivId { get; set; } //T
        public string CanonicalArxivId { get; set; }
        public string VersionTag { get; set; } //T
        public DateTime PublishedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Comment { get; set; }

        public string PrimarySubjectCode { get; set; }
        public List<string> SubjectCodes { get; set; } = new List<string>();
        public string JournalReference { get; set; }
        public string Doi { get; set; }

        public string PdfLink { get; set; }
        public string DoiLink { get; set; }

        public List<AuthorItem> Authors { get; set; } = new List<AuthorItem>();
    }
}
