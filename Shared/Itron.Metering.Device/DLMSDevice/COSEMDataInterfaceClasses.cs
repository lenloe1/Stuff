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
//                           Copyright © 2012 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Itron.Metering.Utilities;
using Itron.Metering.Communications.DLMS;

namespace Itron.Metering.Device.DLMSDevice
{
    #region enums
    /// <summary>
    /// The list of supported wire types
    /// </summary>
    public enum PhaseTypeCode : byte
    {
        /// <summary>
        /// Single Phase 2 Wire
        /// </summary>
        SinglePhase2Wire = 0,
        /// <summary>
        /// Single Phase 3 Wire
        /// </summary>
        SinglePhase3Wire = 1,
        /// <summary>
        /// Three Phase 3 Wire
        /// </summary>
        ThreePhase3Wire = 2,
        /// <summary>
        /// Three Phase 4 Wire
        /// </summary>
        ThreePhase4Wire = 3,
    };
    #endregion

    /// <summary>
    /// COSEM Interface Class used to store data
    /// </summary>
    public class COSEMDataInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Active Firmware Version" Data COSEM object
        /// </summary>
        public static readonly byte[] ACTIVE_FIRMWARE_VERSION_LN = new byte[] { 1, 1, 0, 2, 0, 255 };
        /// <summary>
        /// The logical name for the "Current transformation ratio (numerator)" Data COSEM object
        /// </summary>
        public static readonly byte[] CURRENT_TRANSFORMATION_RATIO_NUMERATOR_LN = new byte[] { 1, 0, 0, 4, 2, 255 };
        /// <summary>
        /// The logical name for the "Voltage transformation ratio (numerator)" Data COSEM object
        /// </summary>
        public static readonly byte[] VOLTAGE_TRANSFORMATION_RATIO_NUMERATOR_LN = new byte[] { 1, 0, 0, 4, 3, 255 };
        /// <summary>
        /// The logical name for the "Transformation ratio (numerator)" Data COSEM object
        /// </summary>
        public static readonly byte[] TRANSFORMATION_RATIO_NUMERATOR_LN = new byte[] { 1, 0, 0, 4, 4, 255 };
        /// <summary>
        /// The logical name for the "Current transformation ratio (denominator)" Data COSEM object
        /// </summary>
        public static readonly byte[] CURRENT_TRANSFORMATION_RATIO_DENOMINATOR_LN = new byte[] { 1, 0, 0, 4, 5, 255 };
        /// <summary>
        /// The logical name for the "Voltage transformation ratio (denominator)" Data COSEM object
        /// </summary>
        public static readonly byte[] VOLTAGE_TRANSFORMATION_RATIO_DENOMINATOR_LN = new byte[] { 1, 0, 0, 4, 6, 255 };
        /// <summary>
        /// The logical name for the "Transformation ratio (denominator)" Data COSEM object
        /// </summary>
        public static readonly byte[] TRANSFORMATION_RATIO_DENOMINATOR_LN = new byte[] { 1, 0, 0, 4, 7, 255 };
        /// <summary>
        /// The logical name for the "Multiplying Factor" Data COSEM object
        /// </summary>
        public static readonly byte[] MULTIPLYING_FACTOR_LN = new byte[] { 1, 65, 0, 16, 0, 255 };
        /// <summary>
        /// The logical name for the "Multiplying Factor Method" Data COSEM object
        /// </summary>
        public static readonly byte[] MULTIPLYING_FACTOR_METHOD_LN = new byte[] { 1, 65, 0, 16, 1, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Exported Energy" Data COSEM object
        /// </summary>
        public static readonly byte[] ENABLE_DISABLE_DISPLAY_ITEMS_LN = new byte[] { 1, 65, 0, 33, 0, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Exported Energy" Data COSEM object
        /// </summary>
        public static readonly byte[] OTHER_DISPLAY_ITEMS_LN = new byte[] { 1, 65, 0, 33, 1, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Exported Energy" Data COSEM object
        /// </summary>
        public static readonly byte[] ENABLE_DISABLE_FLICKER_LN = new byte[] { 1, 65, 0, 34, 0, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Exported Energy" Data COSEM object
        /// </summary>
        public static readonly byte[] FLICKER_STATE_LN = new byte[] { 1, 65, 0, 34, 1, 255 };
        /// <summary>
        /// The logical name for the "Current Time" Data COSEM object
        /// </summary>
        public static readonly byte[] CURRENT_TIME_LN = COSEMLogicalNamesDictionary.ParseLogicalName(CURRENT_TIME);
        /// <summary>
        /// The logical name for the "Current Date" Data COSEM object
        /// </summary>
        public static readonly byte[] CURRENT_DATE_LN = COSEMLogicalNamesDictionary.ParseLogicalName(CURRENT_DATE);
        /// <summary>
        /// The logical name for the "Enable/Disable Event recording" Data COSEM object
        /// </summary>
        public static readonly byte[] ENABLE_DISABLE_EVENTS_LN = new byte[] { 1, 65, 0, 17, 0, 255 };
        /// <summary>
        /// The logical name for the "Enable/Disable Event recording" Data COSEM object
        /// </summary>
        public static readonly byte[] EVENT_CODE_LN = new byte[] { 0, 0, 96, 11, 0, 255 };
        /// <summary>
        /// The logical name for the "Switching class" Data COSEM object
        /// </summary>
        public static readonly byte[] SWITCHING_CLASS_LN = new byte[] { 1, 65, 0, 128, 0, 255 };
        /// <summary>
        /// The logical name for the "Switching class" Data COSEM object
        /// </summary>
        public static readonly byte[] NUMBER_OF_SWITCH_OPERATIONS_LN = new byte[] { 1, 65, 0, 131, 0, 255 };
        /// <summary>
        /// The logical name for the "Upper Limits of Authentication Errors" Data COSEM object
        /// </summary>
        public static readonly byte[] UPPER_LIMIT_AUTH_ERRORS_LN = new byte[] { 0, 65, 43, 0, 0, 255 };
        /// <summary>Load Control (basic settings) Logical Name</summary>
        public static readonly byte[] LOAD_CONTROL_BASIC_LN = COSEMLogicalNamesDictionary.ParseLogicalName(LOAD_CONTROL_BASIC);
        /// <summary>Load Control (temporary settings) Logical Name</summary>
        public static readonly byte[] LOAD_CONTROL_TEMPORARY_LN = COSEMLogicalNamesDictionary.ParseLogicalName(LOAD_CONTROL_TEMP);
        /// <summary>Load Control (operation value) Logical Name</summary>
        public static readonly byte[] LOAD_CONTROL_OPERATIONAL_LN = COSEMLogicalNamesDictionary.ParseLogicalName(LOAD_CONTROL_OP_VALUE);
        /// <summary>Load Limit Reserve Logical Name</summary>
        public static readonly byte[] LOAD_LIMIT_RESERVE_LN = COSEMLogicalNamesDictionary.ParseLogicalName(LOAD_LIMIT_RESERVE);
        /// <summary>
        /// The logical name for the "Energization Start Time Setting" Data COSEM object
        /// </summary>
        public static readonly byte[] ENERGIZATION_START_TIME_SETTING_LN = new byte[] { 1, 65, 0, 142, 0, 255 };
        /// <summary>
        /// The logical name for the "Individual Energization Setting" Data COSEM object
        /// </summary>
        public static readonly byte[] INDIVIDUAL_ENERGIZATION_SETTING_LN = new byte[] { 1, 65, 0, 142, 1, 255 };
        /// <summary>
        /// The logical name for the "Multi-stage Energization Setting" Data COSEM object
        /// </summary>
        public static readonly byte[] MULTI_STAGE_ENERGIZATION_SETTING_LN = new byte[] { 1, 65, 0, 142, 2, 255 };
        /// <summary>
        /// The logical name for the "Phase/wire type" Data COSEM object
        /// </summary>
        public static readonly byte[] PHASE_WIRE_LN = new byte[] { 1, 0, 0, 2, 4, 255 };
        /// <summary>
        /// The logical name for the Meter Log File Data COSEM object
        /// </summary>
        public static readonly byte[] METER_LOGS_LN = new byte[] {0, 128, 96, 7, 1, 255 };


        private const string LOAD_CONTROL_BASIC = "1-65:0.129.0*255";
        private const string LOAD_CONTROL_TEMP = "1-65:0.129.1*255";
        private const string LOAD_CONTROL_OP_VALUE = "1-65:0.129.2*255";
        private const string LOAD_LIMIT_RESERVE = "1-65:0.130.0*255";
        private const string ENERGIZATION_START_TIME = "1-65:0.142.0*255";
        private const string INDIVIDUAL_ENERGIZATION_SETTINGS = "1-65:0.142.1*255";
        private const string MULTI_STAGE_ENERGIZATION_SETTINGS = "1-65:0.142.2*255";
        private const string CURRENT_DATE = "1-0:0.9.2*255";
        private const string CURRENT_TIME = "1-0:0.9.1*255";
        private const string CONFIGURATION_XML = "0-128:96.0.1*255";
        private const string RECONFIG_RESULT = "0-128:96.0.2*255";
        private const string BOOT_COUNT_UNLOCK = "0-157:0.189.203*255";
        private const string RANGE_FOR_REGISTRATION_ATTEMPTS = "0-128:96.1.3*255";
        private const string PUSH_DESTINATION_OVERRIDE = "0-128:96.1.4*255";

        // NGC OBIS Codes
        private const string NGC_DEBUG_METRICS = "0-128:96.11.0*255";
        private const string NGC_DEBUG_DETAIL = "0-128:96.11.1*255";
        private const string NGC_DEBUG_DESCRIPTION = "0-128:96.11.2*255";
        private const string NGC_DEBUG_UDP_METRICS = "0-128:96.11.3*255";
        private const string NGC_DEBUG_RPL_INSTANCE = "0-128:96.11.4*255";
        private const string NGC_DEBUG_RPL_CONFIG = "0-128:96.11.5*255";
        private const string NGC_DEBUG_WARM_START = "0-128:96.11.6*255";
        private const string NGC_DEBUG_RPL_STATS = "0-128:96.11.7*255";
        private const string NGC_DEBUG_NAN = "0-128:96.11.8*255";

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the appropriate instance of the Data Interface Class
        /// </summary>
        /// <param name="logicalName">The logical name of the interface class</param>
        /// <param name="dlms">The DLMS protocol object for the current session</param>
        /// <returns>The instance of the Interface Class</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public static COSEMDataInterfaceClass Create(byte[] logicalName, DLMSProtocol dlms)
        {
            string LNString = COSEMLogicalNamesDictionary.LogicalNameString(logicalName);
            COSEMDataInterfaceClass InterfaceClass = null;

            switch(LNString)
            {
                case LOAD_CONTROL_BASIC:
                case LOAD_CONTROL_TEMP:
                case LOAD_CONTROL_OP_VALUE:
                {
                    InterfaceClass = new COSEMLoadControlSettingsInterfaceClass(logicalName, dlms);
                    break;
                }
                case LOAD_LIMIT_RESERVE:
                {
                    InterfaceClass = new COSEMLoadLimitReserveInterfaceClass(logicalName, dlms);
                    break;
                }
                case ENERGIZATION_START_TIME:
                {
                    InterfaceClass = new COSEMEnergizationStartTimeInterfaceClass(logicalName, dlms);
                    break;
                }
                case INDIVIDUAL_ENERGIZATION_SETTINGS:
                {
                    InterfaceClass = new COSEMIndividualEnergizationSettingInterfaceClass(logicalName, dlms);
                    break;
                }
                case MULTI_STAGE_ENERGIZATION_SETTINGS:
                {
                    InterfaceClass = new COSEMMultiStageEnergizationSettingsInterfaceClass(logicalName, dlms);
                    break;
                }
                case CURRENT_DATE:
                {
                    InterfaceClass = new COSEMCurrentDateInterfaceClass(logicalName, dlms);
                    break;
                }
                case CURRENT_TIME:
                {
                    InterfaceClass = new COSEMCurrentTimeInterfaceClass(logicalName, dlms);
                    break;
                }
                case CONFIGURATION_XML:
                {
                    InterfaceClass = new COSEMConfigurationXMLInterfaceClass(logicalName, dlms);
                    break;
                }
                case RECONFIG_RESULT:
                {
                    InterfaceClass = new COSEMReconfigurationResultInterfaceClass(logicalName, dlms);
                    break;
                }
                case BOOT_COUNT_UNLOCK:
                {
                    InterfaceClass = new COSEMBootCountUnlockInterfaceClass(logicalName, dlms);
                    break;
                }
                case RANGE_FOR_REGISTRATION_ATTEMPTS:
                {
                    InterfaceClass = new COSEMRangeForRegistrationAttemptsInterfaceClass(logicalName, dlms);
                    break;
                }
                case PUSH_DESTINATION_OVERRIDE:
                {
                    InterfaceClass = new COSEMPushDestinationOverrideInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_METRICS:
                {
                    InterfaceClass = new NGCDebugMetricsInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_DETAIL:
                {
                    InterfaceClass = new NGCDetailedMetricsInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_DESCRIPTION:
                {
                    InterfaceClass = new NGCDescriptionInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_UDP_METRICS:
                {
                    InterfaceClass = new NGCUDPMetricsInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_RPL_INSTANCE:
                {
                    InterfaceClass = new NGCRPLInstanceInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_RPL_CONFIG:
                {
                    InterfaceClass = new NGCRPLConfigInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_WARM_START:
                {
                    InterfaceClass = new NGCRPLWarmStartInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_RPL_STATS:
                {
                    InterfaceClass = new NGCRPLStatsInterfaceClass(logicalName, dlms);
                    break;
                }
                case NGC_DEBUG_NAN:
                {
                    InterfaceClass = new NGCNANDriverInterfaceClass(logicalName, dlms);
                    break;
                }
                default:
                {
                    InterfaceClass = new COSEMDataInterfaceClass(logicalName, dlms);
                    break;
                }
            }

            return InterfaceClass;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/18/13 RCG 2.80.21 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", Value);
                    break;
                }
                default:
                {
                    throw new ArgumentException("Gets of attribute " + attributeID.ToString(CultureInfo.InvariantCulture) + " are not supported by this method");
                }
            }

            return AttributeDefinition;
        }

        /// <summary>
        /// Gets the Definition of the data returned by the specified attribute
        /// </summary>
        /// <param name="attributeID">The attribute to get the definition for</param>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/19/13 RCG 3.00.01 N/A    Created
        
        public virtual ObjectDefinition GetDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = new ObjectDefinition("Value", COSEMDataTypes.NullData);
                    AttributeDefinition.Value = null;
                    break;
                }
                default:
                {
                    throw new ArgumentException("Gets of attribute " + attributeID.ToString(CultureInfo.InvariantCulture) + " are not supported by this method");
                }
            }

            return AttributeDefinition;
        }

        /// <summary>
        /// Gets the name of the specified attribute
        /// </summary>
        /// <param name="attributeID">The attribute ID</param>
        /// <returns>The name of the attribute</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/16/13 RCG 2.80.54 N/A    Created

        public override string GetAttributeName(sbyte attributeID)
        {
            string Name = "";

            switch (attributeID)
            {
                case 2:
                {
                    Name = "Value";
                    break;
                }
                default:
                {
                    throw new ArgumentException("Attribute " + attributeID.ToString(CultureInfo.InvariantCulture) + " is not supported by this Interface Class.");
                }
            }

            return Name;
        }

        /// <summary>
        /// Gets the list of supported attributes
        /// </summary>
        /// <returns>The list of Attribute IDs supported</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/31/13 RCG 2.85.06 418571 Created
        
        public static List<sbyte> GetSupportedAttributes()
        {
            return (new sbyte[] { 1, 2 }).ToList();
        }

        /// <summary>
        /// Gets the list of supported methods
        /// </summary>
        /// <returns>The list of Method IDs supported</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/31/13 RCG 2.85.06 418571 Created
        
        public static List<sbyte> GetSupportedMethods()
        {
            return (new sbyte[] { }).ToList();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        protected COSEMDataInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 1;
            m_Version = 0;

            m_Value = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the value
        /// </summary>
        /// <param name="data">The Get Data Result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseValue(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_Value = data.DataValue;
                }
                catch (Exception e)
                {
                    m_Value = null;
                    WriteToLog("Failed to Get the Data Value - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Value = null;
                WriteToLog("Failed to Get the Data Value - Reason: " + data.DataAccessResult.ToDescription());
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the value of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMData Value
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseValue(Result);
                    }
                }

                return m_Value;
            }
            set
            {
                if (m_DLMS.IsConnected && value != null)
                {
                    DataAccessResults Result;
                    m_Value = value;

                    Result = m_DLMS.Set(m_ClassID, m_LogicalName, 2, m_Value);

                    if (Result != DataAccessResults.Success)
                    {
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Set, "The Value could not be set. Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result));
                    }
                }
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The data value</summary>
        protected COSEMData m_Value;

        #endregion
    }

    /// <summary>
    /// The Load Control Settings structure
    /// </summary>
    public class LoadControlSettings
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public LoadControlSettings()
        {
            m_LoadLimit = "";
            m_LoadCurrent = byte.MaxValue;
            m_AutoConnectTime = ushort.MaxValue;
            m_MaxAutoConnectsInPeriod = byte.MaxValue;
            m_AutoConnectPeriod = byte.MaxValue;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public LoadControlSettings(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 5)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_LoadLimit = (string)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Load Limit is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_LoadCurrent = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Load Current is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_AutoConnectTime = (ushort)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Switch Delay is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_MaxAutoConnectsInPeriod = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Switches in period is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_AutoConnectPeriod = (byte)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Switch Period is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 5.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Load Control Settings");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Load Limit", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = "";
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Load Current", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = byte.MaxValue;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Auto Connect Time (seconds)", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = ushort.MaxValue;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Max Auto Connect Count", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = byte.MaxValue;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Auto Connect Clear Time (minutes)", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = byte.MaxValue;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_LoadLimit;
            Definition.StructureDefinition[1].Value = m_LoadCurrent;
            Definition.StructureDefinition[2].Value = m_AutoConnectTime;
            Definition.StructureDefinition[3].Value = m_MaxAutoConnectsInPeriod;
            Definition.StructureDefinition[4].Value = m_AutoConnectPeriod;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets the Load Limit Settings as a COSEM Data object
        /// </summary>
        /// <returns>The settings as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/07/13 RCG 2.85.10 N/A    Created
        
        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[5];

            DataValue.DataType = COSEMDataTypes.Structure;

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.VisibleString;
            StructureData[0].Value = m_LoadLimit;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Unsigned;
            StructureData[1].Value = m_LoadCurrent;

            StructureData[2] = new COSEMData();
            StructureData[2].DataType = COSEMDataTypes.LongUnsigned;
            StructureData[2].Value = m_AutoConnectTime;

            StructureData[3] = new COSEMData();
            StructureData[3].DataType = COSEMDataTypes.Unsigned;
            StructureData[3].Value = m_MaxAutoConnectsInPeriod;

            StructureData[4] = new COSEMData();
            StructureData[4].DataType = COSEMDataTypes.Unsigned;
            StructureData[4].Value = m_AutoConnectPeriod;

            DataValue.Value = StructureData;

            return DataValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the load limit type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public string LoadLimit
        {
            get
            {
                return m_LoadLimit;
            }
            set
            {
                m_LoadLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the Load Current limit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public byte LoadCurrent
        {
            get
            {
                return m_LoadCurrent;
            }
            set
            {
                m_LoadCurrent = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of seconds before the auto reconnect will occur
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public ushort AutoConnectTime
        {
            get
            {
                return m_AutoConnectTime;
            }
            set
            {
                m_AutoConnectTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the Maximum Number of Auto Reconnects that can occur within the period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public byte MaxAutoConnectsInPeriod
        {
            get
            {
                return m_MaxAutoConnectsInPeriod;
            }
            set
            {
                m_MaxAutoConnectsInPeriod = value;
            }
        }

        /// <summary>
        /// Gets or sets the Switch Period in minutes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public byte AutoConnectPeriod
        {
            get
            {
                return m_AutoConnectPeriod;
            }
            set
            {
                m_AutoConnectPeriod = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_LoadLimit;
        private byte m_LoadCurrent;
        private ushort m_AutoConnectTime;
        private byte m_MaxAutoConnectsInPeriod;
        private byte m_AutoConnectPeriod;

        #endregion
    }

    /// <summary>
    /// Load Control settings interface class
    /// </summary>
    public class COSEMLoadControlSettingsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public COSEMLoadControlSettingsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    LoadControlSettings CurrentSettings = Settings;

                    if (CurrentSettings != null)
                    {
                        AttributeDefinition = CurrentSettings.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Load Control Settings
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public LoadControlSettings Settings
        {
            get
            {
                LoadControlSettings CurrentSettings = null;
                COSEMData CurrentValue = Value;

                if (CurrentValue != null && CurrentValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentSettings = new LoadControlSettings(CurrentValue);
                }

                return CurrentSettings;
            }
            set
            {
                if (value != null)
                {
                    COSEMData DataValue = value.ToCOSEMData();

                    Value = DataValue;
                }
                else
                {
                    throw new ArgumentNullException("The Settings may not be set to null");
        }
            }
        }

        #endregion
    }

    /// <summary>
    /// Switch Reservation Definition Structure
    /// </summary>
    public class SwitchReservationDefinition
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public SwitchReservationDefinition()
        {
            m_ReserveDateTime = new COSEMDateTime();
            m_Settings = new LoadControlSettings();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public SwitchReservationDefinition(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DateTime)
                        {
                            m_ReserveDateTime = StructureData[0].Value as COSEMDateTime;
                        }
                        else
                        {
                            throw new ArgumentException("The Reserve Date is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Structure)
                        {
                            m_Settings = new LoadControlSettings(StructureData[1]);
                        }
                        else
                        {
                            throw new ArgumentException("The Load Control Setting is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Load Control Settings");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Reserve Date", COSEMDataTypes.DateTime);
            NewObjectDefinition.Value = new COSEMDateTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            Definition.StructureDefinition.Add(LoadControlSettings.GetStructureDefinition());

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ReserveDateTime;
            Definition.StructureDefinition[1] = m_Settings.ToObjectDefinition();

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets the object as a COSEM Data object
        /// </summary>
        /// <returns>The Switch Reservation as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/07/13 RCG 2.85.10 N/A    Created
        
        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.DateTime;
            StructureData[0].Value = m_ReserveDateTime;

            StructureData[1] = m_Settings.ToCOSEMData();

            DataValue.DataType = COSEMDataTypes.Structure;
            DataValue.Value = StructureData;

            return DataValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Reserve Date Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public COSEMDateTime ReserveDateTime
        {
            get
            {
                return m_ReserveDateTime;
            }
            set
            {
                m_ReserveDateTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the settings
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public LoadControlSettings Settings
        {
            get
            {
                return m_Settings;
            }
            set
            {
                m_Settings = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMDateTime m_ReserveDateTime;
        private LoadControlSettings m_Settings;

        #endregion
    }

    /// <summary>
    /// Load Limit Reserve Interface Class
    /// </summary>
    public class COSEMLoadLimitReserveInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public COSEMLoadLimitReserveInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    List<SwitchReservationDefinition> CurrentReservations = Reservations;

                    if (CurrentReservations != null)
                    {
                        ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Switch Reservation Definitions", SwitchReservationDefinition.GetStructureDefinition());

                        foreach (SwitchReservationDefinition CurrentElement in CurrentReservations)
                        {
                            ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                        }

                        AttributeDefinition = ArrayDefinition;
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Load Control Settings
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/13 RCG 2.85.00 N/A    Created
        
        public List<SwitchReservationDefinition> Reservations
        {
            get
            {
                List<SwitchReservationDefinition> CurrentReservations = new List<SwitchReservationDefinition>();
                COSEMData CurrentValue = Value;

                if (CurrentValue != null && CurrentValue.DataType == COSEMDataTypes.Array)
                {
                    COSEMData[] ArrayData = CurrentValue.Value as COSEMData[];

                    foreach (COSEMData CurrentElement in ArrayData)
                    {
                        CurrentReservations.Add(new SwitchReservationDefinition(CurrentElement));
                    }
                }

                return CurrentReservations;
            }
            set
            {
                if (value != null)
                {
                    COSEMData DataValue = new COSEMData();
                    COSEMData[] ArrayData = new COSEMData[value.Count];

                    for (int iIndex = 0; iIndex < value.Count; iIndex++)
                    {
                        ArrayData[iIndex] = value[iIndex].ToCOSEMData();
                    }

                    DataValue.DataType = COSEMDataTypes.Array;
                    DataValue.Value = ArrayData;

                    Value = DataValue;
                }
                else
                {
                    throw new ArgumentNullException("value", "The value may not be set to null");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Energization Start Time Setting Structure
    /// </summary>
    public class EnergizationStartTimeSetting
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public EnergizationStartTimeSetting()
        {
            m_TimeSwitchFunction = "";
            m_EnergizationTime = 0;
            m_StartTime = new COSEMTime();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public EnergizationStartTimeSetting(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 3)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_TimeSwitchFunction = StructureData[0].Value as string;
                        }
                        else
                        {
                            throw new ArgumentException("The Time Switch Function is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_EnergizationTime = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Energization Time is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Time)
                        {
                            m_StartTime = StructureData[2].Value as COSEMTime;
                        }
                        else if (StructureData[2].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[2].Value as byte[];

                            if (TimeData != null)
                            {
                                m_StartTime = new COSEMTime(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The Start Time is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Start Time is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 3.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Energization Start Time Setting");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Time Switch Function", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = "";
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Energization Time", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Start Time", COSEMDataTypes.Time);
            NewObjectDefinition.Value = new COSEMTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_TimeSwitchFunction;
            Definition.StructureDefinition[1].Value = m_EnergizationTime;
            Definition.StructureDefinition[2].Value = m_StartTime;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets the Energization StartTime Setting as a COSEM Data object
        /// </summary>
        /// <returns>The settings as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/13 PGH 2.85.19    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[3];

            DataValue.DataType = COSEMDataTypes.Structure;

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.VisibleString;
            StructureData[0].Value = m_TimeSwitchFunction;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Unsigned;
            StructureData[1].Value = m_EnergizationTime;

            StructureData[2] = new COSEMData();
            StructureData[2].DataType = COSEMDataTypes.Time;
            StructureData[2].Value = m_StartTime;

            DataValue.Value = StructureData;

            return DataValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time Switch Function
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public string TimeSwitchFunction
        {
            get
            {
                return m_TimeSwitchFunction;
            }
            set
            {
                m_TimeSwitchFunction = value;
            }
        }

        /// <summary>
        /// Gets or sets the Energization Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public byte EnergizationTime
        {
            get
            {
                return m_EnergizationTime;
            }
            set
            {
                m_EnergizationTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the Start Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_TimeSwitchFunction;
        private byte m_EnergizationTime;
        private COSEMTime m_StartTime;

        #endregion
    }

    /// <summary>
    /// Individual Energization Setting Structure
    /// </summary>
    public class IndividualEnergizationSetting
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public IndividualEnergizationSetting()
        {
            m_StartDateTime = new COSEMDateTime();
            m_EndDateTime = new COSEMDateTime();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public IndividualEnergizationSetting(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DateTime)
                        {
                            m_StartDateTime = StructureData[0].Value as COSEMDateTime;
                        }
                        else if (StructureData[0].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[0].Value as byte[];

                            if (TimeData != null)
                            {
                                m_StartDateTime = new COSEMDateTime(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The Start Time is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Start Time is not the expected data type.");
                        }


                        if (StructureData[1].DataType == COSEMDataTypes.DateTime)
                        {
                            m_EndDateTime = StructureData[1].Value as COSEMDateTime;
                        }
                        else if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[1].Value as byte[];

                            if (TimeData != null)
                            {
                                m_EndDateTime = new COSEMDateTime(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The End Time is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The End Time is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Individual Energization Setting");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Start Date and Time", COSEMDataTypes.DateTime);
            NewObjectDefinition.Value = new COSEMDateTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("End Date and Time", COSEMDataTypes.DateTime);
            NewObjectDefinition.Value = new COSEMDateTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_StartDateTime;
            Definition.StructureDefinition[1].Value = m_EndDateTime;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets the Individual Energization Setting as a COSEM Data object
        /// </summary>
        /// <returns>The settings as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/13 PGH 2.85.19    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];

            DataValue.DataType = COSEMDataTypes.Structure;

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.DateTime;
            StructureData[0].Value = m_StartDateTime;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.DateTime;
            StructureData[1].Value = m_EndDateTime;

            DataValue.Value = StructureData;

            return DataValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Start Date Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMDateTime StartDateTime
        {
            get
            {
                return m_StartDateTime;
            }
            set
            {
                m_StartDateTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the End Date Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMDateTime EndDateTime
        {
            get
            {
                return m_EndDateTime;
            }
            set
            {
                m_EndDateTime = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMDateTime m_StartDateTime;
        private COSEMDateTime m_EndDateTime;

        #endregion
    }

    /// <summary>
    /// Multi Stage Energization Time structure
    /// </summary>
    public class MultiStageEnergizationTimes
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public MultiStageEnergizationTimes()
        {
            m_DeenergizationTime = new COSEMTime();
            m_EnergizationTime = new COSEMTime();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public MultiStageEnergizationTimes(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Time)
                        {
                            m_DeenergizationTime = StructureData[0].Value as COSEMTime;
                        }
                        else if (StructureData[0].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[0].Value as byte[];

                            if (TimeData != null)
                            {
                                m_DeenergizationTime = new COSEMTime(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The Deenergization Time is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Deenergization Time is not the expected data type.");
                        }


                        if (StructureData[1].DataType == COSEMDataTypes.Time)
                        {
                            m_EnergizationTime = StructureData[1].Value as COSEMTime;
                        }
                        else if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[1].Value as byte[];

                            if (TimeData != null)
                            {
                                m_EnergizationTime = new COSEMTime(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The Energization Time is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Energization Time is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Multi Stage Energization Times");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Deenergization Time", COSEMDataTypes.Time);
            NewObjectDefinition.Value = new COSEMTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Energization Time", COSEMDataTypes.Time);
            NewObjectDefinition.Value = new COSEMTime();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_DeenergizationTime;
            Definition.StructureDefinition[1].Value = m_EnergizationTime;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Deenergization Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMTime DeenergizationTime
        {
            get
            {
                return m_DeenergizationTime;
            }
            set
            {
                m_DeenergizationTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the Energization Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMTime EnergizationTime
        {
            get
            {
                return m_EnergizationTime;
            }
            set
            {
                m_EnergizationTime = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMTime m_DeenergizationTime;
        private COSEMTime m_EnergizationTime;

        #endregion
    }

    /// <summary>
    /// Multi Stage Energization Structure
    /// </summary>
    public class MultiStageEnergizationSetting
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public MultiStageEnergizationSetting()
        {
            m_TimeSwitchFunction = "";
            m_StartMonthAndDay = new COSEMDate();
            m_EndMonthAndDay = new COSEMDate();
            m_EnergizationTimes = new List<MultiStageEnergizationTimes>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Load Control Settings</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public MultiStageEnergizationSetting(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 4)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_TimeSwitchFunction = StructureData[0].Value as string;
                        }
                        else
                        {
                            throw new ArgumentException("The Time Switch Function is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Date)
                        {
                            m_StartMonthAndDay = StructureData[1].Value as COSEMDate;
                        }
                        else if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[1].Value as byte[];

                            if (TimeData != null)
                            {
                                m_StartMonthAndDay = new COSEMDate(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The Start Month and Day is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Start Month and Day is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Date)
                        {
                            m_EndMonthAndDay = StructureData[2].Value as COSEMDate;
                        }
                        else if (StructureData[2].DataType == COSEMDataTypes.OctetString)
                        {
                            byte[] TimeData = StructureData[2].Value as byte[];

                            if (TimeData != null)
                            {
                                m_EndMonthAndDay = new COSEMDate(TimeData);
                            }
                            else
                            {
                                throw new ArgumentException("The End Month and Day is null");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The End Month and Day is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] ArrayData = StructureData[3].Value as COSEMData[];
                            m_EnergizationTimes = new List<MultiStageEnergizationTimes>();

                            if (ArrayData != null)
                            {
                                foreach (COSEMData CurrentElement in ArrayData)
                                {
                                    m_EnergizationTimes.Add(new MultiStageEnergizationTimes(CurrentElement));
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Multi Stage Energization Times are not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 4.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Multi Stage Energization Settings");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Time Switch Function", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = "";
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Start Month and Day", COSEMDataTypes.Date);
            NewObjectDefinition.Value = new COSEMDate();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("End Month and Day", COSEMDataTypes.Date);
            NewObjectDefinition.Value = new COSEMDate();
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ArrayObjectDefinition("Multi Stage Energization Times", MultiStageEnergizationTimes.GetStructureDefinition());
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();
            ArrayObjectDefinition ArrayDefinition;

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_TimeSwitchFunction;
            Definition.StructureDefinition[1].Value = m_StartMonthAndDay;
            Definition.StructureDefinition[2].Value = m_EndMonthAndDay;

            ArrayDefinition = Definition.StructureDefinition[3] as ArrayObjectDefinition;

            if (ArrayDefinition != null)
            {
                foreach (MultiStageEnergizationTimes CurrentElement in m_EnergizationTimes)
                {
                    ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                }
            }

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets the Multi-stage Energization Setting as a COSEM Data object
        /// </summary>
        /// <returns>The settings as a COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/15/13 PGH 2.85.19    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[4];

            DataValue.DataType = COSEMDataTypes.Structure;

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.VisibleString;
            StructureData[0].Value = m_TimeSwitchFunction;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Date;
            StructureData[1].Value = m_StartMonthAndDay;

            StructureData[2] = new COSEMData();
            StructureData[2].DataType = COSEMDataTypes.Date;
            StructureData[2].Value = m_EndMonthAndDay;

            StructureData[3] = new COSEMData();
            StructureData[3].DataType = COSEMDataTypes.Array;
            StructureData[3].Value = m_EnergizationTimes;

            DataValue.Value = StructureData;

            return DataValue;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time Switch Function
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public string TimeSwitchFunction
        {
            get
            {
                return m_TimeSwitchFunction;
            }
            set
            {
                m_TimeSwitchFunction = value;
            }
        }

        /// <summary>
        /// Gets or set the Start Month and Day
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMDate StartMonthAndDay
        {
            get
            {
                return m_StartMonthAndDay;
            }
            set
            {
                m_StartMonthAndDay = value;
            }
        }

        /// <summary>
        /// Gets or sets the End Month and Day
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMDate EndMonthAndDay
        {
            get
            {
                return m_EndMonthAndDay;
            }
            set
            {
                m_EndMonthAndDay = value;
            }
        }

        /// <summary>
        /// Gets or sets the energization times
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public List<MultiStageEnergizationTimes> EnergizationTimes
        {
            get
            {
                return m_EnergizationTimes;
            }
            set
            {
                m_EnergizationTimes = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_TimeSwitchFunction;
        private COSEMDate m_StartMonthAndDay;
        private COSEMDate m_EndMonthAndDay;
        private List<MultiStageEnergizationTimes> m_EnergizationTimes;

        #endregion
    }

    /// <summary>
    /// Data Interface Class object specific to the Energization Start Time
    /// </summary>
    public class COSEMEnergizationStartTimeInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMEnergizationStartTimeInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    EnergizationStartTimeSetting Settings = EnergizationStartTime;

                    if (Settings != null)
                    {
                        AttributeDefinition = Settings.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as an Energization Start Time Settings structure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public EnergizationStartTimeSetting EnergizationStartTime
        {
            get
            {
                EnergizationStartTimeSetting Settings = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    Settings = new EnergizationStartTimeSetting(DataValue);
                }

                return Settings;
            }
            set
            {
                if (value != null)
                {
                    COSEMData DataValue = value.ToCOSEMData();

                    Value = DataValue;
                }
                else
                {
                    throw new ArgumentNullException("The Settings may not be set to null");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface Class object specific to the Individual Energization Settings
    /// </summary>
    public class COSEMIndividualEnergizationSettingInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMIndividualEnergizationSettingInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    IndividualEnergizationSetting Settings = IndividualSetting;

                    if (Settings != null)
                    {
                        AttributeDefinition = Settings.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as an Energization Start Time Settings structure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public IndividualEnergizationSetting IndividualSetting
        {
            get
            {
                IndividualEnergizationSetting Settings = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    Settings = new IndividualEnergizationSetting(DataValue);
                }

                return Settings;
            }
            set
            {
                if (value != null)
                {
                    COSEMData DataValue = value.ToCOSEMData();

                    Value = DataValue;
                }
                else
                {
                    throw new ArgumentNullException("The Settings may not be set to null");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface Class object specific to the Multi Stage Energization Settings
    /// </summary>
    public class COSEMMultiStageEnergizationSettingsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMMultiStageEnergizationSettingsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    MultiStageEnergizationSetting Settings = MultiStageSettings;

                    if (Settings != null)
                    {
                        AttributeDefinition = Settings.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as an Energization Start Time Settings structure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public MultiStageEnergizationSetting MultiStageSettings
        {
            get
            {
                MultiStageEnergizationSetting Settings = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    Settings = new MultiStageEnergizationSetting(DataValue);
                }

                return Settings;
            }
            set
            {
                if (value != null)
                {
                    COSEMData DataValue = value.ToCOSEMData();

                    Value = DataValue;
                }
                else
                {
                    throw new ArgumentNullException("The Settings may not be set to null");
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface Class object specific to the Current Date
    /// </summary>
    public class COSEMCurrentDateInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMCurrentDateInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    COSEMDate Date = CurrentDate;

                    if (Date != null)
                    {
                        AttributeDefinition = new ObjectDefinition("Current Date", COSEMDataTypes.Date);
                        AttributeDefinition.Value = Date;
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a COSEM Date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMDate CurrentDate
        {
            get
            {
                COSEMDate Date = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Date)
                {
                    Date = DataValue.Value as COSEMDate;
                }
                else if (DataValue != null && DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    Date = new COSEMDate(DataValue.Value as byte[]);
                }

                return Date;
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface Class object specific to the Current Time
    /// </summary>
    public class COSEMCurrentTimeInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMCurrentTimeInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    COSEMTime Time = CurrentTime;

                    if (Time != null)
                    {
                        AttributeDefinition = new ObjectDefinition("Current Time", COSEMDataTypes.Time);
                        AttributeDefinition.Value = Time;
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a COSEM Date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created
        
        public COSEMTime CurrentTime
        {
            get
            {
                COSEMTime Time = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Time)
                {
                    Time = DataValue.Value as COSEMTime;
                }
                else if (DataValue != null && DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    Time = new COSEMTime(DataValue.Value as byte[]);
                }

                return Time;
            }
        }

        #endregion
    }

    /// <summary>
    /// Configuration XML Structure
    /// </summary>
    public class ConfigurationXML
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public ConfigurationXML()
        {
            m_ContentType = COSEMConfigurationContentTypes.NoConfiguration;
            m_Content = new byte[0];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public ConfigurationXML(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Enum)
                        {
                            m_ContentType = (COSEMConfigurationContentTypes)(byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Content Type is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_Content = StructureData[1].Value as byte[];
                        }
                        else
                        {
                            throw new ArgumentException("The Content is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Configuration File");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new EnumObjectDefinition("Content Type", typeof(COSEMConfigurationContentTypes));
            NewObjectDefinition.Value = COSEMConfigurationContentTypes.NoConfiguration;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Content", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ContentType;
            Definition.StructureDefinition[1].Value = m_Content;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Converts the object to a COSEMData object
        /// </summary>
        /// <returns>The object as a COSEMData object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public COSEMData ToCOSEMData()
        {
            COSEMData ConfigFileStructure = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.Enum;
            StructureData[0].Value = (byte)m_ContentType;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.OctetString;

            if (m_Content != null)
            {
                StructureData[1].Value = m_Content;
            }
            else
            {
                StructureData[1].Value = new byte[0];
            }

            ConfigFileStructure.DataType = COSEMDataTypes.Structure;
            ConfigFileStructure.Value = StructureData;

            return ConfigFileStructure;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Content Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public COSEMConfigurationContentTypes ContentType
        {
            get
            {
                return m_ContentType;
            }
            set
            {
                m_ContentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Content
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public byte[] Content
        {
            get
            {
                return m_Content;
            }
            set
            {
                m_Content = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMConfigurationContentTypes m_ContentType;
        private byte[] m_Content;

        #endregion
    }

    /// <summary>
    /// Configuration XML specific Data Interface Class
    /// </summary>
    public class COSEMConfigurationXMLInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMConfigurationXMLInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    ConfigurationXML Config = ConfigurationFile;

                    if (Config != null)
                    {
                        AttributeDefinition = Config.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a Configuration XML object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public ConfigurationXML ConfigurationFile
        {
            get
            {
                ConfigurationXML Config = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    Config = new ConfigurationXML(DataValue);
                }

                return Config;
            }
            set
            {
                if(value != null)
                {
                    Value = value.ToCOSEMData();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Reconfigure Result Enumeration
    /// </summary>
    public enum ReconfigureResult : byte
    {
        /// <summary>Success</summary>
        [EnumDescription("Success")]
        Success = 0,
        /// <summary>Success</summary>
        [EnumDescription("In Progress")]
        InProgress = 1,

        // Everything else could be an error code but since you can still cast the value to this enum if the value is not
        // defined then there doesn't seem to be a good reason to list out all 253 error values. Doing a .ToDescription()
        // or .ToString() will actually print out the number of the error code which is all we care about.
    }

    /// <summary>
    /// Reconfigure Result data structure
    /// </summary>
    public class ReconfigureResultStructure
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public ReconfigureResultStructure()
        {
            m_Result = ReconfigureResult.Success;
            m_ExtraInformation = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public ReconfigureResultStructure(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Enum)
                        {
                            m_Result = (ReconfigureResult)(byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Reconfigure Result is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_ExtraInformation = StructureData[1].Value as string;
                        }
                        else
                        {
                            throw new ArgumentException("The Extra Information is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Reconfigure Result");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new EnumObjectDefinition("Result", typeof(ReconfigureResult));
            NewObjectDefinition.Value = ReconfigureResult.Success;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Extra Information", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = "";
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_Result;
            Definition.StructureDefinition[1].Value = m_ExtraInformation;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result
        /// </summary>
        public ReconfigureResult Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                m_Result = value;
            }
        }

        /// <summary>
        /// Gets or sets the Extra Information
        /// </summary>
        public string ExtraInformation
        {
            get
            {
                return m_ExtraInformation;
            }
            set
            {
                m_ExtraInformation = value;
            }
        }

        #endregion

        #region Member Variables

        private ReconfigureResult m_Result;
        private string m_ExtraInformation;

        #endregion
    }

    /// <summary>
    /// Data Interface Class specific to the Reconfiguration Result
    /// </summary>
    public class COSEMReconfigurationResultInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMReconfigurationResultInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    ReconfigureResultStructure ReconfigResult = Result;

                    if (ReconfigResult != null)
                    {
                        AttributeDefinition = ReconfigResult.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a Configuration XML object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public ReconfigureResultStructure Result
        {
            get
            {
                ReconfigureResultStructure ReconfigResult = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    ReconfigResult = new ReconfigureResultStructure(DataValue);
                }

                return ReconfigResult;
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface Class specific to the Boot Count Unlock
    /// </summary>
    public class COSEMBootCountUnlockInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMBootCountUnlockInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    // You can't actually get this item so return an empty string
                    AttributeDefinition = new ObjectDefinition("Value", COSEMDataTypes.VisibleString);
                    AttributeDefinition.Value = "";
                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        /// <summary>
        /// Gets the Definition of the data returned by the specified attribute
        /// </summary>
        /// <param name="attributeID">The attribute to get the definition for</param>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/19/13 RCG 3.00.01 N/A    Created

        public override ObjectDefinition GetDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = new ObjectDefinition("Value", COSEMDataTypes.VisibleString);
                    AttributeDefinition.Value = "";
                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sets the Boot Count Unlock
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/19/13 RCG 3.00.01 N/A    Created
        
        public string BootCountUnlock
        {
            set
            {
                COSEMData DataValue = new COSEMData();
                DataValue.DataType = COSEMDataTypes.VisibleString;

                DataValue.Value = value;

                Value = DataValue;
            }
        }

        #endregion
    }

    /// <summary>
    /// Registration Ranges
    /// </summary>
    public class RegistrationRange
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created
        
        public RegistrationRange()
        {
            m_Minimum = 0;
            m_Maximum = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created

        public RegistrationRange(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_Minimum = (ushort)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Minimum is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_Maximum = (ushort)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Maximum is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Registration Ranges");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Minimum", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Maximum", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_Minimum;
            Definition.StructureDefinition[1].Value = m_Maximum;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Minimum period of time in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created
        
        public ushort Minimum
        {
            get
            {
                return m_Minimum;
            }
            set
            {
                m_Minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the Maximum period of time in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 RCG 3.00.06 N/A    Created
        
        public ushort Maximum
        {
            get
            {
                return m_Maximum;
            }
            set
            {
                m_Maximum = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Minimum;
        private ushort m_Maximum;

        #endregion
    }

    /// <summary>
    /// Data Interface class object for the Range for Registration Attempts
    /// </summary>
    public class COSEMRangeForRegistrationAttemptsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMRangeForRegistrationAttemptsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    RegistrationRange RegistrationRanges = Range;

                    if (RegistrationRanges != null)
                    {
                        AttributeDefinition = RegistrationRanges.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a Configuration XML object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created
        
        public RegistrationRange Range
        {
            get
            {
                RegistrationRange RegistrationRanges = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    RegistrationRanges = new RegistrationRange(DataValue);
                }

                return RegistrationRanges;
            }
        }

        #endregion
    }

    /// <summary>
    /// Data Interface class object for the Push Destination Override
    /// </summary>
    public class COSEMPushDestinationOverrideInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public COSEMPushDestinationOverrideInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.07 419083 Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    COSEMSendDestinationAndMethod DestinationAndMethod = SendDestinationAndMethod;

                    if (DestinationAndMethod != null)
                    {
                        AttributeDefinition = DestinationAndMethod.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Value as a Configuration XML object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/01/13 RCG 2.85.08 419079 Created

        public COSEMSendDestinationAndMethod SendDestinationAndMethod
        {
            get
            {
                COSEMSendDestinationAndMethod DestinationAndMethod = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    DestinationAndMethod = new COSEMSendDestinationAndMethod(DataValue);
                }

                return DestinationAndMethod;
            }
        }

        #endregion
    }

    /// <summary>
    /// NGC Debug Metrics
    /// </summary>
    public class NGCDebugMetrics
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDebugMetrics()
        {
            m_InSpeed = 0;
            m_OutSpeed = 0;
            m_AdminStatus = 0;
            m_OperationStatus = 0;
            m_LastChanged = 0;
            m_InOctets = 0;
            m_OutOctets = 0;
            m_InDiscards = 0;
            m_InErrors = 0;
            m_OutDiscards = 0;
            m_OutErrors = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDebugMetrics(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 11)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InSpeed = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Speed is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutSpeed = (uint)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Speed is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_AdminStatus = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Admin Status is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OperationStatus = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Operation Status is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_LastChanged = (uint)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Last Changed value is not the expected data type.");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InOctets = (uint)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Octets is not the expected data type.");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutOctets = (uint)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Octets is not the expected data type.");
                        }

                        if (StructureData[7].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InDiscards = (uint)StructureData[7].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Discards is not the expected data type.");
                        }

                        if (StructureData[8].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InErrors = (uint)StructureData[8].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Errors is not the expected data type.");
                        }

                        if (StructureData[9].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutDiscards = (uint)StructureData[9].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Discards is not the expected data type.");
                        }

                        if (StructureData[10].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutErrors = (uint)StructureData[10].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Errors is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 11.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("NGC Debug Metrics");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("In Speed", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Speed", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Admin Status", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Operation Status", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Last Changed", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Octets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Octets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Discards", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Errors", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Discards", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Errors", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_InSpeed;
            Definition.StructureDefinition[1].Value = m_OutSpeed;
            Definition.StructureDefinition[2].Value = m_AdminStatus;
            Definition.StructureDefinition[3].Value = m_OperationStatus;
            Definition.StructureDefinition[4].Value = m_LastChanged;
            Definition.StructureDefinition[5].Value = m_InOctets;
            Definition.StructureDefinition[6].Value = m_OutOctets;
            Definition.StructureDefinition[7].Value = m_InDiscards;
            Definition.StructureDefinition[8].Value = m_InErrors;
            Definition.StructureDefinition[9].Value = m_OutDiscards;
            Definition.StructureDefinition[10].Value = m_OutErrors;
            
            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the In Speed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint InSpeed
        {
            get
            {
                return m_InSpeed;
            }
            set
            {
                m_InSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Speed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint OutSpeed
        {
            get
            {
                return m_OutSpeed;
            }
            set
            {
                m_OutSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the Admin Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint AdminStatus
        {
            get
            {
                return m_AdminStatus;
            }
            set
            {
                m_AdminStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the Operation Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint OperationStatus
        {
            get
            {
                return m_OperationStatus;
            }
            set
            {
                m_OperationStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the Last Changed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint LastChanged
        {
            get
            {
                return m_LastChanged;
            }
            set
            {
                m_LastChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets the In Octets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint InOctets
        {
            get
            {
                return m_InOctets;
            }
            set
            {
                m_InOctets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Octets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint OutOctets
        {
            get
            {
                return m_OutOctets;
            }
            set
            {
                m_OutOctets = value;
            }
        }

        /// <summary>
        /// Gets or sets the In Discards
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint InDiscards
        {
            get
            {
                return m_InDiscards;
            }
            set
            {
                m_InDiscards = value;
            }
        }

        /// <summary>
        /// Gets or sets the In Errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint InErrors
        {
            get
            {
                return m_InErrors;
            }
            set
            {
                m_InErrors = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Discards
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint OutDiscards
        {
            get
            {
                return m_OutDiscards;
            }
            set
            {
                m_OutDiscards = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint OutErrors
        {
            get
            {
                return m_OutErrors;
            }
            set
            {
                m_OutErrors = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_InSpeed;
        private uint m_OutSpeed;
        private uint m_AdminStatus;
        private uint m_OperationStatus;
        private uint m_LastChanged;
        private uint m_InOctets;
        private uint m_OutOctets;
        private uint m_InDiscards;
        private uint m_InErrors;
        private uint m_OutDiscards;
        private uint m_OutErrors;

        #endregion
    }

    /// <summary>
    /// Interface Class for the NGC Debug Metrics
    /// </summary>
    public class NGCDebugMetricsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDebugMetricsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    NGCDebugMetrics DebugMetrics = Metrics;

                    if (DebugMetrics != null)
                    {
                        AttributeDefinition = DebugMetrics.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug metrics
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDebugMetrics Metrics
        {
            get
            {
                NGCDebugMetrics DebugMetrics = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    DebugMetrics = new NGCDebugMetrics(DataValue);
                }

                return DebugMetrics;
            }
        }

        #endregion
    }

    /// <summary>
    /// NGC Detailed Metrics
    /// </summary>
    public class NGCDetailedMetrics
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDetailedMetrics()
        {
            m_InUnicastPackets = 0;
            m_InBroadcastPackets = 0;
            m_InMulticastPackets = 0;
            m_InUnknownProtocolPackets = 0;
            m_OutUnicastPackets = 0;
            m_OutBroadcastPackets = 0;
            m_OutMulticastPackets = 0;
            m_OutQueueLength = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDetailedMetrics(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 8)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InUnicastPackets = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Unicast Packets is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InBroadcastPackets = (uint)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Broadcast Packets is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InMulticastPackets = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Multicast Packets is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_InUnknownProtocolPackets = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Unknown Protocol Packets is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutUnicastPackets = (uint)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Unicast Packets is not the expected data type.");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutBroadcastPackets = (uint)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Broadcast Packets is not the expected data type.");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutMulticastPackets = (uint)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Multicast Packets is not the expected data type.");
                        }

                        if (StructureData[7].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_OutQueueLength = (uint)StructureData[7].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Queue Length is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 8.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("NGC Detailed Metrics");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("In Unicast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Broadcast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Multicast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("In Unknown Protocol Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Unicast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Broadcast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Multicast Packets", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Out Queue Length", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_InUnicastPackets;
            Definition.StructureDefinition[1].Value = m_InBroadcastPackets;
            Definition.StructureDefinition[2].Value = m_InMulticastPackets;
            Definition.StructureDefinition[3].Value = m_InUnknownProtocolPackets;
            Definition.StructureDefinition[4].Value = m_OutUnicastPackets;
            Definition.StructureDefinition[5].Value = m_OutBroadcastPackets;
            Definition.StructureDefinition[6].Value = m_OutMulticastPackets;
            Definition.StructureDefinition[7].Value = m_OutQueueLength;
            
            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the In Unicast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint InUnicastPackets
        {
            get
            {
                return m_InUnicastPackets;
            }
            set
            {
                m_InUnicastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the In Broadcast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint InBroadcastPackets
        {
            get
            {
                return m_InBroadcastPackets;
            }
            set
            {
                m_InBroadcastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the In Multicast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint InMulticastPackets
        {
            get
            {
                return m_InMulticastPackets;
            }
            set
            {
                m_InMulticastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Unknown Protocol Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint InUnknownProtocolPackets
        {
            get
            {
                return m_InUnknownProtocolPackets;
            }
            set
            {
                m_InUnknownProtocolPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Unicast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint OutUnicastPackets
        {
            get
            {
                return m_OutUnicastPackets;
            }
            set
            {
                m_OutUnicastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Broadcast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint OutBroadcastPackets
        {
            get
            {
                return m_OutBroadcastPackets;
            }
            set
            {
                m_OutBroadcastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Multicast Packets
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint OutMulticastPackets
        {
            get
            {
                return m_OutMulticastPackets;
            }
            set
            {
                m_OutMulticastPackets = value;
            }
        }

        /// <summary>
        /// Gets or sets the Out Queue Length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint OutQueueLength
        {
            get
            {
                return m_OutQueueLength;
            }
            set
            {
                m_OutQueueLength = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_InUnicastPackets;
        private uint m_InBroadcastPackets;
        private uint m_InMulticastPackets;
        private uint m_InUnknownProtocolPackets;
        private uint m_OutUnicastPackets;
        private uint m_OutBroadcastPackets;
        private uint m_OutMulticastPackets;
        private uint m_OutQueueLength;

        #endregion
    }

    /// <summary>
    /// NGC Detailed Metrics Interface Class
    /// </summary>
    public class NGCDetailedMetricsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDetailedMetricsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    NGCDetailedMetrics DetailedMetrics = Metrics;

                    if (DetailedMetrics != null)
                    {
                        AttributeDefinition = DetailedMetrics.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug metrics
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDetailedMetrics Metrics
        {
            get
            {
                NGCDetailedMetrics DetailedMetrics = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    DetailedMetrics = new NGCDetailedMetrics(DataValue);
                }

                return DetailedMetrics;
            }
        }

        #endregion
    }

    /// <summary>
    /// NGC Description Structure
    /// </summary>
    public class NGCDescription
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDescription()
        {
            m_Name = null;
            m_Description = null;
            m_Type = 0;
            m_MTU = 0;
            m_PhysicalAddress = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDescription(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 5)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_Name = (string)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Name is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.VisibleString)
                        {
                            m_Description = (string)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Description is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_Type = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Type is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_MTU = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The MTU is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.OctetString)
                        {
                            m_PhysicalAddress = (byte[])StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Physical Address is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 5.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("NGC Debug Metrics");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Name", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Description", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Type", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("MTU", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Physical Address", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_Name;
            Definition.StructureDefinition[1].Value = m_Description;
            Definition.StructureDefinition[2].Value = m_Type;
            Definition.StructureDefinition[3].Value = m_MTU;
            Definition.StructureDefinition[4].Value = m_PhysicalAddress;
            
            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }

        /// <summary>
        /// Gets or sets the Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the MTU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint MTU
        {
            get
            {
                return m_MTU;
            }
            set
            {
                m_MTU = value;
            }
        }

        /// <summary>
        /// Gets or sets the Physical Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public byte[] PhysicalAddress
        {
            get
            {
                return m_PhysicalAddress;
            }
            set
            {
                m_PhysicalAddress = value;
            }
        }

        #endregion      

        #region Member Variables

        private string m_Name;
        private string m_Description;
        private uint m_Type;
        private uint m_MTU;
        private byte[] m_PhysicalAddress;

        #endregion
    }

    /// <summary>
    /// NGC Description Interface Class
    /// </summary>
    public class NGCDescriptionInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCDescriptionInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    NGCDescription DescriptionValue = Description;

                    if (DescriptionValue != null)
                    {
                        AttributeDefinition = DescriptionValue.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug metrics
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCDescription Description
        {
            get
            {
                NGCDescription DescriptionValue = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    DescriptionValue = new NGCDescription(DataValue);
                }

                return DescriptionValue;
            }
        }

        #endregion
    }

    /// <summary>
    /// NGC UDPv6 Metrics
    /// </summary>
    public class NGCUDPMetrics
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCUDPMetrics()
        {
            m_UDPv6InDatagrams = 0;
            m_UDPv6NoPorts = 0;
            m_UDPv6InErrors = 0;
            m_UDPv6OutDatagrams = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCUDPMetrics(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 4)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UDPv6InDatagrams = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Datagrams value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UDPv6NoPorts = (uint)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The No Ports value is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UDPv6InErrors = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The In Errors value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UDPv6OutDatagrams = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Out Datagrams value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 4.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("UDPv6 Metrics");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("UDPv6 In Datagrams", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("UDPv6 No Ports", COSEMDataTypes.VisibleString);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("UDPv6 In Errors", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("UDPv6 Out Datagrams", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_UDPv6InDatagrams;
            Definition.StructureDefinition[1].Value = m_UDPv6NoPorts;
            Definition.StructureDefinition[2].Value = m_UDPv6InErrors;
            Definition.StructureDefinition[3].Value = m_UDPv6OutDatagrams;
            
            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the number of In Datagrams
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint UDPv6InDatagrams
        {
            get
            {
                return m_UDPv6InDatagrams;
            }
            set
            {
                m_UDPv6InDatagrams = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of No Port Errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint UDPv6NoPorts
        {
            get
            {
                return m_UDPv6NoPorts;
            }
            set
            {
                m_UDPv6NoPorts = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of In Errors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint UDPv6InErrors
        {
            get
            {
                return m_UDPv6InErrors;
            }
            set
            {
                m_UDPv6InErrors = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of out datagrams
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint UDPv6OutDatagrams
        {
            get
            {
                return m_UDPv6OutDatagrams;
            }
            set
            {
                m_UDPv6OutDatagrams = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_UDPv6InDatagrams;
        private uint m_UDPv6NoPorts;
        private uint m_UDPv6InErrors;
        private uint m_UDPv6OutDatagrams;

        #endregion
    }

    /// <summary>
    /// NGC UDPv6 Metrics Interface Class
    /// </summary>
    public class NGCUDPMetricsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCUDPMetricsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    NGCUDPMetrics UDPMetrics = Metrics;

                    if (UDPMetrics != null)
                    {
                        AttributeDefinition = UDPMetrics.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the debug metrics
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NGCUDPMetrics Metrics
        {
            get
            {
                NGCUDPMetrics UDPMetrics = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    UDPMetrics = new NGCUDPMetrics(DataValue);
                }

                return UDPMetrics;
            }
        }

        #endregion
    }

    /// <summary>
    /// RPL Instance Structure
    /// </summary>
    public class RPLInstance
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLInstance()
        {
            m_InstanceID = 0;
            m_DoDagID = null;
            m_DoDagVersion = 0;
            m_Rank = 0;
            m_NumberOfLocalRepairs = 0;
            m_NumberOfGlobalRepairs = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public RPLInstance(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 7)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_InstanceID = (byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Instance ID value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_DoDagID = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DoDag ID value is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DoDagVersion = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DoDag Version value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DateTime)
                        {
                            m_DoDagLastChanged = (COSEMDateTime)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DoDag Last Changed value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_Rank = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Rank value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_NumberOfLocalRepairs = (ushort)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Local Repairs value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_NumberOfGlobalRepairs = (ushort)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Global Repairs value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 7.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("RPL Instance");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Instance ID", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DoDag ID", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DoDag Version", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DoDag Last Changed", COSEMDataTypes.DateTime);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Rank", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("Number of Local Repairs", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Number of Global Repairs", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Merit", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Registered Status", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_InstanceID;
            Definition.StructureDefinition[1].Value = m_DoDagID;
            Definition.StructureDefinition[2].Value = m_DoDagVersion;
            Definition.StructureDefinition[3].Value = m_DoDagLastChanged;
            Definition.StructureDefinition[4].Value = m_Rank;
            Definition.StructureDefinition[5].Value = m_NumberOfLocalRepairs;
            Definition.StructureDefinition[6].Value = m_NumberOfGlobalRepairs;
            
            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Instance ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte InstanceID
        {
            get
            {
                return m_InstanceID;
            }
            set
            {
                m_InstanceID = value;
            }
        }

        /// <summary>
        /// Gets or sets the DoDag ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte[] DoDagID
        {
            get
            {
                return m_DoDagID;
            }
            set
            {
                m_DoDagID = value;
            }
        }

        /// <summary>
        /// Gets or sets the DoDag Version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint DoDagVersion
        {
            get
            {
                return m_DoDagVersion;
            }
            set
            {
                m_DoDagVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the date and time the DoDag was last changed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public COSEMDateTime DoDagLastChanged
        {
            get
            {
                return m_DoDagLastChanged;
            }
            set
            {
                m_DoDagLastChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets the Rank
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint Rank
        {
            get
            {
                return m_Rank;
            }
            set
            {
                m_Rank = value;
            }
        }

        /// <summary>
        /// Gets or sets the Number of Local Repairs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort NumberOfLocalRepairs
        {
            get
            {
                return m_NumberOfLocalRepairs;
            }
            set
            {
                m_NumberOfLocalRepairs = value;
            }
        }

        /// <summary>
        /// Gets or sets the Number of Global Repairs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort NumberOfGlobalRepairs
        {
            get
            {
                return m_NumberOfGlobalRepairs;
            }
            set
            {
                m_NumberOfGlobalRepairs = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_InstanceID;
        private byte[] m_DoDagID;
        private uint m_DoDagVersion;
        private COSEMDateTime m_DoDagLastChanged;
        private uint m_Rank;
        private ushort m_NumberOfLocalRepairs;
        private ushort m_NumberOfGlobalRepairs;

        #endregion
    }

    /// <summary>
    /// RPL Instance Interface Class
    /// </summary>
    public class NGCRPLInstanceInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCRPLInstanceInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    RPLInstance CurrentInstance = Instance;

                    if (CurrentInstance != null)
                    {
                        AttributeDefinition = CurrentInstance.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLInstance Instance
        {
            get
            {
                RPLInstance CurrentInstance = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentInstance = new RPLInstance(DataValue);
                }

                return CurrentInstance;
            }
        }

        #endregion
    }

    /// <summary>
    /// RPL Config Structure
    /// </summary>
    public class RPLConfig
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLConfig()
        {
            m_MigrationTransitInterval = 0;
            m_MaxNumberOfParents = 0;
            m_MaxNumberOfBestParents = 0;
            m_DIOIntervalMin = 0;
            m_DIOIntervalMax = 0;
            m_DIORedundancy = 0;
            m_DIOIntervalDoublings = 0;
            m_DAONormalTimeout = 0;
            m_DAOLongTimeout = 0;
            m_DAOOptimizedTimeout = 0;
            m_DAOMaxRetry = 0;
            m_DAOAckTimeout = 0;
            m_MinRankIncrease = 0;
            m_MaxRankIncrease = 0;
            m_DTSN = 0;
            m_DoDagOCP = 0;
            m_GlobalETTHysteresis = 0;
            m_GlobalETTWeight = 0;
            m_LocalETTWeight = 0;
            m_LocalLQLWeight = 0;
            m_DSIWeight = 0;
            m_LQLFactor = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public RPLConfig(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 22)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_MigrationTransitInterval = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Migration Transit Interval value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_MaxNumberOfParents = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Number of Parents value is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_MaxNumberOfBestParents = (byte)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Number of Best Parents value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DIOIntervalMin = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Interval Min value is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DIOIntervalMax = (byte)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Interval Max value is not the expected data type.");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DIORedundancy = (byte)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Redundancy value is not the expected data type.");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DIOIntervalDoublings = (byte)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Interval Doublings value is not the expected data type.");
                        }

                        if (StructureData[7].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DAONormalTimeout = (ushort)StructureData[7].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Normal Timeout value is not the expected data type.");
                        }

                        if (StructureData[8].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DAOLongTimeout = (ushort)StructureData[8].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Long Timeout value is not the expected data type.");
                        }

                        if (StructureData[9].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DAOOptimizedTimeout = (ushort)StructureData[9].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Optimized Timeout value is not the expected data type.");
                        }

                        if (StructureData[10].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DAOMaxRetry = (ushort)StructureData[10].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Max Retry value is not the expected data type.");
                        }

                        if (StructureData[11].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DAOAckTimeout = (ushort)StructureData[11].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Ack Timeout value is not the expected data type.");
                        }

                        if (StructureData[12].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_MinRankIncrease = (ushort)StructureData[12].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Min Rank Increase value is not the expected data type.");
                        }

                        if (StructureData[13].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_MaxRankIncrease = (ushort)StructureData[13].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Rank Increase value is not the expected data type.");
                        }

                        if (StructureData[14].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DTSN = (ushort)StructureData[14].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DTSN value is not the expected data type.");
                        }

                        if (StructureData[15].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DoDagOCP = (byte)StructureData[15].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DoDag OCP value is not the expected data type.");
                        }

                        if (StructureData[16].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_GlobalETTHysteresis = (ushort)StructureData[16].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Global ETT Hysteresis value is not the expected data type.");
                        }

                        if (StructureData[17].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_GlobalETTWeight = (ushort)StructureData[17].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Global ETT Weight value is not the expected data type.");
                        }

                        if (StructureData[18].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_LocalETTWeight = (ushort)StructureData[18].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Local ETT Weight value is not the expected data type.");
                        }

                        if (StructureData[19].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_LocalLQLWeight = (ushort)StructureData[19].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Local LQL Weight value is not the expected data type.");
                        }

                        if (StructureData[20].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DSIWeight = (ushort)StructureData[20].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Instance ID value is not the expected data type.");
                        }

                        if (StructureData[21].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_LQLFactor = (ushort)StructureData[21].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The LQL Factor value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 22.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("RPL Configuration");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Migration Transit Interval", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Max Number of DAO Best Parents", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Max Parent List Size", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIO Interval Min", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIO Interval Max", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIO Redundancy Repetition", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Normal Timeout", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Long Timeout", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("DAO Retries Timeout", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("DAO Immediate Update Timeout", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("DAO Optimized Update Timeout", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("Hop Rank Increase", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);
            
            NewObjectDefinition = new ObjectDefinition("Max Rank Increase", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("ETT NB Parents Hysteresis", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Dodag Weight LETT", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Dodag Weight LQL", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Dodag Weight DSI", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Dodag Weight Rank", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_MigrationTransitInterval;
            Definition.StructureDefinition[1].Value = m_MaxNumberOfParents;
            Definition.StructureDefinition[2].Value = m_MaxNumberOfBestParents;
            Definition.StructureDefinition[3].Value = m_DIOIntervalMin;
            Definition.StructureDefinition[4].Value = m_DIOIntervalMax;
            Definition.StructureDefinition[5].Value = m_DIORedundancy;
            Definition.StructureDefinition[6].Value = m_DIOIntervalDoublings;
            Definition.StructureDefinition[7].Value = m_DAONormalTimeout;
            Definition.StructureDefinition[8].Value = m_DAOLongTimeout;
            Definition.StructureDefinition[9].Value = m_DAOOptimizedTimeout;
            Definition.StructureDefinition[10].Value = m_DAOMaxRetry;
            Definition.StructureDefinition[11].Value = m_DAOAckTimeout;
            Definition.StructureDefinition[12].Value = m_MinRankIncrease;
            Definition.StructureDefinition[13].Value = m_MaxRankIncrease;
            Definition.StructureDefinition[14].Value = m_DTSN;
            Definition.StructureDefinition[15].Value = m_DoDagOCP;
            Definition.StructureDefinition[16].Value = m_GlobalETTHysteresis;
            Definition.StructureDefinition[17].Value = m_GlobalETTWeight;
            Definition.StructureDefinition[18].Value = m_LocalETTWeight;
            Definition.StructureDefinition[19].Value = m_LocalLQLWeight;
            Definition.StructureDefinition[20].Value = m_DSIWeight;
            Definition.StructureDefinition[21].Value = m_LQLFactor;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Migration Transit Interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint MigrationTransitInterval
        {
            get
            {
                return m_MigrationTransitInterval;
            }
            set
            {
                m_MigrationTransitInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets the Max Number of Parents
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte MaxNumberOfParents
        {
            get
            {
                return m_MaxNumberOfParents;
            }
            set
            {
                m_MaxNumberOfParents = value;
            }
        }

        /// <summary>
        /// Gets or sets the Max Number of Best Parents
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte MaxNumberOfBestParents
        {
            get
            {
                return m_MaxNumberOfBestParents;
            }
            set
            {
                m_MaxNumberOfBestParents = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIO Interval Min
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte DIOIntervalMin
        {
            get
            {
                return m_DIOIntervalMin;
            }
            set
            {
                m_DIOIntervalMin = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIO Interval Max
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte DIOIntervalMax
        {
            get
            {
                return m_DIOIntervalMax;
            }
            set
            {
                m_DIOIntervalMax = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIO Redundancy
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte DIORedundancy
        {
            get
            {
                return m_DIORedundancy;
            }
            set
            {
                m_DIORedundancy = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIO Interval Doublings
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte DIOIntervalDoublings
        {
            get
            {
                return m_DIOIntervalDoublings;
            }
            set
            {
                m_DIOIntervalDoublings = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Normal Timeout
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DAONormalTimeout
        {
            get
            {
                return m_DAONormalTimeout;
            }
            set
            {
                m_DAONormalTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Long Timeout
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DAOLongTimeout
        {
            get
            {
                return m_DAOLongTimeout;
            }
            set
            {
                m_DAOLongTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Optimized Timeout
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DAOOptimizedTimeout
        {
            get
            {
                return m_DAOOptimizedTimeout;
            }
            set
            {
                m_DAOOptimizedTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Max Retry
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DAOMaxRetry
        {
            get
            {
                return m_DAOMaxRetry;
            }
            set
            {
                m_DAOMaxRetry = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Ack Timeout
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DAOAckTimeout
        {
            get
            {
                return m_DAOAckTimeout;
            }
            set
            {
                m_DAOAckTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the Min Rank Increase
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort MinRankIncrease
        {
            get
            {
                return m_MinRankIncrease;
            }
            set
            {
                m_MinRankIncrease = value;
            }
        }

        /// <summary>
        /// Gets or sets the Max Rank Increase
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort MaxRankIncrease
        {
            get
            {
                return m_MaxRankIncrease;
            }
            set
            {
                m_MaxRankIncrease = value;
            }
        }

        /// <summary>
        /// Gets or sets the DTSN
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort DTSN
        {
            get
            {
                return m_DTSN;
            }
            set
            {
                m_DTSN = value;
            }
        }

        /// <summary>
        /// Gets or sets the DoDag OCP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte DoDagOCP
        {
            get
            {
                return m_DoDagOCP;
            }
            set
            {
                m_DoDagOCP = value;
            }
        }

        /// <summary>
        /// Gets or sets the Global ETT Hysteresis
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort GlobalETTHysteresis
        {
            get
            {
                return m_GlobalETTHysteresis;
            }
            set
            {
                m_GlobalETTHysteresis = value;
            }
        }

        /// <summary>
        /// Gets or sets the Global ETT Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort GlobalETTWeight
        {
            get
            {
                return m_GlobalETTWeight;
            }
            set
            {
                m_GlobalETTWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the Local ETT Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public ushort LocalETTWeight
        {
            get
            {
                return m_LocalETTWeight;
            }
            set
            {
                m_LocalETTWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the Local LQL Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort LocalLQLWeight
        {
            get
            {
                return m_LocalLQLWeight;
            }
            set
            {
                m_LocalLQLWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the DSI Weight
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort DSIWeight
        {
            get
            {
                return m_DSIWeight;
            }
            set
            {
                m_DSIWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the LQL Factor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort LQLFactor
        {
            get
            {
                return m_LQLFactor;
            }
            set
            {
                m_LQLFactor = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_MigrationTransitInterval;
        private byte m_MaxNumberOfParents;
        private byte m_MaxNumberOfBestParents;
        private byte m_DIOIntervalMin;
        private byte m_DIOIntervalMax;
        private byte m_DIORedundancy;
        private byte m_DIOIntervalDoublings;
        private ushort m_DAONormalTimeout;
        private ushort m_DAOLongTimeout;
        private ushort m_DAOOptimizedTimeout;
        private ushort m_DAOMaxRetry;
        private ushort m_DAOAckTimeout;
        private ushort m_MinRankIncrease;
        private ushort m_MaxRankIncrease;
        private ushort m_DTSN;
        private byte m_DoDagOCP;
        private ushort m_GlobalETTHysteresis;
        private ushort m_GlobalETTWeight;
        private ushort m_LocalETTWeight;
        private ushort m_LocalLQLWeight;
        private ushort m_DSIWeight;
        private ushort m_LQLFactor;

        #endregion
    }

    /// <summary>
    /// RPL Configuration Interface Class
    /// </summary>
    public class NGCRPLConfigInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCRPLConfigInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    RPLConfig CurrentConfig = Configuration;

                    if (CurrentConfig != null)
                    {
                        AttributeDefinition = CurrentConfig.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLConfig Configuration
        {
            get
            {
                RPLConfig CurrentConfig = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentConfig = new RPLConfig(DataValue);
                }

                return CurrentConfig;
            }
        }

        #endregion
    }

    /// <summary>
    /// RPL Warm Start structure
    /// </summary>
    public class RPLWarmStart
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLWarmStart()
        {
            m_NumberOfParents = 0;
            m_NumberOfBestParents = 0;
            m_BestParents = null;
            m_NumberOfPANs = 0;
            m_PANID = 0;
            m_DSI = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public RPLWarmStart(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 6)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_NumberOfParents = (byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Parents value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_NumberOfBestParents = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Best Parents value is not the expected data type.");
                        }

                        if(StructureData[2].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] ArrayData = (COSEMData[])StructureData[2].Value;
                            m_BestParents = new List<byte[]>();

                            foreach(COSEMData CurrentValue in ArrayData)
                            {
                                if(CurrentValue.DataType == COSEMDataTypes.OctetString)
                                {
                                    m_BestParents.Add((byte[])CurrentValue.Value);
                                }
                                else
                                {
                                    throw new ArgumentException("The Array value is not the expected data type.");
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Best Parents value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_NumberOfPANs = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of PANs value is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_PANID = (ushort)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The PAN ID value is not the expected data type.");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DSI = (byte)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DSI value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 6.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("RPL Warm Start");
            ObjectDefinition NewObjectDefinition;
            ArrayObjectDefinition ArrayObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Number of Parents", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Number of Best Parents", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            ArrayObjectDefinition = new ArrayObjectDefinition("Best Parents", new ObjectDefinition("Entry", COSEMDataTypes.OctetString));
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(ArrayObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Number of PANs", COSEMDataTypes.Unsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("PAN ID", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DSI", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();
            ArrayObjectDefinition ArrayDefinition;

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_NumberOfParents;
            Definition.StructureDefinition[1].Value = m_NumberOfBestParents;

            Definition.StructureDefinition[2].Value = m_BestParents;

            ArrayDefinition = Definition.StructureDefinition[2] as ArrayObjectDefinition;

            if(ArrayDefinition != null)
            {
                ArrayDefinition.Elements.Clear();

                for(int iIndex = 0; iIndex < m_BestParents.Count; iIndex++)
                {
                    ObjectDefinition ElementDefinition = new ObjectDefinition("[" + iIndex.ToString(CultureInfo.InvariantCulture) + "]", COSEMDataTypes.OctetString);
                    ElementDefinition.Value = m_BestParents[iIndex];
                    ArrayDefinition.Elements.Add(ElementDefinition);
                }
            }

            Definition.StructureDefinition[3].Value = m_NumberOfPANs;
            Definition.StructureDefinition[4].Value = m_PANID;
            Definition.StructureDefinition[5].Value = m_DSI;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Max Number of Parents
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte NumberOfParents
        {
            get
            {
                return m_NumberOfParents;
            }
            set
            {
                m_NumberOfParents = value;
            }
        }

        /// <summary>
        /// Gets or sets the Max Number of Best Parents
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public byte NumberOfBestParents
        {
            get
            {
                return m_NumberOfBestParents;
            }
            set
            {
                m_NumberOfBestParents = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_NumberOfParents;
        private byte m_NumberOfBestParents;
        private List<byte[]> m_BestParents;
        private byte m_NumberOfPANs;
        private ushort m_PANID;
        private ushort m_DSI;

        #endregion
    }

    /// <summary>
    /// RPL Warm Start Interface Class
    /// </summary>
    public class NGCRPLWarmStartInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCRPLWarmStartInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    RPLWarmStart CurrentWarmStart = WarmStart;

                    if (CurrentWarmStart != null)
                    {
                        AttributeDefinition = CurrentWarmStart.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLWarmStart WarmStart
        {
            get
            {
                RPLWarmStart CurrentWarmStart = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentWarmStart = new RPLWarmStart(DataValue);
                }

                return CurrentWarmStart;
            }
        }

        #endregion
    }

    /// <summary>
    /// RPL Statistics
    /// </summary>
    public class RPLStats
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLStats()
        {
            m_DIOReceived = 0;
            m_DIOTransmitted = 0;
            m_DISReceieved = 0;
            m_DISTransmitted = 0;
            m_DAOReceived = 0;
            m_DAOTransmitted = 0;
            m_DAOAckReceived = 0;
            m_DAOAckTransmitted = 0;
            m_GlobalETT = 0;
            m_NumberOfMalformedMessages = 0;
            m_GlobalLQL = null;
            m_NumberOfNeighbors = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public RPLStats(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 12)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DIOReceived = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Received value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DIOTransmitted = (uint)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIO Transmitted value is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DISReceieved = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIS Received value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DISTransmitted = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DIS Transmitted value is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DAOReceived = (uint)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Received value is not the expected data type.");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DAOTransmitted = (uint)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Transmitted value is not the expected data type.");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DAOAckReceived = (uint)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Ack Received value is not the expected data type.");
                        }

                        if (StructureData[7].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DAOAckTransmitted = (uint)StructureData[7].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DAO Ack Transmitted value is not the expected data type.");
                        }

                        if (StructureData[8].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_GlobalETT = (ushort)StructureData[8].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Global ETT value is not the expected data type.");
                        }

                        if (StructureData[9].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_NumberOfMalformedMessages = (ushort)StructureData[9].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Malformed Messages value is not the expected data type.");
                        }

                        if (StructureData[10].DataType == COSEMDataTypes.OctetString)
                        {
                            m_GlobalLQL = (byte[])StructureData[10].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Global LQL Messages value is not the expected data type.");
                        }

                        if (StructureData[11].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_NumberOfNeighbors = (ushort)StructureData[11].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Number of Neighbors value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 12.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("RPL Statistics");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("DIO Received", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIO Transmitted", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIS Received", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DIS Transmitted", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Received", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Transmitted", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Ack Received", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("DAO Ack Transmitted", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Global ETT", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Number of Malformed Messages", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Global LQL", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = null;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Number of Neighbors", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_DIOReceived;
            Definition.StructureDefinition[1].Value = m_DIOTransmitted;
            Definition.StructureDefinition[2].Value = m_DISReceieved;
            Definition.StructureDefinition[3].Value = m_DISTransmitted;
            Definition.StructureDefinition[4].Value = m_DAOReceived;
            Definition.StructureDefinition[5].Value = m_DAOTransmitted;
            Definition.StructureDefinition[6].Value = m_DAOAckReceived;
            Definition.StructureDefinition[7].Value = m_DAOAckTransmitted;
            Definition.StructureDefinition[8].Value = m_GlobalETT;
            Definition.StructureDefinition[9].Value = m_NumberOfMalformedMessages;
            Definition.StructureDefinition[10].Value = m_GlobalLQL;
            Definition.StructureDefinition[11].Value = m_NumberOfNeighbors;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the DIO Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DIOReceived
        {
            get
            {
                return m_DIOReceived;
            }
            set
            {
                m_DIOReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIO Transmitted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DIOTransmitted
        {
            get
            {
                return m_DIOTransmitted;
            }
            set
            {
                m_DIOTransmitted = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIS Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DISReceieved
        {
            get
            {
                return m_DISReceieved;
            }
            set
            {
                m_DISReceieved = value;
            }
        }

        /// <summary>
        /// Gets or sets the DIS Transmitted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DISTransmitted
        {
            get
            {
                return m_DISTransmitted;
            }
            set
            {
                m_DISTransmitted = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DAOReceived
        {
            get
            {
                return m_DAOReceived;
            }
            set
            {
                m_DAOReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Transmitted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DAOTransmitted
        {
            get
            {
                return m_DAOTransmitted;
            }
            set
            {
                m_DAOTransmitted = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Ack Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DAOAckReceived
        {
            get
            {
                return m_DAOAckReceived;
            }
            set
            {
                m_DAOAckReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the DAO Ack Transmitted
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DAOAckTransmitted
        {
            get
            {
                return m_DAOAckTransmitted;
            }
            set
            {
                m_DAOAckTransmitted = value;
            }
        }

        /// <summary>
        /// Gets or sets the Global ETT
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort GlobalETT
        {
            get
            {
                return m_GlobalETT;
            }
            set
            {
                m_GlobalETT = value;
            }
        }

        /// <summary>
        /// Gets or sets the Number of Malformed Messages
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort NumberOfMalformedMessages
        {
            get
            {
                return m_NumberOfMalformedMessages;
            }
            set
            {
                m_NumberOfMalformedMessages = value;
            }
        }

        /// <summary>
        /// Gets or sets the Global LQL
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public byte[] GlobalLQL
        {
            get
            {
                return m_GlobalLQL;
            }
            set
            {
                m_GlobalLQL = value;
            }
        }

        /// <summary>
        /// Gets or sets the Number of Neighbors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ushort NumberOfNeighbors
        {
            get
            {
                return m_NumberOfNeighbors;
            }
            set
            {
                m_NumberOfNeighbors = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_DIOReceived;
        private uint m_DIOTransmitted;
        private uint m_DISReceieved;
        private uint m_DISTransmitted;
        private uint m_DAOReceived;
        private uint m_DAOTransmitted;
        private uint m_DAOAckReceived;
        private uint m_DAOAckTransmitted;
        private ushort m_GlobalETT;
        private ushort m_NumberOfMalformedMessages;
        private byte[] m_GlobalLQL;
        private ushort m_NumberOfNeighbors;

        #endregion
    }

    /// <summary>
    /// RPL Statistics Interface Class
    /// </summary>
    public class NGCRPLStatsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCRPLStatsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    RPLStats CurrentStats = Statistics;

                    if (CurrentStats != null)
                    {
                        AttributeDefinition = CurrentStats.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public RPLStats Statistics
        {
            get
            {
                RPLStats CurrentStats = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentStats = new RPLStats(DataValue);
                }

                return CurrentStats;
            }
        }

        #endregion
    }

    /// <summary>
    /// NAN Driver Statistics
    /// </summary>
    public class NANDriver
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public NANDriver()
        {
            m_UpwardReceived = 0;
            m_UpwardTransmitted = 0;
            m_DownwardReceived = 0;
            m_DownwardTransmitted = 0;
            m_IPv6Address = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NANDriver(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 5)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UpwardReceived = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Upward Received value is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_UpwardTransmitted = (uint)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Upward Transmitted value is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DownwardReceived = (uint)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Downward Received value is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_DownwardTransmitted = (uint)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Downward Transmitted value is not the expected data type.");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.OctetString)
                        {
                            m_IPv6Address = (byte[])StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The IPv6 Address value is not the expected data type.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The data must be a structure of length 5.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data is not a structure", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data may not be null");
            }
        }

        /// <summary>
        /// Gets the definition of the Registration Range Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Config Discover Neighborhood");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Scan Duration", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Time Tolerance", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Scan Request Interval", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("IPv6 Address Timeout", COSEMDataTypes.DoubleLongUnsigned);
            NewObjectDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_UpwardReceived;
            Definition.StructureDefinition[1].Value = m_UpwardTransmitted;
            Definition.StructureDefinition[2].Value = m_DownwardReceived;
            Definition.StructureDefinition[3].Value = m_DownwardTransmitted;
            Definition.StructureDefinition[4].Value = m_IPv6Address;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Upward Received count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created
        
        public uint UpwardReceived
        {
            get
            {
                return m_UpwardReceived;
            }
            set
            {
                m_UpwardReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the Upward Transmitted count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint UpwardTransmitted
        {
            get
            {
                return m_UpwardTransmitted;
            }
            set
            {
                m_UpwardTransmitted = value;
            }
        }

        /// <summary>
        /// Gets or sets the Downward Received count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DownwardReceived
        {
            get
            {
                return m_DownwardReceived;
            }
            set
            {
                m_DownwardReceived = value;
            }
        }

        /// <summary>
        /// Gets or sets the Downward Transmitted count
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public uint DownwardTransmitted
        {
            get
            {
                return m_DownwardTransmitted;
            }
            set
            {
                m_DownwardTransmitted = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the IPv6 Address
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public byte[] IPv6Address
        {
            get
            {
                return m_IPv6Address;
            }
            set
            {
                m_IPv6Address = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_UpwardReceived;
        private uint m_UpwardTransmitted;
        private uint m_DownwardReceived;
        private uint m_DownwardTransmitted;
        private byte[] m_IPv6Address;

        #endregion
    }

    /// <summary>
    /// NAN Driver Statistics Interface Class
    /// </summary>
    public class NGCNANDriverInterfaceClass : COSEMDataInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The Logical Name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NGCNANDriverInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    NANDriver CurrentStats = Statistics;

                    if (CurrentStats != null)
                    {
                        AttributeDefinition = CurrentStats.ToObjectDefinition();
                    }
                    else
                    {
                        COSEMData NullData = new COSEMData();
                        NullData.DataType = COSEMDataTypes.NullData;

                        AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Value", NullData);
                    }

                    break;
                }
                default:
                {
                    AttributeDefinition = base.GetAttributeWithDefinition(attributeID);
                    break;
                }
            }

            return AttributeDefinition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/03/13 RCG 3.50.09 N/A    Created

        public NANDriver Statistics
        {
            get
            {
                NANDriver CurrentStats = null;
                COSEMData DataValue = Value;

                if (DataValue != null && DataValue.DataType == COSEMDataTypes.Structure)
                {
                    CurrentStats = new NANDriver(DataValue);
                }

                return CurrentStats;
            }
        }

        #endregion
    }
}
