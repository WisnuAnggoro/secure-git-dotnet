using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SecureGit.ConsoleApp.Helpers;
using SecureGit.ConsoleApp.Models;
using SecureGit.RsaLibrary;
using SecureGit.RsaLibrary.Models;

namespace SecureGit.ConsoleApp.Logics
{
    public class GitHttpClient
    {
        private HttpClient _client;
        private RsaLib _rsaLib;
        private SecureString _privateKey;
        private SecureString _publicKey;
        private SecureString _apiPublicKey;

        public string ServerPublicKey { get; private set; }

        public string LastResponseMessage { get; private set; }
        public string LastErrorMessage { get; private set; }

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

            _rsaLib = new RsaLib();
            _rsaLib.GenerateKeyPairs(
                out _privateKey,
                out _publicKey);
        }

        public bool RequestKey()
        {
            try
            {
                Task<string> tRes = Get(
                    "request/key");

                if (String.IsNullOrEmpty(tRes.Result))
                    return false;

                // ServerPublicKey = res.Result;

                _apiPublicKey = _rsaLib.ConvertToSecureString(
                    tRes.Result);

                return true;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }
        }

        public bool Login(
            string username,
            SecureString password)
        {
            // Creating login credential
            LoginCredential logCredential = new LoginCredential()
            {
                Username = username,
                Password = _rsaLib.ExtractSecureString(password),
                RsaPublicKey = JsonConvert.DeserializeObject<RsaPublicKey>(
                    _rsaLib.ExtractSecureString(
                        _publicKey))
            };

            // Serializing to JSON
            string logCredString = JsonConvert.SerializeObject(
                logCredential);

            // Encrypting the logCredential using server public key
            // before sending to server
            Task<bool> t = Post(
                "login",
                _rsaLib.Encrypt(
                    logCredString,
                    _apiPublicKey)
            );

            return t.Result;
        }

        public async Task<string> Get(
            string QueryPath)
        {
            // object objContent = null;
            string contentString = null;

            HttpResponseMessage response =
                await _client.GetAsync(
                    $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}");

            if (response.IsSuccessStatusCode)
            {
                contentString = await response
                    .Content
                    .ReadAsAsync<string>();

                // string json = JsonConvert.SerializeObject(objContent);
            }

            // if(contentString != null)
            // {
            //     // Check if it's JSON
            //     // bool bo = DataHelper.GetJsonStringFromObject(
            //     //     objContent,
            //     //     out contentString);
            //     // DataHelper.GetJsonStringFromObject(
            //     //     objContent,
            //     //     out contentString);
            //     contentString = (string)objContent;
            // }

            return contentString;
        }

        public async Task<bool> Post(
            string QueryPath,
            string JsonStringResource,
            string JwtToken = null)
        {
            if (!String.IsNullOrWhiteSpace(JwtToken))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", JwtToken);

            // string s = _client.DefaultRequestHeaders.Authorization.Scheme;

            HttpResponseMessage response =
                await _client.PostAsJsonAsync(
                    $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}",
                    JsonStringResource);

            if (!response.IsSuccessStatusCode)
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