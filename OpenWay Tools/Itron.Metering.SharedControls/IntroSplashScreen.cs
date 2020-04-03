using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
	/// <summary>
	///     
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	public partial class IntroSplashScreen : Form
	{
        /// <summary>
        /// Constructor - retrieves and initializes the splash screen with the current
        /// application version and copyright information.
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  03/20/09 jrf 9.60.01 125531 Modifying copyright year to extract the year the 
        ///                              the app was created to so we don't have to 
        ///                              keep changing this date each year.
        ///  
        /// </remarks>
		public IntroSplashScreen()
		{
			InitializeComponent();

			lblProductName.Text = Application.ProductName;

            DateTime buildDate = new System.IO.FileInfo(Application.ExecutablePath).CreationTime;
            
            lblCopyright.Text = "Copyright " + buildDate.Year.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " Itron Inc.";
		}
	}
}