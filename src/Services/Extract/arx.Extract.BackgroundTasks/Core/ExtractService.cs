﻿using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using arx.Extract.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Core
{
    public class ExtractService : IExtractService
    {
        private readonly BackgroundTasksConfiguration _config;
        private readonly ILogger<ExtractService> _logger;
        private readonly IJobRepository _jobRepository;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly IFulfillmentItemRepository _fulfillmentItemRepository;
        private readonly IJobItemRepository _jobItemRepository;

        public ExtractService(IOptions<BackgroundTasksConfiguration> configSettings,
            ILogger<ExtractService> logger,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IFulfillmentRepository fulfillmentRepository,
            IFulfillmentItemRepository fulfillmentItemRepository)
        {
            _config = configSettings?.Value ?? throw new ArgumentException("IOptions<BackgroundTasksConfiguration> configuration is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _jobRepository = jobRepository;
            _fulfillmentRepository = fulfillmentRepository;
            _fulfillmentItemRepository = fulfillmentItemRepository;
            _jobItemRepository = jobItemRepository;
        }

        public (bool, JobEntity, List<JobItemEntity>) GetArchiveJob()
        {
            try
            {
                JobEntity job = _jobRepository.GetJob(ExtractTypeEnum.Archive, _config.ArchiveJobName);

                List<JobItemEntity> jobItems = _jobItemRepository.GetJobItems(job?.UniqueName);

                bool isSuccess = (job != null && jobItems != null && jobItems.Count > 0);

                return (isSuccess, job, jobItems);
            }
            catch (Exception)
            {
                return (false, null, null);
            }
        }

        public (bool, bool, FulfillmentEntity) GetLastSuccessfulArchiveFulfillment(string jobUniqueName)
        {
            FulfillmentEntity lastFulfillment = _fulfillmentRepository.GetLastSuccessfulFulfillment(jobUniqueName).Result;

            bool isInitialFulfillment = false;
            bool isFound = lastFulfillment != null;

            if(!isFound)
            {
                isInitialFulfillment = !_fulfillmentRepository.FulfillmentsExist(jobUniqueName);
            }

            return (isFound, isInitialFulfillment, lastFulfillment);
        }

        public (bool, FulfillmentEntity, List<FulfillmentItemEntity>) CreateArchiveFulfillmentSaga(JobEntity job, List<JobItemEntity> jobItems, FulfillmentEntity lastFulfillment, bool isFirstFulfillment)
        {
            try
            {
                int minQueryDateInterval = (int)Math.Floor(jobItems.Average(x => x.QueryDateInterval));

                DateTime previousQueryFromDate = DateTime.MinValue;
                DateTime previousQueryToDate = DateTime.MinValue;

                if (!isFirstFulfillment
                    && (lastFulfillment == null || lastFulfillment?.QueryFromDate == null || lastFulfillment?.QueryToDate == null))
                {
                    _logger.LogCritical("Error - Unable to find previous lastFulfillment Entit. Returned value is null");
                    return (false, null, null);
                }
                else if (!isFirstFulfillment)
                {
                    previousQueryFromDate = lastFulfillment.QueryFromDate;
                    previousQueryToDate = lastFulfillment.QueryToDate;
                }

                FulfillmentEntity newFulfillment 
                    = ExtractUtil.MakeNewFulfillment(job,
                                                    previousQueryFromDate, 
                                                    previousQueryToDate, 
                                                    minQueryDateInterval);

                if (ExtractUtil.HasPassedTerminationDate(_config.ArchiveTerminateDate, newFulfillment.QueryToDate))
                {
                    _logger.LogInformation("Stopping Service. Query Date window From [{0}] To [{1}] has passed Archive Terminate Date [{2}]",
                        _config.ArchiveTerminateDate, newFulfillment.QueryFromDate, newFulfillment.QueryToDate);
                    return (false, null, null);
                }

                newFulfillment = _fulfillmentRepository.SaveFulfillment(newFulfillment).Result;

                if (newFulfillment == null)
                {
                    _logger.LogCritical("Error persisting New Extract Fulfillment to Storage - [{0}]-[{1}] - Query From [{2}] To [{3}]",
                            newFulfillment.JobName, newFulfillment.FulfillmentId, newFulfillment.QueryFromDate.ToString("dd MMMM yyyy")
                            , newFulfillment.QueryToDate.ToString("dd MMMM yyyy"));
                    return (false, null, null);
                }
                else
                {
                    _logger.LogInformation("New Fulfillment [{0}]-[{1}] - Query From [{2}] To [{3}] - Started @ {4}",
                                        newFulfillment.JobName, newFulfillment.FulfillmentId, newFulfillment.QueryFromDate.ToString("dd MMMM yyyy"),
                                        newFulfillment.QueryToDate.ToString("dd MMMM yyyy"), newFulfillment.JobStartedDate);

                    List<FulfillmentItemEntity> newFulfillmentItems = new List<FulfillmentItemEntity>();

                    foreach (var jobItem in jobItems)
                    {
                        if (isFirstFulfillment)
                        {
                            var initialInterval = ExtractUtil.GetRequestChunkedArchiveDates
                                                    (DateTime.MinValue, DateTime.MinValue, jobItem.QueryDateInterval).FirstOrDefault();

                            if (ExtractUtil.HasPassedTerminationDate(_config.ArchiveTerminateDate, initialInterval.QueryToDate) == false)
                            {
                                newFulfillmentItems.Add(ExtractUtil.MakeNewFulfillmentItem(jobItem, initialInterval, job.QueryBaseUrl, newFulfillment.FulfillmentId));
                            }
                        }
                        else
                        {
                            //For an optimal choice of query interval date values per query, the loop below will only be executed once.
                            foreach (var interval in ExtractUtil.GetRequestChunkedArchiveDates
                                                    (lastFulfillment.QueryFromDate,lastFulfillment.QueryToDate, jobItem.QueryDateInterval))
                            {
                                if (ExtractUtil.HasPassedTerminationDate(_config.ArchiveTerminateDate, interval.QueryToDate) == false)
                                {
                                    newFulfillmentItems.Add(ExtractUtil.MakeNewFulfillmentItem(jobItem, interval, job.QueryBaseUrl, newFulfillment.FulfillmentId));
                                }
                            }
                        }
                    }

                    List<FulfillmentItemEntity> fulfillmentItems = _fulfillmentItemRepository.SaveFulfillmentItems(newFulfillmentItems);

                    if (fulfillmentItems == null || fulfillmentItems.Count == 0)
                    {
                        _logger.LogCritical($"Error persisting [{fulfillmentItems.Count}] New Fulfillment Items from Fulfillment to Storage {newFulfillment.FulfillmentId} - @{DateTime.UtcNow}");

                        return (false, null, null);
                    }

                    return (true, newFulfillment, fulfillmentItems);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception thrown in CreateArchiveFulfillmentSaga", ex);
                return (false, null, null);
            }
        }

        public async Task UpdateFulfilment(FulfillmentEntity newFulfillment)
        {
            try
            {
                //Persist to Storage
                var savedNew = await _fulfillmentRepository.SaveFulfillment(newFulfillment);

                //Log Persistence Outcome
                string logString = $"Fulfillment {newFulfillment.JobName} -[{newFulfillment.FulfillmentId}] - Completed @{newFulfillment.JobCompletedDate} - Total Count = {newFulfillment.TotalCount} - From [{ newFulfillment.QueryFromDate}] To [{ newFulfillment.QueryToDate}]";
                if (savedNew == null)
                {
                    _logger.LogDebug($"Error Saving - {logString}");
                }
                else
                {
                    _logger.LogInformation(logString);
                }
            }
            catch (Exception)
            {

            }
        }

        public void UpdateFulfilmentItem(FulfillmentItemEntity fulfillmentItem)
        {
            try
            {
                //Save to fulfillment Item to database
                var savedItem =  _fulfillmentItemRepository.SaveFulfillmentItem(fulfillmentItem);

                //Log fulfillment Item summary
                string activeCode = string.IsNullOrEmpty(fulfillmentItem.QuerySubjectCode)
                                        ? fulfillmentItem.QuerySubjectGroup 
                                        : fulfillmentItem.QuerySubjectCode;

                string logFulfillmentItem = $"FulfillmentItem [{fulfillmentItem.ItemUId}] - Subject Query [{activeCode}] - Started @{fulfillmentItem.JobItemStartDate} - Completed @{fulfillmentItem.JobItemCompletedDate} - Fetched ={fulfillmentItem.TotalResults}";

                if (savedItem != null)
                    _logger.LogInformation(logFulfillmentItem);
                else
                    _logger.LogError($"Error Saving {logFulfillmentItem}");
            }
            catch (Exception)
            {
            }
        }
    }
}
