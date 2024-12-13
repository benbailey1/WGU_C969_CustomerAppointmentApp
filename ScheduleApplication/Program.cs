using ScheduleApplication.Features.Login;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
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

            // Configure Dependencies for initial form
            IDbConnectionFactory dbConnectionFactory = new DBConnectionFactory();
            IUserLoginRepository userRepository = new UserLoginRepository(dbConnectionFactory);
            IAuthService authService = new AuthService(userRepository);

            // Create and run Login form
            Application.Run(new LoginForm(authService, dbConnectionFactory));
        }
    }
}
