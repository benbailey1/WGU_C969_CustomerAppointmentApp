using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScheduleApplication.Shared.Classes;
using ScheduleApplication.Shared.Domain.Cities;

namespace ScheduleApplication.Features.Customers
{
    public partial class CustomerDetailForm : Form
    {
        private readonly ICustomerService _customerService;
        private readonly ICityRepository _cityRepo;
        private readonly int? _customerId;
        private bool _hasUnsavedChanges;
        private Dictionary<string, Label> _errorLabels;
        public CustomerDetailForm(ICustomerService customerService, ICityRepository cityRepo, int? customerId = null)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _cityRepo = cityRepo ?? throw new ArgumentNullException(nameof(cityRepo));
            _customerId = customerId;
            InitializeComponent();
            SetupErrorLabels();
            GetCities();
            if (_customerId.HasValue)
            {
                LoadCustomer();
            }
        }

        private async void GetCities()
        {
            try
            {
                var cities = await _cityRepo.GetAllCitiesAsync();
                if (cities != null && cities.Any())
                {
                    comboBoxCity.DataSource = cities;
                    comboBoxCity.DisplayMember = "CityName";
                    comboBoxCity.ValueMember = "CityId";
                    comboBoxCity.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("No cities found in the database.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cities: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetupErrorLabels()
        {
            _errorLabels = new Dictionary<string, Label>
            {
                { "CustomerName", new Label { ForeColor = Color.Red, AutoSize = true } },
                { "Address1", new Label { ForeColor = Color.Red, AutoSize = true } },
                { "Address2", new Label { ForeColor = Color.Red, AutoSize = true } },
                { "City", new Label { ForeColor = Color.Red, AutoSize = true } },
                { "PostalCode", new Label { ForeColor = Color.Red, AutoSize = true } },
                { "Phone", new Label { ForeColor = Color.Red, AutoSize = true } }
            };

            foreach (var label in _errorLabels.Values)
            {
                this.Controls.Add(label);
            }
        }

        private async void LoadCustomer()
        {
            var result = await _customerService.GetCustomerByIdAsync(_customerId.Value);
            if (result.IsSuccess)
            {
                txtCustomerName.Text = result.Value.CustomerName;
                txtAddress1.Text = result.Value.Address1;
                txtAddress2.Text = result.Value.Address2;
                txtPostalCode.Text = result.Value.PostalCode;
                txtPhone.Text = result.Value.Phone;

                var cities = (List<City>)comboBoxCity.DataSource;
                var cityIndex = cities.FindIndex(c => c.CityId.ToString() == result.Value.City);
                if (cityIndex < 0)
                {
                    comboBoxCity.SelectedIndex = cityIndex;
                }
            }
        }



        private async void btnSave_Click(object sender, EventArgs e)
        {
            ClearErrors();
            if (!ValidateInput()) return;

            int selectedCityId = comboBoxCity.SelectedItem != null ?
                ((City)comboBoxCity.SelectedItem).CityId : -1;

            var customer = new Customer
            {
                CustomerId = _customerId ?? 0,
                CustomerName = txtCustomerName.Text.Trim(),
                Address = new Address
                {
                    Address1 = txtAddress1.Text.Trim(),
                    Address2 = txtAddress2.Text.Trim(),
                    CityId = selectedCityId,
                    PostalCode = txtPostalCode.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                },
                Active = true,
                AuditInfo = new AuditInfo
                {
                    CreatedBy = "system",
                    LastUpdateBy = "system"
                }
            };

            Result<bool> result;
            if (_customerId.HasValue)
            {
                result = await _customerService.UpdateCustomerAsync(customer);
            }
            else
            {
                var createResult = await _customerService.CreateCustomerAsync(customer);

                // Convert Result<int> to Result<bool>
                result = createResult.IsSuccess
                    ? Result<bool>.Success(true)
                    : Result<bool>.Failure(createResult.Error);
            }

            if (result.IsSuccess)
            {
                _hasUnsavedChanges = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure you want to cancel?",
                    "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            int selectedCityId = comboBoxCity.SelectedItem != null ?
                ((City)comboBoxCity.SelectedItem).CityId : -1;

            var customer = new Customer
            {
                CustomerName = txtCustomerName.Text.Trim(),
                Address = new Address
                {
                    Address1 = txtAddress1.Text.Trim(),
                    Address2 = txtAddress2.Text.Trim(),
                    CityId = selectedCityId,
                    PostalCode = txtPostalCode.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                }
            };

            var errors = CustomerValidator.ValidateCustomer(customer);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    if (error.Contains("Customer name"))
                        _errorLabels["CustomerName"].Text = error;
                    else if (error.Contains("Street and House"))
                        _errorLabels["Address1"].Text = error;
                    else if (error.Contains("Postal code"))
                        _errorLabels["PostalCode"].Text = error;
                    else if (error.Contains("City"))
                        _errorLabels["City"].Text = error;
                    else if (error.Contains("Phone"))
                        _errorLabels["Phone"].Text = error;
                }
                return false;
            }
            return true;
        }

        private void ClearErrors()
        {
            foreach (var label in _errorLabels.Values)
            {
                label.Text = "";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _hasUnsavedChanges && this.DialogResult != DialogResult.OK)
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure you want to close?",
                    "Confirm Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}
