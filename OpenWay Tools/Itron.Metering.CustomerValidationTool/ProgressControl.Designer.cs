namespace Itron.Metering.CustomerValidationTool
{
    partial class ProgressControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ControlLayoutPanel = new Itron.Metering.SharedControls.DoubleBufferedTableLayoutPanel();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblResults = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblPassed = new System.Windows.Forms.Label();
            this.lblPassedCount = new System.Windows.Forms.Label();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.lblFailed = new System.Windows.Forms.Label();
            this.lblFailedCount = new System.Windows.Forms.Label();
            this.lblSkipped = new System.Windows.Forms.Label();
            this.lblSkippedCount = new System.Windows.Forms.Label();
            this.ControlLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlLayoutPanel
            // 
            this.ControlLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ControlLayoutPanel.ColumnCount = 7;
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.ControlLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ControlLayoutPanel.Controls.Add(this.ProgressBar, 0, 1);
            this.ControlLayoutPanel.Controls.Add(this.lblStatus, 0, 2);
            this.ControlLayoutPanel.Controls.Add(this.lblResults, 0, 3);
            this.ControlLayoutPanel.Controls.Add(this.lblTitle, 0, 0);
            this.ControlLayoutPanel.Controls.Add(this.lblPassed, 1, 4);
            this.ControlLayoutPanel.Controls.Add(this.lblPassedCount, 2, 4);
            this.ControlLayoutPanel.Controls.Add(this.btnViewDetails, 2, 3);
            this.ControlLayoutPanel.Controls.Add(this.lblFailed, 3, 4);
            this.ControlLayoutPanel.Controls.Add(this.lblFailedCount, 4, 4);
            this.ControlLayoutPanel.Controls.Add(this.lblSkipped, 5, 4);
            this.ControlLayoutPanel.Controls.Add(this.lblSkippedCount, 6, 4);
            this.ControlLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ControlLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ControlLayoutPanel.Name = "ControlLayoutPanel";
            this.ControlLayoutPanel.RowCount = 6;
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.ControlLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ControlLayoutPanel.Size = new System.Drawing.Size(230, 95);
            this.ControlLayoutPanel.TabIndex = 0;
            // 
            // ProgressBar
            // 
            this.ControlLayoutPanel.SetColumnSpan(this.ProgressBar, 7);
            this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressBar.Location = new System.Drawing.Point(3, 18);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(224, 14);
            this.ProgressBar.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.ControlLayoutPanel.SetColumnSpan(this.lblStatus, 7);
            this.lblStatus.Location = new System.Drawing.Point(3, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // lblResults
            // 
            this.lblResults.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblResults.AutoSize = true;
            this.ControlLayoutPanel.SetColumnSpan(this.lblResults, 2);
            this.lblResults.Location = new System.Drawing.Point(3, 58);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(45, 13);
            this.lblResults.TabIndex = 3;
            this.lblResults.Text = "Results:";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.ControlLayoutPanel.SetColumnSpan(this.lblTitle, 6);
            this.lblTitle.Location = new System.Drawing.Point(3, 1);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Title";
            // 
            // lblPassed
            // 
            this.lblPassed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPassed.AutoSize = true;
            this.lblPassed.Location = new System.Drawing.Point(13, 81);
            this.lblPassed.Name = "lblPassed";
            this.lblPassed.Size = new System.Drawing.Size(54, 13);
            this.lblPassed.TabIndex = 4;
            this.lblPassed.Text = "No Errors:";
            // 
            // lblPassedCount
            // 
            this.lblPassedCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPassedCount.AutoSize = true;
            this.lblPassedCount.Location = new System.Drawing.Point(73, 81);
            this.lblPassedCount.Name = "lblPassedCount";
            this.lblPassedCount.Size = new System.Drawing.Size(13, 13);
            this.lblPassedCount.TabIndex = 6;
            this.lblPassedCount.Text = "--";
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ControlLayoutPanel.SetColumnSpan(this.btnViewDetails, 5);
            this.btnViewDetails.Location = new System.Drawing.Point(100, 53);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(100, 24);
            this.btnViewDetails.TabIndex = 8;
            this.btnViewDetails.Text = "View Details >>";
            this.btnViewDetails.UseVisualStyleBackColor = true;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // lblFailed
            // 
            this.lblFailed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblFailed.AutoSize = true;
            this.lblFailed.Location = new System.Drawing.Point(95, 81);
            this.lblFailed.Name = "lblFailed";
            this.lblFailed.Size = new System.Drawing.Size(37, 13);
            this.lblFailed.TabIndex = 5;
            this.lblFailed.Text = "Errors:";
            // 
            // lblFailedCount
            // 
            this.lblFailedCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFailedCount.AutoSize = true;
            this.lblFailedCount.Location = new System.Drawing.Point(138, 81);
            this.lblFailedCount.Name = "lblFailedCount";
            this.lblFailedCount.Size = new System.Drawing.Size(13, 13);
            this.lblFailedCount.TabIndex = 7;
            this.lblFailedCount.Text = "--";
            // 
            // lblSkipped
            // 
            this.lblSkipped.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSkipped.AutoSize = true;
            this.lblSkipped.Location = new System.Drawing.Point(158, 81);
            this.lblSkipped.Name = "lblSkipped";
            this.lblSkipped.Size = new System.Drawing.Size(49, 13);
            this.lblSkipped.TabIndex = 9;
            this.lblSkipped.Text = "Skipped:";
            // 
            // lblSkippedCount
            // 
            this.lblSkippedCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSkippedCount.AutoSize = true;
            this.lblSkippedCount.Location = new System.Drawing.Point(213, 81);
            this.lblSkippedCount.Name = "lblSkippedCount";
            this.lblSkippedCount.Size = new System.Drawing.Size(13, 13);
            this.lblSkippedCount.TabIndex = 10;
            this.lblSkippedCount.Text = "--";
            // 
            // ProgressControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ControlLayoutPanel);
            this.DoubleBuffered = true;
            this.Name = "ProgressControl";
            this.Size = new System.Drawing.Size(230, 95);
            this.ControlLayoutPanel.ResumeLayout(false);
            this.ControlLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Itron.Metering.SharedControls.DoubleBufferedTableLayoutPanel ControlLayoutPanel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.Label lblPassed;
        private System.Windows.Forms.Label lblFailed;
        private System.Windows.Forms.Label lblPassedCount;
        private System.Windows.Forms.Label lblFailedCount;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Label lblSkipped;
        private System.Windows.Forms.Label lblSkippedCount;
    }
}
