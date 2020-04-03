///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2006 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.Utilities
{

    /// <summary>
    /// This class represents a single entry in the Meter Activity log.
    /// </summary>
    public class MeterActivityLogEntry
	{
		/// <summary>
		///  This enumeration represents all of the activities that a 
		///  meter activity log entry can represent.  A new item should be 
		///  added to this enumeration whenever a new meter action is added
		///  to a client application.  
		/// </summary>
		public enum ActivityTypeEnum
		{
			/// <summary>
			///     
			/// </summary>
			Logon,
			/// <summary>
			///     
			/// </summary>
			Initialization,
			/// <summary>
			///     
			/// </summary>
			TOUReconfiguration,
			/// <summary>
			///     
			/// </summary>
			DemandReset,
			/// <summary>
			///     
			/// </summary>
			ClockAdjust,
			/// <summary>
			///     
			/// </summary>
			ResetBillingRegisters,
			/// <summary>
			///     
			/// </summary>
			EditRegisters,
			/// <summary>
			///     
			/// </summary>
			RegisterHost,
			/// <summary>
			///     
			/// </summary>
			DeregisterHost,
			/// <summary>
			///     
			/// </summary>
			ResetRFLAN,
			/// <summary>
			///     
			/// </summary>
			DecommissionHAN,
			/// <summary>
			///     
			/// </summary>
			DecommissionDropNode,
			/// <summary>
			///     
			/// </summary>
			SetDataDeliveryConfig,
			/// <summary>
			///     
			/// </summary>
			SetCollectionConfig,
			/// <summary>
			///     
			/// </summary>
			ActivatePendingTable,
			/// <summary>
			///     
			/// </summary>
			ClearPendingTable,
			/// <summary>
			///     
			/// </summary>
			ResetActivityStatus,
			/// <summary>
			///     
			/// </summary>
			ResetInversionTamper,
			/// <summary>
			///     
			/// </summary>
			ResetRemovalTamper,
			/// <summary>
			///     
			/// </summary>
			EnterTestMode,
			/// <summary>
			///     
			/// </summary>
			ExitTestMode,
			/// <summary>
			///     
			/// </summary>
			ConnectService,
			/// <summary>
			///     
			/// </summary>
			DisconnectService,
			/// <summary>
			///     
			/// </summary>
			ValidateProgram,
			/// <summary>
			///     
			/// </summary>
			FirmwareLoad,
			/// <summary>
			///     
			/// </summary>
			CreateEDL,
			/// <summary>
			///     
			/// </summary>
			CreateHHF,
			/// <summary>
			///     
			/// </summary>
			CreateMIF,
			/// <summary>
			///     
			/// </summary>
			ClearVQData,
			/// <summary>
			///     
			/// </summary>
			ClearSitescanSnapshots,
			/// <summary>
			///     
			/// </summary>
			ReconfigModemAnswerDelay,
			/// <summary>
			///     
			/// </summary>
			ReconfigPhonePrefix,
			/// <summary>
			///     
			/// </summary>
			WritePendingTOU,
			/// <summary>
			///     
			/// </summary>
			ReconfigurePasswords,
			/// <summary>
			///     
			/// </summary>
			ReconfigureTertiaryPassword,
			/// <summary>
			///     
			/// </summary>
			ReconfigureCustomSchedule,
			/// <summary>
			///     
			/// </summary>
			ReconfigureDST,
            /// <summary>
            /// 
            /// </summary>
            CreateCRF,
            /// <summary>
            /// 
            /// </summary>
            ClearAllMeterData,
            /// <summary>
            /// 
            /// </summary>
            ConfigureKYZ,
            /// <summary>
            /// 
            /// </summary>
            EnableHANJoining,
            /// <summary>
            /// 
            /// </summary>
            ForceTimeSync,
            /// <summary>
            /// 
            /// </summary>
            SiteScanReconfigure,
            /// <summary>
            /// 
            /// </summary>
            SetHANMultiplier,
            /// <summary>
            /// 
            /// </summary>
            EnableHAN,
            /// <summary>
            /// 
            /// </summary>
            DisableHAN,
            /// <summary>
            /// 
            /// </summary>
            EnableSL,
            /// <summary>
            /// 
            /// </summary>
            DisableSL,
            /// <summary>
            /// 
            /// </summary>
            EnableDisconnectSwitch,
            /// <summary>
            /// 
            /// </summary>
            DisableDisconnectSwitch,
            /// <summary>
            /// Signed Authorization disabled
            /// </summary>
            DisableSignedAuthorization,
            /// <summary>
            /// Enable C12.18 Over ZigBee
            /// </summary>
            EnableC1218OverZigBee,
            /// <summary>
            /// Disable C12.18 Over ZigBee
            /// </summary>
            DisableC1218OverZigBee,
            /// <summary>
            /// Clear the firmware download event log
            /// </summary>
            ClearFWDLEventLog,
            /// <summary>
            /// Switch Communications to OpenWay Operational Mode
            /// </summary>
            SwitchCommToOpenWay,
            /// <summary>
            /// Switch Communications to ChoiceConnect Operational Mode
            /// </summary>
            SwitchCommToChoiceConnect,
            /// <summary>
            /// Perform HAN Move Out Operations
            /// </summary>
            HANMoveOut,
            /// <summary>
            /// Disable HAN pricing model.
            /// </summary>
            HANDisablePricing,
            /// <summary>
            ///     
            /// </summary>
            ResetMagneticTamper,
            /// <summary>
            /// Clear HAN Reset Limiting Halt Condition
            /// </summary>
            ClearResetLimitingHaltCondition,
            /// <summary>
            /// Update the Cellular Gateway Address
            /// </summary>
            UpdateCellularGateway,
            /// <summary>
            /// Update the ERT Utility ID
            /// </summary>
            UpdateERTUtilityID,
            /// <summary>
            /// Seal Canadian Procedure
            /// </summary>
            SealCanadian,
            /// <summary>
            /// Unseal Canadian Procedure
            /// </summary>
            UnsealCanadian,
            /// <summary>
            /// Create a Core Dump File
            /// </summary>
            CreateCoreDump,
		};

		/// <summary>
		/// This enumeration contains the set of all possible activity outcomes.
		/// Note that many statuses are activity specific but care should be taken
		/// when adding new statuses to insure that there are no ambiguous entries.
		/// </summary>
		public enum ActivityStatusEnum
		{
			/// <summary>
			///     
			/// </summary>
			Success,
			/// <summary>
			///     
			/// </summary>
			SecurityError,
			/// <summary>
			///     
			/// </summary>
			Timeout,
			/// <summary>
			///     
			/// </summary>
			DeviceSetupConflict,
			/// <summary>
			///     
			/// </summary>
			InvalidConfiguration,
			/// <summary>
			///     
			/// </summary>
			OperationIncomplete,
			/// <summary>
			///     
			/// </summary>
			UnsupportedOperation,
			/// <summary>
			///     
			/// </summary>
			TimingConstraint,
			/// <summary>
			///     
			/// </summary>
			UnrecognizedError,
			/// <summary>
			///     
			/// </summary>
			LoadVoltageDetected,
			/// <summary>
			///     
			/// </summary>
			ProtocolError,
			/// <summary>
			///     
			/// </summary>
			InsufficentDiskSpace,
			/// <summary>
			///     
			/// </summary>
			InvalidPath,
			/// <summary>
			///     
			/// </summary>
			CannotOpenFile,
			/// <summary>
			///     
			/// </summary>
			CannotReadFile,
			/// <summary>
			///     
			/// </summary>
			FileIOError,
			/// <summary>
			///     
			/// </summary>
			MemoryError,
			/// <summary>
			///     
			/// </summary>
			MemoryMapError,
			/// <summary>
			///     
			/// </summary>
			NetworkError,
			/// <summary>
			///     
			/// </summary>
			OffLine,
			/// <summary>
			///     
			/// </summary>
			ClockError,
			/// <summary>
			///     
			/// </summary>
			DataMissing,
			/// <summary>
			///     
			/// </summary>
			NoChangeRequested,
			/// <summary>
			///     
			/// </summary>
			RetryError,
			/// <summary>
			///     
			/// </summary>
			ScheduleNotSupported,
			/// <summary>
			///     
			/// </summary>
			ScheduleExpired,
			/// <summary>
			///     
			/// </summary>
			ScheduleNotValid,
			/// <summary>
			///     
			/// </summary>
			DSTNotSupported,
			/// <summary>
			///     
			/// </summary>
			TOUNotSupported,
			/// <summary>
			///     
			/// </summary>
			NoCalendarData,
			/// <summary>
			///     
			/// </summary>
			NoTOUData,
			/// <summary>
			///     
			/// </summary>
			NoBillingScheduleData,
			/// <summary>
			///     
			/// </summary>
			PendingTablesFull,
			/// <summary>
			///     
			/// </summary>
			DuplicatePassword,
			/// <summary>
			///     
			/// </summary>
			TimeCrossesIntervalBoundry,
			/// <summary>
			///     
			/// </summary>
			InDSTChange,
			/// <summary>
			///     
			/// </summary>
			FileTooLarge,
			/// <summary>
			///     
			/// </summary>
			InvalidType,
			/// <summary>
			///     
			/// </summary>
			VersionOutOfRange,
			/// <summary>
			///     
			/// </summary>
			RevisionOutOfRange,
			/// <summary>
			///     
			/// </summary>
			WriteError,
			/// <summary>
			///     
			/// </summary>
			ZigbeeFWTypeInvalid,
			/// <summary>
			///     
			/// </summary>
			MeterInTestMode,
			/// <summary>
			///     
			/// </summary>
			MeterNotInTestMode,
			/// <summary>
			///     
			/// </summary>
			CommunicationsError,
			/// <summary>
			///     
			/// </summary>
			CRCError,
			/// <summary>
			///     
			/// </summary>
			InvalidDate,
			/// <summary>
			///     
			/// </summary>
			InvalidParameter,
			/// <summary>
			///     
			/// </summary>
			NoDataSelected,
			/// <summary>
			///     
			/// </summary>
			NoProfileData,
			/// <summary>
			///     
			/// </summary>
			TIMBlockMismatch,
			/// <summary>
			///     
			/// </summary>
			/// <summary>
			///     
			/// </summary>
			TIMNotFound,
			/// <summary>
			///     
			/// </summary>
			/// <summary>
			///     
			/// </summary>
			SystemResourceError,
			/// <summary>
			///     
			/// </summary>
			TIMError,
			/// <summary>
			///     
			/// </summary>
			DatabaseAccessError,
			/// <summary>
			///     
			/// </summary>
			MismatchedID,
			/// <summary>
			///     
			/// </summary>
			ProgramNotFound,
			/// <summary>
			///     
			/// </summary>
			InvalidProgram,
			/// <summary>
			///     
			/// </summary>
			UserAborted,
            /// <summary>
            /// 
            /// </summary>
            OutOfSyncLessThanHysteresis,
            /// <summary>
            /// 
            /// </summary>
            ReconfigureError,
            /// <summary>
            /// The meter was not synced
            /// </summary>
            NotSynced,
            /// <summary>
            /// This operation is already in progress
            /// </summary>
            OperationInProgress,
            /// <summary>
            /// Error committing data.
            /// </summary>
            CommitError
		};

		#region Constructors

		/// <summary>
		/// This constructor should be used when it is impossible to determine
		/// the meter's identity.
		/// </summary>
		/// <param name="eActivityType" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityTypeEnum">
		/// </param>
		/// <param name="eActivityStatus" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityStatusEnum">
		/// </param>
		public MeterActivityLogEntry( ActivityTypeEnum eActivityType,
									  ActivityStatusEnum eActivityStatus ) 
		{
			//Initialize member variables
			EventTime = DateTime.Now;
			MeterType = "";
			UnitID = "";
			SerialNumber = "";
			Source = "";
			SourceVersion = "";
			ResultFile = "";
			ActivityType = eActivityType;
			ActivityStatus = eActivityStatus;
		}

		/// <summary>
		/// This constructor should be used whenever logging an event and 
		/// the meter's identity is known.
		/// </summary>
		/// <param name="strDeviceType" type="string">
		/// </param>
		/// <param name="strUnitID" type="string">
		/// </param>
		/// <param name="strSerialNumber" type="string">
		/// </param>
		/// <param name="eActivityType" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityTypeEnum">
		/// </param>
		/// <param name="eActivityStatus" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityStatusEnum">
		/// </param>
		public MeterActivityLogEntry(String strDeviceType,
										String strUnitID,
										String strSerialNumber,
										ActivityTypeEnum eActivityType,
										ActivityStatusEnum eActivityStatus)
		{
			//Initialize member variables
			EventTime = DateTime.Now;
			MeterType = strDeviceType;
			UnitID = strUnitID;
			SerialNumber = strSerialNumber;
			Source = "";
			SourceVersion = "";
			ResultFile = "";
			ActivityType = eActivityType;
			ActivityStatus = eActivityStatus;
		}

		/// <summary>
		/// This constructor should only be used to retrieve activity log
		/// entries from the database. Since all of the member fields will be 
		/// initialized from persistent data, there is no need to initialize
		/// them here
		/// </summary>
		internal MeterActivityLogEntry()
		{
			// This constructor should only be used to retrieve activity log
			// entries from the
		}

		#endregion
							
		#region Public Properties
		/// <summary>
		/// Property to get and set the date and time that the event occurred.
		/// Returned as a DateTime object
		/// </summary>
		/// <example>
		/// <code>
		/// CCustSchedItem x = new CCustSchedItem();
		/// x.EventTime = DateTime.Now;
		/// DateTime y = x.EventTime;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/14/05 rrr 7.13.00 N/A	Creation of class  
		public DateTime EventTime
		{
			get
			{
				return m_dtmEventTime;
			}
			set
			{
				m_dtmEventTime = value;
			}
		}

		/// <summary>
		/// Property that gets and sets the meter type for the log item
		/// </summary>
		/// <example>
		/// <code>
		/// CCustSchedItem x = new CCustSchedItem();
		/// x.MeterType = "SENTINEL";
		/// string y = x.MeterType;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/14/05 rrr 7.13.00 N/A	Creation of class  
		public string MeterType
		{
			get
			{
				return m_strMeterType;
			}
			set
			{
				m_strMeterType = value;
			}
		}

		/// <summary>
		/// Property that gets and sets the unit id for the log item
		/// </summary>
		/// <example>
		/// <code>
		/// CCustSchedItem x = new CCustSchedItem();
		/// x.UnitID = "SENTINEL";
		/// string y = x.UnitID;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/14/05 rrr 7.13.00 N/A	Creation of class 
		public String UnitID
		{
			get
			{
				return m_strUnitID;
			}
			set
			{
				m_strUnitID = value;
			}
		}

		/// <summary>
		/// Property that gets and sets the meter serial number for the log item
		/// </summary>
		/// <example>
		/// <code>
		/// CCustSchedItem x = new CCustSchedItem();
		/// x.SerialNumber = "SENTINEL";
		/// string y = x.SerialNumber;
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 07/14/05 rrr 7.13.00 N/A	Creation of class 
		public String SerialNumber
		{
			get
			{
				return m_strSerialNumber;
			}
			set
			{
				m_strSerialNumber = value;
			}
		}

		/// <summary>
		/// Property to retrieve or record the type of activity that was performed    
		/// </summary>
		public ActivityTypeEnum ActivityType
		{
			get
			{
				return m_eActivityType;
			}
			set
			{
				m_eActivityType = value;
			}
		}

		/// <summary>
		/// Property to retrieve or record the outcome of activity that was performed    
		/// </summary>
		public ActivityStatusEnum ActivityStatus
		{
			get
			{
				return m_eActivityStatus;
			}
			set
			{
				m_eActivityStatus = value;
			}

		}

		/// <summary>
		/// Property to retrieve or record any data used in performing a given activity.
		///  For example, this property should contain the name of the program used to 
		///  configure a meter or the name of the firmware file when downloading  
		///  firmware to a meter.
		/// </summary>
		public String Source
		{
			get
			{
				return m_strSource;
			}
			set
			{
				m_strSource = value;
			}
		}

		/// <summary>
		/// This property represents the version number associated with the source
		/// property (if applicable).  Note that this property should not be used 
		/// if a given activity does not use versioned input data
		/// </summary>
		public String SourceVersion
		{
			get
			{
				return m_strSourceVersion;
			}
			set
			{
				m_strSourceVersion = value;
			}
		}

		/// <summary>
		/// This property represents the name of the file that was generated as 
		/// a result the given activity.  Note that we typically only record the
		/// file name and extension.
		/// </summary>
		public String ResultFile
		{
			get
			{
				return m_strResultFile;
			}
			set
			{
				m_strResultFile = value;
			}
		}

		/// <summary>
		/// This property represents the name of the application that performed
		/// the given action.  This property is typically set implicitly when the 
		/// log entry is added to the meter activity log
		/// </summary>
		public String ApplicationName
		{
			get
			{
				return m_strApplicationName;
			}
			set
			{
				m_strApplicationName = value;
			}
		}

		/// <summary>
		/// This property represents the revision and version of the application that  
		/// performed the given action.  This property is typically set implicitly when
		/// the log entry is added to the meter activity log
		/// </summary>
		public String ApplicationVersion
		{
			get
			{
				return m_strApplicationVersion;
			}
			set
			{
				m_strApplicationVersion = value;
			}
		}

		/// <summary>
		/// This property represents the name of the PC or workstation that performed  
		/// the given action.  This property is typically set implicitly when the log
		/// entry is added to the meter activity log
		/// </summary>
		public String MachineName
		{
			get
			{
				return m_strMachineName;
			}
			set
			{
				m_strMachineName = value;
			}
		}

		/// <summary>
		/// This property represents the login name of the user that performed the given 
		/// action. This property is typically set implicitly when the log entry is 
		/// added to the meter activity log
		/// </summary>
		public String UserName
		{
			get
			{
				return m_strUserName;
			}
			set
			{
				m_strUserName = value;
			}
		}

        #endregion

        /// <summary>
        /// This method is used to translate any activity log entry event type
        /// into a string for display purposes
        /// </summary>
        /// <param name="eActivityType" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityTypeEnum">
        /// </param>
        /// <returns>
        /// A text representation of the activity name
        /// </returns>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  04/18/08 MAH		Created
        ///  06/30/09 AF  2.20.10 136279 Changed "CIM" to "CRF"
        ///  08/05/11 AF  2.51.33        Added clear fwdl log
        ///  05/01/12 jrf 2.60.19 TREQ2893 Added HAN move out.
        ///  05/07/12 jrf 2.60.20 TREQ5994 Added disable HAN pricing.
        ///  06/14/13 jrf 2.80.38 TQ???? Added case for updating ert utility id.
        ///  10/07/16 AF  4.70.21 717544 Added case for create core dump file
        ///
        /// Note that the switch statement in this method causes an excessive 
        /// complexity warning.  This is per design and the warning has been disabled.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public static String TranslateActivityName(MeterActivityLogEntry.ActivityTypeEnum eActivityType)
		{
			String strActivityName;

			switch (eActivityType)
			{
				case MeterActivityLogEntry.ActivityTypeEnum.ActivatePendingTable:
					strActivityName = "Activate Pending Table";
					break;
                case MeterActivityLogEntry.ActivityTypeEnum.ClearAllMeterData:
                    strActivityName = "Clear All Meter Data";
                    break;
				case MeterActivityLogEntry.ActivityTypeEnum.ClearPendingTable:
					strActivityName = "Clear Pending Table";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ClearSitescanSnapshots:
					strActivityName = "Clear SiteScan Snapshots";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ClearVQData:
					strActivityName = "Clear Voltage Quality Data";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ClockAdjust:
					strActivityName = "Adjust Clock";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ConnectService:
					strActivityName = "Connect Service";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.CreateEDL:
					strActivityName = "Create EDL File";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.CreateHHF:
					strActivityName = "Create HHF File";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.CreateMIF:
					strActivityName = "Create MIF File";
					break;
                case MeterActivityLogEntry.ActivityTypeEnum.CreateCRF:
                    strActivityName = "Create CRF File";
                    break;
				case MeterActivityLogEntry.ActivityTypeEnum.DecommissionDropNode:
					strActivityName = "Decommission HAN Node";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.DecommissionHAN:
					strActivityName = "Decommission HAN";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.DemandReset:
					strActivityName = "Reset Demand";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.DeregisterHost:
					strActivityName = "Deregister from Host";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.DisconnectService:
					strActivityName = "Disconnect Service";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.EditRegisters:
					strActivityName = "Edit Registers";
					break;
                case MeterActivityLogEntry.ActivityTypeEnum.EnableHANJoining:
                    strActivityName = "Enable HAN Joining";
                    break;
				case MeterActivityLogEntry.ActivityTypeEnum.EnterTestMode:
					strActivityName = "Enter Test Mode";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ExitTestMode:
					strActivityName = "Exit Test Mode";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.FirmwareLoad:
					strActivityName = "Load Firmware";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.Initialization:
					strActivityName = "Initialize Device";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.Logon:
					strActivityName = "Logon";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigModemAnswerDelay:
					strActivityName = "Reconfigure Modem Answer Delay";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigPhonePrefix:
					strActivityName = "Reconfigure Phone Number Prefix";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigureCustomSchedule:
					strActivityName = "Reconfigure Custom Schedule";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigureDST:
					strActivityName = "Reconfigure DST Dates";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigurePasswords:
					strActivityName = "Reconfigure Passwords";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ReconfigureTertiaryPassword:
					strActivityName = "Reconfigure Tertiary Password";
					break;
                case MeterActivityLogEntry.ActivityTypeEnum.ConfigureKYZ:
                    strActivityName = "Configure KYZ Board";
                    break;
				case MeterActivityLogEntry.ActivityTypeEnum.RegisterHost:
					strActivityName = "Register with Host";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ResetActivityStatus:
					strActivityName = "Reset Activity Status";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ResetBillingRegisters:
					strActivityName = "Reset Billing Registers";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ResetInversionTamper:
					strActivityName = "Reset Inversion Tamper Count";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ResetRemovalTamper:
					strActivityName = "Reset Removal Tamper Count";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ResetRFLAN:
					strActivityName = "Reset Comm Module";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.SetCollectionConfig:
					strActivityName = "Set Collection Configuration";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.SetDataDeliveryConfig:
					strActivityName = "Set Data Delivery Configuration";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.TOUReconfiguration:
					strActivityName = "Reconfigure Time Of Use";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.ValidateProgram:
					strActivityName = "Validate Program";
					break;
				case MeterActivityLogEntry.ActivityTypeEnum.WritePendingTOU:
					strActivityName = "Write Pending TOU Table";
					break;
                case MeterActivityLogEntry.ActivityTypeEnum.SiteScanReconfigure:
                    strActivityName = "Reconfigure SiteScan";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.SetHANMultiplier:
                    strActivityName = "Set HAN Multiplier";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.EnableHAN:
                    strActivityName = "Start HAN";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.DisableHAN:
                    strActivityName = "Stop HAN";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.EnableDisconnectSwitch:
                    strActivityName = "Enable Remote Disconnect Switch";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.DisableDisconnectSwitch:
                    strActivityName = "Disable Remote Disconnect Switch";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.DisableSignedAuthorization:
                    strActivityName = "Disable Signed Authorization";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.EnableC1218OverZigBee:
                    strActivityName = "Enable ANSI C12.18 Over ZigBee";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.DisableC1218OverZigBee:
                    strActivityName = "Disable ANSI C12.18 Over ZigBee";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.ClearFWDLEventLog:
                    strActivityName = "Clear Firmware Download Event Log";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.SwitchCommToOpenWay:
                    strActivityName = "Switch Communications to OpenWay Operational Mode";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.SwitchCommToChoiceConnect:
                    strActivityName = "Switch Communications to ChoiceConnect Operational Mode";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.HANMoveOut:
                    strActivityName = "HAN Move Out Operations";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.HANDisablePricing:
                    strActivityName = "Disable HAN Pricing";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.ResetMagneticTamper:
                    strActivityName = "Reset Magnetic Tamper Counts";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.UpdateCellularGateway:
                    strActivityName = "Update Cellular Gateway";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.UpdateERTUtilityID:
                    strActivityName = "Update ERT Utiltiy ID";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.SealCanadian:
                    strActivityName = "Seal Canadian";
                    break;
                case MeterActivityLogEntry.ActivityTypeEnum.UnsealCanadian:
                    strActivityName = "Unseal Canadian";
                    break;
                case ActivityTypeEnum.CreateCoreDump:
                    strActivityName = "Core Dump File Created";
                    break;
				default:
					strActivityName = eActivityType.ToString();
					break;
			}

			return strActivityName;
		}

		/// <summary>
		/// This method is used to translate any activity log event status
		/// into a string for display purposes
		/// </summary>
		/// <param name="eActivityStatus" type="Itron.Metering.Utilities.MeterActivityLogEntry.ActivityStatusEnum">
		/// </param>
		/// <returns>
		/// A text representation of the activity result
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/18/08 MAH		Created
		///
		/// Note that the switch statement in this method causes an excessive 
		/// complexity warning.  This is per design and the warning has been disabled.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public static String TranslateActivityStatus(MeterActivityLogEntry.ActivityStatusEnum eActivityStatus)
		{
			String strActivityStatus;

			switch (eActivityStatus)
			{
                case ActivityStatusEnum.CannotOpenFile:
					strActivityStatus = "Failed - " + "Cannot open file";
					break;
				case ActivityStatusEnum.CannotReadFile:
					strActivityStatus = "Failed - " + "Cannot read file";
					break;
				case ActivityStatusEnum.ClockError:
					strActivityStatus = "Failed - " + "Clock error";
					break;
				case ActivityStatusEnum.DataMissing:
					strActivityStatus = "Failed - " + "Data missing";
					break;
				case ActivityStatusEnum.DeviceSetupConflict:
					strActivityStatus = "Failed - " + "Device setup conflict";
					break;
				case ActivityStatusEnum.DSTNotSupported:
					strActivityStatus = "Failed - " + "DST not supported";
					break;
				case ActivityStatusEnum.DuplicatePassword:
					strActivityStatus = "Failed - " + "Duplicate password";
					break;
				case ActivityStatusEnum.FileIOError:
					strActivityStatus = "Failed - " + "File I/O Error";
					break;
				case ActivityStatusEnum.FileTooLarge:
					strActivityStatus = "Failed - " + "File too large";
					break;
				case ActivityStatusEnum.InDSTChange:
					strActivityStatus = "Failed - " + "In DST change";
					break;
				case ActivityStatusEnum.InsufficentDiskSpace:
					strActivityStatus = "Failed - " + "Insufficient disk space";
					break;
				case ActivityStatusEnum.InvalidConfiguration:
					strActivityStatus = "Failed - " + "Invalid configuration";
					break;
				case ActivityStatusEnum.InvalidPath:
					strActivityStatus = "Failed - " + "Invalid path";
					break;
				case ActivityStatusEnum.InvalidType:
					strActivityStatus = "Failed - " + "Invalid type";
					break;
				case ActivityStatusEnum.LoadVoltageDetected:
					strActivityStatus = "Failed - " + "Load voltage detected";
					break;
				case ActivityStatusEnum.MemoryError:
					strActivityStatus = "Failed - " + "Memory error";
					break;
				case ActivityStatusEnum.MemoryMapError:
					strActivityStatus = "Failed - " + "Memory map error";
					break;
				case ActivityStatusEnum.MeterInTestMode:
					strActivityStatus = "Failed - " + "Meter already in test mode";
					break;
				case ActivityStatusEnum.MeterNotInTestMode:
					strActivityStatus = "Failed - " + "Meter not in test mode";
					break;
				case ActivityStatusEnum.NetworkError:
					strActivityStatus = "Failed - " + "Communications network error";
					break;
				case ActivityStatusEnum.NoBillingScheduleData:
					strActivityStatus = "Failed - " + "No billing schedule data";
					break;
				case ActivityStatusEnum.NoCalendarData:
					strActivityStatus = "Failed - " + "No calendar data";
					break;
				case ActivityStatusEnum.NoChangeRequested:
					strActivityStatus = "Failed - " + "No change requested";
					break;
				case ActivityStatusEnum.NoTOUData:
					strActivityStatus = "Failed - " + "No TOU data found";
					break;
				case ActivityStatusEnum.OffLine:
					strActivityStatus = "Failed - " + "Meter was offline";
					break;
				case ActivityStatusEnum.OperationIncomplete:
					strActivityStatus = "Failed - " + "Operation incomplete";
					break;
                case ActivityStatusEnum.OutOfSyncLessThanHysteresis:
                    strActivityStatus = "Failed - " + "Time out of sync was less than hysteresis";
                    break;
				case ActivityStatusEnum.PendingTablesFull:
					strActivityStatus = "Failed - " + "Pending tables were full";
					break;
				case ActivityStatusEnum.ProtocolError:
					strActivityStatus = "Failed - " + "Protocol error";
					break;
				case ActivityStatusEnum.RetryError:
					strActivityStatus = "Failed - " + "Retry error";
					break;
				case ActivityStatusEnum.RevisionOutOfRange:
					strActivityStatus = "Failed - " + "Revision was out of range";
					break;
				case ActivityStatusEnum.ScheduleExpired:
					strActivityStatus = "Failed - " + "Schedule expired";
					break;
				case ActivityStatusEnum.ScheduleNotSupported:
					strActivityStatus = "Failed - " + "Schedule not supported";
					break;
				case ActivityStatusEnum.ScheduleNotValid:
					strActivityStatus = "Failed - " + "Schedule not valid";
					break;
				case ActivityStatusEnum.SecurityError:
					strActivityStatus = "Failed - " + "Security error";
					break;
				case ActivityStatusEnum.Success:
					strActivityStatus = "Success";
					break;
				case ActivityStatusEnum.TimeCrossesIntervalBoundry:
					strActivityStatus = "Failed - " + "Time crosses interval boundry";
					break;
				case ActivityStatusEnum.Timeout:
					strActivityStatus = "Failed - " + "Meter timed out";
					break;
				case ActivityStatusEnum.TimingConstraint:
					strActivityStatus = "Failed - " + "Timing constraint";
					break;
				case ActivityStatusEnum.TOUNotSupported:
					strActivityStatus = "Failed - " + "TOU not supported";
					break;
				case ActivityStatusEnum.UnrecognizedError:
					strActivityStatus = "Failed - " + "Unrecognized error";
					break;
				case ActivityStatusEnum.UnsupportedOperation:
					strActivityStatus = "Failed - " + "Unsupported operation";
					break;
				case ActivityStatusEnum.VersionOutOfRange:
					strActivityStatus = "Failed - " + "Version out of range";
					break;
				case ActivityStatusEnum.WriteError:
					strActivityStatus = "Failed - " + "Write error";
					break;
				case ActivityStatusEnum.ZigbeeFWTypeInvalid:
					strActivityStatus = "Failed - " + "ZigBee FW type invalid";
					break;
                case ActivityStatusEnum.ReconfigureError:
                    strActivityStatus = "Failed - " + "Reconfigure failed";
                    break;
                case ActivityStatusEnum.NotSynced:
                    strActivityStatus = "Failed - " + "Meter is not synced";
                    break;
                case ActivityStatusEnum.OperationInProgress:
                    strActivityStatus = "Failed - " + "Operation is already in progress";
                    break;
				default:
					strActivityStatus = eActivityStatus.ToString();
					break;
			}

			return strActivityStatus;
		}


		#region Members

		private DateTime m_dtmEventTime;
		private String m_strMeterType;
		private String m_strUnitID;
		private String m_strSerialNumber;
		private ActivityTypeEnum m_eActivityType;
		private ActivityStatusEnum m_eActivityStatus;
		private String m_strSource;
		private String m_strSourceVersion;
		private String m_strResultFile;
		private String m_strApplicationName;
		private String m_strApplicationVersion;
		private String m_strMachineName;
		private String m_strUserName;

		#endregion
	}

}
