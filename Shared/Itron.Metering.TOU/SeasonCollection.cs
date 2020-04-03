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
	/// Represents a list of seasons in a TOU Schedule
	/// </summary>
	public class CSeasonCollection : CollectionBase
	{
		/// <summary>
		/// Creates an instance of the Season Collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CSeasonCollection()
		{
			
		}//CSeasonCollection


		/// <summary>
		/// Gets/Sets a Season at an index of the SeasonCollection.  Allows access
		/// to elements like an array
		/// </summary>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// CSeason temp = coll[0];
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CSeason this[int intIndex]
		{
			get 
			{ 
				return (CSeason) InnerList[intIndex]; 
			}
            set
            {
                InnerList[intIndex] = value;
            }
		}//this[]


		/// <summary>
		/// Adds a Season to the end of the SeasonCollection
		/// </summary>
		/// <param name="objToAdd">
		/// The Season to be added
		/// </param>
		/// <returns>
		/// The zero base index of the Season added
		/// </returns>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// coll.Add(new CSeason());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int Add(CSeason objToAdd)
		{
			return InnerList.Add(objToAdd);
		}//Add


		/// <summary>
		/// Adds a Season to the SeasonCollection at the given index
		/// </summary>
		/// <param name="intIndex">
		/// Index to insert the Season into in the collection
		/// </param>
		/// <param name="objToAdd">
		/// The Season to be added
		/// </param>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// coll.Insert(3, new CSeason());
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public void Insert(int intIndex, CSeason objToAdd)
		{
			InnerList.Insert(intIndex, objToAdd);
		}//Insert


		/// <summary>
		/// Returns the index of the first occurance of the given Season.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="objFind">
		/// Name of the Season to find the index of
		/// </param>
		/// <returns>
		/// The index of the Season
		/// </returns>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// CSeason temp = new CSeason();
		/// coll.Add(temp);
		/// int intIndex = coll.IndexOf(temp);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int IndexOf(CSeason objFind)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CSeason objSeason = (CSeason) InnerList[intIterate];
				if(objSeason.ID == objFind.ID )
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given Season is not in the Collection",
				"CSeason objFind");	
		}//IndexOf


		/// <summary>
		/// Searches the collection for the given Season ID and returns the index.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="intSeasonID">
		/// The ID to search for
		/// </param>
		/// <returns>
		/// The index of the Season ID
		/// </returns>
		/// <example>
		/// <code>
		/// CSeasonCollection coll = new CSeasonCollection();
		/// CSeason temp = new CSeason();
		/// coll.Add(temp);
		/// int intIndex = coll.SearchID(temp.SeasonID);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int SearchID(int intSeasonID)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < InnerList.Count; intIterate++)
			{
				CSeason objSeason = (CSeason) InnerList[intIterate];
				if(objSeason.ID == intSeasonID )
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given SeasonID is not in the Collection",
				"int intSeasonID");
		}//SearchID

        /// <summary>
        /// Returns the next available (unused) season ID.  This method assumes
        /// that gaps are allowed between season IDs
        /// </summary>
        /// <returns>
        /// An ID that can be used to create a new season 
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/28/06 mah N/A	 N/A	Added this method  
        public int GetNextSeasonID()
        {
            int nNextSeasonID = 1; // Assumes zero is not a legal season ID

            // All we need for a valid season ID is a number that is one
            // greater than the highest ID currently in use.  So troll through
            // the season list and find the highest ID value.

            foreach (CSeason season in InnerList)
            {
                if (season.ID >= nNextSeasonID)
                    nNextSeasonID = season.ID + 1;
            }

            return nNextSeasonID;
        } // GetNextSeasonID


	}
}
