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
using System.IO;
using System.Text;
using Itron.Metering.Datafiles;
using Itron.Metering.Device;
using Itron.Metering.Progressable;
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Collection of MIFs that can be indexed like an array or iterated through
    /// with a foreach loop.
    /// </summary>
    public class MIFFileCollection: FileCollection
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created 
        public MIFFileCollection()
            : base()
        {
            //Get the directory for the MIF files
            CXMLFieldProSettings xmlFPSettings = new CXMLFieldProSettings("");
            m_strDirectory = xmlFPSettings.AllDevices.MasterStationDataDirectory;

            Refresh();
        }//end MIFFileCollection

        /// <summary>
        /// Constructor that takes the directory of the MIF files
        /// </summary>
        /// <param name="strDirectory">Directory containing MIF files</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created 
        public MIFFileCollection(string strDirectory)
            : base(strDirectory)
        {
            Refresh();
        }//end MIFFileCollection

        /// <summary>
        /// Method is used to refresh the collection of MIF's
        /// </summary>
        /// <remarks>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/06/07 RDB         N/A	Created		
		/// 07/27/07 MAH		Removed filters based on file extensions - all files will be examined
		/// </remarks>
        public void Refresh()
        {

            //Local variables
            MIF objMIF;
            DirectoryInfo objDir;

            //clear the current collection
            InnerList.Clear();

            //Create a directory info object based on the directory
            objDir = new DirectoryInfo(m_strDirectory);

			//Go through the list of mif files in the directory
			foreach (FileInfo objFile in objDir.GetFiles() )
			{
				if (MIF.IsMeterImageFile(objFile.FullName))
				{
					//Create a new MIF from the File info object
					objMIF = new MIF(objFile.FullName);

					//Add the MIF to the collection
					InnerList.Add(objMIF);
				}
			}
 
            //Sort list
            IComparer myComparer = new MIFComparer();
            InnerList.Sort(myComparer);

        }//end Refresh

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an index of MIFFileCollection.  Allows the user to index the 
        /// collection in the same manner as an array
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created  
        public MIF this[int index]
        {
            get
            {
                return (MIF)InnerList[index];
            }
        }//end

        #endregion

    }//end MIFFileCollection

    /// <summary>
    /// This class is needed to sort the MIF's by name
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/06/07 RDB         N/A	Created 
    public class MIFComparer : IComparer
    {

        /// <summary>
        /// Makes it possible for MIFFileCollections to be sorted alphabetically
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created	
        int IComparer.Compare(object x, object y)
        {
            MIF xMIF = (MIF)x;
            MIF yMIF = (MIF)y;

            if (xMIF == null && yMIF == null)
            {
                return 0;
            }
            else if (xMIF == null && yMIF != null)
            {
                return -1;
            }
            else if (xMIF != null && yMIF == null)
            {
                return 1;
            }
            else
            {
                return xMIF.FileName.CompareTo(yMIF.FileName);
            }
        }//end Compare

    }//end MIFComparer

}
