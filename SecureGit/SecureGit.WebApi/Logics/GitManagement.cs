using System.IO;
using MySql.Data.MySqlClient;

namespace SecureGit.WebApi.Logics
{
    public class GitManagement
    {
        // private string _dbConnectionString;
        private readonly DatabaseManagement _dbManagement;

        public GitManagement(
            string DBConnectionString)
        {
            // _dbConnectionString = DBConnectionString;
            _dbManagement = new DatabaseManagement(
                DBConnectionString);
        }
        public string GetProjectList(
            /*string ProjectListFilePath*/
            string username)
        {
            // MySqlDataReader reader = _dbManagement.RunSqlCommand(
            //     $"select * from Users where Username = '{username}'");

            // if (reader == null)
            //     return null;

            // while (reader.Read())
            // {
            //     return reader["AssignedProjects"].ToString();
            // }

            // return null;

            string sqlCommand = $"select * from Users where Username = '{username}'";

            return _dbManagement.RunSqlCommand(
                sqlCommand,
                reader => 
                {
                    if (reader == null)
                        return null;

                    while (reader.Read())
                    {
                        return reader["AssignedProjects"].ToString();
                    }

                    return null;
                }
            );
        }
    }
}