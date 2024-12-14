using System.Drawing;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Customers
{
    partial class CustomerDetailFormOLD
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtCustomerName = new TextBox { Name = "txtCustomerName" };
            this.txtAddress1 = new TextBox { Name = "txtAddress1" };
            this.txtAddress2 = new TextBox { Name = "txtAddress2" };
            this.txtCity = new TextBox { Name = "txtCity" };
            this.txtPostalCode = new TextBox { Name = "txtPostalCode" };
            this.txtPhone = new TextBox { Name = "txtPhone" };
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = _customerId.HasValue ? "Update Customer" : "Add Customer";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 500);

            // Labels
            int labelX = 20;
            int currentY = 20;
            int spacing = 45;

            var labels = new[]
            {
                "Customer Name",
                "Address",
                "Address 2",
                "City",
                "Postal Code",
                "Phone"
            };

            var textBoxes = new[]
            {
                txtCustomerName,
                txtAddress1,
                txtAddress2,
                txtCity,
                txtPostalCode,
                txtPhone
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var label = new Label
                {
                    Text = labels[i],
                    Location = new Point(labelX, currentY),
                    AutoSize = true
                };
                this.Controls.Add(label);

                var textBox = textBoxes[i];
                textBox.Location = new Point(labelX + 120, currentY);
                textBox.Width = 200;
                textBox.TextChanged += (s, e) => _hasUnsavedChanges = true;
                this.Controls.Add(textBox);

                _errorLabels[textBox.Name.Substring(3)].Location = new Point(labelX, currentY + 20);

                currentY += spacing;
            }

            // Buttons
            btnSave.Text = "Save";
            btnSave.Location = new Point(labelX + 120, currentY);
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(labelX + 220, currentY);
            btnCancel.Click += btnCancel_Click;
            this.Controls.Add(btnCancel);
        }

        #endregion
    }
}