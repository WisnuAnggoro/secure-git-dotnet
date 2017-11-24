using System;
using System.IO;
using System.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecureGit.CryptoLibrary;
using SecureGit.CryptoLibrary.Extensions;
using SecureGit.CryptoLibrary.Models;
using SecureGit.WebApi.Logics;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi.Controllers
{
    [Authorize]
    [Route("v1/[controller]")]
    public class UserController : Controller
    {
        private readonly SettingOptions _settingOptions;
        private readonly UserManagement _userManagement;

        public UserController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;
            
            _userManagement = new UserManagement(
                _settingOptions.ConnectionString);
        }

        [Route("password/change")]
        [HttpPost]
        public IActionResult ResetPassword(
            [FromBody]Packet packet)
        {
            try
            {
                // Get PacketLib and JsonLib instance
                PacketLib packetLib = new PacketLib();
                JsonLib jsonLib = new JsonLib();

                // Decrypt packet to get the payload
                SecureString payload = packetLib.UnwrapPacket(
                    packet,
                    System.IO.File.ReadAllText(
                        Path.Combine(
                            _settingOptions.RsaKeyDirectory,
                            _settingOptions.RsaPrivateKeyName))
                        .ToSecureString());

                // Deserialize payload
                LoginCredentialReset loginCredentialReset = 
                    jsonLib.Deserialize<LoginCredentialReset>(
                    payload.ToPlainString());

                // Reset Password
                if (!_userManagement.ResetPassword(
                    loginCredentialReset.Username,
                    loginCredentialReset.OldPassword,
                    loginCredentialReset.NewPassword))
                    return Unauthorized();

                // // Check the RSA public key validity
                // if(!packetLib.IsValidRsaPublicKey(
                //     loginCredential.RsaPublicKey))
                //     return BadRequest();

                // // Check Login Credential validity
                // // then generate token if valid
                // if (IsValidLoginData(
                //     loginCredential.Username,
                //     loginCredential.Password))
                // {
                //     // Save user's public key
                //     using (UserManagement um = new UserManagement(
                //         _settingOptions.ConnectionString))
                //         um.UpdateUserPublicKey(
                //             loginCredential.Username,
                //             loginCredential.RsaPublicKey);

                //     // Return JWT Token
                //     return Content(
                //         GenerateToken(loginCredential.Username));
                // }
                // else
                //     return Unauthorized();

                return Ok();
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                return BadRequest();
            }
        }
    }
}