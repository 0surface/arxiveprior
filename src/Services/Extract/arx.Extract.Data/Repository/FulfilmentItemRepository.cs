using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
            throw new NotImplementedException();
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
