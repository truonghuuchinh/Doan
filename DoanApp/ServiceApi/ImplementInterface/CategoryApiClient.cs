using DoanApp.Commons;
using DoanApp.Models;
using DoanData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoanApp.ServiceApi
{
    public class CategoryApiClient : ICategoryApiClient
    {

        private HttpClient _client=HttpClientSigleton.Instance.Client();
       /* private readonly HttpClientHandler _handler;
        private readonly HttpClient _client;*/
       /* public CategoryApiClient()
        {
           *//* _handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            _client = new HttpClient(_handler);
            _client.BaseAddress = new Uri(LinkServerApi.link);*//*
        }*/

        public async Task<List<Category>> GetAll( string token,string nameSearch=null)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respone = await _client.GetAsync("/api/categorys/?nameSearch=" + nameSearch);
            var stringRespone = await respone.Content.ReadAsStringAsync();
            var listCategory = JsonConvert.DeserializeObject<List<Category>>(stringRespone);
            return listCategory;
        }

        public async Task<int> Create(string token, CategoryRequest request)
        {
            var JsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(JsonContent, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            var respone = await _client.PostAsync("/api/categorys", httpContent);
            var result = JsonConvert.DeserializeObject<Category>( await respone.Content.ReadAsStringAsync());
            if (result != null)
            {
                return 1;
            }
            return -1;
        }

        public async Task<int> Update(string token, CategoryRequest request)
        {
            var JsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(JsonContent, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respone = await _client.PutAsync("/api/categorys", httpContent);
            var result =int.Parse( await respone.Content.ReadAsStringAsync());
            if (result > 0) return 1;
            return -1;
        }

        public async Task<int> Delete(string token,int id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var respone = await _client.DeleteAsync($"/api/categorys/?categoryid={id}");
            var result = int.Parse(await respone.Content.ReadAsStringAsync());
            if (result > 0) return 1;
            return -1;
        }
    }
}
