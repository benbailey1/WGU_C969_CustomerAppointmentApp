using ScheduleApplication.Features.Appointments.Models;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Login
{
    public interface IAuthService
    {
        Task<AuthResult<UserLoginModel>> AuthenticateAsync(string username, string password);
        Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(int userId);
        void LogUserLogin(string username, bool loginSuccess);
    }
    public class AuthService : IAuthService
    {
        private IUserLoginRepository _userRepo;
        private bool projectTestingForSchool = true; // Flip to false when NOT using school database 

        public AuthService(IUserLoginRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(int userId)
        {
            DateTime now = DateTime.UtcNow;

            return await _userRepo.GetUpcomingAppointmentsAsync(
                userId,
                now,
                now.AddMinutes(15));
        }

        public async Task<AuthResult<UserLoginModel>> AuthenticateAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return AuthResult<UserLoginModel>.Failure("Username and password cannot be empty.");
                
                
                string hashedPassword = HashPassword(password);
                var user = await _userRepo.GetUserByCredentialsAsync(username, hashedPassword);

                if (user == null)
                    return AuthResult<UserLoginModel>.Failure("Invalid username or password");

                return AuthResult<UserLoginModel>.Success(user);
            }
            catch (Exception ex)
            {

                return AuthResult<UserLoginModel>.Failure($"Authentication error: {ex.Message}");
            }
        }

        public void LogUserLogin(string username, bool loginSuccess)
        {
            try
            {
                // string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Login_History.txt");

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string solutionDir = Directory.GetParent(baseDir).Parent.Parent.FullName;

                string loggingDir = Path.Combine(solutionDir, "Features", "Logging");
                string logFilePath = Path.Combine(loggingDir, "Login_History.txt");

                if (!Directory.Exists(loggingDir))
                {
                    Directory.CreateDirectory(loggingDir);
                }

                string logEntry;

                if (!loginSuccess)
                {
                    logEntry = $"{DateTime.UtcNow:yyy-MM-dd HH:mm:ss UTC}: User login failed {Environment.MachineName}";
                }
                else
                {
                    logEntry = $"{DateTime.UtcNow:yyy-MM-dd HH:mm:ss UTC}: User '{username}' logged in from {Environment.MachineName}";
                }
              
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(logEntry);
                }
}
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to log user login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Failed to log user login: {ex.Message}");
            }
            
        }

        // Using plain text here only b/c we're working with a pre-configured database with this school project
        // Switch property `projectTestingForSchool = false;` to hash password
        // DO NOT USE PLAIN TEXT IN PRODUCTION!!!!!!!!!!!!
        private string HashPassword(string password)
        {
            if (projectTestingForSchool)
            {
                return password;
            }
            else
            {
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(hashedBytes);
                }
            }
            
        }
    }
}
