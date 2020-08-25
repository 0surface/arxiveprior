using arx.Extract.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace arx.Extract.BackgroundTasks.Core
{
    public interface IExtractService
    {
        (bool, JobEntity, List<JobItemEntity>) GetArchiveJob();

        (bool,bool, FulfillmentEntity) GetLastSuccessfulArchiveFulfillment(string jobName);

        (bool, FulfillmentEntity, List<FulfillmentItemEntity>) CreateArchiveFulfillmentSaga
            (JobEntity job, List<JobItemEntity> jobItems, FulfillmentEntity lastFulfillment, bool isFirstFulfillment);

        Task UpdateFulfilment(FulfillmentEntity newFulfillment);

        void UpdateFulfilmentItem(FulfillmentItemEntity newFulfillmentItem);
    }
}
