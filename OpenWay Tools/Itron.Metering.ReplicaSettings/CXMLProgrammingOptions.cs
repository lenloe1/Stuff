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
//                              Copyright © 2004 - 2005
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
	///05/13/05 REM 7.20.XX        Adding support for FULCRUM, DATASTAR, QUANTUM, and 200 Series
	///</pre></remarks>
	public class CXMLProgrammingOptions : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//REM 02/27/05: Replaced constants with CShared class
		//protected const string METER_TYPE_CENTRON = "CENTRON";
		//protected const string METER_TYPE_Q1000 = "Q1000";
		//protected const string METER_TYPE_SENTINEL = "SENTINEL";
		//protected const string METER_TYPE_VECTRON = "VECTRON";

		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLCENTRONProgrammingOptions m_CENTRONProgrammingOptions;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsCENTRONMono m_ProgrammingOptionsCENTRONMono;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsCENTRONPoly m_ProgrammingOptionsCENTRONPoly;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLQ1000ProgrammingOptions m_Q1000ProgrammingOptions;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLSENTINELProgrammingOptions m_SENTINELProgrammingOptions;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLVECTRONProgrammingOptions m_VECTRONProgrammingOptions;
		//REM 05/13/05: Adding support for FULCRUM, QUANTUM, 200 Series, and DATASTAR
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsFULCRUM m_ProgrammingOptionsFULCRUM;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsQUANTUM m_ProgrammingOptionsQUANTUM;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsDATASTAR m_ProgrammingOptionsDATASTAR;
		/// <summary>
		/// CXMLProgrammingOptions protected member variable
		/// </summary>
		protected CXMLProgrammingOptionsDMTMTR200 m_ProgrammingOptionsDMTMTR200;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFilePath">File path to use for the .xml file. If "" is passed in the default will be used</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM, QUANTUM, 200 Series, and DATASTAR
		///</pre></remarks>
		public CXMLProgrammingOptions( string strFilePath )
		{
			m_XMLSettings = new CXMLSettings( DEFAULT_SETTINGS_DIRECTORY + "Programming Options.xml", "", "ProgrammingOptions" );

			if( null != m_XMLSettings )
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}

			m_CENTRONProgrammingOptions = new CXMLCENTRONProgrammingOptions( m_XMLSettings );
			m_ProgrammingOptionsCENTRONMono = new CXMLProgrammingOptionsCENTRONMono( m_XMLSettings );
			m_ProgrammingOptionsCENTRONPoly = new CXMLProgrammingOptionsCENTRONPoly( m_XMLSettings );
			m_Q1000ProgrammingOptions = new CXMLQ1000ProgrammingOptions( m_XMLSettings );
			m_SENTINELProgrammingOptions = new CXMLSENTINELProgrammingOptions( m_XMLSettings );
			m_VECTRONProgrammingOptions = new CXMLVECTRONProgrammingOptions( m_XMLSettings );
			//REM 05/13/05: Adding support for FULCRUM, QUANTUM, 200 Series, and DATASTAR
			m_ProgrammingOptionsFULCRUM = new CXMLProgrammingOptionsFULCRUM( m_XMLSettings );
			m_ProgrammingOptionsQUANTUM = new CXMLProgrammingOptionsQUANTUM( m_XMLSettings );
			m_ProgrammingOptionsDATASTAR = new CXMLProgrammingOptionsDATASTAR( m_XMLSettings );
			m_ProgrammingOptionsDMTMTR200 = new CXMLProgrammingOptionsDMTMTR200( m_XMLSettings );
		}
		
		/// <summary>
		/// Returns the CENTRON Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLCENTRONProgrammingOptions CENTRON
		{
			get
			{
				return m_CENTRONProgrammingOptions;
			}
		}
		
		/// <summary>
		/// Returns the CENTRON MonoProgramming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsCENTRONMono CENTRONMono
		{
			get
			{
				return m_ProgrammingOptionsCENTRONMono;
			}
		}

		/// <summary>
		/// Returns the CENTRON Poly Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///02/17/05 REM 7.20.XX        Adding support for CENTRON Mono and Poly
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsCENTRONPoly CENTRONPoly
		{
			get
			{
				return m_ProgrammingOptionsCENTRONPoly;
			}
		}

		/// <summary>
		/// Returns the Q1000 Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLQ1000ProgrammingOptions Q1000
		{
			get
			{
				return m_Q1000ProgrammingOptions;
			}
		}

		/// <summary>
		/// Returns the SENTINEL Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLSENTINELProgrammingOptions SENTINEL
		{
			get
			{
				return m_SENTINELProgrammingOptions;
			}
		}

		/// <summary>
		/// Returns the VECTRON Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual CXMLVECTRONProgrammingOptions VECTRON
		{
			get
			{
				return m_VECTRONProgrammingOptions;
			}
		}

		/// <summary>
		/// Returns the FULCRUM Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for FULCRUM
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsFULCRUM FULCRUM
		{
			get
			{
				return m_ProgrammingOptionsFULCRUM;
			}
		}

		/// <summary>
		/// Returns the QUANTUM Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for QUANTUM
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsQUANTUM QUANTUM
		{
			get
			{
				return m_ProgrammingOptionsQUANTUM;
			}
		}

		/// <summary>
		/// Returns the DATASTAR Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for DATASTAR
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsDATASTAR DATASTAR
		{
			get
			{
				return m_ProgrammingOptionsDATASTAR;
			}
		}

		/// <summary>
		/// Returns the D/MT/MTR 200 Programming Options object
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for D/MT/MTR 200
		///</pre></remarks>
		public virtual CXMLProgrammingOptionsDMTMTR200 DMTMTR200
		{
			get
			{
				return m_ProgrammingOptionsDMTMTR200;
			}
		}
	}
}
