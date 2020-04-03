using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Custom Schedule Programmer XML Settings class
	/// </summary>
	/// <remarks><pre>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------------
	/// 06/20/05 mrj 7.13.00        Created
	///	08/16/06 mrj 7.35.00		Added items for the 7.35 project
	///	
	///</pre></remarks>
    public class CXMLCustSchedProgSettings : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Enumeration
		/// <summary>
		/// Custom Schedule Programmer settings enumeration
		/// </summary>
		protected enum CustSchedProgSettings
		{
			/// <summary>
			/// UseWorklist = 0,
			/// </summary>
			UseWorklist = 0,
			/// <summary>
			/// WorklistLocation = 1,
			/// </summary>
			WorklistLocation = 1,
			/// <summary>
			/// Override = 2,
			/// </summary>
			Override = 2,
			/// <summary>
			/// UserData2 = 3,
			/// </summary>
			UserData2 = 3,
			/// <summary>
			/// TransferActivityLog = 4,
			/// </summary>
			TransferActivityLog = 4,
			/// <summary>
			/// MasterStationDataDirectory = 5,
			/// </summary>
			MasterStationDataDirectory = 5,
			/// <summary>
			/// DSTUpdate = 6
			/// </summary>
			DSTUpdate = 6,
			/// <summary>
			/// ClockAdjust = 7
			/// </summary>
			ClockAdjust = 7,
			/// <summary>
			/// ClockAdjustThreshold = 8
			/// </summary>
			ClockAdjustThreshold = 8,
			/// <summary>
			/// ResetDiag = 9
			/// </summary>
			ResetDiag = 9,			
			/// <summary>
			/// PasswordReconfig
			/// </summary>
			PasswordReconfig,
			/// <summary>
			/// ResetDemand
			/// </summary>
			ResetDemand,
			/// <summary>
			/// ResetBilling
			/// </summary>
			ResetBilling,
			/// <summary>
			/// ResetCanadianEventLog
			/// </summary>
			ResetCanadianEventLog,
			/// <summary>
			/// ReconfigureTOU
			/// </summary>
			ReconfigureTOU,
			/// <summary>
			/// MV90HHF
			/// </summary>
			MV90HHF,
			/// <summary>
			/// HHFNumDays
			/// </summary>
			HHFNumDays,
			/// <summary>
			/// OverridePassword
			/// </summary>
			OverridePassword,
			/// <summary>
			/// MasterStationHHFDataDirectory
			/// </summary>
			MasterStationHHFDataDirectory,
			/// <summary>
			/// NumberSettings in the CustSchedProgSettings enumeration
			/// </summary>
			NumberSettings
		}

		//Constants
		/// <summary>
		/// XML_NODE_CUSTOM_SCEDULE_PROGAMMER = "CustomScheduleProgrammer";
		/// </summary>
		protected const string XML_NODE_CUSTOM_SCEDULE_PROGAMMER = "CustomScheduleProgrammer";
		/// <summary>
		/// XML_NODE_USE_WORKLIST = "UseWorklist";
		/// </summary>
		protected const string XML_NODE_USE_WORKLIST = "UseWorklist";
		/// <summary>
		/// XML_NODE_OVERRIDE = "Override";
		/// </summary>
		protected const string XML_NODE_OVERRIDE = "Override";
		/// <summary>
		/// XML_NODE_USER_DATA_2 = "UserData2";
		/// </summary>
		protected const string XML_NODE_USER_DATA_2 = "UserData2";
		/// <summary>
		/// XML_NODE_TRANSFER_ACTIVITY_LOG = "TransferActivitLog";
		/// </summary>
		protected const string XML_NODE_TRANSFER_ACTIVITY_LOG = "TransferActivitLog";
		/// <summary>
		/// XML_NODE_MASTER_STATION_DIRECTORY = "MasterStationDirectory";
		/// </summary>
		protected const string XML_NODE_MASTER_STATION_DIRECTORY = "MasterStationDirectory";
		/// <summary>
		/// XML_NODE_DST_UPDATE = "DSTUpdate";
		/// </summary>
		protected const string XML_NODE_DST_UPDATE = "DSTUpdate";
		/// <summary>
		/// XML_NODE_CLOCK_ADJUST = "ClockAdjust";
		/// </summary>
		protected const string XML_NODE_CLOCK_ADJUST = "ClockAdjust";
		/// <summary>
		/// XML_NODE_CLOCK_ADJUST_THRESHOLD = "ClockAdjustThreshold";
		/// </summary>
		protected const string XML_NODE_CLOCK_ADJUST_THRESHOLD = "ClockAdjustThreshold";
		/// <summary>
		/// XML_NODE_RESET_DIAG = "ResetDiag";
		/// </summary>
		protected const string XML_NODE_RESET_DIAG = "ResetDiag";

		//mrj 8/16/06, added new items for 7.35 project
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "PasswordReconfig";
		/// </summary>		
		protected const string XML_NODE_PASSWORD_RECONFIG = "PasswordReconfig";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "ResetDemand";
		/// </summary>		
		protected const string XML_NODE_RESET_DEMAND = "ResetDemand";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "ResetBilling";
		/// </summary>		
		protected const string XML_NODE_RESET_BILLING = "ResetBilling";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "ResetCanadianEventLog";
		/// </summary>		
		protected const string XML_NODE_RESET_CANADIAN_LOG = "ResetCanadianEventLog";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "ReconfigureTOU";
		/// </summary>		
		protected const string XML_NODE_RECONFIG_TOU = "ReconfigureTOU";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "MV90HHF";
		/// </summary>		
		protected const string XML_NODE_HHF_DATA_TYPES = "MV90HHF";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "HHFNumDays";
		/// </summary>		
		protected const string XML_NODE_HHF_NUM_DAYS = "HHFNumDays";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "OverridePassword";
		/// </summary>
		protected const string XML_NODE_OVERRIDE_PASSWORD = "OverridePassword";
		/// <summary>
		/// XML_NODE_PASSWORD_RECONFIG = "MasterStationHHFDataDirectory";
		/// </summary>		
		protected const string XML_NODE_HHF_SYNC_DIR = "MasterStationHHFDataDirectory";
		
		private const string DEFAULT_DIRECTORY = @"C:\";

		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File Name of XML file to use. If "" is passed in the default is used</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public CXMLCustSchedProgSettings( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + "CustSchedProgrammer.xml", "", "CustomScheduleProgrammer" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}
		}

		/// <summary>
		/// Choice of the use of the Worklist in the Custom Schedule Programmer.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public virtual int UseWorklist
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_USE_WORKLIST, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_USE_WORKLIST, true );
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Choice of the ability to override the Worklist (Custom Schedule Programmer).
		/// Choice of the ability to reconfigure the Custom Scheudle (HH-Pro).
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public virtual int Override
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_OVERRIDE, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_OVERRIDE, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// SENTINEL - Custom Schedule Name in User Data #2 During Reconfigure
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public virtual int UserData2
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_2, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_USER_DATA_2, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to transfer the activity log to the master station
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public virtual int TransferActivityLog
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_TRANSFER_ACTIVITY_LOG, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_TRANSFER_ACTIVITY_LOG, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Synchronization - Master Station's Log Directory
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///06/20/05 mrj 7.13.00        Created
		///</pre></remarks>
		public virtual string MasterStationDataDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_MASTER_STATION_DIRECTORY, true );								
				string strReturn  = m_XMLSettings.CurrentNodeString;

				if( null == strReturn || "" == strReturn )
				{
					strReturn = DEFAULT_DIRECTORY;
				}

				return strReturn;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_MASTER_STATION_DIRECTORY, true );
				m_XMLSettings.CurrentNodeString = value;
			}
		}

		/// <summary>
		/// Choice to update DST
		/// </summary>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///04/18/06 mrj 7.30.00        Created for HH-Pro
		///
		public virtual int DSTUpdate
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_DST_UPDATE, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_DST_UPDATE, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to update device clock
		/// </summary>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///04/18/06 mrj 7.30.00        Created for HH-Pro
		///
		public virtual int ClockAdjust
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CLOCK_ADJUST, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CLOCK_ADJUST, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Device Clock adjust threshold
		/// </summary>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///04/18/06 mrj 7.30.00        Created for HH-Pro
		///07/18/06 mrj 7.30.33 2242   Changed default to 60 seconds.
		///
		public virtual int ClockAdjustThreshold
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CLOCK_ADJUST_THRESHOLD, true );
				int iReturn = m_XMLSettings.CurrentNodeInt;

				if( 0 == iReturn )
				{
					iReturn = 60;
				}
				return iReturn;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_CLOCK_ADJUST_THRESHOLD, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow reset of diagnostics
		/// </summary>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///04/18/06 mrj 7.30.00        Created for HH-Pro
		///
		public virtual int ResetDiag
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_DIAG, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_DIAG, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow password reconfigure
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int PasswordReconfig
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_PASSWORD_RECONFIG, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_PASSWORD_RECONFIG, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow reset of demand
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int ResetDemand
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_DEMAND, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_DEMAND, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow reset of billing registers
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int ResetBilling
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_BILLING, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_BILLING, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow reset of Canadian event log
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int ResetCanadianEventLog
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_CANADIAN_LOG, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RESET_CANADIAN_LOG, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}
	
		/// <summary>
		/// Choice to allow reconfigure of TOU
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int ReconfigureTOU
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RECONFIG_TOU, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_RECONFIG_TOU, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice of what data types to include in the HHF file.
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int HHFDataTypes
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_DATA_TYPES, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_DATA_TYPES, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice of how many LP days to include in the HHF file.
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int HHFNumDays
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_NUM_DAYS, true );
				return m_XMLSettings.CurrentNodeInt;									
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_NUM_DAYS, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}

		/// <summary>
		/// Choice to allow override of meter password.
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual int OverridePassword
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_OVERRIDE_PASSWORD, true );
				return m_XMLSettings.CurrentNodeInt;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_OVERRIDE_PASSWORD, true );
				m_XMLSettings.CurrentNodeInt = value;
			}			
		}
		
		/// <summary>
		/// Synchronization - Master Station's HHF Data Directory
		/// </summary>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 08/16/06 mrj 7.35.00        Added for the 7.35 project
		///
		public virtual string MasterStationHHFDataDirectory
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_SYNC_DIR, true );
				string strReturn  = m_XMLSettings.CurrentNodeString;

				if( null == strReturn || "" == strReturn )
				{
					strReturn = DEFAULT_DIRECTORY;
				}

				return strReturn;				
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode( XML_NODE_HHF_SYNC_DIR, true );
				m_XMLSettings.CurrentNodeString = value;
			}			
		}		
	}
}
