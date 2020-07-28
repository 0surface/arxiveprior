using arx.Extract.Data.Entities;
using arx.Extract.Data.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace arx.Extract.BackgroundTasks.Core
{
    public class ExtractService : IExtractService
    {
        private readonly IOptions<BackgroundTaskSettings> _configSettings;
        private readonly ILogger<ExtractService> _logger;
        private readonly IJobRepository _jobRepository;
        private readonly IFulfillmentRepository _fulfillmentRepository;
        private readonly IFulfillmentItemRepository _fulfillmentItemRepository;
        private readonly IJobItemRepository _jobItemRepository;

        public ExtractService(IOptions<BackgroundTaskSettings> configSettings,
            ILogger<ExtractService> logger,
            IJobRepository jobRepository,
            IJobItemRepository jobItemRepository,
            IFulfillmentRepository fulfillmentRepository,
            IFulfillmentItemRepository fulfillmentItemRepository)
        {
            _configSettings = configSettings;
            _logger = logger;
            _jobRepository = jobRepository;
            _fulfillmentRepository = fulfillmentRepository;
            _fulfillmentItemRepository = fulfillmentItemRepository;
            _jobItemRepository = jobItemRepository;
        }

        public (bool, FulfillmentEntity, List<FulfillmentItemEntity>) CreateArchiveFulfillmentSaga(List<JobItemEntity> jobItems, FulfillmentEntity lastFulfillment, DateTime archiveTerminateDate)
        {
            throw new NotImplementedException();
        }

        public (bool, JobEntity, List<JobItemEntity>) GetArchiveJob(string jobName)
        {
            throw new NotImplementedException();
        }

        public (bool, FulfillmentEntity) GetLastSuccessfulArchiveFulfillment(string jobName)
        {
            throw new NotImplementedException();
        }
    }
}
