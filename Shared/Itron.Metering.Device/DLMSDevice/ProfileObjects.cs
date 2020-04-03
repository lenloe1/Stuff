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
//                          Copyright © 2013 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Communications.DLMS;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device.DLMSDevice
{

    /// <summary>
    /// Generic Profile Interval object
    /// </summary>
    public class GenericProfileInterval
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileInterval()
        {
            m_Parent = null;
            m_Values = new List<COSEMData>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The Profile Data containing the interval</param>
        /// <param name="intervalData">The Interval Data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileInterval(GenericProfileData parent, COSEMData intervalData)
            : this()
        {
            m_Parent = parent;

            Parse(intervalData);
        }

        /// <summary>
        /// Parses the interval from the specified COSEM data
        /// </summary>
        /// <param name="intervalData">The COSEM Data object containing the interval</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public void Parse(COSEMData intervalData)
        {
            if (intervalData != null)
            {
                if (intervalData.DataType == COSEMDataTypes.Structure)
                {
                    COSEMData[] StructureData = intervalData.Value as COSEMData[];

                    if (StructureData != null)
                    {
                        m_Values.Clear();

                        // Make sure the number of Columns is correct
                        if (m_Parent != null && m_Parent.Columns.Count != StructureData.Length)
                        {
                            throw new ArgumentException("The interval data does not contain the correct number of columns", "intervalData");
                        }

                        m_Values.AddRange(StructureData);
                    }
                }
                else
                {
                    throw new ArgumentException("The interval data is not a structure.", "intervalData");
                }
            }
            else
            {
                throw new ArgumentNullException("intervalData", "The intervalData may not be null");
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="column">The Capture Object of the column to retrieve</param>
        /// <returns>The interval value of the specified capture object</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData this[COSEMProfileCaptureObject column]
        {
            get
            {
                COSEMData DataValue = null;
                int Index = -1;

                if (m_Parent != null)
                {
                    Index = m_Parent.Columns.FindIndex(c => c.Equals(column));

                    if (Index >= 0)
                    {
                        DataValue = m_Values[Index];
                    }
                }

                return DataValue;
            }
            set
            {
                int Index = -1;

                if (m_Parent != null)
                {
                    Index = m_Parent.Columns.FindIndex(c => c.Equals(column));

                    if (Index >= 0)
                    {
                        m_Values[Index] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">The index of the value to get or set</param>
        /// <returns>The interval value at the specified index</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMData this[int index]
        {
            get
            {
                return m_Values[index];
            }
            set
            {
                m_Values[index] = value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the parent Generic Profile Data object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileData Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of interval values
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public List<COSEMData> Values
        {
            get
            {
                return m_Values;
            }
            set
            {
                if (value != null)
                {
                    m_Values = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "Values may not be set to null");
                }
            }
        }

        /// <summary>
        /// Gets the Date and Time of the interval if present
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public COSEMDateTime IntervalDateTime
        {
            get
            {
                COSEMDateTime CurrentDate = null;
                COSEMProfileCaptureObject ClockCaptureObject = new COSEMProfileCaptureObject();
                COSEMData ClockValue = null;

                ClockCaptureObject.LogicalName = COSEMClockInterfaceClass.CLOCK_LN;
                ClockCaptureObject.ClassID = 8;
                ClockCaptureObject.AttributeIndex = 2;
                ClockCaptureObject.DataIndex = 0;

                ClockValue = this[ClockCaptureObject];

                if (ClockValue != null)
                {
                    if (ClockValue.DataType == COSEMDataTypes.DateTime)
                    {
                        CurrentDate = ClockValue.Value as COSEMDateTime;
                    }
                    else if (ClockValue.DataType == COSEMDataTypes.OctetString)
                    {
                        CurrentDate = new COSEMDateTime(ClockValue.Value as byte[]);
                    }
                }

                return CurrentDate;
            }
        }

        #endregion

        #region Member Variables

        private GenericProfileData m_Parent;
        private List<COSEMData> m_Values;

        #endregion
    }

    /// <summary>
    /// Generic Profile Data object
    /// </summary>
    public class GenericProfileData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileData()
        {
            m_Columns = new List<COSEMProfileCaptureObject>();
            m_Intervals = new List<GenericProfileInterval>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buffer">The buffer containing the profile data</param>
        /// <param name="columns">The Capture Objects corresponding to each column</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileData(COSEMData[] buffer, List<COSEMProfileCaptureObject> columns)
            : this()
        {
            m_Columns = columns;

            AddRange(buffer);
        }

        /// <summary>
        /// Adds the intervals found in the buffer data
        /// </summary>
        /// <param name="buffer">The buffer containing the interval data</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public void AddRange(COSEMData[] buffer)
        {
            if (buffer != null)
            {
                foreach (COSEMData CurrentIntervalData in buffer)
                {
                    m_Intervals.Add(new GenericProfileInterval(this, CurrentIntervalData));
                }
            }
            else
            {
                throw new ArgumentNullException("buffer", "The buffer may not be null");
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index">The index of the value to get or set</param>
        /// <returns>The interval at the specified index</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public GenericProfileInterval this[int index]
        {
            get
            {
                return m_Intervals[index];
            }
            set
            {
                m_Intervals[index] = value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Column Identifiers
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public List<COSEMProfileCaptureObject> Columns
        {
            get
            {
                return m_Columns;
            }
        }

        /// <summary>
        /// Gets the list of Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created

        public List<GenericProfileInterval> Intervals
        {
            get
            {
                return m_Intervals;
            }
        }

        /// <summary>
        /// Gets the number of Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public int IntervalCount
        {
            get
            {
                return m_Intervals.Count;
            }
        }

        /// <summary>
        /// Gets the number of Columns
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public int ColumnCount
        {
            get
            {
                return m_Columns.Count;
            }
        }

        #endregion

        #region Member Variables

        private List<COSEMProfileCaptureObject> m_Columns;
        private List<GenericProfileInterval> m_Intervals;

        #endregion
    }

    /// <summary>
    /// Switching Classes
    /// </summary>
    public enum SwitchingClasses : byte
    {
        /// <summary>Disconnected Manually</summary>
        [EnumDescription("Disconnected (Direct)")]
        DisconnectedDirect = 0,
        /// <summary>Connected Manually</summary>
        [EnumDescription("Connected (Direct)")]
        ConnectedDirect = 1,
        /// <summary>Disconnected by Service Limiting</summary>
        [EnumDescription("Disconnected (Over Current)")]
        DisconnectedServiceLimiting = 2,
        /// <summary>Connected by Service Limiting</summary>
        [EnumDescription("Connected (Auto Connect)")]
        ConnectedServiceLimiting = 3,
        /// <summary>Disconnected by Time</summary>
        [EnumDescription("Disconnected (Time)")]
        DisconnectedTime = 6,
        /// <summary>Connected by Time</summary>
        [EnumDescription("Connected (Time)")]
        ConnectedTime = 7,
        /// <summary>No Switch</summary>
        [EnumDescription("No Switch")]
        NoSwitch = 255,
    }

    /// <summary>
    /// Switch Operation History Entry
    /// </summary>
    public class SwitchOperationHistoryEntry
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public SwitchOperationHistoryEntry()
        {
            m_RecordNumber = 0;
            m_SwitchingClass = SwitchingClasses.NoSwitch;
            m_Clock = new COSEMDateTime();
            m_AverageCurrent = 0.0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Record Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public ushort RecordNumber
        {
            get
            {
                return m_RecordNumber;
            }
            set
            {
                m_RecordNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Switching Class
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public SwitchingClasses SwitchingClass
        {
            get
            {
                return m_SwitchingClass;
            }
            set
            {
                m_SwitchingClass = value;
            }
        }

        /// <summary>
        /// Gets or sets the Clock
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public COSEMDateTime Clock
        {
            get
            {
                return m_Clock;
            }
            set
            {
                m_Clock = value;
            }
        }

        /// <summary>
        /// Gets or sets the Average Current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/08/13 RCG 2.85.14 N/A    Created
        
        public double AverageCurrent
        {
            get
            {
                return m_AverageCurrent;
            }
            set
            {
                m_AverageCurrent = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_RecordNumber;
        private SwitchingClasses m_SwitchingClass;
        private COSEMDateTime m_Clock;
        private double m_AverageCurrent;

        #endregion
    }

    /// <summary>
    /// COSEM Load Profile Status
    /// </summary>
    public enum COSEMLoadProfileStatus : byte
    {
        /// <summary>The load profile data was read successfully</summary>
        Success = 0x00,
        /// <summary>The DLMS Protocol object is not connected to a meter</summary>
        DLMSProtocolObjectIsNotConnected = 0x01,
        /// <summary>The COSEM Load Profile Interface object could not be found</summary>
        LoadProfileInterfaceClassObjectNotFound = 0x02,
        /// <summary>No Capture objects were found in the COSEM Load Profile Interface object</summary>
        CaptureObjectsNotFound = 0x03,
        /// <summary>The Range Descriptor Restricting object was not found</summary>
        RangeDescriptorRestrictingObjectNotFound = 0x04,
        /// <summary>The Load Profile Access buffer is null</summary>
        LoadProfileAccessBufferIsNull = 0x05,
        /// <summary>Data Format Error</summary>
        DataFormatError = 0x06,
        /// <summary>Load Profile data has not been accessed</summary>
        LoadProfileDataHasNotBeenAccessed = 0x07,
    }

    /// <summary>
    /// COSEM Load Profile Set One Data
    /// </summary>
    public class COSEMLoadProfileSetOne
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public COSEMLoadProfileSetOne()
        {
            m_Status = COSEMLoadProfileStatus.LoadProfileDataHasNotBeenAccessed;
            m_PeriodDuration = 0;
            m_WhDeliveredIntervals = new List<COSEMLoadProfileInterval>();
            m_WhReceivedIntervals = new List<COSEMLoadProfileInterval>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public COSEMLoadProfileStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets the duration of the load profile period in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public uint PeriodDuration
        {
            get
            {
                return m_PeriodDuration;
            }
            set
            {
                m_PeriodDuration = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Delivered Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public List<COSEMLoadProfileInterval> WhDelivered
        {
            get
            {
                return m_WhDeliveredIntervals;
            }
            set
            {
                m_WhDeliveredIntervals = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Received Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public List<COSEMLoadProfileInterval> WhReceived
        {
            get
            {
                return m_WhReceivedIntervals;
            }
            set
            {
                m_WhReceivedIntervals = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMLoadProfileStatus m_Status;
        private uint m_PeriodDuration;
        private List<COSEMLoadProfileInterval> m_WhDeliveredIntervals;
        private List<COSEMLoadProfileInterval> m_WhReceivedIntervals;

        #endregion
    }

    /// <summary>
    /// COSEM Meter Specification Profile Data
    /// </summary>
    public class COSEMMeterSpecificationProfile
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.06        Created

        public COSEMMeterSpecificationProfile()
        {
            m_Status = COSEMLoadProfileStatus.LoadProfileDataHasNotBeenAccessed;
            m_Intervals = new List<COSEMMeterSpecificationProfileInterval>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.06        Created

        public COSEMLoadProfileStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Delivered Intervals
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.06        Created

        public List<COSEMMeterSpecificationProfileInterval> Intervals
        {
            get
            {
                return m_Intervals;
            }
            set
            {
                m_Intervals = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMLoadProfileStatus m_Status;
        private List<COSEMMeterSpecificationProfileInterval> m_Intervals;

        #endregion
    }

    /// <summary>
    /// COSEM Load Profile Set Two Data
    /// </summary>
    public class COSEMLoadProfileSetTwo
    {
        #region Constants

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/13 PGH 2.85.11        Created

        public COSEMLoadProfileSetTwo()
        {
            m_Status = COSEMLoadProfileStatus.LoadProfileDataHasNotBeenAccessed;
            m_PeriodDuration = 0;
            m_AvgVoltagePhaseAIntervals = new List<COSEMLoadProfileInterval>();
            m_AvgVoltagePhaseCIntervals = new List<COSEMLoadProfileInterval>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the request
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/13 PGH 2.85.11        Created

        public COSEMLoadProfileStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// Gets the duration of the load profile period in seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/13 PGH 2.85.11        Created

        public uint PeriodDuration
        {
            get
            {
                return m_PeriodDuration;
            }
            set
            {
                m_PeriodDuration = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Average Voltage Phase A
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/13 PGH 2.85.11        Created

        public List<COSEMLoadProfileInterval> AvgVoltagePhaseA
        {
            get
            {
                return m_AvgVoltagePhaseAIntervals;
            }
            set
            {
                m_AvgVoltagePhaseAIntervals = value;
            }
        }

        /// <summary>
        /// Gets the Load Profile Average Voltage Phase C
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/05/13 PGH 2.85.11        Created

        public List<COSEMLoadProfileInterval> AvgVoltagePhaseC
        {
            get
            {
                return m_AvgVoltagePhaseCIntervals;
            }
            set
            {
                m_AvgVoltagePhaseCIntervals = value;
            }
        }

        #endregion

        #region Member Variables

        private COSEMLoadProfileStatus m_Status;
        private uint m_PeriodDuration;
        private List<COSEMLoadProfileInterval> m_AvgVoltagePhaseAIntervals;
        private List<COSEMLoadProfileInterval> m_AvgVoltagePhaseCIntervals;

        #endregion
    }

    /// <summary>
    /// COSEM Load Profile interval
    /// </summary>
    public class COSEMLoadProfileInterval
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="intervalTime">The time the interval ended</param>
        /// <param name="intervalValue">The load profile value</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10       Created

        public COSEMLoadProfileInterval(DateTime intervalTime, double intervalValue)
        {
            m_Time = intervalTime;
            m_Value = intervalValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public DateTime IntervalEndTime
        {
            get
            {
                return m_Time;
            }
        }

        /// <summary>
        /// Gets the value of the interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/02/13 PGH 2.85.10        Created

        public double IntervalValue
        {
            get
            {
                return m_Value;
            }
        }

        #endregion

        #region Member Variables

        private DateTime m_Time;
        private double m_Value;

        #endregion
    }

    /// <summary>
    /// COSEM Meter Specification Profile interval
    /// </summary>
    public class COSEMMeterSpecificationProfileInterval
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public COSEMMeterSpecificationProfileInterval(DateTime intervalTime,
                                                        PhaseTypeCode phaseTypeCode,
                                                        double ratedVoltage,
                                                        double ratedCurrent,
                                                        string meterModel,
                                                        uint transformerRationNumerator,
                                                        ushort transformerRatioDenominator,
                                                        string meterFunction,
                                                        byte numberOfDigitsOfTheMeter,
                                                        uint displayTimeImportedEnergy,
                                                        uint displayTimeExportedEnergy)
        {
            m_Time = intervalTime;
            m_PhaseTypeCode = phaseTypeCode;
            m_RatedVoltage = ratedVoltage;
            m_RatedCurrent = ratedCurrent;
            m_MeterModel = meterModel;
            m_TransformerRationNumerator = transformerRationNumerator;
            m_TransformerRatioDenominator = transformerRatioDenominator;

            if (string.IsNullOrEmpty(meterFunction) || meterFunction.Length < 3)
            {
                throw new ArgumentException("Cannot create meter specification interval. Meter function must be a non-null string of at least length 3");
            }

            m_MeterFunction = meterFunction;
            m_NumberOfDigitsOfTheMeter = numberOfDigitsOfTheMeter;
            m_DisplayTimeImportedEnergy = displayTimeImportedEnergy;
            m_DisplayTimeExportedEnergy = displayTimeExportedEnergy;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the end time of the interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public DateTime IntervalEndTime
        {
            get
            {
                return m_Time;
            }
        }

        /// <summary>
        /// Gets the value of the rated voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public double RatedVoltage
        {
            get
            {
                return m_RatedVoltage;
            }
        }

        /// <summary>
        /// Gets the value of the rated current
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public double RatedCurrent
        {
            get
            {
                return m_RatedCurrent;
            }
        }

        /// <summary>
        /// Gets the value of the phase/wire type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public PhaseTypeCode PhaseWireType
        {
            get
            {
                return m_PhaseTypeCode;
            }
        }

        /// <summary>
        /// Gets the value of the meter model
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public string MeterModel
        {
            get
            {
                return m_MeterModel;
            }
        }

        /// <summary>
        /// Gets the value of the transformer ratio numerator
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public uint TransformerRatioNumerator
        {
            get
            {
                return m_TransformerRationNumerator;
            }
        }

        /// <summary>
        /// Gets the value of the transformer ratio denominator
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public ushort TransformerRatioDenominator
        {
            get
            {
                return m_TransformerRatioDenominator;
            }
        }

        /// <summary>
        /// Gets the transformer ratio
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public double TransformerRatio
        {
            get
            {
                double returnValue = 0;
                if (m_TransformerRatioDenominator != 0)
                {
                    returnValue = m_TransformerRationNumerator / m_TransformerRatioDenominator;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the value of the meter function
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public string MeterFunction
        {
            get
            {
                return m_MeterFunction;
            }
        }

        /// <summary>
        /// Returns the type of switching function
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public Itron.Metering.Device.DLMSDevice.COSEMMeterFunctionInterfaceClass.SwitchOperationType SwitchOperation
        {
            get
            {
                return (Itron.Metering.Device.DLMSDevice.COSEMMeterFunctionInterfaceClass.SwitchOperationType)((byte)MeterFunction[0] - (byte)'0');
            }
        }


        /// <summary>
        /// Returns a flag indicating if a battery is present or not
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public Boolean BatteryPresent
        {
            get
            {
                return (MeterFunction[1] != '0');
            }
        }

        /// <summary>
        /// Returns a flag indicating if the meter support bidirectional measurement
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public Boolean BidirectionalMeasurement
        {
            get
            {
                return (MeterFunction[2] != '0');
            }
        }

        /// <summary>
        /// Gets the value of the number of digits of the meter
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public byte NumberOfDisplayDigits
        {
            get
            {
                return m_NumberOfDigitsOfTheMeter;
            }
        }

        /// <summary>
        /// Gets the value of the display time for imported energy
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public uint DisplayTimeImportedEnergy
        {
            get
            {
                return m_DisplayTimeImportedEnergy;
            }
        }

        /// <summary>
        /// Gets the value of the display time for exported energy
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/24/13 jkw 3.00.05        Created

        public uint DisplayTimeExportedEnergy
        {
            get
            {
                return m_DisplayTimeExportedEnergy;
            }
        }

        #endregion

        #region Member Variables

        private DateTime m_Time;
        private PhaseTypeCode m_PhaseTypeCode;
        private double m_RatedVoltage;
        private double m_RatedCurrent;
        private string m_MeterModel;
        private uint m_TransformerRationNumerator;
        private ushort m_TransformerRatioDenominator;
        private string m_MeterFunction;
        private byte m_NumberOfDigitsOfTheMeter;
        private uint m_DisplayTimeImportedEnergy;
        private uint m_DisplayTimeExportedEnergy;

        #endregion
    }

    /// <summary>
    /// Event Profile Entry
    /// </summary>
    public class EventProfileEntry
    {

        #region Constants

        // Event Codes

        /// <summary>
        /// Power Outage
        /// </summary>
        public const string POWER_OUTAGE = "A0";
        /// <summary>
        /// Power Recovery
        /// </summary>
        public const string POWER_RECOVERY = "A2";
        /// <summary>
        /// Voltage Drop Terminal 1
        /// </summary>
        public const string VOLTAGE_DROP_TERMINAL_1 = "B0";
        /// <summary>
        /// Voltage Drop Terminal 3
        /// </summary>
        public const string VOLTAGE_DROP_TERMINAL_3 = "B1";
        /// <summary>
        /// Voltage Recovery Terminal 1
        /// </summary>
        public const string VOLTAGE_RECOVERY_TERMINAL_1 = "B2";
        /// <summary>
        /// Voltage Recovery Terminal 3
        /// </summary>
        public const string VOLTAGE_RECOVERY_TERMINAL_3 = "B3";
        /// <summary>
        /// Terminal Cover open
        /// </summary>
        public const string TERMINAL_COVER_OPEN = "D0";
        /// <summary>
        /// Terminal Cover close
        /// </summary>
        public const string TERMINAL_COVER_CLOSE = "D1";
        /// <summary>
        /// Switch Disconnect
        /// </summary>
        public const string SWITCH_DISCONNECT = "E0";
        /// <summary>
        /// Switch Connect
        /// </summary>
        public const string SWITCH_CONNECT = "E1";
        /// <summary>
        /// Switch Disconnect Overcurrent
        /// </summary>
        public const string SWITCH_DISCONNECT_OVERCURRENT = "E2";
        /// <summary>
        /// Switch Connect Auto-connection
        /// </summary>
        public const string SWITCH_CONNECT_AUTO_CONNECTION = "E3";
        /// <summary>
        /// Load Limit Disabled
        /// </summary>
        public const string LOAD_LIMIT_DISABLED = "F0";
        /// <summary>
        /// Load Limit Enabled
        /// </summary>
        public const string LOAD_LIMIT_ENABLED = "F1";
        /// <summary>
        /// Load Limit Enabled Forward Only
        /// </summary>
        public const string LOAD_LIMIT_ENABLED_FORWARD_ONLY = "F2";
        /// <summary>
        /// Load Limit Temporary Setting
        /// </summary>
        public const string LOAD_LIMIT_TEMPORARY_SETTING = "FA";
        /// <summary>
        /// Load Limit Temporary Setting Released
        /// </summary>
        public const string LOAD_LIMIT_TEMPORARY_SETTING_RELEASED = "FR";
        /// <summary>
        /// Time Change
        /// </summary>
        public const string TIME_CHANGE = "R0";
        /// <summary>
        /// Remote Reading Response
        /// </summary>
        public const string REMOTE_READING_RESPONSE = "  "; // two spaces

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public EventProfileEntry()
        {
            m_RecordNumber = 0;
            m_EventCode = "";
            m_Clock = DateTime.MinValue;
            m_WhDelivered = 0.0;
            m_WhReceived = 0.0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Record Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public ushort RecordNumber
        {
            get
            {
                return m_RecordNumber;
            }
            set
            {
                m_RecordNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Event Code
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public string EventCode
        {
            get
            {
                return m_EventCode;
            }
            set
            {
                m_EventCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Clock
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public DateTime Clock
        {
            get
            {
                return m_Clock;
            }
            set
            {
                m_Clock = value;
            }
        }

        /// <summary>
        /// Gets or sets the Wh Delivered
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public double WhDelivered
        {
            get
            {
                return m_WhDelivered;
            }
            set
            {
                m_WhDelivered = value;
            }
        }

        /// <summary>
        /// Gets or sets the Wh Received
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/22/13 PGH 2.85.22        Created

        public double WhReceived
        {
            get
            {
                return m_WhReceived;
            }
            set
            {
                m_WhReceived = value;
            }
        }

        #endregion

        #region Member Variables

        private ushort m_RecordNumber;
        private string m_EventCode;
        private DateTime m_Clock;
        private double m_WhDelivered;
        private double m_WhReceived;

        #endregion
    }
}
