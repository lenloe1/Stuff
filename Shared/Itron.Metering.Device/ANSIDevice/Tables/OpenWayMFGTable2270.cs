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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{

    /// <summary>
    ///  OpenWay MFG Table 2270 is a read only table which holds HW 3.0 Task execution Times
    /// </summary>
    public class OpenWayMFGTable2270 : AnsiTable
    {
        #region Constants

        private const int TABLE_TIMEOUT = 500;
        private const int SIZE_OF_TASK_RECORD = 13; // byte + + uint32 + uint32 + uint16 + uint16
        private readonly DateTime REFERENCE_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
       
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="fltRegFWVer">register firmware version</param>
        /// <param name="uiRegFWBuild">register firmware build</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/11 MMD  	                Created
        // 05/15/15 jrf 4.50.124 584991 Allowing automatic resizing of table. 
        // 05/27/15 jrf 4.50.126 584991 Moving instantiation of AMIHW3TASKRCD[] 
        //                              to ParseData()
        public OpenWayMFGTable2270(CPSEM psem, float fltRegFWVer, uint uiRegFWBuild)
            : base(psem, 2270, GetTableSize(fltRegFWVer, uiRegFWBuild) , TABLE_TIMEOUT)
        {
            m_blnAllowAutomaticTableResizing = true;
        }

        /// <summary>
        /// Reads the sub table from the meter
        /// </summary>
        /// <returns>The response of the read.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/11 MMD  	                Created

        public override PSEMResponse Read()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "OpenWayMFGTable2270.Read");

            PSEMResponse Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                ParseData();
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets free running time
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/20/11 MMD  	                Created

        public DateTime FreeRunningTime
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                DateTime CurrentOccurrance = REFERENCE_TIME;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HW3 Task Exe time Table"));
                    }
                    else
                    {
                        if (m_uiFreeRunningTime != 0xFFFFFFFF)
                        {
                            CurrentOccurrance = CurrentOccurrance.AddMilliseconds(m_uiFreeRunningTime);

                        }
                    }
                }

                return CurrentOccurrance;

            }
        }

        /// <summary>
        /// This is essentially a dump of table 2270.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        // 05/20/11 MMD  	                Created
        //
        public AMIHW3TASKRCD[] AMIHW3TASKRCD
        {
            get
            {
                PSEMResponse Result = PSEMResponse.Ok;
                if (TableState.Loaded != m_TableState)
                {
                    //Read Table
                    Result = Read();
                    if (PSEMResponse.Ok != Result)
                    {
                        throw (new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading HW3 Task Exe time Table"));
                    }
                }

                return m_aAMIHW3TASKRCD;
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the data that has just been read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/10 RCG 2.40.25		   Created
        // 05/15/15 jrf 4.50.124 584991 Updating to determine number of tasks based on size of table. 
        // 05/27/15 jrf 4.50.126 584991 Need to instantiate AMIHW3TASKRCD[] here since this is where 
        //                              we calculate the number of tasks
        private void ParseData()
        {
            m_uiFreeRunningTime = m_Reader.ReadUInt32();

            //The number of firmware version variations for the size of this table was getting ridiculous to manage.
            //Now we will automatically resize table and then compute the number of task records based on size of table data 
            //returned from the meter.
            //# tasks = (size of table data returned from meter - 4 bytes for free running time) / size of a task record
            m_numberTasks = ((int)m_Size - 4) / SIZE_OF_TASK_RECORD;
            m_aAMIHW3TASKRCD = new AMIHW3TASKRCD[m_numberTasks]; 

            for (int index = 0; index < m_numberTasks; index++)
            {
                m_aAMIHW3TASKRCD[index] = new AMIHW3TASKRCD();
                m_aAMIHW3TASKRCD[index].TaskName = m_Reader.ReadByte();
                m_aAMIHW3TASKRCD[index].LastSwitchInTime = m_Reader.ReadUInt32();
                m_aAMIHW3TASKRCD[index].LastSwitchOutTime = m_Reader.ReadUInt32();
                m_aAMIHW3TASKRCD[index].LastExecutionTime = m_Reader.ReadUInt16();
                m_aAMIHW3TASKRCD[index].MaxExecutionTime = m_Reader.ReadUInt16();
            }
            
        }

        /// <summary>
        /// Determines the size of the table based on the firmware version running in the meter
        /// </summary>
        /// <param name="fltRegFWRevision">The version.revision of register f/w expressed as a floating point number</param>
        /// <param name="uiRegFWBuild">The build of the register f/w expressed as a uint</param>
        /// <returns>The size of the table</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/10/13 jkw 2.70.56        Created
        //  07/16/13 mah 2.80.53        Added corrections for Michigan table size
        //  04/15/14 jrf 3.50.75 489638 Sizing table appropriately for Carbon version 5.006.xxx.
        //  12/29/14 AF  4.50.26 553961 Sizing table appropriately for RAM Robustness version 5.009.xxx.
        //  12/30/14 AF  4.50.27 NA     Change robustness name for merging with the 4.0 branch
        //
        private static uint GetTableSize(float fltRegFWRevision, uint uiRegFWBuild)
        {
            m_numberTasks = 11;

            if ((VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_5_5_CARBON) == 0) ||
                 (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_CARBON) == 0) || 
                (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_5_8_BRIDGE_PHASE_2) == 0) ||
                (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_BORON_5_0) == 0 && uiRegFWBuild > 33) )
            {
                m_numberTasks = 14;
            }
            else if ((VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_BORON_5_2) == 0) ||
                    (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_5_9_CARBON_BRIDGE_ROBUST_RAM) == 0))
            {
                m_numberTasks = 13;
            }
            else if ((VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_LITHIUM_3_12) == 0 && uiRegFWBuild > 119) )
            {
                m_numberTasks = 10;
            }


            if (VersionChecker.CompareTo(fltRegFWRevision, CENTRON_AMI.VERSION_6_0_MICHIGAN) >= 0)
            {
                m_numberTasks = 15;
                m_tableSize = 209;
            }
            else
            {
                m_tableSize = (uint)(SIZE_OF_TASK_RECORD * m_numberTasks + 4);
            }

            return m_tableSize;
        }

        #endregion

        #region Member Variables
        private uint m_uiFreeRunningTime;
        private AMIHW3TASKRCD[] m_aAMIHW3TASKRCD;
        private static int m_numberTasks = 11;
        private static uint m_tableSize = 147;

        #endregion
    }


    /// <summary>
    /// Class that represents a single Task Information
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  05/20/11 MMD                 Created
    //
    public class AMIHW3TASKRCD
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public AMIHW3TASKRCD()
        {
           
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Task Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public byte TaskName
        {
            get
            {
                return m_byTaskName;
            }
            set
            {
                m_byTaskName = value;
            }
        }

        /// <summary>
        /// Last Switch In Time in milli seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public uint LastSwitchInTime
        {
            get
            {
                return m_uiLastSwitchInTime;
            }
            set
            {
                m_uiLastSwitchInTime = value;
            }
        }

        /// <summary>
        /// Last Switch Out Time in milli seconds
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public uint LastSwitchOutTime
        {
            get
            {
                return m_uiLastSwitchOutTime;
            }
            set
            {
                m_uiLastSwitchOutTime = value;
            }
        }

        /// <summary>
        /// LastExecutionTime
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public ushort LastExecutionTime
        {
            get
            {
                return m_uiLastExecutionTime;
            }
            set
            {
                m_uiLastExecutionTime = value;
            }
        }

        /// <summary>
        /// Max Execution Time
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/20/11 MMD                 Created
        //
        public ushort MaxExecutionTime
        {
            get
            {
                return m_uiMaxExecutionTime;
            }
            set
            {
                m_uiMaxExecutionTime = value;
            }
        }

      #endregion

        #region Members
        private byte m_byTaskName;
        private uint m_uiLastSwitchInTime;
        private uint m_uiLastSwitchOutTime;
        private ushort m_uiLastExecutionTime;
        private ushort m_uiMaxExecutionTime;

        #endregion
    }
}
