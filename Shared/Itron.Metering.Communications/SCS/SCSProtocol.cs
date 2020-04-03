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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Threading;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications;

namespace Itron.Metering.Communications.SCS
{
	/// <summary>
	/// SCSProtocolResponse enumeration encapsulates the SCS Protocol responses.
	/// </summary>
    /// <remarks>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ -------------------------------------------
	/// 04/03/06 mrj 7.30.00 N/A	Created
    /// </remarks>
    public enum SCSProtocolResponse : byte
    {
		/// <summary>
		/// NoResponse = 0,
		/// </summary>
        NoResponse = 0,
		/// <summary>
		/// SCS_ACK = 0x06,  (acknowledge)
		/// </summary>
        SCS_ACK = 0x06,
		/// <summary>
		/// SCS_NAK = 0x15,  (negative acknowledge)
		/// </summary>
        SCS_NAK = 0x15,
		/// <summary>
		/// SCS_CAN = 0x18,  (Cancel, security error)
		/// </summary>
        SCS_CAN = 0x18
    };

	/// <summary>
	/// SCSCommands enumeration encapsulates the SCS Protocol commands.
	/// </summary>
    /// <remarks>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 04/03/06 mrj 7.30.00 N/A	Created
    /// </remarks>
    public enum SCSCommands : byte
    {
		/// <summary>
		/// SCS_I = 0x49, (Identify)
		/// </summary>
        [EnumDescription("Identify")]
        SCS_I = 0x49,
		/// <summary>
		/// SCS_S = 0x53, (Security)
		/// </summary>
        [EnumDescription("Security")]
        SCS_S = 0x53,
		/// <summary>
		/// SCS_U = 0x55, (Upload)
		/// </summary>
        [EnumDescription("Upload")]
        SCS_U = 0x55,
		/// <summary>
		/// SCS_D = 0x44, (Download)
		/// </summary>
        [EnumDescription("Download")]
        SCS_D = 0x44,
		/// <summary>
		/// SCS_ENQ = 0x05, (Enquire)
		/// </summary>
        [EnumDescription("Enquire")]
        SCS_ENQ = 0x05
    };

	/// <summary>
	/// Class which handles all SCS Protocol communications.
	/// </summary>
    /// <remarks>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 04/03/06 mrj 7.30.00  N/A	Created
	/// 05/10/06 jrf 7.30.00  N/A	Modified
	/// 06/06/06 jrf 7.30.00  N/A   Add in Logger Code
    /// </remarks>
	public class SCSProtocol
    {
		// Member Variables
        /// <summary>
        /// Communication object
        /// </summary>
		public ICommunications m_CommPort;

        #region Private and Protected Variables

        private uint m_uiMaxRetries;
		private uint m_uiMinReceiveTimeout;
		private uint m_uiMaxUploadSize;
        private uint m_uiMaxDownloadSize;
        private ManualResetEvent m_ReadEvent = null;
		private bool m_bTimedout;
		private Logger m_hLogFile;

        // If the device has been identified, save the data. Note that if you
        // change device (daisy chaining, etc) you will need to re-identify it.
		private bool m_bIdentified;
        private string m_strDeviceID; 
        private string m_strDeviceType; 
        private int m_iMeterStartAddress; 
        private int m_iMeterStopAddress;

		//Members to support resource strings:
		private static readonly string RESOURCE_FILE_PROJECT_STRINGS = 
		 "Itron.Metering.Communications.SCS.SCSStrings";
		/// <summary>
		/// Resourse Manager object that supports extracting strings from the 
		/// resourse file.
		/// </summary>
		protected System.Resources.ResourceManager m_rmStrings = null;

        private System.Windows.Forms.Timer m_SessionTimer = null;
                
        #endregion Private and Protected Variables

        #region Constants

        // Constants
		private const int	SCS_MAX_SECURITY_LENGTH = 8;
		// command + unit type + ID (CRC not included)
		private const int	SCS_IDENTIFY_CMD_LENGTH = 1 + 3 + 8; 
		// ACK + unit type + ID + Start + End (CRC not included)
		private const int	SCS_IDENTIFY_RESP_LENGTH = 1 + 3 + 8 + 3 + 3; 
		// command + security code (CRC not included)
		private const int	SCS_SECURITY_CMD_LENGTH = 1 + 8; 
		private const int	SCS_SECURITY_RESP_LENGTH = 1; // ACK (No CRC)
		// command + start addr + stop addr (CRC not included)
		private const int	SCS_UPLOAD_CMD_LENGTH = 1 + 3 + 3;
		// command + start addr + stop addr (CRC not included)
		private const int	SCS_DOWNLOAD_CMD_LENGTH = 1 + 3 + 3; 
		private const int	SCS_DOWNLOAD_RESP_LENGTH = 1; // ACK (No CRC)
		private const int	SCS_ENQUIRE_CMD_LENGTH = 1; // only command
		private const int	SCS_ENQUIRE_RESP_LENGTH = 1; // ACK (No CRC)
		private const int	SCS_CRC_LENGTH = 2;
		private const uint	SCS_DEFAULT_MAX_RETRIES = 3;
		private const uint	SCS_DEFAULT_MIN_REC_TIMEOUT = 2000; // milliseconds
		private const uint  SCS_MAX_REC_TIMEOUT = 254000; // milliseconds
        private const int SCS_KEEP_ALIVE_FREQUENCY = 5000; // milliseconds
        private const uint SCS_DEFAULT_MAX_PACKET_SIZE = 16;
		private const int   SCS_MIN_PACKET_SIZE = SCS_IDENTIFY_RESP_LENGTH + 
			SCS_CRC_LENGTH; // Can go no lower than largest expected response        
		private const int   SCS_WAKE_UP_MAX_RETRIES = 35;
		private const int   SCS_KEEP_ALIVE_MAX_RETRIES = 3;
        private const int   SCS_DEVICE_TYPE_LENGTH = 3;
		private const int	SCS_DEVICE_ID_LENGTH = 8;
		private const int	SCS_DEVICE_TYPE_INDEX = 1;
		private const int	SCS_DEVICE_ID_INDEX = 4;
		private const int	SCS_UPLOAD_START_ADDR_INDEX = 1;
		private const int   SCS_UPLOAD_END_ADDR_INDEX = 4;
		private const int	SCS_DOWNLOAD_START_ADDR_INDEX = 1;
		private const int	SCS_DOWNLOAD_END_ADDR_INDEX = 4;
		private const int	SCS_COMM_DATA_WAIT_TIME = 50; // milliseconds
        private const int   SCS_COMM_WAKEUP_WAIT_TIME = 100; // milliseconds
		private const int	SCS_MEM_START_ADDR_INDEX = 12;
		private const int	SCS_MEM_END_ADDR_INDEX = 15;


        #endregion Constants

        #region Properties

        /// <summary>This property gets or sets the amount of time to wait 
		/// for a response after sending an SCS command.</summary>
		/// <returns>
		/// An uint representing the timeout in milliseconds.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// scsProtocol.MinReceiveTimeout = 2000;
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A	Modified
		/// 
        public uint MinReceiveTimeout
        {
            get { return m_uiMinReceiveTimeout; }
            set 
			{ 
				if ( value > SCS_MAX_REC_TIMEOUT )
				{
					m_uiMinReceiveTimeout = SCS_MAX_REC_TIMEOUT;
				}
				m_uiMinReceiveTimeout = value; 
			}
        }

		/// <summary>This property gets or sets the maximum retries for sending
		/// any SCS command.</summary>
		/// <returns>
		/// An uint representing the retries.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// scsProtocol.MaxRetries = 3;
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A	Modified
		/// 
        public uint MaxRetries
        {
            get { return m_uiMaxRetries; }
            set { m_uiMaxRetries = value; }
        }

		/// <summary>This property gets or sets the maximum packet size.
		/// </summary>
		/// <returns>
		/// An int representing the maximum packet size.
		/// </returns>
		/// <remarks>This value can not be set lower than the minimum packet 
		/// size.</remarks>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// scsProtocol.MaxPacketSize = 64;
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A	Modified
		/// 06/16/06 jrf 7.30.00  N/A	Removed tertiary operator
		/// 
		public uint MaxUploadSize
		{
			get { return m_uiMaxUploadSize; }
			set 
			{   
				// Do not set lower than the minimum packet size
				if ( value < SCS_MIN_PACKET_SIZE )
				{
					m_uiMaxUploadSize = SCS_MIN_PACKET_SIZE;
				}
				else
				{
					m_uiMaxUploadSize = value;
				}
			}
		}
        /// <summary>This property gets or sets the maximum download packet size.
        /// </summary>
        /// <returns>
        /// An int representing the maximum packet size.
        /// </returns>
        /// <remarks>This value can not be set lower than the minimum packet 
        /// size.</remarks>
        /// <example>
        /// <code>
        /// Communication Comm = new Communication();
        /// Comm.OpenPort("COM4:");
        /// SCSProtocol scsProtocol = new SCSProtocol(Comm);
        /// scsProtocol.MaxPacketSize = 64;
        /// </code>
        /// </example>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/12/06 mrj 7.30.00  N/A   Created
        /// 05/10/06 jrf 7.30.00  N/A	Modified
        /// 06/16/06 jrf 7.30.00  N/A	Removed tertiary operator
		/// 02/07/07 mrj 8.00.11		Changed to allow any packet size to be
		///								set.
        /// 
        public uint MaxDownloadSize
        {
            get { return m_uiMaxDownloadSize; }
            set
            {               
				m_uiMaxDownloadSize = value;             
            }
        }

        /// <summary>
        /// Has this device been identified?  If so, you can use the protocol's
        /// saved device identity values to avoid reissuing Wakeup and Identify
        /// requests. Note that if you switch devices (daisy chaining, etc.), 
        /// you still need to re-identify the new device.
        /// </summary>
        /// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/08/06 mcm 7.35.00  N/A   Created
        /// 
        public bool Identified
        {
            get
            {
                return m_bIdentified;
            }
            set
            {
                m_bIdentified = value;
            }
        }

        /// <summary>
        /// Returns the device ID retreived during the last Identify request.
        /// If Identify has not been called, you shouldn't be asking, but I'll
        /// return an empty string.
        /// </summary>
        /// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/08/06 mcm 7.35.00  N/A   Created
        /// 
        public string DeviceID
        {
            get
            {
                if (Identified)
                {
                    return m_strDeviceID;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Returns the device Type retreived during the last Identify request.
        /// If Identify has not been called, you shouldn't be asking, but I'll
        /// return an empty string.
        /// </summary>
        /// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/08/06 mcm 7.35.00  N/A   Created
        /// 
        public string DeviceType
        {
            get
            {
                if (Identified)
                {
                    return m_strDeviceType;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Returns the Meter Start Address retreived during the last Identify
        /// request. If Identify has not been called, you shouldn't be asking,
        /// but I'll return an empty string.
        /// </summary>
        /// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/08/06 mcm 7.35.00  N/A   Created
        /// 
        public int MeterStartAddress
        {
            get
            {
                if (Identified)
                {
                    return m_iMeterStartAddress;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns the Meter Stop Address retreived during the last Identify
        /// request. If Identify has not been called, you shouldn't be asking,
        /// but I'll return an empty string.
        /// </summary>
        /// 
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 09/08/06 mcm 7.35.00  N/A   Created
        /// 
        public int MeterStopAddress
        {
            get
            {
                if (Identified)
                {
                    return m_iMeterStopAddress;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion Properties

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Comm">The communication object that supports
		/// communication over the physical port.</param>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A	Modified
		/// 
        public SCSProtocol(ICommunications Comm )
        {
            m_CommPort = Comm;
            m_uiMaxRetries = SCS_DEFAULT_MAX_RETRIES;
            m_uiMinReceiveTimeout = SCS_DEFAULT_MIN_REC_TIMEOUT; 
            MaxDownloadSize = SCS_DEFAULT_MAX_PACKET_SIZE;
            MaxUploadSize = SCS_DEFAULT_MAX_PACKET_SIZE;
            m_hLogFile = Logger.TheInstance;
			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, 
				this.GetType().Assembly );
			// For checking when commport has received data
			m_CommPort.FlagCharReceived +=new CommEvent(RcvdData);
			m_bTimedout = false;
            m_bIdentified = false;
			// Create semaphore to handle data received			
            m_ReadEvent = new ManualResetEvent(false);
        }

		/// <summary>
		/// Destructor.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00  N/A   Created
		/// 
		~SCSProtocol()
		{
			m_rmStrings.ReleaseAllResources();
			m_rmStrings = null;

            DisableSessionMaintenance();
        }
	        
        /// <summary>
        /// This method performs actions to get the attention of a
        /// SCS device. In the SCS protocol this is performed
        /// by sending multiple ENQs to the device until the device 
        /// acknowledges it with an ACK message.
		///<remarks>When an ACK message is received we must pause for 1 second
		/// before returning processing to the calling routine.</remarks>
        /// </summary>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// scsProtocol.WakeUpDevice(25);		
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A    Created
		/// 05/04/06 jrf 7.30.00  N/A	 Rewrote
		/// 06/16/06 jrf 7.30.00  N/A	 Changed to Send Enquires one after
		///								 another with no pause
        /// 09/28/06 mrj 7.35.00         Adjusted for better performance on 
        ///                              handheld.
		///
        public SCSProtocolResponse WakeUpDevice()
        {                       
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            byte[] byCmdBuffer = new byte[SCS_ENQUIRE_CMD_LENGTH];
            byte[] byRxByte = null;
            int iIndex = 0;

            m_hLogFile.WriteLine(Logger.LoggingLevel.Protocol,
                m_rmStrings.GetString("SCS_WAKEUP"));

            byCmdBuffer[iIndex] = (byte)SCSCommands.SCS_ENQ;

            
            while (SCSProtocolResponse.SCS_ACK != ProtocolResponse && 
                   iIndex < SCS_WAKE_UP_MAX_RETRIES)
            {
                m_ReadEvent.Reset();

                //Send the ENQ
                m_CommPort.Send(byCmdBuffer);
                
                //Wait on bytes received for up to 100ms
                if (m_ReadEvent.WaitOne(SCS_COMM_WAKEUP_WAIT_TIME, false))
                {
                    if (0 < m_CommPort.Read(SCS_ENQUIRE_RESP_LENGTH, 0))
                    {
                        byRxByte = new byte[m_CommPort.InputLen];
                        Array.Copy(m_CommPort.Input,
                            0,
                            byRxByte,
                            0,
                            SCS_ENQUIRE_RESP_LENGTH);
                    }
                    if ((byRxByte != null) &&
                        (System.Enum.IsDefined(ProtocolResponse.GetType(), byRxByte[0])))
                    {
                        ProtocolResponse = (SCSProtocolResponse)byRxByte[0];
                    }
                }                
                
                iIndex++;
            }

            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
            {
                // Pause for 1 second
                Thread.Sleep(1000);
            }

            return ProtocolResponse;
        } // End WakeUpDevice()		
		
        /// <summary>
        /// Format the SCS I Command and send it to the device.  This method
        /// returns the device type, device ID as well as the memory start and
        /// stop addresses.  It assumes that the device has been woken up prior
        /// to being called.  
        /// </summary>
        /// <param name="strDeviceID">The device ID of the connected meter
        /// </param>
        /// <param name="strDeviceType">The device type of the connected meter
        /// </param>
        /// <param name="iMeterStartAddress">The start address for the base page
        /// of the connected meter.</param>
        /// <param name="iMeterStopAddress">The stop address for the base page of
        /// the connected meter.</param>
        /// <devdoc>Saves the device identity info and marks the device this 
        /// protocol is connected to as identified. SCSDevice.Logon uses this 
        /// info to avoid reissuing unnecessary WakeUp and Indentify requests.
        /// </devdoc>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// 
		/// string strID;
		/// string strType;
		/// int iMemStart;
		/// int iMemEnd;
		/// 
		/// scsProtocol.WakeUpDevice(25);
		/// scsProtocol.Identify(out strID, out strType, out iMemStart, out iMemEnd);		
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A	Revised
        /// 09/09/06 mcm 7.35.00  N/A   Save device identity info for future use
        ///
		public SCSProtocolResponse Identify(out string strDeviceID, 
                             out string strDeviceType, 
                             out int iMeterStartAddress, 
                             out int iMeterStopAddress )
        {
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
            byte[] byResponseBuffer;
			byte[] byCRC;
            byte[] byCmdBuffer = new byte[SCS_IDENTIFY_CMD_LENGTH + SCS_CRC_LENGTH];
			byte[] byDeviceID;
			byte[] byDeviceType;
			ASCIIEncoding Encoder;  // For converting byte array to string
			 
			strDeviceID = "";
			strDeviceType = "";
			iMeterStartAddress = 0;
			iMeterStopAddress = 0;

			m_hLogFile.WriteLine(Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("SCS_IDENTIFY"));

			// Set up I command 
			// 'I' + Dev. Type or 0s + Dev. Ids or 0s + CRC
			byCmdBuffer.Initialize();
			byCmdBuffer[0] = (byte)SCSCommands.SCS_I;
  
			SCSCalculateCRC(ref byCmdBuffer, SCS_IDENTIFY_CMD_LENGTH, out byCRC);
			
			byCmdBuffer[12] = byCRC[0];
			byCmdBuffer[13] = byCRC[1];
	
            ProtocolResponse = 
				SendAndReceive(ref byCmdBuffer, 
							   SCS_IDENTIFY_CMD_LENGTH + SCS_CRC_LENGTH,
							   out byResponseBuffer, 
				               SCS_IDENTIFY_RESP_LENGTH + SCS_CRC_LENGTH);

            if ( SCSProtocolResponse.SCS_ACK == ProtocolResponse )
            {	
				Encoder = new ASCIIEncoding();
				
				// Read the Device Type
				byDeviceType = new byte[SCS_DEVICE_TYPE_LENGTH];
				Array.Copy(
					byResponseBuffer, 
					SCS_DEVICE_TYPE_INDEX, 
					byDeviceType, 
					0, 
					SCS_DEVICE_TYPE_LENGTH);				
				m_strDeviceType = 
					Encoder.GetString(byDeviceType, 0 ,SCS_DEVICE_TYPE_LENGTH);
				strDeviceType = m_strDeviceType;

                // Read the device ID
				byDeviceID = new byte[SCS_DEVICE_ID_LENGTH];
				Array.Copy(
					byResponseBuffer, 
					SCS_DEVICE_ID_INDEX, 
					byDeviceID, 
					0, 
					SCS_DEVICE_ID_LENGTH);
				m_strDeviceID = 
					Encoder.GetString(byDeviceID, 0, SCS_DEVICE_ID_LENGTH);
				strDeviceID = m_strDeviceID;

                // Read the memory start address
                SCSAddress StartAddress = new SCSAddress();
                StartAddress.Extract(ref byResponseBuffer, SCS_MEM_START_ADDR_INDEX);
                m_iMeterStartAddress = StartAddress.Address;
                iMeterStartAddress = m_iMeterStartAddress;

                // Read the memory stop address
                SCSAddress StopAddress = new SCSAddress();
                StopAddress.Extract(ref byResponseBuffer, SCS_MEM_END_ADDR_INDEX);
                m_iMeterStopAddress = StopAddress.Address;
                iMeterStopAddress = m_iMeterStopAddress;

                m_bIdentified = true;
            }

            return ProtocolResponse;
        } // End Identify()

        /// <summary>
        /// This method sends the SCS security command to the device.  Note that 
        /// no information is returned from the device other than an 
        /// acknowledgement or rejection of the security code
        /// </summary>
        /// <param name="strSecurityCode">The security code for the device.
        /// </param>
        /// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// 
		/// string strID;
		/// string strType;
		/// int iMemStart;
		/// int iMemEnd;
		/// 
		/// scsProtocol.WakeUpDevice(25);
		/// scsProtocol.Identify(out strID, out strType, out iMemStart, out iMemEnd);		
		/// scsProtocol.Security("");
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A   Revised
		///
        public SCSProtocolResponse Security(string strSecurityCode)
        {
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
			byte[] byCRC;
            byte[] byResponseBuffer;
            byte[] byCmdBuffer = new byte[SCS_SECURITY_CMD_LENGTH + SCS_CRC_LENGTH];
 		    string strSCSSecurity = "";
			char[] acSecurityCode;
			int intLenSecurityCode = 0;

			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("SCS_SECURITY") );
			
			// Precondition the security code string given to us.  First remove 
			// all spaces from the beginning and the end of the string and then 
			// make sure that it is no longer than the maximum length
			strSCSSecurity = strSecurityCode.Trim();
			acSecurityCode = strSCSSecurity.ToCharArray();
			intLenSecurityCode = acSecurityCode.Length;
			
			if (intLenSecurityCode > SCS_MAX_SECURITY_LENGTH)
			{
				intLenSecurityCode = SCS_MAX_SECURITY_LENGTH;
			}

            // Set up S command 
			// 'S' + security code + CRC
            byCmdBuffer[0] = (byte)SCSCommands.SCS_S;          
			
			for (int iIndex = 0; iIndex < intLenSecurityCode; iIndex++)
            {
                byCmdBuffer[iIndex + 1] = (byte)acSecurityCode[iIndex]; 
            }

            // Complete the command by padding the security code with nulls 
            for (int iIndex = intLenSecurityCode; iIndex < SCS_MAX_SECURITY_LENGTH; iIndex++)
            {
                byCmdBuffer[iIndex + 1] = 0; 
            }

			SCSCalculateCRC(ref byCmdBuffer, SCS_SECURITY_CMD_LENGTH, out byCRC);
			
			byCmdBuffer[9] = byCRC[0];
			byCmdBuffer[10] = byCRC[1];

            // Next send the command to the meter
            ProtocolResponse = SendAndReceive(ref byCmdBuffer, 
				                                 SCS_SECURITY_CMD_LENGTH,
                                                 out byResponseBuffer, 
				                                 SCS_SECURITY_RESP_LENGTH);

            
			if (ProtocolResponse == SCSProtocolResponse.SCS_ACK)
			{
				MaintainCommunicationsSession();
			}
            
            return ProtocolResponse;
        } // End Security()

        /// <summary>
        ///	Oversees the uploading process, breaking request up into smaller
        ///	chunks if neccessary and calling UploadData on each.
        /// </summary>
        /// <param name="iUploadAddress">The base page address to start the
        /// upload</param>
        /// <param name="iDataLength">The data length of the upload</param>
        /// <param name="byData">The data buffer to read the bytes into</param>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// 
		/// string strID;
		/// string strType;
		/// int iMemStart;
		/// int iMemEnd;
		/// 
		/// scsProtocol.WakeUpDevice(25);
		/// scsProtocol.Identify(out strID, out strType, out iMemStart, out iMemEnd);		
		/// scsProtocol.Security("");
		/// 
		/// byte[] abytOperatingSetup;
		/// scsProtocol.Upload(0x2196, 1, out abytOperatingSetup);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A   Rewrote
		/// 06/19/06 jrf 7.30.00  N/A	Broke into two methods to avoid recursion
		///        
        public SCSProtocolResponse Upload(int iUploadAddress,
										  int iDataLength,
                                          out byte[] byData )
        {
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
            int iActualResponseLength = 0;   			
            // Ack + Data + CRC
			int iExpectedResponseLength = 1 + iDataLength + SCS_CRC_LENGTH; 
            
			byData = new byte[iDataLength];

			// Check to see if data to be retrieved exceeds our maximum packet size
			if (iExpectedResponseLength > MaxUploadSize )
            {
                // Break this request into multiple upload requests
                byte [] byPartialDataBlock;
                int iPartialUploadAddress = iUploadAddress;

                do
                {
                    int iPartialRequestLength = iDataLength - iActualResponseLength;

					// check if the length of Data + ACK + CRC is greater than 
					// maximum packet size
					if ((iPartialRequestLength + (1 + SCS_CRC_LENGTH)) >
                        MaxUploadSize)
					{
                        iPartialRequestLength = (int)MaxUploadSize - 
							(1 + SCS_CRC_LENGTH);
					}

                    ProtocolResponse = UploadData(iPartialUploadAddress,
                                                 iPartialRequestLength,
                                                 out byPartialDataBlock);

                    if (ProtocolResponse == SCSProtocolResponse.SCS_ACK)
                    {
                        // copy the data into the complete response buffer
						for (int iIndex = 0; iIndex < iPartialRequestLength; iIndex++)
						{
							byData[iActualResponseLength + iIndex] = 
								byPartialDataBlock[iIndex];
						}

                        // Account for the data we just received
                        iPartialUploadAddress += iPartialRequestLength;
                        iActualResponseLength += iPartialRequestLength;
                    }
                }
                while ((iActualResponseLength < iDataLength) &&
                    (ProtocolResponse == SCSProtocolResponse.SCS_ACK));


            }
            else
            {
				// Send whole upload request
				ProtocolResponse = UploadData(iUploadAddress,
					iDataLength,
					out byData);
            }
           
            return ProtocolResponse;
        }// End Upload()
		

		/// <summary>
		///	Uploads data from the device starting at a given address.
		/// </summary>
		/// <param name="iUploadAddress">The base page address to start the
		/// upload</param>
		/// <param name="iDataLength">The data length of the upload</param>
		/// <param name="byData">The data buffer to read the bytes into</param>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00  N/A   Created
		///        
		public SCSProtocolResponse UploadData(int iUploadAddress,
			int iDataLength,
			out byte[] byData )
		{			
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
			// Ack + Data + CRC
			int iExpectedResponseLength = 1 + iDataLength + SCS_CRC_LENGTH; 
			byte[] byResponseBuffer;

			byData = new byte[iDataLength];

			byte[] byCmdBuffer = new byte[SCS_UPLOAD_CMD_LENGTH + SCS_CRC_LENGTH];
			byte[] byCRC;
			byResponseBuffer = new byte[iExpectedResponseLength];

			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("SCS_UPLOAD") );
			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("ADDRESS") + iUploadAddress.ToString("X4", CultureInfo.CurrentCulture));
			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("LENGTH") + iDataLength.ToString(CultureInfo.CurrentCulture) );
			
			// Start building the command 
			// 'U' + start address + stop address + CRC
            byCmdBuffer[0] = (byte)SCSCommands.SCS_U;

            SCSAddress StartSCSAddress = new SCSAddress(iUploadAddress);
            SCSAddress StopSCSAddress = new SCSAddress(iUploadAddress + 
				(iDataLength - 1));

            StartSCSAddress.Insert(ref byCmdBuffer, SCS_UPLOAD_START_ADDR_INDEX);
            StopSCSAddress.Insert(ref byCmdBuffer, SCS_UPLOAD_END_ADDR_INDEX);

			SCSCalculateCRC(ref byCmdBuffer, SCS_UPLOAD_CMD_LENGTH, out byCRC);
		
			byCmdBuffer[7] = byCRC[0];
			byCmdBuffer[8] = byCRC[1];

            // OK, the outgoing command is set up - send it and see what we get 
			// in response
            ProtocolResponse = 
				SendAndReceive(ref byCmdBuffer, 
					           SCS_UPLOAD_CMD_LENGTH + SCS_CRC_LENGTH,
                               out byResponseBuffer, 
					           iExpectedResponseLength);

			if (ProtocolResponse == SCSProtocolResponse.SCS_ACK)
			{
				// Copy the data from the input buffer to the data buffer to be 
				// returned to the caller.  But be careful not to copy the ACK 
				// into the data buffer
				Array.Copy(byResponseBuffer, 1, byData, 0, iDataLength);
			}
         
            return ProtocolResponse;
		}
		
		/// <summary>
		///	Oversees the downloading process, breaking download up into smaller
		///	chunks if neccessary and calling DownloadData on each.
		/// </summary>
		/// <param name="iDownloadAddress">The base page address to start the 
		/// download</param>
		/// <param name="iDownloadLength">The length of bytes to download</param>
		/// <param name="byData">The buffer of data to download</param>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// <example>
		/// <code>
		/// Communication Comm = new Communication();
		/// Comm.OpenPort("COM4:");
		/// SCSProtocol scsProtocol = new SCSProtocol(Comm);
		/// 
		/// string strID;
		/// string strType;
		/// int iMemStart;
		/// int iMemEnd;
		/// 
		/// scsProtocol.WakeUpDevice(25);
		/// scsProtocol.Identify(out strID, out strType, out iMemStart, out iMemEnd);		
		/// scsProtocol.Security("");
		/// 
		/// byte[] byHangUpFlag = new byte[1];
		/// byHangUpFlag[0] = 0xFF;
		/// SCSProtocolResponse ProtoResponse = 
		///		scsProtocol.Download(0x1B03, 1, ref byHangUpFlag);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 05/10/06 jrf 7.30.00  N/A   Rewrote
		/// 06/19/06 jrf 7.30.00  N/A	Broke into two methods to avoid recursion
		/// 
		public SCSProtocolResponse Download(int iDownloadAddress,
											int iDownloadLength,
											ref byte[] byData )
		{
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
			if (iDownloadLength + SCS_CRC_LENGTH > MaxDownloadSize)
			{
				// Break this request into multiple download requests
				byte[] byPartialDownloadDataBlock;
				int iPartialDownloadAddress = iDownloadAddress;
				int iDataSentLength = 0;
				int iPartialDownloadDataLength = iDownloadLength;

				do
				{
					iPartialDownloadDataLength = iDownloadLength - iDataSentLength;

                    if ((iPartialDownloadDataLength + SCS_CRC_LENGTH) > MaxDownloadSize)
					{
                        iPartialDownloadDataLength = (int)MaxDownloadSize - 
							SCS_CRC_LENGTH;
					}

					byPartialDownloadDataBlock = new byte[iPartialDownloadDataLength];
					Array.Copy(byData,
						iDataSentLength, 
						byPartialDownloadDataBlock, 
						0, 
						byPartialDownloadDataBlock.Length);

					// Send the Download request sized to fit within packet size
					ProtocolResponse = DownloadData(iPartialDownloadAddress,
						iPartialDownloadDataLength,
						ref byPartialDownloadDataBlock);

					if (ProtocolResponse == SCSProtocolResponse.SCS_ACK)
					{
						// Account for the data we just received
						iPartialDownloadAddress += iPartialDownloadDataLength;
						iDataSentLength += iPartialDownloadDataLength;
					}
				}
				while ((iDataSentLength < iDownloadLength) &&
					(ProtocolResponse == SCSProtocolResponse.SCS_ACK));
			}
			else
			{
				// Send the whole request
				ProtocolResponse = DownloadData(iDownloadAddress,
					iDownloadLength,
					ref byData);			
			}
            
			return ProtocolResponse;
		} // End Download()

		/// <summary>
		///	Downloads data to the device starting at a given address.
		/// </summary>
		/// <param name="iDownloadAddress">The base page address to start the 
		/// download</param>
		/// <param name="iDownloadLength">The length of bytes to download</param>
		/// <param name="byData">The buffer of data to download</param>
		/// <returns>A SCSProtocolResponse representing the protocol response.
		/// </returns>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/19/06 jrf 7.30.00  N/A   Created
		/// 
		public SCSProtocolResponse DownloadData(int iDownloadAddress,
			int iDownloadLength,
			ref byte[] byData )
		{
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
			byte[] byDownloadRequest = new byte[SCS_DOWNLOAD_CMD_LENGTH + SCS_CRC_LENGTH];
			byte[] byDownloadData = new byte[iDownloadLength + SCS_CRC_LENGTH];
			byte[] byResponse = new byte[SCS_DOWNLOAD_RESP_LENGTH];
			byte[] byCRC;
			uint uiDownloadAttempts = 0;
			uint uiDownloadMaxRetries = m_uiMaxRetries;	
			uint uiMaxRetries;  

			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("SCS_DOWNLOAD") );
			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("ADDRESS") + iDownloadAddress.ToString("X4", CultureInfo.CurrentCulture));
			m_hLogFile.WriteLine(
				Logger.LoggingLevel.Protocol, 
				m_rmStrings.GetString("LENGTH") + iDownloadLength.ToString(CultureInfo.CurrentCulture) );

			// Set up the Download Request
			// 'D' + start address + end address + CRC
			byDownloadRequest[0] = (byte)SCSCommands.SCS_D;

			SCSAddress StartAddress = new SCSAddress(iDownloadAddress);
			SCSAddress StopAddress = new SCSAddress(iDownloadAddress + (iDownloadLength-1));

			StartAddress.Insert(ref byDownloadRequest, SCS_DOWNLOAD_START_ADDR_INDEX);
			StopAddress.Insert(ref byDownloadRequest, SCS_DOWNLOAD_END_ADDR_INDEX);

			SCSCalculateCRC(ref byDownloadRequest, SCS_DOWNLOAD_CMD_LENGTH, out byCRC);
			
			byDownloadRequest[7] = byCRC[0];
			byDownloadRequest[8] = byCRC[1];

			// OK, the outgoing command is set up - send it and see what we get 
			// in response
			while (SCSProtocolResponse.SCS_ACK != ProtocolResponse 
				&& uiDownloadAttempts < uiDownloadMaxRetries)
			{
				// Send the Download Request
				ProtocolResponse = SendAndReceive(ref byDownloadRequest, 
					SCS_DOWNLOAD_CMD_LENGTH + SCS_CRC_LENGTH,
					out byResponse, 
					SCS_DOWNLOAD_RESP_LENGTH);

				if ( SCSProtocolResponse.SCS_ACK == ProtocolResponse )
				{
					// Set up the data to be downloaded
					Array.Copy(byData, 0, byDownloadData, 0, iDownloadLength);

					SCSCalculateCRC(ref byData, iDownloadLength, out byCRC);
			
					byDownloadData[byDownloadData.Length - 2] = byCRC[0];
					byDownloadData[byDownloadData.Length - 1] = byCRC[1];

					// Temporarily change number of retries to 1.  Due to the two 
					// part download command we are moving the retry logic for the 
					// second part out to this method.
					uiMaxRetries = m_uiMaxRetries;
					m_uiMaxRetries = 1;

					// Now send the data
					ProtocolResponse = SendAndReceive(ref byDownloadData, 
						iDownloadLength + SCS_CRC_LENGTH,
						out byResponse, 
						SCS_DOWNLOAD_RESP_LENGTH);

					//Restore the number of retries
					m_uiMaxRetries = uiMaxRetries;
				} 
				else if ( SCSProtocolResponse.SCS_CAN == ProtocolResponse )
				{
					// The download request failed due to security.  
					// No need to continue. 
					break;
				}
				
				uiDownloadAttempts++;
			}
				
			return ProtocolResponse;
			
		} // End DownloadData()


        /// <summary>
        /// Sends a command and gets a response from a connected device
        /// </summary>
        /// <param name="byCmdBuffer">The command to send to the device</param>
        /// <param name="iCmdLength">The length of the command to send</param>
        /// <param name="byResponseBuffer">The buffer to hold the response from 
        /// the device</param>
        /// <param name="iResponseLength">The length of the response</param>
        /// <returns>A SCSProtocolResponse representing the protocol response.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------		
        //  04/12/06 mrj 7.30.00  N/A   Created
        //  05/10/06 jrf 7.30.00  N/A   Reworked
        //  06/21/06 jrf 7.30.00  N/A   Modified to use blocking and asynchronous
        //								events to process comm data
        //	06/29/06 jrf 7.30.00  N/A   Added code to allow a retry on NAK and 
        //								removed unnecessary break statment
        //  03/06/07 mrj 8.00.17 2588	Stopped the session timer (keep alive) if
        //								the meter has timed out.
        //  08/30/16 jrf 4.70.15 No WR  Quieting compiler warning and disposing of tmrTimeout in a finally block.
        protected SCSProtocolResponse SendAndReceive(ref byte[] byCmdBuffer, 
			int iCmdLength, out byte[] byResponseBuffer, int iResponseLength )
		{
			byte[] byExpectedCRC;
			SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse; 
			int iActualResponseLength = 0;
			int iBytesRemaining = iResponseLength;
			int iResponseBufferIndex = 0;
			int iResponseBufferCopyLength = 0;
			int iTries = 0;
			byte[]    byRxByte = null;
			byResponseBuffer = new byte[iResponseLength];
			int iNoChange = 0;
            TimerCallback timerDelegate = null;
            System.Threading.Timer tmrTimeout = null;

            PauseSessionTimer();

            try
            {

                //Create the a response timeout timer
                timerDelegate = new TimerCallback(SetTimeout);
                tmrTimeout = new System.Threading.Timer(timerDelegate,
                    null,
                    Timeout.Infinite,
                    Timeout.Infinite);

                while ((SCSProtocolResponse.NoResponse == ProtocolResponse ||
                    SCSProtocolResponse.SCS_NAK == ProtocolResponse) &&
                    iTries < m_uiMaxRetries)
                {
                    try
                    {
                        if (m_CommPort.IsOpen)
                        {
                            byRxByte = null;
                            iActualResponseLength = 0;
                            ProtocolResponse = SCSProtocolResponse.NoResponse;
                            Array.Clear(byResponseBuffer, 0, byResponseBuffer.Length);

                            // Send the command to the device 
                            m_CommPort.Send(byCmdBuffer);

                            // Reset timeout
                            m_bTimedout = false;
                            // Timer should fire only once upon min. rec. timeout
                            tmrTimeout.Change(m_uiMinReceiveTimeout, Timeout.Infinite);


                            m_ReadEvent.Reset();

                            // Wait for signal from comm event but may need to check
                            // a few times in case signal is not sent
                            while (byRxByte == null && !m_bTimedout)
                            {
                                // Wait until read is signaled
                                m_ReadEvent.WaitOne(SCS_COMM_DATA_WAIT_TIME, false);

                                if (0 < m_CommPort.Read(1, 0) && !m_bTimedout)
                                {
                                    // We got a byte!   
                                    byRxByte = new byte[1];
                                    Array.Copy(m_CommPort.Input,
                                        0,
                                        byRxByte,
                                        0,
                                        1);

                                    byResponseBuffer[0] = byRxByte[0];
                                    iActualResponseLength = 1;
                                }
                            }

                            // Continue only if we got a byte and it is a valid SCS 
                            // response
                            if ((byRxByte != null) &&
                                (System.Enum.IsDefined(
                                   ProtocolResponse.GetType(),
                                   byRxByte[0])))
                            {
                                ProtocolResponse = (SCSProtocolResponse)byRxByte[0];

                                if (SCSProtocolResponse.SCS_ACK == ProtocolResponse)
                                {
                                    // Only if the response was positive do we try to read 
                                    // the data.  Note that if we are not expecting anything 
                                    // more than the command acknowledgement, we're done!

                                    // Read the data until we get all of it or timeout
                                    while (iActualResponseLength < iResponseLength &&
                                            !m_bTimedout)
                                    {
                                        iBytesRemaining = iResponseLength -
                                            iActualResponseLength;
                                        iResponseBufferIndex = iActualResponseLength;

                                        iNoChange = iActualResponseLength;

                                        // Try to read without waiting									
                                        iActualResponseLength += m_CommPort.Read(0, 50);

                                        while (iNoChange == iActualResponseLength &&
                                            !m_bTimedout)
                                        {
                                            // Wait until read is signaled 										
                                            m_ReadEvent.Reset();
                                            m_ReadEvent.WaitOne(SCS_COMM_DATA_WAIT_TIME, false);

                                            iActualResponseLength += m_CommPort.Read(0, 50);
                                        }
                                        // Set up to copy the right amount of data
                                        if (m_CommPort.InputLen < iBytesRemaining)
                                        {
                                            iResponseBufferCopyLength =
                                                (int)m_CommPort.InputLen;
                                        }
                                        else
                                        {
                                            iResponseBufferCopyLength = iBytesRemaining;
                                        }
                                        // Only copy if we get response from the read
                                        if (0 != iResponseBufferCopyLength)
                                        {
                                            Array.Copy(m_CommPort.Input,
                                                0,
                                                byResponseBuffer,
                                                iResponseBufferIndex,
                                                iResponseBufferCopyLength);
                                        }
                                    }
                                }
                            }

                            // If we received a NAK, initiate and Enq until Ack sequence to 
                            // get the meter and the system back in synch
                            if (SCSProtocolResponse.SCS_NAK == ProtocolResponse)
                            {
                                WakeUpDevice();
                            }

                            // If we don't have all the data and we didn't receive a 
                            // NAK or CANCEL then we timed out
                            if (SCSProtocolResponse.SCS_CAN != ProtocolResponse &&
                                SCSProtocolResponse.SCS_NAK != ProtocolResponse &&
                                iActualResponseLength < iResponseLength &&
                                m_bTimedout)

                            {
                                throw (new TimeOutException(
                                    m_rmStrings.GetString("PROTOCOL_TIMEOUT")));
                            }

                            //verify CRC, only for Identify and Upload
                            if (SCSProtocolResponse.SCS_ACK == ProtocolResponse &&
                                (byte)SCSCommands.SCS_I == byCmdBuffer[0] &&
                                (byte)SCSCommands.SCS_U == byCmdBuffer[0])
                            {
                                SCSCalculateCRC(
                                    ref byResponseBuffer,
                                    iResponseLength - SCS_CRC_LENGTH,
                                    out byExpectedCRC);
                                if ((byExpectedCRC[0] !=
                                    byResponseBuffer[iResponseLength - 2]) ||
                                    (byExpectedCRC[1] !=
                                    byResponseBuffer[iResponseLength - 1]))
                                {
                                    // Bad CRC treat as no response
                                    ProtocolResponse = SCSProtocolResponse.NoResponse;
                                }
                            }
                        }
                    }

                    // Only catch timeout exceptions.
                    catch (TimeOutException)
                    {
                        ProtocolResponse = SCSProtocolResponse.NoResponse;
                        if ((iTries + 1) >= m_uiMaxRetries)
                        {
                            //Stop the session timer
                            DisableSessionMaintenance();

                            m_hLogFile.WriteLine(Logger.LoggingLevel.Detailed, "Protocol Timeout");
                            throw;
                        }
                    }

                    iTries++;

                }
            }
            finally
            {
                if (null != tmrTimeout)
                {
                    // Dispose of timer
                    tmrTimeout.Dispose();
                }
            }

            ResetSessionTimer();

			return ProtocolResponse;
		}// End SendAndReceive()

		/// <summary>
		/// Method called when the communications port character received flag
		/// is set.
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/20/06 jrf 7.30.00 N/A	Created
		private void RcvdData()
		{
			// Signal semaphore when data is received on the port			
            m_ReadEvent.Set();
		}

		/// <summary>
		/// Method called when SendAndReceive's timeout timer fires.
		/// </summary>
		/// <param name="stateInfo">Used solely to match TimerCallback's 
		/// signature</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/20/06 jrf 7.30.00 N/A	Created
		/// 
		private void SetTimeout(object stateInfo)
		{
			m_bTimedout = true;
		}

        /// <summary>
        /// This routine calculates the 16 bit CRC used with the SCS protocol
        /// </summary>
        /// <param name="abytBuffer">Buffer containing the data to calculate CRC
        /// </param>
        /// <param name="iDataLength">The length of the data in the buffer that 
        /// needs the CRC</param>
        /// <param name="byCRC">Buffer to return the CRC</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 
        protected void SCSCalculateCRC(ref byte[] abytBuffer, int iDataLength, 
			out byte[] byCRC)
        {
            byCRC = new byte[SCS_CRC_LENGTH];

            byte byCRCmsb = 0xFF;
            byte byCRClsb = 0xFF; 
            byte byScratch;

            byte byAccumulator;

            for (int iIndex = 0; iIndex < iDataLength; iIndex++)
            {
                byAccumulator = abytBuffer[iIndex];
                byAccumulator ^= byCRCmsb;
                byScratch = byAccumulator;
                byAccumulator = (byte)(byAccumulator >> 4);
                byAccumulator = (byte)(byAccumulator ^ byScratch);
                byScratch = byAccumulator;
                byAccumulator = (byte)(byAccumulator << 4);
                byAccumulator ^= byCRClsb;
                byCRCmsb = byAccumulator;
                byAccumulator = byScratch;
                byAccumulator = (byte)(byAccumulator >> 3);
                byAccumulator ^= byCRCmsb;
                byCRCmsb = byAccumulator;
                byAccumulator = byScratch;
                byAccumulator = (byte)(byAccumulator << 5);
                byAccumulator ^= byScratch;
                byCRClsb = byAccumulator;
            }

            byCRC[0] = byCRCmsb;
            byCRC[1] = byCRClsb;
        } // End SCSCalculateCRC()

        /// <summary>
        ///  This method is used to maintain continuous communications with a metering device by
        ///  periodically issuing wake up commands.  The time between wait commands is determined
        ///  by the protocol's default timeout values and will be reset whenever commands are issued
        ///  to the meter.
        /// 
        ///  Note that this method is a private method and all exception handling is the responsibility
        ///  of the calling routine
        /// </summary>
        /// <remarks>
        ///  Note that Windows.Forms Timers are used instead of system timers.  Windows.Form timers
        ///  are intended for single threaded, UI oriented applications while system timers are intended for 
        ///  thread safe applications.  Since serial communications is inherently single threaded and this protocol
        ///  class is not intended for concurrent use, Windows.Forms.Timers are the better choice
		/// </remarks>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/20/06 mah 8.00.00 N/A	Created
		//  03/23/07 mrj 8.00.21 2723	Do not allow multiple session timers
		// 								to get created.
		//        
        private void MaintainCommunicationsSession()
        {
			if (null == m_SessionTimer)
			{
				m_SessionTimer = new System.Windows.Forms.Timer();

				m_SessionTimer.Interval = SCS_KEEP_ALIVE_FREQUENCY;
				m_SessionTimer.Tick += new EventHandler(KeepSessionAliveHandler);

				m_SessionTimer.Enabled = true;
			}
        }

        /// <summary>
        /// This method is to be called when logging off a meter.  It disables
        /// periodic calls to the meter and releases the timer coordinating the calls
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void DisableSessionMaintenance()
        {
            if (m_SessionTimer != null)
            {                
				m_SessionTimer.Enabled = false;
                m_SessionTimer = null;
            }
        }

        /// <summary>
        /// This method is called when the maintenance timer is fired.  It is responsible
        /// for issueing a command to the meter to keep it alive and/or cleaning up if 
        /// we are no longer on line with the meter
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  11/20/06 mah 8.00.00 N/A	Created
        //  03/12/07 mrj 8.00.18 2629	Stop the timer if the meter has timed out.
		//  
        private void KeepSessionAliveHandler(Object myObject, EventArgs myEventArgs)		
        {
			try
			{
				if (m_CommPort.IsOpen)
				{
					KeepAlive();

					ResetSessionTimer();
				}
				else if (m_SessionTimer != null)
				{
					m_SessionTimer.Enabled = false;
				}
			}
			catch
			{
				//If we had a timeout exception or any other exception we must catch
				//it here.  Disable the keep alive since we timed out.
				DisableSessionMaintenance();				
			}				
        }

        /// <summary>
        /// This method should be called after each successful communication with the meter.
        /// It resets the maintenance timer for the communication session and prevents 
        /// unnecessary wakeup commands from being issued and slowing the 
        /// application down unnecessarily
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void ResetSessionTimer()
        {
            if (m_SessionTimer != null)
            {
                m_SessionTimer.Interval = SCS_KEEP_ALIVE_FREQUENCY;                
				m_SessionTimer.Enabled = true;
            }
        }

        /// <summary>
        /// This method should be called immediately before communicating with the meter.
        /// It stops the maintenance timer from firing and prevents the application from
        /// queuing up a wakeup command during the execution of another meter 
        /// operation
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/06 mah 8.00.00 N/A	Created
        /// </remarks>
        private void PauseSessionTimer()
        {
            if (m_SessionTimer != null)
            {                
				m_SessionTimer.Enabled = false;
            }
        }

		/// <summary>
		/// Sends ENQ's to keep the meter's session alive (up to 3 ENQ's looking for
		/// and ACK).
		/// </summary>		
		/// <exception cref="TimeOutException">
		/// Timeout thrown if ACK is not received from ENQ.
		/// </exception>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  03/12/07 mrj 8.00.18 2629	Created
		//  
		private void KeepAlive()        
        {                       
            SCSProtocolResponse ProtocolResponse = SCSProtocolResponse.NoResponse;
            byte[] byCmdBuffer = new byte[SCS_ENQUIRE_CMD_LENGTH];
            byte[] byRxByte = null;
            int iIndex = 0;

            m_hLogFile.WriteLine(Logger.LoggingLevel.Protocol, "Keep Alive");

            byCmdBuffer[iIndex] = (byte)SCSCommands.SCS_ENQ;

            
            while (SCSProtocolResponse.SCS_ACK != ProtocolResponse &&
				   iIndex < SCS_KEEP_ALIVE_MAX_RETRIES)
            {
                m_ReadEvent.Reset();

                //Send the ENQ
                m_CommPort.Send(byCmdBuffer);
                
                //Wait on bytes received for up to 100ms
                if (m_ReadEvent.WaitOne(SCS_COMM_WAKEUP_WAIT_TIME, false))
                {
                    if (0 < m_CommPort.Read(SCS_ENQUIRE_RESP_LENGTH, 0))
                    {
                        byRxByte = new byte[m_CommPort.InputLen];
                        Array.Copy(m_CommPort.Input,
                            0,
                            byRxByte,
                            0,
                            SCS_ENQUIRE_RESP_LENGTH);
                    }
                    if ((byRxByte != null) &&
                        (System.Enum.IsDefined(ProtocolResponse.GetType(), byRxByte[0])))
                    {
                        ProtocolResponse = (SCSProtocolResponse)byRxByte[0];
                    }
                }                
                
                iIndex++;
            }

            if (SCSProtocolResponse.SCS_ACK != ProtocolResponse)
            {
				//The meter must have timed out
				throw (new TimeOutException(m_rmStrings.GetString("PROTOCOL_TIMEOUT")));
            }            
        }
    }

	/// <summary>
	/// This internal class represents an SCS address
	/// </summary>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ -------------------------------------------
	/// 04/13/06 mrj 7.30.00  N/A   Created
	///
	internal class SCSAddress
    {

		/// <summary>
		/// Constructor
		/// </summary>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 
		public SCSAddress()
		{
			m_iAddress = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="iAddress">The SCS address</param>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 
		public SCSAddress(int iAddress)
        {
            m_iAddress = iAddress;
        }

		/// <summary>This property gets or sets the SCS address.</summary>
		/// <returns>
		/// An int representing the SCS address
		/// </returns>
		/// <example>
		/// <code>
		/// SCSAddress Address = new SCSAddress();
		/// Address.Address = 0x1234;
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		/// 
        public int Address
        {
            get { return m_iAddress; }
            set { m_iAddress = value; }
        }
        
		/// <summary>
		///	Inserts the address into an array at a specified offset.
		/// </summary>
		/// <param name="byArray">The buffer to insert the address</param>
		/// <param name="iOffset">The location in the buffer to start the 
		/// insertion</param>
		/// <example>
		/// <code>
		/// SCSAddress Address = new SCSAddress(1234);
		/// byte[] byAddress = new byte[3];
		/// 
		/// Address.Insert(byAddress, 0);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		///
        public void Insert( ref byte [] byArray, int iOffset)
        {
            byArray[iOffset] = (byte)(m_iAddress >> 16);
            byArray[iOffset + 1] = (byte)(( m_iAddress & 0x0000FFFF )>> 8);
            byArray[iOffset + 2] = (byte)(m_iAddress & 0x000000FF);
        }

		/// <summary>
		///	Extracts the address from an array at a specified offset.
		/// </summary>
		/// <param name="byArray">The buffer to extract the address from</param>
		/// <param name="iOffset">The location in the buffer to start the 
		/// extraction</param>
		/// <example>
		/// <code>
		/// SCSAddress Address = new SCSAddress(1234);
		/// byte[] byAddress = new byte[3];
		/// 
		/// byAddress[0] = 3;
		/// byAddress[1] = 4;
		/// byAddress[2] = 5;
		/// 
		/// Address.Extract(byAddress, 0);
		/// </code>
		/// </example>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00  N/A   Created
		///
        public void Extract(ref byte[] byArray, int iOffset)
        {
            m_iAddress = 0;

            m_iAddress = byArray[iOffset] << 16;
            m_iAddress += byArray[iOffset + 1] << 8;
            m_iAddress += byArray[iOffset + 2];
        }

        private int m_iAddress;
    }
}
