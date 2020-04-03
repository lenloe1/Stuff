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
//                              Copyright © 2005
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// DATASTAR Programming Options XML Settings Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///05/13/05 REM 7.20.XX        Adding support for DATASTAR
	///</pre></remarks>
	public class CXMLProgrammingOptionsDATASTAR : Itron.Metering.ReplicaSettings.CXMLDeviceOptions
	{
		/// <summary>
		/// protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		/// </summary>
		protected const string XML_NODE_UNIT_ID_START_POSITION = "UnitIDStartPosition";
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for DATASTAR
		///</pre></remarks>
		public CXMLProgrammingOptionsDATASTAR( CXMLSettings XMLSettings ) : base( CShared.METER_TYPE_DATASTAR, XMLSettings )
		{
		}

		/// <summary>
		/// DATASTAR Unit ID Start Position Programming Option
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///05/13/05 REM 7.20.XX        Adding support for DATASTAR
		///</pre></remarks>
		public virtual int UnitIDStartPosition
		{
			get
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID_START_POSITION, true );
				
				return m_XMLSettings.CurrentNodeInt;
			}
			set
			{
				m_XMLSettings.SetCurrentToAnchor();

				m_XMLSettings.SelectNode( XML_NODE_METER_TYPE, true );
				m_XMLSettings.SelectNode( XML_NODE_UNIT_ID_START_POSITION, true );

				m_XMLSettings.CurrentNodeInt = value;
			}
		}
	}
}
