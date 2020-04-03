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
//                           Copyright © 2006 - 2007 
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class represents a C12.19 REGISTER_DATA_RCD (table 23) structure.
    /// This table contains all of the current register values for the meter.
    /// </summary>
    public class C1219_RegisterDataRcd
    {
        #region Definitions

        /// <summary>
        /// This structure represents a C12.19 table 23 DEMAND_RCD with all the
        /// flags on.  Note that some values may not be valid depending on the
        /// actual values of the flags.
        /// </summary>
        public struct DEMANDS_RCD
        {
            /// <summary>Demand peak timestamps</summary>
            public DateTime[] DemandTOO;
            /// <summary>CumDemand</summary>
            public double CumDemand;
            /// <summary>CCumDemand</summary>
            public double CCumDemand;
            /// <summary>Demand peak values.  NOTE that C12.19 defines this as
            /// NI_FMAT2, but Kevin Guthrie's library treats all NI_FMAT1 and
            /// NI_FMAT2 types as doubles.</summary>
            public double[] Demand;
        }

        /// <summary>
        /// This structure represents a C12.19 table 23 DEMAND_BLK_RCD.
        /// </summary>
        public struct DEMANDS_BLK_RCD
        {
            /// <summary></summary>
            public double[] Summations;
            /// <summary></summary>
            public DEMANDS_RCD[] Demands;
            // Coincidents - not needed, not implemented
        }

        /// <summary>
        /// This structure represents a C12.19 table 23 REGISTER_DATA_RCD with
        /// the DEMAND_RESET_CTR_FLAG on.  Note that the NbrDemandResets will 
        /// not be valid if the actual DEMAND_RESET_CTR_FLAG is off. 
        /// </summary>
        public struct REGISTER_DATA_RCD
        {
            /// <summary></summary>
            public byte NbrDemandResets;
            /// <summary></summary>
            public DEMANDS_BLK_RCD TotDataBlock;
            /// <summary></summary>
            public DEMANDS_BLK_RCD[] TierDataBlock;
        }

        private const byte DATE_TIME_FLAG_MASK = 0x02;
        private const byte DEMAND_RESET_CTR_FLAG_MASK = 0x04;
        private const byte CUM_DEMAND_FLAG_MASK = 0x10;
        private const byte CCUM_DEMAND_FLAG_MASK = 0x20;

        #endregion Definitions

        #region Constructors

        /// <summary>Constructs a C12.19 REGISTER_DATA_RCD (table 23) based
        /// on the table 21 values.  This class assumes that NI_FMT1 is a 
        /// long float and NI_FMT2 is a short float.  This is true for all of 
        /// our ANSI meters to date.</summary>
        /// <param name="RegFunc1Bfld">Table 21 REG_FUNC1_FLAGS</param>
        /// <param name="NbrSummations">Table 21 NBR_SUMMATIONS (Energy)</param>
        /// <param name="NbrDemands">Table 21 NBR_DEMANDS</param>
        /// <param name="NbrOccur">Table 21 NBR_OCCUR (Peaks)</param>
        /// <param name="NbrTiers">Table 21 NBR_TIERS (Rates)</param>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public C1219_RegisterDataRcd(byte RegFunc1Bfld, byte NbrSummations,
            byte NbrDemands, byte NbrOccur, byte NbrTiers)
        {
            m_RegFunc1Bfld=RegFunc1Bfld; 
            m_NbrSummations=NbrSummations;
            m_NbrDemands=NbrDemands;
            m_NbrOccur=NbrOccur;
            m_NbrTiers=NbrTiers;

            // Instantiate our structure according to our sizing parameters.
            m_CurrentRegisters.TotDataBlock.Summations = new double[m_NbrSummations];
            m_CurrentRegisters.TotDataBlock.Demands = new DEMANDS_RCD[m_NbrDemands];
            for( int i=0; i<m_CurrentRegisters.TotDataBlock.Demands.Length; i++)
            {
                if (DateTimeFlag)
                {
                    m_CurrentRegisters.TotDataBlock.Demands[i].DemandTOO = new DateTime[m_NbrOccur];
                }
                m_CurrentRegisters.TotDataBlock.Demands[i].Demand = new double[m_NbrDemands];
            }

            m_CurrentRegisters.TierDataBlock = new DEMANDS_BLK_RCD[m_NbrTiers];
            for (int i = 0; i < m_CurrentRegisters.TierDataBlock.Length;i++ )
            {
                m_CurrentRegisters.TierDataBlock[i].Summations = new double[m_NbrSummations];
                m_CurrentRegisters.TierDataBlock[i].Demands = new DEMANDS_RCD[m_NbrDemands];
                for (int j = 0; j < m_CurrentRegisters.TierDataBlock[i].Demands.Length;j++ )
                {
                    if (DateTimeFlag)
                    {
                        m_CurrentRegisters.TierDataBlock[i].Demands[j].DemandTOO = new DateTime[m_NbrOccur];
                    }
                    m_CurrentRegisters.TierDataBlock[i].Demands[j].Demand = new double[m_NbrDemands];
                }
            }
        }

        #endregion Constructors

        #region Properties 

        /// <summary>(table 21) ACT_REGS_TBL.DATE_TIME_FIELD_FLAG</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public bool DateTimeFlag
        {
            get { return (0 != (m_RegFunc1Bfld & DATE_TIME_FLAG_MASK)); }
        }
 
        /// <summary>(table 21) ACT_REGS_TBL.DEMAND_RESET_CTR_FLAG</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public bool DemandResetCtrFlag
        {
            get { return (0 != (m_RegFunc1Bfld & DEMAND_RESET_CTR_FLAG_MASK)); }
        }

        /// <summary>(table 21) ACT_REGS_TBL.CUM_DEMAND_FLAG</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public bool CumDemandFlag
        {
            get { return (0 != (m_RegFunc1Bfld & CUM_DEMAND_FLAG_MASK)); }
        }

        /// <summary>(table 21) ACT_REGS_TBL.CONT_CUM_DEMAND_FLAG</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public bool CCumDemandFlag
        {
            get { return (0 != (m_RegFunc1Bfld & CCUM_DEMAND_FLAG_MASK)); }
        }

        /// <summary>(table 21) ACT_REGS_TBL.NBR_SUMMATIONS</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public byte NbrSummations
        {
            get { return m_NbrSummations; }
        }

        /// <summary>(table 21) ACT_REGS_TBL.NBR_DEMANDS</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public byte NbrDemands
        {
            get { return m_NbrDemands; }
        }

        /// <summary>(table 21) ACT_REGS_TBL.NBR_OCCUR</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public byte NbrOccur
        {
            get { return m_NbrOccur; }
        }

        /// <summary>(table 21) ACT_REGS_TBL.NBR_TIERS</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public byte NbrTiers
        {
            get { return m_NbrTiers; }
        }

        /// <summary>Accesses the number of demand resets from table 23</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public byte NbrDemandResets
        {
            get { return m_CurrentRegisters.NbrDemandResets; }
            set { m_CurrentRegisters.NbrDemandResets = value; }
        }

        /// <summary>Accesses the whole table structure</summary>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/15/07 mcm 8.10.05		Created
        /// </remarks>
        public REGISTER_DATA_RCD CurrentRegisterData
        {
            get { return m_CurrentRegisters; }
        }

        #endregion Properties

        #region Private Members

        private byte m_RegFunc1Bfld;
        private byte m_NbrSummations;
        private byte m_NbrDemands;
        private byte m_NbrOccur;
        private byte m_NbrTiers;
        private REGISTER_DATA_RCD m_CurrentRegisters;

        #endregion Private Members

    } // C1219_RegisterDataRcd
}
