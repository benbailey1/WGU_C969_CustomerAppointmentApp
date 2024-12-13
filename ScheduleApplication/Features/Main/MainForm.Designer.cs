using System.Drawing;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Main
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.navigationTreeView = new System.Windows.Forms.TreeView();
            this.contentPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();

            // splitContainer1
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1MinSize = 200;  // Minimum width for navigation panel

            // splitContainer1.Panel1 (Navigation Panel)
            this.splitContainer1.Panel1.Controls.Add(this.navigationTreeView);
            this.splitContainer1.Panel1.Padding = new Padding(5);

            // splitContainer1.Panel2 (Content Panel)
            this.splitContainer1.Panel2.Controls.Add(this.contentPanel);
            this.splitContainer1.Panel2.Padding = new Padding(5);

            this.splitContainer1.Size = new System.Drawing.Size(1024, 768);  // Larger default size
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 0;

            // navigationTreeView
            this.navigationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationTreeView.Location = new System.Drawing.Point(5, 5);
            this.navigationTreeView.Name = "navigationTreeView";
            this.navigationTreeView.ShowLines = true;  // Shows tree lines
            this.navigationTreeView.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.navigationTreeView.TabIndex = 0;

            // contentPanel
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(5, 5);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.TabIndex = 0;

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);  // Larger default size
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new Size(800, 600);  // Set minimum form size
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Appointment Management System";

            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView navigationTreeView;
        private System.Windows.Forms.Panel contentPanel;
    }
}