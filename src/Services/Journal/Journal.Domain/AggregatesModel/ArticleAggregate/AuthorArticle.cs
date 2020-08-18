namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class AuthorArticle
    {
        public int AuthorId { get; set; }
        public int ArticleId { get; set; }
        public Author Author { get; set; }
        public Article Article { get; set; }
    }
}
