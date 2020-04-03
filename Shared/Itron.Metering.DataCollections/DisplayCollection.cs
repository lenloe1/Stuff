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
//                              Copyright © 2007 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using System.Globalization;
using Itron.Metering.Display;
using Itron.Metering.Utilities;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Collection of meter displays that can be indexed like an array
    /// </summary>
    public class DisplayCollection : CollectionBase
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public DisplayCollection( ref DisplayDBConnection DBConnection )
        {
			Refresh( ref DBConnection );
        }//end DisplayCollection

        /// <summary>
        /// Returns the display with the given name or null if the display does not
        /// exist in the collection.
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/18/07 RDB         N/A	   Created 
        public Display.Display Find(string strName)
        {
            Display.Display match = null;

            foreach (Display.Display disp in InnerList)
            {
                if (disp.Name == strName)
                {
                    match = disp;
                    break;
                }
            }

            return match;
        }//end Find

        /// <summary>
        /// Refresh the collection of displays
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created
		public void Refresh(ref DisplayDBConnection DBConnection)
        {
            //Clears the collection
            InnerList.Clear();

            string strQueryString =
                "SELECT RECORDID, DISPLAYNAME, DISPLAYTYPE, DESCRIP, DATEMODIFIED FROM DISPLAYS;";

			OleDbConnection Connection = DBConnection.Open();

			OleDbCommand Command = Connection.CreateCommand();
			Command.CommandText = strQueryString;

            // Query Database
            OleDbDataReader Reader = Command.ExecuteReader();

            while (Reader.Read())
            {
                Display.Display objDisplay = new Display.Display();

                objDisplay.ID = Convert.ToInt32(Reader[0].ToString(), CultureInfo.CurrentCulture);
                objDisplay.Name = Reader[1].ToString();

				// Since the 16 bit and 32 bit display editors have used different display type strings
				// to identify where a display can be used, convert the display type name into an
				// equivalent device type name
                objDisplay.Type = ConvertDisplayTypeToMeterType(Reader[2].ToString());

                objDisplay.Description = Reader[3].ToString();
                objDisplay.Modified = Convert.ToDateTime(Reader[4].ToString(), CultureInfo.CurrentCulture);

                InnerList.Add(objDisplay);
            }

            Reader.Close();

            //sort displays
            DisplayComparer comparer = new DisplayComparer();
            InnerList.Sort(comparer);

        }//end Refresh

        #endregion

        #region Public Properties

        /// <summary>
        /// Allows the collection to be indexed like an array
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created
        public Display.Display this[int index]
        {
            get
            {
                return (Display.Display)InnerList[index];
            }
        }//end

        #endregion

		/// <summary>
		/// This method translates a display type name into a common device type name
		/// </summary>
		/// <param name="strDisplayType" type="string">
		/// </param>
		/// <returns>
		/// A string that identifies the device type for which the given display was created
		/// </returns>
		/// <remarks >
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 08/28/07 MAH         N/A	   Created
		/// </remarks>
		private String ConvertDisplayTypeToMeterType(String strDisplayType)
		{
			String strMeterType = "GENERIC";

            String strTempDisplayType = strDisplayType.ToUpper(CultureInfo.InvariantCulture);

			if ( strTempDisplayType.IndexOf( "VEC", StringComparison.Ordinal ) >= 0 )
			{ 
				strMeterType = DeviceType.GetDeviceTypeString( DeviceType.eDeviceTypes.VECTRON );
			}
			else if (strTempDisplayType.IndexOf("CEN", StringComparison.Ordinal) >= 0)
			{ 
				strMeterType = DeviceType.GetDeviceTypeString( DeviceType.eDeviceTypes.CENTRON );
			}
			else if ((strTempDisplayType.IndexOf("200", StringComparison.Ordinal) >= 0) || (strTempDisplayType.IndexOf("MT2", StringComparison.Ordinal)  >= 0) )
			{ 
				strMeterType = DeviceType.GetDeviceTypeString( DeviceType.eDeviceTypes.TWO_HUNDRED_SERIES );
			}
			else if ((strTempDisplayType.IndexOf("FUL", StringComparison.Ordinal) >= 0) || (strTempDisplayType.IndexOf("X20", StringComparison.Ordinal) >= 0))
			{ 
				strMeterType = DeviceType.GetDeviceTypeString( DeviceType.eDeviceTypes.FULCRUM );
			}
			else if (strTempDisplayType.IndexOf("Q", StringComparison.Ordinal) >= 0 )
			{ 
				strMeterType = DeviceType.GetDeviceTypeString( DeviceType.eDeviceTypes.QUANTUM) ;
			}

			return strMeterType;
		}

    }//end DisplayCollection

    /// <summary>
    /// This class is needed to sort the displays by name
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/14/07 RDB         N/A	   Created 
    public class DisplayComparer : IComparer
    {

        /// <summary>
        /// Calls CaseInsensitiveComparer.Compare
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created	
        int IComparer.Compare(object x, object y)
        {
            Display.Display xDisp = (Display.Display)x;
            Display.Display yDisp = (Display.Display)y;

            if (xDisp == null && yDisp == null)
            {
                return 0;
            }
            else if (xDisp == null && yDisp != null)
            {
                return -1;
            }
            else if (xDisp != null && yDisp == null)
            {
                return 1;
            }
            else
            {
                return xDisp.Name.CompareTo(yDisp.Name);
            }
        }//end Compare



    }//end DisplayComparer

}
