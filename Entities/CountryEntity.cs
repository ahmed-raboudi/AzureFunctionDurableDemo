using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class CountryEntity : TableEntity
    {
        public CountryEntity(string countryName, string countryCode, string azureLocation)
        {
            this.PartitionKey = "Common";
            this.RowKey = $"CO_{countryCode}";
            this.CountryCode = countryCode;
            this.CountryName = countryName;
            this.AzureLocation = azureLocation;
        }
        public CountryEntity() { }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string AzureLocation { get; set; }
    }
}