using Common.Types;

namespace arx.Extract.Types
{

    public class PublicationItem : ArxivPublication
    {
        public string FulfillmentId { get; set; }//PK        
        public string FulFillmentItemId { get; set; }
    }
}
