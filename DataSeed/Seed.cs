using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using VSTS.Entities;
using SkillsBundle.Services;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Skillsbundle.Data
{
    public static class Seed
    {
        private static CloudStorageAccount storageAccount;
        private static Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccountForQueue;
        public static async Task Init(string azureStorageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            storageAccountForQueue = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(azureStorageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            var queueClient = storageAccountForQueue.CreateCloudQueueClient();

            // Get reference for storage tables and queues
            var countryCloudTable = tableClient.GetTableReference("Country");
            var vstsInstanceCloudTable = tableClient.GetTableReference("VstsInstance");
            var vstsUserCloudTable = tableClient.GetTableReference("VstsUser");
            var talentChallengeCloudTable = tableClient.GetTableReference("TalentChallenges");
            var vstsavailableinstancesWestEurope = queueClient.GetQueueReference($"vstsavailableinstances-westeurope");
            var vstspendingavailableinstancesQueue = queueClient.GetQueueReference($"vstspendingavailableinstances");

            // Create the storage tables and queues if not exists

            await vstsavailableinstancesWestEurope.CreateIfNotExistsAsync();
            await vstspendingavailableinstancesQueue.CreateIfNotExistsAsync();


            var isCountryInitialized = await countryCloudTable.ExistsAsync();
            var isVSTSInstanceInitialized = await vstsInstanceCloudTable.ExistsAsync();
            var isVSTSUserInitialized = await vstsUserCloudTable.ExistsAsync();
            var isTalentChallengeInitialized = await talentChallengeCloudTable.ExistsAsync();

            // Init data
            if (!isCountryInitialized)
            {
                await countryCloudTable.CreateAsync();
                await InitCountries(countryCloudTable);
            }

            if (!isVSTSInstanceInitialized)
            {
                await vstsInstanceCloudTable.CreateAsync();
                await InitVSTSInstances(vstsInstanceCloudTable,vstspendingavailableinstancesQueue);
            }

             if (!isVSTSUserInitialized)
            {
                await vstsUserCloudTable.CreateAsync();                
            }

             if (!isTalentChallengeInitialized)
            {
                await talentChallengeCloudTable.CreateAsync();                
            }
        }

        private async static Task InitCountries(CloudTable countryCloudTable)
        {
            // Create the TableOperation object that inserts the customer entity.
            TableBatchOperation batchOperation = new TableBatchOperation();
            var countries = InitData.GetCountriesData();
            for (var i = 0; i <= countries.Count - 1; i++)
            {
                var countryEntity = countries[i];
                batchOperation.Insert(countryEntity);
                if (i % 50 == 0)
                {
                    // Execute the insert operation.
                    await countryCloudTable.ExecuteBatchAsync(batchOperation);
                    batchOperation = new TableBatchOperation();
                }
            }
            await countryCloudTable.ExecuteBatchAsync(batchOperation);
        }

        private async static Task InitVSTSInstances(CloudTable vstsInstanceCloudTable, CloudQueue pendingVstsCloudQueue)
        {
            var vstsEntity = new VstsInstanceEntity("SkillsBundle", "westeurope");
            vstsEntity.AvailableFreeBasicUserSlot = 4;
            var vstsInstanceService = new VstsInstanceService(Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString"));
            await vstsInstanceService.Add(vstsEntity);
            await pendingVstsCloudQueue.AddMessageAsync(new CloudQueueMessage(vstsEntity.InstanceName));
        }
    }
}