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

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Base class that represents a collection of files.  This class should
    /// be extended by other classes so that it represents collections of
    /// specific types of files that can be indexed into like an array or
    /// iterated through by a foreach loop.
    /// </summary>
    public class FileCollection: CollectionBase
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/07 RDB         N/A	Created 
        public FileCollection()
        {
        }//end FileCollection

        /// <summary>
        /// Constructor that takes the path to the directory containing the files
        /// </summary>
        /// <param name="strDirectory">Directory containing files</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created 
        public FileCollection(string strDirectory)
        {
            m_strDirectory = strDirectory;
        }//end FileCollection

        #endregion

        #region Protected Members

        /// <summary>
        /// Path to directory containing files
        /// </summary>
        protected String m_strDirectory;

        #endregion

    }//end FileCollection

}
