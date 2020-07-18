using Microsoft.Azure.Cosmos.Table;

namespace arx.Extract.Data.Entities
{
    public class SubjectEntity : TableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string Discipline { get; set; }
        public string Description { get; set; }
    }
}
