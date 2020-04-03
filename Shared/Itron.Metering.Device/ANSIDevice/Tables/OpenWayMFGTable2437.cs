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
//                              Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Utilities;
using Itron.Metering.TOU;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This OpenWayMFGTable2437 class contains the 25 Year TOU Schedule.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  11/15/13 jrf 3.50.04 TQ 9478  Created.
    //  11/20/13 jrf 3.50.06 TQ 9479 Made class public. 
    // 
    public class OpenWayMFGTable2437
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The protocol instance to use</param>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //  11/20/13 jrf 3.50.06 TQ 9479 Removed unnecessary member variable, m_PSEM. 
        // 
        public OpenWayMFGTable2437(CPSEM psem)
        {
            m_CalendarConfig = new CENTRON_AMI_25_Year_CalendarConfig(psem, 0, CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_SIZE, 
                CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_YEARS);
            m_TOUConfig = new CENTRON_AMI_25_Year_TOUConfig(psem, CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_SIZE);
        }

        /// <summary>
        /// Constructor used to get Data from the EDL file
        /// </summary>
        /// <param name="reader">The current PSEM binary reader object.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/18/13 jrf 3.50.05 TQ 9479 Created.
        //  11/20/13 jrf 3.50.06 TQ 9479 Removed unnecessary member variable, m_PSEM. 
        //
        public OpenWayMFGTable2437(PSEMBinaryReader reader)
        {
            m_CalendarConfig = new CENTRON_AMI_25_Year_CalendarConfig(reader, 0, CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_SIZE, 
                CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_YEARS);
            m_TOUConfig = new CENTRON_AMI_25_Year_TOUConfig(reader, CENTRON_AMI_25_Year_CalendarConfig.CENTRON_AMI_CAL_SIZE);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Calendar Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //
        public CalendarConfig CalendarConfig
        {
            get
            {
                return m_CalendarConfig;
            }
        }

        /// <summary>
        /// Provides access to the TOU Config table
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ --------------------------------------------- 
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //
        public TOUConfig TOUConfig
        {
            get
            {
                return m_TOUConfig;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The Calendar configuration
        /// </summary>
        private CalendarConfig m_CalendarConfig;

        /// <summary>
        /// The TOU configuration
        /// </summary>
        private TOUConfig m_TOUConfig;

        #endregion
    }

    
    /// <summary>
    /// The CENTRON_AMI_25_Year_TOUConfig class represents the 25 Year TOU Configuration 
    /// data block in table 2347. The TOU portion of the configuration defines the seasons
    /// that are applied across the years of the TOU schedule. Seasons are
    /// applied to years in the CalendarConfig portion of the configuration.
    /// </summary>
    /// 
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ -------------------------------------------
    //  11/15/13 jrf 3.50.04 TQ 9478  Created.
    //
    public class CENTRON_AMI_25_Year_TOUConfig : CENTRON_AMI_TOUConfig
    {
        #region Constants
        
        /// <summary>
        /// Size of the TOU Table
        /// </summary>
        new public const ushort TOU_CONFIG_SIZE = 408;

        private const uint NUM_SUPPORTED_SEASONS = 8;

        #endregion

        #region public methods
        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Offset of this subtable within table 2048</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //  11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData().
        //  01/10/14 jrf 3.50.23 TQ 9478 Setting the appropriate table ID.
        //
        public CENTRON_AMI_25_Year_TOUConfig(CPSEM psem, ushort Offset)
            : base(psem, 2437, Offset, TOU_CONFIG_SIZE)
        {
        }

        /// <summary>
        /// Constructor for 25 Year TOU Configuration table used with file based structure
        /// </summary>
        /// <param name="BinaryReader">PSEM data reader.</param>
        /// <param name="Offset"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //  11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData().
        //  01/10/14 jrf 3.50.23 TQ 9478 Setting the appropriate table ID and config size.
        //
        public CENTRON_AMI_25_Year_TOUConfig(PSEMBinaryReader BinaryReader, ushort Offset)
            : base(BinaryReader, 2437, Offset, TOU_CONFIG_SIZE)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the number of Supported Seasons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.

        public override uint NumberOfSupportedSeasons
        {
            get
            {
                return NUM_SUPPORTED_SEASONS;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Setup data items
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/20/13 jrf 3.50.06 TQ 9478 Created. 
        //
        protected override void InitializeData()
        {
            base.InitializeData();

            m_blnHasSeasonProgrammedByte = true;
        }

        #endregion

    }


    /// <summary>
    /// The CENTRON_AMI_25_Year_CalendarConfig class represents the 25 Year Calendar Configuration data 
    /// block in table 2437.
    /// </summary>
    /// 
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    //  11/15/13 jrf 3.50.04 TQ 9478  Created.
    // 
    public class CENTRON_AMI_25_Year_CalendarConfig : CENTRON_AMI_CalendarConfig
    {
        #region Constants

        /// <summary>
        /// The size of the Calendar
        /// </summary>
        new public const ushort CENTRON_AMI_CAL_SIZE = 1631;

        /// <summary>
        /// Number of Years in the Calendar
        /// </summary>
        new public const byte CENTRON_AMI_CAL_YEARS = 25;

        private const int EVENTS_PER_YEAR = 32;

        #endregion

        #region public methods

        /// <summary>Constructor</summary>
        /// <param name="psem">Protocol instance to use</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478 Created.
        //  11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData(...).
        //  01/10/14 jrf 3.50.23 TQ 9478 Setting the appropriate table ID.
        // 
        public CENTRON_AMI_25_Year_CalendarConfig(CPSEM psem, ushort Offset, ushort Size,
                              byte MaxCalYears)
            : base(psem, 2437, Offset, Size, MaxCalYears)
        {            
        }

        /// <summary>
        /// Calendar Configuration Construcutor used for file based structure
        /// </summary>
        /// <param name="BinaryReader">The binary Reader containing the data stream</param>
        /// <param name="Offset">Byte offset of the start of this table</param>
        /// <param name="Size">Size of the table in bytes</param>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.
        //  11/20/13 jrf 3.50.06 TQ 9478 Moving data setup to InitializeData(...).
        //  01/10/14 jrf 3.50.23 TQ 9478 Setting the appropriate table ID.
        //
        public CENTRON_AMI_25_Year_CalendarConfig(PSEMBinaryReader BinaryReader, ushort Offset, ushort Size,
                                byte MaxCalYears)
            : base(BinaryReader, 2437, Offset, Size, MaxCalYears)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Number of Events per Year
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/15/13 jrf 3.50.04 TQ 9478  Created.

        public override int EventsPerYear
        {
            get
            {
                return EVENTS_PER_YEAR;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Setup data items
        /// </summary>
        /// <param name="MaxCalYears">The max calendar years the TOU calendar can 
        /// support</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/19/13 jrf 3.50.06 TQ 9478 Created. 
        //
        protected override void InitializeData(byte MaxCalYears)
        {
            base.InitializeData(MaxCalYears);

            //With season changes we now have control byte.
            m_blnHasControlByte = true;
        }

        #endregion
    }

    
}
