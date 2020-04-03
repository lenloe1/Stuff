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
//                           Copyright © 2014 
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// A Comparer that uses a Func to perform the comparison
    /// </summary>
    public class FuncComparer<T> : Comparer<T>
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="comparisonFunction">The comparison Function</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/14 RCG 3.50.36 N/A    Created
        
        public FuncComparer(Func<T, T, int> comparisonFunction)
        {
            m_ComparisonFunction = comparisonFunction;
        }

        /// <summary>
        /// Compares two values
        /// </summary>
        /// <param name="first">The first value</param>
        /// <param name="second">The second value</param>
        /// <returns>0 if equal, a negative value if less than, a positive value if greater than</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/14 RCG 3.50.36 N/A    Created
        
        public override int Compare(T first, T second)
        {
            return m_ComparisonFunction(first, second);
        }

        #endregion

        #region Member Variables

        private Func<T, T, int> m_ComparisonFunction;

        #endregion
    }
}
