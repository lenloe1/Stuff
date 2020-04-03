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
using Itron.Metering.Utilities;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// A collection of protocol files that can be easily traversed or indexed
    /// like an array.
    /// </summary>
    public class ProtocolFileCollection: FileCollection
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public ProtocolFileCollection()
            : base()
        {
            m_strDirectory = CRegistryHelper.GetFilePath("Replica") + @"Protocol Files";
            Refresh();
        }//end ProtocolFileCollection

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strPath">
        /// Path of folder that contains protocol files
        /// </param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public ProtocolFileCollection(string strPath)
            : base(strPath)
        {
            Refresh();
        }//end ProtocolFileCollection

        /// <summary>
        /// Refreshes the contents of the protocol file collection
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public void Refresh()
        {

            //Local variables
            ProtocolFile objPF;
            DirectoryInfo objDir;

            //clear the current collection
            InnerList.Clear();

            //Create a directory info object based on the directory
            objDir = new DirectoryInfo(m_strDirectory);

            //Go through the list of protocol files in the directory
            foreach (FileInfo objFile in objDir.GetFiles("*.dnp"))
            {
                //Create a new protocol file from the File info object
                objPF = new ProtocolFile(objFile.FullName);
                objPF.Name = objFile.Name;
                objPF.Type = ProtocolFile.eProtocolType.DNP_3;
                objPF.LastModified = objFile.LastWriteTime;

                //Add the protocol file to the collection
                InnerList.Add(objPF);
            }
            foreach (FileInfo objFile in objDir.GetFiles("*.pds"))
            {
                //Create a new protocol file from the File info object
                objPF = new ProtocolFile(objFile.FullName);
                objPF.Name = objFile.Name;
                objPF.Type = ProtocolFile.eProtocolType.PDS;
                objPF.LastModified = objFile.LastWriteTime;

                //Add the protocol file to the collection
                InnerList.Add(objPF);
            }
            foreach (FileInfo objFile in objDir.GetFiles("*.mbu"))
            {
                //Create a new protocol file from the File info object
                objPF = new ProtocolFile(objFile.FullName);
                objPF.Name = objFile.Name;
                objPF.Type = ProtocolFile.eProtocolType.Modbus;
                objPF.LastModified = objFile.LastWriteTime;

                //Add the protocol file to the collection
                InnerList.Add(objPF);
            }
            foreach (FileInfo objFile in objDir.GetFiles("*.ie2"))
            {
                //Create a new protocol file from the File info object
                objPF = new ProtocolFile(objFile.FullName);
                objPF.Name = objFile.Name;
                objPF.Type = ProtocolFile.eProtocolType.IEC_60870_5_102;
                objPF.LastModified = objFile.LastWriteTime;

                //Add the protocol file to the collection
                InnerList.Add(objPF);
            }
            foreach (FileInfo objFile in objDir.GetFiles("*.i2p"))
            {
                //Create a new protocol file from the File info object
                objPF = new ProtocolFile(objFile.FullName);
                objPF.Name = objFile.Name;
                objPF.Type = ProtocolFile.eProtocolType.IEC_60870_5_102_Plus;
                objPF.LastModified = objFile.LastWriteTime;

                //Add the protocol file to the collection
                InnerList.Add(objPF);
            }

            //sort protocol files
            ProtocolFileComparer comparer = new ProtocolFileComparer();
            InnerList.Sort(comparer);

        }//end Refresh

        #endregion

        #region Public Properties

        /// <summary>
        /// Allows the collection to be indexed like an array
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created 
        public ProtocolFile this[int index]
        {
            get
            {
                return (ProtocolFile)InnerList[index];
            }
        }//end

        #endregion

    }

    /// <summary>
    /// This class is needed to sort the protocol files by name
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/14/07 RDB         N/A	   Created 
    public class ProtocolFileComparer : IComparer
    {

        /// <summary>
        /// Calls CaseInsensitiveComparer.Compare
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/14/07 RDB         N/A	   Created	
        int IComparer.Compare(object x, object y)
        {
            ProtocolFile xPF = (ProtocolFile)x;
            ProtocolFile yPF = (ProtocolFile)y;

            return string.Compare(xPF.Name, yPF.Name, StringComparison.Ordinal);
        }//end Compare

    }//end ProtocolFileComparer

}
