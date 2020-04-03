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
//                              Copyright © 2004 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
	/// <summary>
	/// Summary description for CProgressIndicator.
	/// </summary>
	///<remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///08/26/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CProgressIndicator : System.Windows.Forms.Form
	{
		/// <summary>
		/// Contains whether or not the user canceled the operation.
		/// </summary>
		protected bool m_blnCanceled;
		/// <summary>
		/// The CProgressIndicator member variable for the title
		/// </summary>
		protected string m_strTitle = "";
		/// <summary>
		/// CProgressIndicator protected member variable
		/// </summary>
		protected System.Windows.Forms.ProgressBar m_pbrProgress;
		/// <summary>
		/// CProgressIndicator protected member variable
		/// </summary>
		protected System.Windows.Forms.Button m_butCancel;
		/// <summary>
		/// CProgressIndicator protected member variable
		/// </summary>
		protected System.Windows.Forms.Label m_lblCurrentAction;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Contructor
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CProgressIndicator( Form frmOwner )
		{
			try
			{
				// Required for Windows Form Designer support
				InitializeComponent();

				if( null != frmOwner )
				{
					this.Owner = frmOwner;
				}
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		///<remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected override void Dispose( bool disposing )
		{
			try
			{
				if( disposing )
				{
					if(components != null)
					{
						components.Dispose();
					}
				}
				base.Dispose( disposing );
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_pbrProgress = new System.Windows.Forms.ProgressBar();
			this.m_butCancel = new System.Windows.Forms.Button();
			this.m_lblCurrentAction = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_pbrProgress
			// 
			this.m_pbrProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_pbrProgress.Location = new System.Drawing.Point(0, 129);
			this.m_pbrProgress.Name = "m_pbrProgress";
			this.m_pbrProgress.Size = new System.Drawing.Size(642, 23);
			this.m_pbrProgress.TabIndex = 0;
			// 
			// m_butCancel
			// 
			this.m_butCancel.Location = new System.Drawing.Point(284, 88);
			this.m_butCancel.Name = "m_butCancel";
			this.m_butCancel.TabIndex = 1;
			this.m_butCancel.Text = "&Cancel";
			this.m_butCancel.Visible = false;
			this.m_butCancel.Click += new System.EventHandler(this.m_butCancel_Click);
			// 
			// m_lblCurrentAction
			// 
			this.m_lblCurrentAction.Location = new System.Drawing.Point(21, 24);
			this.m_lblCurrentAction.Name = "m_lblCurrentAction";
			this.m_lblCurrentAction.Size = new System.Drawing.Size(600, 48);
			this.m_lblCurrentAction.TabIndex = 2;
			this.m_lblCurrentAction.Text = "label1";
			this.m_lblCurrentAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CProgressIndicator
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(642, 152);
			this.ControlBox = false;
			this.Controls.Add(this.m_lblCurrentAction);
			this.Controls.Add(this.m_butCancel);
			this.Controls.Add(this.m_pbrProgress);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.KeyPreview = true;
			this.Name = "CProgressIndicator";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CProgressIndicator";
			this.Activated += new System.EventHandler(this.CProgressIndicator_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Whether or not the user canceled the operation.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public bool Canceled
		{
			get
			{
				return m_blnCanceled;
			}
			set
			{
				m_blnCanceled = value;
			}
		}

		/// <summary>
		/// Sets the text for the title of the Progress Indicator.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void SetTitle(string strTitle)
		{
			try
			{
				m_strTitle = strTitle;
				Text = strTitle + " ( " + ( (int) ( ( (float)this.m_pbrProgress.Value / (float)( m_pbrProgress.Maximum - m_pbrProgress.Minimum ) ) * 100 ) ).ToString() + "% )";
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Sets the label text that appears in the middle of the Progress Indicator Form.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void SetCurrentActionText(string strCurrentAction)
		{
			try
			{
				m_lblCurrentAction.Text = strCurrentAction;
				Refresh();
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Shows or hides the Cancel button based upon the blnEnabledButton parameter.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void ShowCancelButton(bool blnShowButton)
		{
			try
			{
				m_butCancel.Visible = blnShowButton;
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Enables or disables the Cancel button based upon the blnEnableButton parameter.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void EnableCancelButton(bool blnEnableButton)
		{
			try
			{
				m_butCancel.Enabled = blnEnableButton;
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Sets the min and max progress. If the min and max are set successfully then the method will return true otherwise false will be returned.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SetProgressRange(int intMinProgress, int intMaxProgress)
		{
			bool blnReturn = false;
			
			try
			{
				if( intMinProgress < intMaxProgress )
				{
					if( -1 < intMinProgress )
					{
						m_pbrProgress.Minimum = intMinProgress;
						m_pbrProgress.Maximum = intMaxProgress;
						blnReturn = true;
					}
				}

				Refresh();
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}

			return blnReturn;
		}

		/// <summary>
		/// Sets the current progress of the Progress Bar. Updates the percent complete in the title.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void SetProgress(int intProgress)
		{
			try
			{
				if( intProgress < m_pbrProgress.Minimum )
				{
					m_pbrProgress.Value = m_pbrProgress.Minimum;
				}
				else
				{
					if( intProgress > m_pbrProgress.Maximum )
					{
						m_pbrProgress.Value = m_pbrProgress.Maximum;
					}
					else
					{
						m_pbrProgress.Value = intProgress;
					}
				}
				
				SetTitle( m_strTitle );
				Refresh();
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Increments the progress bar by one. Updates the percent complete in the title.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void IncrementProgress()
		{
			try
			{
				m_pbrProgress.Increment( 1 );
				SetTitle( m_strTitle );
				Refresh();
			}
			catch( Exception e )
			{
				MessageBox.Show( e.Message );
			}
		}

		/// <summary>
		/// Catches the click event for the Cancel button
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/26/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		private void m_butCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				m_blnCanceled = true;
			}
			catch( Exception exception )
			{
				MessageBox.Show( exception.Message );
			}
		}

		/// <summary>
		/// Shows the owner when activated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///09/17/04 REM 7.00.18		   Adding support for showing owner on activate
		///11/03/04 REM 7.00.27 793    A box appears in the progress indicator of System Manager
		///</pre></remarks>
		private void CProgressIndicator_Activated(object sender, System.EventArgs e)
		{
			try
			{
				//REM 11/03/04: We should refresh the dialog so that it gets repainted properly
				Refresh();
				if( null != this.Owner )
				{
					Owner.Show();
				}
			}
			catch( Exception exception )
			{
				MessageBox.Show( exception.ToString() );
			}
		}
	}
}
