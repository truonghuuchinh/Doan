using DoanApp.Areas.Administration.Models;
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
    public class UserApiClient : IUserApiCient
    {
        private HttpClient client = HttpClientSigleton.Instance.Client();
       /* private readonly HttpClientHandler _handler;
        private readonly HttpClient client;
        public UserApiClient()
        {
            _handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            client = new HttpClient(_handler);
            client.BaseAddress = new Uri(LinkServerApi.link);
        }*/
        public async Task<string> Authenticated(LoginRequest request)
        {
          
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
           var respones=await client.PostAsync("/api/users/authenticated",httpContent);
            var token = await respones.Content.ReadAsStringAsync();
            return token;
        }

        public async Task<string> CheckToken(string token, string email)
        {
            if (token == null && email == null) return null;
            if(token==null)
            return await RefreshGetToken(new LoginRequest() { Email = email });
            return null;
        }

        public async Task<int> Delete(string token, int id)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            var result = await client.DeleteAsync($"/api/users/?id={id}");
            if (result.IsSuccessStatusCode)
                return 1;
            return -1;
        }

        public async Task<List<AppUser>> GetAll(string token, string nameSearch)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            var result = await client.GetAsync($"/api/users/?nameSearch={nameSearch}");
            if (result.IsSuccessStatusCode)
            {
                var list = JsonConvert.DeserializeObject<List<AppUser>>( await result.Content.ReadAsStringAsync());
                return list;
            }
            return null;
        }

        public async Task<List<UserAdmin>> GetAllUserAdmin(string token, string emailUser,string nameSearch=null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            var result = await client.GetAsync($"/api/users/UserAdmin/?email={emailUser}&nameSearch={nameSearch}");
            if (result.IsSuccessStatusCode)
            {
                var list = JsonConvert.DeserializeObject<List<UserAdmin>>( await result.Content.ReadAsStringAsync());
                return list;
            }
            return null;
        }

        public async Task<string> RefreshGetToken(LoginRequest request)
        {
            if (request.Email != null)
            {
                return await Authenticated(request);
            }
            return null;
        }
    }
}
