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
//                           Copyright © 2013 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Itron.Metering.Utilities;
using Itron.Metering.Communications.DLMS;
using OpenSSLWrapper;

namespace Itron.Metering.Device.DLMSDevice
{

    #region Global Definitions

    /// <summary>
    /// Scalar Unit Types
    /// </summary>
    public enum COSEMUnits
    {
        /// <summary>Years</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Years")]
        Years = 1,
        /// <summary>Months</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Months")]
        Months = 2,
        /// <summary>Weeks</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Weeks")]
        Weeks = 3,
        /// <summary>Days</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Days")]
        Days = 4,
        /// <summary>Hours</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Hours")]
        Hours = 5,
        /// <summary>Minutes</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Minutes")]
        Minutes = 6,
        /// <summary>Seconds</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Seconds")]
        Seconds = 7,
        /// <summary>Phase Angle in Degrees</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Degree")]
        PhaseAngle = 8,
        /// <summary>Temperature (C)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Celsius")]
        Temperature = 9,
        /// <summary>Currency</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Currency")]
        Currency = 10,
        /// <summary>Length</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Length")]
        Length = 11,
        /// <summary>Speed</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Speed")]
        Speed = 12,
        /// <summary>Volume (m^3)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeter")]
        VolumeCubicMeter = 13,
        /// <summary>Corrected Volume (m^3)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeter")]
        CorrectedVolume = 14,
        /// <summary>Volume Flux (per hour)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeterPerHour")]
        VolumeFluxPerHour = 15,
        /// <summary>Corrected Volume Flux (per hour)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeterPerHour")]
        CorrectedVolumeFluxPerHour = 16,
        /// <summary>Volume Flux (per Day)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeterPerDay")]
        VolumeFluxPerDay = 17,
        /// <summary>Corrected Volume Flux (per Day)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "CubicMeterPerDay")]
        CorrectedVolumeFluxPerDay = 18,
        /// <summary>Volume (l)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Litre")]
        VolumeLitre = 19,
        /// <summary>Mass (kg)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "KiloGram")]
        Mass = 20,
        /// <summary>Force (N)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Newton")]
        Force = 21,
        /// <summary>Energy (Nm)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NewtonMeter")]
        EnergyNewtonMeter = 22,
        /// <summary>Pressure (Pa)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Pascal")]
        PressurePascal = 23,
        /// <summary>Pressure (bar)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "bar")]
        PressureBar = 24,
        /// <summary>Energy (J)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Joule")]
        EnergyJoule = 25,
        /// <summary>Thermal Power (J/h)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "JoulePerHour")]
        ThermalPower = 26,
        /// <summary>Active Power (W)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Watt")]
        ActivePower = 27,
        /// <summary>Apparent Power (VA)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "VA")]
        ApparentPower = 28,
        /// <summary>Reactive Power (var)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "var")]
        ReactivePower = 29,
        /// <summary>Active Energy (Wh)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Wh")]
        ActiveEnergy = 30,
        /// <summary>Apparent Energy (VAh)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "VAh")]
        ApparentEnergy = 31,
        /// <summary>Reactive Energy (varh)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "varh")]
        ReactiveEnergy = 32,
        /// <summary>Current (A)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Amp")]
        Current = 33,
        /// <summary>Electrical Charge (C)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Coulomb")]
        ElectricalCharge = 34,
        /// <summary>Voltage</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Volt")]
        Voltage = 35,
        /// <summary>Electrical Field Strength</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "VoltPerMeter")]
        ElectricalFieldStrength = 36,
        /// <summary>Capacitance (F)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Farad")]
        Capacitance = 37,
        /// <summary>Resistance (Ω)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Ohm")]
        Resistance = 38,
        /// <summary>Resistivity</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Resistivity")]
        Resistivity = 39,
        /// <summary>Magnetic Flux (Wb)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Weber")]
        MagneticFlux = 40,
        /// <summary>Magnetic Flux Density (T)</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Tesla")]
        MagneticFluxDensity = 41,
        /// <summary>Not Specified</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NotSpecified")]
        NotSpecified = 255,
    }

    /// <summary>
    /// Sort methods used in COSEM
    /// </summary>
    public enum COSEMSortMethod
    {
        /// <summary>First in first out</summary>
        [EnumDescription("FIFO")]
        Fifo = 1,
        /// <summary>Last in first out</summary>
        [EnumDescription("LIFO")]
        Lifo = 2,
        /// <summary>Largest first</summary>
        [EnumDescription("Largest")]
        Largest = 3,
        /// <summary>Smallest first</summary>
        [EnumDescription("Smallest")]
        Smallest = 4,
        /// <summary>Values closest to zero first</summary>
        [EnumDescription("Nearest to Zero")]
        NearestToZero = 5,
        /// <summary>Values farthest from zero first</summary>
        [EnumDescription("Farthest from Zero")]
        FarthestFromZero = 6,
    }

    /// <summary>
    /// Modes of access for COSEM attributes
    /// </summary>
    public enum COSEMAttributeAccessMode
    {
        /// <summary>Attribute can not be accessed</summary>
        [EnumDescription("No Access")]
        NoAccess = 0,
        /// <summary>Attribute can only be read</summary>
        [EnumDescription("Read Only")]
        ReadOnly = 1,
        /// <summary>Attribute can only be written</summary>
        [EnumDescription("Write Only")]
        WriteOnly = 2,
        /// <summary>Attribute can be read and written</summary>
        [EnumDescription("Read and Write")]
        ReadAndWrite = 3,
        /// <summary>Attribute can only be read when authenticated</summary>
        [EnumDescription("Authenticated Read Only")]
        AuthenticatedReadOnly = 4,
        /// <summary>Attribute can only be written when authenticated</summary>
        [EnumDescription("Authenticated Write Only")]
        AuthenticatedWriteOnly = 5,
        /// <summary>Attribute can be read or written when authenticated</summary>
        [EnumDescription("Authenticated Read and Write")]
        AuthenticatedReadAndWrite = 6,
    }

    /// <summary>
    /// Modes of access for COSEM methods
    /// </summary>
    public enum COSEMMethodAccessMode
    {
        /// <summary>Method can not be accessed</summary>
        [EnumDescription("No Access")]
        NoAccess = 0,
        /// <summary>Method can be accessed</summary>
        [EnumDescription("Access")]
        Access = 1,
        /// <summary>Method can be accessed only when authenticated</summary>
        [EnumDescription("Authenticated Access")]
        AuthenticatedAccess = 2,
    }

    /// <summary>
    /// Association statuses
    /// </summary>
    public enum COSEMAssociationStatus
    {
        /// <summary>Not currently associated</summary>
        [EnumDescription("Not Associated")]
        NotAssociated = 0,
        /// <summary>Association is currently pending</summary>
        [EnumDescription("Association Pending")]
        AssociationPending = 1,
        /// <summary>Currently Associated</summary>
        [EnumDescription("Associated")]
        Associated = 2,
    }

    /// <summary>
    /// The status of an individual block
    /// </summary>
    public enum COSEMImageBlockStatus
    {
        /// <summary>Block has not been transferred</summary>
        [EnumDescription("Not Transferred")]
        NotTransferred = 0,
        /// <summary>Block has been transferred</summary>
        [EnumDescription("Transferred")]
        Transferred = 1,
    }

    /// <summary>
    /// Image Transfer Statuses
    /// </summary>
    public enum COSEMImageTransferStatus
    {
        /// <summary>Transfer not initiated</summary>
        [EnumDescription("Transfer Not Initiated")]
        TransferNotInitiated = 0,
        /// <summary>Transfer initiated</summary>
        [EnumDescription("Transfer Initiated")]
        TransferInitiated = 1,
        /// <summary>Image Verification initiated</summary>
        [EnumDescription("Verification Initiated")]
        VerificationInitiated = 2,
        /// <summary>Image Verification successful</summary>
        [EnumDescription("Verification Successful")]
        VerificationSuccessful = 3,
        /// <summary>Image Verification failed</summary>
        [EnumDescription("Verification Failed")]
        VerificationFailed = 4,
        /// <summary>Image Activation initiated</summary>
        [EnumDescription("Activation Initiated")]
        ActivationInitiated = 5,
        /// <summary>Image Activation successful</summary>
        [EnumDescription("Activation Successful")]
        ActivationSuccessful = 6,
        /// <summary>Image Activation failed</summary>
        [EnumDescription("Activation Failed")]
        ActivationFailed = 7,
    }

    /// <summary>
    /// The Key types
    /// </summary>
    public enum COSEMKeyIDs
    {
        /// <summary>Key used to encrypt unicast messages</summary>
        [EnumDescription("Unicast Encryption Key")]
        UnicastEncryptionKey = 0,
        /// <summary>Key used to encrypt broadcast messages</summary>
        [EnumDescription("Broadcast Encryption Key")]
        BroadcastEncryptionKey = 1,
        /// <summary>Key used for authentication</summary>
        [EnumDescription("Authentication Key")]
        AuthenticationKey = 2,
        /// <summary>Key used for key transfer</summary>
        [EnumDescription("Master Key")]
        MasterKey = 100,
    }

    /// <summary>
    /// Defines where the timing information originates
    /// </summary>
    public enum COSEMClockBase
    {
        /// <summary>Not defined</summary>
        [EnumDescription("Not Defined")]
        NotDefined = 0,
        /// <summary>Internal Crystal</summary>
        [EnumDescription("Internal Crystal")]
        InternalCrystal = 1,
        /// <summary>50 Hz line frequency</summary>
        [EnumDescription("Mains (50Hz)")]
        Mains50Hz = 2,
        /// <summary>60 Hz line frequency</summary>
        [EnumDescription("Mains (60Hz)")]
        Mains60Hz = 3,
        /// <summary>GPS</summary>
        [EnumDescription("GPS")]
        GPS = 4,
        /// <summary>Radio</summary>
        [EnumDescription("Radio Controlled")]
        RadioControlled = 5,
    }

    /// <summary>
    /// Weekday bit string
    /// </summary>
    [Flags]
    public enum COSEMExecuteWeekdays
    {
        /// <summary>Monday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Monday")]
        Monday = 0x01,
        /// <summary>Tuesday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Tuesday")]
        Tuesday = 0x02,
        /// <summary>Wednesday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Wednesday")]
        Wednesday = 0x04,
        /// <summary>Thursday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Thursday")]
        Thursday = 0x08,
        /// <summary>Friday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Friday")]
        Friday = 0x10,
        /// <summary>Saturday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Saturday")]
        Saturday = 0x20,
        /// <summary>Sunday</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "Sunday")]
        Sunday = 0x40,
    }

    /// <summary>
    /// Service ID used to indicate the type of Script Action
    /// </summary>
    public enum COSEMActionServiceID
    {
        /// <summary>Write an attribute</summary>
        [EnumDescription("Write Attribute")]
        WriteAttribute = 1,
        /// <summary>Execute a method</summary>
        [EnumDescription("Execute Method")]
        ExecuteMethod = 2,
    }

    /// <summary>
    /// The types of single action schedule that may be used
    /// </summary>
    public enum COSEMSingleActionType
    {
        /// <summary>One date is specified with wildcards in the date</summary>
        [EnumDescription("Single Date With Wildcards")]
        SingleDateWithWildcards = 1,
        /// <summary>Multiple dates are specified with no wildcards and all times match</summary>
        [EnumDescription("Multiple Dates Without Wildcards (matching times)")]
        MultipleDatesWithoutWildcardsAllTimesMatch = 2,
        /// <summary>Multiple dates are specified with wildcards and all times match</summary>
        [EnumDescription("Multiple Dates With Wildcards (matching times)")]
        MultipleDatesWithWildcardsAllTimesMatch = 3,
        /// <summary>Multiple dates are specified with no wildcards and times may vary</summary>
        [EnumDescription("Multiple Dates Without Wildcards (varying times)")]
        MultipleDatesWithoutWildcardsTimesVary = 4,
        /// <summary>Multiple dates are specified with wildcards and times may vary</summary>
        [EnumDescription("Multiple Dates With Wildcards (varying times)")]
        MultipleDatesWithWildcardsTimesVary = 5,
    }

    /// <summary>
    /// The internal disconnect states
    /// </summary>
    public enum COSEMDisconnectControlState
    {
        /// <summary>Currently disconnected</summary>
        [EnumDescription("Disconnected")]
        Disconnected = 0,
        /// <summary>Currently connected</summary>
        [EnumDescription("Connected")]
        Connected = 1,
        /// <summary>Disconnected but ready to be connected</summary>
        [EnumDescription("Ready for Connection")]
        ReadyForConnection = 2,
    }

    /// <summary>
    /// The mode used by the disconnect switch
    /// </summary>
    public enum COSEMDisconnectControlMode
    {
        /// <summary>The meter is always connected</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "None")]
        None = 0,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Connected to Ready for connection
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Ready for connection
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Not Supported
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "AllDisconnectsRemoteReconnectsToReadyManualReconnectNoLocalReconnect")]
        AllDisconnectsRemoteReconnectsToReadyManualReconnectNoLocalReconnect = 1,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Connected to Ready for connection
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Connected
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Not Supported
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "AllDisconnectsRemoteReconnectManualReconnectNoLocalReconnect")]
        AllDisconnectsRemoteReconnectManualReconnectNoLocalReconnect = 2,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Not supported
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Ready for connection
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Not Supported
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NoManualDisconnectRemoteReconnectToReadyManualReconnectNoLocalReconnect")]
        NoManualDisconnectRemoteReconnectToReadyManualReconnectNoLocalReconnect = 3,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Not supported
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Connected
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Not Supported
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NoManualDisconnectRemoteReconnectManualReconnectNoLocalReconnect")]
        NoManualDisconnectRemoteReconnectManualReconnectNoLocalReconnect = 4,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Connected to Ready for connection
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Ready for connection
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Ready for connection to Connected
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "AllDisconnectsRemoteReconnectToReadyManualReconnectLocalReconnect")]
        AllDisconnectsRemoteReconnectToReadyManualReconnectLocalReconnect = 5,
        /// <summary>
        /// Remote Disconnect - Connected to Disconnected, Ready for connection to Disconnected
        /// Manual Disconnect - Not supported
        /// Local Disconnect - Connected to Ready for connection
        /// Remote Connect - Disconnected to Ready for connection
        /// Manual Connect - Ready for connection to Connected
        /// Local Connect - Ready for connection to Connected
        /// </summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NoManualDisconnectRemoteReconnectToReadyManualReconnectLocalReconnect")]
        NoManualDisconnectRemoteReconnectToReadyManualReconnectLocalReconnect = 6,
    }

    /// <summary>
    /// The various Configuration Content Types
    /// </summary>
    public enum COSEMConfigurationContentTypes : byte
    {
        /// <summary>No Configuration</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "NoConfiguration")]
        NoConfiguration = 0,
        /// <summary>Full XML</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "FullXML")]
        FullXML = 1,
        /// <summary>Full EXI</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "FullEXI")]
        FullEXI = 2,
        /// <summary>DiffGram XML</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "DiffGramXML")]
        DiffGramXML = 3,
        /// <summary>DiffGram EXI</summary>
        [EnumDescription("Itron.Metering.Device.DLMSDevice.COSEMResourceStrings", typeof(COSEMResourceStrings), "DiffGramEXI")]
        DiffGramEXI = 4,
    }

    #endregion

    /// <summary>
    /// COSEM Interface Class base object
    /// </summary>
    public abstract class COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/18/13 RCG 2.80.21 N/A    Created
        
        public virtual ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            return null;
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
        
        public virtual string GetAttributeName(sbyte attributeID)
        {
            return "";
        }

        /// <summary>
        /// Gets the list of Attributes supported by the specified class
        /// </summary>
        /// <param name="classID">The ID of the class requested</param>
        /// <returns>The list of supported attribute IDs</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/31/13 RCG 2.85.06 418571 Created
        
        public static List<sbyte> GetSupportedAttributes(ushort classID)
        {
            List<sbyte> SupportedAttributes = new List<sbyte>();

            switch(classID)
            {
                case 1: // Data
                {
                    SupportedAttributes = COSEMDataInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 3: // Register Value
                {
                    SupportedAttributes = COSEMRegisterInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 4:
                {
                    SupportedAttributes = COSEMExtendedRegisterInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 5:
                {
                    SupportedAttributes = COSEMDemandRegisterInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 7: // Profile Generic
                {
                    SupportedAttributes = COSEMProfileGenericInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 8: // Clock
                {
                    SupportedAttributes = COSEMClockInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 9: // Script Table
                {
                    SupportedAttributes = COSEMScriptTableInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 15: // LN Association
                {
                    SupportedAttributes = COSEMAssociateLongNameInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 18:
                {
                    SupportedAttributes = COSEMImageTransferInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 21:
                {
                    SupportedAttributes = COSEMRegisterMonitorInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 22:
                {
                    SupportedAttributes = COSEMSingleActionScheduleInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 23:
                {
                    SupportedAttributes = COSEMHDLCSetupInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 40:
                {
                    SupportedAttributes = COSEMPushSetupInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 64:
                {
                    SupportedAttributes = COSEMSecuritySetupInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 70:
                {
                    SupportedAttributes = COSEMDisconnectControlInterfaceClass.GetSupportedAttributes();
                    break;
                }
                case 8192:
                {
                    SupportedAttributes = COSEMInterrogationBuilderInterfaceClass.GetSupportedAttributes(); 
                    break;
                }
            }

            return SupportedAttributes;
        }

        /// <summary>
        /// Gets the list of Methods supported by the specified class
        /// </summary>
        /// <param name="classID">The ID of the class requested</param>
        /// <returns>The list of supported method IDs</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/31/13 RCG 2.85.06 418571 Created
        
        public static List<sbyte> GetSupportedMethods(ushort classID)
        {
            List<sbyte> SupportedMethods = new List<sbyte>();

            switch (classID)
            {
                case 1: // Data
                {
                    SupportedMethods = COSEMDataInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 3: // Register Value
                {
                    SupportedMethods = COSEMRegisterInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 4:
                {
                    SupportedMethods = COSEMExtendedRegisterInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 5:
                {
                    SupportedMethods = COSEMDemandRegisterInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 7: // Profile Generic
                {
                    SupportedMethods = COSEMProfileGenericInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 8: // Clock
                {
                    SupportedMethods = COSEMClockInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 9: // Script Table
                {
                    SupportedMethods = COSEMScriptTableInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 15: // LN Association
                {
                    SupportedMethods = COSEMAssociateLongNameInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 18:
                {
                    SupportedMethods = COSEMImageTransferInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 21:
                {
                    SupportedMethods = COSEMRegisterMonitorInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 22:
                {
                    SupportedMethods = COSEMSingleActionScheduleInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 23:
                {
                    SupportedMethods = COSEMHDLCSetupInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 40:
                {
                    SupportedMethods = COSEMPushSetupInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 64:
                {
                    SupportedMethods = COSEMSecuritySetupInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 70:
                {
                    SupportedMethods = COSEMDisconnectControlInterfaceClass.GetSupportedMethods();
                    break;
                }
                case 8192:
                {
                    SupportedMethods = COSEMInterrogationBuilderInterfaceClass.GetSupportedMethods();
                    break;
                }
            }

            return SupportedMethods;
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

        protected COSEMInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
        {
            m_Logger = Logger.TheInstance;

            m_LogicalName = logicalName;
            m_ClassID = 0;
            m_Version = 0;

            m_DLMS = dlms;
        }

        /// <summary>
        /// Writes the message to the log file
        /// </summary>
        /// <param name="message">The message to write</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        protected void WriteToLog(string message)
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Functional, "Interface Class ID: " + m_ClassID.ToString(CultureInfo.InvariantCulture)
                + " LN: " + COSEMLogicalNamesDictionary.LogicalNameString(m_LogicalName) + " - " + message);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the logical name of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
        }

        /// <summary>
        /// Gets the class ID of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
        }

        /// <summary>
        /// Gets the version of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public int Version
        {
            get
            {
                return m_Version;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The logical name of the object</summary>
        protected byte[] m_LogicalName;
        /// <summary>The interface class ID</summary>
        protected ushort m_ClassID;
        /// <summary>The interface version</summary>
        protected int m_Version;
        /// <summary>The Communications device</summary>
        protected DLMSProtocol m_DLMS;
        private Logger m_Logger;


        #endregion
    }

    /// <summary>
    /// Scalar Unit Type
    /// </summary>
    public class COSEMScalerUnitType
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/23/12 RCG 2.70.50 N/A    Created
        
        public COSEMScalerUnitType()
        {
            m_Scaler = 1;
            m_Unit = COSEMUnits.Years;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Scaler Unit type</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/23/12 RCG 2.70.50 N/A    Created
        
        public COSEMScalerUnitType(COSEMData data)
        {
            if (data.DataType == COSEMDataTypes.Structure)
            {
                COSEMData[] StructureData = data.Value as COSEMData[];

                if (StructureData.Length == 2)
                {
                    if (StructureData[0].DataType == COSEMDataTypes.Integer)
                    {
                        m_Scaler = (sbyte)StructureData[0].Value;
                    }
                    else
                    {
                        throw new ArgumentException("The Scaler value is not of type Integer");
                    }

                    if(StructureData[1].DataType == COSEMDataTypes.Enum)
                    {
                        m_Unit = (COSEMUnits)(byte)StructureData[1].Value;
                    }
                    else
                    {
                        throw new ArgumentException("The Units value is not of type Enum");
                    }
                }
                else
                {
                    throw new ArgumentException("The data structure is not length 2.");
                }
            }
            else
            {
                throw new ArgumentException("The data received is not a structure.");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/23/13 RCG 2.80.22 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Scalar Unit");
            ObjectDefinition ScalarDefinition = new ObjectDefinition("Scalar", COSEMDataTypes.Integer);
            EnumObjectDefinition UnitDefinition = new EnumObjectDefinition("Unit", typeof(COSEMUnits));

            ScalarDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(ScalarDefinition);

            UnitDefinition.Value = COSEMUnits.ActiveEnergy;
            Definition.StructureDefinition.Add(UnitDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/23/13 RCG 2.80.22 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_Scaler;
            Definition.StructureDefinition[1].Value = m_Unit;

            // Set the Value field to this instance of the Push Element
            Definition.Value = this;

            return Definition;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Scaler value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/23/12 RCG 2.70.50 N/A    Created
        
        public sbyte Scaler
        {
            get
            {
                return m_Scaler;
            }
            set
            {
                m_Scaler = value;
            }
        }

        /// <summary>
        /// Gets or sets the Unit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/23/12 RCG 2.70.50 N/A    Created
        
        public COSEMUnits Unit
        {
            get
            {
                return m_Unit;
            }
            set
            {
                m_Unit = value;
            }
        }

        #endregion

        #region Member Variables

        private sbyte m_Scaler;
        private COSEMUnits m_Unit;  

        #endregion
    }

    /// <summary>
    /// COSEM Interface Class used to store Register data
    /// </summary>
    public class COSEMRegisterInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Average Voltage Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_VOLTAGE_PHASE_A_LN = new byte[] { 1, 0, 32, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Voltage Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_VOLTAGE_PHASE_B_LN = new byte[] { 1, 0, 52, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Voltage Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_VOLTAGE_PHASE_C_LN = new byte[] { 1, 0, 72, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Current Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_CURRENT_PHASE_A_LN = new byte[] { 1, 0, 31, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Current Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_CURRENT_PHASE_B_LN = new byte[] { 1, 0, 51, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Current Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_CURRENT_PHASE_C_LN = new byte[] { 1, 0, 71, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Power Factor" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_POWER_FACTOR_LN = new byte[] { 1, 0, 13, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Voltage Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_VOLTAGE_PHASE_A_LN = new byte[] { 1, 0, 32, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Voltage Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_VOLTAGE_PHASE_B_LN = new byte[] { 1, 0, 52, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Voltage Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_VOLTAGE_PHASE_C_LN = new byte[] { 1, 0, 72, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Current Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_CURRENT_PHASE_A_LN = new byte[] { 1, 0, 31, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous Current Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_CURRENT_PHASE_B_LN = new byte[] { 1, 0, 51, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Instantaneous CURRENT Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] INSTANTANEOUS_CURRENT_PHASE_C_LN = new byte[] { 1, 0, 71, 7, 0, 255 };
        /// <summary>
        /// The logical name for the "Wh Delivered" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_DELIVERED_LN = new byte[] { 1, 0, 1, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "Wh Received" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_RECEIVED_LN = new byte[] { 1, 0, 2, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "varh Delivered" Register COSEM object
        /// </summary>
        public static readonly byte[] VARH_DELIVERED_LN = new byte[] { 1, 0, 3, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "varh Received" Register COSEM object
        /// </summary>
        public static readonly byte[] VARH_RECEIVED_LN = new byte[] { 1, 0, 4, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "vah Delivered" Register COSEM object
        /// </summary>
        public static readonly byte[] VAH_DELIVERED_LN = new byte[] { 1, 0, 9, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "vah Received" Register COSEM object
        /// </summary>
        public static readonly byte[] VAH_RECEIVED_LN = new byte[] { 1, 0, 10, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Delivered Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_DELIVERED_PHASE_A_LN = new byte[] { 1, 0, 21, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Received Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_RECEIVED_PHASE_A_LN = new byte[] { 1, 0, 22, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "varh Delivered Phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] VARH_DELIVERED_PHASE_A_LN = new byte[] { 1, 0, 23, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Delivered Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_DELIVERED_PHASE_B_LN = new byte[] { 1, 0, 41, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Received Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_RECEIVED_PHASE_B_LN = new byte[] { 1, 0, 42, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "varh Delivered Phase B" Register COSEM object
        /// </summary>
        public static readonly byte[] VARH_DELIVERED_PHASE_B_LN = new byte[] { 1, 0, 43, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Delivered Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_DELIVERED_PHASE_C_LN = new byte[] { 1, 0, 61, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "wh Received Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] WH_RECEIVED_PHASE_C_LN = new byte[] { 1, 0, 62, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "varh Delivered Phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] VARH_DELIVERED_PHASE_C_LN = new byte[] { 1, 0, 63, 8, 0, 255 };
        /// <summary>
        /// The logical name for the "Average Power Factor" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_POWER_FACTOR_LN = new byte[] { 1, 0, 13, 24, 0, 255 };
        /// <summary>
        /// The logical name for the "Rated voltage" Data COSEM object
        /// </summary>
        public static readonly byte[] RATED_VOLTAGE_LN = new byte[] { 1, 0, 0, 6, 0, 255 };
        /// <summary>
        /// The logical name for the "Number of Digits" Data COSEM object
        /// </summary>
        public static readonly byte[] RATED_CURRENT_LN = new byte[] { 1, 0, 0, 6, 1, 255 };
        /// <summary>
        /// The logical name for the "Average active energy imported" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_ACTIVE_ENERGY_IMPORTED_LN = new byte[] { 1, 0, 1, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Average active energy exported" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_ACTIVE_ENERGY_EXPORTED_LN = new byte[] { 1, 0, 2, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "30 Minute Average voltage - phase A" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_30_MINUTES_VOLTS_PHASE_A_LN = new byte[] { 1, 0, 32, 5, 0, 255 };
       /// <summary>
        /// The logical name for the "30 Minute Average voltage - phase C" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_30_MINUTES_VOLTS_PHASE_C_LN = new byte[] { 1, 0, 72, 5, 0, 255 };
        /// <summary>
        /// The logical name for the "Average resultant current from phases A + C" Register COSEM object
        /// </summary>
        public static readonly byte[] AVERAGE_RESULTANT_CURRENT_PHASE_AC_LN = new byte[] { 1, 0, 90, 25, 0, 255 };
        /// <summary>
        /// The logical name for the "Event data record number" Register COSEM object
        /// </summary>
        public static readonly byte[] EVENT_DATA_RECORD_NUMBER_LN = new byte[] { 0, 0, 96, 15, 0, 255 };
        /// <summary>
        /// The logical name for the "30 minute active energy data record number" Register COSEM object
        /// </summary>
        public static readonly byte[] ACTIVE_ENERGY_DATA_RECORD_NUMBER_LN = new byte[] { 0, 0, 96, 15, 1, 255 };
        /// <summary>
        /// The logical name for the "30 minute voltage data record number" Data COSEM object
        /// </summary>
        public static readonly byte[] VOLTAGE_DATA_RECORD_NUMBER_LN = new byte[] { 0, 0, 96, 15, 2, 255 };
        /// <summary>
        /// The logical name for the "Communications Locking Time" Data COSEM object
        /// </summary>
        public static readonly byte[] COMM_LOCKING_TIME_LN = new byte[] { 0, 65, 43, 0, 1, 255 };
        /// <summary>Switch Operation Record Number</summary>
        public static readonly byte[] SWITCH_OPERATION_RECORD_NUMBER_LN = new byte[] { 0, 0, 96, 15, 3, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Imported Energy" Register COSEM object
        /// </summary>
        public static readonly byte[] DISPLAY_TIME_IMPORTED_ENERGY_LN = new byte[] { 1, 65, 0, 32, 0, 255 };
        /// <summary>
        /// The logical name for the "Display Time for Exported Energy" Register COSEM object
        /// </summary>
        public static readonly byte[] DISPLAY_TIME_EXPORTED_ENERGY_LN = new byte[] { 1, 65, 0, 32, 1, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMRegisterInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 3;
            m_Version = 0;
        }

        /// <summary>
        /// Resets the data to it's default value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public virtual ActionResults Reset()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
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
                case 3:
                {
                    AttributeDefinition = ScalarUnit.ToObjectDefinition();
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
                case 3:
                {
                    Name = "Scalar Unit";
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
            return (new sbyte[] { 1, 2, 3 }).ToList();
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
            return (new sbyte[] { 1 }).ToList();
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
                WriteToLog("Failed to Get the Data Value - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the value
        /// </summary>
        /// <param name="data">The Get Data Result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseScalarUnit(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_ScalarUnit = new COSEMScalerUnitType(data.DataValue);
                }
                catch (Exception e)
                {
                    m_ScalarUnit = null;
                    WriteToLog("Failed to Get the Scalar Unit - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ScalarUnit = null;
                WriteToLog("Failed to Get the Scalar Unit - Reason: " + data.DataAccessResult.ToDescription());
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
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

        /// <summary>
        /// Gets the scalar unit of the object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScalerUnitType ScalarUnit
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseScalarUnit(Result);
                    }
                }

                return m_ScalarUnit;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The register value</summary>
        protected COSEMData m_Value;
        /// <summary>The unit type of the register value</summary>
        protected COSEMScalerUnitType m_ScalarUnit;

        #endregion
    }

    /// <summary>
    /// COSEM Interface Class used to store Extended Register Data
    /// </summary>
    public class COSEMExtendedRegisterInterfaceClass : COSEMRegisterInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the item</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMExtendedRegisterInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 4;
            m_Version = 0;
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
                case 3:
                {
                    AttributeDefinition = ScalarUnit.ToObjectDefinition();
                    break;
                }
                case 4:
                {
                    COSEMDateTime Value = CaptureTime;

                    if (Value != null)
                    {
                        AttributeDefinition = new ObjectDefinition("Capture Time", COSEMDataTypes.DateTime);
                        AttributeDefinition.Value = CaptureTime;
                    }
                    else
                    {
                        AttributeDefinition = new ObjectDefinition("Capture Time", COSEMDataTypes.NullData);
                        AttributeDefinition.Value = null;
                    }

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
                case 3:
                {
                    Name = "Scalar Unit";
                    break;
                }
                case 4:
                {
                    Name = "Capture Time";
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

        public static new List<sbyte> GetSupportedAttributes()
        {
            return (new sbyte[] { 1, 2, 3, 4 }).ToList();
        }

        /// <summary>
        /// Gets the list of supported methods
        /// </summary>
        /// <returns>The list of Method IDs supported</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/31/13 RCG 2.85.06 418571 Created

        public static new List<sbyte> GetSupportedMethods()
        {
            return (new sbyte[] { 1 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the status from the Get Data Result
        /// </summary>
        /// <param name="data">The Get Data Result containing the status</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created
        
        private void ParseStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                switch (data.DataValue.DataType)
                {
                    case COSEMDataTypes.NullData:
                    case COSEMDataTypes.BitString:
                    case COSEMDataTypes.DoubleLongUnsigned:
                    case COSEMDataTypes.OctetString:
                    case COSEMDataTypes.VisibleString:
                    case COSEMDataTypes.UTF8String:
                    case COSEMDataTypes.Unsigned:
                    case COSEMDataTypes.LongUnsigned:
                    case COSEMDataTypes.Long64Unsigned:
                    {
                        try
                        {
                            m_Status = data.DataValue;
                        }
                        catch (Exception e)
                        {
                            m_Status = null;
                            WriteToLog("Failed to Get the Status - Exception Occurred while parsing the data. Message: " + e.Message);
                            throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                        }
                        break;
                    }
                    default:
                    {
                        m_Status = null;
                        WriteToLog("Failed to parse the Status - Unexpected data type.");
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                    }
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Status = null;
                WriteToLog("Failed to Get the Status - Reason: " + data.DataAccessResult.ToDescription());
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the capture time from the Get Data Result
        /// </summary>
        /// <param name="data">The Get Data Result containing the capture time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseCaptureTime(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_CaptureTime = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_CaptureTime = null;
                        WriteToLog("Failed to Get the Capture Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_CaptureTime = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_CaptureTime = null;
                        WriteToLog("Failed to Get the Capture Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.NullData)
                {
                    m_CaptureTime = null;
                }
                else
                {
                    m_CaptureTime = null;
                    WriteToLog("Failed to parse the Capture Time - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CaptureTime = null;
                WriteToLog("Failed to Get the Capture Time - Reason: " + data.DataAccessResult.ToDescription());
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The status of the extended register
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMData Status
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseStatus(Result);
                    }
                }

                return m_Status;
            }
        }

        /// <summary>
        /// Gets the Date and Time the Value was captured
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMDateTime CaptureTime
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseCaptureTime(Result);
                    }
                }

                return m_CaptureTime;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The status of the register object</summary>
        protected COSEMData m_Status;
        /// <summary>The time the register value was captured</summary>
        protected COSEMDateTime m_CaptureTime;

        #endregion
    }

    /// <summary>
    /// COSEM Interface Class used to store Demand data
    /// </summary>
    public class COSEMDemandRegisterInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDemandRegisterInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 5;
            m_Version = 0;
        }

        /// <summary>
        /// Resets the data to the default value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults Reset()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Closes the current period and starts a new one
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults NextPeriod()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Current Average Value", CurrentAverageValue);
                    break;
                }
                case 3:
                {
                    AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Last Average Value", LastAverageValue);
                    break;
                }
                case 4:
                {
                    AttributeDefinition = ScalarUnit.ToObjectDefinition();
                    break;
                }
                case 5:
                {
                    AttributeDefinition = ObjectDefinition.CreateFromCOSEMData("Status", Status);
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new ObjectDefinition("Capture Time", COSEMDataTypes.DateTime);
                    AttributeDefinition.Value = CaptureTime;
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("Start Time Current", COSEMDataTypes.DateTime);
                    AttributeDefinition.Value = StartTimeCurrent;
                    break;
                }
                case 8:
                {
                    AttributeDefinition = new ObjectDefinition("Period", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = Period;
                    break;
                }
                case 9:
                {
                    AttributeDefinition = new ObjectDefinition("Number of Periods", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = NumberOfPeriods;
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
                    Name = "Current Average Value";
                    break;
                }
                case 3:
                {
                    Name = "Last Average Value";
                    break;
                }
                case 4:
                {
                    Name = "Scalar Unit";
                    break;
                }
                case 5:
                {
                    Name = "Status";
                    break;
                }
                case 6:
                {
                    Name = "Capture Time";
                    break;
                }
                case 7:
                {
                    Name = "Start Time Current";
                    break;
                }
                case 8:
                {
                    Name = "Period";
                    break;
                }
                case 9:
                {
                    Name = "Number of Periods";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).ToList();
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
            return (new sbyte[] { 1, 2 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the average value for the current interval
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseCurrentAverage(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_CurrentAverageValue = data.DataValue;
                }
                catch (Exception e)
                {
                    m_CurrentAverageValue = null;
                    WriteToLog("Failed to Get the Current Average Value - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CurrentAverageValue = null;
                WriteToLog("Failed to Get the Current Average Value - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the average value for the last interval
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseLastAverage(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_LastAverageValue = data.DataValue;
                }
                catch (Exception e)
                {
                    m_LastAverageValue = null;
                    WriteToLog("Failed to Get the Last Average Value - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_LastAverageValue = null;
                WriteToLog("Failed to Get the Last Average Value - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the value
        /// </summary>
        /// <param name="data">The Get Data Result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseScalarUnit(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_ScalarUnit = new COSEMScalerUnitType(data.DataValue);
                }
                catch (Exception e)
                {
                    m_ScalarUnit = null;
                    WriteToLog("Failed to Get the Scalar Unit - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ScalarUnit = null;
                WriteToLog("Failed to Get the Scalar Unit - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the status from the Get Data Result
        /// </summary>
        /// <param name="data">The Get Data Result containing the status</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                    switch (data.DataValue.DataType)
                    {
                        case COSEMDataTypes.NullData:
                        case COSEMDataTypes.BitString:
                        case COSEMDataTypes.DoubleLongUnsigned:
                        case COSEMDataTypes.OctetString:
                        case COSEMDataTypes.VisibleString:
                        case COSEMDataTypes.UTF8String:
                        case COSEMDataTypes.Unsigned:
                        case COSEMDataTypes.LongUnsigned:
                        case COSEMDataTypes.Long64Unsigned:
                        {
                            try
                            {
                                m_Status = data.DataValue;
                            }
                            catch (Exception e)
                            {
                                m_Status = null;
                                WriteToLog("Failed to Get the Status - Exception Occurred while parsing the data. Message: " + e.Message);
                                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                            }
                            break;
                        }
                        default:
                        {
                            m_Status = null;
                            WriteToLog("Failed to parse the Status - Unexpected data type.");
                            throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                        }
                    }
            }
            else
            {
                // We received some sort of error message from the get
                m_Status = null;
                WriteToLog("Failed to Get the Status - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the capture time from the Get Data Result
        /// </summary>
        /// <param name="data">The Get Data Result containing the capture time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseCaptureTime(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_CaptureTime = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_CaptureTime = null;
                        WriteToLog("Failed to Get the Capture Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_CaptureTime = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_CaptureTime = null;
                        WriteToLog("Failed to Get the Capture Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    m_CaptureTime = null;
                    WriteToLog("Failed to parse the Capture Time - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CaptureTime = null;
                WriteToLog("Failed to Get the Capture Time - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the start time of the current interval
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParseStartTimeCurrent(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_StartTimeCurrent = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_StartTimeCurrent = null;
                        WriteToLog("Failed to Get the Start Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_StartTimeCurrent = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_StartTimeCurrent = null;
                        WriteToLog("Failed to Get the Start Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    m_StartTimeCurrent = null;
                    WriteToLog("Failed to parse the Start Time - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_StartTimeCurrent = null;
                WriteToLog("Failed to Get the Start Time - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the period length
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created

        private void ParsePeriod(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_Period = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_Period = 0;
                        WriteToLog("Failed to Get the Period - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Period = 0;
                    WriteToLog("Failed to parse the Period - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Period = 0;
                WriteToLog("Failed to Get the Period - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the number of periods
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created
        
        private void ParseNumberOfPeriods(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_NumberOfPeriods = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_NumberOfPeriods = 0;
                        WriteToLog("Failed to Get the Number of Periods - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    m_NumberOfPeriods = 0;
                    WriteToLog("Failed to parse the Number of Periods - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_NumberOfPeriods = 0;
                throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The demand in the current period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMData CurrentAverageValue
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseCurrentAverage(Result);
                    }
                }

                return m_CurrentAverageValue;
            }
        }

        /// <summary>
        /// The average energy accumulated in a period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMData LastAverageValue
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseLastAverage(Result);
                    }
                }

                return m_LastAverageValue;
            }
        }

        /// <summary>
        /// Gets the scalar unit of the demand register
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScalerUnitType ScalarUnit
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseScalarUnit(Result);
                    }
                }

                return m_ScalarUnit;
            }
        }

        /// <summary>
        /// Gets the status of the demand register
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMData Status
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    if (Result != null)
                    {
                        ParseStatus(Result);
                    }
                }

                return m_Status;
            }
        }

        /// <summary>
        /// Gets the Date and Time that the last average value was calculated
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDateTime CaptureTime
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    if (Result != null)
                    {
                        ParseCaptureTime(Result);
                    }
                }

                return m_CaptureTime;
            }
        }

        /// <summary>
        /// Gets the Start Time of the Current Period
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDateTime StartTimeCurrent
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    if (Result != null)
                    {
                        ParseStartTimeCurrent(Result);
                    }
                }

                return m_StartTimeCurrent;
            }
        }

        /// <summary>
        /// Gets the length of the period in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint Period
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 8);

                    if (Result != null)
                    {
                        ParsePeriod(Result);
                    }
                }

                return m_Period;
            }
        }

        /// <summary>
        /// Gets the number of periods used to calculate the average
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort NumberOfPeriods
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 9);

                    if (Result != null)
                    {
                        ParseNumberOfPeriods(Result);
                    }
                }

                return m_NumberOfPeriods;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Current Average Demand</summary>
        protected COSEMData m_CurrentAverageValue;
        /// <summary>Last Average Demand</summary>
        protected COSEMData m_LastAverageValue;
        /// <summary>Scalar Units</summary>
        protected COSEMScalerUnitType m_ScalarUnit;
        /// <summary>Status of the value</summary>
        protected COSEMData m_Status;
        /// <summary>The Date and Time the Last Average was calculated</summary>
        protected COSEMDateTime m_CaptureTime;
        /// <summary>The start time of the current period</summary>
        protected COSEMDateTime m_StartTimeCurrent;
        /// <summary>The length of the period in seconds</summary>
        protected uint m_Period;
        /// <summary>The number of periods the average is calculated across</summary>
        protected ushort m_NumberOfPeriods;

        #endregion
    }

    /// <summary>
    /// Register Activation Mask
    /// </summary>
    public class COSEMRegisterActivationMask
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMRegisterActivationMask()
        {
            m_MaskName = null;
            m_IndexList = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Mask Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] MaskName
        {
            get
            {
                return m_MaskName;
            }
            set
            {
                m_MaskName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Index List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] IndexList
        {
            get
            {
                return m_IndexList;
            }
            set
            {
                m_IndexList = value;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_MaskName;
        private byte[] m_IndexList;

        #endregion
    }

    /// <summary>
    /// Register Activation Interface Class
    /// </summary>
    public class COSEMRegisterActivationInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMRegisterActivationInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 6;
            m_Version = 0;
        }

        /// <summary>
        /// Adds the specified register to the list
        /// </summary>
        /// <param name="newRegister">The register to add</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void AddRegister(COSEMInterfaceClass newRegister)
        {
        }

        /// <summary>
        /// Adds the specified mask to the list
        /// </summary>
        /// <param name="newMask">The mask to add</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void AddMask(COSEMRegisterActivationMask newMask)
        {
        }

        /// <summary>
        /// Deletes the specified mask
        /// </summary>
        /// <param name="maskName">The mask to remove</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void DeleteMask(byte[] maskName)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the register assignments
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<COSEMInterfaceClass> RegisterAssignment
        {
            get
            {
                return m_RegisterAssignment;
            }
        }

        /// <summary>
        /// Gets the list of masks
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<COSEMRegisterActivationMask> MaskList
        {
            get
            {
                return m_MaskList;
            }
        }

        /// <summary>
        /// Gets the name of the active mask
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] ActivaMaskName
        {
            get
            {
                return m_ActiveMaskName;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The list of register assignments</summary>
        protected List<COSEMInterfaceClass> m_RegisterAssignment;
        /// <summary>The list of register activation masks</summary>
        protected List<COSEMRegisterActivationMask> m_MaskList;
        /// <summary>The name of the active mask</summary>
        protected byte[] m_ActiveMaskName;

        #endregion
    }

    /// <summary>
    /// COSEM Generic Profile Capture Object
    /// </summary>
    public class COSEMProfileCaptureObject : IEquatable<COSEMProfileCaptureObject>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMProfileCaptureObject()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">Logical Name of the </param>
        /// <param name="classID"></param>
        /// <param name="attributeID"></param>
        /// <param name="dataIndex"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public COSEMProfileCaptureObject(byte[] logicalName, ushort classID, sbyte attributeID, ushort dataIndex)
        {
            m_LogicalName = logicalName;
            m_ClassID = classID;
            m_AttributeIndex = attributeID;
            m_DataIndex = dataIndex;

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Profile Capture Object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMProfileCaptureObject(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 4)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ClassID = (ushort)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Class ID is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_LogicalName = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Logical Name is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Integer)
                        {
                            m_AttributeIndex = (sbyte)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Attribute Index is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DataIndex = (ushort)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Data Index is not the expected data type.");
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
        //  05/07/13 RCG 2.80.27 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Capture Object");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new ObjectDefinition("Class ID", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Logical Name", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Attribute Index", COSEMDataTypes.Integer);
            NewObjectDefinition.Value = (sbyte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Data Index", COSEMDataTypes.LongUnsigned);
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
        //  05/07/13 RCG 2.80.27 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ClassID;
            Definition.StructureDefinition[1].Value = m_LogicalName;
            Definition.StructureDefinition[2].Value = m_AttributeIndex;
            Definition.StructureDefinition[3].Value = m_DataIndex;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets whether or not the two Capture Objects are equal
        /// </summary>
        /// <param name="other">The Capture Object to check for equality</param>
        /// <returns>True if the Capture Objects are equal. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public bool Equals(COSEMProfileCaptureObject other)
        {
            bool IsEqual = false;

            if (other != null)
            {
                IsEqual = LogicalName.IsEqual(other.LogicalName) && AttributeIndex.Equals(other.AttributeIndex) && DataIndex.Equals(other.DataIndex);
            }

            return IsEqual;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the logical name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public sbyte AttributeIndex
        {
            get
            {
                return m_AttributeIndex;
            }
            set
            {
                m_AttributeIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the data index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public ushort DataIndex
        {
            get
            {
                return m_DataIndex;
            }
            set
            {
                m_DataIndex = value;
            }
        }

        /// <summary>
        /// Gets the COSEMData object that represents the Profile Capture Object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMData Data
        {
            get
            {
                COSEMData CaptureObject = new COSEMData();
                COSEMData[] StructureData = new COSEMData[4];

                // Class ID
                StructureData[0] = new COSEMData();
                StructureData[0].DataType = COSEMDataTypes.LongUnsigned;
                StructureData[0].Value = m_ClassID;

                // Logical Name
                StructureData[1] = new COSEMData();
                StructureData[1].DataType = COSEMDataTypes.OctetString;
                StructureData[1].Value = m_LogicalName;

                // Attribute Index
                StructureData[2] = new COSEMData();
                StructureData[2].DataType = COSEMDataTypes.Integer;
                StructureData[2].Value = m_AttributeIndex;

                // Data Index
                StructureData[3] = new COSEMData();
                StructureData[3].DataType = COSEMDataTypes.LongUnsigned;
                StructureData[3].Value = m_DataIndex;

                CaptureObject.DataType = COSEMDataTypes.Structure;
                CaptureObject.Value = StructureData;

                return CaptureObject;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private sbyte m_AttributeIndex;
        private ushort m_DataIndex;

        #endregion
    }

    /// <summary>
    /// Profile Range Descriptor for selective access
    /// </summary>
    public class COSEMProfileRangeDescriptor
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMProfileRangeDescriptor()
        {
            m_RestrictingObject = null;
            m_FromValue = new COSEMData();
            m_ToValue = new COSEMData();
            m_SelectedValues = new List<COSEMProfileCaptureObject>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks to see if the COSEMData objects is a valid limit data type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the value is a valid limit data type. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        private bool CheckValidLimit(COSEMData value)
        {
            bool IsValid = false;

            switch(value.DataType)
            {
                case COSEMDataTypes.DoubleLong:
                case COSEMDataTypes.DoubleLongUnsigned:
                case COSEMDataTypes.OctetString:
                case COSEMDataTypes.VisibleString:
                case COSEMDataTypes.UTF8String:
                case COSEMDataTypes.Integer:
                case COSEMDataTypes.Long:
                case COSEMDataTypes.Unsigned:
                case COSEMDataTypes.LongUnsigned:
                case COSEMDataTypes.Long64:
                case COSEMDataTypes.Long64Unsigned:
                case COSEMDataTypes.Float32:
                case COSEMDataTypes.Float64:
                case COSEMDataTypes.DateTime:
                case COSEMDataTypes.Date:
                case COSEMDataTypes.Time:
                {
                    IsValid = true;
                    break;
                }
            }

            return IsValid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the restricting object used for selective access
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created

        public COSEMProfileCaptureObject RestrictingObject
        {
            get
            {
                return m_RestrictingObject;
            }
            set
            {
                if (value != null)
                {
                    m_RestrictingObject = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The Restricting Object may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the oldest or smallest entry to retrieve
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created

        public COSEMData FromValue
        {
            get
            {
                return m_FromValue;
            }
            set
            {
                if (value != null)
                {
                    if (CheckValidLimit(value))
                    {
                        m_FromValue = value;
                    }
                    else
                    {
                        throw new ArgumentException("The specified From Value is not a valid limiting data type", "value");
                    }
                }
                else
                {
                    throw new ArgumentNullException("value", "The From Value may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the newest or largest entry to retrieve
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created

        public COSEMData ToValue
        {
            get
            {
                return m_ToValue;
            }
            set
            {
                if (value != null)
                {
                    if (CheckValidLimit(value))
                    {
                        m_ToValue = value;
                    }
                    else
                    {
                        throw new ArgumentException("The specified To Value is not a valid limiting data type", "value");
                    }
                }
                else
                {
                    throw new ArgumentNullException("value", "The To Value may not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Selected Values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public List<COSEMProfileCaptureObject> SelectedValues
        {
            get
            {
                return m_SelectedValues;
            }
            set
            {
                if (value != null)
                {
                    m_SelectedValues = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The Selected Values may not be null");
                }
            }
        }

        /// <summary>
        /// Gets the COSEMData representation of the Range Descriptor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMData Data
        {
            get
            {
                COSEMData RangeDescriptor = new COSEMData();
                COSEMData[] StructureData = new COSEMData[4];
                COSEMData[] SelectedValues = new COSEMData[m_SelectedValues.Count];

                // Restricting Object
                StructureData[0] = m_RestrictingObject.Data;

                // From Value
                StructureData[1] = m_FromValue;

                // To Value
                StructureData[2] = m_ToValue;

                // Selected Values
                for (int Index = 0; Index < m_SelectedValues.Count; Index++)
                {
                    SelectedValues[Index] = m_SelectedValues[Index].Data;
                }

                StructureData[3] = new COSEMData();
                StructureData[3].DataType = COSEMDataTypes.Array;
                StructureData[3].Value = SelectedValues;

                RangeDescriptor.DataType = COSEMDataTypes.Structure;
                RangeDescriptor.Value = StructureData;

                return RangeDescriptor;
            }
        }

        #endregion

        #region Member Variables

        private COSEMProfileCaptureObject m_RestrictingObject;
        private COSEMData m_FromValue;
        private COSEMData m_ToValue;
        private List<COSEMProfileCaptureObject> m_SelectedValues;

        #endregion
    }

    /// <summary>
    /// Profile Entry Descriptor for selective access
    /// </summary>
    public class COSEMProfileEntryDescriptor
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMProfileEntryDescriptor()
        {
            m_FromEntry = 0;
            m_ToEntry = 0;
            m_FromSelectedValue = 0;
            m_ToSelectedValue = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the first entry to receive (row).
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public uint FromEntry
        {
            get
            {
                return m_FromEntry;
            }
            set
            {
                m_FromEntry = value;
            }
        }

        /// <summary>
        /// Gets or sets the last entry to received (row). A 0 value means highest available. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public uint ToEntry
        {
            get
            {
                return m_ToEntry;
            }
            set
            {
                m_ToEntry = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the first value to retrieve (column)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public ushort FromSelectedValue
        {
            get
            {
                return m_FromSelectedValue;
            }
            set
            {
                m_FromSelectedValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the last value to retrieve (column). A 0 value means highest available value.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public ushort ToSelectedValue
        {
            get
            {
                return m_ToSelectedValue;
            }
            set
            {
                m_ToSelectedValue = value;
            }
        }

        /// <summary>
        /// Gets the Entry Descriptor in it's COSEMData form
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMData Data
        {
            get
            {
                COSEMData EntryDescriptor = new COSEMData();
                COSEMData[] StructureData = new COSEMData[4];

                // From Entry
                StructureData[0] = new COSEMData();
                StructureData[0].DataType = COSEMDataTypes.DoubleLongUnsigned;
                StructureData[0].Value = m_FromEntry;

                // To Entry
                StructureData[1] = new COSEMData();
                StructureData[1].DataType = COSEMDataTypes.DoubleLongUnsigned;
                StructureData[1].Value = m_ToEntry;

                // From Selected Value
                StructureData[2] = new COSEMData();
                StructureData[2].DataType = COSEMDataTypes.LongUnsigned;
                StructureData[2].Value = m_FromSelectedValue;

                // To Selected Value
                StructureData[3] = new COSEMData();
                StructureData[3].DataType = COSEMDataTypes.LongUnsigned;
                StructureData[3].Value = m_ToSelectedValue;


                EntryDescriptor.DataType = COSEMDataTypes.Structure;
                EntryDescriptor.Value = StructureData;

                return EntryDescriptor;
            }
        }

        #endregion

        #region Member Variables

        private uint m_FromEntry;
        private uint m_ToEntry;
        private ushort m_FromSelectedValue;
        private ushort m_ToSelectedValue;

        #endregion
    }

    /// <summary>
    /// COSEM Generic Profile Interface Class
    /// </summary>
    public class COSEMProfileGenericInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Billing Period 1 Data" COSEM object
        /// </summary>
        public static readonly byte[] BILLING_PERIOD_ONE_DATA_LN = new byte[] { 0, 0, 98, 1, 0, 255 };
        /// <summary>
        /// The logical name for the "Load Profile Set 1" COSEM object
        /// </summary>
        public static readonly byte[] LOAD_PROFILE_SET_ONE_LN = new byte[] { 1, 0, 99, 1, 0, 255 };
        /// <summary>
        /// The logical name for the "Load Profile Set 2" COSEM object
        /// </summary>
        public static readonly byte[] LOAD_PROFILE_SET_TWO_LN = new byte[] { 1, 0, 99, 1, 1, 255 };
        /// <summary>
        /// The logical name for the Switch Operation History COSEM object
        /// </summary>
        public static readonly byte[] SWITCH_OPERATION_HISTORY_LN = new byte[] { 0, 0, 99, 98, 2, 255 };
        /// <summary>
        /// The logical name for the Event Profile COSEM object
        /// </summary>
        public static readonly byte[] EVENT_PROFILE_LN = new byte[] { 0, 0, 99, 98, 0, 255 };
        /// <summary>
        /// The logical name for the Meter Specification Profile COSEM object
        /// </summary>
        public static readonly byte[] METER_SPECIFICATION_PROFILE_LN = new byte[] { 1, 65, 98, 99, 0, 255 };
        /// <summary>
        /// The logical name for the Meter Reading of the Current Value Profile COSEM object
        /// </summary>
        public static readonly byte[] METER_READING_OF_THE_CURRENT_VALUE_PROFILE_LN = new byte[] { 1, 65, 99, 99, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMProfileGenericInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 7;
            m_Version = 1;
        }

        /// <summary>
        /// Resets the profile data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public ActionResults Reset()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Copies the values of the objects to capture into the buffer by reading each capture object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public ActionResults Capture()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Uses Selective Access to retrieve data from the buffer
        /// </summary>
        /// <param name="range">The range of data to retrieve</param>
        /// <returns>The buffer data retrieved</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMData[] GetBuffer(COSEMProfileRangeDescriptor range)
        {
            COSEMData[] Buffer = null;

            if (m_DLMS.IsConnected)
            {
                GetDataResult Result = null;
                SelectiveAccessDescriptor AccessSelector = new SelectiveAccessDescriptor();
                AccessSelector.AccessSelector = 1; // Range Descriptor
                AccessSelector.AccessParameters = range.Data;

                Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2, AccessSelector);

                if (Result.GetDataResultType == GetDataResultChoices.Data)
                {
                    try
                    {
                        if (Result.DataValue.DataType == COSEMDataTypes.Array)
                        {
                            Buffer = Result.DataValue.Value as COSEMData[];
                        }
                        else if (Result.DataValue.DataType == COSEMDataTypes.CompactArray)
                        {
                            // TODO: Handle compact array
                        }
                        else
                        {
                            WriteToLog("Failed to parse the Selective Access Buffer - Unexpected data type.");
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToLog("Failed to Get the Selective Access Buffer - Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    // We received some sort of error message from the get
                    WriteToLog("Failed to Get the Selective Access Buffer - Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result.DataAccessResult));
                }
            }

            return Buffer;
        }

        /// <summary>
        /// Uses Selective Access to retrieve data from the buffer
        /// </summary>
        /// <param name="entry">The range of data to retrieve</param>
        /// <returns>The buffer data retrieved</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/12/13 RCG 2.80.07 N/A    Created
        
        public COSEMData[] GetBuffer(COSEMProfileEntryDescriptor entry)
        {
            COSEMData[] Buffer = null;

            if (m_DLMS.IsConnected)
            {
                GetDataResult Result = null;
                SelectiveAccessDescriptor AccessSelector = new SelectiveAccessDescriptor();
                AccessSelector.AccessSelector = 2; // Entry Descriptor
                AccessSelector.AccessParameters = entry.Data;

                Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2, AccessSelector);

                if (Result.GetDataResultType == GetDataResultChoices.Data)
                {
                    try
                    {
                        if (Result.DataValue.DataType == COSEMDataTypes.Array)
                        {
                            Buffer = Result.DataValue.Value as COSEMData[];
                        }
                        else if (Result.DataValue.DataType == COSEMDataTypes.CompactArray)
                        {
                            // TODO: Handle compact array
                        }
                        else
                        {
                            WriteToLog("Failed to parse the Selective Access Buffer - Unexpected data type.");
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToLog("Failed to Get the Selective Access Buffer - Exception Occurred while parsing the data. Message: " + e.Message);
                    }
                }
                else
                {
                    // We received some sort of error message from the get
                    WriteToLog("Failed to Get the Selective Access Buffer - Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result.DataAccessResult));
                }
            }

            return Buffer;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case -1:
                {
                    AttributeDefinition = new ObjectDefinition("Auto Capture on Read", COSEMDataTypes.Boolean);
                    AttributeDefinition.Value = AutoCaptureOnRead;
                    break;
                }
                case 3:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Capture Objects", COSEMProfileCaptureObject.GetStructureDefinition());
                    List<COSEMProfileCaptureObject> Objects = CaptureObjects;

                    if (Objects != null)
                    {
                        foreach (COSEMProfileCaptureObject CurrentObject in Objects)
                        {
                            ArrayDefinition.Elements.Add(CurrentObject.ToObjectDefinition());
                        }
                    }

                    AttributeDefinition = ArrayDefinition;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new ObjectDefinition("Capture Period", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = CapturePeriod;
                    break;
                }
                case 5:
                {
                    EnumObjectDefinition EnumDefinition = new EnumObjectDefinition("Sort Method", typeof(COSEMSortMethod));
                    EnumDefinition.Value = SortMethod;

                    AttributeDefinition = EnumDefinition;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = SortObject.ToObjectDefinition();
                    AttributeDefinition.ItemName = "Sort Object";
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("Entries in Use", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = EntriesInUse;
                    break;
                }
                case 8:
                {
                    AttributeDefinition = new ObjectDefinition("Profile Entries", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = ProfileEntries;
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
                case -1:
                {
                    Name = "Auto Capture on Read";
                    break;
                }
                case 3:
                {
                    Name = "Capture Objects";
                    break;
                }
                case 4:
                {
                    Name = "Capture Period";
                    break;
                }
                case 5:
                {
                    Name = "Sort Method";
                    break;
                }
                case 6:
                {
                    Name = "Sort Object";
                    break;
                }
                case 7:
                {
                    Name = "Entries in Use";
                    break;
                }
                case 8:
                {
                    Name = "Profile Entries";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8 }).ToList();
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
            return (new sbyte[] { 1, 2 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created

        private void ParseAutoCaptureOnRead(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Boolean)
                {
                    try
                    {
                        m_AutoCaptureOnRead = (bool)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_AutoCaptureOnRead = false;
                        WriteToLog("Failed to Get Auto Capture On Read - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_AutoCaptureOnRead = false;
                    WriteToLog("Failed to parse Auto Capture On Read - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_AutoCaptureOnRead = false;
                WriteToLog("Failed to Get Auto Capture On Read - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Capture objects
        /// </summary>
        /// <param name="data">The Get Data Result containing the Capture Objects</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        private void ParseCaptureObjects(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        m_CaptureObjects = new List<COSEMProfileCaptureObject>();

                        COSEMData[] CaptureData = data.DataValue.Value as COSEMData[];

                        foreach (COSEMData CurrentCaptureData in CaptureData)
                        {
                            m_CaptureObjects.Add(new COSEMProfileCaptureObject(CurrentCaptureData));
                        }
                    }
                    catch (Exception e)
                    {
                        m_CaptureObjects = null;
                        WriteToLog("Failed to Get the Capture Objects - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_CaptureObjects = null;
                    WriteToLog("Failed to parse the Capture Objects - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CaptureObjects = null;
                WriteToLog("Failed to Get the Capture Objects - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Capture period
        /// </summary>
        /// <param name="data">The Get Data Result containing the Capture Period</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseCapturePeriod(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_CapturePeriod = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_CapturePeriod = 0;
                        WriteToLog("Failed to Get the Capture Period - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_CapturePeriod = 0;
                    WriteToLog("Failed to parse the Capture Period - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CapturePeriod = 0;
                WriteToLog("Failed to Get the Capture Period - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Sort Method
        /// </summary>
        /// <param name="data">The Get Data Result containing the Sort Method</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseSortMethod(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_SortMethod = (COSEMSortMethod)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_SortMethod = COSEMSortMethod.Fifo;
                        WriteToLog("Failed to Get the Sort Method - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_SortMethod = COSEMSortMethod.Fifo;
                    WriteToLog("Failed to parse the Sort Method - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SortMethod = COSEMSortMethod.Fifo;
                WriteToLog("Failed to Get the Sort Method - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Sort Object
        /// </summary>
        /// <param name="data">The Get Data Result containing the sort object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        private void ParseSortObject(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Structure)
                {
                    try
                    {
                        m_SortObject = new COSEMProfileCaptureObject(data.DataValue);
                    }
                    catch (Exception e)
                    {
                        m_SortObject = null;
                        WriteToLog("Failed to Get the Sort Object - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_SortObject = null;
                    WriteToLog("Failed to parse the Sort Object - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SortObject = null;
                WriteToLog("Failed to Get the Sort Object - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Entries in Use
        /// </summary>
        /// <param name="data">The Get Data Result containing the Entries in Use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseEntriesInUse(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_EntriesInUse = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_EntriesInUse = 0;
                        WriteToLog("Failed to Get the Entries in Use - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_EntriesInUse = 0;
                    WriteToLog("Failed to parse the Entries in Use - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_EntriesInUse = 0;
                WriteToLog("Failed to Get the Entries in Use - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Profile Entries
        /// </summary>
        /// <param name="data">The Get Data Result containing the Profile Entries</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseProfileEntries(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_ProfileEntries = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ProfileEntries = 0;
                        WriteToLog("Failed to Get the Profile Entries - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ProfileEntries = 0;
                    WriteToLog("Failed to parse the Profile Entries - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ProfileEntries = 0;
                WriteToLog("Failed to Get the Profile Entries - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter will perform an auto capture on a read of the buffer data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public bool AutoCaptureOnRead
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, -1);

                    ParseAutoCaptureOnRead(Result);
                }

                return m_AutoCaptureOnRead;
            }
        }

        /// <summary>
        /// Gets the list of Capture Objects for the profile data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMProfileCaptureObject> CaptureObjects
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    ParseCaptureObjects(Result);
                }

                return m_CaptureObjects;
            }
        }

        /// <summary>
        /// Gets the length of capture period in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint CapturePeriod
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    ParseCapturePeriod(Result);
                }

                return m_CapturePeriod;
            }
        }

        /// <summary>
        /// Gets the method used to sort the data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMSortMethod SortMethod
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    ParseSortMethod(Result);
                }

                return m_SortMethod;
            }
        }

        /// <summary>
        /// The register or clock that the ordering is based on
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMProfileCaptureObject SortObject
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    ParseSortObject(Result);
                }

                return m_SortObject;
            }
        }

        /// <summary>
        /// The number of valid entries stored in the buffer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public uint EntriesInUse
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    ParseEntriesInUse(Result);
                }

                return m_EntriesInUse;
            }
        }

        /// <summary>
        /// The number of entries that can be retained in the buffer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public uint ProfileEntries
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 8);

                    ParseProfileEntries(Result);
                }

                return m_ProfileEntries;
            }
        }

        #endregion

        #region Member Variables

        private bool m_AutoCaptureOnRead;
        private List<COSEMProfileCaptureObject> m_CaptureObjects;
        private uint m_CapturePeriod;
        private COSEMSortMethod m_SortMethod;
        private COSEMProfileCaptureObject m_SortObject;
        private uint m_EntriesInUse;
        private uint m_ProfileEntries;

        #endregion
    }

    /// <summary>
    /// COSEM Utility Table Interface Class
    /// </summary>
    public class COSEMUtilityTableInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMUtilityTableInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 26;
            m_Version = 0;
        } 

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Table ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort TableID
        {
            get
            {
                return m_TableID;
            }
        }

        /// <summary>
        /// Gets the length of the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint Length
        {
            get
            {
                return m_Length;
            }
        }

        /// <summary>
        /// Gets the table buffer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] Buffer
        {
            get
            {
                return m_Buffer;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Table ID</summary>
        protected ushort m_TableID;
        /// <summary>The length of the table</summary>
        protected uint m_Length;
        /// <summary>The table buffer</summary>
        protected byte[] m_Buffer;

        #endregion
    }

    /// <summary>
    /// COSEM Table Cell Definition
    /// </summary>
    public class COSEMTableCellDefinition
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMTableCellDefinition()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Logical Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Group E Values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<byte> GroupEValues
        {
            get
            {
                return m_GroupEValues;
            }
            set
            {
                m_GroupEValues = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte AttributeIndex
        {
            get
            {
                return m_AttributeIndex;
            }
            set
            {
                m_AttributeIndex = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private List<byte> m_GroupEValues;
        private sbyte m_AttributeIndex;

        #endregion
    }

    /// <summary>
    /// COSEM Register Table Interface Class
    /// </summary>
    public class COSEMRegisterTableInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMRegisterTableInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 61;
            m_Version = 0;
        }

        /// <summary>
        /// Resets the data in the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void Reset()
        {
        }

        /// <summary>
        /// Copies the values of the attributes into the table cell values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void Capture()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the register values in the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<object> TableCellValues
        {
            get
            {
                return m_TableCellValues;
            }
        }

        /// <summary>
        /// Gets the table cell defintion
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMTableCellDefinition TableCellDefinition
        {
            get
            {
                return m_TableCellDefinition;
            }
        }

        /// <summary>
        /// Gets the scalar units for the register table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMUnits ScalarUnits
        {
            get
            {
                return m_ScalarUnits;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Table Cell Values</summary>
        protected List<object> m_TableCellValues;
        /// <summary>Table Cell Definition</summary>
        protected COSEMTableCellDefinition m_TableCellDefinition;
        /// <summary>Scalar Units for the data</summary>
        protected COSEMUnits m_ScalarUnits;

        #endregion
    }

    /// <summary>
    /// COSEM Mapping Table object
    /// </summary>
    public class COSEMMappingTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMMappingTable()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Reference Table ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte ReferenceTableID
        {
            get
            {
                return m_ReferenceTableID;
            }
            set
            {
                m_ReferenceTableID = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of table entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<ushort> Entries
        {
            get
            {
                return m_Entries;
            }
            set
            {
                m_Entries = value;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The identifier of the reference status table</summary>
        protected byte m_ReferenceTableID;
        /// <summary></summary>
        protected List<ushort> m_Entries;

        #endregion
    }

    /// <summary>
    /// COSEM Status Mapping Interface Class
    /// </summary>
    public class COSEMStatusMappingInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMStatusMappingInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 63;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current status word
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public object StatusWord
        {
            get
            {
                return m_StatusWord;
            }
        }

        /// <summary>
        /// Gets the Mapping Table for the status word
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMMappingTable MappingTable
        {
            get
            {
                return m_MappingTable;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The status word</summary>
        protected object m_StatusWord;
        /// <summary>The mapping of the status word to the positions in the reference table</summary>
        protected COSEMMappingTable m_MappingTable;

        #endregion
    }

    /// <summary>
    /// Access Rights Element used in short name association
    /// </summary>
    public class COSEMAccessRight
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAccessRight()
        {
            m_AttributeAccess = new List<COSEMAttributeAccessItem>();
            m_MethodAccess = new List<COSEMMethodAccessItem>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Access Right</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMAccessRight(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 2)
                    {
                        // Parse the list of Attribute Access Items
                        if (StructureData[0].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] AttributeAccessData = StructureData[0].Value as COSEMData[];

                            m_AttributeAccess = new List<COSEMAttributeAccessItem>();

                            foreach (COSEMData CurrentAccessData in AttributeAccessData)
                            {
                                if (CurrentAccessData.DataType == COSEMDataTypes.Structure)
                                {
                                    COSEMAttributeAccessItem NewAccessItem = new COSEMAttributeAccessItem(CurrentAccessData);
                                    m_AttributeAccess.Add(NewAccessItem);
                                }
                                else
                                {
                                    throw new ArgumentException("The Attribute Access Item data type is incorrect.", "data");
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Attribute Access List data type is incorrect.", "data");
                        }

                        // Parse the list of Method Access Items
                        if (StructureData[1].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] MethodAccessData = StructureData[1].Value as COSEMData[];

                            m_MethodAccess = new List<COSEMMethodAccessItem>();

                            foreach (COSEMData CurrentAccessData in MethodAccessData)
                            {
                                if (CurrentAccessData.DataType == COSEMDataTypes.Structure)
                                {
                                    COSEMMethodAccessItem NewAccessItem = new COSEMMethodAccessItem(CurrentAccessData);
                                    m_MethodAccess.Add(NewAccessItem);
                                }
                                else
                                {
                                    throw new ArgumentException("The Method Access Item data type is incorrect.", "data");
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Method Access List data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Method Right element structure is not of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        ///  Converts the Access Right to a COSEMData object
        /// </summary>
        /// <returns>The COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataObject = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];
            COSEMData[] AttributeAccess = new COSEMData[m_AttributeAccess.Count];
            COSEMData[] MethodAccess = new COSEMData[m_MethodAccess.Count];

            for (int iIndex = 0; iIndex < m_AttributeAccess.Count; iIndex++)
            {
                AttributeAccess[iIndex] = m_AttributeAccess[iIndex].ToCOSEMData();
            }

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.Array;
            StructureData[0].Value = AttributeAccess;

            for (int iIndex = 0; iIndex < m_MethodAccess.Count; iIndex++)
            {
                MethodAccess[iIndex] = m_MethodAccess[iIndex].ToCOSEMData();
            }

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Array;
            StructureData[1].Value = MethodAccess;

            DataObject.DataType = COSEMDataTypes.Structure;
            DataObject.Value = StructureData;

            return DataObject;
        }

        /// <summary>
        /// Gets the definition of the Structure
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Object List Element");
            ArrayObjectDefinition CurrentDefinition;

            CurrentDefinition = new ArrayObjectDefinition("Attributes", COSEMAttributeAccessItem.GetStructureDefinition());
            CurrentDefinition.Value = COSEMAttributeAccessItem.GetStructureDefinition();
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ArrayObjectDefinition("Methods", COSEMMethodAccessItem.GetStructureDefinition());
            CurrentDefinition.Value = COSEMMethodAccessItem.GetStructureDefinition();
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();
            ArrayObjectDefinition ArrayDefinition;

            // Attributes
            ArrayDefinition = Definition.StructureDefinition[0] as ArrayObjectDefinition;

            if (ArrayDefinition != null)
            {
                ArrayDefinition.Elements.Clear();

                foreach (COSEMAttributeAccessItem CurrentElement in m_AttributeAccess)
                {
                    ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                }
            }

            // Methods
            ArrayDefinition = Definition.StructureDefinition[1] as ArrayObjectDefinition;

            if (ArrayDefinition != null)
            {
                ArrayDefinition.Elements.Clear();

                foreach (COSEMMethodAccessItem CurrentElement in m_MethodAccess)
                {
                    ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                }
            }

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        /// <summary>
        /// Gets whether or not a specific attribute can be read
        /// </summary>
        /// <param name="attributeID">The ID of the attribute to check</param>
        /// <returns>True if the attribute is supported. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/23/13 RCG 2.85.00 N/A    Created

        public bool IsAttributeReadable(sbyte attributeID)
        {
            bool Readable = false;

            if (m_AttributeAccess != null && m_AttributeAccess.Where(a => a.AttributeID == attributeID).Count() > 0)
            {
                COSEMAttributeAccessItem AttributeAccess = m_AttributeAccess.Where(a => a.AttributeID == attributeID).First();

                Readable = AttributeAccess.AccessMode == COSEMAttributeAccessMode.ReadOnly || AttributeAccess.AccessMode == COSEMAttributeAccessMode.ReadAndWrite
                    || AttributeAccess.AccessMode == COSEMAttributeAccessMode.AuthenticatedReadOnly || AttributeAccess.AccessMode == COSEMAttributeAccessMode.AuthenticatedReadAndWrite;
            }            

            return Readable;
        }

        /// <summary>
        /// Gets whether or not a specific attribute can be written
        /// </summary>
        /// <param name="attributeID">The ID of the attribute to check</param>
        /// <returns>True if the attribute is supported. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/23/13 RCG 2.85.00 N/A    Created

        public bool IsAttributeWriteable(sbyte attributeID)
        {
            bool Writeable = false;

            if (m_AttributeAccess != null && m_AttributeAccess.Where(a => a.AttributeID == attributeID).Count() > 0)
            {
                COSEMAttributeAccessItem AttributeAccess = m_AttributeAccess.Where(a => a.AttributeID == attributeID).First();

                Writeable = AttributeAccess.AccessMode == COSEMAttributeAccessMode.WriteOnly || AttributeAccess.AccessMode == COSEMAttributeAccessMode.ReadAndWrite
                    || AttributeAccess.AccessMode == COSEMAttributeAccessMode.AuthenticatedWriteOnly || AttributeAccess.AccessMode == COSEMAttributeAccessMode.AuthenticatedReadAndWrite;
            }

            return Writeable;
        }

        /// <summary>
        /// Gets whether or not a specific method is supported
        /// </summary>
        /// <param name="methodID">The ID of the method to check</param>
        /// <returns>True if the method is supported. False otherwise</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/23/13 RCG 2.85.00 N/A    Created

        public bool IsMethodSupported(sbyte methodID)
        {
            bool Supported = false;

            if (m_MethodAccess != null && m_MethodAccess.Where(m => m.MethodID == methodID).Count() > 0)
            {
                COSEMMethodAccessItem MethodAccess = m_MethodAccess.Where(m => m.MethodID == methodID).First();

                Supported = MethodAccess.AccessMode != COSEMMethodAccessMode.NoAccess;
            }

            return Supported;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the attribute access list
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<COSEMAttributeAccessItem> AttributeAccess
        {
            get
            {
                return m_AttributeAccess;
            }
            set
            {
                m_AttributeAccess = value;
            }
        }

        /// <summary>
        /// Gets or sets the method access list
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<COSEMMethodAccessItem> MethodAccess
        {
            get
            {
                return m_MethodAccess;
            }
            set
            {
                m_MethodAccess = value;
            }
        }

        #endregion

        #region Member Variables

        private List<COSEMAttributeAccessItem> m_AttributeAccess;
        private List<COSEMMethodAccessItem> m_MethodAccess;

        #endregion
    }

    /// <summary>
    /// Attribute Access Item for short name association
    /// </summary>
    public class COSEMAttributeAccessItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMAttributeAccessItem()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMAttributeAccessItem(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 3)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Integer)
                        {
                            m_AttributeID = (sbyte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Attribute ID data type is incorrect.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Enum)
                        {
                            m_AccessMode = (COSEMAttributeAccessMode)(byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Access Mode data type is incorrect.", "data");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.NullData)
                        {
                            m_AccessSelectors = null;
                        }
                        else if (StructureData[2].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] AccessSelectors = StructureData[2].Value as COSEMData[];
                            m_AccessSelectors = new List<sbyte>();

                            foreach (COSEMData CurrentSelector in AccessSelectors)
                            {
                                if (CurrentSelector.DataType == COSEMDataTypes.Integer)
                                {
                                    m_AccessSelectors.Add((sbyte)CurrentSelector.Value);
                                }
                                else
                                {
                                    throw new ArgumentException("The Access Selector element data type is incorrect.", "data");
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Access Selectors data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Attribute Access Item structure is not of length 3.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        ///  Converts the Access Descriptor to a COSEMData object
        /// </summary>
        /// <returns>The COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataObject = new COSEMData();
            COSEMData[] StructureData = new COSEMData[3];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.Integer;
            StructureData[0].Value = m_AttributeID;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Enum;
            StructureData[1].Value = (byte)m_AccessMode;

            StructureData[2] = new COSEMData();

            if (m_AccessSelectors == null)
            {
                StructureData[2].DataType = COSEMDataTypes.NullData;
                StructureData[2].Value = null;
            }
            else
            {
                COSEMData[] AccessSelectors = new COSEMData[m_AccessSelectors.Count];

                for(int iIndex = 0; iIndex < m_AccessSelectors.Count; iIndex++)
                {
                    AccessSelectors[iIndex] = new COSEMData();
                    AccessSelectors[iIndex].DataType = COSEMDataTypes.Integer;
                    AccessSelectors[iIndex].Value = m_AccessSelectors[iIndex];
                }

                StructureData[2].DataType = COSEMDataTypes.Array;
                StructureData[2].Value = AccessSelectors;
            }

            DataObject.DataType = COSEMDataTypes.Structure;
            DataObject.Value = StructureData;

            return DataObject;
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Attribute Access Item");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Attribute ID", COSEMDataTypes.Integer);
            CurrentDefinition.Value = (sbyte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new EnumObjectDefinition("Access Mode", typeof(COSEMAttributeAccessMode));
            CurrentDefinition.Value = COSEMAttributeAccessMode.NoAccess;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ArrayObjectDefinition("Access Selectors", new ObjectDefinition("Selector", COSEMDataTypes.Integer));
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();
            ArrayObjectDefinition ArrayDefinition;

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_AttributeID;
            Definition.StructureDefinition[1].Value = m_AccessMode;
            
            ArrayDefinition = Definition.StructureDefinition[2] as ArrayObjectDefinition;

            if (ArrayDefinition != null && m_AccessSelectors != null)
            {
                ArrayDefinition.Elements.Clear();

                for (int iIndex = 0; iIndex < m_AccessSelectors.Count; iIndex++)
                {
                    ObjectDefinition CurrentValueDefinition = new ObjectDefinition("[" + iIndex.ToString(CultureInfo.InvariantCulture) + "]", COSEMDataTypes.Integer);
                    CurrentValueDefinition.Value = m_AccessSelectors[iIndex];

                    ArrayDefinition.Elements.Add(CurrentValueDefinition);
                }
            }

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the attribute ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public sbyte AttributeID
        {
            get
            {
                return m_AttributeID;
            }
            set
            {
                m_AttributeID = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute access mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMAttributeAccessMode AccessMode
        {
            get
            {
                return m_AccessMode;
            }
            set
            {
                m_AccessMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Access Selectors
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<sbyte> AccessSelectors
        {
            get
            {
                return m_AccessSelectors;
            }
            set
            {
                m_AccessSelectors = value;
            }
        }

        #endregion

        #region Member Variables

        private sbyte m_AttributeID;
        private COSEMAttributeAccessMode m_AccessMode;
        private List<sbyte> m_AccessSelectors;

        #endregion
    }

    /// <summary>
    /// Method Access Item for short name association
    /// </summary>
    public class COSEMMethodAccessItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMMethodAccessItem()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data containing the Method Access Item</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMMethodAccessItem(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 2)
                    {
                        // Get the Method ID
                        if (StructureData[0].DataType == COSEMDataTypes.Integer)
                        {
                            m_MethodID = (sbyte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Method ID data type is incorrect.", "data");
                        }

                        // Get the Access Mode
                        if (StructureData[1].DataType == COSEMDataTypes.Enum)
                        {
                            m_AccessMode = (COSEMMethodAccessMode)(byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Access Mode data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Method Access Item structure is not of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        ///  Converts the Access Descriptor to a COSEMData object
        /// </summary>
        /// <returns>The COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMData ToCOSEMData()
        {
            COSEMData DataObject = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.Integer;
            StructureData[0].Value = m_MethodID;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Enum;
            StructureData[1].Value = (byte)m_AccessMode;

            DataObject.DataType = COSEMDataTypes.Structure;
            DataObject.Value = StructureData;

            return DataObject;
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Attribute Access Item");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Method ID", COSEMDataTypes.Integer);
            CurrentDefinition.Value = (sbyte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new EnumObjectDefinition("Access Mode", typeof(COSEMMethodAccessMode));
            CurrentDefinition.Value = COSEMMethodAccessMode.NoAccess;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_MethodID;
            Definition.StructureDefinition[1].Value = m_AccessMode;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the method ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public sbyte MethodID
        {
            get
            {
                return m_MethodID;
            }
            set
            {
                m_MethodID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Access Mode
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMMethodAccessMode AccessMode
        {
            get
            {
                return m_AccessMode;
            }
            set
            {
                m_AccessMode = value;
            }
        }

        #endregion

        #region Member Variables

        private sbyte m_MethodID;
        private COSEMMethodAccessMode m_AccessMode;

        #endregion
    }

    /// <summary>
    /// Object used when reading by logical name
    /// </summary>
    public class COSEMAttributeIdentification
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMAttributeIdentification()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the logical name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Attribute Index which indicates the number of matching items to retrieve. (0 means all)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public sbyte AttributeIndex
        {
            get
            {
                return m_AttributeIndex;
            }
            set
            {
                m_AttributeIndex = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private sbyte m_AttributeIndex;

        #endregion
    }

    /// <summary>
    /// Association Object List Element
    /// </summary>
    public class COSEMLongNameObjectListElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMLongNameObjectListElement()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the data for the LN Object list element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMLongNameObjectListElement(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] DataList = data.Value as COSEMData[];

                    // The data is a structure so it should always have 4 elements
                    if (DataList.Length == 4)
                    {
                        if (DataList[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ClassID = (ushort)DataList[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Class ID data type is incorrect.", "data");
                        }

                        if (DataList[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_Version = (byte)DataList[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Version data type is incorrect.", "data");
                        }

                        if (DataList[2].DataType == COSEMDataTypes.OctetString)
                        {
                            m_LogicalName = (byte[])DataList[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Logical Name data type is incorrect.", "data");
                        }

                        if (DataList[3].DataType == COSEMDataTypes.Structure)
                        {
                            m_AccessRight = new COSEMAccessRight(DataList[3]);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The object list element structure is not of length 4.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        ///  Converts the Object List Element to a COSEMData object
        /// </summary>
        /// <returns>The COSEM Data object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMData ToCOSEMData()
        {
            COSEMData DataObject = new COSEMData();
            COSEMData[] StructureData = new COSEMData[4];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.LongUnsigned;
            StructureData[0].Value = m_ClassID;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.Unsigned;
            StructureData[1].Value = m_Version;

            StructureData[2] = new COSEMData();
            StructureData[2].DataType = COSEMDataTypes.OctetString;
            StructureData[2].Value = m_LogicalName;

            StructureData[3] = m_AccessRight.ToCOSEMData();

            DataObject.DataType = COSEMDataTypes.Structure;
            DataObject.Value = StructureData;

            return DataObject;
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Object List Element");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Class ID", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Version", COSEMDataTypes.Unsigned);
            CurrentDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Logical Name", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ArrayObjectDefinition("Access Rights", COSEMAccessRight.GetStructureDefinition());
            CurrentDefinition.Value = COSEMAccessRight.GetStructureDefinition();
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ClassID;
            Definition.StructureDefinition[1].Value = m_Version;
            Definition.StructureDefinition[2].Value = m_LogicalName;
            Definition.StructureDefinition[3] = m_AccessRight.ToObjectDefinition();

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte Version
        {
            get
            {
                return m_Version;
            }
            set
            {
                m_Version = value;
            }
        }

        /// <summary>
        /// Gets or sets the logical name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the access rights
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAccessRight AccessRight
        {
            get
            {
                return m_AccessRight;
            }
            set
            {
                m_AccessRight = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte m_Version;
        private byte[] m_LogicalName;
        private COSEMAccessRight m_AccessRight;

        #endregion
    }

    /// <summary>
    /// COSEM Associated Partners
    /// </summary>
    public class COSEMAssociatedPartnersType
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAssociatedPartnersType()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the data for the Associated Partners Type</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMAssociatedPartnersType(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 2)
                    {
                        // Get the Client SAP
                        if (StructureData[0].DataType == COSEMDataTypes.Integer)
                        {
                            m_ClientSAP = (sbyte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Client SAP data type is incorrect.", "data");
                        }

                        // Get the Server SAP
                        if (StructureData[1].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ServerSAP = (ushort)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Server SAP data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Associated Partners ID structure is not of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Associated Partners");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Client SAP", COSEMDataTypes.Integer);
            CurrentDefinition.Value = (sbyte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Server SAP", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ClientSAP;
            Definition.StructureDefinition[1].Value = m_ServerSAP;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Client SAP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte ClientSAP
        {
            get
            {
                return m_ClientSAP;
            }
            set
            {
                m_ClientSAP = value;
            }
        }

        /// <summary>
        /// Gets or sets the Server SAP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ServerSAP
        {
            get
            {
                return m_ServerSAP;
            }
            set
            {
                m_ServerSAP = value;
            }
        }

        #endregion

        #region Member Variables

        private sbyte m_ClientSAP;
        private ushort m_ServerSAP;

        #endregion
    }

    /// <summary>
    /// xDLMS context for an association
    /// </summary>
    public class COSEMxDLMSContextInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMxDLMSContextInfo()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the data for the LN Object list element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMxDLMSContextInfo(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 6)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.BitString)
                        {
                            byte[] BitStringData = (byte[])StructureData[0].Value;
                            m_Conformance = DLMSBinaryReader.ConvertBitStringToEnum<DLMSConformanceFlags>(BitStringData);
                        }
                        else
                        {
                            throw new ArgumentException("The Conformance data type is incorrect.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_MaxReceivePDUSize = (ushort)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Receive PDU Size data type is incorrect.", "data");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_MaxSendPDUSize = (ushort)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Max Send PDU Size data type is incorrect.", "data");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DLMSVersionNumber = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DLMS Version data type is incorrect.", "data");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.Integer)
                        {
                            m_QualityOfService = (sbyte)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Quality of Service data type is incorrect.", "data");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.OctetString)
                        {
                            m_CypheringInfo = (byte[])StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Cyphering Data data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Associated Partners ID structure is not of length 6.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the xDLMS conformance block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DLMSConformanceFlags Conformance
        {
            get
            {
                return m_Conformance;
            }
            set
            {
                m_Conformance = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum length for an APDU that may be received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort MaxReceivePDUSize
        {
            get
            {
                return m_MaxReceivePDUSize;
            }
            set
            {
                m_MaxReceivePDUSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum length for an APDU that may be sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort MaxSendPDUSize
        {
            get
            {
                return m_MaxSendPDUSize;
            }
            set
            {
                m_MaxSendPDUSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the DLSM Version Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte DLMSVersionNumber
        {
            get
            {
                return m_DLMSVersionNumber;
            }
            set
            {
                m_DLMSVersionNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Quality of Service
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte QualityOfService
        {
            get
            {
                return m_QualityOfService;
            }
            set
            {
                m_QualityOfService = value;
            }
        }

        /// <summary>
        /// Gets or sets the key used in the xDLMS initiate request APDU
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] CypheringInfo
        {
            get
            {
                return m_CypheringInfo;
            }
            set
            {
                m_CypheringInfo = value;
            }
        }

        #endregion

        #region Member Variables

        private DLMSConformanceFlags m_Conformance;
        private ushort m_MaxReceivePDUSize;
        private ushort m_MaxSendPDUSize;
        private byte m_DLMSVersionNumber;
        private sbyte m_QualityOfService;
        private byte[] m_CypheringInfo;

        #endregion
    }

    /// <summary>
    /// The COSEM Application Context
    /// </summary>
    public class COSEMApplicationContextName
    {
        #region Constants

        private const byte LSB_SEVEN = 0x7F;
        private const byte MSB = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMApplicationContextName()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the data for the Application Context Name</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMApplicationContextName(COSEMData data)
        {
            if (data != null)
            {
                // The Context Name can be returned as a structure or as a byte array
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 7)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_JointISOCTTElement = (byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Joint ISO CTT Element data type is incorrect.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_CountryElement = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Country Element data type is incorrect.", "data");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_CountryNameElement = (ushort)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Country Name Element data type is incorrect.", "data");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_IdentifiedOrganizationElement = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Identified Organization Element data type is incorrect.", "data");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DLMSUAElement = (byte)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DLMS UA Element data type is incorrect.", "data");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_ApplicationContextElement = (byte)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Application Context Element data type is incorrect.", "data");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_ContextIDElement = (byte)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Context ID Element data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Application Context Name structure is not of length 7.", "data");
                    }
                }
                else if (data.DataType == COSEMDataTypes.OctetString)
                {
                    // It's stored as a byte[] but we can get the same data by parsing it out
                    byte[] ContextNameData = (byte[])data.Value;

                    if (ContextNameData.Length > 0)
                    {
                        MemoryStream DataStream = new MemoryStream(ContextNameData);
                        DLMSBinaryReader DataReader = new DLMSBinaryReader(DataStream);

                        // The first byte is a combination of the first to parts of the ID
                        byte CurrentByte = DataReader.ReadByte();

                        if (CurrentByte <= 39)
                        {
                            m_JointISOCTTElement = 0;
                            m_CountryElement = CurrentByte;
                        }
                        else if (CurrentByte <= 79)
                        {
                            m_JointISOCTTElement = 1;
                            m_CountryElement = (byte)(CurrentByte - 40);
                        }
                        else
                        {
                            m_JointISOCTTElement = 2;
                            m_CountryElement = (byte)(CurrentByte - 80);
                        }


                        m_CountryNameElement = (ushort)(((DataReader.ReadByte() & LSB_SEVEN) << 7) | (DataReader.ReadByte() & LSB_SEVEN));
                        m_IdentifiedOrganizationElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_DLMSUAElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_ApplicationContextElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_ContextIDElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Joint ISO-CTT Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte JointISOCTTElement
        {
            get
            {
                return m_JointISOCTTElement;
            }
            set
            {
                m_JointISOCTTElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the country element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte CountryElement
        {
            get
            {
                return m_CountryElement;
            }
            set
            {
                m_CountryElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the country name element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort CountryNameElement
        {
            get
            {
                return m_CountryNameElement;
            }
            set
            {
                m_CountryNameElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the identified organization element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte IdentifiedOrganizationElement
        {
            get
            {
                return m_IdentifiedOrganizationElement;
            }
            set
            {
                m_IdentifiedOrganizationElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the DLMS UA Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte DLMSUAElement
        {
            get
            {
                return m_DLMSUAElement;
            }
            set
            {
                m_DLMSUAElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the Application Context Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte ApplicationContextElement
        {
            get
            {
                return m_ApplicationContextElement;
            }
            set
            {
                m_ApplicationContextElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the Context ID Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte ContextIDElement
        {
            get
            {
                return m_ContextIDElement;
            }
            set
            {
                m_ContextIDElement = value;
            }
        }

        /// <summary>
        /// Gets the Context Name as an Octet String
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] OctetString
        {
            get
            {
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.Write((byte)(40 * m_JointISOCTTElement + m_CountryElement));
                DataWriter.Write((byte)(m_CountryNameElement >> 7 | MSB));
                DataWriter.Write((byte)(m_CountryNameElement & LSB_SEVEN));
                DataWriter.Write(m_IdentifiedOrganizationElement);
                DataWriter.Write(m_DLMSUAElement);
                DataWriter.Write(m_ApplicationContextElement);
                DataWriter.Write(m_ContextIDElement);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private byte m_JointISOCTTElement;
        private byte m_CountryElement;
        private ushort m_CountryNameElement;
        private byte m_IdentifiedOrganizationElement;
        private byte m_DLMSUAElement;
        private byte m_ApplicationContextElement;
        private byte m_ContextIDElement;

        #endregion
    }

    /// <summary>
    /// The Authentication Mechanism
    /// </summary>
    public class COSEMAuthenticationMechanismName
    {
        #region Constants

        private const byte LSB_SEVEN = 0x7F;
        private const byte MSB = 0x80;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAuthenticationMechanismName()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the data for the Authentication Mechanism Name</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        public COSEMAuthenticationMechanismName(COSEMData data)
        {
            if (data != null)
            {
                // The Context Name can be returned as a structure or as a byte array
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 7)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_JointISOCTTElement = (byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Joint ISO CTT Element data type is incorrect.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_CountryElement = (byte)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Country Element data type is incorrect.", "data");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_CountryNameElement = (ushort)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Country Name Element data type is incorrect.", "data");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_IdentifiedOrganizationElement = (byte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Identified Organization Element data type is incorrect.", "data");
                        }

                        if (StructureData[4].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_DLMSUAElement = (byte)StructureData[4].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The DLMS UA Element data type is incorrect.", "data");
                        }

                        if (StructureData[5].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_AuthenticationMechanismNameElement = (byte)StructureData[5].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Authentication Mechanism Name Element data type is incorrect.", "data");
                        }

                        if (StructureData[6].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_MechanismIDElement = (byte)StructureData[6].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Mechanism ID Element data type is incorrect.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Authentication Name structure is not of length 7.", "data");
                    }
                }
                else if (data.DataType == COSEMDataTypes.OctetString)
                {
                    // It's stored as a byte[] but we can get the same data by parsing it out
                    byte[] ContextNameData = (byte[])data.Value;

                    if (ContextNameData.Length > 0)
                    {
                        MemoryStream DataStream = new MemoryStream(ContextNameData);
                        DLMSBinaryReader DataReader = new DLMSBinaryReader(DataStream);

                        // The first byte is a combination of the first to parts of the ID
                        byte CurrentByte = DataReader.ReadByte();

                        if (CurrentByte <= 39)
                        {
                            m_JointISOCTTElement = 0;
                            m_CountryElement = CurrentByte;
                        }
                        else if (CurrentByte <= 79)
                        {
                            m_JointISOCTTElement = 1;
                            m_CountryElement = (byte)(CurrentByte - 40);
                        }
                        else
                        {
                            m_JointISOCTTElement = 2;
                            m_CountryElement = (byte)(CurrentByte - 80);
                        }


                        m_CountryNameElement = (ushort)(((DataReader.ReadByte() & LSB_SEVEN) << 7) | (DataReader.ReadByte() & LSB_SEVEN));
                        m_IdentifiedOrganizationElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_DLMSUAElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_AuthenticationMechanismNameElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                        m_MechanismIDElement = (byte)(DataReader.ReadByte() & LSB_SEVEN);
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Joint ISO-CTT Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte JointISOCTTElement
        {
            get
            {
                return m_JointISOCTTElement;
            }
            set
            {
                m_JointISOCTTElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the country element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte CountryElement
        {
            get
            {
                return m_CountryElement;
            }
            set
            {
                m_CountryElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the country name element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort CountryNameElement
        {
            get
            {
                return m_CountryNameElement;
            }
            set
            {
                m_CountryNameElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the identified organization element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte IdentifiedOrganizationElement
        {
            get
            {
                return m_IdentifiedOrganizationElement;
            }
            set
            {
                m_IdentifiedOrganizationElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the DLMS UA Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte DLMSUAElement
        {
            get
            {
                return m_DLMSUAElement;
            }
            set
            {
                m_DLMSUAElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the Authentication Mechanism Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte AuthenticationMechanismNameElement
        {
            get
            {
                return m_AuthenticationMechanismNameElement;
            }
            set
            {
                m_AuthenticationMechanismNameElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the Mechanism ID Element
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte MechanismIDElement
        {
            get
            {
                return m_MechanismIDElement;
            }
            set
            {
                m_MechanismIDElement = value;
            }
        }

        /// <summary>
        /// Gets the Context Name as an Octet String
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] OctetString
        {
            get
            {
                MemoryStream DataStream = new MemoryStream();
                DLMSBinaryWriter DataWriter = new DLMSBinaryWriter(DataStream);

                DataWriter.Write((byte)(40 * m_JointISOCTTElement + m_CountryElement));
                DataWriter.Write((byte)(m_CountryNameElement >> 7 | MSB));
                DataWriter.Write((byte)(m_CountryNameElement & LSB_SEVEN));
                DataWriter.Write(m_IdentifiedOrganizationElement);
                DataWriter.Write(m_DLMSUAElement);
                DataWriter.Write(m_AuthenticationMechanismNameElement);
                DataWriter.Write(m_MechanismIDElement);

                return DataStream.ToArray();
            }
        }

        #endregion

        #region Member Variables

        private byte m_JointISOCTTElement;
        private byte m_CountryElement;
        private ushort m_CountryNameElement;
        private byte m_IdentifiedOrganizationElement;
        private byte m_DLMSUAElement;
        private byte m_AuthenticationMechanismNameElement;
        private byte m_MechanismIDElement;

        #endregion
    }

    /// <summary>
    /// Interface class used for associating using long names
    /// </summary>
    public class COSEMAssociateLongNameInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the current association
        /// </summary>
        public static readonly byte[] CURRENT_ASSOCIATION_LN = new byte[] {0, 0, 40, 0, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the current association
        /// </summary>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMAssociateLongNameInterfaceClass(DLMSProtocol dlms)
            : this(CURRENT_ASSOCIATION_LN, dlms)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMAssociateLongNameInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 15;
            m_Version = 1;
        }

        /// <summary>
        /// Reply to an HLS security challenge
        /// </summary>
        /// <param name="reply">The response to the challenge</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ReplyToHLSAuthentication(byte[] reply)
        {
            byte[] Result = null;

            if (m_DLMS.IsConnected)
            {
                DLMSEncryptionModes OriginalEncryptionMode = m_DLMS.CurrentEncryptionMode;
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.OctetString;
                ParameterData.Value = reply;

                // We need to send the reply using global encryption
                m_DLMS.CurrentEncryptionMode = DLMSEncryptionModes.Global;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                if (Response.Result == ActionResults.Success && Response.ReturnParameters.GetDataResultType == GetDataResultChoices.Data
                    && Response.ReturnParameters.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    Result = Response.ReturnParameters.DataValue.Value as byte[];
                }

                m_DLMS.CurrentEncryptionMode = OriginalEncryptionMode;
            }

            return Result;
        }

        /// <summary>
        /// Change the secret used of authentication
        /// </summary>
        /// <param name="secret">The new secret</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ChangeHLSSecret(byte[] secret)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.OctetString;
                ParameterData.Value = secret;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Adds the specified object to the list
        /// </summary>
        /// <param name="newObject">The object to add</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults AddObject(COSEMLongNameObjectListElement newObject)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = newObject.ToCOSEMData();

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 3, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Removes the specified object
        /// </summary>
        /// <param name="objectToRemove">The object to remove.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults RemoveObject(COSEMLongNameObjectListElement objectToRemove)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = objectToRemove.ToCOSEMData();

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 4, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Object List", COSEMLongNameObjectListElement.GetStructureDefinition());
                    List<COSEMLongNameObjectListElement> CurrentObjectList = ObjectList;

                    foreach (COSEMLongNameObjectListElement CurrentElement in CurrentObjectList)
                    {
                        ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                    }

                    AttributeDefinition = ArrayDefinition;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = AssociatedPartnersID.ToObjectDefinition();
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new ObjectDefinition("Application Context Name", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = ApplicationContextName.OctetString;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new ObjectDefinition("Authentication Mechanism Name", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = AuthenticationMechanismName.OctetString;
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("Secret", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = Secret;
                    break;
                }
                case 8:
                {
                    AttributeDefinition = new EnumObjectDefinition("Association Status", typeof(COSEMAssociationStatus));
                    AttributeDefinition.Value = AssociationStatus;
                    break;
                }
                case 9:
                {
                    AttributeDefinition = new ObjectDefinition("Security Setup Reference", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = SecuritySetupReference;
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
                    Name = "Object List";
                    break;
                }
                case 3:
                {
                    Name = "Associated Partners";
                    break;
                }
                case 4:
                {
                    Name = "Application Context Name";
                    break;
                }
                case 6:
                {
                    Name = "Authentication Mechanism Name";
                    break;
                }
                case 7:
                {
                    Name = "Secret";
                    break;
                }
                case 8:
                {
                    Name = "Association Status";
                    break;
                }
                case 9:
                {
                    Name = "Security Setup Reference";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).ToList();
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
            return (new sbyte[] { 1, 2, 3, 4 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the object list from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        private void ParseObjectList(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                // The data should be an array of COSEM Data objects
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    m_ObjectList = new List<COSEMLongNameObjectListElement>();

                    COSEMData[] DataObjects = data.DataValue.Value as COSEMData[];

                    foreach (COSEMData CurrentDataObject in DataObjects)
                    {
                        try
                        {
                            COSEMLongNameObjectListElement NewElement = new COSEMLongNameObjectListElement(CurrentDataObject);

                            m_ObjectList.Add(NewElement);
                        }
                        catch (Exception e)
                        {
                            m_ObjectList = null;
                            WriteToLog("Failed to Get the Object List - Exception while parsing Object List. Message: " + e.Message);
                        }
                    }
                }
                else
                {
                    m_ObjectList = null;
                    WriteToLog("Failed to Get the Object List - The data received is not an array.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ObjectList = null;
                WriteToLog("Failed to Get the Object List - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the associated partners ID from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseAssociatedPartnersID(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_AssociatedPartnersID = new COSEMAssociatedPartnersType(data.DataValue);
                }
                catch (Exception e)
                {
                    m_AssociatedPartnersID = null;
                    WriteToLog("Failed to Get the Associated Partners ID - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_AssociatedPartnersID = null;
                WriteToLog("Failed to Get the Associated Partners ID - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Application Context Name from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseApplicationContextName(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_ApplicationContextName = new COSEMApplicationContextName(data.DataValue);
                }
                catch (Exception e)
                {
                    m_ApplicationContextName = null;
                    WriteToLog("Failed to Get the Application Context Name - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ApplicationContextName = null;
                WriteToLog("Failed to Get the Application Context Name - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the xDLMS Context Info from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParsexDLMSContextInfo(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_xDLMSContextInfo = new COSEMxDLMSContextInfo(data.DataValue);
                }
                catch (Exception e)
                {
                    m_xDLMSContextInfo = null;
                    WriteToLog("Failed to Get the xDLMS Context Info - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_xDLMSContextInfo = null;
                WriteToLog("Failed to Get the xDLMS Context Info - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the authentication mechanism name from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseAuthenticationMechanismName(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                try
                {
                    m_AuthenticationMechanismName = new COSEMAuthenticationMechanismName(data.DataValue);
                }
                catch (Exception e)
                {
                    m_AuthenticationMechanismName = null;
                    WriteToLog("Failed to Get the Authentication Mechanism Name - Exception Occurred while parsing the data. Message: " + e.Message);
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_AuthenticationMechanismName = null;
                WriteToLog("Failed to Get the Authentication Mechanism Name - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the secret from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseSecret(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    m_Secret = (byte[])data.DataValue.Value;
                }
                else
                {
                    m_Secret = null;
                    WriteToLog("Failed To Get the Secret - The data type is incorrect.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Secret = null;
                WriteToLog("Failed to Get the Secret - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the association status from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseAssociationStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    m_AssociationStatus = (COSEMAssociationStatus)(byte)data.DataValue.Value;
                }
                else
                {
                    WriteToLog("Failed To Get the Association Status - The data type is incorrect.");
                    throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                WriteToLog("Failed to Get the Association Status - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the security setup reference from the data result
        /// </summary>
        /// <param name="data">The data result</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseSecuritySetupReference(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    m_SecuritySetupReference = (byte[])data.DataValue.Value;
                }
                else
                {
                    m_SecuritySetupReference = null;
                    WriteToLog("Failed To Get the Security Setup Reference - The data type is incorrect.");
                    throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SecuritySetupReference = null;
                WriteToLog("Failed to Get the Security Setup Reference - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of objects supported by the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMLongNameObjectListElement> ObjectList
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseObjectList(Result);
                    }
                }

                return m_ObjectList;
            }
        }

        /// <summary>
        /// Gets the Associated Partners ID info.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAssociatedPartnersType AssociatedPartnersID
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseAssociatedPartnersID(Result);
                    }
                }

                return m_AssociatedPartnersID;
            }
        }

        /// <summary>
        /// Gets the Application Context Name (could be a byte[] or an application context object)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMApplicationContextName ApplicationContextName
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseApplicationContextName(Result);
                    }
                }

                return m_ApplicationContextName;
            }
        }

        /// <summary>
        /// Gets the xDLMS context for the association
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMxDLMSContextInfo xDLMSContextInfo
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    if (Result != null)
                    {
                        ParsexDLMSContextInfo(Result);
                    }
                }

                return m_xDLMSContextInfo;
            }
        }

        /// <summary>
        /// Gets the Authentication Mechanism Name (could be a byte[] or an authentication mechanism object)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAuthenticationMechanismName AuthenticationMechanismName
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    if (Result != null)
                    {
                        ParseAuthenticationMechanismName(Result);
                    }
                }

                return m_AuthenticationMechanismName;
            }
        }

        /// <summary>
        /// Gets the secret used for LLS or HLS authentication
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] Secret
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    if (Result != null)
                    {
                        ParseSecret(Result);
                    }
                }

                return m_Secret;
            }
        }

        /// <summary>
        /// Gets the current Association Status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAssociationStatus AssociationStatus
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 8);

                    if (Result != null)
                    {
                        ParseAssociationStatus(Result);
                    }
                }

                return m_AssociationStatus;
            }
        }

        /// <summary>
        /// Gets the logical name of the security setup object.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] SecuritySetupReference
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 9);

                    if (Result != null)
                    {
                        ParseSecuritySetupReference(Result);
                    }
                }

                return m_SecuritySetupReference;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The list of objects supported by the device</summary>
        protected List<COSEMLongNameObjectListElement> m_ObjectList;
        /// <summary>The Associated Partner IDs</summary>
        protected COSEMAssociatedPartnersType m_AssociatedPartnersID;
        /// <summary>The Application Context Name</summary>
        protected COSEMApplicationContextName m_ApplicationContextName;
        /// <summary>The xDLMS Context Info</summary>
        protected COSEMxDLMSContextInfo m_xDLMSContextInfo;
        /// <summary>The Authentication Machanism Name</summary>
        protected COSEMAuthenticationMechanismName m_AuthenticationMechanismName;
        /// <summary>The Secret used in LLS or HLS authentication</summary>
        protected byte[] m_Secret;
        /// <summary>The current Association Status</summary>
        protected COSEMAssociationStatus m_AssociationStatus;
        /// <summary>The logical name of the Security Setup object</summary>
        protected byte[] m_SecuritySetupReference;

        #endregion
    }

    /// <summary>
    /// SAP Assignment List Element
    /// </summary>
    public class COSEMAssignementListElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMAssignementListElement()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SAP assigned to
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort SAP
        {
            get
            {
                return m_SAP;
            }
            set
            {
                m_SAP = value;
            }
        }

        /// <summary>
        /// Gets or sets the Logical Device Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalDeviceName
        {
            get
            {
                return m_LogicalDeviceName;
            }
            set
            {
                m_LogicalDeviceName = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_SAP;
        private byte[] m_LogicalDeviceName;

        #endregion
    }

    /// <summary>
    /// Provides information on the assignment of the logical devices to their SAP
    /// </summary>
    public class COSEMSAPAssignmentInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMSAPAssignmentInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 17;
            m_Version = 0;
        }

        /// <summary>
        /// Connects a logical device to a SAP
        /// </summary>
        /// <param name="newAssignment">The new connection to add</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void ConnectLogicalDevice(COSEMAssignementListElement newAssignment)
        {
        }

        #endregion

        #region Member Variables

        /// <summary>The SAP Assignment list</summary>
        protected List<COSEMAssignementListElement> m_SAPAssignmentList;

        #endregion
    }

    /// <summary>
    /// Provides information on the images ready for activation
    /// </summary>
    public class COSEMImageToActivateInfoElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMImageToActivateInfoElement()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Activate Info Element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        public COSEMImageToActivateInfoElement(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 3)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.DoubleLongUnsigned)
                        {
                            m_ImageSize = (uint)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Image Size is not the correct data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_ImageIdentification = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Image Identification is not the correct data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.OctetString)
                        {
                            m_ImageSignature = (byte[])StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Image Signature is not the correct data type.");
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
        //  05/07/13 RCG 2.80.26 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Image To Activate Info Element");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Image Size", COSEMDataTypes.DoubleLongUnsigned);
            CurrentDefinition.Value = (uint)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Image Identification", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Image Signature", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.26 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ImageSize;
            Definition.StructureDefinition[1].Value = m_ImageIdentification;
            Definition.StructureDefinition[2].Value = m_ImageSignature;

            // Set the Value field to this instance of the Action Item
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the image size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint ImageSize
        {
            get
            {
                return m_ImageSize;
            }
            set
            {
                m_ImageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the image identification info
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ImageIdentification
        {
            get
            {
                return m_ImageIdentification;
            }
            set
            {
                m_ImageIdentification = value;
            }
        }

        /// <summary>
        /// Gets or sets the Image Signature
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ImageSignature
        {
            get
            {
                return m_ImageSignature;
            }
            set
            {
                m_ImageSignature = value;
            }
        }

        #endregion

        #region Member Variables

        private uint m_ImageSize;
        private byte[] m_ImageIdentification;
        private byte[] m_ImageSignature;

        #endregion
    }

    /// <summary>
    /// Handles the transfer of firmware images to the COSEM device
    /// </summary>
    public class COSEMImageTransferInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMImageTransferInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 18;
            m_Version = 0;
        }

        /// <summary>
        /// Initiates the transfer of an image
        /// </summary>
        /// <param name="imageIdentifier">The image identifier</param>
        /// <param name="imageSize">The size of the image</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ImageTransferInitiate(byte[] imageIdentifier, uint imageSize)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();
                COSEMData[] StructureData = new COSEMData[2];

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Structure;

                StructureData[0] = new COSEMData();
                StructureData[0].DataType = COSEMDataTypes.OctetString;
                StructureData[0].Value = imageIdentifier;

                StructureData[1] = new COSEMData();
                StructureData[1].DataType = COSEMDataTypes.DoubleLongUnsigned;
                StructureData[1].Value = imageSize;

                ParameterData.Value = StructureData;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Transfers a block to the device
        /// </summary>
        /// <param name="imageBlockNumber">The block number being transferred</param>
        /// <param name="imageBlockValue">The block data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ImageBlockTransfer(uint imageBlockNumber, byte[] imageBlockValue)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();
                COSEMData[] StructureData = new COSEMData[2];

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Structure;

                StructureData[0] = new COSEMData();
                StructureData[0].DataType = COSEMDataTypes.DoubleLongUnsigned;
                StructureData[0].Value = imageBlockNumber;

                StructureData[1] = new COSEMData();
                StructureData[1].DataType = COSEMDataTypes.OctetString;
                StructureData[1].Value = imageBlockValue;

                ParameterData.Value = StructureData;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;

        }

        /// <summary>
        /// Verifies the image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ImageVerify()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 3, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Activates the image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ImageActivate()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 4, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Activates the image
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/13/12 RCG 2.85.44 N/A    Created

        public ActionResults Cancel()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, -1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case -2:
                {
                    AttributeDefinition = new ObjectDefinition("Verification Hash", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = VerificationHash;
                    break;
                }
                case -1:
                {
                    AttributeDefinition = new ObjectDefinition("Automatic Verification Check", COSEMDataTypes.Boolean);
                    AttributeDefinition.Value = AutomaticVerificationCheck;
                    break;
                }
                case 2:
                {
                    AttributeDefinition = new ObjectDefinition("Image Block Size", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = ImageBlockSize;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new ObjectDefinition("Image Transfer Blocks Status", COSEMDataTypes.BitString);
                    AttributeDefinition.Value = ImageTransferredBlocksStatus;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new ObjectDefinition("Image First Not Transferred Block Number", COSEMDataTypes.DoubleLongUnsigned);
                    AttributeDefinition.Value = ImageFirstNotTransferredBlockNumber;
                    break;
                }
                case 5:
                {
                    AttributeDefinition = new ObjectDefinition("Image Transfer Enabled", COSEMDataTypes.Boolean);
                    AttributeDefinition.Value = ImageTransferEnabled;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new EnumObjectDefinition("Image Transfer Status", typeof(COSEMImageTransferStatus));
                    AttributeDefinition.Value = ImageTransferStatus;
                    break;
                }
                case 7:
                {
                    ArrayObjectDefinition ArrayObject = new ArrayObjectDefinition("Image To Activate Info", COSEMImageToActivateInfoElement.GetStructureDefinition());
                    List<COSEMImageToActivateInfoElement> CurrentList = ImageToActivateInfo;

                    foreach(COSEMImageToActivateInfoElement CurrentElement in CurrentList)
                    {
                        ArrayObject.Elements.Add(CurrentElement.ToObjectDefinition());
                    }

                    AttributeDefinition = ArrayObject;
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
                case -2:
                {
                    Name = "Verification Hash";
                    break;
                }
                case -1:
                {
                    Name = "Automatic Verification Check";
                    break;
                }
                case 2:
                {
                    Name = "Image Block Size";
                    break;
                }
                case 3:
                {
                    Name = "Image Transfer Blocks Status";
                    break;
                }
                case 4:
                {
                    Name = "Image First Not Transferred Block Number";
                    break;
                }
                case 5:
                {
                    Name = "Image Transfer Enabled";
                    break;
                }
                case 6:
                {
                    Name = "Image Transfer Status";
                    break;
                }
                case 7:
                {
                    Name = "Image To Activate Info";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7 }).ToList();
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
            return (new sbyte[] { -1, 1, 2, 3, 4 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Automatic Verification Check
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created
        
        private void ParseAutomaticVerificationCheck(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Boolean)
                {
                    try
                    {
                        m_AutomaticVerificationCheck = (bool)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_AutomaticVerificationCheck = false;
                        WriteToLog("Failed to Get the Automatic Verification Check - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, -2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_AutomaticVerificationCheck = false;
                    WriteToLog("Failed to parse the Automatic Verification Check - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, -2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_AutomaticVerificationCheck = false;
                WriteToLog("Failed to Get the Automatic Verification Check - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, -2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Verification Hash
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created
        
        private void ParseVerificationHash(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_VerificationHash = data.DataValue.Value as byte[];
                    }
                    catch (Exception e)
                    {
                        WriteToLog("Failed to Get the Verification Has - Exception Occurred while parsing the data. Message: " + e.Message);
                        m_VerificationHash = null;
                        throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_VerificationHash = null;
                    WriteToLog("Failed to parse the Verification Hash - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_VerificationHash = null;
                WriteToLog("Failed to Get the Verification Hash - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, -1, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the image block size
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created
        
        private void ParseImageBlockSize(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_ImageBlockSize = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ImageBlockSize = 0;
                        WriteToLog("Failed to Get the Image Block Size - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageBlockSize = 0;
                    WriteToLog("Failed to parse the Image Block Size - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ImageBlockSize = 0;
                WriteToLog("Failed to Get the Image Block Size - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the image block size
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseImageTansferredBlocksStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.BitString)
                {
                    try
                    {
                        byte[] BitString = (byte[])data.DataValue.Value;

                        // Each bit in the bit string corresponds to the status of a block
                        // To simplify things a bit to our user lets convert this to a list of enums

                        m_ImageTransferredBlocksStatus = new List<COSEMImageBlockStatus>();

                        foreach (byte CurrentByte in BitString)
                        {
                            // The left most bit is the first bit so we need to work backwards
                            for (int BitShift = 7; BitShift > 0; BitShift--)
                            {
                                m_ImageTransferredBlocksStatus.Add((COSEMImageBlockStatus)((CurrentByte >> BitShift) & 0x01));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_ImageTransferredBlocksStatus = null;
                        WriteToLog("Failed to Get the Image Transferred Block Status - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageTransferredBlocksStatus = null;
                    WriteToLog("Failed to parse the Image Transferred Block Status - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ImageTransferredBlocksStatus = null;
                WriteToLog("Failed to Get the Image Transferred Block Status - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the block number of the first block not transferred
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseImageFirstNotTransferredBlockNumber(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DoubleLongUnsigned)
                {
                    try
                    {
                        m_ImageFirstNotTransferredBlockNumber = (uint)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ImageFirstNotTransferredBlockNumber = 0;
                        WriteToLog("Failed to Get the Image First Not Transferred Block Number - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageFirstNotTransferredBlockNumber = 0;
                    WriteToLog("Failed to parse the First Not Transferred Block Number - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ImageFirstNotTransferredBlockNumber = 0;
                WriteToLog("Failed to Get the Image First Not Transferred Block Number - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses whether or not the Image Transfer is enabled
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseImageTransferEnabled(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Boolean)
                {
                    try
                    {
                        m_ImageTransferEnabled = (bool)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ImageTransferEnabled = false;
                        WriteToLog("Failed to Get the Image Transfer Enabled - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageTransferEnabled = false;
                    WriteToLog("Failed to parse the Image Transfer Enabled - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ImageTransferEnabled = false;
                WriteToLog("Failed to Get the Image Transfer Enabled - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Image Transfer Status
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseImageTransferStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_ImageTransferStatus = (COSEMImageTransferStatus)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ImageTransferStatus = COSEMImageTransferStatus.TransferNotInitiated;
                        WriteToLog("Failed to Get the Image Transfer Status - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageTransferStatus = COSEMImageTransferStatus.TransferNotInitiated;
                    WriteToLog("Failed to parse the Image Transfer Status - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                WriteToLog("Failed to Get the Image Transfer Status - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                m_ImageTransferStatus = COSEMImageTransferStatus.TransferNotInitiated;
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Image to Activate Info
        /// </summary>
        /// <param name="data">The data result to parse</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  12/06/12 RCG 2.70.47 N/A    Created

        private void ParseImageToActivateInfo(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        m_ImageToActivateInfo = new List<COSEMImageToActivateInfoElement>();

                        foreach (COSEMData CurrentData in ArrayData)
                        {
                            m_ImageToActivateInfo.Add(new COSEMImageToActivateInfoElement(CurrentData));
                        }
                    }
                    catch (Exception e)
                    {
                        m_ImageToActivateInfo = null;
                        WriteToLog("Failed to Get the Image to Activate Info - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ImageToActivateInfo = null;
                    WriteToLog("Failed to parse the Image to Activate Info - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                WriteToLog("Failed to Get the Image to Activate Info - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                m_ImageToActivateInfo = null;
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not the meter does an Automatic Verification Check
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created
        
        public bool AutomaticVerificationCheck
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, -2);

                    ParseAutomaticVerificationCheck(Result);
                }

                return m_AutomaticVerificationCheck;
            }
        }

        /// <summary>
        /// Gets the Verification Hash
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created
        
        public byte[] VerificationHash
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, -1);

                    ParseVerificationHash(Result);
                }

                return m_VerificationHash;
            }
        }

        /// <summary>
        /// Gets the size of an image block
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint ImageBlockSize
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    ParseImageBlockSize(Result);
                }

                return m_ImageBlockSize;
            }
            set
            {
                if (m_DLMS.IsConnected)
                {
                    DataAccessResults Result;
                    COSEMData DataValue = new COSEMData();
                                        
                    m_ImageBlockSize = value;

                    DataValue.DataType = COSEMDataTypes.DoubleLongUnsigned;
                    DataValue.Value = m_ImageBlockSize;

                    Result = m_DLMS.Set(m_ClassID, m_LogicalName, 2, DataValue);

                    if (Result != DataAccessResults.Success)
                    {
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Set, "The Image Block Size could not be set. Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the status of each block that is to be transferred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMImageBlockStatus> ImageTransferredBlocksStatus
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    ParseImageTansferredBlocksStatus(Result);
                }

                return m_ImageTransferredBlocksStatus;
            }
        }

        /// <summary>
        /// Gets the index of the first block that has not been transferred
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint ImageFirstNotTransferredBlockNumber
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    ParseImageFirstNotTransferredBlockNumber(Result);
                }

                return m_ImageFirstNotTransferredBlockNumber;
            }
        }

        /// <summary>
        /// Gets whether or not image transfer is currently enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public bool ImageTransferEnabled
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    ParseImageTransferEnabled(Result);
                }

                return m_ImageTransferEnabled;
            }
        }

        /// <summary>
        /// Gets the current status of the image transfer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMImageTransferStatus ImageTransferStatus
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    ParseImageTransferStatus(Result);
                }

                return m_ImageTransferStatus;
            }
        }

        /// <summary>
        /// Gets the list of images that are currently ready for activation
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMImageToActivateInfoElement> ImageToActivateInfo
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    ParseImageToActivateInfo(Result);
                }

                return m_ImageToActivateInfo;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_VerificationHash;
        private bool m_AutomaticVerificationCheck;
        private uint m_ImageBlockSize;
        private List<COSEMImageBlockStatus> m_ImageTransferredBlocksStatus;
        private uint m_ImageFirstNotTransferredBlockNumber;
        private bool m_ImageTransferEnabled;
        private COSEMImageTransferStatus m_ImageTransferStatus;
        private List<COSEMImageToActivateInfoElement> m_ImageToActivateInfo; 

        #endregion
    }

    /// <summary>
    /// Key data used to set a security key
    /// </summary>
    public class COSEMKeyData
    {
        #region Constants

        private static readonly byte[] KEY_WRAP_IV = new byte[] { 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6, 0xA6 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMKeyData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyID">The ID of the key to set</param>
        /// <param name="wrappedKey">The Wrapped Key</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/02/13 RCG 2.85.10 419203 Created
        
        public COSEMKeyData(COSEMKeyIDs keyID, byte[] wrappedKey)
        {
            m_KeyID = keyID;
            m_WrappedKey = wrappedKey;
        }

        /// <summary>
        /// Converts the object to a COSEMData object
        /// </summary>
        /// <returns>The COSEM Data structure</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/18/13 RCG 2.70.69 N/A    Created
        
        public COSEMData ToCOSEMData()
        {
            COSEMData DataValue = new COSEMData();
            COSEMData[] StructureData = new COSEMData[2];

            StructureData[0] = new COSEMData();
            StructureData[0].DataType = COSEMDataTypes.Enum;
            StructureData[0].Value = (byte)m_KeyID;

            StructureData[1] = new COSEMData();
            StructureData[1].DataType = COSEMDataTypes.OctetString;
            StructureData[1].Value = m_WrappedKey;

            DataValue.DataType = COSEMDataTypes.Structure;
            DataValue.Value = StructureData;

            return DataValue;
        }

        /// <summary>
        /// Wraps a security key for Key Transfer
        /// </summary>
        /// <param name="newKey">The key to wrap</param>
        /// <param name="masterKey">The master key used during the wrapping process</param>
        /// <returns>The wrapped key</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/02/13 RCG 2.85.10 419203 Created

        public static byte[] WrapKey(byte[] newKey, byte[] masterKey)
        {
            return Cipher.WrapKey(masterKey, KEY_WRAP_IV, newKey);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Key ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMKeyIDs KeyID
        {
            get
            {
                return m_KeyID;
            }
            set
            {
                m_KeyID = value;
            }
        }

        /// <summary>
        /// Gets or sets the wrapped key
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] WrappedKey
        {
            get
            {
                return m_WrappedKey;
            }
            set
            {
                m_WrappedKey = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMKeyIDs m_KeyID;
        private byte[] m_WrappedKey;

        #endregion
    }

    /// <summary>
    /// Contains the information on the security policy
    /// </summary>
    public class COSEMSecuritySetupInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Security Setup" COSEM object
        /// </summary>
        public static readonly byte[] SECURITY_SETUP_LN = new byte[] { 0, 0, 43, 0, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMSecuritySetupInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 64;
            m_Version = 0;
        }

        /// <summary>
        /// Activates and strengthens the security policy
        /// </summary>
        /// <param name="securityPolicy">The new security policy</param>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults SecurityActivate(DLMSSecurityPolicy securityPolicy)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Enum;
                ParameterData.Value = (byte)securityPolicy;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Updates one or more global keys.
        /// </summary>
        /// <param name="keys">The keys to update</param>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults GlobalKeyTransfer(List<COSEMKeyData> keys)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();
                COSEMData[] KeyData = new COSEMData[keys.Count];

                for (int iIndex = 0; iIndex < keys.Count; iIndex++)
                {
                    KeyData[iIndex] = keys[iIndex].ToCOSEMData();
                }

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Array;
                ParameterData.Value = KeyData;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = new EnumObjectDefinition("Security Policy", typeof(DLMSSecurityPolicy));
                    AttributeDefinition.Value = SecurityPolicy;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new EnumObjectDefinition("Security Suite", typeof(DLMSSecuritySuites));
                    AttributeDefinition.Value = SecuritySuite;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new ObjectDefinition("Client System Title", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = ClientSystemTitle;
                    break;
                }
                case 5:
                {
                    AttributeDefinition = new ObjectDefinition("Server System Title", COSEMDataTypes.OctetString);
                    AttributeDefinition.Value = ServerSystemTitle;
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
                    Name = "Security Policy";
                    break;
                }
                case 3:
                {
                    Name = "Security Suite";
                    break;
                }
                case 4:
                {
                    Name = "Client System Title";
                    break;
                }
                case 5:
                {
                    Name = "Server System Title";
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
            return (new sbyte[] { 1, 2, 3, 4, 5 }).ToList();
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
            return (new sbyte[] { 1, 2 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse the Security Policy from the data get data result
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/15/13 RCG 2.70.69 N/A    Created
        
        private void ParseSecurityPolicy(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_SecurityPolicy = (DLMSSecurityPolicy)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_SecurityPolicy = DLMSSecurityPolicy.None;
                        WriteToLog("Failed to Get the Security Policy - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_SecurityPolicy = DLMSSecurityPolicy.None;
                    WriteToLog("Failed to parse the Security Policy - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SecurityPolicy = DLMSSecurityPolicy.None;
                WriteToLog("Failed to Get the Security Policy - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parse the Security Suite from the data get data result
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/15/13 RCG 2.70.69 N/A    Created

        private void ParseSecuritySuite(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_SecuritySuite = (DLMSSecuritySuites)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_SecuritySuite = DLMSSecuritySuites.AES128;
                        WriteToLog("Failed to Get the Security Suite - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_SecuritySuite = DLMSSecuritySuites.AES128;
                    WriteToLog("Failed to parse the Security Suite - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SecuritySuite = DLMSSecuritySuites.AES128;
                WriteToLog("Failed to Get the Security Suite - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parse the Client System Title from the data get data result
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/15/13 RCG 2.70.69 N/A    Created

        private void ParseClientSystemTitle(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_ClientSystemTitle = (byte[])data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ClientSystemTitle = new byte[0];
                        WriteToLog("Failed to Get the Client System Title - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ClientSystemTitle = new byte[0];
                    WriteToLog("Failed to parse the Client System Title - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ClientSystemTitle = new byte[0];
                WriteToLog("Failed to Get the Client System Title - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parse the Server System Title from the data get data result
        /// </summary>
        /// <param name="data">The get data result containing the value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/15/13 RCG 2.70.69 N/A    Created

        private void ParseServerSystemTitle(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_ServerSystemTitle = (byte[])data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ServerSystemTitle = new byte[0];
                        WriteToLog("Failed to Get the Server System Title - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ServerSystemTitle = new byte[0];
                    WriteToLog("Failed to parse the Server System Title - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ServerSystemTitle = new byte[0];
                WriteToLog("Failed to Get the Server System Title - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the current security policy
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DLMSSecurityPolicy SecurityPolicy
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(ClassID, LogicalName, 2);

                    ParseSecurityPolicy(Result);
                }

                return m_SecurityPolicy;
            }
        }

        /// <summary>
        /// Gets the current security suite
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DLMSSecuritySuites SecuritySuite
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(ClassID, LogicalName, 3);

                    ParseSecuritySuite(Result);
                }

                return m_SecuritySuite;
            }
        }

        /// <summary>
        /// Gets the current client's system title
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ClientSystemTitle
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(ClassID, LogicalName, 4);

                    ParseClientSystemTitle(Result);
                }

                return m_ClientSystemTitle;
            }
        }

        /// <summary>
        /// Gets the server's system title
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ServerSystemTitle
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(ClassID, LogicalName, 5);

                    ParseServerSystemTitle(Result);
                }

                return m_ServerSystemTitle;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The security policy in use</summary>
        protected DLMSSecurityPolicy m_SecurityPolicy;
        /// <summary>The algorithm used for security</summary>
        protected DLMSSecuritySuites m_SecuritySuite;
        /// <summary>The current client system title</summary>
        protected byte[] m_ClientSystemTitle;
        /// <summary>The server system title</summary>
        protected byte[] m_ServerSystemTitle;

        #endregion
    }

    /// <summary>
    /// Interface Class used to retrieve clock information
    /// </summary>
    public class COSEMClockInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Clock" COSEM object
        /// </summary>
        public static readonly byte[] CLOCK_LN = new byte[] { 0, 0, 1, 0, 0, 255 };

        /// <summary>
        /// Meter reference time
        /// </summary>
        public static readonly DateTime METER_REFERENCE_TIME = new DateTime(2000, 1, 1);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMClockInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 8;
            m_Version = 0;
        }

        /// <summary>
        /// Adjust the clock to the nearest quarter hour (X:00, X:15, X:30, X:45)
        /// </summary>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults AdjustToQuarter()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Adjust the clock to the nearest start of a measuring period
        /// </summary>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults AdjustToMeasuringPeriod()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Adjust the clock to the nearest minute
        /// </summary>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults AdjustToMinute()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 3, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Adjust the clock to the preset value
        /// </summary>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults AdjustToPresetTime()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 4, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Sets the preset clock value
        /// </summary>
        /// <param name="presetTime">The preset date and time</param>
        /// <param name="validityIntervalStart">The start date when the preset is valid</param>
        /// <param name="validityIntervalEnd">The end date when the preset is valid</param>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults PresetAdjustingTime(COSEMDateTime presetTime, COSEMDateTime validityIntervalStart, COSEMDateTime validityIntervalEnd)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();
                COSEMData[] StructureData = new COSEMData[3];

                StructureData[0] = new COSEMData();
                StructureData[0].DataType = COSEMDataTypes.DateTime;
                StructureData[0].Value = presetTime;

                StructureData[1] = new COSEMData();
                StructureData[1].DataType = COSEMDataTypes.DateTime;
                StructureData[1].Value = validityIntervalStart;

                StructureData[2] = new COSEMData();
                StructureData[2].DataType = COSEMDataTypes.DateTime;
                StructureData[2].Value = validityIntervalEnd;

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Structure;
                ParameterData.Value = StructureData;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 5, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Shifts the clock by the specified number of seconds
        /// </summary>
        /// <param name="seconds">The number of seconds to shift (-900 to 900)</param>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults ShiftTime(short seconds)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Long;
                ParameterData.Value = seconds;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 6, ParameterData);

                Result = Response.Result;
            }

            return Result;
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
                    AttributeDefinition = new ObjectDefinition("Time", COSEMDataTypes.DateTime);
                    AttributeDefinition.Value = Time;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new ObjectDefinition("Time Zone", COSEMDataTypes.Long);
                    AttributeDefinition.Value = TimeZone;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new EnumObjectDefinition("Status", typeof(COSEMClockStatus));
                    AttributeDefinition.Value = Status;
                    break;
                }
                case 5:
                {
                    AttributeDefinition = new ObjectDefinition("DST Begin", COSEMDataTypes.DateTime);
                    AttributeDefinition.Value = DSTBegin;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new ObjectDefinition("DST End", COSEMDataTypes.DateTime);
                    AttributeDefinition.Value = DSTEnd;
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("DST Deviation", COSEMDataTypes.Integer);
                    AttributeDefinition.Value = DSTDeviation;
                    break;
                }
                case 8:
                {
                    AttributeDefinition = new ObjectDefinition("DST Enabled", COSEMDataTypes.Boolean);
                    AttributeDefinition.Value = DSTEnabled;
                    break;
                }
                case 9:
                {
                    AttributeDefinition = new EnumObjectDefinition("Clock Base", typeof(COSEMClockBase));
                    AttributeDefinition.Value = ClockBase;
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
                    Name = "Time";
                    break;
                }
                case 3:
                {
                    Name = "Time Zone";
                    break;
                }
                case 4:
                {
                    Name = "Status";
                    break;
                }
                case 5:
                {
                    Name = "DST Begin";
                    break;
                }
                case 6:
                {
                    Name = "DST End";
                    break;
                }
                case 7:
                {
                    Name = "DST Deviation";
                    break;
                }
                case 8:
                {
                    Name = "DST Enabled";
                    break;
                }
                case 9:
                {
                    Name = "Clock Base";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).ToList();
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Time
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        private void ParseTime(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_Time = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_Time = null;
                        WriteToLog("Failed to Get the Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_Time = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_Time = null;
                        WriteToLog("Failed to Get the Time - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Time = null;
                    WriteToLog("Failed to parse the Time - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Time = null;
                WriteToLog("Failed to Get the Time - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Time Zone
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseTimeZone(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Long)
                {
                    try
                    {
                        m_TimeZone = (short)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_TimeZone = 0;
                        WriteToLog("Failed to Get the Time Zone - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_TimeZone = 0;
                    WriteToLog("Failed to parse the Time Zone - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_TimeZone = 0;
                WriteToLog("Failed to Get the Time Zone - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Status
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseStatus(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Unsigned)
                {
                    try
                    {
                        m_Status = (COSEMClockStatus)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_Status = COSEMClockStatus.InvalidClockStatus;
                        WriteToLog("Failed to Get the Status - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Status = COSEMClockStatus.InvalidClockStatus;
                    WriteToLog("Failed to parse the Status - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Status = COSEMClockStatus.InvalidClockStatus;
                WriteToLog("Failed to Get the Status - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the DST Begin date
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseDSTBegin(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_DSTBegin = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_DSTBegin = null;
                        WriteToLog("Failed to Get the DST Begin date - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_DSTBegin = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_DSTBegin = null;
                        WriteToLog("Failed to Get the DST Begin date - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_DSTBegin = null;
                    WriteToLog("Failed to parse the DST Begin date - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_DSTBegin = null;
                WriteToLog("Failed to Get the DST Begin date - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the DST End date
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseDSTEnd(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.DateTime)
                {
                    try
                    {
                        m_DSTEnd = data.DataValue.Value as COSEMDateTime;
                    }
                    catch (Exception e)
                    {
                        m_DSTEnd = null;
                        WriteToLog("Failed to Get the DST End date - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else if (data.DataValue.DataType == COSEMDataTypes.OctetString)
                {
                    try
                    {
                        m_DSTEnd = new COSEMDateTime((byte[])data.DataValue.Value);
                    }
                    catch (Exception e)
                    {
                        m_DSTEnd = null;
                        WriteToLog("Failed to Get the DST End date - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_DSTEnd = null;
                    WriteToLog("Failed to parse the DST End date - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_DSTEnd = null;
                WriteToLog("Failed to Get the DST End date - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the DST Deviation
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseDSTDeviation(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Integer)
                {
                    try
                    {
                        m_DSTDeviation = (sbyte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_DSTDeviation = 0;
                        WriteToLog("Failed to Get the DST Deviation - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_DSTDeviation = 0;
                    WriteToLog("Failed to parse the DST Deviation - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_DSTDeviation = 0;
                WriteToLog("Failed to Get the DST Deviation - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the DST Enabled value
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseDSTEnabled(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Boolean)
                {
                    try
                    {
                        m_DSTEnabled = (bool)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_DSTEnabled = false;
                        WriteToLog("Failed to Get the DST Enabled flag - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_DSTEnabled = false;
                    WriteToLog("Failed to parse the DST Enabled flag - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_DSTEnabled = false;
                WriteToLog("Failed to Get the DST Enabled flag - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Clock Base
        /// </summary>
        /// <param name="data">The Get Result containing the data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private void ParseClockBase(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_ClockBase = (COSEMClockBase)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ClockBase = COSEMClockBase.NotDefined;
                        WriteToLog("Failed to Get the Clock Base - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ClockBase = COSEMClockBase.NotDefined;
                    WriteToLog("Failed to parse the Clock Base - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ClockBase = COSEMClockBase.NotDefined;
                WriteToLog("Failed to Get the Clock Base - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the device time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDateTime Time
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseTime(Result);
                    }
                }

                return m_Time;
            }
            set
            {
                if (m_DLMS.IsConnected && value != null)
                {
                    DataAccessResults Result;
                    COSEMData DataValue = new COSEMData();
                    m_Time = value;

                    DataValue.DataType = COSEMDataTypes.OctetString;
                    DataValue.Value = m_Time.Data;

                    Result = m_DLMS.Set(m_ClassID, m_LogicalName, 2, DataValue);

                    if (Result != DataAccessResults.Success)
                    {
                        throw new InvalidOperationException("The Time could not be set. Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the time zone offset in minutes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public short TimeZone
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseTimeZone(Result);
                    }
                }

                return m_TimeZone;
            }
        }

        /// <summary>
        /// Gets the clock status
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMClockStatus Status
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseStatus(Result);
                    }
                }

                return m_Status;
            }
        }

        /// <summary>
        /// Gets the DST start date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDateTime DSTBegin
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    if (Result != null)
                    {
                        ParseDSTBegin(Result);
                    }
                }

                return m_DSTBegin;
            }
        }

        /// <summary>
        /// Gets the DST end date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDateTime DSTEnd
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    if (Result != null)
                    {
                        ParseDSTEnd(Result);
                    }
                }

                return m_DSTEnd;
            }
        }

        /// <summary>
        /// Gets the number of minutes to adjust when DST occurs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte DSTDeviation
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    if (Result != null)
                    {
                        ParseDSTDeviation(Result);
                    }
                }

                return m_DSTDeviation;
            }
        }

        /// <summary>
        /// Gets whether or not DST is currently enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public bool DSTEnabled
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 8);

                    if (Result != null)
                    {
                        ParseDSTEnabled(Result);
                    }
                }

                return m_DSTEnabled;
            }
        }

        /// <summary>
        /// Gets the method used by the clock timer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMClockBase ClockBase
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 9);

                    if (Result != null)
                    {
                        ParseClockBase(Result);
                    }
                }

                return m_ClockBase;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The device time</summary>
        protected COSEMDateTime m_Time;
        /// <summary>The deviation of the local time from UTC in minutes</summary>
        protected short m_TimeZone;
        /// <summary>The clock status information</summary>
        protected COSEMClockStatus m_Status;
        /// <summary>The DST start date</summary>
        protected COSEMDateTime m_DSTBegin;
        /// <summary>The DST end date</summary>
        protected COSEMDateTime m_DSTEnd;
        /// <summary>The number of minutes the time is adjusted during DST</summary>
        protected sbyte m_DSTDeviation;
        /// <summary>Whether or not DST is enabled</summary>
        protected bool m_DSTEnabled;
        /// <summary>The method used by the clock timer</summary>
        protected COSEMClockBase m_ClockBase;

        #endregion
    }

    /// <summary>
    /// Specifies a single script action
    /// </summary>
    public class COSEMActionSpecification
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionSpecification()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Action Specification</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/13 RCG 2.80.11 N/A    Created
        
        public COSEMActionSpecification(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 5)
                    {
                        // Get the Service ID
                        if (StructureData[0].DataType == COSEMDataTypes.Enum)
                        {
                            m_ServiceID = (COSEMActionServiceID)(byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Service ID data type is not correct.", "data");
                        }

                        // Get the Class ID
                        if (StructureData[1].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ClassID = (ushort)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Class ID data type is not correct.", "data");
                        }

                        // Get the Logical Name
                        if (StructureData[2].DataType == COSEMDataTypes.OctetString)
                        {
                            m_LogicalName = (byte[])StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Logical Name data type is not correct.", "data");
                        }

                        // Get the Index
                        if (StructureData[3].DataType == COSEMDataTypes.Integer)
                        {
                            m_Index = (sbyte)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Index data type is not correct.", "data");
                        }

                        // Get the Parameter data
                        m_Parameter = StructureData[4];
                    }
                    else
                    {
                        throw new ArgumentException("The Action Specification is not of length 5.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Action Specification");
            ObjectDefinition NewObjectDefinition;

            NewObjectDefinition = new EnumObjectDefinition("Service ID", typeof(COSEMActionServiceID));
            NewObjectDefinition.Value = COSEMActionServiceID.WriteAttribute;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Class ID", COSEMDataTypes.LongUnsigned);
            NewObjectDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Logical Name", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Index", COSEMDataTypes.Integer);
            NewObjectDefinition.Value = (sbyte)0;
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewObjectDefinition = new ObjectDefinition("Parameter Data", COSEMDataTypes.NullData);
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
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ServiceID;
            Definition.StructureDefinition[1].Value = m_ClassID;
            Definition.StructureDefinition[2].Value = m_LogicalName;
            Definition.StructureDefinition[3].Value = m_Index;

            // The Parameter is a generic value so we need to update the data type
            Definition.StructureDefinition[4] = ObjectDefinition.CreateFromCOSEMData(Definition.StructureDefinition[4].ItemName, m_Parameter);

            // Set the Value field to this instance of the Action Item
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the action type (Attribute or Method)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionServiceID ServiceID
        {
            get
            {
                return m_ServiceID;
            }
            set
            {
                m_ServiceID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Class ID of the IC the attribute or method is in
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the logical name of the IC the attribute or method is in
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the attribute or method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        /// <summary>
        /// Gets or sets the parameter data for the attribute write or method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMData Parameter
        {
            get
            {
                return m_Parameter;
            }
            set
            {
                m_Parameter = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMActionServiceID m_ServiceID;
        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private sbyte m_Index;
        private COSEMData m_Parameter;

        #endregion
    }

    /// <summary>
    /// Script table object
    /// </summary>
    public class COSEMScript
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScript()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/13 RCG 2.80.11 N/A    Created
        
        public COSEMScript(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData.Length == 2)
                    {
                        // Get the Script Identifier
                        if (StructureData[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ScriptID = (ushort)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Script Identifier is not the correct data type.", "data");
                        }

                        // Get the Array of Actions
                        if (StructureData[1].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] Actions = StructureData[1].Value as COSEMData[];
                            m_Actions = new List<COSEMActionSpecification>();

                            foreach (COSEMData CurrentAction in Actions)
                            {
                                m_Actions.Add(new COSEMActionSpecification(CurrentAction));
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Actions array is not the correct data type.", "data");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The Script structure is not of length 2.", "data");
                    }
                }
                else
                {
                    throw new ArgumentException("The data parameter must contain a COSEMData object of type structure.", "data");
                }
            }
            else
            {
                throw new ArgumentNullException("data", "The data parameter may not be null.");
            }
        }

        /// <summary>
        /// Gets the definition of the Push Object Element
        /// </summary>
        /// <returns>The object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Script");
            ObjectDefinition IdentifierDefinition;
            ArrayObjectDefinition ActionsDefinition;

            IdentifierDefinition = new ObjectDefinition("Script Identifier", COSEMDataTypes.LongUnsigned);
            IdentifierDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(IdentifierDefinition);

            ActionsDefinition = new ArrayObjectDefinition("Actions", COSEMActionSpecification.GetStructureDefinition());
            Definition.StructureDefinition.Add(ActionsDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/08/13 RCG 2.80.28 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ScriptID;

            // We need to set up the list of elements
            Definition.StructureDefinition[1].Value = m_Actions;

            ArrayObjectDefinition ArrayDefinition = Definition.StructureDefinition[1] as ArrayObjectDefinition;

            if (ArrayDefinition != null && m_Actions != null)
            {
                ArrayDefinition.Elements.Clear();

                foreach (COSEMActionSpecification CurrentElement in m_Actions)
                {
                    ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                }
            }

            // Set the Value field to this instance of the Action Item
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the ID of the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ScriptID
        {
            get
            {
                return m_ScriptID;
            }
            set
            {
                m_ScriptID = value;
            }
        }

        /// <summary>
        /// Gets the list of actions performed by the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMActionSpecification> Actions
        {
            get
            {
                return m_Actions;
            }
            set
            {
                m_Actions = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ScriptID;
        private List<COSEMActionSpecification> m_Actions;

        #endregion
    }

    /// <summary>
    /// Interface class used for storing scripts
    /// </summary>
    public class COSEMScriptTableInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// Global Meter Reset LN
        /// </summary>
        public static readonly byte[] GLOBAL_METER_RESET = new byte[] { 0x00, 0x00, 0x0A, 0x00, 0x00, 0xFF };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScriptTableInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 9;
            m_Version = 0;
        }

        /// <summary>
        /// Executes the specified script
        /// </summary>
        /// <param name="scriptID">The ID of the script to execute</param>
        /// <returns>The result of the execute</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public ActionResults Execute(ushort scriptID)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.LongUnsigned;
                ParameterData.Value = scriptID;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.28 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Scripts", COSEMScript.GetStructureDefinition());
                    List<COSEMScript> CurrentScripts = Scripts;

                    foreach (COSEMScript CurrentElement in CurrentScripts)
                    {
                        ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                    }

                    AttributeDefinition = ArrayDefinition;
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
                    Name = "Scripts";
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
            return (new sbyte[] { 1 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the list of scripts from the result
        /// </summary>
        /// <param name="data">The Get Data Result containing the Scripts Get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/25/13 RCG 2.80.10 N/A    Created
        
        private void ParseScripts(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        m_Scripts = new List<COSEMScript>();

                        foreach (COSEMData CurrentValue in ArrayData)
                        {
                            m_Scripts.Add(new COSEMScript(CurrentValue));
                        }
                    }
                    catch (Exception e)
                    {
                        m_Scripts = null;
                        WriteToLog("Failed to Get the Scripts - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Scripts = null;
                    WriteToLog("Failed to parse the Scripts - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Scripts = null;
                WriteToLog("Failed to Get the Scripts - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the scripts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMScript> Scripts
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseScripts(Result);
                    }
                }

                return m_Scripts;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The list of scripts in the table</summary>
        protected List<COSEMScript> m_Scripts;

        #endregion
    }

    /// <summary>
    /// Schedule Table Entry
    /// </summary>
    public class COSEMScheduleTableEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScheduleTableEntry()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the entry index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not the item entry is enabled
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the logical name of the Script table that contains the script to run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ScriptLogicalName
        {
            get
            {
                return m_ScriptLogicalName;
            }
            set
            {
                m_ScriptLogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the script identifier of the script to run 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ScriptSelector
        {
            get
            {
                return m_ScriptSelector;
            }
            set
            {
                m_ScriptSelector = value;
            }
        }

        /// <summary>
        /// Gets or sets the time of day to run the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime SwitchTime
        {
            get
            {
                return m_SwitchTime;
            }
            set
            {
                m_SwitchTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the period in minutes which an entry must be processed after a power failure
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ValidityWindow
        {
            get
            {
                return m_ValidityWindow;
            }
            set
            {
                m_ValidityWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets the days of the week that the script should be run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMExecuteWeekdays ExecuteWeekdays
        {
            get
            {
                return m_ExecuteWeekdays;
            }
            set
            {
                m_ExecuteWeekdays = value;
            }
        }

        /// <summary>
        /// Gets or sets the special days on which the script should be run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<bool> ExecuteSpecialDays
        {
            get
            {
                return m_ExecuteSpecialDays;
            }
            set
            {
                m_ExecuteSpecialDays = value;
            }
        }

        /// <summary>
        /// Gets or sets the start date of when the entry is valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime BeginDate
        {
            get
            {
                return m_BeginDate;
            }
            set
            {
                m_BeginDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the end date of when the entry is valid
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime EndDate
        {
            get
            {
                return m_EndDate;
            }
            set
            {
                m_EndDate = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Index;
        private bool m_Enabled;
        private byte[] m_ScriptLogicalName;
        private ushort m_ScriptSelector;
        private DateTime m_SwitchTime;
        private ushort m_ValidityWindow;
        private COSEMExecuteWeekdays m_ExecuteWeekdays; // TODO: Make sure this data type is correct
        private List<bool> m_ExecuteSpecialDays; // TODO: Make sure this data type makes sense
        private DateTime m_BeginDate;
        private DateTime m_EndDate;

        #endregion
    }

    /// <summary>
    /// Interface class used for time and date driven activities
    /// </summary>
    public class COSEMScheduleInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name for the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMScheduleInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 10;
            m_Version = 0;
        }

        /// <summary>
        /// Enables and disables the specified schedule entries
        /// </summary>
        /// <param name="disableStartIndex">The index of the first entry to disable</param>
        /// <param name="disableLastIndex">The index of the last entry to disable</param>
        /// <param name="enableStartIndex">The index of the first entry to enable</param>
        /// <param name="enableLastIndex">The index of the last entry to enable</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void EnableAndDisable(ushort disableStartIndex, ushort disableLastIndex, ushort enableStartIndex, ushort enableLastIndex)
        {
        }

        /// <summary>
        /// Inserts a new schedule entry in the table
        /// </summary>
        /// <param name="newEntry">The entry to insert</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void Insert(COSEMScheduleTableEntry newEntry)
        {
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="entry"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public void Delete(COSEMScheduleTableEntry entry)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Schedule Table Entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMScheduleTableEntry> Entries
        {
            get
            {
                return m_Entries;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The list of Schedule entries</summary>
        protected List<COSEMScheduleTableEntry> m_Entries;

        #endregion
    }

    /// <summary>
    /// Special Day entry
    /// </summary>
    public class COSEMSpecialDay
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMSpecialDay()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        /// <summary>
        /// Gets or sets the Date of the special day
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime Date
        {
            get
            {
                return m_Date;
            }
            set
            {
                m_Date = value;
            }
        }

        /// <summary>
        /// Gets or sets the Day ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        private byte DayID
        {
            get
            {
                return m_DayID;
            }
            set
            {
                m_DayID = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_Index;
        private DateTime m_Date;
        private byte m_DayID;

        #endregion
    }

    /// <summary>
    /// Defines special dates used by the device
    /// </summary>
    public class COSEMSpecialDaysInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMSpecialDaysInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
        }

        /// <summary>
        /// Inserts the specified special day
        /// </summary>
        /// <param name="specialDay">The special day to insert</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void Insert(COSEMSpecialDay specialDay)
        {
        }

        /// <summary>
        /// Removes the specified special day
        /// </summary>
        /// <param name="specialDay">The special day to remove</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void Delete(COSEMSpecialDay specialDay)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of special day entries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMSpecialDay> Entries
        {
            get
            {
                return m_Entries;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The list of special days</summary>
        protected List<COSEMSpecialDay> m_Entries;

        #endregion
    }

    /// <summary>
    /// COSEM Activity Calendar Season
    /// </summary>
    public class COSEMSeason
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMSeason()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Season Profile Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] SeasonProfileName
        {
            get
            {
                return m_SeasonProfileName;
            }
            set
            {
                m_SeasonProfileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the season start date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime SeasonStart
        {
            get
            {
                return m_SeasonStart;
            }
            set
            {
                m_SeasonStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the week associated with this season
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] WeekName
        {
            get
            {
                return m_WeekName;
            }
            set
            {
                m_WeekName = value;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_SeasonProfileName;
        private DateTime m_SeasonStart;
        private byte[] m_WeekName;

        #endregion
    }

    /// <summary>
    /// COSEM Activity Calendar Week
    /// </summary>
    public class COSEMWeek
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMWeek()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the profile name for the week
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] WeekProfileName
        {
            get
            {
                return m_WeekProfileName;
            }
            set
            {
                m_WeekProfileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Monday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte MondayID
        {
            get
            {
                return m_MondayID;
            }
            set
            {
                m_MondayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Tuesday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte TuesdayID
        {
            get
            {
                return m_TuesdayID;
            }
            set
            {
                m_TuesdayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Wednesday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte WednesdayID
        {
            get
            {
                return m_WednesdayID;
            }
            set
            {
                m_WednesdayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Thursday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte ThursdayID
        {
            get
            {
                return m_ThursdayID;
            }
            set
            {
                m_ThursdayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Friday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte FridayID
        {
            get
            {
                return m_FridayID;
            }
            set
            {
                m_FridayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Saturday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte SaturdayID
        {
            get
            {
                return m_SaturdayID;
            }
            set
            {
                m_SaturdayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the day ID for Sunday
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte SundayID
        {
            get
            {
                return m_SundayID;
            }
            set
            {
                m_SundayID = value;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_WeekProfileName;
        private byte m_MondayID;
        private byte m_TuesdayID;
        private byte m_WednesdayID;
        private byte m_ThursdayID;
        private byte m_FridayID;
        private byte m_SaturdayID;
        private byte m_SundayID;

        #endregion
    }

    /// <summary>
    /// COSEM Activity Calendar Day
    /// </summary>
    public class COSEMDay
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDay()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the day identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte DayID
        {
            get
            {
                return m_DayID;
            }
            set
            {
                m_DayID = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduled actions for the day
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMDayProfileAction> DaySchedule
        {
            get
            {
                return m_DaySchedule;
            }
            set
            {
                m_DaySchedule = value;
            }
        }

        #endregion

        #region Member Variables

        private byte m_DayID;
        private List<COSEMDayProfileAction> m_DaySchedule;

        #endregion
    }

    /// <summary>
    /// COSEM Activity Calendar Day Action
    /// </summary>
    public class COSEMDayProfileAction
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDayProfileAction()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the action start time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public DateTime StartTime
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

        /// <summary>
        /// Gets or sets the logical name for the script table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public byte[] ScriptLogicalName
        {
            get
            {
                return m_ScriptLogicalName;
            }
            set
            {
                m_ScriptLogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the script to run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public ushort ScriptSelector
        {
            get
            {
                return m_ScriptSelector;
            }
            set
            {
                m_ScriptSelector = value;
            }
        }

        #endregion

        #region Member Variables

        private DateTime m_StartTime;
        private byte[] m_ScriptLogicalName;
        private ushort m_ScriptSelector;

        #endregion
    }

    /// <summary>
    /// Interface class used for handling an activity calendar
    /// </summary>
    public class COSEMActivityCalendarInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMActivityCalendarInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 20;
            m_Version = 0;
        }

        /// <summary>
        /// Activates the passive calendar
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public void ActivatePassiveCalendar()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the active calendar's name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ActiveCalendarName
        {
            get
            {
                return m_ActiveCalendarName;
            }
        }

        /// <summary>
        /// Gets the active calendar's seasons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMSeason> ActiveSeasonProfiles
        {
            get
            {
                return m_ActiveSeasonProfiles;
            }
        }

        /// <summary>
        /// Gets the active calendar's weeks
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMWeek> ActiveWeekProfiles
        {
            get
            {
                return m_ActiveWeekProfiles;
            }
        }

        /// <summary>
        /// Gets the active calendar's days
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMDay> ActiveDayProfiles
        {
            get
            {
                return m_ActiveDayProfiles;
            }
        }

        /// <summary>
        /// Gets the passive calendar's name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] PassiveCalendarName
        {
            get
            {
                return m_PassiveCalendarName;
            }
        }

        /// <summary>
        /// Gets the passive calendar's seasons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMSeason> PassiveSeasonProfiles
        {
            get
            {
                return m_PassiveSeasonProfiles;
            }
        }

        /// <summary>
        /// Gets the passive calendar's weeks
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMWeek> PassiveWeekProfiles
        {
            get
            {
                return m_PassiveWeekProfiles;
            }
        }

        /// <summary>
        /// Gets the passive calendar's days
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMDay> PassiveDayProfiles
        {
            get
            {
                return m_PassiveDayProfiles;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The active calendar identifier</summary>
        protected byte[] m_ActiveCalendarName;
        /// <summary>List of season in the active calendar</summary>
        protected List<COSEMSeason> m_ActiveSeasonProfiles;
        /// <summary>List of week profiles in the active calendar</summary>
        protected List<COSEMWeek> m_ActiveWeekProfiles;
        /// <summary>List of day profile in the active calendar</summary>
        protected List<COSEMDay> m_ActiveDayProfiles;
        /// <summary>The passive calendar identifier</summary>
        protected byte[] m_PassiveCalendarName;
        /// <summary>List of seasons in the passive calendar</summary>
        protected List<COSEMSeason> m_PassiveSeasonProfiles;
        /// <summary>List of week profiles in the passive calendar</summary>
        protected List<COSEMWeek> m_PassiveWeekProfiles;
        /// <summary>List of day profiles in the passive calendar</summary>
        protected List<COSEMDay> m_PassiveDayProfiles;

        #endregion
    }

    /// <summary>
    /// Describes which value is being monitored
    /// </summary>
    public class COSEMMonitoredValue
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMMonitoredValue()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Monitored Value</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created

        public COSEMMonitoredValue(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 3)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ClassID = (ushort)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Class ID is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_LogicalName = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Logical Name is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Integer)
                        {
                            m_AttributeIndex = (sbyte)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Attribute Index is not the expected data type.");
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
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Monitored Value");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Class ID", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Logical Name", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Attribute Index", COSEMDataTypes.Integer);
            CurrentDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ClassID;
            Definition.StructureDefinition[1].Value = m_LogicalName;
            Definition.StructureDefinition[2].Value = m_AttributeIndex;

            // Set the Value field to this instance of the Push Element
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID of the object storing the monitored value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Logical Name of the object storing the monitored value
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the attribute that is being monitored
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public sbyte AttributeIndex
        {
            get
            {
                return m_AttributeIndex;
            }
            set
            {
                m_AttributeIndex = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private sbyte m_AttributeIndex;

        #endregion
    }

    /// <summary>
    /// Identifies an action to take
    /// </summary>
    public class COSEMActionItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionItem()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the action item</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created

        public COSEMActionItem(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.OctetString)
                        {
                            m_ScriptLogicalName = (byte[])StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Script Logical Name is not the correct data type.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ScriptSelector = (ushort)StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Script Selector is not the correct data type.", "data");
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
        //  04/29/13 RCG 2.80.24 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Action Item");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Script Logical Name", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Script Selector", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ScriptLogicalName;
            Definition.StructureDefinition[1].Value = m_ScriptSelector;

            // Set the Value field to this instance of the Action Item
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the logical name of the object containing the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public byte[] ScriptLogicalName
        {
            get
            {
                return m_ScriptLogicalName;
            }
            set
            {
                m_ScriptLogicalName = value;
            }
        }

        /// <summary>
        /// Gets the selector for the script to run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort ScriptSelector
        {
            get
            {
                return m_ScriptSelector;
            }
            set
            {
                m_ScriptSelector = value;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_ScriptLogicalName;
        private ushort m_ScriptSelector;

        #endregion
    }

    /// <summary>
    /// The set of actions to take on a monitored register value
    /// </summary>
    public class COSEMActionSet
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionSet()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Action Set</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created

        public COSEMActionSet(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Structure)
                        {
                            m_ActionUp = new COSEMActionItem(StructureData[0].Value as COSEMData);
                        }
                        else if (StructureData[0].DataType == COSEMDataTypes.NullData)
                        {
                            m_ActionUp = null;
                        }
                        else
                        {
                            throw new ArgumentException("The Action Up item is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Structure)
                        {
                            m_ActionDown = new COSEMActionItem(StructureData[1].Value as COSEMData);
                        }
                        else if (StructureData[1].DataType == COSEMDataTypes.NullData)
                        {
                            m_ActionDown = null;
                        }
                        else
                        {
                            throw new ArgumentException("The Action Down item is not the expected data type.");
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
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Monitored Value");

            StructureObjectDefinition ActionUpDefinition = COSEMActionItem.GetStructureDefinition();
            StructureObjectDefinition ActionDownDefinition = COSEMActionItem.GetStructureDefinition();

            ActionUpDefinition.ItemName = "Action Up";
            ActionDownDefinition.ItemName = "Action Down";

            Definition.StructureDefinition.Add(ActionUpDefinition);
            Definition.StructureDefinition.Add(ActionDownDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            StructureObjectDefinition ActionUpDefinition = m_ActionUp.ToObjectDefinition() as StructureObjectDefinition;
            StructureObjectDefinition ActionDownDefinition = m_ActionDown.ToObjectDefinition() as StructureObjectDefinition;

            ActionUpDefinition.ItemName = Definition.StructureDefinition[0].ItemName;
            ActionDownDefinition.ItemName = Definition.StructureDefinition[1].ItemName;

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0] = ActionUpDefinition;
            Definition.StructureDefinition[1] = ActionDownDefinition;

            // Set the Value field to this instance of the Push Element
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The action to take when the threshold is crossed going up
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionItem ActionUp
        {
            get
            {
                return m_ActionUp;
            }
            set
            {
                m_ActionUp = value;
            }
        }

        /// <summary>
        /// The action to take when the threshold is crossed going down
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionItem ActionDown
        {
            get
            {
                return m_ActionDown;
            }
            set
            {
                m_ActionDown = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMActionItem m_ActionUp;
        private COSEMActionItem m_ActionDown;

        #endregion
    }

    /// <summary>
    /// Allows Data, Register, Extended Register, and Demand register objects to be monitored
    /// </summary>
    public class COSEMRegisterMonitorInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMRegisterMonitorInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 21;
            m_Version = 0;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    // The data type of the Thresholds will vary depending on what has been selected as the Monitored Value. It
                    // will always be a simple data type so we don't need to worry about Arrays and Structures.
                    List<COSEMData> ThresholdValues = Thresholds;
                    ObjectDefinition ElementDefinition = null;

                    if (ThresholdValues.Count > 0)
                    {
                        // We have valid thresholds to tell us what the data type is.
                        ElementDefinition = new ObjectDefinition("Element", ThresholdValues[0].DataType);
                    }
                    else
                    {
                        // We still don't know the data type so we are going to need to read the actual attribute value
                        COSEMMonitoredValue ValueMonitored = MonitoredValue;

                        if (ValueMonitored != null)
                        {
                            GetDataResult Result = m_DLMS.Get(ValueMonitored.ClassID, ValueMonitored.LogicalName, ValueMonitored.AttributeIndex);

                            if (Result.GetDataResultType == GetDataResultChoices.Data)
                            {
                                // Again this should be a simple data type so we can just use the resulting data type
                                ElementDefinition = new ObjectDefinition("Element", Result.DataValue.DataType);
                            }
                        }
                    }

                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Thresholds", ElementDefinition);

                    for (int iIndex = 0; iIndex < ThresholdValues.Count; iIndex++)
                    {
                        ArrayDefinition.Elements.Add(ObjectDefinition.CreateFromCOSEMData("[" + iIndex.ToString(CultureInfo.InvariantCulture) + "]", ThresholdValues[iIndex]));
                    }

                    AttributeDefinition = ArrayDefinition;

                    break;
                }
                case 3:
                {
                    AttributeDefinition = MonitoredValue.ToObjectDefinition();
                    break;
                }
                case 4:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Actions", COSEMActionSet.GetStructureDefinition());
                    List<COSEMActionSet> CurrentList = Actions;

                    foreach (COSEMActionSet CurrentAction in CurrentList)
                    {
                        ArrayDefinition.Elements.Add(CurrentAction.ToObjectDefinition());
                    }

                    AttributeDefinition = ArrayDefinition;
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
                    Name = "Thresholds";
                    break;
                }
                case 3:
                {
                    Name = "Monitored Value";
                    break;
                }
                case 4:
                {
                    Name = "Actions";
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
            return (new sbyte[] { 1, 2, 3, 4 }).ToList();
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

        #region Private Methods

        /// <summary>
        /// Parses the Thresholds
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        private void ParseThresholds(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        if (ArrayData != null)
                        {
                            m_Thresholds = ArrayData.ToList();
                        }
                    }
                    catch (Exception e)
                    {
                        m_Thresholds = new List<COSEMData>();
                        WriteToLog("Failed to Get the Thresholds - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Thresholds = new List<COSEMData>();
                    WriteToLog("Failed to parse the Thresholds - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Thresholds = new List<COSEMData>();
                WriteToLog("Failed to Get the Thresholds - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Monitored Value
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        private void ParseMonitoredValue(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Structure)
                {
                    try
                    {
                        m_MonitoredValue = new COSEMMonitoredValue(data.DataValue as COSEMData);
                    }
                    catch (Exception e)
                    {
                        m_MonitoredValue = null;
                        WriteToLog("Failed to Get the Monitored Value - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_MonitoredValue = null;
                    WriteToLog("Failed to parse the Monitored Value - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_MonitoredValue = null;
                WriteToLog("Failed to Get the Monitored Value - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Actions
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/29/13 RCG 2.80.24 N/A    Created
        
        private void ParseActions(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        m_Actions = new List<COSEMActionSet>();

                        foreach (COSEMData CurrentElement in ArrayData)
                        {
                            m_Actions.Add(new COSEMActionSet(CurrentElement));
                        }
                    }
                    catch (Exception e)
                    {
                        m_Actions = new List<COSEMActionSet>();
                        WriteToLog("Failed to Get the Actions - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Actions = new List<COSEMActionSet>();
                    WriteToLog("Failed to parse the Actions - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Actions = new List<COSEMActionSet>();
                WriteToLog("Failed to Get the Actions - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of thresholds to monitor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMData> Thresholds
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseThresholds(Result);
                    }
                }

                return m_Thresholds;
            }
        }

        /// <summary>
        /// Gets the information on the value to monitor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMMonitoredValue MonitoredValue
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseMonitoredValue(Result);
                    }
                }

                return m_MonitoredValue;
            }
        }

        /// <summary>
        /// Gets the list of actions to take when a threshold is crossed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<COSEMActionSet> Actions
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseActions(Result);
                    }
                }

                return m_Actions;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The threshold values</summary>
        protected List<COSEMData> m_Thresholds;
        /// <summary>The value monitored</summary>
        protected COSEMMonitoredValue m_MonitoredValue;
        /// <summary>The actions to take when a threshold is crossed</summary>
        protected List<COSEMActionSet> m_Actions;

        #endregion
    }

    /// <summary>
    /// Single Action Schedule execution times
    /// </summary>
    public class COSEMExecutionTime
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public COSEMExecutionTime()
        {
            m_Time = new COSEMTime();
            m_Date = new COSEMDate();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Execution Time</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public COSEMExecutionTime(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.OctetString
                            || StructureData[0].DataType == COSEMDataTypes.Time)
                        {
                            m_Time = new COSEMTime((byte[])StructureData[0].Value);
                        }
                        else
                        {
                            throw new ArgumentException("The Time data type is not correct.", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString
                            || StructureData[1].DataType == COSEMDataTypes.Date)
                        {
                            m_Date = new COSEMDate((byte[])StructureData[1].Value);
                        }
                        else
                        {
                            throw new ArgumentException("The Date data type is not correct.", "data");
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
        //  05/06/13 RCG 2.80.26 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Push Object Element");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Time", COSEMDataTypes.Time);
            CurrentDefinition.Value = new COSEMTime();
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Date", COSEMDataTypes.Date);
            CurrentDefinition.Value = new COSEMDate();
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/06/13 RCG 2.80.26 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_Time;
            Definition.StructureDefinition[1].Value = m_Date;

            // Set the Value field to this instance of the Push Element
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public COSEMTime Time
        {
            get
            {
                return m_Time;
            }
            set
            {
                if (value != null)
                {
                    m_Time = value;
                }
                else
                {
                    throw new ArgumentNullException("The Time value may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created

        public COSEMDate Date
        {
            get
            {
                return m_Date;
            }
            set
            {
                if (value != null)
                {
                    m_Date = value;
                }
                else
                {
                    throw new ArgumentNullException("The Date value may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private COSEMTime m_Time;
        private COSEMDate m_Date;

        #endregion
    }

    /// <summary>
    /// Allows for periodic execution of an action within the meter
    /// </summary>
    public class COSEMSingleActionScheduleInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMSingleActionScheduleInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 22;
            m_Version = 0;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/06/13 RCG 2.80.26 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = ExecutedScript.ToObjectDefinition();
                    AttributeDefinition.ItemName = "Executed Script";
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new EnumObjectDefinition("Type", typeof(COSEMSingleActionType));
                    AttributeDefinition.Value = Type;
                    break;
                }
                case 4:
                {
                    ArrayObjectDefinition ExecutionTimesDefinition = new ArrayObjectDefinition("Execution Times", COSEMExecutionTime.GetStructureDefinition());
                    List<COSEMExecutionTime> CurrentList = ExecutionTimes;

                    foreach (COSEMExecutionTime CurrentExecutionTime in CurrentList)
                    {
                        ExecutionTimesDefinition.Elements.Add(CurrentExecutionTime.ToObjectDefinition());
                    }

                    AttributeDefinition = ExecutionTimesDefinition;                  

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
                    Name = "Executed Script";
                    break;
                }
                case 3:
                {
                    Name = "Type";
                    break;
                }
                case 4:
                {
                    Name = "Execution Times";
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
            return (new sbyte[] { 1, 2, 3, 4 }).ToList();
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

        #region Private Methods

        /// <summary>
        /// Parses the Executed Script data from the Get Data Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseExecutedScript(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Structure)
                {
                    try
                    {
                        m_ExecutedScript = new COSEMActionItem(data.DataValue);
                    }
                    catch (Exception e)
                    {
                        m_ExecutedScript = null;
                        WriteToLog("Failed to Get the Executed Script - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ExecutedScript = null;
                    WriteToLog("Failed to parse the Executed Script - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ExecutedScript = null;
                WriteToLog("Failed to Get the Executed Script - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the type from the Get Data Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseType(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_Type = (COSEMSingleActionType)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_Type = COSEMSingleActionType.SingleDateWithWildcards;
                        WriteToLog("Failed to Get the Type - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Type = COSEMSingleActionType.SingleDateWithWildcards;
                    WriteToLog("Failed to parse the Type - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Type = COSEMSingleActionType.SingleDateWithWildcards;
                WriteToLog("Failed to Get the Type - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Execution Times from the Get Data Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseExecutionTimes(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];
                        m_ExecutionTimes = new List<COSEMExecutionTime>();

                        foreach (COSEMData CurrentItem in ArrayData)
                        {
                            m_ExecutionTimes.Add(new COSEMExecutionTime(CurrentItem));
                        }
                    }
                    catch (Exception e)
                    {
                        m_ExecutionTimes = null;
                        WriteToLog("Failed to Get the Execution Times - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ExecutionTimes = null;
                    WriteToLog("Failed to parse the Execution Times - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ExecutionTimes = null;
                WriteToLog("Failed to Get the Execution Times - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the script information that will be run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMActionItem ExecutedScript
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParseExecutedScript(Result);
                    }
                }

                return m_ExecutedScript;
            }
        }

        /// <summary>
        /// Gets the type of recurring action
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public COSEMSingleActionType Type
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseType(Result);
                    }
                }

                return m_Type;
            }
        }

        /// <summary>
        /// Gets the list of execution times
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created
        
        public List<COSEMExecutionTime> ExecutionTimes
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseExecutionTimes(Result);
                    }
                }

                return m_ExecutionTimes;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Identifies the script to run</summary>
        protected COSEMActionItem m_ExecutedScript;
        /// <summary>The type of scheduling to use</summary>
        protected COSEMSingleActionType m_Type;
        /// <summary>When the action should be executed</summary>
        protected List<COSEMExecutionTime> m_ExecutionTimes;

        #endregion
    }

    /// <summary>
    /// Disconnect Control Interface Class
    /// </summary>
    public class COSEMDisconnectControlInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDisconnectControlInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 70;
            m_Version = 0;
        }

        /// <summary>
        /// Disconnects the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults RemoteDisconnect()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Reconnects the device 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ActionResults RemoteReconnect()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                Result = Response.Result;
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/26/13 RCG 2.80.23 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = new ObjectDefinition("Output State", COSEMDataTypes.Boolean);
                    AttributeDefinition.Value = OutputState;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new EnumObjectDefinition("Control State", typeof(COSEMDisconnectControlState));
                    AttributeDefinition.Value = ControlState;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new EnumObjectDefinition("Control Mode", typeof(COSEMDisconnectControlMode));
                    AttributeDefinition.Value = ControlMode;
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
                    Name = "Output State";
                    break;
                }
                case 3:
                {
                    Name = "Control State";
                    break;
                }
                case 4:
                {
                    Name = "Control Mode";
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
            return (new sbyte[] { 1, 2, 3, 4 }).ToList();
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
            return (new sbyte[] { 1, 2 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Output State from the Get Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/26/13 RCG 2.80.23 N/A    Created
        
        private void ParseOutputState(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Boolean)
                {
                    try
                    {
                        m_OutputState = (bool)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_OutputState = true;
                        WriteToLog("Failed to Get the Output State - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_OutputState = true;
                    WriteToLog("Failed to parse the Output State - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_OutputState = true;
                WriteToLog("Failed to Get the Output State - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Control State from the Get Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/26/13 RCG 2.80.23 N/A    Created

        private void ParseControlState(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_ControlState = (COSEMDisconnectControlState)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ControlState = COSEMDisconnectControlState.Connected;
                        WriteToLog("Failed to Get the Control State - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ControlState = COSEMDisconnectControlState.Connected;
                    WriteToLog("Failed to parse the Control State - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ControlState = COSEMDisconnectControlState.Connected;
                WriteToLog("Failed to Get the Control State - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Control Mode from the Get Result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/26/13 RCG 2.80.23 N/A    Created

        private void ParseControlMode(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_ControlMode = (COSEMDisconnectControlMode)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_ControlMode = COSEMDisconnectControlMode.None;
                        WriteToLog("Failed to Get the Control Mode - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_ControlMode = COSEMDisconnectControlMode.None;
                    WriteToLog("Failed to parse the Control Mode - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_ControlMode = COSEMDisconnectControlMode.None;
                WriteToLog("Failed to Get the Control Mode - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the state of the physical switch. True is closed. False open.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public bool OutputState
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    ParseOutputState(Result);
                }

                return m_OutputState;
            }
        }

        /// <summary>
        /// Gets the internal state of the disconnect switch
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDisconnectControlState ControlState
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    ParseControlState(Result);
                }

                return m_ControlState;
            }
        }

        /// <summary>
        /// Gets the disconnect mode used by the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMDisconnectControlMode ControlMode
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    ParseControlMode(Result);
                }

                return m_ControlMode;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>The actual state of the disconnect unit. (True means closed. False means open)</summary>
        protected bool m_OutputState;
        /// <summary>The internal state of the disconnect</summary>
        protected COSEMDisconnectControlState m_ControlState;
        /// <summary>Configures the behavior of the disconnect control for all triggers</summary>
        protected COSEMDisconnectControlMode m_ControlMode;

        #endregion
    }

    /// <summary>
    /// Information relating to an Emergency event
    /// </summary>
    public class COSEMEmergencyProfile
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMEmergencyProfile()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the ID of the Emergency Profile
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public ushort EmergencyProfileID
        {
            get
            {
                return m_EmergencyProfileID;
            }
            set
            {
                m_EmergencyProfileID = value;
            }
        }

        /// <summary>
        /// Gets or sets the date and time the emergency event started
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public DateTime EmergencyActivationTime
        {
            get
            {
                return m_EmergencyActivationTime;
            }
            set
            {
                m_EmergencyActivationTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration in seconds that the emergency profile is activated.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint EmergencyDuration
        {
            get
            {
                return m_EmergencyDuration;
            }
            set
            {
                m_EmergencyDuration = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_EmergencyProfileID;
        private DateTime m_EmergencyActivationTime;
        private uint m_EmergencyDuration;

        #endregion
    }

    /// <summary>
    /// Limiter Interface Class
    /// </summary>
    public class COSEMLimiterInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS protocol object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMLimiterInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            :base(logicalName, dlms)
        {
            m_ClassID = 71;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets information about the value monitored
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMMonitoredValue MonitoredValue
        {
            get
            {
                return m_MonitoredValue;
            }
        }

        /// <summary>
        /// Gets the currently active threshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public object ThresholdActive
        {
            get
            {
                return m_ThresholdActive;
            }
        }

        /// <summary>
        /// Gets the threshold when running under normal conditions
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public object ThresholdNormal
        {
            get
            {
                return m_ThresholdNormal;
            }
        }

        /// <summary>
        /// Gets the threshold when running under emergency conditions
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public object ThresholdEmergency
        {
            get
            {
                return m_ThresholdEmergency;
            }
        }

        /// <summary>
        /// Gets the number of seconds for the value to be over the threshold to trigger the action
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint MinimumOverThresholdDuration
        {
            get
            {
                return m_MinimumOverThresholdDuration;
            }
        }

        /// <summary>
        /// Gets the number of seconds for the value to be under the threshold to trigger the action
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public uint MinimumUnderThresholdDuration
        {
            get
            {
                return m_MinimumUnderThresholdDuration;
            }
        }

        /// <summary>
        /// Gets information about the emergency profile in use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMEmergencyProfile EmergencyProfile
        {
            get
            {
                return m_EmergencyProfile;
            }
        }

        /// <summary>
        /// Gets the group IDs for the emergency profiles 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public List<ushort> EmergencyProfileGroupIDs
        {
            get
            {
                return m_EmergencyProfileGroupIDs;
            }
        }

        /// <summary>
        /// Gets whether or not the emergency profile is currently in use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public bool EmergencyProfileActive
        {
            get
            {
                return m_EmergencyProfileActive;
            }
        }

        /// <summary>
        /// Gets the actions to take when the threshold has been crossed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/31/12 RCG 2.70.10 N/A    Created

        public COSEMActionSet Actions
        {
            get
            {
                return m_Actions;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>Identifies the value being monitored</summary>
        protected COSEMMonitoredValue m_MonitoredValue;
        /// <summary>The current threshold value</summary>
        protected object m_ThresholdActive;
        /// <summary>The threshold value for normal operation</summary>
        protected object m_ThresholdNormal;
        /// <summary>The threshold for the value for emergency operation</summary>
        protected object m_ThresholdEmergency;
        /// <summary>The minimum time in seconds to be over the threshold before the action is executed</summary>
        protected uint m_MinimumOverThresholdDuration;
        /// <summary>The minimum time in seconds to be under the threshold before the action is executed</summary>
        protected uint m_MinimumUnderThresholdDuration;
        /// <summary>Defines the emergency profile</summary>
        protected COSEMEmergencyProfile m_EmergencyProfile;
        /// <summary>List of group IDs of the emergency profile</summary>
        protected List<ushort> m_EmergencyProfileGroupIDs;
        /// <summary>Whether or not the emergency profile is currently active</summary>
        protected bool m_EmergencyProfileActive;
        /// <summary>The actions to be taken when the active threshold is crossed </summary>
        protected COSEMActionSet m_Actions;

        #endregion
    }

    /// <summary>
    /// Describes a push object
    /// </summary>
    public class COSEMPushObject
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMPushObject()
        {
            m_ClassID = 0;
            m_LogicalName = null;
            m_AttributeIndex = 0;
            m_DataIndex = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the Push Object</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMPushObject(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 4)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_ClassID = (ushort)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Class ID is not the expected data type.");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_LogicalName = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Logical Name is not the expected data type.");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Integer)
                        {
                            m_AttributeIndex = (sbyte)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Attribute Index is not the expected data type.");
                        }

                        if (StructureData[3].DataType == COSEMDataTypes.LongUnsigned)
                        {
                            m_DataIndex = (ushort)StructureData[3].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Data Index is not the expected data type.");
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
        //  04/23/13 RCG 2.80.22 N/A    Created
        
        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Push Object Element");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Class ID", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Logical Name", COSEMDataTypes.OctetString);
            CurrentDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Attribute Index", COSEMDataTypes.Integer);
            CurrentDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("Data Index", COSEMDataTypes.LongUnsigned);
            CurrentDefinition.Value = (ushort)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/23/13 RCG 2.80.22 N/A    Created
        
        public ObjectDefinition ToObjectDefinition()
        {
            // First get the definition object without the values 
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Now set each of the corresponding element values
            Definition.StructureDefinition[0].Value = m_ClassID;
            Definition.StructureDefinition[1].Value = m_LogicalName;
            Definition.StructureDefinition[2].Value = m_AttributeIndex;
            Definition.StructureDefinition[3].Value = m_DataIndex;

            // Set the Value field to this instance of the Push Element
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Class ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created

        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
            set
            {
                m_ClassID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Logical Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public byte[] LogicalName
        {
            get
            {
                return m_LogicalName;
            }
            set
            {
                m_LogicalName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Attribute Index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public sbyte AttributeIndex
        {
            get
            {
                return m_AttributeIndex;
            }
            set
            {
                m_AttributeIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the Data Index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public ushort DataIndex
        {
            get
            {
                return m_DataIndex;
            }
            set
            {
                m_DataIndex = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_ClassID;
        private byte[] m_LogicalName;
        private sbyte m_AttributeIndex;
        private ushort m_DataIndex;

        #endregion
    }

    /// <summary>
    /// Transport Service Types
    /// </summary>
    public enum COSEMTransportServiceTypes : byte
    {
        /// <summary>TCP</summary>
        [EnumDescription("TCP")]
        TCP = 0,
        /// <summary>UDP</summary>
        [EnumDescription("UDP")]
        UDP = 1,
        /// <summary>FTP</summary>
        [EnumDescription("FTP")]
        FTP = 2,
        /// <summary>SMTP</summary>
        [EnumDescription("SMTP")]
        SMTP = 3,
        /// <summary>SMS</summary>
        [EnumDescription("SMS")]
        SMS = 4,
        /// <summary>HDLC</summary>
        [EnumDescription("HDLC")]
        HDLC = 5,
    }

    /// <summary>
    /// Message Types
    /// </summary>
    public enum COSEMMessageTypes : byte
    {
        /// <summary>A-XDR Encoded</summary>
        [EnumDescription("A-XDR Encoded")]
        AXDR = 0,
        /// <summary>TCP</summary>
        [EnumDescription("XML Encoded")]
        XML = 1,
    }

    /// <summary>
    /// The send destination and method
    /// </summary>
    public class COSEMSendDestinationAndMethod
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMSendDestinationAndMethod()
        {
            m_TransportServiceType = COSEMTransportServiceTypes.TCP;
            m_Destination = null;
            m_MessageType = COSEMMessageTypes.AXDR;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Send Destination and Method</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMSendDestinationAndMethod(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 3)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Enum)
                        {
                            m_TransportServiceType = (COSEMTransportServiceTypes)(byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Transport Service Type data type is not correct", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_Destination = (byte[])StructureData[1].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Destination data type is not correct", "data");
                        }

                        if (StructureData[2].DataType == COSEMDataTypes.Enum)
                        {
                            m_MessageType = (COSEMMessageTypes)(byte)StructureData[2].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The Transport Service Type data type is not correct", "data");
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
        //  04/23/13 RCG 2.80.22 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Send Destination and Method");
            EnumObjectDefinition NewEnumDefinition;
            ObjectDefinition NewObjectDefinition;

            NewEnumDefinition = new EnumObjectDefinition("Transport Service", typeof(COSEMTransportServiceTypes));
            NewEnumDefinition.Value = COSEMTransportServiceTypes.UDP;
            Definition.StructureDefinition.Add(NewEnumDefinition);

            NewObjectDefinition = new ObjectDefinition("Destination", COSEMDataTypes.OctetString);
            NewObjectDefinition.Value = new byte[0];
            Definition.StructureDefinition.Add(NewObjectDefinition);

            NewEnumDefinition = new EnumObjectDefinition("Message Type", typeof(COSEMMessageTypes));
            NewEnumDefinition.Value = COSEMMessageTypes.AXDR;
            Definition.StructureDefinition.Add(NewEnumDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/23/13 RCG 2.80.22 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_TransportServiceType;
            Definition.StructureDefinition[1].Value = m_Destination;
            Definition.StructureDefinition[2].Value = m_MessageType;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Transport Service Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMTransportServiceTypes TransportServiceType
        {
            get
            {
                return m_TransportServiceType;
            }
            set
            {
                m_TransportServiceType = value;
            }
        }

        /// <summary>
        /// Gets or sets the Destination
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public byte[] Destination
        {
            get
            {
                return m_Destination;
            }
            set
            {
                m_Destination = value;
            }
        }

        /// <summary>
        /// Gets or sets the message type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMMessageTypes MessageType
        {
            get
            {
                return m_MessageType;
            }
            set
            {
                m_MessageType = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMTransportServiceTypes m_TransportServiceType;
        private byte[] m_Destination;
        private COSEMMessageTypes m_MessageType;

        #endregion
    }

    /// <summary>
    /// Push Notification Window Element
    /// </summary>
    public class COSEMWindowElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMWindowElement()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM data object containing the Window Element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMWindowElement(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.OctetString)
                        {
                            m_StartTime = new COSEMDateTime((byte[])StructureData[0].Value);
                        }
                        else
                        {
                            throw new ArgumentException("The Start Time data type is not correct", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.OctetString)
                        {
                            m_EndTime = new COSEMDateTime((byte[])StructureData[1].Value);
                        }
                        else
                        {
                            throw new ArgumentException("The End Time data type is not correct", "data");
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
        //  04/23/13 RCG 2.80.22 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Window Element");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("Start Time", COSEMDataTypes.DateTime);
            CurrentDefinition.Value = new COSEMDateTime();
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ObjectDefinition("End Time", COSEMDataTypes.DateTime);
            CurrentDefinition.Value = new COSEMDateTime();
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/23/13 RCG 2.80.22 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_StartTime;
            Definition.StructureDefinition[1].Value = m_EndTime;

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the window's start time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMDateTime StartTime
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

        /// <summary>
        /// Gets or set the window's end time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/27/13 RCG 2.80.12 N/A    Created
        
        public COSEMDateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
            set
            {
                m_EndTime = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMDateTime m_StartTime;
        private COSEMDateTime m_EndTime;

        #endregion
    }

    /// <summary>
    /// Push Setup Interface Class
    /// </summary>
    public class COSEMPushSetupInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the interface class</param>
        /// <param name="dlms">The DLMS Protocol object for the current device</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public COSEMPushSetupInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            :base(logicalName, dlms)
        {
            m_ClassID = 40;
            m_Version = 0;

            m_PushObjectList = null;
            m_SendDestinationAndMethod = null;
            m_CommunicationWindow = null;
            m_RandomizationStartInterval = 0;
            m_NumberOfRetries = 0;
            m_RepetitionDelay = 0;
        }

        /// <summary>
        /// Sends the Push Notification
        /// </summary>
        /// <returns>The result of the action</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/01/13 RCG 2.80.15 N/A    Created
        
        public ActionResults Push()
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                ActionResponseWithOptionalData Response;
                COSEMData ParameterData = new COSEMData();

                // Build up the parameter data first
                ParameterData.DataType = COSEMDataTypes.Integer;
                ParameterData.Value = (sbyte)0;

                Response = m_DLMS.Action(m_ClassID, m_LogicalName, 1, ParameterData);

                Result = Response.Result;
            }

            return Result;
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

            switch(attributeID)
            {
                case 2:
                {
                    StructureObjectDefinition PushObjectElementDefinition = COSEMPushObject.GetStructureDefinition();
                    ArrayObjectDefinition PushObjectListDefinition = new ArrayObjectDefinition("Push Object List", PushObjectElementDefinition);

                    List<COSEMPushObject> CurrentList = PushObjectList;

                    foreach (COSEMPushObject CurrentPushObject in CurrentList)
                    {
                        PushObjectListDefinition.Elements.Add(CurrentPushObject.ToObjectDefinition());
                    }

                    AttributeDefinition = PushObjectListDefinition;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = SendDestinationAndMethod.ToObjectDefinition();
                    break;
                }
                case 4:
                {
                    StructureObjectDefinition WindowElement = COSEMWindowElement.GetStructureDefinition();
                    ArrayObjectDefinition WindowElementListDefinition = new ArrayObjectDefinition("Communication Windows", WindowElement);

                    List<COSEMWindowElement> CurrentList = CommunicationWindow;

                    foreach (COSEMWindowElement CurrentElement in CurrentList)
                    {
                        WindowElementListDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                    }

                    AttributeDefinition = WindowElementListDefinition;
                    break;
                }
                case 5:
                {
                    AttributeDefinition = new ObjectDefinition("Randomization Start Interval", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = RandomizationStartInterval;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new ObjectDefinition("Number of Retries", COSEMDataTypes.Unsigned);
                    AttributeDefinition.Value = NumberOfRetries;
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("Repetition Delay", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = RepetitionDelay;
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
                    Name = "Push Object List";
                    break;
                }
                case 3:
                {
                    Name = "Send Destination and Method";
                    break;
                }
                case 4:
                {
                    Name = "Communication Windows";
                    break;
                }
                case 5:
                {
                    Name = "Randomization Start Interval";
                    break;
                }
                case 6:
                {
                    Name = "Number of Retries";
                    break;
                }
                case 7:
                {
                    Name = "Repetition Delay";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7 }).ToList();
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
            return (new sbyte[] { 1 }).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the Push Object List
        /// </summary>
        /// <param name="data">The result of the get request</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParsePushObjectList(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];
                        m_PushObjectList = new List<COSEMPushObject>();

                        foreach (COSEMData CurrentItem in ArrayData)
                        {
                            m_PushObjectList.Add(new COSEMPushObject(CurrentItem));
                        }
                    }
                    catch (Exception e)
                    {
                        m_PushObjectList = null;
                        WriteToLog("Failed to Get the Push Object List - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_PushObjectList = null;
                    WriteToLog("Failed to parse the Push Object List - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_PushObjectList = null;
                WriteToLog("Failed to Get the Push Object List - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Send Destination and Method from a get result
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseSendDestinationAndMethod(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Structure)
                {
                    try
                    {
                        m_SendDestinationAndMethod = new COSEMSendDestinationAndMethod(data.DataValue);
                    }
                    catch (Exception e)
                    {
                        m_SendDestinationAndMethod = null;
                        WriteToLog("Failed to Get the Send Destination and Method - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_SendDestinationAndMethod = null;
                    WriteToLog("Failed to parse the Send Destination and Method - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_SendDestinationAndMethod = null;
                WriteToLog("Failed to Get the Send Destination and Method - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Communication Window from the get result
        /// </summary>
        /// <param name="data">The get result data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseCommunicationWindow(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];
                        m_CommunicationWindow = new List<COSEMWindowElement>();

                        foreach (COSEMData CurrentItem in ArrayData)
                        {
                            m_CommunicationWindow.Add(new COSEMWindowElement(CurrentItem));
                        }
                    }
                    catch (Exception e)
                    {
                        m_CommunicationWindow = null;
                        WriteToLog("Failed to Get the Communication Window - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_CommunicationWindow = null;
                    WriteToLog("Failed to parse the Communication Window - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CommunicationWindow = null;
                WriteToLog("Failed to Get the Communication Window - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Randomization Start Interval from the get results
        /// </summary>
        /// <param name="data">The results of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseRandomizationStartInterval(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_RandomizationStartInterval = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_RandomizationStartInterval = 0;
                        WriteToLog("Failed to Get the Randomization Start Interval - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_RandomizationStartInterval = 0;
                    WriteToLog("Failed to parse the Randomization Start Interval - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_RandomizationStartInterval = 0;
                WriteToLog("Failed to Get the Randomization Start Interval - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Number of Retries from the get results
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseNumberOfRetries(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Unsigned)
                {
                    try
                    {
                        m_NumberOfRetries = (byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_NumberOfRetries = 0;
                        WriteToLog("Failed to Get the Number of Retries - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_NumberOfRetries = 0;
                    WriteToLog("Failed to parse the Number of Retries - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_NumberOfRetries = 0;
                WriteToLog("Failed to Get the Number of Retries - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Repetition Delay from the get results
        /// </summary>
        /// <param name="data"></param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        private void ParseRepetitionDelay(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_RepetitionDelay = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_RepetitionDelay = 0;
                        WriteToLog("Failed to Get the Repetition Delay - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_RepetitionDelay = 0;
                    WriteToLog("Failed to parse the Repetition Delay - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_RepetitionDelay = 0;
                WriteToLog("Failed to Get the Repetition Delay - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Push
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created

        public List<COSEMPushObject> PushObjectList
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    if (Result != null)
                    {
                        ParsePushObjectList(Result);
                    }
                }

                return m_PushObjectList;
            }
        }

        /// <summary>
        /// Gets the Send Destination and Method
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public COSEMSendDestinationAndMethod SendDestinationAndMethod
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    if (Result != null)
                    {
                        ParseSendDestinationAndMethod(Result);
                    }
                }

                return m_SendDestinationAndMethod;
            }
        }

        /// <summary>
        /// Gets the Communication Window
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public List<COSEMWindowElement> CommunicationWindow
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    if (Result != null)
                    {
                        ParseCommunicationWindow(Result);
                    }
                }

                return m_CommunicationWindow;
            }
        }

        /// <summary>
        /// Gets the Randomization Start Interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public ushort RandomizationStartInterval
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    if (Result != null)
                    {
                        ParseRandomizationStartInterval(Result);
                    }
                }

                return m_RandomizationStartInterval;
            }
        }

        /// <summary>
        /// Gets the Number of Retries
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created
        
        public byte NumberOfRetries
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    if (Result != null)
                    {
                        ParseNumberOfRetries(Result);
                    }
                }

                return m_NumberOfRetries;
            }
        }

        /// <summary>
        /// Gets the Repetition Delay
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  03/26/13 RCG 2.80.11 N/A    Created

        public ushort RepetitionDelay
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    if (Result != null)
                    {
                        ParseRepetitionDelay(Result);
                    }
                }

                return m_RepetitionDelay;
            }
        }

        #endregion

        #region Member Variables

        private List<COSEMPushObject> m_PushObjectList;
        private COSEMSendDestinationAndMethod m_SendDestinationAndMethod;
        private List<COSEMWindowElement> m_CommunicationWindow;
        private ushort m_RandomizationStartInterval;
        private byte m_NumberOfRetries;
        private ushort m_RepetitionDelay;

        #endregion
    }

    /// <summary>
    /// HDLC Communication Speeds
    /// </summary>
    public enum COSEMCommSpeeds : byte
    {
        /// <summary>300 baud</summary>
        [EnumDescription("300")]
        Baud300 = 0,
        /// <summary>600 baud</summary>
        [EnumDescription("600")]
        Baud600 = 1,
        /// <summary>1200 baud</summary>
        [EnumDescription("1200")]
        Baud1200 = 2,
        /// <summary>2400 baud</summary>
        [EnumDescription("2400")]
        Baud2400 = 3,
        /// <summary>4800 baud</summary>
        [EnumDescription("4800")]
        Baud4800 = 4,
        /// <summary>9600 baud</summary>
        [EnumDescription("9600")]
        Baud9600 = 5,
        /// <summary>19200 baud</summary>
        [EnumDescription("19200")]
        Baud19200 = 6,
        /// <summary>38400 baud</summary>
        [EnumDescription("38400")]
        Baud38400 = 7,
        /// <summary>57600 baud</summary>
        [EnumDescription("57600")]
        Baud57600 = 8,
        /// <summary>115200 baud</summary>
        [EnumDescription("115200")]
        Baud115200 = 9,
        /// <summary>230400 baud</summary>
        [EnumDescription("230400")]
        Baud230400 = 10,
        /// <summary>460800 baud</summary>
        [EnumDescription("460800")]
        Baud460800 = 11,
        /// <summary>500000 baud</summary>
        [EnumDescription("500000")]
        Baud500000 = 12,
        /// <summary>576000 baud</summary>
        [EnumDescription("576000")]
        Baud576000 = 13,
        /// <summary>921600 baud</summary>
        [EnumDescription("921600")]
        Baud921600 = 14,
    }

    /// <summary>
    /// HDLC Setup Interface Class
    /// </summary>
    public class COSEMHDLCSetupInterfaceClass : COSEMInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "HDLC Setting" IEC HDLC setup COSEM object
        /// </summary>
        public static readonly byte[] HDLC_SETTING_LN = new byte[] { 0, 0, 22, 0, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the interface class</param>
        /// <param name="dlms">The DLMS protocol object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public COSEMHDLCSetupInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base (logicalName, dlms)
        {
            m_ClassID = 23;
            m_Version = 1;

            m_CommSpeed = COSEMCommSpeeds.Baud9600;
            m_WindowSizeTransmit = 1;
            m_WindowSizeReceive = 1;
            m_MaxInfoFieldLengthTransmit = 128;
            m_MaxInfoFieldLengthReceived = 128;
            m_InterOctetTimeOut = 25;
            m_InactivityTimeOut = 120;
            m_DeviceAddress = 0;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/07/13 RCG 2.80.27 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    AttributeDefinition = new EnumObjectDefinition("Comm Speed", typeof(COSEMCommSpeeds));
                    AttributeDefinition.Value = CommSpeed;
                    break;
                }
                case 3:
                {
                    AttributeDefinition = new ObjectDefinition("Transmit Window Size", COSEMDataTypes.Unsigned);
                    AttributeDefinition.Value = WindowSizeTransmit;
                    break;
                }
                case 4:
                {
                    AttributeDefinition = new ObjectDefinition("Receive Window Size", COSEMDataTypes.Unsigned);
                    AttributeDefinition.Value = WindowSizeReceived;
                    break;
                }
                case 5:
                {
                    AttributeDefinition = new ObjectDefinition("Transmit Max Info Field Length", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = MaxInfoFieldLengthTransmit;
                    break;
                }
                case 6:
                {
                    AttributeDefinition = new ObjectDefinition("Receive Max Info Field Length", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = MaxInfoFieldLengthReceived;
                    break;
                }
                case 7:
                {
                    AttributeDefinition = new ObjectDefinition("Inter Octet Time Out", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = InterOctetTimeOut;
                    break;
                }
                case 8:
                {
                    AttributeDefinition = new ObjectDefinition("Inactivity Time Out", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = InactivityTimeOut;
                    break;
                }
                case 9:
                {
                    AttributeDefinition = new ObjectDefinition("Device Address", COSEMDataTypes.LongUnsigned);
                    AttributeDefinition.Value = DeviceAddress;
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
                    Name = "Comm Speed";
                    break;
                }
                case 3:
                {
                    Name = "Transmit Window Size";
                    break;
                }
                case 4:
                {
                    Name = "Receive Window Size";
                    break;
                }
                case 5:
                {
                    Name = "Transmit Max Info Field Length";
                    break;
                }
                case 6:
                {
                    Name = "Receive Max Info Field Length";
                    break;
                }
                case 7:
                {
                    Name = "Inter Octet Time Out";
                    break;
                }
                case 8:
                {
                    Name = "Inactivity Time Out";
                    break;
                }
                case 9:
                {
                    Name = "Device Address";
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
            return (new sbyte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }).ToList();
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

        #region Private Methods

        /// <summary>
        /// Parses the Comm Speed
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        private void ParseCommSpeed(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Enum)
                {
                    try
                    {
                        m_CommSpeed = (COSEMCommSpeeds)(byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_CommSpeed = COSEMCommSpeeds.Baud9600;
                        WriteToLog("Failed to Get the Comm Speed - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_CommSpeed = COSEMCommSpeeds.Baud9600;
                    WriteToLog("Failed to parse the Comm Speed - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_CommSpeed = COSEMCommSpeeds.Baud9600;
                WriteToLog("Failed to Get the Comm Speed - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Transmit Window Size
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        private void ParseWindowSizeTransmit(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Unsigned)
                {
                    try
                    {
                        m_WindowSizeTransmit = (byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_WindowSizeTransmit = 0;
                        WriteToLog("Failed to Get the Transmit Window Size - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_WindowSizeTransmit = 0;
                    WriteToLog("Failed to parse the Transmit Window Size - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_WindowSizeTransmit = 0;
                WriteToLog("Failed to Get the Transmit Window Size - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Receive Window Size
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseWindowSizeReceived(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Unsigned)
                {
                    try
                    {
                        m_WindowSizeReceive = (byte)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_WindowSizeReceive = 0;
                        WriteToLog("Failed to Get the Receive Window Size - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_WindowSizeReceive = 0;
                    WriteToLog("Failed to parse the Receive Window Size - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_WindowSizeReceive = 0;
                WriteToLog("Failed to Get the Receive Window Size - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 4, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Tranmit Max Info Field Length
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseMaxInfoFieldLengthTransmit(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_MaxInfoFieldLengthTransmit = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_MaxInfoFieldLengthTransmit = 0;
                        WriteToLog("Failed to Get the Transmit Max Info Field Length - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_MaxInfoFieldLengthTransmit = 0;
                    WriteToLog("Failed to parse the Transmit Max Info Field Length - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_MaxInfoFieldLengthTransmit = 0;
                WriteToLog("Failed to Get the Transmit Max Info Field Length - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 5, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Receive Max Info Field Length
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseMaxInfoFieldLengthReceived(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_MaxInfoFieldLengthReceived = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_MaxInfoFieldLengthReceived = 0;
                        WriteToLog("Failed to Get the Receive Max Info Field Length - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_MaxInfoFieldLengthReceived = 0;
                    WriteToLog("Failed to parse the Receive Max Info Field Length - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_MaxInfoFieldLengthReceived = 0;
                WriteToLog("Failed to Get the Receive Max Info Field Length - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 6, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Inter Octet Time Out
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseInterOctetTimeOut(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_InterOctetTimeOut = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_InterOctetTimeOut = 0;
                        WriteToLog("Failed to Get the Inter Octet Time Out - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_InterOctetTimeOut = 0;
                    WriteToLog("Failed to parse the Inter Octet Time Out - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_InterOctetTimeOut = 0;
                WriteToLog("Failed to Get the Inter Octet Time Out - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Inactivity Time Out
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseInactivityTimeOut(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_InactivityTimeOut = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_InactivityTimeOut = 0;
                        WriteToLog("Failed to Get the Inactivity Time Out - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_InactivityTimeOut = 0;
                    WriteToLog("Failed to parse the Inactivity Time Out - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_InactivityTimeOut = 0;
                WriteToLog("Failed to Get the Inactivity Time Out - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Device Address
        /// </summary>
        /// <param name="data">The result of the get</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created

        private void ParseDeviceAddress(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.LongUnsigned)
                {
                    try
                    {
                        m_DeviceAddress = (ushort)data.DataValue.Value;
                    }
                    catch (Exception e)
                    {
                        m_DeviceAddress = 0;
                        WriteToLog("Failed to Get the Device Address - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_DeviceAddress = 0;
                    WriteToLog("Failed to parse the Device Address - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_DeviceAddress = 0;
                WriteToLog("Failed to Get the Device Address - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 9, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Comm Speed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public COSEMCommSpeeds CommSpeed
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    ParseCommSpeed(Result);
                }

                return m_CommSpeed;
            }
        }

        /// <summary>
        /// Gets the Maximum Transmit Window Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public byte WindowSizeTransmit
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    ParseWindowSizeTransmit(Result);
                }

                return m_WindowSizeTransmit;
            }
        }

        /// <summary>
        /// Gets the Maximum Receive Window Size
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public byte WindowSizeReceived
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 4);

                    ParseWindowSizeReceived(Result);
                }

                return m_WindowSizeReceive;
            }
        }

        /// <summary>
        /// Gets the maximum information field length that the device can transmit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public ushort MaxInfoFieldLengthTransmit
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 5);

                    ParseMaxInfoFieldLengthTransmit(Result);
                }

                return m_MaxInfoFieldLengthTransmit;
            }
        }

        /// <summary>
        /// Gets the maximum information field length that the device can receive
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public ushort MaxInfoFieldLengthReceived
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 6);

                    ParseMaxInfoFieldLengthReceived(Result);
                }

                return m_MaxInfoFieldLengthReceived;
            }
        }

        /// <summary>
        /// Gets the inter octet time out (ms)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public ushort InterOctetTimeOut
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 7);

                    ParseInterOctetTimeOut(Result);
                }

                return m_InterOctetTimeOut;
            }
            set
            {
                if (m_DLMS.IsConnected)
                {
                    DataAccessResults Result;
                    COSEMData DataValue = new COSEMData();

                    m_InterOctetTimeOut = value;

                    DataValue.DataType = COSEMDataTypes.LongUnsigned;
                    DataValue.Value = m_InterOctetTimeOut;

                    Result = m_DLMS.Set(m_ClassID, m_LogicalName, 7, DataValue);

                    if (Result != DataAccessResults.Success)
                    {
                        throw new COSEMException(m_LogicalName, 7, COSEMExceptionRequestType.Set, "The Inter Octet time out could not be set. Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the amount of time before a time out occurs due to inactivity (s)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public ushort InactivityTimeOut
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 8);

                    ParseInactivityTimeOut(Result);
                }

                return m_InactivityTimeOut;
            }
            set
            {
                if (m_DLMS.IsConnected)
                {
                    DataAccessResults Result;
                    COSEMData DataValue = new COSEMData();

                    m_InactivityTimeOut = value;

                    DataValue.DataType = COSEMDataTypes.LongUnsigned;
                    DataValue.Value = m_InactivityTimeOut;

                    Result = m_DLMS.Set(m_ClassID, m_LogicalName, 8, DataValue);

                    if (Result != DataAccessResults.Success)
                    {
                        throw new COSEMException(m_LogicalName, 8, COSEMExceptionRequestType.Set, "The Inactivity time out could not be set. Reason: " + EnumDescriptionRetriever.RetrieveDescription(Result));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the physical address of the device
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  04/02/13 RCG 2.80.16 N/A    Created
        
        public ushort DeviceAddress
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 9);

                    ParseDeviceAddress(Result);
                }

                return m_DeviceAddress;
            }
        }

        #endregion

        #region Member Variables

        private COSEMCommSpeeds m_CommSpeed;
        private byte m_WindowSizeTransmit;
        private byte m_WindowSizeReceive;
        private ushort m_MaxInfoFieldLengthTransmit;
        private ushort m_MaxInfoFieldLengthReceived;
        private ushort m_InterOctetTimeOut;
        private ushort m_InactivityTimeOut;
        private ushort m_DeviceAddress;

        #endregion
    }

    /// <summary>
    /// Interrogation Builder Template object
    /// </summary>
    public class COSEMInterrogationBuilderTemplate
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public COSEMInterrogationBuilderTemplate()
        {
            m_ID = 0;
            m_PushObjects = new List<COSEMPushObject>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The COSEM Data object containing the structure</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public COSEMInterrogationBuilderTemplate(COSEMData data)
        {
            if (data != null)
            {
                if (data.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = data.Value as COSEMData[];

                    if (StructureData != null && StructureData.Length == 2)
                    {
                        if (StructureData[0].DataType == COSEMDataTypes.Unsigned)
                        {
                            m_ID = (byte)StructureData[0].Value;
                        }
                        else
                        {
                            throw new ArgumentException("The ID data type is not correct", "data");
                        }

                        if (StructureData[1].DataType == COSEMDataTypes.Array)
                        {
                            COSEMData[] ArrayData = StructureData[1].Value as COSEMData[];

                            m_PushObjects = new List<COSEMPushObject>();

                            if (ArrayData != null)
                            {
                                foreach (COSEMData CurrentData in ArrayData)
                                {
                                    m_PushObjects.Add(new COSEMPushObject(CurrentData));
                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("The Push Object List data type is not correct", "data");
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
        //  07/12/13 RCG 2.80.52 N/A    Created

        public static StructureObjectDefinition GetStructureDefinition()
        {
            StructureObjectDefinition Definition = new StructureObjectDefinition("Interrogation Builder Template");
            ObjectDefinition CurrentDefinition;

            CurrentDefinition = new ObjectDefinition("ID", COSEMDataTypes.Unsigned);
            CurrentDefinition.Value = (byte)0;
            Definition.StructureDefinition.Add(CurrentDefinition);

            CurrentDefinition = new ArrayObjectDefinition("Push Object List", COSEMPushObject.GetStructureDefinition());
            CurrentDefinition.Value = COSEMPushObject.GetStructureDefinition();
            Definition.StructureDefinition.Add(CurrentDefinition);

            return Definition;
        }

        /// <summary>
        /// Converts the current object to an Object Definition
        /// </summary>
        /// <returns>The Push Object Element as an object definition</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created

        public ObjectDefinition ToObjectDefinition()
        {
            // This will set up the definition
            StructureObjectDefinition Definition = GetStructureDefinition();
            ArrayObjectDefinition ArrayDefinition = null;

            // Add in the current values
            Definition.StructureDefinition[0].Value = m_ID;

            ArrayDefinition = Definition.StructureDefinition[1] as ArrayObjectDefinition;

            if (ArrayDefinition != null)
            {
                ArrayDefinition.Elements.Clear();

                foreach (COSEMPushObject CurrentElement in m_PushObjects)
                {
                    ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                }
            }

            // Set the Value to this instance of the object
            Definition.Value = this;

            return Definition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the template ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created

        public byte ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

        /// <summary>
        /// Gets or sets the Push Object List
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public List<COSEMPushObject> PushObjectList
        {
            get
            {
                return m_PushObjects;
            }
            set
            {
                if (value != null)
                {
                    m_PushObjects = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "The Push Object List may not be set to null");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_ID;
        private List<COSEMPushObject> m_PushObjects;

        #endregion
    }

    /// <summary>
    /// Interrogation Builder Interface Class
    /// </summary>
    public class COSEMInterrogationBuilderInterfaceClass : COSEMInterfaceClass
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the interface class</param>
        /// <param name="dlms">The DLMS protocol object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public COSEMInterrogationBuilderInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 8192;
            m_Version = 0;

            m_Templates = new List<COSEMInterrogationBuilderTemplate>();
            m_PushStartElements = new List<sbyte>();
        }

        /// <summary>
        /// Builds a push setup object
        /// </summary>
        /// <param name="logicalName">The Logical Name to use</param>
        /// <param name="templateIDs">The list of template IDs to use</param>
        /// <returns>The result of the action request</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public ActionResults BuildPushSetup(byte[] logicalName, byte[] templateIDs)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                if (logicalName != null)
                {
                    if (templateIDs != null)
                    {
                        ActionResponseWithOptionalData Response;
                        COSEMData ParameterData = new COSEMData();
                        COSEMData[] StructureData = new COSEMData[2];
                        COSEMData[] ArrayData = new COSEMData[templateIDs.Length];

                        // Build up the parameter data first
                        ParameterData.DataType = COSEMDataTypes.Structure;

                        StructureData[0] = new COSEMData();
                        StructureData[0].DataType = COSEMDataTypes.OctetString;
                        StructureData[0].Value = logicalName;

                        StructureData[1] = new COSEMData();
                        StructureData[1].DataType = COSEMDataTypes.Array;

                        for (int iIndex = 0; iIndex < templateIDs.Length; iIndex++)
                        {
                            ArrayData[iIndex] = new COSEMData();
                            ArrayData[iIndex].DataType = COSEMDataTypes.Unsigned;
                            ArrayData[iIndex].Value = templateIDs[iIndex];
                        }

                        StructureData[1].Value = ArrayData;

                        ParameterData.Value = StructureData;

                        Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                        Result = Response.Result;
                    }
                    else
                    {
                        throw new ArgumentNullException("templatedIDs", "The Template IDs may not be null");
                    }
                }
                else
                {
                    throw new ArgumentNullException("logicalName", "The Logical Name may not be null");
                }
            }

            return Result;
        }

        /// <summary>
        /// Removes the specified push setup
        /// </summary>
        /// <param name="logicalName">The logical name of the push setup to remove</param>
        /// <returns>The result of the action request</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public ActionResults RemovePushSetup(byte[] logicalName)
        {
            ActionResults Result = ActionResults.Other;

            if (m_DLMS.IsConnected)
            {
                if (logicalName != null)
                {
                    ActionResponseWithOptionalData Response;
                    COSEMData ParameterData = new COSEMData();

                    // Build up the parameter data first
                    ParameterData.DataType = COSEMDataTypes.OctetString;
                    ParameterData.Value = logicalName;

                    Response = m_DLMS.Action(m_ClassID, m_LogicalName, 2, ParameterData);

                    Result = Response.Result;
                }
                else
                {
                    throw new ArgumentNullException("logicalName", "The Logical Name may not be null");
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets the specified attribute as a self defined value
        /// </summary>
        /// <param name="attributeID">The ID of the Attribute to get</param>
        /// <returns>The definition of attribute's value</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created

        public override ObjectDefinition GetAttributeWithDefinition(sbyte attributeID)
        {
            ObjectDefinition AttributeDefinition = null;

            switch (attributeID)
            {
                case 2:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Templates", COSEMInterrogationBuilderTemplate.GetStructureDefinition());
                    List<COSEMInterrogationBuilderTemplate> CurrentTemplates = Templates;

                    foreach (COSEMInterrogationBuilderTemplate CurrentElement in CurrentTemplates)
                    {
                        ArrayDefinition.Elements.Add(CurrentElement.ToObjectDefinition());
                    }

                    AttributeDefinition = ArrayDefinition;
                    break;
                }
                case 3:
                {
                    ArrayObjectDefinition ArrayDefinition = new ArrayObjectDefinition("Push Start Elements", new ObjectDefinition("Element", COSEMDataTypes.Integer));
                    List<sbyte> CurrentPushStartElements = PushStartElements;

                    for (int iIndex = 0; iIndex < CurrentPushStartElements.Count; iIndex++)
                    {
                        ObjectDefinition CurrentElement = new ObjectDefinition("[" + iIndex.ToString(CultureInfo.InvariantCulture) + "]", COSEMDataTypes.Integer);
                        CurrentElement.Value = CurrentPushStartElements[iIndex];

                        ArrayDefinition.Elements.Add(CurrentElement);
                    }

                    AttributeDefinition = ArrayDefinition;

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
                    Name = "Templates";
                    break;
                }
                case 3:
                {
                    Name = "Push Start Elements";
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
            return (new sbyte[] { 1, 2, 3 }).ToList();
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

        #region Private Methods

        /// <summary>
        /// Parses the Template List
        /// </summary>
        /// <param name="data">The result of the get request</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        private void ParseTemplates(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        m_Templates = new List<COSEMInterrogationBuilderTemplate>();

                        foreach (COSEMData CurrentElement in ArrayData)
                        {
                            try
                            {
                                m_Templates.Add(new COSEMInterrogationBuilderTemplate(CurrentElement));
                            }
                            catch (Exception e)
                            {
                                WriteToLog("Failed to Get the Templates - Exception while parsing Templates. Message: " + e.Message);
                                m_Templates = null;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_Templates = null;
                        WriteToLog("Failed to Get the Templates - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_Templates = null;
                    WriteToLog("Failed to parse the Templates - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_Templates = null;
                WriteToLog("Failed to Get the Templates - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 2, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        /// <summary>
        /// Parses the Push Start Elements
        /// </summary>
        /// <param name="data">The result of the get request</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created

        private void ParsePushStartElements(GetDataResult data)
        {
            if (data.GetDataResultType == GetDataResultChoices.Data)
            {
                if (data.DataValue.DataType == COSEMDataTypes.Array)
                {
                    try
                    {
                        COSEMData[] ArrayData = data.DataValue.Value as COSEMData[];

                        m_PushStartElements = new List<sbyte>();

                        foreach (COSEMData CurrentElement in ArrayData)
                        {
                            try
                            {
                                m_PushStartElements.Add((sbyte)CurrentElement.Value);
                            }
                            catch (Exception e)
                            {
                                WriteToLog("Failed to Get the Push Start Elements - Exception while parsing Push Start Elements. Message: " + e.Message);
                                m_PushStartElements = null;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_PushStartElements = null;
                        WriteToLog("Failed to Get the Push Start Elements - Exception Occurred while parsing the data. Message: " + e.Message);
                        throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Exception Occurred while parsing the response. Message: " + e.Message);
                    }
                }
                else
                {
                    m_PushStartElements = null;
                    WriteToLog("Failed to parse the Push Start Elements - Unexpected data type.");
                    throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Unexpected data type received. Received: " + data.DataValue.DataType.ToDescription());
                }
            }
            else
            {
                // We received some sort of error message from the get
                m_PushStartElements = null;
                WriteToLog("Failed to Get the Push Start Elements - Reason: " + EnumDescriptionRetriever.RetrieveDescription(data.DataAccessResult));
                throw new COSEMException(m_LogicalName, 3, COSEMExceptionRequestType.Get, "Request Failed. Reason: " + data.DataAccessResult.ToDescription());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of templates
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created
        
        public List<COSEMInterrogationBuilderTemplate> Templates
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 2);

                    ParseTemplates(Result);
                }

                return m_Templates;
            }
        }

        /// <summary>
        /// Gets the Push Start Elements
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/12/13 RCG 2.80.52 N/A    Created

        public List<sbyte> PushStartElements
        {
            get
            {
                if (m_DLMS.IsConnected)
                {
                    GetDataResult Result = m_DLMS.Get(m_ClassID, m_LogicalName, 3);

                    ParsePushStartElements(Result);
                }

                return m_PushStartElements;
            }
        }

        #endregion

        #region Member Variables

        private List<COSEMInterrogationBuilderTemplate> m_Templates;
        private List<sbyte> m_PushStartElements;

        #endregion
    }

    /// <summary>
    /// Meter Model interface class
    /// </summary>
    public class COSEMIDNumberInterfaceClass : COSEMDataInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "ID Number" Data COSEM object
        /// </summary>
        public static readonly byte[] ID_NUMBER_LN = new byte[] { 0, 0, 96, 1, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public COSEMIDNumberInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 1;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Meter ID String
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public String MeterID
        {
            get
            {
                COSEMData CurrentValue = Value;

                String strMeterID = System.Text.Encoding.Default.GetString(CurrentValue.Data);

                return strMeterID;
            }
        }

        #endregion
    }

    /// <summary>
    /// Meter Model interface class
    /// </summary>
    public class COSEMMeterModelInterfaceClass : COSEMDataInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Meter Model" Data COSEM object
        /// </summary>
        public static readonly byte[] METER_MODEL_LN = new byte[] { 1, 0, 0, 0, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public COSEMMeterModelInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 1;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Load Control Settings
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public String MeterModel
        {
            get
            {
                COSEMData CurrentValue = Value;

                String strModel = System.Text.Encoding.Default.GetString(CurrentValue.Data);

                return strModel;
            }
        }

        #endregion
    }

    /// <summary>
    /// Number of Digits interface class
    /// </summary>
    public class COSEMNumberOfDigitsInterfaceClass : COSEMDataInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Number of Digits" Data COSEM object
        /// </summary>
        public static readonly byte[] NUMBER_DIGITS_LN = new byte[] { 1, 65, 0, 0, 0, 255 };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public COSEMNumberOfDigitsInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 1;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Number of digits
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public int NumberOfDigits
        {
            get
            {
                COSEMData CurrentValue = Value;

                int intDigits = CurrentValue.Data[0];

                return intDigits;
            }
        }

        #endregion
    }

    /// <summary>
    /// Phase wire type interface class
    /// </summary>
    public class COSEMMeterFunctionInterfaceClass : COSEMDataInterfaceClass
    {
        #region Constants

        /// <summary>
        /// The logical name for the "Number of Digits" Data COSEM object
        /// </summary>
        public static readonly byte[] METER_FUNCTION_LN = new byte[] { 1, 65, 0, 0, 1, 255 };

        /// <summary>
        /// The list switching codes
        /// </summary>
        public enum SwitchOperationType
        {
            /// <summary>
            /// 
            /// </summary>
            NoSwitchingFunction = 0,
            /// <summary>
            /// 
            /// </summary>
            ManualSwitchingFunction = 1,
            /// <summary>
            /// 
            /// </summary>
            CurrentControlledSwitchingFunction = 2,
            /// <summary>
            /// 
            /// </summary>
            TimeControlledSwitchingFunction = 3,
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logicalName">The logical name of the object</param>
        /// <param name="dlms">The DLMS object for the current session</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created

        public COSEMMeterFunctionInterfaceClass(byte[] logicalName, DLMSProtocol dlms)
            : base(logicalName, dlms)
        {
            m_ClassID = 1;
            m_Version = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the type of switching function
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created
        public SwitchOperationType SwitchOperation
        {
            get
            {
                string meterFunction = GetMeterFunctionString();

                return (SwitchOperationType)((byte)meterFunction[0] - (byte)'0');
            }
        }


        /// <summary>
        /// Returns a flag indicating if a battery is present or not
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created
        public Boolean BatteryPresent
        {
            get
            {
                string meterFunction = GetMeterFunctionString();

                return (meterFunction[1] != '0');
            }
        }

        /// <summary>
        /// Returns a flag indicating if a battery is present or not
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        // 08/05/13 mah 2.85.10        Created
        public Boolean BidirectionalMeasurement
        {
            get
            {
                string meterFunction = GetMeterFunctionString();

                return (meterFunction[2] != '0');
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Gets the meter function string. 
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/19/13 jkw 2.85.46        Created
        private string GetMeterFunctionString()
        {
            string meterFunction = string.Empty;

            if (Value.DataType == COSEMDataTypes.VisibleString)
            {
                meterFunction = (string)Value.Value;

                if (meterFunction == null || meterFunction.Length < 3)
                {
                    throw new ArgumentException("The Meter Function string length is not >= 3");
                }
            }
            else
            {
                throw new ArgumentException("The Meter Function is not of type string.");
            }
            return meterFunction;
        }
        #endregion
    }

}
