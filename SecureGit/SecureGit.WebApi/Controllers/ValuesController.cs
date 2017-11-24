using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SecureGit.CryptoLibrary;
using SecureGit.WebApi.Logics;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        private readonly SettingOptions _settingOptions;

        public ValuesController(IOptions<SettingOptions> optionAccessor)
        {
            _settingOptions = optionAccessor.Value;
        }

        //[Authorize]
        // public IActionResult GetUserDetails(){
        //     return new ObjectResult(new {
        //         Username = User.Identity.Name
        //     });                
        // }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            // List<UserDatabase> ls = new List<UserDatabase>
            // {
            //     new UserDatabase()
            //     {
            //         user = "wisnu",
            //         pass = "wisnu",
            //         email = "wisnu@abc.com",
            //         role = UserRole.SuperAdmin
            //     },
            //     new UserDatabase()
            //     {
            //         user = "user1",
            //         pass = "user1",
            //         email = "user1@abc.com",
            //         role = UserRole.Admin
            //     },
            //     new UserDatabase()
            //     {
            //         user = "user2",
            //         pass = "user2",
            //         email = "user2@abc.com",
            //         role = UserRole.Developer
            //     },
            // };

            // string json = JsonConvert.SerializeObject(ls);

            // System.IO.File.WriteAllText("/home/wisnu/gituserdb/userdb", json);

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // [HttpGet]
        // public IActionResult GetUsers()
        // {
        //     try
        //     {
        //         // DatabaseManagement dm = new DatabaseManagement(
        //         //     _settingOptions.ConnectionString
        //         // );

        //         UserManagement userMgm = new UserManagement(
        //             _settingOptions.ConnectionString);

        //         JsonLib jsonLib = new JsonLib();

        //         // List<User> ls = userMgm.GetAllUsers();

        //         return new ObjectResult(
        //             jsonLib.Serialize<List<User>>(ls));
        //     }
        //     catch
        //     {
        //         return BadRequest();
        //     }
        // }
    }
}
