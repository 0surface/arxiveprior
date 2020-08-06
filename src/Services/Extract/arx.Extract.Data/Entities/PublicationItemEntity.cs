using arx.Extract.Data.Common;
using Common.Types;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Entities
{
    public class PublicationItemEntity : TableEntity, IArxivPublication
    {
        public string FulfillmentId { get; set; }//PK
        public string ArxivId { get; set; } //T , RK

        public string FulFillmentItemId { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Comment { get; set; }

        public string PrimarySubjectCode { get; set; }


        public string JournalReference { get; set; }
        public string Doi { get; set; }
        public string DoiLinks { get; set; }

        [EntityJsonPropertyConverter]
        public List<string> SubjectCodes { get; set; }
        public string MscCodes { get; set; }
        public string AcmCodes { get; set; }

        [EntityJsonPropertyConverter]
        public List<string> Authors { get; set; }

        [EntityJsonPropertyConverter]
        public List<string> AuthorSpillOverListOne { get; set; }
        [EntityJsonPropertyConverter]
        public List<string> AuthorSpillOverListTwo { get; set; }

        [EntityJsonPropertyConverter]
        public List<string> AuthorSpillOverListThree { get; set; }

        public bool AuthorListTruncated { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityJsonPropertyConverter.Serialize(this, results);
            this.PartitionKey = this.FulfillmentId;
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
