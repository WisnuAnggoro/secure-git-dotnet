using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecureGit.ConsoleApp.Helpers;
using SecureGit.ConsoleApp.Models;
using SecureGit.RsaLibrary;

namespace SecureGit.ConsoleApp.Logics
{
    public class GitHttpClient
    {
        private HttpClient _client;

        public string ServerPublicKey { get; private set; }

        public string LastResponseMessage { get; private set; }

        public GitHttpClient() : this("localhost", 5000)
        {
        }

        public GitHttpClient(
            string Host,
            Int32 Port) : this(Host, Port, "v1")
        {
        }

        public GitHttpClient(
            string Host,
            Int32 Port,
            string ApiVersion)
        {
            UriBuilder address = new UriBuilder("http", Host, Port, ApiVersion);

            _client = new HttpClient();
            _client.BaseAddress = address.Uri;

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"));
            
        }

        public bool Connect()
        {
            Task<string> res = Get(
                "connect");

            if(res.Result == "")
                return false;
            
            ServerPublicKey = res.Result;
            return true;
        }

        public string Login(
            string username,
            string password,
            string rsaPublicKey
        )
        {
            // Creating login credential
            LoginCredential logCredential = new LoginCredential()
            {
                Username = username,
                Password = password,
                RsaPublicKey = rsaPublicKey
            };

            // Serializing to JSON
            string logCredString = JsonConvert.SerializeObject(
                logCredential, Formatting.Indented);

            // Encrypting the json using server public key
            RsaLib rsa = new RsaLib();

            Task<bool> s = Post(
                "login",
                rsa.Encrypt(
                    logCredString,
                    ServerPublicKey)
            );

            return "";
        }

        public async Task<string> Get(
            string QueryPath)
        {
            object objContent = null;
            string contentString = "";

            HttpResponseMessage response = 
                await _client.GetAsync(
                    $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}");

            if(response.IsSuccessStatusCode)
            {
                objContent = await response
                    .Content
                    .ReadAsAsync<object>();

                string json = JsonConvert.SerializeObject(objContent);
            }

            if(objContent != null)
            {
                // Check if it's JSON
                // bool bo = DataHelper.GetJsonStringFromObject(
                //     objContent,
                //     out contentString);
                DataHelper.GetJsonStringFromObject(
                    objContent,
                    out contentString);
            }

            return contentString;
        }

        public async Task<bool> Post(
            string QueryPath,
            string JsonStringResource)
        {
            HttpResponseMessage response = 
                await _client.PostAsJsonAsync(
                    $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}",
                    JsonStringResource);

            if(!response.IsSuccessStatusCode)
            {
                LastResponseMessage = "";
                return false;
            }

            // return URI of the created resource.
            LastResponseMessage = await response.Content.ReadAsStringAsync();
            return true;
        }


    }
}