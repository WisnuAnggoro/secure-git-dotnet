using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    public class GitController : Controller
    {
        private readonly SettingOptions _settingOptions;
        public GitController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;
        }

        public IActionResult Index()
        {
            var option1 = _settingOptions.UserDBPath;
            var option2 = _settingOptions.JwtPrivateKey;
            return Content($"option1 = {option1}, option2 = {option2}");
        }
    }
}