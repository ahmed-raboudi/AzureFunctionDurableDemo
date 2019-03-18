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
    public static class VstsGitRepositoriesActivities
    {

        [FunctionName("ImportChallengeSourceGitRepository")]
        public static async Task<string> ImportChallengeSourceGitRepository(
                 [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
                 ILogger log
             )
        {
            string Url = $@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_apis/git/repositories/{vstsIntegrationContext.ChallengeProjectName}/importRequests?api-version=4.0-preview";
            var content = JsonConvert.SerializeObject(
                new
                {
                    parameters = new
                    {
                        gitSource = new
                        {
                            url = vstsIntegrationContext.ChallengeSourceGitUrl
                        },
                        serviceEndpointId = vstsIntegrationContext.ChallengeSourceGitEndPointId,
                        deleteServiceEndpointAfterImportIsDone = true
                    }
                });
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "POST", content);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.url;
        }

        [FunctionName("GetDefaultProjectGitRepositorySize")]
        public static async Task<int> GetDefaultProjectGitRepositorySize(
                          [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
                          ILogger log
                      )
        {
            string Url = string.Format($@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_apis/git/repositories/{vstsIntegrationContext.ChallengeProjectName}?api-version=4.1-preview");           
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "GET", null);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.size;
        }     
    }
}