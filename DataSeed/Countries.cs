using System;
using System.Collections.Generic;
using VSTS.Entities;

namespace Skillsbundle.Data
{
    public static class InitData
    {
        public static List<CountryEntity> GetCountriesData()
        {
            return new List<CountryEntity>(){

                new CountryEntity("France","FR","westeurope"),
             
           };
        }
    }
}