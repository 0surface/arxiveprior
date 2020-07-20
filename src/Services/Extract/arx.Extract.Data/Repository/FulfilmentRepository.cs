using arx.Extract.Data.Common;
using arx.Extract.Data.Entities;
using System;
using System.Collections.Generic;

namespace arx.Extract.Data.Repository
{
    public interface IFulfilmentRepository
    {
        FulfilmentEntity GetLastJobRecord();
        List<FulfilmentItemEntity> GetJobRecordItems(string JobRecordId);
        FulfilmentEntity SaveFulfilment(FulfilmentEntity jobRecord);      
        
    }
    public class FulfilmentRepository : TableStorage, IFulfilmentRepository
    {
        public FulfilmentRepository(string connectionString, string tableName) : base(tableName, connectionString)
        {
            Reference.CreateIfNotExists();
        }

        public FulfilmentEntity GetLastJobRecord()
        {
            throw new NotImplementedException();
        }

        public FulfilmentEntity SaveFulfilment(FulfilmentEntity fulfilment)
        {
            try
            {
                var response = InsertOrReplace(fulfilment).Result;
                return (FulfilmentEntity)response.Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
    }
}
