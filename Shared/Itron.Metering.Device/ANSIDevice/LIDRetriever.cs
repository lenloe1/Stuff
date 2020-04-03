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
//                              Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class is used to get a value from a meter using a LID, via
    /// tables 2049 and 2050.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 05/24/06 mrj 7.30.00 N/A	Created
    ///
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class LIDRetriever
    {
        #region Constants

        private const ushort LID_REQUEST_TABLE = 2049;
        private const ushort LID_RETRIEVE_TABLE = 2050;
        private const int LID_REQUEST_MAX_SIZE = 34;
        internal const int MAX_LIDS_PER_READ = 8;

        #endregion

        #region Definitions

        //Enumeration for setting the request mode for LIDs
        internal enum RequestMode
        {
            DataOnly = 0,
            LIDAndData = 1,
            LIDAndDataByBlock = 2,
            LIDOnly = 3,
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for communication with the meter.</param>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 mrj 7.30.00 N/A	Created
        /// 
        public LIDRetriever(CPSEM psem)
        {
            m_PSEM = psem;
            m_DataReader = null;
        }

        /// <summary>
        /// Retrieve LID
        /// </summary>
        /// <param name="lid">The Lid to Retrieve</param>
        /// <param name="objValue">an object containing the data associated with the LID</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //
        public PSEMResponse RetrieveLID(LID lid, out object objValue)
        {
            return RetrieveLID(lid, RequestMode.DataOnly, out objValue);
        }
        /// <summary>
        /// Retrieve LID
        /// </summary>
        /// <param name="lid">The LID to retrieve</param>
        /// <param name="eMode">The request mode</param>
        /// <param name="objValue">an object containing the data associated with the LID</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //
        public PSEMResponse RetrieveLID(LID lid, RequestMode eMode, out object objValue)
        {
            PSEMResponse Result;
            objValue = null;
            LID[] lids = new LID[1];
            lids[0] = lid;
            List<object> objValues;

            Result = RetrieveMulitpleLIDs(lids, eMode, out objValues);

            if (PSEMResponse.Ok == Result)
            {
                objValue = objValues[0];
            }

            return Result;
        }

        /// <summary>
        /// Retrieve Multiple LIDs
        /// </summary>
        /// <param name="lids">Array of LIDs to retrieve</param>
        /// <param name="objValues">A List of objects containing the data associated with the LIDs</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //
        public PSEMResponse RetrieveMulitpleLIDs(LID[] lids, out List<object> objValues)
        {
            return RetrieveMulitpleLIDs(lids, RequestMode.DataOnly, out objValues);
        }

        /// <summary>
        /// Retrieve Multiple LIDs
        /// </summary>
        /// <param name="lids">Array of LIDs to retrieve</param>
        /// <param name="eMode">The Request Mode</param>
        /// <param name="objValues">A List of objects containing the data associated with the LIDs</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 7.36.00 N/A    Created
        //  04/05/07 KRC 8.00.25        Adding code to handle Empty LID Types
        //
        public PSEMResponse RetrieveMulitpleLIDs(LID[] lids, RequestMode eMode, out List<object> objValues)
        {
            PSEMResponse Result;
            byte[] Data;
            object objValue;

            objValues = new List<object>();

            // We need to look through the LID List and make sure there are no invalid LIDs
            for (int iIndex = 0; iIndex < lids.GetLength(0); iIndex++)
            {
                switch (lids[iIndex].lidType)
                {
                    case TypeCode.Empty:
                    {
                        // Set the LID description to indicate the unknown item
                        lids[iIndex].lidDescription = "Unknown Item: " + lids[iIndex].lidValue.ToString(CultureInfo.InvariantCulture);

                        // Now change the LID to something we know we can get, just so we don't mess up the multiple LID Read (Num Test Intervals)
                        lids[iIndex].lidValue = (uint)DefinedLIDs.BaseLIDs.DEMAND_CONFIG + (uint)DefinedLIDs.DemandConfig_Data.CONF_TST_NBR_SUB;

                        // I want the LIDType to stay Empty, so I can use this fact later one to manipulate the data
                        lids[iIndex].lidType = TypeCode.Empty;
                        break;
                    }
                }
            }

            Result = RetrieveMulitpleLIDs(lids, eMode, out Data);

            if (PSEMResponse.Ok == Result)
            {
                for (int iIndex = 0; iIndex < lids.GetLength(0); iIndex++)
                {
                    switch (lids[iIndex].lidType)
                    {
                        case TypeCode.Byte:
                        {
                            objValue = DataReader.ReadByte();
                            break;
                        }
                        case TypeCode.UInt16:
                        {
                            objValue = DataReader.ReadUInt16();
                            break;
                        }
                        case TypeCode.UInt32:
                        {
                            objValue = DataReader.ReadUInt32();
                            break;
                        }
                        case TypeCode.Single:
                        {
                            objValue = DataReader.ReadSingle();
                            break;
                        }
                        case TypeCode.Double:
                        {
                            objValue = DataReader.ReadDouble();
                            break;
                        }
                        case TypeCode.DateTime:
                        {
                            // The LID DateTime is really a UINT32, that needs to be converted
                            objValue = DataReader.ReadUInt32();
                            break;
                        }
                        case TypeCode.String:
                        {
                            objValue = DataReader.ReadString((int)lids[iIndex].lidLength);
                            break;
                        }
                        case TypeCode.Empty:
                        {
                            // Up above we know we set all Empties to be the Number of Test SubIntervals, which is a byte.
                            // We are not going to show this value, but we need to read it so we can continue read other data.
                            byte byTemp = DataReader.ReadByte();

                            // What we want to show 0 (Only thing that can be formatted for all display types)
                            objValue = "0";
                            break;
                        }
                        default:
                        {
                            String strError = "Missing case: " + lids[iIndex].lidType.ToString();
                            objValue = strError;
                            break;
                        }
                    }
                    objValues.Add(objValue);
                }
            }
            return Result;
        }

        /// <summary>
        /// This method does the work of writing a lid to table 2049 and reading
        /// the results from table 2050.
        /// </summary>
        /// <remarks>
        /// It is up to the calling method to translate the data.  This method
        /// always sets the mode to DataOnly.
        /// 
        /// Also, it is up to the calling method to perform the PSEM wait.
        /// </remarks>
        /// <param name="lid">The requested LID</param>
        /// <param name="data">The data returned from the device</param>
        /// <returns>PSEMResponse returned from the device</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 mrj 7.30.00 N/A	Created
        /// 09/27/06 KRC 7.35.00 N/A    Add RequestMode
        /// 
        public PSEMResponse RetrieveLID(LID lid, out byte[] data)
        {
            LID[] lids = new LID[1];
            lids[0] = lid;

            return RetrieveMulitpleLIDs(lids, RequestMode.DataOnly, out data);
        }

        /// <summary>
        /// This method does the work of writing a lid to table 2049 and reading
        /// the results from table 2050.
        /// </summary>
        /// <remarks>
        /// It is up to the calling method to translate the data.  This method
        /// always sets the mode to DataOnly.
        /// 
        /// Also, it is up to the calling method to perform the PSEM wait.
        /// </remarks>
        /// <param name="lid">The requested LID</param>
        /// <param name="eMode">Request Mode</param>
        /// <param name="data">The data returned from the device</param>
        /// <returns>PSEMResponse returned from the device</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 mrj 7.30.00 N/A	Created
        /// 09/27/06 KRC 7.35.00 N/A    Add RequestMode
        /// 
        public PSEMResponse RetrieveLID(LID lid, RequestMode eMode, out byte[] data)
        {
            LID[] lids = new LID[1];
            lids[0] = lid;

            return RetrieveMulitpleLIDs(lids, eMode, out data);
        }

        /// <summary>
        /// This method does the work of writing multiple lids to table 2049 and
        /// reading the results from table 2050.  It can handle up to 8 lids.
        /// </summary>
        /// <remarks>
        /// It is up to the calling method to translate the data.  This method
        /// always sets the mode to DataOnly.
        /// 
        /// Also, it is up to the calling method to perform the PSEM wait.
        /// </remarks>
        /// <param name="lids">The requested LIDs, up to 8</param>
        /// <param name="data">The data returned from the device</param>
        /// <returns>PSEMResponse returned from the device</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 mrj 7.30.00 N/A	Created
        /// 09/27/06 KRC 7.35.00 N/A    Add RequestMode
        /// 
        public PSEMResponse RetrieveMulitpleLIDs(LID[] lids, out byte[] data)
        {
            return RetrieveMulitpleLIDs(lids, RequestMode.DataOnly, out data);
        }

        /// <summary>
        /// This method does the work of writing multiple lids to table 2049 and
        /// reading the results from table 2050.  It can handle up to 8 lids.
        /// </summary>
        /// <remarks>
        /// It is up to the calling method to translate the data.  This method
        /// always sets the mode to DataOnly.
        /// 
        /// Also, it is up to the calling method to perform the PSEM wait.
        /// </remarks>
        /// <param name="lids">The requested LIDs, up to 8</param>
        /// <param name="eMode">Request Mode</param>
        /// <param name="data">The data returned from the device</param>
        /// <returns>PSEMResponse returned from the device</returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 mrj 7.30.00 N/A	Created
        /// 09/27/06 KRC 7.35.00 N/A    Add RequestMode
        /// 10/04/06 RCG 7.40.00 N/A    Added check to see if the data is null before
        ///                             creating the BinaryReader to avoid an exception
        /// 
        public PSEMResponse RetrieveMulitpleLIDs(LID[] lids, RequestMode eMode, out byte[] data)
        {
            PSEMResponse Result = PSEMResponse.Err;
            byte[] bySendData = new byte[2 + (4 * lids.Length)];
            byte[] byReadData = null;
            int iLIDIndex = 2;

            //Build the table to send to the meter
            bySendData[0] = (byte)eMode;                  // Request Mode
            bySendData[1] = (byte)lids.Length;            // Number of LIDs

            //Copy the LIDs to the output array
            for (int iIndex = 0; iIndex < lids.Length; iIndex++)
            {
                byte[] byLID = BitConverter.GetBytes(lids[iIndex].lidValue);
                Array.Copy(byLID, 0, bySendData, iLIDIndex, 4);
                iLIDIndex = iLIDIndex + 4;
            }

            //Make the request for the LIDs 
            Result = m_PSEM.FullWrite(LID_REQUEST_TABLE, bySendData);

            //Retrieve the LIDs
            if (PSEMResponse.Ok == Result)
            {
                Result = m_PSEM.FullRead(LID_RETRIEVE_TABLE, out byReadData);
                if (PSEMResponse.Ok != Result)
                {
                    byReadData = null;
                }
            }
            
            data = byReadData;

            // Assign the data to the Binary data reader
            if (data != null)
            {
                m_DataReader = new PSEMBinaryReader(new MemoryStream(data));
            }
            else
            {
                m_DataReader = null;
            }

            return Result;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the BinaryReader
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //
        public PSEMBinaryReader DataReader
        {
            get
            {
                return m_DataReader;
            }
        }
        #endregion

        #region Members

        /// <summary>
		/// PSEM object for communication with the meter. 
		/// </summary>
		protected CPSEM m_PSEM = null;
        private PSEMBinaryReader m_DataReader;

        #endregion
	}	
}
