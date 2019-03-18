using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using VSTS.Entities;

namespace Skillsbundle.AzureTable.Repositories
{
    public class VstsInstanceRepository
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccountForQueue;
        private readonly CloudTable vstsUserCloudTable;
        public VstsInstanceRepository(string azureStorageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            storageAccountForQueue = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(azureStorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            vstsUserCloudTable = tableClient.GetTableReference("VstsInstance");
            
        }

        public async Task Add(VstsInstanceEntity vstsInstanceEntity)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(vstsInstanceEntity);
            // Execute the insert operation.
            await vstsUserCloudTable.ExecuteAsync(insertOperation);
        }

        public async Task<VstsInstanceEntity> Get(string instanceName)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<VstsInstanceEntity>("VSTS", $"Ins_{instanceName}");
            // Execute the retrieve operation.
            TableResult retrievedResult = await vstsUserCloudTable.ExecuteAsync(retrieveOperation);
            // Print the phone number of the result.
            return retrievedResult.Result as VstsInstanceEntity;
        }

        public async Task<VstsInstanceEntity> GetAvailableInstance(string azureLocation)
        {
            CloudQueueClient queueClient = storageAccountForQueue.CreateCloudQueueClient();
            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference($"vstsavailableinstances-{azureLocation}");
            // Get the next message
            CloudQueueMessage availableInstance = await queue.GetMessageAsync();
            if (availableInstance == null)
            {
                throw new Exception($"No more availble VSTS instance on region [{azureLocation}]. Please try later.");
            }
            var instanceName = availableInstance.AsString;
            var vstsInstanceEntity = await Get(instanceName);
            await queue.DeleteMessageAsync(availableInstance);
            return vstsInstanceEntity;
        }

        public async Task Update(VstsInstanceEntity vstsInstanceEntity)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Replace(vstsInstanceEntity);
            // Execute the insert operation.
            await vstsUserCloudTable.ExecuteAsync(insertOperation);
        }
    }
}