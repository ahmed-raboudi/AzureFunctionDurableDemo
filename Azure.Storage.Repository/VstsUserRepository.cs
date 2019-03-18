using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using VSTS.Entities;

namespace Skillsbundle.AzureTable.Repositories
{
    public class VstsUserRepository
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTable vstsUserCloudTable;
        public VstsUserRepository(string azureStorageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            vstsUserCloudTable = tableClient.GetTableReference("VstsUser");
        }

        public void Add(VstsUserEntity vstsUserEntity)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(vstsUserEntity);
            // Execute the insert operation.
            vstsUserCloudTable.ExecuteAsync(insertOperation);
        }

        public VstsUserEntity Get(string userOID)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<VstsUserEntity>("VSTS", $"U_{userOID}");
            // Execute the retrieve operation.
            var retrievedResult =  vstsUserCloudTable.ExecuteAsync(retrieveOperation);
            // Print the phone number of the result.
            return retrievedResult.Result.Result as VstsUserEntity;
        }
    }
}