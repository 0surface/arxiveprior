namespace arx.Extract.API
{
    public class StorageConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string SubjectTableName { get; set; }
        public string PublicationTableName { get; set; }
        public string JobTableName { get; set; }
        public string JobItemTableName { get; set; }
        public string FulfillmentTableName { get; set; }
        public string FulfillmentItemTableName { get; set; }
    }
}
