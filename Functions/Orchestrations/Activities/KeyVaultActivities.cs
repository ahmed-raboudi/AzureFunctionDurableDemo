using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Microsoft.Extensions.Logging;

namespace SkillsBundle.Function.Activities
{
    public static class KeyVaultActivities
    {
        [FunctionName("GetKVSecret")]
        public static async Task<string> GetSecret(
           [ActivityTrigger] string secretUrl,
           ILogger log
       )
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            try
            {
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var secret = await keyVaultClient.GetSecretAsync(secretUrl)
                    .ConfigureAwait(false);
                return secret.Value;
            }
            catch (Exception exp)
            {
                log.LogError($"Something went wrong: {exp.ToString()}");
            }
            return null;
        }
    }
}