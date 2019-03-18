using System.Net;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using SkillsBundle.Function.Models;
using VSTS.Helpers;
using Microsoft.Extensions.Logging;

namespace SkillsBundle.Function.Activities
{
    public static class VstsLongrunningOperationActivities
    {
        [FunctionName("CheckLongRunningOperationStatus")]
        public static async Task<bool> CheckLongRunningOperationStatus(
             [ActivityTrigger] VSTSLongrunningOperationContext vstsLongrunningOperationContext,
                  ILogger log
              )
        {
            var response = await VSTSHelpers.CallVSTSAPI(vstsLongrunningOperationContext.VstsInstance
                                            , vstsLongrunningOperationContext.VstsPAT, vstsLongrunningOperationContext.Url, "GET", null);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            if (data.status != "succeeded")
            {
                throw new Exception($"Operation not completed : {vstsLongrunningOperationContext.Url}");
            }
            return true;
        }
        [FunctionName("GetVSTSOperationStatus")]
        public static async Task<string> GetVSTSOperationStatus(
                     [ActivityTrigger] VSTSLongrunningOperationContext vstsLongrunningOperationContext,
                          ILogger log
                      )
        {
            var response = await VSTSHelpers.CallVSTSAPI(vstsLongrunningOperationContext.VstsInstance
                                            , vstsLongrunningOperationContext.VstsPAT, vstsLongrunningOperationContext.Url, "GET", null);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.status;
        }
    }
}