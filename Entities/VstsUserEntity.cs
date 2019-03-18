using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.Entities
{
    public class VstsUserEntity : TableEntity
    {
        public VstsUserEntity(string userOID,string vstsInstance, string vstsUserId)
        {
            this.PartitionKey = "VSTS";
            this.RowKey = $"U_{userOID}";
            this.UserOID = userOID;
            this.VstsInstance = vstsInstance;
            this.VstsUserId = vstsUserId;
        }
        public VstsUserEntity() { }
        // User assigned VSTS instance
        public string VstsInstance { get; set; }
        // VSTS internal User Id
        public string VstsUserId { get; set; }
        // Azure AD id
        public string UserOID { get; set; }
    }
}