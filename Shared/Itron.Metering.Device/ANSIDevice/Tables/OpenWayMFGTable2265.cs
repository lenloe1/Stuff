///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class that describes the mfg table 217 object
    /// </summary>
    public class MFGTable2265CTEConfig : ANSISubTable
    {
        #region Constants

        private const ushort CTE_CONFIG_TBL_OFFSET = 315;
        private const ushort CTE_CONFIG_TBL_SIZE = 6;

        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //
        public MFGTable2265CTEConfig(CPSEM psem)
            : base(psem, 2265, CTE_CONFIG_TBL_OFFSET, CTE_CONFIG_TBL_SIZE)
        {
        }

        /// <summary>
        /// Constructor used to get Event Data from the EDL file
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/25/13 AF  2.80.23 TR7590 Created
        //
        public MFGTable2265CTEConfig(PSEMBinaryReader reader)
            : base(2265, CTE_CONFIG_TBL_SIZE)
        {
            m_Reader = reader;
            ParseCTEConfig();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the current per phase threshold config out of Mfg table 217
        /// </summary>
        /// <returns>PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors)</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "CurrentThresholdExceededConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                ParseCTEConfig();
            }

            return Result;
        }

        /// <summary>
        ///  Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                   "Dump of CurrentThresholdExceededConfig Table");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CTE Enable = " + m_CTEEnable.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CTE Threshold (%) = " + m_CTEThreshold.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CTE Hysteresis (%) = " + m_CTEHysteresis.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CTE Debounce (sec) = " + m_CTEDebounce.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CTE Min Active Duration (sec) = " + m_CTEMinActiveDuration.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                try
                {
                    m_Logger.WriteException(this, e);
                }
                catch
                {
                    // No exceptions thrown from this debugging method
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the current threshold exceeded enable field from table 2265, true to enable, false to disable. Set
        /// is for automated testing only.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //  04/08/13 MP  2.80.24        added set
        //
        public bool CTEEnable
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                bool bEnable = false;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Enable"));
                    }
                }

                // If this part of mfg table 217 is not supported, we will be reading the
                // "Unused" field which is all 0xFF
                if ((m_CTEEnable != 0) && (m_CTEEnable != 0xFF))
                {
                    bEnable = true;
                }

                return bEnable;
            }

            set // AUTOMATED TESTING ONLY
            {
                ReadUnloadedTable();
                m_SubTableOffset = CTE_CONFIG_TBL_OFFSET;
                m_Size = CTE_CONFIG_TBL_SIZE;

                // Set the value of the table.
                if (value)
                {
                    // This will enable CTE. Will need to write some other values to get the CTE enabled.
                    m_CTEEnable = (byte)(m_CTEEnable | 0x01); // 0x01 is enabled
                }
                else
                {
                    // This will disable CTE
                    m_CTEEnable = (byte)(m_CTEEnable & 0x00); // 0x00 is disabled
                }

                m_DataStream.Position = 0;
                m_Writer.Write(m_CTEEnable);

                base.Write(0,1);
            }
        }

        /// <summary>
        /// Gets the current threshold exceeded enable field from table 2265 and
        /// determines whether or not it has been configured.  CTE can be configured
        /// but not enabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/24/13 AF  2.80.23 TR7590 Created
        //
        public bool CTEConfigured
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                bool bConfigured = false;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Enable"));
                    }
                }

                // If this part of mfg table 217 is not configured, we will be reading the
                // "Unused" field which is all 0xFF
                if (m_CTEEnable != 0xFF)
                {
                    bConfigured = true;
                }

                return bConfigured;
            }
        }

        /// <summary>
        /// Gets or sets the current threshold exceeded threshold field from table 2265
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //  05/08/13 MP  2.80.24        added set. For automated Testing only
        //
        public byte CTEThreshold
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Threshold"));
                    }
                }

                return m_CTEThreshold;
            }
            set // AUTOMATED TESTING ONLY
            {
                ReadUnloadedTable();
                m_SubTableOffset = CTE_CONFIG_TBL_OFFSET;
                m_Size = CTE_CONFIG_TBL_SIZE;

                // Set the value of the table.
                if (value > 125 || value < 4)
                {
                    // Do nothing. Value entered is invalid.
                }
                else
                {
                    m_CTEThreshold = (byte)(m_CTEThreshold & 0x00);
                    m_CTEThreshold = (byte)(m_CTEThreshold | value);
                }

                m_DataStream.Position = 1;
                m_Writer.Write(m_CTEThreshold);

                base.Write(1, 1);
            }
        }

        /// <summary>
        /// Gets or sets the current threshold exceeded hysteresis field from table 2265
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //  05/08/13 MP  2.80.24        added set. For automated Testing only
        //
        public byte CTEHysteresis
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Hysteresis"));
                    }
                }

                return m_CTEHysteresis;
            }
            set // AUTOMATED TESTING ONLY
            {
                ReadUnloadedTable();
                m_SubTableOffset = CTE_CONFIG_TBL_OFFSET;
                m_Size = CTE_CONFIG_TBL_SIZE;

                // Set the value of the table.
                if ((int)value > 125 || (int)value < 4)
                {
                    // Do nothing. Value entered is invalid.
                }
                else
                {
                    m_CTEHysteresis = (byte)(m_CTEHysteresis & 0x00);
                    m_CTEHysteresis = (byte)(m_CTEHysteresis | value);
                }

                m_DataStream.Position = 2;
                m_Writer.Write(m_CTEHysteresis);

                base.Write(2, 1);
            }
        }

        /// <summary>
        /// Gets or sets the Current Threshold Exceeded Debounce field from table 2265
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //  05/08/13 MP  2.80.24        added set. For automated Testing only
        //
        public byte CTEDebounce
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Debounce"));
                    }
                }

                return m_CTEDebounce;
            }
            set // AUTOMATED TESTING ONLY
            {
                ReadUnloadedTable();
                m_SubTableOffset = CTE_CONFIG_TBL_OFFSET;
                m_Size = CTE_CONFIG_TBL_SIZE;

                // Set the value of the table.
                if (value > 100 || value < 0)
                {
                    // Do nothing. Value entered is invalid.
                }
                else
                {
                    m_CTEDebounce = (byte)(m_CTEDebounce & 0x00);
                    m_CTEDebounce = (byte)(m_CTEDebounce | value);
                }

                m_DataStream.Position = 3;
                m_Writer.Write(m_CTEDebounce);

                base.Write(3, 1);
            }
        }

        /// <summary>
        /// Gets or sets the Current Threshold Exceeded minimum active duration from table 2265
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/08/13 AF  2.80.07 TR7590 Created
        //  05/08/13 MP  2.80.24        added set. For automated Testing only
        //
        public UInt16 CTEMinActiveDuration
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading CTE Min Active Duration"));
                    }
                }

                return m_CTEMinActiveDuration;
            }
            set // AUTOMATED TESTING ONLY
            {
                ReadUnloadedTable();
                m_SubTableOffset = CTE_CONFIG_TBL_OFFSET;
                m_Size = CTE_CONFIG_TBL_SIZE;

                // Set the value of the table.
                if (value > 43200 || value < 0)
                {
                    // Do nothing. Value entered is invalid.
                }
                else
                {
                    m_CTEMinActiveDuration = value;
                }

                m_DataStream.Position = 4;
                m_Writer.Write(m_CTEMinActiveDuration);

                base.Write(4, 2);
            }
        }

        #endregion

        /// <summary>
        /// Parses the CTE data out of the stream.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/26/13 AF  2.80.23 TR7590 Created
        //  05/31/13 AF  2.80.36 TR7590 CE dlls don't support reading the CTE reserved byte,
        //                              so removed it.
        //
        private void ParseCTEConfig()
        {
            //Populate the member variables that represent the table
            m_CTEEnable = m_Reader.ReadByte();
            m_CTEThreshold = m_Reader.ReadByte();
            m_CTEHysteresis = m_Reader.ReadByte();
            m_CTEDebounce = m_Reader.ReadByte();
            m_CTEMinActiveDuration = m_Reader.ReadUInt16();
        }

        #region Members

        private byte m_CTEEnable;
        private byte m_CTEThreshold;
        private byte m_CTEHysteresis;
        private byte m_CTEDebounce;
        private UInt16 m_CTEMinActiveDuration;

        #endregion

    }

    /// <summary>
    /// Class for reading through the Extended Self Read configuration
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#    Description
    //  -------- --- ------- ------    -------------------------------------------
    //  08/02/16 MP  4.70.11 WR701234  Created
    //
    public class MFGTable2265ExtendedSelfReadConfig : ANSISubTable
    {
        #region Constants

        private const ushort ESR_TBL_OFFSET = 232;
        private const ushort ESR_CONFIG_TBL_SIZE = 80;

        #endregion

        #region Members

        private List<SR2InstQuantity> m_ConfigList;

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem"></param>
        public MFGTable2265ExtendedSelfReadConfig(CPSEM psem)
            : base(psem, 2265, ESR_TBL_OFFSET, ESR_CONFIG_TBL_SIZE)
        {
            m_ConfigList = new List<SR2InstQuantity>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"></param>
        public MFGTable2265ExtendedSelfReadConfig(PSEMBinaryReader reader)
            : base(2265, ESR_CONFIG_TBL_SIZE)
        {
            m_ConfigList = new List<SR2InstQuantity>();
            m_Reader = reader;
            ParseESRConfig();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Read sub table
        /// </summary>
        /// <returns></returns>
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "ESRConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                ParseESRConfig();
            }

            return Result;
        }

        #endregion

        #region Private Methods

        private void ParseESRConfig()
        {
            SR2InstQuantity ConfigItem = null;
            for (int i = 0; i < 16; i++)
            {
                ConfigItem = new SR2InstQuantity()
                {
                    LID = new LID(m_Reader.ReadUInt32()),
                    Qualifier = m_Reader.ReadByte()
                };

                m_ConfigList.Add(ConfigItem);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Publicly accessible ConfigList
        /// </summary>
        public List<SR2InstQuantity> ESRConfiguration { get { return m_ConfigList; } }

        #endregion
    }

    /// <summary>
    /// Object for SR2 configuration
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue#    Description
    //  -------- --- ------- ------    -------------------------------------------
    //  08/02/16 MP  4.70.11 WR701234  Created
    //
    public class SR2InstQuantity
    {
        #region Members

        private LID m_LID;
        private byte m_Qualifier;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LID"></param>
        /// <param name="Qualifier"></param>
        public SR2InstQuantity(uint LID, byte Qualifier)
        {
            m_LID = new LID(LID);
            m_Qualifier = Qualifier;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SR2InstQuantity()
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Publicly accessible LID
        /// </summary>
        public LID LID
        {
            get
            {
                return m_LID;
            }
            set
            {
                m_LID = value;
            }
        }

        /// <summary>
        /// Publicly accessible Qualifier
        /// </summary>
        public byte Qualifier
        {
            get
            {
                return m_Qualifier;
            }
            set
            {
                m_Qualifier = value;
            }
        }
        #endregion
    }

}
