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
//                              Copyright © 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
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
	/// This class provides a simple, consistent mechanism for displaying a message 
	/// to the user in a modal dialog box.  It is similar to the                    
	/// System.Forms.MessageBox class but the form properties are more consistent 
	/// with the current Itron/DataManager form styles.  The message form should 
	/// only be accessed via the static ShowMessage method.
	/// </summary>
	public partial class MessageForm : C1.Win.C1Ribbon.C1RibbonForm
	{
		/// <summary>
		/// The default constructor for the message form.  Note that it is protected and
		/// cannot be called directly from a client application.  Instead the 
		/// 'ShowMessage' method should be used to create and display the message form.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/27/08 MAH 9.50				    Created
		/// </remarks>
		protected MessageForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// This property sets the primary text displayed on the message form    
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/27/08 MAH 9.50				    Created
		/// </remarks>
		protected String MessageText
		{
			get
			{
				return lblMessage.Text;
			}
			set
			{
				lblMessage.Text = value;
			}
		}

		/// <summary>
		/// This is the primary method to be used when displaying an interactive text 
		/// message to the user.  This method should be used instead of the standard 
		/// System.Forms.MessageBox class in order to provide a more consistent user 
		/// interface to the operator
		/// </summary>
		/// <param name="strTitle" type="string">
		/// The text of the form's title.  Typically this is the application and/or 
		/// function name
		/// </param>
		/// <param name="strMessage" type="string">
		/// The text of the message to be displayed
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/27/08 MAH 9.50				    Created
		/// </remarks>
		public static void ShowMessage(String strTitle, String strMessage)
		{
			MessageForm frmMessage = new MessageForm();

			frmMessage.Text = strTitle;
			frmMessage.MessageText = strMessage;

			frmMessage.ShowDialog();
		}

	}
}
