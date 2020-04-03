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
using System.IO;
using System.Collections.Generic;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// Represents and TOU Schedule File, has its ID, Name, and FilePath. 
	/// This will be used to create a TOUSchedule.
	/// </summary>
	public class CTOUScheduleFile
	{
		/// <summary>
		/// Represents the ID of a TOU Schedule File
		/// </summary>
		private int m_intTOUID;

		/// <summary>
		/// Represents the Name of a TOU Schedule File
		/// </summary>
		private string m_strTOUName;

		/// <summary>
		/// Represents the FilePath of a TOU Schedule File
		/// </summary>
		private string m_strFilePath;

		/// <summary>
		/// Constructor to create an instance of the TOU Schedule File class
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFile sched = new CTOUScheduleFile(2, "TOU Schedule", 
		///										"C:\\Documents\\This.xml");
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CTOUScheduleFile(int intID, string strName, string strFilePath)
		{
			m_intTOUID = intID;
			m_strTOUName = strName;
			m_strFilePath = strFilePath;
		}//CTOUSchedule

		/// <summary>
		/// Property to get the ID of a TOU Schedule
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFile sched = new CTOUScheduleFile(2, "TOU Schedule", 
		///										"C:\\Documents\\This.xml");
		/// int intTOUID = sched.ID;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int ID
		{
			get
			{
				return m_intTOUID;
			}
		}//ID


		/// <summary>
		/// Property to get the name of a TOU Schedule
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFile sched = new CTOUScheduleFile(2, "TOU Schedule", 
		///										"C:\\Documents\\This.xml");
		/// string strTOUName = sched.Name;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public string Name
		{
			get
			{
				return m_strTOUName;
			}
		}//Name


		/// <summary>
		/// Property to get the full file path of a TOU Schedule
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFile sched = new CTOUScheduleFile(2, "TOU Schedule", 
		///										"C:\\Documents\\This.xml");
		/// string strFilePath = sched.FilePath;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public string FilePath
		{
			get
			{
				return m_strFilePath;
			}
		}//FilePath

		/// <summary>
		/// This method searchs the list of given TOU files for a specific TOU name.  If that
		/// schedule name is found, the index of the item will be returned
		/// </summary>
		/// <param name="strTOUName" type="string"></param>
		/// <param name="lstTOUs"></param>
		/// <param name="nListIndex"></param>
		/// <returns>
		///  A flag indicating whether or not the given display name was found in the list of items
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  08/03/07 MAH		Created
		///
		/// </remarks>
		static public Boolean FindIndexOf(String strTOUName,
						ref List<CTOUScheduleFile> lstTOUs, out int nListIndex)
		{
			nListIndex = 0;
			Boolean boolMatchFound = false;

			// Now search this collection one by one
			while (nListIndex < lstTOUs.Count && !boolMatchFound)
			{
				CTOUScheduleFile touSchedule = lstTOUs[nListIndex];

				if (touSchedule.Name == strTOUName)
				{
					boolMatchFound = true;
				}
				else
				{
					nListIndex++;
				}
			}

			return boolMatchFound;
		}

	}
}
