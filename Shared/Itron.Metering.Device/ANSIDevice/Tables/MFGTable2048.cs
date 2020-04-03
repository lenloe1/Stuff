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
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using Itron.Metering.TOU;
using System.Globalization;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This CTable2048 class manages the header and config blocks of 2048.  This 
    /// is the base class and is overriden for device specific changes.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    // 
    internal abstract class CTable2048
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/22/06 mrj 7.30.00 N/A    Created
        //  10/30/06 AF  7.40.00        Added history log config init
        // 
        public CTable2048()
        {
            m_2048Header = null;
            m_Logger = Logger.TheInstance;
            m_CoefficientsConfig = null;
            m_ConstantsConfig = null;
            m_DemandConfig = null;
            m_SelfReadConfig = null;
            m_BillingSchedConfig = null;
            m_OptBoardConfig = null;
            m_TOUConfig = null;
            m_DisplayConfig = null;
            m_HistoryLogConfig = null;
        }

        /// <summary>
        /// Constructor, create the header and config blocks that will be used.
        /// It reads the header out of the meter to get the offsets.
        /// </summary>
        /// <param name="psem">Protocol instance to be used by the table</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable2048 Table2048 = new CTable2048( PSEM ); 
        /// </code></example>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public CTable2048(CPSEM psem)
        {
            m_PSEM = psem;

            m_2048Header = new CTable2048Header(psem);
            m_2048Header.Read();

            m_Logger = Logger.TheInstance;
        }

        /// <summary>
        /// Dumps the config tables to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  
        public void Dump()
        {
            try
            {
                // Read them in necessary
                if (AnsiTable.TableState.Unloaded == m_CoefficientsConfig.State)
                {
                    m_CoefficientsConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_ConstantsConfig.State)
                {
                    m_ConstantsConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_DemandConfig.State)
                {
                    m_DemandConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_SelfReadConfig.State)
                {
                    m_SelfReadConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_OptBoardConfig.State)
                {
                    m_OptBoardConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_BillingSchedConfig.State)
                {
                    m_BillingSchedConfig.Read();
                }
                if (AnsiTable.TableState.Unloaded == m_CalendarConfig.State)
                {
                    m_CalendarConfig.Read();
                }

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Dump of All Subtables in Table 2048");

                m_2048Header.Dump();
                m_CoefficientsConfig.Dump();
                m_ConstantsConfig.Dump();
                m_DemandConfig.Dump();
                m_SelfReadConfig.Dump();
                m_OptBoardConfig.Dump();
                m_BillingSchedConfig.Dump();
                m_CalendarConfig.Dump();

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "End Dump of All Subtables in Table 2048");
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

        } // Dump

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the 2048 Header
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public CTable2048Header Table2048Header
        {
            get
            {
                return m_2048Header;
            }
        }

        /// <summary>
        /// Provides access to the Coefficients config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/10/06 RCG 7.35.00 N/A    Created
        //
        public CoefficientsConfig CoefficientsConfig
        {
            get
            {
                return m_CoefficientsConfig;
            }
        }

        /// <summary>
        /// Provides access to the Constants config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public ConstantsConfig ConstantsConfig
        {
            get
            {
                return m_ConstantsConfig;
            }
        }

        /// <summary>
        /// Provides access to the Billing Schedule Config Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public BillingSchedConfig BillingSchedConfig
        {
            get
            {
                return m_BillingSchedConfig;
            }
        }

        /// <summary>
        /// Provides access to the Demand Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public DemandConfig DemandConfig
        {
            get
            {
                return m_DemandConfig;
            }
        }

        /// <summary>
        /// Provides access to the Calendar Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public CalendarConfig CalendarConfig
        {
            get
            {
                return m_CalendarConfig;
            }
        }

        /// <summary>
        /// Provides access to the TOU Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public TOUConfig TOUConfig
        {
            get
            {
                return m_TOUConfig;
            }
        }

        /// <summary>
        /// Provides access to the Display Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public DisplayConfig DisplayConfig
        {
            get
            {
                return m_DisplayConfig;
            }
        }

        /// <summary>
        /// Provides access to the History Log Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/30/06 AF  7.40.00 N/A    Created
        //
        public HistoryLogConfig HistoryLogConfig
        {
            get
            {
                return m_HistoryLogConfig;
            }
        }

        /// <summary>
        /// Provides access to the Option Board Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public OptionBoardHeader OptionBoardConfig
        {
            get
            {
                return m_OptBoardConfig;
            }
        }

        /// <summary>
        /// Provides access to the Demand Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/04/06 KRC 7.35.00 N/A    Created
        //
        public SelfReadConfig SelfReadConfig
        {
            get
            {
                return m_SelfReadConfig;
            }
        }

        /// <summary>
        /// Provides access to the Mode Control Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 08/21/07 RCG 8.10.21 N/A    Created

        public ModeControl ModeControl
        {
            get
            {
                return m_ModeControl;
            }
        }

        /// <summary>
        /// Provides access to the IO Config Table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 04/21/09 jrf 2.20.02 N/A    Created

        public IOConfig IOConfig
        {
            get
            {
                return m_IOConfig;
            }
        }

        /// <summary>
        /// Returns true if the meter is configured for DST
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 09/12/06 KRC 7.35.00 N/A    Adding Exception
        // 10/13/06 mcm 7.35.05 50     never returns false
        // 03/15/07 RCG 8.00.18 N/A    Removing redundant read code since the property reads the table
        public virtual bool HasDST
        {
            get
            {
                bool HaveDST = false;

                // If we don't have a calendar configuration 
                // (PRE_SATURN SENTINELS), we won't have DST.
                if (m_2048Header.CalendarOffset > 0)
                {
                    if (0 < m_CalendarConfig.DSTOffset)
                    {
                        HaveDST = true;
                    }
                }

                return HaveDST;
            }
        }

        /// <summary>
        /// Returns the configured TOU ID.  Value is 0 if meter wasn't configured
        /// for TOU.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        // 10/25/06 mcm 7.35.07 113    # Rates is not sufficient for determining if 
        //                             TOU was configured
        // 03/15/07 RCG 8.00.18        Removing redundant read code since the property reads the table
        public virtual ushort TOU_ID
        {
            get
            {
                ushort uintTOU_ID = 0;

                // If we don't have a calendar configuration 
                // (PRE_SATURN SENTINELS), we won't have DST.
                if (m_2048Header.CalendarOffset > 0)
                {
                        uintTOU_ID = m_CalendarConfig.CalendarID;
                }

                return uintTOU_ID;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The header block for table 2048
        /// </summary>
        protected CTable2048Header m_2048Header;
        /// <summary>
        /// The coefficients config block from table 2048.
        /// </summary>
        protected CoefficientsConfig m_CoefficientsConfig;
        /// <summary>
        /// The constants config block from table 2048, It is up to the inherited
        /// device object to create the correct constants config block
        /// </summary>
        protected ConstantsConfig m_ConstantsConfig;
        /// <summary>
        /// The demand config block from table 2048
        /// </summary>
        protected DemandConfig m_DemandConfig;
        /// <summary>
        /// The self read config block from table 2048
        /// </summary>
        protected SelfReadConfig m_SelfReadConfig;
        /// <summary>
        /// The custom schedule config block from table 2048
        /// </summary>
        protected BillingSchedConfig m_BillingSchedConfig;
        /// <summary>
        /// The option board config block header from table 2048
        /// </summary>
        protected OptionBoardHeader m_OptBoardConfig;
        /// <summary>
        /// The Calendar config from table 2048
        /// </summary>
        protected CalendarConfig m_CalendarConfig;
        /// <summary>
        /// The IO config from table 2048.
        /// </summary>
        protected IOConfig m_IOConfig;
        /// <summary>
        /// The TOU config from table 2048
        /// </summary>
        protected TOUConfig m_TOUConfig;
        /// <summary>
        /// The Display Config from 2048
        /// </summary>
        protected DisplayConfig m_DisplayConfig;
        /// <summary>
        /// The Event Config from 2048
        /// </summary>
        protected HistoryLogConfig m_HistoryLogConfig;
        /// <summary>
        /// The Mode Control table from 2048
        /// </summary>
        protected ModeControl m_ModeControl;
        /// <summary>
        /// The protocol object
        /// </summary>
        protected CPSEM m_PSEM;
        /// <summary>
        /// The debug logger
        /// </summary>
        protected Logger m_Logger;

        #endregion variable declarations

    } // CTable2048 class

    /// <summary>
    /// This CTable2048 class manages the header and config blocks of 2048.  This 
    /// is the base class and is overriden for device specific changes.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    // 
    internal abstract class CTable2048_Shared : CTable2048
    {
        #region Constants

        private const byte DEFAULT_CALENDAR_YEARS = 25;
        private const byte CAL_YEAR_SIZE = 89;
        private const byte CAL_HEADER_SIZE = 6;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/22/06 mrj 7.30.00 N/A    Created
        // 
        public CTable2048_Shared()
            : base()
        {
        }

        /// <summary>
        /// Constructor, create the header and config blocks that will be used.
        /// It reads the header out of the meter to get the offsets.
        /// </summary>
        /// <param name="psem">Protocol instance to be used by the table</param>
        /// <example><code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// CPSEM PSEM = new CPSEM(comm);
        /// PSEM.Logon("");
        /// PSEM.Security"("");
        /// CTable2048 Table2048 = new CTable2048( PSEM ); 
        /// </code></example>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public CTable2048_Shared(CPSEM psem)
            : base(psem)
        {
            //Read 2048's header to get the offsets and then create the config blocks
            m_DemandConfig = new DemandConfig(psem, m_2048Header.DemandOffset);
            m_SelfReadConfig = new SelfReadConfig(psem, m_2048Header.SelfReadOffset);

            if (0 != m_2048Header.OptionBoardOffset)
            {
                m_OptBoardConfig = new OptionBoardHeader(psem, m_2048Header.OptionBoardOffset);
            }

            m_CoefficientsConfig = new CoefficientsConfig(psem, m_2048Header.CoefficientsOffset);

            if (0 != m_2048Header.TOUOffset)
            {
                m_TOUConfig = new TOUConfig(psem, m_2048Header.TOUOffset);
            }
            else
            {
                m_TOUConfig = null;
            }
        }

        /// <summary>
        /// Handles device dependent special cases that can't be done during
        /// construction.
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public void InitializeSpecialCases()
        {
            ushort Size = (ushort)(m_MaxCalYears * CAL_YEAR_SIZE + CAL_HEADER_SIZE);

            m_CalendarConfig = new CalendarConfig(m_PSEM,
                                                  m_2048Header.CalendarOffset,
                                                  Size, m_MaxCalYears);
        }

        #endregion Public Methods

        #region Members

        /// <summary>
        /// The max calendar years supported by this device
        /// </summary>
        protected byte m_MaxCalYears = DEFAULT_CALENDAR_YEARS;

        #endregion variable declarations

    } // CTable2048_Shared class

    /// <summary>
    /// This CTable2048Header class handles the reading of the header of 2048
    ///	to get the offset information. The reading of this table in the meter
    ///	will be implicit.  (read-only)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    // 
    public class CTable2048Header : ANSISubTable
    {

        #region Constants

        /// <summary>Length of the ANSI Header Table</summary>
        public const int HEADER_LENGTH_2048 = 72;

        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">The protocol instance to use</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public CTable2048Header(CPSEM psem)
            : base(psem, 2048, 0, HEADER_LENGTH_2048)
        {
        }

        /// <summary>
        /// Header Constructor used for file based structure
        /// </summary>
        /// <param name="BinaryReader"></param>
        public CTable2048Header(PSEMBinaryReader BinaryReader)
            : base(2048, HEADER_LENGTH_2048)
        {
            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;
            PopulateMembers();
        }
        /// <summary>
        /// Reads table 2048's header out of the meter.  We only care about certain
        /// offsets.  So, the offsets that we do not want are put into filler arrays.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                PopulateMembers();
            }

            return Result;
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Dump of Table 2048 Header");
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ConfigurationSize = " + m_ConfigurationSize);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ConfigVersion = " + m_ConfigVersion);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ConfigDate = " + m_ConfigDate);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SWVersion = " + m_SWVersion);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SWRevision = " + m_SWRevision);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_CoefficientsOffset = " + m_CoefficientsOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ConstantsOffset = " + m_ConstantsOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_EnergyOffset = " + m_EnergyOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_DemandOffset = " + m_DemandOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_DisplayOffset = " + m_DisplayOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_HistoryLogOffset = " + m_HistoryLogOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_CPCOffset = " + m_CPCOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_StateMonitorOffset = " + m_StateMonitorOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_IOOffset = " + m_IOOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_LoadProfileOffset = " + m_LoadProfileOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_BatteryOffset = " + m_BatteryOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SelfReadOffset = " + m_SelfReadOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SiteScanOffset = " + m_SiteScanOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_CalendarOffset = " + m_CalendarOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_TOUOffset = " + m_TOUOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SLCOffset = " + m_SLCOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_PQOffset = " + m_PQOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ModeControlOffset = " + m_ModeControlOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_MiscOffset = " + m_MiscOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_ReservedOffset = " + m_ReservedOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_OptionBoardOffset = " + m_OptionBoardOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_BillingSchedOffset = " + m_BillingSchedOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_TotalizersOffset = " + m_TotalizersOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_Decade9Offset = " + m_Decade9Offset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_Decade0Offset = " + m_Decade0Offset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_Decade8Offset = " + m_Decade8Offset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SelfReadBuffersOffset = " + m_SelfReadBuffersOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_RFOffset = " + m_RFOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_MeterKeyOffset = " + m_MeterKeyOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_BaseDataOffset = " + m_BaseDataOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "   m_SimulatorOffset = " + m_SimulatorOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "End Dump of Table 2048 Header" + m_TableID);
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

        #endregion public methods

        #region public properties

        /// <summary>
        /// Gets the Configuration Date in 2048.  
        /// </summary>
        /// <returns>The Configuration Date from the config block</returns>
        /// /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/18/06 KRC 7.35.00 N/A    Created
        // 
        public uint DateProgrammed
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
                                "Error Reading date Programmed"));
                    }
                }

                return (uint)m_ConfigDate;
            }
        }

        /// <summary>
        /// Gets the software revision.version  
        /// </summary>
        /// /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/07 mcm 8.00.18 2514   SW Rev not supported for ANSI devices
        //  05/29/07 RCG 8.10.05 2992   SW Version was not correct because it was using
        //                              integer division when converting the revision to float.

        public float SWVerRev
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
                                "Error reading software revision"));
                    }
                }

                return (float)(m_SWVersion + (m_SWRevision / 100.0));
            }
        }

        /// <summary>
        /// Get the offset for the CPC data in 2048
        /// </summary>
        public ushort CPCOffset
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
                                "Error reading CPC Offset"));
                    }
                }

                return m_CPCOffset;
            }
        }
        /// <summary>
        /// Gets the offset for Coefficients in 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/09/06 RCG 7.40.00 N/A    Created

        public ushort CoefficientsOffset
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
                                "Error reading Coefficients Offset"));
                    }
                }

                return m_CoefficientsOffset;
            }
        }

        /// <summary>
        /// Gets the offset for Constants in 2048.  If a 0 offset is returned
        /// then the caller should know that the read failed.
        /// </summary>
        /// <returns>The offset to the constants config block</returns>
        /// /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public ushort ConstantsOffset
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
                                "Error reading Constants Offset"));
                    }
                }

                return m_ConstantsOffset;
            }
        }

        /// <summary>
        /// Gets the offset for demand in 2048.  If a 0 offset is returned
        /// then the caller should know that the read failed.
        /// </summary>
        /// <returns>The offset to the demand config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public ushort DemandOffset
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
                                "Error Reading Demand Offset"));
                    }
                }

                return m_DemandOffset;
            }
        }

        /// <summary>
        /// Gets the offset for self read in 2048.  If a 0 offset is returned
        /// then the caller should know that the read failed.
        /// </summary>
        /// <returns>The offset to the self read config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public ushort SelfReadOffset
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
                                "Error Reading Self Read Offset"));
                    }
                }

                return m_SelfReadOffset;
            }
        }

        /// <summary>
        /// Gets the offset for option board in 2048.  If a 0 offset is
        /// returned then the caller should know that the option board is not
        /// configured.
        /// </summary>
        /// <returns>The offset to the billing schedule config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public ushort OptionBoardOffset
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
                                "Error Reading Option Board Offset"));
                    }
                }

                return m_OptionBoardOffset;
            }
        }

        /// <summary>
        /// Gets the offset for IO in 2048.  If a 0 offset is
        /// returned then the caller should know that the IO is not
        /// configured.
        /// </summary>
        /// <returns>The offset to the IO config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/21/09 jrf 2.20.02 N/A    Created
        // 
        public ushort IOOffset
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
                                "Error Reading Option Board Offset"));
                    }
                }

                return m_IOOffset;
            }
        }

        /// <summary>
        /// Gets the offset for billing schedule in 2048.  If a 0 offset is
        /// returned then the caller should know that the read failed.
        /// </summary>
        /// <returns>The offset to the billing schedule config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public ushort BillingSchedOffset
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
                                "Error Reading Billing Schedule Offset"));
                    }
                }

                return m_BillingSchedOffset;
            }
        }

        /// <summary>
        /// Gets the offset for CalendarOffset in 2048.  If a 0 offset is
        /// returned then the either the read failed or this is a pre-Saturn
        /// SENTINEL that doesn't have DST or TOU configured (it might still 
        /// have a clock).
        /// </summary>
        /// <returns>The offset to the billing schedule config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/06 mcm 7.13.00 N/A    Created
        // 
        public ushort CalendarOffset
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
                                "Error Reading Calendar Offset"));
                    }
                }

                return m_CalendarOffset;
            }
        }

        /// <summary>
        /// Gets the offset for TOUOffset in 2048.  If a 0 offset is returned
        /// then the either the read failed or the meter is not configured for 
        /// TOU.
        /// </summary>
        /// <returns>The offset to the billing schedule config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/06/06 mcm 7.13.00 N/A    Created
        // 
        public ushort TOUOffset
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
                                "Error Reading TOU Offset"));
                    }
                }

                return m_TOUOffset;
            }
        }

        /// <summary>
        /// Gets the offset for DisplayOffset in 2048.  If a 0 offset is returned
        /// then the either the read failed or the meter is not configured for 
        /// Display.
        /// </summary>
        /// <returns>The offset to the Display config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        // 
        public ushort DisplayOffset
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
                                "Error Reading Display Offset"));
                    }
                }

                return m_DisplayOffset;
            }
        }

        /// <summary>
        /// History Log Offset
        /// </summary>
        public ushort HistoryLogOffset
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
                                "Error Reading History Log Offset"));
                    }
                }

                return m_HistoryLogOffset;
            }
        }

        /// <summary>
        /// Gets the offset for power quality from 2048.  If a 0 offset is
        /// returned then power quality is not configured.
        /// </summary>
        /// <returns>The offset to the power quality config block</returns>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/05 mcm 8.00.11 N/A    Created
        // 
        public ushort PQOffset
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
                                "Error Reading Billing Schedule Offset"));
                    }
                }

                return m_PQOffset;
            }
        }

        /// <summary>
        /// Gets the offset of the Mode Control sub table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/20/07 RCG 8.10.21 N/A    Created

        public ushort ModeControlOffset
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
                                "Error Reading Billing Schedule Offset"));
                    }
                }

                return m_ModeControlOffset;
            }
        }

        /// <summary>
        /// Gets the offset of the SiteScan sub table
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/09 RCG 2.20.03        Created

        public ushort SiteScanOffset
        {
            get
            {
                ReadUnloadedTable();

                return m_SiteScanOffset;
            }
        }

        /// <summary>
        /// Publicly accessible Config Version
        /// </summary>
        public ushort ConfigurationVersion { get { return m_ConfigVersion; } }

        #endregion public properties

        #region Privmate Methods

        /// <summary>
        /// Populates our member variables from the PSEMBinaryReader, which
        ///  has been populated by this time.
        /// </summary>
        private void PopulateMembers()
        {
            m_ConfigurationSize = m_Reader.ReadUInt16();
            m_ConfigVersion = m_Reader.ReadUInt16();
            m_ConfigDate = m_Reader.ReadUInt32();
            m_SWVersion = m_Reader.ReadByte();
            m_SWRevision = m_Reader.ReadByte();
            m_CoefficientsOffset = m_Reader.ReadUInt16();
            m_ConstantsOffset = m_Reader.ReadUInt16();
            m_EnergyOffset = m_Reader.ReadUInt16();
            m_DemandOffset = m_Reader.ReadUInt16();
            m_DisplayOffset = m_Reader.ReadUInt16();
            m_HistoryLogOffset = m_Reader.ReadUInt16();
            m_CPCOffset = m_Reader.ReadUInt16();
            m_StateMonitorOffset = m_Reader.ReadUInt16();
            m_IOOffset = m_Reader.ReadUInt16();
            m_LoadProfileOffset = m_Reader.ReadUInt16();
            m_BatteryOffset = m_Reader.ReadUInt16();
            m_SelfReadOffset = m_Reader.ReadUInt16();
            m_SiteScanOffset = m_Reader.ReadUInt16();
            m_CalendarOffset = m_Reader.ReadUInt16();
            m_TOUOffset = m_Reader.ReadUInt16();
            m_SLCOffset = m_Reader.ReadUInt16();
            m_PQOffset = m_Reader.ReadUInt16();
            m_ModeControlOffset = m_Reader.ReadUInt16();
            m_MiscOffset = m_Reader.ReadUInt16();
            m_ReservedOffset = m_Reader.ReadUInt16();
            m_OptionBoardOffset = m_Reader.ReadUInt16();
            m_BillingSchedOffset = m_Reader.ReadUInt16();
            m_TotalizersOffset = m_Reader.ReadUInt16();
            m_Decade9Offset = m_Reader.ReadUInt16();
            m_Decade0Offset = m_Reader.ReadUInt16();
            m_Decade8Offset = m_Reader.ReadUInt16();
            m_SelfReadBuffersOffset = m_Reader.ReadUInt16();
            m_RFOffset = m_Reader.ReadUInt16();
            m_MeterKeyOffset = m_Reader.ReadUInt16();
            m_BaseDataOffset = m_Reader.ReadUInt16();
            m_SimulatorOffset = m_Reader.ReadUInt16();
        }

        #endregion Private Methods

        #region Members

        private ushort m_ConfigurationSize = 0;
        private ushort m_ConfigVersion = 0;
        private uint m_ConfigDate = 0;
        private byte m_SWVersion = 0;
        private ushort m_SWRevision = 0;
        private ushort m_CoefficientsOffset = 0;
        private ushort m_ConstantsOffset = 0;
        private ushort m_EnergyOffset = 0;
        private ushort m_DemandOffset = 0;
        private ushort m_DisplayOffset = 0;
        private ushort m_HistoryLogOffset = 0;
        private ushort m_CPCOffset = 0;
        private ushort m_StateMonitorOffset = 0;
        private ushort m_IOOffset = 0;
        private ushort m_LoadProfileOffset = 0;
        private ushort m_BatteryOffset = 0;
        private ushort m_SelfReadOffset = 0;
        private ushort m_SiteScanOffset = 0;
        private ushort m_CalendarOffset = 0;
        private ushort m_TOUOffset = 0;
        private ushort m_SLCOffset = 0;
        private ushort m_PQOffset = 0;
        private ushort m_ModeControlOffset = 0;
        private ushort m_MiscOffset = 0;
        private ushort m_ReservedOffset = 0;
        private ushort m_OptionBoardOffset = 0;
        private ushort m_BillingSchedOffset = 0;
        private ushort m_TotalizersOffset = 0;
        private ushort m_Decade9Offset = 0;
        private ushort m_Decade0Offset = 0;
        private ushort m_Decade8Offset = 0;
        private ushort m_SelfReadBuffersOffset = 0;
        private ushort m_RFOffset = 0;
        private ushort m_MeterKeyOffset = 0;
        private ushort m_BaseDataOffset = 0;
        private ushort m_SimulatorOffset = 0;

        #endregion Members

    } // CTable2048Header class

    /// <summary>
    /// This CoefficientConfig class handles the reading/writing of the
    /// coefficients config block of 2048. The reading of this table will
    /// be implicit while the writing of this table will need to be explicitly
    /// called. (read/write)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  10/10/06 RCG 7.40.00 N/A    Created
    internal class CoefficientsConfig : ANSISubTable
    {
        #region Definitions
        private const ushort COEFF_CONFIG_TBL_SIZE = 12;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="offset">The offset of the table in 2048.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public CoefficientsConfig(CPSEM psem, ushort offset)
            : base(psem, 2048, offset, COEFF_CONFIG_TBL_SIZE)
        {
        }

        /// <summary>
        /// Reads the Coefficients Config block out of 2048
        /// </summary>
        /// <returns>AA PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "CoefficientsConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_fCTMultiplier = m_Reader.ReadSingle();
                m_fVTMultiplier = m_Reader.ReadSingle();
                m_fRegisterMultiplier = m_Reader.ReadSingle();
            }

            return Result;
        }

        /// <summary>
        /// Writes the Coefficients Config block to table 2048. The member
        /// variables must contain the data that is to be written to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "CoefficientsConfig.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_fCTMultiplier);
            m_Writer.Write(m_fVTMultiplier);
            m_Writer.Write(m_fRegisterMultiplier);

            return base.Write();
        }

        /// <summary>
        /// Writes the data to the log file for debugging purposes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of CoefficientsConfig Table");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CT Multiplier = " + m_fCTMultiplier.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "VT Multiplier = " + m_fVTMultiplier.ToString(CultureInfo.InvariantCulture));
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Register Multiplier = " + m_fRegisterMultiplier.ToString(CultureInfo.InvariantCulture));

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of CoefficientsConfig Table ");
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

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets the CT Multiplier field in table 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created
        //
        public float CTMultiplier
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
                            "Error Reading CT Multiplier"));
                    }
                }

                return m_fCTMultiplier;
            }
        }

        /// <summary>
        /// Gets the VT Multiplier field in table 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public float VTMultiplier
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
                            "Error Reading VT Multiplier"));
                    }
                }

                return m_fVTMultiplier;
            }
        }

        /// <summary>
        /// Gets the Register Multiplier field in 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 RCG 7.40.00 N/A    Created

        public float RegisterMultiplier
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
                            "Error Reading Register Multiplier"));
                    }
                }

                return m_fRegisterMultiplier;
            }
        }

        #endregion

        #region Member Variables

        private float m_fCTMultiplier;
        private float m_fVTMultiplier;
        private float m_fRegisterMultiplier;

        #endregion Member Variables
    }

    /// <summary>
    /// This ConstantsConfig class handles the reading/writing of the
    ///	the constants config block of 2048. The reading of this table in the 
    ///	meter will be implicit while the writing of this table will need to be
    ///	explicitly called.  (read/write)	
    /// </summary>
    /// <remarks>
    /// This class is specific to the Sentinel meter.  The Image meters use an
    /// inherited class.  The fields of the table should be defined here.  The
    /// sizes of the fields are defined in this base class or the inherited class.
    /// </remarks>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/30/06 mrj 7.30.00 N/A    Created
    //  06/15/06 mcm 7.30.00 N/A    Changed to inherit from base class
    // 
    internal class ConstantsConfig : ANSISubTable
    {
        #region Constants

        /// <summary>
        /// SENTINEL Constant config block length
        /// </summary>
        public const ushort SENTINEL_CONSTANTS_CONFIG_LENGTH = 72;
        /// <summary>
        /// IMAGE Constant config block length
        /// </summary>
        public const ushort IMAGE_CONSTANTS_CONFIG_LENGTH = 74;
        /// <summary>
        /// OpenWay Constatn config block length
        /// </summary>
        public const ushort OPENWAY_CONSTANTS_CONFIG_LENGTH = 74;

        private const int MAX_USER_DATA_LENGTH = 9;   // Null terminated

        #endregion

        #region public methods

        /// <summary>
        /// Constructor
        /// </summary>		
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// <param name="Size">Meter type dependent size of the calendar 
        /// configuration</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/06 mrj 7.30.00 N/A    Created
        //  06/08/06 mcm 7.30.00 N/A	Changed to use table base classes
        // 
        public ConstantsConfig(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, 2048, Offset, Size)
        {
            //Create the fields

            if (SENTINEL_CONSTANTS_CONFIG_LENGTH == Size)
            {
                m_CustomerSerialNum = new char[10];
                m_Filler1 = new char[21];
                m_LoadResearchID = new byte[1];
            }
            else
            {
                m_CustomerSerialNum = new char[16];
                m_Filler1 = new char[2];
                m_LoadResearchID = new byte[16];
            }

            //Initialize the fields
            m_CustomerSerialNum.Initialize();
            m_Filler1.Initialize();
            m_LoadResearchID.Initialize();
            m_NetworkID = 0;
            m_ProgramID = 0;
            m_UserData1 = "";
            m_UserData2 = "";
            m_UserData3 = "";
            m_PrimSec = 0;
            m_ClockSync = 0;
            m_IntBaudRate = 0;
            m_TimeZoneOffset = 0;

        } // ConstantsConfig

        /// <summary>
        /// Reads the Constants config block out of table 2048 in the meter.  We only
        /// care about the User Data 2 field but we need all of the table so that
        /// we can write it.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/06 mrj 7.30.00 N/A    Created
        //  03/13/07 mcm 8.00.18 2454   Load Research ID read incorrectly for Sentinels
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ConstantsConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_CustomerSerialNum = m_Reader.ReadChars(m_CustomerSerialNum.Length);
                m_Filler1 = m_Reader.ReadChars(m_Filler1.Length);
                m_LoadResearchID = m_Reader.ReadBytes(m_LoadResearchID.Length);

                m_NetworkID = m_Reader.ReadUInt16();
                m_ProgramID = m_Reader.ReadUInt16();
                m_UserData1 = m_Reader.ReadString(10);
                m_UserData2 = m_Reader.ReadString(10);
                m_UserData3 = m_Reader.ReadString(10);
                m_PrimSec = m_Reader.ReadByte();
                m_ClockSync = m_Reader.ReadByte();
                m_IntBaudRate = m_Reader.ReadUInt16();
                m_TimeZoneOffset = m_Reader.ReadInt16();

            }

            return Result;
        }

        /// <summary>
        /// Writes the Constants config block to the meter.  The member variable
        /// must contain the values that are to be written.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// <example><code>
        /// Procedures.ProcedureResultCodes ProcResult = 
        /// 		Procedures.ProcedureResultCodes.INVALID_PARAM;
        /// byte[] ProcParam;
        /// byte[] ProcResponse;
        /// ProcParam = new byte[0];
        /// 
        /// ProcResult = ExecuteProcedure( Procedure.OPEN_CONFIG_FILE, 
        /// 						 ProcParam, out ProcResponse );
        /// 
        /// if( Procedures.ProcedureResultCodes.COMPLETED == ProcResult )
        /// {
        /// 	Result = GetDSTResult(Table2048.m_CalendarConfig.Write());
        /// }
        /// 
        /// if(DSTUpdateResult.SUCCESS == Result)
        /// {
        ///		ProcParam = new byte[4];
        ///		ProcParam.Initialize();
        /// 
        ///		ProcResult = ExecuteProcedure( Procedure.CLOSE_CONFIG_FILE, 
        ///									   ProcParam, out ProcResponse );
        /// </code></example>		
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/06 mrj 7.30.00 N/A    Created
		//	03/27/07 mrj 8.00.21 2745	When the User Data fields were changed from
		//								character arrays to strings the Write method
		//								was not changed to the correct one.
        // 
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "ConstantsConfig.Write");
			
            // Resynch our members to the base's data array
            m_DataStream.Position = 0;
            m_Writer.Write(m_CustomerSerialNum);
            m_Writer.Write(m_Filler1);
            m_Writer.Write(m_LoadResearchID);
            m_Writer.Write(m_NetworkID);
            m_Writer.Write(m_ProgramID);
            m_Writer.Write(m_UserData1, 10);
            m_Writer.Write(m_UserData2, 10);
            m_Writer.Write(m_UserData3, 10);
            m_Writer.Write(m_PrimSec);
            m_Writer.Write(m_ClockSync);
            m_Writer.Write(m_IntBaudRate);
            m_Writer.Write(m_TimeZoneOffset);

            return base.Write();
        }


        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of ConstantsConfig Table");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "CustomerSerialNum = " + new string(m_CustomerSerialNum));
                if (1 == m_LoadResearchID.Length)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        "LoadResearchID = " + (byte)m_LoadResearchID[0]);
                }
                else
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        "LoadResearchID = " + m_LoadResearchID.ToString());
                }
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "NetworkID = " + m_NetworkID);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ProgramID = " + m_ProgramID);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "UserData1 = " + m_UserData1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "UserData2 = " + m_UserData2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "UserData3 = " + m_UserData3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "PrimSec = " + m_PrimSec);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ClockSync = " + m_ClockSync);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "IntBaudRate = " + m_IntBaudRate);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "TimeZoneOffset = " + m_TimeZoneOffset);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of ConstantsConfig Table ");
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

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Provides access to User data 1
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 KRC 7.36.00 N/A    Created
        // 
        public string UserData1
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strReturn = "";
                string strUserData;
                int iIndex = 0;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading User Data 1"));
                    }
                }

                strUserData = m_UserData1;

                iIndex = strUserData.IndexOf('\0');

                if (iIndex > 0)
                {
                    strReturn = strUserData.Substring(0, iIndex);
                }
                else if (iIndex < 0)
                {
                    // \0 was not found
                    strReturn = strUserData;
                }

                return strReturn;
            }
        }

        /// <summary>
        /// Sets the user data 2 field in 2048.  The calling class will need to 
        /// explicitly write this table down to the meter.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/30/06 mrj 7.30.00 N/A    Created
        // 
        public string UserData2
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strReturn = "";
                string strUserData;
                int iIndex = 0;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading User Data 2"));
                    }
                }

                strUserData = m_UserData2;

                iIndex = strUserData.IndexOf('\0');

                if (iIndex > 0)
                {
                    strReturn = strUserData.Substring(0, iIndex);
                }
                else if (iIndex < 0)
                {
                    // \0 was not found
                    strReturn = strUserData;
                }

                return strReturn;
            }
            set
            {
                PSEMResponse Result = PSEMResponse.Ok;

                //Since we currently only fill in the user data 2, we need to
                //read the other fields before we write this table
                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading User Data 2"));
                    }
                }

                m_UserData2 = value;

                if (m_UserData2.Length > MAX_USER_DATA_LENGTH)
                {
                    // We need to truncate what we have.
                    m_UserData2 = m_UserData2.Substring(0, MAX_USER_DATA_LENGTH);
                }
            }
        }

        /// <summary>
        /// Provides access to User data 3
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/10/06 KRC 7.36.00 N/A    Created
        // 
        public string UserData3
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strReturn = "";
                string strUserData;
                int iIndex = 0;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading User Data 3"));
                    }
                }

                strUserData = m_UserData3;

                iIndex = strUserData.IndexOf('\0');

                if (iIndex > 0)
                {
                    strReturn = strUserData.Substring(0, iIndex);
                }
                else if (iIndex < 0)
                {
                    // \0 was not found
                    strReturn = strUserData;
                }

                return strReturn;
            }
        }

        /// <summary>
        /// Program ID property from the constants config block.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/23/06 mrj 7.30.00 N/A    Created
        // 
        public ushort ProgramID
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
                            "Error Reading Program ID"));
                    }
                }

                return m_ProgramID;
            }
        }

        /// <summary>
        /// CustomSerialNumber property from the constants config block.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/06 mrj 7.30.00 N/A    Created
        // 		
        public string CustomerSerialNumber
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Customer Serial Number"));
                    }
                }

                string strSerialNum = new string(m_CustomerSerialNum);

                return strSerialNum;
            }
        }

        /// <summary>
        /// Load Research ID.  Sentinel meters store this value as a byte.
        /// Image meters store this value as a string.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  03/13/07 mcm 8.00.18 2454   Add Load Research ID support
        /// </remarks>		
        public string LoadResearchID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                string strValue = "";

                if (TableState.Unloaded == m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Load Research ID"));
                    }
                }

                if (1 == m_LoadResearchID.Length)
                {
                    strValue = m_LoadResearchID[0].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    // There's probably a better way to do this, but the Sentinel case 
                    // needs to be stored as a byte.
                    for (int i = 0; i < m_LoadResearchID.Length; i++)
                    {
                        if (0 == m_LoadResearchID[i])
                        {
                            // null characters in strings don't display well.
                            break;
                        }
                        else
                        {
                            strValue += (char)m_LoadResearchID[i];
                        }
                    }
                }

                return strValue;
            }
        }


        #endregion properties

        #region Members

        //The table's member variables which represent the MFG ANSI table, 
        //constants config block
        /// <summary>
        /// Custom serial number
        /// </summary>
        protected char[] m_CustomerSerialNum;
        /// <summary>
        /// Filler - This is the legacy meter ID, if in the future we need to configure
        /// the meter ID/unit ID then we will have to write it here for pre-Saturn meters.
        /// </summary>
        protected char[] m_Filler1;
        /// <summary>
        /// Load research ID - Sentinel and Image are different sizes
        /// </summary>
        protected byte[] m_LoadResearchID;
        /// <summary>
        /// Network ID
        /// </summary>
        protected ushort m_NetworkID;
        /// <summary>
        /// Program ID
        /// </summary>
        protected ushort m_ProgramID;
        /// <summary>
        /// User data 1
        /// </summary>
        protected string m_UserData1;
        /// <summary>
        /// User Data 2
        /// </summary>
        protected string m_UserData2;
        /// <summary>
        /// User data 3
        /// </summary>
        protected string m_UserData3;
        /// <summary>
        /// Primary/Secondary
        /// </summary>
        protected byte m_PrimSec;
        /// <summary>
        /// Clock sync
        /// </summary>
        protected byte m_ClockSync;
        /// <summary>
        /// Internal baud rate
        /// </summary>
        protected ushort m_IntBaudRate;
        /// <summary>
        /// Time zone offset
        /// </summary>
        protected short m_TimeZoneOffset;

        #endregion Members

    } // ConstantsConfig class

    /// <summary>
    /// This IOConfig class handles the reading/writing of the
    ///	the IO config block of 2048. The reading of this table in the 
    ///	meter will be implicit while the writing of this table will need to be
    ///	explicitly called.  (read/write)	
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/07/09 jrf 2.20.02 N/A    Created
    // 
    public class IOConfig : ANSISubTable
    {
        #region Constants

        /// <summary>
        /// Size of IO Configuration
        /// </summary>
        public const ushort IO_CONFIG_TBL_SIZE = 52;
        
        #endregion

        #region public methods

        /// <summary>
        /// Constructor.
        /// </summary>		
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/07/09 jrf 2.20    N/A    Created
        // 
        public IOConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, IO_CONFIG_TBL_SIZE)
        {
            InitializeMemebers();
        }

        /// <summary>
        /// Constructor used for file based structures
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 jrf 2.20.03    N/A    Created
        // 
        public IOConfig(PSEMBinaryReader EDLBinaryReader)
            : base(2048, IO_CONFIG_TBL_SIZE)
        {
            InitializeMemebers();

            m_Reader = EDLBinaryReader;
            m_TableState = TableState.Loaded;

            ParseData();
        }

        /// <summary>
        /// Reads the IO config block out of table 2048 in the meter.  
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/07/09 jrf 2.20    N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "IOConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the IO config block to the meter.  The member variable
        /// must contain the values that are to be written.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/07/09 jrf 2.20    N/A    Created
        // 
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "IOConfig.Write");

            // Resynch our members to the base's data array
            m_DataStream.Position = 0;

            //Output Channel 1
            m_Writer.Write(m_byOutputCh1Type);
            m_Writer.Write(m_byOutputCh1Event);
            m_Writer.Write(m_byOutputCh1Energy);
            m_Writer.Write(m_uiOutputCh1PulseWidth);
            m_Writer.Write(m_uiOutputCh1PulseWeight);
            m_Writer.Write(m_byOutputCh1TestMode);

            //Output Channel 2
            m_Writer.Write(m_byOutputCh2Type);
            m_Writer.Write(m_byOutputCh2Event);
            m_Writer.Write(m_byOutputCh2Energy);
            m_Writer.Write(m_uiOutputCh2PulseWidth);
            m_Writer.Write(m_uiOutputCh2PulseWeight);
            m_Writer.Write(m_byOutputCh2TestMode);

            //Output Channel 3
            m_Writer.Write(m_byOutputCh3Type);
            m_Writer.Write(m_byOutputCh3Event);
            m_Writer.Write(m_byOutputCh3Energy);
            m_Writer.Write(m_uiOutputCh3PulseWidth);
            m_Writer.Write(m_uiOutputCh3PulseWeight);
            m_Writer.Write(m_byOutputCh3TestMode);

            //Output Channel 4
            m_Writer.Write(m_byOutputCh4Type);
            m_Writer.Write(m_byOutputCh4Event);
            m_Writer.Write(m_byOutputCh4Energy);
            m_Writer.Write(m_uiOutputCh4PulseWidth);
            m_Writer.Write(m_uiOutputCh4PulseWeight);
            m_Writer.Write(m_byOutputCh4TestMode);

            //Output Channel 5
            m_Writer.Write(m_byOutputCh5Type);
            m_Writer.Write(m_byOutputCh5Event);
            m_Writer.Write(m_byOutputCh5Energy);
            m_Writer.Write(m_uiOutputCh5PulseWidth);
            m_Writer.Write(m_uiOutputCh5PulseWeight);
            m_Writer.Write(m_byOutputCh5TestMode);

            //Input Channel 1
            m_Writer.Write(m_byInputCh1Action);
            m_Writer.Write(m_byInputCh1TriggerType);
            m_Writer.Write(m_fltInputCh1PulseWeight);

            //Input Channel 2
            m_Writer.Write(m_byInputCh2Action);
            m_Writer.Write(m_byInputCh2TriggerType);
            m_Writer.Write(m_fltInputCh2PulseWeight);

            return base.Write();
        }


        /// <summary>
        /// Writes the IOConfig contents to the logger for debugging.
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created.
        //
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of IOConfig Table ");

                //Output Channel 1
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1Type = " + m_byOutputCh1Type);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1Event = " + m_byOutputCh1Event);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1Energy = " + m_byOutputCh1Energy);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1PulseWidth = " + m_uiOutputCh1PulseWidth);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1PulseWeight = " + m_uiOutputCh1PulseWeight);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh1TestMode = " + m_byOutputCh1TestMode);
                //Output Channel 2
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2Type = " + m_byOutputCh2Type);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2Event = " + m_byOutputCh2Event);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2Energy = " + m_byOutputCh2Energy);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2PulseWidth = " + m_uiOutputCh2PulseWidth);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2PulseWeight = " + m_uiOutputCh2PulseWeight);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh2TestMode = " + m_byOutputCh2TestMode);
                //Output Channel 3
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3Type = " + m_byOutputCh3Type);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3Event = " + m_byOutputCh3Event);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3Energy = " + m_byOutputCh3Energy);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3PulseWidth = " + m_uiOutputCh3PulseWidth);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3PulseWeight = " + m_uiOutputCh3PulseWeight);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh3TestMode = " + m_byOutputCh3TestMode);
                //Output Channel 4
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4Type = " + m_byOutputCh4Type);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4Event = " + m_byOutputCh4Event);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4Energy = " + m_byOutputCh4Energy);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4PulseWidth = " + m_uiOutputCh4PulseWidth);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4PulseWeight = " + m_uiOutputCh4PulseWeight);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh4TestMode = " + m_byOutputCh4TestMode);
                //Output Channel 5
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5Type = " + m_byOutputCh5Type);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5Event = " + m_byOutputCh5Event);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5Energy = " + m_byOutputCh5Energy);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5PulseWidth = " + m_uiOutputCh5PulseWidth);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5PulseWeight = " + m_uiOutputCh5PulseWeight);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutputCh5TestMode = " + m_byOutputCh5TestMode);
                //Input Channel 1
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh1Action = " + m_byInputCh1Action);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh1TriggerType = " + m_byInputCh1TriggerType);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh1PulseWeight = " + m_fltInputCh1PulseWeight);
                //Input Channel 2
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh2Action = " + m_byInputCh2Action);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh2TriggerType = " + m_byInputCh2TriggerType);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "InputCh2PulseWeight = " + m_fltInputCh2PulseWeight);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of IOConfig Table ");
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

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Gets/Sets the KYZ configuration data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created.
        //
        public KYZData IOData
        {
            get
            {
                KYZData KYZConfig = new KYZData();
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Loaded != m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading IO Data"));
                    }
                }

                RetrieveKYZConfiguration(ref KYZConfig);

                return KYZConfig;
            }

            set
            {
                KYZData KYZConfig = value;

                PopulateKYZConfiguration(KYZConfig);

                m_TableState = TableState.Dirty;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses the data read by the call to Read. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/07/09 jrf 2.20.02 N/A    Created
        //
        protected virtual void ParseData()
        {
            //Populate the member variables that represent the table
            
            //Output Channel 1
            m_byOutputCh1Type = m_Reader.ReadByte();
            m_byOutputCh1Event = m_Reader.ReadByte();
            m_byOutputCh1Energy = m_Reader.ReadByte();
            m_uiOutputCh1PulseWidth = m_Reader.ReadUInt16();
            m_uiOutputCh1PulseWeight = m_Reader.ReadUInt16();
            m_byOutputCh1TestMode = m_Reader.ReadByte();
            
            //Output Channel 2
            m_byOutputCh2Type = m_Reader.ReadByte();
            m_byOutputCh2Event = m_Reader.ReadByte();
            m_byOutputCh2Energy = m_Reader.ReadByte();
            m_uiOutputCh2PulseWidth = m_Reader.ReadUInt16();
            m_uiOutputCh2PulseWeight = m_Reader.ReadUInt16();
            m_byOutputCh2TestMode = m_Reader.ReadByte();
            
            //Output Channel 3
            m_byOutputCh3Type = m_Reader.ReadByte();
            m_byOutputCh3Event = m_Reader.ReadByte();
            m_byOutputCh3Energy = m_Reader.ReadByte();
            m_uiOutputCh3PulseWidth = m_Reader.ReadUInt16();
            m_uiOutputCh3PulseWeight = m_Reader.ReadUInt16();
            m_byOutputCh3TestMode = m_Reader.ReadByte();
            
            //Output Channel 4
            m_byOutputCh4Type = m_Reader.ReadByte();
            m_byOutputCh4Event = m_Reader.ReadByte();
            m_byOutputCh4Energy = m_Reader.ReadByte();
            m_uiOutputCh4PulseWidth = m_Reader.ReadUInt16();
            m_uiOutputCh4PulseWeight = m_Reader.ReadUInt16();
            m_byOutputCh4TestMode = m_Reader.ReadByte();
            
            //Output Channel 5
            m_byOutputCh5Type = m_Reader.ReadByte();
            m_byOutputCh5Event = m_Reader.ReadByte();
            m_byOutputCh5Energy = m_Reader.ReadByte();
            m_uiOutputCh5PulseWidth = m_Reader.ReadUInt16();
            m_uiOutputCh5PulseWeight = m_Reader.ReadUInt16();
            m_byOutputCh5TestMode = m_Reader.ReadByte();
            
            //Input Channel 1
            m_byInputCh1Action = m_Reader.ReadByte();
            m_byInputCh1TriggerType = m_Reader.ReadByte();
            m_fltInputCh1PulseWeight = m_Reader.ReadSingle();
            
            //Input Channel 2
            m_byInputCh2Action = m_Reader.ReadByte();
            m_byInputCh2TriggerType = m_Reader.ReadByte();
            m_fltInputCh2PulseWeight = m_Reader.ReadSingle();
        }

        /// <summary>
        /// This method retrieves the KYZ configuration. 
        /// </summary>
        /// <param name="KYZConfig">A reference to the KYZ configuration object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created
        //
        protected virtual void RetrieveKYZConfiguration(ref KYZData KYZConfig)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method populates the KYZ configuration. 
        /// </summary>
        /// <param name="KYZConfig">A KYZ configuration object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/21/09 jrf 2.20.02 N/A    Created
        //
        protected virtual void PopulateKYZConfiguration(KYZData KYZConfig)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method initializes the member variables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 jrf 2.20.03    N/A    Created
        //
        private void InitializeMemebers()
        {
            //Output Channel 1
            m_byOutputCh1Type = 0;
            m_byOutputCh1Event = 0;
            m_byOutputCh1Energy = 0;
            m_uiOutputCh1PulseWidth = 0;
            m_uiOutputCh1PulseWeight = 0;
            m_byOutputCh1TestMode = 0;

            //Output Channel 2
            m_byOutputCh2Type = 0;
            m_byOutputCh2Event = 0;
            m_byOutputCh2Energy = 0;
            m_uiOutputCh2PulseWidth = 0;
            m_uiOutputCh2PulseWeight = 0;
            m_byOutputCh2TestMode = 0;

            //Output Channel 3
            m_byOutputCh3Type = 0;
            m_byOutputCh3Event = 0;
            m_byOutputCh3Energy = 0;
            m_uiOutputCh3PulseWidth = 0;
            m_uiOutputCh3PulseWeight = 0;
            m_byOutputCh3TestMode = 0;

            //Output Channel 4
            m_byOutputCh4Type = 0;
            m_byOutputCh4Event = 0;
            m_byOutputCh4Energy = 0;
            m_uiOutputCh4PulseWidth = 0;
            m_uiOutputCh4PulseWeight = 0;
            m_byOutputCh4TestMode = 0;

            //Output Channel 5
            m_byOutputCh5Type = 0;
            m_byOutputCh5Event = 0;
            m_byOutputCh5Energy = 0;
            m_uiOutputCh5PulseWidth = 0;
            m_uiOutputCh5PulseWeight = 0;
            m_byOutputCh5TestMode = 0;

            //Input Channel 1
            m_byInputCh1Action = 0;
            m_byInputCh1TriggerType = 0;
            m_fltInputCh1PulseWeight = 0.0f;

            //Input Channel 2
            m_byInputCh2Action = 0;
            m_byInputCh2TriggerType = 0;
            m_fltInputCh2PulseWeight = 0.0f;
        }

        #endregion

        #region Members

        //Output Channel 1
        /// <summary>Output Channel 1 Type</summary>
        protected byte m_byOutputCh1Type;
        /// <summary>Output Channel 1 Event</summary>
        protected byte m_byOutputCh1Event;
        /// <summary>Output Channel 1 Energy</summary>
        protected byte m_byOutputCh1Energy;
        /// <summary>Output Channel 1 Pulse Width</summary>
        protected UInt16 m_uiOutputCh1PulseWidth;
        /// <summary>Output Channel 1 Pulse Weight</summary>
        protected UInt16 m_uiOutputCh1PulseWeight;
        /// <summary>Output Channel 1 Test Mode</summary>
        protected byte m_byOutputCh1TestMode;
        
        //Output Channel 2
        /// <summary>Output Channel 2 Type</summary>
        protected byte m_byOutputCh2Type;
        /// <summary>Output Channel 2 Event</summary>
        protected byte m_byOutputCh2Event;
        /// <summary>Output Channel 2 Energy</summary>
        protected byte m_byOutputCh2Energy;
        /// <summary>Output Channel 2 Pulse Width</summary>
        protected UInt16 m_uiOutputCh2PulseWidth;
        /// <summary>Output Channel 2 Pulse Weight</summary>
        protected UInt16 m_uiOutputCh2PulseWeight;
        /// <summary>Output Channel 2 Test Mode</summary>
        protected byte m_byOutputCh2TestMode;
        
        //Output Channel 3
        /// <summary>Output Channel 3 Type</summary>
        protected byte m_byOutputCh3Type;
        /// <summary>Output Channel 3 Event</summary>
        protected byte m_byOutputCh3Event;
        /// <summary>Output Channel 3 Energy</summary>
        protected byte m_byOutputCh3Energy;
        /// <summary>Output Channel 3 Pulse Width</summary>
        protected UInt16 m_uiOutputCh3PulseWidth;
        /// <summary>Output Channel 3 Pulse Weight</summary>
        protected UInt16 m_uiOutputCh3PulseWeight;
        /// <summary>Output Channel 3 Test Mode</summary>
        protected byte m_byOutputCh3TestMode;
        
        //Output Channel 4
        /// <summary>Output Channel 4 Type</summary>
        protected byte m_byOutputCh4Type;
        /// <summary>Output Channel 4 Event</summary>
        protected byte m_byOutputCh4Event;
        /// <summary>Output Channel 4 Energy</summary>
        protected byte m_byOutputCh4Energy;
        /// <summary>Output Channel 4 Pulse Width</summary>
        protected UInt16 m_uiOutputCh4PulseWidth;
        /// <summary>Output Channel 4 Pulse Weight</summary>
        protected UInt16 m_uiOutputCh4PulseWeight;
        /// <summary>Output Channel 4 Test Mode</summary>
        protected byte m_byOutputCh4TestMode;
        
        //Output Channel 5
        /// <summary>Output Channel 5 Type</summary>
        protected byte m_byOutputCh5Type;
        /// <summary>Output Channel 5 Event</summary>
        protected byte m_byOutputCh5Event;
        /// <summary>Output Channel 5 Energy</summary>
        protected byte m_byOutputCh5Energy;
        /// <summary>Output Channel 5 Pulse Width</summary>
        protected UInt16 m_uiOutputCh5PulseWidth;
        /// <summary>Output Channel 5 Pulse Weight</summary>
        protected UInt16 m_uiOutputCh5PulseWeight;
        /// <summary>Output Channel 5 Test Mode</summary>
        protected byte m_byOutputCh5TestMode;
        
        //Input Channel 1
        /// <summary>Input Channel 1 Action</summary>
        protected byte m_byInputCh1Action;
        /// <summary>Input Channel 1 Trigger Type</summary>
        protected byte m_byInputCh1TriggerType;
        /// <summary>Input Channel 1 Event</summary>
        protected float m_fltInputCh1PulseWeight;
        
        //Input Channel 2
        /// <summary>Input Channel 2 Action</summary>
        protected byte m_byInputCh2Action;
        /// <summary>Input Channel 2 Trigger Type</summary>
        protected byte m_byInputCh2TriggerType;
        /// <summary>Input Channel 2 Event</summary>
        protected float m_fltInputCh2PulseWeight;
        
        #endregion

    } // IOConfig class

    /// <summary>
    /// This DemandConfig class handles the reading of the demand config 
    /// block of 2048. The reading of this table in the meter will be implicit.
    /// (read-only)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    //  06/09/06 mcm 7.30.00 N/A    Modified to inherit from bases classes
    // 
    internal class DemandConfig : ANSISubTable
    {
        #region Constants

        private const int DEMAND_CONFIG_BLOCK_LENGTH = 88;

        #endregion

        #region Definitions

        /// <summary>Schedule Control</summary>
        protected enum SCHEDCONTROL
        {
            /// <summary>
            /// Demand reset Disabled
            /// </summary>
            DEMAND_RESET_DISABLED = 0,
            /// <summary>Demand Reset Every N Days</summary>
            DEMAND_RESET_N_DAYS = 1,
            /// <summary>Demand Reset Nth Day of the Month</summary>
            DEMAND_RESET_N_DAY_MONTH = 2,
            /// <summary>Demand Reset N Days before the End of the month</summary>
            DEMAND_RESET_N_DAY_END_MONTH = 3,
            /// <summary>Custom Demand Reset</summary>
            DEMAND_RESET_CUSTOM = 4
        }

        /// <summary>
        /// The Demand Control types.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created

        internal enum DemandControl : byte
        {
            Block = 0x00,
            Sliding = 0x01,
            Thermal = 0x02,
        }

        #endregion

        #region Public Methods

        /// <summary>Constructor</summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        //  06/15/06 mcm 7.30.00 N/A	Modified to use base classes
        //  10/03/06 KRC 7.36.00 N/A    Change so we can inherit from it.
        //  
        public DemandConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, DEMAND_CONFIG_BLOCK_LENGTH)
        {
            m_SchedControl = 0;
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// <param name="Size">Size of Table</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/03/06 KRC 7.36.00 N/A    Created
        //  
        protected DemandConfig(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, 2048, Offset, Size)
        {
            m_SchedControl = 0;
        }

        /// <summary>
        /// Reads the Demand config block out of table 2048 in the meter.  We only
        /// care about the schedule control byte, for now put the rest of the block
        /// in filler arrays.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  10/11/06 RCG 7.40.00        Filling in the remaining values with variables
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "DemandConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  10/11/06 RCG 7.40.00        Filled in remaining values
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of DemandConfig Table ");

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandControl = " + m_byDemandControl);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "NumSubIntervals = " + m_byNumSubIntervals);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "IntervalLength = " + m_byIntervalLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "TestModeNumSubIntervals = " + m_byTestModeNumSubIntervals);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "TestModeIntervalLength = " + m_byTestModeIntervalLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "SchedControl = " + m_SchedControl);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetHour = " + m_byDemandResetHour);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetMinute = " + m_byDemandResetMinute);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandResetDay = " + m_byDemandResetDay);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition1 = " + m_uiDemandDefinition1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition2 = " + m_uiDemandDefinition2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition3 = " + m_uiDemandDefinition3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition4 = " + m_uiDemandDefinition4);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition5 = " + m_uiDemandDefinition5);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition6 = " + m_uiDemandDefinition6);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition7 = " + m_uiDemandDefinition7);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition8 = " + m_uiDemandDefinition8);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition9 = " + m_uiDemandDefinition9);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "DemandDefinition10 = " + m_uiDemandDefinition10);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource1 = " + m_uiThresholdSource1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel1 = " + m_fThresholdLevel1);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource2 = " + m_uiThresholdSource2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel2 = " + m_fThresholdLevel2);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource3 = " + m_uiThresholdSource3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel3 = " + m_fThresholdLevel3);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdSource4 = " + m_uiThresholdSource4);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ThresholdLevel4 = " + m_fThresholdLevel4);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "RegisterFullScale = " + m_fRegisterFullScale);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "OutageLength = " + m_usOutageLength);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "ColdLoadPickupTime = " + m_byColdLoadPickupTime);

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of DemandConfig Table ");
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

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Returns whether or not the custom schedule is programmed
        /// for demand reset billing schedule. (out parameter)
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public bool CustomSchedSupported
        {
            get
            {
                bool Supported = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Custom Schedule supported flag"));
                    }
                }

                if (SCHEDCONTROL.DEMAND_RESET_CUSTOM == (SCHEDCONTROL)m_SchedControl)
                {
                    //Custom schedule is set for demand reset billing schedule
                    Supported = true;
                }

                return Supported;
            }
        }

        /// <summary>
        /// Gets the number of sub intervals for Demand Registers
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created

        public int NumberOfSubIntervals
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
                            "Error Reading Number of Sub Intervals"));
                    }
                }

                return (int)m_byNumSubIntervals;
            }
        }

        /// <summary>
        /// Gets the interval length for Demand Registers
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created

        public int IntervalLength
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
                            "Error Reading Interval Length"));
                    }
                }

                return (int)m_byIntervalLength;
            }
        }

        /// <summary>
        /// Gets the number of test mode sub intervals for Demand Registers
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        //
        public int NumberOfTestModeSubIntervals
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
                            "Error Reading Number of Test Mode Sub Intervals"));
                    }
                }

                return (int)m_byTestModeNumSubIntervals;
            }
        }

        /// <summary>
        /// Gets the test mode interval length for Demand Registers
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        //
        public int TestModeIntervalLength
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
                            "Error Reading Test Mode Interval Length"));
                    }
                }

                return (int)m_byTestModeIntervalLength;
            }
        }

        /// <summary>
        /// Gets the Outage Length before Cold Load Pickup in seconds.
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        //
        public int OutageLength
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
                            "Error Reading Outage Length"));
                    }
                }

                return (int)m_usOutageLength;
            }
        }

        /// <summary>
        /// Gets the Cold Load Pickup Time in minutes
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        //
        public int ColdLoadPickupTime
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
                            "Error Reading Outage Length"));
                    }
                }

                return (int)m_byColdLoadPickupTime;
            }
        }

        /// <summary>
        /// Gets the Demand Control Byte - (In OpenWay bit 7 is used for Load Control reconnect)
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 07/20/07 KRC 8.10.15 N/A    Created

        public int DemandControlByte
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
                            "Error Reading Demand Control Byte"));
                    }
                }

                return (int)m_byDemandControl;
            }
        }

        /// <summary>
        /// Gets the Demand Threshold 1 - Level - (In OpenWay this indicates if Load Control is enabled)
        /// </summary>
        /// <exception cref="PSEMException">
        /// Thrown when a PSEM communications error occurs
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 07/20/07 KRC 8.10.15 N/A    Created

        public Single DemandThreshold_1_Level
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
                            "Error Reading Threshold"));
                    }
                }

                return (Single)m_fThresholdLevel1;
            }
        }

        /// <summary>
        /// Gets the list of configured demands as LID numbers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public virtual List<uint> Demands
        {
            get
            {
                List<uint> DemandList = new List<uint>();
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Threshold"));
                    }
                }

                DemandList.Add(m_uiDemandDefinition1);
                DemandList.Add(m_uiDemandDefinition2);
                DemandList.Add(m_uiDemandDefinition3);
                DemandList.Add(m_uiDemandDefinition4);
                DemandList.Add(m_uiDemandDefinition5);
                DemandList.Add(m_uiDemandDefinition6);
                DemandList.Add(m_uiDemandDefinition7);
                DemandList.Add(m_uiDemandDefinition8);
                DemandList.Add(m_uiDemandDefinition9);
                DemandList.Add(m_uiDemandDefinition10);

                return DemandList;
            }
        }

        #endregion properties

        #region Protected Methods

        /// <summary>
        /// Parses the data read by the call to Read. This is used so
        /// that we do not try to parse to much data in the CENTRON_AMI
        /// version of the DemandConfig table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/11/06 RCG 7.40.00 N/A    Created
        //
        protected virtual void ParseData()
        {
            //Populate the member variables that represent the table
            m_byDemandControl = m_Reader.ReadByte();
            m_byNumSubIntervals = m_Reader.ReadByte();
            m_byIntervalLength = m_Reader.ReadByte();
            m_byTestModeNumSubIntervals = m_Reader.ReadByte();
            m_byTestModeIntervalLength = m_Reader.ReadByte();
            m_SchedControl = m_Reader.ReadByte();
            m_byDemandResetHour = m_Reader.ReadByte();
            m_byDemandResetMinute = m_Reader.ReadByte();
            m_byDemandResetDay = m_Reader.ReadByte();
            m_uiDemandDefinition1 = m_Reader.ReadUInt32();
            m_uiDemandDefinition2 = m_Reader.ReadUInt32();
            m_uiDemandDefinition3 = m_Reader.ReadUInt32();
            m_uiDemandDefinition4 = m_Reader.ReadUInt32();
            m_uiDemandDefinition5 = m_Reader.ReadUInt32();
            m_uiDemandDefinition6 = m_Reader.ReadUInt32();
            m_uiDemandDefinition7 = m_Reader.ReadUInt32();
            m_uiDemandDefinition8 = m_Reader.ReadUInt32();
            m_uiDemandDefinition9 = m_Reader.ReadUInt32();
            m_uiDemandDefinition10 = m_Reader.ReadUInt32();
            m_uiThresholdSource1 = m_Reader.ReadUInt32();
            m_fThresholdLevel1 = m_Reader.ReadSingle();
            m_uiThresholdSource2 = m_Reader.ReadUInt32();
            m_fThresholdLevel2 = m_Reader.ReadSingle();
            m_uiThresholdSource3 = m_Reader.ReadUInt32();
            m_fThresholdLevel3 = m_Reader.ReadSingle();
            m_uiThresholdSource4 = m_Reader.ReadUInt32();
            m_fThresholdLevel4 = m_Reader.ReadSingle();
            m_fRegisterFullScale = m_Reader.ReadSingle();
            m_usOutageLength = m_Reader.ReadUInt16();
            m_byColdLoadPickupTime = m_Reader.ReadByte();
        }

        #endregion

        #region Members

        //The table's member variables which represent the MFG ANSI table, 
        //demand config block

        /// <summary>Demand Control</summary>
        protected byte m_byDemandControl;
        /// <summary>Number of Sub Intervals</summary>
        protected byte m_byNumSubIntervals;
        /// <summary>Interval Length</summary>
        protected byte m_byIntervalLength;
        /// <summary>Test Mode Number of Subintervals</summary>
        protected byte m_byTestModeNumSubIntervals;
        /// <summary>Test Mode Interval Length</summary>
        protected byte m_byTestModeIntervalLength;
        /// <summary>Schedule Control</summary>
        protected byte m_SchedControl;
        /// <summary>Demand reset Hour</summary>
        protected byte m_byDemandResetHour;
        /// <summary>Demand Reset Minute</summary>
        protected byte m_byDemandResetMinute;
        /// <summary>Demand Reset Day</summary>
        protected byte m_byDemandResetDay;
        /// <summary>Demand Definition 1</summary>
        protected uint m_uiDemandDefinition1;
        /// <summary>Demand Definition 2</summary>
        protected uint m_uiDemandDefinition2;
        /// <summary>Demand Definition 3</summary>
        protected uint m_uiDemandDefinition3;
        /// <summary>Demand Definition 4</summary>
        protected uint m_uiDemandDefinition4;
        /// <summary>Demand Definition 5</summary>
        protected uint m_uiDemandDefinition5;
        /// <summary>Demand Definition 6</summary>
        protected uint m_uiDemandDefinition6;
        /// <summary>Demand Defintion 7</summary>
        protected uint m_uiDemandDefinition7;
        /// <summary>Demand Defintion 8</summary>
        protected uint m_uiDemandDefinition8;
        /// <summary>Demand Defintion 9</summary>
        protected uint m_uiDemandDefinition9;
        /// <summary>Demand Defintion 10</summary>
        protected uint m_uiDemandDefinition10;
        /// <summary>Threshold Source 1</summary>
        protected uint m_uiThresholdSource1;
        /// <summary>Threshold Level 1</summary>
        protected float m_fThresholdLevel1;
        /// <summary>Threshold Source 2</summary>
        protected uint m_uiThresholdSource2;
        /// <summary>Threshold Level 2</summary>
        protected float m_fThresholdLevel2;
        /// <summary>Threshold Source 3</summary>
        protected uint m_uiThresholdSource3;
        /// <summary>Threshold Level 3</summary>
        protected float m_fThresholdLevel3;
        /// <summary>Threshold Source 4</summary>
        protected uint m_uiThresholdSource4;
        /// <summary>Threshould Level 4</summary>
        protected float m_fThresholdLevel4;
        /// <summary>Register Full Scale</summary>
        protected float m_fRegisterFullScale;
        /// <summary>Outage Length</summary>
        protected ushort m_usOutageLength;
        /// <summary>Cold Load Pickup Time</summary>
        protected byte m_byColdLoadPickupTime;

        #endregion

    } // DemandConfig class

    /// <summary>
    /// This SelfReadConfig class handles the reading of the self read config 
    /// block of 2048. The reading of this table in the meter will be
    /// implicit.  (read-only)
    /// </summary>
    /// 
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    // 
    internal class SelfReadConfig : ANSISubTable
    {
        #region Constants

        private const int SR_CONFIG_BLOCK_LENGTH = 6;

        #endregion

        #region Definitions

        private enum SCHEDCONTROL
        {
            SR_DISABLED = 0,
            SR_N_DAYS = 1,
            SR_N_DAY_MONTH = 2,
            SR_N_DAY_END_MONTH = 3,
            SR_CUSTOM = 4
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        //  06/08/06 mcm 7.30.00 N/A	Modified to inherit from base classes
        // 
        public SelfReadConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, SR_CONFIG_BLOCK_LENGTH)
        {
            m_SchedControl = 0;
        }

        /// <summary>
        /// Reads the SelfReadConfig component and popuplates its fields
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "SelfReadConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "SelfReadConfig.Read succeeded");

                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_Filler1 = m_Reader.ReadBytes(m_Filler1.Length);
                m_SchedControl = m_Reader.ReadByte();
                m_Filler2 = m_Reader.ReadBytes(m_Filler2.Length);
            }

            return Result;
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of SelfReadConfig Table ");
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "SchedControl = " + m_SchedControl);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of SelfReadConfig Table ");
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

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Returns whether or not the custom schedule is programmed
        /// for self reads. (out parameter)
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        public bool CustomSchedSupported
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
                            "Error Reading Self Read Custom Schedule Supported flag"));
                    }
                }

                return (SCHEDCONTROL.SR_CUSTOM == (SCHEDCONTROL)m_SchedControl);
            }
        }

        #endregion properties

        #region Members

        //The table's member variables which represent the MFG ANSI table, 
        //self read config block
        private byte[] m_Filler1 = new byte[3];
        private byte m_SchedControl;
        private byte[] m_Filler2 = new byte[2];

        #endregion

    } // SelfReadConfig class

    /// <summary>
    /// This CTable2048CustomSched class handles the writing and reading of the 
    /// billing schedule (custom schedule) config block to 2048.  This table will 
    /// need to be explicitly written to the meter. (read-write)
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  07/13/05 mrj 7.13.00 N/A    Created
    //  09/22/05 mrj 7.20.17        Added support for Image
    //  10/23/06 AF  7.40.xx        Added support for AMI meters
    // 
    internal class BillingSchedConfig : ANSISubTable
    {
        #region Constants

        /// <summary>
        /// This is the number of custom schedule items allowed in the meter (300)
        /// </summary>
        public const int NUMBER_OF_CUSTOM_SCHED_ITEMS = 300;
        /// <summary>
        /// This is the number of custom schedule items allowed in AMI meters (8)
        /// </summary>
        public const int NUMBER_OF_CUSTOM_SCHED_ITEMS_AMI = 8;
        /// <summary>
        /// Size of the table for Sentinel meters
        /// </summary>
        public const int SENTINEL_BILLING_SCHED_CONFIG_LENGTH = 600;
        /// <summary>
        /// Size of the table for Image meters
        /// </summary>
        public const int IMAGE_BILLING_SCHED_CONFIG_LENGTH = 610;
        /// <summary>
        /// Size of the table for AMI (Open Way) meters
        /// </summary>
        public const int AMI_BILLING_SCHED_CONFIG_LENGTH = 16;
        /// <summary>
        /// This needs to be set as the last schedule date
        /// </summary>
        public const ushort END_OF_CUST_SCHED = 0xFFFF;

        //The FW Spec says 9 characters + null but Pc-Pro+ writes 10 characters.
        //At the time they discusses it with marketing and FW and decided to
        //leave it as 10 characters.
        private const int MAX_SCHED_NAME_LENGTH = 10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// <param name="Size">Meter type dependent size of the Billing 
        /// Schedule configuration</param>
        /// in table 2048.</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        //  10/23/06 AF  7.40.xx N/A    Added support for Open Way meters (AMI)
        // 
        public BillingSchedConfig(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, 2048, Offset, Size)
        {
            //Create the fields
            if (SENTINEL_BILLING_SCHED_CONFIG_LENGTH == Size)
            {
                m_ScheduleName = null;   //not supported by Sentinel
                m_ScheduleDate = new ushort[NUMBER_OF_CUSTOM_SCHED_ITEMS];
            }
            else if (AMI_BILLING_SCHED_CONFIG_LENGTH == Size)
            {
                m_ScheduleName = null;  //not supported by Open Way
                m_ScheduleDate = new ushort[NUMBER_OF_CUSTOM_SCHED_ITEMS_AMI];
            }
            else
            {
                m_ScheduleName = new char[10];
                m_ScheduleDate = new ushort[NUMBER_OF_CUSTOM_SCHED_ITEMS];
            }

            m_ScheduleDate.Initialize();
        }

        /// <summary>
        /// Reads the BillingSchedConfig component and popuplates its fields
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  10/23/06 AF  7.40.xx N/A    Added support for Open Way meters
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "BillingSchedConfig.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                for (int Index = 0; Index < m_ScheduleDate.Length; Index++)
                {
                    m_ScheduleDate[Index] = m_Reader.ReadUInt16();
                }

                if (null != m_ScheduleName)
                {
                    m_ScheduleName = m_Reader.ReadChars(m_ScheduleName.Length);
                }
            }

            return Result;
        }

        /// <summary>
        /// Writes the BillingSchedConfig component to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  10/23/06 AF  7.40.xx N/A    Added support for Open Way meters (AMI)
        //  
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "BillingSchedConfig.Write");

            // Have to resynch our members to the base's data array
            m_DataStream.Position = 0;

            //Loop through the schedule dates and add them to the data array
            for (int Index = 0; Index < m_ScheduleDate.Length; Index++)
            {
                m_Writer.Write(m_ScheduleDate[Index]);
            }

            if (null != m_ScheduleName)
            {
                m_Writer.Write(m_ScheduleName);
            }

            return base.Write();
        }

        /// <summary>
        /// Writes BillingSchedConfig to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/06 mcm 7.30.00 N/A	Created
        //  10/23/06 AF  7.40.xx N/A    Added support for Open Way (AMI) meters
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Dump of BillingSchedConfig Table");

                //Loop through the schedule dates and add them to the data array
                for (int Index = 0; Index < m_ScheduleDate.Length; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                        "Scheduled Demand Reset [" + Index +
                                        "] = " + m_ScheduleDate[Index]);
                }

                if (null != m_ScheduleName)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                        "Schedule Name = " +
                                        new string(m_ScheduleName));
                }


                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "End Dump of BillingSchedConfig Table ");
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

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Gets or sets all the custom schedule dates as number of days since 01/01/2000
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        //  10/23/06 AF  7.40.xx N/A    Added support for Open Way (AMI) meters 
        //                              and added a "get"
        // 
        public ushort[] ScheduleDates
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
                            "Error Reading Billing Schedule Config"));
                    }
                }

                return m_ScheduleDate;
            }
            set
            {
                if (m_ScheduleDate.Length >= value.Length)
                {
                    //Copy all of the elements
                    value.CopyTo(m_ScheduleDate, 0);
                }
                else
                {
                    //Since the value has more elements, we need to only copy the
                    //number supported (300 or 8)
                    Array.Copy(value, 0, m_ScheduleDate, 0, m_ScheduleDate.Length);
                }
            }
        }

        /// <summary>
        /// Gets the number of custom schedule dates configured in the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/23/06 AF  7.40.xx N/A    Created
        //
        public int ScheduleLength
        {
            get
            {
                return m_ScheduleDate.Length;
            }
        }

        /// <summary>
        /// Gets or sets the Schedule Name field in 2048.  The calling class will need to 
        /// explicitly write this table down to the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/06 mrj 7.30.00 N/A    Created
        //  03/13/07 jrf 8.00.18 2521   Adding the ability to read the schedule name
        // 
        public string ScheduleName
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
                            "Error Reading Billing Schedule Config"));
                    }
                }

                return new string(m_ScheduleName);
            }
            set
            {
                if (null != m_ScheduleName)
                {
                    //Clear out the contents of schedule name 2
                    Array.Clear(m_ScheduleName, 0, m_ScheduleName.Length);

                    if (MAX_SCHED_NAME_LENGTH >= value.Length)
                    {
                        value.CopyTo(0, m_ScheduleName, 0, value.Length);
                    }
                    else
                    {
                        value.CopyTo(0, m_ScheduleName, 0, MAX_SCHED_NAME_LENGTH);
                    }
                }
            }
        }

        /// <summary>
        /// Property used to get whether or not the schedule name is supported
        /// in this config block.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/06 mrj 7.30.00 N/A    Created
        // 
        public bool ScheduleNameSupported
        {
            get
            {
                if (null != m_ScheduleName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion properties

        #region Members

        //The table's member variables which represent the MFG ANSI table, 
        //billing schedule config block
        /// <summary>
        /// Schedule dates array
        /// </summary>
        protected ushort[] m_ScheduleDate;
        /// <summary>
        /// Schedule name field, not supported by the Sentinel or Open Way
        /// </summary>
        protected char[] m_ScheduleName;

        #endregion

    } // BillingSchedConfig

    /// <summary>
    /// This OptionBoardHeader class handles the reading of the header info
    /// from the option board config block in 2048.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/02/06 mrj 7.30.00 N/A    Created
    // 
    internal class OptionBoardHeader : ANSISubTable
    {
        #region Constants

        /// <summary>Option Board Header Length</summary>
        public const int OPTION_BOARD_HEADER_LENGTH = 23;

        #endregion

        #region Definitions

        /// <summary>
        /// Option board IDs
        /// </summary>
        public enum OptionBoardIDs
        {
            /// <summary>Modem</summary>
            Modem = 0xBAD1,
            /// <summary>R300 - 0 ERT IDs</summary>
            R300_0ERT_IDs = 0x0BB8,
            /// <summary>R300 - 1 ERT ID</summary>
            R300_1_ERT_ID = 0x0BB9,
            /// <summary>R300 - 2 ERT IDs</summary>
            R300_2_ERT_IDs = 0x0BBA,
            /// <summary>R300 - 3 ERT IDs</summary>
            R300_3_ERT_IDs = 0x0BBB,
            /// <summary>R900 - 0 ERT IDs</summary>
            R900_0ERT_IDs = 0x2328,
            /// <summary>R900 - 1 ERT ID</summary>
            R900_1_ERT_ID = 0x2329,
            /// <summary>R900 - 2 ERT IDs</summary>
            R900_2_ERT_IDs = 0x232A,
            /// <summary>R900 - 3 ERT IDs</summary>
            R900_3_ERT_IDs = 0x232B,
            /// <summary>CellNet</summary>
            CellNet = 0xCE11,
            /// <summary>RS232_485 - 2 Port</summary>
            RS232_485_2_Prt = 0x485A,
            /// <summary>RS232_485 - 1 Port</summary>
            RS232_485_1_Prt = 0x485B,
            // I don't know what the name of these boards should be.
            //			RS232_485_2_Prt	= 0x485C,
            //			RS232_485_1_Prt	= 0x485D,
            /// <summary>Generic</summary>
            Generic = 0xFACE,
            /// <summary>SmartSync</summary>
            SmartSync = 0x554C,
            /// <summary>Ethernet Rs232</summary>
            Ethernet_RS232 = 0x485E,
            /// <summary>Ethernet</summary>
            Ethernet = 0x485F,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048.</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public OptionBoardHeader(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, OPTION_BOARD_HEADER_LENGTH)
        {
            m_SecurityLevel = 0;
            m_PasscodeKey.Initialize();
            m_OptionBoardID = 0;
        }

        /// <summary>
        /// Reads the option board header out of table 2048 in the meter.
        /// This class only reads the header, the first 23 bytes.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "OptionBoardHeader.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "OptionBoardHeader.Read succeeded");

                m_DataStream.Position = 0;

                //Populate the member variables that represent the table
                m_SecurityLevel = m_Reader.ReadByte();
                m_PasscodeKey = m_Reader.ReadBytes(m_PasscodeKey.Length);
                m_OptionBoardID = m_Reader.ReadUInt16();
            }

            return Result;

        } // OptionBoardHeader.Read

        /// <summary>
        /// Writes OptionBoardHeader to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
                string Key;
				
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Dump of OptionBoardHeader Table");

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "SecurityLevel = " + m_SecurityLevel);

                // There's got to be a better way. If you know, please tell me!
                Key = "PasscodeKey = 0x";
                for (int Index = 0; Index < m_PasscodeKey.Length; Index++)
                {
                    Key = Key + m_PasscodeKey[Index].ToString("X2", CultureInfo.InvariantCulture);
                }
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, Key);
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "OptionBoardID = " + m_OptionBoardID +
                    ", 0x" + m_OptionBoardID.ToString("X4", CultureInfo.InvariantCulture));

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "End Dump of OptionBoardHeader Table ");
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
        } // OptionBoardHeader.Dump

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Property used to get the option board ID
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public ushort OptionBoardID
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
                            "Error Reading Option Board ID"));
                    }
                }

                return m_OptionBoardID;
            }
        }

        #endregion properties

        #region Members

        //The table's member variables which represent the MFG ANSI table, 
        //self read config block
        private byte m_SecurityLevel;
        private byte[] m_PasscodeKey = new byte[20];
        private ushort m_OptionBoardID;

        #endregion

    } // OptionBoardHeader class

    /// <summary>
    /// This class represents the configuration portion of the R300 option 
    /// board config.  The option board header info that identifies the board
    /// is not handled by the OptionBoardHeader class.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/02/06 mrj 7.30.00 N/A    Created
    // 
    internal class R300Config : ANSISubTable
    {
        #region Constants

        //The size of the R300 config block is always 32 bytes, regardless
        //of the number of ERTs supported
        private const int R300_CONFIG_BLOCK_LENGTH = 32;

        #endregion

        #region Definitions

        /// <summary>R300 ERT Configuration</summary>
        public struct R300_ERT_Config
        {
            /// <summary>
            /// ERT Qty
            /// </summary>
            public uint uiLid;
            /// <summary>
            /// Data type
            /// </summary>
            public byte byLidDataType;
            /// <summary>
            /// Display Format
            /// </summary>
            public byte byLidDisplayFormat;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The Offset is the offset of this config block
        /// in table 2048. This offset should be the OptionBoardHeader offset +
        /// the size of the option board header (23 bytes)</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public R300Config(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, R300_CONFIG_BLOCK_LENGTH)
        {
            //R300 fields
            m_Filler.Initialize();
            m_R300ErtConfig.Initialize();
        }

        /// <summary>
        /// Reads the R300 config block out of table 2048 in the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/02/06 mrj 7.30.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "OptionBoardHeader.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "OptionBoardHeader.Read succeeded");

                m_DataStream.Position = 0;

                //Populate the member variables that represent the R300 config
                m_Filler = m_Reader.ReadBytes(m_Filler.Length);

                for (int Index = 0; Index < m_R300ErtConfig.Length; Index++)
                {
                    m_R300ErtConfig[Index].uiLid = m_Reader.ReadUInt32();
                    m_R300ErtConfig[Index].byLidDataType = m_Reader.ReadByte();
                    m_R300ErtConfig[Index].byLidDisplayFormat = m_Reader.ReadByte();
                }
            }

            return Result;

        } // R300Config.Read

        /// <summary>
        /// Writes R300Config to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/16/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "Dump of R300Config Table");

                for (int Index = 0; Index < m_R300ErtConfig.Length; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "ERT " + (Index + 1));
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "   LID = " +
                        m_R300ErtConfig[Index].uiLid.ToString("X8", CultureInfo.InvariantCulture));
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "   DataType = 0x" +
                        m_R300ErtConfig[Index].byLidDataType.ToString("X2", CultureInfo.InvariantCulture));
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "   DisplayFormat = 0x" +
                        m_R300ErtConfig[Index].byLidDisplayFormat.ToString("X2", CultureInfo.InvariantCulture));
                }

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "End Dump of R300Config Table ");
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
        } // R300Config.Dump

        #endregion public methods

        #region Public Properties

        /// <summary>
        /// Property used to get the configuration of an ERT
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mrj 7.30.00 N/A    Created
        ///
        public R300_ERT_Config[] R300ERTConfig
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                if (0 == m_R300ErtConfig.Length)
                {
                    throw new Exception("R300 is not configured");
                }

                if (TableState.Unloaded == m_TableState)
                {
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading R300 ERT Config"));
                    }
                }

                return m_R300ErtConfig;
            }
        }

        #endregion properties

        #region Members

        //The R300 fields of the option board config block
        private byte[] m_Filler = new byte[14];
        private R300_ERT_Config[] m_R300ErtConfig = new R300_ERT_Config[3];

        #endregion

    } // R300Config class

    /// <summary>
    /// This class handles the configuration data for the Modem option board.
    /// This should only be used if the Option Board ID obtained from the 
    /// OptionBoardHeader class.
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  02/16/07 RCG 8.00.12        Created

    internal sealed class ModemConfig : ANSISubTable
    {

        #region Constants

        private const ushort MODEM_CONFIG_SIZE = 337;
        private const int MAX_DAY_TYPES = 4;
        private const int MAX_WINDOWS_PER_DAY = 2;
        private const int MAX_PHONE_NUMBER_LENGTH = 64;
        private const int MAX_NUMBER_OF_HOSTS = 4;
        private const int MAX_NUMBER_OF_PHEVENTS = 32;

        // Determine the size of the CallWindows
        private const int CALL_WINDOW_SIZE = 2 * MAX_DAY_TYPES * MAX_WINDOWS_PER_DAY * 2; 

        // Mode Masks
        private const byte ANSWER_OUTSIDE_WINDOW_MASK = 0x04;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="usOffset">The offset of the table into 2048.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/16/07 RCG 8.00.12        Created

        public ModemConfig(CPSEM psem, ushort usOffset)
            : base(psem, 2048, usOffset, MODEM_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Reads the Modem Configuration out of the meter
        /// </summary>
        /// <returns>The PSEM Response code for the read request.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/16/07 RCG 8.00.12        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "ModemConfig.Read");

            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                // Parse the data that was read
                m_byModes = m_Reader.ReadByte();
                m_byBlindDialDelay = m_Reader.ReadByte();
                m_byCallTimeout = m_Reader.ReadByte();
                m_byMaxRetries = m_Reader.ReadByte();
                m_usAnswerBaudRate = m_Reader.ReadUInt16();

                // Read the Host structures out of the meter.
                // We will store the numbers and baud rates seperately to make
                // it easier to use elsewhere.

                m_strPhoneNumbers = new string[MAX_NUMBER_OF_HOSTS];
                m_usBaudRates = new ushort[MAX_NUMBER_OF_HOSTS];

                for (int iIndex = 0; iIndex < MAX_NUMBER_OF_HOSTS; iIndex++)
                {
                    m_strPhoneNumbers[iIndex] = m_Reader.ReadString(MAX_PHONE_NUMBER_LENGTH);
                    m_usBaudRates[iIndex] = m_Reader.ReadUInt16();
                }

                m_byPHEvents = m_Reader.ReadBytes(MAX_NUMBER_OF_PHEVENTS);
                m_sbyPLSAddress = m_Reader.ReadSByte();
                m_byIWSeconds = m_Reader.ReadByte();
                m_byOWSeconds = m_Reader.ReadByte();

                // For now we will just read the CallWindows as a byte array so that we do not
                // have to get the correct byte order for the 3 dimensional array.
                // CallWindows should be of type ushort[MAX_DAY_TYPES][MAX_WINDOWS_PER_DAY][2]
                m_byCallWindows = m_Reader.ReadBytes(CALL_WINDOW_SIZE);

            }

            return Response;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The PSEM response code for the write to the meter.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/16/07 RCG 8.00.12        Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ModemConfig.Write");

            // Have to resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_byModes);
            m_Writer.Write(m_byBlindDialDelay);
            m_Writer.Write(m_byCallTimeout);
            m_Writer.Write(m_byMaxRetries);
            m_Writer.Write(m_usAnswerBaudRate);

            // Write the Host data
            for (int iIndex = 0; iIndex < MAX_NUMBER_OF_HOSTS; iIndex++)
            {
                m_Writer.Write(m_strPhoneNumbers[iIndex], MAX_PHONE_NUMBER_LENGTH);
                m_Writer.Write(m_usBaudRates[iIndex]);
            }

            m_Writer.Write(m_byPHEvents);
            m_Writer.Write(m_sbyPLSAddress);
            m_Writer.Write(m_byIWSeconds);
            m_Writer.Write(m_byOWSeconds);
            m_Writer.Write(m_byCallWindows);

            // Let the base class handle the actual writing to the meter
            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of phone numbers configured into the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/16/07 RCG 8.00.12        Created
        
        public string[] PhoneNumbers
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                return m_strPhoneNumbers;
            }
            set
            {
                // Make sure we have read the rest of the information already

                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                m_strPhoneNumbers = value;
            }
        }

        /// <summary>
        /// Gets or sets the inside call window wait time in seconds.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public byte InsideWindowSeconds
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                return m_byIWSeconds;
            }
            set
            {
                // Make sure we have already read the remaining information
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                m_byIWSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the inside call window wait time in seconds.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public byte OutsideWindowSeconds
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                return m_byOWSeconds;
            }
            set
            {
                // Make sure we have already read the remaining information
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                m_byOWSeconds = value;
            }
        }

        /// <summary>
        /// Gets whether or not the modem is configured to answer calls outside of
        /// a call window
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public bool AnswerOutsideWindow
        {
            get
            {
                // Make sure we have already read the remaining information
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading ModemConfig");
                    }
                }

                return (m_byModes & ANSWER_OUTSIDE_WINDOW_MASK) == ANSWER_OUTSIDE_WINDOW_MASK;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byModes;
        private byte m_byBlindDialDelay;
        private byte m_byCallTimeout;
        private byte m_byMaxRetries;
        private ushort m_usAnswerBaudRate;
        private string[] m_strPhoneNumbers;
        private ushort[] m_usBaudRates;
        private byte[] m_byPHEvents;
        private sbyte m_sbyPLSAddress;
        private byte m_byIWSeconds;
        private byte m_byOWSeconds;
        private byte[] m_byCallWindows;

        #endregion

    }

    /// <summary>
    /// The CalendarConfig class represents the Calendar Configuration data 
    /// block in table 2048.
    /// </summary>
    /// 
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/09/06 mcm 7.30.00 N/A    Created
    // 
    public class CalendarConfig : ANSISubTable
    {
        #region Constants

        private const ushort CAL_ID_OFFSET = 0;
        private const ushort CAL_ID_LENGTH = 2;

        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  11/20/13 jrf 3.50.06 TQ 9478 Calling common method to setup data.
        //  
        public CalendarConfig(CPSEM psem, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(psem, 2048, Offset, Size)
        {
            InitializeData(MaxCalYears);
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  11/20/13 jrf 3.50.06 TQ 9478 Calling common method to setup data.
        //  
        public CalendarConfig(CPSEM psem, ushort TableID, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(psem, TableID, Offset, Size)
        {
            InitializeData(MaxCalYears);
        }
        
        /// <summary>
        /// Calendar Configuartion Table Constructor for file based structure
        /// </summary>
        /// <param name="BinaryReader">PSEM binary reader</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/20/13 jrf 3.50.06 TQ 9478 Calling common method to setup data.
        // 
        public CalendarConfig(PSEMBinaryReader BinaryReader, ushort Offset, ushort Size,
                                byte MaxCalYears)
            : base(2048, Size)
        {
            InitializeData(MaxCalYears);

            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;

            ParseData();
        }

        /// <summary>
        /// Calendar Configuartion Table Constructor for file based structure
        /// </summary>
        /// <param name="BinaryReader">PSEM binary reader</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/20/13 jrf 3.50.06 TQ 9478 Calling common method to setup data.
        // 
        public CalendarConfig(PSEMBinaryReader BinaryReader, ushort TableID, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(TableID, Size)
        {
            InitializeData(MaxCalYears);

            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;

            ParseData();
        }

        /// <summary>
        /// Reads the Calendar configuration component from table 2048 and 
        /// populates the CalendarConfig fields with the values read
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Calendar.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        /// <summary>
        /// Writes the Calendar configuration portion of table 2048 to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Calendar.Write");

            // Have to resynch our members to the base's data array
            m_DataStream.Position = 0;

            m_Writer.Write(m_CalendarID.Value);
            m_Writer.Write(m_Control);
            m_Writer.Write(m_DSTHour);
            m_Writer.Write(m_DSTMinute);
            m_Writer.Write(m_DSTOffset);

            for (int Index = 0; Index < m_Years.Length; Index++)
            {
                m_Writer.Write(m_Years[Index].Year);

                for (int EventIndex = 0; EventIndex < EventsPerYear; EventIndex++)
                {
                    m_Writer.Write(m_Years[Index].Events[EventIndex].Event);
                }
            }

            return base.Write();

        } // CalendarConfig.Write

        /// <summary>
        /// Clears MOST of the CalendarConfig table.  This is usually done 
        /// prior to reconfiguration, so unused portions don't explicitly have 
        /// to be updated. The Control value is not cleared because it's not
        /// in the TOU schedule.
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  03/29/07 mcm 8.00.22 2746   Mark the table as dirty when it's cleared
        public virtual void Clear()
        {
            m_CalendarID.Value = 0;
            m_DSTOffset = 0;

            // mcm - DO NOT CLEAR the Control value.  It is not  available
            // in the TOU schedule, so we will use the existing value.

            for (int YearIndex = 0; YearIndex < m_Years.Length; YearIndex++)
            {
                m_Years[YearIndex].Year = 0;

                for (int EventIndex = 0;
                     EventIndex < m_Years[YearIndex].Events.Length; EventIndex++)
                {
                    m_Years[YearIndex].Events[EventIndex].Event =
                        (ushort)CalendarEvent.CalendarEventType.NO_EVENT;
                }
            }

            // 03/29/07 mcm (scr 2746) - Mark the table as dirty when it's 
            // cleared, so the properties won't reread it during reconfiguration.
            m_TableState = TableState.Dirty;
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/09/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {               
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of CalendarConfig Table ");
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "   m_CalendarID = " + m_CalendarID);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "   m_Control = " + m_Control);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "   m_DSTHour = " + m_DSTHour);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "   m_DSTMinute = " + m_DSTMinute);
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "   m_DSTOffset = " + m_DSTOffset);

                for (int Index = 0; Index < m_Years.Length; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        " *********** Year " +
                        (m_Years[Index].Year + CALENDAR_REFERENCE_YEAR));

                    for (int EventIndex = 0; EventIndex < EventsPerYear; EventIndex++)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                            " Event " + EventIndex + " = 0x" +
                            m_Years[Index].Events[EventIndex].Event.ToString("X4", CultureInfo.InvariantCulture) +
                            ", " + m_Years[Index].Events[EventIndex].TranslatedType +
                            ", Month " + (m_Years[Index].Events[EventIndex].Month + 1) +
                            ", Day " + (m_Years[Index].Events[EventIndex].Day + 1));
                    }
                }
                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of CalendarConfig Table ");
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
        } // CalendarConfig.Dump

        /// <summary>
        /// Finds the index of the requested year in the calendar's Years array
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public bool FindYearIndex(int Year, out byte Index)
        {
            bool YearExists = false;

            Index = 0;

            if (TableState.Unloaded == m_TableState)
            {
                if (PSEMResponse.Ok != Read())
                {
                    //We could not read the table so throw an exception
                    throw (new Exception("Table Could Not be Read"));
                }
            }
            else
            {
                byte YearIndex = 0;

                if (Year >= CALENDAR_REFERENCE_YEAR)
                {
                    // Adjust the Year to match our reference
                    Year = Year - CALENDAR_REFERENCE_YEAR;

                    for (YearIndex = 0; YearIndex < MaxYears; YearIndex++)
                    {
                        if (m_Years[YearIndex].Year == Year)
                        {
                            Index = YearIndex;
                            YearExists = true;
                            break;
                        }
                    }
                }
            }

            return YearExists;

        }  // CalendarConfig.FindYearIndex

        /// <summary>
        /// Translates the CalendarEventType for a Calendar event into an eEventType
        /// for the TOUSchedule class.
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 01/04/07 RCG 8.00.04		    Made more generic and promoted from CENTRON_AMI

        public virtual eEventType GetEventType(ushort usEvent)
        {
            eEventType eType = eEventType.HOLIDAY;

            switch (usEvent)
            {
                case (ushort)CalendarEvent.CalendarEventType.ADD_DST:
                    {
                        eType = eEventType.TO_DST;
                        break;
                    }
                case (ushort)CalendarEvent.CalendarEventType.SUB_DST:
                    {
                        eType = eEventType.FROM_DST;
                        break;
                    }
                case (ushort)CalendarEvent.CalendarEventType.NO_EVENT:
                    {
                        eType = eEventType.NO_EVENT;
                        break;
                    }
                case (ushort)CalendarEvent.CalendarEventType.HOLIDAY:
                    {
                        eType = eEventType.HOLIDAY;
                        break;
                    }
                case (ushort)CalendarEvent.CalendarEventType.SEASON1:
                case (ushort)CalendarEvent.CalendarEventType.SEASON2:
                case (ushort)CalendarEvent.CalendarEventType.SEASON3:
                case (ushort)CalendarEvent.CalendarEventType.SEASON4:
                case (ushort)CalendarEvent.CalendarEventType.SEASON5:
                case (ushort)CalendarEvent.CalendarEventType.SEASON6:
                case (ushort)CalendarEvent.CalendarEventType.SEASON7:
                case (ushort)CalendarEvent.CalendarEventType.SEASON8:
                    {
                        eType = eEventType.SEASON;
                        break;
                    }
            }

            return eType;
        }

        /// <summary>
        /// Translates the CalendarEventType for a Calendar event into an eEventType
        /// for the TOUSchedule class.
        /// </summary>
        /// <returns>The TOU Schedule object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 11/18/13 jrf 3.50.04 TQ 9478 Created.

        public virtual int GetSeasonIndex(ushort usEvent)
        {
            int iSeasonIndex = 0;

            iSeasonIndex = usEvent - (int)CalendarEvent.CalendarEventType.SEASON1;

            return iSeasonIndex;
        }

        #endregion public methods

        #region properties

        /// <summary>
        /// Returns the configured Calendar ID 
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  04/20/07 RCG 8.01.01 2364   Changing to use an offset read rather than a full read
        //                              in order to speed up menu browsing in Field-Pro

        public ushort CalendarID
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte[] byData;

                if (m_CalendarID.Cached == false)
                {
                    Result = m_PSEM.OffsetRead(m_TableID, m_SubTableOffset + CAL_ID_OFFSET, CAL_ID_LENGTH, out byData);

                    if (PSEMResponse.Ok != Result)
                    {
                        //We could not read the table so throw an exception
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Calendar ID"));
                    }
                    else
                    {
                        // Convert the bytes read to something useful.
                        MemoryStream DataStream = new MemoryStream(byData);
                        PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                        m_CalendarID.Value = Reader.ReadUInt16();
                    }
                }                

                return m_CalendarID.Value;
            }
            set
            {
                m_CalendarID.Value = value;
            }
        }

        /// <summary>
        /// Returns the configured Calendar Control value
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public byte Control
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
                            "Error Reading Calendar Control"));
                    }
                }

                return m_Control;
            }
            set
            {
                m_Control = value;
            }
        }

        /// <summary>
        /// Returns the configured DST change hour
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public byte DSTHour
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
                            "Error Reading DST Hour"));
                    }
                }

                return m_DSTHour;
            }
            set
            {
                m_DSTHour = value;
            }
        }

        /// <summary>
        /// Returns the configured DST change minute
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public byte DSTMinute
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
                            "Error Reading DST Minute"));
                    }
                }

                return m_DSTMinute;
            }
            set
            {
                m_DSTMinute = value;
            }
        }

        /// <summary>
        /// Returns the configured DST change amount in minutes
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public byte DSTOffset
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
                            "Error reading DST Offset"));
                    }
                }

                return m_DSTOffset;
            }
            set
            {
                m_DSTOffset = value;
            }
        }

        /// <summary>
        /// Returns the max calendar years supported by the device.  This value 
        /// is device and firmware specific.
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created
        //  
        public int MaxYears
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
                            "Error Reading Max Years"));
                    }
                }

                return m_Years.Length;
            }
        }

        /// <summary>
        /// Gets the CalendarYear data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Created

        public CalendarYear[] Years
        {
            get
            {
                // Make sure the data has been read
                if (TableState.Unloaded == m_TableState)
                {
                    PSEMResponse Response;

                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Years");
                    }
                }

                return m_Years;
            }
        }

        /// <summary>
        /// Provides access to the Number of Events per Year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public virtual int EventsPerYear
        {
            get
            {
                return EVENTS_PER_YEAR;
            }
        }

        /// <summary>
        /// Provides access to the Number of DST Events per Year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Created

        public virtual int DSTEventsPerYear
        {
            get
            {
                return DST_EVENTS_PER_YEAR;
            }
        }

        #endregion properties

        #region Protected Methods

        /// <summary>
        /// Parses the data read by the call to Read. This is used so
        /// that we do not try to parse to much data in the CENTRON_AMI
        /// version of the TOUConfig table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/20/06 KRC 7.36.00 N/A    Created
        //
        protected virtual void ParseData()
        {
            //Populate the member variables that represent the table
            m_CalendarID.Value = m_Reader.ReadUInt16();
            m_Control = m_Reader.ReadByte();
            m_DSTHour = m_Reader.ReadByte();
            m_DSTMinute = m_Reader.ReadByte();
            m_DSTOffset = m_Reader.ReadByte();

            for (int Index = 0; Index < m_Years.Length; Index++)
            {
                m_Years[Index].Year = m_Reader.ReadByte();

                for (int EventIndex = 0; EventIndex < EventsPerYear; EventIndex++)
                {
                    m_Years[Index].Events[EventIndex] = new CalendarEvent(m_Reader.ReadUInt16());
                }
            }
        }

        /// <summary>
        /// Setup data items
        /// </summary>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/19/13 jrf 3.50.06 TQ 9478 Created. 
        //        
        protected virtual void InitializeData(byte MaxCalYears)
        {
            m_CalendarID = new CachedUshort();
            m_Years = new CalendarYear[MaxCalYears];
            for (int Index = 0; Index < MaxCalYears; Index++)
            {
                m_Years[Index] = new CalendarYear(EventsPerYear);
            }
        }

        #endregion

        #region public definitions

        /// <summary>
        /// Year values have implied century 2000
        /// </summary>
        public const int CALENDAR_REFERENCE_YEAR = 2000;

        #endregion public definitions

        #region private definitions

        private const int SIZE_OF_HEADER = 6;
        private const int EVENTS_PER_YEAR = 44;
        private const int DST_EVENTS_PER_YEAR = 2;

        #endregion private definitions

        #region Member Variables

        /// <summary>Array of Calendar Years</summary>
        protected CalendarYear[] m_Years;

        /// <summary>The TOU Calendar ID</summary>
        protected CachedUshort m_CalendarID;
        /// <summary>Control Byte</summary>
        protected byte m_Control = 0;  // For CentronII Meter, this field is for DemandReset flag
        /// <summary>DST Hour </summary>
        protected byte m_DSTHour = 0;
        /// <summary>DST Minute</summary>
        protected byte m_DSTMinute = 0;
        /// <summary>DST Offset</summary>
        protected byte m_DSTOffset = 0;

        #endregion Member Variables

    } // CalendarConfig class

    /// <summary>
    /// Represents the structure of a Calendar Year as defined by the
    /// the device's CONFIGURATION_DATA.doc.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 03/16/07 RCG 8.00.19 N/A    Moved out of CalendarConfig

    public class CalendarYear
    {
        #region Public Methods

        /// <summary>
        /// Constructor that takes the number of events per year
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        //  06/08/06 mcm 7.30.00 N/A	Created

        public CalendarYear(int iEventsPerYear)
        {
            Events = new CalendarEvent[iEventsPerYear];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the year that this object represents
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A	   Created

        public byte Year
        {
            get
            {
                return m_byYear;
            }
            set
            {
                m_byYear = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of events for the year
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/16/07 RCG 8.00.19 N/A	   Created

        public CalendarEvent[] Events
        {
            get
            {
                return m_Events;
            }
            set
            {
                m_Events = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byYear;
        private CalendarEvent[] m_Events;

        #endregion
    }


    /// <summary>
    /// Represents the structure of a Calendar Day event.  See the
    /// TIME_MAN_DESIGN.doc, WindChill document #D0209255 for more info.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    // 03/15/07 RCG 8.00.19 N/A    Moved out of CalendarConfig and converted to a class

    public class CalendarEvent
    {
        #region Constants

        private const ushort TYPE_MASK = 0x000F;
        private const ushort CLEAR_TYPE = 0xFFF0;
        private const ushort MONTH_MASK = 0x00F0;
        private const ushort CLEAR_MONTH = 0xFF0F;
        private const ushort MONTH_SHIFT = 4;
        private const ushort DAY_MASK = 0x1F00;
        private const ushort CLEAR_DAY = 0xE0FF;
        private const ushort DAY_SHIFT = 8;

        #endregion Constants

        #region Definitions

        /// <summary>
        /// Calendar Event Types
        /// </summary>
        public enum CalendarEventType : ushort
        {
            /// <summary>DST Forward</summary>
            ADD_DST = 0x0000,
            /// <summary>DST Backward</summary>
            SUB_DST = 0x0001,
            /// <summary>Holiday</summary>
            HOLIDAY = 0x0002,
            /// <summary>Season 1</summary>
            SEASON1 = 0x0003,
            /// <summary>Season 2</summary>
            SEASON2 = 0x0004,
            /// <summary>Season 3</summary>
            SEASON3 = 0x0005,
            /// <summary>Season 4</summary>
            SEASON4 = 0x0006,
            /// <summary>Season 5</summary>
            SEASON5 = 0x0007,
            /// <summary>Season 6</summary>
            SEASON6 = 0x0008,
            /// <summary>Season 7</summary>
            SEASON7 = 0x0009,
            /// <summary>Season 8</summary>
            SEASON8 = 0x000A,
            /// <summary>No Event</summary>
            NO_EVENT = 0x000F,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Created

        public CalendarEvent()
            : this(0)
        {
        }

        /// <summary>
        /// Constructor. Takes the event data from 2048 as a paramater.
        /// </summary>
        /// <param name="usEvent">The event data from the Calendar Config in 2048</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Created

        public CalendarEvent(ushort usEvent)
        {
            m_usEvent = usEvent;
        }

        /// <summary>
        /// Returns true if this event is a ToDST or FromDST event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Made into a virtual function

        public virtual bool IsDST()
        {
            if (((ushort)CalendarEventType.ADD_DST != (m_usEvent & TYPE_MASK)) ||
                ((ushort)CalendarEventType.SUB_DST != (m_usEvent & TYPE_MASK)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this event is a ToDST or FromDST event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Made into a virtual function

        public virtual bool IsSeason()
        {
            if (((ushort)CalendarEventType.SEASON1 <= (m_usEvent & TYPE_MASK)) &&
                ((ushort)CalendarEventType.SEASON8 >= (m_usEvent & TYPE_MASK)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Accesses this event's Month - 0 indexed
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        //                             Created

        public byte Month
        {
            get
            {
                return (byte)((m_usEvent & MONTH_MASK) >> MONTH_SHIFT);
            }
            set
            {
                if (value < 12)
                {
                    ushort Month = value;

                    // Clear the month bits first
                    m_usEvent = (ushort)(m_usEvent & CLEAR_MONTH);

                    // Set the month they gave us
                    Month = (ushort)(Month << MONTH_SHIFT);
                    m_usEvent = (ushort)(m_usEvent | Month);

                    // PC-PRO+ sets the top two bits if the event is 
                    // defined.  I don't know why.  The bits' values are
                    // not defined, so it shouldn't make any difference.
                    // This code will emulate PC-PRO+ so the configurations
                    // are exactly the same.
                    m_usEvent = (ushort)(m_usEvent | 0xC000);
                }
            }
        }

        /// <summary>
        /// Accesses this event's Day - 0 indexed
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        //                             Created

        public byte Day
        {
            get
            {
                return (byte)((m_usEvent & DAY_MASK) >> DAY_SHIFT);
            }
            set
            {
                if (value < 31)
                {
                    ushort Day = value;

                    // Clear the DAY bits first
                    m_usEvent = (ushort)(m_usEvent & CLEAR_DAY);

                    // Set the DAY they gave us
                    Day = (ushort)(Day << DAY_SHIFT);
                    m_usEvent = (ushort)(m_usEvent | Day);
                }
            }
        }

        /// <summary>
        /// Accesses this event's Type
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Made into a virtual property

        public virtual byte Type
        {
            get
            {
                return (byte)(m_usEvent & TYPE_MASK);
            }
            set
            {
                if (value <= (ushort)CalendarEventType.SEASON8)
                {
                    // Clear then set the event bits
                    m_usEvent = (ushort)(m_usEvent & CLEAR_TYPE);
                    m_usEvent = (ushort)(m_usEvent | value);
                }
            }
        }

        /// <summary>
        /// Accesses this event's event type as a string for debuggering
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Made into a virtual property

        public virtual string TranslatedType
        {
            get
            {
                string Type = "";

                switch (m_usEvent & TYPE_MASK)
                {
                    case (ushort)CalendarEventType.ADD_DST:
                    {
                        Type = "ADD_DST";
                        break;
                    }
                    case (ushort)CalendarEventType.SUB_DST:
                    {
                        Type = "SUB_DST";
                        break;
                    }
                    case (ushort)CalendarEventType.HOLIDAY:
                    {
                        Type = "HOLIDAY";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON1:
                    {
                        Type = "SEASON1";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON2:
                    {
                        Type = "SEASON2";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON3:
                    {
                        Type = "SEASON3";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON4:
                    {
                        Type = "SEASON4";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON5:
                    {
                        Type = "SEASON5";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON6:
                    {
                        Type = "SEASON6";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON7:
                    {
                        Type = "SEASON7";
                        break;
                    }
                    case (ushort)CalendarEventType.SEASON8:
                    {
                        Type = "SEASON8";
                        break;
                    }
                    case (ushort)CalendarEventType.NO_EVENT:
                    {
                        Type = "NO_EVENT";
                        break;
                    }
                    default:
                    {
                        Type = "invalid";
                        break;
                    }
                }

                return Type;
            }
        }

        /// <summary>
        /// Gets or sets the event bit field for this event
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 03/15/07 RCG 8.00.19 N/A    Made into a virtual property

        public ushort Event
        {
            get
            {
                return m_usEvent;
            }
            set
            {
                m_usEvent = value;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Calendar Event</summary>
        protected ushort m_usEvent;

        #endregion


    } // CalendarEvent struct

    /// <summary>
    /// The TOUConfig class represents the TOU Configuration data block in
    /// table 2048. The TOU portion of the configuration defines the seasons
    /// that are applied across the years of the TOU schedule. Seasons are
    /// applied to years in the CalendarConfig portion of the configuration.
    /// </summary>
    /// 
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/23/06 mcm 7.30.00 N/A    Created
    // 
    public class TOUConfig : ANSISubTable
    {
        #region Constants

        private const uint NUM_SUPPORTED_SEASONS = 8;
        private const ushort TOU_CONFIG_SIZE = 1560;
        /// <summary>
        /// Holiday Type Index
        /// </summary> 
        public const ushort HOLIDAY_TYPE_INDEX = 3;

        private const int EVENTS_PER_DAYTYPE = 24;
        private const int DAYTYPES_PER_SEASON = 4;

        #endregion

        #region Definitions

        /// <summary>
        /// Represents the configuration of a TOU Season.  See each meter's
        /// CONFIGURATION_DATA.doc, WindChill documents 
        /// #JBAKER01192005-1 (Image), #D0209235 (Sentinel).
        /// </summary>
        public class TOU_Season
        {
            #region Constants
            // The mask for an individual item in the Day to Daytype bitfield
            private const ushort DAY_TO_DAY_MASK = 0x00003;

            // The offsets for each day in the Day to Daytype bitfield
            private const int SUNDAY_OFFSET = 0;
            private const int MONDAY_OFFSET = 2;
            private const int TUESDAY_OFFSET = 4;
            private const int WEDNESDAY_OFFSET = 6;
            private const int THURSDAY_OFFSET = 8;
            private const int FRIDAY_OFFSET = 10;
            private const int SATURDAY_OFFSET = 12;
            private const int HOLIDAY_OFFSET = 14;

            // The masks for each of the days in the Day to Daytype bitfield
            private const ushort SUNDAY_MASK = DAY_TO_DAY_MASK << SUNDAY_OFFSET;
            private const ushort MONDAY_MASK = DAY_TO_DAY_MASK << MONDAY_OFFSET;
            private const ushort TUESDAY_MASK = DAY_TO_DAY_MASK << TUESDAY_OFFSET;
            private const ushort WEDNESDAY_MASK = DAY_TO_DAY_MASK << WEDNESDAY_OFFSET;
            private const ushort THURSDAY_MASK = DAY_TO_DAY_MASK << THURSDAY_OFFSET;
            private const ushort FRIDAY_MASK = DAY_TO_DAY_MASK << FRIDAY_OFFSET;
            private const ushort SATURDAY_MASK = DAY_TO_DAY_MASK << SATURDAY_OFFSET;
            private const ushort HOLIDAY_MASK = DAY_TO_DAY_MASK << HOLIDAY_OFFSET;


            #endregion

            #region Public Methods
            /// <summary>
            /// Constructor
            /// </summary>
            public TOU_Season(int EventsPerDayType, int DayTypesPerSeason)
            {
                m_iEventsPerDayType = EventsPerDayType;
                m_iDaytypesPerSeason = DayTypesPerSeason;

                IsProgrammed = 0;
                Daytypes = 0;
                TimeOfDayEvents = new DayEvent[m_iDaytypesPerSeason, m_iEventsPerDayType];
            }

            /// <summary>
            /// Clears the data in preparation for writing
            /// </summary>
            public void Clear()
            {
                IsProgrammed = 0;
                Daytypes = 0;

                for (int i = 0; i < m_iDaytypesPerSeason; i++)
                {
                    for (int j = 0; j < m_iEventsPerDayType; j++)
                    {
                        TimeOfDayEvents[i, j].Event = 0;
                    }
                }
            }

            /// <summary>
            /// Sorts DayEvents by time
            /// </summary>
            ///         
            // Revision History	
            // MM/DD/YY who Version Issue# Description
            // -------- --- ------- ------ ---------------------------------------
            // 10/24/06 mcm 7.35.07 105    Added to support changes for SCR 105
            // 
            public void Sort()
            {
                DayEvent TempEvent;

                for (int DayTypeIndex = 0; DayTypeIndex < DAYTYPES_PER_SEASON; DayTypeIndex++)
                {
                    for (int EventIndex = 0; EventIndex < EVENTS_PER_DAYTYPE; EventIndex++)
                    {
                        // Checks for out of order events
                        while ((EventIndex > 0) &&
                               (TimeOfDayEvents[DayTypeIndex, EventIndex] <
                                TimeOfDayEvents[DayTypeIndex, EventIndex - 1]))
                        {
                            // swap 'em
                            TempEvent = TimeOfDayEvents[DayTypeIndex, EventIndex - 1];
                            TimeOfDayEvents[DayTypeIndex, EventIndex - 1] =
                                TimeOfDayEvents[DayTypeIndex, EventIndex];
                            TimeOfDayEvents[DayTypeIndex, EventIndex] = TempEvent;


                            // On loop back check the order of swapped event against 
                            // its new previous event 
                            EventIndex--;
                        }
                    }
                }
            }

            #endregion

            #region Public Properties
            /// <summary>
            /// Gets the Number of daytypes per Season
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  10/19/06 KRC 7.36.00
            //
            public int DayTypesPerSeason
            {
                get
                {
                    return m_iDaytypesPerSeason;
                }
            }

            /// <summary>
            /// Gets the Number of Events per daytype
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  10/19/06 KRC 7.36.00
            //
            public int EventsPerDayType
            {
                get
                {
                    return m_iEventsPerDayType;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Sunday.
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalSunday
            {
                get
                {
                    return ((Daytypes & SUNDAY_MASK) >> SUNDAY_OFFSET);
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Monday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalMonday
            {
                get
                {
                    return (Daytypes & MONDAY_MASK) >> MONDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Tuesday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalTuesday
            {
                get
                {
                    return (Daytypes & TUESDAY_MASK) >> TUESDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Wednesday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalWednesday
            {
                get
                {
                    return (Daytypes & WEDNESDAY_MASK) >> WEDNESDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Thursday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalThursday
            {
                get
                {
                    return (Daytypes & THURSDAY_MASK) >> THURSDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Friday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalFriday
            {
                get
                {
                    return (Daytypes & FRIDAY_MASK) >> FRIDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Saturday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalSaturday
            {
                get
                {
                    return (Daytypes & SATURDAY_MASK) >> SATURDAY_OFFSET;
                }
            }

            /// <summary>
            /// Gets the daytype index for the typical Holiday
            /// </summary>
            //  Revision History
            //  MM/DD/YY Who Version Issue# Description
            //  -------- --- ------- ------ ---------------------------------------------
            //  06/05/07 RCG 8.10.06        Created

            public int TypicalHoliday
            {
                get
                {
                    return (Daytypes & HOLIDAY_MASK) >> HOLIDAY_OFFSET;
                }
            }

            #endregion

            #region Member Variables

            /// <summary>
            /// Is this season used?
            /// </summary>
            public byte IsProgrammed;

            /// <summary>
            /// Days of the week to DayType assignments. Two bits used per day
            /// plus the holiday
            /// </summary>
            public ushort Daytypes;

            /// <summary>
            /// Time of day events - switchpoints
            /// </summary>
            public DayEvent[,] TimeOfDayEvents;

            private int m_iEventsPerDayType;
            private int m_iDaytypesPerSeason;

            #endregion

        } // TOUSeason class

        /// <summary>
        /// Time of Day events (TOU Pattern). Rate change and output events.
        /// See the TIME_MAN_DESIGN.doc, WindChill document #D0209255 for 
        /// more info.
        /// </summary>
        public struct DayEvent
        {
            #region Constants

            /// <summary>The Event Mask</summary>
            public const ushort EVENT_MASK = 0x000F;
            private const ushort MINUTE_MASK = 0x03F0;
            private const ushort HOUR_MASK = 0x7C00;
            private const ushort HOUR_SHIFT = 0x0400;
            private const ushort MINUTE_SHIFT = 0x0010;

            #endregion

            #region Definitions

            /// <summary>
            /// Enumerated values defined for bits 0-3
            /// </summary>
            public enum TOUEvent
            {
                /// <summary>No More Changes</summary>
                NoMoreChanges = 0,
                /// <summary>Rate A</summary>
                RateA = 1,
                /// <summary>Rate B</summary>
                RateB = 2,
                /// <summary>Rate C</summary>
                RateC = 3,
                /// <summary>Rate D</summary>
                RateD = 4,
                /// <summary>Rate E</summary>
                RateE = 5,
                /// <summary>Rate F</summary>
                RateF = 6,
                /// <summary>Rate G</summary>
                RateG = 7,
                /// <summary>Output 1</summary>
                Output1 = 8,
                /// <summary>Output 2</summary>
                Output2 = 9,
                /// <summary>Output 3</summary>
                Output3 = 10,
                /// <summary>Output 4</summary>
                Output4 = 11,
                /// <summary>Output 1 Off</summary>
                Output1Off = 12,
                /// <summary>Output 2 Off</summary>
                Output2Off = 13,
                /// <summary>Output 3 Off</summary>
                Output3Off = 14,
                /// <summary>Output 4 Off</summary>
                Output4Off = 15,
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="EventType">Event type - DST start/stop, holiday,
            /// season change</param>
            /// <param name="Hour">0 based Hour the event starts (0..23)</param>
            /// <param name="Minute">0 based minute the event starts (0..59)</param>
            public DayEvent(TOUEvent EventType, byte Hour, byte Minute)
            {
                Event = (ushort)((ushort)EventType +
                                 (ushort)(Hour * HOUR_SHIFT) +
                                 (ushort)(Minute * MINUTE_SHIFT));
            }

            /// <summary>
            /// Constructor - Take Event from Meter
            /// </summary>
            /// <param name="usEvent">Meters Event</param>
            public DayEvent(ushort usEvent)
            {
                Event = usEvent;
            }

            /// <summary>
            /// Overloaded less than operator.
            /// </summary>
            /// <param name="firstEvent">The left hand side.</param>
            /// <param name="secondEvent">The right hand side.</param>
            /// <returns>A boolean indicating the result of the comparison.</returns>
            /// MM/DD/YY who Version Issue# Description
            /// -------- --- ------- ------ ---------------------------------------
            /// 06/06/06 jrf 7.30.00  N/A   Created
            ///
            public static bool operator <(DayEvent firstEvent, DayEvent secondEvent)
            {
                bool bReturn = false;

                if ((firstEvent.EventType == (byte)TOUEvent.NoMoreChanges) ||
                    (secondEvent.EventType == (byte)TOUEvent.NoMoreChanges) ||
                    (firstEvent.Hour > secondEvent.Hour))
                {
                    bReturn = false;
                }
                else if (firstEvent.Hour < secondEvent.Hour)
                {
                    bReturn = true;
                }
                else if ((firstEvent.Hour == secondEvent.Hour) &&
                         (firstEvent.Minute < secondEvent.Minute))
                {
                    bReturn = true;
                }

                return bReturn;

            }// End operator<

            /// <summary>
            /// Overloaded greater than operator.
            /// </summary>
            /// <param name="firstEvent">The left hand side.</param>
            /// <param name="secondEvent">The right hand side.</param>
            /// <returns>A boolean indicating the result of the comparison.</returns>
            /// MM/DD/YY who Version Issue# Description
            /// -------- --- ------- ------ ---------------------------------------
            /// 06/06/06 jrf 7.30.00  N/A   Created
            ///
            public static bool operator >(DayEvent firstEvent, DayEvent secondEvent)
            {
                bool bReturn = false;

                if ((firstEvent.EventType == (byte)TOUEvent.NoMoreChanges) ||
                    (secondEvent.EventType == (byte)TOUEvent.NoMoreChanges) ||
                    (firstEvent.Hour < secondEvent.Hour))
                {
                    bReturn = false;
                }
                else if (firstEvent.Hour > secondEvent.Hour)
                {
                    bReturn = true;
                }
                else if ((firstEvent.Hour == secondEvent.Hour) &&
                         (firstEvent.Minute > secondEvent.Minute))
                {
                    bReturn = true;
                }

                return bReturn;

            }// End operator>

            #endregion

            #region Public Properties

            /// <summary>
            /// Returns the hour the event starts
            /// </summary>
            public byte Hour
            {
                get
                {
                    return (byte)((Event & HOUR_MASK) >> 10);
                }
            }

            /// <summary>
            /// Returns the minute the event starts
            /// </summary>
            public byte Minute
            {
                get
                {
                    return (byte)((Event & MINUTE_MASK) >> 4);
                }
            }

            /// <summary>
            /// Returns the Event Type
            /// </summary>
            public byte EventType
            {
                get
                {
                    return (byte)((Event & EVENT_MASK));
                }
            }

            /// <summary>
            /// Returns a translated description for debugging
            /// </summary>
            public string Description
            {
                get
                {
                    string Desc = "0x" + Event.ToString("X4", CultureInfo.InvariantCulture) + ", ";
                    int Type = Event & EVENT_MASK;

                    // Add the type of event
                    switch (Type)
                    {
                        case (int)TOUEvent.NoMoreChanges:
                        {
                            Desc = Desc + "NoMoreChanges, ";
                            break;
                        }
                        case (int)TOUEvent.RateA:
                        {
                            Desc = Desc + "RateA, ";
                            break;
                        }
                        case (int)TOUEvent.RateB:
                        {
                            Desc = Desc + "RateB, ";
                            break;
                        }
                        case (int)TOUEvent.RateC:
                        {
                            Desc = Desc + "RateC, ";
                            break;
                        }
                        case (int)TOUEvent.RateD:
                        {
                            Desc = Desc + "RateD, ";
                            break;
                        }
                        case (int)TOUEvent.RateE:
                        {
                            Desc = Desc + "RateE, ";
                            break;
                        }
                        case (int)TOUEvent.RateF:
                        {
                            Desc = Desc + "RateF, ";
                            break;
                        }
                        case (int)TOUEvent.RateG:
                        {
                            Desc = Desc + "RateG, ";
                            break;
                        }
                        case (int)TOUEvent.Output1:
                        {
                            Desc = Desc + "Output1, ";
                            break;
                        }
                        case (int)TOUEvent.Output2:
                        {
                            Desc = Desc + "Output2, ";
                            break;
                        }
                        case (int)TOUEvent.Output3:
                        {
                            Desc = Desc + "Output3, ";
                            break;
                        }
                        case (int)TOUEvent.Output4:
                        {
                            Desc = Desc + "Output4, ";
                            break;
                        }
                        default:
                        {
                            Desc = Desc + "Type=" + Type.ToString("X2", CultureInfo.InvariantCulture) + ", ";
                            break;
                        }
                    }

                    Desc = Desc + Hour.ToString("d2", CultureInfo.InvariantCulture) + ":" +
                            Minute.ToString("d2", CultureInfo.InvariantCulture);

                    return Desc;
                }
            }

            #endregion
            
            #region Member Variables

            /// <summary>
            /// Packed bits describing A Time of Day event
            /// </summary>
            public ushort Event;

            #endregion

        } // TOUConfig.DayEvent 
        
        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  
        public TOUConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, TOU_CONFIG_SIZE)
        {
            InitializeData();
        }

        /// <summary>
        /// TOU Configuration Constructor for file based strucuture.
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        public TOUConfig(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(2048, TOU_CONFIG_SIZE)
        {
            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;

            InitializeData();

            ParseData();
        }

        /// <summary>
        /// TOU Configuration Constructor for file based strucuture.
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset"></param>
        /// <param name="ConfigSize">Size of the TOU config</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        public TOUConfig(PSEMBinaryReader BinaryReader, ushort TableID, ushort Offset, ushort ConfigSize)
            : base(TableID, ConfigSize)
        {
            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;

            InitializeData();

            ParseData();
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// <param name="ConfigSize">Size of Configuration.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/18/06 KRC 7.36.00 N/A	Created
        //  
        protected TOUConfig(CPSEM psem, ushort Offset, ushort ConfigSize)
            : base(psem, 2048, Offset, ConfigSize)
        {
            InitializeData();
        }

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="TableID">ID of this table</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// <param name="ConfigSize">Size of Configuration.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  01/10/14 jrf 3.50.23 TQ 9478  Created.
        // 
        protected TOUConfig(CPSEM psem, ushort TableID, ushort Offset, ushort ConfigSize)
            : base(psem, TableID, Offset, ConfigSize)
        {
            InitializeData();
        }

        /// <summary>
        /// Reads the TOU configuration component from table 2048 and 
        /// populates the TOUConfig fields with the values read
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "TOU.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;

        }


        /// <summary>
        /// Writes the TOU configuration portion of table 2048 to the meter
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  
        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "TOU.Write");

            // Have to resynch our members to the base's data array
            m_DataStream.Position = 0;

            for (int Index = 0; Index < m_Seasons.Length; Index++)
            {
                m_Writer.Write(m_Seasons[Index].IsProgrammed);
                m_Writer.Write(m_Seasons[Index].Daytypes);

                for (int DaytypeIndex = 0;
                    DaytypeIndex < m_Seasons[Index].DayTypesPerSeason; DaytypeIndex++)
                {
                    for (int EventIndex = 0;
                        EventIndex < m_Seasons[Index].EventsPerDayType; EventIndex++)
                    {
                        m_Writer.Write(
                            m_Seasons[Index].TimeOfDayEvents[DaytypeIndex,
                                                             EventIndex].Event);
                    }
                }
            }

            return base.Write();

        } // TOUConfig.Write

        /// <summary>
        /// Clears the TOUConfig table.  This is usually done prior to 
        /// reconfiguration, so unused portions don't explicitly have to be 
        /// updated
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  03/29/07 mcm 8.00.22 2746   Mark the table as dirty when it's cleared
        public void Clear()
        {
            for (int Index = 0; Index < m_Seasons.Length; Index++)
            {
                m_Seasons[Index].Clear();
            }

            // 03/29/07 mcm - SCR 2746 - Mark the table as dirty when it's 
            // cleared so properties don't reread it.
            m_TableState = TableState.Dirty;
        }

        /// <summary>
        /// Writes the m_Data contents to the logger for debugging
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/23/06 mcm 7.30.00 N/A	Created
        //  
        public override void Dump()
        {
            try
            {
				m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "Dump of TOUConfig Table ");

                for (int Index = 0; Index < m_Seasons.Length; Index++)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        "Season " + Index);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        "  IsProgrammed = " + m_Seasons[Index].IsProgrammed);
                    m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                        "   Daytypes = " + m_Seasons[Index].Daytypes.ToString("X2", CultureInfo.InvariantCulture));

                    for (int DaytypeIndex = 0;
                        DaytypeIndex < m_Seasons[Index].DayTypesPerSeason; DaytypeIndex++)
                    {
                        for (int EventIndex = 0;
                            EventIndex < m_Seasons[Index].EventsPerDayType; EventIndex++)
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                                "   DayType " + DaytypeIndex + " Event " + EventIndex + " = " +
                                m_Seasons[Index].TimeOfDayEvents[DaytypeIndex, EventIndex].Description);
                        }
                    }
                }

                m_Logger.WriteLine(Logger.LoggingLevel.Protocol,
                    "End Dump of TOUConfig Table ");
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
        } // TOUConfig.Dump

        #endregion public methods

        #region Public Properties
        /// <summary>
        /// Gets the array of Season Data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Created

        public TOU_Season[] Seasons
        {
            get
            {
                if (TableState.Unloaded == m_TableState)
                {
                    PSEMResponse Response;

                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Seasons");
                    }
                }

                return m_Seasons;
            }
        }

        /// <summary>
        /// Provides access to the number of Supported Seasons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public virtual uint NumberOfSupportedSeasons
        {
            get
            {
                return NUM_SUPPORTED_SEASONS;
            }
        }

        /// <summary>
        /// Provides access to the number of Day Types per season
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public virtual int DayTypesPerSeason
        {
            get
            {
                return DAYTYPES_PER_SEASON;
            }
        }

        /// <summary>
        /// Provides access to the number of Events Per day Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/04/07 RCG 8.00.04 N/A	Changed to a Public Property

        public virtual int EventsPerDayType
        {
            get
            {
                return EVENTS_PER_DAYTYPE;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Parses the Data during the read
        /// </summary>
        /// <exception>
        /// None.  This debugging method catches its exceptions
        /// </exception>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/23/06 KRC 7.36.00 N/A	Created
        //  11/15/13 jrf 3.50.04 TQ 9478 Modified to use Seasons property and to check 
        //                               if season programmed byte is available. 
        // 
        protected virtual void ParseData()
        {
            for (int Index = 0; Index < Seasons.Length; Index++)
            {
                if (true == m_blnHasSeasonProgrammedByte)
                {
                    Seasons[Index].IsProgrammed = m_Reader.ReadByte();
                }

                Seasons[Index].Daytypes = m_Reader.ReadUInt16();

                for (int DaytypeIndex = 0;
                    DaytypeIndex < Seasons[Index].DayTypesPerSeason; DaytypeIndex++)
                {
                    for (int EventIndex = 0;
                        EventIndex < Seasons[Index].EventsPerDayType; EventIndex++)
                    {
                        Seasons[Index].TimeOfDayEvents[DaytypeIndex,
                                                         EventIndex].Event
                                        = m_Reader.ReadUInt16();
                    }
                }
            }
        }

        /// <summary>
        /// Setup data items
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/18/06 KRC 7.36.00
        //  11/15/13 jrf 3.50.06 TQ 9478 Made protected virtual. 
        //
        protected virtual void InitializeData()
        {
            m_Seasons = new TOU_Season[NumberOfSupportedSeasons];
            for (int Index = 0; Index < m_Seasons.Length; Index++)
            {
                m_Seasons[Index] = new TOU_Season(EventsPerDayType, DayTypesPerSeason);
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// TOUConfig season configuration array
        /// </summary>
        protected TOU_Season[] m_Seasons;

        /// <summary>
        /// Paramter that determines if a season programmed byte is defined.
        /// </summary>
        protected bool m_blnHasSeasonProgrammedByte = true;

        #endregion

    } // TOUConfig class

    /// <summary>
    /// The DisplayConfig class represents the display configuration data in 2048.
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  10/02/06 KRC 7.35.00 N/A    Created
    //
    public class DisplayConfig : ANSISubTable
    {
        #region Constants

        private const ushort DISPLAY_CONFIG_SIZE = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        public DisplayConfig(CPSEM psem, ushort Offset)
            : this(psem, Offset, DISPLAY_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Constuctor for file based strucutre
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        /// <param name="Size"></param>
        public DisplayConfig(PSEMBinaryReader BinaryReader, ushort Offset, ushort Size)
            : base(2048, Size)
        {
            m_NormalDisplayData = null;
            m_AltDisplayData = null;
            m_TestDisplayData = null;

            m_Reader = BinaryReader;
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        protected DisplayConfig(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, 2048, Offset, Size)
        {
            m_NormalDisplayData = null;
            m_AltDisplayData = null;
            m_TestDisplayData = null;
        }


        /// <summary>
        /// Reads the Displaytconfig block out of table 2048 in the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            Result = base.Read();

            return Result;

        }

        /// <summary>
        /// Provides Access to the NonFatal Error Scroll and Lock bits
        /// </summary>
        /// MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  11/03/16 CFB 4.70.31 WR682857 Created
        public virtual byte[] NonFatalScrollLockBits()
        {
            return (m_ScrollLockNonFatal);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides Access to the Normal Display Data
        /// </summary>
        public List<ANSIDisplayData> NormalDisplayData
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    PSEMResponse Response = PSEMResponse.Ok;
                    // Read the subtable to get the Display Configuration
                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Display Configuration");
                    }
                }

                return m_NormalDisplayData;
            }
        }

        /// <summary>
        /// Provides Access to the Alt Display Data
        /// </summary>
        public List<ANSIDisplayData> AlternateDisplayData
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    PSEMResponse Response = PSEMResponse.Ok;
                    // Read the subtable to get the Display Configuration
                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Display Configuration");
                    }
                }

                return m_AltDisplayData;
            }
        }

        /// <summary>
        /// Provides Access to the Test Display Data
        /// </summary>
        public List<ANSIDisplayData> TestDisplayData
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    PSEMResponse Response = PSEMResponse.Ok;
                    // Read the subtable to get the Display Configuration
                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Display Configuration");
                    }
                }

                return m_TestDisplayData;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Builds up the Normal and Test Displays (Alt = null)
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/06 KRC 7.36.00 N/A    Created
        //  01/18/06 RCG 8.00.08        Updating for other ANSI devices

        protected virtual void BuildDisplayLists()
        {
            m_NormalDisplayData = new List<ANSIDisplayData>();
            m_AltDisplayData = new List<ANSIDisplayData>();
            m_TestDisplayData = new List<ANSIDisplayData>();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the reference time for the the display items
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/12/07 RCG 8.00.06 N/A    Created

        protected virtual DateTime ReferenceTime
        {
            get
            {
                return new DateTime(2000, 1, 1);
            }
        }

        #endregion

        #region Members

        /// <summary>Normal Display Data</summary>
        protected List<ANSIDisplayData> m_NormalDisplayData;
        /// <summary>Alternate Displaty Data</summary>
        protected List<ANSIDisplayData> m_AltDisplayData;
        /// <summary>Test Display Data</summary>
        protected List<ANSIDisplayData> m_TestDisplayData;
        /// <summary>Byte array of NonFatal Scroll Lock Bits</summary>
        protected byte[] m_ScrollLockNonFatal;

        #endregion

    }

    /// <summary>
    /// Display Configuration Shared Class
    /// </summary>
    public class DisplayConfig_Shared : DisplayConfig
    {
        #region Constants

        /// <summary>Display Configuration Size in Bytes</summary>
        public const int DISPLAY_CONFIG_SIZE = 814;
        private const int MAX_NUM_DISPLAY_ITEMS = 80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        public DisplayConfig_Shared(CPSEM psem, ushort Offset)
            : this(psem, Offset, DISPLAY_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Constructor for file based strucuture
        /// </summary>
        /// <param name="BinaryReader"></param>
        /// <param name="Offset"></param>
        public DisplayConfig_Shared(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(BinaryReader, Offset, DISPLAY_CONFIG_SIZE)
        {
            m_DisplayData = new List<ANSIDisplayData>();

            ParseData();
            BuildDisplayLists();
        }

        /// <summary>
        /// Constructor for the Display Configuration piece of 2048.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.36.00 N/A    Created
        // 
        protected DisplayConfig_Shared(CPSEM psem, ushort Offset, ushort Size)
            : base(psem, Offset, Size)
        {
            m_DisplayData = new List<ANSIDisplayData>();
        }

        /// <summary>
        /// Reads the Displaytconfig block out of table 2048 in the meter.
        /// </summary>
        /// <returns>A PSEMResponse encapsulating the layer 7 response to the 
        /// layer 7 request. (PSEM errors).</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        // 
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "DisplayConfig.Read");

            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                    "DisplayConfig.Read succeeded");

                m_DataStream.Position = 0;

                ParseData();
            }

            BuildDisplayLists();

            return Result;

        } // DisplayConfig.Read

        #endregion

        #region Protected Methods

        /// <summary>
        /// Builds up the Normal and Test Displays (Alt = null)
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/06 KRC 7.36.00 N/A    Created
        //  01/18/06 RCG 8.00.08        Updating for other ANSI devices
        //  02/26/07 KRC 8.00.14        Adding Editable List
        //
        protected override void BuildDisplayLists()
        {
            int iIndex = 0;

            base.BuildDisplayLists();

            // Now that we have all of the data, we can build the individula display lists.
            for (iIndex = m_byNormalStart; iIndex <= m_byNormalEnd; iIndex++)
            {   
                m_NormalDisplayData.Add(m_DisplayData[iIndex]);
            }

            for (iIndex = m_byAltStart; iIndex <= m_byAltEnd; iIndex++)
            {
                m_AltDisplayData.Add(m_DisplayData[iIndex]);
            }

            for (iIndex = m_byTestStart; iIndex <= m_byTestEnd; iIndex++)
            {
                m_TestDisplayData.Add(m_DisplayData[iIndex]);
            }
        }

        /// <summary>
        /// Creates and returns byte array containing Scroll and Lock Bits for NonFatal errors
        /// </summary>
        /// MM/DD/YY Who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  11/03/16 CFB 4.70.31 WR682857 Created
        public override byte[] NonFatalScrollLockBits()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            if (TableState.Unloaded == m_TableState)
            {
                //Read Table
                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading NonFatal Error Control Bits"));
                }
            }
            m_ScrollLockNonFatal = new byte[] {m_byLockNonFatal1, m_byScrollNonFatal1, m_byLockNonFatal2, m_byScrollNonFatal2};
            return (m_ScrollLockNonFatal);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data out of the Binary Reader and into the member variables
        /// </summary>
        private void ParseData()
        {
            //Populate the member variables that represent the R300 config
            m_byDisplayTime = m_Reader.ReadByte();
            m_byNormalStart = m_Reader.ReadByte();
            m_byNormalEnd = m_Reader.ReadByte();
            m_byAltStart = m_Reader.ReadByte();
            m_byAltEnd = m_Reader.ReadByte();
            m_byTestStart = m_Reader.ReadByte();
            m_byTestEnd = m_Reader.ReadByte();
            m_byDisplayControl = m_Reader.ReadByte();
            m_byScrollNonFatal1 = m_Reader.ReadByte();
            m_byLockNonFatal1 = m_Reader.ReadByte();
            m_byScrollNonFatal2 = m_Reader.ReadByte();
            m_byLockNonFatal2 = m_Reader.ReadByte();
            m_byScrollDiag = m_Reader.ReadByte();
            m_byLockDiag = m_Reader.ReadByte();

            // Make sure we start with a clean Display List
            m_DisplayData.Clear();

            for (int iIndex = 0; iIndex < MAX_NUM_DISPLAY_ITEMS; iIndex++)
            {
                UInt32 uiLID = m_Reader.ReadUInt32();
                string strDispID = m_Reader.ReadString(3);
                char[] charTrim = { '\0' };
                strDispID = strDispID.Trim(charTrim);
                ushort usFormat = m_Reader.ReadUInt16();
                byte byDim = m_Reader.ReadByte();
                m_DisplayData.Add(new ANSIDisplayData(uiLID, strDispID, usFormat, byDim));
            }
        }

        #endregion

        #region Members

        /// <summary>Display data</summary>
        protected List<ANSIDisplayData> m_DisplayData;

        /// <summary>Display Time</summary>
        protected byte m_byDisplayTime;
        /// <summary>Normal Start Index</summary>
        protected byte m_byNormalStart;
        /// <summary>Normal End Index</summary>
        protected byte m_byNormalEnd;
        /// <summary>Alternate List Start Index</summary>
        protected byte m_byAltStart;
        /// <summary>Alternate List End Index</summary>
        protected byte m_byAltEnd;
        /// <summary>Test List Start Index</summary>
        protected byte m_byTestStart;
        /// <summary>Test List End Index</summary>
        protected byte m_byTestEnd;
        /// <summary>Display Control Byte</summary>
        protected byte m_byDisplayControl;
        /// <summary>Scroll NonFatal Error 1 Settings</summary>
        protected byte m_byScrollNonFatal1;
        /// <summary>Scroll NonFatal Error 2 Settings</summary>
        protected byte m_byScrollNonFatal2;
        /// <summary>Lock NonFatal Error 1 Settings</summary>
        protected byte m_byLockNonFatal1;
        /// <summary>Lock NonFatal Error 2 Settings</summary>
        protected byte m_byLockNonFatal2;
        /// <summary>Scroll Diagnostic Settings</summary>
        protected byte m_byScrollDiag;
        /// <summary>Lock Diagnostic Settings</summary>
        protected byte m_byLockDiag;

        #endregion
    }

    /// <summary>
    /// The HistoryLogConfig class represents the history log configuration in 2048
    /// </summary>
    /// <remarks>
    /// If you add events to this file, be sure to add them also to HistoryEvents in
    /// ANSIEventTables.cs
    /// </remarks>
    //  Revision History
    //  MM/DD/YY Who Version Issue#    Description
    //  -------- --- ------- ------    ---------------------------------------------
    //  10/31/06 AF  7.40.00 N/A       Created
    //  04/03/07 AF  8.00.24 2675      Added to the Event_16_31 enum to handle SiteScan
    //                                 error for OpenWay meters.
    //  07/12/16 MP  4.70.7  WR688986  Changed name of a couple of enums
    //  07/13/16 MP  4.70.7  WR688986  Changed LOSS_OF_PHASE and SITESCAN_ERROR_CLEARED to match history event enum names
    //  07/18/16 MP  4.70.8  WR600059  Added definition for event 137 (NETWORK_TIME_UNAVAILABLE)
    //  07/29/16 MP  4.70.11 WR704220  Added definition for events 213 and 214 (WRONG_CONFIG_CRC and CHECK_CONFIG_CRC)

    public class HistoryLogConfig : ANSISubTable
    {
        #region Constants

        /// <summary>
        /// Size of Event Configuration
        /// </summary>
        public const ushort EVENT_CONFIG_SIZE = 34;

        #endregion

        #region Definitions

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_0_15
        {
            /// <summary>Power Outage - Index 1</summary>
            PRIMARY_POWER_DOWN = 0x02,  
            /// <summary>Power Restored - Index 2</summary>
            PRIMARY_POWER_UP = 0x04,
            /// <summary>Clear Billing Data - Index 3</summary>
            BILLING_DATA_CLEARED = 0x08,
            /// <summary>Billing Schedule Expirted - Index 4</summary>
            BILLING_SCHED_EXPIRED = 0x10,
            /// <summary>DST Time Change - Index 5</summary>
            DST_TIME_CHANGE = 0x20,
            /// <summary>Clock Reset - Index 6</summary>
            CLOCK_RESET = 0x40, 
            /// <summary>Demand Threshold Exceeded - Index 7</summary>
            DEMAND_THRESHOLD_EXCEEDED = 0x80,
            /// <summary>Demand Threshold Restored - Index 8</summary>
            DEMAND_THRESHOLD_RESTORED = 0x0100,
            /// <summary>Logon Successful - Index 10</summary>
            LOGON_SUCCESSFUL = 0x0400,
            /// <summary>Security Successful - Index 12</summary>
            SECURITY_SUCCESSFUL = 0x1000, 
            /// <summary>Security Failed - Index 13</summary>
            SECURITY_FAILED = 0x2000, 
            /// <summary>Load Profile Reset - Index 14</summary>
            LOAD_PROFILE_RESET = 0x4000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_16_31
        {
            /// <summary>History Log Cleared - Index 16</summary>
            HIST_LOG_CLEARED = 0x01,
            /// <summary>History Pointers Updated - Index 17</summary>
            HIST_PTRS_UPDATED = 0x02,
            /// <summary>Event Log Cleared - Index 18</summary>
            EVENT_LOG_CLEARED = 0x04,   
            /// <summary>Event Log Pointers Updated - Index 19</summary>
            EVENT_LOG_PTRS_UPDATED = 0x08,
            /// <summary>Demand Reset - Index 20</summary>
            DEMAND_RESET = 0x10,    
            /// <summary>Self Read Occurred - Index 21</summary>
            SELF_READ_OCCURRED = 0x20, 
            /// <summary>Input Channel Hi - Index 22</summary>
            INPUT_CHANNEL_HI = 0x40,
            /// <summary>Input Channel Low - Index 23</summary>
            INPUT_CHANNEL_LO = 0x80,
            /// <summary>TOU Season Changed - Index 24</summary>
            TOU_SEASON_CHANGED = 0x100,    
            /// <summary>TOU Rate Change - Index 25</summary>
            RATE_CHANGE = 0x200, 
            /// <summary>External Event - Index 26</summary>
            EXTERNAL_EVENT = 0x400,
            /// <summary>SiteScan for AMI; Custom Schedule Changed - V and I and C12.19 - Index 27</summary>
            SITESCAN_ERROR_OR_CUST_SCHED_CHANGED_EVT = 0x800,
            /// <summary>Pending table Activation - Index 28</summary>
            PENDING_TABLE_ACTIVATION = 0x1000,
            /// <summary>SiteScan for Non-AMI; Pending Table Clear for AMI - Index 29</summary>
            PENDING_TABLE_CLEAR = 0x2000,   
            /// <summary>VQ Log Pointers Updated - Index 30</summary>
            VQ_LOG_PTRS_UPDATED = 0x4000,  
            /// <summary>VQ Log Nearly Full - Index 31</summary>
            VQ_LOG_NEARLY_FULL = 0x8000,    
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_32_47
        {
            /// <summary>Enter Test Mode - Index 32</summary>
            ENTER_TEST_MODE = 0x01,
            /// <summary>Exit Test Mode - Index 33</summary>
            EXIT_TEST_MODE = 0x02,  
            /// <summary>ABC Phase Rotation Active - Index 34</summary>
            ABC_PH_ROTATION_ACTIVE = 0x04,  
            /// <summary>CBA Phase Rotation Active - Index 35</summary>
            CBA_PH_ROTATION_ACTIVE = 0x08, 
            /// <summary>Meter Reprogrammed - Index 36</summary>
            METER_REPROGRAMMED = 0x10,
            /// <summary>Illegal Configuration Error - Index 37</summary>
            ILLEGAL_CONFIG_ERROR = 0x20,   
            /// <summary>CPC Communication Error - Index 38</summary>
            CPC_COMM_ERROR = 0x40,
            /// <summary>Reverse Rotation Restored - Index 39</summary>
            REVERSE_ROTATION_RESTORE = 0x80,
            /// <summary>VQ Log Cleared - Index 40</summary>
            VQ_LOG_CLEARED = 0x100, 
            /// <summary>TOU Schedule Error - Index 41</summary>
            TOU_SCHEDULE_ERROR = 0x200, 
            /// <summary>Mass Memory Error - Index 42</summary>
            MASS_MEMORY_ERROR = 0x400,
            /// <summary>Loss of Phase Restore - Index 43</summary>
            LOSS_OF_PHASE_RESTORE = 0x800,
            /// <summary>Low Battery - Index 44</summary>
            LOW_BATTERY = 0x1000,  
            /// <summary>Loss of Voltage phase A or Loss of Phase for OpenWay- Index 45</summary>
            LOSS_OF_PHASE = 0x2000,    
            /// <summary>Register Full Scale - Index 46</summary>
            REGISTER_FULL_SCALE = 0x4000,   
            /// <summary>Reverse Power Flow Restore - Index 47</summary>
            REVERSE_POWER_FLOW_RESTORE = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_48_63
        {
            /// <summary>Reverse Power Flow - Index 48</summary>
            REVERSE_POWER_FLOW = 0x01,
            /// <summary>Site Scan Diagnostic 1 Active - Index 49</summary>
            SS_DIAG1_ACTIVE = 0x02,    
            /// <summary>Site Scan Diagnostic 2 Active - Index 50</summary>
            SS_DIAG2_ACTIVE = 0x04,    
            /// <summary>Site Scan Diagnostic 3 Active - Index 51</summary>
            SS_DIAG3_ACTIVE = 0x08,     
            /// <summary>Site Scan Diagnostic 4 Active - Index 52</summary>
            SS_DIAG4_ACTIVE = 0x10,    
            /// <summary>Site Scan Diagnostic 5 Active - Index 53</summary>
            SS_DIAG5_ACTIVE = 0x20,     
            /// <summary>Site Scan Diagnostic 1 Inactive - Index 54</summary>
            SS_DIAG1_INACTIVE = 0x40,   
            /// <summary>Site Scan Diagnostic 2 Inactive - Index 55</summary>
            SS_DIAG2_INACTIVE = 0x80,   
            /// <summary>Site Scan Diagnostic 3 Inactive - Index 56</summary>
            SS_DIAG3_INACTIVE = 0x100,   
            /// <summary>Site Scan Diagnostic 4 Inactive - Index 57</summary>
            SS_DIAG4_INACTIVE = 0x200,   
            /// <summary>Site Scan Diagnostic 5 Inactive - Index 58</summary>
            SS_DIAG5_INACTIVE = 0x400,   
            /// <summary>Site Scan Diagnostic 6 Active - Index 59</summary>
            SS_DIAG6_ACTIVE = 0x800,     
            /// <summary>Site Scan Diagnostic 6 Inactive - Index 60</summary>
            SS_DIAG6_INACTIVE = 0x1000,  
            /// <summary>Self Read Cleared - Index 61</summary>
            SELF_READ_CLEARED = 0x2000,
            /// <summary>Inversion Tamper - Index 62</summary>
            INVERSION_TAMPER = 0x4000,  
            /// <summary>Removal tamper - Index 63</summary>
            REMOVAL_TAMPER = 0x8000,    
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_64_79
        {
            /// <summary>Register Firmware Download Failed - Index 66</summary>
            REG_DWLD_FAILED = 0x04, 
            /// <summary>Register Firmware Download Succeeded - Index 67</summary>
            REG_DWLD_SUCCEEDED = 0x08,
            /// <summary>RFLAN Firmware Download Succeeded - Index 68</summary>
            RFLAN_DWLD_SUCCEEDED = 0x10,   
            /// <summary>ZigBee Firmware Download Succeeded - Index 69</summary>
            ZIGBEE_DWLD_SUCCEEDED = 0x20,   
            /// <summary>Display Firmware Download Succeeded - Index 72</summary>
            METER_FW_DWLD_SUCCEDED = 0x100,
            /// <summary>Display Firmware Download Failed - Index 73</summary>
            METER_DWLD_FAILED = 0x200,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_80_95
        {
            /// <summary>ZigBee Firmware Download Failed - Inxed 81</summary>
            ZIGBEE_DWLD_FAILED = 0x02,  
            /// <summary>RFLAN Firmware Download Failed - Index 82</summary>
            RFLAN_DWLD_FAILED = 0x04, 
            /// <summary>SiteScan Error Cleared - Index 84</summary>
            SITESCAN_ERROR_CLEARED = 0x10,
            /// <summary>Load Firmare - Index 85</summary>
            LOAD_FIRMWARE = 0x20,   
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_96_111
        {
            /// <summary>Reset Counter - Index 101</summary>
            RESET_COUNTERS = 0x20,  
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version    Issue#        Description
        //  -------- --- -------    ------        -------------------------------------------
        //  10/26/16 jrf 4.70.28    WR230427      Added missing event definition for PERIODIC_READ(125).
        protected enum Event_112_127
        {
            /// <summary>Fatal Error - Index 121</summary>
            FATAL_ERROR = 0x200,
            /// <summary>Periodic Read Occurred - Index 125</summary>
            PERIODIC_READ = 0x2000,
            /// <summary>Service Limiting Active Tier Changed - Index 126</summary>
            SERVICE_LIMITING_ACTIVE_TIER_CHANGED = 0x4000,
            
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_128_143
        {
            /// <summary>Table Written</summary>
            TABLE_WRITTEN = 0x4,
            /// <summary>Base Mode Error</summary>
            BASE_MODE_ERROR = 0x8,
            /// <summary>Pending Table Activated</summary>
            PENDING_RECONFIGURE = 0x40,
            /// <summary>Magnetic Tamper Detected - Index 135</summary>
            MAGNETIC_TAMPER_DETECTED = 0x80,
            /// <summary>Magnetic Tamper Cleared - Index 136</summary>
            MAGNETIC_TAMPER_CLEARED = 0x100,
            /// <summary>Network Time Unavailable - Index 137</summary>
            NETWORK_TIME_UNAVAILABLE = 0x200,
            /// <summary>Current Threshold Exceeded - Index 140</summary>
            CTE_EVENT = 0x1000,
            /// <summary>Event Tamper Cleared - Index 141</summary>
            EVENT_TAMPER_CLEARED = 0x2000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_144_159
        {
            /// <summary>HAN/LAN Log Reset - Index 148</summary>
            LAN_HAN_LOG_RESET = 0x10,
            /// <summary>Display Firmware Download Initiated - Index 151</summary>
            DISP_DWLD_INITIATED = 0x80,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        // 04/21/16 AF  4.50.252 WR604349 Changed HAN_LOAD_CONTROL_EVENT_SENT to ERT_242_COMMAND_REQUEST
        //
        protected enum Event_160_175
        {
            /// <summary>Pending Table Activate Failed</summary>
            PENDING_TABLE_ACTIVATE_FAIL = 0x02,
            /// <summary>HAN Device Status Change</summary>
            HAN_DEVICE_STATUS_CHANGE = 0x04,
            /// <summary>ERT 242 Command Request</summary>
            ERT_242_COMMAND_REQUEST = 0x08,
            /// <summary>HAN Load Control Event Status</summary>
            HAN_LOAD_CONTROL_EVENT_STATUS = 0x10,
            /// <summary>HAN Load Control Event Opt Out</summary>
            HAN_LOAD_CONTROL_EVENT_OPT_OUT = 0x20,
            /// <summary>HAN Messaging Event</summary>
            HAN_MESSAGING_EVENT = 0x40,
            /// <summary>HAN Device Added or Removed</summary>
            HAN_DEVICE_ADDED_OR_REMOVED = 0x80,
            /// <summary>Display Firmware Download Initiation Failed - Index 168</summary>
            DISP_DWLD_INITIATION_FAILED = 0x100,
            /// <summary>Register Firmware Download Initiated - Index 169</summary>
            REG_DWLD_INITIATED = 0x200, 
            /// <summary>RFLAN Firmware Download Initiated - Index 170</summary>
            RFLAN_DWLD_INITIATED = 0x400,   
            /// <summary>ZigBee Firmware Download Initiated - Index 171</summary>
            ZIGBEE_DWLD_INITIATED = 0x800,  
            /// <summary>Register Firmware Download Initiation Failed - Index 172</summary>
            REG_DWLD_INITIATION_FAILED = 0x1000,   
            /// <summary>RFLAN Firmware Download Initiation Failed - Index 173</summary>
            RFLAN_DWLD_INITIATION_FAILED = 0x2000,
            /// <summary>ZigBee Firmware Download Initiation Failed - Index 174</summary>
            ZIGBEE_DWLD_INITIATION_FAILED = 0x4000, 
            /// <summary>Firmware Download Event Log Full - Index 175</summary>
            FW_DWLD_EVENT_LOG_FULL = 0x8000,    
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_176_191
        {
            /// <summary>RFLAN Firmawre Download Status - Index 176</summary>
            RFLAN_FW_DWLD_STATUS = 0x01,   
            /// <summary>ZigBee Firmware Download Status - Index 177</summary>
            ZIGBEE_FW_DWLD_STATUS = 0x02,  
            /// <summary>Register Firmware Download Alreaday Active - Index 178</summary>
            REG_DWLD_ALREADY_ACTIVE = 0x04,
            /// <summary>RFLAN Firmware Download Already Active - Index 179</summary>
            RFLAN_DWLD_ALREADY_ACTIVE = 0x08, 
            /// <summary>Extended Outage Recovery Mode Entered - Index 180</summary>
            EXTENDED_OUTAGE_RECOVERY_MODE_ENTERED = 0x10,
            /// <summary>Third Party HAN Device Firmware Download Status - Index 181</summary>
            THIRD_PARTY_HAN_FW_DWLD_STATUS = 0x20,
            /// <summary>Firmware Download Abort - Index 184</summary>
            FW_DWLD_ABORT = 0x100,
            /// <summary>Remote Connect Failed - Index 185</summary>
            REMOTE_CONNECT_FAILED = 0x200, 
            /// <summary>Remote Disconnected Failed - Index 196</summary>
            REMOTE_DISCONNECT_FAILED = 0x400,   
            /// <summary>Remote Disconnect Relay Activated - Index 187</summary>
            REMOTE_DISCONNECT_RELAY_ACTIVATED = 0x800, 
            /// <summary>Remote Connect Relay Activated - Index 188</summary>
            REMOTE_CONNECT_RELAY_ACTIVATED = 0x1000,    
            /// <summary>Remote Connect Relay Initiated - Index 189/// </summary>
            REMOTE_CONNECT_RELAY_INITIATED = 0x2000, 
            /// <summary>Table Write for Asset Synchronization - Index 191</summary>
            TABLE_CONFIGURATION = 0x4000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_192_207
        {
            /// <summary>Critical Peak Pricing - Index 192</summary>
            CPP_EVENT = 0x01,
            /// <summary>VRMS Voltage Goes From Below Low To Normal - Index 194</summary>
            RMS_VOLTAGE_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL = 0x04,
            /// <summary>VRMS Voltage Goes From Above High To Normal - Index 195</summary>
            RMS_VOLTAGE_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL = 0x08,
            /// <summary>Vh Goes From Below Low To Normal - Index 196</summary>
            VOLT_HOUR_FROM_BELOW_LOW_THRESHOLD_TO_NORMAL = 0x10,
            /// <summary>Vh Goes From Above High To Normal - Index 197</summary>
            VOLT_HOUR_FROM_ABOVE_HIGH_THRESHOLD_TO_NORMAL = 0x20,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_208_223
        {
            /// <summary>WRONG_CONFIG_CRC - Index 213</summary>
            WRONG_CONFIG_CRC = 0x20,
            /// <summary>CHECK_CONFIG_CRC - Index 214</summary>
            CHECK_CONFIG_CRC = 0x40,
            /// <summary>Hardware Error - Index 215</summary>
            EVENT_HARDWARE_ERROR_DETECTION = 0x80,
            /// <summary>Temperature exceeds threshold 1 - Index 223/// </summary>
            TEMPERATURE_EXCEEDS_THRESHOLD1 = 0x8000,  
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_224_239
        {
            /// <summary>Temperature exceeds threshold 2 - Index 224</summary>
            TEMPERATURE_EXCEEDS_THRESHOLD2 = 0x01,
            /// <summary>Temperature returned to normal - Index 225</summary>
            TEMPERATURE_RETURNED_TO_NORMAL = 0x02,
            /// <summary>Network Hush Started - Index 228</summary>
            NETWORK_HUSH_STARTED = 0x10,
            /// <summary>Load Voltage Preset - Index 230</summary>
            LOAD_VOLT_PRESENT = 0x40,          
            /// <summary>Firmware Download Aborted - Index 231</summary>
            PENDING_TABLE_CLEAR_FAIL = 0x80,         
            /// <summary>Pending Table Full - Index 232</summary>
            FIRMWARE_PENDING_TABLE_FULL = 0x100,       
            /// <summary>Pending Table Swap - Index 233</summary>
            FIRMWARE_PENDING_TABLE_HEADER_SWAPPED = 0x200,       
            /// <summary>Event Scheduling Rejected - Index 234</summary>
            EVENT_SCHEDULING_REJECTED = 0x400,      
            /// <summary>C12.22 Registration Attempt - Index 235</summary>
            C12_22_REGISTRATION_ATTEMPT = 0x800,    
            /// <summary>C12.22 Registered - Index 236</summary>
            C12_22_REGISTERED = 0x1000,      
            /// <summary>C12.22 Deregistration Attempt - Index 237</summary>
            C12_22_DEREGISTRATION_ATTEMPT = 0x2000, 
            /// <summary>C12.22 Deregistered - Index 238</summary>
            C12_22_DEREGISTERED = 0x4000,        
            /// <summary>C12.22 RFLAN Cell ID Change - Index 239/// </summary>
            C12_22_RFLAN_CELL_ID_CHANGE = 0x8000,   
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event
        /// </summary>
        protected enum Event_240_255
        {
            /// <summary>Time Adjustment Failed</summary>
            TIME_ADJUSTMENT_FAILED = 0x01,
            /// <summary>Event Cache Overflow - Index 241</summary>
            EVENT_CACHE_OVERFLOW = 0x02,
            /// <summary>On Demand Periodic Read - Index 244</summary>
            ON_DEMAND_PERIODIC_READ = 0X10,
            /// <summary>Generic history log event. MSB in argument defines type</summary>
            EVENT_GENERIC_HISTORY_EVENT = 0x20,
            /// <summary>RMS Voltage Bleow Low Threshold</summary>
            RMS_VOLTAGE_BELOW_LOW_THRESHOLD = 0x40,
            /// <summary>Volt(RMS) Above Threshold</summary>
            VOLT_RMS_ABOVE_THRESHOLD = 0x80,
            /// <summary>Volt Hour Below Low Threshold</summary>
            VOLT_HOUR_BELOW_LOW_THRESHOLD = 0x100,
            /// <summary>VOlt Hour Above Threshold</summary>
            VOLT_HOUR_ABOVE_THRESHOLD = 0x200,
            /// <summary>Pending Table Error</summary>
            PENDING_TABLE_ERROR = 0x400,
            /// <summary>Security Event</summary>
            SECURITY_EVENT = 0x1000,
            /// <summary>Key Rollover Pass</summary>
            KEY_ROLLOVER_PASS = 0x2000,
            /// <summary>Sign Key Rpelace Processing Pass</summary>
            SIGN_KEY_REPLACE_PROCESSING_PASS = 0x4000,
            /// <summary>Symmetric Key Replace Processing Pass</summary>
            SYMMETRIC_KEY_REPLACE_PROCESSING_PASS = 0x8000,
        }

        /// <summary>
        /// Each item in the enum is a bit mask for identifying whether or
        /// not the meter has been configured to record the event. This range is
        /// for the M2 Gateway only
        /// </summary>
        protected enum Event_256_271
        {
            /// <summary>Gateway Configuration Download</summary>
            GW_CONFIGURATION_DOWNLOAD = 0x01,
        }

        /// <summary>
        /// The IDs associated with each event
        /// </summary>
        protected enum EventID: int
        {
            /// <summary>Power Outage - Index 1</summary>
            POWER_OUTAGE = 1,
            /// <summary>Power Restored - Index 2</summary>
            POWER_RESTORED = 2,
            /// <summary>Clear Billing Data - Index 3</summary>
            CLEAR_BILLING_DATA = 3,
            /// <summary>Billing Schedule Expirted - Index 4</summary>
            BILLING_SCHED_EXPIRED = 4,
            /// <summary>DST Time Change - Index 5</summary>
            DST_TIME_CHANGE = 5,
            /// <summary>Clock Reset - Index 6</summary>
            CLOCK_RESET = 6,
            /// <summary>Demand Threshold Exceeded - Index 7</summary>
            DEMAND_THRESHOLD_EXCEEDED = 7,
            /// <summary>Demand Threshold Restored - Index 8</summary>
            DEMAND_THRESHOLD_RESTORED = 8,
            /// <summary>Logon Successful - Index 10</summary>
            LOGON_SUCCESSFUL = 10,
            /// <summary>Security Successful - Index 12</summary>
            SECURITY_SUCCESSFUL = 12,
            /// <summary>Security Failed - Index 13</summary>
            SECURITY_FAILED = 13,
            /// <summary>Load Profile Reset - Index 14</summary>
            LOAD_PROFILE_RESET = 14,
            /// <summary>History Log Cleared - Index 16</summary>
            HIST_LOG_CLEARED = 16,
            /// <summary>History Pointers Updated - Index 17</summary>
            HIST_PTRS_UPDATED = 17,
            /// <summary>Event Log Cleared - Index 18</summary>
            EVENT_LOG_CLEARED = 18,
            /// <summary>Event Log Pointers Updated - Index 19</summary>
            EVENT_LOG_PTRS_UPDATED = 19,
            /// <summary>Demand Reset - Index 20</summary>
            DEMAND_RESET = 20,
            /// <summary>Self Read Occurred - Index 21</summary>
            SELF_READ_OCCURRED = 21,
            /// <summary>Input Channel Hi - Index 22</summary>
            INPUT_CHANNEL_HI = 22,
            /// <summary>Input Channel Low - Index 23</summary>
            INPUT_CHANNEL_LO = 23,
            /// <summary>TOU Season Changed - Index 24</summary>
            TOU_SEASON_CHANGED = 24,
            /// <summary>TOU Rate Change - Index 25</summary>
            RATE_CHANGE = 25,
            /// <summary>External Event - Index 26</summary>
            EXTERNAL_EVENT = 26,
            /// <summary>SiteScan for AMI; Custom Schedule Changed - V and I and C12.19 - Index 27</summary>
            SITESCAN_OR_CUSTOM_SCHED_ERROR = 27,
            /// <summary>Pending table Activation - Index 28</summary>
            PENDING_TABLE_ACTIVATION = 28,
            /// <summary>SiteScan for Non-AMI; Pending Table Clear for AMI - Index 29</summary>
            SITESCAN_OR_PENDING_TABLE_CLEAR = 29,
            /// <summary>VQ Log Pointers Updated - Index 30</summary>
            VQ_LOG_PTRS_UPDATED = 30,
            /// <summary>VQ Log Nearly Full - Index 31</summary>
            VQ_LOG_NEARLY_FULL = 31,
            /// <summary>Enter Test Mode - Index 32</summary>
            ENTER_TEST_MODE = 32,
            /// <summary>Exit Test Mode - Index 33</summary>
            EXIT_TEST_MODE = 33,
            /// <summary>ABC Phase Rotation Active - Index 34</summary>
            ABC_PH_ROTATION_ACTIVE = 34,
            /// <summary>CBA Phase Rotation Active - Index 35</summary>
            CBA_PH_ROTATION_ACTIVE = 35,
            /// <summary>Meter Reprogrammed - Index 36</summary>
            METER_REPROGRAMMED = 36,
            /// <summary>Illegal Configuration Error - Index 37</summary>
            ILLEGAL_CONFIG_ERROR = 37,
            /// <summary>CPC Communication Error - Index 38</summary>
            CPC_COMM_ERROR = 38,
            /// <summary>Reverse Rotation Restored - Index 39</summary>
            REVERSE_ROTATION_RESTORE = 39,
            /// <summary>VQ Log Cleared - Index 40</summary>
            VQ_LOG_CLEARED = 40,
            /// <summary>TOU Schedule Error - Index 41</summary>
            TOU_SCHEDULE_ERROR = 41,
            /// <summary>Mass Memory Error - Index 42</summary>
            MASS_MEMORY_ERROR = 42,
            /// <summary>Loss of Phase Restore - Index 43</summary>
            LOSS_OF_PHASE_RESTORE = 43,
            /// <summary>Low Battery - Index 44</summary>
            LOW_BATTERY = 44,
            /// <summary>Loss of Voltage phase A or Loss of Phase for OpenWay- Index 45</summary>
            LOSS_VOLTAGE_A_OR_LOSS_OF_PHASE = 45,
            /// <summary>Register Full Scale - Index 46</summary>
            REGISTER_FULL_SCALE = 46,
            /// <summary>Reverse Power Flow Restore - Index 47</summary>
            REVERSE_POWER_FLOW_RESTORE = 47,
            /// <summary>Reverse Power Flow - Index 48</summary>
            REVERSE_POWER_FLOW = 48,
            /// <summary>Site Scan Diagnostic 1 Active - Index 49</summary>
            SS_DIAG1_ACTIVE = 49,
            /// <summary>Site Scan Diagnostic 2 Active - Index 50</summary>
            SS_DIAG2_ACTIVE = 50,
            /// <summary>Site Scan Diagnostic 3 Active - Index 51</summary>
            SS_DIAG3_ACTIVE = 51,
            /// <summary>Site Scan Diagnostic 4 Active - Index 52</summary>
            SS_DIAG4_ACTIVE = 52,
            /// <summary>Site Scan Diagnostic 5 Active - Index 53</summary>
            SS_DIAG5_ACTIVE = 53,
            /// <summary>Site Scan Diagnostic 1 Inactive - Index 54</summary>
            SS_DIAG1_INACTIVE = 54,
            /// <summary>Site Scan Diagnostic 2 Inactive - Index 55</summary>
            SS_DIAG2_INACTIVE = 55,
            /// <summary>Site Scan Diagnostic 3 Inactive - Index 56</summary>
            SS_DIAG3_INACTIVE = 56,
            /// <summary>Site Scan Diagnostic 4 Inactive - Index 57</summary>
            SS_DIAG4_INACTIVE = 57,
            /// <summary>Site Scan Diagnostic 5 Inactive - Index 58</summary>
            SS_DIAG5_INACTIVE = 58,
            /// <summary>Site Scan Diagnostic 6 Active - Index 59</summary>
            SS_DIAG6_ACTIVE = 59,
            /// <summary>Site Scan Diagnostic 6 Inactive - Index 60</summary>
            SS_DIAG6_INACTIVE = 60,
            /// <summary>Self Read Cleared - Index 61</summary>
            SELF_READ_CLEARED = 61,
            /// <summary>Inversion Tamper - Index 62</summary>
            INVERSION_TAMPER = 62,
            /// <summary>Removal tamper - Index 63</summary>
            REMOVAL_TAMPER = 63,
            /// <summary>Register Firmware Download Failed - Index 66</summary>
            REG_DWLD_FAILED = 66,
            /// <summary>Register Firmware Download Succeeded - Index 67</summary>
            REG_DWLD_SUCCEEDED = 67,
            /// <summary>RFLAN Firmware Download Succeeded - Index 68</summary>
            RFLAN_DWLD_SUCCEEDED = 68,
            /// <summary>ZigBee Firmware Download Succeeded - Index 69</summary>
            ZIGBEE_DWLD_SUCCEEDED = 69,
            /// <summary>Display Firmware Download Succeeded - Index 72</summary>
            DISP_DWLD_SUCCEEDED = 72,
            /// <summary>Display Firmware Download Failed - Index 73</summary>
            DISP_DWLD_FAILED = 73,
            /// <summary>ZigBee Firmware Download Failed - Inxed 81</summary>
            ZIGBEE_DWLD_FAILED = 81,
            /// <summary>RFLAN Firmware Download Failed - Index 82</summary>
            RFLAN_DWLD_FAILED = 82,
            /// <summary>SiteScan Error Cleared - Index 84</summary>
            SITESCAN_ERROR_CLEAR = 84,
            /// <summary>Load Firmare - Index 85</summary>
            LOAD_FIRMWARE = 85,
            /// <summary>Reset Counter - Index 101</summary>
            RESET_COUNTERS = 101,
            /// <summary>Fatal Error - Index 121</summary>
            FATAL_ERROR = 121,
            /// <summary>Service Limiting Active Tier Changed - Index 126</summary>
            SERVICE_LIMITING_ACTIVE_TIER_CHANGED = 126,
            /// <summary>Table Written</summary>
            TABLE_WRITTEN = 130,
            /// <summary>Base Mode Error</summary>
            BASE_MODE_ERROR = 131,
            /// <summary>Pending Table Activated</summary>
            PENDING_RECONFIGURE = 134,
            /// <summary>Event Tamper Cleared - Index 141</summary>
            EVENT_TAMPER_CLEARED = 141,
            /// <summary>HAN/LAN Log Reset - Index 148</summary>
            LAN_HAN_LOG_RESET = 148,
            /// <summary>Display Firmware Download Initiated - Index 151</summary>
            DISP_DWLD_INITIATED = 151,
            /// <summary>Pending Table Activate Failed</summary>
            PENDING_TABLE_ACTIVATE_FAILED = 161,
            /// <summary>HAN Device Status Change</summary>
            HAN_DEVICE_STATUS_CHANGE = 162,
            /// <summary>HAN Load Control Event Sent</summary>
            HAN_LOAD_CONTROL_EVENT_SENT = 163,
            /// <summary>HAN Load Control Event Status</summary>
            HAN_LOAD_CONTROL_EVENT_STATUS = 164,
            /// <summary>HAN Load Control Event Opt Out</summary>
            HAN_LOAD_CONTROL_EVENT_OPT_OUT = 165,
            /// <summary>HAN Messaging Event</summary>
            HAN_MESSAGING_EVENT = 166,
            /// <summary>HAN Device Added or Removed</summary>
            HAN_DEVICE_ADDED_OR_REMOVED = 167,
            /// <summary>Display Firmware Download Initiation Failed - Index 168</summary>
            DISP_DWLD_INITIATION_FAILED = 168,
            /// <summary>Register Firmware Download Initiated - Index 169</summary>
            REG_DWLD_INITIATED = 169,
            /// <summary>RFLAN Firmware Download Initiated - Index 170</summary>
            RFLAN_DWLD_INITIATED = 170,
            /// <summary>ZigBee Firmware Download Initiated - Index 171</summary>
            ZIGBEE_DWLD_INITIATED = 171,
            /// <summary>Register Firmware Download Initiation Failed - Index 172</summary>
            REG_DWLD_INITIATION_FAILED = 172,
            /// <summary>RFLAN Firmware Download Initiation Failed - Index 173</summary>
            RFLAN_DWLD_INITIATION_FAILED = 173,
            /// <summary>ZigBee Firmware Download Initiation Failed - Index 174</summary>
            ZIGBEE_DWLD_INITIATION_FAILED = 174,
            /// <summary>Register Firmware Download Status - Index 175</summary>
            REG_FW_DWLD_STATUS = 175,
            /// <summary>RFLAN Firmawre Download Status - Index 176</summary>
            RFLAN_FW_DWLD_STATUS = 176,
            /// <summary>ZigBee Firmware Download Status - Index 177</summary>
            ZIGBEE_FW_DWLD_STATUS = 177,
            /// <summary>Register Firmware Download Alreaday Active - Index 178</summary>
            REG_DWLD_ALREADY_ACTIVE = 178,
            /// <summary>RFLAN Firmware Download Already Active - Index 179</summary>
            RFLAN_DWLD_ALREADY_ACTIVE = 179,
            /// <summary>Third Party HAN Device Firmware Download Status - Index 181</summary>
            THIRD_PARTY_HAN_FW_DWLD_STATUS = 181,
            /// <summary>Firmware Download Abort - Index 184</summary>
            FW_DWLD_ABORT = 184,
            /// <summary>Remote Connect Failed - Index 185</summary>
            REMOTE_CONNECT_FAILED = 185,
            /// <summary>Remote Disconnected Failed - Index 186</summary>
            REMOTE_DISCONNECT_FAILED = 186,
            /// <summary>Remote Disconnect Relay Activated - Index 187</summary>
            REMOTE_DISCONNECT_RELAY_ACTIVATED = 187,
            /// <summary>Remote Connect Relay Activated - Index 188</summary>
            REMOTE_CONNECT_RELAY_ACTIVATED = 188,
            /// <summary>Remote Connect Relay Initiated - Index 189/// </summary>
            REMOTE_CONNECT_RELAY_INITIATED = 189,
            /// <summary>Critical Peak Pricing - Index 192/// </summary>
            CRITICAL_PEAK_PRICING = 192,
            /// <summary>Billing Schedule Change - Index 213</summary>
            BILLING_SCHEDULE_CHANGE = 213,
            /// <summary>Network Hush Started</summary>
            NETWORK_HUSH_STARTED = 228,
            /// <summary>Load Voltage Preset - Index 230</summary>
            LOAD_VOLT_PRESENT = 230,
            /// <summary>Firmware Download Aborted - Index 231</summary>
            PENDING_TABLE_CLEAR_FAILED = 231,
            /// <summary>Pending Table Full - Index 232</summary>
            PENDING_TABLE_FULL = 232,
            /// <summary>Pending Table Swap - Index 233</summary>
            PENDING_TABLE_SWAP = 233,
            /// <summary>Event Scheduling Rejected - Index 234</summary>
            EVENT_SCHEDULING_REJECTED = 234,
            /// <summary>C12.22 Registration Attempt - Index 235</summary>
            C12_22_REGISTRATION_ATTEMPT = 235,
            /// <summary>C12.22 Registered - Index 236</summary>
            C12_22_REGISTERED = 236,
            /// <summary>C12.22 Deregistration Attempt - Index 237</summary>
            C12_22_DEREGISTRATION_ATTEMPT = 237,
            /// <summary>C12.22 Deregistered - Index 238</summary>
            C12_22_DEREGISTERED = 238,
            /// <summary>C12.22 RFLAN Cell ID Change - Index 239/// </summary>
            C12_22_RFLAN_CELL_ID_CHANGE = 239,
            /// <summary>Time Adjustment Failed</summary>
            TIME_ADJUSTMENT_FAILED = 240,
            /// <summary>Event Cache Overflow - Index 241</summary>
            EVENT_CACHE_OVERFLOW = 241,
            /// <summary>RMS Voltage Bleow Low Threshold</summary>
            RMS_VOLTAGE_BELOW_LOW_THRESHOLD = 246,
            /// <summary>Volt(RMS) Above Threshold</summary>
            VOLT_RMS_ABOVE_THRESHOLD = 247,
            /// <summary>Volt Hour Below Low Threshold</summary>
            VOLT_HOUR_BELOW_LOW_THRESHOLD = 248,
            /// <summary>VOlt Hour Above Threshold</summary>
            VOLT_HOUR_ABOVE_THRESHOLD = 249,
            /// <summary>Pending Table Error</summary>
            PENDING_TABLE_ERROR = 250,
            /// <summary>Security Event</summary>
            SECURITY_EVENT = 252,
            /// <summary>Key Rollover Pass</summary>
            KEY_ROLLOVER_PASS = 253,
            /// <summary>Sign Key Rpelace Processing Pass</summary>
            SIGN_KEY_REPLACE_PROCESSING_PASS = 254,
            /// <summary>Symmetric Key Replace Processing Pass</summary>
            SYMMETRIC_KEY_REPLACE_PROCESSING_PASS = 255,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM protocol object</param>
        /// <param name="Offset">The offset of this config block</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/31/06 AF  7.40.00 N/A    Created
        //
        public HistoryLogConfig(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, EVENT_CONFIG_SIZE)
        {
            m_lstEvents = new List<MFG2048EventItem>();
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
        }

        /// <summary>
        /// Constructor used for file based structures
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        public HistoryLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(2048, EVENT_CONFIG_SIZE)
        {
            m_lstEvents = new List<MFG2048EventItem>();
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS,
                                                               this.GetType().Assembly);
            m_Reader = EDLBinaryReader;
            m_TableState = TableState.Loaded;
            ParseData();
        }

        /// <summary>
        /// Reads the HistoryLogConfig component and populates its fields
        /// </summary>
        /// <returns>PSEMResponse</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/31/06 AF  7.40.00 N/A    Created
        //
        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "HistoryLogConfig.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the History Log configuration from the meter and stores
        /// it in a list of items each of which contains a description and a
        /// boolean telling whether or not it is enabled
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#     Description
        // -------- --- ------- ------     ---------------------------------------
        // 10/31/06 AF  7.40.00 N/A        Created
        // 11/03/06 AF  7.40.00            Corrected bug -- the list was not being cleared
        //                                 between accesses.
        // 07/12/16 MP  4.70.7  WR688986   Changed how event descriptions were accessed 
        public virtual List<MFG2048EventItem> HistoryLogEventList
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
                            "Error Reading History Log Config"));
                    }
                }

                //clear out the list
                m_lstEvents.Clear();

                //build the event list
                // Add Event 0 - 15 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_DOWN),       m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_DOWN));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.PRIMARY_POWER_UP),         m_usEvent0_15, (UInt16)(Event_0_15.PRIMARY_POWER_UP));
                AddEventItem(Enum.GetName(typeof(Event_0_15), Event_0_15.CLOCK_RESET),              m_usEvent0_15, (UInt16)(Event_0_15.CLOCK_RESET));

                // Add Event 16 - 31 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.HIST_LOG_CLEARED),       m_usEvent16_31, (UInt16)(Event_16_31.HIST_LOG_CLEARED));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.DEMAND_RESET),           m_usEvent16_31, (UInt16)(Event_16_31.DEMAND_RESET));
                AddEventItem(Enum.GetName(typeof(Event_16_31), Event_16_31.SELF_READ_OCCURRED),     m_usEvent16_31, (UInt16)(Event_16_31.SELF_READ_OCCURRED));

                // Add Event 32 - 47 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ENTER_TEST_MODE),        m_usEvent32_47, (UInt16)(Event_32_47.ENTER_TEST_MODE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.EXIT_TEST_MODE),         m_usEvent32_47, (UInt16)(Event_32_47.EXIT_TEST_MODE));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.METER_REPROGRAMMED),     m_usEvent32_47, (UInt16)(Event_32_47.METER_REPROGRAMMED));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.ILLEGAL_CONFIG_ERROR),   m_usEvent32_47, (UInt16)(Event_32_47.ILLEGAL_CONFIG_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.TOU_SCHEDULE_ERROR),     m_usEvent32_47, (UInt16)(Event_32_47.TOU_SCHEDULE_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.MASS_MEMORY_ERROR),      m_usEvent32_47, (UInt16)(Event_32_47.MASS_MEMORY_ERROR));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.LOW_BATTERY),            m_usEvent32_47, (UInt16)(Event_32_47.LOW_BATTERY));
                AddEventItem(Enum.GetName(typeof(Event_32_47), Event_32_47.REGISTER_FULL_SCALE),    m_usEvent32_47, (UInt16)(Event_32_47.REGISTER_FULL_SCALE));

                // Add Event 48 - 63 bitfield items
                AddEventItem(Enum.GetName(typeof(Event_48_63), Event_48_63.REVERSE_POWER_FLOW),     m_usEvent48_63, (UInt16)(Event_48_63.REVERSE_POWER_FLOW));


                return m_lstEvents;
            }
        }
      
        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds an event item to the list.
        /// </summary>
        /// <param name="strResourceString">The description of the event</param>
        /// <param name="usEventField">The raw data from the meter</param>
        /// <param name="usEventMask">The mask to apply to determine whether or not
        /// the event is enabled</param>
        protected void AddEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = GetEventItem(strResourceString, usEventField, usEventMask);
            m_lstEvents.Add(eventItem);
        }

        /// <summary>
        /// Gets the Event Item
        /// </summary>
        /// <param name="strResourceString"> The Description of the Event</param>
        /// <param name="usEventField">The raw data from the device</param>
        /// <param name="usEventMask">The mast to apply to determ if the event is enabled</param>
        /// <returns></returns>
        protected MFG2048EventItem GetEventItem(string strResourceString, UInt16 usEventField, UInt16 usEventMask)
        {
            MFG2048EventItem eventItem = new MFG2048EventItem();
            eventItem.Description = m_rmStrings.GetString(strResourceString);
            eventItem.Enabled = false;
            if (0 != (usEventField & usEventMask))
            {
                eventItem.Enabled = true;
            }

            return eventItem;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the data out of the PSEMBinaryRead and into the member variables
        /// </summary>
        private void ParseData()
        {
            m_usEvent0_15 = m_Reader.ReadUInt16();
            m_usEvent16_31 = m_Reader.ReadUInt16();
            m_usEvent32_47 = m_Reader.ReadUInt16();
            m_usEvent48_63 = m_Reader.ReadUInt16();
            m_usEvent64_79 = m_Reader.ReadUInt16();
            m_usEvent80_95 = m_Reader.ReadUInt16();
            m_usEvent96_111 = m_Reader.ReadUInt16();
            m_usEvent112_127 = m_Reader.ReadUInt16();
            m_usEvent128_143 = m_Reader.ReadUInt16();
            m_usEvent144_159 = m_Reader.ReadUInt16();
            m_usEvent160_175 = m_Reader.ReadUInt16();
            m_usEvent176_191 = m_Reader.ReadUInt16();
            m_usEvent192_207 = m_Reader.ReadUInt16();
            m_usEvent208_223 = m_Reader.ReadUInt16();
            m_usEvent224_239 = m_Reader.ReadUInt16();
            m_usEvent240_255 = m_Reader.ReadUInt16();
            m_bytLogType = m_Reader.ReadByte();
        }

        #endregion Private Methods

        #region Members

        /// <summary>Events 0-15</summary>
        protected UInt16 m_usEvent0_15;
        /// <summary>Events 16-31</summary>
        protected UInt16 m_usEvent16_31;
        /// <summary>Events 32-47</summary>
        protected UInt16 m_usEvent32_47;
        /// <summary>Events 48-63</summary>
        protected UInt16 m_usEvent48_63;
        /// <summary>Events 64-79</summary>
        protected UInt16 m_usEvent64_79;
        /// <summary>Events 80-95</summary>
        protected UInt16 m_usEvent80_95;
        /// <summary>Events 96-111</summary>
        protected UInt16 m_usEvent96_111;
        /// <summary>Events 112-127</summary>
        protected UInt16 m_usEvent112_127;
        /// <summary>Events 128-143</summary>
        protected UInt16 m_usEvent128_143;
        /// <summary>Events 144-159</summary>
        protected UInt16 m_usEvent144_159;
        /// <summary>Events 160-175</summary>
        protected UInt16 m_usEvent160_175;
        /// <summary>Events 176-191</summary>
        protected UInt16 m_usEvent176_191;
        /// <summary>Events 192-207</summary>
        protected UInt16 m_usEvent192_207;
        /// <summary>Events 208-223</summary>
        protected UInt16 m_usEvent208_223;
        /// <summary>Events 224- 239</summary>
        protected UInt16 m_usEvent224_239;
        /// <summary>Events 240-255</summary>
        protected UInt16 m_usEvent240_255;
        /// <summary>Events 256-271</summary>
        protected UInt16 m_usEvent256_271;
        /// <summary>Log Type</summary>
        protected byte m_bytLogType;
        /// <summary>The Event List</summary>
        protected List<MFG2048EventItem> m_lstEvents;
        /// <summary>The Resource Manager</summary>
        protected System.Resources.ResourceManager m_rmStrings;
        /// <summary>The Resource Project strings</summary>
        protected static readonly string RESOURCE_FILE_PROJECT_STRINGS =
                                            "Itron.Metering.Device.ANSIDevice.ANSIDeviceStrings";

        #endregion

    }

    /// <summary>
    /// The ModeControl class represents the Mode Control configuration for the device.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 08/20/07 RCG 8.10.21 N/A    Created

    internal class ModeControl : ANSISubTable
    {

        #region Constants

        private const ushort MODE_CONTROL_SIZE = 4;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device</param>
        /// <param name="Offset">The offset of the Mode Control table in 2048.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/20/07 RCG 8.10.21 N/A    Created

        public ModeControl(CPSEM psem, ushort Offset)
            : base(psem, 2048, Offset, MODE_CONTROL_SIZE)
        {
        }

        /// <summary>
        /// Reads the Mode Control table from the meter.
        /// </summary>
        /// <returns>PSEM response code for the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/20/07 RCG 8.10.21 N/A    Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ModeControl.Read");
            PSEMResponse Result = base.Read();

            if (PSEMResponse.Ok == Result)
            {
                m_DataStream.Position = 0;

                m_byModeTimeout = m_Reader.ReadByte();
                m_usDRLockoutTime = m_Reader.ReadUInt16();
                m_byDisableSwitches = m_Reader.ReadByte();
            }

            return Result;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the time in minutes it will take for the display to return to the Normals Display
        /// mode after being changed to Alt, Test, or Scroll Lock modes.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/20/07 RCG 8.10.21 N/A    Created

        public byte ModeTimeout
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    PSEMResponse Response = PSEMResponse.Ok;
                    // Read the subtable to get the Display Configuration
                    Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading Mode Control");
                    }
                }

                return m_byModeTimeout;
            }
        }

        /// <summary>
        /// Gets the time in minutes since a Demand Reset button push to ignore
        /// further Demand Reset button pushes.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/06/13 AF  2.80.26 TR7887 Created
        //
        public UInt16 DRLockoutTime
        {
            get
            {
                ReadUnloadedTable();

                return m_usDRLockoutTime;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// The timeout for the current display mode.
        /// </summary>
        protected byte m_byModeTimeout;

        /// <summary>
        /// The time in minutes since a Demand Reset button push to ignore
        /// further Demand Reset button pushes.
        /// </summary>
        protected ushort m_usDRLockoutTime;

        /// <summary>
        /// Whether buttons are enabled/disabled.
        /// </summary>
        protected byte m_byDisableSwitches;

        #endregion
    }

    /// <summary>
    /// Simple class to represent an MFG table 2048 event item as a description
    /// and a boolean, which tells whether or not the event is enabled in
    /// the meter.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 10/31/06 AF  7.40.00 N/A    Created
    //
    public class MFG2048EventItem : IEquatable<MFG2048EventItem>, IComparable<MFG2048EventItem>
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/31/06 AF  7.40.00 N/A    Created
        // 11/17/10 jrf 2.45.13        Added support for setting an ID for the event.
        //
        public MFG2048EventItem()
        {
            m_strDescription = "";
            m_blnEnabled = false;
            m_iID = 0;
        }
        
        /// <summary>
        /// Determines whether the two Events are equal
        /// </summary>
        /// <param name="other">The event to compare to</param>
        /// <returns>True if the evenst are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/08 RCG	1.50.14		   Created

        public bool Equals(MFG2048EventItem other)
        {
            return Description.Equals(other.Description);
        }

        /// <summary>
        /// Determines whether the values are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if they are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/08 RCG	1.50.14		   Created

        public override bool Equals(object obj)
        {
            bool bEquals = false;
            MFG2048EventItem Other = obj as MFG2048EventItem;

            if (Other != null)
            {
                bEquals = Equals(Other);
            }

            return bEquals;
        }

        /// <summary>
        /// Gets the hash code for the event.
        /// </summary>
        /// <returns>The hash code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/08 RCG	1.50.14		   Created

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }

        /// <summary>
        /// Compares the two events.
        /// </summary>
        /// <param name="other">The event to compare to.</param>
        /// <returns>0 if the values are equal. A negative number if less than or a positve number if greater than.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/01/08 RCG	1.50.14		   Created

        public int CompareTo(MFG2048EventItem other)
        {
            return String.Compare(Description, other.Description, StringComparison.CurrentCulture);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event's description
        /// </summary>
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the event has been enabled
        /// in the meter
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_blnEnabled;
            }
            set
            {
                m_blnEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the event's ID code.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/10 jrf 2.45.13		   Created
        public int ID
        {
            get
            {
                return m_iID;
            }
            set
            {
                m_iID = value;
            }
        }

        #endregion

        #region Members

        private string m_strDescription;
        private bool m_blnEnabled;
        private int m_iID;

        #endregion
    }

    /// <summary>
    /// The CommModuleLogConfig class
    /// </summary>
    public class CommModuleLogConfig : HistoryLogConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor for Comm Module Log Config class
        /// </summary>
        /// <param name="psem"></param>
        /// <param name="Offset"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 05/16/13 MSC 2.80.30 TQ7586 Created
        //
        public CommModuleLogConfig(CPSEM psem, ushort Offset)
            : base(psem, Offset)
        {
            m_lstCommModuleEvents = null;
        }

        /// <summary>
        /// Constructor used to get Comm Module Event Data from the EDL file
        /// </summary>
        /// <param name="EDLBinaryReader"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        // 05/16/13 MSC 2.80.30 TQ7586 Created
        //
        public CommModuleLogConfig(PSEMBinaryReader EDLBinaryReader)
            : base(EDLBinaryReader)
        {
            m_lstCommModuleEvents = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of Comm Module Events
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 05/16/13 MSC 2.80.30 TQ7586 Created
        //
        public virtual List<MFG2048EventItem> CommModuleEventList
        {
            get
            {
                return m_lstCommModuleEvents;
            }
        }

        #endregion

        #region Protected Members

        /// <summary>The Comm Module Event List</summary>
        protected List<MFG2048EventItem> m_lstCommModuleEvents;

        #endregion
    }
}



