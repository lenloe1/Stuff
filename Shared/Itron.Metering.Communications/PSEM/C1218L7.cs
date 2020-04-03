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
using Itron.Metering.Communications;

namespace Itron.Metering.Communications.PSEM
{
	
	/// <summary>
	/// C1218L7 supports the ANSI C12.18 application layer 7 communication 
	/// with a device. 
	/// </summary>
	/// <remarks>
	/// C1218L7 internal which implies it is not visible 
	/// outside the assembly.
	/// </remarks>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	internal class CC1218L7 : CANSIL7
	{

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comm">
		/// The communication object that supports 
		/// communication over the physical layer.
		/// </param>
		/// <example>
		/// <code>
		/// Communication comm = new Communication();
		/// comm.OpenPort("COM4:");
		/// C1218L7 c1218l7 = new C1218L7(comm);
		/// </code>
		/// </example>
		public CC1218L7(ICommunications comm):base( new CC1218L2(comm) )
		{
		}
	}
}
