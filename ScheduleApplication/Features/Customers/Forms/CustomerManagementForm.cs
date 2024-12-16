using System;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Customers
{
    public partial class CustomerManagementForm : Form
    {
        private readonly ICustomerService _customerService;
        private bool _isLoading = false;
        public CustomerManagementForm(ICustomerService customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            InitializeComponent();
            SetupDataGridView();
            LoadCustomers();
        }

        private void SetupDataGridView()
        {
            customerGridView.AutoGenerateColumns = false;
            customerGridView.AllowUserToAddRows = false;
            customerGridView.AllowUserToDeleteRows = false;
            customerGridView.MultiSelect = false;
            customerGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            customerGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            customerGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerId",
                HeaderText = "Customer ID",
                DataPropertyName = "CustomerId",
                ReadOnly = true
            });

            customerGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                DataPropertyName = "CustomerName",
                ReadOnly = true
            });

            customerGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Address",
                HeaderText = "Address",
                DataPropertyName = "Address1",
                ReadOnly = true
            });
        }

        private async void LoadCustomers()
        {
            try
            {
                _isLoading = true;
                var result = await _customerService.GetAllCustomersAsync();
                if (result.IsSuccess)
                {
                    customerGridView.DataSource = result.Value;
                }
                else
                {
                    MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            using (var detailForm = new CustomerDetailForm(_customerService))
            {
                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            if (customerGridView.SelectedRows.Count == 0) return;

            var customer = (CustomerResponse)customerGridView.SelectedRows[0].DataBoundItem;
            using (var detailForm = new CustomerDetailForm(_customerService, customer.CustomerId))
            {
                if (detailForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
        }

        private async void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (customerGridView.SelectedRows.Count == 0) return;

            var customer = (CustomerResponse)customerGridView.SelectedRows[0].DataBoundItem;
            if (MessageBox.Show($"Are you sure you want to delete {customer.CustomerName}?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var result = await _customerService.DeleteCustomerAsync(customer.CustomerId);
                if (result.IsSuccess)
                {
                    LoadCustomers();
                }
                else
                {
                    MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
