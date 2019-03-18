using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
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
    public static class VSTSProjectActivities
    {
        [FunctionName("CreateVSTSProject")]
        public static async Task<string> CreateVSTSProject(
          [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
          ILogger log
      )
        {
            string Url = string.Format(@"https://{0}.visualstudio.com/_apis/projects?api-version=4.0-preview", vstsIntegrationContext.VstsInstance);
            var content = JsonConvert.SerializeObject(
                new
                {
                    name = $"{vstsIntegrationContext.ChallengeProjectName}",
                    description = vstsIntegrationContext.ChallengeProjectDescription,
                    capabilities = new
                    {
                        versioncontrol = new
                        {
                            sourceControlType = vstsIntegrationContext.SourceControlType
                        },
                        processTemplate = new
                        {
                            templateTypeId = vstsIntegrationContext.TemplateTypeId
                        }
                    }
                });
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "POST", content);
            response.EnsureSuccessStatusCode();
            dynamic data = await response.Content.ReadAsAsync<object>();
            return data.url;
        }

        [FunctionName("GetChallengeVSTSProject")]
        public static async Task<string> GetChallengeVSTSProject(
          [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
          ILogger log
      )
        {
            string Url = $@"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/_apis/projects/{vstsIntegrationContext.ChallengeProjectName}?api-version=4.0-preview";
            var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "GET", null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                dynamic data = await response.Content.ReadAsAsync<object>();
                return data.id;
            }
            else
            {
                return null;
            }

        }

    }
}