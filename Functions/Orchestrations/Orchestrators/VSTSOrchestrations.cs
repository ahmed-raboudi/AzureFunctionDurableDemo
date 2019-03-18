using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Azure;
using VSTS.Entities;
using SkillsBundle.Function.Models;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SkillsBundle.Function.Orchestrations
{
    public static class VSTSOrchestrations
    {
        [FunctionName("InitializeUserVSTSInstance")]
        /// <summary>
        /// Assign a user to an available VSTS instance located on the nearest Azure region.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task InitializeUserVSTSInstance(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log
        )
        {
            log.LogInformation("Start User VSTS Integration");
            var vstsIntegrationContext = context.GetInput<VSTSIntegrationContext>();
            //Check if the user has an assigned VSTS instance
            var userVstsInstanceEntity = await context.CallActivityAsync<VstsUserEntity>("GetUserVSTSInstance", vstsIntegrationContext.UserOID);
            if (userVstsInstanceEntity == null)
            {
                //Get VSTS PAT
                vstsIntegrationContext.VstsPAT =
                    await context.CallActivityAsync<string>("GetKVSecret", Environment.GetEnvironmentVariable("VSTSPATSecretURL"));
                VSTSUserIntegrationContext vstsUserIntegrationContext = new VSTSUserIntegrationContext()
                {
                    UserOID = vstsIntegrationContext.UserOID,
                };
                //Get user best VSTS Azure location
                vstsIntegrationContext.AzureLocation =
                        await context.CallActivityAsync<string>("GetCountryBestAzureLocation", vstsIntegrationContext.CountryCode);
                //Get an available VSTS instance with free slot
                var vstsInstanceEntity = await context.CallActivityAsync<VstsInstanceEntity>("GetAvailableVSTSInstance", vstsIntegrationContext);
                vstsUserIntegrationContext.VstsInstance = vstsInstanceEntity.InstanceName;
                vstsIntegrationContext.VstsInstance = vstsInstanceEntity.InstanceName;
                // Add the user as a basic VSTS user
                vstsUserIntegrationContext.VstsUserId = await context.CallActivityAsync<string>("AddUserEntitlment", vstsIntegrationContext);
                //Save user VSTS integration information's
                await context.CallActivityAsync<string>("AssignUserVSTSInstance", vstsUserIntegrationContext);
            }
        }

        [FunctionName("InitializeVSTSChallenge")]
        public static async Task<object> InitializeVSTSChallenge(
        [OrchestrationTrigger] DurableOrchestrationContext context,
        ILogger log)
        {
            if (!context.IsReplaying)
                log.LogInformation("======================== START =============================");
            var vstsIntegrationContext = context.GetInput<VSTSIntegrationContext>();
            var isRepositoryCreationNeeded = true;
            var KeyVaultTasks = new Task<string>[2];
            //Get VSTS PAT
            KeyVaultTasks[0] = context.CallActivityAsync<string>("GetKVSecret", Environment.GetEnvironmentVariable("VSTSPATSecretURL"));
            //Get SonarCloud Token
            KeyVaultTasks[1] = context.CallActivityAsync<string>("GetKVSecret", Environment.GetEnvironmentVariable("SonarSecretURL"));
            // Get Key vault in parallel
            await Task.WhenAll(KeyVaultTasks);
            vstsIntegrationContext.VstsPAT = KeyVaultTasks[0].Result;
            vstsIntegrationContext.SonarToken = KeyVaultTasks[1].Result;
            //Get VSTS instance assigned to the user
            var userVstsInstanceEntity = await context.CallActivityAsync<VstsUserEntity>("GetUserVSTSInstance", vstsIntegrationContext.UserOID);
            if (userVstsInstanceEntity != null)
            {
                vstsIntegrationContext.VstsInstance = userVstsInstanceEntity.VstsInstance;
                vstsIntegrationContext.VstsUserId = userVstsInstanceEntity.VstsUserId;
            }
            else
            {
                throw new NullReferenceException($"The user {vstsIntegrationContext.UserOID} was not assigned to a VSTS account");
            }
            //Check if the Challenge project exists in vsts instance
            var challengeProjectId = await context.CallActivityAsync<string>("GetChallengeVSTSProject", vstsIntegrationContext);
            if (string.IsNullOrEmpty(challengeProjectId))
            {
                //Create the Challenge project
                log.LogInformation("Start creating Team project");
                var vstsProjectCreationOperationUrl = await context.CallActivityAsync<string>("CreateVSTSProject", vstsIntegrationContext);

                var vstsProjectCreationOperationCtx = new VSTSLongrunningOperationContext
                {
                    OperationFriendlyName = "VSTSProjectCreationOperation",
                    Url = vstsProjectCreationOperationUrl,
                    VstsInstance = vstsIntegrationContext.VstsInstance,
                    VstsPAT = vstsIntegrationContext.VstsPAT,
                    CheckIntervalInSeconds = 5,
                    MaxRetryCount = 10
                };
                await context.CallSubOrchestratorAsync("GetVSTSOperationStatusOrchestration",
                                Guid.NewGuid().ToString(),
                                vstsProjectCreationOperationCtx);
                log.LogInformation("Project successfully created");
                // Get the project id
                challengeProjectId = await context.CallActivityAsync<string>("GetChallengeVSTSProject", vstsIntegrationContext);
            }
            else
            {
                //TODO: Check if there is another contributor for this project. In this case we have an issue with the projects naming pattern
                //Check if the project default git repository is already initialized
                var defaultGitRepoSize = await context.CallActivityAsync<int>("GetDefaultProjectGitRepositorySize", vstsIntegrationContext);
                if (defaultGitRepoSize > 0)
                {
                    isRepositoryCreationNeeded = false;
                    log.LogInformation("Default git repository was already imported");
                }
                // Get the list of available Service Endpoint on the project
                var listEP = await context.CallActivityAsync<Dictionary<string, string>>("GetProjectEndpoints", vstsIntegrationContext);
                foreach (var ep in listEP)
                {
                    if (ep.Value == "SonarCloudServiceEP")
                    {
                        vstsIntegrationContext.SonarEndPointId = ep.Key;
                    }
                }
            }
            log.LogInformation($"============= Project Id: {challengeProjectId}");
            vstsIntegrationContext.ChallengeCreatedProjectId = challengeProjectId;

            var vstsEPtasks = new List<Task<string>>();
            // Assign the user as a contributor to the project
            vstsEPtasks.Add(context.CallActivityAsync<string>("AddUserAsProjectContributor", vstsIntegrationContext));
            //Add the Challenge code Git repository EndPoint to the project
            if (isRepositoryCreationNeeded)
            {
                vstsEPtasks.Add(context.CallActivityAsync<string>("CreateChallengeSourceGitServiceEndpoint", vstsIntegrationContext));
            }

            //Add the SonarCloud Service EndPoint to the project
            if (string.IsNullOrEmpty(vstsIntegrationContext.SonarEndPointId))
            {
                vstsEPtasks.Add(context.CallActivityAsync<string>("CreateSonarServiceEndpoint", vstsIntegrationContext));
            }

            // Wait for parallel executions results
            await Task.WhenAll(vstsEPtasks);
            if (string.IsNullOrEmpty(vstsIntegrationContext.SonarEndPointId))
            {
                vstsIntegrationContext.SonarEndPointId = vstsEPtasks[2].Result;
            }
            if (isRepositoryCreationNeeded)
            {
                vstsIntegrationContext.ChallengeSourceGitEndPointId = vstsEPtasks[1].Result;
                // Import challenge Git Repository
                log.LogInformation("Start importing code repository");
                var vstsGitRepoImportOperationUrl = await context.CallActivityAsync<string>("ImportChallengeSourceGitRepository", vstsIntegrationContext);
                var vstsGitRepoImportOperationCtx = new VSTSLongrunningOperationContext
                {
                    OperationFriendlyName = "VSTSGitRepoImportOperation",
                    Url = vstsGitRepoImportOperationUrl,
                    VstsInstance = vstsIntegrationContext.VstsInstance,
                    VstsPAT = vstsIntegrationContext.VstsPAT,
                    CheckIntervalInSeconds = 10,
                    MaxRetryCount = 10
                };
                var vstsGitRepoImportOperationTask = context.CallSubOrchestratorAsync("GetVSTSOperationStatusOrchestration", Guid.NewGuid().ToString(), vstsGitRepoImportOperationCtx);
                await Task.WhenAll(vstsGitRepoImportOperationTask);
            }

            if (!context.IsReplaying)
                log.LogInformation("======================== FINISH =============================");
            var output = 
                new
                {
                    vstsInstance = vstsIntegrationContext.VstsInstance,
                    vstsProjectName = vstsIntegrationContext.VstsInstance,
                    sonarEndPointId = vstsIntegrationContext.SonarEndPointId,
                    codeSourceGitUrl = $"https://{vstsIntegrationContext.VstsInstance}.visualstudio.com/{vstsIntegrationContext.ChallengeProjectName}/_git/{vstsIntegrationContext.ChallengeProjectName}"
                };
            return output;
        }

        [FunctionName("GetVSTSOperationStatusOrchestration")]
        public static async Task GetVSTSOperationStatusOrchestration([OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            var vstsLongrunningOperationContext = context.GetInput<VSTSLongrunningOperationContext>();
            var status = "inprogress";
            var retryCount = 0;
            // Loop infinitly if the MaxRetryCount equals 0
            var maxRetryCount = vstsLongrunningOperationContext.MaxRetryCount > 0 ? vstsLongrunningOperationContext.MaxRetryCount : int.MaxValue;
            while (retryCount <= vstsLongrunningOperationContext.MaxRetryCount && status.ToLower() == "inprogress")
            {
                DateTime nextCheck = context.CurrentUtcDateTime.AddSeconds(vstsLongrunningOperationContext.CheckIntervalInSeconds);
                await context.CreateTimer(nextCheck, CancellationToken.None);
                status =
                    await context.CallActivityAsync<string>("GetVSTSOperationStatus", vstsLongrunningOperationContext);
            }

            if (retryCount > vstsLongrunningOperationContext.MaxRetryCount)
            {
                throw new Exception($"VSTS operation {vstsLongrunningOperationContext.OperationFriendlyName} take too much time to finish. url [{vstsLongrunningOperationContext.Url}]");
            }
            if (status.ToLower() == "failed")
            {
                throw new Exception($"VSTS operation {vstsLongrunningOperationContext.OperationFriendlyName} failed. url [{vstsLongrunningOperationContext.Url}]");
            }
            if (!context.IsReplaying)
                log.LogInformation($"VSTs operation {vstsLongrunningOperationContext.OperationFriendlyName} finished after {retryCount} retries each {vstsLongrunningOperationContext.CheckIntervalInSeconds} seconds with status {status}");
        }

    }
}