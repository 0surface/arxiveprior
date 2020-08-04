namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class CategoryArticle
    {
        public int CategoryId { get; set; }
        public int ArticleId { get; set; }
        public Category Category { get; set; }
        public Article Article { get; set; }
    }
}
