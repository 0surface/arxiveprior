namespace Journal.API.Application.Models.Subject
{
    public class SubjectItem
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string GroupCode { get; private set; }
        public string GroupName { get; private set; }
        public string Discipline { get; private set; }
        public string Description { get; private set; }
    }

    public class SubjectSummaryItem
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
    }

}
