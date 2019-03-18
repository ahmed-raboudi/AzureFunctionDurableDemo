using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using VSTS.Entities;

namespace Skillsbundle.AzureTable.Repositories
{
    public class CountryRepository
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTable countryCloudTable;
        public CountryRepository(string azureStorageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            countryCloudTable = tableClient.GetTableReference("Country");
        }

        public async Task<CountryEntity> Get(string countryCode)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CountryEntity>("Common", $"CO_{countryCode}");
            // Execute the retrieve operation.
            TableResult retrievedResult = await countryCloudTable.ExecuteAsync(retrieveOperation);
            // Print the phone number of the result.
            return retrievedResult.Result as CountryEntity;
        }
    }
}