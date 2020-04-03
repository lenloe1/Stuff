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
//                              Copyright © 2006 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Form used for adding or editing a Field-Pro menu title
	/// </summary>
	public partial class frmFieldProAddMenu : Form
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bAddMenu">True if is this for adding a new menu, false if
        /// this is for editing an existing menu.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/01/06 mrj 8.00.00		Created
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        public frmFieldProAddMenu(bool bAddMenu)
		{
			InitializeComponent();

			if (bAddMenu)
			{
				this.Text = "Add Menu";
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the menu's title
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/01/06 mrj 8.00.00		Created
		//
		public string MenuTitle
		{
			get
			{
				return txtMenuTitle.Text;
			}
			set
			{
				txtMenuTitle.Text = value;
			}
		}

		#endregion		

		#region Private Methods

		/// <summary>
		/// Validate that there is at least one character before allowing the OK
		/// to be enabled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  12/11/06 mrj 8.00.00		Created
		//
		private void txtMenuTitle_TextChanged(object sender, EventArgs e)
		{
			if (txtMenuTitle.Text.Length > 0)
			{
				btnOK.Enabled = true;
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		#endregion
	}
}