using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VSTS.Helpers
{
    public class VSTSHelpers
    {
        public static async Task<HttpResponseMessage> CallVSTSAPI(string accountName, string pat, string url, 
                            string httpMethod, string jsonContent,string contentType = "application/json")
        {
            string Url = string.Format(url, accountName);
            var httpClient = HttpClientPool.GetInstance(new Uri(url), pat);
            var method = new HttpMethod(httpMethod);
            var request = new HttpRequestMessage(method, Url);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                request.Content = new StringContent(jsonContent, Encoding.UTF8, contentType);
            }
            return await httpClient.SendAsync(request);
        }

         public static async Task<HttpResponseMessage> CallRestAPI(string token, string url, 
                            string httpMethod, string jsonContent,ILogger log,string contentType = "application/json")
        {
            var httpClient = HttpClientPool.GetInstance(new Uri(url), token);            
            var method = new HttpMethod(httpMethod);
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                request.Content = new StringContent(jsonContent, Encoding.UTF8, contentType);                
            }
            return await httpClient.SendAsync(request);
        }
    }
}