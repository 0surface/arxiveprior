using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace arx.Extract.Data.Common
{
    /* Modified code from github source. See source for further reference 
    https://github.com/microsoft/Azure.Data.Wrappers/blob/master/Azure.Data.Wrappers/TableStorage.cs*/
    public class AzureStorage
    {
        #region Members
        /// <summary>
        /// Cloud Storage Account
        /// </summary>
        private readonly CloudStorageAccount account;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        public AzureStorage(string connectionString)
            : this(CloudStorageAccount.Parse(connectionString))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">Storage Account</param>
        public AzureStorage(CloudStorageAccount account)
        {
            if (null == account)
            {
                throw new ArgumentNullException(nameof(account));
            }

            this.account = account;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Cloud Storage Account
        /// </summary>
        public CloudStorageAccount Account
        {
            get
            {
                return this.account;
            }
        }

        public string GetSharedAccessSignature(SharedAccessAccountPolicy policy)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            return this.account.GetSharedAccessSignature(policy);
        }
        #endregion
    }
}
