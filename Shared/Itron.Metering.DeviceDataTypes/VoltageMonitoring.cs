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
//                              Copyright © 2008 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    #region Public Enumerations

    /// <summary>
    /// Extended Status Flags
    /// </summary>

    [Flags]
    public enum VMStatusFlags : ushort
    {
        /// <summary>
        /// No status
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Instantaneous Voltage for Phase A is below the threshold
        /// </summary>
        PhaseABelowMinInsVoltage = 0x0001,
        /// <summary>
        /// Instantaneous Voltage for Phase B is below the threshold
        /// </summary>
        PhaseBBelowMinInsVoltage = 0x0002,
        /// <summary>
        /// Instantaneous Voltage for Phase C is below the threshold
        /// </summary>
        PhaseCBelowMinInsVoltage = 0x0004,
        /// <summary>
        /// Instantaneous Voltage for Phase A is above the threshold
        /// </summary>
        PhaseAAboveMaxInsVoltage = 0x0008,
        /// <summary>
        /// Instantaneous Voltage for Phase B is above the threshold
        /// </summary>
        PhaseBAboveMaxInsVoltage = 0x0010,
        /// <summary>
        /// Instantaneous Voltage for Phase C is above the threshold
        /// </summary>
        PhaseCAboveMaxInsVoltage = 0x0020,
        /// <summary>
        /// The service type was invalid for this interval.
        /// </summary>
        InvalidServiceType = 0x0040,
        /// <summary>
        /// Not Used
        /// </summary>
        Filler2 = 0x0080,
        /// <summary>
        /// Partial Interval
        /// </summary>
        Partial = 0x0100,
        /// <summary>
        /// Long Interval
        /// </summary>
        Long = 0x0200,
        /// <summary>
        /// Interval was skipped
        /// </summary>
        Skipped = 0x0400,
        /// <summary>
        /// Interval occurred while in test mode
        /// </summary>
        TestMode = 0x0800,
        /// <summary>
        /// Interval occurred while in DST
        /// </summary>
        DST = 0x1000,
        /// <summary>
        /// A power outage occurred during the interval
        /// </summary>
        PowerOutage = 0x2000,
        /// <summary>
        /// The clock was adjusted forward during the interval
        /// </summary>
        TimeAdjustedForward = 0x4000,
        /// <summary>
        /// The clock was adjusted backward during the interval
        /// </summary>
        TimeAdjustedBackward = 0x8000,
    }

    #endregion

    /// <summary>
    /// Voltage Monitoring data class
    /// </summary>
    public class VMData
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public VMData()
        {
            m_Intervals = new List<VMInterval>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhLowThreshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/20/08 RCG 1.52.00 N/A    Adapted from EDL file
        //
        public ushort VhLowPercentage
        {
            get
            {
                return m_usVhLowThresholdPercentage;
            }
            set
            {
                m_usVhLowThresholdPercentage = value;
            }
        }

        /// <summary>
        /// Determine the Percentage of Nominal for VMVhHighThreshold
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/20/08 RCG 1.52.00 N/A    Adapted from EDL file
        //
        public ushort VhHighPercentage
        {
            get
            {
                return m_usVhHighThresholdPercentage;
            }
            set
            {
                m_usVhHighThresholdPercentage = value;
            }
        }

        /// <summary>
        /// Gets the low threshold for Vh
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public float[] VhLowThreshold
        {
            get
            {
                float[] Thresholds = new float[m_byNumberOfPhases];

                for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
                {
                    Thresholds[iIndex] = NominalVoltages[iIndex] * VhLowPercentage / 100.0f;
                }

                return Thresholds;
            }
        }

        /// <summary>
        /// Gets the high threshold for Vh
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public float[] VhHighThreshold
        {
            get
            {
                float[] Thresholds = new float[m_byNumberOfPhases];

                for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
                {
                    Thresholds[iIndex] = NominalVoltages[iIndex] * VhHighPercentage / 100.0f;
                }

                return Thresholds;
            }
        }

        /// <summary>
        /// Gets the low threshold for RMS voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public float[] RMSVoltageLowThreshold
        {
            get
            {
                float[] Thresholds = new float[m_byNumberOfPhases];

                for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
                {
                    Thresholds[iIndex] = NominalVoltages[iIndex] * RMSVoltageLowPercentage / 100.0f;
                }

                return Thresholds;
            }
        }

        /// <summary>
        /// Gets the high threshold for RMS voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public float[] RMSVoltageHighThreshold
        {
            get
            {
                float[] Thresholds = new float[m_byNumberOfPhases];

                for (int iIndex = 0; iIndex < m_byNumberOfPhases; iIndex++)
                {
                    Thresholds[iIndex] = NominalVoltages[iIndex] * RMSVoltageHighPercentage / 100.0f;
                }

                return Thresholds;
            }
        }

        /// <summary>
        /// Gets the low threshold for RMS voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 RCG 2.20.05 N/A    Created

        public ushort RMSVoltageLowPercentage
        {
            get
            {
                return m_usRMSVoltLowPercentage;
            }
            set
            {
                m_usRMSVoltLowPercentage = value;
            }
        }

        /// <summary>
        /// Gets the high threshold for RMS voltage
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 RCG 2.20.05 N/A    Created

        public ushort RMSVoltageHighPercentage
        {
            get
            {
                return m_usRMSVoltHighPercentage;
            }
            set
            {
                m_usRMSVoltHighPercentage = value;
            }
        }

        /// <summary>
        /// Gets the length of one interval
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public TimeSpan IntervalLength
        {
            get
            {
                return m_tsIntervalLength;
            }
            set
            {
                m_tsIntervalLength = value;
            }
        }

        /// <summary>
        /// Gets the number of phases in the data
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public byte NumberOfPhases
        {
            get
            {
                return m_byNumberOfPhases;
            }
            set
            {
                m_byNumberOfPhases = value;
            }
        }

        /// <summary>
        /// Gets the interval data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public List<VMInterval> Intervals
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

        /// <summary>
        /// Gets or sets the nominal voltages.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/14/09 RCG 2.20.05 N/A    Created

        public ushort[] NominalVoltages
        {
            get
            {
                return m_NominalVoltages;
            }
            set
            {
                if (value != null && value.Length == this.m_byNumberOfPhases)
                {
                    m_NominalVoltages = value;
                }
                else
                {
                    throw new ArgumentException("Number of phases must match number of Nominal Voltages");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_byNumberOfPhases;
        private ushort m_usVhLowThresholdPercentage;
        private ushort m_usVhHighThresholdPercentage;
        private ushort m_usRMSVoltLowPercentage;
        private ushort m_usRMSVoltHighPercentage;
        private TimeSpan m_tsIntervalLength;
        private List<VMInterval> m_Intervals;

        private ushort[] m_NominalVoltages;

        #endregion
    }

    /// <summary>
    /// Class that represents a Voltage Monitoring interval.
    /// </summary>

    public class VMInterval
    {

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Status">The status of the interval.</param>
        /// <param name="fVhData">The voltage monitoring data.</param>
        /// <param name="dtEndTime">The end time of the interval.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public VMInterval(VMStatusFlags Status, List<float> fVhData, DateTime dtEndTime)
        {
            m_Status = Status;
            m_VhData = fVhData;
            m_dtEndTime = dtEndTime;
            m_VminData = new List<float>();
            m_VmaxData = new List<float>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Status">The status of the interval.</param>
        /// <param name="fVhData">The Vh data.</param>
        /// <param name="fVminData">The Vmin data.</param>
        /// <param name="fVmaxData">The Vmax data.</param>
        /// <param name="dtEndTime">The end time of the interval.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created

        public VMInterval(VMStatusFlags Status, List<float> fVhData, List<float> fVminData, List<float> fVmaxData, DateTime dtEndTime)
        {
            m_Status = Status;
            m_VhData = fVhData;
            m_dtEndTime = dtEndTime;
            m_VminData = fVminData;
            m_VmaxData = fVmaxData;
            
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status of the current interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public VMStatusFlags IntervalStatus
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// Gets the Interval Status as a string. This property does not include thresholds.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/13/08 RCG 1.50.24 N/A    Created
        //  06/15/12 MAH 2.60.31 199940 Changed interval statuses to be consistent with LP
        //  06/19/12 MAH 2.60.33 200125 Moved the 'D' status to be the first in the string
        //  05/11/16 PGH 4.50.266 602339 Added RS status
        //  06/09/16 PGH 4.50.282 691829 Return only one S per interval
        //  08/22/16 AF  4.60.04  701320 Corrected the logic for interval status "RS"
        //  08/22/16 AF  4.60.04  701320 If the status is "RS", remove the "K" status - it's part of the "RS"
        //
        public string IntervalStatusString
        {
            get
            {
                string strStatus = "";

                if ((m_Status & VMStatusFlags.DST) == VMStatusFlags.DST)
                {
                    strStatus += "D";
                }

                if ((m_Status & VMStatusFlags.InvalidServiceType) == VMStatusFlags.InvalidServiceType)
                {
                    strStatus += "I";
                }

                if ((m_Status & VMStatusFlags.TimeAdjustedForward) == VMStatusFlags.TimeAdjustedForward)
                {
                    strStatus += "A";
                }

                if ((m_Status & VMStatusFlags.TimeAdjustedBackward) == VMStatusFlags.TimeAdjustedBackward)
                {
                    strStatus += "A";
                }

                if ((m_Status & VMStatusFlags.Long) == VMStatusFlags.Long)
                {
                    strStatus += "L";
                }

                if ((m_Status & VMStatusFlags.Skipped) == VMStatusFlags.Skipped)
                {
                    strStatus += "K";
                }

                if ((m_Status & VMStatusFlags.TestMode) == VMStatusFlags.TestMode)
                {
                    strStatus += "T";
                }

                if ((m_Status & VMStatusFlags.PowerOutage) == VMStatusFlags.PowerOutage)
                {
                    strStatus += "O";
                }

                if ((m_Status & (VMStatusFlags.Skipped | VMStatusFlags.Partial)) == (VMStatusFlags.Skipped | VMStatusFlags.Partial))
                {
                    // If we have an RS status, we shouldn't show K
                    if (strStatus.Contains("K"))
                    {
                        strStatus = strStatus.Replace("K", string.Empty);
                    }

                    strStatus += "RS";
                }
                else if ((m_Status & VMStatusFlags.Partial) == VMStatusFlags.Partial)
                {
                    strStatus += "S"; // Short
                }

                return strStatus;
            }
        }

        /// <summary>
        /// Gets the status for the Vh High Threshold as a string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/13/08 RCG 1.50.24 N/A    Created

        public string VhHighStatusString
        {
            get
            {
                string strStatus = "";

                if ((m_Status & VMStatusFlags.PhaseAAboveMaxInsVoltage) == VMStatusFlags.PhaseAAboveMaxInsVoltage)
                {
                    strStatus += "A";
                }

                if ((m_Status & VMStatusFlags.PhaseBAboveMaxInsVoltage) == VMStatusFlags.PhaseBAboveMaxInsVoltage)
                {
                    strStatus += "B";
                }

                if ((m_Status & VMStatusFlags.PhaseCAboveMaxInsVoltage) == VMStatusFlags.PhaseCAboveMaxInsVoltage)
                {
                    strStatus += "C";
                }

                return strStatus;
            }
        }

        /// <summary>
        /// Gets the status for the Vh Low Threshold as a string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/13/08 RCG 1.50.24 N/A    Created

        public string VhLowStatusString
        {
            get
            {
                string strStatus = "";

                if ((m_Status & VMStatusFlags.PhaseABelowMinInsVoltage) == VMStatusFlags.PhaseABelowMinInsVoltage)
                {
                    strStatus += "A";
                }

                if ((m_Status & VMStatusFlags.PhaseBBelowMinInsVoltage) == VMStatusFlags.PhaseBBelowMinInsVoltage)
                {
                    strStatus += "B";
                }

                if ((m_Status & VMStatusFlags.PhaseCBelowMinInsVoltage) == VMStatusFlags.PhaseCBelowMinInsVoltage)
                {
                    strStatus += "C";
                }

                return strStatus;
            }
        }

        /// <summary>
        /// Gets the voltage data of the current interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public List<float> VhData
        {
            get
            {
                return m_VhData;
            }
        }

        /// <summary>
        /// Gets the Vmin data of the current interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created

        public List<float> VminData
        {
            get
            {
                return m_VminData;
            }
        }

        /// <summary>
        /// Gets the Vmax data of the current interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/28/11 jrf 2.53.10 TC5321 Created

        public List<float> VmaxData
        {
            get
            {
                return m_VmaxData;
            }
        }

        /// <summary>
        /// Gets the end time of the current interval.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/07/08 RCG 1.50.23 N/A    Created

        public DateTime IntervalEndTime
        {
            get
            {
                return m_dtEndTime;
            }
        }

        #endregion

        #region Member Variables

        private VMStatusFlags m_Status;
        private List<float> m_VhData;
        private List<float> m_VminData;
        private List<float> m_VmaxData;
        private DateTime m_dtEndTime;

        #endregion
    }
}
