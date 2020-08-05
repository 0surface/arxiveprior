using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using arx.Extract.Lib;
using arx.Extract.Types;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    int codesFound = 0;
                    for (int i = 0; i < categoryCodes.Count; i++)
                    {
                        string value = categoryCodes[i].CategoryCode.Trim();

                        if (_arxivSubjectCodes.ContainsKey(value))
                        {
                            pub.SubjectCodes.Add(value);
                            codesFound++;
                            continue;
                        }

                        if (IsAcmCode(value))
                        {
                            pub.AcmCodes = value;
                            codesFound++;
                            continue;
                        }

                        if (IsMcsCode(value))
                        {
                            pub.MscCodes = value;
                            codesFound++;
                        }                        
                    }

                    if (codesFound != categoryCodes.Count)
                    {
                        _logger.LogError($"Error Category Code Processing - [{pub.ArxivId}] - Found/Expected = {codesFound} / {categoryCodes.Count}");
                        string[] c = new string[categoryCodes.Count];
                        categoryCodes.ForEach(x => c.Append(x.CategoryCode));                        
                        _logger.LogError($"Error Category Code Processing {pub.ArxivId} | Pre-processed Codes =[{string.Join('|', c)}]");
                    }
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

        public static bool IsAcmCode(string value)
        {
            return (value.Contains(';')) // "A.O.O; B.0.0"
                || (value.Length == 3 && value[1] == '.') //A.0
                || (value.Length >= 5 && value[1] == '.' && value[3] == '.'); // A.0.0
        }

        public static bool IsMcsCode(string value)
        {
            return value.Contains(',') // "00A00, 00B00"                
                || value.ToLower().Contains("primary")
                || value.ToLower().Contains("secondary")//["90C17 (Primary), 90C25 (Secondary), 90C34 (Tertiary)"]                
                || value.ToLower().Contains("tertiary")
                || value.Contains(':') //2010 MSC: 60H15
                || (value.Length >= 3 && char.IsNumber(value[0]) && char.IsNumber(value[1])); //03F, 42Bxx
        }
    }
}

