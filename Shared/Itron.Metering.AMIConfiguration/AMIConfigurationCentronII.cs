using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Runtime.InteropServices;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.AMIConfiguration
{
    #region AMIConfiguration class
    /// <summary>
    /// Configures an AMI meter from an EDL file and prompt for values 
    /// </summary>
    // Revison History:
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ----------------------------------------------------------------
    // 12/23/10 MMD         N/A    Created for MaxIMAGE
    [Guid("A917DD52-B34D-4d11-BD53-D5587FF3B492")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class AMIConfigurationCentronII : AMIConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/01/06 RCG 7.40.00 N/A    Created

        public AMIConfigurationCentronII() :base()
        {


        }


        /// <summary>
        /// Creates Instance of AMIConfirguration Device
        /// </summary>
        ///  Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 23/12/19 MMD           N/A	   Created

        public override void CreateInstance(CPSEM m_PSEM)
        {
            m_ConfigureDevice = new AMIConfigureCentronII(m_PSEM);
        }

   
        /// <summary>
        /// Configures the meter with the specified EDL file and does not display a prompt for dialog.
        /// </summary>
        /// <param name="strEDLFileName">Path and filename of the EDL file.</param>
        /// <returns>ConfigurationError code.</returns>
        // Revison History:
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 23/12/19 MMD           N/A	   Created

        [ComVisible(true)]
        public override ConfigurationError FactoryConfigure(string strEDLFileName)
        {
            m_ConfigureDevice.IsFactoryConfig = true;

            return m_ConfigureDevice.Configure(strEDLFileName);
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
        // 23/12/19 MMD           N/A	   Created

        [ComVisible(true)]
        public override ConfigurationError Configure(string strEDLFileName)
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
        // 23/12/19 MMD           N/A	   Created

        [ComVisible(true)]
        public override ConfigurationError SetValue(uint uiID, object objValue)
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
        // 23/12/19 MMD           N/A	   Created

        [ComVisible(true)]
        public override ConfigurationError GetValue(uint uiID, out object objValue)
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
        /// Contains the functions the configure the meter
        /// </summary>
        private AMIConfigureCentronII m_ConfigureDevice;
        #endregion
    }

    #endregion
}
