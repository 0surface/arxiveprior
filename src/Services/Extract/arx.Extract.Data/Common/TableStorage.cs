using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arx.Extract.Data.Common
{
    /* Modified code from github source. See source for further reference 
     https://github.com/microsoft/Azure.Data.Wrappers/blob/master/Azure.Data.Wrappers/TableStorage.cs*/
    public class TableStorage  : AzureStorage
    {
        #region Members
        /// <summary>
        /// Partition Key
        /// </summary>
        public const string PartitionKey = "PartitionKey";

        /// <summary>
        /// Row Key
        /// </summary>
        public const string RowKey = "RowKey";

        /// <summary>
        /// Timestamp
        /// </summary>
        public const string Timestamp = "Timestamp";

        /// <summary>
        /// ETag
        /// </summary>
        public const string ETag = "ETag";

        /// <summary>
        /// Maximum Insert Batch
        /// </summary>
        public const int MaimumxInsertBatch = 100;

        /// <summary>
        /// Table Client
        /// </summary>
        private readonly CloudTableClient client;

        /// <summary>
        /// Table
        /// </summary>
        private readonly CloudTable reference;
        #endregion

        #region Constructors
        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="location">Location Mode</param>
        public TableStorage(string tableName, string connectionString, LocationMode location = LocationMode.PrimaryThenSecondary)
            : this(tableName, CloudStorageAccount.Parse(connectionString), location)
        {
        }

        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="account">Storage Account</param>
        /// <param name="location">Location Mode</param>
        public TableStorage(string tableName, CloudStorageAccount account, LocationMode location = LocationMode.PrimaryThenSecondary)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("tableName");
            }

            this.client = base.Account.CreateCloudTableClient();
            this.client.DefaultRequestOptions.LocationMode = location;

            this.reference = client.GetTableReference(tableName);
        }
        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="options">Default request options</param>
        public TableStorage(string tableName, string connectionString, TableRequestOptions options)
            : this(tableName, CloudStorageAccount.Parse(connectionString), options)
        {
        }
        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="account">Storage Account</param>
        /// <param name="options">Default request options</param>
        public TableStorage(string tableName, CloudStorageAccount account, TableRequestOptions options)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("tableName");
            }
            if (options == null)
                throw new ArgumentNullException("options");

            this.client = base.Account.CreateCloudTableClient();
            this.client.DefaultRequestOptions = options;

            this.reference = client.GetTableReference(tableName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Table Name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.reference.Name;
            }
        }

        /// <summary>
        /// Table Client
        /// </summary>
        public virtual CloudTableClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// Table
        /// </summary>
        public virtual CloudTable Reference
        {
            get
            {
                return this.reference;
            }
        }
        #endregion

        #region Create Table
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            return await this.reference.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        public virtual async Task<bool> Create()
        {
            return await this.reference.CreateIfNotExistsAsync().ConfigureAwait(false);
        }
        #endregion

        #region Query Object
        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="partitionKey"></param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<T>> QueryByPartition<T>(string partitionKey)
            where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));
            return await this.Query<T>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <remarks>
        /// Without providing the partion this query may not perform well.
        /// </remarks>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<T>> QueryByRow<T>(string rowKey)
            where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey));
            return await this.Query<T>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Query By Partition and Row
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row</param>
        /// <returns></returns>
        public virtual async Task<T> QueryByPartitionAndRow<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new()
        {
            var partitionFilter = TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey);
            var rowFilter = TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey);
            var filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);
            var query = new TableQuery<T>().Where(filter);

            var result = await this.Query<T>(query).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Query
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="query">Table Query</param>
        /// <returns>Results</returns>
        public virtual async Task<IEnumerable<T>> Query<T>(TableQuery<T> query)
            where T : ITableEntity, new()
        {
            if (null == query)
            {
                throw new ArgumentNullException("query");
            }

            var entities = new List<T>();
            TableContinuationToken token = null;

            do
            {
                var queryResult = await this.reference.ExecuteQuerySegmentedAsync<T>(query, token).ConfigureAwait(false);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (null != token);

            return entities;
        }

        public virtual async Task<bool> TableExists()
        {
            return await this.reference.ExistsAsync();
        }

        public virtual bool PartitionExists(string partitionKey)
        {
            var query = new TableQuery().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));
            TableContinuationToken token = null;
            return  this.reference.ExecuteQuerySegmentedAsync(query, token).Result.Results?.Any() ?? false;
        }

        public virtual bool HasAnyPartitionKey()
        {
            var query = new TableQuery();
            TableContinuationToken token = null;
            return this.reference.ExecuteQuerySegmentedAsync(query, token).Result.Results?.Any() ?? false;
        }

        #endregion

        #region Save Data
        /// <summary>
        /// Insert or update the record in table
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<TableResult> InsertOrReplace(ITableEntity entity)
        {
            return await this.reference.ExecuteAsync(TableOperation.InsertOrReplace(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<TableResult>> Insert(IEnumerable<ITableEntity> entities)
        {
            var result = new List<TableResult>();

            foreach (var batch in this.Batch(entities))
            {
                var batchOperation = new TableBatchOperation();
                batch.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
                var r = await this.reference.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
                result.AddRange(r);
            }

            return result;
        }
        #endregion

        #region Additional Methods
        /// <summary>
        /// Break Entities into batches
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Batches</returns>
        public virtual IEnumerable<IEnumerable<ITableEntity>> Batch(IEnumerable<ITableEntity> entities)
        {
            return entities.GroupBy(en => en.PartitionKey).SelectMany(e => this.Chunk<ITableEntity>(e));
        }

        /// <summary>
        /// Break Entities into batches
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Batches</returns>
        public virtual IEnumerable<IEnumerable<IDictionary<string, object>>> Batch(IEnumerable<IDictionary<string, object>> entities)
        {
            return entities.GroupBy(en => en[PartitionKey]).SelectMany(e => this.Chunk<IDictionary<string, object>>(e));
        }

        /// <summary>
        /// Chunk data into smaller blocks
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entities">Entities</param>
        /// <returns>Chunks</returns>
        public virtual IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> entities)
        {
            return entities.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / TableStorage.MaimumxInsertBatch).Select(x => x.Select(v => v.Value));
        }
        #endregion
    }
}
