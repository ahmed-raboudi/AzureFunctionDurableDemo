using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skillsbundle.AzureTable.Repositories;
using VSTS.Entities;

namespace SkillsBundle.Services
{
    public class VstsInstanceService
    {
        private readonly VstsInstanceRepository vstsInstanceRepo;
        public VstsInstanceService(string connectionString)
        {
            vstsInstanceRepo = new VstsInstanceRepository(connectionString);
        }

        public async Task Add(VstsInstanceEntity vstsInstanceEntity)
        {
            await vstsInstanceRepo.Add(vstsInstanceEntity);
        }

        public async Task<VstsInstanceEntity> Get(string instanceName)
        {
            return await vstsInstanceRepo.Get(instanceName);
        }

        public async Task<VstsInstanceEntity> GetAvailableInstance(string azureLocation)
        {
            return await vstsInstanceRepo.GetAvailableInstance(azureLocation);
        }
    }
}