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
using Itron.Metering.ReplicaSettings;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Collection of MV90 HHF files that can be easily indexed like an array
    /// or iterated through with a foreach loop
    /// </summary>
    public class MV90HHFFileCollection: FileCollection
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/07 RDB         N/A	Created 
        public MV90HHFFileCollection()
            : base()
        {
            //Get the directory for the MV90 HHF files
            CXMLFieldProSettings xmlFPSettings = new CXMLFieldProSettings("");
            m_strDirectory = xmlFPSettings.AllDevices.MasterStationDataDirectory;

            Refresh();
        }//end MV90HHFFileCollection

        /// <summary>
        /// Constructor that takes the directory of the MV90 HHF files
        /// </summary>
        /// <param name="strDataDirectory">Directory containing MV90 HHF files</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created 
        public MV90HHFFileCollection(string strDataDirectory)
            : base(strDataDirectory)
        {
            Refresh();
        }//end MV90HHFFileCollection

        /// <summary>
        /// Method is used to refresh the collection of MV90 HHF's
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
            MV90_HHF objMV90HHF;
            DirectoryInfo objDir;

            //clear the collection
            InnerList.Clear();

            //Create a directory info object based on the directory
            objDir = new DirectoryInfo(m_strDirectory);

            //Go through the list of hhf files in the directory
            foreach (FileInfo objFile in objDir.GetFiles())
            {
                if (MV90_HHF.IsMV90HHFFile(objFile.FullName))
                {
                    //Create a new MV90_HHF from the File info object
                    objMV90HHF = new MV90_HHF(objFile.FullName);

                    //Add the MV90_HHF to the collection
                    InnerList.Add(objMV90HHF);
                }
            }

            //Sort list
            IComparer myComparer = new MV90_HHFComparer();
            InnerList.Sort(myComparer);

        }//end Refresh

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an index of MV90HHFFileCollection.  Allows the user to index the 
        /// collection in the same manner as an array
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/05/07 RDB         N/A	Created  
        public MV90_HHF this[int index]
        {
            get
            {
                return (MV90_HHF)InnerList[index];
            }
        }//end

        #endregion

    }//end MV90HHFFileCollection

    /// <summary>
    /// This class is needed to sort the MV90 HHF's by name
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/06/07 RDB         N/A	Created 
    public class MV90_HHFComparer : IComparer
    {

        /// <summary>
        /// Calls CaseInsensitiveComparer.Compare
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/06/07 RDB         N/A	Created	
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        int IComparer.Compare(object x, object y)
        {
            MV90_HHF xHHF = (MV90_HHF)x;
            MV90_HHF yHHF = (MV90_HHF)y;

            if (xHHF == null && yHHF == null)
            {
                return 0;
            }
            else if (xHHF == null && yHHF != null)
            {
                return -1;
            }
            else if (xHHF != null && yHHF == null)
            {
                return 1;
            }
            else
            {
                return xHHF.FileName.CompareTo(yHHF.FileName);
            }
        }//end Compare

    }//end MV90_HHFComparer

}
