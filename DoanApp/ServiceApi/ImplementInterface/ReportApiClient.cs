using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
using DoanData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DoanApp.ServiceApi
{
    public class ReportApiClient : IReportApiClient
    {
        private HttpClient client = HttpClientSigleton.Instance.Client();
        /* private readonly HttpClientHandler _handler;
         private readonly HttpClient client;
         public ReportApiClient()
         {
             _handler = new HttpClientHandler
             {
                 ClientCertificateOptions = ClientCertificateOption.Manual,
                 ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
             };
             client = new HttpClient(_handler);
             client.BaseAddress = new Uri(LinkServerApi.link);
         }*/
        public async Task<int> Delete(string token, int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await client.DeleteAsync($"/api/reports/?id={id}");
            if (result.IsSuccessStatusCode)
            {
                return 1;
            }
            return -1;
        }

        public async Task<List<ReportVideo_Vm>> GetAll(string token, string nameSearch = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await client.GetAsync($"/api/reports/?nameSearch={nameSearch}");
            if (result.IsSuccessStatusCode)
            {
                var list = JsonConvert.DeserializeObject<List<ReportVideo_Vm>>(await result.Content.ReadAsStringAsync());
                return list;
            }
            return null;
        }
    }
}
