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
//                              Copyright © 2006 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;

namespace Itron.Metering.DST
{
	/// <summary>
	/// Represents a list of CDSTDatePair objects
	/// </summary>
	public class CDSTDatePairCollection : CollectionBase
	{
		/// <summary>
		/// Creates an instance of the CDSTDatePair Collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTDatePairCollection()
		{
			
		}

		/// <summary>
		/// Gets a CDSTDatePair at an index of the CDSTDatePairCollection.  Allows 
		/// access to elements like an array
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// CDSTDatePair temp = coll[0];
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public CDSTDatePair this[int intIndex]
		{
			get 
			{ 
				return (CDSTDatePair) InnerList[intIndex]; 
			}
		}

		/// <summary>
		/// Adds a CDSTDatePair to the end of the CDSTDatePairCollection
		/// </summary>
		/// <param name="objToAdd">
		/// The CDSTDatePair to be added
		/// </param>
		/// <returns>
		/// The zero base index of the CDSTDatePair added
		/// </returns>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// coll.Add(new CDSTDatePair(new DateTime(2006,4,4), new DateTime(2006,10,29)));
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class   
		public int Add(CDSTDatePair objToAdd)
		{
			return InnerList.Add(objToAdd);

		}

		/// <summary>
		/// Adds a CDSTDatePair to the CDSTDatePairCollection at the given index
		/// </summary>
		/// <param name="intIndex">
		/// Index to insert the CDSTDatePair into in the collection
		/// </param>
		/// <param name="objToAdd">
		/// The CDSTDatePair to be added
		/// </param>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// coll.Insert(3, new CDSTDatePair(new DateTime(2006,4,4), new DateTime(2006,10,29)));
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public void Insert(int intIndex, CDSTDatePair objToAdd)
		{
			InnerList.Insert(intIndex, objToAdd);
		}

		/// <summary>
		/// Searches the collection for the given CDSTDatePair and returns the index.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="intYear">
		/// The year to search for
		/// </param>
		/// <returns>
		/// The index of the CDSTDatePair
		/// </returns>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// CDSTDatePair temp = new CDSTDatePair(new DateTime(2006,4,4), new DateTime(2006,10,29));
		/// coll.Add(temp);
		/// int intIndex = coll.SearchYear(temp.Year);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public int FindYear(int intYear)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CDSTDatePair objYear = (CDSTDatePair) InnerList[intIterate];
				if(objYear.ToDate.Year == intYear)
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given CDSTDatePair is not in the Collection",
				"int intYear");
		}

		/// <summary>
		/// Used to sort the CDSTDatePair collection based on the years of the 
		/// CDSTDatePair objects
		/// </summary>
		/// <example>
		/// <code>
		/// CDSTDatePairCollection coll = new CDSTDatePairCollection();
		/// CDSTDatePair temp = new CDSTDatePair(new DateTime(2006,4,4), new DateTime(2006,10,29));
		/// CDSTDatePair temp1 = new CDSTDatePair(new DateTime(2007,4,4), new DateTime(2007,10,29));
		/// coll.Add(temp);
		/// coll.Add(temp1);
		/// coll.Sort();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 03/03/06 rrr N/A	 N/A	Creation of class  
		public void Sort()
		{
			InnerList.Sort();
		}
	}
}
