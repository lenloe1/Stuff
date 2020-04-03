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
//                           Copyright © 2006 - 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
	/// <summary>
	/// Class representing the CENTRON meter.
	/// </summary>
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------
	//  04/27/06 mrj 7.30.00  N/A	Created
	//  05/31/06 jrf 7.30.00  N/A	Modified
    //  04/02/07 AF  8.00.23  2814  Corrected the capitalization of the meter name
    //
    public partial class CENTRON : MT200
    {
        #region Constants

		private const int CENTRON_MAX_DOWNLOAD_PACKET_SIZE = 16;
		private const int CENTRON_MAX_UPLOAD_PACKET_SIZE = 64;

        private const string CENTRON_TYPE = "CENTRON";
        private const string CENTRON_NAME = "CENTRON";

        #endregion

        #region Definitions

        private enum CENTRONAddresses : int
        {
            RATEE_REVERSE_KWH = 0x0551
        };

        #endregion

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="objSerialPort">The communication object used for the 
		/// serial port communications.</param>
		/// <example>
		/// <code>
		/// Communication objComm = new Communication();
		/// objComm.OpenPort("COM4:");
		/// CENTRON objCENTRON = new CENTRON(objComm);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/27/06 mrj 7.30.00  N/A   Created
		/// 05/31/06 jrf 7.30.00  N/A	Modified
		///
        public CENTRON(ICommunications objSerialPort) :
            base( objSerialPort )       
        {}

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
		/// CENTRON centron = new CENTRON(scsProtocol);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 jrf 7.30.00  N/A   Created
		/// 06/19/06 jrf 7.30.00  N/A   Changed to pass protocol object to base 
		///								Constructor
		///
		public CENTRON(SCSProtocol protocol)
			:
			base(protocol)
		{
			protocol.MaxDownloadSize = CENTRON_MAX_DOWNLOAD_PACKET_SIZE;
			protocol.MaxUploadSize = CENTRON_MAX_UPLOAD_PACKET_SIZE;
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
				return CENTRON_TYPE;
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
                return CENTRON_NAME;
            }
        }


		/// <summary>This property gets device time from the meter.</summary>
		/// <returns>
		/// A date/time representing the device time.
		/// </returns>
		// Revision History
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 03/21/07 mah 8.00.20  N/A   Created
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

        /// <summary>
        /// Property used to get the meter type as used within a MIF file.  Note
        /// that with some devices this is different from the normal meter type
        /// </summary>
        override protected string MIFType
        {
            get
            {
                return "MT2";
            }
        }

        /// <summary>
        /// Provides access to the Watts Received Quantity
        /// </summary>
		/// <exception cref="SCSException">
		/// Throws SCSException if an error occurs while reading the quantity
		/// </exception>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  11/21/06 MAH 8.00.00 N/A    Adding support to get Watts Rec
        /// </remarks>
        public Quantity WattsReceived
        {
            get
            {
                double dblkWh;
                byte[] abytWhReceived;
                SCSProtocolResponse objProtocolResponse = SCSProtocolResponse.NoResponse;

                // The CENTRON only supports kWh received.  No demand values are available
                // so we only have to retrieve one measurement and set the demand and TOU
                // measurements to null to indicate that they are not available

                // Prepare to read the values as a block - this is much more efficent than reading each quantity individually

                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Reading Rate E Wh Received");

                objProtocolResponse = m_SCSProtocol.Upload(
                                                        (int)CENTRONAddresses.RATEE_REVERSE_KWH,
                                                        MT2_KWH_LENGTH,
                                                        out abytWhReceived);

                if (SCSProtocolResponse.SCS_ACK == objProtocolResponse)
                {
                    dblkWh = (double)BCD.FixedBCDtoFloat(ref abytWhReceived);
                }
                else
                {
                    SCSException objSCSException = new SCSException(
                                                    SCSCommands.SCS_U,
                                                    objProtocolResponse,
                                                        (int)CENTRONAddresses.RATEE_REVERSE_KWH,
                                                    "Rate E Wh Received");

                    throw objSCSException;
                }

                // Now build the quantity structure

                Quantity WattsRec = new Quantity("Watts Received");

                WattsRec.TotalEnergy = new Measurement(dblkWh, "kWh r");
                WattsRec.TotalMaxDemand = null;

                return WattsRec;
            }
        }

        /// <summary>
        /// Proves access to a list of measured quantities
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  11/21/06 MAH 8.00.00 N/A    Created
        /// </remarks>
        override public List<Quantity> CurrentRegisters
        {
            get
            {
                List<Quantity> QuantityList = base.CurrentRegisters;
                
                Quantity Qty;

                // Add Watts Rec
                Qty = WattsReceived;
                if (null != Qty)
                {
                    QuantityList.Add(Qty);
                }

                return QuantityList;
            }
        }

        /// <summary>
        /// The MTR 200 doesn't have a tertiary password, but the 
        /// CENTRON does.  Return 0
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.35.00  N/A   Created
        ///	</remarks>			
        protected override int TertiaryPasswordAddress
        {
            get
            {
                return (int)CENAddresses.TERTIARY_PASSWORD;
            }
        }

        /// <summary>This meter does have a tertiary password.</summary>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/17/06 mcm 7.30.00  N/A   Created
        ///		
        protected override bool HasTertiaryPassword
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// This method stops and starts the metering operation of the CENTRON
        /// </summary>
        /// <param name="disableMeter">The boolean to determine if the meter needs
        /// to be disabled or enabled</param>
        /// <returns>A SCSProtocolResponse</returns>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/26/07 KRC 8.00.09        Add stop metering for Edit registers.
        /// 
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
		/// MT2Addresses enumeration encapsulates the MT200 basepage addresses.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/30/06 jrf 7.30.00 N/A	Created
		/// 
        private enum CENAddresses : int
        {
            TERTIARY_PASSWORD           = 0x01ED,
        }
    }
}
