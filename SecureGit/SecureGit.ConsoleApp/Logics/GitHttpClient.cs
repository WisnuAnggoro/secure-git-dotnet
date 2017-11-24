using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using SecureGit.ConsoleApp.Helpers;
using SecureGit.CryptoLibrary;
using SecureGit.CryptoLibrary.Extensions;
using SecureGit.CryptoLibrary.Models;
using SecureGit.RsaLibrary;
using SecureGit.RsaLibrary.Models;

namespace SecureGit.ConsoleApp.Logics
{
    public class GitHttpClient
    {
        // private HttpClient _client;
        // private RestClient _restClient;
        private Uri _baseAddress;
        private RsaLib _rsaLib;
        private AesLib _aesLib;
        private RngLib _rngLib;
        private JsonLib _jsonLib;
        private SecureString _privateKey;
        private string _publicKey;
        private string _apiPublicKey;
        private SecureString _jwtToken;

        // public string ServerPublicKey { get; private set; }

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
            _baseAddress = address.Uri;
            // _restClient = new RestClient(address.Uri);
            // _client.BaseAddress = address.Uri;

            // _client.DefaultRequestHeaders.Accept.Clear();
            // _client.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue(
            //         "application/json"));

            _rsaLib = new RsaLib();
            _rsaLib.GenerateKeyPairs(
                out _privateKey,
                out _publicKey);

            _aesLib = new AesLib();
            _rngLib = new RngLib();
            _jsonLib = new JsonLib();
        }

        // RequestKey() retrieves API public key
        // Contact server administrator if it return false
        public bool RequestKey()
        {
            try
            {
                // Task<string> tRes = Get(
                //     "request/key");

                // if (String.IsNullOrEmpty(tRes.Result))
                //     return false;

                // // ServerPublicKey = res.Result;

                // _apiPublicKey = tRes.Result;

                string s = Get("request/key");

                if (String.IsNullOrEmpty(s))
                    return false;

                _apiPublicKey = s;

                return true;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }
        }

        // Login() creates JWT in string
        public bool Login(
            string username,
            SecureString password)
        {
            // string ss = password.ToPlainString();

            // Creating login credential
            LoginCredential logCredential = new LoginCredential()
            {
                Username = username,
                Password = password.ToPlainString(),
                RsaPublicKey = _publicKey
            };

            // Serializing to JSON
            SecureString payload =
                _jsonLib.Serialize<LoginCredential>(
                    logCredential)
                .ToSecureString();

            // // Encrypt payload
            // SecureString aesKey = _rngLib.GenerateRandomSecureString(
            //     _aesLib.KeySize);
            // string encryptedPayload = _aesLib.Encrypt(
            //     payload,
            //     aesKey);

            // // Encrypt AES Key
            // string encryptedAesKey = _rsaLib.Encrypt(
            //     aesKey,
            //     _apiPublicKey);

            // // Wrap Packet
            // Packet packet = new Packet()
            // {
            //     Header = encryptedAesKey,
            //     Payload = encryptedPayload
            // };

            PacketLib packetLib = new PacketLib();
            Packet packet = packetLib.WrapPacket(
                payload,
                _apiPublicKey);

            string json = _jsonLib.Serialize<Packet>(
                packet);

            // Encrypting the logCredential using server public key
            // before sending to server
            // Task<bool> t = Post(
            //     "login",
            //     json);

            // if(t.Result)
            // {
            //     _jwtToken = LastResponseMessage.ToSecureString();
            //     return true;
            // }
            // else
            //     return false;

            string token = Post(
                "login",
                packet);

            if(!String.IsNullOrEmpty(token))
            {
                _jwtToken = token.ToSecureString();
                return true;
            }
            else
                return false;
        }

        public bool ChangePassword(
            string username,
            SecureString oldPassword,
            SecureString newPassword)
        {
            // string ss = password.ToPlainString();

            // Creating login credential
            LoginCredentialReset logCredentialReset = new LoginCredentialReset()
            {
                Username = username,
                OldPassword = oldPassword.ToPlainString(),
                NewPassword = newPassword.ToPlainString()
            };

            // Serializing to JSON
            SecureString payload =
                _jsonLib.Serialize<LoginCredentialReset>(
                    logCredentialReset)
                .ToSecureString();

            PacketLib packetLib = new PacketLib();
            Packet packet = packetLib.WrapPacket(
                payload,
                _apiPublicKey);

            string json = _jsonLib.Serialize<Packet>(
                packet);

            string token = Post(
                "user/password/change",
                packet);

            if(!String.IsNullOrEmpty(token))
            {
                _jwtToken = token.ToSecureString();
                return true;
            }
            else
                return false;
        }

        public string GetProjectList()
        {
            // Encrypting the logCredential using server public key
            // before sending to server
            // Task<string> t = Get(
            //     "git/projects",
            //     _jwtToken.ToPlainString());

            string s = Get(
                "git/projects",
                _jwtToken.ToPlainString());

            if (!String.IsNullOrWhiteSpace(s))
                return s;
            else
                return null;
        }

        // public async Task<string> Get(
        //     string QueryPath,
        //     string JwtToken = null)
        // {
        //     // object objContent = null;
        //     string contentString = null;

        //     if (!String.IsNullOrWhiteSpace(JwtToken))
        //         _client.DefaultRequestHeaders.Authorization =
        //             new AuthenticationHeaderValue("Bearer", JwtToken);

        //     HttpResponseMessage response =
        //         await _client.GetAsync(
        //             $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}");

        //     if (response.IsSuccessStatusCode)
        //     {
        //         contentString = await response
        //             .Content
        //             .ReadAsAsync<string>();

        //         // string json = JsonConvert.SerializeObject(objContent);
        //     }

        //     // if(contentString != null)
        //     // {
        //     //     // Check if it's JSON
        //     //     // bool bo = DataHelper.GetJsonStringFromObject(
        //     //     //     objContent,
        //     //     //     out contentString);
        //     //     // DataHelper.GetJsonStringFromObject(
        //     //     //     objContent,
        //     //     //     out contentString);
        //     //     contentString = (string)objContent;
        //     // }

        //     return contentString;
        // }

        // public async Task<bool> Post(
        //     string QueryPath,
        //     string JsonStringResource,
        //     string JwtToken = null)
        // {
        //     if (!String.IsNullOrWhiteSpace(JwtToken))
        //         _client.DefaultRequestHeaders.Authorization =
        //             new AuthenticationHeaderValue("Bearer", JwtToken);

        //     // // string s = _client.DefaultRequestHeaders.Authorization.Scheme;

        //     // _client.DefaultRequestHeaders.Accept.Clear();
        //     // _client.DefaultRequestHeaders.Accept.Add(
        //     //     new MediaTypeWithQualityHeaderValue(
        //     //         "application/json"));

        //     HttpResponseMessage response =
        //         await _client.PostAsJsonAsync(
        //             $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}",
        //             JsonStringResource);

        //     // var content = new FormUrlEncodedContent(new[]
        //     // {
        //     //     new KeyValuePair<string, string>(
        //     //         "packet", 
        //     //         JsonStringResource)
        //     // });

        //     // HttpResponseMessage response =
        //     //     await _client.PostAsync(
        //     //         $"{_client.BaseAddress.ToString()}/{QueryPath.TrimStart('/')}",
        //     //         content);           

        //     if (!response.IsSuccessStatusCode)
        //     {
        //         LastResponseMessage = "";
        //         return false;
        //     }

        //     // return URI of the created resource.
        //     LastResponseMessage = await response.Content.ReadAsStringAsync();
        //     return true;
        // }

        public string Get(
            string QueryPath,
            string JwtToken = null)
        {
            try
            {
                RestClient client = new RestClient(_baseAddress);

                RestRequest request = new RestRequest(
                    QueryPath,
                    Method.GET);

                // request.AddHeader("Accept", "application/json");
                // request.Parameters.Clear();
                // request.AddParameter(
                //     "application/json", 
                //     strJSONContent, 
                //     ParameterType.RequestBody);
                // request.AddHeader("Content-Type", "application/json; charset=utf-8");

                if (!String.IsNullOrWhiteSpace(JwtToken))
                {
                    // string s = $"Bearer {JwtToken}";
                    request.AddHeader(
                        "Authorization",
                        // s);
                        $"Bearer {JwtToken}");
                        // "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoid2lzbnUiLCJuYmYiOjE1MTEyNTE2MjcsImV4cCI6MTUxMTI1MjUyNywiaXNzIjoidGhlIGlzc3VlciIsImF1ZCI6InRoZSBhdWRpZW5jZSJ9.XJdDR6eWswQkJnnxVKf2YhvJAHjaqumfhf-iDNhZ4p4");
                    client.Authenticator = new JwtAuthenticator(JwtToken);
                    // client.Authenticator.Authenticate(client, request);
                }

                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch
            {
                return null;
            }
        }

        public string Post(
            string QueryPath,
            // string JsonStringResource,
            Packet packet,
            string JwtToken = null)
        {
            try
            {
                RestClient rc = new RestClient(_baseAddress);

                RestRequest request = new RestRequest(
                    QueryPath,
                    Method.POST);

                request.AddJsonBody(packet);

                if (!String.IsNullOrWhiteSpace(JwtToken))
                    request.AddHeader(
                        "Authorization",
                        $"Bearer {JwtToken}");

                IRestResponse response = rc.Execute(request);
                return response.Content;
            }
            catch
            {
                return null;
            }
        }
    }
}