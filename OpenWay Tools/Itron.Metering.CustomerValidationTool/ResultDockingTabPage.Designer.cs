namespace Itron.Metering.CustomerValidationTool
{
    partial class ResultDockingTabPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultDockingTabPage));
            this.ReasonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ResultsFlexGrid = new C1.Win.C1FlexGrid.C1FlexGrid();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsFlexGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ResultDockingTabPage
            // 
            // 
            // ResultsFlexGrid
            // 
            this.ResultsFlexGrid.AllowEditing = false;
            this.ResultsFlexGrid.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Free;
            this.ResultsFlexGrid.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Free;
            this.ResultsFlexGrid.ColumnInfo = resources.GetString("ResultsFlexGrid.ColumnInfo");
            this.ResultsFlexGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsFlexGrid.ExtendLastCol = true;
            this.ResultsFlexGrid.Location = new System.Drawing.Point(0, 0);
            this.ResultsFlexGrid.Name = "ResultsFlexGrid";
            this.ResultsFlexGrid.Rows.Count = 1;
            this.ResultsFlexGrid.Rows.DefaultSize = 17;
            this.ResultsFlexGrid.ScrollOptions = C1.Win.C1FlexGrid.ScrollFlags.AlwaysVisible;
            this.ResultsFlexGrid.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
            this.ResultsFlexGrid.Size = new System.Drawing.Size(0, 0);
            this.ResultsFlexGrid.StyleInfo = resources.GetString("ResultsFlexGrid.StyleInfo");
            this.ResultsFlexGrid.TabIndex = 0;
            this.ResultsFlexGrid.Tree.Column = 0;
            this.Controls.Add(this.ResultsFlexGrid);
            ((System.ComponentModel.ISupportInitialize)(this.ResultsFlexGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ReasonTooltip;
        private C1.Win.C1FlexGrid.C1FlexGrid ResultsFlexGrid;
    }
}
