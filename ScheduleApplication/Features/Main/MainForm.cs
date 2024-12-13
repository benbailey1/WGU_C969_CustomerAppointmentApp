using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleApplication.Features.Appointments;
using ScheduleApplication.Features.Customers;
using ScheduleApplication.Shared.Infrastructure.Database;

namespace ScheduleApplication.Features.Main
{
    public partial class MainForm : Form
    {
        private int _loggedInUserId;
        private IAppointmentService _apptService;
        public MainForm(int loggedInUserId, IDbConnectionFactory dbConFact)
        {
            Console.WriteLine($"Starting MainForm constructor. User ID: {_loggedInUserId}");

            InitializeComponent();

            // Add immediate visual feedback
            this.BackColor = Color.White;
            Label loadingLabel = new Label
            {
                Text = "Loading Main Form...",
                AutoSize = true,
                Font = new Font("Segoe UI", 14),
                Location = new Point(10, 10)
            };
            this.Controls.Add(loadingLabel);

            _loggedInUserId = loggedInUserId;

            Console.WriteLine($"Navigated to Main Form. User ID: {_loggedInUserId}");

            // Set form properties
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;

            var appointmentRepo = new AppointmentRepository(dbConFact);
            var appointmentValidator = new AppointmentValidator();
            _apptService = new AppointmentService(appointmentRepo, appointmentValidator);

            Console.WriteLine("Setting up navigation tree view...");
            SetupNavigationTreeView();

            this.Controls.Remove(loadingLabel);

        }

        private void btnManageAppointments_Click(object sender, EventArgs e)
        {
            AppointmentManagementForm appointmentForm = new AppointmentManagementForm(_apptService);
            appointmentForm.Show();
        }

        private void SetupNavigationTreeView()
        {
            Console.WriteLine("In SetupNavigationTreeView()");
            // Create the main nodes
            TreeNode reportsNode = new TreeNode("Reports");
            TreeNode managementNode = new TreeNode("Management");

            // Add report subnodes
            reportsNode.Nodes.Add(new TreeNode("Appointment Types by Month") { Tag = "appointment-types" });
            reportsNode.Nodes.Add(new TreeNode("Schedule for Each User") { Tag = "user-schedules" });
            reportsNode.Nodes.Add(new TreeNode("Appointments by Location") { Tag = "location-report" });

            // Add management subnodes
            managementNode.Nodes.Add(new TreeNode("Manage Customers") { Tag = "customers" });
            managementNode.Nodes.Add(new TreeNode("Manage Appointments") { Tag = "appointments" });

            // Add all main nodes to TreeView
            navigationTreeView.Nodes.Add(reportsNode);
            navigationTreeView.Nodes.Add(managementNode);

            // Add logout node
            navigationTreeView.Nodes.Add(new TreeNode("Log Out") { Tag = "logout" });

            // Expand all nodes for better visibility
            navigationTreeView.ExpandAll();

            // Handle node selection
            navigationTreeView.AfterSelect += NavigationTreeView_AfterSelect;
        }

        private void NavigationTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return; // Main category nodes don't have actions

            switch (e.Node.Tag.ToString())
            {
                case "appointment-types":
                    ShowAppointmentTypesReport();
                    break;

                case "user-schedules":
                    ShowUserSchedulesReport();
                    break;

                case "location-report":
                    ShowLocationReport();
                    break;

                case "customers":
                    ShowCustomerManagement();
                    break;

                case "appointments":
                    ShowAppointmentManagement();
                    break;

                case "logout":
                    HandleLogout();
                    break;
            }
        }

        private async void ShowAppointmentTypesReport()
        {
            try
            {
                contentPanel.Controls.Clear();

                MessageBox.Show("Showing Appointment Types Report");

                Label loadingLabel = new Label
                {
                    Text = "Loading report...",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12),
                    Location = new Point(10, 10)
                };
                contentPanel.Controls.Add(loadingLabel);

                var result = await _apptService.GetAppointmentTypesByMonthAsync();

                if (!result.IsSuccess)
                {
                    MessageBox.Show($"Error loading report: {result.Errors}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                contentPanel.Controls.Remove(loadingLabel);

                Panel reportPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                };
                contentPanel.Controls.Add(reportPanel);


                Label titleLabel = new Label
                {
                    Text = "Appointment Types by Month Report",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                contentPanel.Controls.Add(titleLabel);

                ComboBox viewSelector = new ComboBox
                {
                    Location = new Point(titleLabel.Right + 20, titleLabel.Top),
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                viewSelector.Items.AddRange(new string[]
                {
                    "Monthly Detail View",
                    "Type Summary View",
                    "Monthly Trend View"
                });
                viewSelector.SelectedIndex = 0;
                reportPanel.Controls.Add(viewSelector);


                DataGridView gridView = new DataGridView
                {
                    Location = new Point(10, titleLabel.Bottom + 20),
                    Width = contentPanel.Width - 40,
                    Height = contentPanel.Height - viewSelector.Bottom - 40,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.Fixed3D,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.AliceBlue }
                };
                reportPanel.Controls.Add(gridView);

                DataTable detailView = CreateDetailViewTable(result.Value);
                DataTable summaryView = CreateSummaryViewTable(result.Value);
                DataTable trendView = CreateTrendViewTable(result.Value);

                viewSelector.SelectedIndexChanged += (sender, e) =>
                {
                    switch (viewSelector.SelectedIndex)
                    {
                        case 0: // Monthly Detail View
                            gridView.DataSource = detailView;
                            break;
                        case 1: // Type Summary View
                            gridView.DataSource = summaryView;
                            break;
                        case 2: //Monthly Trend View
                            gridView.DataSource = trendView;
                            break;
                    }

                    gridView.AutoResizeColumns();

                    if (viewSelector.SelectedIndex == 1)
                    {
                        gridView.Columns["Percentage"].DefaultCellStyle.Format = "P1";
                    }
                };

                gridView.DataSource = detailView;
                gridView.AutoResizeColumns();

                reportPanel.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private DataTable CreateTrendViewTable(List<MonthlyAppointmentTypes> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Month", typeof(string));
            dt.Columns.Add("Appointment Type", typeof(string));
            dt.Columns.Add("Count", typeof(int));

            foreach (var month in data.OrderBy(m => DateTime.ParseExact(m.Month, "MMMM", null)))
            {
                foreach (var typeCount in month.TypeCounts.OrderByDescending(tc => tc.Count))
                {
                    dt.Rows.Add(month.Month, typeCount.Type, typeCount.Count);
                }
            }

            return dt;
        }

        private DataTable CreateSummaryViewTable(List<MonthlyAppointmentTypes> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Appointment Type", typeof(string));
            dt.Columns.Add("Total Count", typeof(int));
            dt.Columns.Add("Percentage", typeof(double));

            var typeSummary = data
                .SelectMany(m => m.TypeCounts)
                .GroupBy(tc => tc.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Sum(tc => tc.Count)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            int totalAppointments = typeSummary.Sum(x => x.Count);

            foreach (var summary in typeSummary)
            {
                dt.Rows.Add(
                    summary.Type,
                    summary.Count,
                    (double)summary.Count / totalAppointments
                );
            }

            return dt;
        }

        private DataTable CreateDetailViewTable(List<MonthlyAppointmentTypes> data)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Month", typeof(string));
            dt.Columns.Add("Total Appointments", typeof(int));
            dt.Columns.Add("Most Common Type", typeof(string));
            dt.Columns.Add("Unique Types", typeof(int));

            foreach (var month in data.OrderBy(m => DateTime.ParseExact(m.Month, "MMMM", null)))
            {
                dt.Rows.Add(
                    month.Month,
                    month.TypeCounts.Sum(tc => tc.Count),
                    month.TypeCounts.OrderByDescending(tc => tc.Count).First().Type,
                    month.TypeCounts.Count
                );
            }

            return dt;
        }

        private async void ShowUserSchedulesReport()
        {
            try
            {   
                MessageBox.Show("Showing User Schedules Report");
                contentPanel.Controls.Clear();

                Label loadingLabel = new Label
                {
                    Text = "Loading report...",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12),
                    Location = new Point(10, 10)
                };
                contentPanel.Controls.Add(loadingLabel);

                var result = await _apptService.GetScheduleForEachUserAsync();

                if (!result.IsSuccess)
                {
                    MessageBox.Show($"Error loading report: {result.Errors}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                contentPanel.Controls.Remove(loadingLabel);

                Panel reportPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                };
                contentPanel.Controls.Add(reportPanel);


                Label titleLabel = new Label
                {
                    Text = "Appointments By User Report",
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                contentPanel.Controls.Add(titleLabel);

                DataGridView gridView = new DataGridView
                {
                    Location = new Point(10, titleLabel.Bottom + 20),
                    Width = contentPanel.Width - 40,
                    Height = contentPanel.Height - 40,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.Fixed3D,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.AliceBlue }
                };
                reportPanel.Controls.Add(gridView);

                DataTable dt = new DataTable();
                dt.Columns.Add("UserId", typeof(int));
                dt.Columns.Add("Appointment Type", typeof(string));
                dt.Columns.Add("Date", typeof(int));

                //foreach (var month in data.OrderBy(m => DateTime.ParseExact(m.Month, "MMMM", null)))
                //{
                //    foreach (var typeCount in month.TypeCounts.OrderByDescending(tc => tc.Count))
                //    {
                //        dt.Rows.Add(month.Month, typeCount.Type, typeCount.Count);
                //    }
                //}

                //return dt;

            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        private void ShowLocationReport()
        {
            contentPanel.Controls.Clear();
            // TODO: Add your report display logic here
            MessageBox.Show("Showing Location Report");
        }

        private void ShowCustomerManagement()
        {
            contentPanel.Controls.Clear();
            var customerForm = new CustomerManagementForm();
            customerForm.TopLevel = false;
            customerForm.FormBorderStyle = FormBorderStyle.None;
            customerForm.Dock = DockStyle.Fill;

            contentPanel.Controls.Add(customerForm);
            customerForm.Show();
        }

        private void ShowAppointmentManagement()
        {
            contentPanel.Controls.Clear();
            var appointmentForm = new AppointmentManagementForm(_apptService);
            appointmentForm.TopLevel = false;
            appointmentForm.FormBorderStyle = FormBorderStyle.None;
            appointmentForm.Dock = DockStyle.Fill;

            contentPanel.Controls.Add(appointmentForm);
            appointmentForm.Show();
        }

        private void HandleLogout()
        {
            if (MessageBox.Show("Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
