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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Win32;
using Itron.Metering.Communications;
using Itron.Metering.Utilities;


namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// Itron defined Zigbee node types.  This identifier is the 6th byte
    /// in the IEEE address for the device.
    /// </summary>
    public enum ZigbeeDeviceType
    {
        /// <summary>ELECTRIC_METER</summary>
        ELECTRIC_METER = 0,
        /// <summary>CELL_RELEAY</summary>
        CELL_RELAY = 1,
        /// <summary>GAS_METER</summary>
        GAS_METER = 2,
        /// <summary>WATER_METER</summary>
        WATER_METER = 3,
        /// <summary>HHC</summary>
        HHC = 4,
        /// <summary>COMVERGE_GATEWAY</summary>
        COMVERGE_GATEWAY = 6,
    
    } // ZigbeeDeviceType

    /// <summary>The Zigbee node type</summary>
    public enum ZigbeeLogicalType
    {
        /// <summary></summary>
        COORDINATOR = 0,
        /// <summary></summary>
        ROUTER = 1,
        /// <summary></summary>
        ENDDEVICE = 2
    }

    /// <summary>C177 profile Command Clusters</summary>
    public enum ItronClusters
    {
        /// <summary></summary>
        UNENCRYPTED_OTA = 0x0001,
        /// <summary></summary>
        DECOMMISSION = 0x0008,
        /// <summary></summary>
        DATA_REQUEST = 0x0009,
        /// <summary></summary>
        DATA_RESPONSE = 0x000A,
        /// <summary></summary>
        SERIAL_SERVER_REQUEST = 0x000C,
        /// <summary></summary>
        SERIAL_SERVER_RESPONSE = 0x000D,
        /// <summary></summary>
        LINK_STATUS_REQUEST = 0x0014,
        /// <summary></summary>
        LINK_STATUS_RESPONSE = 0x0015,
        /// <summary></summary>
        HEARTBEAT_REQUEST = 0x001A,
        /// <summary></summary>
        HEARTBEAT_RESPONSE = 0x001B,
        /// <summary></summary>
        ENCRYPTED_OTA = 0x001C,
        /// <summary></summary>
        NETWORK_SET_CONFIG_REQUEST = 0x0024,
        /// <summary></summary>
        NETWORK_SET_CONFIG_RESPONSE = 0x0025,
        /// <summary></summary>
        NETWORK_GET_CONFIG_REQUEST = 0x0026,
        /// <summary></summary>
        NETWORK_GET_CONFIG_RESPONSE = 0x0027,
    }

    /// <summary>
    /// Represents an abstract Zigbee Radio.  At least 2 radios will eventually
    /// inherit from this class, the Integration Associates Zigbee Dongle, and
    /// the FC200 Zibee Radio.
    /// </summary>
    public abstract partial class Radio
    {
        /// <summary>Notification event for an end device joining the radio's 
        /// network when the radio is configured as a Trust Center. Only the
        /// GasModule class should be handling this event. All application 
        /// events will come from the GasModule class via GasModuleEvents.
        /// </summary>
        public abstract event EndDeviceJoinedEventHandler EndDeviceJoinedEvent;

        #region Private Constants

        private const string ENCRYPTED_KEY_NAME = "Key";
        private const string ENCRYPTED_LINK_KEY_NAME = "GlobalLinkKey";

        #endregion Private Constants

        #region Protected Constants

        /// <summary>
        /// The number of beacons to send at one time during beacon burst.
        /// </summary>
        protected const int NUMBER_OF_BEACONS = 30;

        #endregion

        #region Public Constants

        /// <summary>
        /// Max frame size supported by Itron Zigbee radio stacks
        /// </summary>
        public const ushort MAX_DATA_SIZE = 50;
               
        /// <summary>
        /// Coordinators are node 0 within their networks
        /// </summary>
        public const ushort COORDINATOR_SHORT_ADDRESS = 0;
       
        /// <summary>
        /// The only invalid network address. A network address of 0xFFFF
        /// indicates that we are not in a network.
        /// </summary>
        public const ushort OUT_OF_NETWORK_PAN_ID = 0xFFFF;

        /// <summary>IEEE address of the MAC layer.  This address includes the 
        /// ITRON identifier and designates the radio's device type as HHC.  
        /// The lower four digits are bogus and need to be revisited.</summary>
        public const ulong C177_HANDHELD_PROGRAMMER_MAC = 0x0007810400000001;

        /// <summary>
        /// Each bit represents a channel. This set represents the channels the
        /// OpenWay will be using (11, 15, 20, 25)
        /// </summary>
        public const uint OPENWAY_CHANNELS = 0x002108800;   

        /// <summary>
        /// Each bit represents a channel. This constant represents all of the 
        /// ZigBee channels (11-26)
        /// </summary>
        public const uint ALL_CHANNELS = 0x07FFF800;
        
        /// <summary>
        /// Each bit represents a channel. This constant represents channel 11. 
        /// </summary>
        public const uint CHANNEL_11 = 0x00000800;
        
        /// <summary>
        /// Each bit represents a channel. This constant represents channel 15. 
        /// </summary>
        public const uint CHANNEL_15 = 0x000008000;
        
        /// <summary>
        /// Each bit represents a channel. This constant represents channel 20. 
        /// </summary>
        public const uint CHANNEL_20 = 0x000100000;
        
        /// <summary>
        /// Each bit represents a channel. This constant represents channel 25. 
        /// </summary>
        public const uint CHANNEL_25 = 0x002000000;

        /// <summary>
        /// Each bit represents a channel. This set represents the channel 
        /// reserved for programming gas modules (17)
        /// </summary>
        public const uint GAS_MODULE_PROGRAMMING_CHANNEL = 0x20000;

        /// <summary>
        /// The base MAC address for Itron devices.
        /// </summary>
        public const ulong ITRON_DEVICE_MAC_BASE =  0x0007810000000000;

        /// <summary>
        /// The mask for the base section of the MAC address
        /// </summary>
        public const ulong ITRON_DEVICE_MAC_MASK =  0xFFFFFF0000000000;

        /// <summary>
        /// The mask for the device type section of the MAC address
        /// </summary>
        public const ulong ITRON_DEVICE_TYPE_MASK = 0x000000FF00000000;

        /// <summary>
        /// The number of places to shift in order to get the device type
        /// </summary>
        public const int ITRON_DEVICE_TYPE_SHIFT = 32;

        #endregion Public Constants

        #region Public ZigBee-Specific Methods

        /// <summary>
        /// Connects to radio hardware. This method does not start the radio.
        /// This method should be used to verify the hardware exists and is
        /// available for use.
        /// </summary>
        /// <returns>true if the hardware is available</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/07/07 mcm 8.10.26 
        ///</remarks>
        public abstract bool Connect(IntPtr hWnd);

        /// <summary>
        /// Connects to radio hardware with the specified device address.
        /// </summary>
        /// <param name="hWnd">The handle to use for the connection.</param>
        /// <param name="DevAddr">The address of the device to use.</param>
        /// <returns>true if the connection was successful.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/07/08 RCG 1.00           Created

        public abstract bool Connect(IntPtr hWnd, string DevAddr);

        /// <summary>
        /// Disconnects from radio hardware. This method will stop the radio.
        /// </summary>
        /// <returns>true if the hardware is available</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/07/07 mcm 8.10.26 
        ///</remarks>
        public abstract void Disconnect();
        
        /// <summary>Starts the Radio. Returns a ZigbeeResult indicating success.  
        /// This method can be used to detect whether the radio exists.
        /// </summary>
        /// <param name="MAC">MAC address for the radio</param>
        /// <param name="LogicalType">The type of device to configure.  When
        /// joined to a cell relay, this should be a router, otherwise this
        /// should be a coordinator</param>
        /// <param name="ScanChannels">This now represents the logical channel
		/// id.  If the channel is 0 then the dongle will try a mask of multiple
		/// channels.  This is only supported in the dongle, not the belt clip
		/// radio.
		/// 
		/// Note: this is the old definintion: Packed bits representing the channels
        /// to search.  Only channels 15-26 are valid, so only bits 15 (0x800)
        /// through bit 26 (0x4000000).  Note that bits are 0 indexed, so 
        /// bit 0 = 0x01.</param>
        /// <param name="ExPanID">The 8 byte extended Pan ID you want to start 
        /// with.  This value can be 0, which will cause the radio to either 
        /// assign one at random or join the first suitable network it finds 
        /// depending on the LogicalType.</param>
        /// <returns>True if the radio exists and was successfully started</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/08/06 mcm 8.10.26 
        ///</remarks>
        public abstract ZigbeeResult Start(ulong MAC, ulong ExPanID, 
            ZigbeeLogicalType LogicalType, uint ScanChannels);

        /// <summary>Stops the Radio. </summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/30/07 mcm 8.10.05        Initial Release
        /// </remarks>
        public abstract void Stop();

        /// <summary>
        /// Finds the networks around the dongle. Note that radios may be 
        /// limited in the number of networks they keep track of, so you may
        /// not get a complete list, and the list may vary.
        /// </summary>
        /// <param name="ScanChannels">Packed bits representing the channels
        /// to search.  Only channels 15-26 are valid, so only bits 15 (0x800)
        /// through bit 26 (0x4000000).  Note that bits are 0 indexed, so 
        /// bit 0 = 0x01.</param>
        /// <param name="Networks">Returned array of found networks</param>
        /// <param name="Fast">Indicates whether or not the duration period 
        /// used during scan should be set to a small number.</param>
        /// <returns>ZigbeeResult indicating success of search</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/08/06 mcm 1.0.0   Initial Release
        // 09/16/15 jrf 4.21.04 616082 Added fast parameter.
        public abstract ZigbeeResult FindNetworks(uint ScanChannels, 
            out ZigbeeNetwork[] Networks, bool Fast = false);

        /// <summary>
        /// Sends a series of find network commands (beacons) in quick succession.
        /// This helps some devices with communication issues to become 
        /// responsive.
        /// </summary>
        /// <param name="Channels">Packed bits representing the channels
        /// to search.  Only channels 15-26 are valid, so only bits 15 (0x800)
        /// through bit 26 (0x4000000).  Note that bits are 0 indexed, so 
        /// bit 0 = 0x01.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/16/15 jrf 4.21.04 616082 Created.
        public virtual ZigbeeNetwork[] SendBeaconBurst(uint Channels)
        {
            ZigbeeNetwork[] ZigBeeNetworks = null;
            ZigbeeResult Result = ZigbeeResult.ERROR;
            List<ZigbeeNetwork> AccumulatedZigBeeNetworks = new List<ZigbeeNetwork>();

            for (int i = 0; i < NUMBER_OF_BEACONS; i++)
            {
                Result = FindNetworks(Channels, out ZigBeeNetworks, true);
                if (null != ZigBeeNetworks)
                {
                    AccumulatedZigBeeNetworks.AddRange(ZigBeeNetworks);
                }
            }

            return AccumulatedZigBeeNetworks.ToArray();
        }

        /// <summary>
        /// Sends a data packet to the Unencrypted OTA cluster
        /// </summary>
        /// <param name="TargetAddress"></param>
        /// <param name="Msg"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public abstract ZigbeeResult SendUnencryptedOTA(ushort TargetAddress,
            byte[] Msg, out byte[] Response);

        /// <summary>
        /// Sends a data packet to the Data Request cluster
        /// </summary>
        /// <param name="TargetAddress"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public abstract ZigbeeResult SendDataRequest(ushort TargetAddress,
            byte[] Msg);



        /// <summary>
        /// When the radio is running as a coordinator and Trust Center for
        /// Gas and Water Modules and one of those end devices joins the 
        /// network, the radio class will raise a EndDeviceJoinedEvent. The
        /// client application must handle that event, decide whether to allow
        /// the device to join, and authenticate the device before the radio
        /// times out.  Call this methods to authenticate the device.
        /// </summary>
        /// <param name="AllowToJoin">True to allow the end device to join the
        /// network.  False to remove it from the network.</param>
        /// <param name="MAC">The MAC (IEEE) address of the device to 
        /// authenticate. This value is passed to the client in the 
        /// EndDeviceJoinedEvent's arguement.</param>
        /// <param name="ParentMAC">The MAC address of the parent the end
        /// device joined. This should always be this radio's MAC.  This value
        /// is passed to the client in the EndDeviceJoinedEvent's arguement.
        /// </param>
        /// <param name="SecureStatus">The security status the end device
        /// joined with This value is passed to the client in the 
        /// EndDeviceJoinedEvent's arguement.</param>
        public abstract void Authenticate(bool AllowToJoin, ulong MAC,
            ulong ParentMAC, byte SecureStatus);

        /// <summary>Translates the channel set to a string.  For example,
        /// translating the GAS_MODULE_CHANNELS (0x007108000) would result
        /// in "15, 20, 24, 25, 26"</summary>
        /// <param name="Channels"></param>
        /// <returns></returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/11/07 mcm 1.0.0   Initial Release
        ///</remarks>
        public static string TranslateChannelBits(uint Channels)
        {
            string strChannels = "";
            uint BitMask = 1;

            for (int BitIndex = 0; BitIndex < 32; BitIndex++)
            {
                if (0 != ((BitMask << BitIndex) & Channels))
                {
                    if (strChannels.Length > 0)
                    {
                        strChannels += ", ";
                    }
                    strChannels += BitIndex.ToString(CultureInfo.InvariantCulture);
                }
            }

            return strChannels;
        }

        #endregion Public ZigBee-Specific Methods

        #region Protected Methods

        /// <summary>
        /// Takes an encrypted key in the form of a string as input
        /// and applies the DES encryption algorithm to produce an
        /// unencrypted byte array
        /// </summary>
        /// <param name="strEncryptedKey">Encrypted security key</param>
        /// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/21/13 RCG 2.70.77        Copied from CENTRON_AMI

        private byte[] DecryptHANKey(string strEncryptedKey)
        {
            int Discarded;

            SecureDataStorage DataStorage = null;
            DESCryptoServiceProvider Crypto = null;
            MemoryStream EncryptedStream = null;
            MemoryStream DecryptedStream = null;
            string strDecryptedKey = null;
            StreamReader sr = null;

            try
            {
                DataStorage = new SecureDataStorage(SecureDataStorage.DEFAULT_LOCATION);
                Crypto = new DESCryptoServiceProvider();
                Crypto.Key = DataStorage.RetrieveSecureData(SecureDataStorage.ZIGBEE_KEY_ID);
                Crypto.IV = DataStorage.RetrieveSecureData(SecureDataStorage.ZIGBEE_IV_ID);


                byte[] EncryptedKey = HexEncoding.GetBytes(strEncryptedKey, out Discarded);

                //Create a memory stream to the passed buffer.
                EncryptedStream = new MemoryStream(EncryptedKey);
                DecryptedStream = new MemoryStream();

                Encryption.DecryptData(Crypto, EncryptedStream, DecryptedStream);

                //We must rewind the stream
                DecryptedStream.Position = 0;

                // Create a StreamReader for reading the stream.
                sr = new StreamReader(DecryptedStream);

                // Read the stream as a string.
                strDecryptedKey = sr.ReadLine();
            }
            finally
            {
                // Close the streams.
                //Closing sr also closes DecryptedStream
                if (null != sr)
                {
                    sr.Close();
                }
                else
                {
                    DecryptedStream.Close();
                }
                EncryptedStream.Close();
                Crypto.Dispose();
            }
            //Transform the string into a byte array
            return HexEncoding.GetBytes(strDecryptedKey, out Discarded);
        }

        /// <summary>
        /// Reads the encrypted key from the registry and decrypts the byte array.
        /// This version of GetSecurityKey can get either of the HAN security keys.
        /// </summary>
        /// <param name="IncludeSeqNbr">Whether or not to include the sequence
        /// number at the start of the key</param>
        /// <param name="KeyName">The name of the key to fetch - either the
        /// network key or the global link key
        /// </param>
        /// <returns>the decrypted key as a byte array</returns>
        //
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/11/08 AF  1.50.04        Created
        // 10/02/09 AF  2.30.05        Added Compare() parameter for compiler warning
        // 11/25/09 AF  2.30.22        Changed String.Compare() to quiet compiler warning
        // 06/12/12 RCG 2.70.77        Changed to use the Secured Encryption Keys

        protected byte[] GetSecurityKey(bool IncludeSeqNbr, string KeyName)
        {
            byte[] DecryptedKeyWithSeqNumber = new byte[17];
            byte[] DecryptedKey;

            string EncryptedKey = (string)CRegistryHelper.GetApplicationValue("GasPro", KeyName);

            DecryptedKey = DecryptHANKey(EncryptedKey);

            // By definition the sequence number is the first byte of the key
            DecryptedKeyWithSeqNumber[0] = DecryptedKey[0];
            Array.Copy(DecryptedKey, 0, DecryptedKeyWithSeqNumber, 1, DecryptedKey.Length);

            if (IncludeSeqNbr)
            {
                return DecryptedKeyWithSeqNumber;
            }
            else
            {
                return DecryptedKey;
            }
        }

        /// <summary>
        /// Reads the encrypted key from the registry and decrypts the byte array.
        /// </summary>
        /// <param name="IncludeSeqNbr">Whether or not to include the sequence
        /// number at the start of the key</param>
        /// <returns>the decrypted key as a byte array</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/14/07 mcm 8.10.27        Added to support customer specific security keys
        // 04/10/08 AF  1.50.15        Corrected the hard coded default marketing key
        // 06/17/08 AF  1.50.37        Modified to call the other GetSecurityKey
        // 
        protected byte[] GetSecurityKey( bool IncludeSeqNbr )
        {
            return GetSecurityKey(IncludeSeqNbr, ENCRYPTED_KEY_NAME);
        }

        #endregion Protected Methods

        /// <summary>
        /// 
        /// </summary>
        protected ZigbeeLogicalType m_RadioType;

        /// <summary>
        /// 
        /// </summary>
        protected ushort m_TargetShortAddress = COORDINATOR_SHORT_ADDRESS;

        /// <summary>
        /// 
        /// </summary>
        protected static Logger m_Logger = Logger.TheInstance;

        

    }
}
