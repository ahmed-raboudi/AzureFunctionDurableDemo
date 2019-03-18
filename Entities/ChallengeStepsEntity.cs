using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class ChallengeStepsEntity : TableEntity
    {
        public ChallengeStepsEntity(string challengeName, string stepName, string description, int estimatedDuration, string requirements)
        {
            this.PartitionKey = "Challenge";
            this.RowKey = $"CS_{challengeName}_{stepName}";
            this.Name = stepName;
            this.Description = description;
            this.EstimatedDuration = estimatedDuration;
            this.Requirements = requirements;
        }
        public ChallengeStepsEntity() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EstimatedDuration { get; set; }
        public string Requirements { get; set; }
    }
}