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
using System.Text;

namespace Itron.Metering.TOU
{

    /// <summary>
    /// This class is a collection of integers that will be used in
    /// the seasons class to hold the pattern ids for the various
    /// holiday and normal day types.
    /// </summary>
    public class Int16Collection : CollectionBase   
    {

        /// <summary>
        /// Allows for indexing into the collection like an array.
        /// </summary>
        /// <param name="index">
        /// The index of the int that will be returned.
        /// </param>
        /// <returns>
        /// The integer at the given location
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Creation of class  
        public Int16 this[int index]
        {
            get
            {
                return ((Int16)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Adds the value to the collection.
        /// </summary>
        /// <param name="value">
        /// The integer value that is to be added to the collection.
        /// </param>
        /// <returns>
        /// The index where the value was added.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Creation of class  
        public int Add(Int16 value)
        {
            return (List.Add(value));
        }

        /// <summary>
        /// Returns the value of the given value.
        /// </summary>
        /// <param name="value">
        /// The value whose index is to be found.
        /// </param>
        /// <returns>
        /// The index of the given value in the collection.
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Creation of class  
        public int IndexOf(Int16 value)
        {
            return (List.IndexOf(value));
        }

        /// <summary>
        /// Inserts the given value at the specified index.
        /// </summary>
        /// <param name="index">
        /// The location in the collection where the value will be added.
        /// </param>
        /// <param name="value">
        /// The value that will be inserted at the given index.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Creation of class  
        public void Insert(int index, Int16 value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value from the collection.
        /// </summary>
        /// <param name="value">
        /// The value that will be removed from the collection.
        /// </param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/25/06 ach N/A	 N/A	Creation of class  
        public void Remove(Int16 value)
        {
            List.Remove(value);
        }

    }
}
