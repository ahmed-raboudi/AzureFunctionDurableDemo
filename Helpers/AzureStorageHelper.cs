using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillsBundle.Helpers
{
    public static class AzureStorageHelper
    {
        public static string ConnectionString => Environment.GetEnvironmentVariable("SkillsBundleTablesConnectionsString");
    }
}