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
//                              Copyright © 2006-2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The Meter Class ratings
    /// </summary>
    public enum MeterClass : byte
    {
        /// <summary>Class 2</summary>
        [EnumDescription("Class 2")]
        CLASS_2 = 0,
        /// <summary>Class 10</summary>
        [EnumDescription("Class 10")]
        CLASS_10 = 1,
        /// <summary>Class 20</summary>
        [EnumDescription("Class 20")]
        CLASS_20 = 2,
        /// <summary>Class 100</summary>
        [EnumDescription("Class 100")]
        CLASS_100 = 3,
        /// <summary>Class 150</summary>
        [EnumDescription("Class 150")]
        CLASS_150 = 4,
        /// <summary>Class 200</summary>
        [EnumDescription("Class 200")]
        CLASS_200 = 5,
        /// <summary>Class 320</summary>
        [EnumDescription("Class 320")]
        CLASS_320 = 6,
        /// <summary>Class 480</summary>
        [EnumDescription("Class 480")]
        CLASS_480 = 7,
    }

    /// <summary>
    /// The External MCP types.
    /// </summary>
    public enum MCPType : byte
    {
        /// <summary>
        /// Spansion full size memory chip. 8M Flash and 4M RAM.
        /// </summary>
        [EnumDescription("Spansion")]
        Full = 0,
        /// <summary>
        /// ESMT half size memory chip. 4M Flash and 2M RAM.
        /// </summary>
        [EnumDescription("ESMT")]
        Half = 1,
    }

	/// <summary>
	/// The CMeterKeyTable class handles the reading of MFG Table 2054. The reading of 
	/// this table in the meter will be implicit.  (read-only)
	/// </summary>
	// Revision History	
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ -------------------------------------------
	// 05/31/06 mrj 7.30.00 N/A    Created
    // 03/15/13 PGH 2.80.08 327121 Changed class from internal to public
	//
	public class CMeterKeyTable : AnsiTable
    {
        #region Constants
        private const ushort MISC_KEY_OFFSET = 28;
        #endregion

        #region public methods

        /// <summary>
		/// Constructor, initializes the table
		/// </summary>
		/// <example>
		/// <code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// CPSEM PSEM = new CPSEM(comm);
		/// PSEM.Logon("username");
		/// CMeterKeyTable MeterKeyTable = new CMeterKeyTable( PSEM ); 
		/// </code>
		/// </example> 
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 07/13/05 mrj 7.13.00 N/A    Created
		//
		public CMeterKeyTable( CPSEM psem )
			: base( psem, 2054, METERKEY_TABLE_LENGTH_2054 )
		{
			m_MeterKeyVersion = 0;
			m_MeterKeyRevision = 0;
			m_MeterFlavor = 0;
			m_TimeofLastMeterKey  = 0;
			m_EnergyKey1 = 0;
			m_DemandKey = 0;
			m_TOUKey = 0;
			m_LoadProfileKey = 0;
			m_PowerQualityKey = 0;
			m_MiscKey = 0;
			m_IOKey = 0;
			m_OptionBoardKey = 0;
			m_InstantaneousKey = 0;
			m_SelfReadKey = 0;
			m_CalendarKey = 0;
			m_EnergyKey2 = 0;			
		}

		/// <summary>
		/// Reads table 2054 (MeterKey) out of the meter.
		/// </summary>
		/// <returns>A PSEMResponse encapsulating the layer 7 response to the 
		/// layer 7 request. (PSEM errors)</returns>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/30/06 mrj 7.30.00 N/A    Created
		//
		public override PSEMResponse Read()
		{
			m_Logger.WriteLine( Logger.LoggingLevel.Detailed, "2054.Read" );


			PSEMResponse Result = base.Read();
			
			if( PSEMResponse.Ok == Result )
			{
				m_DataStream.Position = 0;

				//Populate the member variables that represent the table
				m_MeterKeyVersion = m_Reader.ReadByte();
				m_MeterKeyRevision = m_Reader.ReadByte();
				m_MeterFlavor = m_Reader.ReadUInt16();
				m_TimeofLastMeterKey  = m_Reader.ReadUInt32();
				m_EnergyKey1 = m_Reader.ReadUInt32();
				m_DemandKey = m_Reader.ReadUInt32();
				m_TOUKey = m_Reader.ReadUInt32();
				m_LoadProfileKey = m_Reader.ReadUInt32();
				m_PowerQualityKey = m_Reader.ReadUInt32();
				m_MiscKey = m_Reader.ReadUInt32();
				m_IOKey = m_Reader.ReadUInt32();
				m_OptionBoardKey = m_Reader.ReadUInt32();
				m_InstantaneousKey = m_Reader.ReadUInt32();
				m_SelfReadKey = m_Reader.ReadUInt32();
				m_CalendarKey = m_Reader.ReadUInt32();
				m_EnergyKey2 = m_Reader.ReadUInt32();
			}
            

			return Result;			
		}

		/// <summary>
		/// Writes the contents to the MeterKey table logger for debugging
		/// </summary>
		/// <exception>
		/// None.  This debugging method catches its exceptions
		/// </exception>
		/// 
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/09/06 mcm 7.30.00 N/A	Created
		// 
		public override void Dump()
		{
			Logger		m_Logger = Logger.TheInstance;
			DateTime	LastMK = new DateTime(2000,1,1);
			try
			{
				if( TableState.Unloaded == m_TableState )
				{
					//Read MeterKey Table
					Read();				
				}

				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"Dump of MeterKey Table" );
				
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"m_MeterKeyVersion = " + m_MeterKeyVersion );
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"m_MeterKeyRevision = " + m_MeterKeyRevision );
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"m_MeterFlavor = " + m_MeterFlavor );

				LastMK = LastMK.AddSeconds(m_TimeofLastMeterKey);
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"m_TimeofLastMeterKey = " + LastMK );

				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_EnergyKey1 = 0x" + m_EnergyKey1.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_DemandKey = 0x" + m_DemandKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_TOUKey = 0x" + m_TOUKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_LoadProfileKey = 0x" + m_LoadProfileKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_PowerQualityKey = 0x" + m_PowerQualityKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_MiscKey = 0x" + m_MiscKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_IOKey = 0x" + m_IOKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_OptionBoardKey = 0x" + m_OptionBoardKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_InstantaneousKey = 0x" + m_InstantaneousKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_SelfReadKey = 0x" + m_SelfReadKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_CalendarKey = 0x" + m_CalendarKey.ToString("X8", CultureInfo.InvariantCulture));
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol,
                    "m_EnergyKey2 = 0x" + m_EnergyKey2.ToString("X8", CultureInfo.InvariantCulture));

				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
					"End Dump of MeterKey Table " );
			}
			catch( Exception e )
			{
				try
				{
					m_Logger.WriteException( this, e );
				}
				catch
				{
					// No exceptions thrown from this debugging method
				}
			}		
		}

		#endregion

		#region properties

		/// <summary>
		/// Returns whether or not the multiple custom schedule MeterKey bit
		/// is set.
		/// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/31/06 mrj 7.30.00 N/A    Created
		//
		public bool MultipleCustomScheduleEnabled
		{
			get
			{
                PSEMResponse Result = PSEMResponse.Ok;
				bool bEnabled = false;

				if( TableState.Unloaded == m_TableState )
				{
					//Read MeterKey Table
                    Result = Read();
					if( PSEMResponse.Ok != Result )
					{
						throw( new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
					}
				}

				if( MCS_MASK == (m_MiscKey & MCS_MASK) )
				{
					//The MCS bit is set
					bEnabled = true;
				}

				return bEnabled;
			}
		}

		/// <summary>
		/// Returns the value for the number of supported rates.
		/// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/22/06 mcm 7.30.00 N/A    Created
		//
		public uint TOURatesSupported
		{
			get
			{
                PSEMResponse Result = PSEMResponse.Ok;
				if( TableState.Unloaded == m_TableState )
				{
					//Read MeterKey Table
                    Result = Read();
					if( PSEMResponse.Ok != Result )
					{
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
					}	
				}

				return m_TOUKey & TOU_MASK;			
			}
		}

		/// <summary>
		/// Returns whether or not the power quality MeterKey bit is set.
		/// </summary> 
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public bool PQSupported
		{
			get
			{
				bool bEnabled = false;
				PSEMResponse Result = PSEMResponse.Ok;

				if (TableState.Unloaded == m_TableState)
				{
					//Read MeterKey Table
					Result = Read();
					if (PSEMResponse.Ok != Result)
					{
						throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
							"Error accessing MeterKey data"));
					}
				}

				if (PQ_MASK == (m_PowerQualityKey & PQ_MASK))
				{					
					bEnabled = true;
				}

				return bEnabled;
			}
		}

        /// <summary>
        /// Returns whether or not the MISC - Disconnect Avaiable bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/06/07 KRC 8.10.12		Created
        //  
        public bool DisconnectAvailable
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (DISCONNECT_MASK == (m_MiscKey & DISCONNECT_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the MISC - Pole Top Cell Relay bit is set
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  07/31/09 AF  2.20.19 138931 Created to be able to detect Pole Top Cell
        //
        public bool PoleTopCellRelaySupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (PTCR_MASK == (m_MiscKey & PTCR_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the IO MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/22/09 jrf 2.20.02 n/a	Created
        //  
        public bool IOSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (IO_MASK == (m_IOKey & IO_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }
               
        /// <summary>
        /// Returns the type of multi-chip package used.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/18/14 jrf 4.00.56 534458	Created
        //  
        public MCPType MCPTypeUsed
        {
            get
            {
                MCPType MCPUsed = MCPType.Full;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (EXTERNAL_MCP_TYPE_MASK == (m_IOKey & EXTERNAL_MCP_TYPE_MASK))
                {
                    MCPUsed = MCPType.Half;
                }

                return MCPUsed;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh rec quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool WhRecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (WH_REC_MASK == (m_EnergyKey1 & WH_REC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q1 quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhQ1Supported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_Q1_MASK == (m_EnergyKey1 & VARH_Q1_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q2 quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhQ2Supported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_Q2_MASK == (m_EnergyKey1 & VARH_Q2_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q3 quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhQ3Supported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_Q3_MASK == (m_EnergyKey1 & VARH_Q3_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh Q4 quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhQ4Supported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_Q4_MASK == (m_EnergyKey1 & VARH_Q4_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net del quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhNetDelSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_NET_DEL_MASK == (m_EnergyKey1 & VARH_NET_DEL_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net rec quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VarhNetRecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_NET_REC_MASK == (m_EnergyKey1 & VARH_NET_REC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh del arithmetic quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VAhDelArithSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VAH_DEL_ARITH_MASK == (m_EnergyKey1 & VAH_DEL_ARITH_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh rec arithmetic quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VAhRecArithSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VAH_REC_ARITH_MASK == (m_EnergyKey1 & VAH_REC_ARITH_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh del vectorial quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.02 n/a	Created
        //  
        public bool VAhDelVecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VAH_DEL_VEC_MASK == (m_EnergyKey1 & VAH_DEL_VEC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh rec vectorial quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VAhRecVecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VAH_REC_VEC_MASK == (m_EnergyKey1 & VAH_REC_VEC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the VAh lag quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VAhLagSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VAH_LAG_MASK == (m_EnergyKey1 & VAH_LAG_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Qh del quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool QhDelSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (QH_DEL_MASK == (m_EnergyKey1 & QH_DEL_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase A quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VhASupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VH_A_MASK == (m_EnergyKey1 & VH_A_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase B quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VhBSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VH_B_MASK == (m_EnergyKey1 & VH_B_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase C quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VhCSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VH_C_MASK == (m_EnergyKey1 & VH_C_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Vh Phase Average quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool VhPhaseAverageSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VH_AVG_MASK == (m_EnergyKey1 & VH_AVG_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase A quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool AhPhaseASupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (AH_A_MASK == (m_EnergyKey1 & AH_A_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase B quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool AhPhaseBSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (AH_B_MASK == (m_EnergyKey1 & AH_B_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Phase C quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool AhPhaseCSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (AH_C_MASK == (m_EnergyKey1 & AH_C_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Ah Neutral quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool AhNeutralSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (AH_NEUT_MASK == (m_EnergyKey1 & AH_NEUT_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the V Squared h Aggregate quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool V2hAggregateSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (V2H_AGG_MASK == (m_EnergyKey1 & V2H_AGG_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the I Squared h Aggregate quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/11/09 jrf 2.21.01 n/a	Created
        //  
        public bool I2hAggregateSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (I2H_AGG_MASK == (m_EnergyKey1 & I2H_AGG_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh del quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool VarhDelSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_DEL_MASK == (m_EnergyKey1 & VARH_DEL_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh rec quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool VarhRecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_REC_MASK == (m_EnergyKey1 & VARH_REC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh net quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool WhNetSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (WH_NET_MASK == (m_EnergyKey2 & WH_NET_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Varh net quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool VarhNetSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (VARH_NET_MASK == (m_EnergyKey2 & VARH_NET_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Qh rec quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool QhRecSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (QH_REC_MASK == (m_EnergyKey2 & QH_REC_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Wh unidirectional quantity MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool WhUnidirectionalSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (WH_UNI_MASK == (m_EnergyKey2 & WH_UNI_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns what the maximum number of peaks MeterKey bits are set to.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public UInt16 MaxPeaks
        {
            get
            {
                UInt16 uiMaxPeaks = 0;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                uiMaxPeaks = (UInt16)(m_DemandKey & MAX_PEAKS_MASK);

                return uiMaxPeaks;
            }
        }

        /// <summary>
        /// Returns whether or not the Coincident Demand MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool CoincidentDemandSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (COINCIDENT_DEM_MASK == (m_DemandKey & COINCIDENT_DEM_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Thermal Demand MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool ThermalDemandSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (THERMAL_MASK == (m_DemandKey & THERMAL_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Demand Thresholds Allowed MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool DemandThresholdsAllowed
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (THRESHOLDS_MASK == (m_DemandKey & THRESHOLDS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns what the maximum number of scheduled demand resets MeterKey bits are set to.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public UInt16 MaxDemandResets
        {
            get
            {
                UInt16 uiMaxDemandResets = 0;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                uiMaxDemandResets = (UInt16)((m_DemandKey & NUM_DEM_RESETS_MASK) >> 6);

                return uiMaxDemandResets;
            }
        }

        /// <summary>
        /// Returns whether or not the Scheduled Demand Resets Allowed MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.02 n/a	Created
        //  
        public bool ScheduledResetsAllowed
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (ALLOW_SCHED_DEM_RESETS_MASK == (m_DemandKey & ALLOW_SCHED_DEM_RESETS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns what the maximum load profile  MeterKey bits are set to.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public UInt16 MaximumLPMemorySize
        {
            get
            {
                UInt16 uiMaxLPSize = 0;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                uiMaxLPSize = (UInt16)(m_LoadProfileKey & MAX_LP_MEM_MASK);

                return uiMaxLPSize;
            }
        }

        /// <summary>
        /// Returns whether or not the Request Enhanced Blurts MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool EnhancedBlurtsSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (ENHANCED_BLURTS_MASK == (m_MiscKey & ENHANCED_BLURTS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Request Advanced Blurts MeterKey bit is set.
        /// Note that this meter key is used for the ITRD device class and indicates that
        /// varh in returned instead of VAh
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/21/11 mah         n/a	Created
        //  
        public bool AdvancedBlurtsSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (ADVANCED_BLURTS_MASK == (m_MiscKey & ADVANCED_BLURTS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Enable 9S in 6S Service MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool Enable9SIn6SServiceSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (SERVICE_9S_IN_6S_MASK == (m_MiscKey & SERVICE_9S_IN_6S_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Use SR1.0 Device Class MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool UseSR1DeviceClass
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (USE_SR1_DEVICE_CLASS_MASK == (m_MiscKey & USE_SR1_DEVICE_CLASS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the ZigBee Debug MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool ZigBeeDebug
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (ZIGBEE_DEBUG_MASK == (m_MiscKey & ZIGBEE_DEBUG_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Transparent Device MeterKey bits are set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/14/09 jrf 2.21.01 n/a	Created
        //  
        public bool TransparentDevice
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (TRANSPARENT_DEVICE_MASK == (m_MiscKey & TRANSPARENT_DEVICE_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Disable Core Dump MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool DisableCoreDump
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (DISABLE_CORE_DUMP_MASK == (m_MiscKey & DISABLE_CORE_DUMP_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Inject Events MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/26/11 MMD            	Created
        //  
        public bool InjectEventsEnabled
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (INJECT_EVENTS_MASK == (m_MiscKey & INJECT_EVENTS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        
        }

        /// <summary>
        /// Returns whether or not the Inject FW Download Events MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/24/11 MMD            	Created
        //  
        public bool InjectFWDownloadEventsEnabled
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (INJECT_FW_DOWNLOAD_EVENTS_MASK == (m_MiscKey & INJECT_FW_DOWNLOAD_EVENTS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }

        }

        /// <summary>
        /// Gets whether or not the MeterKey bit to disable the HAN Event Injection Limit is set
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/01/12 RCG 2.53.46 185543 Created
        
        public bool HANEventInjectionLimitDisabled
        {
            get
            {
                bool Disabled = false;

                ReadUnloadedTable();

                Disabled = (m_MiscKey & DISABLE_HAN_EVENT_LIMIT_MASK) == DISABLE_HAN_EVENT_LIMIT_MASK;

                return Disabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Disable Core Dump on Total Stack Use Limit MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool DisableCoreDumpOnTotalStackUseLimit
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (DISABLE_CORE_DUMP_ON_TOTAL_STACK_USE_LIMIT_MASK == 
                    (m_MiscKey & DISABLE_CORE_DUMP_ON_TOTAL_STACK_USE_LIMIT_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Advanced Poly MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool AdvancedPolySupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (ADVANCED_POLY_MASK == (m_MiscKey & ADVANCED_POLY_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Option Board Config MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool OptionBoardConfigSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (OPT_BOARD_MASK == (m_OptionBoardKey & OPT_BOARD_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the ANSI C12.21 Modem MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool ModemAllowed
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (MODEM_MASK == (m_OptionBoardKey & MODEM_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the CPC Instantaneous Vars MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool CPCVarsSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (CPC_VARS_MASK == (m_InstantaneousKey & CPC_VARS_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the CPC Instantaneous VA MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool CPCVASupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (CPC_VA_MASK == (m_InstantaneousKey & CPC_VA_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns whether or not the Self Read Config Allowed MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool SRConfigSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (SR_MASK == (m_SelfReadKey & SR_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns what the number of additional self read buffers MeterKey bits are set to.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public UInt16 NumberAdditionalSRs
        {
            get
            {
                UInt16 uiNumAdditionalSRs = 0;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                uiNumAdditionalSRs = (UInt16)((m_SelfReadKey & ADDED_SR_MASK) >> 1);

                return uiNumAdditionalSRs;
            }
        }

        /// <summary>
        /// Returns whether or not the Calendar Config Allowed MeterKey bit is set.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/12/09 jrf 2.21.01 n/a	Created
        //  
        public bool CalendarConfigSupported
        {
            get
            {
                bool bEnabled = false;
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (CALENDAR_MASK == (m_CalendarKey & CALENDAR_MASK))
                {
                    bEnabled = true;
                }

                return bEnabled;
            }
        }

        /// <summary>
        /// Returns a string representing the quantity configuration based on the meter key settings.
        /// </summary> 
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/13/09 jrf 2.21.00 n/a	Created
        //  08/14/09 jrf 2.21.01 n/a    Adding Transparent Device.
        //  08/17/09 jrf 2.21.01 n/a    Correcting energy masks comparisons.
        //  
        public string QuantityConfiguration
        {
            get
            {
                string strDeviceType = "Unrecognized";
                PSEMResponse Result = PSEMResponse.Ok;

                if (TableState.Unloaded == m_TableState)
                {
                    //Read MeterKey Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error accessing MeterKey data"));
                    }
                }

                if (POLE_TOP_ENERGY_1_MASK == m_EnergyKey1
                    && POLE_TOP_ENERGY_2_MASK == m_EnergyKey2)
                {
                    if (true == TransparentDevice)
                    {
                        strDeviceType = "Transparent Device";
                    }
                    else if (true == PoleTopCellRelaySupported)
                    {
                        strDeviceType = "Pole Top Cell Relay";
                    }
                }
                else
                {
                    if (true == AdvancedPolySupported && ADV_POLY_ENERGY_1_MASK == m_EnergyKey1
                    && ADV_POLY_ENERGY_2_MASK == m_EnergyKey2)
                    {
                        strDeviceType = "Advanced Polyphase";
                    }
                    else if (BASIC_POLY_ENERGY_1_MASK == m_EnergyKey1
                    && BASIC_POLY_ENERGY_2_MASK == m_EnergyKey2)
                    {
                        strDeviceType = "Basic Polyphase";
                    }
                    else if (LEVEL_1_ENERGY_1_MASK == m_EnergyKey1
                    && LEVEL_1_ENERGY_2_MASK == m_EnergyKey2)
                    {
                        strDeviceType = "Level 1";
                    }
                }

                return strDeviceType;
            }
        }

        /// <summary>
        /// Gets the Energy 1 MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint EnergyKey1
        {
            get
            {
                ReadUnloadedTable();

                return m_EnergyKey1;
            }
        }

        /// <summary>
        /// Gets the Energy 2 MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint EnergyKey2
        {
            get
            {
                ReadUnloadedTable();

                return m_EnergyKey2;
            }
        }

        /// <summary>
        /// Gets the Demand MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint DemandKey
        {
            get
            {
                ReadUnloadedTable();

                return m_DemandKey;
            }
        }

        /// <summary>
        /// Gets the TOU MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint TOUKey
        {
            get
            {
                ReadUnloadedTable();

                return m_TOUKey;
            }
        }

        /// <summary>
        /// Gets the Load Profile MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint LoadProfileKey
        {
            get
            {
                ReadUnloadedTable();

                return m_LoadProfileKey;
            }
        }

        /// <summary>
        /// Gets the Power Quality MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint PowerQualityKey
        {
            get
            {
                ReadUnloadedTable();

                return m_PowerQualityKey;
            }
        }

        /// <summary>
        /// Gets the Misc MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint MiscKey
        {
            get
            {
                ReadUnloadedTable();

                return m_MiscKey;
            }
        }

        /// <summary>
        /// Gets the I/O MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint IOKey
        {
            get
            {
                ReadUnloadedTable();

                return m_IOKey;
            }
        }

        /// <summary>
        /// Gets the Option Board MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint OptionBoardKey
        {
            get
            {
                ReadUnloadedTable();

                return m_OptionBoardKey;
            }
        }

        /// <summary>
        /// Gets the Instantaneous MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint InstantaneousKey
        {
            get
            {
                ReadUnloadedTable();

                return m_InstantaneousKey;
            }
        }

        /// <summary>
        /// Gets the Self Read MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created

        public uint SelfReadKey
        {
            get
            {
                ReadUnloadedTable();

                return m_SelfReadKey;
            }
        }

        /// <summary>
        /// Gets the Calendar MeterKey
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 11/16/11 RCG 2.53.07 N/A    Created
        
        public uint CalendarKey
        {
            get
            {
                ReadUnloadedTable();

                return m_CalendarKey;
            }
        }

		#endregion			

		#region public definitions

		#endregion public definitions

		#region private definitions

		//MeterKey masks
		private const int MCS_MASK	 = 0x00000001;
		private const int TOU_MASK	 = 0x00000007;
		private const int PQ_MASK = 0x00000001;
        private const int DISCONNECT_MASK = 0x00000004;
        private const int IO_MASK = 0x00000001;
        private const int PTCR_MASK = 0x00000008;
        private const int INJECT_EVENTS_MASK = 0x04000000;
        private const int DISABLE_HAN_EVENT_LIMIT_MASK = 0x10000000;
        private const int INJECT_FW_DOWNLOAD_EVENTS_MASK = 0x40000000;

        private const int WH_REC_MASK = 0x00000001;
        private const int VARH_Q1_MASK = 0x00000002;
        private const int VARH_Q2_MASK = 0x00000004;
        private const int VARH_Q3_MASK = 0x00000008;
        private const int VARH_Q4_MASK = 0x00000010;
        private const int VARH_NET_DEL_MASK = 0x00000020;
        private const int VARH_NET_REC_MASK = 0x00000040;
        private const int VAH_DEL_ARITH_MASK = 0x00000080;
        private const int VAH_REC_ARITH_MASK = 0x00000100;
        private const int VAH_DEL_VEC_MASK = 0x00000200;
        private const int VAH_REC_VEC_MASK = 0x00000400;
        private const int VAH_LAG_MASK = 0x00000800;
        private const int QH_DEL_MASK = 0x00001000;
        private const int VH_A_MASK = 0x00002000;
        private const int VH_B_MASK = 0x00004000;
        private const int VH_C_MASK = 0x00008000;
        private const int VH_AVG_MASK = 0x00010000;
        private const int AH_A_MASK = 0x00020000;
        private const int AH_B_MASK = 0x00040000;
        private const int AH_C_MASK = 0x00080000;
        private const int AH_NEUT_MASK = 0x00100000;
        private const int V2H_AGG_MASK = 0x00200000;
        private const int I2H_AGG_MASK = 0x00400000;
        private const int VARH_DEL_MASK = 0x40000000;
        private const Int64 VARH_REC_MASK = 0x80000000;

        private const int WH_NET_MASK = 0x00000004;
        private const int VARH_NET_MASK = 0x00000008;
        private const int QH_REC_MASK = 0x00000010;
        private const int WH_UNI_MASK = 0x00000020;

        private const int MAX_PEAKS_MASK = 0x00000007;
        private const int COINCIDENT_DEM_MASK = 0x00000008;
        private const int THERMAL_MASK = 0x00000010;
        private const int THRESHOLDS_MASK = 0x00000020;
        private const int NUM_DEM_RESETS_MASK = 0x00007FC0;
        private const int ALLOW_SCHED_DEM_RESETS_MASK = 0x00008000;

        private const int NUM_RATES_ALLOWED_MASK = 0x00000007;

        private const int MAX_LP_MEM_MASK = 0x000000FF;

        private const int ENHANCED_BLURTS_MASK = 0x00000001;
        private const int SERVICE_9S_IN_6S_MASK = 0x00000002;
        private const int USE_SR1_DEVICE_CLASS_MASK = 0x00000010;
        private const int ZIGBEE_DEBUG_MASK = 0x00000020;
        private const int TRANSPARENT_DEVICE_MASK = 0x00000088;
        private const int DISABLE_CORE_DUMP_MASK = 0x00020000;
        private const int DISABLE_CORE_DUMP_ON_TOTAL_STACK_USE_LIMIT_MASK = 0x00040000;
        private const int ADVANCED_POLY_MASK    = 0x00080000;
        private const int ADVANCED_BLURTS_MASK  = 0x02000000;

        private const int OPT_BOARD_MASK = 0x00000001;
        private const int MODEM_MASK = 0x00000002;

        private const int CPC_VARS_MASK = 0x00000001;
        private const int CPC_VA_MASK = 0x00000002;

        private const int SR_MASK = 0x00000001;
        private const int ADDED_SR_MASK = 0x0000001E;

        private const int CALENDAR_MASK = 0x00000001;

        private const int LEVEL_1_ENERGY_1_MASK = 0x00002181;
        private const int LEVEL_1_ENERGY_2_MASK = 0x00000024;
        private const int POLE_TOP_ENERGY_1_MASK = 0x00000181;
        private const int POLE_TOP_ENERGY_2_MASK = 0x00000024;
        private const int BASIC_POLY_ENERGY_1_MASK = 0x00000FF3;
        private const int BASIC_POLY_ENERGY_2_MASK = 0x00000024;
        private const Int64 ADV_POLY_ENERGY_1_MASK = 0xC0000FF3;
        private const int ADV_POLY_ENERGY_2_MASK = 0x0000002C;

        private const int EXTERNAL_MCP_TYPE_MASK = 0x00800000;

        private const int METERKEY_TABLE_LENGTH_2054 = 56;

		#endregion

		#region variable declarations
		
		//The table's member variables which represent the table 		
		private byte	m_MeterKeyVersion;
		private byte	m_MeterKeyRevision;
		private ushort	m_MeterFlavor;
		private uint	m_TimeofLastMeterKey ;
		private uint	m_EnergyKey1;
		private uint	m_DemandKey;
		private uint	m_TOUKey;
		private uint	m_LoadProfileKey;
		private uint	m_PowerQualityKey;
		private uint	m_MiscKey;
		private uint	m_IOKey;
		private uint	m_OptionBoardKey;
		private uint	m_InstantaneousKey;
		private uint	m_SelfReadKey;
		private uint	m_CalendarKey;
		private uint	m_EnergyKey2;

		#endregion
	}

	/// <summary>
	/// The CTable2084 class handles the reading of MFG Table 2084 (MCS). The 
	/// reading of this table in the meter will be implicit.  (read-only)
	/// </summary>
	/// <remarks>
	/// This table is only supported by meters which have the MCS MeterKey bit
	/// set.
	/// 
	/// This table currently only supports a partial read of this table.  It reads
	/// the status field only.
	/// </remarks>
	// Revision History	
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ -------------------------------------------
	// 05/31/06 mrj 7.30.00 N/A    Created
	//
	public class CTable2084 : AnsiTable
    {
        #region constants

        private const int DATA_FLASH_WRITE_COUNT_OFFSET = 316;
        private const int DATA_FLASH_WRITE_COUNT_BYTES = 4;

        #endregion

        #region public methods

        /// <summary>
		/// Constructor, initializes the table
		/// </summary>
		/// <example>
		/// <code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// CPSEM PSEM = new CPSEM(comm);
		/// PSEM.Logon("username");
		/// CTable2084 Table2084 = new CTable2084( PSEM ); 
		/// </code>
		/// </example> 
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 07/13/05 mrj 7.13.00 N/A    Created
		//
		public CTable2084( CPSEM psem)
			: base( psem, 2084, MCS_TABLE_LENGTH_2084 )
		{
			m_usStatus = 0;
           
		}

		/// <summary>
		/// Partial read of table 2084 (Multiple Custom Schedules) out of the
		/// meter.  It only reads the status bytes.
		/// </summary>
		/// <returns>A PSEMResponse encapsulating the layer 7 response to the 
		/// layer 7 request. (PSEM errors)</returns>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/30/06 mrj 7.30.00 N/A    Created
		//
		public override PSEMResponse Read()
		{
            PSEMResponse Result = PSEMResponse.Err;
			m_Logger.WriteLine( Logger.LoggingLevel.Detailed, "CTable2084.Read" );

            
                // This is new code added for the Lithium Hot Fix 3 check of the 
                //  total number of Data Flash Items.  
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2084.OffsetRead");

                Result = base.Read(DATA_FLASH_WRITE_COUNT_OFFSET, DATA_FLASH_WRITE_COUNT_BYTES);

                if (Result == PSEMResponse.Ok)
                {
                    m_byTotalDataFlashWriteCount = m_Reader.ReadUInt32();
                }
            						
			
			return Result;
		}

		#endregion

		#region properties

        /// <summary>
        /// Returns the Total number of Data Flash Writes
        /// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/24/13 DG           N/A    Created
        //
        public uint TotalDataFlashWriteCount
        {
            get
            {
                ReadUnloadedTable();                

                return m_byTotalDataFlashWriteCount;
            }
        }


		/// <summary>
		/// Returns whether or not the multiple custom schedule are configured
		/// in this table.
		/// </summary>
        /// <exception>
        /// Throws: TimeoutException for Timeouts
        /// 		PSEMException for other communication errors.
        /// </exception>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/31/06 mrj 7.30.00 N/A    Created
		//
		public bool MultipleCustomScheduleConfigured
		{
			get
			{
                PSEMResponse Result = PSEMResponse.Ok;
				bool bConfigured = false;

				if( TableState.Unloaded == m_TableState )
				{
					//Read the Multiple Custom Schedules (2084) table
                    Result = Read();
					if( PSEMResponse.Ok != Result )
					{
						throw( new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error Reading Multiple Custom Schedule data"));
					}
				}

				//Only bit 0 is used in this status field
				if( 1 == (m_usStatus & 0x0001) )
				{
					bConfigured = true;
				}
				

				return bConfigured;			
			}
		}

        /// <summary>
        /// Returns the number of Seal/Unseal entries remaining in the FWDL event log
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/16 PGH 4.50.234 REQ574469 Created
        //
        public byte NumberOfSealUnsealEntriesRemainingForPolyPhaseMeter
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;

                PSEMResult = m_PSEM.OffsetRead(2084, 402, 1, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Number of Seal/Unseal Log Entries Remaining"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_byNumSealUnsealEntriesRemaining = Reader.ReadByte();
                }

                return m_byNumSealUnsealEntriesRemaining;
            }
        }

        /// <summary>
        /// Returns the number of Seal/Unseal entries remaining in the FWDL event log
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/03/16 PGH 4.50.234 REQ574469 Created
        //
        public byte NumberOfSealUnsealEntriesRemainingForSinglePhaseMeter
        {
            get
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;
                byte[] byaData;

                PSEMResult = m_PSEM.OffsetRead(2084, 386, 1, out byaData);

                if (PSEMResponse.Ok != PSEMResult)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, PSEMResult,
                        "Error Reading Number of Seal/Unseal Log Entries Remaining"));
                }
                else
                {
                    MemoryStream DataStream = new MemoryStream(byaData);
                    PSEMBinaryReader Reader = new PSEMBinaryReader(DataStream);
                    m_byNumSealUnsealEntriesRemaining = Reader.ReadByte();
                }

                return m_byNumSealUnsealEntriesRemaining;
            }
        }
		
		#endregion

		#region public definitions

		#endregion public definitions

		#region private definitions

		private const int MCS_TABLE_LENGTH_2084 = 15252;

		#endregion

        #region private methods


        #endregion
        #region variable declarations

        //The table's member variables which represent the table 		
        private ushort m_usStatus;
        private uint m_byTotalDataFlashWriteCount = 0;
        private byte m_byNumSealUnsealEntriesRemaining = 0;

		#endregion
	}

	/// <summary>
	/// The Table2088 class handles the reading of MFG Table 2088, SiteScan
	/// Snapshot Status.
	/// </summary>
	/// <remarks>
	/// This table is only supported by Image Poly meters and Sentinel Saturn
	/// meters.
	/// </remarks>	
	public class Table2088 : AnsiTable
	{
		#region Constants

		private const int MCS_TABLE_LENGTH_2088 = 14;

		#endregion Constants

		#region Public Methods

		/// <summary>
		/// Constructor, initializes the table
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public Table2088(CPSEM psem)
			: base(psem, 2088, MCS_TABLE_LENGTH_2088)
		{
			m_NumberSnapshots = 0;
			m_SnapshotSize = 0;
			m_ProgramID = 0;
			m_MeterClass = 0;
			m_MeterForm = 0;
			m_MeterBase = 0;
			m_FirmwareVersion = 0;
			m_FirmwareRevision = 0;
			m_CalculationMethod = 0;
			m_PeakDemandCurrent = 0.0f;
		}

		/// <summary>
		/// Reads out table 2088 out of the meter.
		/// </summary>
		/// <returns>
		/// A PSEMResponse encapsulating the layer 7 response to the 
		/// layer 7 request. (PSEM errors)
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public override PSEMResponse Read()
		{
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "CTable2088.Read");

			PSEMResponse Result = base.Read();

			if (PSEMResponse.Ok == Result)
			{
				m_DataStream.Position = 0;

				//Populate the member variables that represent the table
				m_NumberSnapshots = m_Reader.ReadByte();
				m_SnapshotSize = m_Reader.ReadByte();
				m_ProgramID = m_Reader.ReadUInt16();
				m_MeterClass = m_Reader.ReadByte();
				m_MeterForm = m_Reader.ReadByte();
				m_MeterBase = m_Reader.ReadByte();
				m_FirmwareVersion = m_Reader.ReadByte();
				m_FirmwareRevision = m_Reader.ReadByte();
				m_CalculationMethod = m_Reader.ReadByte();
				m_PeakDemandCurrent = m_Reader.ReadSingle();
			}

			return Result;
		}

		#endregion Public Methods

		#region Public Properties

		/// <summary>
		/// Property to get the number of snapshots in 2089.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public byte NumberSnapshots
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_NumberSnapshots;
			}
		}

		/// <summary>
		/// Property to get the size of each snapshot in 2089.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public byte SnapshotSize
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_SnapshotSize;
			}
		}

		/// <summary>
		/// Property that will return the size of table 2089 based off the values in
		/// table 2088.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public uint SizeOfTable2089
		{
			get
			{
				return (uint)(SnapshotSize * NumberSnapshots);
			}
		}

		/// <summary>
		/// Property to get the program ID in the snapshot
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public ushort SnapshotProgramID
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_ProgramID;
			}
		}

		/// <summary>
		/// Property to get the byte representing the meter class in the snapshot.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public MeterClass SnapshotMeterClass
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return (MeterClass)m_MeterClass;
			}
		}

		/// <summary>
		/// Property to get the byte representing the meter form in the snapshot.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public byte SnapshotMeterForm
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_MeterForm;
			}
		}

		/// <summary>
		/// Property to get the byte representing the meter base in the snapshot.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public byte SnapshotMeterBase
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_MeterBase;
			}
		}

		/// <summary>
		/// Property to get the meter firmware from the snapshot.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public float SnapshotFirmwareVer
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return (m_FirmwareVersion + m_FirmwareRevision / 1000.0F); ;
			}
		}

		/// <summary>
		/// Property to get the peak demand current in the snapshot.  The Image
		/// Poly firmware always returns 0.0 for this field.
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public float SnapshotPeakDemandCurrent
		{
			get
			{
				//Read the table if it is unloaded
				ReadTable();

				return m_PeakDemandCurrent;
			}
		}

		#endregion Public Properties

		#region Private Methods

		/// <summary>
		/// This method reads the table only if it is unloaded
		/// </summary>
		/// <exception>
		/// Throws: TimeoutException for Timeouts
		/// 		PSEMException for other communication errors.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		private void ReadTable()
		{
			if (TableState.Unloaded == m_TableState)
			{
				PSEMResponse Response = Read();

				if (Response != PSEMResponse.Ok)
				{
					throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, "Error Reading SiteScan Snapshots");
				}
			}
		}

		#endregion Private Methods
		
		#region Members

		//The table's member variables which represent the table		
		private byte m_NumberSnapshots;
		private byte m_SnapshotSize;
		private ushort m_ProgramID;
		private byte m_MeterClass;
		private byte m_MeterForm;
		private byte m_MeterBase;
		private byte m_FirmwareVersion;
		private byte m_FirmwareRevision;
		private byte m_CalculationMethod;
		private float m_PeakDemandCurrent;

		#endregion Members
	}

	/// <summary>
	/// The Table2089 class handles the reading of MFG Table 2089, SiteScan
	/// Snapshot Data.
	/// </summary>
	/// <remarks>
	/// This table is only supported by Image Poly meters and Sentinel Saturn
	/// meters.
	/// </remarks>	
	internal class Table2089 : AnsiTable
	{
		#region Public Methods

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="psem">PSEM object for this current session</param>
		/// <param name="table2088">Table 2088 object</param>			
		/// <param name="bPQEnabled">Boolean for power quality supported</param>
		/// <param name="bIRMSSupported">Boolean for I RMS supported</param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/19/07 mrj 8.00.12		Created
		//  
		public Table2089(CPSEM psem, Table2088 table2088, bool bPQEnabled, bool bIRMSSupported)
			: base(psem, 2089, table2088.SizeOfTable2089)
		{
			m_table2088 = table2088;

			m_Table2089SnapshotList = new List<Table2089Snapshot>();
			
			m_bPQEnabled = bPQEnabled;
			m_bIRMSSupported = bIRMSSupported;			
		}

		/// <summary>
		/// Reads table 74 out of the meter.
		/// </summary>
		/// <returns>A PSEMResponse encapsulating the layer 7 response to the 
		/// layer 7 request. (PSEM errors)</returns>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/18/06 KRC 7.35.00 N/A    Created
		///
		public override PSEMResponse Read()
		{
			PSEMResponse Result = PSEMResponse.Ok;
			
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Table2089.Read");

			//Populate the snapshot data
			m_DataStream.Position = 0;
			
			for (int iIndex = 0; iIndex < m_table2088.NumberSnapshots && PSEMResponse.Ok == Result; iIndex++)
			{
				Table2089Snapshot Snapshot = new Table2089Snapshot();

				//Read a snapshot out
				Result = base.Read((ushort)(iIndex * m_table2088.SnapshotSize), (ushort)m_table2088.SnapshotSize);

				if (PSEMResponse.Ok == Result)
				{
					Snapshot.SnapshotTimeSeconds = m_Reader.ReadUInt32();
					Snapshot.SnapshotTrigger = m_Reader.ReadByte();
					Snapshot.MeterService = m_Reader.ReadByte();
					Snapshot.PhaseRotation = m_Reader.ReadByte();
					Snapshot.InsWatt = m_Reader.ReadInt32();
					Snapshot.InsVA = m_Reader.ReadInt32();
					Snapshot.InsVar = m_Reader.ReadInt32();
					Snapshot.InsPF = m_Reader.ReadInt16();
					if (m_bIRMSSupported)
					{
						Snapshot.IRMS = m_Reader.ReadUInt16();
					}
					Snapshot.LineFrequency = m_Reader.ReadUInt16();
					Snapshot.VoltagePhaseA = m_Reader.ReadUInt16();
					Snapshot.VoltageAnglePhaseA = m_Reader.ReadUInt16();
					Snapshot.CurrentPhaseA = m_Reader.ReadUInt16();
					Snapshot.CurrentAnglePhaseA = m_Reader.ReadUInt16();
					Snapshot.VoltagePhaseB = m_Reader.ReadUInt16();
					Snapshot.VoltageAnglePhaseB = m_Reader.ReadUInt16();
					Snapshot.CurrentPhaseB = m_Reader.ReadUInt16();
					Snapshot.CurrentAnglePhaseB = m_Reader.ReadUInt16();
					Snapshot.VoltagePhaseC = m_Reader.ReadUInt16();
					Snapshot.VoltageAnglePhaseC = m_Reader.ReadUInt16();
					Snapshot.CurrentPhaseC = m_Reader.ReadUInt16();
					Snapshot.CurrentAnglePhaseC = m_Reader.ReadUInt16();
					Snapshot.Diag1Counter = m_Reader.ReadByte();
					Snapshot.Diag2Counter = m_Reader.ReadByte();
					Snapshot.Diag3Counter = m_Reader.ReadByte();
					Snapshot.Diag4Counter = m_Reader.ReadByte();
					Snapshot.Diag5ACounter = m_Reader.ReadByte();
					Snapshot.Diag5BCounter = m_Reader.ReadByte();
					Snapshot.Diag5CCounter = m_Reader.ReadByte();
					Snapshot.Diag5TCounter = m_Reader.ReadByte();
					Snapshot.Diag6Counter = m_Reader.ReadByte();
					Snapshot.PowerOutageCount = m_Reader.ReadByte();

					if (m_bPQEnabled)
					{
						Snapshot.VQSagCounter = m_Reader.ReadByte();
						Snapshot.VQSwellCounter = m_Reader.ReadByte();
						Snapshot.VQInterruptionCounter = m_Reader.ReadByte();
						Snapshot.VQImbalanceVCounter = m_Reader.ReadByte();
						Snapshot.VQImbalanceICounter = m_Reader.ReadByte();
						Snapshot.THDPhaseA = m_Reader.ReadSingle();
						Snapshot.THDPhaseB = m_Reader.ReadSingle();
						Snapshot.THDPhaseC = m_Reader.ReadSingle();
						Snapshot.TDDPhaseA = m_Reader.ReadSingle();
						Snapshot.TDDPhaseB = m_Reader.ReadSingle();
						Snapshot.TDDPhaseC = m_Reader.ReadSingle();
					}

					m_Table2089SnapshotList.Add(Snapshot);
				}
			}		

			return Result;
		}

		#endregion Public Methods

		#region Public Properties
				
		/// <summary>
		/// Access to the list of snapshots
		/// </summary>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/18/06 KRC 7.35.00 N/A    Created
		///
		public List<Table2089Snapshot> Table2089SnapshotData
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
								"Error reading SiteScan Snapshots"));
					}
				}
				return m_Table2089SnapshotList;
			}
		}
		
		#endregion Public Properties
		
		#region Members

		Table2088 m_table2088;

		List<Table2089Snapshot> m_Table2089SnapshotList;
		
		private bool m_bPQEnabled;
		private bool m_bIRMSSupported;
			
		#endregion Members
	}
	
	/// <summary>
	/// This class represents the individual snapshot entries as represented in
	/// table 2089.
	/// </summary>	
	internal class Table2089Snapshot
	{	
		#region Public Methods
		
		/// <summary>
		/// Constructor
		/// </summary>		
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/20/07 mrj 8.00.13		Created
		//  
		public Table2089Snapshot()
		{			
		}

		#endregion Public Methods

		#region Public Properties
		
		/// <summary>
		/// Snapshot time in seconds since 1/1/2000
		/// </summary>		
		public uint SnapshotTimeSeconds
		{
			get {return m_SnapshotTimeSeconds;}
			set {m_SnapshotTimeSeconds = value;}
		}
		/// <summary>
		/// The dianostic number that triggered the snapshot.
		/// </summary>		
		public byte SnapshotTrigger
		{
			get {return m_SnapshotTrigger;}
			set {m_SnapshotTrigger = value;}
		}
		/// <summary>
		/// The number representing the meter service.
		/// </summary>		
		public byte MeterService
		{
			get {return m_MeterService;}
			set {m_MeterService = value;}
		}
		/// <summary>
		/// The number representing the meter phase rotation.
		/// </summary>		
		public byte PhaseRotation
		{
			get	{return m_PhaseRotation;}
			set	{m_PhaseRotation = value;}
		}
		/// <summary>
		/// The instantaneous watts, in milli-watt units.
		/// </summary>		
		public int InsWatt
		{
			get {return m_InsWatt;}
			set {m_InsWatt = value;}
		}
		/// <summary>
		/// The instantaneous VA, in milli-volt/amp units.
		/// </summary>	
		public int InsVA
		{
			get	{return m_InsVA;}
			set {m_InsVA = value;}
		}
		/// <summary>
		/// The instantaneous var, in milli-var units.
		/// </summary>		
		public int InsVar
		{
			get {return m_InsVar;}
			set {m_InsVar = value;}
		}
		/// <summary>
		/// The instantaneous PF, in units of 0.0001.
		/// </summary>		
		public short InsPF
		{
			get {return m_InsPF;}
			set {m_InsPF = value;}
		}
		/// <summary>
		/// The instantaneous PF, in units of 0.0001.
		/// </summary>		
		public ushort IRMS
		{
			get {return m_IRMS;}
			set {m_IRMS = value;}
		}
		/// <summary>
		/// The line frequency  in units of 0.01 Hz.
		/// </summary>
		public ushort LineFrequency
		{
			get { return m_LineFrequency; }
			set { m_LineFrequency = value; }
		}
		/// <summary>
		/// Voltage in units of 0.025 V.
		/// </summary>
		public ushort VoltagePhaseA
		{
			get { return m_VoltagePhaseA; }
			set { m_VoltagePhaseA = value; }
		}
		/// <summary>
		/// Voltage in units of 0.025 V.
		/// </summary>
		public ushort VoltagePhaseB
		{
			get { return m_VoltagePhaseB; }
			set { m_VoltagePhaseB = value; }
		}
		/// <summary>
		/// Voltage in units of 0.025 V.
		/// </summary>
		public ushort VoltagePhaseC
		{
			get { return m_VoltagePhaseC; }
			set { m_VoltagePhaseC = value; }
		}
		/// <summary>
		/// Voltage angle in units of 0.1 degree
		/// </summary>
		public ushort VoltageAnglePhaseA
		{
			get { return m_VoltageAnglePhaseA; }
			set { m_VoltageAnglePhaseA = value; }
		}
		/// <summary>
		/// Voltage angle in units of 0.1 degree
		/// </summary>
		public ushort VoltageAnglePhaseB
		{
			get { return m_VoltageAnglePhaseB; }
			set { m_VoltageAnglePhaseB = value; }
		}
		/// <summary>
		/// Voltage angle in units of 0.1 degree
		/// </summary>
		public ushort VoltageAnglePhaseC
		{
			get { return m_VoltageAnglePhaseC; }
			set { m_VoltageAnglePhaseC = value; }
		}
		/// <summary>
		/// Current in units based on meter class
		/// </summary>
		public ushort CurrentPhaseA
		{
			get { return m_CurrentPhaseA; }
			set { m_CurrentPhaseA = value; }
		}
		/// <summary>
		/// Current in units based on meter class
		/// </summary>
		public ushort CurrentPhaseB
		{
			get { return m_CurrentPhaseB; }
			set { m_CurrentPhaseB = value; }
		}
		/// <summary>
		/// Current in units based on meter class
		/// </summary>
		public ushort CurrentPhaseC
		{
			get { return m_CurrentPhaseC; }
			set { m_CurrentPhaseC = value; }
		}
		/// <summary>
		/// Current angle in units of 0.1 degree
		/// </summary>
		public ushort CurrentAnglePhaseA
		{
			get { return m_CurrentAnglePhaseA; }
			set { m_CurrentAnglePhaseA = value; }
		}
		/// <summary>
		/// Current angle in units of 0.1 degree
		/// </summary>
		public ushort CurrentAnglePhaseB
		{
			get { return m_CurrentAnglePhaseB; }
			set { m_CurrentAnglePhaseB = value; }
		}
		/// <summary>
		/// Current angle in units of 0.1 degree
		/// </summary>
		public ushort CurrentAnglePhaseC
		{
			get { return m_CurrentAnglePhaseC; }
			set { m_CurrentAnglePhaseC = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag1Counter
		{
			get { return m_Diag1Counter; }
			set { m_Diag1Counter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag2Counter
		{
			get { return m_Diag2Counter; }
			set { m_Diag2Counter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag3Counter
		{
			get { return m_Diag3Counter; }
			set { m_Diag3Counter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag4Counter
		{
			get { return m_Diag4Counter; }
			set { m_Diag4Counter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag5ACounter
		{
			get { return m_Diag5ACounter; }
			set { m_Diag5ACounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag5BCounter
		{
			get { return m_Diag5BCounter; }
			set { m_Diag5BCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag5CCounter
		{
			get { return m_Diag5CCounter; }
			set { m_Diag5CCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag5TCounter
		{
			get { return m_Diag5TCounter; }
			set { m_Diag5TCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte Diag6Counter
		{
			get { return m_Diag6Counter; }
			set { m_Diag6Counter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte PowerOutageCount
		{
			get { return m_PowerOutageCount; }
			set { m_PowerOutageCount = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte VQSagCounter
		{
			get { return m_VQSagCounter; }
			set { m_VQSagCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte VQSwellCounter
		{
			get { return m_VQSwellCounter; }
			set { m_VQSwellCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte VQInterruptionCounter
		{
			get { return m_VQInterruptionCounter; }
			set { m_VQInterruptionCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte VQImbalanceVCounter
		{
			get { return m_VQImbalanceVCounter; }
			set { m_VQImbalanceVCounter = value; }
		}
		/// <summary>
		/// Counter
		/// </summary>
		public byte VQImbalanceICounter
		{
			get { return m_VQImbalanceICounter; }
			set { m_VQImbalanceICounter = value; }
		}		
		/// <summary>
		/// %THD
		/// </summary>
		public float THDPhaseA
		{
			get { return m_THDPhaseA; }
			set { m_THDPhaseA = value; }
		}
		/// <summary>
		/// %THD
		/// </summary>
		public float THDPhaseB
		{
			get { return m_THDPhaseB; }
			set { m_THDPhaseB = value; }
		}
		/// <summary>
		/// %THD
		/// </summary>
		public float THDPhaseC
		{
			get { return m_THDPhaseC; }
			set { m_THDPhaseC = value; }
		}
		/// <summary>
		/// %TDD
		/// </summary>
		public float TDDPhaseA
		{
			get { return m_TDDPhaseA; }
			set { m_TDDPhaseA = value; }
		}
		/// <summary>
		/// %TDD
		/// </summary>
		public float TDDPhaseB
		{
			get { return m_TDDPhaseB; }
			set { m_TDDPhaseB = value; }
		}
		/// <summary>
		/// %TDD
		/// </summary>
		public float TDDPhaseC
		{
			get { return m_TDDPhaseC; }
			set { m_TDDPhaseC = value; }
		}

		#endregion Public Properties
		
		#region Members

		private uint m_SnapshotTimeSeconds;
		private byte m_SnapshotTrigger;
		private byte m_MeterService;
		private byte m_PhaseRotation;
		private int m_InsWatt;
		private int m_InsVA;
		private int m_InsVar;
		private short m_InsPF;	
		private ushort m_IRMS;		
		private ushort m_LineFrequency;
		private ushort m_VoltagePhaseA;
		private ushort m_VoltageAnglePhaseA;
		private ushort m_CurrentPhaseA;
		private ushort m_CurrentAnglePhaseA;
		private ushort m_VoltagePhaseB;
		private ushort m_VoltageAnglePhaseB;
		private ushort m_CurrentPhaseB;
		private ushort m_CurrentAnglePhaseB;
		private ushort m_VoltagePhaseC;
		private ushort m_VoltageAnglePhaseC;
		private ushort m_CurrentPhaseC;
		private ushort m_CurrentAnglePhaseC;
		private byte m_Diag1Counter;
		private byte m_Diag2Counter;
		private byte m_Diag3Counter;
		private byte m_Diag4Counter;
		private byte m_Diag5ACounter;
		private byte m_Diag5BCounter;
		private byte m_Diag5CCounter;
		private byte m_Diag5TCounter;
		private byte m_Diag6Counter;
		private byte m_PowerOutageCount;

		//Only support if PQ is enabled
		private byte m_VQSagCounter;
		private byte m_VQSwellCounter;
		private byte m_VQInterruptionCounter;
		private byte m_VQImbalanceVCounter;
		private byte m_VQImbalanceICounter;
		private float m_THDPhaseA;
		private float m_THDPhaseB;
		private float m_THDPhaseC;
		private float m_TDDPhaseA;
		private float m_TDDPhaseB;
		private float m_TDDPhaseC;		

		#endregion Members
	}	
}