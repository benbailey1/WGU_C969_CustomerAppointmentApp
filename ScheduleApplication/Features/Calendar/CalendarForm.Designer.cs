namespace ScheduleApplication.Features.Calendar
{
    partial class CalendarForm
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
            this.AppointmentCalendar = new System.Windows.Forms.MonthCalendar();
            this.dataGridViewAppointments = new System.Windows.Forms.DataGridView();
            this.lblCalendar = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppointments)).BeginInit();
            this.SuspendLayout();
            // 
            // AppointmentCalendar
            // 
            this.AppointmentCalendar.Location = new System.Drawing.Point(33, 104);
            this.AppointmentCalendar.Margin = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.AppointmentCalendar.Name = "AppointmentCalendar";
            this.AppointmentCalendar.TabIndex = 1;
            this.AppointmentCalendar.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.AppointmentCalendar_DateChanged);
            // 
            // dataGridViewAppointments
            // 
            this.dataGridViewAppointments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAppointments.Location = new System.Drawing.Point(335, 72);
            this.dataGridViewAppointments.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewAppointments.Name = "dataGridViewAppointments";
            this.dataGridViewAppointments.RowHeadersWidth = 51;
            this.dataGridViewAppointments.Size = new System.Drawing.Size(452, 278);
            this.dataGridViewAppointments.TabIndex = 2;
            // 
            // lblCalendar
            // 
            this.lblCalendar.AutoSize = true;
            this.lblCalendar.Location = new System.Drawing.Point(59, 72);
            this.lblCalendar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCalendar.Name = "lblCalendar";
            this.lblCalendar.Size = new System.Drawing.Size(214, 16);
            this.lblCalendar.TabIndex = 3;
            this.lblCalendar.Text = "Select a date to view appointments";
            // 
            // CalendarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblCalendar);
            this.Controls.Add(this.dataGridViewAppointments);
            this.Controls.Add(this.AppointmentCalendar);
            this.Name = "CalendarForm";
            this.Text = "CalendarForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppointments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar AppointmentCalendar;
        private System.Windows.Forms.DataGridView dataGridViewAppointments;
        private System.Windows.Forms.Label lblCalendar;
    }
}