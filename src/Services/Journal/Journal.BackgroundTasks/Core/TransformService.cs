using Common.Types;
using Journal.Domain.AggregatesModel.ArticleAggregate;
using Journal.Infrastructure.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journal.BackgroundTasks.Core
{
    public class TransformService : ITransformService
    {
        private readonly SubjectRepository _subjRepo;
        private static Dictionary<string, string> SubjectCodeGroupDictionary { get; set; }

        public TransformService(SubjectRepository subjRepo)
        {
            _subjRepo = subjRepo;
        }

        public (bool, List<Article>) MapToDomain(List<ArxivPublication> publications, int journalProcessId)
        {
            List<Article> articles = new List<Article>();
            try
            {
                HydrateDictinary();

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

        private void HydrateDictinary()
        {
            try
            {
                SubjectCodeGroupDictionary =
                    _subjRepo.GetAllSubjects().Result.ToDictionary(x => x.Code, x => x.GroupCode);

                if (SubjectCodeGroupDictionary == null || SubjectCodeGroupDictionary.Count < 155)
                {
                    throw new ApplicationException();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"HydrateDictinary() has thrown an exception attempting to fetch Subject Data from database");
            }
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
                    pub.MscCodes,
                    pub.AcmCodes,
                    pub.Doi,
                    pub.DoiLinks);

                article.ProcessDates();
                article.ProcessDerived(SubjectCodeGroupDictionary);
                article.AddVersion();
                article.AddSubjects(pub.SubjectCodes);
                article.AddAuthors(pub.Authors);

                return article;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"MapArxivPublicationToArticle has thrown an exception");
                return null;
            }
        }
    }
}
