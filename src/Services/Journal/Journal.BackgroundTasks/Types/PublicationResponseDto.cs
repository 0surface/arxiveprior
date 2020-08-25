using Common.Types;
using System;
using System.Collections.Generic;

namespace Journal.BackgroundTasks.Types
{
    public class PublicationResponseDto
    {
        public List<PublicationItemDto> PublicationItems { get; set; } = new List<PublicationItemDto>();
        public DateTime QueryFromDate { get; set; }
        public DateTime QueryToDate { get; set; }
        public string ExtractFulfillmentId { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PublicationItemDto : IArxivPublication
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

