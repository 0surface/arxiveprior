using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class PublicationItemEntity : TableEntity
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
        public List<string> SubjectCodes { get; set; }
        public string JournalReference { get; set; }
        public string Doi { get; set; }

        public string PdfLink { get; set; }
        public string DoiLink { get; set; }

        public string Authors { get; set; }
    }

}
