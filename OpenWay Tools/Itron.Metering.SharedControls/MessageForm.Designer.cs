namespace Itron.Metering.SharedControls
{
	partial class MessageForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
			this.btnOK = new System.Windows.Forms.Button();
			this.pictureWarningIcon = new System.Windows.Forms.PictureBox();
			this.lblMessage = new System.Windows.Forms.Label();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureWarningIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(419, 24);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(74, 22);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// pictureWarningIcon
			// 
			this.pictureWarningIcon.Image = ((System.Drawing.Image)(resources.GetObject("pictureWarningIcon.Image")));
			this.pictureWarningIcon.Location = new System.Drawing.Point(22, 19);
			this.pictureWarningIcon.Name = "pictureWarningIcon";
			this.pictureWarningIcon.Size = new System.Drawing.Size(32, 32);
			this.pictureWarningIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureWarningIcon.TabIndex = 8;
			this.pictureWarningIcon.TabStop = false;
			// 
			// lblMessage
			// 
			this.lblMessage.Location = new System.Drawing.Point(65, 12);
			this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(346, 47);
			this.lblMessage.TabIndex = 7;
			this.lblMessage.Text = "label1";
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Information.png");
			// 
			// MessageForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(514, 71);
			this.Controls.Add(this.pictureWarningIcon);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MessageForm";
			((System.ComponentModel.ISupportInitialize)(this.pictureWarningIcon)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.PictureBox pictureWarningIcon;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.ImageList imageList1;
	}
}