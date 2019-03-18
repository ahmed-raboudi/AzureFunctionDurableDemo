using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Skillsbundle.Data;

namespace SkillsBundle.Function.Orchestrations
{
    public static class InitializeData
    {
        [FunctionName("SeedData")]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log
        )
        {
           log.LogInformation("Start User VSTS Integration");
           await context.CallActivityAsync("InitData", Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString"));                           
        }
    }
}