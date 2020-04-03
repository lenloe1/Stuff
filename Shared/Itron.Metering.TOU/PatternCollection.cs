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
using System.Collections.ObjectModel;

namespace Itron.Metering.TOU
{
	/// <summary>
	/// Class Represents the list of patterns availabe in a TOU Schedule
	/// </summary>
	public class CPatternCollection : Collection<CPattern>
	{
		/// <summary>
		/// Creates an instance of the Pattern Collection object
		/// </summary>
		/// <example>
		/// <code>
		/// CPatternCollection coll = new CPatternCollection();
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public CPatternCollection()
		{
			
		}//CpatternCollection


		/// <summary>
        /// THis method looks for a pattern matching the given collection
        /// of switchpoints.
        /// </summary>
        /// <param name="SwitchPoints">The collection of switchpoints to search for.</param>
        /// <returns>It returns the matching pattern's ID or -1 for no match.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/01/08 jrf 9.50.16        Created
        //
        public int FindPatternID(CSwitchPointCollection SwitchPoints)
        {
            int iPatternID = -1;

            foreach (CPattern Pattern in Items)
            {
                if (Pattern.Equals(new CPattern(0, "", SwitchPoints)))
                {
                    iPatternID = Pattern.ID;
                }
            }

            return iPatternID;
        }


		/// <summary>
		/// Searches the collection for the given pattern ID and returns the index.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="intPatternID">
		/// The ID to search for
		/// </param>
		/// <returns>
		/// The index of the Pattern ID
		/// </returns>
		/// <example>
		/// <code>
		/// CPatternCollection coll = new CPatternCollection();
		/// CPattern temp = new CPattern();
		/// coll.Add(temp);
		/// int intIndex = coll.SearchID(temp.PatternID);
		/// </code>
		/// </example>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 02/15/06 rrr N/A	 N/A	Creation of class  
		public int SearchID(int intPatternID)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for(int intIterate = 0; intIterate < Items.Count; intIterate++)
			{
				CPattern objPattern = (CPattern) Items[intIterate];
				if(objPattern.ID == intPatternID )
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given Pattern ID is not in the Collection",
				"int intPatternID");	
		}//SearchID

		/// <summary>
		/// Searches the collection for the given pattern name and returns the index.
		/// If the item is not found in the collection then an ArgumentException 
		/// will be thrown
		/// </summary>
		/// <param name="strPatternName">
		/// The pattern name to search for
		/// </param>
		/// <returns>
		/// The index of the pattern 
		/// </returns>
		/// <remarks>
		/// Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		/// 08/15/07 MAH		Created			
		///
		///	</remarks>
		public int SearchName(String strPatternName)
		{
			//Go through the collection checking for matches and return the index
			//when one is found
			for (int intIterate = 0; intIterate < Items.Count; intIterate++)
			{
				CPattern objPattern = (CPattern)Items[intIterate];
				if (objPattern.Name == strPatternName)
				{
					return intIterate;
				}
			}

			//If the item was not found then throw an error
			throw new ArgumentException("The given pattern name is not in the Collection",
				"String strPatternName");
		}//SearchName

        /// <summary>
        /// Returns the next available (unused) pattern ID.  This method assumes
        /// that gaps are allowed between pattern IDs
        /// </summary>
        /// <returns>
        /// An ID that can be used to create a new pattern 
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mah N/A	 N/A	Added this method  
        public int GetNextPatternID()
        {
            int nNextPatternID = 1; // Assumes zero is not a legal season ID

            // All we need for a valid season ID is a number that is one
            // greater than the highest ID currently in use.  So troll through
            // the season list and find the highest ID value.

            foreach (CPattern pattern in Items)
            {
                if (pattern.ID >= nNextPatternID)
                    nNextPatternID = pattern.ID + 1;
            }

            return nNextPatternID;
        } // GetNextSeasonID


	}
}
