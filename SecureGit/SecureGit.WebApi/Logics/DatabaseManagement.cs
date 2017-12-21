using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using SecureGit.WebApi.Models;

namespace SecureGit.WebApi.Logics
{
    public class DatabaseManagement
    {
        public string ConnectionString { get; set; }

        public DatabaseManagement(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        // public bool UpdateColumnValue(
        //     string TableName,
        //     string ColumnName,
        //     string NewValue,
        //     string Clause)
        // {
        //     try
        //     {
        //         using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //         {
        //             conn.Open();
        //             MySqlCommand cmd = new MySqlCommand(
        //                 $"update {TableName} set {ColumnName} = '{NewValue}' where {Clause}",
        //                 conn);

        //             cmd.ExecuteReader();

        //             return true;
        //         }
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // public string FetchColumnValueByRow(
        //     string TableName,
        //     string ColumnName)
        // {
        //     return "";
        // }

        // public MySqlDataReader RunSqlCommand(
        //     string SqlCommand)
        // {
        //     try
        //     {
        //         using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //         {
        //             conn.Open();
        //             MySqlCommand cmd = new MySqlCommand(
        //                 SqlCommand,
        //                 conn);
        //             // return cmd.ExecuteReader();

        //             MySqlDataReader reader = cmd.ExecuteReader(
        //                 CommandBehavior.CloseConnection);

        //             return reader;
        //         }
        //     }
        //     catch
        //     {
        //         return null;
        //     }
        // }

        public T RunSqlCommand<T>(
            string SqlCommand,
            Func<MySqlDataReader, T> doThis)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        SqlCommand,
                        conn);
                    // return cmd.ExecuteReader();

                    // MySqlDataReader reader = cmd.ExecuteReader();

                    // Since the MySqlDataReader will be disposed after quit from 'using' scope
                    // I need to run the doThis so the RunSqlCommand() is reusable
                    return doThis(cmd.ExecuteReader());

                    // return reader;
                }
            }
            catch
            {
                return default(T);
            }
        }

        // public bool RunSqlCommand(
        //     string SqlCommand,
        //     out MySqlDataReader DataReader)
        // {
        //     DataReader = null;

        //     try
        //     {
        //         using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //         {
        //             conn.Open();
        //             MySqlCommand cmd = new MySqlCommand(
        //                 SqlCommand,
        //                 conn);
        //             DataReader = cmd.ExecuteReader();

        //             return true;
        //         }
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        public List<User> GetAllUsers()
        {
            List<User> list = new List<User>();

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    "select * from Users",
                    conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = reader["Username"].ToString(),
                            Email = reader["Email"].ToString(),
                            Password = reader["Password"].ToString(),
                            Role = (UserRole)Convert.ToInt32(reader["Role"]),
                            PublicKey = reader["PublicKey"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // public bool UpdateUserPublicKey(
        //     string username,
        //     string publicKey)
        // {
        //     try
        //     {
        //         using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //         {
        //             conn.Open();
        //             MySqlCommand cmd = new MySqlCommand(
        //                 $"update Users set PublicKey = '{publicKey}' where Username = '{username}'",
        //                 conn);

        //             cmd.ExecuteReader();

        //             return true;
        //         }
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // public string FetchUserPublicKey(
        //     string username)
        // {
        //     using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //     {
        //         conn.Open();
        //         MySqlCommand cmd = new MySqlCommand(
        //             $"select * from Users where Username = '{username}'",
        //             conn);

        //         using (var reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 return reader["PublicKey"].ToString();
        //             }
        //         }

        //         return null;
        //     }
        // }

        // public bool UpdateUserAssignedProjects(
        //     string username,
        //     string projectList)
        // {
        //     try
        //     {
        //         using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //         {
        //             conn.Open();
        //             MySqlCommand cmd = new MySqlCommand(
        //                 $"update Users set AssignedProjects = '{projectList}' where Username = '{username}'",
        //                 conn);

        //             cmd.ExecuteReader();

        //             return true;
        //         }
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // public string FetchUserAssignedProjects(
        //     string username)
        // {
        //     using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        //     {
        //         conn.Open();
        //         MySqlCommand cmd = new MySqlCommand(
        //             $"select * from Users where Username = '{username}'",
        //             conn);

        //         using (var reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 return reader["AssignedProjects"].ToString();
        //             }
        //         }

        //         return null;
        //     }
        // }
    }
}