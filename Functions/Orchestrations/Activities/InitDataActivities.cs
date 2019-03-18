using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Skillsbundle.Data;
using Microsoft.Extensions.Logging;

namespace SkillsBundle.Function.Activities
{
    public static class InitDataActivities
    {
        [FunctionName("InitData")]
        public static async Task InitData(
           [ActivityTrigger] string connectionString,
           ILogger log
       )
        {            
            await Seed.Init(connectionString);
        }
    }
}