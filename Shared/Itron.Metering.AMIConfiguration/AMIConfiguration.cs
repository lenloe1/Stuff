///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any 
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                           Copyright © 20?? - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Xml;
using System.Runtime.InteropServices;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.AMIConfiguration
{

    #region ConfigurationError enumeration
    /// <summary>
    /// Error Codes for the Configuration object
    /// </summary>
    [Guid("2FEFC88F-F2E1-4dfa-88A0-6C1A4C4E3E23")]
    [ComVisible(true)]
    public enum ConfigurationError
    {
        /// <summary>
        /// Operation succeded
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Operation failed
        /// </summary>
        GENERAL_ERROR = 1,
        /// <summary>
        /// Operation failed - aborted by user
        /// </summary>
        USER_ABORT = 2,
        /// <summary>
        /// Operation failed - specified file was not found
        /// </summary>
        FILE_NOT_FOUND = 3,
        /// <summary>
        /// Operation failed - the program is not valid
        /// </summary>
        INVALID_PROGRAM = 4,
        /// <summary>
        /// Operation failed - specified item was not found
        /// </summary>
        ITEM_NOT_FOUND = 5,
        /// <summary>
        /// Operation failed - the device timed out
        /// </summary>
        TIMEOUT = 6,
        /// <summary>
        /// Operation failed - error communicating with device
        /// </summary>
        COMMUNICATION_ERROR = 7,
        /// <summary>
        /// Operation failed - error occured during the prompt for
        /// </summary>
        PROMPT_FOR_ERROR = 8,
        /// <summary>
        /// Operation failed - a procedure call failed
        /// </summary>
        FAILED_PROCEDURE = 9,
        /// <summary>
        /// Operation failed - the specified port could not be opened
        /// </summary>
        UNABLE_TO_OPEN_PORT = 10,

        // PSEM error codes
        /// <summary>
        /// Operation failed - PSEM service not supported
        /// </summary>
        SERVICE_NOT_SUPPORTED = 50,
        /// <summary>
        /// Operation failed - PSEM security error
        /// </summary>
        SECURITY_ERROR = 51,
        /// <summary>
        /// Operation failed - PSEM operation not possible
        /// </summary>
        OPERATION_NOT_POSSIBLE = 52,
        /// <summary>
        /// Operation failed - PSEM innapropriate action request
        /// </summary>
        INNAPROPRIATE_ACTION_REQUEST = 53,
        /// <summary>
        /// Operation failed - PSEM device busy
        /// </summary>
        DEVICE_BUSY = 54,
        /// <summary>
        /// Operation failed - PSEM data not ready
        /// </summary>
        DATA_NOT_READY = 55,
        /// <summary>
        /// Operation failed - PSEM data locked
        /// </summary>
        DATA_LOCKED = 56,
        /// <summary>
        /// Operation failed - PSEM renegotiate request
        /// </summary>
        RENEGOTIATE_REQUEST = 57,
        /// <summary>
        /// Operation faiiled - PSEM invalid service sequence state
        /// </summary>
        INVALID_SERVICE_SEQUENCE_STATE = 58,
        /// <summary>
        /// Operation failed - The ICS gateway address format is not valid
        /// </summary>
        INVALID_ICS_GATEWAY_ADDRESS = 59,
    }
    #endregion

    #region AMIConfigurationItem enumeration
    /// <summary>
    /// Constants used for calling SetValue and GetValue
    /// </summary>
    [Guid("4CD962D6-0A5F-4d11-9BCC-CE9BEF8F2156")]
    [ComVisible(true)]
    public enum AMIConfigurationItem
    {
        /// <summary>
        /// The meter's unit ID
        /// </summary>
        UNIT_ID = 0,
        /// <summary>
        /// The meter's customer serial number
        /// </summary>
        CUSTOMER_SERIAL_NUMBER = 1,
        /// <summary>
        /// Indicates if the meter is Canadian
        /// </summary>
        CANADIAN_METER = 2
    }
    #endregion

    #region ProcedureResultCodes
    /// <summary>
    /// Result codes that are returned in table 08 after a procedure has
    /// been written to table 07
    /// </summary>
    internal enum ProcedureResultCodes
    {
        /// <summary>
        /// COMPLETED - Procedure success
        /// </summary>
        COMPLETED = 0,
        /// <summary>
        /// NOT_FULLY_COMPLETED - Procedure in-progress
        /// </summary>
        NOT_FULLY_COMPLETED = 1,
        /// <summary>
        /// INVALID_PARAM - Procedure ignored
        /// </summary>
        INVALID_PARAM = 2,
        /// <summary>
        /// DEVICE_SETUP_CONFLICT - Procedure ignored
        /// </summary>
        DEVICE_SETUP_CONFLICT = 3,
        /// <summary>
        /// TIMING_CONSTRAINT - Procedure ignored
        /// </summary>
        TIMING_CONSTRAINT = 4,
        /// <summary>
        /// NO_AUTHORIZATION - Procedure ignored
        /// </summary>
        NO_AUTHORIZATION = 5,
        /// <summary>
        /// UNRECONGNIZED_PROC - Procedure ignored
        /// </summary>
        UNRECOGNIZED_PROC = 6
    }
    #endregion

    #region IANSIConfiguration interface

    /// <summary>
    /// Interface for ANSI meter Configuration classes
    /// </summary>
    [Guid("E05CF9CA-25B1-452b-8C9E-1716079808D6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IANSIConfiguration
    {
        /// <summary>
        /// Logs on to the meter
        /// </summary>
        /// <param name="usUserID">The user ID number</param>
        /// <param name="strUserName">The user's name</param>
        /// <param name="strPassword">Logon security password</param>
        /// <param name="strSerialPort">The serial port used to log on to the meter</param>
        /// <param name="uiBaudRate">The desired baud rate to use during communications</param>
        /// <returns>ConfigurationError code</returns>
        [DispId(1)]
        ConfigurationError Logon(ushort usUserID, String strUserName, String strPassword, String strSerialPort, uint uiBaudRate);

        /// <summary>
        /// Logs off of the meter
        /// </summary>
        /// <returns>ConfigurationError code</returns>
        [DispId(2)]
        ConfigurationError Logoff();

        /// <summary>
        /// Configures the meter without displaying a prompt for dialog
        /// </summary>
        /// <param name="strEDLFileName">The path of the EDL program</param>
        /// <returns>ConfigurationError code</returns>
        [DispId(3)]
        ConfigurationError FactoryConfigure(String strEDLFileName);

        /// <summary>
        /// Configures the meter and displays a prompt for dialog
        /// </summary>
        /// <param name="strEDLFileName">The path of the EDL program</param>
        /// <returns>ConfigurationError code</returns>
        [DispId(4)]
        ConfigurationError Configure(String strEDLFileName);

        /// <summary>
        /// Sets the specified item into the program
        /// </summary>
        /// <param name="uiID">The item to set.</param>
        /// <param name="objValue">The value of the item being set</param>
        /// <returns>ConfigurationError code</returns>
        [DispId(5)]
        ConfigurationError SetValue(uint uiID, object objValue);

        /// <summary>
        /// Gets the specified item from the program.
        /// </summary>
        /// <param name="uiID">The item to get.</param>
        /// <param name="objValue">The value of the item requested.</param>
        /// <returns>ConfigurationError code</returns>
        [DispId(6)]
        ConfigurationError GetValue(uint uiID, out object objValue);

        /// <summary>
        /// Configures only the register portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        [DispId(7)]
        ConfigurationError FactoryConfigureICSRegister(string strEDLFileName);

        /// <summary>
        /// Configures only the comm module portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICS comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICS comm module for filtering ERT data.</param>
        /// <returns>ConfigurationError code.</returns>
        /// <param name="byIsERTPopulated">Determines whether or not the ICS comm module puplishes ERT tables
        /// in std. table 0.</param>
        [DispId(8)]
        ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress,
            ushort usERTUtilityID, byte byIsERTPopulated);

        /// <summary>
        /// Configures only the comm module portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICS comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICS comm module for filtering ERT data.</param>
        /// <returns>ConfigurationError code.</returns>
        /// <param name="blnIsERTPopulated">Determines whether or not the ICS comm module puplishes ERT tables
        /// in std. table 0.</param>
        [DispId(9)]
        ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress,
            ushort usERTUtilityID, bool blnIsERTPopulated);

        /// <summary>
        /// Configures only the comm module portion of a 4G ICM meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICM comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICM comm module for filtering ERT data.
        /// A value of 0 will indicate utility ID should not be configured.</param>
        /// <returns>ConfigurationError code.</returns>
        [DispId(10)]
        ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress,
            ushort usERTUtilityID);
    }

    #endregion

    #region AMIConfiguration class
    /// <summary>
    /// Configures an AMI meter from an EDL file and prompt for values 
    /// </summary>
    // Revison History:
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ----------------------------------------------------------------
    // 12/23/10 MMD         N/A    Removed the sealed Identifier to be inherited by MaxIMAGE Class
    [Guid("A917DD52-B34D-4d11-BD53-D5587FF3B492")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class AMIConfiguration : IANSIConfiguration
    {
        #region Constants

        private const byte YES = 1;
        private const byte NO = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A    Created
        // 04/12/16 AF  4.50.244 674921 Added optional parameter for M2Gateway
        public AMIConfiguration(bool bIsM2Gateway = false)
        {
#if (!WindowsCE)
            m_SerialPort = new SerialCommDesktop();
#else
			m_SerialPort = new SerialCommCE();
#endif
            m_PSEM = new CPSEM(m_SerialPort);

            if (bIsM2Gateway)
            {
                CreateInstanceM2Gateway(m_PSEM);
            }
            else
            {
                CreateInstance(m_PSEM);
            }
           
        }

        /// <summary>
        /// Creates Instance of AMIConfirguration Device
        /// </summary>
        ///  Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 23/12/19 MMD           N/A	   Created

        public virtual void CreateInstance(CPSEM m_PSEM)
        {
            m_ConfigureDevice = new AMIConfigureDevice(m_PSEM);
        }

        /// <summary>
        /// Creates an instance of AMIConfigureM2Gateway
        /// </summary>
        /// <param name="m_PSEM"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  04/12/16 AF  4.50.244 WR 674921  Created
        //
        public void CreateInstanceM2Gateway(CPSEM m_PSEM)
        {
            m_ConfigureDevice = new AMIConfigureM2Gateway(m_PSEM);
        }

        /// <summary>
        /// Logs on to the meter.
        /// </summary>
        /// <param name="usUserID">User ID for the user logging on to the meter.</param>
        /// <param name="strUserName">User name for the user logging on to the meter.</param>
        /// <param name="strPassword">Password required for logging on to the meter.</param>
        /// <param name="strSerialPort">Specifies which serial port to use.</param>
        /// <param name="uiBaudRate">Specifies the baud rate to initialize with.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created
		// 03/13/07 mrj 8.00.18        Removed wait, keep alive is now used.

        [ComVisible(true)]
        public ConfigurationError Logon(ushort usUserID, String strUserName, string strPassword, string strSerialPort, uint uiBaudRate)
        {
            PSEMResponse PSEMResult = new PSEMResponse();

            // Set up the serial port
            if (!m_SerialPort.IsOpen)
            {
                try
                {
                    // Open the Serial Port
                    m_SerialPort.OpenPort(strSerialPort);
                }
                #region SerialPort Open catch statements
                catch (InvalidOperationException)
                {
                    // The port is already open
                    return ConfigurationError.UNABLE_TO_OPEN_PORT;
                }
                catch (ArgumentException)
                {
                    // The port name is invalid
                    return ConfigurationError.UNABLE_TO_OPEN_PORT;
                }
                catch (IOException)
                {
                    // The port is in an invalid state
                    return ConfigurationError.UNABLE_TO_OPEN_PORT;
                }
                catch (UnauthorizedAccessException)
                {
                    // Port access was denied
                    return ConfigurationError.UNABLE_TO_OPEN_PORT;
                }
                #endregion

                // Log on to the meter
                try
                {
                    PSEMResult = m_PSEM.Identify();

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMResult = m_PSEM.Negotiate(512, 254, uiBaudRate);
                    }

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMResult = m_PSEM.Logon(strUserName, usUserID); 
                    }

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        PSEMResult = m_PSEM.Security(strPassword);
                    }                    

                    if (PSEMResult != PSEMResponse.Ok)
                    {
                        // TODO: Add additional error translations
                        // Translate the PSEM error code
                        return AMIConfigureDevice.InterpretPSEMResult(PSEMResult);
                    }
                }
                catch (TimeOutException)
                {
                    // The meter has timed out so return an error
                    m_SerialPort.ClosePort();
                    return ConfigurationError.TIMEOUT;
                }
            }
            else
            {
                // The serial port is already open so we can not continue logging on
                return ConfigurationError.UNABLE_TO_OPEN_PORT;
            }

            return ConfigurationError.SUCCESS;
        }

        /// <summary>
        /// Logs off of the meter.
        /// </summary>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        [ComVisible(true)]
        public ConfigurationError Logoff()
        {
            try
            {
                m_PSEM.Logoff();
                m_PSEM.Terminate();
            }
            catch (Exception)
            {
                // We do not care if the logoff fails since we no longer wish to
                // communicate with the meter. The meter will recover on it's own.
            }

            m_SerialPort.ClosePort();

            return ConfigurationError.SUCCESS;

        }

        /// <summary>
        /// Configures the meter with the specified EDL file and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        [ComVisible(true)]
        public virtual ConfigurationError FactoryConfigure(string strEDLFileName)
        {
            m_ConfigureDevice.IsFactoryConfig = true;

            return m_ConfigureDevice.Configure(strEDLFileName);
        }

        /// <summary>
        /// Configures only the register portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/17/13 jrf 2.80.38 N/A    Created
        //
        [ComVisible(true)]
        public virtual ConfigurationError FactoryConfigureICSRegister(string strEDLFileName)
        {
            m_ConfigureDevice.IsFactoryConfig = true;
            m_ConfigureDevice.FactoryConfigType = AMIConfigureDevice.ConfigurationOptions.ICSRegisterOnly;

            return m_ConfigureDevice.Configure(strEDLFileName);
        }

        /// <summary>
        /// Configures only the comm module portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICS comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICS comm module for filtering ERT data.</param>
        /// <param name="byIsERTPopulated">Determines whether or not the ICS comm module puplishes ERT tables
        /// in std. table 0.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 06/17/13 jrf 2.80.38    N/A    Created
        // 07/18/13 jrf 2.80.54 WR 417794 Adding configuration of Is ERT Populated field in comm module.
        // 05/27/15 jrf 4.20.08 WR 585332 Adding try/catch per code review comments.
        [ComVisible(true)]
        public virtual ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress, 
            ushort usERTUtilityID, byte byIsERTPopulated)
        {
            ConfigurationError ConfigurationResult = ConfigurationError.SUCCESS;
            IPAddress GatewayAddress = null;
                        
            try
            {
                IPAddress.TryParse(strGatewayAddress, out GatewayAddress);
            }
            catch
            {
                ConfigurationResult = ConfigurationError.INVALID_ICS_GATEWAY_ADDRESS;
            }

            if (ConfigurationError.SUCCESS == ConfigurationResult)
            {

                m_ConfigureDevice.IsFactoryConfig = true;
                m_ConfigureDevice.FactoryConfigType = AMIConfigureDevice.ConfigurationOptions.ICSCommModuleOnly;
                m_ConfigureDevice.GatewayAddress = GatewayAddress;
                m_ConfigureDevice.ERTUtilityID = usERTUtilityID;
                m_ConfigureDevice.IsERTPopulated = byIsERTPopulated;

                ConfigurationResult = m_ConfigureDevice.Configure(strEDLFileName);
            }

            return ConfigurationResult;
        }

        /// <summary>
        /// Configures only the comm module portion of an ICS meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICS comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICS comm module for filtering ERT data.</param>
        /// <param name="blnIsERTPopulated">Determines whether or not the ICS comm module puplishes ERT tables
        /// in std. table 0.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 08/14/13 jrf 2.85.17 WR 417794 Created so IsERTPopulated can be passed in as bool.
        //
        [ComVisible(true)]
        public ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress,
            ushort usERTUtilityID, bool blnIsERTPopulated)
        {
            byte byIsERTPopulated = YES;

            if (false == blnIsERTPopulated)
            {
                byIsERTPopulated = NO;
            }

            return FactoryConfigureICSCommModule(strEDLFileName, strGatewayAddress, usERTUtilityID, byIsERTPopulated);            
        }

        /// <summary>
        /// Configures only the comm module portion of a 4G ICM meter with the specified EDL file 
        /// and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <param name="strGatewayAddress">The Gateway IP address of the ICS comm module.</param>
        /// <param name="usERTUtilityID">The utility ID used by the ICS comm module for filtering ERT data.
        /// A value of 0 will indicate utility ID should not be configured.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version ID Issue# Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 05/15/15 jrf 4.20.07 WR 585332 Created to not configure IsERTPopulated field.
        // 05/27/15 jrf 4.20.08 WR 585332 Adding try/catch per code review comments.
        [ComVisible(true)]
        public ConfigurationError FactoryConfigureICSCommModule(string strEDLFileName, string strGatewayAddress,
            ushort usERTUtilityID)
        {
            ConfigurationError ConfigurationResult = ConfigurationError.SUCCESS;
            IPAddress GatewayAddress = null;

            try
            {
                IPAddress.TryParse(strGatewayAddress, out GatewayAddress);
            }
            catch
            {
                ConfigurationResult = ConfigurationError.INVALID_ICS_GATEWAY_ADDRESS;
            }

            if (ConfigurationError.SUCCESS == ConfigurationResult)
            {

                m_ConfigureDevice.IsFactoryConfig = true;
                m_ConfigureDevice.FactoryConfigType = AMIConfigureDevice.ConfigurationOptions.ICSCommModuleOnly;
                m_ConfigureDevice.GatewayAddress = GatewayAddress;

                if (0 != usERTUtilityID)
                {
                    m_ConfigureDevice.ERTUtilityID = usERTUtilityID;
                }

                ConfigurationResult = m_ConfigureDevice.Configure(strEDLFileName);
            }
            
            return ConfigurationResult;
        }

        /// <summary>
        /// Configures the meter with the specified EDL file and displays a dialog
        /// to retrieve the prompt for values.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A	   Created

        [ComVisible(true)]
        public virtual ConfigurationError Configure(string strEDLFileName)
        {
            return m_ConfigureDevice.Configure(strEDLFileName);
        }

        /// <summary>
        /// Sets the value of the specified item.
        /// </summary>
        /// <param name="uiID">The ID of the item to set.</param>
        /// <param name="objValue">The value the item will be set to.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/25/06 RCG 7.40.00 N/A	   Created

        [ComVisible(true)]
        public virtual ConfigurationError SetValue(uint uiID, object objValue)
        {
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;

            switch (uiID)
            {
                case (uint)AMIConfigurationItem.UNIT_ID:
                    {
                        string strUnitID = objValue as string;

                        if (strUnitID != null)
                        {
                            m_ConfigureDevice.UnitID = strUnitID;
                            ConfigError = ConfigurationError.SUCCESS;
                        }
                        else
                        {
                            ConfigError = ConfigurationError.GENERAL_ERROR;
                        }

                        break;
                    }
                case (uint)AMIConfigurationItem.CUSTOMER_SERIAL_NUMBER:
                    {
                        string strCustomerSerialNumber = objValue as string;

                        if (strCustomerSerialNumber != null)
                        {
                            m_ConfigureDevice.CustomerSerialNumber = strCustomerSerialNumber;
                            ConfigError = ConfigurationError.SUCCESS;
                        }
                        else
                        {
                            ConfigError = ConfigurationError.GENERAL_ERROR;
                        }

                        break;
                    }
                case (uint)AMIConfigurationItem.CANADIAN_METER:
                    {
                        if (objValue is bool)
                        {
                            m_ConfigureDevice.IsCanadian = (bool)objValue;
                            ConfigError = ConfigurationError.SUCCESS;
                        }
                        else
                        {
                            ConfigError = ConfigurationError.GENERAL_ERROR;
                        }

                        break;
                    }
                default:
                    {
                        ConfigError = ConfigurationError.ITEM_NOT_FOUND;
                        break;
                    }
            }

            return ConfigError;
        }

        /// <summary>
        /// Returns a value for the specified item.
        /// </summary>
        /// <param name="uiID">The ID of the item to get.</param>
        /// <param name="objValue">The value of the item.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/25/06 RCG 7.40.00 N/A	   Created

        [ComVisible(true)]
        public virtual ConfigurationError GetValue(uint uiID, out object objValue)
        {
            ConfigurationError ConfigError = ConfigurationError.GENERAL_ERROR;
            object objOutput = null;

            switch (uiID)
            {
                case (uint)AMIConfigurationItem.UNIT_ID:
                    {
                        objOutput = (object)m_ConfigureDevice.UnitID;
                        ConfigError = ConfigurationError.SUCCESS;

                        break;
                    }
                case (uint)AMIConfigurationItem.CUSTOMER_SERIAL_NUMBER:
                    {
                        objOutput = (object)m_ConfigureDevice.CustomerSerialNumber;
                        ConfigError = ConfigurationError.SUCCESS;
                        
                        break;
                    }
                case (uint)AMIConfigurationItem.CANADIAN_METER:
                    {
                        objOutput = (object)m_ConfigureDevice.IsCanadian;
                        ConfigError = ConfigurationError.SUCCESS;

                        break;
                    }
                default:
                    {
                        objOutput = null;
                        ConfigError = ConfigurationError.ITEM_NOT_FOUND;
                        break;
                    }
            }

            objValue = objOutput;
            return ConfigError;
        }
        #endregion

        #region Member Variables

        /// <summary>
        /// SerialPort object used for communications with the meter.
        /// </summary>
        private ICommunications m_SerialPort;

        /// <summary>
        /// PSEM object used to send and recieve PSEM commands
        /// </summary>
        private CPSEM m_PSEM;

        /// <summary>
        /// Contains the functions the configure the meter
        /// </summary>
        private AMIConfigureDevice m_ConfigureDevice;
        #endregion
    }

    #endregion
}