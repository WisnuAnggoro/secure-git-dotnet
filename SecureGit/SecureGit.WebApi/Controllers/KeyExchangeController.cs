using Microsoft.AspNetCore.Mvc;
using SecureGit.Logics;

namespace SecureGit.WebApi
{
    [Route("v1/connect")]
    public class KeyExchange : Controller
    {
        public void GenerateKey()
        {
            RsaLib rsa = new RsaLib();

            // rsa.Main();
            rsa.GenerateKeyPairs();

        }
    }
}