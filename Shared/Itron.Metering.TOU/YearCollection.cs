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
	/// Reprsents a list of years in a TOU Schedule
	/// </summary>
	public class CYearCollection : CollectionBase
	{
		/// <summary>
		/// Creates an instance of the Year Collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CYearCollection()
		{
			
		}//CYearCollection


		/// <summary>
		/// Gets a Year at an index of the YearCollection.  Allows access
		/// to elements like an array
		/// </summary>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// CYear temp = coll[0];
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CYear this[int intIndex]
		{
			get 
			{ 
				return (CYear) InnerList[intIndex]; 
			}
		}//this[]


		/// <summary>
		/// Adds a Year to the end of the YearCollection
		/// </summary>
		/// <param name="objToAdd">
		/// The Year to be added
		/// </param>
		/// <returns>
		/// The zero base index of the Year added
		/// </returns>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// coll.Add(new CYear(2004, new CEventCollection()));
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int Add(CYear objToAdd)
		{
			return InnerList.Add(objToAdd);

		}//Add


		/// <summary>
		/// Adds a Year to the YearCollection at the given index
		/// </summary>
		/// <param name="intIndex">
		/// Index to insert the Year into in the collection
		/// </param>
		/// <param name="objToAdd">
		/// The Year to be added
		/// </param>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// coll.Insert(3, new CYear(2004, new CEventCollection()));
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public void Insert(int intIndex, CYear objToAdd)
		{
			InnerList.Insert(intIndex, objToAdd);
		}//Insert


		/// <summary>
		/// Returns the index of the first occurance of the given Year.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="objFind">
		/// Name of the Year to find the index of
		/// </param>
		/// <returns>
		/// The index of the Year
		/// </returns>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// CYear temp = new CYear(2004, new CEventCollection());
		/// coll.Add(temp);
		/// int intIndex = coll.IndexOf(temp);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int IndexOf(CYear objFind)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CYear objYear = (CYear) InnerList[intIterate];
				if(objYear.Year == objFind.Year)
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given Year is not in the Collection",
				"CYear objFind");
			
		}//IndexOf


		/// <summary>
		/// Searches the collection for the given year and returns the index.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="intYear">
		/// The year to search for
		/// </param>
		/// <returns>
		/// The index of the year
		/// </returns>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// CYear temp = new CYear(2004, new CEventCollection());
		/// coll.Add(temp);
		/// int intIndex = coll.SearchYear(temp.Year);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int SearchYear(int intYear)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CYear objYear = (CYear) InnerList[intIterate];
				if(objYear.Year == intYear)
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given Year is not in the Collection",
				"int intYear");

		}//SearchYear


		/// <summary>
		/// Used to sort the year collection based on the years of the CYear objects
		/// </summary>
		/// <example>
		/// <code>
		/// CYearCollection coll = new CYearCollection();
		/// CYear temp = new CYear(2004, new CEventCollection());
		/// CYear temp1 = new CYear(2003, new CEventCollection());
		/// coll.Add(temp);
		/// coll.Add(temp1);
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
