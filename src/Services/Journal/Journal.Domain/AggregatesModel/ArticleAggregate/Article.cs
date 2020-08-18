using Common.Util.String;
using Journal.Domain.AggregatesModel.SubjectAggregate;
using Journal.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace Journal.Domain.AggregatesModel.ArticleAggregate
{
    public class Article
        : Entity, IAggregateRoot
    {
        public string VersionedArxivId { get; private set; }
        public DateTime PublishedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public string Title { get; private set; }
        public string Summary { get; private set; }
        public string Comment { get; private set; }
        public string JournalReference { get; private set; }

        /// <summary>
        /// Digital Object Identifier (for a scientific paper)
        /// </summary>
        public string Doi { get; private set; }
        public string DoiLinks { get; private set; }        
        public SubjectCode PrimarySubjectCode { get; private set; }

        /// <summary>
        /// MSC = Mathematics Subject Classification - is an alphanumerical classification scheme.
        /// </summary>
        public string MscCategory { get; private set; }
        /// <summary>
        /// ACM = Association of Computing Machinery Classification
        /// </summary>
        public string AcmCategory { get; private set; }
        public List<AuthorArticle> AuthorArticles { get; private set; }
        public List<CategoryArticle> CategoryArticles { get; private set; }

        ///DDD Patterns comment
        /// Using a private collection field, better for DDD Aggregate's encapsulation
        /// so Versions cannot be added from "outside the AggregateRoot" directly to the collection,
        /// but only through the method ArticleAggrergateRoot.AddVersion() which includes behaviour.
        private readonly List<PaperVersion> _paperVersions; // Backing field
        public IReadOnlyList<PaperVersion> PaperVersions => _paperVersions;

        #region Derived Properties

        public string PdfLink { get; private set; }
        public string ArxivId { get; private set; }
        public SubjectGroup PrimarySubjectGroupCode { get; private set; }
        public Discipline PrmiaryDiscipline { get; private set; }
        public int UpdatedDay { get; private set; }
        public int UpdatedMonth { get; private set; }
        public int UpdatedYear { get; private set; }
        public int VersionNumber { get; private set; }
        public bool IsLatestVersion { get; private set; }
        public bool IsWithdrawn { get; private set; }

        #endregion Derived

        #region Meta Data

        public int JournalProcessedId { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastModified { get; private set; }
        public bool HasProcessingError { get; private set; }

        #endregion Meta Data

        protected Article()
        {
            AuthorArticles = new List<AuthorArticle>();
            CategoryArticles = new List<CategoryArticle>();
            _paperVersions = new List<PaperVersion>();
            Created = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }

        public Article(int journalProcessedId, string arxivId,
            DateTime publishedDate, DateTime updatedDate,
            string title, string summary, string comment,
            string primarySubjectCode, string journalReference,
            string mscCategory, string acmCategory,
            string doi, string doiLinks,
            List<string> subjects, List<String> authors) 
            : this()
        {
            JournalProcessedId = journalProcessedId;
            VersionedArxivId = arxivId;
            PublishedDate = publishedDate;
            UpdatedDate = updatedDate;
            Title = title;
            Summary = summary;
            Comment = comment;
            PrimarySubjectCode = SubjectCode.FindByCode(primarySubjectCode);
            PrimarySubjectGroupCode = PrimarySubjectCode.SubjectGroup;
            PrmiaryDiscipline = PrimarySubjectGroupCode.Discipline;
            JournalReference = journalReference;
            MscCategory = mscCategory;
            AcmCategory = acmCategory;
            Doi = doi;
            DoiLinks = doiLinks;

            ProcessDates();
            ProcessDerived();
            AddSubjects(subjects);
            AddAuthors(authors);
            AddVersion();

            Created = DateTime.UtcNow;       
            LastModified = DateTime.UtcNow;
        }

        private void ProcessDates()
        {
            if (PublishedDate == null || PublishedDate == DateTime.MinValue)
            {
                PublishedDate = DateTime.MinValue;
                HasProcessingError = true;
            }
            if (UpdatedDate == null || UpdatedDate == DateTime.MinValue)
            {
                UpdatedDate = DateTime.MinValue;
                HasProcessingError = true;
            }
        }
        private void ProcessDerived()
        {
            UpdatedDay = UpdatedDate.Day;
            UpdatedMonth = UpdatedDate.Month;
            UpdatedYear = UpdatedDate.Year;

           

            string tag = VersionedArxivId.GetSubStringAfterCharValue('v', true);
            ArxivId = ArxivId.Replace(tag, string.Empty);

            int.TryParse(tag.Replace("v", string.Empty), out int vnum);
            VersionNumber = vnum;

            bool withdrawn = Comment.Contains("has been withdrawn");
            
            IsWithdrawn = withdrawn;
            
            PdfLink = withdrawn ? string.Empty : $"https://arxiv.org/pdf/{ArxivId}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjects"></param>
        private void AddSubjects(List<string> subjects)
        {
            try
            {
                if (subjects == null && subjects.Count == 0) return;

                subjects?.ForEach(s => 
                {
                    CategoryArticles.Add(new CategoryArticle()
                    {
                        Article = this,
                        Category = new Category(s)
                    });
                });
            }
            catch (Exception)
            {
                HasProcessingError = true;
            }
        }

        private void AddAuthors(List<string> authors)
        {
            try
            {
                authors?.ForEach(desc =>
                {
                    if (desc.Contains('|'))
                    {
                        string[] arr = desc.Split('|');
                        var newAuthor = new Author(arr[0]);
                        newAuthor.AddAffiliation(arr[1]);
                        AuthorArticles.Add(new AuthorArticle() { Article = this, Author = newAuthor });
                    }
                    else
                    {
                        var newAuthor = new Author(desc);
                        AuthorArticles.Add(new AuthorArticle() { Article = this, Author = newAuthor });
                    }
                });
            }
            catch (Exception)
            {
                HasProcessingError = true;
            }
        }

        private void AddVersion()
        {
            if (!_paperVersions.Any(x => x.Number == VersionNumber))
            {
                _paperVersions.Add(new PaperVersion(ArxivId, UpdatedDate, VersionNumber));
            }
        }

        public void Update(Article updated)
        {
            if(updated == null) return;

            if (updated.VersionNumber == VersionNumber)
            {
                return;
            }
            else if (updated.VersionNumber < VersionNumber)
            {
                ///'updated' is a Retired vesion, add to vesion collection.
                if (updated.PaperVersions != null && updated.PaperVersions.Count != 0)
                {
                    _paperVersions.Add(updated.PaperVersions.First());
                }
                return;
            }
            else
            {
                //Update to New Version
                JournalProcessedId = updated.JournalProcessedId;
                VersionedArxivId =  updated.ArxivId;
                PublishedDate =  updated.PublishedDate;
                UpdatedDate =  updated.UpdatedDate;
                Title =  updated.Title;
                Summary =  updated.Summary;
                Comment =  updated.Comment;
                PrimarySubjectCode = updated.PrimarySubjectCode;
                PrimarySubjectGroupCode = updated.PrimarySubjectGroupCode;
                PrmiaryDiscipline = updated.PrmiaryDiscipline;
                JournalReference =  updated.JournalReference;
                MscCategory =  updated.MscCategory;
                AcmCategory =  updated.AcmCategory;
                Doi =  updated.Doi;
                DoiLinks =  updated.DoiLinks;

                UpdateSubjects(updated.CategoryArticles);
                UpdateAuthors(updated.AuthorArticles);
                UpdateVersion(updated.PaperVersions.FirstOrDefault());

                LastModified = DateTime.UtcNow;
            }
        }

        public void UpdateVersion(PaperVersion newVersion)
        {           
            if (!_paperVersions.Any(x => x.Number == newVersion?.Number))
            {
                _paperVersions.Add(newVersion);
            }
        }

        private void UpdateSubjects(List<CategoryArticle> categoryArticleList)
        {
            try
            {
                if (categoryArticleList == null && categoryArticleList.Count == 0) 
                    return;

                if (CategoryArticles.Count == 0)
                {
                    CategoryArticles.AddRange(categoryArticleList);
                }
                else
                {
                    var existingCodes = CategoryArticles.Select(e => e.Category.SubjectCode).ToList();
                    var newCodes = categoryArticleList.Select(e => e.Category.SubjectCode).ToList();

                    if (existingCodes.Count == newCodes.Count &&
                        existingCodes.All(newCodes.Contains))
                    {
                        //All elements match, do nothing
                        return;
                    }
                    else
                    {
                        //Add new, if not in list
                        categoryArticleList.ForEach(ca =>
                        {
                            if (!existingCodes.Contains(ca.Category.SubjectCode))
                            {
                                CategoryArticles.Add(ca);
                            }
                        });

                        //Remove outdated
                        CategoryArticles.RemoveAll(ca => newCodes.Contains(ca.Category.SubjectCode) == false);
                    }
                }
            }
            catch (Exception)
            {
                HasProcessingError = true;
            }
        }

        private void UpdateAuthors(List<AuthorArticle> authorArticleList)
        {
            try
            {
                if (authorArticleList == null || authorArticleList.Count == 0)
                    return;
                
                if(AuthorArticles.Count == 0)
                {
                    ///This scenario should never happen. Inserted here for defensive coding.
                    AuthorArticles.AddRange(authorArticleList);
                    return;
                }
                else
                {
                    var existingAuthorNames = AuthorArticles.Select(x => x.Author.Name).ToList();
                    var newAuthorNames = authorArticleList.Select(x => x.Author.Name).ToList();

                    if (existingAuthorNames.Count == newAuthorNames.Count &&
                        existingAuthorNames.All(newAuthorNames.Contains))
                    {
                        //All elements match, do nothing.
                        return;
                    }
                    else
                    {
                        //Add new
                        authorArticleList.ForEach(aa =>
                        {
                            if (existingAuthorNames.Contains(aa.Author.Name) == false)
                            {
                                AuthorArticles.Add(aa);
                            }
                        });

                        //Remove outdated
                        AuthorArticles.RemoveAll(aa => newAuthorNames.Contains(aa.Author.Name) == false);
                    }
                }
            }
            catch (Exception)
            {
                HasProcessingError = true;
            }
        }
    }
}
