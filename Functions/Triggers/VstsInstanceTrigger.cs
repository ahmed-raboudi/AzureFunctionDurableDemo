using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using SkillsBundle.Services;

namespace SkillsBundle.Function.Triggers
{
    public static class VstsInstanceTriggers
    {
        [FunctionName("ReQueueAvailableVstsInstance")]
        public static async Task ReQueueAvailableVstsInstance(
            [QueueTrigger("vstspendingavailableinstances", Connection = "SkillsBundleTablesConnectionsString")] string pendingInstance,
             IBinder binder,
            ILogger log)
        {
            var VstsInstanceService = new VstsInstanceService(Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString"));
            var vstsInstanceEntity = await VstsInstanceService.Get(pendingInstance);
            var queueAttribute = new QueueAttribute($"vstsavailableinstances-{vstsInstanceEntity.Region}");
            queueAttribute.Connection = "SkillsBundleTablesConnectionsString";

            var queue = await binder.BindAsync<CloudQueue>(queueAttribute);
            await queue.AddMessageAsync(new CloudQueueMessage(pendingInstance));
        }

        [FunctionName("QueueAvailableVSTSInstance")]
        public static void TimerTriggerCSharp(
            [TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }
            
        }
    }
}