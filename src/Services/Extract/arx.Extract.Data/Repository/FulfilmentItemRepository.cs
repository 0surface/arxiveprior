using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace arx.Extract.Data.Repository
{
    public interface IFulfilmentItemRepository
    {
        FulfilmentItemEntity SaveFulfilmentItem(FulfilmentItemEntity fulfilmentItem);
        List<FulfilmentItemEntity> SaveFulfilmentItems(List<FulfilmentItemEntity> newFulfilmentItems);
    }
    public class FulfilmentItemRepository : TableStorage, IFulfilmentItemRepository
    {
        public FulfilmentItemRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }
        public List<FulfilmentItemEntity> GetFulfilmentItems(List<FulfilmentItemEntity> newFulfilmentItems)
        {
            throw new NotImplementedException();
        }

        public List<FulfilmentItemEntity> SaveFulfilmentItems(List<FulfilmentItemEntity> newFulfilmentItems)
        {
            List<FulfilmentItemEntity> entities = new List<FulfilmentItemEntity>();
            try
            {
                foreach (var item in newFulfilmentItems)
                {
                    var tableResult = InsertOrReplace(item).Result;

                    if (tableResult.HttpStatusCode >= (int)HttpStatusCode.OK
                        && tableResult.HttpStatusCode < (int)HttpStatusCode.Ambiguous)
                    {
                        entities.Add((FulfilmentItemEntity)tableResult.Result);
                    }
                }
            }
            catch (Exception) { }

            return entities;
        }

        public FulfilmentItemEntity SaveFulfilmentItem(FulfilmentItemEntity fulfilmentItem)
        {
            try
            {
                var response = InsertOrReplace(fulfilmentItem).Result;
                return (FulfilmentItemEntity)response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
