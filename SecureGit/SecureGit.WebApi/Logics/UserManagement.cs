using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi.Logics
{
    public class UserManagement
    {
        private List<UserDatabase> users;
        public UserManagement(
            string UserDatabaseFilePath)
        {
            users = GetUserList(
                UserDatabaseFilePath
            );
        }
        private List<UserDatabase> GetUserList(
            string UserDatabaseFilePath)
        {
            string users = File.ReadAllText(UserDatabaseFilePath);
            return JsonConvert.DeserializeObject<List<UserDatabase>>(users);
        }

        private bool FindIfUserExist(string username)
        {
            UserDatabase ud = users.Find(d => d.user == username);
            return ud != null;
        }

        private string FindPasswordOfUser(string username)
        {
            return users.Find(d => d.user == username).pass;
        }

        public bool IsCredentialValid(
            string username,
            string password)
        {
            if (!FindIfUserExist(username))
                return false;

            string passwordInMd5;
            using (MD5 md5Hash = MD5.Create())
            {

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(
                    Encoding.UTF8.GetBytes(
                        password));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                passwordInMd5 = sBuilder.ToString().ToLower();
            }

            if (FindPasswordOfUser(username).ToLower().Equals(passwordInMd5))
                return true;
            else
                return false;

        }
    }
}