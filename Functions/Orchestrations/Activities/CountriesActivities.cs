using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Skillsbundle.Data;
using SkillsBundle.Services;
using SkillsBundle.Helpers;
using Microsoft.Extensions.Logging;

namespace SkillsBundle.Function.Activities
{
    public static class CountriesActivities
    {
        [FunctionName("GetCountryBestAzureLocation")]
        public static async Task<string> GetCountryBestAzureLocation(
           [ActivityTrigger] string countryCode,
           ILogger log
       )
        {
            var countryService = new CountryService(AzureStorageHelper.ConnectionString);
            var countryEntity = await countryService.Get(countryCode);
            return countryEntity.AzureLocation;            
        }
    }
}