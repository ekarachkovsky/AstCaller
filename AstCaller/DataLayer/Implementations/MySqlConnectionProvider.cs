using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace AstCaller.DataLayer.Implementations
{
    public class MySqlConnectionProvider : ISqlConnectionProvider
    {
        private readonly IConfiguration _config;

        public MySqlConnectionProvider(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection GetConnection()
        {
            var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            return connection;
        }
    }
}
