using Journal.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Article
        : Entity, IAggregateRoot
    {
        public string ArxivId { get; private set;  }
        public string CanonicalArxivId { get; private set;  }
        public DateTime PublishedDate { get; private set;  }
        public DateTime UpdatedDate { get; private set;  }
        public string Title { get; private set;  }
        public string Abstract { get; private set;  }
        public string Comment { get; private set;  }
        public string JournalReference { get; private set;  }
        /// <summary>
        /// Digital Object Identifier (for a scientific paper)
        /// </summary>
        public string Doi { get; private set;  }
        public string DoiLink { get; private set;  }
        public string PdfLink { get; private set;  }
        public string PrimarySubjectCode { get; private set; }

        /// <summary>
        /// MSC = Mathematics Subject Classification - is an alphanumerical classification scheme.
        /// </summary>
        public string MscSchemes { get; private set; }
        /// <summary>
        /// ACM = Association of Computing Machinery Classification
        /// </summary>
        public string AcmSchemes { get; private set; }
        public List<AuthorArticle> AuthorArticles { get; private set; }
        public List<CategoryArticle> CategoryArticles { get; private set; }

        ///DDD Patterns comment
        /// Using a private collection field, better for DDD Aggregate's encapsulation
        /// so Versions cannot be added from "outside the AggregateRoot" directly to the collection,
        /// but only through the method ArticleAggrergateRoot.AddVersion() which includes behaviour.
        private readonly List<Version> _versions;
        public IReadOnlyCollection<Version> Versions => _versions;

        #region Derived Properties
        
        public string PrimarySubjectGroupCode { get; private set; }        
        public int UpdatedDay { get; private set; }
        public int UpdatedMonth { get; private set; }
        public int UpdatedYear { get; private set; }
        public int VersionNumber { get; private set; }
        public bool IsLatestVersion { get; private set; }

        #endregion Derived

        #region Meta Data

        public int JournalProcessedId { get; set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime LastModifiedDate { get; private set; }

        #endregion Meta Data
    }
}
