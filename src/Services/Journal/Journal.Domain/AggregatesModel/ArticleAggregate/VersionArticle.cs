namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class VersionArticle
    {
        public int VersionId { get; set; }
        public int ArticleId { get; set; }
        public Version Version { get; set; }
        public Article Article { get; set; }
    }
}
