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

//#define TOU_RECONFIG_TEST


using System;
using System.Threading;
using System.Collections;
using System.Resources;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DST;
using Itron.Metering.TOU;
using Itron.Metering.TIM;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{	
	/// <summary>
	/// The base class for all SCS devices.
	/// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ ---------------------------------------
	// 05/22/06 mrj 7.30.00  N/A	Created
	// 06/13/06 jrf 7.30.00  N/A	Development
	// 
	public abstract partial class SCSDevice : ItronDevice
	{
        #region Constants

        /// <summary>
        /// Max size of Fulcrum's DST table. Limits Calendar configuration
        /// if used.
        /// </summary>
        public const int FULC_MAX_DST_RECORDS = 25;

        private const int SCS_DST_MASK = 0x20;
        private const int SCS_CANADIAN_MASK = 0x10;
        private const int SCS_MODEL_TYPE_MASK = 0x07;
		private const int SCS_LINE_FREQUENCY_MASK = 0x01;
        private const int SCS_PROGRAM_ID_LENGTH = 2;
        private const int SCS_UNIT_ID_LENGTH = 8;
        private const int SCS_TOU_EXPIRATION_LENGTH = 4;
        private const int SCS_USERDATAFIELD_LENGTH = 9;
        private const int SCS_CLPU_LENGTH = 2;
        private const int SCS_DEMANDCONFIGURATIONBLOCK_LENGTH = 3;
        private const int SCS_USERDATABLOCK_LENGTH = SCS_USERDATAFIELD_LENGTH * 3;
        private const int SCS_TOU_EVENT_SIZE = 2;
        private const int SCS_CHANNEL_COUNT_SIZE = 1;
        private const int SCS_DISPLAYITEM_LENGTH = 4;
        private const int MINUTES_PER_HOUR = 60;
        private const byte SCS_MAX_COMM_TIMEOUT = 0xFE; // 254 sec.
        private const byte SCS_SEASON_CHG_MASK = 0x10;
        private const byte SCS_IOENABLED_MASK = 0x08;
        private const byte HOLIDAY_TYPE_INDEX = 3;


        /// <summary>
        /// The length of the real time stored in the SCS device.
        /// </summary>
        protected const int SCS_REAL_TIME_LENGTH = 7;
        /// <summary>
        /// The length of a flag value in the SCS device.
        /// </summary>
        protected const int SCS_FLAG_LENGTH = 1;
        /// <summary>
        /// The length of MM Interval data stored in the SCS device.
        /// </summary>
        protected const int SCS_INTERVAL_LENGTH_SIZE = 1;
        /// <summary>
        /// The length of the firmware data stored in the SCS device.
        /// </summary>
        protected const int SCS_FW_VERSION_LENGTH = 2;
        /// <summary>
        /// The length of the software data stored in the SCS device.
        /// </summary>
        protected const int SCS_SW_VERSION_LENGTH = 2;
        /// <summary>
        /// The length of the TOU calendar address in the SCS device.
        /// </summary>
        protected const int SCS_TOU_CALENDAR_LENGTH = 2;
        /// <summary>
        /// The length of the operating setup data stored in the SCS device.
        /// </summary>
        protected const int SCS_OPERATING_SETUP_LENGTH = 2;
        /// <summary>
        /// The length of the model type stored in the SCS device.
        /// </summary>
        protected const int SCS_MODEL_TYPE_LENGTH = 2;
        /// <summary>
        /// The length of a typical counter (2 bytes) in an SCS device.
        /// </summary>
        protected const int SCS_EVENT_COUNTER_LENGTH = 2;
        /// <summary>
        /// The length of the number of minutes on battery.  Note that this
        /// can be a very large number
        /// </summary>
        protected const int SCS_MINUTES_ON_BATTERY_LENGTH = 3;
        /// <summary>
        /// The length of a M-D-H-M timestamp in an SCS device.
        /// </summary>
        protected const int SCS_SHORT_DATE_TIME_LENGTH = 4;
        /// <summary>
        /// The maximum events that should be read out of the SCS device
        /// at one time.
        /// </summary>
        protected const int SCS_DEFAULT_MAX_READ_EVENTS = 64;
        /// <summary>
        /// The maximum number of bytes that should be written to the SCS device
        /// at one time.
        /// </summary>
        protected const int SCS_DEFAULT_MAX_WRITE_SIZE = 16;
        /// <summary>
        /// The base to add to recent years stored in the SCS device in order to 
        /// compute the correct year.
        /// </summary>
        protected const int SCS_CURRENT_YEAR_BASE = 2000;
        /// <summary>
        /// The previous base to add to older years stored in the SCS device in 
        /// order to compute the correct year.
        /// </summary>
        protected const int SCS_PREVIOUS_YEAR_BASE = 1900;
        /// <summary>
        /// The previous base to add to older years stored in the SCS device in 
        /// order to compute the correct year.
        /// </summary>
        protected const int SCS_YEAR_BASE_CUTOFF = 79;
        /// <summary>
        ///  The year that the Energy Act of 2005 begins.
        /// </summary>
        protected const int ENERGY_ACT_OF_2005_START_YEAR = 2007;
        /// <summary>
        /// The value used to enable a flag in a SCS device.
        /// </summary>
        protected const byte SCS_FLAG_ON = 0xFF;
        /// <summary>
        /// The value used to disable a flag in a SCS device.
        /// </summary>
        protected const byte SCS_FLAG_OFF = 0x00;
        /// <summary>
        /// This value is used by address accessor properties that don't 
        /// have the address.
        /// </summary>
        protected const int UNSUPPORTED_ADDRESS = -1;
        /// <summary>
        /// All SCS devices have 8 byte passwords.
        /// </summary>
        protected const int SIZE_OF_SCS_PASSWORD = 8;
		/// <summary>
		/// The length of the transformer ratio data stored in the SCS device.
		/// </summary>
		protected const int SCS_TRANSFORMER_RATIO_LENGTH = 2;
		/// <summary>
		/// The length of the Load Research ID in the SCS device.
		/// </summary>
        protected const int SCS_LOAD_RESEARCH_ID_LENGTH = 14;

        #endregion Constants
        
		#region Definitions

		/// <summary>
		/// SCSModelTypes enumeration encapsulates the SCS model types.
		/// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00 N/A	Created
		/// </remarks>
		/// 
		protected enum SCSModelTypes
		{
			/// <summary>
			/// Unknown Model
			/// </summary>
			Unknown	= 0,
			/// <summary>
			/// Demand Only Model
			/// </summary>
			DemandOnly = 1,
			/// <summary>
			/// Demand/TOU Model
			/// </summary>
			DemandTOUModel = 2,
			/// <summary>
			/// Demand/TOU/Load Profile Model
			/// </summary>
			DemandTOULPModel = 4
		};
                   
		#endregion Definitions

		#region Public Methods

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="SerialPort">The communication object that supports
		/// communication over the physical port.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSDevice SCSDevice = new SCSDevice(Comm);
		/// </code>
		/// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00  N/A   Created
		/// 05/23/06 jrf 7.30.00  N/A	Modified
		/// 06/16/06 jrf 7.30.00  N/A	Rewrote 
		/// </remarks>
		/// 
		public SCSDevice( ICommunications SerialPort )
		{
			m_SCSProtocol = new SCSProtocol(SerialPort);
			InitializeInstanceVariables();

            m_strCurrentSecurityCode = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Protocol">The SCS protocol object that supports
		/// communication with SCS devices.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// SCSDevice SCSDevice = new SCSDevice(scsProtocol);
		/// </code>
		/// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 jrf 7.30.00  N/A   Created
		/// 06/16/06 jrf 7.30.00  N/A	Rewrote.
		/// </remarks>
		/// 
		public SCSDevice( SCSProtocol Protocol ) 
		{
			m_SCSProtocol = Protocol;
			InitializeInstanceVariables();

            m_strCurrentSecurityCode = null;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00 N/A	Created
		/// </remarks>
		/// 
		~SCSDevice()
		{
			m_rmStrings.ReleaseAllResources();
			m_rmStrings = null;
		}

		/// <summary>
		/// This method logs on to an SCS device.  If the device has already
        /// been identified, it will store the device information.  If not 
        /// aleady identified, this method will wakes up the SCS device and
		/// identify it.  Note that when changing devices using the same 
        /// connection (daisy chaining, etc.), that the new device must be
        /// re-identified.
		/// </summary>
		/// <returns>A ItronDeviceResult representing the Result of the logon.
		/// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a NAK is received from a SCS protocol request.
		/// </exception>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSDevice SCSDevice = new SCSDevice(Comm);
		/// SCSDevice.Logon();		
		/// </code>
		/// </example>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00  N/A    Created
		/// 05/23/06 jrf 7.30.00  N/A	 Rewrote
		/// 06/19/06 jrf 7.30.00  N/A    Changed Result to initialize to error 
		///                              and modified cases accordingly
		/// 06/29/06 jrf 7.30.00  N/A    Added check to not set unit id for 
		///                              FULCRUM
		/// 07/24/06 jrf 7.30.35 SCR 17 Remove trailing nulls from unit ID.
        /// 09/09/06 mcm 7.35.00  N/A   Skip identify if already identified
		/// </remarks>
		/// 
		public override ItronDeviceResult Logon()
		{
			ItronDeviceResult	Result = ItronDeviceResult.ERROR;
            string strNullChar = "\0";

            // If the device has already been identified, skip Wakeup and
            // Identify.
            if (m_SCSProtocol.Identified)
            {
                m_deviceType.Value = m_SCSProtocol.DeviceType;
                m_memoryStart.Value = m_SCSProtocol.MeterStartAddress;
                m_memoryEnd.Value = m_SCSProtocol.MeterStopAddress;

                // FULCRUM's Identify reports unit id as all 0's.  Read it 
                // later.
                if (MeterType != m_rmStrings.GetString("FULC_METER_NAME"))
                {
                    m_unitID.Value = m_SCSProtocol.DeviceID;
                    // jrf - SCR 17
                    // Remove trailing null characters from string
                    m_unitID.Value = m_unitID.Value.Trim(strNullChar.ToCharArray());
                }

                Result = ItronDeviceResult.SUCCESS;
            }
            else
            {
                string strID = "";
                string strType = "";
                int iMemStart = 0;
                int iMemEnd = 0;
                SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

                m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Logon");

                // Wake up the meter
                ProtocolResponse =
                    m_SCSProtocol.WakeUpDevice();

                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                {
                    // Get Identification info from meter
                    ProtocolResponse = m_SCSProtocol.Identify(
                        out strID,
                        out strType,
                        out iMemStart,
                        out iMemEnd);
                }

                // Determine the SCS Response
                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                {
                    // FULCRUM's Identify reports unit id as all 0's.  Read it 
                    // later.
                    if (MeterType != m_rmStrings.GetString("FULC_METER_NAME"))
                    {
                        m_unitID.Value = strID;
                        // jrf - SCR 17
                        // Remove trailing null characters from string
                        m_unitID.Value = m_unitID.Value.Trim(strNullChar.ToCharArray());
                    }
                    m_deviceType.Value = strType;
                    m_memoryStart.Value = iMemStart;
                    m_memoryEnd.Value = iMemEnd;

                    // Check to make sure derived class matches what the 
                    // meter is telling us
                    if (VerifyDeviceType())
                    {
                        Result = ItronDeviceResult.SUCCESS;
                    }
                }
                else if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
                {
                    Result = ItronDeviceResult.SECURITY_ERROR;
                }
                else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
                {
                    SCSException scsException = new SCSException(
                        SCSCommands.SCS_S,
                        ProtocolResponse,
                        0,
                        m_rmStrings.GetString("LOGON_REQUEST"));
                    throw scsException;
                }
            }

			return Result;
		} // End Logon()

		/// <summary>
		/// This method sends a list of passwords and obtains security clearance from the 
		/// SCS device.
		/// </summary>
        /// <param name="Passwords">A list of passwords to be issued to the 
        /// meter. An empty string should be supplied if a null password is 
        /// to be attempted.</param>
		/// <returns>An ItronDeviceResult representing the result of trying 
		/// security.</returns>
		/// <remarks>If we have to resend security then the WakeUpDevice and 
		/// Identify must be reissued prior to security.</remarks>
		/// <exception cref="Exception">
		/// None. Catches exceptions  on the security request and unusual
        /// responses and interprets them as security errors. Note that the 
        /// protocol might still throw an exception (most likely timeout) on
        /// the WakeUp request.
		/// </exception>        
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/22/06 mrj 7.30.00  N/A   Created
		//  05/23/06 jrf 7.30.00  N/A	Revised
		//  06/19/06 jrf 7.30.00  N/A   Changed Result to initialize to error 
		//                              and modified cases accordingly.  Added 
		//                              constant for comm timeout.
        //  08/21/06 mrj 7.35.00 N/A    Changed to take a list of passwords.
        //  08/29/06 mrj 7.35.00        Store off the current security code
        //  09/13/06 mrj 7.35.00        Do not log the security codes
        //  09/14/06 mcm 7.35.00  N/A   Interpret all odd responses and protocol
        //                              exceptions as security errors
        //  10/12/06 mrj 7.35.04 48     Don't need to issue security if it has
        //                              already been set.
        //   		 
        public override ItronDeviceResult Security(System.Collections.Generic.List<string> Passwords)
		{
			ItronDeviceResult		Result = ItronDeviceResult.ERROR;           
            byte[] byCommTimeout = new byte[1];
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            SCSProtocolResponse WakeUpResponse = SCSProtocolResponse.NoResponse;
			
			m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Security" );
			
					
			// For setting comm. timeout in meter to 254 secs.	
			byCommTimeout[0] = SCS_MAX_COMM_TIMEOUT;

            
            if (null == m_strCurrentSecurityCode)
            {            
                for (int iIndex = 0; iIndex < Passwords.Count && SCSProtocolResponse.SCS_ACK != ProtocolResponse; iIndex++)
                {
                    //Do not show the security code in the log file
                    Logger.LoggingState CurrentState = m_Logger.LoggerState;
                    m_Logger.LoggerState = Logger.LoggingState.PROTOCOL_SENDS_SUSPENDED;

                    //Issue the password
                    string strPassword = Passwords[iIndex];

                    try
                    {
                        ProtocolResponse = m_SCSProtocol.Security(strPassword);
                    }
                    catch
                    {
                        // Some meters don't answer or are slow to answer invalid
                        // security requests.  Interpret all protocol exceptions 
                        // as security errors.
                        ProtocolResponse = SCSProtocolResponse.SCS_CAN;
                    }

                    //Resume logging
                    m_Logger.LoggerState = CurrentState;


                    if (iIndex < Passwords.Count &&
                        SCSProtocolResponse.SCS_ACK != ProtocolResponse)
                    {
                        //Security failed and we are going to try again so we need
                        //to re-issue WakeUpDevice and Identify                 
                        WakeUpResponse = m_SCSProtocol.WakeUpDevice();

                        string sID;
                        string sType = "";
                        int nMemStart;
                        int nMemEnd;

                        if (SCSProtocolResponse.SCS_ACK == WakeUpResponse)
                        {
                            WakeUpResponse = m_SCSProtocol.Identify(out sID,
                                out sType, out nMemStart, out nMemEnd);
                        }

                        if (SCSProtocolResponse.SCS_ACK != WakeUpResponse)
                        {
                            //If the meter did not wakeup and get identified then 
                            //we cannot issue security.  Break out and return an
                            //error.
                            break;
                        }
                    }
                    else if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                    {
                        //Store off the current security code
                        m_strCurrentSecurityCode = strPassword;
                    }
                }                
            }
            else
            {
                //Security was already issued
                ProtocolResponse = SCSProtocolResponse.SCS_ACK;
            }
            
							
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				// Check to make sure derived class matches what the 
				// meter is telling us
				if ( VerifyDeviceType())
				{				
					Result = ItronDeviceResult.SUCCESS;
						
					m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Comm. Timeout" );
					// Set the comm timeout in the meter.  We do not 
					// check the result due to the fact if we have a 
					// tertiary password this will fail yet we still 
					// want to pass security.
					m_SCSProtocol.Download(
						CommunicationTimeoutAddress, 
						byCommTimeout.Length, 
						ref byCommTimeout);					
				}
			}
			else 
			{
                // Interpret all other responses as security errors. The MTR200
                // in particular behaves erratically to invalid passwords.
				Result = ItronDeviceResult.SECURITY_ERROR;
			}           
						
			return Result;			
		} // End Security()

		/// <summary>
		/// This method logs off the SCS device.
		/// </summary>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSDevice SCSDevice = new SCSDevice(Comm);
		/// SCSDevice.Logon();
		/// SCSDevice.Security();
		/// SCSDevice.Logoff();		
		/// </code>
		/// </example>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A    Created
		// 05/23/06 jrf 7.30.00  N/A	 Rewrote
		// 06/19/06 jrf 7.30.00  N/A    Removed error cases cause we just don't 
		//                              care if logoff fails.  Added flag on 
		//                              constant.
        // 12/21/06 jrf 8.00.03 SCR 74  Added try catch around hangup flag download
        //                              to catch timeout exception

		public override void Logoff()
		{	
			m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting Logoff" );
		
			// Set up the hang up flag to send
			byte[] byHangUpFlag = new byte[1];
			byHangUpFlag[0] = SCS_FLAG_ON;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Hangup Flag" );
			
            try
            {
                // Send the hang up flag
                m_SCSProtocol.Download(
                    CommunicationsHangUpFlagAddress,
                    byHangUpFlag.Length,
                    ref byHangUpFlag);
            }
            catch (Exception)
            {
                // If we cannot logoff, then the meter has timed out or something has 
                // gone wrong.  In any case we will let the meter recover on
                // it's own.
            }

            m_strCurrentSecurityCode = null;

		} // End Logoff()	

		/// <summary>
		/// This method adjusts a connected SCS device's time from 
        /// the system time based on the given offset.
		/// </summary>
		/// <param name="intOffset">Offset from system time</param>
		/// <returns>A ClockAdjustResult representing the result of the 
		/// clock adjust.</returns>
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
		//  05/22/06 mrj 7.30.00  N/A    Created
		//  05/23/06 jrf 7.30.00  N/A	 Revised
		//  06/20/06 jrf 7.30.00  N/A    Removed commport check, delayed setting 
		//	 							 time until just before it was downloaded.
		//	06/29/06 jrf 7.30.00  N/A	 Moved reading of LP interval size to after 
		//								 check for LP. 
		//  07/19/06 jrf 7.30.34  SCR 9, Modified adjust clock to offset from the  
		//						  10,11, meter's time and not the system's time.
		//						  12
		//	07/21/06 jrf 7.30.35  SCR 9  Fixed for case where meter is not a load 
		//								 profile meter.
		// 	07/21/06 jrf 7.30.35  SCR 27 Fixed problem with reenabling the clock
		// 	                             after a failed reconfigure. 		
        //  10/24/06 mrj 7.35.07 112     Only stop metering if clock adjust needed.
		//  01/26/07 KRC 8.00.09         Since Stop Metering is going to be implemented
        //                                for all devices I had to remove from here.  I overrode
        //                                adjustclock in the Fulcrum so it could stop the metering.
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
					
			m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting AdjustClock" );

			// Check clock
			if ( !m_clockEnabled.Cached )
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Checking Clock" );
				Result = SCSToClockAdjustResult( m_SCSProtocol.Upload(
					ClockRunFlagAddress, 
					SCS_FLAG_LENGTH, 
					out byClockRunFlag ) );
				if ( ClockAdjustResult.SUCCESS == Result )
				{
					m_clockEnabled.Value = ( byClockRunFlag[0] != SCS_FLAG_OFF );
				}				
			}
			
			if ( m_clockEnabled.Cached && !m_clockEnabled.Value )
			{
				Result = ClockAdjustResult.ERROR_CLOCK_NOT_RUNNING;			
			}
					
			if ( ClockAdjustResult.SUCCESS == Result )
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

			if ( ClockAdjustResult.SUCCESS == Result )
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
                    }// End if ( Result == Success )                    
				}
				catch(Exception e)
				{
					// If anything happens try to put the meter back the 
					// way you found it.
					
					// Turn the clock back on if necessary  
					if ( bReconfigureClock )
					{
						Result = ClockAdjustSetReconfigureFlag( bCheckIntBound );
					}
					throw(e);
				}

			}// End if ( Result == SUCCESS )
				 			
			return Result;
		} // End AdjustClock()
		
		/// <summary>
		/// Updates DST dates in the connected SCS device.  This method does 
        /// not reconfigure DST dates. Only future dates in 2007 and beyond 
        /// are updated.
		/// </summary>
		/// <param name="strFileName">The filename including path for the DST 
		/// file</param>
		/// <returns>A DSTUpdateResult representing the result of the DST Update.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 06/12/06 jrf 7.30.00  N/A   Rewrote
		// 06/29/06 jrf 7.30.00  N/A   Added Check around UpdateDSTCalendar to 
		//								make sure it is only run with Result
		//								being success.
        // 01/12/07 mah 8.00.00 Cleared the TOU cache after updating DST so
        //                             that all subsequent data displays will reflect the 
        //                             meter's new DST values
		//
		public override DSTUpdateResult UpdateDST( string strFileName )
		{
			DSTUpdateResult	Result = DSTUpdateResult.SUCCESS;

			m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Starting UpdateDST" );

			try
			{
				m_dstSchedule = new CDSTSchedule( strFileName );
			}
			catch ( Exception )
			{
				Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
			}

			if ( DSTUpdateResult.SUCCESS == Result )
			{
				Result = UpdateDSTCalendar();
			}

			// release the DST server
			m_dstSchedule = null;
            
            //Also clear the cached TOU schedule if it exists
            m_touSchedule = null;

			return Result;
		}
		
		/// <summary>
		/// Reconfigures TOU in the connected meter.
		/// </summary>
		/// <param name="TOUFileName">The filename including path for the TOU
		/// export</param>
		/// <param name="DSTFileName">The filename including path for the DST
		/// file.  This parameter must be provided if the meter is configured  
		/// with DST.</param>
		/// <returns>A TOUReconfigResult representing the result of the TOU 
        /// Reconfigure.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/30/06 jrf 7.30.00  N/A   Created
		// 01/24/07 mrj 8.00.08		Clear the cache after the reconfigure
		//	</remarks>
		//	
		public override TOUReconfigResult ReconfigureTOU( string TOUFileName, 
			string DSTFileName )
		{		
			TOUReconfigResult		Result = TOUReconfigResult.ERROR;
			CTOUSchedule			TOUSchedule = null;
			SCSTOUEventCollection	TOUEventList = new SCSTOUEventCollection();
			SCSSeasonCollection		TOUSeasons = new SCSSeasonCollection();
			SCSTOUInfo				TOUInfo = null;
			int						ConfigSpaceAvailable = 0;


#if TOU_RECONFIG_TEST 
			ReadTOUInfo( out TOUInfo );
			TOUInfo.Dump();
			ReadTOUEvents( TOUInfo, out TOUEventList );
			TOUEventList.Dump();
			ReadSeasons( TOUInfo, out TOUSeasons );
			TOUSeasons.Dump();
			TOUSeasons.Clear();
			TOUEventList.Clear();
#endif

		
			// Read the block of TOU info for the current config from the meter.
			// We will need this for  validation and for writing the new 
			// data later.  
			if( SCSProtocolResponse.SCS_ACK == ReadTOUInfo( out TOUInfo ))
			{
                Result = ValidateForTOUReconfigure(TOUFileName, DSTFileName,
                    TOUInfo, out TOUSchedule, out m_dstSchedule);
			}

            if( TOUReconfigResult.SUCCESS == Result )
			{
				ReconfigureSeasons( TOUSchedule, TOUSeasons );

                Result = ReconfigureCalendar(TOUSchedule, m_dstSchedule, 
					TOUEventList );
			}

			// Calculate the amount of space available for the TOU Calendar
			// configuration based on the current configuration (Display Size,
			// etc.), the size of the new seasons, and whether this meter saves
			// last season register data.
			if( TOUReconfigResult.SUCCESS == Result )
			{
				if( TOUConfigLastAvailAddress > 
					TOUInfo.StartOfYearlySchedule + TOUSeasons.Size )
				{
					ConfigSpaceAvailable = TOUConfigLastAvailAddress - 
						TOUInfo.StartOfYearlySchedule - TOUSeasons.Size;
					
					// Try to truncate the event list (calendar) to fit in the
					// available space.  If we can't fit at least one whole
					// year in, fail this reconfigure attempt.
					if( false == TOUEventList.TruncateToFit(ConfigSpaceAvailable) )
					{
						m_Logger.WriteLine(Logger.LoggingLevel.Detailed, 
							"Not enough space to configure the schedule" );
						m_Logger.WriteLine(Logger.LoggingLevel.Detailed, 
							"Available space = " + ConfigSpaceAvailable );

						Result = TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
					}
				}
				else
				{
					m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "TOUConfigLastAvailAddress = 0x" +
                        TOUConfigLastAvailAddress.ToString("X4", CultureInfo.InvariantCulture) + ", TOUSeasons.Size = 0x" +
                        TOUSeasons.Size.ToString("X4", CultureInfo.InvariantCulture) + "StartOfYearlySchedule = 0x" +
                        TOUInfo.StartOfYearlySchedule.ToString("X4", CultureInfo.InvariantCulture));

					TOUSeasons.Dump();
					Result = TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
				}
			}
				
			// Update the TOU Info to reflect the new configuration.
			if( TOUReconfigResult .SUCCESS == Result )
			{
				ReconfigureTOUInfo( TOUSchedule, TOUInfo, 
									TOUEventList, TOUSeasons );
			}

			// If all is well, stop the run flag, write the new configuration,
			// and restart metering.
			if( TOUReconfigResult.SUCCESS == Result )
			{
				Result = WriteTOU( TOUInfo, TOUEventList, TOUSeasons );
			}

            // If everything succeeded, we still have to check for a conditional
            // success if they passed a DST filename but the meter wasn't 
            // configured for DST.
            if ((TOUReconfigResult.SUCCESS == Result) && (false == DSTEnabled) 
                && (null != DSTFileName) && (0 < DSTFileName.Length))
            {
                Result = TOUReconfigResult.SUCCESS_DST_NOT_SUPPORTED;
            }

			// Release the servers
            m_dstSchedule = null;
			TOUSchedule = null;
						
			//Also clear the tou schedule and status info
			m_touSchedule = null;
			m_touEnabled.Flush();
			m_touRunFlag.Flush();
			m_TOUScheduleID.Flush();
			m_dateTOUExpiration.Flush();
			m_dstEnabled.Flush();
			m_clockEnabled.Flush();


#if TOU_RECONFIG_TEST 
			ReadTOUInfo( out TOUInfo );
			TOUInfo.Dump();
			ReadTOUEvents( TOUInfo, out TOUEventList );
			TOUEventList.Dump();
			ReadSeasons( TOUInfo, out TOUSeasons );
			TOUSeasons.Dump();
#endif

			return Result;
		}

        /// <summary>
        /// This method resets the demand registers on a connected SCS device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// <example>
        /// <code>
        /// Communication Comm = new Communication();
        /// Comm.OpenPort("COM4:");
        /// SCSDevice SCSDevice = new SCSDevice(Comm);
        /// SCSDevice.Logon();
        /// SCSDevice.Security();
        /// SCSDevice.ResetDemand();
        /// SCSDevice.Logoff();		
        /// </code>
        /// </example>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/15/06 mah 7.35.00  N/A    Created
        // 
        public override ItronDeviceResult ResetDemand()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            byte[] byFlag = new byte[1];

            byFlag[0] = SCS_FLAG_ON;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting the demand reset flag");

            // Set the Reset Demand Flag

            SCSProtocolResponse SCSResponse = m_SCSProtocol.Download(
                    DemandResetFlagAddress,
                    SCS_FLAG_LENGTH,
                    ref byFlag);

            switch ( SCSResponse )
            {
                case SCSProtocolResponse.SCS_ACK : Result = ItronDeviceResult.SUCCESS;
                                                   break;
                case SCSProtocolResponse.SCS_CAN : Result = ItronDeviceResult.SECURITY_ERROR;
                                                   break;
                default : Result = ItronDeviceResult .ERROR;
                          break;
            }

            return Result;
        } // End ResetDemand()

        /// <summary>
        /// This method clears the billing data on a connected SCS device.
        /// </summary>
        /// <returns>An ItronDeviceResult representing the result of the reset
        /// operation.</returns>
        /// <example>
        /// <code>
        /// Communication Comm = new Communication();
        /// Comm.OpenPort("COM4:");
        /// SCSDevice SCSDevice = new SCSDevice(Comm);
        /// SCSDevice.Logon();
        /// SCSDevice.Security();
        /// SCSDevice.ClearBillingData();
        /// SCSDevice.Logoff();		
        /// </code>
        /// </example>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/15/06 mah 7.35.00  N/A    Created
        // 
        public override ItronDeviceResult ClearBillingData()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            byte[] byFlag = new byte[1];

            byFlag[0] = SCS_FLAG_ON;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting the clear billing data flag");

            // Set the Reset Demand Flag

            SCSProtocolResponse SCSResponse = m_SCSProtocol.Download(
                    ClearBillingDataFlagAddress,
                    SCS_FLAG_LENGTH,
                    ref byFlag);

            switch (SCSResponse)
            {
                case SCSProtocolResponse.SCS_ACK:
                    // Meter needs a 1 second pause
                    Thread.Sleep(1000);

                    Result = ItronDeviceResult.SUCCESS;
                    break;
                
                case SCSProtocolResponse.SCS_CAN: 
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                
                default: 
                    Result = ItronDeviceResult.ERROR;
                    break;
            }

            return Result;
        } // End ResetDemand()

        /// <summary>
        /// The PasswordReconfigResult reconfigures passwords.
        /// </summary>
        /// <param name="Passwords">A list of passwords to write to the meter. 
        /// The Primary password should be listed first followed by the secondary
        /// password and so on.  Use empty strings for null passwords.  Passwords
        /// will be truncated or null filled as needed to fit in the device.</param>
        /// <returns>A PasswordReconfigResult object</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 08/16/06 mcm 7.35.00 N/A    Created
        //	
        public override PasswordReconfigResult ReconfigurePasswords(
                            System.Collections.Generic.List<string> Passwords)
        {
            PasswordReconfigResult  Result = PasswordReconfigResult.ERROR;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.SCS_ACK;
            byte[] byPassword = new byte[SIZE_OF_SCS_PASSWORD];


            // Write the passwords in reverse order. If it fails, they'll 
            // probably still have their primary password.
            if ((HasTertiaryPassword) && (3 <= Passwords.Count))
            {
                NullFillPassword(Passwords[2], SIZE_OF_SCS_PASSWORD, ref byPassword);
                ProtocolResponse = m_SCSProtocol.Download(TertiaryPasswordAddress,
                                    SIZE_OF_SCS_PASSWORD, ref byPassword);

                // The VECTRON gets a copy of the tertiary password for its modem to 
                // use. Do this after writing the Tertiary password because the meter
                // allowed me to write to the modem password with tertiary access.
                if ((SCSProtocolResponse.SCS_ACK == ProtocolResponse) &&
                   (HasModemPassword) && (3 <= Passwords.Count))
                {
                    NullFillPassword(Passwords[2], SIZE_OF_SCS_PASSWORD, ref byPassword);
                    ProtocolResponse = m_SCSProtocol.Download(ModemPasswordAddress,
                                        SIZE_OF_SCS_PASSWORD, ref byPassword);
                }
            }
            if ((SCSProtocolResponse.SCS_ACK == ProtocolResponse) &&
                (2 <= Passwords.Count))
            {
                NullFillPassword(Passwords[1], SIZE_OF_SCS_PASSWORD, ref byPassword);
                ProtocolResponse = m_SCSProtocol.Download(SecondaryPasswordAddress,
                                    SIZE_OF_SCS_PASSWORD, ref byPassword);
            }
            if ((SCSProtocolResponse.SCS_ACK == ProtocolResponse) &&
                (1 <= Passwords.Count))
            {
                NullFillPassword(Passwords[0], SIZE_OF_SCS_PASSWORD, ref byPassword);
                ProtocolResponse = m_SCSProtocol.Download(PrimaryPasswordAddress,
                                    SIZE_OF_SCS_PASSWORD, ref byPassword);
            }
            if ((SCSProtocolResponse.SCS_ACK == ProtocolResponse) &&
                (1 <= Passwords.Count))
            {
                ProtocolResponse = ForceCopyData();
            }

            // Translate Protocol result
            if( SCSProtocolResponse.SCS_ACK == ProtocolResponse ) 
            {
                Result = PasswordReconfigResult.SUCCESS;
            }
            else if( SCSProtocolResponse.SCS_CAN == ProtocolResponse ) 
            {
                Result = PasswordReconfigResult.SECURITY_ERROR;
            }
            else if( SCSProtocolResponse.SCS_NAK == ProtocolResponse ) 
            {
                Result = PasswordReconfigResult.PROTOCOL_ERROR;
            }
            else // ( SCSProtocolResponse.NoResponse == ProtocolResponse ) 
            {
                Result = PasswordReconfigResult.IO_TIMEOUT;
            }
            
            return Result;

        } // ReconfigurePasswords

        /// <summary>
        /// This method creates an MV-90 HHF file.
        /// </summary>        
        /// <param name="bReadRegisters">Flag indicating whether or not to 
        /// include TOU register data in the HHF</param>
        /// <param name="bReadLP">Flag indicating whether or not to include
        /// load profile data in the HHF</param>
        /// <param name="uiDaysToRead">The max number of days of load profile
        /// data to include in the HHF.  Passing a zero value will result in 
        /// all load profile data being read.</param>        
        /// <param name="strHHFFilePath">The path and file name of the HHF file</param>
        /// <returns>TIM_HHFCreationResult representing the result of the 
        /// HHF creation.</returns>
		/// <exception cref="TimeOutException">
		/// Thrown if unable to log back on to the meter after creating the HHF file
		/// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/29/06 mrj 7.30.00 N/A    Created
        //  09/14/06 jrf 7.35.00 N/A    Setting protocol's property Identified to false
        //                              so that Logon will wakeup and identify meter; this is 
        //                              necessary for the call to Security to work.
        //  09/14/06 jrf 7.35.00 N/A    Fixing bug where the intervals per hour was set to 
        //                              interval length.
        //  10/12/06 mrj 7.35.04 72     Need to save off security code before
        //                              calling logoff.
        //  02/14/07 jrf 8.00.12        Removed file name and directory parameters
        //                              and added a HHF file path parameter, modified 
        //                              call to TIM.CreateHHF()
        //  02/15/07 jrf 8.00.12        Changed call to TIM.CreateHHF(), added check 
        //                              to make sure logfilename was not null
        //  04/11/06 jrf 8.00.29  2901  We need to set the baud rate when logging 
        //                              back on after HHF creation otherwise 
        //                              logging on at 4800 baud will fail.
        // 04/12/07  jrf 8.00.29 2883   Changed to pass in baud rate to TIM.CreateHHF()
        // 
        public override TIM_HHFCreationResult CreateHHF(bool bReadRegisters,
                                                        bool bReadLP,
                                                        uint uiDaysToRead,
                                                        string strHHFFilePath)
        {
            TIM_HHFCreationResult HHFResult = TIM_HHFCreationResult.ERROR;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;
            string strLogFileName = "";

            //Save off current communication information            
            string strPortName = m_SCSProtocol.m_CommPort.PortName;
            uint uiPortNumber = 0;
            uint uiBaudRate = m_SCSProtocol.m_CommPort.BaudRate;
            OpticalProbeTypes OpticalProbe = m_SCSProtocol.m_CommPort.OpticalProbe;

            //Convert the port name to port number
            string temp = strPortName.Remove(0, 3);
            temp = temp.TrimEnd(':');
            uiPortNumber = Convert.ToUInt32(temp, CultureInfo.InvariantCulture);

            //Get the intervals per hour
            uint uiIntPerHour = 0;
            if (LPRunning)
            {
                // Upload and convert interval size
                uiIntPerHour = MINUTES_PER_HOUR / (uint)LPIntervalLength;
            }
            
            //Save off the meter type
            string strDevice = MeterType;


            if (m_Logger.Initialized)
            {
                //Save off the current log file name
                strLogFileName = m_Logger.FileName;
                if (null == strLogFileName)
                {
                    strLogFileName = "";
                }
            }


            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Creating HHF");

            //Before we logoff we need to get the security code, since it will
            //get cleared
            string strSecurityCode = m_strCurrentSecurityCode;

            //Logoff the current device
            Logoff();

            //Close the port
            m_SCSProtocol.m_CommPort.ClosePort();


            try
            {

                //Call the TIMRunner to create the HHF
                HHFResult = Itron.Metering.TIM.TIM.CreateHHF(uiPortNumber,
                                                             uiBaudRate,
                                                             OpticalProbe,
                                                             strDevice,
                                                             strSecurityCode,
                                                             bReadRegisters,
                                                             bReadLP,
                                                             uiDaysToRead,
                                                             uiIntPerHour,
                                                             strLogFileName,
                                                             strHHFFilePath);
            }
            catch
            {
                //If this fails then just return an error
                HHFResult = TIM_HHFCreationResult.ERROR;
            }

            //Need to do this so Logon will reissue Wakeup and Identify requests, otherwise
            //Security will fail.
            m_SCSProtocol.Identified = false;

            //Re-open the port
            m_SCSProtocol.m_CommPort.OpenPort(strPortName);

            //Set to the appropriate baud rate
            m_SCSProtocol.m_CommPort.BaudRate = uiBaudRate;

            
            if (MeterType == m_rmStrings.GetString("FULC_METER_NAME"))
            {
                //Fulcrum needs a few seconds to itself before allowing a logon
                //after a logoff
                Thread.Sleep(4000);
            }            

            //Re-Logon to the meter            
            Result = Logon();
            
            if (ItronDeviceResult.SUCCESS == Result)
            {                
                //Issue Security
                System.Collections.Generic.List<string> passwords = new System.Collections.Generic.List<string>();
                
                                
                //It should be cleared but just be be safe, cleare out the current
                //security code so we will issue the command.
                m_strCurrentSecurityCode = null;
                passwords.Add(strSecurityCode);
                
                Result = Security(passwords);
            }

            if (ItronDeviceResult.SUCCESS != Result)
            {
                //We were not successful in logging back on so throw a timeout
                throw new TimeOutException();
            }
            

            return HHFResult;
        }

#if (!WindowsCE)

        /// <summary>
        /// This method creates an MIF file. 
        /// </summary>   
        /// <param name="strMIFFileName">
        /// The full file name and path of the MIF file to be created.  
        /// </param>
        /// <returns>MIFCreationResult representing the result of the MIF 
        /// creation.</returns>
		/// <exception cref="TimeOutException">
		/// Thrown if unable to log back on to the meter after creating the MIF file
		/// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/09/07 MAH 8.00.00        created
        //  02/15/07 jrf 8.00.12        Reworked to use Device Wrapper
        //  02/21/07 jrf 8.00.13        Updating the name of the namespace and class
        //                              for calling create mif.
        //  04/11/06 jrf 8.00.29  2891  We need to set the baud rate when logging 
        //                              back on after MIF creation otherwise 
        //                              logging on at 4800 baud will fail.

        public override MIFCreationResult CreateMIF( String strMIFFileName)
        {
            #region .Net Implementation
            // Leaving this code to be used when we switch to a .Net Solution
            
            /*Boolean boolDiscardMIF = false; // used when errors are detected
            MIF.CreateMIFResult Result = MIF.CreateMIFResult.SUCCESS;

            // Create the file

            MIF mifFile = new MIF( strMIFFileName );

            Result = mifFile.CreateFile(MIFType, UnitID, SerialNumber, NumberLPChannels);

            if (MIF.CreateMIFResult.SUCCESS == Result)
            {
                try
                {
                    byte[] abytBasepageImage = CreateBasepageImage();

                    // Write the meter image

                    mifFile.WriteMeterData(abytBasepageImage);
                }

                catch (Exception e)
                {
                    // Since an unexpected error occurred, set a flag to indicate that the MIF
                    // file should be thrown away since if was not completed
                    boolDiscardMIF = true;

                    // Then just throw the error back to the caller.  This will provide them with the 
                    // detailed error information to allow them to either correct or work around
                    // the problem
                    throw (e);
                }

                finally
                {
                    // In all cases we have to close the MIF file and clean up

                    mifFile.CloseFile(boolDiscardMIF);
                }
            }

            return Result;*/
            #endregion

            MIFCreationResult MIFResult = MIFCreationResult.ERROR;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;

            //Save off current communication information            
            string strPortName = m_SCSProtocol.m_CommPort.PortName;
            uint uiPortNumber = 0;
            uint uiBaudRate = m_SCSProtocol.m_CommPort.BaudRate;
            OpticalProbeTypes OpticalProbe = m_SCSProtocol.m_CommPort.OpticalProbe;

            //Before we logoff we need to get the security code, since it will
            //get cleared
            string strSecurityCode = m_strCurrentSecurityCode;

            //Convert the port name to port number
            string temp = strPortName.Remove(0, 3);
            temp = temp.TrimEnd(':');
            uiPortNumber = Convert.ToUInt32(temp, CultureInfo.InvariantCulture);

            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Creating MIF");

            //Logoff the current device
            Logoff();

            //Close the port
            m_SCSProtocol.m_CommPort.ClosePort();

            try
            {
                //Create the MIF
                MIFResult = (MIFCreationResult)
                    Itron.Metering.DeviceWrapper.DeviceWrapper.CreateMIF(strMIFFileName,
                                                                                  MeterType,
                                                                                  strSecurityCode,
                                                                                  uiPortNumber,
                                                                                  uiBaudRate,
                                                                                  OpticalProbe);
            }
            catch
            {
                MIFResult = MIFCreationResult.ERROR;
            }

            //Need to do this so Logon will reissue Wakeup and Identify requests, otherwise
            //Security will fail.
            m_SCSProtocol.Identified = false;

            //Re-open the port
            m_SCSProtocol.m_CommPort.OpenPort(strPortName);

            //Set to the appropriate baud rate
            m_SCSProtocol.m_CommPort.BaudRate = uiBaudRate;

            if (MeterType == m_rmStrings.GetString("FULC_METER_NAME"))
            {
                //Fulcrum needs a few seconds to itself before allowing a logon
                //after a logoff
                Thread.Sleep(4000);
            }

            //Re-Logon to the meter            
            Result = Logon();

            if (ItronDeviceResult.SUCCESS == Result)
            {
                //Issue Security
                System.Collections.Generic.List<string> passwords = new System.Collections.Generic.List<string>();


                //It should be cleared but just be be safe, clear out the current
                //security code so we will issue the command.
                m_strCurrentSecurityCode = null;
                passwords.Add(strSecurityCode);

                Result = Security(passwords);
            }

            if (ItronDeviceResult.SUCCESS != Result)
            {
                //We were not successful in logging back on so throw a timeout
                throw new TimeOutException();
            }

            return MIFResult;
        }
#endif

        /// <summary>
        /// Change the Display Mode
        /// </summary>
        /// <returns>ItronDeviceResult representing the result of the 
        /// display mode change.</returns>
		/// <exception cref="NotImplementedException">
		/// Thrown if eMode does not equal test mode.  SCS devices do not support
		/// switching to any display modes other than test mode
		/// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/21/06 MAH 8.00.00 N/A    Created

        public override ItronDeviceResult ChangeDisplayMode(DisplayMode eMode)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            //First we need to figure out the correct command based on what the user
            // wants to switch to.
            if (eMode != DisplayMode.TEST_MODE)
            {
                throw (new NotImplementedException("SCS devices only support switching to test mode"));
            }
            else
            {
                byte[] byFlag = new byte[1];

                byFlag[0] = SCS_FLAG_ON;

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Switching to test mode");

                SCSProtocolResponse SCSResponse = m_SCSProtocol.Download(
                        RemoteTestModeFlagAddress,
                        SCS_FLAG_LENGTH,
                        ref byFlag);

                switch (SCSResponse)
                {
                    case SCSProtocolResponse.SCS_ACK:
                        // Meter needs a 1 second pause
                        Thread.Sleep(1000);

                        Result = ItronDeviceResult.SUCCESS;
                        break;

                    case SCSProtocolResponse.SCS_CAN:
                        Result = ItronDeviceResult.SECURITY_ERROR;
                        break;

                    default:
                        Result = ItronDeviceResult.ERROR;
                        break;
                }
            }
            
            return Result;
        }

        /// <summary>
        /// Changes the list of registers sent back to a new value
        /// </summary>
        /// <returns>
        /// List of Editable Items (We use DisplayItems as the transport).  (Null if list does not exist)
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/16/07 KRC  8.00.06			Created 
        public override ItronDeviceResult EditRegisters(List<DisplayItem> lstRegisters)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;

            SCSProtocolResponse SCSResponse = StopMetering(true);

            // Meter requires a sleep after issuing StopMetering
            Thread.Sleep(1000);

            if (SCSProtocolResponse.SCS_ACK == SCSResponse)
            {
                foreach (DisplayItem dispItem in lstRegisters)
                {
                    if ((dispItem is SCSDisplayItem) && (true == dispItem.Editable))
                    {
                        // The value is editable so there may be a value.  If the value is null or empty then don't bother
                        if (null != dispItem.Value && "" != dispItem.Value)
                        {
                            Result = ((SCSDisplayItem)dispItem).WriteNewValue(this);
                        }
                    }
                }
            }

            // Always try to start the metering flag again no matter what happened before
            SCSResponse = StopMetering(false);

            // Meter requires a sleep after issuing StopMetering
            Thread.Sleep(1000);

            switch (SCSResponse)
            {
                case SCSProtocolResponse.SCS_ACK: Result = ItronDeviceResult.SUCCESS;
                break;
                case SCSProtocolResponse.SCS_CAN: Result = ItronDeviceResult.SECURITY_ERROR;
               break;
               default: Result = ItronDeviceResult.ERROR;
               break;
            }

            return Result;
        }

		/// <summary>
		/// Resets the count of demand resets
		/// </summary>
		/// <returns>ItronDeviceResult representing the reset of the count 
        /// of demand resets.</returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  01/29/07 MAH  8.00.09			Created 
		public override ItronDeviceResult ResetNumberDemandResets()
		{
			ItronDeviceResult Result = ItronDeviceResult.ERROR; // guilty until proven innocent

			SCSProtocolResponse ProtocolResponse = ResetEventCounter(NumResetsAddress, "Number of Demand Resets");

			// Determine the SCS Response
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				Result = ItronDeviceResult.SUCCESS;
			}
			else if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
			{
				Result = ItronDeviceResult.SECURITY_ERROR;
			}
			else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_S,
					ProtocolResponse,
					0,
					"Error occurred while resetting number of demand resets");

				throw scsException;
			}

			return Result;
		}

		/// <summary>
		/// Resets the Number of Power Outages
		/// </summary>
		/// <returns>ItronDeviceResult representing the result of resetting
        /// the number of power outages.</returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  01/29/07 MAH  8.00.09			Created 
		public override ItronDeviceResult ResetNumberPowerOutages()
		{
			ItronDeviceResult Result = ItronDeviceResult.ERROR; // guilty until proven innocent

			SCSProtocolResponse ProtocolResponse = ResetEventCounter(NumOutagesAddress, "Number of Outages");

			// Determine the SCS Response
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				Result = ItronDeviceResult.SUCCESS;

				m_numOutages.Flush(); // Clear the cache since the value  should now be 0
			}
			else if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
			{
				Result = ItronDeviceResult.SECURITY_ERROR;
			}
			else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_S,
					ProtocolResponse,
					0,
					"Error occurred while resetting number of outages");

				throw scsException;
			}

			return Result;
		}

		/// <summary>
		/// Resets the Number of Times Programmed
		/// </summary>
		/// <returns>ItronDeviceResult representing the resetting of the number 
        /// of time programmed.</returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  01/29/07 MAH  8.00.09			Created 
		public override ItronDeviceResult ResetNumberTimesProgrammed()
		{
			ItronDeviceResult Result = ItronDeviceResult.ERROR; // guilty until proven innocent

			SCSProtocolResponse ProtocolResponse = ResetEventCounter(NumTimesProgrammedAddress, "Number of Times Programmed");

			// Determine the SCS Response
			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				Result = ItronDeviceResult.SUCCESS;

				m_numTimesProgrammed.Flush(); // Clear the cache since the value  should now be 0
			}
			else if (SCSProtocolResponse.SCS_CAN == ProtocolResponse)
			{
				Result = ItronDeviceResult.SECURITY_ERROR;
			}
			else if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
			{
				SCSException scsException = new SCSException(
					SCSCommands.SCS_S,
					ProtocolResponse,
					0,
					"Error occurred while resetting number of times programmed");

				throw scsException;
			}

			return Result;
		}

        #endregion Public Methods

		#region Public Properties
        
		/// <summary>This property gets unit ID from the meter.</summary>
		/// <returns>
		/// A string representing the unit ID.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified
		//	
		public override string UnitID
		{
			get
			{
				if (!m_unitID.Cached)
				{
					ReadUnitID();
				}
				return m_unitID.Value;			
			}
		}

		/// <summary>This property gets program ID from the meter.</summary>
		/// <returns>
		/// An int representing the program ID.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified

		//	
		public override int ProgramID
		{
			get
			{
				if (!m_programID.Cached)
				{
					ReadProgramID();
				}
				return m_programID.Value;	
			}
		}

		/// <summary>This property gets device time from the meter.</summary>
		/// <returns>
		/// A date/time representing the device time.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified
		//	
		public override DateTime DeviceTime
		{
			get
			{
				DateTime objDeviceTime; 
				ReadDeviceTime(out objDeviceTime);
				return objDeviceTime;
			}
		}

		/// <summary>This property gets firmware revision from the meter.
		/// </summary>
		/// <returns>
		/// A float representing the firmware revision.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified
		//	
		public override float FWRevision
		{
			get 
			{
				if( !m_fwVersion.Cached ) 
				{
					ReadFWVersion();
				}

				return m_fwVersion.Value; 
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
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		//	
		public override string MeterType
		{
			get
			{
				return "";
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
                return "";
            }
        }

		/// <summary>This property gets serial number from the meter.</summary>
		/// <returns>
		/// A string representing the serial number.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/23/06 jrf 7.30.00  N/A   Created
		//	
		public override string SerialNumber
		{
			get
			{
				if( !m_serialNumber.Cached ) 
				{
					ReadSerialNumber();
				}
				
				return m_serialNumber.Value; 
			}
		}

		/// <summary>This property gets dst enabled flag from the meter.</summary>
		/// <returns>
		/// A bool representing the dst enabled flag.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified
		//	
		public override bool DSTEnabled
		{
			get
			{
				if( !m_dstEnabled.Cached ) 
				{
					ReadOperatingSetup();
				}
				return m_dstEnabled.Value;
			}
		}

		/// <summary>
        /// TOU is considered enabled if the clock is running and the meter
        /// is configured to follow a TOU schedule.  TOU does not have to be
        /// running for this property to return true.  For example an expired
        /// TOU schedule is enabled but not running.
        /// </summary>
		/// <returns>
		/// True if TOU is configured and the clock is running.
		/// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/22/06 mrj 7.30.00  N/A   Created
		// 05/23/06 jrf 7.30.00  N/A   Modified
        // 10/25/06 mcm 7.35.07 106    Clarified definition and modified to match
		//	
		public override bool TOUEnabled
		{
			get
			{
				if( !m_touEnabled.Cached ) 
				{
                    if (ClockEnabled)
                    {
                        SCSProtocolResponse Result;
			            SCSTOUInfo	TOUInfo = null;

                        Result = ReadTOUInfo(out TOUInfo);
                        if (SCSProtocolResponse.SCS_ACK == Result)
                        {
                            m_touEnabled.Value = TOUInfo.TOUIsConfigured;
                        }
                        else
                        {
                            SCSException scsException = new SCSException(
                                    SCSCommands.SCS_U,
                                    Result,
                                    YearlyScheduleAddress,
                                    "TOU Info");

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

		/// <summary>
        /// Returns the state of the TOU Run Flag.  TOU might be running for
        /// DST without TOU being enabled (TOU Rate schedule), so make sure you
        /// use the right property!
        /// </summary>
		/// <returns>
		/// A bool representing the tou enabled flag.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
        // 10/25/06 mcm 7.35.07 106    Separated TOU Run flag from TOU enabled
		//	
		public bool TOURunFlag
		{
			get
			{
                if (!m_touRunFlag.Cached) 
				{
                    GetTOURunFlag();
				}

                return m_touRunFlag.Value;
			}
		}
		/// <summary>This property gets clock enabled flag from the meter.</summary>
		/// <returns>
		/// A bool representing the clock enabled flag.
		/// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 07/04/06 mcm 7.30.00  N/A   Created
		//	
		public bool ClockEnabled
		{
			get
			{
				byte[]				byClockRunFlag;
				SCSProtocolResponse	Result; 

				// Check clock
				if ( !m_clockEnabled.Cached )
				{
					Result = m_SCSProtocol.Upload( ClockRunFlagAddress, 
						SCS_FLAG_LENGTH, 
						out byClockRunFlag );

					if ( SCSProtocolResponse.SCS_ACK == Result )
					{
						m_clockEnabled.Value = 
							( byClockRunFlag[0] != SCS_FLAG_OFF );
					}
                    else
                    {
                        SCSException scsException = new SCSException(
                                SCSCommands.SCS_U,
                                Result,
                                ClockRunFlagAddress,
                                "Clock Run Flag Address");

                        throw scsException;
                    }			
				}
			
				return m_clockEnabled.Value;
			}
		}

		/// <summary>This property gets device type from the meter.</summary>
		/// <returns>
		/// A string representing the device type.
		/// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/19/06 jrf 7.30.00  N/A   Created
		//	
		public string DeviceType
		{
			get
			{
				if( !m_deviceType.Cached ) 
				{
					// Calls protocol's identify which retrieves the device type
					Logon();
				}
				return m_deviceType.Value;
			}
		}

		/// <summary>
		/// Gets a list of errors in the device.
		/// </summary>	
        /// <returns>
        /// A string array containing the errors the device has.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 05/30/06 jrf 7.30.00  N/A   Created
		// 
		public override string[] ErrorsList
		{
			get
			{	
				string[] strErrors;
				ReadErrors(out strErrors);
				return strErrors;
			}
		}

		/// <summary>This property gets Firmware Options from the meter.</summary>
		/// <returns>
		/// A bool representing the Season Change Registers bit in the 
		/// Firmware Options byte.
		/// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        // Revision History
        // MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 07/04/06 mcm 7.30.00  N/A   Created
		//	
		public bool SeasonChangeEnabled
		{
			get
			{
				byte[]				FWOptions;
				SCSProtocolResponse	Result; 

				// Check clock
				if ( !m_FirmwareOptions.Cached )
				{
					if( 0 < FirmwareOptionsAddress )
					{
						Result = m_SCSProtocol.Upload( FirmwareOptionsAddress, 
							SCS_FLAG_LENGTH, 
							out FWOptions );

						if ( SCSProtocolResponse.SCS_ACK == Result )
						{
							m_FirmwareOptions.Value = FWOptions[0];
						}
                        else
                        {
                            SCSException scsException = new SCSException(
                                    SCSCommands.SCS_U,
                                    Result,
                                    FirmwareOptionsAddress,
                                    "Firmware Options Address");

                            throw scsException;
                        }
					}
					else
					{
						m_FirmwareOptions.Value = 0;
					}
				}
			
				if( 0 == ( m_FirmwareOptions.Value & SCS_SEASON_CHG_MASK ))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

        /// <summary>
        /// This property gets whether IO is enabled.
        /// </summary>
        public bool IOEnabled
        {
            get
            {
                byte[] FWOptions;
                SCSProtocolResponse Result;

                // Check clock
                if (!m_FirmwareOptions.Cached)
                {
                    if (0 < FirmwareOptionsAddress)
                    {
                        Result = m_SCSProtocol.Upload(FirmwareOptionsAddress,
                            SCS_FLAG_LENGTH,
                            out FWOptions);

                        if (SCSProtocolResponse.SCS_ACK == Result)
                        {
                            m_FirmwareOptions.Value = FWOptions[0];
                        }
                        else
                        {
                            SCSException scsException = new SCSException(
                                    SCSCommands.SCS_U,
                                    Result,
                                    FirmwareOptionsAddress,
                                    "Firmware Options Address");

                            throw scsException;
                        }
                    }
                    else
                    {
                        m_FirmwareOptions.Value = 0;
                    }
                }

                if (0 == (m_FirmwareOptions.Value & SCS_IOENABLED_MASK))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>This property returns the number of times the meter
        /// has been programmed.  Note that this value is cached and
        /// cannot be updated in any given communications session.  This is 
        /// a read-only value.</summary>
        /// <returns>
        /// An integer representing the number of times programmed.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 mah 7.35.00  N/A   Created
        //	</remarks>
        //	
        public override int NumTimeProgrammed
        {
            get
            {
                if (!m_numTimesProgrammed.Cached)
                {
                    m_numTimesProgrammed.Value = ReadEventCounter(NumTimesProgrammedAddress, "Number of Times Programmed");
                }

                return m_numTimesProgrammed.Value;
            }
        }

        /// <summary>This property returns the date and time when the meter 
        /// was last programmed.  Note that this value is cached and
        /// cannot be updated in any given communications session.  This is 
        /// a read-only value.</summary>
        /// <returns>
        /// A date/time indicating when the meter was programmed.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 mah 7.35.00  N/A   Created
        //	
        public override DateTime DateProgrammed
        {
            get
            {
                if (!m_dateLastProgrammed.Cached)
                {
                    ReadDateLastProgrammed();
                }

                return m_dateLastProgrammed.Value;
            }
        }

        /// <summary>This property returns the number of times the meter's
        /// demand values have been reset.  Note that this value is 
        /// a read-only value.</summary>
        /// <returns>
        /// An integer representing the number of times a demand reset has 
        /// been performed.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 mah 7.35.00  N/A   Created
        //	
        public override int NumDemandResets
        {
            get
            {
                return ReadEventCounter(NumResetsAddress, "Number of Demand Resets");
            }
        }

        /// <summary>This property returns the date and time when the meter's 
        /// demand was last reset. This is a read-only value.</summary>
        /// <returns>
        /// A date/time indicating when the demand was last reset.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 mah 7.35.00  N/A   Created
        //	
        public override DateTime DateLastDemandReset
        {
            get
            {
                return ReadLastResetDate();
            }
        }

        /// <summary>This property returns the number of outages that the
        /// meter has observed.  Note that this value is cached and
        /// cannot be updated in any given communications session - if a 
        /// new outage occurs while online with the meter, the communication
        /// session will be dropped.  This is a read-only value.</summary>
        /// <returns>
        /// An integer representing the number of power outages.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 mah 7.35.00  N/A   Created
        //	
        public override int NumOutages
        {
            get
            {
                if (!m_numOutages.Cached)
                {
                    m_numOutages.Value = ReadEventCounter(NumOutagesAddress, "Number of Outages");
                }

                return m_numOutages.Value;
            }
        }

        /// <summary>This property returns the number of minutes that the
        /// meter run on battery power.  Note that this value is cached and
        /// cannot be updated in any given communications session - if a 
        /// new outage occurs while online with the meter, the communication
        /// session will be dropped.  This is a read-only value.</summary>
        /// <returns>
        /// An integer representing the number of minutes on battery power.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/08/06 mah 7.35.00  N/A   Created
        // 09/12/06 mah 7.35.00  N/A   Simplified by calling 
        // 							virtual method ReadMinutesOnBattery
        //	
        public override uint NumberOfMinutesOnBattery
        {
            get
            {
                if (!m_numMinOnBattery.Cached)
                {
                    ReadMinutesOnBattery();
                }

                return m_numMinOnBattery.Value;
            }
        }

        /// <summary>
        /// Gets the Date of Last Outage
        /// </summary>
        /// <returns>
        /// The Date/Time of the Last Outage.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Thrown since this property has not been implemented
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/18/06 KRC 7.35.00 N/A    Created
        //	
        public override DateTime DateLastOutage
        {
            get
            {
                throw (new NotImplementedException());
            }
        }
        
        /// <summary>
        /// Gets the Date of the TOU Expiration
        /// </summary>
        /// <returns>
        /// The Date/Time that the TOU schedule expires.
        /// </returns>
        /// <exception>
        /// SCSException will be thrown if the value cannot be downloaded from
        /// the meter
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/06 MAH 7.35.00 N/A    Created
        public override DateTime TOUExpirationDate
        {
            get
            {
                if (!m_dateTOUExpiration.Cached)
                {
                    DateTime dateTOUExpiration;

                    SCSProtocolResponse ProtocolResponse = GetTOUExpiration(out dateTOUExpiration);

                    if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                    { 
                        m_dateTOUExpiration.Value = dateTOUExpiration;
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
        /// Gets the Cold Load Pickup Time in minutes
        /// </summary>
        /// <returns>
        /// An unsigned integer representing the cold load pickup time in minutes.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/06 MAH 8.00.00			Created 
        public override uint ColdLoadPickupTime
        {
            get
            {
                if (!m_nCLPU.Cached)
                {
                    ReadCLPU();
                }

                return m_nCLPU.Value;
            }
        }

        /// <summary>
        /// Gets the Number of Sub Intervals for Demands
        /// </summary>
        /// <returns>
        /// An integer representing the number of demand sub intervals.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/06 MAH 8.00.00			Created 
        public override int NumberOfSubIntervals
        {
            get
            {
                if (!m_NumberOfSubIntervals.Cached)
                {
                    ReadDemandConfiguration();
                }

                return m_NumberOfSubIntervals.Value;
            }
        }

        /// <summary>
        /// Gets the Interval Length for Demands
        /// </summary>
        /// <returns>
        /// An integer representing the length of the demand interval.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/06 MAH 8.00.00			Created 
        public override int DemandIntervalLength
        {
            get
            {
                if (!m_DemandIntervalLength.Cached)
                {
                    ReadDemandConfiguration();
                }

                return m_DemandIntervalLength.Value;
            }
        }

        /// <summary>
        /// Gets the Number of Test Mode Sub Intervals for Demands
        /// </summary>
        /// <returns>
        /// An integer representing the number of test mode sub intervals.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/06 MAH 8.00.00			Created 
        public override int NumberOfTestModeSubIntervals
        {
            get
            {
                if (!m_NumberOfTestModeSubIntervals.Cached)
                {
                    ReadDemandConfiguration();
                }

                return m_NumberOfTestModeSubIntervals.Value;
            }
        }

        /// <summary>
        /// Gets the Test Mode Interval Length for Demands
        /// </summary>
        /// <returns>
        /// An integer representing the test mode interval length in minutes.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/17/06 MAH 8.00.00			Created 
        public override int TestModeIntervalLength
        {
            get
            {
                if (!m_TestModeIntervalLength.Cached)
                {
                    ReadDemandConfiguration();
                }

                return m_TestModeIntervalLength.Value;
            }
        }


        /// <summary>
        /// Returns true if the meter is configured for Canada.
        /// </summary>
        /// <returns>
        /// A bool that indicates if the meter is configured for Canada.
        /// </returns>
        /// <remarks>
        /// Testing Note: The Fulcrum doesn't support sealed Canadian behavior,
        /// so it will always return false.
        ///	</remarks>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/23/06 mcm 7.35.00 N/A    Created
        public override bool IsCanadian
        {
			get
			{
				if( !m_IsCanadian.Cached ) 
				{
					ReadOperatingSetup();
				}
				return m_IsCanadian.Value;
			}
        }

        /// <summary>
        /// Returns the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        /// <returns>
        /// An integer representing the number of load profile channels.
        /// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/05/06 MAH 8.00.00
        override public int NumberLPChannels
        {
            get
            {
                byte[] byNumChannels;
                SCSProtocolResponse Result;

                if (!m_NumberLPChannels.Cached)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading number of load profile intervals");

                    // Upload interval size
                    Result = m_SCSProtocol.Upload(ChannelCountAddress, SCS_CHANNEL_COUNT_SIZE, out byNumChannels);

                    if (SCSProtocolResponse.SCS_ACK == Result)
                    {
                        m_NumberLPChannels.Value = BCD.BCDtoByte(byNumChannels[0]);
                    }
                    else
                    {
                        SCSException scsException = new SCSException(
                                SCSCommands.SCS_U,
                                Result,
                                ChannelCountAddress,
                                "LP Channel Count");

                        throw scsException;
                    }
                }

                return m_NumberLPChannels.Value;
            }
        }

        /// <summary>
        /// This property returns a list of user data strings.  If the meter has 3 user data fields
        /// then the list will contain 3 strings corresponding to each user data field.
        /// </summary>
        /// <returns>
        /// An a list of user data strings.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/16/06 mah 8.00 N/A    Created
        public override List<String> UserData
        {
            get
            {
                if (null == m_UserDataList)
                {
                    m_UserDataList = ReadUserDataList();
                }

                return m_UserDataList;
            }
        }

        /// <summary>This property gets the load profile interval length in 
        /// the meter</summary>
        /// <returns>
        /// An integer representing the load profile interval length in minutes.
        /// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/06 mrj 7.35.00        Created
        //	
        override public int LPIntervalLength
        {
            get
            {
                byte[] byIntervalLength;
                SCSProtocolResponse Result;
                                
                if (!m_LPIntervalLength.Cached)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Interval Size" );

                    // Upload interval size
                    Result = m_SCSProtocol.Upload(IntervalLengthAddress, SCS_INTERVAL_LENGTH_SIZE, out byIntervalLength);
                    
                    if (SCSProtocolResponse.SCS_ACK == Result)
                    {
                        m_LPIntervalLength.Value = BCD.BCDtoByte(byIntervalLength[0]);
                    }
                    else
                    {
                        SCSException scsException = new SCSException(
                                SCSCommands.SCS_U,
                                Result,
                                IntervalLengthAddress,
                                "LP Interval Length");

                        throw scsException;
                    }
                }

                return m_LPIntervalLength.Value;
            }
        }

        /// <summary>This property gets LP running flag in the meter</summary>
        /// <returns>
        /// True if load profile is running
        /// </returns>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/06 mrj 7.35.00        Created
        //	
        override public bool LPRunning
        {
            get
            {
                byte[] byFlag = new byte[1];
                SCSProtocolResponse Result;
                                
                if (!m_LPRunning.Cached)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Checking Load Profile" );
                    				
                    //Upload LP flag
				    Result = m_SCSProtocol.Upload(LoadProfileFlagAddress, SCS_FLAG_LENGTH, out byFlag );
									                    
                    if (SCSProtocolResponse.SCS_ACK == Result)
                    {
                        if ( SCS_FLAG_OFF != byFlag[0] )
					    {
                            //LP is enabled
                            m_LPRunning.Value = true;
                        }
                        else
                        {
                            m_LPRunning.Value = false;
                        }                        
                    }
                    else
                    {
                        SCSException scsException = new SCSException(
                                SCSCommands.SCS_U,
                                Result,
                                LoadProfileFlagAddress,
                                "Load Profile Flag Address (LP Running Flag)");

                        throw scsException;
                    }
                }

                return m_LPRunning.Value;
            }
        }

        /// <summary>
        /// Provides access to the TOU Schedule in the meter
        /// </summary>
        /// <returns>
        /// The TOU schedule programmed in the meter.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/27/06 MAH 8.00.00
        //
        override public CTOUSchedule TimeOfUseSchedule
        {
            get
            {
                if (m_touSchedule == null)
                {
                    // Start off with an empty schedule.  We will fill it in with the data retrieved
                    // from the device.  This is simpler said than do.  Unfortunately there is a lot
                    // of data translation that must take place before we are finished.
                    m_touSchedule = new SCSTOUSchedule();

                    SCSTOUInfo scsTOUInfo;
                    SCSSeasonCollection scsSeasonList = null;
                    SCSTOUEventCollection scsEventList;
                    SCSProtocolResponse readResult;

                    readResult = ReadTOUInfo(out scsTOUInfo);

                    if (readResult == SCSProtocolResponse.SCS_ACK)
                    {
                        // set up the typical week structure
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.SUNDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalSunday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.MONDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalMonday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.TUESDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalTuesday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.WEDNESDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalWednesday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.THURSDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalThursday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.FRIDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalFriday];
                        m_touSchedule.TypicalWeek[(int)eTypicalDay.SATURDAY] = m_touSchedule.NormalDays[scsTOUInfo.TypicalSaturday];

                        // Next read the event list from the meter and add the translated
                        // events to the TOU schedule object
                        ReadTOUEvents(scsTOUInfo, out scsEventList);

                        TranslateSCSEventList(ref scsEventList);
                    }

                    if (readResult == SCSProtocolResponse.SCS_ACK)
                    {
                        readResult = ReadSeasons(scsTOUInfo, out scsSeasonList);
                    }

                    if (readResult == SCSProtocolResponse.SCS_ACK)
                    {
                        int nSeasonIndex = 0;

                        foreach (SCSSeason scsSeason in scsSeasonList)
                        {
                            TranslateSCSSeason(scsSeason, nSeasonIndex);
                            nSeasonIndex++;
                        }
                    }

                    // If we detect any reading errors then we cannot assume that 
                    // the schedule is reliable.  Therefore return an empty schedule
                    if (readResult != SCSProtocolResponse.SCS_ACK)
                    {
                        m_touSchedule = null;
                    }
                }

                return m_touSchedule;
            }
        }

        /// <summary>
        /// Provides access to the DST Dates in the meter
        /// </summary>
        /// <returns>
        /// A list of dst date pairs.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/27/06 MAH 8.00.00
        // 
        override public List<CDSTDatePair> DST
        {
            get
            {
                // Always read the DST dates from the TOU calendar.  Note that since
                // the TOU calendar is a cached object, the DST calendar will be as well

                DateTime dateToDST = DateTime.Now; // Initialize just to have a valid value
                DateTime dateFromDST = DateTime.Now; // Initialize just to have a valid value

                List<CDSTDatePair> listDSTDates = new List<CDSTDatePair>();
                CTOUSchedule localTOUSchedule = TimeOfUseSchedule;

                if (localTOUSchedule != null)
                {
                    foreach (CYear touYear in localTOUSchedule.Years)
                    {
                        Boolean boolToDSTFound = false;
                        Boolean boolFromDSTFound = false;

                        foreach (CEvent touEvent in touYear.Events)
                        {
                            if (touEvent.Type == eEventType.TO_DST)
                            {
                                boolToDSTFound = true;
                                dateToDST = touEvent.Date;
                            }
                            else if (touEvent.Type == eEventType.FROM_DST)
                            {
                                boolFromDSTFound = true;
                                dateFromDST = touEvent.Date;
                            }
                        }

                        // At this point we should have a to and from date for the current year.  Create an 
                        // object to represent the change dates and add them to the collection 

                        if ((boolToDSTFound) && (boolFromDSTFound))
                        {
                            listDSTDates.Add(new CDSTDatePair(dateToDST, dateFromDST)); 
                        }
                    }
                }

                // Return the list of DST dates found in the TOU schedule
                return listDSTDates;
            }
        }

        /// <summary>
        /// Provides access to the meter's time of use schedule ID.
        /// Note that this is returned as a string since one or more
        /// meters allow non-numeric TOU schedule identifiers
        /// </summary>
        /// <returns>
        /// A string representing the time of use schedule identifier
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/14/06 MAH  8.00.00			Created 
        override public String TOUScheduleID
        {
            get
            {
                if (!m_TOUScheduleID.Cached)
                {
                    ReadTOUScheduleID();
                }
                return m_TOUScheduleID.Value;
            }
        }

        /// <summary>
        /// Property used to get the software version from the meter
        /// </summary>
        /// <returns>
        /// A string representing the software version.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        public override String SWRevision
        {
            get
            {
                if (!m_swVersion.Cached)
                {
                    ReadSWVersion();
                }
                return m_swVersion.Value.ToString("#0.00", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Read-only property that identifies the start of the meter's
        /// basepage
        /// </summary>
        /// <returns>
        /// An integer representing the starting address of memory.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        public int  MemoryStart
        {
            get
            {
                return m_memoryStart.Value;
            }
        }

        /// <summary>
        /// Read-only property that returns the ending address in the
        /// meter's basepage
        /// </summary>
        /// <returns>
        /// An integer representing the ending address of memory.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        public int MemoryEnd
        {
            get
            {
                return m_memoryEnd.Value;
            }
        }

		/// <summary>
		/// Property to get the line frequency from the device.
		/// </summary>
        /// <returns>
        /// A float representing the line frequency.
        /// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		public override float LineFrequency
		{
			get
			{
				if (!m_LineFrequency.Cached) 
				{
					ReadOperatingSetup();
				}
				return (float)m_LineFrequency.Value;
			}		
		}

		/// <summary>
		/// Property to get the model type from the device.
		/// </summary>
        /// <returns>
        /// A string representing the model type.
        /// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		public override string ModelType
		{
			get
			{
				if (!m_ModelType.Cached)
				{
					ReadModelType();				
				}
				return m_ModelType.Value;
			}
		}

		/// <summary>
		/// Property to get the transformer ratio from the device.
		/// </summary>
        /// <returns>
        /// A float representing the transformer ratio.
        /// </returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/08/07 mrj 8.00.11		Created
		//  
		public override float TransformerRatio
		{
			get
			{
				if (!m_TranformerRatio.Cached)
				{
					ReadTransformerRatio();
				}
				return m_TranformerRatio.Value;
			}
		}
            
        /// <summary>
        /// This property gets clock running flag in the meter.
        /// </summary>
        /// <returns>
        /// True if the clock is running.
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 03/13/07 mcm 8.00.18 2454   Created
        public override bool ClockRunning
        {
            get
            {
                return ClockEnabled;
            }
        }

        /// <summary>
        /// Property to get the Load Research ID from the device.
        /// </summary>
        /// <returns>
        /// A string representing the Load Research ID.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/13/07 mcm 8.00.18 2454	 Created
        public override string LoadResearchID
        {
            get
            {
                // CQ 193215 removed FULCRUN reference
                //if (!(this is FULCRUM))
                //{
                //    if (!m_LoadResearchID.Cached)
                //    {
                //        ReadLoadResearchID();
                //    }
                //    return m_LoadResearchID.Value;
                //}
                //else
                //{
                    return "";
                //}
            }
        }

        /// <summary>
        /// Determines whether or not the meter is currently in test mode.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/14/07 RCG 8.10.04        Created.

        public override bool IsInTestMode
        {
           get
           {
               SCSProtocolResponse SCSResponse = SCSProtocolResponse.NoResponse;
               byte[] byTestFlag = new byte[1];

               SCSResponse = m_SCSProtocol.Upload(
                   RemoteTestModeFlagAddress,
                   SCS_FLAG_LENGTH,
                   out byTestFlag);

               if (SCSResponse != SCSProtocolResponse.SCS_ACK)
               {
                   throw new SCSException(SCSCommands.SCS_U, SCSResponse, RemoteTestModeFlagAddress, "Error reading Test Mode Flag");
               }

               return SCS_FLAG_ON == byTestFlag[0];
           }
        }

        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// This method reads the program ID from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the program id cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/30/06 jrf 7.30.00  N/A   Revised
        /// 
        virtual internal void ReadProgramID()
        {
            byte[] byProgramID;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Program ID");
            //Get the program ID
            ProtocolResponse =
                m_SCSProtocol.Upload(
                ProgramIDAddress,
                SCS_PROGRAM_ID_LENGTH,
                out byProgramID);

            if (ProtocolResponse == SCSProtocolResponse.SCS_ACK)
            {
                // Convert the bytes read from the SCS device into (int) program id
                m_programID.Value =
                    TranslateProgramID(ref byProgramID, SCS_PROGRAM_ID_LENGTH);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    ProgramIDAddress,
                    m_rmStrings.GetString("PROGRAM_ID"));
                throw scsException;
            }
        }// End ReadProgramID()

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
        virtual internal void ReadTOUScheduleID()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read TOU Schedule ID");

            m_TOUScheduleID.Value = ReadBCDInteger(TOUScheduleIDAddress, 2).ToString(CultureInfo.InvariantCulture);
        }// End ReadTOUScheduleID()

        #endregion Internal Methods

        #region Protected Methods

        /// <summary>
        /// The abstract ReadErrors method which must be implemented by the 
        /// derived class.
        /// </summary>
        /// <param name="strErrors">An array of strings to which the errors 
        /// will be written</param>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/06 jrf 7.30.00  N/A   Created
        /// 
        protected abstract void ReadErrors(out string[] strErrors);

        /// <summary>
        /// Updates DST calendar on the connected SCS device.
        /// </summary>
        /// <returns>A DSTUpdateResult</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 06/13/06 jrf 7.30.00  N/A   Modified
        virtual protected DSTUpdateResult UpdateDSTCalendar()
        {
            DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
            SCSTOUEventCollection TOUEventList = new SCSTOUEventCollection();
            byte[] byClockRunFlag;

            // Determine if meter has a working clock
            if (!m_clockEnabled.Cached)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Checking Clock");
                Result = SCSToDSTResult(m_SCSProtocol.Upload(
                    ClockRunFlagAddress,
                    SCS_FLAG_LENGTH,
                    out byClockRunFlag));
                if (DSTUpdateResult.SUCCESS == Result)
                {
                    m_clockEnabled.Value = (byClockRunFlag[0] != SCS_FLAG_OFF);
                }
            }

            if (m_clockEnabled.Cached && !m_clockEnabled.Value)
            {
                Result = DSTUpdateResult.CLOCK_ERROR;
            }

            // First verify that the meter is actually configured for DST 
            if ((DSTUpdateResult.SUCCESS == Result) && !DSTEnabled)
            {
                Result = DSTUpdateResult.SUCCESS_NOT_CONFIGURED_FOR_DST;
            }

            if (DSTUpdateResult.SUCCESS == Result)
            {
                // Determine if the DST dates have expired
                DateTime TOUExpiration;
                Result = SCSToDSTResult(GetTOUExpiration(out TOUExpiration));
                if (DSTUpdateResult.SUCCESS == Result && DateTime.Now > TOUExpiration)
                {
                    Result = DSTUpdateResult.ERROR_DST_DATES_EXPIRED;
                }
            }

            if (DSTUpdateResult.SUCCESS == Result)
            {
                // Read the DST calendar and update the dates 
                Result = ReadTOUEventList(ref TOUEventList);
            }

            if (DSTUpdateResult.SUCCESS == Result)
            {
                TOUEventList.Sort();

                Result = SCSToDSTResult(SetTOURunFlag(false));
            }

            if (DSTUpdateResult.SUCCESS == Result)
            {
                try
                {
                    Thread.Sleep(1000);

                    Result = PostTOUEvents(ref TOUEventList);

                    if (DSTUpdateResult.SUCCESS == Result)
                    {
                        Result = SCSToDSTResult(SetTOURunFlag(true));
                    }

                    if (DSTUpdateResult.SUCCESS == Result)
                    {
                        Result = SCSToDSTResult(SetTOUReconfigureFlag());
                    }

                    if (DSTUpdateResult.SUCCESS != Result)
                    {
                        Result = DSTUpdateResult.ERROR_RETRY;
                    }
                }
                catch (Exception e)
                {
                    // Try to set the flag since it had to be turned off
                    // if we have gotten to this point.
                    SetTOURunFlag(true);
                    throw (e);
                }
            }

            if (DSTUpdateResult.SUCCESS_PREVIOUSLY_UPDATED == Result)
            {
                if (!TOURunFlag)
                {
                    // We probably failed before we wrote the run flag, try again
                    Result = SCSToDSTResult(SetTOURunFlag(true));

                    if (DSTUpdateResult.SUCCESS == Result)
                    {
                        Result = SCSToDSTResult(SetTOUReconfigureFlag());
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Determines the best DSTUpdateResult base on a given SCSProtocolResponse
        /// </summary>
        /// <param name="protocolResponse">the protocol response to convert</param>
        /// <returns>A DSTUpdateResult</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/13/06 jrf 7.30.00  N/A   Created
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        protected DSTUpdateResult SCSToDSTResult(SCSProtocolResponse protocolResponse)
        {
            DSTUpdateResult Result = DSTUpdateResult.PROTOCOL_ERROR;

            if (SCSProtocolResponse.SCS_ACK == protocolResponse)
            {
                Result = DSTUpdateResult.SUCCESS;
            }
            else if (SCSProtocolResponse.SCS_CAN == protocolResponse)
            {
                Result = DSTUpdateResult.INSUFFICIENT_SECURITY_ERROR;
            }

            return Result;
        }

        /// <summary>
        /// Determines the best ClockAdjustResult base on a given SCSProtocolResponse
        /// </summary>
        /// <param name="protocolResponse">the protocol response to convert</param>
        /// <returns>A ClockAdjustResult</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/20/06 jrf 7.30.00  N/A   Created
        protected ClockAdjustResult SCSToClockAdjustResult(
            SCSProtocolResponse protocolResponse)
        {
            ClockAdjustResult Result = ClockAdjustResult.ERROR;

            if (SCSProtocolResponse.SCS_ACK == protocolResponse)
            {
                Result = ClockAdjustResult.SUCCESS;
            }
            else if (SCSProtocolResponse.SCS_CAN == protocolResponse)
            {
                Result = ClockAdjustResult.SECURITY_ERROR;
            }

            return Result;
        }

        /// <summary>
        /// Writes TOU events to the SCS device.
        /// </summary>
        /// <param name="TOUEventList">The event list which holds the tou events
        /// </param>
        /// <returns>A DSTUpdateResult</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 06/13/06 jrf 7.30.00  N/A   Modified
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to private
        protected virtual DSTUpdateResult PostTOUEvents(
                                        ref SCSTOUEventCollection TOUEventList)
        {
            DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
            int iNextEventAddress = 0;
            int iFullWrites = 0;
            int iWriteOffset = 0;
            int iWriteSize = MaxWriteSize;
            byte[] byTOUEvents = TOUEventList.ByteList;
            byte[] byTOUEventWrite = new byte[MaxWriteSize];
            byte[] byNextEventAddress = new byte[SCS_TOU_CALENDAR_LENGTH];


            iNextEventAddress = TOUCalendarAddress;

            // Take care of all the full writes
            iFullWrites = byTOUEvents.Length / iWriteSize;

            for (int iIndex = 0; (iIndex < iFullWrites) &&
                (DSTUpdateResult.SUCCESS == Result); iIndex++)
            {
                Array.Copy(byTOUEvents, iWriteOffset, byTOUEventWrite, 0, iWriteSize);

                Result = SCSToDSTResult(m_SCSProtocol.Download(
                    iNextEventAddress,
                    iWriteSize,
                    ref byTOUEventWrite));
                iWriteOffset += iWriteSize;
                iNextEventAddress += iWriteSize;
            }

            // Now perform a partial write of remaining events (if necessary)
            iWriteSize = byTOUEvents.Length % iWriteSize;
            if ((0 < iWriteSize) &&
                (DSTUpdateResult.SUCCESS == Result))
            {
                Array.Copy(byTOUEvents, iWriteOffset, byTOUEventWrite, 0, iWriteSize);

                Result = SCSToDSTResult(m_SCSProtocol.Download(
                    iNextEventAddress,
                    iWriteSize,
                    ref byTOUEventWrite));
            }

            return Result;
        }

        /// <summary>
        /// This method sets the passed in DateTime objects to the start of the 
        /// current and next interval.
        /// </summary>
        /// <param name="CurrentTime">The current time of the device</param>
        /// <param name="NextIntStartTime">The object to hold the next 
        /// interval's start time</param>
        /// <param name="CurrentIntStartTime">The object to hold the current 
        /// interval's start time</param>
        /// <param name="iIntervalSize">The size of the load profile 
        /// interval</param>
        /// <returns>A SCSProtocolResponse</returns>
        /// <exception cref="SCSException">	
        /// Thrown when a NAK is received from a SCS protocol request.
        /// </exception>
        /// MM/DD/YY who Version Issue#  Description
        /// -------- --- ------- ------- ---------------------------------------
        /// 05/23/06 jrf 7.30.00  N/A    Created
        /// 06/20/06 jrf 7.30.00  N/A    Modified to pass in the interval length
        /// 06/29/06 jrf 7.30.00  N/A    Modified to correct next interval 
        ///								 computation.
        ///	07/19/06 jrf 7.30.00  SCR 9, Modified to return current interval start
        ///						  10,12  time as well.
        ///				
        ///							
        /// 
        protected void GetCurrentAndNextIntervalStartTimes(DateTime CurrentTime,
            out DateTime NextIntStartTime, out DateTime CurrentIntStartTime,
            int iIntervalSize)
        {
            DateTime TempTime = CurrentTime;
            int iMinute = TempTime.Minute;

            // Zero out the seconds value of the DateTime
            TempTime = TempTime.AddSeconds((double)(-(TempTime.Second)));

            // Back the device time's minute up to the start of the current interval
            iMinute -= TempTime.Minute % iIntervalSize;

            // Zero out the minutes value of the date time
            TempTime = TempTime.AddMinutes((double)(-(TempTime.Minute)));

            // Set the current interval time that will be passed out of method.  
            // Increment it by the minutes at start of current interval to get to 
            // the start of the current interval.
            CurrentIntStartTime = TempTime.AddMinutes((double)(iMinute));

            // Set the next interval time that will be passed out of method.  Increment it 
            // by an interval size + the minutes at start of current interval 
            // to get to the start of the next interval.
            NextIntStartTime = TempTime.AddMinutes((double)(iIntervalSize + iMinute));
        }

        /// <summary>
        /// This method puts the time in a form that can be downloaded to the 
        /// meter.
        /// </summary>
        /// <param name="byTime">The byte array to store the date/time for 
        /// download to the meter</param>
        /// <param name="newTime">The DateTime object that should be used to 
        /// build the date/time in the byte array</param>
        /// <returns>A SCSProtocolResponse</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/31/06 jrf 7.30.00  N/A   Created
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        /// 
        virtual protected void PrepareTime(out byte[] byTime, DateTime newTime)
        {
            byTime = new byte[7];

            // Prepare the new time for download to meter
            byTime[0] = BCD.BytetoBCD((byte)(newTime.Year % 100));
            byTime[1] = BCD.BytetoBCD((byte)(newTime.Month));
            byTime[2] = BCD.BytetoBCD((byte)(newTime.Day));
            byTime[3] = BCD.BytetoBCD((byte)(newTime.Hour));
            byTime[4] = BCD.BytetoBCD((byte)(newTime.Minute));
            byTime[5] = BCD.BytetoBCD((byte)(newTime.Second));
            byTime[6] = BCD.BytetoBCD((byte)(newTime.DayOfWeek + 1));
        }

        /// <summary>
        /// This method can be overriden by SCS Devices which need to stop the 
        /// meter in order to adjust the time.
        /// </summary>
        /// <param name="disableMeter">The boolean to determine if the meter 
        /// needs to be disabled or enabled</param>
        /// <returns>A SCSProtocolResponse</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/01/06 jrf 7.30.00  N/A   Created
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        /// 
        virtual protected SCSProtocolResponse StopMetering(bool disableMeter)
        {
            return SCSProtocolResponse.SCS_ACK;
        }

        /// <summary>
        /// This method performs set up operations that should occur before a 
        /// clock adjustment.  This method can be overriden by those SCS Devices 
        /// which require a different setup routine, ex. the FULCRUM.
        /// </summary>
        /// <param name="massMemoryEnabled">A boolean determining if mass memory is 
        /// enabled.</param>
        /// <returns>ClockAdjustResult indicating the results</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/29/06 jrf 7.30.00  N/A   Created
        virtual protected ClockAdjustResult ClockAdjustSetUp(bool massMemoryEnabled)
        {
            byte[] byFlag = new byte[1];
            ClockAdjustResult Result = ClockAdjustResult.ERROR;

            byFlag[0] = SCS_FLAG_OFF;

            if (massMemoryEnabled)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Turning Off Mass Memory");
                // Clear the Mass Memory Run Flag
                Result = SCSToClockAdjustResult(m_SCSProtocol.Download(
                    LoadProfileFlagAddress,
                    SCS_FLAG_LENGTH,
                    ref byFlag));
            }

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Turning Off Clock");
            // Clear the Clock Option Run Flag
            Result = SCSToClockAdjustResult(m_SCSProtocol.Download(
                StopClockFlagAddress,
                SCS_FLAG_LENGTH,
                ref byFlag));

            return Result;
        }

        /// <summary>
        /// This method sets the appropriate time reconfigure flag that should be set
        /// after a clock adjustment.  This method can be overriden by those SCS Devices 
        /// which require a different routine, ex. the FULCRUM.
        /// </summary>
        /// <param name="massMemoryEnabled">A boolean determining if mass memory is 
        /// enabled.</param>
        /// <returns>ClockAdjustResult indicating the results</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/29/06 jrf 7.30.00  N/A   Created
        virtual protected ClockAdjustResult ClockAdjustSetReconfigureFlag(bool massMemoryEnabled)
        {
            byte[] byFlag = new byte[1];
            ClockAdjustResult Result = ClockAdjustResult.ERROR;

            byFlag[0] = SCS_FLAG_ON;

            if (massMemoryEnabled)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Load Profile Adjust Time Flag");
                // Set the Load Profile Adjust Time Flag
                Result = SCSToClockAdjustResult(m_SCSProtocol.Download(
                    LoadProfileAdjustTimeFlagAddress,
                    SCS_FLAG_LENGTH,
                    ref byFlag));
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Setting Clock Reconfigure Flag");
                // Set the Clock Reconfigure Flag
                Result = SCSToClockAdjustResult(m_SCSProtocol.Download(
                    ClockReconfigureFlagAddress,
                    SCS_FLAG_LENGTH,
                    ref byFlag));
            }

            return Result;
        }

        /// <summary>
        /// This method should be overridden in derived classes.  It will verify
        /// that the derived device type matches the SCS device's type 
        /// </summary>
        /// <returns>
        /// a boolean indicating whether or not the device type is correct
        /// </returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        /// 
        virtual protected bool VerifyDeviceType()
        {
            return true;
        }

        /// <summary>
        /// This method reads the time from the SCS device. 
        /// </summary>
        /// <param name="MeterTime">The object to hold the SCS device's time</param>
        /// <exception cref="SCSException">
        /// Thrown when the time cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        /// 	
        virtual protected void ReadDeviceTime(out DateTime MeterTime)
        {
            // assign a default time just in case the call fails;
            MeterTime = new DateTime();

            byte[] byRealTime;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Meter Time");
            // Get the time from the meter
            SCSProtocolResponse ProtocolResponse = m_SCSProtocol.Upload(
                RealTimeClockAddress,
                SCS_REAL_TIME_LENGTH,
                out byRealTime);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                MeterTime = BCD.GetDateTime(
                    ref byRealTime,
                    BCD.BCDDateTimeFormat.YrMoDaHrMiSeDow);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_D,
                    ProtocolResponse,
                    RealTimeClockAddress,
                    m_rmStrings.GetString("DEVICE_TIME"));
                throw scsException;
            }
        }// End ReadDeviceTime()

        /// <summary>
        /// This method reads the firmware version from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the firmware cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///                             and to use division to form firmware number.
        /// 10/17/06 mrj 7.35.05 44     Fixed conversion for 200 series 
        /// 
        virtual protected void ReadFWVersion()
        {
            byte[] byFWVersion;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Firmware Version");
            // Get the FirmWare version
            ProtocolResponse = m_SCSProtocol.Upload(
                FWVersionAddress,
                SCS_FW_VERSION_LENGTH,
                out byFWVersion);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_fwVersion.Value = (float)(BCD.BCDtoByte(byFWVersion[1]) / 10f);

                //Always divide by ten once to get the correct revision
                m_fwVersion.Value /= 10f;                
                
                while (m_fwVersion.Value > 1.0f)
                {
                    m_fwVersion.Value /= 10f;
                }

                m_fwVersion.Value += BCD.BCDtoByte(byFWVersion[0]);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    FWVersionAddress,
                    m_rmStrings.GetString("FIRMWARE_VERSION"));
                throw scsException;
            }
        }// End ReadFWVersion()

        /// <summary>
        /// This method reads the software version from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the software cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///                             and to use division to form firmware number.
		/// 01/29/08 mrj 9.01.03 3589	Fixed revision.
        /// 
        virtual protected void ReadSWVersion()
        {
            byte[] bySWVersion;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Software Version");
            // Get the software version
            ProtocolResponse = m_SCSProtocol.Upload(
                SWVersionAddress,
                SCS_SW_VERSION_LENGTH,
                out bySWVersion);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_swVersion.Value = (float)(BCD.BCDtoByte(bySWVersion[1]) / 100f);
              
                m_swVersion.Value += BCD.BCDtoByte(bySWVersion[0]);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    SWVersionAddress,
                    m_rmStrings.GetString("SOFTWARE_VERSION"));
                throw scsException;
            }
        } // End ReadSWVersion()

        /// <summary>This method reads the number of minutes that the
        /// meter run on battery power.  
        /// </summary>
		/// <exception cref="SCSException">
		/// Thrown when a protocol error occurs
		/// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/12/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        virtual protected void ReadMinutesOnBattery()
        {
            byte[] byDataBuffer;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Minutes on Battery");

            // Read the data from the meter

            ProtocolResponse = m_SCSProtocol.Upload(
                NumOfMinutesOnBatteryAddress,
                SCS_MINUTES_ON_BATTERY_LENGTH,
                out byDataBuffer);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_numMinOnBattery.Value = (uint)BCD.BCDtoInt(ref byDataBuffer, SCS_MINUTES_ON_BATTERY_LENGTH);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    NumOfMinutesOnBatteryAddress,
                    "Minutes on Battery");

                throw scsException;
            }
        }

        /// <summary>
        /// This method reads the cold load pickup time from the meter
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        virtual protected void ReadCLPU()
        {
            byte[] byDataBuffer;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading cold load pickup time");

            // Read the data from the meter

            ProtocolResponse = m_SCSProtocol.Upload(
                ColdLoadPickupTimeAddress,
                SCS_CLPU_LENGTH,
                out byDataBuffer);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_nCLPU.Value = TranslateCLPU( ref byDataBuffer, SCS_CLPU_LENGTH);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    ColdLoadPickupTimeAddress,
                    "Cold Load Pickup Time");

                throw scsException;
            }
        }

        /// <summary>
        /// This method translates the program ID read from the SCS device. 
        /// </summary>
        /// <remarks>This method is necessary due to the fact that the FULCRUM
        /// stores the program ID differently.</remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/06 jrf 7.30.00  N/A   Created
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
        /// 
        virtual protected int TranslateProgramID(ref byte[] byProgramID, int intLength)
        {
            return BCD.BCDtoInt(ref byProgramID, intLength);
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
        virtual protected uint TranslateCLPU(ref byte[] byCLPU, int intLength)
        {
            return (uint)(SCSConversion.BytetoInt(ref byCLPU, intLength) / 60);
        }

        /// <summary>
        /// This method reads the unit ID from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the unit id cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected 
        ///								and the response conversion.
        ///	07/24/06 jrf 7.30.35 SCR 17 Remove trailing nulls from unit ID.
        ///
        virtual protected void ReadUnitID()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Unit ID");

            m_unitID.Value = ReadASCIIValue(UnitIDAddress, SCS_UNIT_ID_LENGTH);
        } //End ReadUnitID()

        /// <summary>
        /// This method reads the serial number from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the serial number cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected 
        ///                             and the response conversion.
        /// 06/29/06 jrf 7.30.00  N/A   Removed nulls from converted serial number
        /// 07/24/06 jrf 7.30.35 SCR 17 Remove trailing nulls from serial number.
        ///
        virtual protected void ReadSerialNumber()
        {
            byte[] bySerialNumber;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            ASCIIEncoding Encoder;  // For converting byte array to string
            int iTwoPartSerialNumberLength = 18; // Serial Number with two sections
            int iSecondSectionIndex = 9;
            int iSectionLength = 9;
            string strTempString;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Serial Number");
            // Get the serial number
            ProtocolResponse = m_SCSProtocol.Upload(
                SerialNumberAddress,
                SerialNumberLength,
                out bySerialNumber);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                Encoder = new ASCIIEncoding();
                string strNullChar = "\0";
                m_serialNumber.Value = "";

                // Convert the response to a string 
                if (iTwoPartSerialNumberLength == SerialNumberLength)
                {
                    // Get 2nd section of a two part serial number
                    m_serialNumber.Value = Encoder.GetString(bySerialNumber, iSecondSectionIndex, iSectionLength);
                    // jrf - SCR 17
                    // Remove trailing null characters from string
                    m_serialNumber.Value = m_serialNumber.Value.Trim(strNullChar.ToCharArray());

                }

                // Get 1st part of serial number.  It may be the only part.

                // jrf - SCR 17
                // Remove trailing null characters from string
                strTempString = Encoder.GetString(bySerialNumber, 0, iSectionLength);
                strTempString = strTempString.Trim(strNullChar.ToCharArray());
                m_serialNumber.Value = strTempString + m_serialNumber.Value;
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    SerialNumberAddress,
                    m_rmStrings.GetString("SERIAL_NUMBER"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method reads the operating setup from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the operating setup cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected.
		/// 02/08/07 mrj 8.00.11		Added support for getting the line frequncy.
        ///
        virtual protected void ReadOperatingSetup()
        {
            byte[] byOperatingSetup;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Operating Setup");
            // Read the operating set up to check DST configuration
            ProtocolResponse = m_SCSProtocol.Upload(
                OperatingSetupAddress,
                SCS_OPERATING_SETUP_LENGTH,
                out byOperatingSetup);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                //Extract items from the operating setup
                m_dstEnabled.Value = (0 != (byOperatingSetup[0] & DSTMask));
                m_IsCanadian.Value = (0 != (byOperatingSetup[0] & SCS_CANADIAN_MASK));
				
				if( 0 != (byOperatingSetup[0] & SCS_LINE_FREQUENCY_MASK) )
				{
					//Bit is set so this is 60 hertz
					m_LineFrequency.Value = 60;
				}
				else
				{
					//Bit is not set so this is 50 hertz
					m_LineFrequency.Value = 50;
				}
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    OperatingSetupAddress,
                    m_rmStrings.GetString("OPERATING_SETUP"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method reads the Model Type from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the model type cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected 
        ///                             and added in constant for model type mask.
		/// 02/08/07 mrj 8.00.11		Cache a string representation of the model
		///								type.
        ///
        virtual protected void ReadModelType()
        {
            byte[] byModelType;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Model Type");
            // Get the model type
            ProtocolResponse = m_SCSProtocol.Upload(
                ModelTypeAddress,
                SCS_MODEL_TYPE_LENGTH,
                out byModelType);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                // Get the model type from the first three bits
                switch (byModelType[0] & SCS_MODEL_TYPE_MASK)
                {
					case 0x01:
					{
						m_Model = SCSModelTypes.DemandOnly;
						m_ModelType.Value = m_rmStrings.GetString("MODEL_TYPE_D");
						break;
					}
					case 0x02:
					{
						m_Model = SCSModelTypes.DemandTOUModel;
						m_ModelType.Value = m_rmStrings.GetString("MODEL_TYPE_DT");
						break;
					}
					case 0x04:
					{
						m_Model = SCSModelTypes.DemandTOULPModel;
						m_ModelType.Value = m_rmStrings.GetString("MODEL_TYPE_DTL");
						break;
					}
					default:
					{
						m_Model = SCSModelTypes.Unknown;
						m_ModelType.Value = m_rmStrings.GetString("UNKNOWN");
						break;
					}
                }
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    ModelTypeAddress,
                    m_rmStrings.GetString("MODEL_TYPE"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method reads and returns the TOU run flag from the SCS device. 
        /// </summary>
        /// <returns>A bool representing the TOU run flag</returns>
        /// <exception cref="SCSException">
        /// Thrown when the TOU run flag cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///								and added flag off constant
        /// 10/25/06 mcm 7.35.07 113    TOU running an TOU enabled are not the same
        ///
        virtual protected void GetTOURunFlag()
        {
            byte[] byTOURunFlag;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read TOU Run Flag");
            //Get the TOU run flag
            ProtocolResponse = m_SCSProtocol.Upload(
                TOURunFlagAddress,
                SCS_FLAG_LENGTH,
                out byTOURunFlag);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_touRunFlag.Value = (byTOURunFlag[0] != SCS_FLAG_OFF);
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    TOURunFlagAddress,
                    m_rmStrings.GetString("TOU_RUN_FLAG"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method sets the TOU run flag in the SCS device to the passed in 
        /// value. </summary>
        /// <param name="bEnableRunFlag">a bool representing the value to set the 
        /// TOU run flag</param>
        /// <exception cref="SCSException">
        /// Thrown when the TOU run flag cannot be set in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/13/06 jrf 7.30.00  N/A   Added Return of SCSProtocol Response
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///								and added flag on and off constants
        /// 10/25/06 mcm 7.35.07 113    TOU running an TOU enabled are not the same
        ///
        virtual protected SCSProtocolResponse SetTOURunFlag(bool bEnableRunFlag)
        {
            byte[] byTOURunFlag = new byte[1];
            SCSProtocolResponse protocolResponse = SCSProtocolResponse.NoResponse;

            //Set the byte to send
            if (bEnableRunFlag)
                byTOURunFlag[0] = SCS_FLAG_ON;
            else
                byTOURunFlag[0] = SCS_FLAG_OFF;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set TOU Run Flag");
            // Set the TOU run flag
            protocolResponse = m_SCSProtocol.Download(
                TOURunFlagAddress,
                SCS_FLAG_LENGTH,
                ref byTOURunFlag);

            if (SCSProtocolResponse.SCS_ACK == protocolResponse)
            {
                m_touRunFlag.Value = bEnableRunFlag;
            }

            return protocolResponse;
        }

        /// <summary>
        /// This method sets the TOU reconfigure flag on in the SCS device. 
        /// </summary>
        /// <returns>a SCSProtocolResponse</returns>
        /// <exception cref="SCSException">
        /// Thrown when the TOU reconfigure flag cannot be set in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///								and added flag on constant
        ///
        virtual protected SCSProtocolResponse SetTOUReconfigureFlag()
        {
            byte[] byTOUFlag = new byte[1];
            SCSProtocolResponse protocolResponse = SCSProtocolResponse.NoResponse;

            byTOUFlag[0] = SCS_FLAG_ON;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set TOU Reconfigure Flag");
            // Set the TOU reconfigure flag
            protocolResponse = m_SCSProtocol.Download(
                TOUReconfigureFlagAddress,
                SCS_FLAG_LENGTH,
                ref byTOUFlag);

            return protocolResponse;
        }

        /// <summary>
        /// This method sets the clock run flag in the SCS device to the passed 
        /// in value. 
        /// </summary>
        /// <param name="bEnableRunFlag">a bool representing the value to set the 
        /// TOU run flag</param>
        /// <exception cref="SCSException">
        /// Thrown when the clock run flag cannot be set in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///								and added flag on and off constants
        ///
        virtual protected void SetClockRunFlag(bool bEnableRunFlag)
        {
            byte[] byRunFlag = new byte[1];
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            //Set the byte to send
            if (bEnableRunFlag)
                byRunFlag[0] = SCS_FLAG_ON;
            else
                byRunFlag[0] = SCS_FLAG_OFF;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Clock Run Flag");
            //Set the clock run flag
            ProtocolResponse = m_SCSProtocol.Download(
                StopClockFlagAddress,
                SCS_FLAG_LENGTH,
                ref byRunFlag);

            if (SCSProtocolResponse.SCS_ACK != ProtocolResponse)
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_D,
                    ProtocolResponse,
                    StopClockFlagAddress,
                    m_rmStrings.GetString("CLOCK_RUN_FLAG"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method sets the Clock reconfigure flag on in the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the clock reconfigure flag cannot be set in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Rewrote
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///								and added flag on constant
        ///
        virtual protected void SetClockReconfigureFlag()
        {
            byte[] byClockFlag = new byte[1];
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            byClockFlag[0] = SCS_FLAG_ON;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Clock Reconfigure Flag");
            //Set the clock reconfigure flag
            ProtocolResponse = m_SCSProtocol.Download(
                ClockReconfigureFlagAddress,
                SCS_FLAG_LENGTH,
                ref byClockFlag);

            if (SCSProtocolResponse.SCS_ACK != ProtocolResponse)
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_D,
                    ProtocolResponse,
                    ClockReconfigureFlagAddress,
                    m_rmStrings.GetString("CLOCK_RECONFIGURE_FLAG"));
                throw scsException;
            }
        }

        /// <summary>
        /// This method reads and caches the user data fields from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the user data fields cannot be retreived from the meter.
        /// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/16/06 mah 8.00.00  N/A   Created
        /// </remarks>
        virtual protected List<String> ReadUserDataList()
        {
            byte[] byUserData;
            List<String> localUserDataList = null;
            ASCIIEncoding Encoder;  // For converting byte array to string	
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read User Data List");
            
            // Note that we are going to take advantage of the fact that the user data fields
            // are physically next to each other.  This way we can get the values of all the
            // fields with a single read rather than multiple reads. Remember - faster is better
            ProtocolResponse = m_SCSProtocol.Upload(
                UserDataBlockAddress,
                SCS_USERDATABLOCK_LENGTH,
                out byUserData);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                Encoder = new ASCIIEncoding();
                localUserDataList = new List<String>();

                // All SCS devices support three user data fields.  We will extract each
                // one, convert it to a string, and then add it to the user data list
                localUserDataList.Add(Encoder.GetString(byUserData, 0, SCS_USERDATAFIELD_LENGTH));
                localUserDataList.Add(Encoder.GetString(byUserData, SCS_USERDATAFIELD_LENGTH, SCS_USERDATAFIELD_LENGTH));
                localUserDataList.Add(Encoder.GetString(byUserData, (SCS_USERDATAFIELD_LENGTH * 2), SCS_USERDATAFIELD_LENGTH));
            }
            else
            {
                SCSException scsException = new SCSException(
                    SCSCommands.SCS_U,
                    ProtocolResponse,
                    SWVersionAddress,
                    m_rmStrings.GetString("User Data List"));
                throw scsException;
            }

            return localUserDataList;

        } // End ReadUserDataList()

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
        /// 11/14/08 mah 9.50.22        Corrected CQ#121179. The nibbles for test mode 
        /// and normal mode interval lengths were switched.
        /// </remarks>
        virtual protected void ReadDemandConfiguration()
        {
            byte[] byDemandConfiguration;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Demand Configuration");

            // Note that we are going to take advantage of the fact that the user data fields
            // are physically next to each other.  This way we can get the values of all the
            // fields with a single read rather than multiple reads. Remember - faster is better
            ProtocolResponse = m_SCSProtocol.Upload(
                DemandConfigurationAddress,
                SCS_DEMANDCONFIGURATIONBLOCK_LENGTH,
                out byDemandConfiguration);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                // The first byte of the demand configuration block holds the number of sub intervals
                // The lower nibble contains the number of subintervals for normal mode while 
                // the upper nibble contains the number of subintervals for test mode

                m_NumberOfTestModeSubIntervals.Value = byDemandConfiguration[0] >> 4;
				m_NumberOfSubIntervals.Value = byDemandConfiguration[0] & 0x0F;

                // The second byte holds the normal mode subinterval length

                m_DemandIntervalLength.Value = (int)BCD.BCDtoByte(byDemandConfiguration[1]) * m_NumberOfSubIntervals.Value;

                // The third byte holds the test mode subinterval length

                m_TestModeIntervalLength.Value = (int)BCD.BCDtoByte(byDemandConfiguration[2]) * m_NumberOfTestModeSubIntervals.Value;
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
        /// This method gets the TOU expiration date. 
        /// </summary>
        /// <param name="touExpiration">a bool representing the value to set the 
        /// TOU run flag</param>
        /// <exception cref="SCSException">
        /// Thrown when the clock reconfigure flag cannot be set in the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// 05/23/06 jrf 7.30.00  N/A   Revised
        /// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to protected
        ///
        virtual protected SCSProtocolResponse GetTOUExpiration(out DateTime touExpiration)
        {
            byte[] byTOUExpiration;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            int iYear;
            int iMonth;
            int iDay;
            string strDate;

            touExpiration = DateTime.MinValue;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read TOU Exiration Date");
            // Set the TOU expiration date
            ProtocolResponse = m_SCSProtocol.Upload(
                TOUExpirationAddress,
                SCS_TOU_EXPIRATION_LENGTH,
                out byTOUExpiration);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {

                strDate = byTOUExpiration[0].ToString("X2", CultureInfo.InvariantCulture) +
                    byTOUExpiration[1].ToString("X2", CultureInfo.InvariantCulture);
                iYear = Convert.ToInt32(strDate, 10);
                strDate = byTOUExpiration[2].ToString("X2", CultureInfo.InvariantCulture);
                iMonth = Convert.ToInt32(strDate, 10);
                strDate = byTOUExpiration[3].ToString("X2", CultureInfo.InvariantCulture);
                iDay = Convert.ToInt32(strDate, 10);

                // TOU expriation date bytes [ yr1 yr2 mo da ] 
                touExpiration = new DateTime(iYear, iMonth, iDay);
            }

            return ProtocolResponse;


        }
        
        /// <summary>Reads the block of TOU Info Items</summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mcm 7.30.00 N/A	Created
        /// 
        protected virtual SCSProtocolResponse ReadTOUInfo(out SCSTOUInfo TOUInfo)
        {
            SCSProtocolResponse Result;
            byte[] Data;

            Result = m_SCSProtocol.Upload(YearlyScheduleAddress,
                                           SCSTOUInfo.SIZE_OF_TOU_INFO,
                                           out Data);

            if (SCSProtocolResponse.SCS_ACK == Result)
            {
                TOUInfo = new SCSTOUInfo(Data);
            }
            else
            {
                TOUInfo = new SCSTOUInfo();
            }

            return Result;

        } // ReadTOUInfo

        /// <summary>Writes the block of TOU Info Items</summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mcm 7.30.00 N/A	Created
        /// 
        protected virtual SCSProtocolResponse WriteTOUInfo(SCSTOUInfo TOUInfo)
        {
            byte[] Data = TOUInfo.Data;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "Writing TOU Info block");

            return m_SCSProtocol.Download(YearlyScheduleAddress,
                                           SCSTOUInfo.SIZE_OF_TOU_INFO,
                                           ref Data);

        } // WriteTOUInfo

        /// <summary>Finds the first season change event in the TOU calendar
        /// and returns true if it is configured for a demand reset. When we 
        /// reconfigure TOU we'll want to configure it the same way, because
        /// this info is not in the TOU file.</summary>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/07/06 mcm 7.30.00 N/A	Created
        /// 
        protected SCSProtocolResponse ResetDemandOnSeasonChange(out bool ResetDemand)
        {
            SCSProtocolResponse Result = SCSProtocolResponse.SCS_ACK;
            byte[] Data;
            SCSTOUEvent Event;
            int EventAddr = TOUCalendarAddress;


            ResetDemand = false;

            while ((SCSProtocolResponse.SCS_ACK == Result) &&
                  (EventAddr < TOUConfigLastAvailAddress))
            {
                Result = m_SCSProtocol.Upload(EventAddr, 2, out Data);
                EventAddr = EventAddr + 2;

                if (SCSProtocolResponse.SCS_ACK == Result)
                {
                    Event = new SCSTOUEvent((short)(Data[0] * 0x100 + Data[1]));

                    if (SCSTOUEvent.EventTypes.SeasonSelect == Event.Type)
                    {
                        ResetDemand = Event.DemandReset;
                        break;
                    }
                    else if (SCSTOUEvent.EventTypes.CalendarEnd == Event.Type)
                    {
                        // shouldn't come here
                        break;
                    }
                }
            }

            return Result;

        } // ResetDemandOnSeasonChange

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
        /// 09/13/06 mcm 7.35.00 N/A    DailySchedOffset not set correctly
        /// 
        protected virtual void ReconfigureTOUInfo(CTOUSchedule TOUSchedule,
            SCSTOUInfo TOUInfo, SCSTOUEventCollection TOUEventList,
            SCSSeasonCollection TOUSeasons)
        {
            TOUInfo.ExpirationDate = TOUEventList.ExpirationDate;
            TOUInfo.ScheduleID = (ushort)TOUSchedule.TOUID;
            TOUInfo.StartOfDailySchedule = (ushort)(TOUInfo.StartOfYearlySchedule +
                                                    TOUEventList.Size);
            TOUInfo.StartOfSeason0 = TOUInfo.StartOfDailySchedule;
            TOUInfo.StartOfSeason1 = (ushort)(TOUInfo.StartOfSeason0 +
                                               TOUSeasons.Season0Size);
            TOUInfo.StartOfSeason2 = (ushort)(TOUInfo.StartOfSeason1 +
                                               TOUSeasons.Season1Size);
            TOUInfo.StartOfSeason3 = (ushort)(TOUInfo.StartOfSeason2 +
                                               TOUSeasons.Season2Size);
            TOUInfo.StartOfSeason4 = (ushort)(TOUInfo.StartOfSeason3 +
                                               TOUSeasons.Season3Size);
            TOUInfo.StartOfSeason5 = (ushort)(TOUInfo.StartOfSeason4 +
                                               TOUSeasons.Season4Size);
            TOUInfo.StartOfSeason6 = (ushort)(TOUInfo.StartOfSeason5 +
                                               TOUSeasons.Season5Size);
            TOUInfo.StartOfSeason7 = (ushort)(TOUInfo.StartOfSeason6 +
                                               TOUSeasons.Season6Size);

            TOUInfo.TypicalMonday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.MONDAY);
            TOUInfo.TypicalTuesday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.TUESDAY);
            TOUInfo.TypicalWednesday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.WEDNESDAY);
            TOUInfo.TypicalThursday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.THURSDAY);
            TOUInfo.TypicalFriday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.FRIDAY);
            TOUInfo.TypicalSaturday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.SATURDAY);
            TOUInfo.TypicalSunday = GetDaytypeIndex(TOUSchedule,
                                                     TOU.eTypicalDay.SUNDAY);

        } // ReconfigureTOUInfo

        /// <summary>Returns the daytype index for the give day</summary>
        /// <param name="TOUSchedule">TOU Server with TOU file open.</param>
        /// <param name="Day">The day you want the daytype index assigned to</param>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/06 mcm 7.30.00 N/A	Created
        /// 
        protected byte GetDaytypeIndex(CTOUSchedule TOUSchedule,
            TOU.eTypicalDay Day)
        {
            byte Index;
            TOU.CDayType Daytype;
            string[] TypicalWeek;

            // Jump through hoops to set the typical week
            TypicalWeek = TOUSchedule.TypicalWeek;

            Daytype = TOUSchedule.GetDayType(TypicalWeek[(int)Day]);

            if (TOU.eDayType.NORMAL == Daytype.Type)
            {
                Index = (byte)Daytype.Index;
            }
            else
            {
                Index = HOLIDAY_TYPE_INDEX;
            }

            return Index;
        }

        /// <summary>
        /// Turn off the TOU run flag, writes all of the configuration data to
        /// the meter, and turns the flag back on. NOTE that this is overridden
        /// by the FULCRUM because it's so special.
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
        protected virtual TOUReconfigResult WriteTOU(SCSTOUInfo TOUInfo,
            SCSTOUEventCollection TOUEventList, SCSSeasonCollection TOUSeasons)
        {
            TOUReconfigResult Result = TOUReconfigResult.SUCCESS;


            if (SCSProtocolResponse.SCS_CAN == SetTOURunFlag(false))
            {
                // If the meter rejected our request, we probably don't
                // have sufficient security clearance.
                Result = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
            }

            if (TOUReconfigResult.SUCCESS == Result)
            {
                try
                {
                    Thread.Sleep(1000);

                    // Write the Info block first, if it succeeds, write
                    // the calendar.
                    if (SCSProtocolResponse.SCS_ACK ==
                        WriteTOUInfo(TOUInfo))
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Writing TOU Event Calendar block");

                        Result = (TOUReconfigResult)
                            PostTOUEvents(ref TOUEventList);
                    }
                    else
                    {
                        Result = TOUReconfigResult.IO_TIMEOUT;
                    }

                    if (TOUReconfigResult.SUCCESS == Result)
                    {
                        byte[] SeasonData = TOUSeasons.Data;

                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Writing TOU Seasons block");

                        if (SCSProtocolResponse.SCS_ACK !=
                            m_SCSProtocol.Download(
                            TOUInfo.StartOfYearlySchedule + TOUEventList.Size,
                            SeasonData.Length, ref SeasonData))
                        {
                            Result = TOUReconfigResult.IO_TIMEOUT;
                        }
                    }

                    if (TOUReconfigResult.SUCCESS == Result)
                    {
                        Result = (TOUReconfigResult)
                            SCSToDSTResult(SetTOURunFlag(true));
                    }

                    if (TOUReconfigResult.SUCCESS == Result)
                    {
                        Result = (TOUReconfigResult)
                            SCSToDSTResult(SetTOUReconfigureFlag());
                    }

                    // If anything went wrong since we stopped the TOU flag,
                    // tell the user to try again.
                    if (TOUReconfigResult.SUCCESS != Result)
                    {
                        Result = TOUReconfigResult.ERROR_RETRY;
                    }
                }
                catch (Exception e)
                {
                    // Try to set the flag since it had to be turned off
                    // if we have gotten to this point.
                    SetTOURunFlag(true);
                    throw (e);
                }
            }

            return Result;

        } // WriteTOU

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
        protected virtual SCSProtocolResponse ForceCopyData()
        {
            return SCSProtocolResponse.SCS_ACK;

        } // SCSProtocolResponse

        /// <summary>
        /// This method reads the number of power outages from the SCS device. 
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the number of outages cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        virtual protected int ReadEventCounter(int nAddress, string strFieldName)
        {
            byte[] byDataBuffer;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading " + strFieldName);


            // Read the data byte from the meter

            ProtocolResponse = m_SCSProtocol.Upload(
                nAddress,
                SCS_EVENT_COUNTER_LENGTH,
                out byDataBuffer);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                return TranslateEventCounter(ref byDataBuffer, SCS_EVENT_COUNTER_LENGTH);
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

        }// End ReadEventCounter()

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
        virtual protected int TranslateEventCounter(ref byte[] byInputBuffer, int intLength)
        {
            return BCD.BCDtoInt(ref byInputBuffer, intLength);
        }

		/// <summary>
		/// This method resets a 2 byte BCD event counter in an SCS device. 
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the counter cannot be written to the meter.
		/// </exception>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 01/29/07 mah 8.00.09  N/A   Created
		/// </remarks>
		virtual protected SCSProtocolResponse ResetEventCounter(int nAddress, string strFieldName)
		{
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Resetting " + strFieldName);

			byte[] byDataBuffer = BCD.InttoBCD(0, 2);

			// Read the data byte from the meter

			return m_SCSProtocol.Download( nAddress,
				SCS_EVENT_COUNTER_LENGTH,
				ref byDataBuffer);

		}// End ReadEventCounter()


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
        virtual protected DateTime ReadLastResetDate()
        {
            return ReadShortDateTime(LastResetDateAddress, "Last Reset Date");
        }

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
        virtual protected void ReadDateLastProgrammed()
        {
            m_dateLastProgrammed.Value = ReadShortDateTime(LastProgrammedDateAddress, "Last Programmed Date");
        }

        /// <summary>
        /// This method reads a 4 byte time stamp from the meter.  The timestamp
        /// is in the M-D-H-M format.
        /// </summary>
        /// <exception cref="SCSException">
        /// Thrown when the date cannot be retreived from the meter.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        /// 
        protected DateTime ReadShortDateTime(int nAddress, string strFieldName)
        {
            DateTime dtMeterTimeStamp;
            byte[] byDataBuffer;
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading " + strFieldName);

            // Read the data byte from the meter

            ProtocolResponse = m_SCSProtocol.Upload(
                nAddress,
                SCS_SHORT_DATE_TIME_LENGTH,
                out byDataBuffer);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                int nMonth = BCD.BCDtoByte(byDataBuffer[0]);
                int nDay = BCD.BCDtoByte(byDataBuffer[1]);

                if ((0 < nMonth) && (0 < nDay)) // we have a valid year
                {
                    // Note that the device does not keep track of the year for this
                    // timestamp.  Therefore use the current year as the default

                    dtMeterTimeStamp = new DateTime(DateTime.Now.Year,
                                            nMonth, // Month
                                            nDay, // Day
                                            BCD.BCDtoByte(byDataBuffer[2]), // Hour
                                            BCD.BCDtoByte(byDataBuffer[3]), // minute
                                            0); // Second

                    // But we know that if we end up with a date in the future, the 
                    // year has to be wrong - so correct it...

                    if (DateTime.Now < dtMeterTimeStamp)
                    {
                        dtMeterTimeStamp = dtMeterTimeStamp.AddYears(-1);
                    }
                }
                else // The meter's clock is not running or the event has not occurred
                {
                    dtMeterTimeStamp = new DateTime(1980, 1, 1);
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
		protected virtual void ReadTransformerRatio()
		{
			byte[] byTransformerRatio;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Transformer Ratio");			
			ProtocolResponse = m_SCSProtocol.Upload(TransformerRatioAddress,
												    SCS_TRANSFORMER_RATIO_LENGTH,
													out byTransformerRatio);

			if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
			{
				int iTransRatio = BCD.BCDtoInt(ref byTransformerRatio, SCS_TRANSFORMER_RATIO_LENGTH);
				m_TranformerRatio.Value = (float)iTransRatio;
			}
			else
			{
				SCSException scsException = new SCSException(SCSCommands.SCS_U,
															 ProtocolResponse,
															 TransformerRatioAddress,
															 "Transformer Ratio");
				throw scsException;
			}
		}

		/// <summary>
		/// This method reads the LoadResearchID from the SCS device. 
		/// </summary>
		/// <exception cref="SCSException">
		/// Thrown when the transformer ratio cannot be retreived from the meter.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  03/13/07 mcm 8.00.18		Created
		//  
		protected virtual void ReadLoadResearchID()
		{
			byte[] byLoadResearchID;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read Load Research ID");			
			ProtocolResponse = m_SCSProtocol.Upload(LoadResearchIDAddress,
												    SCS_LOAD_RESEARCH_ID_LENGTH,
													out byLoadResearchID);

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                m_LoadResearchID.Value = "";
                for (int i = 0; i < byLoadResearchID.Length; i++)
                {
                    if (0 == byLoadResearchID[i])
                    {
                        break;
                    }
                    else
                    {
                        m_LoadResearchID.Value += (char)byLoadResearchID[i];
                    }
                }
            }
            else
            {
                SCSException scsException = new SCSException(SCSCommands.SCS_U,
                                                             ProtocolResponse,
                                                             LoadResearchIDAddress,
                                                             "Load Research ID");
                throw scsException;
            }
		}

        #endregion Protected Methods

        #region Protected Properties

        /// <summary>This property gets the expected device type.</summary>
        /// <returns>
        /// An string representing the expected device type.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract string ExpectedDeviceType
        {
            get;
        }

        /// <summary>This property returns the address of the number of power outages.</summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int NumOutagesAddress
        {
            get;
        }

        /// <summary>This property returns the address of the number of demand resets.</summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        protected abstract int NumResetsAddress
        {
            get;
        }

        /// <summary>This property returns the address of the number of demand resets.</summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int LastResetDateAddress
        {
            get;
        }

        /// <summary>This property returns the address of the number of times a device
        /// was programmed.</summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int NumTimesProgrammedAddress
        {
            get;
        }

        /// <summary>This property returns the address of the number of minutes the 
        /// device was on battery power.
        /// </summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------  
        /// 09/08/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int NumOfMinutesOnBatteryAddress
        {
            get;
        }

        /// <summary>This property returns the address of the date/time when the device
        /// was programmed.</summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int LastProgrammedDateAddress
        {
            get;
        }

        /// <summary>This property gets the hang up flag address.</summary>
        /// <returns>
        /// An int representing the hang up flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int CommunicationsHangUpFlagAddress
        {
            get;
        }

        /// <summary>This property gets the demand reset flag address.</summary>
        /// <returns>
        /// An int representing the demand reset flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int DemandResetFlagAddress
        {
            get;
        }

        /// <summary>This property gets the clear billing data flag address.</summary>
        /// <returns>
        /// An int representing the clear billing data flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/15/06 mah 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int ClearBillingDataFlagAddress
        {
            get;
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
        protected abstract int RemoteTestModeFlagAddress
        {
            get;
        }

        /// <summary>This property gets the stop clock flag address.</summary>
        /// <returns>
        /// An int representing the stop clock flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int StopClockFlagAddress
        {
            get;
        }

        /// <summary>This property gets the TOU run flag address.</summary>
        /// <returns>
        /// An int representing the TOU run flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int TOURunFlagAddress
        {
            get;
        }

        /// <summary>This property gets the CLPU Time address.</summary>
        /// <returns>
        /// An int representing the CLPU Time address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int ColdLoadPickupTimeAddress
        {
            get;
        }

        /// <summary>This property gets the clock run flag address.</summary>
        /// <returns>
        /// An int representing the clock run flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/13/06 jrf 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int ClockRunFlagAddress
        {
            get;
        }

        /// <summary>This property gets the TOU reconfigure flag address.
        /// </summary>
        /// <returns>
        /// An int representing the TOU reconfigure flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int TOUReconfigureFlagAddress
        {
            get;
        }

        /// <summary>This property gets the real time clock address.</summary>
        /// <returns>
        /// An int representing the real time clock address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        protected abstract int RealTimeClockAddress
        {
            get;
        }

        /// <summary>This property gets the model type address.</summary>
        /// <returns>
        /// An int representing the model type address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int ModelTypeAddress
        {
            get;
        }

        /// <summary>This property gets the FW version address.</summary>
        /// <returns>
        /// An int representing the FW version address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int FWVersionAddress
        {
            get;
        }

        /// <summary>This property gets the SW version address.</summary>
        /// <returns>
        /// An int representing the SW version address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int SWVersionAddress
        {
            get;
        }

        /// <summary>This property gets the program ID address.</summary>
        /// <returns>
        /// An int representing the program ID address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int ProgramIDAddress
        {
            get;
        }

        /// <summary>This property gets the unit ID address.</summary>
        /// <returns>
        /// An int representing the unit ID address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int UnitIDAddress
        {
            get;
        }

        /// <summary>This property gets the TOU address.</summary>
        /// <returns>
        /// An int representing the TOU address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int TOUCalendarAddress
        {
            get;
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
        protected abstract int TOUScheduleIDAddress
        {
            get;
        }

        /// <summary>This property gets the operating setup address.</summary>
        /// <returns>
        /// An int representing the operating setup address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int OperatingSetupAddress
        {
            get;
        }

        /// <summary>This property gets the clock reconfigure flag address.
        /// </summary>
        /// <returns>
        /// An int representing the clock reconfigure flag address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/22/06 mrj 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int ClockReconfigureFlagAddress
        {
            get;
        }

        /// <summary>This property gets the communication timeout address.
        /// </summary>
        /// <returns>
        /// An int representing the communication timeout address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int CommunicationTimeoutAddress
        {
            get;
        }

        /// <summary>This property gets the inteval length address.</summary>
        /// <returns>
        /// An int representing the inteval length address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int IntervalLengthAddress
        {
            get;
        }

        /// <summary>
        /// This property gets the load profile run flag address.
        /// </summary>
        /// <returns>
        /// An int representing the load profile run flag.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/31/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int LoadProfileFlagAddress
        {
            get;
        }

        /// <summary>
        /// This property gets the load profile run flag address.
        /// </summary>
        /// <returns>
        /// An int representing the load profile run flag.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/31/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int LoadProfileAdjustTimeFlagAddress
        {
            get;
        }

        /// <summary>
        /// This property gets the tou expiration address.
        /// </summary>
        /// <returns>
        /// An int representing the tou expiration address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/12/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int TOUExpirationAddress
        {
            get;
        }

        /// <summary>This property gets the serial number address.</summary>
        /// <returns>
        /// An int representing the serial number address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int SerialNumberAddress
        {
            get;
        }

        /// <summary>This property gets the serial number length.</summary>
        /// <returns>
        /// An int representing the serial number length.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/23/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int SerialNumberLength
        {
            get;
        }

        /// <summary>
        /// Returns the address of the primary security code.
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/16/06 mcm 7.35.00  N/A   Created
        /// </remarks>
        /// 
        protected abstract int PrimaryPasswordAddress
        {
            get;
        }

        /// <summary>
        /// Returns the address of the Secondary security code.</summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int SecondaryPasswordAddress
        {
            get;
        }

        /// <summary>Returns the address of the Tertiary security code.</summary>
        /// 
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int TertiaryPasswordAddress
        {
            get;
        }

        /// <summary>Returns the address of the Modem security code. Only 
        /// supported by the </summary>
        /// 
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int ModemPasswordAddress
        {
            get
            {
                return UNSUPPORTED_ADDRESS;
            }
        }

        /// <summary>This property gets the Firmware Options address.</summary>
        /// <returns>
        /// An int representing the Firmware Options address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int FirmwareOptionsAddress
        {
            get
            {
                return 0;
            }
        }

        /// <summary>This property gets the address of the first user data field.</summary>
        /// <returns>
        /// An int representing the first user data field address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/16/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int UserDataBlockAddress
        {
            get;
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
        protected abstract int DisplayTableAddress
        {
            get;
        }


        /// <summary>
        /// This property gets the address of the first byte of demand configuration data
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/17/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected abstract int DemandConfigurationAddress
        {
            get;
        }

        /// <summary>This property gets the DST mask for an SCS device.</summary>
        /// <returns>
        /// An byte representing the DST mask.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/06 jrf 7.30.00  N/A   Created
        /// </remarks>
        /// 
        protected virtual byte DSTMask
        {
            get
            {
                return SCS_DST_MASK;
            }
        }

        /// <summary>This property gets the maximum number of TOU calendar 
        /// events to read from an SCS device.</summary>
        /// <returns>
        /// An int representing the number of events.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int MaxReadEvents
        {
            get
            {
                return SCS_DEFAULT_MAX_READ_EVENTS;
            }
        }

        /// <summary>This property gets the maximum number of TOU calendar 
        /// events to read from an SCS device.</summary>
        /// <returns>
        /// An int representing the number of events.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/06 jrf 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int MaxWriteSize
        {
            get
            {
                return SCS_DEFAULT_MAX_WRITE_SIZE;
            }
        }

        /// <summary>This property gets the TOU Yearly Schedule address.
        /// Not all have this item in their base page.</summary>
        /// <returns>
        /// An int representing the TOU address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/06/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int YearlyScheduleAddress
        {
            get
            {
                return 0;
            }
        }

        /// <summary>This property gets the Last Season data address.
        /// Not all have this item in their base page.</summary>
        /// <returns>
        /// An int representing the TOU address.
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int LastSeasonDataAddress
        {
            get
            {
                return 0;
            }
        }

        /// <summary>This property the address of the last available byte that
        /// can be used for TOU configuration. This value is meter, firmware, 
        /// configuration dependent.</summary>
        /// <returns>An int representing the address of the last byte that can
        /// be used by the TOU configuration.</returns>
        /// 
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/10/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int TOUConfigLastAvailAddress
        {
            get
            {
                if (SeasonChangeEnabled)
                {
                    return LastSeasonDataAddress - 1;
                }
                else
                {
                    return YearlyScheduleAddress - 1;
                }
            }
        }

        /// <summary>Does this meter have a separate modem password?  Only the
        /// VECTRON returns true.</summary>
        /// 
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual bool HasModemPassword
        {
            get
            {
                return false;
            }
        }

        /// <summary>Does this meter have a tertiary password?  Only the MTR
        /// 200 meter returns false.</summary>
        /// 
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.30.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual bool HasTertiaryPassword
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// This property returns the address of the number of load profile channels
        /// Note that this is not an abstract property since not all SCS devices
        /// support this basepage field
        /// </summary>
        /// <returns>
        /// An int representing a basepage address or 0 if the basepage 
        /// field does not exist (MT200 and CENTRON only)
        /// </returns>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 12/05/06 mah 8.00.00  N/A   Created
        ///	</remarks>
        ///	
        protected virtual int ChannelCountAddress
        {
            get
            {
                return 0; 
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
		protected abstract int TransformerRatioAddress
		{
			get;
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
		protected abstract int LoadResearchIDAddress
		{
			get;
		}

        #endregion Protected Properties

        #region Private Methods

        /// <summary>
		/// This method initializes the instance variables for the constructors.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/16/06 jrf 7.30.00  N/A   Created
		///
		private void InitializeInstanceVariables()
		{
			m_iTOUBaseAddress = 0;
			m_strDSTFile = "";
			m_Model = SCSModelTypes.Unknown;	
			m_serialNumber = new CachedString();
			m_unitID = new CachedString();
			m_deviceType = new CachedString();
			m_memoryStart = new CachedInt();
			m_memoryEnd = new CachedInt();
			m_programID = new CachedInt();
			m_touCalendarStartAddress = new CachedInt();
			m_fwVersion = new CachedFloat();
			m_swVersion = new CachedFloat();
			m_dstEnabled = new CachedBool();
            m_IsCanadian = new CachedBool();
			m_touEnabled = new CachedBool();
            m_touRunFlag = new CachedBool();
			m_touExpired = new CachedBool();
			m_clockEnabled = new CachedBool();
			m_FirmwareOptions = new CachedByte();
            m_dateLastProgrammed = new CachedDate();
            m_dateTOUExpiration = new CachedDate();
            m_numOutages = new CachedInt();
            m_numTimesProgrammed = new CachedInt();
            m_numMinOnBattery = new CachedUint();
            m_nCLPU  = new CachedUint();
            m_LPIntervalLength = new CachedInt();
            m_LPRunning = new CachedBool();
            m_NumberLPChannels = new CachedInt();
            m_DemandIntervalLength = new CachedInt();
            m_NumberOfSubIntervals = new CachedInt();
            m_TestModeIntervalLength = new CachedInt();
            m_NumberOfTestModeSubIntervals = new CachedInt();
            m_EnergyFormat = new SCSDisplayFormat();
            m_DemandFormat = new SCSDisplayFormat();
            m_CumulativeFormat = new SCSDisplayFormat();
            m_TOUScheduleID = new CachedString();
			m_LineFrequency = new CachedInt();
			m_ModelType = new CachedString();
			m_TranformerRatio = new CachedFloat();
            m_LoadResearchID = new CachedString();

            m_touSchedule = null;
			m_dstSchedule = null;
			m_Logger = Logger.TheInstance;
			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, 
				this.GetType().Assembly );
		}
        

		/// <summary>
		/// Reads TOU events from SCS device and update DST dates internally.
		/// </summary>
		/// <param name="TOUEventList">The event list to store the tou events
		/// </param>
		/// <returns>A DSTUpdateResult</returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00  N/A   Created
		/// 06/13/06 jrf 7.30.00  N/A   Modified
		/// 06/19/06 jrf 7.30.00  N/A   Changed access from internal to private
		/// 06/29/06 jrf 7.30.00  N/A	Added code to return ERROR_DST_DATA_MISSING
		///								if a dst year from the meter's calendar
		///								is not found in the DST file. 
		///	07/24/06 jrf 7.30.35 SCR 20 Added code to handle case where TOU hasn't 
		///								expired but DST has.
		private DSTUpdateResult ReadTOUEventList(
			ref SCSTOUEventCollection TOUEventList )
		{			
			SCSTOUEvent NextEvent;
			CDSTDatePairCollection DSTDatePairs = m_dstSchedule.DSTDatePairs;
			DSTUpdateResult Result = DSTUpdateResult.SUCCESS;
			DateTime DSTEventDate = new DateTime(1,1,1);
			int iNextEventAddress = 0;
			int iExcepAddr = TOUCalendarAddress;
			int iCurrentEventIndex = 0;
			int iStartYear = 0;
			int iDSTUpdateStartYear = 0;
			int iCurrentYear = DateTime.Now.Year;
			int iIndex = 0;		
			bool bNeedsUpdate = false;
			bool bUpdated = false;
			bool bEndOfCalendar = false;
			byte[] byTOUEvents;

			// The start year for the update is the later of current year and 
			// earliest year from dst file			 
			if( iCurrentYear > ENERGY_ACT_OF_2005_START_YEAR ) 
			{
				iDSTUpdateStartYear = iCurrentYear;
			}
			else
			{
				iDSTUpdateStartYear = ENERGY_ACT_OF_2005_START_YEAR;
			}

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read TOU Base Address" );

            // Start at the tou base address
            iNextEventAddress = TOUCalendarAddress;
		
			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Start TOU Calendar Reads" );
			
			while (!bEndOfCalendar && DSTUpdateResult.SUCCESS == Result)
			{
				m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading TOU Calendar" );
				Result = SCSToDSTResult( m_SCSProtocol.Upload(
					iNextEventAddress, 
					SCS_TOU_EVENT_SIZE * MaxReadEvents, 
					out byTOUEvents ) );

				if ( DSTUpdateResult.SUCCESS == Result )
				{				
					iCurrentEventIndex = 0;

					// Lets get the bytes in the right order
					ReorderShorts(ref byTOUEvents);
				}	

				while ( !bEndOfCalendar 
					&& ( DSTUpdateResult.SUCCESS == Result )
					&& ( iCurrentEventIndex < MaxReadEvents ) )
				{
					NextEvent = new SCSTOUEvent(
						BitConverter.ToInt16(byTOUEvents, 
						( iCurrentEventIndex * SCS_TOU_EVENT_SIZE ) ) );
					
					if ( SCSTOUEvent.EventTypes.StartYear == NextEvent.Type )
					{
						// Set up start year for comparison
						iStartYear = NextEvent.Year;
						
						if ( NextEvent.Year > SCS_YEAR_BASE_CUTOFF )
						{
							iStartYear += SCS_PREVIOUS_YEAR_BASE;
						}
						else
						{
							iStartYear += SCS_CURRENT_YEAR_BASE;
						}
					} 
					else if ( SCSTOUEvent.EventTypes.DSTChange == NextEvent.Type )

					{
						// jrf - SCR 20
						// Set this date regardless, to determine if DST has
						// expired.
						DSTEventDate = new DateTime(
							iStartYear, 
							NextEvent.Month, 
							NextEvent.Day);

						if ( iStartYear >= iDSTUpdateStartYear )
						{
							try
							{
								// FindYear throws exception if year is not found
								iIndex = DSTDatePairs.FindYear(iStartYear);
								bNeedsUpdate = true;
																							
								if ( SCSTOUEvent.DirectionTypes.Advance == 
									NextEvent.Direction )
								{
									// Only change DST dates during the current 
									// year where the old DST date has not occured 
									// yet and the new DST date will occur 
									// in the future 
									if ( ( iStartYear == DateTime.Now.Year && 
										DSTEventDate > DateTime.Now && 
										DSTDatePairs[iIndex].ToDate > DateTime.Now ) || 
										( iStartYear != DateTime.Now.Year ) )
									{
										// Check DST advance month
										if( DSTDatePairs[iIndex].ToDate.Month != 
											NextEvent.Month )
										{
											NextEvent.Month = 
												DSTDatePairs[iIndex].ToDate.Month;
											bUpdated = true;
										}
										// Check DST advance day
										if( DSTDatePairs[iIndex].ToDate.Day != 
											NextEvent.Day )
										{
											NextEvent.Day = 
												DSTDatePairs[iIndex].ToDate.Day;
											bUpdated = true;
										}
									}
								}
								else
								{
									if ( ( iStartYear == DateTime.Now.Year && 
										DSTEventDate > DateTime.Now && 
										DSTDatePairs[iIndex].FromDate > DateTime.Now ) || 
										( iStartYear != DateTime.Now.Year ) )
									{
										// Check DST retard month
										if( DSTDatePairs[iIndex].FromDate.Month != 
											NextEvent.Month )
										{
											NextEvent.Month = 
												DSTDatePairs[iIndex].FromDate.Month;
											bUpdated = true;
										}
										// Check DST retard day
										if( DSTDatePairs[iIndex].FromDate.Day != 
											NextEvent.Day )
										{
											NextEvent.Day = 
												DSTDatePairs[iIndex].FromDate.Day;
											bUpdated = true;
										}
									}
								} // End if...else to test the tou event's direction
								

							}
							catch( ArgumentException )
							{
								// We didn't find the year in the collection 
								// of DSTDatePairs.  This is an error.
								Result = DSTUpdateResult.ERROR_DST_DATA_MISSING;
							}
						} // End if to check if start year is >= update start year

					} // End if...else to check tou event's type

					if ( SCSTOUEvent.EventTypes.CalendarEnd == NextEvent.Type )
					{
						bEndOfCalendar = true;
					}
	
					TOUEventList.Add( NextEvent );

					iCurrentEventIndex++;
				} // End while to check events from one read

				// Let's move to the next address
				iNextEventAddress += 2*MaxReadEvents;

			} // End while to read all events in tou calendar
			
			if ( DSTUpdateResult.SUCCESS == Result && bNeedsUpdate && !bUpdated )
			{
				Result = DSTUpdateResult.SUCCESS_PREVIOUSLY_UPDATED;
			}

			// jrf - SCR 20
			// Need to check case where TOU hasn't expired yet DST has.
			if ( DSTUpdateResult.SUCCESS == Result && DSTEventDate != new DateTime(1,1,1) && 
				DSTEventDate < DateTime.Now )
			{
				Result = DSTUpdateResult.ERROR_DST_DATES_EXPIRED;
			}

			return Result;
		} // End ReadTOUEventList()	
		
		/// <summary>
		/// This method reorders an array of bytes so that every pair of 
		/// bytes is reversed.
		/// </summary>
		/// <param name="byShortBytes">a byte array to </param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/01/06 jrf 7.30.00  N/A   Created
		/// 
		private void ReorderShorts(ref byte[] byShortBytes)
		{
			byte[] byTempValue = new byte[2]; 
			
			for( int iIndex = 0; iIndex + 2 <= byShortBytes.Length; iIndex+=2 )
			{										   
				byTempValue[0] = byShortBytes[iIndex+1];
				byTempValue[1] = byShortBytes[iIndex+0];

				byShortBytes[iIndex+0] = byTempValue[0];
				byShortBytes[iIndex+1] = byTempValue[1];
			}
			
		} // End ReorderShorts() 
        		
		/// <summary>
		/// Does all validation to see if this meter can be reconfigured to use
		/// this TOU and DST file. If everything is valid, the TOU server will
		/// be instantiated and the given filename will be opened. If 
		/// everything is valid and the meter is configured for DST, the DST
		/// server will be instantiated and the DST file will be opened.
		/// </summary>
		/// <param name="TOUFileName">PC-PRO+ TOU schedule file name</param>
		/// <param name="DSTFileName">PC-PRO+ DST file name (DST.xml). Pass
		/// an empty string if the meter doesn't support DST.</param>
		/// <param name="TOUInfo">TOU Info object that has already been read
		/// from the meter</param>
		/// <param name="TOUSchedule">TOU Server with TOU file open.</param>
		/// <param name="DSTSchedule">DST Server with DST file open. This can
		/// be null if the meter isn't configured with DST.</param>
		/// <returns>SUCCESS, SUCCESS_NOT_CONFIGURED_FOR_TOU, CLOCK_ERROR, 
		/// ERROR_DST_DATA_MISSING, ERROR_SCHED_NOT_SUPPORTED</returns>
		/// 
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/22/06 mcm 7.30.00 N/A	Created
		//  04/05/07 mrj 8.00.25 2839	Added check for correct device types.
		//	04/10/07 mrj 8.00.27 2860	Need to allow any SCS supported TOU
		//								schedule.
		//  04/24/07 mcm 8.01.01 157(TIMS) Changed error code returned for 
        //                              DST file name
        //  04/25/07 mcm 8.01.01 163(TIMS) Changed how the meter is checked for TOU
        //
		private TOUReconfigResult ValidateForTOUReconfigure( string TOUFileName,
			string DSTFileName, SCSTOUInfo TOUInfo, out CTOUSchedule TOUSchedule, 
			out CDSTSchedule DSTSchedule )
		{
			TOUReconfigResult	Result = TOUReconfigResult.SUCCESS; 

			// Satisfy compiler for out parameters
			DSTSchedule = null;
			TOUSchedule = null;

			// The following conditions must be met before reconfiguring TOU:
			//		The meter must have a clock.
			//		The meter must already be configured for TOU.
			//		The TOU file must be a valid PC-PRO+ 98 TOU schedule.
			//		The meter type must be supported by the TOU schedule.
			//		If the meter has DST, the DST file must be a valid PC-PRO+ 
			//			98 DST file.

			// Does the meter must have a clock?
			if( false == ClockEnabled )
			{
				Result = TOUReconfigResult.CLOCK_ERROR;
			}
			// The meter has a clock is it configured for TOU? Since TOU
			// and DST info is in the same place for SCS meters (FULCRUM
			// has a second copy of the DST dates), the TOU flag must be
			// on for DST to work. Check the size of season 0 to see if
			// TOU is really configured. 

            // mcm 04/25/2007 TIMS SCR 163, Changed this check from 
            // TOUInfo.TOUIsConfigured to TOUEnabled.  For all SCS devices
            // except the Fulcrum, the TOUEnabled method just checks the clock
            // and TOUInfo.TOUIsConfigured. The Fulcrum's implementation also
            // checks the firmware options bit, and that's about the only way
            // to tell if the meter was configured for TOU.
            else if (false == TOUEnabled)
			{
				Result = TOUReconfigResult.SUCCESS_NOT_CONFIGURED_FOR_TOU;
			}
			//	The meter is configured for TOU, try to open the TOU file.
			else
			{
				try
				{
					TOUSchedule = new CTOUSchedule( TOUFileName );
				}
				catch
				{
					m_Logger.WriteLine( Logger.LoggingLevel.Detailed, 
						"Error opening TOU Schedule " +
						TOUFileName );

					// If an exception was thrown, catch it and set the result.
					Result = TOUReconfigResult.ERROR_TOU_NOT_VALID;
				}
			}
			
			// It's open, is our device supported?
			if( TOUReconfigResult.SUCCESS == Result )
			{				
				//If any of the legacy (SCS) devices are supported then we should
				//allow the reconfigure.
				if (TOUSchedule.IsSupported("VECTRON") ||
					TOUSchedule.IsSupported("CENTRON") ||
					TOUSchedule.IsSupported("FULCRUM") ||
					TOUSchedule.IsSupported("QUANTUM") ||
					TOUSchedule.IsSupported("200 Series"))
				{
					Result = TOUReconfigResult.SUCCESS;				
				}
				else
				{
					Result = TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;				
				}
			}

			// If everything has gone well, check DST
			if(( TOUReconfigResult.SUCCESS == Result ) && DSTEnabled )
			{
				try
				{
					if(( null != DSTFileName ) && ( 0 < DSTFileName.Length ))
					{
						DSTSchedule = new CDSTSchedule( DSTFileName );
					}
					else
					{
						Result = TOUReconfigResult.ERROR_DST_DATA_MISSING;
					}
				}
				catch
				{
					m_Logger.WriteLine( Logger.LoggingLevel.Detailed, 
						"Error opening DST File " + DSTFileName );

					// If an exception was thrown, catch it and set the result.
					Result = TOUReconfigResult.ERROR_DST_DATA_MISSING;
				}
			}

			return Result;

		} // ValidateForTOUReconfigure
		
        /// <summary>
		/// Reconfigures the TOU calendar portion of the TOU schedule info
		/// </summary>
		/// <returns>TOUReconfigResult indicating success or failure</returns>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/04/06 mcm 7.30.00 N/A	Created
		/// 
		private TOUReconfigResult ReconfigureCalendar( CTOUSchedule TOUSchedule, 
			CDSTSchedule DSTSchedule, SCSTOUEventCollection TOUEventList )
		{
			TOUReconfigResult	Result = TOUReconfigResult.SUCCESS;
			byte				DSTIndex = 0;
			bool				ResetDemand = false;
			int					iYearBase = 100; 
            byte                byStartYear = 0;

			// All SCS devices except the FULCRUM define start year events to 
			// be modulo 100. Actually the lying fulcrum basepage does too.
			if( "X20" == DeviceType )
			{
				iYearBase = 1900;
			}

			if( SCSProtocolResponse.SCS_ACK != 
				ResetDemandOnSeasonChange(out ResetDemand))
			{
				return TOUReconfigResult.IO_TIMEOUT;
			}

			for(int iIndex = 0; iIndex < TOUSchedule.Years.Count; iIndex++ )
			{
                // Calculate the start year and save it so it can be used
                // mark the end of the calendar.
                byStartYear = (byte)(TOUSchedule.Years[iIndex].Year % iYearBase);
				TOUEventList.Add( new SCSTOUEvent(byStartYear));

				foreach( TOU.CEvent Event in TOUSchedule.Years[iIndex].Events )
				{
					if( TOU.eEventType.HOLIDAY == Event.Type )
					{
						TOUEventList.Add( new SCSTOUEvent(Event.Date));
					}
					else
					{
						TOUEventList.Add( new SCSTOUEvent((short)Event.Index, 
							Event.Date, ResetDemand ));
					}
				}

				// If we have a DST schedule, add the DST dates
				if( null != DSTSchedule )
				{
					if( false == DSTSchedule.FindDSTIndex(
                        TOUSchedule.Years[iIndex].Year, out DSTIndex))
					{
						// might give them a break if the year is old...
						Result = TOUReconfigResult.ERROR_DST_DATA_MISSING;
						break;
					}
					else
					{
						TOUEventList.Add( 
							new SCSTOUEvent(SCSTOUEvent.DirectionTypes.Advance,
							DSTSchedule.DSTDatePairs[DSTIndex].ToDate));
						
						TOUEventList.Add( 
							new SCSTOUEvent(SCSTOUEvent.DirectionTypes.Retard,
							DSTSchedule.DSTDatePairs[DSTIndex].FromDate));
					}

                    // The FULCRUM has an additional constraint on its calendar
                    // if it has DST. The FULCRUM DST dates are stored in a 
                    // separate table and the calendar can't exceed their limit
                    // of 25 years.
                    if (("X20" == DeviceType) && 
                        (iIndex == FULC_MAX_DST_RECORDS - 1))
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Calendar limited to 25 years for DST");
                        break;
                    }
				}

			} // FOREACH year in the schedule

			if( TOUReconfigResult.SUCCESS == Result )
			{
                // Add a start year event to denote the end of the previous year
                byStartYear++;
                TOUEventList.Add(new SCSTOUEvent(byStartYear));

				// Add an end of calendar event
				TOUEventList.Add( new SCSTOUEvent() );

				// Sort them all - Required for AddFirstStartDate()
				TOUEventList.Sort();

				// Add an Season start date for the first day of the first year
				// if there isn't one already.
				TOUEventList.AddFirstStartDate();
			}

			return Result;
		
		} // ReconfigureCalendar

		/// <summary>
		/// Reconfigures the season portion of the TOU schedule info
		/// </summary>
		/// <returns>TOUReconfigResult indicating success or failure</returns>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/04/06 mcm 7.30.00 N/A	Created
        /// 10/12/06 mcm 7.35.04 70,75,76 Imported 16 bit schedules can be incomplete
		/// 
		private void ReconfigureSeasons( CTOUSchedule TOUSchedule, 
			SCSSeasonCollection SeasonList )
		{
			SCSSeason	NewSeason;
			bool		IsFulcrum = false;


			if( "X20" == DeviceType )
			{
				IsFulcrum = true;
			}

			foreach( TOU.CSeason Season in TOUSchedule.Seasons )
			{
				NewSeason = new SCSSeason();

				// Add the pattern for each Daytype	to the season
				AddPattern(TOUSchedule, SCSSwitchpoint.DayType.NormalDay1, 
					NewSeason, TOUSchedule.Patterns.SearchID(Season.NormalDays[0]), 
					IsFulcrum );

                // mcm - 10/12/06 - SCRs 70, 75, 76 - When 16 bit schedules are imported 
                // but never saved, some daytypes might be missing if they aren't used.
                // Check to make sure they exist before using them.
                if (2 <= Season.NormalDays.Count)
                {
                    AddPattern(TOUSchedule, SCSSwitchpoint.DayType.NormalDay2,
                         NewSeason, TOUSchedule.Patterns.SearchID(Season.NormalDays[1]),
                        IsFulcrum);
                }
                if (3 <= Season.NormalDays.Count)
                {
                    AddPattern(TOUSchedule, SCSSwitchpoint.DayType.NormalDay3,
                        NewSeason, TOUSchedule.Patterns.SearchID(Season.NormalDays[2]),
                        IsFulcrum);
                }
                if (1 <= Season.Holidays.Count)
                {
                    AddPattern(TOUSchedule, SCSSwitchpoint.DayType.Holiday,
                        NewSeason, TOUSchedule.Patterns.SearchID(Season.Holidays[0]),
                        IsFulcrum);
                }
				// Add an end of season switchpoint
				NewSeason.Add(new SCSSwitchpoint(IsFulcrum));
				SeasonList.Add( NewSeason );
			}

			SeasonList.Sort();
			SeasonList.RemoveExtraOutputs();
		
		} // ReconfigureSeasons

		/// <summary>Adds the switchpoints from the given pattern to the season
		/// </summary>
		/// <param name="TOUSchedule">TOU Server with TOU file open.</param>
		/// <param name="DaytypeIndex">Daytype for this pattern</param>
		/// <param name="Season">Season to add the switchpoints to</param>
		/// <param name="PatternIndex">Index of the pattern to use</param>
		/// <param name="IsFulcrum">true if this is a Fulcrum</param>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/04/06 mcm 7.30.00 N/A	Created
		/// 
		private void AddPattern( CTOUSchedule TOUSchedule, 
			SCSSwitchpoint.DayType DaytypeIndex, SCSSeason Season, 
			int PatternIndex, bool IsFulcrum )
		{
			SCSSwitchpoint	AllOutputsOff;


			AllOutputsOff = new SCSSwitchpoint(IsFulcrum, DaytypeIndex);

			// First add an All Outputs Off switchpoint at 00:00
			Season.Add(AllOutputsOff);

			foreach( TOU.CSwitchPoint SP in 
				TOUSchedule.Patterns[PatternIndex].SwitchPoints )
			{
				Season.Add(new SCSSwitchpoint(IsFulcrum, DaytypeIndex, SP));

				// If it was an output, add an all outputs off SP at its stop
				// time, but don't add one at the end of the day (00:00).
				if(( TOU.eSwitchPointType.OUTPUT == SP.SwitchPointType )&&
				   (( 0 != SP.TimeOfStop.Hour ) || ( 0 != SP.TimeOfStop.Minute )))
				{
					AllOutputsOff = new SCSSwitchpoint(IsFulcrum, DaytypeIndex);
					AllOutputsOff.Hour = (byte)SP.TimeOfStop.Hour;
					AllOutputsOff.Minute = (byte)SP.TimeOfStop.Minute;
					Season.Add(AllOutputsOff);
				}
			}
		}

		/// <summary>This method reads the TOU Event List from the meter. 
		/// It is intended for debugging, but it might find some use later.
		/// </summary>
		/// <param name="TOUInfo">TOU Info object that has already been read
		/// from the meter</param>
		/// <param name="TOUEventList">TOU Event List to be filled with 
		/// configuration data from the meter.</param>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/04/06 mcm 7.30.00 N/A	Created
		/// 
		private void ReadTOUEvents(SCSTOUInfo TOUInfo, 
			out SCSTOUEventCollection TOUEventList )
		{
			SCSProtocolResponse Result = SCSProtocolResponse.SCS_ACK;
			SCSTOUEvent NextEvent;
			byte[]		TOUEvents;


			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read TOU Events" );
			
			TOUEventList = new SCSTOUEventCollection();

			// Let's get the tou base address
			Result = m_SCSProtocol.Upload( TOUInfo.StartOfYearlySchedule, 
				TOUInfo.SizeOfYearlySchedule, out TOUEvents );

			if( SCSProtocolResponse.SCS_ACK == Result)
			{
				for( int I = 0; I < TOUInfo.SizeOfYearlySchedule; )
				{
					NextEvent = new SCSTOUEvent( (short)( TOUEvents[I++] * 0x100 +
						TOUEvents[I++] ));
					TOUEventList.Add( NextEvent );
				}
			} 
			
		} // ReadTOUEvents


		/// <summary>
        /// This method reads the TOU Season List from the meter. 
		/// It is intended for debugging, but it might find some use later.
		/// </summary>
		/// <param name="TOUInfo">TOU Info object that has already been read
		/// from the meter</param>
		/// <param name="SeasonList">TOU Season List to be filled with 
		/// configuration data from the meter.</param>
		/// <remarks>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/07/06 mcm 7.30.00 N/A	Created
        /// </remarks>
		internal virtual SCSProtocolResponse ReadSeasons(SCSTOUInfo TOUInfo, 
			out SCSSeasonCollection SeasonList )
		{
			SCSProtocolResponse Result = SCSProtocolResponse.NoResponse;
			byte[]				SeasonInfo = null;
			bool				IsFulcrum = false;


			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, 
								"Starting TOU Season Reads" );

			if( "X20" == DeviceType )
			{
				IsFulcrum = true;
			}

			SeasonList = new SCSSeasonCollection();

			// Read Season 0
			if( 0 < TOUInfo.SizeOfSeason0 )
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason0,
					TOUInfo.SizeOfSeason0, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 0
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 1
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason1 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason1, 
					TOUInfo.SizeOfSeason1, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 1
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}
			
			// Read Season 2
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason2 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason2, 
					TOUInfo.SizeOfSeason2, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 2
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 3
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason3 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason3, 
					TOUInfo.SizeOfSeason3, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 3
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 4
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason4 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason4, 
					TOUInfo.SizeOfSeason4, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 4
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 5
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason5 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason5, 
					TOUInfo.SizeOfSeason5, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 5
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 6
			if(( SCSProtocolResponse.SCS_ACK == Result ) &&
				( 0 < TOUInfo.SizeOfSeason6 ))
			{
				Result = m_SCSProtocol.Upload( TOUInfo.StartOfSeason6, 
					TOUInfo.SizeOfSeason6, out SeasonInfo );

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 6
					SeasonList.Add( IsFulcrum, SeasonInfo );
				}
			}

			// Read Season 7
			if(( SCSProtocolResponse.SCS_ACK == Result ) && 
			   ( TOUInfo.StartOfSeason6 != TOUInfo.StartOfSeason7 ))
			{
				SCSSeason		Season = new SCSSeason();
				SCSSwitchpoint	SP = new SCSSwitchpoint(IsFulcrum, (ushort)0);
				ushort			Address = TOUInfo.StartOfSeason7;
				byte[]			Data;


				while(( SCSProtocolResponse.SCS_ACK == Result ) &&
					  ( !SP.IsEndOfSeason ))
				{
					Result = m_SCSProtocol.Upload( Address, 2, out Data );
					Address += 2;

					if( SCSProtocolResponse.SCS_ACK == Result )
					{
						SP = new SCSSwitchpoint( IsFulcrum, 
							(ushort)(Data[0] * 0x100 + Data[1]));

						Season.Add( SP );
					}
				}

				if( SCSProtocolResponse.SCS_ACK == Result )
				{
					// Add season 7
					SeasonList.Add( Season );
				}
			} // End read of season 7

		
			return Result;	
			
		} // ReadSeasons

        /// <summary>
        /// This translates a season from the internal SCS format to the 
        /// generic TOUSchedule object
        /// </summary>
        /// <param name="scsSeason"></param>
        /// <param name="nSeasonIndex"></param>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/29/06 mah 8.00.00 N/A	Created
		///  03/12/07 mah 8.00.18        Corrected SCR #2615 by properly using Pattern IDs instead of indexes
        /// </remarks>
        private void TranslateSCSSeason(SCSSeason scsSeason, int nSeasonIndex)
        {

			// Since the SCS schedule does not have the concept of patterns, we are going to
            // create a new pattern for each day type used in each season.  This means that there
            // will be three patterns for the normal day types and one pattern created for the 
			// holiday day type and Add each pattern to the time of use schedule
			
			int nDayType1PatternID = m_touSchedule.Patterns.GetNextPatternID();
            CPattern patternDayType1 = new CPattern(nDayType1PatternID, "Season " + nSeasonIndex.ToString(CultureInfo.InvariantCulture) + " day type 1 pattern", new CSwitchPointCollection());
			m_touSchedule.Patterns.Add(patternDayType1);

			int nDayType2PatternID = m_touSchedule.Patterns.GetNextPatternID();
            CPattern patternDayType2 = new CPattern(nDayType2PatternID, "Season " + nSeasonIndex.ToString(CultureInfo.InvariantCulture) + " day type 2 pattern", new CSwitchPointCollection());
			m_touSchedule.Patterns.Add(patternDayType2);

			int nDayType3PatternID = m_touSchedule.Patterns.GetNextPatternID();
            CPattern patternDayType3 = new CPattern(nDayType3PatternID, "Season " + nSeasonIndex.ToString(CultureInfo.InvariantCulture) + " day type 3 pattern", new CSwitchPointCollection());
			m_touSchedule.Patterns.Add(patternDayType3);

			int nHolidayPatternID = m_touSchedule.Patterns.GetNextPatternID();
			CPattern patternHoliday = new CPattern(nHolidayPatternID, "Season " + nSeasonIndex.ToString(CultureInfo.InvariantCulture) + " holiday pattern", new CSwitchPointCollection());
			m_touSchedule.Patterns.Add(patternHoliday);

            // Now build the day type arrays using the pattern indexes we just created 

			Int16Collection arrayNormalDays = new Int16Collection();
			arrayNormalDays.Add((short)nDayType1PatternID);
            arrayNormalDays.Add((short)nDayType2PatternID);
            arrayNormalDays.Add((short)nDayType3PatternID);

            Int16Collection arrayHolidays = new Int16Collection();
            arrayHolidays.Add((short)nHolidayPatternID);

            // Now create the season object to tie all of these pieces together

            CSeason newSeason = new CSeason(nSeasonIndex + 1, "Season " + nSeasonIndex.ToString(CultureInfo.InvariantCulture), arrayNormalDays, arrayHolidays);
            m_touSchedule.Seasons.Add(newSeason);

            // Next troll through the switch point list to fill in all of the switchpoints for the given season
            // Note that SCS devices do not support overlapping rates so each switchpoint as read from the
            // meter only has a single start time. 

            eSwitchPointType touSwitchPointType;
            int nRateOutputIndex;

            foreach (SCSSwitchpoint Switchpoint in scsSeason)
            {
                if (!Switchpoint.IsEndOfSeason)
                {
                    if (Switchpoint.Type == SCSSwitchpoint.SwitchpointType.Rate)
                    {
                        touSwitchPointType = eSwitchPointType.RATE;
                        nRateOutputIndex = TranslateRateIndex(Switchpoint);
                    }
                    else
                    {
                        touSwitchPointType = eSwitchPointType.OUTPUT;
                        nRateOutputIndex = TranslateOutputIndex(Switchpoint);
                    }

                    CSwitchPoint touSwitchpoint = new CSwitchPoint(Switchpoint.Hour * 60 + Switchpoint.Minute, // switch point start time in minutes since midnight
                                                                                              (24 * 60) - 1, // default stop time @ midnight to be updated after all switch points are read,
                                                                                              nRateOutputIndex,
                                                                                              touSwitchPointType);

                    // Now add the switchpoint to the appropriate pattern list

                    switch (Switchpoint.TypeOfDay)
                    {
                        case SCSSwitchpoint.DayType.NormalDay1:
                            patternDayType1.SwitchPoints.Add(touSwitchpoint);
                            break;
                        case SCSSwitchpoint.DayType.NormalDay2:
                            patternDayType2.SwitchPoints.Add(touSwitchpoint);
                            break;
                        case SCSSwitchpoint.DayType.NormalDay3:
                            patternDayType3.SwitchPoints.Add(touSwitchpoint);
                            break;
                        case SCSSwitchpoint.DayType.Holiday:
                            patternHoliday.SwitchPoints.Add(touSwitchpoint);
                            break;
                    }
                }
            }

            // The last step is to go through each of the switch point lists and set the switch point
            // end times so that they are not shown as overlapping

            SetSwitchPointStopTimes(patternDayType1);
            SetSwitchPointStopTimes(patternDayType2);
            SetSwitchPointStopTimes(patternDayType3);
            SetSwitchPointStopTimes(patternHoliday);
        }

        private static int TranslateOutputIndex(SCSSwitchpoint Switchpoint)
        {
            int nRateOutputIndex;
            switch (Switchpoint.RateOutput)
            {
                case SCSSwitchpoint.RateOutputType.Output1:
                    nRateOutputIndex = 0;
                    break;
                default:
                    nRateOutputIndex = 2;
                    break;
            }
            return nRateOutputIndex;
        }

        private static int TranslateRateIndex(SCSSwitchpoint Switchpoint)
        {
            int nRateOutputIndex;
            switch (Switchpoint.RateOutput)
            {
                case SCSSwitchpoint.RateOutputType.RateA:
                    nRateOutputIndex = 0;
                    break;
                case SCSSwitchpoint.RateOutputType.RateB:
                    nRateOutputIndex = 1;
                    break;
                case SCSSwitchpoint.RateOutputType.RateC:
                    nRateOutputIndex = 2;
                    break;
                case SCSSwitchpoint.RateOutputType.RateD:
                    nRateOutputIndex = 3;
                    break;
                default:
                    nRateOutputIndex = 4;
                    break;
            }
            return nRateOutputIndex;
        }

        private void SetSwitchPointStopTimes(CPattern patternDayType)
        {
            for (int nSwitchpointIndex = 0; nSwitchpointIndex < patternDayType.SwitchPoints.Count; nSwitchpointIndex++)
            {
                Boolean boolStopTimeFound = false;
                int nNextSwitchPoint = nSwitchpointIndex + 1;

                while (!boolStopTimeFound && nNextSwitchPoint < patternDayType.SwitchPoints.Count)
                {
                    if (patternDayType.SwitchPoints[nSwitchpointIndex].SwitchPointType ==
                        patternDayType.SwitchPoints[nNextSwitchPoint].SwitchPointType)
                    {
                        boolStopTimeFound = true;
                        patternDayType.SwitchPoints[nSwitchpointIndex].StopTime = patternDayType.SwitchPoints[nNextSwitchPoint].StartTime;
                    }

                    nNextSwitchPoint++;
                }
            }
        }

        private void TranslateSCSEventList(ref SCSTOUEventCollection scsEventList)
        {
            CYear touYear = null;
            CEventCollection touEventList = null;

            foreach (SCSTOUEvent scsEvent in scsEventList)
            {
                switch (scsEvent.Type)
                {
                    case SCSTOUEvent.EventTypes.StartYear:
                        touEventList = new CEventCollection();

                        // Note that the years are given as two digit numbers 00 to 99 so add the correct
                        // century data to them

                        if (scsEvent.Year >= 80)
                        {
                            touYear = new CYear(scsEvent.Year + 1900, touEventList);
                        }
                        else
                        {
                            touYear = new CYear(scsEvent.Year + 2000, touEventList);
                        }

                        m_touSchedule.Years.Add(touYear);
                        break;
                    case SCSTOUEvent.EventTypes.CalendarEnd:
                        break;
                    case SCSTOUEvent.EventTypes.DSTChange:
                        if (touEventList != null)
                        {
                            if ((touEventList != null) && (touYear != null))
                            {
                                eEventType dstChangeType;

                                DateTime dateDSTChange = new DateTime(touYear.Year, scsEvent.Month, scsEvent.Day);

                                if (scsEvent.Direction == SCSTOUEvent.DirectionTypes.Advance)
                                {
                                    dstChangeType = eEventType.TO_DST;
                                }
                                else
                                {
                                    dstChangeType = eEventType.FROM_DST;
                                }

                                CEvent touEvent = new CEvent(dateDSTChange, dstChangeType, 0, "Daylight Savings Time Change");
                                touEventList.Add(touEvent);
                            }

                        }
                        break;
                    case SCSTOUEvent.EventTypes.HolidaySelect:
                        if ((touEventList != null) && (touYear != null))
                        {
                            DateTime dateHoliday = new DateTime(touYear.Year, scsEvent.Month, scsEvent.Day);

                            CEvent touEvent = new CEvent(dateHoliday, eEventType.HOLIDAY, 0, "Holiday");

                            touEventList.Add(touEvent);
                        }
                        break;
                    case SCSTOUEvent.EventTypes.SeasonSelect:
                        if ((touEventList != null) && (touYear != null))
                        {
                            DateTime dateSeasonChange = new DateTime(touYear.Year, scsEvent.Month, scsEvent.Day);

                            CEvent touEvent = new CEvent(dateSeasonChange, eEventType.SEASON, scsEvent.Season, "Season Change");

                            touEventList.Add(touEvent);
                        }
                        break;
                    case SCSTOUEvent.EventTypes.Unknown:
                        break;
                }
            }
        }

        /// <summary>
        /// This method returns a byte array representing the meter's basepage
        /// </summary>
        /// <returns></returns>
        private byte[] CreateBasepageImage()
        {
            // The goal is to create a byte array that is a mirror image of the device's basepage.  Unfortunately
            // we cannot read the entire basepage in a single pass.  The meters limit the maximum read size.  So
            // the goal here it to read the basepage with the minimum number of upload commands and assemble
            // the data into a single byte array to mimic the basepage.

            int nBytesRead = 0;
            int nReadLength = 0;

            // NOTE The block size was carefully selected to work with all device types.  The VECTRON's maximum
            // upload block size is 256 bytes but tests show that 128 bytes is the largest size that will work efficiently.
            // larger sizes create sporadic failures.  Also, 128 is larger than either the CENTRON's or MT200's block size but 
            // the protocol will break this request down into multiple smaller requests.

            int nBlockSize = 128; 
            int nBasepageSize = MemoryEnd - MemoryStart;
            int nTotalBlocks = (nBasepageSize / nBlockSize) + 1; // used only in progress messages
            int nCurrentBlock = 1;

            // We need two arrays - one to represent the entire basepage and one to hold each block of 
            // data as it is uploaded from the meter

            byte[] abytBasepageImage = new byte[nBasepageSize];
            byte[] abytDataBlock;
            SCSProtocolResponse result = SCSProtocolResponse.SCS_ACK;

            // This can be a particularly lengthy operation.  Be sure to show a progress indicator
            // as we read each item

            OnShowProgress(new ShowProgressEventArgs(1, nBasepageSize / nBlockSize,
                 "Retrieving meter image...", "Retrieving meter image..."));

            // Here's the business end of this method - start at the beginning of the basepage and 
            // read it chunk by chunk, assembling a 'virtual' basepage until we reach the end of the
            // basepage

            while (nBytesRead < nBasepageSize && SCSProtocolResponse.SCS_ACK == result)
            {
                if (nBytesRead + nBlockSize < nBasepageSize)
                {
                    // read a full block
                    nReadLength = nBlockSize;
                }
                else
                {
                    // read a partial block
                    nReadLength = nBasepageSize - nBytesRead;
                }

                result = m_SCSProtocol.Upload(MemoryStart + nBytesRead, nReadLength, out abytDataBlock);

                if (SCSProtocolResponse.SCS_ACK == result)
                {
                    OnStepProgress(new ProgressEventArgs("Reading block " + nCurrentBlock.ToString(CultureInfo.InvariantCulture) + " of " + nTotalBlocks.ToString(CultureInfo.InvariantCulture)));

                    //copy the block into the basepage image

                    abytDataBlock.CopyTo(abytBasepageImage, nBytesRead);
                    nBytesRead += nReadLength;
                    nCurrentBlock++;
                }
                else
                {
                    throw (new SCSException(SCSCommands.SCS_U, result, nBytesRead, "Basepage Image"));
                }
            }

            // All  done - hide the progress indicator and return the basepage image to the 
            // calling routine

            OnHideProgress(new ProgressEventArgs());
            return abytBasepageImage;
        }

        #endregion Private Methods
        
        #region Members

        // Member Variables
        /// <summary>
        /// The location of the address for the TOU Calendar.
        /// </summary>
        protected int m_iTOUBaseAddress;
        /// <summary>
        /// The DST file name used to update the DST.
        /// </summary>
        protected string m_strDSTFile;
        /// <summary>
        /// The model of the SCS device.
        /// </summary>
        protected SCSModelTypes m_Model;
        /// <summary>
        /// The SCS protocol object.
        /// </summary>
        protected SCSProtocol m_SCSProtocol;
        /// <summary>
        /// The DST Schedule that is extracted from the DST file.
        /// </summary>
        protected CDSTSchedule m_dstSchedule;
        /// <summary>
        /// The serial number of the SCS device.
        /// </summary>
        protected CachedString m_serialNumber;
        /// <summary>
        /// The Unit ID of the SCS device.
        /// </summary>
        protected CachedString m_unitID;
        /// <summary>
        /// The type of SCS device.
        /// </summary>
        protected CachedString m_deviceType;
        /// <summary>
        /// The start of memory in the SCS device.
        /// </summary>
        protected CachedInt m_memoryStart;
        /// <summary>
        /// The end of memory in the SCS device.
        /// </summary>
        protected CachedInt m_memoryEnd;
        /// <summary>
        /// The program ID of the SCS device.
        /// </summary>
        protected CachedInt m_programID;
        /// <summary>
        /// The starting address of the TOU calendar.
        /// </summary>
        protected CachedInt m_touCalendarStartAddress;
        /// <summary>
        /// The firmware version of the SCS device.
        /// </summary>
        protected CachedFloat m_fwVersion;
        /// <summary>
        /// The software version of the SCS device.
        /// </summary>
        protected CachedFloat m_swVersion;
        /// <summary>
        /// The state of DST in the SCS device;
        /// </summary>
        protected CachedBool m_dstEnabled;
        /// <summary>
        /// The state of DST in the SCS device;
        /// </summary>
        protected CachedBool m_IsCanadian;
        /// <summary>
        /// TOU is considered enabled if the clock is running and the meter
        /// is configured to follow a TOU schedule.  TOU does not have to be
        /// running for this property to return true.  For example an expired
        /// TOU schedule is enabled but not running.
        /// </summary>
        protected CachedBool m_touEnabled;
       /// <summary>
        /// The state of TOU Run Flag in the SCS device. Does not mean that
        /// TOU is configured!  Meters configured with DST only have this
        /// set to true.
        /// </summary>
        protected CachedBool m_touRunFlag;
        /// <summary>
        /// The state of TOU expiration in the SCS device.
        /// </summary>
        protected CachedBool m_touExpired;
        /// <summary>
        /// The state of the clock in the SCS device.
        /// </summary>
        protected CachedBool m_clockEnabled;
        /// <summary>
        /// The Firmware Option byte.
        /// </summary>
        protected CachedByte m_FirmwareOptions;
        /// <summary>
        /// The number of time programmed.
        /// </summary>
        protected CachedInt m_numTimesProgrammed;
        /// <summary>
        /// The date and time that the meter was last programmed.
        /// </summary>
        protected CachedDate m_dateLastProgrammed;
        /// <summary>
        /// TOU Expiration Date
        /// </summary>
        protected CachedDate m_dateTOUExpiration;
        /// <summary>
        /// The number of power outages.
        /// </summary>
        protected CachedInt m_numOutages;
        /// <summary>
        /// The number of minutes on batterh.
        /// </summary>
        protected CachedUint m_numMinOnBattery;
        /// <summary>
        /// Cold Load Pickup Time in minutes
        /// </summary>
        protected CachedUint m_nCLPU;
        /// <summary>
        /// The logging object used for writing debug information to the log file.
        /// </summary>
        protected Logger m_Logger;
        /// <summary>
        /// The load profile interval length
        /// </summary>
        protected CachedInt m_LPIntervalLength;
        /// <summary>
        /// The load profile flag
        /// </summary>
        protected CachedBool m_LPRunning;
        /// <summary>
        /// The number of load profile channels
        /// </summary>
        protected CachedInt m_NumberLPChannels;

        /// <summary>
        /// The security code used for the current session;
        /// </summary>
        protected string m_strCurrentSecurityCode;

        /// <summary>
        /// Local storage for a list of the user data fields.  These
        /// data items are intended to ba cached in this list.
        /// </summary>
        protected List<String> m_UserDataList;
        
        /// <summary>
        /// Represents the current meter's demand interval length in minutes
        /// </summary>
        protected CachedInt m_DemandIntervalLength;
        
        /// <summary>
        /// Represents the current meter's number of normal mode demand sub intervals
        /// </summary>
        protected CachedInt m_NumberOfSubIntervals;
        
        /// <summary>
        /// Represents the current meter's test mode demand interval length in minutes
        /// </summary>
        protected CachedInt m_TestModeIntervalLength;
        
        /// <summary>
        /// Represents the current meter's number of test mode demand sub intervals
        /// </summary>
        protected CachedInt m_NumberOfTestModeSubIntervals;

        /// <summary>
        /// Provides a temporary cache of the TOU schedule to prevent unnecessary reads of the 
        /// TOU configuration
        /// </summary>
        protected SCSTOUSchedule m_touSchedule;

        /// <summary>
        /// Provides a temporary cache of the normal display list configuration
        /// </summary>
        protected List<DisplayItem> m_NormalDisplayList;

        /// <summary>
        /// Provides a temporary cache of the alternate display list configuration
        /// </summary>
        protected List<DisplayItem> m_AlternateDisplayList;

        /// <summary>
        /// Provides a temporary cache of the test mode display list configuration
        /// </summary>
        protected List<DisplayItem> m_TestDisplayList;

        /// <summary>
        /// The TOU Schedule identifier
        /// </summary>
        protected CachedString m_TOUScheduleID;
		/// <summary>
		/// The line frequency cached item.
		/// </summary>
		protected CachedInt m_LineFrequency;
		/// <summary>
		/// The model type cached item.
		/// </summary>
		protected CachedString m_ModelType;
		/// <summary>
		/// The tranformer ratio cached item.
		/// </summary>
		protected CachedFloat m_TranformerRatio;
		/// <summary>
		/// The model type cached item.
		/// </summary>
		protected CachedString m_LoadResearchID;

        //Members to support resource strings:
        private static readonly string RESOURCE_FILE_PROJECT_STRINGS =
            "Itron.Metering.Device.SCSDevice.SCSDeviceStrings";
        /// <summary>
        /// Resourse Manager object that supports extracting strings from the 
        /// resourse file.
        /// </summary>
        protected System.Resources.ResourceManager m_rmStrings = null;

        #endregion Members
	} // End SCSDevice

} // End Namespace
