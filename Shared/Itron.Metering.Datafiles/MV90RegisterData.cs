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
//                              Copyright © 2008 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Datafiles.Properties;

namespace Itron.Metering.Datafiles
{
   
    /// <summary>
    /// This abstract class represents the common elements of TOU register data in an HHF file.   
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 02/05/08 jrf 1.00.00        Created
    abstract public class MV90RegisterData
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dtReadTime">The date/time of the TOU register read.</param>
        /// <param name="strDataFormat">The format of the TOU register data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public MV90RegisterData(DateTime dtReadTime, string strDataFormat)
        {
            m_dtReadTime = dtReadTime;
            m_strDataFormat = strDataFormat;
        }

        /// <summary>
        /// This virtual method retrieves all available records. 
        /// </summary>
        /// <returns>A list of records.</returns>
        /// <exception cref="NotImplementedException">
        /// Throws a NotImplementException since this method is only implemented in derived 
        /// classes.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        virtual public List<MV90RegisterRecord> RetrieveRecords()
        {
            throw (new NotImplementedException());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets a DateTime that represents the time the register data was read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public DateTime ReadTime
        {
            get
            {
                return m_dtReadTime;
            }

            set
            {
                m_dtReadTime = value;
            }
        }

        /// <summary>
        /// Gets/sets a string that represents the data format of the register data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public string DataFormat
        {
            get
            {
                return m_strDataFormat;
            }

            set
            {
                m_strDataFormat = value;
            }
        }

        /// <summary>
        /// Gets a string that represents the type of meter that the register data
        /// comes from.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws a NotImplementException since this property is only implemented in derived 
        /// classes.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/20/08 jrf	1.00.00	   	   Created
        virtual public string MeterType
        {
            get
            {
                throw (new NotImplementedException());
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The time the register data was read.
        /// </summary>
        protected DateTime m_dtReadTime;

        /// <summary>
        /// The format of the register data
        /// </summary>
        protected string m_strDataFormat;

        #endregion
    }

    /// <summary>
    /// This class represents the common elements of TOU register data for ANSI meters.
    /// </summary>
    public class ANSIMV90RegisterData : MV90RegisterData
    {

        #region Definitions

        /// <summary>
        /// The Type of register value recorded.
        /// </summary>
        public enum RegisterType
        {
            /// <summary>
            /// TOTAL = 0,
            /// </summary>
            TOTAL = 0,
            /// <summary>
            /// RATEA = 1
            /// </summary>
            RATEA = 1,
            /// <summary>
            /// RATEB = 2
            /// </summary>
            RATEB = 2,
            /// <summary>
            /// RATEC = 3
            /// </summary>
            RATEC = 3,
            /// <summary>
            /// RATED = 4
            /// </summary>
            RATED = 4,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public ANSIMV90RegisterData(DateTime dtReadTime, string strDataFormat)
            : base(dtReadTime, strDataFormat)
        {

            //Initialize variables
            m_blnClockEnabled = false;
            m_blnDemThresholdExceeded = false;
            m_blnDSTConfigured = false;
            m_blnLastBPDemThresholdExceeded = false;
            m_blnLastSRDemThresholdExceeded = false;
            m_blnPFAvgAvailable = false;
            m_blnSelfReadData = false;
            m_ablnNonFatalErrors = new bool[6];
            m_ablnNonFatalErrors.Initialize();
            m_ablnFatalErrors = new bool[7];
            m_ablnFatalErrors.Initialize();
            m_ablnReads = new bool[24];
            m_ablnReads.Initialize();
            m_ablnLastBPNonFatalErrors = new bool[6];
            m_ablnLastBPNonFatalErrors.Initialize();
            m_ablnLastBPFatalErrors = new bool[7];
            m_ablnLastBPFatalErrors.Initialize();
            m_ablnLastSRNonFatalErrors = new bool[6];
            m_ablnLastSRNonFatalErrors.Initialize();
            m_ablnLastSRFatalErrors = new bool[7];
            m_ablnLastSRFatalErrors.Initialize();
            m_dtCurrentTime = new DateTime();
            m_dtLastBPTime = new DateTime();
            m_dtLastSRTime = new DateTime();
            m_dtTimeLastOutage = new DateTime();
            m_dtTimeLastInterrogation = new DateTime();
            m_fltCTMultiplier = 0.0f;
            m_fltFWVersionRevision = 0.0f;
            m_fltRegisterMultiplier = 0.0f;
            m_fltVTMultiplier = 0.0f;
            m_dblWhDelivered = 0.0f;
            m_CurrentBatteryReading = 0;
            m_DaysOnBattery = 0;
            m_iDaysSinceLastDR = 0;
            m_iDaysSinceLastTest = 0;
            m_iDemandIntervalLength = 0;
            m_iDemandResetCount = 0;
            m_iEPFCount = 0;
            m_iGoodBatteryReading = 0;
            m_iLastBPDemandResetCount = 0;
            m_iLastBPEPFCount = 0;
            m_iLastBPNumTimesProgrammed = 0;
            m_iLastBPOutageCount = 0;
            m_iLastSRDemandResetCount = 0;
            m_iLastSREPFCount = 0;
            m_iLastSRNumTimesProgrammed = 0;
            m_iLastSROutageCount = 0;
            m_iNumCumDemands = 0;
            m_iNumDemands = 0;
            m_iNumEnergies = 0;
            m_iNumLPChannels = 0;
            m_iNumTimesProgrammed = 0;
            m_iNumTOURates = 0;
            m_iOutageCount = 0;
            m_iProgramID = 0;
            m_iRateID = 0;
            m_auiCumDemandLIDs = null;
            m_auiDemandLIDs = null;
            m_auiDemandTOOLIDs = null;
            m_auiEnergyLIDs = null;
            m_CurrentEnergyQuantities = new QuantityCollection(); 
            m_CurrentDemandQuantities = new QuantityCollection(); 
            m_CurrentCumDemandQuantities = new QuantityCollection();
            m_LastBPEnergyQuantities = new QuantityCollection();
            m_LastBPDemandQuantities = new QuantityCollection();
            m_LastBPCumDemandQuantities = new QuantityCollection();
            m_LastSREnergyQuantities = new QuantityCollection();
            m_LastSRDemandQuantities = new QuantityCollection();
            m_LastSRCumDemandQuantities = new QuantityCollection();


        }

        /// <summary>
        /// This methods retrieves all available configuration records.
        /// </summary>
        /// <returns>A list of records.</returns>
        /// <exception cref="NotImplementedException">
        /// Throws a NotImplementException since this method is only implemented in derived 
        /// classes.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/25/08 jrf	1.00.00	   	   Created
        virtual public List<MV90RegisterRecord> RetrieveConfigurationRecords()
        {
            throw (new NotImplementedException());
        }

        /// <summary>
        /// This methods retrieves all available register records.
        /// </summary>
        /// <returns>A list of records.</returns>
        /// <exception cref="NotImplementedException">
        /// Throws a NotImplementException since this method is only implemented in derived 
        /// classes.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/25/08 jrf	1.00.00	   	   Created
        virtual public List<MV90RegisterRecord> RetrieveRegisterRecords()
        {
            throw (new NotImplementedException());
        }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets a bool that represents whether the clock is enabled in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool ClockEnabled
        {
            get
            {
                return m_blnClockEnabled;
            }

            set
            {
                m_blnClockEnabled = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a demand threshold exceeded error
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool DemandThresholdExceeded
        {
            get
            {
                return m_blnDemThresholdExceeded;
            }

            set
            {
                m_blnDemThresholdExceeded = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether DST is configured.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool DSTConfigured
        {
            get
            {
                return m_blnDSTConfigured;
            }

            set
            {
                m_blnDSTConfigured = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 1
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool NonFatalError1
        {
            get
            {
                return m_ablnNonFatalErrors[0];
            }

            set
            {
                m_ablnNonFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 2
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool NonFatalError2
        {
            get
            {
                return m_ablnNonFatalErrors[1];
            }

            set
            {
                m_ablnNonFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 3
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool NonFatalError3
        {
            get
            {
                return m_ablnNonFatalErrors[2];
            }

            set
            {
                m_ablnNonFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 4
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	10.00.00	   Created
        // 02/28/08 jrf 10.00.00       Corrected typo.
        public bool NonFatalError4
        {
            get
            {
                return m_ablnNonFatalErrors[3];
            }

            set
            {
                m_ablnNonFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 5
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool NonFatalError5
        {
            get
            {
                return m_ablnNonFatalErrors[4];
            }

            set
            {
                m_ablnNonFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 6
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool NonFatalError6
        {
            get
            {
                return m_ablnNonFatalErrors[5];
            }

            set
            {
                m_ablnNonFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 1
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError1
        {
            get
            {
                return m_ablnFatalErrors[0];
            }

            set
            {
                m_ablnFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 2
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError2
        {
            get
            {
                return m_ablnFatalErrors[1];
            }

            set
            {
                m_ablnFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 3
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError3
        {
            get
            {
                return m_ablnFatalErrors[2];
            }

            set
            {
                m_ablnFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 4
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError4
        {
            get
            {
                return m_ablnFatalErrors[3];
            }

            set
            {
                m_ablnFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 5
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError5
        {
            get
            {
                return m_ablnFatalErrors[4];
            }

            set
            {
                m_ablnFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 6
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError6
        {
            get
            {
                return m_ablnFatalErrors[5];
            }

            set
            {
                m_ablnFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 7
        /// is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool FatalError7
        {
            get
            {
                return m_ablnFatalErrors[6];
            }

            set
            {
                m_ablnFatalErrors[6] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a last billing period demand threshold exceeded 
        /// error is active.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPDemandThresholdExceeded
        {
            get
            {
                return m_blnLastBPDemThresholdExceeded;
            }

            set
            {
                m_blnLastBPDemThresholdExceeded = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 1
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError1
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[0];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 2
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError2
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[1];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 3
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError3
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[2];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 4
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError4
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[3];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 5
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError5
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[4];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 6
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPNonFatalError6
        {
            get
            {
                return m_ablnLastBPNonFatalErrors[5];
            }

            set
            {
                m_ablnLastBPNonFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 1
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError1
        {
            get
            {
                return m_ablnLastBPFatalErrors[0];
            }

            set
            {
                m_ablnLastBPFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 2
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError2
        {
            get
            {
                return m_ablnLastBPFatalErrors[1];
            }

            set
            {
                m_ablnLastBPFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 3
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError3
        {
            get
            {
                return m_ablnLastBPFatalErrors[2];
            }

            set
            {
                m_ablnLastBPFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 4
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError4
        {
            get
            {
                return m_ablnLastBPFatalErrors[3];
            }

            set
            {
                m_ablnLastBPFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 5
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError5
        {
            get
            {
                return m_ablnLastBPFatalErrors[4];
            }

            set
            {
                m_ablnLastBPFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 6
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError6
        {
            get
            {
                return m_ablnLastBPFatalErrors[5];
            }

            set
            {
                m_ablnLastBPFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 7
        /// is active for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastBPFatalError7
        {
            get
            {
                return m_ablnLastBPFatalErrors[6];
            }

            set
            {
                m_ablnLastBPFatalErrors[6] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a demand threshold exceeded 
        /// error is active during the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRDemandThresholdExceeded
        {
            get
            {
                return m_blnLastSRDemThresholdExceeded;
            }

            set
            {
                m_blnLastSRDemThresholdExceeded = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 1
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError1
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[0];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 2
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError2
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[1];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 3
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError3
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[2];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 4
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError4
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[3];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 5
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError5
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[4];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Non-Fatal error 6
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRNonFatalError6
        {
            get
            {
                return m_ablnLastSRNonFatalErrors[5];
            }

            set
            {
                m_ablnLastSRNonFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 1
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError1
        {
            get
            {
                return m_ablnLastSRFatalErrors[0];
            }

            set
            {
                m_ablnLastSRFatalErrors[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 2
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError2
        {
            get
            {
                return m_ablnLastSRFatalErrors[1];
            }

            set
            {
                m_ablnLastSRFatalErrors[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 3
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError3
        {
            get
            {
                return m_ablnLastSRFatalErrors[2];
            }

            set
            {
                m_ablnLastSRFatalErrors[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 4
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError4
        {
            get
            {
                return m_ablnLastSRFatalErrors[3];
            }

            set
            {
                m_ablnLastSRFatalErrors[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 5
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError5
        {
            get
            {
                return m_ablnLastSRFatalErrors[4];
            }

            set
            {
                m_ablnLastSRFatalErrors[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 6
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError6
        {
            get
            {
                return m_ablnLastSRFatalErrors[5];
            }

            set
            {
                m_ablnLastSRFatalErrors[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether a Fatal error 7
        /// is active for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool LastSRFatalError7
        {
            get
            {
                return m_ablnLastSRFatalErrors[6];
            }

            set
            {
                m_ablnLastSRFatalErrors[6] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether Power Factor Average billing period
        /// is available.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool PFAverageAvailable
        {
            get
            {
                return m_blnPFAvgAvailable;
            }

            set
            {
                m_blnPFAvgAvailable = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether self read data
        /// is available.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public bool SelfReadDataAvailable
        {
            get
            {
                return m_blnSelfReadData;
            }

            set
            {
                m_blnSelfReadData = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 0 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read0Present
        {
            get
            {
                return m_ablnReads[0];
            }

            set
            {
                m_ablnReads[0] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 1 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read1Present
        {
            get
            {
                return m_ablnReads[1];
            }

            set
            {
                m_ablnReads[1] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 2 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read2Present
        {
            get
            {
                return m_ablnReads[2];
            }

            set
            {
                m_ablnReads[2] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 3 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read3Present
        {
            get
            {
                return m_ablnReads[3];
            }

            set
            {
                m_ablnReads[3] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 4 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read4Present
        {
            get
            {
                return m_ablnReads[4];
            }

            set
            {
                m_ablnReads[4] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 5 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read5Present
        {
            get
            {
                return m_ablnReads[5];
            }

            set
            {
                m_ablnReads[5] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 6 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read6Present
        {
            get
            {
                return m_ablnReads[6];
            }

            set
            {
                m_ablnReads[6] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 7 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read7Present
        {
            get
            {
                return m_ablnReads[7];
            }

            set
            {
                m_ablnReads[7] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 8 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read8Present
        {
            get
            {
                return m_ablnReads[8];
            }

            set
            {
                m_ablnReads[8] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 9 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read9Present
        {
            get
            {
                return m_ablnReads[9];
            }

            set
            {
                m_ablnReads[9] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 10 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read10Present
        {
            get
            {
                return m_ablnReads[10];
            }

            set
            {
                m_ablnReads[10] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 11 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read11Present
        {
            get
            {
                return m_ablnReads[11];
            }

            set
            {
                m_ablnReads[11] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 12 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read12Present
        {
            get
            {
                return m_ablnReads[12];
            }

            set
            {
                m_ablnReads[12] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 13 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read13Present
        {
            get
            {
                return m_ablnReads[13];
            }

            set
            {
                m_ablnReads[13] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 14 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read14Present
        {
            get
            {
                return m_ablnReads[14];
            }

            set
            {
                m_ablnReads[14] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 15 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read15Present
        {
            get
            {
                return m_ablnReads[15];
            }

            set
            {
                m_ablnReads[15] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 16 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read16Present
        {
            get
            {
                return m_ablnReads[16];
            }

            set
            {
                m_ablnReads[16] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 17 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read17Present
        {
            get
            {
                return m_ablnReads[17];
            }

            set
            {
                m_ablnReads[17] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 18 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read18Present
        {
            get
            {
                return m_ablnReads[18];
            }

            set
            {
                m_ablnReads[18] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 19 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read19Present
        {
            get
            {
                return m_ablnReads[19];
            }

            set
            {
                m_ablnReads[19] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 20 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read20Present
        {
            get
            {
                return m_ablnReads[20];
            }

            set
            {
                m_ablnReads[20] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 21 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read21Present
        {
            get
            {
                return m_ablnReads[21];
            }

            set
            {
                m_ablnReads[21] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 22 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read22Present
        {
            get
            {
                return m_ablnReads[22];
            }

            set
            {
                m_ablnReads[22] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 23 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read23Present
        {
            get
            {
                return m_ablnReads[23];
            }

            set
            {
                m_ablnReads[23] = value;
            }
        }

        /// <summary>
        /// Gets/sets a bool that represents whether read 24 is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public bool Read24Present
        {
            get
            {
                return m_ablnReads[24];
            }

            set
            {
                m_ablnReads[24] = value;
            }
        }


        /// <summary>
        /// Gets/sets a DateTime that represents the current time of the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime CurrentTime
        {
            get
            {
                return m_dtCurrentTime;
            }

            set
            {
                m_dtCurrentTime = value;
            }
        }

        /// <summary>
        /// Gets/sets a DateTime that represents the time of the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime LastBPTime
        {
            get
            {
                return m_dtLastBPTime;
            }

            set
            {
                m_dtLastBPTime = value;
            }
        }

        /// <summary>
        /// Gets/sets a DateTime that represents the time of the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime LastSRTime
        {
            get
            {
                return m_dtLastSRTime;
            }

            set
            {
                m_dtLastSRTime = value;
            }
        }

        /// <summary>
        /// Gets/sets a DateTime that represents the time of the last power outage.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime TimeLastOutage
        {
            get
            {
                return m_dtTimeLastOutage;
            }

            set
            {
                m_dtTimeLastOutage = value;
            }
        }

        /// <summary>
        /// Gets/sets a DateTime that represents the time of the last interrogation.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime TimeLastInterrogation
        {
            get
            {
                return m_dtTimeLastInterrogation;
            }

            set
            {
                m_dtTimeLastInterrogation = value;
            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the total demand TOO values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] TotalDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate A demand TOO values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] RateADemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate B demand TOO values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] RateBDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate C demand TOO values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] RateCDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate D demand TOO values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] RateDDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandTOOValues(m_CurrentDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the total demand TOO values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastBPTotalDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate A demand TOO values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastBPRateADemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate B demand TOO values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastBPRateBDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate C demand TOO values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastBPRateCDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate D demand TOO values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastBPRateDDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandTOOValues(m_LastBPDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the total demand TOO values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastSRTotalDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate A demand TOO values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastSRRateADemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate B demand TOO values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastSRRateBDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate C demand TOO values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastSRRateCDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a DateTime array that represents the rate D demand TOO values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public DateTime[] LastSRRateDDemandTOOs
        {
            get
            {
                return GetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandTOOValues(m_LastSRDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float that represents the CT multiplier.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public float CTMultiplier
        {
            get
            {
                return m_fltCTMultiplier;
            }

            set
            {
                m_fltCTMultiplier = value;
            }
        }

        /// <summary>
        /// Gets/sets a float that represents the firmware version and revision.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public float FWVersionRevision
        {
            get
            {
                return m_fltFWVersionRevision;
            }

            set
            {
                m_fltFWVersionRevision = value;
            }
        }

        /// <summary>
        /// Gets/sets a float that represents the register multiplier.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public float RegisterMultiplier
        {
            get
            {
                return m_fltRegisterMultiplier;
            }

            set
            {
                m_fltRegisterMultiplier = value;
            }
        }

        /// <summary>
        /// Gets/sets a float that represents the VT multiplier.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public float VTMultiplier
        {
            get
            {
                return m_fltVTMultiplier;
            }

            set
            {
                m_fltVTMultiplier = value;
            }
        }

        /// <summary>
        /// Gets/sets a float that represents the WhDelivered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double WhDelivered
        {
            get
            {
                return m_dblWhDelivered;
            }

            set
            {
                m_dblWhDelivered = value;
            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total cumulative demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] TotalCumDemands
        {
            get
            {
                return GetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] TotalDemands
        {
            get
            {
                return GetDemandValues(m_CurrentDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandValues(m_CurrentDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total energy values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] TotalEnergies
        {
            get
            {
                return GetEnergyValues(m_CurrentEnergyQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetEnergyValues(m_CurrentEnergyQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate A cumulative demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateACumDemands
        {
            get
            {
                return GetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateADemands
        {
            get
            {
                return GetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A energy values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateAEnergies
        {
            get
            {
                return GetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEA);
            }

            set
            {
                SetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate B cumulative demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateBCumDemands
        {
            get
            {
                return GetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateBDemands
        {
            get
            {
                return GetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B energy values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateBEnergies
        {
            get
            {
                return GetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEB);
            }

            set
            {
                SetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate C cumulative demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateCCumDemands
        {
            get
            {
                return GetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateCDemands
        {
            get
            {
                return GetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandValues(m_CurrentDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C energy values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateCEnergies
        {
            get
            {
                return GetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEC);
            }

            set
            {
                SetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate D cumulative demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateDCumDemands
        {
            get
            {
                return GetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetCumDemandValues(m_CurrentCumDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D demand values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateDDemands
        {
            get
            {
                return GetDemandValues(m_CurrentDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandValues(m_CurrentDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D energy values.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] RateDEnergies
        {
            get
            {
                return GetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATED);
            }

            set
            {
                SetEnergyValues(m_CurrentEnergyQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total cumulative demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPTotalCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPTotalDemands
        {
            get
            {
                return GetDemandValues(m_LastBPDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandValues(m_LastBPDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total energy values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPTotalEnergies
        {
            get
            {
                return GetEnergyValues(m_LastBPEnergyQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetEnergyValues(m_LastBPEnergyQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate A cumulative demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateACumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateADemands
        {
            get
            {
                return GetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A energy values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateAEnergies
        {
            get
            {
                return GetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEA);
            }

            set
            {
                SetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate B cumulative demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateBCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateBDemands
        {
            get
            {
                return GetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B energy values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateBEnergies
        {
            get
            {
                return GetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEB);
            }

            set
            {
                SetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate C cumulative demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateCCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateCDemands
        {
            get
            {
                return GetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandValues(m_LastBPDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C energy values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateCEnergies
        {
            get
            {
                return GetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEC);
            }

            set
            {
                SetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate D cumulative demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateDCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetCumDemandValues(m_LastBPCumDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D demand values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateDDemands
        {
            get
            {
                return GetDemandValues(m_LastBPDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandValues(m_LastBPDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D energy values
        /// for the last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastBPRateDEnergies
        {
            get
            {
                return GetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATED);
            }

            set
            {
                SetEnergyValues(m_LastBPEnergyQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total cumulative demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRTotalCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRTotalDemands
        {
            get
            {
                return GetDemandValues(m_LastSRDemandQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetDemandValues(m_LastSRDemandQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents total energy values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRTotalEnergies
        {
            get
            {
                return GetEnergyValues(m_LastSREnergyQuantities, RegisterType.TOTAL);
            }

            set
            {
                SetEnergyValues(m_LastSREnergyQuantities, RegisterType.TOTAL, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate A cumulative demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateACumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateADemands
        {
            get
            {
                return GetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEA);
            }

            set
            {
                SetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate A energy values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateAEnergies
        {
            get
            {
                return GetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEA);
            }

            set
            {
                SetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEA, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate B cumulative demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateBCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateBDemands
        {
            get
            {
                return GetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEB);
            }

            set
            {
                SetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate B energy values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateBEnergies
        {
            get
            {
                return GetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEB);
            }

            set
            {
                SetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEB, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate C cumulative demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateCCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateCDemands
        {
            get
            {
                return GetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEC);
            }

            set
            {
                SetDemandValues(m_LastSRDemandQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate C energy values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateCEnergies
        {
            get
            {
                return GetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEC);
            }

            set
            {
                SetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATEC, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents Rate D cumulative demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateDCumDemands
        {
            get
            {
                return GetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetCumDemandValues(m_LastSRCumDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D demand values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateDDemands
        {
            get
            {
                return GetDemandValues(m_LastSRDemandQuantities, RegisterType.RATED);
            }

            set
            {
                SetDemandValues(m_LastSRDemandQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets a float array that represents rate D energy values
        /// for the last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public double[] LastSRRateDEnergies
        {
            get
            {
                return GetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATED);
            }

            set
            {
                SetEnergyValues(m_LastSREnergyQuantities, RegisterType.RATED, value);

            }
        }

        /// <summary>
        /// Gets/sets an int that represents the current battery reading.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int CurrentBatteryReading
        {
            get
            {
                return m_CurrentBatteryReading;
            }

            set
            {
                m_CurrentBatteryReading = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the days on battery.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int DaysOnBattery
        {
            get
            {
                return m_DaysOnBattery;
            }

            set
            {
                m_DaysOnBattery = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the days since the last demand reset.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int DaysSinceLastDR
        {
            get
            {
                return m_iDaysSinceLastDR;
            }

            set
            {
                m_iDaysSinceLastDR = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the days since the last test mode was entered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int DaysSinceLastTest
        {
            get
            {
                return m_iDaysSinceLastTest;
            }

            set
            {
                m_iDaysSinceLastTest = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the demand interval length.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int DemandIntervalLength
        {
            get
            {
                return m_iDemandIntervalLength;
            }

            set
            {
                m_iDemandIntervalLength = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the demand reset count.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int DemandResetCount
        {
            get
            {
                return m_iDemandResetCount;
            }

            set
            {
                m_iDemandResetCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the early power fail count.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int EPFCount
        {
            get
            {
                return m_iEPFCount;
            }

            set
            {
                m_iEPFCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the good battery reading.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int GoodBatteryReading
        {
            get
            {
                return m_iGoodBatteryReading;
            }

            set
            {
                m_iGoodBatteryReading = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the demand reset count for the 
        /// last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastBPDemandResetCount
        {
            get
            {
                return m_iLastBPDemandResetCount;
            }

            set
            {
                m_iLastBPDemandResetCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the early power fail count for the 
        /// last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastBPEPFCount
        {
            get
            {
                return m_iLastBPEPFCount;
            }

            set
            {
                m_iLastBPEPFCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of times programmed for the 
        /// last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastBPNumTimesProgrammed
        {
            get
            {
                return m_iLastBPNumTimesProgrammed;
            }

            set
            {
                m_iLastBPNumTimesProgrammed = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of power outages for the 
        /// last billing period.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastBPOutageCount
        {
            get
            {
                return m_iLastBPOutageCount;
            }

            set
            {
                m_iLastBPOutageCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the demand reset count for the 
        /// last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastSRDemandResetCount
        {
            get
            {
                return m_iLastSRDemandResetCount;
            }

            set
            {
                m_iLastSRDemandResetCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the early power fail count for the 
        /// last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastSREPFCount
        {
            get
            {
                return m_iLastSREPFCount;
            }

            set
            {
                m_iLastSREPFCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of times programmed for the 
        /// last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastSRNumTimesProgrammed
        {
            get
            {
                return m_iLastSRNumTimesProgrammed;
            }

            set
            {
                m_iLastSRNumTimesProgrammed = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of power outages for the 
        /// last self read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int LastSROutageCount
        {
            get
            {
                return m_iLastSROutageCount;
            }

            set
            {
                m_iLastSROutageCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of cumulative demands.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumCumDemands
        {
            get
            {
                return m_iNumCumDemands;
            }

            set
            {
                m_iNumCumDemands = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of demands.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumDemands
        {
            get
            {
                return m_iNumDemands;
            }

            set
            {
                m_iNumDemands = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of energies.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumEnergies
        {
            get
            {
                return m_iNumEnergies;
            }

            set
            {
                m_iNumEnergies = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of load profile channels.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumLPChannels
        {
            get
            {
                return m_iNumLPChannels;
            }

            set
            {
                m_iNumLPChannels = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of times programmed.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumTimesProgrammed
        {
            get
            {
                return m_iNumTimesProgrammed;
            }

            set
            {
                m_iNumTimesProgrammed = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of TOU rates.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int NumTOURates
        {
            get
            {
                return m_iNumTOURates;
            }

            set
            {
                m_iNumTOURates = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the number of power outages.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int OutageCount
        {
            get
            {
                return m_iOutageCount;
            }

            set
            {
                m_iOutageCount = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the program ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int ProgramID
        {
            get
            {
                return m_iProgramID;
            }

            set
            {
                m_iProgramID = value;
            }
        }

        /// <summary>
        /// Gets/sets an int that represents the rate ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public int RateID
        {
            get
            {
                return m_iRateID;
            }

            set
            {
                m_iRateID = value;
            }
        }

        /// <summary>
        /// Gets/sets an int array that represents the LIDs for each of the cumulative demands
        /// the meter is recording.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public uint[] CumDemandLIDs
        {
            get
            {
                return m_auiCumDemandLIDs;
            }

            set
            {
                m_auiCumDemandLIDs = value;
            }
        }

        /// <summary>
        /// Gets/sets an int array that represents the LIDs for each of the demands
        /// the meter is recording.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public uint[] DemandLIDs
        {
            get
            {
                return m_auiDemandLIDs;
            }

            set
            {
                m_auiDemandLIDs = value;
            }
        }

        /// <summary>
        /// Gets/sets an int array that represents the LIDs for each of the demand TOOs
        /// the meter is recording.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public uint[] DemandTOOLIDs
        {
            get
            {
                return m_auiDemandTOOLIDs;
            }

            set
            {
                m_auiDemandTOOLIDs = value;
            }
        }

        /// <summary>
        /// Gets/sets an int array that represents the LIDs for each of the energies
        /// the meter is recording.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
        public uint[] EnergyLIDs
        {
            get
            {
                return m_auiEnergyLIDs;
            }

            set
            {
                m_auiEnergyLIDs = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method gets demand time of occurence values from a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <returns>A DateTime array of the demand TOO values.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  06/25/08 jrf 1.50.44 115141 Ensuring that unused TOU rates are not accessed.
        //  08/20/09 jrf 2.21.03 139112 Limiting array size to NumDemands.
        //
        private DateTime[] GetDemandTOOValues(QuantityCollection Qtys, RegisterType eType)
        {
            DateTime[] adtDemandTOOs = new DateTime[NumDemands];
            Quantity Qty = null;

            //Make sure that we are not trying to access TOU rates that aren't available
            if ((int)eType <= m_iNumTOURates)
            {
                for (int i = 0; i < NumDemands; i++)
                {
                    Qty = Qtys.Quantities[i];

                    switch (eType)
                    {
                        case RegisterType.TOTAL:
                            {
                                if (null != Qty.TotalMaxDemand)
                                {
                                    adtDemandTOOs[i] = Qty.TotalMaxDemand.TimeOfOccurrence;
                                }
                                break;
                            }
                        case RegisterType.RATEA:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[0])
                                {
                                    adtDemandTOOs[i] = Qty.TOUMaxDemand[0].TimeOfOccurrence;
                                }
                                break;
                            }
                        case RegisterType.RATEB:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[1])
                                {
                                    adtDemandTOOs[i] = Qty.TOUMaxDemand[1].TimeOfOccurrence;
                                }
                                break;
                            }
                        case RegisterType.RATEC:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[2])
                                {
                                    adtDemandTOOs[i] = Qty.TOUMaxDemand[2].TimeOfOccurrence;
                                }
                                break;
                            }
                        case RegisterType.RATED:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[3])
                                {
                                    adtDemandTOOs[i] = Qty.TOUMaxDemand[3].TimeOfOccurrence;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            else
            {
                adtDemandTOOs = new DateTime[0];
            }
            return adtDemandTOOs;
        }

        /// <summary>
        /// This method gets cumulative demand values from a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <returns>A double array of the cumulative demand values.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  06/25/08 jrf 1.50.44 115141 Ensuring that unused TOU rates are not accessed.
        //  08/20/09 jrf 2.21.03 139112 Limiting array size to NumDemands.
        //
        private double[] GetCumDemandValues(QuantityCollection Qtys, RegisterType eType)
        {
            double[] adblCumDemandValues = new double[NumDemands];
            Quantity Qty = null;

            //Make sure that we are not trying to access TOU rates that aren't available
            if ((int)eType <= m_iNumTOURates)
            {
                for (int i = 0; i < NumDemands; i++)
                {
                    Qty = Qtys.Quantities[i];

                    switch (eType)
                    {
                        case RegisterType.TOTAL:
                            {
                                if (null != Qty.CummulativeDemand)
                                {
                                    adblCumDemandValues[i] = Qty.CummulativeDemand.Value;
                                }
                                break;
                            }
                        case RegisterType.RATEA:
                            {
                                if (null != Qty.TOUCummulativeDemand && null != Qty.TOUCummulativeDemand[0])
                                {
                                    adblCumDemandValues[i] = Qty.TOUCummulativeDemand[0].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEB:
                            {
                                if (null != Qty.TOUCummulativeDemand && null != Qty.TOUCummulativeDemand[1])
                                {
                                    adblCumDemandValues[i] = Qty.TOUCummulativeDemand[1].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEC:
                            {
                                if (null != Qty.TOUCummulativeDemand && null != Qty.TOUCummulativeDemand[2])
                                {
                                    adblCumDemandValues[i] = Qty.TOUCummulativeDemand[2].Value;
                                }
                                break;
                            }
                        case RegisterType.RATED:
                            {
                                if (null != Qty.TOUCummulativeDemand && null != Qty.TOUCummulativeDemand[3])
                                {
                                    adblCumDemandValues[i] = Qty.TOUCummulativeDemand[3].Value;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            else
            {
                adblCumDemandValues = new double[0];
            }
            return adblCumDemandValues;
        }

        /// <summary>
        /// This method gets demand values from a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <returns>A double array of the dmeand values.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  06/25/08 jrf 1.50.44 115141 Ensuring that unused TOU rates are not accessed.
        //  08/20/09 jrf 2.21.03 139112 Conditionally limiting array size to NumDemands
        //                              or NumDemands++ if avg PF is available and rate 
        //                              is total.
        //
        private double[] GetDemandValues(QuantityCollection Qtys, RegisterType eType)
        {
            int iNumDemands = NumDemands;
            if (true == PFAverageAvailable && RegisterType.TOTAL == eType)
            {
                iNumDemands++;
            }
            double[] adblDemandValues = new double[iNumDemands];
            Quantity Qty = null;

            //Make sure that we are not trying to access TOU rates that aren't available
            if ((int)eType <= m_iNumTOURates)
            {
                for (int i = 0; i < iNumDemands; i++)
                {
                    Qty = Qtys.Quantities[i];

                    switch (eType)
                    {
                        case RegisterType.TOTAL:
                            {
                                if (null != Qty.TotalMaxDemand)
                                {
                                    adblDemandValues[i] = Qty.TotalMaxDemand.Value;
                                }
                                break;
                            }
                        case RegisterType.RATEA:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[0])
                                {
                                    adblDemandValues[i] = Qty.TOUMaxDemand[0].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEB:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[1])
                                {
                                    adblDemandValues[i] = Qty.TOUMaxDemand[1].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEC:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[2])
                                {
                                    adblDemandValues[i] = Qty.TOUMaxDemand[2].Value;
                                }
                                break;
                            }
                        case RegisterType.RATED:
                            {
                                if (null != Qty.TOUMaxDemand && null != Qty.TOUMaxDemand[3])
                                {
                                    adblDemandValues[i] = Qty.TOUMaxDemand[3].Value;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            else
            {
                adblDemandValues = new double[0];
            }
            return adblDemandValues;
        }

        /// <summary>
        /// This method gets energy values from a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <returns>A double array of the energy values.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  06/25/08 jrf 1.50.44 115141 Ensuring that unused TOU rates are not accessed.
        //
        private double[] GetEnergyValues(QuantityCollection Qtys, RegisterType eType)
        {
            double[] adblEnergyValues = new double[Qtys.Quantities.Count];
            Quantity Qty = null;

            //Make sure that we are not trying to access TOU rates that aren't available
            if ((int)eType <= m_iNumTOURates)
            {
                for (int i = 0; i < Qtys.Quantities.Count; i++)
                {
                    Qty = Qtys.Quantities[i];

                    switch (eType)
                    {
                        case RegisterType.TOTAL:
                            {
                                if (null != Qty.TotalEnergy)
                                {
                                    adblEnergyValues[i] = Qty.TotalEnergy.Value;
                                }
                                break;
                            }
                        case RegisterType.RATEA:
                            {
                                if (null != Qty.TOUEnergy && null != Qty.TOUEnergy[0])
                                {
                                    adblEnergyValues[i] = Qty.TOUEnergy[0].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEB:
                            {
                                if (null != Qty.TOUEnergy && null != Qty.TOUEnergy[1])
                                {
                                    adblEnergyValues[i] = Qty.TOUEnergy[1].Value;
                                }
                                break;
                            }
                        case RegisterType.RATEC:
                            {
                                if (null != Qty.TOUEnergy && null != Qty.TOUEnergy[2])
                                {
                                    adblEnergyValues[i] = Qty.TOUEnergy[2].Value;
                                }
                                break;
                            }
                        case RegisterType.RATED:
                            {
                                if (null != Qty.TOUEnergy && null != Qty.TOUEnergy[3])
                                {
                                    adblEnergyValues[i] = Qty.TOUEnergy[3].Value;
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            else
            {
                adblEnergyValues = new double[0];
            }
            return adblEnergyValues;
        }

        /// <summary>
        /// This method sets demand time of occurence values in a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <param name="dtValue">The value to set.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  08/20/09 jrf 2.21.03 139112 Modified to not remove quantities when adding new ones.
        //
        private void SetDemandTOOValues(QuantityCollection Qtys, RegisterType eType, DateTime[] dtValue)
        {
            DateTime[] adtDemandTOOs = dtValue;
            Quantity Qty = null;

            //No quantities yet, add 'em
            if (0 == Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adtDemandTOOs.Length);
            }
            else if (adtDemandTOOs.Length > Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adtDemandTOOs.Length - Qtys.Quantities.Count);
            }

            for (int i = 0; i < adtDemandTOOs.Length; i++)
            {
                Qty = Qtys.Quantities[i];



                switch (eType)
                {
                    case RegisterType.TOTAL:
                        {
                            if (null == Qty.TotalMaxDemand)
                            {
                                Qty.TotalMaxDemand = new DemandMeasurement();
                            }
                            Qty.TotalMaxDemand.TimeOfOccurrence = adtDemandTOOs[i];

                            break;
                        }
                    case RegisterType.RATEA:
                        {
                            SetDemandTOUTOOValue(adtDemandTOOs[i], Qty, 0);

                            break;
                        }
                    case RegisterType.RATEB:
                        {
                            SetDemandTOUTOOValue(adtDemandTOOs[i], Qty, 1);

                            break;
                        }
                    case RegisterType.RATEC:
                        {
                            SetDemandTOUTOOValue(adtDemandTOOs[i], Qty, 2);

                            break;
                        }
                    case RegisterType.RATED:
                        {
                            SetDemandTOUTOOValue(adtDemandTOOs[i], Qty, 3);

                            break;
                        }
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// This method sets cumulative demand values in a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <param name="dblValue">The value to set.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  08/20/09 jrf 2.21.03 139112 Modified to not remove quantities when adding new ones.
        //
        private void SetCumDemandValues(QuantityCollection Qtys, RegisterType eType, double[] dblValue)
        {
            double[] adblCumDemandValues = dblValue;
            Quantity Qty = null;

            //No quantities yet, add 'em
            if (0 == Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblCumDemandValues.Length);
            }
            else if (adblCumDemandValues.Length > Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblCumDemandValues.Length - Qtys.Quantities.Count);
            }

            for (int i = 0; i < adblCumDemandValues.Length; i++)
            {
                Qty = Qtys.Quantities[i];

                switch (eType)
                {
                    case RegisterType.TOTAL:
                        {
                            if (null == Qty.CummulativeDemand)
                            {
                                Qty.CummulativeDemand = new Measurement();
                            }
                            Qty.CummulativeDemand.Value = adblCumDemandValues[i];

                            break;
                        }
                    case RegisterType.RATEA:
                        {
                            SetCumDemandTOUValue(adblCumDemandValues[i], Qty, 0);

                            break;
                        }
                    case RegisterType.RATEB:
                        {
                            SetCumDemandTOUValue(adblCumDemandValues[i], Qty, 1);

                            break;
                        }
                    case RegisterType.RATEC:
                        {
                            SetCumDemandTOUValue(adblCumDemandValues[i], Qty, 2);

                            break;
                        }
                    case RegisterType.RATED:
                        {
                            SetCumDemandTOUValue(adblCumDemandValues[i], Qty, 3);

                            break;
                        }
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// This method sets demand values in a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <param name="dblValue">The value to set.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  08/20/09 jrf 2.21.03 139112 Modified to not remove quantities when adding new ones.
        //
        private void SetDemandValues(QuantityCollection Qtys, RegisterType eType, double[] dblValue)
        {
            double[] adblDemandValues = dblValue;
            Quantity Qty = null;

            //No quantities yet, add 'em
            if (0 == Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblDemandValues.Length);
            }
            else if (adblDemandValues.Length > Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblDemandValues.Length - Qtys.Quantities.Count);
            }
            
            for (int i = 0; i < adblDemandValues.Length; i++)
            {
                Qty = Qtys.Quantities[i];

                switch (eType)
                {
                    case RegisterType.TOTAL:
                        {
                            if (null == Qty.TotalMaxDemand)
                            {
                                Qty.TotalMaxDemand = new DemandMeasurement();
                            }
                            Qty.TotalMaxDemand.Value = adblDemandValues[i];

                            break;
                        }
                    case RegisterType.RATEA:
                        {
                            SetDemandTOUValue(adblDemandValues[i], Qty, 0);

                            break;
                        }
                    case RegisterType.RATEB:
                        {
                            SetDemandTOUValue(adblDemandValues[i], Qty, 1);

                            break;
                        }
                    case RegisterType.RATEC:
                        {
                            SetDemandTOUValue(adblDemandValues[i], Qty, 2);

                            break;
                        }
                    case RegisterType.RATED:
                        {
                            SetDemandTOUValue(adblDemandValues[i], Qty, 3);

                            break;
                        }
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// This method sets energy values in a quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="eType">The type of register value.</param>
        /// <param name="dblValue">The value to set.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //  08/20/09 jrf 2.21.03 139112 Modified to not remove quantities when adding new ones.
        //
        private void SetEnergyValues(QuantityCollection Qtys, RegisterType eType, double[] dblValue)
        {
            double[] adblEnergyValues = dblValue;
            Quantity Qty = null;

            //No quantities yet, add 'em
            if (0 == Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblEnergyValues.Length);
            }
            else if (adblEnergyValues.Length > Qtys.Quantities.Count)
            {
                AddQuantities(Qtys, adblEnergyValues.Length - Qtys.Quantities.Count);
            }

            for (int i = 0; i < adblEnergyValues.Length; i++)
            {
                Qty = Qtys.Quantities[i];

                switch (eType)
                {
                    case RegisterType.TOTAL:
                        {
                            if (null == Qty.TotalEnergy)
                            {
                                Qty.TotalEnergy = new Measurement();
                            }
                            Qty.TotalEnergy.Value = adblEnergyValues[i];

                            break;
                        }
                    case RegisterType.RATEA:
                        {
                            SetEnergyTOUValue(adblEnergyValues[i], Qty, 0);

                            break;
                        }
                    case RegisterType.RATEB:
                        {
                            SetEnergyTOUValue(adblEnergyValues[i], Qty, 1);

                            break;
                        }
                    case RegisterType.RATEC:
                        {
                            SetEnergyTOUValue(adblEnergyValues[i], Qty, 2);

                            break;
                        }
                    case RegisterType.RATED:
                        {
                            SetEnergyTOUValue(adblEnergyValues[i], Qty, 3);

                            break;
                        }
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// This method adds the specified number of quanities to the given 
        /// quantity collection.
        /// </summary>
        /// <param name="Qtys">The quantity collection.</param>
        /// <param name="iNumQuantites">The specified number of quantities to add.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //
        private void AddQuantities(QuantityCollection Qtys, int iNumQuantites)
        {
            Quantity Qty = null; 

            for (int i = 0; i < iNumQuantites; i++)
            {
                Qty = new Quantity("");
                Qtys.Quantities.Add(Qty);
            }
        }

        /// <summary>
        /// This method sets a TOU demand time of occurence value for a quantity at a specified rate.
        /// </summary>
        /// <param name="dtValue">The TOU demand time of occurence value.</param>
        /// <param name="Qty">The quantity that is being set.</param>
        /// <param name="iRate">The specified TOU rate.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //
        private void SetDemandTOUTOOValue(DateTime dtValue, Quantity Qty, int iRate)
        {
            if (null == Qty.TOUMaxDemand)
            {
                Qty.TOUMaxDemand = new System.Collections.Generic.List<DemandMeasurement>();
            }

            while (iRate >= Qty.TOUMaxDemand.Count)
            {
                Qty.TOUMaxDemand.Add(new DemandMeasurement());
            }

            Qty.TOUMaxDemand[iRate].TimeOfOccurrence = dtValue;
        }

        /// <summary>
        /// This method sets a TOU cumulative demand value for a quantity at a specified rate.
        /// </summary>
        /// <param name="dblValue">The TOU cumulative demand value.</param>
        /// <param name="Qty">The quantity that is being set.</param>
        /// <param name="iRate">The specified TOU rate.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //
        private void SetCumDemandTOUValue(double dblValue, Quantity Qty, int iRate)
        {
            if (null == Qty.TOUCummulativeDemand)
            {
                Qty.TOUCummulativeDemand = new System.Collections.Generic.List<Measurement>();
            }

            while (iRate >= Qty.TOUCummulativeDemand.Count)
            {
                Qty.TOUCummulativeDemand.Add(new Measurement());
            }

            Qty.TOUCummulativeDemand[iRate].Value = dblValue;
        }

        /// <summary>
        /// This method sets a TOU demand value for a quantity at a specified rate.
        /// </summary>
        /// <param name="dblValue">The TOU demand value.</param>
        /// <param name="Qty">The quantity that is being set.</param>
        /// <param name="iRate">The specified TOU rate.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //
        private void SetDemandTOUValue(double dblValue, Quantity Qty, int iRate)
        {
            if (null == Qty.TOUMaxDemand)
            {
                Qty.TOUMaxDemand = new System.Collections.Generic.List<DemandMeasurement>();
            }

            while (iRate >= Qty.TOUMaxDemand.Count)
            {
                Qty.TOUMaxDemand.Add(new DemandMeasurement());
            }

            Qty.TOUMaxDemand[iRate].Value = dblValue;
        }

        /// <summary>
        /// This method sets a TOU energy value for a quantity at a specified rate.
        /// </summary>
        /// <param name="dblValue">The TOU energy value.</param>
        /// <param name="Qty">The quantity that is being set.</param>
        /// <param name="iRate">The specified TOU rate.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/06/08 jrf 1.00.00        Created
        //
        private void SetEnergyTOUValue(double dblValue, Quantity Qty, int iRate)
        {
            if (null == Qty.TOUEnergy)
            {
                Qty.TOUEnergy = new System.Collections.Generic.List<Measurement>();
            }

            while (iRate >= Qty.TOUEnergy.Count)
            {
                Qty.TOUEnergy.Add(new Measurement());
            }
            
            Qty.TOUEnergy[iRate].Value = dblValue;
        }


        #endregion

        #region Members

        private bool m_blnClockEnabled;
        private bool m_blnDemThresholdExceeded;
        private bool m_blnDSTConfigured;
        private bool[] m_ablnNonFatalErrors;
        private bool[] m_ablnFatalErrors;
        private bool m_blnLastBPDemThresholdExceeded;
        private bool[] m_ablnLastBPNonFatalErrors;
        private bool[] m_ablnLastBPFatalErrors;
        private bool m_blnLastSRDemThresholdExceeded;
        private bool[] m_ablnLastSRNonFatalErrors;
        private bool[] m_ablnLastSRFatalErrors;
        private bool m_blnPFAvgAvailable;
        private bool[] m_ablnReads;
        private bool m_blnSelfReadData;
        private DateTime m_dtCurrentTime;
        private DateTime m_dtLastBPTime;
        private DateTime m_dtLastSRTime;
        private DateTime m_dtTimeLastOutage;
        private DateTime m_dtTimeLastInterrogation;
        private double m_dblWhDelivered;
        private float m_fltCTMultiplier;
        private float m_fltFWVersionRevision;
        private float m_fltRegisterMultiplier;
        private float m_fltVTMultiplier;
        private int m_CurrentBatteryReading;
        private int m_DaysOnBattery;
        private int m_iDaysSinceLastDR;
        private int m_iDaysSinceLastTest;
        private int m_iDemandIntervalLength;
        private int m_iDemandResetCount;
        private int m_iEPFCount;
        private int m_iGoodBatteryReading;
        private int m_iLastBPDemandResetCount;
        private int m_iLastBPEPFCount;
        private int m_iLastBPNumTimesProgrammed;
        private int m_iLastBPOutageCount;
        private int m_iLastSRDemandResetCount;
        private int m_iLastSREPFCount;
        private int m_iLastSRNumTimesProgrammed;
        private int m_iLastSROutageCount;
        private int m_iNumCumDemands;
        private int m_iNumDemands;
        private int m_iNumEnergies;
        private int m_iNumLPChannels;
        private int m_iNumTimesProgrammed;
        private int m_iNumTOURates;
        private int m_iOutageCount;
        private int m_iProgramID;
        private int m_iRateID;
        private uint[] m_auiCumDemandLIDs;
        private uint[] m_auiDemandLIDs;
        private uint[] m_auiDemandTOOLIDs;
        private uint[] m_auiEnergyLIDs;
        private QuantityCollection m_CurrentEnergyQuantities;
        private QuantityCollection m_CurrentDemandQuantities;
        private QuantityCollection m_CurrentCumDemandQuantities;
        private QuantityCollection m_LastBPEnergyQuantities;
        private QuantityCollection m_LastBPDemandQuantities;
        private QuantityCollection m_LastBPCumDemandQuantities;
        private QuantityCollection m_LastSREnergyQuantities;
        private QuantityCollection m_LastSRDemandQuantities;
        private QuantityCollection m_LastSRCumDemandQuantities;

        #endregion

    }


    /// <summary>
    /// This class represents the TOU register data in a Centron OpenWay HHF file.
    /// </summary>
    public class CentronOpenWayMV90RegisterData : ANSIMV90RegisterData
    {

        #region Constants

        private const string CENTRONAMI_NAME = "OpenWay CENTRON";
        private const string LID = "LID";
        private const string WH_DEL = "Wh Delivered";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/05/08 jrf	1.00.00	   	   Created
		public CentronOpenWayMV90RegisterData(DateTime dtReadTime, string strDataFormat)
            : base(dtReadTime, strDataFormat)
        {
        }

        /// <summary>
        /// This methods retrieves all available records with configuration data.
        /// </summary>
        /// <returns>A list of records.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public override List<MV90RegisterRecord> RetrieveConfigurationRecords()
        {
            List<MV90RegisterRecord> lstRecords = new List<MV90RegisterRecord>();
            MV90RegisterRecord Record = null;

            if (Read0Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord0(ref Record);
                lstRecords.Add(Record);
            }

            if (Read1Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord1(ref Record);
                lstRecords.Add(Record);
            }

            if (Read2Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord2(ref Record);
                lstRecords.Add(Record);
            }

            if (Read3Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord3(ref Record);
                lstRecords.Add(Record);
            }

            if (Read4Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord4(ref Record);
                lstRecords.Add(Record);
            }

            if (Read5Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord5(ref Record);
                lstRecords.Add(Record);
            }

            if (Read10Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord10(ref Record);
                lstRecords.Add(Record);
            }

            if (Read15Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord15(ref Record);
                lstRecords.Add(Record);
            }

            return lstRecords;
        }

        /// <summary>
        /// This methods retrieves all available records.
        /// </summary>
        /// <returns>A list of records.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        // 05/20/08 jrf 1.50.27 114449 Added new record 20.
        public override List<MV90RegisterRecord> RetrieveRecords()
        {
            List<MV90RegisterRecord> lstRecords = new List<MV90RegisterRecord>();
            MV90RegisterRecord Record = null;

            if (Read0Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord0(ref Record);
                lstRecords.Add(Record);
            }

            if (Read1Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord1(ref Record);
                lstRecords.Add(Record);
            }

            if (Read2Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord2(ref Record);
                lstRecords.Add(Record);
            }

            if (Read3Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord3(ref Record);
                lstRecords.Add(Record);
            }

            if (Read4Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord4(ref Record);
                lstRecords.Add(Record);
            }

            if (Read5Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord5(ref Record);
                lstRecords.Add(Record);
            }

            if (Read6Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord6(ref Record);
                lstRecords.Add(Record);
            }

            if (Read7Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord7(ref Record);
                lstRecords.Add(Record);
            }

            if (Read8Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord8(ref Record);
                lstRecords.Add(Record);
            }

            if (Read9Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord9(ref Record);
                lstRecords.Add(Record);
            }

            if (Read10Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord10(ref Record);
                lstRecords.Add(Record);
            }

            if (Read11Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord11(ref Record);
                lstRecords.Add(Record);
            }

            if (Read12Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord12(ref Record);
                lstRecords.Add(Record);
            }

            if (Read13Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord13(ref Record);
                lstRecords.Add(Record);
            }

            if (Read14Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord14(ref Record);
                lstRecords.Add(Record);
            }

            if (Read15Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord15(ref Record);
                lstRecords.Add(Record);
            }

            if (Read16Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord16(ref Record);
                lstRecords.Add(Record);
            }

            if (Read17Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord17(ref Record);
                lstRecords.Add(Record);
            }

            if (Read18Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord18(ref Record);
                lstRecords.Add(Record);
            }

            if (Read19Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord19(ref Record);
                lstRecords.Add(Record);
            }

            if (Read20Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord20(ref Record);
                lstRecords.Add(Record);
            }

            return lstRecords;
        }

        /// <summary>
        /// This methods retrieves all available records with register data.
        /// </summary>
        /// <returns>A list of records.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public override List<MV90RegisterRecord> RetrieveRegisterRecords()
        {
            List<MV90RegisterRecord> lstRecords = new List<MV90RegisterRecord>();
            MV90RegisterRecord Record = null;

            //Current register data
            if (Read6Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord6(ref Record);
                lstRecords.Add(Record);
            }

            if (Read7Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord7(ref Record);
                lstRecords.Add(Record);
            }

            if (Read8Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord8(ref Record);
                lstRecords.Add(Record);
            }

            if (Read9Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord9(ref Record);
                lstRecords.Add(Record);
            }

            //Last billing period register data
            if (Read11Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord11(ref Record);
                lstRecords.Add(Record);
            }

            if (Read12Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord12(ref Record);
                lstRecords.Add(Record);
            }

            if (Read13Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord13(ref Record);
                lstRecords.Add(Record);
            }

            if (Read14Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord14(ref Record);
                lstRecords.Add(Record);
            }

            //Last Self Read register data
            if (Read16Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord16(ref Record);
                lstRecords.Add(Record);
            }

            if (Read17Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord17(ref Record);
                lstRecords.Add(Record);
            }

            if (Read18Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord18(ref Record);
                lstRecords.Add(Record);
            }

            if (Read19Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord19(ref Record);
                lstRecords.Add(Record);
            }

            return lstRecords;
        }

        /// <summary>
        /// This methods retrieves all available records with HAN client data.
        /// </summary>
        /// <returns>A list of records.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.50.27 114449 Created
        public List<MV90RegisterRecord> RetrieveHANClientDataRecords()
        {
            List<MV90RegisterRecord> lstRecords = new List<MV90RegisterRecord>();
            MV90RegisterRecord Record = null;

            //HAN client data
            if (Read20Present)
            {
                Record = new MV90RegisterRecord();
                GetRecord20(ref Record);
                lstRecords.Add(Record);
            }

            return lstRecords;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a string that represents the type of meter that the register data
        /// comes from.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/20/08 jrf	1.00.00	   	   Created
        override public string MeterType
        {
            get
            {
                return CENTRONAMI_NAME;
            }
        }

        /// <summary>
        /// Gets/sets a byte that represents the number of HAN clients that can be 
        /// supported.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 jrf	1.50.27 114449 Created
        public byte SupportedNumClients
        {
            get
            {
                return m_bySupportedNumClients;
            }

            set
            {
                m_bySupportedNumClients = value;
            }
        }

        /// <summary>
        /// Gets/sets a Int16 that represents the actual size each HAN client data 
        /// record is capable of supporting.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 jrf	1.50.27 114449 Created
        public UInt16 SupportedClientDataSize
        {
            get
            {
                return m_uiSupportedClientDataSize;
            }

            set
            {
                m_uiSupportedClientDataSize = value;
            }
        }

        /// <summary>
        /// Gets/sets a list of client meters.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 jrf	1.50.27 114449 Created
        public List<ClientMeter> ClientMeters
        {
            get
            {
                return m_lstClientMeters;
            }

            set
            {
                m_lstClientMeters = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method gets TOU register data record 20, the HAN client data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 jrf	1.50.27 114449 Created
        private void GetRecord20(ref MV90RegisterRecord Record)
        {
            //throw new NotImplementedException();
            Record.NumericalID = 20;
            Record.Name = Resources.HAN_CLIENT_DATA;
            int iReadingCount = 0;

            Record.Items.Add(new MV90RegisterItem(Resources.SUPPORTED_NUM_CLIENTS, 
                SupportedNumClients.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.SUPPORTED_CLIENT_DATA_SIZE, 
                SupportedClientDataSize.ToString(CultureInfo.CurrentCulture)));
            
            
            foreach (ClientMeter client in ClientMeters)
            {
                iReadingCount++;

                //Extract items for each client
                Record.Items.Add(new MV90RegisterItem(Resources.CLIENT_METER + " " 
                    + iReadingCount.ToString(CultureInfo.CurrentCulture), ""));
                Record.Items.Add(new MV90RegisterItem(Resources.METER_TYPE, client.NodeType));
                Record.Items.Add(new MV90RegisterItem(Resources.METER_ADDRESS,
                client.MACAddress.ToString("X", CultureInfo.InvariantCulture)));

                if (0 != client.CustomerID)
                {
                    Record.Items.Add(new MV90RegisterItem(Resources.CUSTOMER_ID, client.CustomerID.ToString(CultureInfo.InvariantCulture)));
                }

                Record.Items.Add(new MV90RegisterItem(Resources.READING_TIME, client.TimeRecorded.ToString(CultureInfo.CurrentCulture)));

                if (null != client.LatestConsumption)
                {
                    Record.Items.Add(new MV90RegisterItem(Resources.LATEST_READING, 
                        client.LatestConsumption.Value.ToString(CultureInfo.CurrentCulture), 
                        client.LatestConsumption.Timestamp.ToString(CultureInfo.CurrentCulture)));
                }

                if (null != client.DFTConsumption)
                {
                    Record.Items.Add(new MV90RegisterItem(Resources.DAILY_FREEZE_READING,
                        client.DFTConsumption.Value.ToString(CultureInfo.CurrentCulture),
                        client.DFTConsumption.Timestamp.ToString(CultureInfo.CurrentCulture)));
                }

                int nCmdCnt = 0;

                //Extract items for each command response
                foreach (ClientCommandResponse response in client.CommandResponses)
                {
                    nCmdCnt++;

                    Record.Items.Add(new MV90RegisterItem(Resources.CMD_RESPONSE + " " + nCmdCnt.ToString(CultureInfo.CurrentCulture), ""));
                    Record.Items.Add(new MV90RegisterItem(Resources.CMD, client.CommandResponses[nCmdCnt - 1].CommandName));
                    Record.Items.Add(new MV90RegisterItem(Resources.RESPONSE, client.CommandResponses[nCmdCnt - 1].Data));
                    Record.Items.Add(new MV90RegisterItem(Resources.SUCCESS, client.CommandResponses[nCmdCnt - 1].Success.ToString(CultureInfo.CurrentCulture)));
                }

            }

        }
        
        /// <summary>
        /// This method gets TOU register data record 19, the last self read demand
        /// TOO data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord19(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumDemands];

            Record.NumericalID = 19;
            Record.Name = Resources.LAST_SR_DEM_TOO_DATA;

            for (int i = 0; i < LastSRTotalDemandTOOs.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandTOOLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastSRTotalDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateADemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastSRRateADemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateBDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastSRRateBDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateCDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastSRRateCDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateDDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastSRRateDDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 18, last self read cumulative
        /// demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord18(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumCumDemands];

            Record.NumericalID = 18;
            Record.Name = Resources.LAST_SR_CUM_DEM_DATA;

            for (int i = 0; i < LastSRTotalCumDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(CumDemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastSRTotalCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateACumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastSRRateACumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateBCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastSRRateBCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateCCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastSRRateCCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateDCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastSRRateDCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 17, last self read demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        // 08/20/09 jrf 2.21.03 139112 Making size of RecordedLIDs conditional on
        //                             whether avg. PF is available.
        private void GetRecord17(ref MV90RegisterRecord Record)
        {
            int intNumDemands = NumDemands;
            if (true == PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }

            CentronAMILID[] RecordedLIDs = new CentronAMILID[intNumDemands];

            Record.NumericalID = 17;
            Record.Name = Resources.LAST_SR_DEM_DATA;

            for (int i = 0; i < LastSRTotalDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastSRTotalDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateADemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastSRRateADemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateBDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastSRRateBDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateCDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastSRRateCDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateDDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastSRRateDDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 16, last self read energy
        /// data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord16(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumEnergies];

            Record.NumericalID = 16;
            Record.Name = Resources.LAST_SR_ENERGY_DATA;

            for (int i = 0; i < LastSRTotalEnergies.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(EnergyLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastSRTotalEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateAEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastSRRateAEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateBEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastSRRateBEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateCEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastSRRateCEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastSRRateDEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastSRRateDEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 15, last self read state data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord15(ref MV90RegisterRecord Record)
        {
            bool blnNonFatalErrors = false;
            bool blnFatalErrors = false;
            Record.NumericalID = 15;
            Record.Name = Resources.LAST_SR_STATE_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.DEM_RESET_COUNT, LastSRDemandResetCount.ToString(CultureInfo.CurrentCulture)));

            if (LastSRNonFatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.LOW_BATT));
                blnNonFatalErrors = true;
            }

            if (LastSRNonFatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.PHASE_LOSS));
                blnNonFatalErrors = true;
            }

            if (LastSRNonFatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.TOU_SCHED));
                blnNonFatalErrors = true;
            }

            if (LastSRNonFatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REV_POWER_FLOW));
                blnNonFatalErrors = true;
            }

            if (LastSRNonFatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.MASS_MEMORY));
                blnNonFatalErrors = true;
            }

            if (LastSRNonFatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REG_FULL_SCALE_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (LastSRDemandThresholdExceeded)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.DEM_THRESHOLD_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (!blnNonFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERRS, Resources.NONE));
            }

            if (LastSRFatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.MCU_FLASH));
                blnFatalErrors = true;
            }

            if (LastSRFatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.RAM));
                blnFatalErrors = true;
            }

            if (LastSRFatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.DATA_FLASH));
                blnFatalErrors = true;
            }

            if (LastSRFatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.METROLOGY_COMMS));
                blnFatalErrors = true;
            }

            if (LastSRFatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.EPF_DATA));
                blnFatalErrors = true;
            }

            if (LastSRFatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.FILE_SYS));
                blnFatalErrors = true;
            }

            if (LastSRFatalError7)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.OP_SYS));
                blnFatalErrors = true;
            }

            if (!blnFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERRS, Resources.NONE));
            }

            Record.Items.Add(new MV90RegisterItem(Resources.OUTAGE_COUNT, LastSROutageCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.PROGRAM_COUNT, LastSRNumTimesProgrammed.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.EPF_COUNT, LastSREPFCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.LAST_SR_TIME, LastSRTime.ToString(CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// This method gets TOU register data record 14, last billing period demand 
        /// TOO data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord14(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumDemands];

            Record.NumericalID = 14;
            Record.Name = Resources.LAST_BP_DEM_TOO_DATA;

            for (int i = 0; i < LastBPTotalDemandTOOs.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandTOOLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastBPTotalDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateADemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastBPRateADemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateBDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastBPRateBDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateCDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastBPRateCDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateDDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastBPRateDDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 13, last billing period 
        /// cumulative demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord13(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumCumDemands];

            Record.NumericalID = 13;
            Record.Name = Resources.LAST_SR_CUM_DEM_DATA;

            for (int i = 0; i < LastBPTotalCumDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(CumDemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastBPTotalCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateACumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastBPRateACumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateBCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastBPRateBCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateCCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastBPRateCCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateDCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastBPRateDCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 12, last billing period 
        /// demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        // 08/20/09 jrf 2.21.03 139112 Making size of RecordedLIDs conditional on
        //                             whether avg. PF is available.
        private void GetRecord12(ref MV90RegisterRecord Record)
        {
            int intNumDemands = NumDemands;
            if (true == PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }

            CentronAMILID[] RecordedLIDs = new CentronAMILID[intNumDemands];

            Record.NumericalID = 12;
            Record.Name = Resources.LAST_BP_DEM_DATA;

            for (int i = 0; i < LastBPTotalDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastBPTotalDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateADemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastBPRateADemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateBDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastBPRateBDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateCDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastBPRateCDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateDDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastBPRateDDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 11, last billing period 
        /// energy data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord11(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumEnergies];

            Record.NumericalID = 11;
            Record.Name = Resources.LAST_BP_ENERGY_DATA;

            for (int i = 0; i < LastBPTotalEnergies.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(EnergyLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    LastBPTotalEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateAEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    LastBPRateAEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateBEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    LastBPRateBEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateCEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    LastBPRateCEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < LastBPRateDEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    LastBPRateDEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 10, last billing period 
        /// state data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord10(ref MV90RegisterRecord Record)
        {
            bool blnNonFatalErrors = false;
            bool blnFatalErrors = false;
            Record.NumericalID = 10;
            Record.Name = Resources.LAST_BP_STATE_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.DEM_RESET_COUNT, LastBPDemandResetCount.ToString(CultureInfo.CurrentCulture)));

            if (LastBPNonFatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.LOW_BATT));
                blnNonFatalErrors = true;
            }

            if (LastBPNonFatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.PHASE_LOSS));
                blnNonFatalErrors = true;
            }

            if (LastBPNonFatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.TOU_SCHED));
                blnNonFatalErrors = true;
            }

            if (LastBPNonFatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REV_POWER_FLOW));
                blnNonFatalErrors = true;
            }

            if (LastBPNonFatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.MASS_MEMORY));
                blnNonFatalErrors = true;
            }

            if (LastBPNonFatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REG_FULL_SCALE_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (LastBPDemandThresholdExceeded)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.DEM_THRESHOLD_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (!blnNonFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERRS, Resources.NONE));
            }

            if (LastBPFatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.MCU_FLASH));
                blnFatalErrors = true;
            }

            if (LastBPFatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.RAM));
                blnFatalErrors = true;
            }

            if (LastBPFatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.DATA_FLASH));
                blnFatalErrors = true;
            }

            if (LastBPFatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.METROLOGY_COMMS));
                blnFatalErrors = true;
            }

            if (LastBPFatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.EPF_DATA));
                blnFatalErrors = true;
            }

            if (LastBPFatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.FILE_SYS));
                blnFatalErrors = true;
            }

            if (LastBPFatalError7)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.OP_SYS));
                blnFatalErrors = true;
            }

            if (!blnFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERRS, Resources.NONE));
            }

            Record.Items.Add(new MV90RegisterItem(Resources.OUTAGE_COUNT, LastBPOutageCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.PROGRAM_COUNT, LastBPNumTimesProgrammed.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.EPF_COUNT, LastBPEPFCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.LAST_BP_TIME, LastBPTime.ToString(CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// This method gets TOU register data record 9, current demand TOO data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord9(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumDemands];

            Record.NumericalID = 9;
            Record.Name = Resources.CURRENT_DEM_TOO_DATA;

            for (int i = 0; i < TotalDemandTOOs.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandTOOLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    TotalDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateADemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    RateADemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateBDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    RateBDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateCDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    RateCDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateDDemandTOOs.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    RateDDemandTOOs[i].ToString(CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 8, current cumulative 
        /// demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord8(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumCumDemands];

            Record.NumericalID = 8;
            Record.Name = Resources.CURRENT_CUM_DEM_DATA;

            for (int i = 0; i < TotalCumDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(CumDemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    TotalCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateACumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    RateACumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateBCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    RateBCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateCCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    RateCCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateDCumDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    RateDCumDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 7, current demand data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        // 08/20/09 jrf 2.21.03 139112 Making size of RecordedLIDs conditional on
        //                             whether avg. PF is available.
        private void GetRecord7(ref MV90RegisterRecord Record)
        {
            int intNumDemands = NumDemands;
            if (true == PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }
            CentronAMILID[] RecordedLIDs = new CentronAMILID[intNumDemands];

            Record.NumericalID = 7;
            Record.Name = Resources.CURRENT_DEM_DATA;

            for (int i = 0; i < TotalDemands.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(DemandLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    TotalDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateADemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    RateADemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateBDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    RateBDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateCDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    RateCDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateDDemands.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    RateDDemands[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 6, current energy data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord6(ref MV90RegisterRecord Record)
        {
            CentronAMILID[] RecordedLIDs = new CentronAMILID[NumEnergies];

            Record.NumericalID = 6;
            Record.Name = Resources.CURRENT_ENERGY_DATA;

            for (int i = 0; i < TotalEnergies.Length; i++)
            {
                RecordedLIDs[i] = new CentronAMILID(EnergyLIDs[i]);
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription,
                    TotalEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateAEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEA,
                    RateAEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateBEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEB,
                    RateBEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateCEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATEC,
                    RateCEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }

            for (int i = 0; i < RateDEnergies.Length; i++)
            {
                Record.Items.Add(new MV90RegisterItem(RecordedLIDs[i].lidDescription + " " + Resources.RATED,
                    RateDEnergies[i].ToString("F3", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 5.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord5(ref MV90RegisterRecord Record)
        {
            bool blnNonFatalErrors = false;
            bool blnFatalErrors = false;
            Record.NumericalID = 5;
            Record.Name = Resources.CURRENT_STATE_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.DEM_RESET_COUNT, DemandResetCount.ToString(CultureInfo.CurrentCulture)));

            if (NonFatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.LOW_BATT));
                blnNonFatalErrors = true;
            }

            if (NonFatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.PHASE_LOSS));
                blnNonFatalErrors = true;
            }

            if (NonFatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.TOU_SCHED));
                blnNonFatalErrors = true;
            }

            if (NonFatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REV_POWER_FLOW));
                blnNonFatalErrors = true;
            }

            if (NonFatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.MASS_MEMORY));
                blnNonFatalErrors = true;
            }

            if (NonFatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.REG_FULL_SCALE_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (DemandThresholdExceeded)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERR, Resources.DEM_THRESHOLD_EXCEEDED));
                blnNonFatalErrors = true;
            }

            if (!blnNonFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.NON_FATAL_ERRS, Resources.NONE));
            }

            if (FatalError1)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.MCU_FLASH));
                blnFatalErrors = true;
            }

            if (FatalError2)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.RAM));
                blnFatalErrors = true;
            }

            if (FatalError3)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.DATA_FLASH));
                blnFatalErrors = true;
            }

            if (FatalError4)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.METROLOGY_COMMS));
                blnFatalErrors = true;
            }

            if (FatalError5)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.EPF_DATA));
                blnFatalErrors = true;
            }

            if (FatalError6)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.FILE_SYS));
                blnFatalErrors = true;
            }

            if (FatalError7)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERR, Resources.OP_SYS));
                blnFatalErrors = true;
            }

            if (!blnFatalErrors)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.FATAL_ERRS, Resources.NONE));
            }

            Record.Items.Add(new MV90RegisterItem(Resources.OUTAGE_COUNT, OutageCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.PROGRAM_COUNT, NumTimesProgrammed.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.EPF_COUNT, EPFCount.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.CURRENT_TIME, CurrentTime.ToString(CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// This method gets TOU register data record 4, quantity identification.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord4(ref MV90RegisterRecord Record)
        {
            LID RecordedLID = null;
            Record.NumericalID = 4;
            Record.Name = Resources.QUANT_IDENTIFICATION;

            foreach (uint uiEnergyLID in EnergyLIDs)
            {
                RecordedLID = new CentronAMILID(uiEnergyLID);
                Record.Items.Add(new MV90RegisterItem(RecordedLID.lidDescription + " " + LID,
                    uiEnergyLID.ToString("X", CultureInfo.CurrentCulture)));
            }

            foreach (uint uiDemandLID in DemandLIDs)
            {
                RecordedLID = new CentronAMILID(uiDemandLID);
                Record.Items.Add(new MV90RegisterItem(RecordedLID.lidDescription + " " + LID,
                    uiDemandLID.ToString("X", CultureInfo.CurrentCulture)));
            }

            foreach (uint uiCumDemandLID in CumDemandLIDs)
            {
                RecordedLID = new CentronAMILID(uiCumDemandLID);
                Record.Items.Add(new MV90RegisterItem(RecordedLID.lidDescription + " " + LID,
                    uiCumDemandLID.ToString("X", CultureInfo.CurrentCulture)));
            }

            foreach (uint uiDemandTOOLID in DemandTOOLIDs)
            {
                RecordedLID = new CentronAMILID(uiDemandTOOLID);
                Record.Items.Add(new MV90RegisterItem(RecordedLID.lidDescription + " " + LID,
                    uiDemandTOOLID.ToString("X", CultureInfo.CurrentCulture)));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 3.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord3(ref MV90RegisterRecord Record)
        {
            Record.NumericalID = 3;
            Record.Name = Resources.CLOCK_RELATED_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.DAYS_SINCE_LAST_DR, DaysSinceLastDR.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.DAYS_SINCE_LAST_TEST, DaysSinceLastTest.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.TIME_LAST_OUTAGE, TimeLastOutage.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.TIME_LAST_INTERROGATION, TimeLastInterrogation.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.DAYS_ON_BATT, DaysOnBattery.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.CURRENT_BATT_READ, CurrentBatteryReading.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.GOOD_BATT_READ, GoodBatteryReading.ToString(CultureInfo.CurrentCulture)));

            if (DSTConfigured)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.DST_ENABLED, Resources.YES));
            }
            else
            {
                Record.Items.Add(new MV90RegisterItem(Resources.DST_ENABLED, Resources.NO));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 2, capabilities data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord2(ref MV90RegisterRecord Record)
        {
            Record.NumericalID = 2;
            Record.Name = Resources.CAPABILITIES_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.TOU_RATES, NumTOURates.ToString(CultureInfo.CurrentCulture)));

            if (ClockEnabled)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.CLOCK_ENABLED, Resources.YES));
            }
            else
            {
                Record.Items.Add(new MV90RegisterItem(Resources.CLOCK_ENABLED, Resources.NO));
            }

            if (SelfReadDataAvailable)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.SELF_READ_AVAILABLE, Resources.YES));
            }
            else
            {
                Record.Items.Add(new MV90RegisterItem(Resources.SELF_READ_AVAILABLE, Resources.NO));
            }

            Record.Items.Add(new MV90RegisterItem(Resources.NUM_OF_LP_CHANNELS, NumLPChannels.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.NUM_OF_ENERGIES, NumEnergies.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.NUM_OF_DEMANDS, NumDemands.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.NUM_OF_CUM_DEMANDS, NumCumDemands.ToString(CultureInfo.CurrentCulture)));

            if (PFAverageAvailable)
            {
                Record.Items.Add(new MV90RegisterItem(Resources.PF_AVG_BILL_PERIOD_AVAILABLE, Resources.YES));
            }
            else
            {
                Record.Items.Add(new MV90RegisterItem(Resources.PF_AVG_BILL_PERIOD_AVAILABLE, Resources.NO));
            }
        }

        /// <summary>
        /// This method gets TOU register data record 1, constants data.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString methods
        private void GetRecord1(ref MV90RegisterRecord Record)
        {
            Record.NumericalID = 1;
            Record.Name = Resources.CONSTANTS_DATA;

            Record.Items.Add(new MV90RegisterItem(Resources.CT_MULT, CTMultiplier.ToString("F2", CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.VT_MULT, VTMultiplier.ToString("F2", CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.REG_MULT, RegisterMultiplier.ToString("F2", CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.PROG_ID, ProgramID.ToString(CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.FW_VERSION, FWVersionRevision.ToString("F3", CultureInfo.CurrentCulture)));
            Record.Items.Add(new MV90RegisterItem(Resources.DEM_INT_LEN, DemandIntervalLength.ToString(CultureInfo.CurrentCulture) + " " + Resources.MINUTES));
            Record.Items.Add(new MV90RegisterItem(Resources.RATE_ID, RateID.ToString(CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// This method gets TOU register data record 0.
        /// </summary>
        /// <param name="Record">The retrieved record.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	10.00.00	   Created
        // 02/26/08 jrf 10.00.00       Added culture info to the ToString method
        private void GetRecord0(ref MV90RegisterRecord Record)
        {
            Record.NumericalID = 0;
            Record.Name = Resources.CURRENT_SEASON_TOTAL + " " + WH_DEL;

            Record.Items.Add(new MV90RegisterItem(WH_DEL, WhDelivered.ToString("F3", CultureInfo.CurrentCulture)));
        }

        #endregion

        #region Members

        private byte m_bySupportedNumClients = 0;
        private UInt16 m_uiSupportedClientDataSize = 0;
        private List<ClientMeter> m_lstClientMeters;

        #endregion

    }

    /// <summary>
    /// This class represents a record in the TOU register data.   
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 02/05/08 jrf 1.00.00        Created
    public class MV90RegisterRecord
    {

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public MV90RegisterRecord()
        {
            m_iNumericalID = 0;
            m_strName = "";
            m_lstItems = new List<MV90RegisterItem>();
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets an int that represents the numeric ID for the record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public int NumericalID
        {
            get
            {
                return m_iNumericalID;
            }

            set
            {
                m_iNumericalID = value;
            }
        }

        /// <summary>
        /// Gets/sets a string that represents the name the record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public string Name
        {
            get
            {
                return m_strName;
            }

            set
            {
                m_strName = value;
            }
        }

        /// <summary>
        /// Gets/sets a string array that represents the items in the record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/06/08 jrf	1.00.00	   	   Created
        public List<MV90RegisterItem> Items
        {
            get
            {
                return m_lstItems;
            }

            //set
            //{
            //    m_lstItems = value;
            //}
        }

        #endregion

        #region Members

        /// <summary>
        /// A numerical value that identifies the record.
        /// </summary>
        private int m_iNumericalID;

        /// <summary>
        /// The name of the record.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// The items in the record
        /// </summary>
        private List<MV90RegisterItem> m_lstItems;

        #endregion
    }

    /// <summary>
    /// This class represents one TOU register data item.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  02/05/08 jrf 1.00.00        Created
    //
    public class MV90RegisterItem
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strDescription">The description of the register item.</param>
        /// <param name="strValue">The value of the register item.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public MV90RegisterItem(string strDescription, string strValue)
        {
            m_strDescription = strDescription;
            m_strValue = strValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strDescription">The description of the register item.</param>
        /// <param name="strValue">The value of the register item.</param>
        /// <param name="strExtraData">Any extra data associated with the re</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 jrf	1.50.27 114449 Created
        public MV90RegisterItem(string strDescription, string strValue, string strExtraData)
        {
            m_strDescription = strDescription;
            m_strValue = strValue;
            m_strExtraData = strExtraData;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets/sets a string that represents a description of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public string Description
        {
            get
            {
                return m_strDescription;
            }

            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>
        /// Gets/sets a string that represents the value of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/07/08 jrf	1.00.00	   	   Created
        public string Value
        {
            get
            {
                return m_strValue;
            }

            set
            {
                m_strValue = value;
            }
        }

        /// <summary>
        /// Gets/sets a string that represents extra data associated with the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/08 jrf	1.50.27 114449 Created
        public string ExtraData
        {
            get
            {
                return m_strExtraData;
            }

            set
            {
                m_strExtraData = value;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The description of the TOU register data item.
        /// </summary>
        private string m_strDescription;

        /// <summary>
        /// The value of the TOU register
        /// </summary>
        private string m_strValue;

        /// <summary>
        /// Any extra data associated with the TOU register data item.
        /// </summary>
        private string m_strExtraData;

        #endregion
    }
}
