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
//                              Copyright © 2004 - 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;
using System.Windows.Forms;
using System.Globalization;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Field-Pro XML Settings Base Class
	/// </summary>	
	//  Revision History
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  07/29/04 REM 7.00.15 N/A    Initial Release
	//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
	//  
	public class CXMLSettingsFieldPro : Itron.Metering.ReplicaSettings.CXMLSettings
	{
		/// <summary>
		/// public const string SETTING_OVERRIDE_PASSWORD = "OverridePassword";
		/// </summary>
		public const string SETTING_OVERRIDE_PASSWORD = "OverridePassword";
		/// <summary>
		/// public const string SETTING_COMM_PORT = "PortNumber";
		/// </summary>
		public const string SETTING_COMM_PORT = "PortNumber";		
		/// <summary>
		/// public const string SETTING_OPTICAL_PROBE = "OpticalProbe";
		/// </summary>
		public const string SETTING_OPTICAL_PROBE = "OpticalProbe";
		/// <summary>
		/// public const string SETTING_DTR = "DTR";
		/// </summary>
		public const string SETTING_DTR = "DTR";
		/// <summary>
		/// public const string SETTING_RTS = "RTS";
		/// </summary>
		public const string SETTING_RTS = "RTS";
		/// <summary>
		/// public const string SETTING_MAX_BAUD_RATE = "MaxBaudRate";
		/// </summary>
		public const string SETTING_MAX_BAUD_RATE = "MaxBaudRate";


		/// <summary>
		/// public const string SETTING_PASSWORD = "Password";
		/// </summary>
		public const string SETTING_PASSWORD = "Password";
		/// <summary>
		/// public const string SETTING_INITIALIZE = "Initialize";
		/// </summary>
		public const string SETTING_INITIALIZE = "Initialize";
		/// <summary>
		/// public const string SETTING_RESET_BILLING_REGISTERS = "ResetBillingRegisters";
		/// </summary>
		public const string SETTING_RESET_BILLING_REGISTERS = "ResetBillingRegisters";
		/// <summary>
		/// public const string SETTING_RESET_DEMAND_REGISTERS = "ResetDemandRegisters";
		/// </summary>
		public const string SETTING_RESET_DEMAND_REGISTERS = "ResetDemandRegisters";
		/// <summary>
		/// public const string SETTING_RESET_ACTIVITY_STATUS = "ResetActivityStatus";
		/// </summary>
		public const string SETTING_RESET_ACTIVITY_STATUS = "ResetActivityStatus";
		/// <summary>
		/// public const string SETTING_EDIT_REGISTERS = "EditRegisters";
		/// </summary>
		public const string SETTING_EDIT_REGISTERS = "EditRegisters";
		/// <summary>
		/// public const string SETTING_ADJUST_DEVICE_CLOCK = "AdjustDeviceClock";
		/// </summary>
		public const string SETTING_ADJUST_DEVICE_CLOCK = "AdjustDeviceClock";
		/// <summary>
		/// public const string SETTING_MIF = "MIF";
		/// </summary>
		public const string SETTING_MIF = "MIF";
		/// <summary>
		/// public const string SETTING_MV90 = "MV90";
		/// </summary>
		public const string SETTING_MV90 = "MV90";
		/// <summary>
		/// public const string SETTING_MIF_AUTO = "MIFAuto";
		/// </summary>
		public const string SETTING_MIF_AUTO = "MIFAuto";
		/// <summary>
		/// public const string SETTING_MV90_AUTO = "MV90Auto";
		/// </summary>
		public const string SETTING_MV90_AUTO = "MV90Auto";
		/// <summary>
		/// public const string SETTING_CLEAR_ACTIVITY_LOG = "ClearActivityLog";
		/// </summary>
		public const string SETTING_CLEAR_ACTIVITY_LOG = "ClearActivityLog";
		/// <summary>
		/// public const string SETTING_CLEAR_DATA_EXPORT_FILES = "ClearDataExportFiles";
		/// </summary>
		public const string SETTING_CLEAR_DATA_EXPORT_FILES = "ClearDataExportFiles";
		/// <summary>
		/// public const string SETTING_UPLOAD_READING = "UploadReading";
		/// </summary>
		public const string SETTING_UPLOAD_READING = "UploadReading";
		/// <summary>
		/// public const string SETTING_DOWNLOAD_PROGRAM = "DownloadProgram";
		/// </summary>
		public const string SETTING_DOWNLOAD_PROGRAM = "DownloadProgram";
		/// <summary>
		/// public const string SETTING_UPLOAD_ACTIVITY_LOG = "UploadActivityLog";
		/// </summary>
		public const string SETTING_UPLOAD_ACTIVITY_LOG = "UploadActivityLog";
		/// <summary>
		/// public const string SETTING_PROGRAM_DIRECTORY = "ProgramDirectory";
		/// </summary>
		public const string SETTING_PROGRAM_DIRECTORY = "ProgramDirectory";
		/// <summary>
		/// public const string SETTING_SETTINGS_DIRECTORY = "SettingsDirectory";
		/// </summary>
		public const string SETTING_SETTINGS_DIRECTORY = "SettingsDirectory";
		/// <summary>
		/// public const string SETTING_DATA_DIRECTORY = "DataDirectory";
		/// </summary>
		public const string SETTING_DATA_DIRECTORY = "DataDirectory";
		/// <summary>
		/// public const string SETTING_RESET_VOLTAGE_QUALITY_LOG = "ResetVoltageQuality";
		/// </summary>
		public const string SETTING_RESET_VOLTAGE_QUALITY_LOG = "ResetVoltageQuality";
		/// <summary>
		/// public const string SETTING_RESET_SITESCAN_SNAPSHOTS = "ResetSiteScanSnapShots";
		/// </summary>
		public const string SETTING_RESET_SITESCAN_SNAPSHOTS = "ResetSiteScanSnapShots";
		/// <summary>
		/// public const string SETTING_LAST_SYNCH = "LastSynch";
		/// </summary>
		public const string SETTING_LAST_SYNCH = "LastSynch";
		/// <summary>
		/// public const string SETTING_LOGOFF_AFTER_INITIALIZATION = "LogoffAfterInitialization";
		/// </summary>
		public const string SETTING_LOGOFF_AFTER_INITIALIZATION = "LogoffAfterInitialization";
		/// <summary>
		/// public const string SETTING_RECONFIG_SCHED = "ReconfigureCustomSchedule";
		/// </summary>
		public const string SETTING_RECONFIG_SCHED = "ReconfigureCustomSchedule";
		/// <summary>
		/// public const string SETTING_SCHED_IN_USERDATA2 = "SetCustomSchedNameInUserData2";
		/// </summary>
		public const string SETTING_SCHED_IN_USERDATA2 = "SetCustomSchedNameInUserData2";
		/// <summary>
		/// public const string SETTING_RECONFIG_PW = "ReconfigureTertiarySecurityCode";
		/// </summary>
		public const string SETTING_RECONFIG_PW = "ReconfigureTertiarySecurityCode";
		/// <summary>
		/// public const string SETTING_TRANSFER_SETTINGS = "TransferSettings";
		/// </summary>
		public const string SETTING_TRANSFER_SETTINGS = "TransferSettings";
		/// <summary>
		/// public const string SETTING_RECONFIG_PREFIX = "ReconfigurePhoneHomePrefix";
		/// </summary>
		public const string SETTING_RECONFIG_PREFIX = "ReconfigurePhoneHomePrefix";
		/// <summary>
		/// public const string SETTING_RECONFIG_ANSWER_DELAY = "ReconfigureModemAnswerDelay";
		/// </summary>
		public const string SETTING_RECONFIG_ANSWER_DELAY = "ReconfigureModemAnswerDelay";
		/// <summary>
		/// public const string SETTING_CHANGE_OUT_CREATE_DATA_FILE = "ChangeOutCreateDataFile";
		/// </summary>
		public const string SETTING_CHANGE_OUT_CREATE_DATA_FILE = "ChangeOutCreateDataFile";
		/// <summary>
		/// public const string SETTING_CHANGE_OUT_COPY_REGISTERS = "ChangeOutCopyRegisters";
		/// </summary>
		public const string SETTING_CHANGE_OUT_COPY_REGISTERS = "ChangeOutCopyRegisters";
		/// <summary>
		/// public const string SETTING_CHANGE_OUT_EDIT_REGISTERS = "ChangeOutEditRegisters";
		/// </summary>
		public const string SETTING_CHANGE_OUT_EDIT_REGISTERS = "ChangeOutEditRegisters";
		/// <summary>
		/// public const string SETTING_CHANGE_OUT_DEFAULT_FILE_LOCATION = "ChangeOutDefaultFileLocation";
		/// </summary>
		public const string SETTING_CHANGE_OUT_DEFAULT_FILE_LOCATION = "ChangeOutDefaultFileLocation";

		/// <summary>
		/// public const string SETTING_METER_TYPE_CENTRON = "CENTRON";
		/// </summary>
		public const string SETTING_METER_TYPE_CENTRON = "CENTRON";
		/// <summary>
		/// public const string SETTING_METER_TYPE_Q1000 = "Q1000";
		/// </summary>
		public const string SETTING_METER_TYPE_Q1000 = "Q1000";
		/// <summary>
		/// public const string SETTING_METER_TYPE_SENTINEL = "SENTINEL";
		/// </summary>
		public const string SETTING_METER_TYPE_SENTINEL = "SENTINEL";
		/// <summary>
		/// public const string SETTING_METER_TYPE_CENTRON_C1219 = "CENTRONC1219";
		/// </summary>
		public const string SETTING_METER_TYPE_CENTRON_C1219 = "CENTRONC1219";
		/// <summary>
		/// public const string SETTING_METER_TYPE_CENTRON_VI = "CENTRONVI";
		/// </summary>
		public const string SETTING_METER_TYPE_CENTRON_VI = "CENTRONVI";

		/// <summary>
		/// protected const string APPLICATION_CONSTANT = "FieldPro";
		/// </summary>
		protected const string APPLICATION_CONSTANT = "FieldPro";
		/// <summary>
		/// public const string XML_ENABLED = "Enabled";
		/// </summary>
		public const string XML_ENABLED = "Enabled";
		/// <summary>
		/// public const string XML_DISABLED = "Disabled";
		/// </summary>
		public const string XML_DISABLED = "Disabled";
		/// <summary>
		/// public const string XML_PROMPT = "Prompt";
		/// </summary>
		public const string XML_PROMPT = "Prompt"; //REM 01/24/05: New node value for Reset Billing Registers

		/// <summary>
		/// const stringructor
		/// </summary>
		/// <param name="strFile">Default File Name for .xml file</param>
		/// <param name="strMain">Main Node in the XML file for the settings</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLSettingsFieldPro( string strFile, string strMain ) :
			base( strFile, Encrypt( APPLICATION_CONSTANT ), "" )
		{
		}

		/// <summary>
		/// Encrypts the passed in string
		/// </summary>
		/// <param name="strString"></param>
		/// <returns>Encrypted string</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		static protected string Encrypt( string strString )
		{
			string strReturn = "A";
			string strTemp = "";
			
			for( int intIndex = 0; intIndex < strString.Length; intIndex++ )
			{
                strTemp = (255 - Convert.ToChar(strString.Substring(intIndex, 1), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
				
				for( int int0 = strTemp.Length; int0 < 3; int0++ )
				{
					strTemp = "0" + strTemp;
				}

				strReturn += strTemp;
			}

			return strReturn;
		}

		/// <summary>
		/// Decrypts the passed in string
		/// </summary>
		/// <param name="strString">String to Encrypt</param>
		/// <returns>Decrypted string</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual string Decrypt( string strString )
		{
			string strReturn = "";
			string strTempString = "";
			string strCharacter = "";
    
			if( 4 <= strString.Length )
			{
				strTempString = strString.Substring( 1 );
				while( 3 <= strTempString.Length )
				{
					strCharacter = strTempString.Substring( 0, 3 );
                    strReturn += Convert.ToChar((255 - Convert.ToInt32(strCharacter, CultureInfo.InvariantCulture))).ToString(CultureInfo.InvariantCulture);
					strTempString = strTempString.Substring( 3 );
				}
			}

			return strReturn;
		}

		/// <summary>
		/// Returns the Current Nodes innertext decrypted
		/// </summary>
		///<remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public override string CurrentNodeString
		{
			get
			{
				string strReturn = "";

				if( null != m_xmlnodeCurrent )
				{
					strReturn = Decrypt( m_xmlnodeCurrent.InnerText );
				}

				return strReturn;
			}
			set
			{
				if( null != m_xmlnodeCurrent )
				{
					m_xmlnodeCurrent.InnerText = Encrypt( value );
				}
			}
		}

		/// <summary>
		/// Sets the current node member variable to the Setting Node under the Group node passed in
		/// </summary>
		/// <param name="strGroup">Field-Pro Group to select setting for</param>
		/// <param name="strSetting">Setting node to select</param>
		/// <param name="blnCreate">Whether or not to create the node if it does not exist</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual void SelectSettingNode( string strGroup, string strSetting, bool blnCreate )
		{
			SetCurrentToAnchor();

			SelectNode( strGroup, blnCreate );
			SelectNode( strSetting, blnCreate );
		}

		/// <summary>
		/// Sets the current node to the childnode of the current node named strChildName. If it does not
		/// exist and bCreate is set to true then it will be created.
		/// </summary>
		/// <param name="strChildNode">Name of child node to select</param>
		/// <param name="bCreate">bool</param>
		/// <returns></returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public override bool SelectNode( string strChildNode, bool bCreate )
		{
			bool bReturn = false;
			XmlNode xmlnodeChild;

			if( ( null != m_xmlnodeCurrent ) && ( 0 < strChildNode.Length ) )
			{
				xmlnodeChild = m_xmlnodeCurrent.SelectSingleNode( Encrypt( strChildNode ) );

				if( ( null == xmlnodeChild ) && ( false != bCreate ) )
				{
					xmlnodeChild = CreateElement( Encrypt( strChildNode ) );
					
					if( null != xmlnodeChild )
					{
						xmlnodeChild = m_xmlnodeCurrent.AppendChild( xmlnodeChild );
					}
				}

				if( null != xmlnodeChild )
				{
					m_xmlnodeCurrent = xmlnodeChild;
					bReturn = true;
				}
			}
			
			return bReturn;
		}

        /// <summary>
        /// Sets or gets whether or not the current setting is enabled
        /// </summary>
        ///<remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        public virtual bool CurrentNodeEnabled
		{
			get
			{
				bool blnReturn = false;

				if( 0 == XML_ENABLED.CompareTo( CurrentNodeString ) )
				{
					blnReturn = true;
				}

				return blnReturn;
			}
			set
			{
				if( false == value )
				{
					CurrentNodeString = XML_DISABLED;
				}
				else
				{
					CurrentNodeString = XML_ENABLED;
				}
			}
		}

        /// <summary>
        /// Returns of Sets a int value into the current node
        /// </summary>
        ///<remarks ><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///09/23/04 REM 7.00.20 N/A    Initial Release
        ///</pre></remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        public override int CurrentNodeInt
		{
			get
			{
				int intReturn = 0;
				
				try
				{
					if( null != m_xmlnodeCurrent )
					{
						if( 0 != m_xmlnodeCurrent.InnerText.CompareTo( "" ) )
						{
							intReturn = Convert.ToInt32( Decrypt( m_xmlnodeCurrent.InnerText ), 10 );
						}
					}
				}
				catch
				{
					intReturn = 0;
				}
				return intReturn;
			}
			set
			{
				if( null != m_xmlnodeCurrent )
				{
                    m_xmlnodeCurrent.InnerText = Encrypt(value.ToString(CultureInfo.InvariantCulture));
				}
			}
		}
	}
}
