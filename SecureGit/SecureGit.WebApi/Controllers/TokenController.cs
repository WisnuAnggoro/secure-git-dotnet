using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureGit.CryptoLibrary;
using SecureGit.CryptoLibrary.Models;
using SecureGit.CryptoLibrary.Extensions;
using SecureGit.WebApi.Logics;
using SecureGit.WebApi.Models;
using System.Security;

namespace SecureGit.WebApi
{
    [Route("v1/login")]
    public class TokenController : Controller
    {
        private readonly SettingOptions _settingOptions;

        public TokenController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;
        }

        [HttpPost]
        public IActionResult Create(
            [FromBody]string PacketString)
        {
            try
            {
                // Get PacketLib and JsonLib instance
                PacketLib packetLib = new PacketLib();
                JsonLib jsonLib = new JsonLib();

                Packet packet = jsonLib.Deserialize<Packet>(PacketString);

                // Decrypt packet to get the payload
                SecureString payload = packetLib.UnwrapPacket(
                    packet,
                    System.IO.File.ReadAllText(
                        Path.Combine(
                            _settingOptions.RsaKeyDirectory,
                            _settingOptions.RsaPrivateKeyName))
                        .ToSecureString());

                // Deserialize payload
                LoginCredential loginCredential = jsonLib.Deserialize<LoginCredential>(
                    payload.ToPlainString());

                // Check Login Credential validity
                // then generate token if valid
                if (IsValidLoginData(
                    loginCredential.Username,
                    loginCredential.Password,
                    loginCredential.RsaPublicKey))
                    return new ObjectResult(
                        GenerateToken(loginCredential.Username));
                else
                    return Unauthorized();
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                return BadRequest();
            }
        }


        private bool IsValidLoginData(
            string username,
            string password,
            string key)
        {
            if (String.IsNullOrEmpty(username) ||
                String.IsNullOrEmpty(password) ||
                key == null)
                return false;

            UserManagement um = new UserManagement(
                Path.Combine(
                    _settingOptions.UserDBPath,
                    _settingOptions.UserDBFileName));

            return um.IsCredentialValid(
                username,
                password);
        }

        private string GenerateToken(
            string username)
        {
            var claims = new Claim[]
            {
                new Claim(
                    ClaimTypes.Name,
                    username),
                // new Claim(
                //     JwtRegisteredClaimNames.Nbf, 
                //     new DateTimeOffset(DateTime.Now)
                //         .ToUnixTimeSeconds()
                //         .ToString()),
                // new Claim(
                //     JwtRegisteredClaimNames.Exp, 
                //     new DateTimeOffset(DateTime.Now.AddDays(1))
                //         .ToUnixTimeSeconds()
                //         .ToString())
            };

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                _settingOptions.JwtPrivateKey)),
                        SecurityAlgorithms.HmacSha256)
                ),
                // new JwtPayload(claims)
                new JwtPayload(
                    issuer: _settingOptions.JwtIssuer,
                    audience: _settingOptions.JwtAudience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddMinutes(15)
                )
            );

            // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a secret that needs to be at least 16 characters long"));

            // var token = new JwtSecurityToken(
            //     issuer: "your app",
            //     audience: "the client of your app",
            //     claims: claims,
            //     notBefore: DateTime.Now,
            //     expires: DateTime.Now.AddDays(28),
            //     signingCredentials: new SigningCredentials(
            //         secretKey, 
            //         SecurityAlgorithms.HmacSha256)
            // );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}