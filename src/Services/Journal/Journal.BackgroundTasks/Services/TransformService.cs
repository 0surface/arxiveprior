using Journal.BackgroundTasks.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using Serilog;
using System;
using System.Collections.Generic;

namespace Journal.BackgroundTasks.Services
{
    public class TransformService : ITransformService
    {
        public TransformService()
        {

        }

        public (bool, List<Article>) MapToDomain(List<PublicationItemDto> publications, int journalProcessId)
        {
            List<Article> articles = new List<Article>();
            try
            {
                publications?.ForEach(dto =>
                {
                    articles.AddIfNotNull(MapArxivPublicationToArticle(dto, journalProcessId));
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"TransformService.MapToDomain has thrown an exception");
            }

            return (articles.Count == publications.Count, articles);
        }

        private Article MapArxivPublicationToArticle(PublicationItemDto dto, int jpId)
        {
            try
            {
                Article article = new Article(jpId,
                    dto.ArxivId,
                    dto.PublishedDate,
                    dto.UpdatedDate,
                    dto.Title,
                    dto.Abstract,
                    dto.Comment,
                    dto.PrimarySubjectCode,
                    dto.JournalReference,
                    dto.MscCodes,
                    dto.AcmCodes,
                    dto.Doi,
                    dto.DoiLinks,
                    dto.SubjectCodes,
                    dto.Authors);

                return article;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Arxiv Id [{dto.ArxivId}] - MapArxivPublicationToArticle has thrown an exception");
                return null;
            }
        }
    }
}
