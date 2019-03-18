using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class VstsInstanceEntity : TableEntity
    {
        public VstsInstanceEntity(string instanceName,string region)
        {
            this.PartitionKey = "VSTS";
            this.RowKey = $"Ins_{instanceName}";
            this.InstanceName = instanceName;
            this.Region = region;
        }
        public VstsInstanceEntity() { }
        public string InstanceName { get; set; }
        public string Region { get; set; }
        public int AvailableFreeBasicUserSlot { get; set; }
    }
}