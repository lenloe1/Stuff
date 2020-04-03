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
using System.Diagnostics;
using System.Text;

namespace Itron.Metering.Utilities
{
	/// <summary>
	/// This interface is to be used by any user control that exposes generic 
	/// commands to the user.  The interface exposes methods to execute a command
	/// and a method to determine if the command has been completed
	/// </summary>
	/// <remarks>
	///  Revision History	
	///  MM/DD/YY Who Version Issue# Description
	///  -------- --- ------- ------ -------------------------------------------
	///  02/18/08 MAH                 Created
	///
	/// </remarks>
	public interface ICommandProcessor
	{
		/// <summary>
		/// This event is fired by a data view whenever the view kicks off a sub process or 
		/// application that needs to be managed by the calling application.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  02/18/08 MAH                 Created
		///
		/// </remarks>
		event ProcessStartedHandler ExternalProcessStartedEvent;

		/// <summary>
		///     
		/// </summary>
		/// <param name="strUserCommand" type="string">
		/// </param>
		void ExecuteCommand(String strUserCommand);
       
		/// <summary>
		/// This method is used to determine if the current view can be closed.  Typically
		/// a view cannot be closed if processing is ongoing or if data has not been 
		/// persistently store or if data cannot be validated 
		/// </summary>
		/// <returns>
		/// True - if the view can be closed 
		/// False - if the view should not be closed
		/// </returns>
		Boolean CanClose();
	}

	/// <summary>
	/// This class represents the data that is passed with a ProcessStarted event.   The
	/// primary data item in the class is the NewProcess property which represents the 
	/// process that was just started and needs to be managed
	/// </summary>
	/// <remarks>
	///  Revision History	
	///  MM/DD/YY Who Version Issue# Description
	///  -------- --- ------- ------ -------------------------------------------
	///  07/19/07 MAH                 Created
	///  02/18/08 MAH				  Moved to utilities namespace
	///
	/// </remarks>
	public class ProcessStartedEventArgs : EventArgs
	{
		private Process m_NewProcess;

		/// <summary>
		///     
		/// </summary>
		/// <param name="newProcess" type="System.Diagnostics.Process">
		/// </param>
		public ProcessStartedEventArgs(Process newProcess)
		{
			m_NewProcess = newProcess;
		}

		/// <summary>
		/// This read only property identifies the process object that was just started
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		public Process NewProcess
		{
			get
			{
				return m_NewProcess;
			}
		}
	}//end ProcessStartedEventArgs

	/// <summary>
	///     
	/// </summary>
	/// <param name="sender" type="object">
	/// </param>
	/// <param name="e" type="Itron.Metering.SharedControls.ProcessStartedEventArgs">
	/// </param>
	/// <remarks>
	///     
	/// </remarks>
	public delegate void ProcessStartedHandler(object sender, ProcessStartedEventArgs e);

}
