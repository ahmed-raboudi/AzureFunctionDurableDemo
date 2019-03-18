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
using System.Collections.Generic;

namespace SkillsBundle.Function.Activities
{
    public static class VstsServiceEPActivities
    {
        [FunctionName("CreateSonarServiceEndpoint")]
        public static async Task<string> CreateSonarServiceEndpoint(
                          [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
                          ILogger log
                      )
        {
            string Url = string.Format($@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_apis/distributedtask/serviceendpoints?api-version=4.1-preview");
            var content = JsonConvert.SerializeObject(
                new
                {
                    name = "SonarCloudServiceEP",
                    type = "sonarcloud",
                    url = "https://sonarcloud.io",
                    authorization = new
                    {
                        scheme = "Token",
                        parameters = new
                        {
                            apitoken = vstsIntegrationContext.SonarToken
                        }
                    },
                    isReady = true
                });
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "POST", content);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.id;
        }

        [FunctionName("CreateChallengeSourceGitServiceEndpoint")]
        public static async Task<string> CreateChallengeSourceGitServiceEndpoint(
                          [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
                          ILogger log
                      )
        {
            string Url = string.Format($@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_apis/distributedtask/serviceendpoints?api-version=4.1-preview");
            var content = JsonConvert.SerializeObject(
                new
                {
                    name = vstsIntegrationContext.ChallengeProjectName,
                    type = "git",
                    url = vstsIntegrationContext.ChallengeSourceGitUrl,
                    authorization = new
                    {
                        scheme = "UsernamePassword",
                        parameters = new
                        {
                            username = "",
                            password = vstsIntegrationContext.VstsPAT
                        }
                    },
                    isReady = true
                });
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "POST", content);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.id;
        }

        [FunctionName("GetProjectEndpoints")]
        public static async Task<Dictionary<string, string>> GetProjectEndpoints(
                         [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
                         ILogger log
                     )
        {
            string Url = string.Format($@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_apis/serviceendpoint/endpoints?api-version=5.0-preview.2");
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "GET", null);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            var listEP = new Dictionary<string, string>();
            foreach (var ep in data.value)
            {
                string id = ep.id;
                string name = ep.name;
                log.LogInformation($"id EP: {id} and name : {name}");
                listEP.Add(id, name);
            }
            return listEP; ;
        }
    }
}