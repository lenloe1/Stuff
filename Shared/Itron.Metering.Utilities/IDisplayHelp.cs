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
using System.Text;

namespace Itron.Metering.Utilities
{
	/// <summary>
	///     
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	public interface IDisplayHelp
	{
		/// <summary>
		/// Displays the appropriate help topic
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//
		//
		void DisplayHelpTopic();

		/// <summary>
		/// This property identifies the help topic that will be displayed when the help button on the application window 
		/// is clicked.
		/// </summary>
		string HelpTopic
		{
			get;
			set;
		}

		/// <summary>
		/// This property identifies the help file that will be used
		/// </summary>
		string HelpFilePath
		{
			get;
			set;
		}


	}
}
