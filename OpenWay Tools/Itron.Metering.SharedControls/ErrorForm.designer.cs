namespace Itron.Metering.SharedControls
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.IconList = new System.Windows.Forms.ImageList(this.components);
            this.pictureErrorIcon = new System.Windows.Forms.PictureBox();
            this.pictureWarningIcon = new System.Windows.Forms.PictureBox();
            this.DetailFlexGrid = new C1.Win.C1FlexGrid.C1FlexGrid();
            ((System.ComponentModel.ISupportInitialize)(this.pictureErrorIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWarningIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailFlexGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(464, 16);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(74, 29);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(464, 52);
            this.btnDetails.Margin = new System.Windows.Forms.Padding(4);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(74, 29);
            this.btnDetails.TabIndex = 1;
            this.btnDetails.Text = "Details >>";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(51, 16);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(405, 103);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "label1";
            // 
            // IconList
            // 
            this.IconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconList.ImageStream")));
            this.IconList.TransparentColor = System.Drawing.Color.Transparent;
            this.IconList.Images.SetKeyName(0, "error1.jpg");
            this.IconList.Images.SetKeyName(1, "warning.ico");
            this.IconList.Images.SetKeyName(2, "error.ico");
            // 
            // pictureErrorIcon
            // 
            this.pictureErrorIcon.Image = ((System.Drawing.Image)(resources.GetObject("pictureErrorIcon.Image")));
            this.pictureErrorIcon.Location = new System.Drawing.Point(12, 16);
            this.pictureErrorIcon.Name = "pictureErrorIcon";
            this.pictureErrorIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureErrorIcon.TabIndex = 5;
            this.pictureErrorIcon.TabStop = false;
            // 
            // pictureWarningIcon
            // 
            this.pictureWarningIcon.Image = ((System.Drawing.Image)(resources.GetObject("pictureWarningIcon.Image")));
            this.pictureWarningIcon.Location = new System.Drawing.Point(12, 16);
            this.pictureWarningIcon.Name = "pictureWarningIcon";
            this.pictureWarningIcon.Size = new System.Drawing.Size(32, 32);
            this.pictureWarningIcon.TabIndex = 6;
            this.pictureWarningIcon.TabStop = false;
            // 
            // DetailFlexGrid
            // 
            this.DetailFlexGrid.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.DetailFlexGrid.AllowEditing = false;
            this.DetailFlexGrid.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Both;
            this.DetailFlexGrid.ColumnInfo = resources.GetString("DetailFlexGrid.ColumnInfo");
            this.DetailFlexGrid.ExtendLastCol = true;
            this.DetailFlexGrid.Location = new System.Drawing.Point(12, 122);
            this.DetailFlexGrid.Name = "DetailFlexGrid";
            this.DetailFlexGrid.Rows.Count = 1;
            this.DetailFlexGrid.Rows.DefaultSize = 18;
            this.DetailFlexGrid.Size = new System.Drawing.Size(525, 240);
            this.DetailFlexGrid.StyleInfo = resources.GetString("DetailFlexGrid.StyleInfo");
            this.DetailFlexGrid.TabIndex = 7;
            this.DetailFlexGrid.VisualStyle = C1.Win.C1FlexGrid.VisualStyle.Office2007Black;
            // 
            // ErrorForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(549, 374);
            this.Controls.Add(this.DetailFlexGrid);
            this.Controls.Add(this.pictureWarningIcon);
            this.Controls.Add(this.pictureErrorIcon);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            ((System.ComponentModel.ISupportInitialize)(this.pictureErrorIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureWarningIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DetailFlexGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ImageList IconList;
        private System.Windows.Forms.PictureBox pictureErrorIcon;
        private System.Windows.Forms.PictureBox pictureWarningIcon;
        private C1.Win.C1FlexGrid.C1FlexGrid DetailFlexGrid;
    }
}