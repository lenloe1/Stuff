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
//                              Copyright © 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// Represents a list of switch points in a pattern
	/// </summary>
	public class CSwitchPointCollection : Collection<CSwitchPoint>
	{
		/// <summary>
		/// Creates an instance of the switch point collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPointCollection coll = new CSwitchPointCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CSwitchPointCollection()
		{
			
		}//CSwitchPointCollection


		/// <summary>
		/// Used to sort the switch point collection based on the start time
		/// of the CSwitchPoint objects
		/// </summary>
		/// <example>
		/// <code>
		/// CSwitchPointCollection coll = new CSwitchPointCollection();
		/// CSwitchPoint temp = new CSwitchPoint(120,280,2,RATE);
		/// CSwitchPoint temp1 = new CSwitchPoint(0,50,2,RATE);
		/// CSwitchPoint temp2 = new CSwitchPoint(50,120,2,RATE);
		/// coll.Add(temp);
		/// coll.Add(temp1);
		/// coll.Add(temp2);
		/// coll.Sort();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public void Sort()
		{
            ((List<CSwitchPoint>)Items).Sort();
		}//Sort

	}
}
