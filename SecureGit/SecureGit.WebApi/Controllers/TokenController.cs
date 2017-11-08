using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureGit.WebApi.Models;

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

        // [HttpPost]
        // public IActionResult Create(
        //     [FromBody]string username,
        //     [FromBody]string password)
        // {
        //     if (IsValidLoginData(username, password))
        //         return new ObjectResult(GenerateToken(username));

        //     return BadRequest();
        // }

        [HttpPost]
        public IActionResult Create(
            [FromBody]LoginCredential loginCredential
        )
        {
            if (IsValidLoginData(
                loginCredential.Username, 
                loginCredential.Password))
                return new ObjectResult(
                    GenerateToken(loginCredential.Username));

            return BadRequest();
        }

        private bool IsValidLoginData(
            string username,
            string password)
        {
            return !String.IsNullOrEmpty(username) && username == password;
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