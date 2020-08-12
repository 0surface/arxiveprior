namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class AuthorAffiliation
    {
        public int AuthorId { get; set; }
        public int AffiliationId { get; set; }
        public Author Author { get; set; }
        public Affiliation Affiliation { get; set; }
    }
}
