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
//                           Copyright © 2006 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Threading;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DST;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
	/// <summary>
	/// Class representing the FULCRUM meter.
	/// </summary>
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------
	//  04/27/06 mrj 7.30.00  N/A	Created
	//  05/26/06 jrf 7.30.00  N/A	Modified
    //  04/02/07 AF  8.00.23  2814  Corrected the capitalization of the meter name
    //
    public partial class FULCRUM : SCSDevice
    {
        #region Constants
        private const int FULC_SERIAL_NUMBER_LENGTH = 9;
        private const int FULC_FW_VERSION_LENGTH = 6;
        private const int FULC_SW_VERSION_LENGTH = 6;
        private const int FULC_OPERATING_SETUP_LENGTH = 1;
        private const int FULC_PROGRAM_ID_LENGTH = 2;
        private const int FULC_MINUTES_ON_BATTERY_LENGTH = 4;
        private const int FULCRUM_DEMANDCONFIGURATIONBLOCK_LENGTH = 4;
        private const byte FULC_DST_MASK = 0x04;
        private const byte FULC_TOU_MASK = 0x10;
        private const int FULC_SYSTEM_ERROR_LENGTH = 2;
        private const int FULC_MAX_WRITE_SIZE = 128;
        private const int FULC_DST_RECORD_LENGTH = 7;
        private const int FULC_CALENDAR_END = 0xFF;
        private const int SIZE_OF_TOU_HEADER = 20;
        private const ushort TOU_HEADER_ADDRESS = 0x2AEC;
        private const int SIZE_OF_TOU_ID_AND_EXPIRATION_DATE = 6;

        // Address of the Real Time Clock - 1
        private const int LAST_AVAIL_TOU_ADDRESS = 0x32BB;

        private const string FULCRUM_TYPE = "FULCRUM";
        private const string FULCRUM_NAME = "FULCRUM";
        #endregion

        #region Definitions

        /// <summary>
		/// FULCAddresses enumeration encapsulates the FULCRUM basepage addresses.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00 N/A	Created
        /// 08/15/06 mah 7.35.00 N/A    Added demand reset and clear billing flags
		/// 
		private enum FULCAddresses : int
		{
			
			SYSTEM_ERROR				= 0x2102,
			COMMUNICATIONS_TIMEOUT		= 0x210E,
            DEMAND_RESET_FLAG           = 0x2110,
			HANGUP_FLAG					= 0x2111,
			STOP_METER_FLAG				= 0x2113,
            CLEAR_BILLING_FLAG          = 0x2114,
            COPY_DATA_FLAG              = 0x2115,
			CLOCK_RECONFIGURE_FLAG		= 0x2116,
			METER_RECONFIGURE_FLAG		= 0x2117,
			CLOCK_RUN_FLAG				= 0x2118,
			MM_RUN_FLAG					= 0x2119,
			TOU_RUN_FLAG				= 0x211A,
			MODEL_TYPE					= 0x211B,
            PROGRAM_COUNT               = 0x211C,
            RESET_COUNT                 = 0x211E,
            OUTAGE_COUNT                = 0x2124,
            TEST_MODE_FLAG                  = 0x2130,
			TOU_RECONFIGURE_FLAG		= 0x2137,
			SERIAL_NUMBER				= 0x213C,
			SOFTWARE_REVISION			= 0x2145,
			FIRMWARE_REVISION			= 0x214B,
			PROGRAM_ID					= 0x2151,
			UNIT_ID						= 0x2156,
            USERDEFINED_FIELD1 = 0x215E,
            PRIMARY_PASSWORD            = 0x2179,
            SECONDARY_PASSWORD          = 0x2181,
            TERTIARY_PASSWORD           = 0x2189,
			TRANSFORMER_RATIO			= 0x21B7,
            DISPLAY_TABLE               = 0x21CB,
			OPERATING_SETUP				= 0x27E5,
			FIRMWARE_OPTIONS			= 0x27E7,
            COLD_LOAD_PICKUP_TIME = 0x27F9,
            DEMAND_CONFIGURATION = 0x27FD,
			TOU_BASE					= 0x2AEC,
			YEARLY_SCHEDULE				= 0x2B00,
			REAL_TIME					= 0x32BC,	
		    NUMBER_OF_CHANNELS         = 0x335D,
			MM_INTERVAL_LENGTH			= 0x335F,
			TOU_SCHED_ID				= 0x3363,
			TOU_EXPIRATION_DATE			= 0x3364,
			EXPIRATION_YEAR_ADDRESS 	= 0x3368,
			DST_TABLE					= 0x33B6,
            LAST_PROGRAMMED_DATE        = 0x3483,
            LAST_RESET_DATE             = 0x3488,
            MINUTES_ON_BATTERY          = 0x34A7
				
		};

		/// <summary>
		/// FULCErrors enumeration encapsulates the FULCRUM  
		/// non-fatal errors bitmasks.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00 N/A	Created
		/// 
		private enum FULCErrors : byte
		{
			LOW_BATTERY					= 0x01,
			PHASE_LOSS					= 0x40
		};

		/// <summary>
		/// FULCDiagErrors enumeration encapsulates the FULCRUM diagnostic
		/// errors bitmasks.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00 N/A	Created
		/// 
		private enum FULCDiagErrors : byte
		{
			CLOCK						= 0x02,
			MASS_MEMORY					= 0x04,
			REGISTER_FULL_SCALE			= 0x08,
			INPUT_OUTPUT				= 0x10,
			PHASE_LOSS					= 0x40,
			TOU							= 0x80
		};

		/// <summary>
		/// FULCErrorIndex enumeration encapsulates the indicies for each 
		/// general type of error based on the order in the FULCRUM.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00 N/A	Created
		/// 
		private enum FULCErrorIndex : int
		{
			NON_FATAL					= 0,
			DIAGNOSTIC					= 1
		};

		/// <summary>
		/// FULCDSTRecordIndex enumeration encapsulates the FULCRUM DST Record
		/// Indicies.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/13/06 jrf 7.30.00 N/A	Created
		/// 
		private enum FULCDSTRecordIndex : int
		{
			YEAR						= 0,
			TO_HOUR						= 1,
			TO_DAY						= 2,
			TO_MONTH					= 3,
			FROM_HOUR					= 4,
			FROM_DAY					= 5,
			FROM_MONTH					= 6
		};

		
		
		#endregion Definitions

        #region Public Methods

        /// <summary>
        /// This method adjusts the clock on a connected SCS device.
        /// </summary>
        /// <param name="intOffset">Offset from meter time</param>
        /// <returns>A ClockAdjustResult representing the result of the 
        /// clock adjust.</returns>
        /// <exception cref="SCSException">
        /// Thrown when a NAK is received from a SCS protocol request.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication Comm = new Communication();
        /// Comm.OpenPort("COM4:");
        /// SCSDevice SCSDevice = new SCSDevice(Comm);
        /// SCSDevice.Logon();
        /// SCSDevice.Security();
        /// SCSDevice.AdjustClock(0);
        /// SCSDevice.Logoff();		
        /// </code>
        /// </example>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/26/07 KRC  8.00.09       Moved into FULCRUM so not all meters did adjust clock.
        // 
        public override ClockAdjustResult AdjustClock(int intOffset)
        {
            ClockAdjustResult Result = ClockAdjustResult.SUCCESS;
            DateTime NewTime;
            DateTime NextIntervalTime;
            DateTime CurrentIntervalTime;
            DateTime CurrentTime;
            byte[] byNewDateTime;
            byte[] byClockRunFlag;
            string strExceptionItemName = m_rmStrings.GetString("ADJUST_CLOCK");
            int iIntervalSize = 0;
            bool bCheckIntBound = false;
            bool bReconfigureClock = false;

            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting AdjustClock");

            // Check clock
            if (!m_clockEnabled.Cached)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Checking Clock");
                Result = SCSToClockAdjustResult(m_SCSProtocol.Upload(
                    ClockRunFlagAddress,
                    SCS_FLAG_LENGTH,
                    out byClockRunFlag));
                if (ClockAdjustResult.SUCCESS == Result)
                {
                    m_clockEnabled.Value = (byClockRunFlag[0] != SCS_FLAG_OFF);
                }
            }

            if (m_clockEnabled.Cached && !m_clockEnabled.Value)
            {
                Result = ClockAdjustResult.ERROR_CLOCK_NOT_RUNNING;
            }

            if (ClockAdjustResult.SUCCESS == Result)
            {
                // Check to see if load profile is running; if so we need to 
                // make sure time adjust doesn't cross interval boundary
                if (LPRunning)
                {
                    // Load profile is enabled, must check that we 
                    // don't cross interval boundary
                    bCheckIntBound = true;

                    // Upload interval size
                    iIntervalSize = LPIntervalLength;
                }
            }

            if (ClockAdjustResult.SUCCESS == Result)
            {
                try
                {
                    if (bCheckIntBound)
                    {
                        // Get Meter Time for computing next interval start time
                        CurrentTime = DeviceTime;

                        // Retreive int boundary times
                        GetCurrentAndNextIntervalStartTimes(
                            CurrentTime,
                            out NextIntervalTime,
                            out CurrentIntervalTime,
                            iIntervalSize);

                        // Set Time factoring in the subsequent 1 sec. 
                        // required pause
                        NewTime = CurrentTime.AddSeconds(intOffset + 1);

                        if (NewTime >= NextIntervalTime ||
                            NewTime < CurrentIntervalTime)
                        {
                            // Can't adjust clock for crossing an 
                            // interval boundary
                            Result = ClockAdjustResult.ERROR_CROSSES_INTERVAL;
                        }
                    }
                    else
                    {
                        // Get Meter Time 
                        CurrentTime = DeviceTime;

                        // Set Time factoring in the subsequent 1 sec. 
                        // required pause
                        NewTime = CurrentTime.AddSeconds(intOffset + 1);
                    }

                    if (ClockAdjustResult.SUCCESS == Result)
                    {
                        // Stop Metering while adjusting clock, only SCS devices 
                        // that need to stop metering will override this virtual 
                        // method, otherwise it just returns an ACK response
                        Result = SCSToClockAdjustResult(StopMetering(true));


                        if (ClockAdjustResult.SUCCESS == Result)
                        {
                            // jrf - SCR 29
                            // Moved disabling of clock and MM until after interval
                            // bounds have been checked.

                            // This function performs set up work, i.e turn off MM,
                            // turn off clock.  
                            Result = ClockAdjustSetUp(bCheckIntBound);

                            // Since we have turned off the clock, make sure it gets
                            // turned back on
                            bReconfigureClock = true;

                            // Meter needs a 1 second pause
                            Thread.Sleep(1000);
                        }

                        if (ClockAdjustResult.SUCCESS == Result)
                        {
                            // Prepare the new time for download to meter
                            PrepareTime(out byNewDateTime, NewTime);

                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Time");
                            // Download the time to the meter
                            Result = SCSToClockAdjustResult(
                                m_SCSProtocol.Download(
                                RealTimeClockAddress,
                                SCS_REAL_TIME_LENGTH,
                                ref byNewDateTime));
                        }

                        if (ClockAdjustResult.SUCCESS == Result)
                        {
                            // This function sets the appropriate reconfigure flag.  
                            Result = ClockAdjustSetReconfigureFlag(bCheckIntBound);
                        }
                        else if (bReconfigureClock)
                        {
                            // jrf - SCR 29
                            // When reenabling the clock after a failure the 
                            // reconfigure flags must be used to reenable clock.

                            // The clock adjust has failed.  Turn clock back on.  
                            // Don't worry about saving result since we want to 
                            // leave it at what caused the failure.
                            ClockAdjustSetReconfigureFlag(bCheckIntBound);
                        }

                        // Always try to resume metering inside this if-block since
                        // Metering was stopped successfully to get here
                        SCSProtocolResponse StopMeterResponse = StopMetering(false);

                        // Only change Protocol response on a non-ACK due to fact 
                        // that if StopMeter is not overridden it will always return
                        // an ACK
                        if (SCSProtocolResponse.SCS_ACK != StopMeterResponse)
                        {
                            Result = SCSToClockAdjustResult(StopMeterResponse);
                        }
                    }// End if ( Result == Success )                    
                }
                catch (Exception e)
                {
                    // If anything happens try to put the meter back the 
                    // way you found it.

                    // Turn the clock back on if necessary  
                    if (bReconfigureClock)
                    {
                        Result = ClockAdjustSetReconfigureFlag(bCheckIntBound);
                    }
                    StopMetering(false);
                    throw (e);
                }

            }// End if ( Result == SUCCESS )

            return Result;
        } // End AdjustClock()


        #endregion

        /// <summary>
        /// The following is the length of most FULCRUM time stamps
        /// </summary>
        private const int SCS_FULCRUM_DATE_TIME_LENGTH = 5;

		private byte[]	m_byFULCDSTTable;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objSerialPort">The communication object used for the 
		/// serial port communications.</param>
		/// <example>
		/// <code>
		/// Communication objComm = new Communication();
		/// objComm.OpenPort("COM4:");
		/// FULCRUM objFULCRUM = new FULCRUM(objComm);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        public FULCRUM(ICommunications objSerialPort)
            :
            base(objSerialPort) 
		{
			m_byFULCDSTTable = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="protocol">The SCS protocol object used for communications
		/// with SCS devices.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// FULCRUM fulcrum = new FULCRUM(scsProtocol);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed to pass protocol object to base 
		///								Constructor
		///
		public FULCRUM(SCSProtocol protocol)
			:
			base(protocol) 
		{
			protocol.MaxDownloadSize = FULC_MAX_WRITE_SIZE;
		}

		/// <summary>This property gets the expected device type "X20".</summary>
		/// <returns>
		/// A string representing the expected device type.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override string ExpectedDeviceType
		{ 
			get
			{
				return m_rmStrings.GetString("FULC_DEVICE_TYPE"); 
			}
		}		
		/// <summary>This property gets the hang up flag address.</summary>
		/// <returns>
		/// An int representing the hang up flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int CommunicationsHangUpFlagAddress
		{
			get
			{
				return (int)FULCAddresses.HANGUP_FLAG; 
			}
		}

        /// <summary>This property gets the demand reset flag address.</summary>
        /// <returns>
        /// An int representing the demand reset flag address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 mah 7.35.00  N/A   Created
        ///	
        protected override int DemandResetFlagAddress
        {
            get
            {
                return (int)FULCAddresses.DEMAND_RESET_FLAG;
            }
        }

        /// <summary>This property gets the clear billing data flag address.</summary>
        /// <returns>
        /// An int representing the clear billing data flag address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 mah 7.35.00  N/A   Created
        ///	
        protected override int ClearBillingDataFlagAddress
        {
            get
            {
                return (int)FULCAddresses.CLEAR_BILLING_FLAG;
            }
        }

        /// <summary>This property gets the address of the test mode flag address.</summary>
        /// <returns>
        /// An int representing the test mode flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/21/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        protected override int RemoteTestModeFlagAddress
        {
            get
            {
                return (int)FULCAddresses.TEST_MODE_FLAG;
            }
        }


		/// <summary>This property gets the stop clock flag address.</summary>
		/// <returns>
		/// An int representing the stop clock flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        protected override int StopClockFlagAddress
		{
			get
			{
				return (int)FULCAddresses.CLOCK_RUN_FLAG; 
			}
		}
		
		/// <summary>This property gets the TOU run flag address.</summary>
		/// <returns>
		/// An int representing the TOU run flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int TOURunFlagAddress
		{
			get
			{
				return (int)FULCAddresses.TOU_RUN_FLAG; 
			}
		}

		/// <summary>This property gets the model type address.</summary>
		/// <returns>
		/// An int representing the model type address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        protected override int ModelTypeAddress
		{
			get
			{
				return (int)FULCAddresses.MODEL_TYPE; 
			}
		}

        /// <summary>This property gets the last reset date address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/21/06 mah 7.35.00  N/A   Created
        ///
        protected override int LastResetDateAddress
        {
            get
            {
                return (int)FULCAddresses.LAST_RESET_DATE;
            }
        }

        /// <summary>This property gets the demand reset count address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/21/06 mah 7.35.00  N/A   Created
        ///
        protected override int NumResetsAddress
        {
            get
            {
                return (int)FULCAddresses.RESET_COUNT;
            }
        }

        /// <summary>This property gets the outage count address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/21/06 mah 7.35.00  N/A   Created
        ///
        protected override int NumOutagesAddress
        {
            get
            {
                return (int)FULCAddresses.OUTAGE_COUNT;
            }
        }

        /// <summary>This property gets the last programmed date address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/21/06 mah 7.35.00  N/A   Created
        ///
        protected override int LastProgrammedDateAddress
        {
            get
            {
                return (int)FULCAddresses.LAST_PROGRAMMED_DATE;
            }
        }

        /// <summary>This property gets the address of the program count.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/21/06 mah 7.35.00  N/A   Created
        ///
        protected override int NumTimesProgrammedAddress
        {
            get
            {
                return (int)FULCAddresses.PROGRAM_COUNT;
            }
        }

        /// <summary>This property returns the address of the number of minutes the 
        /// device was on battery power.
        /// </summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------  
        /// 09/08/06 mah 7.35.00  N/A   Created
        protected override int NumOfMinutesOnBatteryAddress
        {
            get
            {
                return (int)FULCAddresses.MINUTES_ON_BATTERY;
            }
        }



		/// <summary>This property gets the TOU reconfigure flag address.</summary>
		/// <returns>
		/// An int representing the TOU reconfigure flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int TOUReconfigureFlagAddress
		{
			get
			{
				return (int)FULCAddresses.TOU_RECONFIGURE_FLAG; 
			}
		}
		
		/// <summary>This property gets the firmware version address.</summary>
		/// <returns>
		/// An int representing the firmware version address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        protected override int FWVersionAddress
		{
			get
			{
				return (int)FULCAddresses.FIRMWARE_REVISION; 
			}
		}

		/// <summary>This property gets the software version address.</summary>
		/// <returns>
		/// An int representing the software version address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int SWVersionAddress
		{
			get
			{
				return (int)FULCAddresses.SOFTWARE_REVISION; 
			}
		}
		
		/// <summary>This property gets the program ID address.</summary>
		/// <returns>
		/// An int representing the program ID address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        protected override int ProgramIDAddress
		{
			get
			{
				return (int)FULCAddresses.PROGRAM_ID; 
			}
		}

        /// <summary>This property gets the address of the TOU Schedule ID.</summary>
        /// <returns>
        /// An int representing the TOU Schedule ID address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/14/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected override int TOUScheduleIDAddress
        {
            get
            {
                return (int)FULCAddresses.TOU_SCHED_ID;
            }
        }


		/// <summary>This property gets the clock reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the clock reconfigure flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int ClockReconfigureFlagAddress
		{
			get
			{
				return (int)FULCAddresses.CLOCK_RECONFIGURE_FLAG; 
			}
		}

		/// <summary>This property gets the clock reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the clock reconfigure flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/18/06 mcm 7.30.00  N/A   Created
		///	
        protected int MeterReconfigureFlagAddress
		{
			get
			{
				return (int)FULCAddresses.METER_RECONFIGURE_FLAG; 
			}
		}		
		/// <summary>This property gets the operating setup address.</summary>
		/// <returns>
		/// An int representing the operating setup address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/25/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int OperatingSetupAddress
		{
			get
			{
				return (int)FULCAddresses.OPERATING_SETUP; 
			}
		}

        /// <summary>This property gets the cold load pickup time address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected override int ColdLoadPickupTimeAddress
        {
            get
            {
                return (int)FULCAddresses.COLD_LOAD_PICKUP_TIME;
            }
        }

        /// <summary>
        /// This property gets the address of the first byte of demand 
        /// configuration data
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected override int DemandConfigurationAddress
        {
            get
            {
                return (int)FULCAddresses.DEMAND_CONFIGURATION;
            }
        }


		/// <summary>This property gets the TOU calendar address.</summary>
		/// <returns>
		/// An int representing the TOU calendar address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///
        protected override int TOUCalendarAddress
		{
			get
			{
				if (!m_touCalendarStartAddress.Cached)
                {
                    m_touCalendarStartAddress.Value = 
                        (int)FULCAddresses.YEARLY_SCHEDULE;
                }
                return m_touCalendarStartAddress.Value;
			}
		}
		/// <summary>This property the address of the last available byte that
		/// can be used for TOU configuration. This value is meter, firmware, 
		/// configuration dependent.</summary>
		/// <returns>An int representing the address of the last byte that can
		/// be used by the TOU configuration.</returns>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/10/06 mcm 7.30.00  N/A   Created
		///		
		protected override int TOUConfigLastAvailAddress
		{
			get
			{
				return LAST_AVAIL_TOU_ADDRESS;
				
			}
		}

		/// <summary>This property gets the real time clock address.</summary>
		/// <returns>
		/// An int representing the real time clock address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/26/06 jrf 7.30.00  N/A	Modified
		///	
        protected override int RealTimeClockAddress
		{
			get
			{
				return (int)FULCAddresses.REAL_TIME; 
			}
		}

		/// <summary>This property gets the communication timeout address.
		/// </summary>
		/// <returns>
		/// An int representing the communication timeout address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00  N/A	Created
		///
		protected override int CommunicationTimeoutAddress
		{
			get
			{
				return (int)FULCAddresses.COMMUNICATIONS_TIMEOUT; 
			}
		}
		
		/// <summary>This property gets the interval length address.</summary>
		/// <returns>
		/// An int representing the interval length address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00  N/A	Created
		///
		protected override int IntervalLengthAddress
		{
			get
			{
				return (int)FULCAddresses.MM_INTERVAL_LENGTH; 
			}
		}

		/// <summary>This property gets the serial number address.</summary>
		/// <returns>
		/// An int representing the serial number address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00  N/A	Created
		///
		protected override int SerialNumberAddress
		{
			get
			{
				return (int)FULCAddresses.SERIAL_NUMBER; 
			}
		}


        /// <summary>
        /// This property gets the address of the display format flags.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected override int EnergyFormatAddress
        {
            get
            {
                // Note that the fulcrum has the formatting information inside the 
                // display list.  Therefore this basepage item is not supported at 
                // all

                throw (new NotSupportedException());
            }
        }

        /// <summary>
        /// This property gets the address of the meter's display options.
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/07/06 mah 8.00.00  N/A   Created
        /// </remarks>
        protected override int DisplayOptionsAddress
        {
            get
            {
                // Note that the fulcrum has the formatting information inside the 
                // display list.  Therefore this basepage item is not supported at 
                // all

                throw (new NotSupportedException());
            }
        }
        
        /// <summary>This property gets the serial number length.</summary>
		/// <returns>
		/// An int representing the serial number length.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/26/06 jrf 7.30.00  N/A	Created
		///
		protected override int SerialNumberLength
		{
			get
			{
				return FULC_SERIAL_NUMBER_LENGTH; 
			}
		}

		/// <summary>This property gets the unit ID address.</summary>
		/// <returns>
		/// An int representing the unit ID address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A	Created
		/// 05/26/06 jrf 7.30.00  N/A	Created
		///
		protected override int UnitIDAddress
		{
			get
			{
				return (int)FULCAddresses.UNIT_ID; 
			}
		}

        /// <summary>This property gets the address of the first user defined data field.</summary>
        /// <returns>
        /// An int representing the user defined field address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/16/06 mah 8.00.00  N/A	Created
        /// </remarks>
        protected override int UserDataBlockAddress
        {
            get
            {
                return (int)FULCAddresses.USERDEFINED_FIELD1;
            }
        }


        /// <summary>This property gets the address of the start of the display table.</summary>
        /// <returns>
        /// An int representing the first display item address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/05/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected override int DisplayTableAddress
        {
            get
            {
                return (int)FULCAddresses.DISPLAY_TABLE;
            }
        }


        /// <summary>
        /// Property used to get the meter type (string).  Use
        /// this property for meter determination and comparison.  
        /// This property should not be confused with MeterName which
        /// is used to obtain a human readable name of the meter.
        /// </summary>
		/// <returns>
		/// A string representing the meter type.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
        /// 03/16/07 jrf 8.00.18        Changed from resource string to constant
		///	
		public override string MeterType
		{
			get
			{
				return FULCRUM_TYPE;
			}
		}

        /// <summary>
        /// Property used to get the human readable meter name 
        /// (string).  Use this property when 
        /// displaying the name of the meter to the user.  
        /// This should not be confused with the MeterType 
        /// which is used for meter determination and comparison.
        /// </summary>
        /// <returns>A string representing the human readable name of the 
        /// meter.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/07 jrf 8.00.19 2653   Created
        //
        public override string MeterName
        {
            get
            {
                return FULCRUM_NAME;
            }
        }

        /// <summary>
        /// Gets the Date of the TOU Expiration
        /// </summary>
        /// <exception>
        /// SCSException will be thrown if the value cannot be downloaded from
        /// the meter
        /// </exception>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/12/06 MAH 7.35.00 N/A    Created
        ///	</remarks>	
        public override DateTime TOUExpirationDate
        {
            get
            {
                if (!m_dateTOUExpiration.Cached)
                {
                    SCSTOUInfo localTOUInfo = new SCSTOUInfo();

                    SCSProtocolResponse ProtocolResponse = ReadTOUInfo(out localTOUInfo);

                    if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                    {
                        m_dateTOUExpiration.Value = localTOUInfo.ExpirationDate;
                    }
                    else
                    {
                        SCSException scsException = new SCSException(
                            SCSCommands.SCS_U,
                            ProtocolResponse,
                            TOUExpirationAddress,
                            "TOU Expiration Date");

                        throw scsException;
                    }
                }

                return m_dateTOUExpiration.Value;
            }
        }

        /// <summary>
        /// TOU is considered enabled if the clock is running and the meter
        /// is configured to follow a TOU schedule.  TOU does not have to be
        /// running for this property to return true.  For example an expired
        /// TOU schedule is enabled but not running.
        /// </summary>
        /// <returns>
        /// An true if TOU is configured and the clock is running.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 10/25/06 mcm 7.35.07 106    Clarified definition and modified to match
        ///	</remarks>
        ///	
        public override bool TOUEnabled
        {
            get
            {
                if (!m_touEnabled.Cached)
                {
                    if (ClockEnabled)
                    {
                        byte[] byFirmwareOptions;
                        SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed, 
                                            "Read Firmware Options");
                        
                        ProtocolResponse = m_SCSProtocol.Upload(
                            (int)FULCAddresses.FIRMWARE_OPTIONS,
                            sizeof(byte), out byFirmwareOptions);

                        if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                        {
                            //Extract DST configuration
                            m_touEnabled.Value = (0 != (byFirmwareOptions[0] & FULC_TOU_MASK));
                        }
                        else
                        {
                            SCSException scsException = new SCSException(
                                SCSCommands.SCS_U,
                                ProtocolResponse,
                                (int)FULCAddresses.FIRMWARE_OPTIONS,
                                m_rmStrings.GetString("FIRMWARE_OPTIONS"));
                            throw scsException;
                        }

                    }
                    else
                    {
                        m_touEnabled.Value = false;
                    }
                }

                return m_touEnabled.Value;
            }
        }

		/// <summary>This property gets the DST mask for the FULCRUM.</summary>
		/// <returns>
		/// An byte representing the DST mask.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00  N/A   Created
		///	
		protected override byte DSTMask
		{
			get
			{
				return FULC_DST_MASK;
			}
		}

		/// <summary>
		/// This property gets the load profile run flag address.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
		///		
		protected override int LoadProfileFlagAddress
		{
			get 
			{
				return (int)FULCAddresses.MM_RUN_FLAG;
			}
		}

		/// <summary>
		/// This property throws an exception since this address does not 
		/// exist in the fulcrum.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		/// <exception cref="ApplicationException">
		/// Thrown when this property is called since it should never be 
		/// needed.
		/// </exception>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
		///		
		protected override int LoadProfileAdjustTimeFlagAddress
		{
			get
			{
				throw new System.ApplicationException(m_rmStrings.GetString("FULCRUM_MISSING_FLAG"));
			}
		}

		/// <summary>
		/// This property gets the tou expiration address.
		/// </summary>
		/// <returns>
		/// An int representing the tou expiration address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/12/06 jrf 7.30.00  N/A   Created
		///		
		protected override int TOUExpirationAddress
		{
			get
			{
				return (int)FULCAddresses.TOU_EXPIRATION_DATE;
			}
		}

		/// <summary>
		/// Gets the DST table address
		/// </summary>
		/// <returns></returns>
		protected int DSTTableAddress 
		{ 
			get
			{
				return (int)FULCAddresses.DST_TABLE; 
			}		
		}

		/// <summary>This property gets the clock run flag address.</summary>
		/// <returns>
		/// An int representing the clock run flag address.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/13/06 jrf 7.30.00  N/A   Created
		///
		protected override int ClockRunFlagAddress
		{
			get
			{
				return (int)FULCAddresses.CLOCK_RUN_FLAG;
			}
		}

		/// <summary>This property gets the maximum number of TOU calendar 
		/// events to read from an SCS device.</summary>
		/// <returns>
		/// An int representing the number of events.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00  N/A   Created
		///	
		protected override int MaxWriteSize
		{
			get
			{
				return FULC_MAX_WRITE_SIZE;
			}
		}

        /// <summary>Returns the address of the primary security code.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/16/06 mcm 7.35.00  N/A   Created
        ///				
        protected override int PrimaryPasswordAddress
        {
            get
            {
                return (int)FULCAddresses.PRIMARY_PASSWORD;
            }
        }

        /// <summary>Returns the address of the Secondary security code.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///				
        protected override int SecondaryPasswordAddress
        {
            get
            {
                return (int)FULCAddresses.SECONDARY_PASSWORD;
            }
        }

        /// <summary>Returns the address of the Tertiary security code.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///				
        protected override int TertiaryPasswordAddress
        {
            get
            {
                return (int)FULCAddresses.TERTIARY_PASSWORD;
            }
        }

        /// <summary>
        /// This property returns the address of the number of load profile channels
        /// </summary>
        /// <returns>
        /// An int representing a basepage address 
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/05/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected override int ChannelCountAddress
        {
            get
            {
                return (int)FULCAddresses.NUMBER_OF_CHANNELS;
            }
        }

		/// <summary>This property gets the transformer ratio address.</summary>
		/// <returns>
		/// An int representing the transformer ratio address.
		/// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		protected override int TransformerRatioAddress
		{
			get
			{
				return (int)FULCAddresses.TRANSFORMER_RATIO;
			}
		}

        /// <summary>This property gets the Load Research ID address.</summary>
        /// <returns>
        /// An int representing the Load Research ID address.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/13/07 mcm 8.00.18		Created
        //  
        protected override int LoadResearchIDAddress
        {
            get
            {
                return 0;
            }
        }

		/// <summary>
		/// This method reads the time from the FULCRUM. 
		/// </summary>
		/// <param name="objMeterTime">The object to hold the SCS device's 
		/// time</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/27/06 mrj 7.30.00  N/A   Created
		//  05/26/06 jrf 7.30.00  N/A   Revised
		//  06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        //  10/18/06 mrj 7.35.05 44     Set the default time to 1/1/1980 to match
        //                              Pc-Pro+
		//
        override protected void ReadDeviceTime(out DateTime objMeterTime)
        {
            byte[] abytRealTime;
            SCSProtocolResponse objProtocolResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Device Time" );
			objProtocolResponse = 
				m_SCSProtocol.Upload(
					(int)FULCAddresses.REAL_TIME, 
					SCS_REAL_TIME_LENGTH, 
					out abytRealTime);

			objMeterTime = new DateTime();    
		   
			if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
            {
                try
                {
                    objMeterTime = new DateTime(
						abytRealTime[6] + SCS_CURRENT_YEAR_BASE, 
                        abytRealTime[5],
                        abytRealTime[4],
                        abytRealTime[3],
                        abytRealTime[2],
                        abytRealTime[1]);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // The date&time have not been set - 
					// Set to 1/1/1980 to match Pc-Pro+
                    objMeterTime = new DateTime(1980, 1, 1);
                }
            }
            else
            {
                SCSException objSCSException = 
					new SCSException(
						SCSCommands.SCS_U, 
						objProtocolResponse, 
						RealTimeClockAddress, 
						m_rmStrings.GetString("DEVICE_TIME"));
                throw objSCSException;
            }
        }

		/// <summary>
		/// This method reads the firmware version from the FULCRUM. 
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/30/06 jrf 7.30.00  N/A   Revised
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		///
        override protected void ReadFWVersion()
        {
            byte[] abytVersion;
            SCSProtocolResponse objProtocolResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Firmware Version" );
			objProtocolResponse = m_SCSProtocol.Upload(
				(int)FULCAddresses.FIRMWARE_REVISION, 
				FULC_FW_VERSION_LENGTH, 
				out abytVersion);
            
			if ( SCSProtocolResponse.SCS_ACK == objProtocolResponse )
            {
                m_fwVersion.Value = 
					TranslateVersion(abytVersion, FULC_FW_VERSION_LENGTH);
            }
            else
            {
                SCSException e = new SCSException(
					SCSCommands.SCS_U, objProtocolResponse, 
					(int)FULCAddresses.FIRMWARE_REVISION, 
					"FW Version");
                throw e;
            }
        }

		/// <summary>
		/// This method reads the software version from the FULCRUM. 
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/30/06 jrf 7.30.00  N/A   Revised
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		///
		override protected void ReadSWVersion()
		{
			byte[] abytVersion;
			SCSProtocolResponse objProtocolResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Software Version" );
			objProtocolResponse = m_SCSProtocol.Upload(
				(int)FULCAddresses.SOFTWARE_REVISION, 
				FULC_SW_VERSION_LENGTH, 
				out abytVersion);
            
			if ( SCSProtocolResponse.SCS_ACK == objProtocolResponse )
			{
				m_swVersion.Value = 
					TranslateVersion(abytVersion, FULC_SW_VERSION_LENGTH);
			}
			else
			{
				SCSException e = new SCSException(
					SCSCommands.SCS_U, objProtocolResponse, 
					(int)FULCAddresses.SOFTWARE_REVISION, 
				    "SW Version");
				throw e;
			}
		}

        /// <summary>
        /// This method reads the Time of Use Schedule ID from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the TOU id cannot be retreived from the meter.
        /// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/14/06 mah 8.00.00  N/A   Revised
        /// </remarks>
        override internal void ReadTOUScheduleID()
        {
            throw (new NotImplementedException());
        }// End ReadTOUScheduleID()

        /// <summary>
        /// This method is not implemented for the Fulcrum. The Fulcrum doesn't
        /// implement the SizeOfSeasonN properties that the base class' 
        /// implementation of this method uses.
        /// </summary>
        /// <exception cref="SCSException">
        /// Always throws a NotImplementedException.
        /// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 8.00.20  N/A   Revised
        /// </remarks>
        override internal SCSProtocolResponse ReadSeasons(SCSTOUInfo TOUInfo,
            out SCSSeasonCollection SeasonList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method reads the Number of minutes the device has been on 
        /// battery power
        /// </summary>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/12/06 mah 7.35.00  N/A   Created
        /// </remarks>
        override protected void ReadMinutesOnBattery()
        {
            byte[] abytResponse;
            SCSProtocolResponse objProtocolResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Minutes On Battery");

            objProtocolResponse = m_SCSProtocol.Upload(
                NumOfMinutesOnBatteryAddress,
                FULC_MINUTES_ON_BATTERY_LENGTH,
                out abytResponse);

            if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
            {
                m_numMinOnBattery.Value =
                    (uint)SCSConversion.BytetoInt(ref abytResponse, FULC_MINUTES_ON_BATTERY_LENGTH);
            }
            else
            {
                SCSException e = new SCSException(
                    SCSCommands.SCS_U, objProtocolResponse,
                    NumOfMinutesOnBatteryAddress,
                    "Minutes On Battery");
                throw e;
            }
        }

        /// <summary>
        /// This method reads the date of the last demand reset from the meter. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the date cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        override protected DateTime ReadLastResetDate()
        {
            return ReadFulcrumDateTime(LastResetDateAddress, "Last Reset Date");
        }

        /// <summary>
        /// This method reads and caches the demand interval length, number of subintervals,
        /// test mode inteval length, and test mode number of subintervals. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the user data fields cannot be retreived from the meter.
        /// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        /// </remarks>
        override protected void ReadDemandConfiguration()
        {
            byte[] byDemandConfiguration;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Fulcrum Demand Configuration");

            // Note that we are going to take advantage of the fact that the user data fields
            // are physically next to each other.  This way we can get the values of all the
            // fields with a single read rather than multiple reads. Remember - faster is better
            ProtocolResponse = m_SCSProtocol.Upload(
                DemandConfigurationAddress,
                FULCRUM_DEMANDCONFIGURATIONBLOCK_LENGTH,
                out byDemandConfiguration);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                // The byte of the demand configuration block @ 0x27FD holds the number of sub intervals 

                m_NumberOfSubIntervals.Value = byDemandConfiguration[0];

                // The byte of the demand configuration block @ 0x27FE holds the subinterval length 

                m_DemandIntervalLength.Value = byDemandConfiguration[1] * m_NumberOfSubIntervals.Value;

                // The byte of the demand configuration block @ 0x27FF holds the test mode number of sub intervals 

                m_NumberOfTestModeSubIntervals.Value = byDemandConfiguration[2];

                // The byte of the demand configuration block @ 0x2800 holds the test mode subinterval length 

                m_TestModeIntervalLength.Value = byDemandConfiguration[3] * m_NumberOfTestModeSubIntervals.Value;
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    SWVersionAddress,
                    "Reading demand configuration block");
                throw scsException;
            }
        } // End ReadDemandConfiguration()

        /// <summary>
        /// This method reads the date and time of the last programming event from 
        /// meter. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the date cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        override protected void ReadDateLastProgrammed()
        {
            m_dateLastProgrammed.Value = ReadFulcrumDateTime(LastProgrammedDateAddress, "Last Programmed Date");
        }

        /// <summary>
        /// This method reads a 5 byte time stamp from the meter.  The timestamp
        /// is in the M-D-H-M format.
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the date cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        protected DateTime ReadFulcrumDateTime(int nAddress, string strFieldName)
        {
            DateTime dtMeterTimeStamp;
            byte[] byDataBuffer;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading " + strFieldName);

            // Read the data byte from the meter

            ProtocolResponse = m_SCSProtocol.Upload(
                nAddress,
                SCS_FULCRUM_DATE_TIME_LENGTH,
                out byDataBuffer);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                int nYear = byDataBuffer[4];
                int nMonth = byDataBuffer[3];
                int nDay = byDataBuffer[2];

                if ((0 < nMonth) && (0 < nDay)) // we have a valid year
                {
                    dtMeterTimeStamp = new DateTime(nYear + SCS_CURRENT_YEAR_BASE,
                                            nMonth, // Month
                                            nDay, // Day
                                            byDataBuffer[1], // Hour
                                            byDataBuffer[0], // minute
                                            0); // Second
                }
                else // The meter's clock is not running or the event has not occurred
                {
                    dtMeterTimeStamp = new DateTime(SCS_CURRENT_YEAR_BASE, 1, 1);
                }
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    nAddress,
                    strFieldName);

                throw scsException;
            }

            return dtMeterTimeStamp;
        }

		/// <summary>
		/// This method translates the program ID read from the FULCRUM. 
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		/// 
		override protected int TranslateProgramID(ref byte[] abytProgramID, int intLength)
		{          
			return SCSConversion.BytetoInt(ref abytProgramID, intLength);
		}


        /// <summary>
        /// This method translates the CLPU time from device format into a number of minutes 
        /// </summary>
        /// <remarks>
        /// This method is necessary due to the fact that the CENTRON and VECTRON
        /// stores the CLPU time in seconds while FULCRUM stores the time in minutes.
        /// </remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        /// 
        override protected uint TranslateCLPU(ref byte[] byCLPU, int intLength)
        {
            return (uint)(SCSConversion.BytetoInt(ref byCLPU, intLength));
        }


        /// <summary>
        /// This method translates a 2 byte event counter into an integer value. 
        /// </summary>
        /// <remarks>This method is necessary due to the fact that not all SCS devices
        /// store the number of demand resets in the same format.  Most devices
        /// use BCD format but this method should be overridden for devices like
        /// the FULCRUM which use unsigned integers</remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        override protected int TranslateEventCounter(ref byte[] byInputBuffer, int intLength)
        {
            return SCSConversion.BytetoInt(ref byInputBuffer, intLength);
        }

		/// <summary>
		/// This method performs set up operations that should occur before a 
		/// clock adjustment.  
		/// </summary>
		/// <param name="massMemoryEnabled">A boolean determining if mass memory is 
		/// enabled.</param>
		/// <returns>ClockAdjustResult indicating the results</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/29/06 jrf 7.30.00  N/A   Created
		override protected ClockAdjustResult ClockAdjustSetUp(bool massMemoryEnabled)
		{
			byte[] byFlag = new byte[1];
			ClockAdjustResult Result = ClockAdjustResult.ERROR;

			byFlag[0] = SCS_FLAG_OFF;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Turning Off Clock" );
			// Clear the Clock Option Run Flag
			Result = SCSToClockAdjustResult( m_SCSProtocol.Download(
				StopClockFlagAddress, 
				SCS_FLAG_LENGTH, 
				ref byFlag ) );

			return Result;
		}

		/// <summary>
		/// This method sets the appropriate time reconfigure flag that should be set
		/// after a clock adjustment.  
		/// </summary>
		/// <param name="massMemoryEnabled">A boolean determining if mass memory is 
		/// enabled.</param>
		/// <returns>ClockAdjustResult indicating the results</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/29/06 jrf 7.30.00  N/A   Created
		override protected ClockAdjustResult ClockAdjustSetReconfigureFlag(bool massMemoryEnabled)
		{
			byte[] byFlag = new byte[1];
			ClockAdjustResult Result = ClockAdjustResult.ERROR;

			byFlag[0] = SCS_FLAG_ON;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Clock Reconfigure Flag" );
			// Set the Clock Reconfigure Flag
			Result = SCSToClockAdjustResult( m_SCSProtocol.Download(
				ClockReconfigureFlagAddress, 
				SCS_FLAG_LENGTH, 
				ref byFlag ) );

			return Result;
		}
	
		/// <summary>
		/// Updates DST calendar on the connected SCS device.
		/// </summary>
		/// <returns>A DSTUpdateResult</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 06/12/06 jrf 7.30.00  N/A   Modified
		/// 
		override protected DSTUpdateResult UpdateDSTCalendar()
		{
			DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
			byte[] byClockRunFlag;

			// Determine if meter has a working clock
			if ( !m_clockEnabled.Cached )
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Clock Run Flag" );
				Result = SCSToDSTResult( m_SCSProtocol.Upload(
					ClockRunFlagAddress, 
					SCS_FLAG_LENGTH, 
					out byClockRunFlag ) );
				if ( DSTUpdateResult.SUCCESS == Result )
				{
					m_clockEnabled.Value = (byClockRunFlag[0] != SCS_FLAG_OFF);
				}				
			}
			
			if ( m_clockEnabled.Cached && !m_clockEnabled.Value )
			{
				Result = DSTUpdateResult.CLOCK_ERROR;			
			}
			
			// First verify that the meter is actually configured for DST 
			if ( DSTUpdateResult.SUCCESS == Result && !DSTEnabled )
			{
				Result = DSTUpdateResult.SUCCESS_NOT_CONFIGURED_FOR_DST;
			}

			if ( DSTUpdateResult.SUCCESS == Result )
			{
				// First update the Fulcrum specific DST table
				Result = ReadFULCDSTTable();
			}

			if ( DSTUpdateResult.SUCCESS == Result )
			{
				// Download the updated calendar to the meter
				Result = PostFULCDSTTable();
			}

			if ( DSTUpdateResult.SUCCESS == Result  )
			{
				// This was done in previous DST update code
				SetTOUReconfigureFlag();
			}

			return Result;
		}
		/// <summary>
		/// Reads DST calendar from the FULCRUM and updates calendar values 
		/// for a subsequent write.
		/// </summary>
		/// <returns>A DSTUpdateResult</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 06/13/06 jrf 7.30.00  N/A   Rewrote
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
		///								
        private DSTUpdateResult ReadFULCDSTTable()
        {					
			CDSTDatePairCollection DSTDatePairs = m_dstSchedule.DSTDatePairs;
			DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
			DateTime DSTToDate = DateTime.MinValue;
			DateTime DSTFromDate = DateTime.MinValue;
			int iYear = 0;
			int iToDay = 0;
			int iFromDay = 0;
			int iToMonth = 0;
			int iFromMonth = 0;
			int iCurrentYear = DateTime.Now.Year;			
			int iDSTUpdateStartYear = 0;
			int iYearIndex = 0;			
			bool bNeedsUpdate = false;
			bool bUpdated = false;
			bool bDSTEnd = false;
						
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Upload DST Table" );
			// Upload the dst table
			Result = SCSToDSTResult( m_SCSProtocol.Upload(
				DSTTableAddress, 
				FULC_MAX_DST_RECORDS * FULC_DST_RECORD_LENGTH, 
				out m_byFULCDSTTable ) );

			// The start year for the update is the later of current year and 
			// earliest year from dst file
			if ( iCurrentYear > ENERGY_ACT_OF_2005_START_YEAR )
			{
				iDSTUpdateStartYear = iCurrentYear;
			}
			else
			{
				iDSTUpdateStartYear = ENERGY_ACT_OF_2005_START_YEAR;
			}

			// Process each DST change record
			for ( int iIndex = 0; iIndex < FULC_MAX_DST_RECORDS && 
				!bDSTEnd; iIndex++ )
			{
				iYear = m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH )];
				
				if ( FULC_CALENDAR_END == iYear )
				{
					bDSTEnd = true;
				}

				// Set up year for comparison
				if (iYear > SCS_YEAR_BASE_CUTOFF)
				{
					iYear += SCS_PREVIOUS_YEAR_BASE;
				}
				else
				{
					iYear += SCS_CURRENT_YEAR_BASE;
				}

				if ( ( iDSTUpdateStartYear <= iYear ) && !bDSTEnd )
				{
					iToDay = m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
						(int)FULCDSTRecordIndex.TO_DAY];
					iToMonth = m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
						(int)FULCDSTRecordIndex.TO_MONTH];
					iFromDay = m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
						(int)FULCDSTRecordIndex.FROM_DAY ];
					iFromMonth = m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
						(int)FULCDSTRecordIndex.FROM_MONTH];
					
					// Set up date objects for subsequent comparisons
					DSTToDate = new DateTime( iYear, iToMonth, iToDay );
					DSTFromDate = new DateTime( iYear, iFromMonth, iFromDay );


					try
					{
						// FindYear throws exception if year is not found
						iYearIndex = DSTDatePairs.FindYear( iYear );
						bNeedsUpdate = true;

						// Only change DST dates during the current year 
						// where the old DST date has not occured yet and 
						// the new DST date will occur in the future 
						if ( ( iYear == DateTime.Now.Year && 
							DSTToDate > DateTime.Now && 
							DSTDatePairs[iYearIndex].ToDate > DateTime.Now ) || 
							( iYear != DateTime.Now.Year ) )
						{
							// Check DST advance month
							if( DSTDatePairs[iYearIndex].ToDate.Month != iToMonth )
							{
								iToMonth = DSTDatePairs[iYearIndex].ToDate.Month;
								
								m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
									(int)FULCDSTRecordIndex.TO_MONTH] = (byte)iToMonth;
								
								bUpdated = true;
							}

							// Check DST advance day
							if( DSTDatePairs[iYearIndex].ToDate.Day != iToDay )
							{
								iToDay = DSTDatePairs[iYearIndex].ToDate.Day;
								
								m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
									(int)FULCDSTRecordIndex.TO_DAY] = (byte)iToDay;
								
								bUpdated = true;
							}
						}

						if ( ( iYear == DateTime.Now.Year && 
							DSTFromDate > DateTime.Now && 
							DSTDatePairs[iYearIndex].FromDate > DateTime.Now ) || 
							( iYear != DateTime.Now.Year ) )
						{
							// Check DST retard month
							if( DSTDatePairs[iYearIndex].FromDate.Month != iFromMonth )
							{
								iFromMonth = DSTDatePairs[iYearIndex].FromDate.Month;
								
								m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
									(int)FULCDSTRecordIndex.FROM_MONTH] = 
									(byte)iFromMonth;
								
								bUpdated = true;
							}
							
							// Check DST retard day
							if( DSTDatePairs[iYearIndex].FromDate.Day != iFromDay )
							{
								iFromDay = DSTDatePairs[iYearIndex].FromDate.Day;
								
								m_byFULCDSTTable[( iIndex * FULC_DST_RECORD_LENGTH ) + 
									(int)FULCDSTRecordIndex.FROM_DAY] = (byte)iFromDay;
								
								bUpdated = true;
							}
						}
					}
					catch( ArgumentException )
					{
						// We didn't find the year in the collection 
						// of DSTDatePairs.  This is an error.
						Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
					}
				
				} // End if statement to check DST change record's year

				if ( DSTUpdateResult.SUCCESS == Result && bNeedsUpdate && !bUpdated )
				{
					Result = DSTUpdateResult.SUCCESS_PREVIOUSLY_UPDATED;
				}	
		
			} // End for statement to process DST change records
			
			if ( DSTUpdateResult.SUCCESS == Result && 
				DSTFromDate < DateTime.Now || DSTToDate < DateTime.Now )
			{
				Result = DSTUpdateResult.ERROR_DST_DATES_EXPIRED;
			}

			return Result;
        } // End ReadFULCDSTTable()

		/// <summary>
		/// Writes the DST calendar to the FULCRUM.
		/// </summary>
		/// <returns>A DSTUpdateResult</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 06/13/06 jrf 7.30.00  N/A   Modified
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
		///								
        private DSTUpdateResult PostFULCDSTTable()
        {
			DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
			
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Write DST Table" );
			// Write the dst table to the FULCRUM
			Result = SCSToDSTResult( m_SCSProtocol.Download(
				(int)FULCAddresses.DST_TABLE, 
				FULC_MAX_DST_RECORDS * FULC_DST_RECORD_LENGTH, 
				ref m_byFULCDSTTable ) );
	
			return Result;
        } // End PostFULCDSTTable()
		   
		/// <summary>
		/// This method translates the firmware or software Version read from 
		/// the FULCRUM from ascii to float. 
		/// </summary>
		/// <returns>
		/// a float representing the firmware or software version
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/30/06 jrf 7.30.00  N/A   Revised
		///
		private float TranslateVersion(byte[] abyteVersion, int intLength)
        {
            string strVersion = "";
			int	intDecimalPointPosition = intLength - 2;

			// Add a character at a time to Version string and decimal point in 
			// correct place
			for (int intIndex = 0; intIndex < intLength; intIndex++)
			{
				if( intDecimalPointPosition == intIndex )
				{
					strVersion += ".";
				}
				strVersion += (char)abyteVersion[intIndex];
			}
            strVersion = strVersion.Trim();
            return (float)(float.Parse(strVersion, CultureInfo.InvariantCulture));
        }

		/// <summary>
		/// This method puts the time in a form that can be downloaded to the 
		/// meter.
		/// </summary>
		/// <param name="abytTime">The byte array to store the date/time for 
		/// download to the meter</param>
		/// <param name="newTime">The DateTime object that should be used to 
		/// build the date/time in the byte array</param>
		/// <returns>A SCSProtocolResponse</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		/// 
		override protected void PrepareTime(out byte[] abytTime, DateTime newTime)
		{
			abytTime = new byte[7];
			
			// Prepare the new time for download to meter
			abytTime[6] = (byte)(newTime.Year % 100);
			abytTime[5] = (byte)(newTime.Month);
			abytTime[4] = (byte)(newTime.Day);
			abytTime[3] = (byte)(newTime.Hour);
			abytTime[2] = (byte)(newTime.Minute);
			abytTime[1] = (byte)(newTime.Second);
			abytTime[0] = (byte)(newTime.DayOfWeek + 1);
		}

		/// <summary>
		/// This method can be overriden by SCS Devices which need to stop the 
		/// meter in order to adjust the time.
		/// </summary>
		/// <param name="disableMeter">The boolean to determine if the meter needs
		/// to be disabled or enabled</param>
		/// <returns>A SCSProtocolResponse</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/01/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		/// 07/18/06 mcm 7.30.00  N/A   Removed parameter. Meter clears this flag.
		/// 
		override protected SCSProtocolResponse StopMetering(bool disableMeter)
		{
			SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;
			byte[] abytFlag = new byte[1];
			
			if ( disableMeter )
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Stopping Metering" );
				abytFlag[0] = SCS_FLAG_ON;
			}
			else
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Starting Metering" );
				abytFlag[0] = SCS_FLAG_OFF;				
			}
				
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Stop Meter Flag" );
			objProtocolResponse = m_SCSProtocol.Download(
									(int)FULCAddresses.STOP_METER_FLAG, 
									SCS_FLAG_LENGTH, 
									ref abytFlag);
			
			return objProtocolResponse;
		}

		/// <summary>
		/// This method verifies that the derived device type matches 
		/// the SCS device's type 
		/// </summary>
		/// <returns>
		/// a boolean indicating whether or not the device type is correct
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00  N/A   Created
		/// 
		override protected bool VerifyDeviceType()
		{
			return ( DeviceType == ExpectedDeviceType );
		}

		/// <summary>
		/// This gets the errors from the meter and converts them to an array of strings.
		/// </summary>
		/// <returns>A string array that contains the errors</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00  N/A   Created
		/// 06/29/06 jrf 7.30.00  N/A   Added code to prevent duplicate reporting 
		///								of phase loss error.
		/// 
		protected override void ReadErrors(out string[] astrErrors)
		{
			byte[] abytErrors;
			ArrayList objErrorList = new ArrayList();
			SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read System Errors" );
			// Get the system errors
			objProtocolResponse = m_SCSProtocol.Upload( 
				(int)FULCAddresses.SYSTEM_ERROR, 
				FULC_SYSTEM_ERROR_LENGTH, 
				out abytErrors);

			if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
			{
				// Check for each non-fatal error and add to list as appropriate
				if (0 != (abytErrors[(int)FULCErrorIndex.NON_FATAL] & 
					(byte)FULCErrors.LOW_BATTERY)) 
				{
					objErrorList.Add(m_rmStrings.GetString("LOW_BATTERY"));
				}
				if (0 != (abytErrors[(int)FULCErrorIndex.NON_FATAL] & 
					(byte)FULCErrors.PHASE_LOSS)) 
				{
					objErrorList.Add(m_rmStrings.GetString("PHASE_LOSS"));
				}
				// Check for each diagnostic error and add to list as appropriate
				if (0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.CLOCK)) 
				{
					objErrorList.Add(m_rmStrings.GetString("CLOCK_ERROR"));
				}
				if (0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.MASS_MEMORY)) 
				{
					objErrorList.Add(m_rmStrings.GetString("MASS_MEMORY_ERROR"));
				}
				if (0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.REGISTER_FULL_SCALE)) 
				{
					objErrorList.Add(m_rmStrings.GetString("REGISTER_FULL_SCALE"));
				}
				if (0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.INPUT_OUTPUT)) 
				{
					objErrorList.Add(m_rmStrings.GetString("INPUT_OUTPUT_ERROR"));
				}
				// Check to avoid duplicate reporting of phase loss
				if ((0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.PHASE_LOSS)) && 
					(0 == (abytErrors[(int)FULCErrorIndex.NON_FATAL] & 
					(byte)FULCErrors.PHASE_LOSS))) 
				{
					objErrorList.Add(m_rmStrings.GetString("PHASE_LOSS"));
				}
				if (0 != (abytErrors[(int)FULCErrorIndex.DIAGNOSTIC] & 
					(byte)FULCDiagErrors.TOU)) 
				{
					objErrorList.Add(m_rmStrings.GetString("TOU_ERROR"));
				}
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U, 
					objProtocolResponse, 
					(int)FULCAddresses.SYSTEM_ERROR, 
					m_rmStrings.GetString("SYSTEM_ERRORS"));
				throw objSCSException;
			}

			astrErrors = (string[])objErrorList.ToArray(typeof(string));	
		
		} // End ReadErrors()
				
		/// <summary>Writes TOU events to the SCS device.</summary>
		/// <param name="TOUEventList">The event list which holds the tou events
		/// </param>
		/// <returns>A DSTUpdateResult</returns>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/12/06 mcm 7.30.00  N/A   Created
		///
		protected override DSTUpdateResult PostTOUEvents(
											ref SCSTOUEventCollection TOUEventList)
		{
			DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
			byte[] Data = TOUEventList.ByteList;

			
			Result = SCSToDSTResult(m_SCSProtocol.Download( 
				TOU_HEADER_ADDRESS + SIZE_OF_TOU_HEADER, 
				Data.Length, ref Data ));

			// Write the TOU calendar
			if(( DSTUpdateResult.SUCCESS == Result ) && 
			   ( null != m_dstSchedule ))
			{
				// Download the updated DST dates to the meter
				Result = PostFULCDSTTable();				
			}
			
			return Result;

		} // PostTOUEvents

		/// <summary>
		/// Turn off the TOU run flag, writes all of the configuration data to
		/// the meter, and turns the flag back on. 
		/// </summary>
		/// <param name="TOUInfo">TOU Info to write</param>
		/// <param name="TOUEventList">TOU Event list calendar to write</param>
		/// <param name="TOUSeasons">Seasons to write</param>
		/// <returns>Success, IOTimeout, Security Error, or Protocol Error</returns>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/14/06 mcm 7.30.00 N/A	Created
		/// 
		protected override TOUReconfigResult WriteTOU( SCSTOUInfo TOUInfo, 
			SCSTOUEventCollection TOUEventList, SCSSeasonCollection TOUSeasons )
		{
			TOUReconfigResult	Result = TOUReconfigResult.SUCCESS;
			SCSProtocolResponse ProtocolResponse;
			byte[]				Flag = new byte[1];

			
			// Update the Fulcrum specific DST table
			if( null != m_dstSchedule )
			{
				Result = (TOUReconfigResult)ReadFULCDSTTable();
				if( TOUReconfigResult.SUCCESS_PREVIOUSLY_UPDATED == Result )
				{	
					// Don't stop reconfigure because the DST dates are correct
					Result = TOUReconfigResult.SUCCESS;
				}
			}

			// We'll be writing this value several times
			Flag[0] = SCS_FLAG_ON;

			ProtocolResponse = StopMetering(true);

			// Stop metering
			if( SCSProtocolResponse.SCS_CAN == ProtocolResponse )
			{
				// If the meter rejected our request, we probably don't
				// have sufficient security clearance.
				Result = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
			}
			else if( SCSProtocolResponse.SCS_ACK == ProtocolResponse )
			{
				// Set the TOU Reconfigure Flag
				ProtocolResponse = m_SCSProtocol.Download( 
					TOUReconfigureFlagAddress, SCS_FLAG_LENGTH, ref Flag);

				Thread.Sleep(2000);
			}

			if( TOUReconfigResult.SUCCESS == Result )
			{		
				try
				{			
					// Write the Info block first, if it succeeds, write
					// the calendar.
					if( SCSProtocolResponse.SCS_ACK == 
						WriteTOUInfo( TOUInfo ))
					{
						m_Logger.WriteLine( Logger.LoggingLevel.Detailed, 
							"Writing TOU Event Calendar block" );

						Result = (TOUReconfigResult)
							PostTOUEvents( ref TOUEventList );
					}
					else
					{
						Result = TOUReconfigResult.IO_TIMEOUT;
					}
			
					if( TOUReconfigResult.SUCCESS == Result )
					{
						byte[] SeasonData = TOUSeasons.Data;

						m_Logger.WriteLine( Logger.LoggingLevel.Detailed, 
							"Writing TOU Seasons block" );

						if( SCSProtocolResponse.SCS_ACK != 
							m_SCSProtocol.Download( 
							TOUInfo.StartOfYearlySchedule + TOUEventList.Size, 
							SeasonData.Length, ref SeasonData ))
						{
							Result = TOUReconfigResult.IO_TIMEOUT;
						}
					}

					if ( TOUReconfigResult.SUCCESS == Result )
					{
						Result = (TOUReconfigResult)
							SCSToDSTResult( SetTOUReconfigureFlag() );
					}

					// If anything went wrong since we stopped the TOU flag,
					// tell the user to try again.
					if ( TOUReconfigResult.SUCCESS != Result )
					{
						Result = TOUReconfigResult.ERROR_RETRY;
					}
					
					// Regardless if it worked, try to restart the meter.
					m_SCSProtocol.Download( ClockReconfigureFlagAddress, 
						SCS_FLAG_LENGTH, ref Flag);

					Thread.Sleep(2000);

					// SET the TOU Reconfigure Flag (it clears itself)
					ProtocolResponse = m_SCSProtocol.Download( 
						MeterReconfigureFlagAddress, SCS_FLAG_LENGTH, ref Flag);

					Thread.Sleep(2000);

					// Restart metering
					StopMetering(false);
				}
				catch(Exception e)
				{
					// Try to set the flag since it had to be turned off
					// if we have gotten to this point.
					SetTOURunFlag(true);
					
					// Regardless if it worked, try to restart the meter.
					m_SCSProtocol.Download( ClockReconfigureFlagAddress, 
						SCS_FLAG_LENGTH, ref Flag);

					// SET the TOU Reconfigure Flag (it clears itself)
					ProtocolResponse = m_SCSProtocol.Download( 
						MeterReconfigureFlagAddress, SCS_FLAG_LENGTH, ref Flag);

					// Restart metering
					StopMetering(false);

					throw (e);
				}
			}

			return Result;

		} // WriteTOU

		/// <summary>Updates the TOU Info block to reflect the size of the new
		/// configuration.</summary>
		/// <param name="TOUSchedule">TOU Server with TOU file open.</param>
		/// <param name="TOUInfo">The TOU Info block to update</param>
		/// <param name="TOUEventList">The new TOU Event configuration</param>
		/// <param name="TOUSeasons">The new TOU Season Cofiguration</param>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/07/06 mcm 7.30.00 N/A	Created
		/// 
		protected override void ReconfigureTOUInfo( CTOUSchedule TOUSchedule, 
			SCSTOUInfo TOUInfo, SCSTOUEventCollection TOUEventList, 
			SCSSeasonCollection TOUSeasons )
		{
			TOUInfo.ExpirationDate = TOUEventList.ExpirationDate;
			TOUInfo.ScheduleID = (ushort)TOUSchedule.TOUID;

			// Unlike all other SCS devices, the FULCRUM defines these values
			// as offsets from the start of the TOU Header. Isn't that special?
			TOUInfo.StartOfYearlySchedule = (ushort)( TOU_HEADER_ADDRESS + 
				SIZE_OF_TOU_HEADER );
			TOUInfo.StartOfSeason0 = (ushort)( TOUInfo.StartOfYearlySchedule + 
				TOUEventList.Size );
			TOUInfo.StartOfSeason1 = (ushort)( TOUInfo.StartOfSeason0 + 
				TOUSeasons.Season0Size );
			TOUInfo.StartOfSeason2 = (ushort)( TOUInfo.StartOfSeason1 + 
				TOUSeasons.Season1Size );
			TOUInfo.StartOfSeason3 = (ushort)( TOUInfo.StartOfSeason2 + 
				TOUSeasons.Season2Size );
			TOUInfo.StartOfSeason4 = (ushort)( TOUInfo.StartOfSeason3 + 
				TOUSeasons.Season3Size );
			TOUInfo.StartOfSeason5 = (ushort)( TOUInfo.StartOfSeason4 + 
				TOUSeasons.Season4Size );
			TOUInfo.StartOfSeason6 = (ushort)( TOUInfo.StartOfSeason5 + 
				TOUSeasons.Season5Size );
			TOUInfo.StartOfSeason7 = (ushort)( TOUInfo.StartOfSeason6 + 
				TOUSeasons.Season6Size );

			TOUInfo.TypicalMonday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.MONDAY);
			TOUInfo.TypicalTuesday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.TUESDAY);
			TOUInfo.TypicalWednesday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.WEDNESDAY);
			TOUInfo.TypicalThursday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.THURSDAY);
			TOUInfo.TypicalFriday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.FRIDAY);
			TOUInfo.TypicalSaturday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.SATURDAY);
			TOUInfo.TypicalSunday = GetDaytypeIndex( TOUSchedule, 
				TOU.eTypicalDay.SUNDAY);

		} // ReconfigureTOUInfo

		/// <summary>Reads the TOU base address and sets it into the TOUInfo
		/// start of yearly schedule and TOU Base address values. This method 
		/// does not read any of the other data in the TOU Info class. The 
		/// FULCRUM doesn't support all of the items, and the ones it does are 
		/// spread out. For now this method is only supporting what it 
		/// absolutely has to.</summary>
		/// <param name="TOUInfo">The TOUInfo object to instantiate and 
		/// initialize with the TOU base address.</param>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/11/06 mcm 7.30.00 N/A	Created
		/// 
		protected override SCSProtocolResponse ReadTOUInfo( out SCSTOUInfo TOUInfo )
		{
			SCSProtocolResponse Result;
			byte[] Data;

			
			// Satisfy compiler by assigning out parameter
			TOUInfo = null;

			Result = m_SCSProtocol.Upload( TOU_HEADER_ADDRESS, 
				SIZE_OF_TOU_HEADER, out Data );

			if( SCSProtocolResponse.SCS_ACK == Result )
			{
				TOUInfo = new FulcrumTOUInfo(Data);
			
				Result = m_SCSProtocol.Upload( (int)FULCAddresses.TOU_SCHED_ID, 
					SIZE_OF_TOU_ID_AND_EXPIRATION_DATE, out Data );
			}

			if( SCSProtocolResponse.SCS_ACK == Result )
			{
				int Year;

				if( 80 < Data[4] )
				{
					Year = 1900 + Data[5];
				}
				else
				{
					Year = 2000 + Data[5];
				}

				DateTime ExpirationDate = new DateTime(Year, Data[4], Data[3]);
				TOUInfo.ExpirationDate = ExpirationDate;
				TOUInfo.ScheduleID = Data[0];
			}

			return Result;

		} // ReadTOUInfo
		
		/// <summary>Writes the block of TOU Info Items</summary>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/11/06 mcm 7.30.00 N/A	Created
		/// 
		protected override SCSProtocolResponse WriteTOUInfo( SCSTOUInfo TOUInfo )
		{
			SCSProtocolResponse Result;
			byte[] TOUID = new byte[1];
			byte[] TOUHeader = new byte[SIZE_OF_TOU_HEADER];
			byte[] ExpirationYear = new byte[1];


			m_Logger.WriteLine( Logger.LoggingLevel.Detailed, 
				"Writing TOU Info block" );

			Array.Copy( TOUInfo.Data, TOUInfo.TypicalWeekOffset,
				TOUHeader, 0, 2 );
			
			// Calendar Start Offset should always be the offset to the first 
			// byte after this header.
			TOUHeader[2] = 0;
			TOUHeader[3] = SIZE_OF_TOU_HEADER;

			Array.Copy( TOUInfo.Data, TOUInfo.Season0Offset, 
				TOUHeader, 4, 16 );
		

			TOUID[0] = (byte)TOUInfo.ScheduleID;
			Result = m_SCSProtocol.Download( (int)FULCAddresses.TOU_SCHED_ID, 
				1, ref TOUID );

			if( SCSProtocolResponse.SCS_ACK == Result )
			{
				Result =  m_SCSProtocol.Download( TOU_HEADER_ADDRESS, 
					SIZE_OF_TOU_HEADER, ref TOUHeader );
			}

			if( SCSProtocolResponse.SCS_ACK == Result )
			{
				// Once again the FULCRUM is special. It's the only SCS device
				// that stores it's date as 5 bytes. The TOUInfo class supports
				// it as 4 bytes, so just write the year. It's the only part 
				// that changes.
				ExpirationYear[0] = (byte)(TOUInfo.ExpirationDate.Year % 100);
				Result =  m_SCSProtocol.Download( (int)FULCAddresses.EXPIRATION_YEAR_ADDRESS, 
					1, ref ExpirationYear );
			}


			return Result;

		} // WriteTOUInfo

        /// <summary>The FULCRUM device is so special that it requires a force
        /// copy byte to be sent after some writes (passwords).  The other SCS
        /// device don't have this requirement.  This base implementation will
        /// cover all devices except the FULCRUM which will override it.</summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00 N/A	Created
        /// 
        protected override SCSProtocolResponse ForceCopyData()
        {
            byte[] byFlag = new byte[1];

			byFlag[0] = SCS_FLAG_ON;

			// Clear the Clock Option Run Flag
            return m_SCSProtocol.Download((int)FULCAddresses.COPY_DATA_FLAG, 
				                           SCS_FLAG_LENGTH, ref byFlag );

        } // SCSProtocolResponse

		/// <summary>
		/// This method reads the transformer ratio from the SCS device. 
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the transformer ratio cannot be retreived from the meter.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		protected override void ReadTransformerRatio()
		{			
			NotImplementedException exception = new NotImplementedException("Transformer ration is not yet implemented");
			throw exception;
		}

    } // End class FULCRUM

			
	/// <summary>
		/// This class manages the block of TOU info for SCS devices. It reads it,
		/// writes it, dumps it, and provides access to each item</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ -------------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A	Created
	/// 
	internal class FulcrumTOUInfo : SCSTOUInfo
	{
		#region Public Methods

		/// <summary>Constructs an empty block of SCSTOUInfo data</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A	Created
		/// 
		public FulcrumTOUInfo()
		{
			m_Logger = Logger.TheInstance;
			m_Data = new byte[SIZE_OF_TOU_INFO];
			m_Data.Initialize();
		}

		/// <summary>SCSTOUInfo constructor</summary>
		/// <param name="Data">TOU Info Data as read from the meter. This 
		/// should be exactly 33 bytes or I will throw up on you!</param>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A	Created
		/// 
		public FulcrumTOUInfo( byte[] Data )
		{
			if( SIZE_OF_TOU_INFO != Data.Length )
			{
				throw( new Exception( "Can't instantiate SCSTOUInfo with " +
					Data.Length + " bytes of data" ));
			}

			m_Logger = Logger.TheInstance;
			m_Data = Data;
		}
		/// <summary>Dumps the SCS TOU info for debugging</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A	Created
		/// 
		public override SCSProtocolResponse Dump()
		{
			SCSProtocolResponse Result = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Starting Dump of FulcrumTOUInfo");

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StartOfYearlySchedule = 0x" + StartOfYearlySchedule.ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ExpirationDate = " + m_ExpirationDate.ToString("d", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "ScheduleID = 0x" + m_ScheduleID.ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason0 = 0x" +
                (StartOfSeason0 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason1 = 0x" +
                (StartOfSeason1 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason2 = 0x" +
                (StartOfSeason2 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason3 = 0x" +
                (StartOfSeason3 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason4 = 0x" +
                (StartOfSeason4 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason5 = 0x" +
                (StartOfSeason5 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason6 = 0x" +
                (StartOfSeason6 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Offset to StartOfSeason7 = 0x" +
                (StartOfSeason7 - TOU_BASE_ADDRESS).ToString("X4", CultureInfo.InvariantCulture));
			
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Monday daytype = " + TypicalMonday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Tuesday daytype = " + TypicalTuesday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Wednesday daytype = " + TypicalWednesday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Thursday daytype = " + TypicalThursday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Friday daytype = " + TypicalFriday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Saturday daytype = " + TypicalSaturday );
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"Typical Sunday daytype = " + TypicalSunday );

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
				"End Dump of FulcrumTOUInfo");

			return Result;

		} // Dump
	
		#endregion Public Methods
	
		#region Properties
	
		/// <summary>Provides access to the address of the start of the yearly
		/// schedule</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfYearlySchedule
		{
			get 
			{ 
				return (ushort)(m_Data[YearlySchedOffset] * 0x100 +
					m_Data[YearlySchedOffset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[YearlySchedOffset] = (byte)( value >> 8 );
				m_Data[YearlySchedOffset + 1] = (byte)value;
			}
		}
	

		/// <summary>NOT SUPPORTED FOR FULCRUM DEVICES!</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/20/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfDailySchedule
		{
			get{ return 0; }
			set{}
		}
	

		/// <summary>Provides access to the address of the expiration date of 
		/// the schedule</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override DateTime ExpirationDate
		{
			get 
			{ 	
				return m_ExpirationDate;
			}
			set 
			{ 
				m_ExpirationDate = value;
			}
		}
	
		///<summary>Provides access to the TOU Schedule ID</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///override 
		public override ushort ScheduleID
		{
			get 
			{ 
				return m_ScheduleID;
			}
			set 
			{ 
				m_ScheduleID = value;
			}
		}

        /// <summary>Retrurns true if Season 0 has more than just an end of
        /// calendar flag in it</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/24/06 mcm 7.30.00  N/A   Created
        ///
        public override bool TOUIsConfigured
        {
            get
            {
                if (StartOfSeason0 > StartOfYearlySchedule)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>Provides access to the offset to Season 0 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason0
		{
			get 
			{ 
				return (ushort)(m_Data[Season0Offset] * 0x100 +
					m_Data[Season0Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season0Offset] = (byte)( value >> 8 );
				m_Data[Season0Offset + 1] = (byte)value;
			}
		}	


		/// <summary>Provides access to the offset to Season 0 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason1
		{
			get 
			{ 
				return (ushort)(m_Data[Season1Offset] * 0x100 +
					m_Data[Season1Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season1Offset] = (byte)( value >> 8 );
				m_Data[Season1Offset + 1] = (byte)value;
			}
		}	

		/// <summary>Provides access to the offset to Season 2 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason2
		{
			get 
			{ 
				return (ushort)(m_Data[Season2Offset] * 0x100 + 
					m_Data[Season2Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season2Offset] = (byte)( value >> 8 );
				m_Data[Season2Offset + 1] = (byte)value;
			}
		}	


		/// <summary>Provides access to the offset to Season 3 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason3
		{
			get 
			{ 
				return (ushort)(m_Data[Season3Offset] * 0x100 + 
					m_Data[Season3Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season3Offset] = (byte)( value >> 8 );
				m_Data[Season3Offset + 1] = (byte)value;
			}
		}	


		/// <summary>Provides access to the offset to Season 4 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason4
		{
			get 
			{ 
				return (ushort)(m_Data[Season4Offset] * 0x100 + 
					m_Data[Season4Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season4Offset] = (byte)( value >> 8 );
				m_Data[Season4Offset + 1] = (byte)value;
			}
		}	


		/// <summary>Provides access to the offset to Season 5 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason5
		{
			get 
			{ 
				return (ushort)(m_Data[Season5Offset] * 0x100 +
					m_Data[Season5Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season5Offset] = (byte)( value >> 8 );
				m_Data[Season5Offset + 1] = (byte)value;
			}
		}	


		/// <summary>Provides access to the offset to Season 6 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason6
		{
			get 
			{ 
				return (ushort)(m_Data[Season6Offset] * 0x100 + 
					m_Data[Season6Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season6Offset] = (byte)( value >> 8 );
				m_Data[Season6Offset + 1] = (byte)value;
			}
		}

        // mcm 03/27/07 - The SizeOfSeasonN properties aren't dependable for 
        // Fulcrums.  PC-PRO+ 3.5 sets the season offsets for unused seasons
        // to 0, so we can't calculate the size of the last used season.  Our
        // .NET reconfigure code does not configure Fulcrums this way.

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason0
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason3
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason4
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason5
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>This property is not implemented for Fulcrums</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 03/27/07 mcm 7.30.00 2742   Last season size can't be determined!
        ///
        public override int SizeOfSeason6
        {
            get
            {
                throw new NotImplementedException();
            }
        }

		/// <summary>Provides access to the offset to Season 7 from the TOU 
		/// base</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/06/06 mcm 7.30.00  N/A   Created
		///
		public override ushort StartOfSeason7
		{
			get 
			{ 
				return (ushort)(m_Data[Season7Offset] * 0x100 + 
					m_Data[Season7Offset + 1] + TOU_BASE_ADDRESS);
			}
			set 
			{ 
				value = (ushort)(value - TOU_BASE_ADDRESS);
				m_Data[Season7Offset] = (byte)( value >> 8 );
				m_Data[Season7Offset + 1] = (byte)value;
			}
		}	

		/// <summary>Returns the Fulcrum's Typical Week data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int TypicalWeekOffset
		{
			get
			{
				return (int)DataOffset.TYPICAL_WEEK;
			}
		}

		/// <summary>Returns the Fulcrum's START_OF_YEARLY_SCHED data offset
		/// </summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int YearlySchedOffset
		{
			get
			{
				return (int)DataOffset.START_OF_YEARLY_SCHED;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_0 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season0Offset
		{
			get
			{
				return (int)DataOffset.SEASON_0;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_1 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season1Offset
		{
			get
			{
				return (int)DataOffset.SEASON_1;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_2 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season2Offset
		{
			get
			{
				return (int)DataOffset.SEASON_2;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_3 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season3Offset
		{
			get
			{
				return (int)DataOffset.SEASON_3;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_4 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season4Offset
		{
			get
			{
				return (int)DataOffset.SEASON_4;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_5 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season5Offset
		{
			get
			{
				return (int)DataOffset.SEASON_5;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_6 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season6Offset
		{
			get
			{
				return (int)DataOffset.SEASON_6;
			}
		}

		/// <summary>Returns the Fulcrum's SEASON_7 data offset</summary>
		/// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/21/06 mcm 7.30.00  N/A   Created
		///
		public override int Season7Offset
		{
			get
			{
				return (int)DataOffset.SEASON_7;
			}
		}		
		
		#endregion Properties
	
		#region Definitions

		/// <summary>Size of this class' data</summary>
		new public const ushort SIZE_OF_TOU_INFO = 20;

		private const ushort TOU_BASE_ADDRESS = 0x2AEC;

		/// <summary>Offsets of the data to be used with the Data[]</summary>
		private enum DataOffset : int
		{
			/// <summary>Offset of the 2 byte typical week value</summary>
			TYPICAL_WEEK			= 0,
			/// <summary>Offset of the offset from the TOU base</summary>
			START_OF_YEARLY_SCHED	= 2,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_0				= 4,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_1				= 6,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_2				= 8,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_3				= 10,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_4				= 12,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_5				= 14,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_6				= 16,
			/// <summary>Offset of the offset from the TOU base</summary>
			SEASON_7				= 18,
		}


		#endregion Definitions


		private DateTime m_ExpirationDate;
		private ushort	 m_ScheduleID;

	} // FulcrumTOUInfo

} // End namespace Itron.Metering.Device 
