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
//                           Copyright © 2004 - 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Replica File XML Settings class
	/// </summary>
	// Revision History
	// MM/DD/YY who Version Issue# Description
	// -------- --- ------- ------ ---------------------------------------------
	// 07/29/04 REM 7.00.15 N/A    Initial Release
	// 11/16/04 REM 7.00.30 1034   Cannot send .Replica files through our e-mail
    // 07/25/07 AF  9.0            Added report specification node
	//
	public class CXMLReplicaFileSettings : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
    {
        #region Constants

        /// <summary>
        /// protected const string XML_NODE_PROGRAMS = "Programs";
        /// </summary>
        protected const string XML_NODE_PROGRAMS = "Programs";
        /// <summary>
        /// protected const string XML_NODE_SETTINGS = "Settings";
        /// </summary>
        protected const string XML_NODE_SETTINGS = "Settings";
        /// <summary>
        /// protected const string XML_NODE_DST = "DST";
        /// </summary>
        protected const string XML_NODE_DST = "DST";
        /// <summary>
        /// protected const string XML_NODE_SECURITY = "Security";
        /// </summary>
        protected const string XML_NODE_SECURITY = "Security";
        /// <summary>
        /// protected const string XML_NODE_FIELD_PRO_SETTINGS = "FieldProSettings";
        /// </summary>
        protected const string XML_NODE_FIELD_PRO_SETTINGS = "FieldProSettings";
        /// <summary>
        /// protected const string XML_NODE_ADDRESS_BOOK = "AddressBook";
        /// </summary>
        protected const string XML_NODE_ADDRESS_BOOK = "AddressBook";
        /// <summary>
        /// protected const string XML_NODE_LAST_REPLICA_DIRECTORY = "ReplicaDirectory";
        /// </summary>
        protected const string XML_NODE_LAST_REPLICA_DIRECTORY = "ReplicaDirectory";
        /// <summary>
        /// protected const string XML_NODE_LAST_FIELD_PRO_DIRECTORY = "FieldProDirectory";
        /// </summary>
        protected const string XML_NODE_LAST_FIELD_PRO_DIRECTORY = "FieldProDirectory";
        /// <summary>
        /// protected const string XML_NODE_PASSWORD_PROTECTED = "PasswordProtected";
        /// </summary>
        protected const string XML_NODE_PASSWORD_PROTECTED = "PasswordProtected";
        /// <summary>
        /// protected const string XML_NODE_REPORT_SPECIFICATION = "ReportSpecification"
        /// </summary>
        protected const string XML_NODE_REPORT_SPECIFICATION = "ReportSpecification";
        /// <summary>
        /// protected const string XML_NODE_FIRMWARE_FILES = "FirmwareFiles"
        /// </summary>
        protected const string XML_NODE_FIRMWARE_FILES = "FirmwareFiles";

        #endregion

        #region Definitions

        //Enumerations
        /// <summary>
        /// CXMLReplicaFileSettings public enumeration
        /// </summary>
        public enum ReplicaFileSettings
        {
            /// <summary>
            /// Programs = 0,
            /// </summary>
            Programs = 0,
            /// <summary>
            /// Settings = 1
            /// </summary>
            Settings,
            /// <summary>
            /// DST = 2
            /// </summary>
            DST,
            /// <summary>
            /// Security = 3
            /// </summary>
            Security,
            /// <summary>
            /// AddressBook = 4
            /// </summary>
            AddressBook,
            /// <summary>
            /// Canadian = 5
            /// </summary>
            Canadian,
            /// <summary>
            /// ReportSpec = 6
            /// </summary>
            ReportSpec,
            /// <summary>
            /// Firmware = 7
            /// </summary>
            Firmware,
            /// <summary>
            /// Number Replica File Settings
            /// </summary>
            NumberReplicaFileSettings, //Last Replica File Settings + 1
            /// <summary>
            /// PASSWORD_PROTECTED = 1000 //This is not a Replica File Component so don't include it
            /// </summary>
            PASSWORD_PROTECTED = 1000 //This is not a Replica File Component so don't include it
        }

        /// <summary>
        /// CXMLReplicaFileSettings program selection enumeration
        /// </summary>
        public enum ProgramSelectionOptions
        {
            /// <summary>
            /// DoNotInclude = 0
            /// </summary>
            DoNotInclude = 0,
            /// <summary>
            /// IncludeAll = 1
            /// </summary>
            IncludeAll,
            /// <summary>
            /// Select = 2
            /// </summary>
            Select
        }

        #endregion

        #region Public Methods

		/// <summary>
		/// Default empty contructor, for use be derived classes.
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  
		//  
		public CXMLReplicaFileSettings()
		{
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFilePath">File Name of XML file to use. If "" is passed in the default is used</param>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public CXMLReplicaFileSettings(string strFilePath)
        {
            m_XMLSettings = new CXMLSettings(DEFAULT_SETTINGS_DIRECTORY + "SystemSettings.xml", "", "SystemSettings");

            if (null != m_XMLSettings)
            {
                m_XMLSettings.XMLFileName = strFilePath;
            }
        }


        #endregion

        #region Public Properties
        /*

        /// <summary>
        /// Choice of whether or not to include Programs in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool Programs
        {
            get
            {
                return GetBool(XML_NODE_PROGRAMS);
            }
            set
            {
                SetBool(XML_NODE_PROGRAMS, value);
            }
        }
         * */
        
        
        
        /// <summary>
        /// Returns the value in the dropdown programs combo box as an int
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/29/08 JMY 9.50.xx N/A    Created
        //

        public virtual ProgramSelectionOptions ProgramsOptions
        {
            get
            {
                return (ProgramSelectionOptions)GetInt(XML_NODE_PROGRAMS);
            }
            set
            {
                int intValue = (int)value;
                SetInt(XML_NODE_PROGRAMS, intValue);
            }
        }       


        /// <summary>
        /// This function returns false if Do Not Include is selected and true if IncludeAll or Selected.  This is
        /// necessary because all the functions that decide what to include in replica files are based off of bool
        /// variables
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/08 JMY 9.50.xx N/A    Created
        //
        public virtual bool Programs
        {
            get
            {
                if (ProgramsOptions == ProgramSelectionOptions.DoNotInclude)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Choice of whether or not to include Settings in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool Settings
        {
            get
            {
                return GetBool(XML_NODE_SETTINGS);
            }
            set
            {
                SetBool(XML_NODE_SETTINGS, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to include DST in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool DST
        {
            get
            {
                return GetBool(XML_NODE_DST);
            }
            set
            {
                SetBool(XML_NODE_DST, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to include Security in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool Security
        {
            get
            {
                return GetBool(XML_NODE_SECURITY);
            }
            set
            {
                SetBool(XML_NODE_SECURITY, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to include Field-Pro Settings in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool FieldProSettings
        {
            get
            {
                return GetBool(XML_NODE_FIELD_PRO_SETTINGS);
            }
            set
            {
                SetBool(XML_NODE_FIELD_PRO_SETTINGS, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to include Address Book in Replica files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool AddressBook
        {
            get
            {
                return GetBool(XML_NODE_ADDRESS_BOOK);
            }
            set
            {
                SetBool(XML_NODE_ADDRESS_BOOK, value);
            }
        }

        /// <summary>
        /// The Canadian option is always included
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/18/07 KRC 8.10.13         Adding Canadian bit to Replica
        ///</pre></remarks>
        public virtual bool Canadian
        {
            get
            {
                // We must always include this
                return true;
            }
            set
            {
            }
        }

        /// <summary>
        /// Choice of whether or not to include Report Specifications in Replica files
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 07/25/07 AF  9.00           Initial Release
        // 
        public virtual bool ReportSpecification
        {
            get
            {
                return GetBool(XML_NODE_REPORT_SPECIFICATION);
            }
            set
            {
                SetBool(XML_NODE_REPORT_SPECIFICATION, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to password protect the Replica file
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///11/16/04 REM 7.00.30 1034   Cannot send .Replica files through our e-mail
        ///</pre></remarks>
        public virtual bool PasswordProtected
        {
            get
            {
                return GetBool(XML_NODE_PASSWORD_PROTECTED);
            }
            set
            {
                SetBool(XML_NODE_PASSWORD_PROTECTED, value);
            }
        }

        /// <summary>
        /// Choice of whether or not to Firmware Files
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///11/16/04 REM 7.00.30 1034   Cannot send .Replica files through our e-mail
        ///</pre></remarks>
        public virtual bool FirmwareFiles
        {
            get
            {
                return GetBool(XML_NODE_FIRMWARE_FILES);
            }
            set
            {
                SetBool(XML_NODE_FIRMWARE_FILES, value);
            }
        }

        /// <summary>
        /// Last path used for Replication
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///08/27/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual string LastReplicaDirectory
        {
            get
            {
                return GetString(XML_NODE_LAST_REPLICA_DIRECTORY);
            }
            set
            {
                SetString(XML_NODE_LAST_REPLICA_DIRECTORY, value);
            }
        }

        /// <summary>
        /// Last path used for Export Field-Pro Settings
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///09/20/04 REM 7.00.19		   Adding support for Export Field-Pro Settings
        ///</pre></remarks>
        public virtual string LastExportFieldProSettingsDirectory
        {
            get
            {
                return GetString(XML_NODE_LAST_FIELD_PRO_DIRECTORY);
            }
            set
            {
                SetString(XML_NODE_LAST_FIELD_PRO_DIRECTORY, value);
            }
        }

        #endregion

	}
}
