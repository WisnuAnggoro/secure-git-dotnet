using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using SecureGit.ConsoleApp.Logics;

namespace SecureGit.ConsoleApp
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static GitHttpClient _client = new GitHttpClient();

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        // static async Task<string> GetProductAsync(string path)
        // {
        //     string product = null;
        //     HttpResponseMessage response = await client.GetAsync(path);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         product = await response.Content.ReadAsAsync<string>();
        //     }
        //     return product;
        // }

        static async Task RunAsync()
        {
            // // New code:
            // client.BaseAddress = new Uri("http://localhost:5000/");
            // client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(
            //     new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // // Create a new product
                // Product product = new Product { Name = "Gizmo", Price = 100, Category = "Widgets" };

                // var url = await CreateProductAsync(product);
                // Console.WriteLine($"Created at {url}");

                // // Get the product
                // product = await GetProductAsync(url.PathAndQuery);
                // ShowProduct(product);

                // // Update the product
                // Console.WriteLine("Updating price...");
                // product.Price = 80;
                // await UpdateProductAsync(product);

                // // Get the updated product
                // product = await GetProductAsync(url.PathAndQuery);
                // ShowProduct(product);

                // // Delete the product
                // var statusCode = await DeleteProductAsync(product.Id);
                // Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

                // await GetProductAsync("http://localhost:5000/api/values");

                // Console.WriteLine(await _client.Get("api/values"));

                // _client.Connect("{\"user\":\"name\"}");

                // Create RSA Key
                // string rsaDir = "/home/wisnu/gitkeyclient";
                // string rsaPri = "prikey";
                // string rsaPub = "pubkey";
                // RsaLib rsa = new RsaLib();
                // bool bo = rsa.GenerateKeyPairs(
                //     rsaDir,
                //     rsaPri,
                //     rsaPub);

                // if (!bo)
                // {
                //     Console.WriteLine("Cannot generate RSA key pairs.");
                //     return;
                // }

                // Connect to server
                bool bo = _client.RequestKey();

                if (!bo)
                {
                    Console.WriteLine("Cannot connect to server.");
                    Console.WriteLine(_client.LastErrorMessage);
                    return;
                }

                // bo = await _client.Post(
                //     "login",
                //     "{\"Header\":\"heads\",\"Payload\":\"payloads\"}"
                // );

                // Login
                SecureString passw = new SecureString();
                passw.AppendChar('u');
                passw.AppendChar('s');
                passw.AppendChar('e');
                passw.AppendChar('r');
                bo = _client.Login(
                    "wisnu",
                    passw);

                if (!bo)
                {
                    Console.WriteLine("Cannot login to server.");
                    Console.WriteLine(_client.LastErrorMessage);
                    return;
                }

                Console.WriteLine("");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


    }
}
