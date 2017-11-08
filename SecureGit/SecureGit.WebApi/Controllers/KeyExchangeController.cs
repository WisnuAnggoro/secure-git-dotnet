using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecureGit.RsaLibrary;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi
{
    [Route("v1/connect")]
    public class KeyExchangeController : Controller
    {
        private readonly SettingOptions _settingOptions;

        public KeyExchangeController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;
        }

        [HttpGet]
        public IActionResult GenerateKey()
        {
            RsaLib rsa = new RsaLib();

            // rsa.Main();
            if (rsa.GenerateKeyPairs(
                _settingOptions.RsaKeyDirectory,
                _settingOptions.RsaPrivateKeyName,
                _settingOptions.RsaPublicKeyName))
                return new ObjectResult(
                    System.IO.File.ReadAllText(
                        Path.Combine(
                            _settingOptions.RsaKeyDirectory,
                            _settingOptions.RsaPublicKeyName)));
            else
                return NoContent();
        }

        // [HttpPost]
        // public IActionResult GenerateKey(
        //     [FromBody] object JsonClientPubKey)
        // {
        //     RsaLib rsa = new RsaLib();

        //     // string aa = JsonClientPubKey;
        //     if (JsonClientPubKey == null)
        //         return BadRequest();

        //     // rsa.Main();
        //     if (rsa.GenerateKeyPairs(
        //         _settingOptions.RsaKeyDirectory,
        //         _settingOptions.RsaPrivateKeyName,
        //         _settingOptions.RsaPublicKeyName))
        //         return new ObjectResult(
        //             System.IO.File.ReadAllText(
        //                 Path.Combine(
        //                     _settingOptions.RsaKeyDirectory,
        //                     _settingOptions.RsaPublicKeyName)));
        //     else
        //         return NoContent();
        // }
    }
}