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

namespace Itron.Metering.TOU
{
	/// <summary>
	/// Represents a list of events that happen during a year
	/// </summary>
	public class CEventCollection : CollectionBase
	{
		/// <summary>
		/// Creates an instance of the Event Collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CEventCollection()
		{
			
		}//CEventCollection


		/// <summary>
		/// Gets an Event at an index of the EventCollection.  Allows access
		/// to elements like an array
		/// </summary>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// CEvent temp = coll[0];
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CEvent this[int intIndex]
		{
			get 
			{ 
				return (CEvent) InnerList[intIndex]; 
			}
		}//this[]


		/// <summary>
		/// Adds an Event to the end of the EventCollection
		/// </summary>
		/// <param name="objToAdd">
		/// The Event to be added
		/// </param>
		/// <returns>
		/// The zero base index of the Event added
		/// </returns>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// coll.Add(new CEvent());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int Add(CEvent objToAdd)
		{
			return InnerList.Add(objToAdd);
		}//Add


		/// <summary>
		/// Adds an Event to the EventCollection at the given index
		/// </summary>
		/// <param name="intIndex">
		/// Index to insert the Event into in the collection
		/// </param>
		/// <param name="objToAdd">
		/// The Event to be added
		/// </param>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// coll.Insert(3, new CEvent());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public void Insert(int intIndex, CEvent objToAdd)
		{
			InnerList.Insert(intIndex, objToAdd);
		}//Insert

		/// <summary>
		/// Returns the index of the first occurance of the given Event.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="objFind">
		/// Name of the Event to find the index of
		/// </param>
		/// <returns>
		/// The index of the Event
		/// </returns>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// CEvent temp = new CEvent();
		/// coll.Add(temp);
		/// int intIndex = coll.IndexOf(temp);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int IndexOf(CEvent objFind)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CEvent objEvent = (CEvent) InnerList[intIterate];
				if(objEvent.Date.Equals(objFind.Date) && objEvent.Type == objFind.Type)
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error           
            throw new ArgumentException("The given Event is not in the Collection",
                    "CEvent objFind");
            
		}//IndexOf


		/// <summary>
		/// Used to sort the event collection based on the date of the CEvent objects
		/// </summary>
		/// <example>
		/// <code>
		/// CEventCollection coll = new CEventCollection();
		/// CEvent temp = new CEvent(new DateTime(2006,1,1), HOLIDAY, 2);
		/// CEvent temp1 = new CEvent(new DateTime(2005,1,1), HOLIDAY, 2);
		/// CEvent temp2 = new CEvent(new DateTime(2004,1,1), HOLIDAY, 2);
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
			InnerList.Sort();
		}//Sort

	}
}
