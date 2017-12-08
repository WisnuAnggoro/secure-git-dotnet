using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SecureGit.CryptoLibrary;
using SecureGit.CryptoLibrary.Extensions;
using SecureGit.WebApi.Logics;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    public class GitController : Controller
    {
        private readonly SettingOptions _settingOptions;
        private readonly UserManagement _userMgm;
        private readonly GitManagement _gitMgm;
        private readonly PacketLib _packetLib;

        public GitController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;

            _userMgm = new UserManagement(_settingOptions.ConnectionString);
            // _gitMgm = new GitManagement(_settingOptions.ConnectionString);
            _packetLib = new PacketLib();
        }

        // public IActionResult Index()
        // {
        //     var option1 = _settingOptions.UserDBPath;
        //     var option2 = _settingOptions.JwtPrivateKey;
        //     return Content($"option1 = {option1}, option2 = {option2}");
        // }

        // Resource: "/git/projects"
        // To get available project list
        [HttpGet]
        public IActionResult Projects()
        {
            string username = User.Identity.Name;

            string payload = _userMgm.FetchUserAssignedProjects(
                username);

            if(String.IsNullOrEmpty(payload))
                return BadRequest();

            return new ObjectResult(
                _packetLib.WrapPacket(
                    payload.ToSecureString(),
                    _userMgm.FetchUserPublicKey(
                        username)));
        }
    }
}