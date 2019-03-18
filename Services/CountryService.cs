using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skillsbundle.AzureTable.Repositories;
using VSTS.Entities;

namespace SkillsBundle.Services
{
    public class CountryService
    {
        private readonly CountryRepository countryRepo;
        public CountryService(string connectionString)
        {
           countryRepo = new CountryRepository(connectionString);
        }

        public async Task<CountryEntity> Get(string countryCode)
        {          
            return await countryRepo.Get(countryCode);
        }
    }
}