using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi.Logics
{
    public class UserManagement// : IDisposable
    {
        // private List<UserDatabase> users;
        private List<User> _users;
        // private string _dbConnectionString;

        private readonly DatabaseManagement _dbManagement;

        public UserManagement(
            string DBConnectionString)
        {
            // _dbConnectionString = DBConnectionString;
            _dbManagement = new DatabaseManagement(
                DBConnectionString);
            _users = GetUserList();
        }

        // private List<UserDatabase> GetUserList(
        //     string UserDatabaseFilePath)
        // {
        //     string users = File.ReadAllText(UserDatabaseFilePath);
        //     return JsonConvert.DeserializeObject<List<UserDatabase>>(users);
        // }

        public bool FindIfUserExist(string username)
        {
            User ud = _users.Find(d => d.Username == username);
            return ud != null;
        }

        private string FindPasswordOfUser(string username)
        {
            return _users.Find(d => d.Username == username).Password;
        }

        private List<User> GetUserList()
        {
            // MySqlDataReader reader = _dbManagement.RunSqlCommand(
            //     "select * from Users");

            // if (reader == null || reader.HasRows == false)
            //     return null;

            // List<User> list = new List<User>();

            // while (reader.Read())
            // {
            //     list.Add(new User()
            //     {
            //         Id = Convert.ToInt32(reader["Id"]),
            //         Username = reader["Username"].ToString(),
            //         Email = reader["Email"].ToString(),
            //         Password = reader["Password"].ToString(),
            //         Role = (UserRole)Convert.ToInt32(reader["Role"]),
            //         PublicKey = reader["PublicKey"].ToString()
            //     });
            // }

            // return list;

            return _dbManagement.GetAllUsers();
        }

        private string StringToMd5Hash(
            string textString
        )
        {
            try
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    // Convert the input string to a byte array and compute the hash.
                    byte[] data = md5Hash.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            textString));

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
                    return sBuilder.ToString().ToLower();
                }
            }
            catch
            {
                return null;
            }
        }

        public bool ResetPassword(
            string username,
            string oldPassword,
            string newPassword)
        {
            if(!IsCredentialValid(
                username, 
                oldPassword)) 
                return false;
            
            MySqlDataReader reader = _dbManagement.RunSqlCommand(
                $"update Users set Password = '{StringToMd5Hash(newPassword)}' where Username = '{username}'");

            return reader != null;
        }

        public bool UpdateUserPublicKey(
            string username,
            string publicKey)
        {
            if (!FindIfUserExist(username))
                return false;
            
            MySqlDataReader reader = _dbManagement.RunSqlCommand(
                $"update Users set PublicKey = '{publicKey}' where Username = '{username}'");

            return reader != null;
        }

        public string FetchUserPublicKey(
            string username)
        {
            if (!FindIfUserExist(username))
                return null;

            MySqlDataReader reader = _dbManagement.RunSqlCommand(
                $"select * from Users where Username = '{username}'");

            if (reader == null)
                return null;

            while (reader.Read())
            {
                return reader["PublicKey"].ToString();
            }

            return null;
        }

        public bool UpdateUserAssignedProjects(
            string username,
            string projectList)
        {
            if (!FindIfUserExist(username))
                return false;
            
            MySqlDataReader reader = _dbManagement.RunSqlCommand(
                $"update Users set AssignedProjects = '{projectList}' where Username = '{username}'");

            return reader != null;
        }

        public string FetchUserAssignedProjects(
            string username)
        {
            if (!FindIfUserExist(username))
                return null;

            MySqlDataReader reader = _dbManagement.RunSqlCommand(
                $"select * from Users where Username = '{username}'");

            if (reader == null)
                return null;

            while (reader.Read())
            {
                return reader["AssignedProjects"].ToString();
            }

            return null;
        }

        public bool IsCredentialValid(
            string username,
            string password)
        {
            if (!FindIfUserExist(username))
                return false;

            string passwordInMd5 = StringToMd5Hash(password);
            // using (MD5 md5Hash = MD5.Create())
            // {
            //     // Convert the input string to a byte array and compute the hash.
            //     byte[] data = md5Hash.ComputeHash(
            //         Encoding.UTF8.GetBytes(
            //             password));

            //     // Create a new Stringbuilder to collect the bytes
            //     // and create a string.
            //     StringBuilder sBuilder = new StringBuilder();

            //     // Loop through each byte of the hashed data 
            //     // and format each one as a hexadecimal string.
            //     for (int i = 0; i < data.Length; i++)
            //     {
            //         sBuilder.Append(data[i].ToString("x2"));
            //     }

            //     // Return the hexadecimal string.
            //     passwordInMd5 = sBuilder.ToString().ToLower();
            // }

            if (FindPasswordOfUser(username).ToLower().Equals(passwordInMd5))
                return true;
            else
                return false;
        }
    }
}