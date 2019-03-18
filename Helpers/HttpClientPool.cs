using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VSTS.Helpers
{  
    public static class HttpClientPool
    {
        private static Dictionary<string, HttpClient> httpClientPool  = new Dictionary<string, HttpClient>();
        public static HttpClient GetInstance(Uri url, string pat)
        {
            lock (httpClientPool)
            {
                if (httpClientPool.ContainsKey(url.Host))
                {
                    return httpClientPool[url.Host];
                }
                else
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                            ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", pat))));
                    httpClient.BaseAddress = url;
                    httpClientPool.Add(url.Host, httpClient);
                    return httpClient;
                }
            }
        }
    }
}