using ScheduleApplication.Features.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Dependencies
            //  var dbConnection = new MySqlConnectionFactory();
            var userRepository = new UserLoginRepository(dbConnection);
            var authService = new AuthService(userRepository);


            Application.Run(new Form1());
        }
    }
}
