///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
// embodying substantial creative efforts and trade secrets, confidential 
// information, ideas and expressions. No part of which may be reproduced or 
// transmitted in any form or by any means electronic, mechanical, or 
// otherwise.  Including photocopying and recording or in connection with any
// information storage or retrieval system without the permission in writing 
// from Itron, Inc.
//
//                              Copyright © 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using Itron.Metering.Program;
using Itron.Metering.Utilities;

namespace Itron.Metering.DataCollections
{

    /// <summary>
    /// Collection of programs that can be accessed like an array or iterated
    /// through by a foreach loop
    /// </summary>
    public class MeterProgramCollection : CollectionBase
    {

        #region Constants

        private const int PROG_NAME_INDEX = 1;
        private const int PROG_ID_INDEX = 2;
        private const int PROG_TYPE_INDEX = 3;
        private const int DATE_MODIFIED_INDEX = 4;

        private const string QUERY_STRING =
            "SELECT RECORDID, PROGRAMNAME, PROGRAMID, PROGTYPE, DATEMODIFIED FROM PROGRAMS;";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="DBConnection">Reference to the shared database connection</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/13/07 RDB         N/A	   Created 
        //
        public MeterProgramCollection(ref ProgramDBConnection DBConnection)
        {
            Refresh(ref DBConnection);
        }//end ProgramCollection

        /// <summary>
        /// Returns the program of the given name, or null if there is not a program
        /// with the given name.
        /// </summary>
        /// <param name="strName">name of the program to find</param>
        /// <returns>
        /// The program of the given name, or null if there is not a program
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/18/07 RDB         N/A	   Created
        //
        public MeterProgram Find(string strName)
        {
            MeterProgram match = null;

            foreach (MeterProgram prog in InnerList)
            {
                if (strName == prog.Name)
                {
                    match = prog;
                    break;
                }
            }

            return match;
        }//end Find

        /// <summary>
        /// Refresh the list of programs.
        /// Exceptions should be caught by the calling application.
        /// </summary>
        /// <param name="DBConnection">
        /// Reference to the shared database connection
        /// </param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/10/08 AF                 Replaced original with this version that 
        //                             passes a shared connection to the database
        //
        public void Refresh(ref ProgramDBConnection DBConnection)
        {
            //Clears the collection
            InnerList.Clear();

            OleDbConnection Connection = DBConnection.Open();

            OleDbCommand Command = Connection.CreateCommand();
            Command.CommandText = QUERY_STRING;

            // Query Database
            OleDbDataReader Reader = Command.ExecuteReader();

            while (Reader.Read())
            {
                MeterProgram objProgram;

                string strMeterType = Reader.GetString(PROG_TYPE_INDEX);

                switch (strMeterType)
                {
                    // CQ 193215 - removing references to legacy devices
                    /*
                    case "X20":
                    {
                        objProgram = new FULCRUMProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.FULCRUM;
                        break;
                    }
                    case "VEC":
                    {
                        objProgram = new VECTRONProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.VECTRON;
                        break;
                    }
                    case "SEN":
                    {
                        objProgram = new SENTINELProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.SENTINEL;
                        break;
                    }
                    case "CVI":
                    {
                        objProgram = new CENTRON_POLYProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.CENTRON_V_AND_I;
                        break;
                    }
                    case "CMA":
                    {
                        objProgram = new CENTRON_MONOProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.CENTRON_C12_19;
                        break;
                    }
                    case "MT2":
                    {
                        objProgram = new CENTRONProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.TWO_HUNDRED_SERIES;
                        break;
                    }
                    case "CEN":
                    {
                        objProgram = new CENTRONProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.CENTRON;
                        break;
                    }
                    case "Q1000":
                    {
                        objProgram = new Q1000Program(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.Q1000;
                        break;
                    }
                      
                    */

                    default:
                    {
                        objProgram = new MeterProgram(ref DBConnection);
                        objProgram.MeterType = DeviceType.eDeviceTypes.UNKNOWN;
                        break;
                    }
                }
               
                objProgram.ID = Reader.GetInt16(PROG_ID_INDEX);
                objProgram.Name = Reader.GetString(PROG_NAME_INDEX);
                objProgram.LastModified = Reader.GetDateTime(DATE_MODIFIED_INDEX);

                InnerList.Add(objProgram);
            }

            Reader.Close();

            //sort programs
            ProgramInfoComparer comparer = new ProgramInfoComparer();
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
        // 06/13/07 RDB         N/A	   Created 
        public MeterProgram this[int index]
        {
            get
            {
                return InnerList[index] as MeterProgram;
            }
        }//end Program

        #endregion

    }//end ProgramCollection

    /// <summary>
    /// This class is needed to sort the programs by name
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/14/07 RDB         N/A	   Created
    public class ProgramInfoComparer : IComparer
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
            MeterProgram xProg = x as MeterProgram;
            MeterProgram yProg = y as MeterProgram;

            if (xProg == null && yProg == null)
            {
                return 0;
            }
            else if (xProg == null && yProg != null)
            {
                return -1;
            }
            else if (xProg != null && yProg == null)
            {
                return 1;
            }
            else
            {
                return xProg.Name.CompareTo(yProg.Name);
            }
        }//end Compare

    }//end ProgramComparer

}
