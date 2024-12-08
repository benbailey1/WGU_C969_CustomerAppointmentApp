using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace ScheduleApplication.Shared.Infrastructure.Database
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
    public class DBConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DBConnectionFactory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;
        }

        public MySqlConnection CreateConnection()
        {
            try
            {
                return new MySqlConnection(_connectionString);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting with the database: ", ex.Message);
                throw;
            }
        }
    }
}
