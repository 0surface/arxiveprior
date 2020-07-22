using arx.Extract.Data.Entities;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;
using Microsoft.OData.UriParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.BackgroundTasks.Core
{
    public interface ITransformService
    {
        List<PublicationItem> TransformArxivEntriesToPublications(List<Entry> allArxivEntries);
    }

    public class TransformService : ITransformService
    {
        private readonly IMapper _mapper;

        public TransformService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public  List<PublicationItem> TransformArxivEntriesToPublications(List<Entry> allArxivEntries)
        {
            List<PublicationItem> results = new List<PublicationItem>();

            foreach (var entry in allArxivEntries)
            {
                var publication = MapEntryToPublication(entry);
                if(publication != null)
                {
                    results.Add(publication);
                }
            }
          
            return results;
        }

        private PublicationItem MapEntryToPublication(Entry item)
        {
            PublicationItem pub = new PublicationItem();
            try
            {
                pub.ArxivId = item.Id.GetSubStringAfterCharValue('/');
                pub.VersionTag = pub.ArxivId.GetSubStringAfterCharValue('v', true);
                pub.CanonicalArxivId = pub.ArxivId.Replace(pub.VersionTag, "");
                pub.PublishedDate = item.PublishDate;
                pub.UpdatedDate = item.UpdatedDate;
                pub.Title = item.Title;
                pub.Abstract = item.Summary;
                pub.Comment = item.comment;
                pub.JournalReference = item.journal_ref;
                pub.Doi = item.doi;

                pub.PrimarySubjectCode = item.primary_category.Subject.Trim();

                item.Subjects?.Where(s => s.Subject != pub.PrimarySubjectCode)
                            ?.ToList()
                            ?.ForEach(s => pub.SubjectCodes.Add(s.Subject));

                pub.PdfLink = item?.Links
                                    ?.Where(l => l.Title == "pdf")
                                    ?.Select(l => l.Href)
                                    ?.SingleOrDefault() ?? string.Empty;
                pub.DoiLink = item?.Links
                                    ?.Where(l => l.Title == "doi")
                                    ?.Select(l => l.Href)
                                    ?.SingleOrDefault() ?? string.Empty;

                pub.Authors = _mapper.Map<List<AuthorItem>>(item.Authors);
            }
            catch (Exception)
            {
                return null;
            }
            return pub;
        }
    }
}
