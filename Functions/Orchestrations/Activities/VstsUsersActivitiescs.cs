using System.Net;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using VSTS.Entities;
using SkillsBundle.Services;
using SkillsBundle.Function.Models;
using VSTS.Helpers;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace SkillsBundle.Function.Activities
{
    public static class VstsUsersActivities
    {

        [FunctionName("AssignUserVSTSInstance")]
        public static void Add(
             [ActivityTrigger] VSTSUserIntegrationContext vstsUserIntegrationContext,
             ILogger log
         )
        {
            // Parse the connection string and return a reference to the storage account.
            var connectionString = Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString");
            var vstsUserService = new VstsUserService(connectionString);
            var vstsUserEntity = new VstsUserEntity(vstsUserIntegrationContext.UserOID, vstsUserIntegrationContext.VstsInstance
            , vstsUserIntegrationContext.VstsUserId);
            vstsUserService.Add(vstsUserEntity);
        }

        [FunctionName("GetUserVSTSInstance")]
        public static VstsUserEntity GetUserVSTSInstance(
             [ActivityTrigger] string userOID,
             ILogger log
         )
        {
            // Parse the connection string and return a reference to the storage account.
            var connectionString = Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString");
            var vstsUserService = new VstsUserService(connectionString);
            return vstsUserService.Get(userOID);
        }

        [FunctionName("AddUserEntitlment")]
        public static async Task<string> AddUserEntitlment(
            [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
            ILogger log
        )
        {
            try
            {
                var accountName = vstsIntegrationContext.VstsInstance;
                string Url = string.Format(@"https://{0}.vsaex.visualstudio.com/_apis/userentitlements?api-version=4.1-preview"
                            , vstsIntegrationContext.VstsInstance);
                var content = JsonConvert.SerializeObject(
                    new
                    {
                        accessLevel = new
                        {
                            accountLicenseType = "express"
                        },
                        user = new
                        {
                            principalName = vstsIntegrationContext.Email,
                            subjectKind = "user"
                        }
                    });
                    log.LogInformation("===========PAT: vstsIntegrationContext.VstsPAT");
                var response = await VSTSHelpers.CallVSTSAPI(vstsIntegrationContext.VstsInstance, vstsIntegrationContext.VstsPAT, Url, "POST", content);
                log.LogInformation("====response:" + response);
                response.EnsureSuccessStatusCode();                
                dynamic data = await response.Content.ReadAsAsync<object>();
                return data.operationResult.userId;
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw;
            }


        }
        [FunctionName("AddUserAsProjectContributor")]
        public static async Task<string> AddUserAsProjectContributor(
           [ActivityTrigger] VSTSIntegrationContext vstsIntegrationContext,
           ILogger log
       )
        {
            log.LogInformation("Start adding the user to VSTS");
            var accountName = vstsIntegrationContext.VstsInstance;
            string Url = string.Format(@"https://{0}.vsaex.visualstudio.com/_apis/userentitlements/{1}?api-version=5.0-preview.2"
                        , vstsIntegrationContext.VstsInstance
                        , vstsIntegrationContext.VstsUserId);
            var content = JsonConvert.SerializeObject(
                new List<object>(){new
                    {
                        from = "",
                        op = "add",
                        path = "/projectEntitlements",
                        value = new{
                            projectRef = new {
                                id = vstsIntegrationContext.ChallengeCreatedProjectId
                            },
                            group = new {
                                groupType = "projectContributor"
                            }
                        }
                    }});

            var response = await VSTSHelpers.CallRestAPI(vstsIntegrationContext.VstsPAT, Url, "Patch", content, log, "application/json-patch+json");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            dynamic data = await response.Content.ReadAsAsync<object>();
            bool isSuccess = data.isSuccess;
            if (!isSuccess)
            {
                var exceptionMessage= "$Could not add user {vstsIntegrationContext.VstsUserId} as contributor to project {vstsIntegrationContext.ChallengeCreatedProjectId} on instance vstsIntegrationContext.VstsInstance";
                string responseErrorMessage = data.errors[0].value;
                exceptionMessage += responseErrorMessage;
                throw new Exception(exceptionMessage);
            }
            return isSuccess.ToString();

        }
    }
}