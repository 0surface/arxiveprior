using arx.Extract.Data.Common;
using arx.Extract.Types;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class PublicationItemEntity : TableEntity, IPublicationItem
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
        
        
        public string JournalReference { get; set; }
        public string Doi { get; set; }

        public string PdfLink { get; set; }
        public string DoiLink { get; set; }

        [EntityJsonPropertyConverter]
        public List<string> SubjectCodes { get; set; }

        [EntityJsonPropertyConverter]
        public List<AuthorItem> Authors { get; set; }


        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityJsonPropertyConverter.Serialize(this, results);
            this.PartitionKey = this.PrimarySubjectCode;
            this.RowKey = this.ArxivId;
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityJsonPropertyConverter.Deserialize(this, properties);
        }

    }

}
