using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using Itron.Metering.Communications;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    
    /// <summary>
    /// SCSDevice class - This is the "device server" for the SCS device.
    /// (IConfiguration implementation)
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ -------------------------------------------
    /// 01/16/07 jrf 8.00    N/A    Created
    ///
    public abstract partial class SCSDevice : IConfiguration
    {

        #region Methods
        
        /// <summary>
        /// Implements the IConfiguration interface.  Initializes the meter
        /// based on the given program name.
        /// </summary>
        /// <param name="strProgramName">The name of the program to use to 
        /// initialize the meter</param>
        /// <returns>ConfigurationResult</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 01/17/07 jrf 8.00    N/A    Created
        // 01/25/07 jrf 8.00    N/A    Modifed to reflect changes to IntializeDevice(),
        //                             now pass in optical probe and not device ID
        // 02/21/07 jrf 8.00.13        Updating the name of the namespace and class
        //                             for calling initialize device.
        // 03/28/07 jrf 8.00.23 2748   Determine from replica setting the reset billing 
        //                             registers on initialization option and pass in 
        //                             call to DeviceWrapper.
        // 04/11/06 jrf 8.00.29 2891   We need to set the baud rate when logging 
        //                              back on after initialization otherwise 
        //                              logging on at 4800 baud will fail.
        //
        public ConfigurationResult Configure(string strProgramName)
        {
            ConfigurationResult InitResult = ConfigurationResult.ERROR;
            ItronDeviceResult Result = ItronDeviceResult.ERROR;

            //Save off current communication information            
            string strPortName = m_SCSProtocol.m_CommPort.PortName;
            uint uiPortNumber = 0;
            uint uiBaudRate = m_SCSProtocol.m_CommPort.BaudRate;
            OpticalProbeTypes OpticalProbe =  m_SCSProtocol.m_CommPort.OpticalProbe;

            // Let the replica settings determine what we should do concerning 
            // resetting billing registers
            CXMLFieldProSettings xmlFieldProSettings = new CXMLFieldProSettings(""); ;

            CXMLFieldProSettingsAllDevices.RESET_BILLING_REG_OPTIONS ResetBillingOption
                                = xmlFieldProSettings.AllDevices.ResetBillingOnInit;

            //Before we logoff we need to get the security code, since it will
            //get cleared
            string strSecurityCode = m_strCurrentSecurityCode;

            //Convert the port name to port number
            string temp = strPortName.Remove(0, 3);
            temp = temp.TrimEnd(':');
            uiPortNumber = Convert.ToUInt32(temp, CultureInfo.InvariantCulture);

            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Initializing Device");

            //Logoff the current device
            Logoff();

            //Close the port
            m_SCSProtocol.m_CommPort.ClosePort();

            try
            {
                //Initialize device
                InitResult = (ConfigurationResult) 
                    Itron.Metering.DeviceWrapper.DeviceWrapper.InitializeDevice(strProgramName,
                                                                                  MeterType,
                                                                                  strSecurityCode,
                                                                                  uiPortNumber,
                                                                                  uiBaudRate,
                                                                                  OpticalProbe,
                                                                                  ResetBillingOption);
            }
            catch
            {
                InitResult = ConfigurationResult.ERROR;
            }

            switch ( InitResult )
            {
                // For these errors, lets log back on so the user may continue
                case ConfigurationResult.MEMORY_ERROR:
                case ConfigurationResult.IO_ERROR:
                case ConfigurationResult.SECURITY_ERROR:
                case ConfigurationResult.UNSUPPORTED_FUNCTION:
                case ConfigurationResult.MISMATCH_ID:
                case ConfigurationResult.USER_ABORT:
                case ConfigurationResult.DB_ACCESS_ERROR:
                case ConfigurationResult.INVALID_CONFIG:
                case ConfigurationResult.MEMORY_MAP_ERROR:
                    {
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

                        break;
                    }
                default:
                    {
                        // Do nothing. Remain Logged off
                        break;
                    }
            }                                                                           

            return InitResult;

        }

        /// <summary>
        /// Configures a device with the specified program and Prompt for data.
        /// </summary>
        /// <param name="sProgramName">Name or path of the program.</param>
        /// <param name="PFData">The prompt for data for the program.</param>
        /// <returns>A ConfigurationError code.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/27/08 RCG 10.00          Created

        public ConfigurationResult Configure(string sProgramName, PromptForData PFData)
        {
            // We currently don't have a prompt for for most devices so just call configure.
            return Configure(sProgramName);
        }

        #endregion


    }
}
