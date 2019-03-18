using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class ChallengesEntity : TableEntity
    {
        public ChallengesEntity(string challengeName, string challengeDescription, string challengePrerequisites
                                , string challengeCodeSourceGitUrl, string vstsProjectName, string vstsProjectDescription)
        {
            this.PartitionKey = "Challenge";
            this.RowKey = $"C_{challengeName}";
            this.ChallengeName = challengeName;
            this.ChallengeDescription = challengeDescription;
            this.ChallengePrerequisites = challengePrerequisites;
            this.ChallengeCodeSourceGitUrl = challengeCodeSourceGitUrl;
            this.VstsProjectName = vstsProjectName;
            this.VstsProjectDescription = vstsProjectDescription;
        }
        public ChallengesEntity() { }
        public string ChallengeName { get; set; }
        public string ChallengeShortDescription { get; set; }
        public string ChallengeDescription { get; set; }
        public string ChallengeImage { get; set; }
        public string ChallengePrerequisites { get; set; }
        public string ChallengeCodeSourceGitUrl { get; set; }
        public string VstsProjectName { get; set; }
        public string VstsProjectDescription { get; set; }
        public DateTime CreationDate { get; set; }
    }
}