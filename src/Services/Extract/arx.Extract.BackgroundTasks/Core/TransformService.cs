using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static arx.Extract.BackgroundTasks.Core.SubjectEntryExtensions;

namespace arx.Extract.BackgroundTasks.Core
{
    public class TransformService : ITransformService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TransformService> _logger;
        private readonly ISubjectRepository _subjectRepo;
        private Dictionary<string, string> _arxivSubjectCodes { get; set; }

        public TransformService(IMapper mapper,
            ILogger<TransformService> logger,
            ISubjectRepository subjectRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _subjectRepo = subjectRepo;
        }

        public (bool, int, List<PublicationItem>) TransformArxivEntriesToPublications(List<Entry> allArxivEntries)
        {
            List<PublicationItem> results = new List<PublicationItem>();

            if (allArxivEntries == null || allArxivEntries.Count == 0)
                return (true, 0, results);

            //Fetch and set Arxiv subject Codes
            _arxivSubjectCodes = _subjectRepo.GetAll().Result.Select(s => s.Code).ToDictionary(x => x);

            foreach (var entry in allArxivEntries)
            {
                results.AddIfNotNull(EntryToPublication(entry));
            }

            return (results.Count == allArxivEntries.Count, results?.Count ?? 0, results);
        }

        public (bool, List<PublicationItemEntity>) TransformPublicationItemsToEntity(string fulfillmentId, string fulfillmentItemId, List<PublicationItem> publications)
        {
            try
            {
                //Map
                List<PublicationItemEntity> entityList
                    = _mapper.Map<List<PublicationItemEntity>>(publications);

                //Set fulfillment Ids
                entityList?.ForEach(e =>
                {
                    e.PartitionKey = fulfillmentId;
                    e.FulfillmentId = fulfillmentId;
                    e.FulFillmentItemId = fulfillmentItemId;
                });

                return (true, entityList);
            }
            catch (Exception)
            {
                return (false, new List<PublicationItemEntity>());
            }
        }

        private PublicationItem EntryToPublication(Entry item)
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

                List<Categories> categoryCodes = item?.CategoryCodes
                                                    ?.Where(s => s.CategoryCode.Trim() != pub.PrimarySubjectCode)
                                                    ?.ToList();

                if (categoryCodes != null && categoryCodes.Count > 0)
                {
                    SubjectEntryResultList subjectEntryResultList = new SubjectEntryResultList(categoryCodes, _arxivSubjectCodes);

                    pub.SubjectCodes = subjectEntryResultList.EntryResultList
                                        ?.Where(x => !string.IsNullOrEmpty(x.ArxivCode))
                                        ?.Select(x => x.ArxivCode)
                                        ?.ToList();

                    pub.AcmCodes = subjectEntryResultList.EntryResultList
                                        ?.Where(x => !string.IsNullOrEmpty(x.AcmCode))
                                        ?.Select(x => x.ArxivCode)
                                        .SingleOrDefault();

                    pub.MscCodes = subjectEntryResultList.EntryResultList
                                    ?.Where(x => !string.IsNullOrEmpty(x.MscCode))
                                    ?.Select(x => x.ArxivCode)
                                    .SingleOrDefault();
                }

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

    public static class SubjectEntryExtensions
    {
        public class SubjectEntryResultList
        {
            public List<SubjectEntryResult> EntryResultList { get; set; } = new List<SubjectEntryResult>();
            internal SubjectEntryResultList(List<Categories> entrySubjects, Dictionary<string, string> arxivCodes)
            {
                entrySubjects?.ForEach(s => EntryResultList.Add(new SubjectEntryResult(s.CategoryCode)));
                EntryResultList?.ForEach(e => e.ProcessCodes(arxivCodes));
            }
        }

        public class SubjectEntryResult
        {
            internal SubjectEntryResult(string entry)
            {
                EntrySubject = entry;
            }
            public string EntrySubject { get; set; }
            public string ArxivCode { get; set; }
            public string AcmCode { get; set; }
            public string MscCode { get; set; }
        }

        public static SubjectEntryResult ProcessCodes(this SubjectEntryResult entry, Dictionary<string, string> arxivCodes)
        {
            string value = entry.EntrySubject.Trim();

            if (arxivCodes.ContainsKey(value))
            {
                entry.ArxivCode = value;
                return entry;
            }

            if (IsAcmCode(value))
            {
                entry.AcmCode = value;
                return entry;
            }

            if (IsMcsCode(value))
            {
                entry.MscCode = value;
                return entry;
            }

            return entry;
        }

        public static bool IsAcmCode(string value)
        {
            return (value.Contains(',')) // "A.O.O; B.0.0"
                || (value.Length == 3 && value[1] == '.') //A.0
                || (value.Length == 5 && value[1] == '.' && value[3] == '.'); // A.0.0
        }

        public static bool IsMcsCode(string value)
        {
            return value.Contains(',') // "00A00, 00B00"
                || value.ToLower().Contains("primary")
                || value.ToLower().Contains("secondary")//["90C17 (Primary), 90C25 (Secondary), 90C34 (Tertiary)"]                
                || value.ToLower().Contains("tertiary")
                || (value.Length == 5 && char.IsNumber(value[0]) && char.IsNumber(value[1])); //42Bxx
        }
    }
}

