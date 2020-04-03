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
//                              Copyright © 2008 
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
    /// Collection of EDL files that can be easily indexed like an array
    /// or iterated through with a foreach loop
    /// </summary>
    public class CentronIIEDLFileCollection: FileCollection
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0    N/A     Created 
        //
        public CentronIIEDLFileCollection()
            : base()
        {
            // TODO - what will the default directory be?
            //Get the directory for the MV90 HHF files
            CXMLFieldProSettings xmlFPSettings = new CXMLFieldProSettings("");
            m_strDirectory = xmlFPSettings.AllDevices.MasterStationDataDirectory;

            Refresh();
        }//end CentronIIEDLFileCollection

        /// <summary>
        /// Constructor that takes the directory of the EDL files
        /// </summary>
        /// <param name="strDataDirectory">Directory containing EDL files</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0    N/A     Created 
        //
        public CentronIIEDLFileCollection(string strDataDirectory)
            : base(strDataDirectory)
        {
            Refresh();
        }//end CentronIIEDLFileCollection

        /// <summary>
        /// Method is used to refresh the collection of EDL files
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0    N/A     Created 
        //  01/29/08 MAH 10.0            Added try catch block so that if (or when) we encounter a file 
		//                                          that appears to be an EDL file but cannot be opened, the 
		//                                          file will not be added to the list rather than throwing an exception
		//                                          and invalidating the list of EDL files
        public void Refresh()
        {
            CentronIIEDLFile objEDLFile;
            DirectoryInfo objDir;

            //clear the collection
            InnerList.Clear();

            //Create a directory info object based on the directory
            objDir = new DirectoryInfo(m_strDirectory);

            //Go through the list of edl files in the directory
            foreach (FileInfo objFile in objDir.GetFiles())
            {
				if (EDLFile.IsEDLFile(objFile.FullName))
                {
					try
					{
						//Create a new EDLFile from the File info object
						objEDLFile = new CentronIIEDLFile(objFile.FullName);

						//Add the EDL File to the collection
						InnerList.Add(objEDLFile);
					}
					catch 
					{
						// Do nothing - for some reason we thought we found a valid EDL file but were unable
						// to actually read the file.  The net result is that, since the file cannot be read, it 
						// will not appear in the list of valid EDL files
					}
                }
            }

            //Sort list
            IComparer myComparer = new CentronIIEDLFileComparer();
            InnerList.Sort(myComparer);

        }//end Refresh

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets an index of EDLFileCollection.  Allows the user to index the 
        /// collection in the same manner as an array
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0    N/A     Created 
        //
        public EDLFile this[int index]
        {
            get
            {
                return (EDLFile)InnerList[index];
            }
        }//end

        #endregion

    }//end CentronIIEDLFileCollection

    /// <summary>
    /// This class is needed to sort the EDL files by name
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  01/21/08 AF  10.0    N/A     Created 
    //
    public class CentronIIEDLFileComparer : IComparer
    {

        /// <summary>
        /// Calls CaseInsensitiveComparer.Compare
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/21/08 AF  10.0    N/A     Created 
        //
        int IComparer.Compare(object x, object y)
        {
            CentronIIEDLFile xEDL = (CentronIIEDLFile)x;
            CentronIIEDLFile yEDL = (CentronIIEDLFile)y;

            if (xEDL == null && yEDL == null)
            {
                return 0;
            }
            else if (xEDL == null && yEDL != null)
            {
                return -1;
            }
            else if (xEDL != null && yEDL == null)
            {
                return 1;
            }
            else
            {
                return xEDL.FileName.CompareTo(yEDL.FileName);
            }
        }//end Compare

    }//end CentronIIEDLFileComparer

}
