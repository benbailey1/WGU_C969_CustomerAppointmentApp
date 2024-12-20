using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleApplication.Features.Main;
using ScheduleApplication.Shared.Infrastructure.Database;

namespace ScheduleApplication.Features.Login
{
    public partial class LoginForm : Form
    {
        private CultureInfo _currentCulture;
        private readonly IAuthService _authService;
        private readonly IDbConnectionFactory _dbConFact;
        public int LoggedInUserId { get; private set; }

        public LoginForm(IAuthService authService, IDbConnectionFactory dbConFact)
        {
            InitializeComponent();

            _authService = authService;
            _dbConFact = dbConFact;

            textPassword.PasswordChar = '*';
            _currentCulture = CultureInfo.CurrentCulture;

            SetLanguage(_currentCulture.TwoLetterISOLanguageName);
            this.AcceptButton = btnLogin;
        }

        private void LoginForm_Load(object sender, EventArgs e) { }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;

            try
            {
                string username = txtUsername.Text.Trim();
                string password = textPassword.Text.Trim();

                var authResult = await _authService.AuthenticateAsync(username, password);

                if (authResult.IsSuccess)
                {
                    LoggedInUserId = authResult.Value.UserId;
                    _authService.LogUserLogin(username, true);

                    await ShowUpcomingAppointments();

                    this.Hide();
                    var mainForm = new MainForm(LoggedInUserId, _dbConFact);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show(GetLocalizedString(authResult.Error));
                    _authService.LogUserLogin(username, false);
                    textPassword.Clear();
                    textPassword.Focus();
                }
            }
            finally
            {
                btnLogin.Enabled = true;
            }

        }

        private async Task ShowUpcomingAppointments()
        {
            try
            {
                var appointments = await _authService.GetUpcomingAppointmentsAsync(LoggedInUserId);
                
                if (appointments.Any())
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("You have upcoming appointments:");

                    foreach (var appt in appointments)
                    {
                        message.AppendLine($"- {appt.CustomerName} at {TimeZoneInfo.ConvertTimeFromUtc(appt.Start, TimeZoneInfo.Local):g} ({appt.Type}");
                    }

                    MessageBox.Show(message.ToString(), "Upcoming Appointments",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking appointments: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void SetLanguage(string language)
        {
            if (language == "es")
            {
                labelGreeting.Text = "Por favor, inicie sesión para gestionar citas";
                labelUsername.Text = "Nombre de usuario";
                labelPassword.Text = "Contraseña";
                btnLogin.Text = "Iniciar sesión";
            }
            else
            {
                labelGreeting.Text = "Please Login to Manage Appointments";
                labelUsername.Text = "Username";
                labelPassword.Text = "Password";
                btnLogin.Text = "Login";
            }
        }

        private string GetLocalizedString(string message)
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "es")
            {
                if (message.StartsWith("Authentication error: "))
                {
                    string errorMessage = message.Substring("Authentication error: ".Length);
                    return $"Error de autenticación: {errorMessage}";
                }
                switch (message)
                {
                    case "Login successful!":
                        return "¡Inicio de sesión exitoso!";
                    case "Invalid username or password":
                        return "El nombre de usuario y la contraseña no coinciden.";
                    case "Username and password cannot be empty.":
                        return "El nombre de usuario y la contraseña no pueden estar vacíos.";
                    default:
                        return message;
                }
            }

            return message;
        }
    }
}
