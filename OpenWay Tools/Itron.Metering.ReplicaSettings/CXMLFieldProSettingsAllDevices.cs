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
//                           Copyright © 2004 - 2009 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Field-Pro Settings - All Devices - Settings Access Class
	/// </summary>	
	//  Revision History
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  07/29/04 REM 7.00.15 N/A    Initial Release
	//  01/24/05 REM 7.10.05 1282   Add the ability to choose to clear or not to clear billing registers
	//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
	//  
	public class CXMLFieldProSettingsAllDevices : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		#region Constants

		/// <summary>
		/// public const string XML_NODE_USER_DATA_2 = "UserData2";
		/// </summary>
		protected const string XML_NODE_USER_DATA_2 = "UserData2";
		/// <summary>
		/// public const string XML_NODE_RESET_BILLING_ON_INIT = "ResetBillOnInit";
		/// </summary>
		protected const string XML_NODE_RESET_BILLING_ON_INIT = "ResetBillOnInit";
		/// <summary>
		/// public const string XML_NODE_CREATE_DATA_TYPE = "CreateDataType";
		/// </summary>
		protected const string XML_NODE_CREATE_DATA_TYPE = "CreateDataType";
		/// <summary>
		/// public const string XML_NODE_MV90HHF = "MV90HHF";
		/// </summary>
		protected const string XML_NODE_MV90HHF = "MV90HHF";
		/// <summary>
		/// public const string XML_NODE_HHF_NUM_DAYS = "HHFNumDays";
		/// </summary>
		protected const string XML_NODE_HHF_NUM_DAYS = "HHFNumDays";
        /// <summary>
        /// public const string XML_NODE_CRF_LP_SET2_NUM_DAYS = "CRFLPSet2NumDays";
        /// </summary>
        protected const string XML_NODE_CRF_LP_SET2_NUM_DAYS = "CRFLPSet2NumDays";
        /// <summary>
        /// public const string XML_NODE_CRF_ALLOW_OVERWRITE = "CRFAllowOverwrite";
        /// </summary>
        protected const string XML_NODE_CRF_ALLOW_OVERWRITE = "CRFAllowOverwrite";
        /// <summary>
        /// public const string XML_NODE_CRF_DATA_SELECTION = "CRFDataSelection";
        /// </summary>
        protected const string XML_NODE_CRF_DATA_SELECTION = "CRFDataSelection";
        /// <summary>
        /// public const string XML_NODE_CRF_NUMBER_OF_DAYS = "CRFNumberOfDays";
        /// </summary>
        protected const string XML_NODE_CRF_NUMBER_OF_DAYS = "CRFNumberOfDays";
        /// <summary>
        /// public const string XML_NODE_CRF_LOAD_PROFILE = "CRFLoadProfile";
        /// </summary>
        protected const string XML_NODE_CRF_LOAD_PROFILE = "CRFLoadProfile";
        /// <summary>
        /// public const string XML_NODE_CRF_EXTENDED_LOAD_PROFILE = "CRFExtendedLoadProfile";
        /// </summary>
        protected const string XML_NODE_CRF_EXTENDED_LOAD_PROFILE = "CRFExtendedLoadProfile";
        /// <summary>
        /// public const string XML_NODE_CRF_VOLTAGE_MONITORING_PROFILE = "CRFVoltageMonitoringProfile";
        /// </summary>
        protected const string XML_NODE_CRF_VOLTAGE_MONITORING_PROFILE = "CRFVoltageMonitoringProfile";
        /// <summary>
        /// public const string XML_NODE_CRF_INSTRUMENTATION_PROFILE = "CRFInstrumentationProfile";
        /// </summary>
        protected const string XML_NODE_CRF_INSTRUMENTATION_PROFILE = "CRFInstrumentationProfile";
        /// <summary>
        /// public const string XML_NODE_CRF_EVENT_DATA = "CRFEventData";
        /// </summary>
        protected const string XML_NODE_CRF_EVENT_DATA = "CRFEventData";
        /// <summary>
        /// protected const string XML_NODE_CRF_EVT_NUM_DAYS = "CRFEventDataNumDays";
        /// </summary>
        protected const string XML_NODE_CRF_EVT_NUM_DAYS = "CRFEventDataNumDays";
		/// <summary>
		/// public const string XML_NODE_AUTO_FILE = "AutoDataFileCreate";
		/// </summary>
		protected const string XML_NODE_AUTO_FILE = "AutoDataFileCreate";
		/// <summary>
		/// public const string XML_NODE_TRANSFER_LOG = "TransferActivitLog";
		/// </summary>
		protected const string XML_NODE_TRANSFER_LOG = "TransferActivitLog";
		/// <summary>
		/// public const string XML_NODE_LOG_DIR = "MasterStationLogDirectory";
		/// </summary>
		protected const string XML_NODE_LOG_DIR = "MasterStationLogDirectory";
		/// <summary>
		/// public const string XML_NODE_DATA_DIR = "MasterStationDataDirectory";
		/// </summary>
		protected const string XML_NODE_DATA_DIR = "MasterStationDataDirectory";
		/// <summary>
		/// public const string XML_NODE_LP_UNITS = "LPDisplayUnits";
		/// </summary>
		protected const string XML_NODE_LP_UNITS = "LPDisplayUnits";
		/// <summary>
		/// public const string XML_NODE_LP_PEAKS = "LPNumPeaks";
		/// </summary>
		protected const string XML_NODE_LP_PEAKS = "LPNumPeaks";
		/// <summary>
		/// public const string XML_NODE_FP_PWD = "FieldProPassword";
		/// </summary>
		protected const string XML_NODE_FP_PWD = "FieldProPassword";
        /// <summary>
        /// protected const string XML_NODE_FP_DEBUG = "DebugInfo";
        /// </summary>
        protected const string XML_NODE_FP_DEBUG = "DebugInfo";
		/// <summary>
		/// public const string XML_NODE_EDL_DATA_DIR = "MasterStationELDDataDirectory";
		/// </summary>
		protected const string XML_NODE_EDL_DATA_DIR = "MasterStationEDLDataDirectory";
		
		private const string DEFAULT_DIRECTORY = @"C:\";
		private const string EMPTY_FIELD_PRO_PASSWORD = "1a2b3c4d5e6f7g8h9j0kL";

		#endregion

		#region Definitions

		/// <summary>
		/// Enumeration for the number of LP days to save in an HHF
		/// </summary>
		public enum HHF_LP_DAYS
		{
            /// <summary>
            /// NO_DAYS = -1,
            /// </summary>
            NO_DAYS = -1,
            /// <summary>
			/// ALL_DAYS = 0,
			/// </summary>
			ALL_DAYS = 0,
            /// <summary>
            /// DAYS_130 = 1,
            /// </summary>
            DAYS_130 = 1,
			/// <summary>
			/// DAYS_70 = 2,
			/// </summary>
			DAYS_70 = 2,
			/// <summary>
			/// DAYS_45 = 3
			/// </summary>
			DAYS_45 = 3,
            /// <summary>
            /// DAYS_30 = 4
            /// </summary>
            DAYS_30 = 4,
            /// <summary>
            /// DAYS_15 = 5
            /// </summary>
            DAYS_15 = 5,
            /// <summary>
            /// USER_DEFINED = 6
            /// </summary>
            USER_DEFINED = 6,
		}

        /// <summary>
        /// Enumeration for the CRF Data Selection
        /// </summary>
        public enum CRF_DATA_SELECTION
        {
            /// <summary>
            /// ALL_DAYS = 0,
            /// </summary>
            ALL_DAYS = 0,
            /// <summary>
            /// NUMBER_OF_DAYS = 1
            /// </summary>
            NUMBER_OF_DAYS = 1,
            /// <summary>
            /// USER_DEFINED = 2
            /// </summary>
            USER_DEFINED = 2,
        }

		/// <summary>
		/// Enumeration for resetting billing on initialization
		/// </summary>
		public enum RESET_BILLING_REG_OPTIONS
		{
			/// <summary>
			/// DISBALED = 0,
			/// </summary>
			DISBALED = 0,
			/// <summary>
			/// ENABLED = 1,
			/// </summary>
			ENABLED = 1,
			/// <summary>
			/// PROMPT = 2
			/// </summary>
			PROMPT = 2
		}

		/// <summary>
		/// Enumeration for create data type
		/// </summary>
		public enum CREATE_DATA_TYPE
		{
			/// <summary>
			/// MIF_ONLY = 0,
			/// </summary>
			MIF_ONLY = 0,
			/// <summary>
			/// HHF_ONLY = 1,
			/// </summary>
			HHF_ONLY = 1,
			/// <summary>
			/// MIF_AND_HHF = 2
			/// </summary>
			MIF_AND_HHF = 2,
            /// <summary>
            /// EDL_ONLY = 3
            /// </summary>
            EDL_ONLY = 3,
		}

		/// <summary>
		/// Enumeration for HHF contents
		/// </summary>
		public enum HHF_CONTENTS
		{
			/// <summary>
			/// LP_ONLY = 0,
			/// </summary>
			LP_ONLY = 0,
			/// <summary>
			/// REG_ONLY = 1,
			/// </summary>
			REG_ONLY = 1,
			/// <summary>
			/// LP_AND_REG = 2
			/// </summary>
			LP_AND_REG = 2
		}

		/// <summary>
		/// Enumeration for LP display units
		/// </summary>
		public enum LP_DISP_UNITS
		{
			/// <summary>
			/// PULSES = 0,
			/// </summary>
			PULSES = 0,
			/// <summary>
			/// ENERGY = 1,
			/// </summary>
			ENERGY = 1,
			/// <summary>
			/// DEMAND = 2
			/// </summary>
			DEMAND = 2
		}

        /// <summary>
        /// Enumeration for the number of days of event data to save in a CRF file
        /// </summary>
        public enum CRF_EVT_DAYS
        {
            /// <summary>
            /// Use the date range specified for LP data
            /// </summary>
            LP_DATE_RANGE = 0,
            /// <summary>
            /// All event data
            /// </summary>
            ALL_DAYS = 1,
            /// <summary>
            /// No event data
            /// </summary>
            NONE = 2,
        }

		#endregion

		#region Public Methods

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>		
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/29/04 REM 7.00.15 N/A    Initial Release
		//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
		//  
		public CXMLFieldProSettingsAllDevices(CXMLSettings XMLSettings)
		{
			m_XMLSettings = XMLSettings;			
		}

		#endregion

		#region Public Properties
	
		/// <summary>
		/// Choice to set custom schedule name to user data 2
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual int SetCSToUserData2
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_USER_DATA_2, true);
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_USER_DATA_2, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Choice to reset billing on initialization
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual RESET_BILLING_REG_OPTIONS ResetBillingOnInit
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_RESET_BILLING_ON_INIT, true);
				return (RESET_BILLING_REG_OPTIONS)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_RESET_BILLING_ON_INIT, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}
		}

		/// <summary>
		/// Choice of data type
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual CREATE_DATA_TYPE CreateDataType
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_CREATE_DATA_TYPE, true);
				return (CREATE_DATA_TYPE)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_CREATE_DATA_TYPE, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}
		}

		/// <summary>
		/// Choice of HHF contents
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual HHF_CONTENTS HHFContents
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_MV90HHF, true);
				return (HHF_CONTENTS)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_MV90HHF, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}
		}

		/// <summary>
		/// Choice of max lp data in HHF
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual HHF_LP_DAYS MaxLPInHHF
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_NUM_DAYS, true);
				return (HHF_LP_DAYS)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_HHF_NUM_DAYS, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}
		}

        /// <summary>
        /// Determines if the max LP set 2 data in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/23/12 jrf 2.53.51 TREQ2891 Created
        // 
        public virtual bool MaxLPSet2InCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_LP_SET2_NUM_DAYS, false);

                return blnNodeExists;
            }
        }
        
        /// <summary>
        /// Choice of max lp set 2 data in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/23/12 jrf 2.53.51 TREQ2891 Created
        //  
        public virtual HHF_LP_DAYS MaxLPSet2InCRF
        {
            get
            {
                if (false == MaxLPSet2InCRFSettingExists)
                {
                    //Default setting should be no days
                    MaxLPSet2InCRF = HHF_LP_DAYS.NO_DAYS;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_LP_SET2_NUM_DAYS, true);
                return (HHF_LP_DAYS)m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_LP_SET2_NUM_DAYS, true);
                m_XMLSettings.CurrentNodeInt = (int)value;
            }
        }

        /// <summary>
        /// Determines if the Allow Overwrite CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool AllowOverwriteInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_ALLOW_OVERWRITE, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Allow Overwrite in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int AllowOverwriteInCRF
        {
            get
            {
                if (false == AllowOverwriteInCRFSettingExists)
                {
                    //Default setting should be 0 (false)
                    AllowOverwriteInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_ALLOW_OVERWRITE, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_ALLOW_OVERWRITE, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if the Data Selection CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool DataSelectionInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_DATA_SELECTION, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Data Selection in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  02/17/16 PGH 4.50.230 649353   Updated
        //  
        public virtual int DataSelectionInCRF
        {
            get
            {
                if (false == DataSelectionInCRFSettingExists)
                {
                    //Default setting
                    DataSelectionInCRF = (int)CRF_DATA_SELECTION.NUMBER_OF_DAYS;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_DATA_SELECTION, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_DATA_SELECTION, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if the Number of Days CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool NumberOfDaysInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_NUMBER_OF_DAYS, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Number of Days in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int NumberOfDaysInCRF
        {
            get
            {
                if (false == NumberOfDaysInCRFSettingExists)
                {
                    //Default setting
                    NumberOfDaysInCRF = 15;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_NUMBER_OF_DAYS, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_NUMBER_OF_DAYS, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if Load Profile in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool LoadProfileInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_LOAD_PROFILE, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Load Profile in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int LoadProfileInCRF
        {
            get
            {
                if (false == LoadProfileInCRFSettingExists)
                {
                    //Default setting
                    LoadProfileInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_LOAD_PROFILE, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_LOAD_PROFILE, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if Extended Load Profile in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool ExtendedLoadProfileInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_EXTENDED_LOAD_PROFILE, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Extended Load Profile in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int ExtendedLoadProfileInCRF
        {
            get
            {
                if (false == ExtendedLoadProfileInCRFSettingExists)
                {
                    //Default setting
                    ExtendedLoadProfileInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_EXTENDED_LOAD_PROFILE, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_EXTENDED_LOAD_PROFILE, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if Voltage Monitoring Profile in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool VoltageMonitoringProfileInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_VOLTAGE_MONITORING_PROFILE, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Voltage Monitoring Profile in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int VoltageMonitoringProfileInCRF
        {
            get
            {
                if (false == VoltageMonitoringProfileInCRFSettingExists)
                {
                    //Default setting
                    VoltageMonitoringProfileInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_VOLTAGE_MONITORING_PROFILE, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_VOLTAGE_MONITORING_PROFILE, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if Instrumentation Profile in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool InstrumentationProfileInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_INSTRUMENTATION_PROFILE, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Instrumentation Profile in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int InstrumentationProfileInCRF
        {
            get
            {
                if (false == InstrumentationProfileInCRFSettingExists)
                {
                    //Default setting
                    InstrumentationProfileInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_INSTRUMENTATION_PROFILE, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_INSTRUMENTATION_PROFILE, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Determines if Event Data in CRF setting exists in the settings file.
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        // 
        public virtual bool EventDataInCRFSettingExists
        {
            get
            {
                bool blnNodeExists = true;

                m_XMLSettings.SetCurrentToAnchor();

                blnNodeExists = m_XMLSettings.SelectNode(XML_NODE_CRF_EVENT_DATA, false);

                return blnNodeExists;
            }
        }

        /// <summary>
        /// Event Data in CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue#   Description
        //  -------- --- ------- -------- ---------------------------------------------
        //  04/22/15 PGH 4.50.106 SREQ7642 Created
        //  
        public virtual int EventDataInCRF
        {
            get
            {
                if (false == EventDataInCRFSettingExists)
                {
                    //Default setting
                    EventDataInCRF = 0;
                }

                m_XMLSettings.SetCurrentToAnchor();

                m_XMLSettings.SelectNode(XML_NODE_CRF_EVENT_DATA, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_EVENT_DATA, true);
                m_XMLSettings.CurrentNodeInt = value;
            }
        }

        /// <summary>
        /// Choice of max event data in CRF
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/18/09 AF  2.30.02        Created
        //
        public virtual CRF_EVT_DAYS MaxEventInCRF
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_EVT_NUM_DAYS, true);
                return (CRF_EVT_DAYS)m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_CRF_EVT_NUM_DAYS, true);
                m_XMLSettings.CurrentNodeInt = (int)value;
            }
        }

        //public virtual 

		/// <summary>
		/// Choice of auto file create
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual int AutoFileCreate
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_AUTO_FILE, true);
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_AUTO_FILE, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Choice to transfer log to master station
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual int TransferLog
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_TRANSFER_LOG, true);
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_TRANSFER_LOG, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Upload - Master Station's Log Directory
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  09/26/07 mrj 9.00.12 2984	Changed default to the application directory.
		//  
		public virtual string MasterStationLogDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LOG_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
#else
					strReturn = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LOG_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Upload - Master Station's Data Directory.  This property is used by the
		/// PC-Pro+ Advanced system of tools.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  09/26/07 mrj 9.00.12 2984	Changed default to the application directory.
		//  
		public virtual string MasterStationDataDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_DATA_DIR, true);
				string strReturn = m_XMLSettings.CurrentNodeString;

				if (null == strReturn || "" == strReturn)
				{					
#if (!WindowsCE)
					strReturn = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
#else
					strReturn = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_DATA_DIR, true);
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Choice of LP display units
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual LP_DISP_UNITS LPDisplayUnits
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LP_UNITS, true);
				return (LP_DISP_UNITS)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LP_UNITS, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}
		}

		/// <summary>
		/// Choice of number LP peaks
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual int LPNumPeaks
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LP_PEAKS, true);
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_LP_PEAKS, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// All Devices - Field-Pro Password
		/// </summary>
		/// <remarks>
		/// Because the user could just remove the password from the XML file, we
		/// needed to create a password an empty password.
		/// </remarks>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual string FieldProPassword
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_FP_PWD, true);

				string strPwd = CShared.DecodeString(m_XMLSettings.CurrentNodeString);
				if ("" == strPwd)
				{
					//This file has been manually changed so return null
					strPwd = null;
				}
				else if (EMPTY_FIELD_PRO_PASSWORD == strPwd)
				{
					//This is the empty password
					strPwd = "";
				}

				return strPwd;
			}
			set
			{
				string strPwd = value;

				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_FP_PWD, true);

				if (null == strPwd ||
					"" == strPwd)
				{
					//Set the empty password
					strPwd = EMPTY_FIELD_PRO_PASSWORD;
				}
				m_XMLSettings.CurrentNodeString = CShared.EncodeString(strPwd);
			}
		}

        /// <summary>
        /// Gets whether or not to show Debug Information in Field-Pro
        /// </summary>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/15/07 RCG 8.10.05        Created

        public virtual int DebugInformation
        {
            get
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_FP_DEBUG, true);
                return m_XMLSettings.CurrentNodeInt;
            }
            set
            {
                m_XMLSettings.SetCurrentToAnchor();
                m_XMLSettings.SelectNode(XML_NODE_FP_DEBUG, true);
                m_XMLSettings.CurrentNodeInt = value;
            }

        }

		#endregion		
	}
}
