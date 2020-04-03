///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
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
//                            Copyright © 2009 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// KYZ outputs that can be configured for the meter.
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------------
    // 04/20/09 jrf 2.20.02 n/a    Created.
    //
    public enum KYZOutput
    {
        /// <summary>
        /// The first KYZ output.
        /// </summary>
        KYZ1,

        /// <summary>
        /// The second KYZ output.
        /// </summary>
        KYZ2,

        /// <summary>
        /// The low current (KY) output.
        /// </summary>
        LC1,
    }
    
    /// <summary>
    /// KYZ Data used to Populate the KYZ Option Board.
    /// </summary>
    public class KYZData
    {
        #region Constants

        /// <summary>
        /// None
        /// </summary>
        public const string NONE = "None";
        /// <summary>
        /// Demand Reset
        /// </summary>
        public const string DEMAND_RESET = "Demand Reset";
        /// <summary>
        /// End of Interval (EOI)
        /// </summary>
        public const string END_OF_INTERVAL = "End of Interval (EOI)";
        /// <summary>
        /// Rate Change
        /// </summary>
        public const string RATE_CHANGE = "Rate Change";
        /// <summary>
        /// Season Change
        /// </summary>
        public const string SEASON_CHANGE = "Season Change";

        private const string WH_D = "Wh d";
        private const string WH_R = "Wh r";
        private const string VARH_Q1 = "VARh Q1";
        private const string VARH_Q2 = "VARh Q2";
        private const string VARH_Q3 = "VARh Q3";
        private const string VARH_Q4 = "VARh Q4";
        private const string VAH_ARITH_D = "VAh Arith d";
        private const string VAH_ARITH_R = "VAh Arith r";
        private const string VAH_VEC_D = "VAh Vec d";
        private const string VAH_VEC_R = "VAh Vec r";
        private const string VAH_LAG = "VAh Lag";
        private const string QH_D = "Qh d";
        private const string VH_A = "Vh(a)";
        private const string VH_B = "Vh(b)";
        private const string VH_C = "Vh(c)";
        private const string VH_AVG = "Vh avg";
        private const string AH_A = "Ah(a)";
        private const string AH_B = "Ah(b)";
        private const string AH_C = "Ah(c)";
        private const string VSQH = "V^2h";
        private const string ASQH = "A^2h";
        private const string VARH_D = "VARh d";
        private const string VARH_R = "VARh r";
        private const string QH_R = "Qh r";
        private const string WH = "Wh";
        private const string UNKNOWN_ENERGY = "Unknown Energy";
        private const string UNKNOWN_EVENT = "Unknown Event";
        
        #endregion

        #region Definitions

        /// <summary>
        /// The indication of what will trigger the output.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public enum OutputType: byte
        {
            /// <summary>
            /// Not used.
            /// </summary>
            NotUsed = 0,
            /// <summary>
            /// Pulse energy.
            /// </summary>
            PulseEnergy = 1,
            /// <summary>
            /// Toggle energy.
            /// </summary>
            ToggleEnergy = 2,
            /// <summary>
            /// Pulse event.
            /// </summary>
            PulseEvent = 3,
            /// <summary>
            /// State change event.
            /// </summary>
            StateChangeEvent = 4,
        }

        /// <summary>
        /// The specific event that will trigger the output.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        private enum Event : byte
        {
            NoEvent = 0,
            DemandReset = 1,
            EndOfInterval = 2,
            RateChange = 3,
            SeasonChange = 4,
        }

        /// <summary>
        /// The specific energy that will trigger the output.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public enum Energy : byte
        {
            /// <summary>
            /// No energy.
            /// </summary>
            NoEnergy = 0,
            /// <summary>
            /// Wh d
            /// </summary>
            WhD = 1,
            /// <summary>
            /// Wh r
            /// </summary>
            WhR = 2,
            /// <summary>
            /// Varh Q1
            /// </summary>
            VarhQ1 = 3,
            /// <summary>
            /// Varh Q2
            /// </summary>
            VarhQ2 = 4,
            /// <summary>
            /// Varh Q3
            /// </summary>
            VarhQ3 = 5,
            /// <summary>
            /// Varh Q4
            /// </summary>
            VarhQ4 = 6,
            /// <summary>
            /// VAh Arith d
            /// </summary>
            VAhArithD = 9,
            /// <summary>
            /// VAh Arith r
            /// </summary>
            VAhArithR = 10,
            /// <summary>
            /// VAh Vec d
            /// </summary>
            VAhVecD = 11,
            /// <summary>
            /// VAh Vec r
            /// </summary>
            VAhVecR = 12,
            /// <summary>
            /// VAh Lag
            /// </summary>
            VAhLag = 13,
            /// <summary>
            /// Qh d
            /// </summary>
            QhD = 14,
            /// <summary>
            /// Vh A
            /// </summary>
            VhA = 15,
            /// <summary>
            /// Vh B
            /// </summary>
            VhB = 16,
            /// <summary>
            /// Vh C
            /// </summary>
            VhC = 17,
            /// <summary>
            /// Vh Avg
            /// </summary>
            VhAvg = 18,
            /// <summary>
            /// Ah A
            /// </summary>
            AhA = 19,
            /// <summary>
            /// Ah B
            /// </summary>
            AhB = 20,
            /// <summary>
            /// Ah C
            /// </summary>
            AhC = 21,
            /// <summary>
            /// V squared h
            /// </summary>
            Vsqh = 23,
            /// <summary>
            /// A squared h
            /// </summary>
            Asqh = 24,
            /// <summary>
            /// Varh d
            /// </summary>
            VarhD = 32,
            /// <summary>
            /// Varh r
            /// </summary>
            VarhR = 33,
            /// <summary>
            /// Qh r
            /// </summary>
            QhR = 38,
            /// <summary>
            /// Wh net
            /// </summary>
            Wh = 39,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for the KYZ Data Object.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public KYZData()
        {
            m_strKYZ1OperationDescription = NONE;
            m_strKYZ2OperationDescription = NONE;
            m_strLC1OperationDescription = NONE;
            m_bytKYZ1EnergyID = (byte)Energy.NoEnergy;
            m_bytKYZ2EnergyID = (byte)Energy.NoEnergy;
            m_bytKYZ1EventID = (byte)Event.NoEvent;
            m_bytKYZ2EventID = (byte)Event.NoEvent; 
            m_bytLC1EventID = (byte)Event.NoEvent;
            m_bytKYZ1OutputType = (byte)OutputType.NotUsed;
            m_bytKYZ2OutputType = (byte)OutputType.NotUsed;
            m_bytLC1OutputType = (byte)OutputType.NotUsed;
            m_fltKYZ1PulseWt = 0.0f;
            m_fltKYZ2PulseWt = 0.0f;
            m_usKYZ1PulseWidth = 0;
            m_usKYZ2PulseWidth = 0;
            m_usLC1PulseWidth = 0;
            m_blnDisableOutputInTestMode = false;
        }

        /// <summary>
        /// This method determines if the given KYZ output uses a pulse weight.
        /// </summary>
        /// <param name="Output">The KYZ output to check.</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public bool UsesPulseWeight(KYZOutput Output)
        {
            bool blnUsesPulseWt = false;

            switch (Output)
            {
                case KYZOutput.KYZ1:
                    {
                        if ((byte)Energy.NoEnergy != m_bytKYZ1EnergyID)
                        {
                            blnUsesPulseWt = true;
                        }
                        break;
                    }
                case KYZOutput.KYZ2:
                    {
                        if ((byte)Energy.NoEnergy != m_bytKYZ2EnergyID)
                        {
                            blnUsesPulseWt = true;
                        }
                        break;
                    }
                case KYZOutput.LC1:
                    {
                        //Low current output never uses energy and thus never uses
                        //a pulse weight.
                        blnUsesPulseWt = false;
                        break;
                    }
                default:
                    {
                        blnUsesPulseWt = false;
                        break;
                    }
            }

            return blnUsesPulseWt;
        }

        /// <summary>
        /// This method determines if the given KYZ output uses a pulse width.
        /// </summary>
        /// <param name="Output">The KYZ output to check.</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public bool UsesPulseWidth(KYZOutput Output)
        {
            bool blnUsesPulseWidth = false;

            switch (Output)
            {
                case KYZOutput.KYZ1:
                    {
                        if ((byte)OutputType.NotUsed != m_bytKYZ1OutputType)
                        {
                            blnUsesPulseWidth = true;
                        }
                        break;
                    }
                case KYZOutput.KYZ2:
                    {
                        if ((byte)OutputType.NotUsed != m_bytKYZ2OutputType)
                        {
                            blnUsesPulseWidth = true;
                        }
                        break;
                    }
                case KYZOutput.LC1:
                    {
                        if ((byte)OutputType.NotUsed != m_bytLC1OutputType)
                        {
                            blnUsesPulseWidth = true;
                        }
                        break;
                    }
                default:
                    {
                        blnUsesPulseWidth = false;
                        break;
                    }
            }

            return blnUsesPulseWidth;
        }

        /// <summary>
        /// This method retrieves the text that should be displayed for the pulse width 
        /// meter value.
        /// </summary>
        /// <param name="Output">The KYZ output to check.</param>
        /// <returns>The text to show for pulse width.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a	Created
        //
        public string GetPulseWidthText(KYZOutput Output)
        {
            string strPulseWidthText = "";
            ushort usPulseWidth = 0;

            switch (Output)
            {
                case KYZOutput.KYZ1:
                    {
                        usPulseWidth = m_usKYZ1PulseWidth;
                        break;
                    }
                case KYZOutput.KYZ2:
                    {
                        usPulseWidth = m_usKYZ2PulseWidth;
                        break;
                    }
                case KYZOutput.LC1:
                    {
                        usPulseWidth = m_usLC1PulseWidth;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid KYZ Output: " + Output.ToString(), "Output");
                    }
            }

            //If zero is pulse width then pulse width is used to 
            //indicate a toggle or a state change.
            if (0 == usPulseWidth)
            {
                //If it uses a pulse weight, we know it's an energy.
                if (true == UsesPulseWeight(Output))
                {
                    strPulseWidthText = "Toggle";
                }
                else
                {
                    strPulseWidthText = "State Change";
                }
            }
            else
            {
                strPulseWidthText = usPulseWidth.ToString("0", CultureInfo.CurrentCulture) + " msec";
            }

            return strPulseWidthText;
        }

        /// <summary>
        /// This method retrieves the text that should be displayed for the pulse weight
        /// meter value.
        /// </summary>
        /// <param name="Output">The KYZ output to check.</param>
        /// <returns>The text to show for the pulse weight.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a	    Created
        // 11/18/14 AF  4.00.89 542760  Show only 2 decimal places in the value 
        //
        public string GetPulseWeightText(KYZOutput Output)
        {
            string strPulseWeightText;
            float fltPulseWeight = 0.0f;

            switch (Output)
            {
                case KYZOutput.KYZ1:
                    {
                        fltPulseWeight = m_fltKYZ1PulseWt;
                        break;
                    }
                case KYZOutput.KYZ2:
                    {
                        fltPulseWeight = m_fltKYZ2PulseWt;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid KYZ Output: " + Output.ToString(), "Output");
                    }
            }

            strPulseWeightText = fltPulseWeight.ToString("0.00", CultureInfo.CurrentCulture) + " Ke";

            return strPulseWeightText;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Get/Set KYZ Operation Description for KYZ1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public string KYZ1OperationDescription
        {
            get
            {
                if (string.IsNullOrEmpty(m_strKYZ1OperationDescription) || NONE == m_strKYZ1OperationDescription)
                {
                    m_strKYZ1OperationDescription = BuildDescription(m_bytKYZ1EnergyID, m_bytKYZ1EventID);
                }
                return m_strKYZ1OperationDescription;
            }
            set
            {
                m_strKYZ1OperationDescription = value;

                DecodeDescription(KYZOutput.KYZ1, m_strKYZ1OperationDescription);
            }
        }

        /// <summary>
        /// Get/Set KYZ Operation Description for KYZ2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public string KYZ2OperationDescription
        {
            get
            {
                if (string.IsNullOrEmpty(m_strKYZ2OperationDescription) || NONE == m_strKYZ2OperationDescription)
                {
                    m_strKYZ2OperationDescription = BuildDescription(m_bytKYZ2EnergyID, m_bytKYZ2EventID);
                }
                return m_strKYZ2OperationDescription;
            }
            set
            {
                m_strKYZ2OperationDescription = value;

                DecodeDescription(KYZOutput.KYZ2, m_strKYZ2OperationDescription);
            }
        }

        /// <summary>
        /// Get/Set KYZ Operation Description for LC1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public string LC1OperationDescription
        {
            get
            {
                if (string.IsNullOrEmpty(m_strLC1OperationDescription) || NONE == m_strLC1OperationDescription)
                {
                    m_strLC1OperationDescription = BuildDescription(0, m_bytLC1EventID);
                }
                return m_strLC1OperationDescription;
            }
            set
            {
                m_strLC1OperationDescription = value;

                DecodeDescription(KYZOutput.LC1, m_strLC1OperationDescription);
            }
        }

        /// <summary>
        /// Get/Set of Pulse Weight for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public float KYZ1PulseWeight
        {
            get
            {
                return m_fltKYZ1PulseWt;
            }
            set
            {
                m_fltKYZ1PulseWt = value;
            }
        }

        /// <summary>
        /// Get/Set of Pulse Weight for KYZ 2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public float KYZ2PulseWeight
        {
            get
            {
                return m_fltKYZ2PulseWt;
            }
            set
            {
                m_fltKYZ2PulseWt = value;
            }
        }

        /// <summary>
        /// Get/Set Pulse Width for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public ushort KYZ1PulseWidth
        {
            get
            {
                return m_usKYZ1PulseWidth;
            }
            set
            {
                m_usKYZ1PulseWidth = value;

                //If the pulse width is 0 then we need to make sure the output 
                //type is set to energy toggle or event state change.
                AdjustOutputType(ref m_bytKYZ1OutputType, m_usKYZ1PulseWidth);
            }
        }

        

        /// <summary>
        /// Get/Set Pulse Width for KYZ 2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public ushort KYZ2PulseWidth
        {
            get
            {
                return m_usKYZ2PulseWidth;
            }
            set
            {
                m_usKYZ2PulseWidth = value;

                //If the pulse width is 0 then we need to make sure the output 
                //type is set to energy toggle or event state change.
                AdjustOutputType(ref m_bytKYZ2OutputType, m_usKYZ2PulseWidth);
            }
        }

        /// <summary>
        /// Get/Set Pulse Width for LC 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public ushort LC1PulseWidth
        {
            get
            {
                return m_usLC1PulseWidth;
            }
            set
            {
                m_usLC1PulseWidth = value;

                //If the pulse width is 0 then we need to make sure the output 
                //type is set to energy toggle or event state change.
                AdjustOutputType(ref m_bytLC1OutputType, m_usLC1PulseWidth);
            }
        }

        /// <summary>
        /// Get/Set Energy ID for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ1EnergyID
        {
            get
            {
                return m_bytKYZ1EnergyID;
            }
            set
            {
                m_bytKYZ1EnergyID = value;
            }
        }

        /// <summary>
        /// Get/Set Energy ID for KYZ 2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ2EnergyID
        {
            get
            {
                return m_bytKYZ2EnergyID;
            }
            set
            {
                m_bytKYZ2EnergyID = value;
            }
        }

        /// <summary>
        /// Get/Set Event ID for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ1EventID
        {
            get
            {
                return m_bytKYZ1EventID;
            }
            set
            {
                m_bytKYZ1EventID = value;
            }
        }

        /// <summary>
        /// Get/Set Event ID for KYZ 2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ2EventID
        {
            get
            {
                return m_bytKYZ2EventID;
            }
            set
            {
                m_bytKYZ2EventID = value;
            }
        }

        /// <summary>
        /// Get/Set Event ID for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte LC1EventID
        {
            get
            {
                return m_bytLC1EventID;
            }
            set
            {
                m_bytLC1EventID = value;
            }
        }

        /// <summary>
        /// Get/Set Whether to disable all outputs while in test mode
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public bool DisableOutputInTestMode
        {
            get
            {
                return m_blnDisableOutputInTestMode;
            }
            set
            {
                m_blnDisableOutputInTestMode = value;
            }
        }

        /// <summary>
        /// Get/Set output type for KYZ 1.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ1OutputType
        {
            get
            {
                return m_bytKYZ1OutputType;
            }
            set
            {
                m_bytKYZ1OutputType = value;
            }
        }

        /// <summary>
        /// Get/Set output type for KYZ 2.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte KYZ2OutputType
        {
            get
            {
                return m_bytKYZ2OutputType;
            }
            set
            {
                m_bytKYZ2OutputType = value;
            }
        }

        /// <summary>
        /// Get/Set output type for Low Current.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        public byte LC1OutputType
        {
            get
            {
                return m_bytLC1OutputType;
            }
            set
            {
                m_bytLC1OutputType = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method builds a description a KYZ output based on the energy and event 
        /// IDs passed in.
        /// </summary>
        /// <param name="bytEnergyID">The energy ID.</param>
        /// <param name="bytEventID">The event ID.</param>
        /// <returns>The description for the KYZ output.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static string BuildDescription(byte bytEnergyID, byte bytEventID)
        {
            string strDescription = NONE;

            if ((byte)Energy.NoEnergy != bytEnergyID)
            {
                switch ((Energy)bytEnergyID)
                {
                    case Energy.WhD:
                        {
                            strDescription = WH_D;
                            break;
                        }
                    case Energy.WhR:
                        {
                            strDescription = WH_R;
                            break;
                        }
                    case Energy.VarhQ1:
                        {
                            strDescription = VARH_Q1;
                            break;
                        }
                    case Energy.VarhQ2:
                        {
                            strDescription = VARH_Q2;
                            break;
                        }
                    case Energy.VarhQ3:
                        {
                            strDescription = VARH_Q3;
                            break;
                        }
                    case Energy.VarhQ4:
                        {
                            strDescription = VARH_Q4;
                            break;
                        }
                    case Energy.VAhArithD:
                        {
                            strDescription = VAH_ARITH_D;
                            break;
                        }
                    case Energy.VAhArithR:
                        {
                            strDescription = VAH_ARITH_R;
                            break;
                        }
                    case Energy.VAhVecD:
                        {
                            strDescription = VAH_VEC_D;
                            break;
                        }
                    case Energy.VAhVecR:
                        {
                            strDescription = VAH_VEC_R;
                            break;
                        }
                    case Energy.VAhLag:
                        {
                            strDescription = VAH_LAG;
                            break;
                        }
                    case Energy.QhD:
                        {
                            strDescription = QH_D;
                            break;
                        }
                    case Energy.VhA:
                        {
                            strDescription = VH_A;
                            break;
                        }
                    case Energy.VhB:
                        {
                            strDescription = VH_B;
                            break;
                        }
                    case Energy.VhC:
                        {
                            strDescription = VH_C;
                            break;
                        }
                    case Energy.VhAvg:
                        {
                            strDescription = VH_AVG;
                            break;
                        }
                    case Energy.AhA:
                        {
                            strDescription = AH_A;
                            break;
                        }
                    case Energy.AhB:
                        {
                            strDescription = AH_B;
                            break;
                        }
                    case Energy.AhC:
                        {
                            strDescription = AH_C;
                            break;
                        }
                    case Energy.Vsqh:
                        {
                            strDescription = VSQH;
                            break;
                        }
                    case Energy.Asqh:
                        {
                            strDescription = ASQH;
                            break;
                        }
                    case Energy.VarhD:
                        {
                            strDescription = VARH_D;
                            break;
                        }
                    case Energy.VarhR:
                        {
                            strDescription = VARH_R;
                            break;
                        }
                    case Energy.QhR:
                        {
                            strDescription = QH_R;
                            break;
                        }
                    case Energy.Wh:
                        {
                            strDescription = WH;
                            break;
                        }
                    default:
                        {
                            //This is not a recognized KYZ output energy ID 
                            strDescription = UNKNOWN_ENERGY;
                            break;
                        }
                }
            }
            else if ((byte)Event.NoEvent != bytEventID)
            {
                switch ((Event)bytEventID)
                {
                    case Event.DemandReset:
                        {
                            strDescription = DEMAND_RESET;
                            break;
                        }
                    case Event.EndOfInterval:
                        {
                            strDescription = END_OF_INTERVAL;
                            break;
                        }
                    case Event.RateChange:
                        {
                            strDescription = RATE_CHANGE;
                            break;
                        }
                    case Event.SeasonChange:
                        {
                            strDescription = SEASON_CHANGE;
                            break;
                        }
                    default:
                        {
                            strDescription = UNKNOWN_EVENT;
                            break;
                        }
                }
            }

            return strDescription;
        }

        /// <summary>
        /// This method determines the event ID, energy ID and output type for 
        /// a particular KYZ output based on a description.
        /// </summary>
        /// <param name="KYZOutput">The KYZ output to obtain values for.</param>
        /// <param name="strDescription">The description to decode.</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        private void DecodeDescription(KYZOutput KYZOutput, string strDescription)
        {
            byte bytEventID = (byte)Event.NoEvent;
            byte bytEnergyID = (byte)Energy.NoEnergy;
            byte bytOutputType = (byte)OutputType.NotUsed;

            //Determine the energy and event IDs
            bytEventID = DetermineEventID(strDescription);

            if ((byte)Event.NoEvent == bytEventID)
            {
                bytEnergyID = DetermineEnergyID(strDescription);
            }
            
            //Default to pulse until proven otherwise
            if ((byte)Energy.NoEnergy != bytEnergyID)
            {
                bytOutputType = (byte)OutputType.PulseEnergy;
            }
            else if ((byte)Event.NoEvent != bytEventID)
            {
                bytOutputType = (byte)OutputType.PulseEvent;
            }

            //Set output type, energy ID and event ID based on the KYZ output
            switch (KYZOutput)
            {
                case KYZOutput.KYZ1:
                    {
                        m_bytKYZ1EnergyID = bytEnergyID;
                        m_bytKYZ1EventID = bytEventID;
                        
                        //Change output type to toggle or state change if necessary
                        AdjustOutputType(ref bytOutputType, m_usKYZ1PulseWidth);

                        m_bytKYZ1OutputType = bytOutputType;
                        break;
                    }
                case KYZOutput.KYZ2:
                    {
                        m_bytKYZ2EnergyID = bytEnergyID;
                        m_bytKYZ2EventID = bytEventID;

                        //Change output type to toggle or state change if necessary
                        AdjustOutputType(ref bytOutputType, m_usKYZ2PulseWidth);

                        m_bytKYZ2OutputType = bytOutputType;
                        break;
                    }
                case KYZOutput.LC1:
                    {
                        m_bytLC1EventID = bytEventID;

                        //Change output type to toggle or state change if necessary
                        AdjustOutputType(ref bytOutputType, m_usLC1PulseWidth);

                        m_bytLC1OutputType = bytOutputType;
                        break;
                    }
                default:
                    break;
            }

        }

        /// <summary>
        /// This method deterimies which event ID should be used based on a 
        /// description.
        /// </summary>
        /// <param name="strDescription">The description to determine event ID from.</param>
        /// <returns>The event ID.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        private static byte DetermineEventID(string strDescription)
        {
            byte bytEventID = (byte)Event.NoEvent;

            switch (strDescription)
            {
                case DEMAND_RESET:
                    {
                        bytEventID = (byte)Event.DemandReset;
                        break;
                    }
                case END_OF_INTERVAL:
                    {
                        bytEventID = (byte)Event.EndOfInterval;
                        break;
                    }
                case RATE_CHANGE:
                    {
                        bytEventID = (byte)Event.RateChange;
                        break;
                    }
                
                default:
                    {
                        //This is not a recognized KYZ output event 
                        bytEventID = (byte)Event.NoEvent;
                        break;
                    }
            }

            return bytEventID;
        }

        /// <summary>
        /// This method deterimies which energy ID should be used based on a 
        /// description.
        /// </summary>
        /// <param name="strDescription">The description to determine energy ID from.</param>
        /// <returns>The energy ID.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static byte DetermineEnergyID(string strDescription)
        {
            byte bytEnergyID = (byte)Energy.NoEnergy;
            switch (strDescription)
            {
                case WH_D:
                    {
                        bytEnergyID = (byte)Energy.WhD;
                        break;
                    }
                case WH_R:
                    {
                        bytEnergyID = (byte)Energy.WhR;
                        break;
                    }
                case VARH_Q1:
                    {
                        bytEnergyID = (byte)Energy.VarhQ1;
                        break;
                    }
                case VARH_Q2:
                    {
                        bytEnergyID = (byte)Energy.VarhQ2;
                        break;
                    }
                case VARH_Q3:
                    {
                        bytEnergyID = (byte)Energy.VarhQ3;
                        break;
                    }
                case VARH_Q4:
                    {
                        bytEnergyID = (byte)Energy.VarhQ4;
                        break;
                    }
                case VAH_ARITH_D:
                    {
                        bytEnergyID = (byte)Energy.VAhArithD;
                        break;
                    }
                case VAH_ARITH_R:
                    {
                        bytEnergyID = (byte)Energy.VAhArithR;
                        break;
                    }
                case VAH_VEC_D:
                    {
                        bytEnergyID = (byte)Energy.VAhVecD;
                        break;
                    }
                case VAH_VEC_R:
                    {
                        bytEnergyID = (byte)Energy.VAhVecR;
                        break;
                    }
                case VAH_LAG:
                    {
                        bytEnergyID = (byte)Energy.VAhLag;
                        break;
                    }
                case QH_D:
                    {
                        bytEnergyID = (byte)Energy.QhD;
                        break;
                    }
                case VH_A:
                    {
                        bytEnergyID = (byte)Energy.VhA;
                        break;
                    }
                case VH_B:
                    {
                        bytEnergyID = (byte)Energy.VhB;
                        break;
                    }
                case VH_C:
                    {
                        bytEnergyID = (byte)Energy.VhC;
                        break;
                    }
                case VH_AVG:
                    {
                        bytEnergyID = (byte)Energy.VhAvg;
                        break;
                    }
                case AH_A:
                    {
                        bytEnergyID = (byte)Energy.AhA;
                        break;
                    }
                case AH_B:
                    {
                        bytEnergyID = (byte)Energy.AhB;
                        break;
                    }
                case AH_C:
                    {
                        bytEnergyID = (byte)Energy.AhC;
                        break;
                    }
                case VSQH:
                    {
                        bytEnergyID = (byte)Energy.Vsqh;
                        break;
                    }
                case ASQH:
                    {
                        bytEnergyID = (byte)Energy.Asqh;
                        break;
                    }
                case VARH_D:
                    {
                        bytEnergyID = (byte)Energy.VarhD;
                        break;
                    }
                case VARH_R:
                    {
                        bytEnergyID = (byte)Energy.VarhR;
                        break;
                    }
                case QH_R:
                    {
                        bytEnergyID = (byte)Energy.QhR;
                        break;
                    }
                case WH:
                    {
                        bytEnergyID = (byte)Energy.Wh;
                        break;
                    }
                default:
                    {
                        //This is not a recognized KYZ output energy quantity 
                        bytEnergyID = (byte)Energy.NoEnergy;
                        break;
                    }
            }

            return bytEnergyID;
        }

        /// <summary>
        /// This method adjust the output type based on the given pulse width.
        /// </summary>
        /// <param name="bytOutputType">The output type to adjust.</param>
        /// <param name="uiPulseWidth">The pulse width to use to determine
        /// what adjustment to make.</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/20/09 jrf 2.20.02 n/a    Created.
        //
        private static void AdjustOutputType(ref byte bytOutputType, uint uiPulseWidth)
        {
            if (0 == uiPulseWidth)
            {
                //Toggle or state change if pulse width is zero.
                if ((byte)OutputType.PulseEnergy == bytOutputType)
                {
                    bytOutputType = (byte)OutputType.ToggleEnergy;
                }
                else if ((byte)OutputType.PulseEvent == bytOutputType)
                {
                    bytOutputType = (byte)OutputType.StateChangeEvent;
                }
            }
            else
            {
                //pulse if pulse width is non-zero.
                if ((byte)OutputType.ToggleEnergy == bytOutputType)
                {
                    bytOutputType = (byte)OutputType.PulseEnergy;
                }
                else if ((byte)OutputType.StateChangeEvent == bytOutputType)
                {
                    bytOutputType = (byte)OutputType.PulseEvent;
                }
            }
        }

        #endregion

        #region Members

        private string m_strKYZ1OperationDescription;
        private string m_strKYZ2OperationDescription;
        private string m_strLC1OperationDescription;
        private byte m_bytKYZ1EnergyID;
        private byte m_bytKYZ2EnergyID;
        private byte m_bytKYZ1EventID;
        private byte m_bytKYZ2EventID;
        private byte m_bytLC1EventID;
        private byte m_bytKYZ1OutputType;
        private byte m_bytKYZ2OutputType;
        private byte m_bytLC1OutputType;
        private float m_fltKYZ1PulseWt;
        private float m_fltKYZ2PulseWt;
        private ushort m_usKYZ1PulseWidth;
        private ushort m_usKYZ2PulseWidth;
        private ushort m_usLC1PulseWidth;
        private bool m_blnDisableOutputInTestMode;

        #endregion
    }
}
