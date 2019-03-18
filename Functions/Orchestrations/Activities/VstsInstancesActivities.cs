using System.Net;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using SkillsBundle.Function.Models;
using SkillsBundle.Services;
using VSTS.Entities;
using Microsoft.Extensions.Logging;

namespace SkillsBundle.Function.Activities
{
    public static class VstsInstancesActivities
    {
        [FunctionName("GetAvailableVSTSInstance")]
        public static async Task<VstsInstanceEntity> GetAvailableVSTSInstance(
           [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
           ILogger log
       )
        {
            var VstsInstanceService = new VstsInstanceService(Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString"));
            
            return await VstsInstanceService.GetAvailableInstance(vstsIntegrationContext.AzureLocation);
        }
    }
}