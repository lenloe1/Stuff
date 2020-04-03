///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
namespace Itron.Metering.ReplicaSettings
{
	partial class ctrlFieldProDynamicOptions
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        private void InitializeComponent()
		{
            this.btnDeleteMenu = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.LayoutTreeView = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddMenu = new System.Windows.Forms.Button();
            this.btnEditMenu = new System.Windows.Forms.Button();
            this.FeaturesTreeView = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDeleteMenu
            // 
            this.btnDeleteMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnDeleteMenu.Enabled = false;
            this.btnDeleteMenu.Location = new System.Drawing.Point(3, 54);
            this.btnDeleteMenu.Name = "btnDeleteMenu";
            this.btnDeleteMenu.Size = new System.Drawing.Size(88, 23);
            this.btnDeleteMenu.TabIndex = 4;
            this.btnDeleteMenu.Text = "Delete Menu";
            this.btnDeleteMenu.UseVisualStyleBackColor = true;
            this.btnDeleteMenu.Click += new System.EventHandler(this.btnDeleteMenu_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(3, 108);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 23);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "<< Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(3, 81);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(88, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add >>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.Location = new System.Drawing.Point(3, 284);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(88, 23);
            this.btnMoveUp.TabIndex = 7;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.Location = new System.Drawing.Point(3, 310);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(88, 23);
            this.btnMoveDown.TabIndex = 8;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Field-Pro Features";
            // 
            // LayoutTreeView
            // 
            this.LayoutTreeView.AllowDrop = true;
            this.LayoutTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutTreeView.HideSelection = false;
            this.LayoutTreeView.HotTracking = true;
            this.LayoutTreeView.Location = new System.Drawing.Point(340, 23);
            this.LayoutTreeView.Name = "LayoutTreeView";
            this.LayoutTreeView.Size = new System.Drawing.Size(232, 333);
            this.LayoutTreeView.TabIndex = 1;
            this.LayoutTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.LayoutTreeView_DragDrop);
            this.LayoutTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LayoutTreeView_AfterSelect);
            this.LayoutTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.LayoutTreeView_DragEnter);
            this.LayoutTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LayoutTreeView_KeyDown);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Field-Pro Screen Layout";
            // 
            // btnAddMenu
            // 
            this.btnAddMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnAddMenu.Enabled = false;
            this.btnAddMenu.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAddMenu.Location = new System.Drawing.Point(3, 0);
            this.btnAddMenu.Name = "btnAddMenu";
            this.btnAddMenu.Size = new System.Drawing.Size(88, 23);
            this.btnAddMenu.TabIndex = 2;
            this.btnAddMenu.Text = "Add Menu...";
            this.btnAddMenu.UseVisualStyleBackColor = true;
            this.btnAddMenu.Click += new System.EventHandler(this.btnAddMenu_Click);
            // 
            // btnEditMenu
            // 
            this.btnEditMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnEditMenu.Enabled = false;
            this.btnEditMenu.Location = new System.Drawing.Point(3, 27);
            this.btnEditMenu.Name = "btnEditMenu";
            this.btnEditMenu.Size = new System.Drawing.Size(88, 23);
            this.btnEditMenu.TabIndex = 3;
            this.btnEditMenu.Text = "Edit Menu...";
            this.btnEditMenu.UseVisualStyleBackColor = true;
            this.btnEditMenu.Click += new System.EventHandler(this.btnEditMenu_Click);
            // 
            // FeaturesTreeView
            // 
            this.FeaturesTreeView.AllowDrop = true;
            this.FeaturesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FeaturesTreeView.HideSelection = false;
            this.FeaturesTreeView.HotTracking = true;
            this.FeaturesTreeView.Location = new System.Drawing.Point(3, 23);
            this.FeaturesTreeView.Name = "FeaturesTreeView";
            this.FeaturesTreeView.Size = new System.Drawing.Size(231, 333);
            this.FeaturesTreeView.TabIndex = 0;
            this.FeaturesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.FeaturesTreeView_AfterSelect);
            this.FeaturesTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.FeaturesTreeView_ItemDrag);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FeaturesTreeView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LayoutTreeView, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(575, 359);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAddMenu);
            this.panel1.Controls.Add(this.btnDeleteMenu);
            this.panel1.Controls.Add(this.btnEditMenu);
            this.panel1.Controls.Add(this.btnClear);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnMoveUp);
            this.panel1.Controls.Add(this.btnMoveDown);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(240, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(94, 333);
            this.panel1.TabIndex = 9;
            // 
            // ctrlFieldProDynamicOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ctrlFieldProDynamicOptions";
            this.Size = new System.Drawing.Size(575, 359);
            this.Load += new System.EventHandler(this.ctrlFieldProDynamicOptions_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ctrlFieldProDynamicOptions_HelpRequested);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnDeleteMenu;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView LayoutTreeView;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnAddMenu;
		private System.Windows.Forms.Button btnEditMenu;
		private System.Windows.Forms.TreeView FeaturesTreeView;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;



	}
}
