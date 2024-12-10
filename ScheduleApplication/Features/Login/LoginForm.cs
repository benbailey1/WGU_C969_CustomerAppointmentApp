using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Login
{
    public partial class LoginForm : Form
    {
        private CultureInfoConverter _currentCulture;
        public int LoggedInUserId { get; private set; }

        public LoginForm()
        {
            InitializeComponent();

            textPassword.PasswordChar = '*';
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;

            try
            {
                string username = txtUsername.Text.Trim();
                string password = textPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show(GetLocalizedString("EmptyFields"));
                    return;
                }

                if (await)
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
