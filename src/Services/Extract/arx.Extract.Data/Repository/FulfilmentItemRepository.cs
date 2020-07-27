using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace arx.Extract.Data.Repository
{
    public interface IFulfillmentItemRepository
    {
        Task<List<FulfillmentItemEntity>> GetFulfillmentItems(string fulfillmentId);
        FulfillmentItemEntity SaveFulfillmentItem(FulfillmentItemEntity fulfillmentItem);
        List<FulfillmentItemEntity> SaveFulfillmentItems(List<FulfillmentItemEntity> newFulfillmentItems);
    }
    public class FulfillmentItemRepository : TableStorage, IFulfillmentItemRepository
    {
        public FulfillmentItemRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public async Task<List<FulfillmentItemEntity>> GetFulfillmentItems(string fulfillmentId)
        {
            var response = await QueryByPartition<FulfillmentItemEntity>(fulfillmentId);

            return response?.ToList() ?? new List<FulfillmentItemEntity>();
        }

        public List<FulfillmentItemEntity> SaveFulfillmentItems(List<FulfillmentItemEntity> newFulfillmentItems)
        {
            List<FulfillmentItemEntity> entities = new List<FulfillmentItemEntity>();
            try
            {
                foreach (var item in newFulfillmentItems)
                {
                    var tableResult = InsertOrReplace(item).Result;

                    if (tableResult.HttpStatusCode >= (int)HttpStatusCode.OK
                        && tableResult.HttpStatusCode < (int)HttpStatusCode.Ambiguous)
                    {
                        entities.Add((FulfillmentItemEntity)tableResult.Result);
                    }
                }
            }
            catch (Exception) { }

            return entities;
        }

        public FulfillmentItemEntity SaveFulfillmentItem(FulfillmentItemEntity fulfillmentItem)
        {
            try
            {
                var response = InsertOrReplace(fulfillmentItem).Result;
                return (FulfillmentItemEntity)response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
