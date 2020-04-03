///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                              Copyright © 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Itron.Metering.SharedControls;
using Itron.Metering.CustomerValidationTool.Properties;

namespace Itron.Metering.CustomerValidationTool
{
	/// <summary>
	///  This form shows the current version number of the data manager as well as copyright
	/// information and allows the execution of Microsoft's System Info application to allow
	/// application engineers access to system configuration information for debugging
	/// </summary>
	public partial class AboutForm : C1.Win.C1Ribbon.C1RibbonForm
	{
		/// <summary>
		/// Primary constructor - retrieves and initializes the user interface with the current
		/// application version information
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  01/21/08 MAH 10.0.0 Now uses application name from the application's properties
		///                                  rather than hard coding the text
        ///  03/20/09 jrf 9.60.01 125531 Modifying copyright year to extract the year the 
        ///                              the app was created to so we don't have to 
        ///                              keep changing this date each year.
		///  
		/// </remarks>
		public AboutForm()
		{
			InitializeComponent();

			string strVersion;

#if FINALRELEASE
            // Remove the build number from the Product Version before placing it on the 
            // about box.
            int iBuildStart = Application.ProductVersion.LastIndexOf('.');

            if (iBuildStart > 0)
            {
                strVersion = Application.ProductVersion.Substring(0, iBuildStart);
            }
            else
            {
                // This should never happen but just in case we should display the whole version
                strVersion = Application.ProductVersion;
            }
#else
			// For non final release builds keep the build number to help with bugs
			strVersion = Application.ProductVersion;
#endif

			this.Text = "About " + Application.ProductName;
			lblApplicationName.Text = Application.ProductName;

			label2.Text = "Version " + strVersion;

            DateTime buildDate = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).CreationTime;

            label3.Text = "Copyright © " + buildDate.Year.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                + " Itron Inc.  All Rights Reserved.  Unpublished Work.";
		} // About_Form

		/// <summary>
		/// This event handler invokes Microsoft's system information application    
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ ---------------------------------------------
		///  01/21/08 MAH 10.0.0 Added exception handling - just in case...
		///  
		/// </remarks>
		private void butSystem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("msinfo32.exe");
			}
			catch (System.Exception err)
			{
				ErrorForm.DisplayError(Resources.ERROR_COMMAND_EXECUTION, err);
			}
		} // butSystem_Click

	}
}