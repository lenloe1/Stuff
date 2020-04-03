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
//                              Copyright © 2006 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.IO;
using System.Globalization;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the SENTINEL. (IERTCheck implementation)
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 05/22/06 mrj 7.30.00 N/A    Created
    /// 11/11/13 AF  3.50.02         Changed the parent class from CANSIDevice to ANSIMeter
    ///
    public partial class SENTINEL : ANSIMeter, ISiteScan, IERTCheck
    {
        #region Properties
        /// <summary>
        /// Gets a boolean indicating whether the device is configured
        /// for RF or not
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/24/06 mrj 7.30.00 N/A    Created
        ///
        bool IERTCheck.RFConfigured
        {
            get
            {
                byte[] Data = null;
                bool bConfigured = false;


                //Get the configured option board
				if (null != Table2048.OptionBoardConfig)
				{
					ushort usConfiguredOptBrd = Table2048.OptionBoardConfig.OptionBoardID;

					if (R300_1_ERT == usConfiguredOptBrd ||
						R300_2_ERT == usConfiguredOptBrd ||
						R300_3_ERT == usConfiguredOptBrd)
					{
						//The meter is configured for R300, now read the LID to get the actual
						//actual option board
						if (PSEMResponse.Ok == m_lidRetriever.RetrieveLID(m_LID.OPT_BRD_ID, out Data))
						{
							usConfiguredOptBrd = Data[0];
							MemoryStream TempStream = new MemoryStream(Data);
							BinaryReader TempBReader = new BinaryReader(TempStream);

							usConfiguredOptBrd = TempBReader.ReadUInt16();

							if (R300_1_ERT == usConfiguredOptBrd ||
								R300_2_ERT == usConfiguredOptBrd ||
								R300_3_ERT == usConfiguredOptBrd)
							{
								//The configuration says R300 and the LID says R300
								//so return true.
								bConfigured = true;
							}
						}
					}
				}
				
                return bConfigured;
            }
        }

        /// <summary>
        /// Implements the IERTCheck interface.  Returns a ERTConfig object containing
        /// all of the ERT configuration in the Sentinel.
        /// </summary>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mrj 7.30.00 N/A    Created
        ///
        ERTConfig[] IERTCheck.ERTCheck
        {
            get
            {
                R300Config R300ConfigTable;
                ERTConfig[] ertConfig;
                R300Config.R300_ERT_Config[] R300ErtConfig;
                byte[] Data = null;
                LID[] LIDArray = new LID[3];
                string strERTId;

								
                //Read the option board ID out of the configuration
                ushort usConfiguredOptBrd = Table2048.OptionBoardConfig.OptionBoardID;

                if (R300_1_ERT == usConfiguredOptBrd)
                {
                    ertConfig = new ERTConfig[1];
                    R300ConfigTable = new R300Config(m_PSEM,
                        (ushort)(Table2048.Table2048Header.OptionBoardOffset +
                        OptionBoardHeader.OPTION_BOARD_HEADER_LENGTH));
                }
                else if (R300_2_ERT == usConfiguredOptBrd)
                {
                    ertConfig = new ERTConfig[2];
                    R300ConfigTable = new R300Config(m_PSEM,
                        (ushort)(Table2048.Table2048Header.OptionBoardOffset +
                        OptionBoardHeader.OPTION_BOARD_HEADER_LENGTH));
                }
                else if (R300_3_ERT == usConfiguredOptBrd)
                {
                    ertConfig = new ERTConfig[3];
                    R300ConfigTable = new R300Config(m_PSEM,
                        (ushort)(Table2048.Table2048Header.OptionBoardOffset +
                        OptionBoardHeader.OPTION_BOARD_HEADER_LENGTH));
                }
                else
                {
                    throw new Exception("R300 not configured.");
                }

                //Read the ERT IDs out of the meter
                LIDArray[0] = m_LID.DISP_VENDOR_FIELD_1;
                LIDArray[1] = m_LID.DISP_VENDOR_FIELD_2;
                LIDArray[2] = m_LID.DISP_VENDOR_FIELD_3;


                if (PSEMResponse.Ok == m_lidRetriever.RetrieveMulitpleLIDs(LIDArray, out Data))
                {
                    MemoryStream TempStream = new MemoryStream(Data);
                    PSEMBinaryReader TempBReader = new PSEMBinaryReader(TempStream);

                    //Set the ERT IDs to the structure
                    for (int iIndex = 0; iIndex < ertConfig.Length; iIndex++)
                    {
                        strERTId = TempBReader.ReadString(10);
                        ertConfig[iIndex].m_iERTID = Convert.ToInt32(strERTId, CultureInfo.InvariantCulture);
                    }
                }


                //Get the R300 ERT configuration from 2048
                R300ErtConfig = R300ConfigTable.R300ERTConfig;


                //Convert the configuration
                for (int iIndex = 0; iIndex < ertConfig.Length; iIndex++)
                {
                    //Convert the LID to a kilo quantity string
                    LID lidERTConfig = new LID(R300ErtConfig[iIndex].uiLid, LID.MeasurementUnit.KILO);
                    ertConfig[iIndex].m_strQty = lidERTConfig.lidDescription;

                    if (OPT_DATE_MDY == R300ErtConfig[iIndex].byLidDataType ||
                        OPT_DATE_DMY == R300ErtConfig[iIndex].byLidDataType ||
                        OPT_DATE_YMD == R300ErtConfig[iIndex].byLidDataType)
                    {
                        //Date type so return 0 digits
                        ertConfig[iIndex].m_bytTotalDigits = 0;
                        ertConfig[iIndex].m_bytDecDigits = 0;
                    }
                    else
                    {
                        //Get the digits
                        ertConfig[iIndex].m_bytTotalDigits = (byte)((R300ErtConfig[iIndex].byLidDisplayFormat & R300_TOTAL_FORMAT_MASK) >> 4);
                        ertConfig[iIndex].m_bytDecDigits = (byte)(R300ErtConfig[iIndex].byLidDisplayFormat & R300_DECIMAL_FORMAT_MASK);
                    }
                }
				                
                return ertConfig;
            }
        }
        #endregion
    }
}