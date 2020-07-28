using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;

namespace arx.Extract.BackgroundTasks.Core
{
    public interface IExtractService
    {
        (bool, JobEntity, List<JobItemEntity>) GetArchiveJob(string jobName);

        (bool, FulfillmentEntity) GetLastSuccessfulArchiveFulfillment(string jobName);

        (bool, FulfillmentEntity, List<FulfillmentItemEntity>) CreateArchiveFulfillmentSaga(List<JobItemEntity> jobItems, FulfillmentEntity lastFulfillment, DateTime archiveTerminateDate);
    }
}
