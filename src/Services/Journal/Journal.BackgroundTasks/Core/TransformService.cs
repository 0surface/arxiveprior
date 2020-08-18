using Common.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using Serilog;
using System;
using System.Collections.Generic;

namespace Journal.BackgroundTasks.Core
{
    public class TransformService : ITransformService
    {
        public TransformService()
        {

        }

        public (bool, List<Article>) MapToDomain(List<ArxivPublication> publications, int journalProcessId)
        {
            List<Article> articles = new List<Article>();
            try
            {
                publications?.ForEach(pub =>
                {
                    articles.AddIfNotNull(
                        MapArxivPublicationToArticle(pub, journalProcessId));
                });

                return (articles.Count == publications.Count, articles);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"MapToDomain has thrown an exception");
            }

            return (articles.Count == publications.Count, articles);
        }


        private Article MapArxivPublicationToArticle(ArxivPublication pub, int jpId)
        {
            try
            {
                Article article = new Article(jpId,
                    pub.ArxivId,
                    pub.PublishedDate,
                    pub.UpdatedDate,
                    pub.Title,
                    pub.Abstract,
                    pub.Comment,
                    pub.PrimarySubjectCode,
                    pub.JournalReference,
                    pub.MscCodes,
                    pub.AcmCodes,
                    pub.Doi,
                    pub.DoiLinks,
                    pub.SubjectCodes,
                    pub.Authors);

                return article;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Pub Arxiv Id [{pub.ArxivId}] - MapArxivPublicationToArticle has thrown an exception");
                return null;
            }
        }
    }
}
