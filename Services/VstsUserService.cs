using System;
using System.Collections.Generic;
using Skillsbundle.AzureTable.Repositories;
using VSTS.Entities;

namespace SkillsBundle.Services
{
    public class VstsUserService
    {
        private readonly VstsUserRepository vstsUserRepo;
        public VstsUserService(string connectionString)
        {
           vstsUserRepo = new VstsUserRepository(connectionString);
        }

        public void Add(VstsUserEntity vstsUserEntity)
        {
            vstsUserRepo.Add(vstsUserEntity);
        }

        public VstsUserEntity Get(string userOID)
        {          
            return  vstsUserRepo.Get(userOID);
        }
    }
}