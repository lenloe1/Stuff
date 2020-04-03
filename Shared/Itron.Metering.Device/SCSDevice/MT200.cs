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
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
	/// <summary>
	/// Class representing the MT200 meter.
	/// </summary>
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------
	//  04/27/06 mrj 7.30.00  N/A	Created
	//  05/30/06 jrf 7.30.00  N/A	Modified
    //  11/22/06 mah 8.00.00  N/A   Added properties to retrieve register readings
    //
	public partial class MT200 : SCSDevice
    {
        #region Definitions

        /// <summary>
		/// MT2Addresses enumeration encapsulates the MT200 basepage addresses.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00 N/A	Created
		/// 
		protected enum MT2Addresses : int
		{
            /// <summary>
            /// Test Mode Flag
            /// </summary>
			TEST_MODE_FLAG               = 0x00E9,
            /// <summary>
            /// Demand Reset Flag
            /// </summary>
            DEMAND_RESET_FLAG           = 0x00EA,
            /// <summary>
            /// Hangup Flag
            /// </summary>
            HANGUP_FLAG					= 0x00EB,
            /// <summary>
            /// Stop Metering Flag
            /// </summary>
            STOP_METER_FLAG             = 0x00EC,
            /// <summary>
            /// Clear Billing Register Flag
            /// </summary>
            CLEAR_BILLING_FLAG          = 0x00ED,
            /// <summary>
            /// Clock Running Flag
            /// </summary>
			CLOCK_RUN_FLAG				= 0x00EE,
            /// <summary>
            /// Load Profile Run Flag (Used to stop Load Profile)
            /// </summary>
			MM_RUN_FLAG					= 0x00EF,
            /// <summary>
            /// TOU Run Flag (Used to stop TOU)
            /// </summary>
			TOU_RUN_FLAG				= 0x00F0,
            /// <summary>
            /// Clock Reconfigure Flag (Used to change the clock)
            /// </summary>
			CLOCK_RECONFIGURE_FLAG		= 0x00F1,
            /// <summary>
            /// TOU Reconfigure Flag (Used to tell meter TOU has been changed)
            /// </summary>
			TOU_RECONFIGURE_FLAG		= 0x00F3,
            /// <summary>
            /// Mass Memory Adjust Time flag
            /// </summary>
			MM_AJUST_TIME_FLAG			= 0x00F4,
            /// <summary>
            /// System Error
            /// </summary>
			SYSTEM_ERROR				= 0x00F8,
            /// <summary>
            /// Read Time
            /// </summary>
			REAL_TIME					= 0x00F9,
			/// <summary>
			/// Access to Model Type
			/// </summary>
            MODEL_TYPE					= 0x0110,
            /// <summary>
            /// Access to Rate E KWh
            /// </summary>
            RATEE_KWH              = 0x0119,
            /// <summary>
            /// Access to Rate E Cum KW
            /// </summary>
            RATEE_CUM_KW             = 0x0120,
			/// <summary>
			/// Access to Rate E KW
			/// </summary>
			RATEE_KW = 0x0124,
            /// <summary>
            /// Access to Last Reset Time
            /// </summary>
            LAST_RESET_DATE = 0x012C,
            /// <summary>
            /// Access to Reset Count
            /// </summary>
            RESET_COUNT                 = 0x0130,
            /// <summary>
            /// Access to Outage Count
            /// </summary>
            OUTAGE_COUNT                = 0x0132,
            /// <summary>
            /// Acccess to Reate A Cum KW
            /// </summary>
            RATEA_CUM_KW             = 0x0134,
            /// <summary>
            /// Access to Rate A KW
            /// </summary>
            RATEA_KW                 = 0x0138,
            /// <summary>
            /// Acccess to Reate B Cum KW
            /// </summary>
            RATEB_CUM_KW             = 0x0140,
            /// <summary>
            /// Access to Rate B KW
            /// </summary>
            RATEB_KW                 = 0x0144,
            /// <summary>
            /// Access to Transformer Ratio
            /// </summary>
			TRANSFORMER_RATIO		 = 0x014D,
            /// <summary>
            /// Access to Display Options
            /// </summary>
            DISPLAY_OPTIONS          = 0x014F,
            /// <summary>
            /// Acccess to Rate C Cum KW
            /// </summary>
            RATEC_CUM_KW             = 0x0150,
            /// <summary>
            /// Access to Rate C KW
            /// </summary>
            RATEC_KW                 = 0x0154,
            /// <summary>
            /// Access to Reate D Cum KW
            /// </summary>
            RATED_CUM_KW             = 0x0160,
            /// <summary>
            /// Access to Rate D KW
            /// </summary>
            RATED_KW                      = 0x0164,
            /// <summary>
            /// Access to Rate A KWh
            /// </summary>
            RATEA_KWH                    = 0x0172,
            /// <summary>
            /// Access to Demand Configuration
            /// </summary>
            DEMAND_CONFIGURATION = 0x018E,
            /// <summary>
            /// Access to Operating Setup
            /// </summary>
			OPERATING_SETUP				= 0x0196,
            /// <summary>
            /// Access to Minutes on Battery
            /// </summary>
            MINUTES_ON_BATTERY          = 0x0197,
            /// <summary>
            /// Access to Program Count
            /// </summary>
            PROGRAM_COUNT               = 0x019A,
            /// <summary>
            /// Access to Last Programmed Date
            /// </summary>
            LAST_PROGRAMMED_DATE        = 0x019C,
            /// <summary>
            /// Access to Communications Timeout
            /// </summary>
			COMMUNICATIONS_TIMEOUT		= 0x01A0,
            /// <summary>
            /// Access to Self Read Rate E KWh
            /// </summary>
            SELF_READ_RATEE_KWH           = 0x01A1,
            /// <summary>
            /// Access to Self Read Rate E KW
            /// </summary>
            SELF_READ_RATEE_KW             = 0x01A4,
            /// <summary>
            /// Access to Self Read Rate A KWh
            /// </summary>
            SELF_READ_RATEA_KWH           = 0x01A7,
            /// <summary>
            /// Access to Self Read Rate A KW
            /// </summary>
            SELF_READ_RATEA_KW             = 0x01AA,
            /// <summary>
            /// Access to Self Read Rate B KWh
            /// </summary>
            SELF_READ_RATEB_KWH           = 0x01AD,
            /// <summary>
            /// Access to Self Read Rate B KW
            /// </summary>
            SELF_READ_RATEB_KW             = 0x01B0,
            /// <summary>
            /// Access to Self Read Rate C KWh
            /// </summary>
            SELF_READ_RATEC_KWH           = 0x01B3,
            /// <summary>
            /// Access to Self Read Rate C KW
            /// </summary>
            SELF_READ_RATEC_KW             = 0x01B6,
            /// <summary>
            /// Access to Self Read Rate D KWh
            /// </summary>
            SELF_READ_RATED_KWH           = 0x01B9,
            /// <summary>
            /// Access to Self Read Rate D KW
            /// </summary>
            SELF_READ_RATED_KW             = 0x01BC,
			/// <summary>
			/// Number of pulses per disk revolution
			/// </summary>
			OUTPUT_PER_DISK_REV			    = 0x01BF,
            /// <summary>
            /// Access to Cold Load Pickup Time
            /// </summary>
            COLD_LOAD_PICKUP_TIME          = 0x01D9,
            /// <summary>
            /// Access to Primary Password
            /// </summary>
            PRIMARY_PASSWORD            = 0x01DD,
            /// <summary>
            /// Access to Secondary Password
            /// </summary>
            SECONDARY_PASSWORD      = 0x01E5,
            /// <summary>
            /// Access to Unit ID
            /// </summary>
			UNIT_ID						                = 0x01F5,
			/// <summary>
			/// Access to Software Version
			/// </summary>
            SOFTWARE_REVISION			    = 0x0201,
            /// <summary>
            /// Access to Firmware Version
            /// </summary>
			FIRMWARE_REVISION			    = 0x0203,
            /// <summary>
            /// Access to User Defied Field 1
            /// </summary>
            USERDEFINED_FIELD1             = 0x0205,
            /// <summary>
            /// Access to Program ID
            /// </summary>
			PROGRAM_ID					        = 0x0220,
            /// <summary>
            /// Access to Serial Number
            /// </summary>
			SERIAL_NUMBER				        = 0x0222,
            /// <summary>
            /// Access to Energy Format Address
            /// </summary>
            ENERGYFORMAT_ADDRESS   = 0x0234,
            /// <summary>
            /// Access to Firmware Options
            /// </summary>
			FIRMWARE_OPTIONS			    = 0x023C,
            /// <summary>
            /// Access to Display Table
            /// </summary>
            DISPLAY_TABLE                      = 0x023D,
            /// <summary>
            /// Access to Last Season Registers
            /// </summary>
			LAST_SEASON_REGISTERS		= 0x0481,
			/// <summary>
			/// Access to Last Season Rate E kW
			/// </summary>
			LAST_SEASON_RATEE_KW = 0x0484,
			/// <summary>
			/// Access to Last Season Rate A kW
			/// </summary>
			LAST_SEASON_RATEA_KW = 0x048A,
			/// <summary>
			/// Access to Last Season Rate B kW
			/// </summary>
			LAST_SEASON_RATEB_KW = 0x0490,
			/// <summary>
			/// Access to Last Season Rate C kW
			/// </summary>
			LAST_SEASON_RATEC_KW = 0x0496,
			/// <summary>
			/// Access to Last Season Rate D kW
			/// </summary>
			LAST_SEASON_RATED_KW = 0x049C,
			/// <summary>
            /// Access to Last Season Cum Rate E
            /// </summary>
            LAST_SEASON_CUM_RATEE   = 0x04B0,
            /// <summary>
            /// Access to Last Season Cum Rate A
            /// </summary>
            LAST_SEASON_CUM_RATEA   = 0x04B3,
            /// <summary>
            /// Access to Last Season Cum Rate B
            /// </summary>
            LAST_SEASON_CUM_RATEB   = 0x04B6,
            /// <summary>
            /// Access to Last Season Cum Rate C
            /// </summary>
            LAST_SEASON_CUM_RATEC   = 0x04B9,
            /// <summary>
            /// Access to Last Season Cum Rate D
            /// </summary>
            LAST_SEASON_CUM_RATED   = 0x04BC,
            /// <summary>
            /// Access to Yearly Schedule
            /// </summary>
			YEARLY_SCHEDULE				= 0x04CB,
            /// <summary>
            /// Access to TOU Exipration Date
            /// </summary>
			TOU_EXPIRATION_DATE		    = 0x04D1,
            /// <summary>
            /// Access to TOU Schedule ID
            /// </summary>
            TOU_SCHEDULE_ID                  = 0x04D5,
            /// <summary>
            /// Access to TOU Base
            /// </summary>
			TOU_BASE					            = 0x04D7,								
            /// <summary>
            /// Access to Load Profile Interval Length
            /// </summary>
			MM_INTERVAL_LENGTH			= 0x0504,
            /// <summary>
            /// Access to Load Research ID
            /// </summary>
			LOAD_RESEARCH_ID = 0x050C,							
		};

		/// <summary>
		/// MT2Errors enumeration encapsulates the MT200 non-fatal system 
		/// errors bitmasks.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/31/06 jrf 7.30.00 N/A	Created
		/// 
		private enum MT2Errors : byte
		{
			LOW_BATTERY					= 0x10,
			REGISTER_FULL_SCALE			= 0x20,
			CLOCK_TOU_MM_ERROR			= 0x40,
			DISK_REVERSE_DIRECTION		= 0x80
		};

        #endregion

        #region Constants
        private const int		MT2_SERIAL_NUMBER_LENGTH		= 18;
		private const int		MT2_SYSTEM_ERROR_LENGTH			= 1;
		private const int		MT2_MAX_READ_EVENTS				= 8;

		private const int MT2_MAX_DOWNLOAD_PACKET_SIZE = 16;
		private const int MT2_MAX_UPLOAD_PACKET_SIZE = 16;

		/// <summary>
		/// The following string is returned from the MeterType property.  Use this
		/// value to verify that you are communicating with a 200 series meter
		/// </summary>
        public const string MT2_TYPE = "MT200";
        private const string MT2_NAME = "200 Series";

        /// <summary>
        /// Defines the length of an energy register
        /// </summary>
        protected const int MT2_KWH_LENGTH = 7;
        
        /// <summary>
        /// The length of a demand value
        /// </summary>
        protected const int MT2_KW_LENGTH = 4;
        
        /// <summary>
        /// The length of a BCD time of occurrance value
        /// </summary>
        protected const int MT2_TOO_LENGTH = 4;
        #endregion

        #region Constructors

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objSerialPort">The communication object used for the 
		/// serial port communications.</param>
		/// <example>
		/// <code>
		/// Communication objComm = new Communication();
		/// objComm.OpenPort("COM4:");
		/// MT200 objMT200 = new MT200(objComm);
		/// </code>
		/// </example>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		public MT200(ICommunications objSerialPort) :
			base( objSerialPort ) { }

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
		/// MT200 mt200 = new MT200(scsProtocol);
		/// </code>
		/// </example>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/09/06 jrf 7.30.00  N/A   Created
		//  06/19/06 jrf 7.30.00  N/A   Changed to pass protocol object to base 
		// 								Constructor
		//  03/07/07 mah 8.00.17       Added code to set max upload and download sizes
		public MT200(SCSProtocol protocol)
			:
			base(protocol) 
		{
			protocol.MaxDownloadSize = MT2_MAX_DOWNLOAD_PACKET_SIZE;
			protocol.MaxUploadSize = MT2_MAX_UPLOAD_PACKET_SIZE;
		}

        #endregion

        #region Public Properties
        /// <summary>
        /// Provides access to the Watts Delivered Quantity
        /// </summary>
        /// <remarks>
        /// </remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/21/06 MAH 8.00.00 N/A    Adding support to get Watts Del
        //  12/13/06 jrf 8.00.00 N/A    Changing "Max" to "max" to match PC-Pro+
        //
        public Quantity WattsDelivered
        {
            get
            {
                double dblkWh;
                double dlbCumkW;
                double dblMaxkW;
                DateTime dateTOOMaxkW;

                Quantity WattsDel = new Quantity( "Watts Delivered" );

                // Start by retrieving the total energy and demand values

                GetRateEMeaurements(out dblkWh, out dlbCumkW, out dblMaxkW, out dateTOOMaxkW);

                WattsDel.TotalEnergy = new Measurement(dblkWh, "kWh");
                WattsDel.TotalMaxDemand = new DemandMeasurement(dblMaxkW, "max kW");
                WattsDel.TotalMaxDemand.TimeOfOccurance = dateTOOMaxkW;

                // Read the cummulative demand value 
                WattsDel.CummulativeDemand = new Measurement(dlbCumkW, "cum kW");
                WattsDel.ContinuousCummulativeDemand = new Measurement( WattsDel.CummulativeDemand.Value + WattsDel.TotalMaxDemand.Value, "ccum kW");

                // Next retrieve the TOU values if and only if this is a TOU meter

                if (base.TOUEnabled)
                {
                    double dblRateAWh;
                    double dblRateBWh;
                    double dblRateCWh;
                    double dblRateDWh;
                    
                    GetTOUEnergyMeasurements(out dblRateAWh, out dblRateBWh, out dblRateCWh, out dblRateDWh);

                    WattsDel.TOUEnergy = new List<Measurement>();
                    WattsDel.TOUEnergy.Add(new Measurement(dblRateAWh, "kWh Rate A"));
                    WattsDel.TOUEnergy.Add(new Measurement(dblRateBWh, "kWh Rate B"));
                    WattsDel.TOUEnergy.Add(new Measurement(dblRateCWh, "kWh Rate C"));
                    WattsDel.TOUEnergy.Add(new Measurement(dblRateDWh, "kWh Rate D"));

                    WattsDel.TOUMaxDemand = new List<DemandMeasurement>();
                    WattsDel.TOUCummulativeDemand = new List<Measurement>();
                    WattsDel.TOUCCummulativeDemand = new List<Measurement>();
                    
                    GetTOUDemandMeasurement(MT2Addresses.RATEA_KW, out dblMaxkW, out dateTOOMaxkW );
                    dlbCumkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.RATEA_CUM_KW, 4), CultureInfo.CurrentCulture);
                    Measurement CumValue = new Measurement( dlbCumkW, "cum kW Rate A" );
                    Measurement CCumValue = new Measurement( dlbCumkW + dblMaxkW, "ccum kW Rate A" );
                    DemandMeasurement MeasurementValue = new DemandMeasurement( dblMaxkW, "max kW Rate A" );
                    MeasurementValue.TimeOfOccurance = dateTOOMaxkW;
                    WattsDel.TOUMaxDemand.Add(MeasurementValue );
                    WattsDel.TOUCummulativeDemand.Add( CumValue );
                    WattsDel.TOUCCummulativeDemand.Add( CCumValue );

                    
                    GetTOUDemandMeasurement(MT2Addresses.RATEB_KW, out dblMaxkW, out dateTOOMaxkW);
                    dlbCumkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.RATEB_CUM_KW, 4), CultureInfo.CurrentCulture);
                    CumValue = new Measurement( dlbCumkW, "cum kW Rate B" );
                    CCumValue = new Measurement( dlbCumkW + dblMaxkW, "ccum kW Rate B" );
                    MeasurementValue = new DemandMeasurement(dblMaxkW, "max kW Rate B");
                    MeasurementValue.TimeOfOccurance = dateTOOMaxkW;
                    WattsDel.TOUMaxDemand.Add(MeasurementValue);
                    WattsDel.TOUCummulativeDemand.Add( CumValue );
                    WattsDel.TOUCCummulativeDemand.Add( CCumValue );

                    GetTOUDemandMeasurement(MT2Addresses.RATEC_KW, out dblMaxkW, out dateTOOMaxkW);
                    dlbCumkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.RATEC_CUM_KW, 4), CultureInfo.CurrentCulture);
                    CumValue = new Measurement( dlbCumkW, "cum kW Rate C" );
                    CCumValue = new Measurement( dlbCumkW + dblMaxkW, "ccum kW Rate C" );
                    MeasurementValue = new DemandMeasurement(dblMaxkW, "max kW Rate C");
                    MeasurementValue.TimeOfOccurance = dateTOOMaxkW;
                    WattsDel.TOUMaxDemand.Add(MeasurementValue);
                    WattsDel.TOUCummulativeDemand.Add( CumValue );
                    WattsDel.TOUCCummulativeDemand.Add( CCumValue );

                    GetTOUDemandMeasurement(MT2Addresses.RATED_KW, out dblMaxkW, out dateTOOMaxkW);
                    dlbCumkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.RATED_CUM_KW, 4), CultureInfo.CurrentCulture);
                    CumValue = new Measurement( dlbCumkW, "cum kW Rate D" );
                    CCumValue = new Measurement( dlbCumkW + dblMaxkW, "ccum kW Rate D" );
                    MeasurementValue = new DemandMeasurement(dblMaxkW, "max kW Rate D");
                    MeasurementValue.TimeOfOccurance = dateTOOMaxkW;
                    WattsDel.TOUMaxDemand.Add(MeasurementValue);
                    WattsDel.TOUCummulativeDemand.Add( CumValue );
                    WattsDel.TOUCCummulativeDemand.Add( CCumValue );
                }

                return WattsDel;
            }
        }

        /// <summary>
        /// Proves access to a list of measured quantities
        /// </summary>
        //   Revision History
        //   MM/DD/YY Who Version Issue# Description
        //   -------- --- ------- ------ ---------------------------------------------
        //   11/21/06 MAH 8.00.00 N/A    Created
        //
        override public List<Quantity> CurrentRegisters
        {
            get
            {
                List<Quantity> QuantityList = new List<Quantity>();
                Quantity Qty;

                // Add Watts Del
                Qty = WattsDelivered;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }

                return QuantityList;
            }
        }

        /// <summary>
        /// Proves access to a list of Self Reads.  Note that the 200
        /// series registers only provide a single self read.  The meter
        /// does not support a date of self read.  There is no 
        /// indicator available to tell us if the self read is present or not so we will 
        /// always return a single self read with the available energy and demand
        /// values
        /// </summary>
        //   Revision History
        //   MM/DD/YY Who Version Issue# Description
        //   -------- --- ------- ------ ---------------------------------------------
        //   11/22/06 MAH 8.00.00 N/A    Created
        // 
        override public List<QuantityCollection> SelfReadRegisters
        {
            get
            {
                List<QuantityCollection> lstSelfReads = new List<QuantityCollection>();

                QuantityCollection SelfRead1 = new QuantityCollection();

                SelfRead1.DateTimeOfReading = new DateTime(1980, 1, 1); // the default, unknown date
                SelfRead1.Quantities.Add( RetrieveSelfReadRegisters() );

                lstSelfReads.Add(SelfRead1);

                return lstSelfReads;
            }
        }

        /// <summary>
        /// Returns the number of load profile channels the meter is 
        /// currently recording
        /// </summary>
        //   Revision History
        //   MM/DD/YY Who Version Issue# Description
        //   -------- --- ------- ------ ---------------------------------------------
        //   12/05/06 MAH 8.00.00
        //
        override public int NumberLPChannels
        {
            get
            {
                // The 200 series and CENTRON only have a single channel of load profile data
                // and the meter does not have a count of the number of channels configured.  Therefore
                // if the profile run flag is turned on then we have one channel otherwise we don't have any
                if (LPRunning)
                {
                    m_NumberLPChannels.Value = 1;
                }
                else
                {
                    m_NumberLPChannels.Value = 0;
                }

                return m_NumberLPChannels.Value;
            }
        }

		/// <summary>This property gets device time from the meter.  Note, however,
		/// that the MT200 implementation does not download the time from the meter.  According
		/// to the basepage document, the real time is kept in RAM and cannot be downloaded
		/// </summary>
		/// <returns>
		/// An int representing the device time.
        /// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  03/07/07 mah 8.00.17  Overrode base implementation since time cannot always be
		//                                    downloaded from the meter in order to correct SCR #2415
		// 	
		public override DateTime DeviceTime
		{
			get
			{
				DateTime objDeviceTime;
				String strDemandMeter = m_rmStrings.GetString("MODEL_TYPE_D");

				if (ModelType == strDemandMeter)
				{
					objDeviceTime = new DateTime(1980, 1, 1, 0, 0, 0);
				}
				else
				{
					ReadDeviceTime(out objDeviceTime);
				}

				return objDeviceTime;
			}
		}

        #endregion

        #region Protected Properties
        /// <summary>This property gets the expected device type "MT2".</summary>
		/// <returns>
		/// A string representing the expected device type.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override string ExpectedDeviceType
		{
			get
			{
				return m_rmStrings.GetString("MT2_DEVICE_TYPE"); 
			}
		}

        /// <summary>This property gets the demand reset flag address.</summary>
        /// <returns>
        /// An int representing the demand reset flag address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/06 mah 7.35.00  N/A   Created
        // 	
        protected override int DemandResetFlagAddress
        {
            get
            {
                return (int)MT2Addresses.DEMAND_RESET_FLAG;
            }
        }

        /// <summary>This property gets the clear billing data flag address.</summary>
        /// <returns>
        /// An int representing the clear billing data flag address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/06 mah 7.35.00  N/A   Created
        // 	
        protected override int ClearBillingDataFlagAddress
        {
            get
            {
                return (int)MT2Addresses.CLEAR_BILLING_FLAG;
            }
        }

        /// <summary>This property gets the address of the test mode flag address.</summary>
        /// <returns>
        /// An int representing the test mode flag address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/21/06 mah 8.00.00  N/A   Created
        //
        protected override int RemoteTestModeFlagAddress
        {
            get
            {
                return (int)MT2Addresses.TEST_MODE_FLAG;
            }
        }

		/// <summary>This property gets the hang up flag address.</summary>
		/// <returns>
		/// An int representing the hang up flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override int CommunicationsHangUpFlagAddress
		{
			get
			{
				return (int)MT2Addresses.HANGUP_FLAG; 
			}
		}

		/// <summary>This property gets the stop clock flag address.</summary>
		/// <returns>
		/// An int representing the stop clock flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int StopClockFlagAddress
		{
			get
			{
				return (int)MT2Addresses.CLOCK_RUN_FLAG; 
			}
		}

		/// <summary>This property gets the TOU run flag address.</summary>
		/// <returns>
		/// An int representing the TOU run flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override int TOURunFlagAddress
		{
			get
			{
				return (int)MT2Addresses.TOU_RUN_FLAG; 
			}		
		}

		/// <summary>This property gets the clock reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the clock reconfigure flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int ClockReconfigureFlagAddress
		{
			get
			{
				return (int)MT2Addresses.CLOCK_RECONFIGURE_FLAG; 
			}
		}

		/// <summary>This property gets the TOU reconfigure flag address.
		/// </summary>
		/// <returns>
		/// An int representing the TOU reconfigure flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int TOUReconfigureFlagAddress
		{
			get
			{
				return (int)MT2Addresses.TOU_RECONFIGURE_FLAG; 
			}
		}

		/// <summary>This property gets the real time clock address.</summary>
		/// <returns>
		/// An int representing the real time clock address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override int RealTimeClockAddress
		{
			get
			{
				return (int)MT2Addresses.REAL_TIME; 
			}
		}

		/// <summary>This property gets the model type address.</summary>
		/// <returns>
		/// An int representing the model type address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int ModelTypeAddress
		{
			get
			{
				return (int)MT2Addresses.MODEL_TYPE; 
			}
		}

        /// <summary>This property gets the last reset date address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/06 mah 7.35.00  N/A   Created
        // 
        protected override int LastResetDateAddress
        {
            get
            {
                return (int)MT2Addresses.LAST_RESET_DATE;
            }
        }

        /// <summary>This property gets the demand reset count address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/06 mah 7.35.00  N/A   Created
        // 
        protected override int NumResetsAddress
        {
            get
            {
                return (int)MT2Addresses.RESET_COUNT;
            }
        }

        /// <summary>
        /// This property gets the address of the display format bit pattern.
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/07/06 mah 8.00.00  N/A   Created
        //
        protected override int EnergyFormatAddress
        {
            get
            {
                return (int)MT2Addresses.ENERGYFORMAT_ADDRESS;
            }
        }

        /// <summary>
        /// This property gets the address of the meter's display options.
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/07/06 mah 8.00.00  N/A   Created
        //
        protected override int DisplayOptionsAddress
        {
            get
            {
                return (int)MT2Addresses.DISPLAY_OPTIONS;
            }
        }

        /// <summary>This property gets the outage count address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/06 mah 7.35.00  N/A   Created
        // 
        protected override int NumOutagesAddress
        {
            get
            {
                return (int)MT2Addresses.OUTAGE_COUNT;
            }
        }

        /// <summary>This property gets the last programmed date address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/06 mah 7.35.00  N/A   Created
        // 
        protected override int LastProgrammedDateAddress
        {
            get
            {
                return (int)MT2Addresses.LAST_PROGRAMMED_DATE;
            }
        }

        /// <summary>This property gets the address of the program count.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/06 mah 7.35.00  N/A   Created
        // 
        protected override int NumTimesProgrammedAddress
        {
            get
            {
                return (int)MT2Addresses.PROGRAM_COUNT;
            }
        }

        /// <summary>This property returns the address of the number of minutes the 
        /// device was on battery power.
        /// </summary>
        /// <returns>
        /// An int representing the basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------  
        //  09/08/06 mah 7.35.00  N/A   Created
        protected override int NumOfMinutesOnBatteryAddress
        {
            get
            {
                return (int)MT2Addresses.MINUTES_ON_BATTERY;
            }
        }

        /// <summary>
        /// This property gets the address of the first byte of demand 
        /// configuration data
        /// </summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/17/06 mah 8.00.00  N/A   Created
        //
        protected override int DemandConfigurationAddress
        {
            get
            {
                return (int)MT2Addresses.DEMAND_CONFIGURATION;
            }
        }


		/// <summary>This property gets the operating setup address.</summary>
		/// <returns>
		/// An int representing the operating setup address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override int OperatingSetupAddress
		{
			get
			{
				return (int)MT2Addresses.OPERATING_SETUP; 
			}
		}

		/// <summary>This property gets the firmware version address.</summary>
		/// <returns>
		/// An int representing the firmware version address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int FWVersionAddress
		{
			get
			{
				return (int)MT2Addresses.FIRMWARE_REVISION; 
			}
		}

		/// <summary>This property gets the software version address.</summary>
		/// <returns>
		/// An int representing the software version address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 	
		protected override int SWVersionAddress
		{
			get
			{
				return (int)MT2Addresses.SOFTWARE_REVISION; 
			}
		}	 	

		/// <summary>This property gets the program ID address.</summary>
		/// <returns>
		/// An int representing the program ID address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int ProgramIDAddress
		{
			get
			{
				return (int)MT2Addresses.PROGRAM_ID; 
			}
		}

        /// <summary>This property gets the address of the TOU Schedule ID.</summary>
        /// <returns>
        /// An int representing the TOU Schedule ID address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/14/06 mah 8.00.00  N/A   Created
        //
        // 	
        protected override int TOUScheduleIDAddress
        {
            get
            {
                return (int)MT2Addresses.TOU_SCHEDULE_ID;
            }
        }

		/// <summary>This property gets the TOU calendar address.</summary>
		/// <returns>
		/// An int representing the TOU calendar address.
		/// </returns>
		/// <exception cref="SCSException">
		/// Throws SCSException if an error occurs while reading the TOU address
		/// </exception>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A   Created
		//  05/30/06 jrf 7.30.00  N/A	Modified
		// 
		protected override int TOUCalendarAddress
		{
			get
			{
                if (!m_touCalendarStartAddress.Cached)
                {
                    byte[] byAddress = new byte[SCS_TOU_CALENDAR_LENGTH];
                    SCSProtocolResponse Result;

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, 
                        "Read TOU Base Address");
                    Result = m_SCSProtocol.Upload((int)MT2Addresses.TOU_BASE,
                        SCS_TOU_CALENDAR_LENGTH, out byAddress);

                    if (SCSProtocolResponse.SCS_ACK == Result)
                    {
                        m_touCalendarStartAddress.Value = byAddress[0] * 0x100 + 
                            byAddress[1];
                    }
                    else
                    {
                        throw new SCSException(SCSCommands.SCS_U, Result,
					        (int)MT2Addresses.TOU_BASE,
                            m_rmStrings.GetString("TOU_BASE_ADDRESS"));
                    }
                }
				return m_touCalendarStartAddress.Value; 
			}
		}

		/// <summary>This property gets the TOU Yearly Schedule address</summary>
		/// <returns>
		/// An int representing the TOU address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/06/06 mcm 7.30.00  N/A   Created
		// 		
		protected override int YearlyScheduleAddress
		{
			get
			{
				return (int)MT2Addresses.YEARLY_SCHEDULE;
			}
		}
	
		/// <summary>This property gets the Last Season data address.
		/// Not all have this item in their base page.</summary>
		/// <returns>
		/// An int representing the TOU address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/10/06 mcm 7.30.00  N/A   Created
		// 		
		protected override int LastSeasonDataAddress
		{
			get
			{
				return (int)MT2Addresses.LAST_SEASON_REGISTERS;
			}
		}
		
		/// <summary>This property gets the communication timeout address.</summary>
		/// <returns>
		/// An int representing the communication timeout address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/30/06 jrf 7.30.00  N/A	Created
		// 
		protected override int CommunicationTimeoutAddress
		{
			get
			{
				return (int)MT2Addresses.COMMUNICATIONS_TIMEOUT; 
			}
		}

        /// <summary>This property gets the cold load pickup time address.</summary>
        /// <returns>
        /// An int representing a basepage address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/17/06 mah 8.00.00  N/A   Created
        //
        protected override int ColdLoadPickupTimeAddress
        {
            get
            {
                return (int)MT2Addresses.COLD_LOAD_PICKUP_TIME;
            }
        }


		/// <summary>This property gets the interval length address.</summary>
		/// <returns>
		/// An int representing the interval length address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/30/06 jrf 7.30.00  N/A	Created
		// 
		protected override int IntervalLengthAddress
		{
			get
			{
				return (int)MT2Addresses.MM_INTERVAL_LENGTH; 
			}
		}

		/// <summary>This property gets the serial number address.</summary>
		/// <returns>
		/// An int representing the serial number address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/30/06 jrf 7.30.00  N/A	Created
		// 
		protected override int SerialNumberAddress
		{
			get
			{
				return (int)MT2Addresses.SERIAL_NUMBER; 
			}
		}

		/// <summary>This property gets the serial number length.</summary>
		/// <returns>
		/// An int representing the serial number length.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/30/06 jrf 7.30.00  N/A	Created
		// 
		protected override int SerialNumberLength
		{
			get
			{
				return MT2_SERIAL_NUMBER_LENGTH; 
			}
		}
		/// <summary>This property gets the Firmware Options address.</summary>
		/// <returns>
		/// An int representing the Firmware Options address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  07/04/06 mcm 7.30.00  N/A   Created
		// 				
		protected override int FirmwareOptionsAddress
		{
			get
			{
				return (int)MT2Addresses.FIRMWARE_OPTIONS;
			}
		}


		/// <summary>This property gets the unit ID address.</summary>
		/// <returns>
		/// An int representing the unit ID address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  04/27/06 mrj 7.30.00  N/A	Created
		//  05/30/06 jrf 7.30.00  N/A	Created
		// 
		protected override int UnitIDAddress
		{
			get
			{
				return (int)MT2Addresses.UNIT_ID; 
			}
		}

        /// <summary>This property gets the address of the first user defined data field.</summary>
        /// <returns>
        /// An int representing the unit ID address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/16/06 mah 8.00.00  N/A	Created
        //
        protected override int UserDataBlockAddress
        {
            get
            {
                return (int)MT2Addresses.USERDEFINED_FIELD1;
            }
        }

        /// <summary>This property gets the address of the start of the display table.</summary>
        /// <returns>
        /// An int representing the first display item address.
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/05/06 mah 8.00.00  N/A   Created
        // 
        // 	
        protected override int DisplayTableAddress
        {
            get
            {
                return (int)MT2Addresses.DISPLAY_TABLE;
            }
        }

        /// <summary>
        /// This method stops and starts the metering operation of the MT200
        /// </summary>
        /// <param name="disableMeter">The boolean to determine if the meter needs
        /// to be disabled or enabled</param>
        /// <returns>A SCSProtocolResponse</returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/07/07 KRC 8.00.10        Add stop metering for Edit registers.
        //  
        override protected SCSProtocolResponse StopMetering(bool disableMeter)
        {
            SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;
            byte[] abytFlag = new byte[1];

            if (disableMeter)
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Stopping Metering");
                abytFlag[0] = SCS_FLAG_ON;
            }
            else
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Starting Metering");
                abytFlag[0] = SCS_FLAG_OFF;
            }

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Set Stop Meter Flag");
            objProtocolResponse = m_SCSProtocol.Download(
                                    (int)MT2Addresses.STOP_METER_FLAG,
                                    SCS_FLAG_LENGTH,
                                    ref abytFlag);

            return objProtocolResponse;
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
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/31/06 jrf 7.30.00  N/A   Created
        //  03/16/07 jrf 8.00.18        Changed from resource string to constant
		// 	
		public override string MeterType
		{
			get
			{
				return MT2_TYPE;
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
                return MT2_NAME;
            }
        }

		/// <summary>
		/// This property gets the load profile run flag address.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/31/06 jrf 7.30.00  N/A   Created
		// 		
		protected override int LoadProfileFlagAddress
		{
			get 
			{
				return (int)MT2Addresses.MM_RUN_FLAG;
			}
		}

		/// <summary>
		/// This property gets the load profile adjust time flag address.
		/// </summary>
		/// <returns>
		/// An int representing the load profile run flag.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/31/06 jrf 7.30.00  N/A   Created
		// 		
		protected override int LoadProfileAdjustTimeFlagAddress
		{
			get
			{
				return (int)MT2Addresses.MM_AJUST_TIME_FLAG;
			}
		}

		/// <summary>
		/// This property gets the tou expiration address.
		/// </summary>
		/// <returns>
		/// An int representing the tou expiration address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/12/06 jrf 7.30.00  N/A   Created
		// 		
		protected override int TOUExpirationAddress
		{
			get
			{
				return (int)MT2Addresses.TOU_EXPIRATION_DATE;
			}
		}

		/// <summary>This property gets the clock run flag address.</summary>
		/// <returns>
		/// An int representing the clock run flag address.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/13/06 jrf 7.30.00  N/A   Created
		// 
		protected override int ClockRunFlagAddress
		{
			get
			{
				return (int)MT2Addresses.CLOCK_RUN_FLAG;
			}
		}

		/// <summary>This property gets the maximum number of TOU calendar 
		/// events to read from an SCS device.</summary>
		/// <returns>
		/// An int representing the number of events.
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/30/06 jrf 7.30.00  N/A   Created
		// 	
		protected override int MaxReadEvents
		{
			get
			{
				return MT2_MAX_READ_EVENTS;
			}
		}

        /// <summary>Returns the address of the primary security code.</summary>
        /// 
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/16/06 mcm 7.35.00  N/A   Created
        // 				
        protected override int PrimaryPasswordAddress
        {
            get
            {
                return (int)MT2Addresses.PRIMARY_PASSWORD;
            }
        }

        /// <summary>Returns the address of the Secondary security code.</summary>
        /// 
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/17/06 mcm 7.35.00  N/A   Created
        // 				
        protected override int SecondaryPasswordAddress
        {
            get
            {
                return (int)MT2Addresses.SECONDARY_PASSWORD;
            }
        }

        /// <summary>The MTR 200 doesn't have a tertiary password, but the 
        /// CENTRON does.  Return 0</summary>
        /// 
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/17/06 mcm 7.35.00  N/A   Created
        // 				
        protected override int TertiaryPasswordAddress
        {
            get
            {
                return UNSUPPORTED_ADDRESS;
            }
        }

        /// <summary>This meter does not have a tertiary password.</summary>
        /// 
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/17/06 mcm 7.30.00  N/A   Created
        // 		
        protected override bool HasTertiaryPassword
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// This method verifies that the derived device type matches 
		/// the SCS device's type 
		/// </summary>
		/// <returns>
		/// a boolean indicating whether or not the device type is correct
		/// </returns>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  06/19/06 jrf 7.30.00  N/A   Created
		//  
		protected override bool VerifyDeviceType()
		{
			return ( DeviceType == ExpectedDeviceType );
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
				return (int)MT2Addresses.TRANSFORMER_RATIO;
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
                return (int)MT2Addresses.LOAD_RESEARCH_ID;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
		/// This gets the errors from the meter and converts them to an array 
		/// of strings.
		/// </summary>
		/// <returns>
		/// A string array that contains the errors
		/// </returns>
		/// <exception cref="SCSException">
		/// Thrown if a protocol error occurs while reading the error flags
		/// </exception>
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  05/31/06 jrf 7.30.00  N/A   Created
        // 
		protected override void ReadErrors(out string[] astrErrors)
		{
			byte[] abytErrors;
			ArrayList objErrorList = new ArrayList();
			SCSProtocolResponse objProtocolResponse = 
				SCSProtocolResponse.NoResponse;

			m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Read System Errors" );
			// Get the system errors
			objProtocolResponse = m_SCSProtocol.Upload( 
				(int)MT2Addresses.SYSTEM_ERROR, 
				MT2_SYSTEM_ERROR_LENGTH, 
				out abytErrors);

			if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
			{
				// Check for each error and add to list as appropriate
				if (0 != (abytErrors[0] & (byte)MT2Errors.LOW_BATTERY)) 
				{
					objErrorList.Add(m_rmStrings.GetString("LOW_BATTERY"));
				}
				if (0 != (abytErrors[0] & (byte)MT2Errors.REGISTER_FULL_SCALE)) 
				{
					objErrorList.Add(m_rmStrings.GetString("REGISTER_FULL_SCALE"));
				}
				if (0 != (abytErrors[0] & (byte)MT2Errors.CLOCK_TOU_MM_ERROR)) 
				{
					objErrorList.Add(m_rmStrings.GetString("CLOCK_TOU_MM_ERROR"));
				}
				if (0 != (abytErrors[0] & (byte)MT2Errors.DISK_REVERSE_DIRECTION)) 
				{
					objErrorList.Add(m_rmStrings.GetString("DISK_REVERSE_DIRECTION"));
				}
			}
			else
			{
				SCSException objSCSException = new SCSException(
					SCSCommands.SCS_U, 
					objProtocolResponse, 
					(int)MT2Addresses.SYSTEM_ERROR, 
					m_rmStrings.GetString("SYSTEM_ERRORS"));
				throw objSCSException;
			}

			astrErrors = (string[])objErrorList.ToArray(typeof(string));
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves all of the self read registers supported by the 200 series register.  It is
        /// important to recognize that this is a subset of the total available registers.  Also note that
        /// values will be returned whether or not a self read was actually performed by the meter.
        /// </summary>
        /// <returns>
        /// A quantity object representing Wh delivered values captured at the last
        /// self read
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        //  12/13/06 jrf 8.00.00  N/A   Changed "Max" to "max" to match PC-Pro+
        // 
        private Quantity RetrieveSelfReadRegisters()
        {
            double dblkWh;
            double dblMaxkW;

            // Create a quantity object to hold the register readings
            Quantity WattsDelivered = new Quantity("Watts Delivered");

            // Start by retrieving the energy and max demand values for the total rate
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Rate E Measurements");

            dblkWh = double.Parse( ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEE_KWH, 3), CultureInfo.CurrentCulture );
            dblMaxkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEE_KW, 3), CultureInfo.CurrentCulture);

            WattsDelivered.TotalEnergy = new Measurement(dblkWh, "kWh");
            WattsDelivered.TotalMaxDemand = new DemandMeasurement (dblMaxkW, "max kW");

            // Next retrieve the TOU self read values if and only if this is a TOU meter
            if (base.TOUEnabled)
            {
                // Create measurement lists for both the energy and max demand values.  Note that 
                // we will be adding one measurement for each of the four available rates

                WattsDelivered.TOUEnergy = new List<Measurement>();
                WattsDelivered.TOUMaxDemand = new List<DemandMeasurement>();

                // Go get the data for the first rate
                dblkWh = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEA_KWH, 3), CultureInfo.CurrentCulture);
                dblMaxkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEA_KW, 3), CultureInfo.CurrentCulture);

                WattsDelivered.TOUEnergy.Add(new Measurement(dblkWh, "kWh Rate A"));
                WattsDelivered.TOUMaxDemand.Add(new DemandMeasurement(dblMaxkW, "max kW Rate A"));

                 // On to rate B
                dblkWh = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEB_KWH, 3), CultureInfo.CurrentCulture);
                dblMaxkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEB_KW, 3), CultureInfo.CurrentCulture);

                WattsDelivered.TOUEnergy.Add(new Measurement(dblkWh, "kWh Rate B"));
                WattsDelivered.TOUMaxDemand.Add(new DemandMeasurement(dblMaxkW, "max kW Rate B"));

                // On to rate C
                dblkWh = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEC_KWH, 3), CultureInfo.CurrentCulture);
                dblMaxkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATEC_KW, 3), CultureInfo.CurrentCulture);

                WattsDelivered.TOUEnergy.Add(new Measurement(dblkWh, "kWh Rate C"));
                WattsDelivered.TOUMaxDemand.Add(new DemandMeasurement(dblMaxkW, "max kW Rate C"));

                // On to rate D
                dblkWh = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATED_KWH, 3), CultureInfo.CurrentCulture);
                dblMaxkW = double.Parse(ReadFloatingBCDValue((int)MT2Addresses.SELF_READ_RATED_KW, 3), CultureInfo.CurrentCulture);

                WattsDelivered.TOUEnergy.Add(new Measurement(dblkWh, "kWh Rate D"));
                WattsDelivered.TOUMaxDemand.Add(new DemandMeasurement(dblMaxkW, "max kW Rate D"));
            }                    

            return WattsDelivered;
        }


        /// <summary>
        /// This method retrieves the energy and demand values for the
        /// total (rate E) register.
        /// </summary>
        /// <param name="dblkWh"></param>
        ///  <param name="dblCumkW"></param>
        ///  <param name="dblMaxkW"></param>
        /// <param name="dateTOOMaxkW"></param>
		/// <exception cref="SCSException">
		/// Thrown if a protocol error occurs while reading the value
		/// </exception>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private void GetRateEMeaurements(out double dblkWh, out double dblCumkW, out double dblMaxkW, out DateTime dateTOOMaxkW)
        {
            byte[] abytMeasurements;
            SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;

            // Initialize the output values to a meaningful value just in case something goes wrong

            dblMaxkW = 0.0;
            dblCumkW = 0.0;
            dblkWh = 0.0;

            dateTOOMaxkW = DateTime.Now;

            // Prepare to read the values as a block - this is much more efficent than reading each quantity individually

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Rate E Measurements");

            objProtocolResponse = m_SCSProtocol.Upload(
                                                    (int)MT2Addresses.RATEE_KWH,
                                                    MT2_KWH_LENGTH + MT2_KW_LENGTH + MT2_KW_LENGTH + MT2_TOO_LENGTH,
                                                    out abytMeasurements);

            if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
            {
                dblkWh = ExtractEnergyReading(ref abytMeasurements, 0);
                
                // the second value in this block is the cummulative demand

                int nStartIndex = MT2_KWH_LENGTH;
                int nFieldLength = MT2_KW_LENGTH;

                byte[] abytCumDemand = new byte[ nFieldLength ];
                                
                for ( int nByteIndex = 0; nByteIndex < nFieldLength; nByteIndex++ )
                {
                    abytCumDemand[ nByteIndex ] = abytMeasurements[ nStartIndex + nByteIndex ];
                }

                dblCumkW = BCD.FloatingBCDtoDouble(ref abytCumDemand, MT2_KW_LENGTH);

                // the third value in the block is the max demand & the fourth value is the time of occurrence

                dblMaxkW = ExtractDemandMeasurement(ref abytMeasurements, MT2_KWH_LENGTH + MT2_KW_LENGTH);
                dateTOOMaxkW = ExtractTOO(ref abytMeasurements, MT2_KWH_LENGTH + MT2_KW_LENGTH + MT2_KW_LENGTH );
            }
            else
            {
                SCSException objSCSException = new SCSException(
                                                SCSCommands.SCS_U,
                                                objProtocolResponse,
                                                (int)MT2Addresses.RATEE_KWH,
                                                "Rate E Measurement Block" );

                throw objSCSException;
            }
        }

        /// <summary>
        /// This method returns the kWh values for each of the four TOU rates supported by the MT200 series register
        /// </summary>
        /// <param name="dblRateAkWh">kWH for Rate A</param>
        /// <param name="dblRateBkWh">kWH for Rate B</param>
        /// <param name="dblRateCkWh">kWH for Rate C</param>
        /// <param name="dblRateDkWh">kWH for Rate D</param>
		/// <exception cref="SCSException">
		/// Thrown if a protocol error occurs while reading the quantity
		/// </exception>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private void GetTOUEnergyMeasurements(out double dblRateAkWh, out double dblRateBkWh, out double dblRateCkWh, out double dblRateDkWh)
        {
            byte[] abytMeasurements;
            SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;

            // Initialize the output values to a meaningful value just in case something goes wrong

            dblRateAkWh = 0.0;
            dblRateBkWh = 0.0;
            dblRateCkWh = 0.0;
            dblRateDkWh = 0.0;

            // Prepare to read the values as a block - this is much more efficent than reading each quantity individually

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading TOU Energy Measurements");

            objProtocolResponse = m_SCSProtocol.Upload(
                                                    (int)MT2Addresses.RATEA_KWH,
                                                    MT2_KWH_LENGTH * 4,
                                                    out abytMeasurements);

            if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
            {
                dblRateAkWh = ExtractEnergyReading(ref abytMeasurements, 0);
                dblRateBkWh = ExtractEnergyReading(ref abytMeasurements, MT2_KWH_LENGTH);
                dblRateCkWh = ExtractEnergyReading(ref abytMeasurements, MT2_KWH_LENGTH * 2);
                dblRateDkWh = ExtractEnergyReading(ref abytMeasurements, MT2_KWH_LENGTH * 3);
            }
            else
            {
                SCSException objSCSException = new SCSException(
                                                SCSCommands.SCS_U,
                                                objProtocolResponse,
                                                (int)MT2Addresses.RATEA_KWH,
                                                "TOU Energy Values");

                throw objSCSException;
            }
        }

        /// <summary>
        /// This method retrieves a maximum demand value and associated time of occurrance value
        /// from the register.  This method takes advantage of the fact that all TOU demand measurements
        /// store the max demand and TOO value contiguously.  Therefore we can read them in a single 
        /// operation rather than forcing two separate reads.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="dblMaxkW"></param>
        /// <param name="dateTOOMaxkW"></param>
		/// <exception cref="SCSException">
		/// Thrown if a protocol error occurs while reading the error flags
		/// </exception>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private void GetTOUDemandMeasurement(MT2Addresses address, out double dblMaxkW, out DateTime dateTOOMaxkW)
        {
            byte[] abytMeasurements;
            SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;

            // Initialize the output values to a meaningful value just in case something goes wrong

            dblMaxkW = 0.0;
            dateTOOMaxkW = DateTime.Now;

            // Prepare to read the values as a block - this is much more efficent than reading each quantity individually

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading demand value");

            objProtocolResponse = m_SCSProtocol.Upload(
                                                    (int)address,
                                                    MT2_KW_LENGTH + MT2_TOO_LENGTH,
                                                    out abytMeasurements);

            if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
            {
                dblMaxkW = ExtractDemandMeasurement(ref abytMeasurements, 0);
                dateTOOMaxkW = ExtractTOO(ref abytMeasurements, MT2_KW_LENGTH );
            }
            else
            {
                SCSException objSCSException = new SCSException(
                                                SCSCommands.SCS_U,
                                                objProtocolResponse,
                                                (int)address,
                                                "Demand Reading");

                throw objSCSException;
            }
        }

        /// <summary>
        /// This method extracts and translates a 4 byte floating BCD demand value from the given
        /// byte array
        /// </summary>
        /// <param name="abytMeasurements"></param>
        /// <param name="nStartIndex"></param>
        /// <returns>
        /// A double precision floating point representation of the demand value
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private static double ExtractDemandMeasurement(ref byte[] abytMeasurements, int nStartIndex)
        {
            int nFieldLength = MT2_KW_LENGTH;

            byte[] abytMaxDemand = new byte[MT2_KW_LENGTH];

            for (int nByteIndex = 0; nByteIndex < nFieldLength; nByteIndex++)
            {
                abytMaxDemand[nByteIndex] = abytMeasurements[nStartIndex + nByteIndex];
            }

            return BCD.FloatingBCDtoDouble(ref abytMaxDemand, MT2_KW_LENGTH);
        }

        /// <summary>
        /// This method extracts and translates a 4 byte time of occurrence timestamp from the given
        /// byte array.  The timestamp must be in the following format MM-DD-HH-mm
        /// </summary>
        /// <param name="abytMeasurements"></param>
        /// <param name="nStartIndex"></param>
        /// <returns>
        /// A datetime object representing the translated date.  Note that since the year is not 
        /// included in the original object, the year is assumed to be in either the current or the 
        /// previous year
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private static DateTime ExtractTOO(ref byte[] abytMeasurements, int nStartIndex)
        {
            int nFieldLength = MT2_TOO_LENGTH;

            byte[] abytTOOMaxDemand = new byte[MT2_TOO_LENGTH];

            for (int nByteIndex = 0; nByteIndex < nFieldLength; nByteIndex++)
            {
                abytTOOMaxDemand[nByteIndex] = abytMeasurements[nStartIndex + nByteIndex];
            }

            return BCD.GetDateTime(ref abytTOOMaxDemand, BCD.BCDDateTimeFormat.MoDaHrMi);
        }


        /// <summary>
        /// This method extracts and translates a 7 byte fixed BCD energy value from the given
        /// byte array
        /// </summary>
        /// <param name="abytMeasurement"></param>
        /// <param name="nStartIndex"></param>
        /// <returns>
        /// A double precision floating point representation of the energy value
        /// </returns>
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/22/06 mah 8.00.00  N/A   Created
        // 
        private static double ExtractEnergyReading(ref byte[] abytMeasurement, int nStartIndex)
        {
            int nFieldLength = MT2_KWH_LENGTH;

            byte[] abytkWH = new byte[MT2_KWH_LENGTH];

            for (int nByteIndex = 0; nByteIndex < nFieldLength; nByteIndex++)
            {
                abytkWH[nByteIndex] = abytMeasurement[nStartIndex + nByteIndex];
            }

            return (double)BCD.FixedBCDtoFloat(ref abytkWH);
        }

        #endregion
    
    }
}
