using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace arx.Extract.Data.Extensions
{
    public static class BatchInsertExtensions
    {
        public static IList<TableResult> ExecuteBatchAsLimitedBatches(this CloudTable table,
                                                              TableBatchOperation batch,
                                                              TableRequestOptions requestOptions)
        {
            if (IsBatchCountUnderSupportedOperationsLimit(batch))
            {
                return table.ExecuteBatch(batch, requestOptions);
            }

            var result = new List<TableResult>();
            var limitedBatchOperationLists = GetLimitedBatchOperationLists(batch);
            foreach (var limitedBatchOperationList in limitedBatchOperationLists)
            {
                var limitedBatch = CreateLimitedTableBatchOperation(limitedBatchOperationList);
                var limitedBatchResult = table.ExecuteBatch(limitedBatch, requestOptions);
                result.AddRange(limitedBatchResult);
            }

            return result;
        }

        private static bool IsBatchCountUnderSupportedOperationsLimit(TableBatchOperation batch)
        {
            return batch.Count <= TableConstants.TableServiceBatchMaximumOperations;
        }

        private static IEnumerable<List<TableOperation>> GetLimitedBatchOperationLists(TableBatchOperation batch)
        {
            return batch.ChunkBy(TableConstants.TableServiceBatchMaximumOperations);
        }
        private static TableBatchOperation CreateLimitedTableBatchOperation(IEnumerable<TableOperation> limitedBatchOperationList)
        {
            var limitedBatch = new TableBatchOperation();
            foreach (var limitedBatchOperation in limitedBatchOperationList)
            {
                limitedBatch.Add(limitedBatchOperation);
            }

            return limitedBatch;
        }
        public static List<List<T>> ChunkBy<T>(this IList<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
