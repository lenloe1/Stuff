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
//                              Copyright © 2004 - 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Device Security Codes XML Setting class
	/// </summary>
	//  Revision History
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  07/29/04 REM 7.00.15 N/A    Initial Release
	//  05/13/05 REM 7.20.XX        Adding support for 16-bit devices.
    //  01/10/07 mrj 8.00.05		Added support for OpenWay Centron (AMI)
	//  
    public class CXMLDeviceSecurityCodes : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//protected const string METER_TYPE_CENTRON = "CENTRON";
		//protected const string METER_TYPE_Q1000 = "Q1000";
		//protected const string METER_TYPE_SENTINEL = "SENTINEL";
		//protected const string METER_TYPE_VECTRON = "VECTRON";

		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesLimited m_SecurityCodesCENTRONMono;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesLimited m_SecurityCodesCENTRONPoly;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesLimited m_SENTINELSecurityCodes;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLQ1000SecurityCodes m_Q1000SecurityCodes;
		//REM 05/13/05: CXMLSecurityCodes only contains upto Secondary now
		//protected CXMLSecurityCodes m_CENTRONSecurityCodes;
		//protected CXMLSecurityCodes m_VECTRONSecurityCodes;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesTertiary m_CENTRONSecurityCodes;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesTertiary m_VECTRONSecurityCodes;
		//REM 05/13/05: Adding support for QUANTUM, FULCRUM, D/MT/MTR 200, and DATASTAR
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesTertiary m_SecurityCodesFULCRUM;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodes m_SecurityCodesDATASTAR;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodes m_SecurityCodesDMTMTR200;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodes m_SecurityCodesQUANTUM;
		/// <summary>
		/// CXMLDeviceSecurityCodes member variable
		/// </summary>
		protected CXMLSecurityCodesLimited m_SecurityCodesCENTRONOpenWay;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File Path to use for Xml file. If "" is passed in the default path is used</param>				
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/29/04 REM 7.00.15 N/A    Initial Release
		//  05/13/05 REM 7.20.XX        Adding support for 16-bit devices. 		
        //  01/10/07 mrj 8.00.05		Added support for OpenWay CENTRON (AMI)
        //  11/17/10 jrf 2.45.13        Added support for Centron II (C12.19)
		//  
		public CXMLDeviceSecurityCodes( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + "Security Codes.xml", "", "Security" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}

			m_SecurityCodesCENTRONMono = new CXMLSecurityCodesLimited( CShared.METER_TYPE_CENTRON_MONO, m_XMLSettings );
			m_SecurityCodesCENTRONPoly = new CXMLSecurityCodesLimited( CShared.METER_TYPE_CENTRON_POLY, m_XMLSettings );
			m_SENTINELSecurityCodes = new CXMLSecurityCodesLimited( CShared.METER_TYPE_SENTINEL, m_XMLSettings );
			m_Q1000SecurityCodes = new CXMLQ1000SecurityCodes( m_XMLSettings );
			//REM 05/13/05: SecurtiyCodes 
			//m_CENTRONSecurityCodes = new CXMLSecurityCodes( CShared.METER_TYPE_CENTRON, m_XMLSettings );
			//m_VECTRONSecurityCodes = new CXMLSecurityCodes( CShared.METER_TYPE_VECTRON, m_XMLSettings );
			m_CENTRONSecurityCodes = new CXMLSecurityCodesTertiary( CShared.METER_TYPE_CENTRON, m_XMLSettings );
			m_VECTRONSecurityCodes = new CXMLSecurityCodesTertiary( CShared.METER_TYPE_VECTRON, m_XMLSettings );
			//REM 05/13/05: Adding support for FULCRUM, QUANTUM, DATASTAR, and D/MT/MTR 200
			m_SecurityCodesFULCRUM = new CXMLSecurityCodesTertiary( CShared.METER_TYPE_FULCRUM, m_XMLSettings );
			m_SecurityCodesDATASTAR = new CXMLSecurityCodes( CShared.METER_TYPE_DATASTAR, m_XMLSettings );
			m_SecurityCodesQUANTUM = new CXMLSecurityCodes( CShared.METER_TYPE_QUANTUM, m_XMLSettings );
			m_SecurityCodesDMTMTR200 = new CXMLSecurityCodes( CShared.METER_TYPE_DMTMTR200, m_XMLSettings );
			m_SecurityCodesCENTRONOpenWay = new CXMLSecurityCodesLimited(CShared.METER_TYPE_CENTRON_OPENWAY, m_XMLSettings);
		}
		
		/// <summary>
		/// Returns the CENTRON Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///05/13/05 REM 7.20.XX        Security Codes only contain Secondary now
		///</pre></remarks>
		//public virtual CXMLSecurityCodes CENTRON
		public virtual CXMLSecurityCodesTertiary CENTRON
		{
			get
			{
				return m_CENTRONSecurityCodes;
			}
		}

		/// <summary>
		/// Returns the CENTRON Mono Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX		   Adding support for CENTRON Mono and Poly
		///</pre></remarks>
		public virtual CXMLSecurityCodesLimited CENTRONMono
		{
			get
			{
				return m_SecurityCodesCENTRONMono;
			}
		}

		/// <summary>
		/// Returns the CENTRON Poly Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX		   Adding support for CENTRON Mono and Poly
		///</pre></remarks>
		public virtual CXMLSecurityCodesLimited CENTRONPoly
		{
			get
			{
				return m_SecurityCodesCENTRONPoly;
			}
		}

		/// <summary>
		/// Returns the Q1000 Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLQ1000SecurityCodes Q1000
		{
			get
			{
				return m_Q1000SecurityCodes;
			}
		}

		/// <summary>
		/// Returns the SENTINEL Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLSecurityCodesLimited SENTINEL
		{
			get
			{
				return m_SENTINELSecurityCodes;
			}
		}

		/// <summary>
		/// Returns the VECTRON Security Codes Object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///05/13/05 REM 7.20.XX        Security Codes only contain Secondary now
		///</pre></remarks>
		//public virtual CXMLSecurityCodes VECTRON
		public virtual CXMLSecurityCodesTertiary VECTRON
		{
			get
			{
				return m_VECTRONSecurityCodes;
			}
		}
		
		/// <summary>
		/// Returns the Security Codes Object FULCRUM
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public virtual CXMLSecurityCodesTertiary FULCRUM
		{
			get
			{
				return m_SecurityCodesFULCRUM;
			}
		}

		/// <summary>
		/// Returns the Security Codes Object QUANTUM
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
		///</pre></remarks>
		public virtual CXMLSecurityCodes QUANTUM
		{
			get
			{
			return m_SecurityCodesQUANTUM;
			}
		}
		/// <summary>
		/// Returns the Security Codes Object DATASTAR
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for DATASTAR
		///</pre></remarks>
		public virtual CXMLSecurityCodes DATASTAR
		{
			get
			{
				return m_SecurityCodesDATASTAR;
			}
		}
		/// <summary>
		/// Returns the Security Codes Object D/MT/MTR 200
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual CXMLSecurityCodes DMTMTR200
		{
			get
			{
				return m_SecurityCodesDMTMTR200;
			}
		}

		/// <summary>
        /// Returns the OpenWay CENTRON Security Codes Object
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/10/07 mrj 8.00.05		Created
		//
		public virtual CXMLSecurityCodesLimited CENTRONOpenWay
		{
			get
			{
				return m_SecurityCodesCENTRONOpenWay;
			}
		}
	}
}
