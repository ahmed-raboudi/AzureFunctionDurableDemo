using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class ChallengeByTechnologyEntity : TableEntity
    {
        public ChallengeByTechnologyEntity(string challengeName, string techName, string challengeDescription,string challengePrerequisites)
        {
            this.PartitionKey = "Challenge";
            this.RowKey = $"CT_{challengeName}_{techName}";
            this.ChallengeName = challengeName;
            this.ChallengeDescription = challengeDescription;
            this.ChallengePrerequisites = challengePrerequisites;
        }
        public ChallengeByTechnologyEntity() { }
        public string ChallengeName { get; set; }
        public string ChallengeDescription { get; set; }
        public string ChallengePrerequisites { get; set; }
    }
}