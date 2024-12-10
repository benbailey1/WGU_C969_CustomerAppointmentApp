using ScheduleApplication.Features.Appointments.Models;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Login
{
    public interface IAuthenticationService
    {
        Task<AuthResult<UserLoginModel>> AuthenticateAsync(string username, string password);
        Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(string userId, DateTime startTime, DateTime endTime);
        void LogUserLogin(string username);
    }
    public class AuthService : IAuthenticationService
    {
        private IUserLoginRepository _userRepo;

        public AuthService(IUserLoginRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(string userId, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResult<UserLoginModel>> AuthenticateAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return AuthResult<UserLoginModel>.Failure("Username and password cannot be empty.");
                }

                string hashedPassword = HashPassword(password)
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void LogUserLogin(string username)
        {
            throw new NotImplementedException();
        }
    }
}
