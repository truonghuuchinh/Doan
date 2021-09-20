using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class HttpClientSigleton
    {
        private static HttpClientSigleton instance;
        static object key = new object();
        private HttpClientSigleton()
        {

        }
        public static HttpClientSigleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new HttpClientSigleton();
                    }
                }
                return instance;
            }
        }
        public  HttpClient Client()
        {
            HttpClientHandler _handler;
            HttpClient _client;
            _handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            _client = new HttpClient(_handler);
            _client.BaseAddress = new Uri(LinkServerApi.link);
            return _client;
        }

    }
}
