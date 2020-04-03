namespace Itron.Metering.SharedControls
{
    partial class FlexGridView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  10/03/07 MAH  9.00.14               Added dispose of the directory watcher object
		///
		/// </remarks>
		protected override void Dispose(bool disposing)
        {
			if (disposing && (m_DirectoryWatcher != null ))
			{
				m_DirectoryWatcher.Dispose();
			}

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
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Microsoft Excel Worksheet (*.xls) |*.xls";
			this.saveFileDialog.Title = "Export Current View to Excel";
			// 
			// DataManagerView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Name = "DataManagerView";
			this.Size = new System.Drawing.Size(394, 294);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SaveFileDialog saveFileDialog;

	}
}
