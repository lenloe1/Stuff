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
using System.Collections;
using System.IO;
using System.Xml;
using Itron.Metering.Utilities;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// This class represents the collection of TOU Schedules in the TOU
	/// schedule directory.  This is not a read only collection so programmers
	/// be careful with the collection.
	/// </summary>
	/// <example>
	/// <code>
	/// CTOUScheduleFileCollection coll = new CTOUScheduleFileCollection();
	/// foreach(CTOUScheduleFile sched in coll)
	///	{
	///		CTOUSchedule mySched = new CTOUSchedule(sched.FilePath);
	///	}
	///	</code>
	/// </example>
	public class CTOUScheduleFileCollection : CollectionBase
	{
		/// <summary>
		/// Variable to represent the billing schedule directory
		/// </summary>
		private string m_strDataDirectory;

		/// <summary>
		/// Name of the registry key which identifies the TOU file locations
		/// </summary>
		public const string CALENDAR_EDITOR = "Calendar Editor";

		//Private constant variable
        private const string CENTRON_POLY_DEVICE = "CENTRON (V&&I)";
        private const string CENTRON_POLY_DEVICE_OBSOLETE = "CENTRON (V&I)";
        private const string SENTINEL_ADVANCED = "SENTINEL - Advanced";
        private const string SENTINEL_BASIC = "SENTINEL - Basic";
		private const string VECTRON = "VECTRON";
		private const string CENTRON = "CENTRON";
		private const string FULCRUM = "FULCRUM";
		private const string QUANTUM = "QUANTUM";
		private const string MT200 = "200 Series";

		/// <summary>
		/// Constructor for TOU Schedule File Collection class.  Gets the TOU 
		/// Schedule directory and then adds each of the schedules to the collection.
		/// The directory will be found using the RegistryAccessCE (for Windows CE)
		/// or the RegistryAccess (for Windows Desktop) class and the 
		/// GetDataDirectory method.  If the directory is not found an Exception
		/// will be thrown to say the directory does not exist.
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFileCollection coll = new CTOUScheduleFileCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class
		/// 10/16/06 mrj 7.35.05        Sort the TOU schedule collection
		///   
		public CTOUScheduleFileCollection()
		{
			Refresh(null);

			try
			{   
				//Sort the collection of TOU schedules
				base.InnerList.Sort( new TOUScheduleComparer() );
			}
			catch
			{
				//If sorting fails then we want to just continue with the unsorted
				//collection
			}

		}//CTOUScheduleFileCollection()


		/// <summary>
		/// Constructor for TOU Schedule File Collection class.  Gets the TOU 
		/// Schedule directory and then adds each of the schedules that supports the 
		/// given device name to the collection.
		/// The directory will be found using the RegistryAccessCE (for Windows CE)
		/// or the RegistryAccess (for Windows Desktop) class and the 
		/// GetDataDirectory method.  If the directory is not found and Exception
		/// will be thrown to say the directory does not exist.  If there are problems
		/// with loading or reading the xml document then an XmlException will be thrown
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFileCollection coll = new CTOUScheduleFileCollection("CENTRON");
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class
        /// 10/11/06 mrj 7.35.04 52     Sort the TOU schedule collection
        /// 10/12/06 mcm 7.35.04 59,66  Support both versions of the CENTRON Poly string
        /// 
        public CTOUScheduleFileCollection(string strDevice)
		{
			Refresh(strDevice);

            
            try
            {   
                //Sort the collection of TOU schedules
                base.InnerList.Sort( new TOUScheduleComparer() );
            }
            catch
            {
                //If sorting fails then we want to just continue with the unsorted
                //collection
            }
            
		}//CTOUScheduleFileCollection(string)


		/// <summary>
		/// Used to refresh the collection of TOU Schedules
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  02/15/06 rrr N/A	 N/A	Creation of class  
		//	04/10/07 mrj 8.00.27 2860	If this is an SCS device then check that
		//								any SCS device is supported.
		//
		private void Refresh(string strDevice)
		{
			//Local variables
			XmlDocument xmldomSchedule = null;
			XmlNodeList xmlnodelistDevices = null;
			CTOUScheduleFile objSchedule = null;
			DirectoryInfo objDir = null;
			bool blnCorrectID = false;
			string strTOUID = null;
			int intTOUID = 0;
			string strName = null;
			string strFilePath = null;


            //Get the directory for the TOU Schdule files
			m_strDataDirectory = CRegistryHelper.GetDataDirectory(CALENDAR_EDITOR);


            //Throw an error if the directory comes back null
			if(null == m_strDataDirectory)
			{
				throw new Exception("The directory for TOU Schedules does not exist");
			}

			//Create a directory info object based on the directory
			objDir = new DirectoryInfo(m_strDataDirectory);

			//Go through the list of xml files in the directory
			foreach (FileInfo objFile in objDir.GetFiles("*.xml")) 
			{
				blnCorrectID = true;

				//Check the first 4 characters of the name to make sure they are digits
				strTOUID = objFile.Name.Substring(0,4);

				for(int intCount = 0; intCount < 4; intCount++)
				{
					if(!Char.IsDigit(strTOUID,intCount))
					{
						blnCorrectID = false;
					}
				}
				//Check that there is a least one char after the TOU ID
				if(!(strTOUID.Length < (objFile.Name.Length - 
					objFile.Extension.Length)))
				{
					blnCorrectID = false;
				}

				//if the collection needs to be filtered
				if(null != strDevice)
				{
					blnCorrectID = false;

					//Create a new XmlDocument and load the file
					xmldomSchedule = new XmlDocument();
					xmldomSchedule.Load(objFile.FullName);

					//Get the SupportedDevice nodes from the xml document and check for the
					//given device name
					xmlnodelistDevices = xmldomSchedule.GetElementsByTagName(
						"SupportedDevice");
					for(int intCount = 0; intCount < xmlnodelistDevices.Count && 
						blnCorrectID == false; intCount++)
					{
						if(strDevice == xmlnodelistDevices[intCount].InnerText)
						{
							blnCorrectID = true;
						}
                        else if (strDevice == SENTINEL_ADVANCED)						
                        {
                            // If the device has reported itself as a SENTINEL advanced schedule, then it should support the 
                            //  SENTINEL - Basic as well.
                            if (SENTINEL_BASIC == xmlnodelistDevices[intCount].InnerText)
                            {
                                blnCorrectID = true;
                            }
                        }
						else if (((CENTRON_POLY_DEVICE == strDevice) ||
								  (CENTRON_POLY_DEVICE_OBSOLETE == strDevice)) &&
								 ((CENTRON_POLY_DEVICE == xmlnodelistDevices[intCount].InnerText) ||
								  (CENTRON_POLY_DEVICE_OBSOLETE == xmlnodelistDevices[intCount].InnerText)))
						{
							// Depending on the version of the msxml DOM object the file was
							// saved with, the CENTRON (V&I) string can have 2 formats. 
							// Support both
							blnCorrectID = true;
						}																		
						else if (strDevice == VECTRON ||
							     strDevice == CENTRON ||
								 strDevice == FULCRUM ||
								 strDevice == QUANTUM ||
								 strDevice == MT200)
						{
							//This is an SCS device so make sure any of these are supported
							if (xmlnodelistDevices[intCount].InnerText == VECTRON ||
								xmlnodelistDevices[intCount].InnerText == CENTRON ||
								xmlnodelistDevices[intCount].InnerText == FULCRUM ||
								xmlnodelistDevices[intCount].InnerText == QUANTUM ||
								xmlnodelistDevices[intCount].InnerText == MT200)
							{
								//The device type is an SCS device and at least one of the
								//SCS devices are supported so we can continue
								blnCorrectID = true;
							}
						}						
					}
				}

				//If the first 4 chars are digits then get the info for the 
				//CTOUScheduleFile class
				if(blnCorrectID)
				{
					intTOUID = int.Parse(strTOUID);
					strName = objFile.Name.Substring(4,objFile.Name.Length-4);
					strName = strName.Remove(strName.Length - objFile.Extension.Length,
						objFile.Extension.Length);
					strFilePath = objFile.FullName;

					//Create a new CTOUScheduleFile from the File info object
					objSchedule = new CTOUScheduleFile(intTOUID, strName, strFilePath);

					//Add the CTOUScheduleFile to the collection
					InnerList.Add( objSchedule );
				}
			}
		}//Refresh()


		/// <summary>
		/// Gets an index of CTOUScheduleFileCollection.  Allows the user 
		/// to index the collection in the same manner as an array
		/// </summary>
		/// <example>
		/// <code>
		/// CTOUScheduleFileCollection coll = new CTOUScheduleFileCollection();
		/// CTOUScheduleFile sched = coll[0];
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CTOUScheduleFile this[int intIndex]
		{
			get
			{
				return (CTOUScheduleFile) InnerList[intIndex];
			}
		}//this[]

        /// <summary>
        /// Deletes the provided file from the computer.
        /// </summary>
        /// <param name="strPath">
        /// Represents the file path of the schedule to be deleted.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/16/07 ach 8.0     90     Added to delete TOU Schedule Files
        public void Delete(String strPath)
        {
            // Remove the schedule file from this instance of the file collection
            foreach(CTOUScheduleFile file in InnerList)
            {
                if (file.FilePath == strPath)
                {
                    InnerList.Remove(strPath);
                }
            }

            // Delete the schedule file from the computer
            File.Delete(strPath);

        } // Delete(string)

		/// <summary>
		/// Returns a TOU schedule file based on the tou schedule name provided.
		/// </summary>
		/// <param name="strTOUName">
		/// The name of the TOU schedule to find.
		/// </param>
		/// <returns>
		/// Returns a CTOUScheduleFile if the schedule exists or it returns null.
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/17/07 mrj 9.00.00		Created
		//  
		public CTOUScheduleFile Find(string strTOUName)
		{
			CTOUScheduleFile TOUFile = null;

			foreach (CTOUScheduleFile file in InnerList)
			{
				if (file.Name == strTOUName)
				{
					//The TOU file was found to break out of the loop and return it
					TOUFile = file;
					break;
				}				
			}

			return TOUFile;
		}

        /// <summary>
        /// Returns a TOU schedule file based on the tou schedule name provided.
        /// </summary>
        /// <param name="iTOUID">
        /// The ID of the TOU schedule to find.
        /// </param>
        /// <returns>
        /// Returns a CTOUScheduleFile if the schedule exists or it returns null.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/17/07 mrj 9.00.00		Created
        //  
        public CTOUScheduleFile Find(int iTOUID)
        {
            CTOUScheduleFile TOUFile = null;

            foreach (CTOUScheduleFile file in InnerList)
            {
                if (file.ID == iTOUID)
                {
                    //The TOU file was found to break out of the loop and return it
                    TOUFile = file;
                    break;
                }
            }

            return TOUFile;
        }

		/// <summary>
		/// Returns a TOU schedule based on the tou schedule name provided.
		/// </summary>
		/// <param name="strTOUName">
		/// The name of the TOU schedule to find.
		/// </param>
		/// <returns>
		/// Returns a CTOUSchedule if the schedule exists or it returns null.
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/17/07 mrj 9.00.00		Created
		//  
		public CTOUSchedule FindSchedule(string strTOUName)
		{
			CTOUSchedule TOUSchedule = null;
			CTOUScheduleFile TOUFile = Find(strTOUName);

			if (TOUFile != null)
			{
				try
				{
					TOUSchedule = new CTOUSchedule(TOUFile.FilePath);
				}
				catch
				{
					TOUSchedule = null;
				}				
			}
			
			return TOUSchedule;
		}

        /// <summary>
        /// Returns a TOU schedule based on the tou schedule name provided.
        /// </summary>
        /// <param name="iTOUID">
        /// The ID of the TOU schedule to find.
        /// </param>
        /// <returns>
        /// Returns a CTOUSchedule if the schedule exists or it returns null.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/17/07 mrj 9.00.00		Created
        //  
        public CTOUSchedule FindSchedule(int iTOUID)
        {
            CTOUSchedule TOUSchedule = null;
            CTOUScheduleFile TOUFile = Find(iTOUID);

            if (TOUFile != null)
            {
                try
                {
                    TOUSchedule = new CTOUSchedule(TOUFile.FilePath);
                }
                catch
                {
                    TOUSchedule = null;
                }
            }

            return TOUSchedule;
        }
	}
        
    /// <summary>
    /// This compare class is used to sort the TOU schedule collection in the
    /// CTOUScheduleFileCollection class.
    /// </summary>
    public class TOUScheduleComparer : IComparer 
    {
        /// <summary>
        /// constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/11/06 mrj 7.35.04        Created
        //
        public TOUScheduleComparer() : base() 
        { 
        } 

        /// <summary>
        /// Implements the compare method for comparing CTOUScheduleFile objects. It
        /// compares the TOU schedule names only.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/11/06 mrj 7.35.04        Created
        //
        int IComparer.Compare(object x, object y) 
        {
            CTOUScheduleFile scheduleX = (CTOUScheduleFile) x;
            CTOUScheduleFile scheduleY = (CTOUScheduleFile) y;

            if (scheduleX == null && scheduleY == null) 
            {
                return 0;
            }
            else if (scheduleX == null && scheduleY != null) 
            {
                return -1;
            }
            else if (scheduleX != null && scheduleY == null) 
            {
                return 1;
            }
            else 
            {
                return scheduleX.Name.CompareTo(scheduleY.Name);
            }
        }
    }    
}
