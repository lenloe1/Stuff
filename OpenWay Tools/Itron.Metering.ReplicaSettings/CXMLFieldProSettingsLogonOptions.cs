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
//                              Copyright © 2004-2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Field-Pro Settings - Logon Options - Settings Access Class
	/// </summary>	
	//  Revision History
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------------
	//  07/29/04 REM 7.00.15 N/A    Initial Release
	//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
	//  
	public class CXMLFieldProSettingsLogonOptions : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{

		/// <summary>
		/// public const string XML_NODE_OVERRIDE_PASSWORD = "OverridePassword";
		/// </summary>
		protected const string XML_NODE_OVERRIDE_PASSWORD = "OverridePassword";		
		/// <summary>
		/// public const string XML_NODE_OPTICAL_PROBE = "OpticalProbe";
		/// </summary>
		protected const string XML_NODE_OPTICAL_PROBE = "OpticalProbe";		
		/// <summary>
		/// public const string XML_NODE_MAX_BAUD_RATE = "MaxBaudRate";
		/// </summary>
		protected const string XML_NODE_MAX_BAUD_RATE = "MaxBaudRate";
		
		private const OpticalProbeTypes DEFAULT_PROBE = OpticalProbeTypes.GENERIC_1_NO_DTR;
		private const int DEFAULT_MAX_BAUD_RATE = 9600;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//	07/29/04 REM 7.00.15 N/A    Initial Release	
		//		
		public CXMLFieldProSettingsLogonOptions(CXMLSettings XMLSettings)
		{
			m_XMLSettings = XMLSettings;			
		}
		
		/// <summary>
		/// Choice to allow override of meter password.
		/// </summary>
		//  Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04        Created
		//  
		public virtual int OverridePassword
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_OVERRIDE_PASSWORD, true);
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_OVERRIDE_PASSWORD, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}

		/// <summary>
		/// Logon Options - Optical Probe Model
		/// </summary>		
		/// Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  07/29/04 REM 7.00.15 N/A    Initial Release		
		//  01/04/07 mrj 8.00.04		Changes for new Field-Pro
		//
		public virtual OpticalProbeTypes OpticalProbeModel
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_OPTICAL_PROBE, true);
				return (OpticalProbeTypes)m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_OPTICAL_PROBE, true);
				m_XMLSettings.CurrentNodeInt = (int)value;
			}		
		}
				
		/// <summary>
		/// Logon Options - Max Baud Rate
		/// </summary>
		/// Revision History
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------		
		//  01/04/07 mrj 8.00.04		Created for new Field-Pro
		//
		public virtual int MaxBaudRate
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_MAX_BAUD_RATE, true);

				int iBaudRate = m_XMLSettings.CurrentNodeInt;
				if (0 == iBaudRate)
				{
					iBaudRate = DEFAULT_MAX_BAUD_RATE;
				}
				return iBaudRate;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();
				m_XMLSettings.SelectNode(XML_NODE_MAX_BAUD_RATE, true);
				m_XMLSettings.CurrentNodeInt = value;
			}
		}	
	}
}
