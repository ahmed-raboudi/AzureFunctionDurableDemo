using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class ChallengeTechnologyEntity : TableEntity
    {
        public ChallengeTechnologyEntity(string challengeName, string techName, string techImage)
        {
            this.PartitionKey = "Challenge";
            this.RowKey = $"C_{challengeName}_{techName}";
            this.TechName = techName;
            this.TechImage = techImage;
        }
        public ChallengeTechnologyEntity() { }
        public string TechName { get; set; }
        public string TechImage { get; set; }
    }
}