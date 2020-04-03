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
//                          Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// MFG Table 2191 (Itron 143) class
    /// </summary>
    public class OpenWayMFGTable2191 : AnsiTable
    {
        #region Constants

        private const uint TABLE_SIZE = 21;

        #endregion

        #region Definitions

        /// <summary>
        /// Contains the mask for the C12.22 Options 0 bitfield
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        private enum C1222Options0Mask : byte
        {
            Ok2ResetRFLANOnEPFExit = 0x01,
            ForceTimeSyncToCommModule = 0x02,
            ForceGenericCommModule = 0x04,
            DelayInitialTimeSync30Min = 0x08,
            Force9600BaudOnly = 0x10,
            DisableTimeSyncOutsideMaxDLT = 0x20,
            LogCMAccessInLANLog = 0x40,
            TimePollFailUseHours = 0x80,
        }

        /// <summary>
        /// Enumeration that identifies configuration fields. Enumeration values
        /// are set to config field offsets.
        /// </summary>
        public enum ConfigFields : ushort
        {
            /// <summary>
            /// C12.22 Options Bitfield 0
            /// </summary>
            [EnumDescription("C12.22 Options Bitfield 0")]
            BitmappedOptions0 = 0x00,
            
            //Skip a few...
            
            /// <summary>
            /// Poll Period Minutes
            /// </summary>
            [EnumDescription("Poll Period Minutes")]
            PollPeriodMinutes = 0x12,
            
            //Skip Max Poll Round Trip Secs

            /// <summary>
            /// Poll Fail Threshold Mins/Hours
            /// </summary>
            [EnumDescription("Poll Fail Threshold Mins/Hours")]
            PollFailThresholdMinsHours = 0x14,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current device.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public OpenWayMFGTable2191(CPSEM psem)
            : base(psem, 2191, TABLE_SIZE)
        {
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public OpenWayMFGTable2191(PSEMBinaryReader reader)
            : base(2191, TABLE_SIZE)
        {
            m_Reader = reader;
            ParseData();
            m_TableState = TableState.Loaded;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The result of the read request.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public override PSEMResponse Read()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2191.Read");

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                ParseData();
            }

            return Result;
        }

        

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets minimum difference that register time will be adjusted to comm module time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public byte MinDeltaSeconds
        {
            get
            {
                ReadUnloadedTable();

                return m_byMinDeltaSecs;
            }
        }

        /// <summary>
        /// Gets maximum difference that register time will be adjusted to comm module time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        public byte MaxDeltaSeconds
        {
            get
            {
                ReadUnloadedTable();

                return m_byMaxDeltaSecs;
            }
        }

        /// <summary>
        /// Gets/Sets whether or not the time poll fail threshold value is 
        /// stored as hours (True) or minutes (False).
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/06/13 jrf 2.85.12 TC 12653  Created.
        //
        public bool TimePollFailThresholdUseHours
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                byte byValue;

                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Time Poll Fail Use Hours"));
                }

                // Mask off the bit we need                
                byValue = (byte)(m_byBitMappedOptions0 & (byte)C1222Options0Mask.TimePollFailUseHours);

                return (byValue == (byte)C1222Options0Mask.TimePollFailUseHours);
            }
            set
            {
                byte m_byValue = m_byBitMappedOptions0;

                if (value)
                {
                    m_byValue = (byte)((byte)m_byBitMappedOptions0 | (byte)C1222Options0Mask.TimePollFailUseHours);
                }
                else
                {
                    m_byValue = (byte)((byte)m_byBitMappedOptions0 & ~((byte)C1222Options0Mask.TimePollFailUseHours));
                }


                m_DataStream.Position = (ushort)ConfigFields.BitmappedOptions0;
                m_Writer.Write(m_byValue);

                PSEMResponse Result = base.Write((ushort)ConfigFields.BitmappedOptions0, 1);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the Time Poll Fail Use Hours"));
                }
                else
                {
                    m_byBitMappedOptions0 = m_byValue;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the frequency the register will attempt to read the time from the comm module.
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/06/13 jrf 2.85.12 TC 12653  Created.
        //
        public byte PollPeriodMinutes
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Poll Period Minutes"));
                }

                return m_byPollPeriodMins;
            }
            set
            {
                m_DataStream.Position = (ushort)ConfigFields.PollPeriodMinutes;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write((ushort)ConfigFields.PollPeriodMinutes, 1);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the Poll Period Minutes"));
                }
                else
                {
                    m_byPollPeriodMins = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the amount of time after which a time adjustment failed event will be recorded
        /// if the comm module time cannot be successfully aquired.
        /// </summary>
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/06/13 jrf 2.85.12 TC 12653  Created.
        //
        public byte PollFailThresholdMinsHours
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;

                Result = Read();
                if (PSEMResponse.Ok != Result)
                {
                    //We could not read the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error Reading the Poll Fail Threshold Minutes/Hours"));
                }

                return m_byPollFailThresholdMins;
            }
            set
            {
                m_DataStream.Position = (ushort)ConfigFields.PollFailThresholdMinsHours;
                m_Writer.Write(value);

                PSEMResponse Result = base.Write((ushort)ConfigFields.PollFailThresholdMinsHours, 1);

                if (PSEMResponse.Ok != Result)
                {
                    //We could not write the table so throw an exception
                    throw (new PSEMException(PSEMException.PSEMCommands.PSEM_WRITE, Result,
                        "Error Writing the Poll Fail Threshold Minutes/Hours"));
                }
                else
                {
                    m_byPollFailThresholdMins = value;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that was just read. 
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/13 jrf 2.80.06 TQ7634 Created.
        //
        private void ParseData()
        {
            //Bitmapped options
            m_byBitMappedOptions0 = m_Reader.ReadByte();
            m_byBitMappedOptions1 = m_Reader.ReadByte();
            m_byBitMappedOptions2 = m_Reader.ReadByte();
            m_byBitMappedOptions3 = m_Reader.ReadByte();
            m_byBitMappedOptions4 = m_Reader.ReadByte();
            m_byBitMappedOptions5 = m_Reader.ReadByte();
            m_byBitMappedOptions6 = m_Reader.ReadByte();
            m_byBitMappedOptions7 = m_Reader.ReadByte();
            
            //Misc Configuration
            m_byMaxAlarmRandPeriodMins = m_Reader.ReadByte();
            m_byLANSendToCMTimeoutSecs = m_Reader.ReadByte();
            m_byLANClientMSGPacingSecs = m_Reader.ReadByte();
            
            //Registration Configuration
            m_byMaxRetryFactor = m_Reader.ReadByte();
            m_byMinRetryInitialValue = m_Reader.ReadByte();
            m_byMaxRetryAddr = m_Reader.ReadByte();
            m_byRetryUnits = m_Reader.ReadByte();
            m_byCellSwitchDivisor = m_Reader.ReadByte();
            
            //TimeSynch Configuration
            m_byMinDeltaSecs = m_Reader.ReadByte();
            m_byMaxDeltaSecs = m_Reader.ReadByte();
            m_byPollPeriodMins = m_Reader.ReadByte();
            m_byMaxPollRoundTripSecs = m_Reader.ReadByte();
            m_byPollFailThresholdMins = m_Reader.ReadByte();
        }

        #endregion

        #region Member Variables

        //Bitmapped options
        private byte m_byBitMappedOptions0 = 0;
        private byte m_byBitMappedOptions1 = 0;
        private byte m_byBitMappedOptions2 = 0;
        private byte m_byBitMappedOptions3 = 0;
        private byte m_byBitMappedOptions4 = 0;
        private byte m_byBitMappedOptions5 = 0;
        private byte m_byBitMappedOptions6 = 0;
        private byte m_byBitMappedOptions7 = 0;
        //Misc Configuration
        private byte m_byMaxAlarmRandPeriodMins = 0;
        private byte m_byLANSendToCMTimeoutSecs = 0;
        private byte m_byLANClientMSGPacingSecs = 0;
        //Registration Configuration
        private byte m_byMaxRetryFactor = 0;
        private byte m_byMinRetryInitialValue = 0;
        private byte m_byMaxRetryAddr = 0;
        private byte m_byRetryUnits = 0;
        private byte m_byCellSwitchDivisor = 0;
        //TimeSynch Configuration
        private byte m_byMinDeltaSecs = 0;
        private byte m_byMaxDeltaSecs = 0;
        private byte m_byPollPeriodMins = 0;
        private byte m_byMaxPollRoundTripSecs = 0;
        private byte m_byPollFailThresholdMins = 0;

        #endregion
    }
}
