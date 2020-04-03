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
//                           Copyright © 2010 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Comm Module object for a PLAN Comm Module
    /// </summary>
    public class PLANCommModule : CommModuleBase
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session</param>
        /// <param name="amiDevice">The current device object.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created
        //  11/14/13 AF  3.50.03	    Class re-architecture - replace CENTRON_AMI parameter 
        //                              with CANSIDevice
        //
        public PLANCommModule(CPSEM psem, CANSIDevice amiDevice)
            : base(psem, amiDevice)
        {
            m_Table2194 = null;
            m_PLANTable2210 = null;
            m_PLANTable2211 = null;
            m_PLANTable2212 = null;
            m_PLANTable2215 = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the MAC Address of the Comm Module.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public override uint MACAddress
        {
            get
            {
                return (uint)LocalMACAddress;
            }
        }

        /// <summary>
        /// Gets whether or not the Comm Module is currently registered.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/08/10 RCG 2.40.23	146449 Created

        public override bool IsRegistered
        {
            get
            {
                // The more accurate way to read this is to check the registration
                // status which is in Vender Display Data for Itron Comm Modules
                return CommModuleRegistrationStatus.Equals("reg");
            }
        }

        /// <summary>
        /// Gets the stack version of the PLC module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string StackVersion
        {
            get
            {
                string strVersion = null;

                if (PLANTable2210 != null)
                {
                    strVersion = PLANTable2210.DSPFWVersion.ToString(CultureInfo.CurrentCulture) + "."
                        + PLANTable2210.DSPFWRevision.ToString(CultureInfo.CurrentCulture);
                }

                return strVersion;
            }
        }

        /// <summary>
        /// Gets the Factory data Version for the PLC Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string FactoryDataVersion
        {
            get
            {
                string strVersion = null;

                if (PLANTable2210 != null)
                {
                    strVersion = PLANTable2210.DSPEEPROMVersion.ToString(CultureInfo.CurrentCulture) + "."
                        + PLANTable2210.DSPEEPROMRevision.ToString(CultureInfo.CurrentCulture);
                }

                return strVersion;
            }
        }

        /// <summary>
        /// Gets the Boot Loader Version for the PLC Module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public string BootLoaderVersion
        {
            get
            {
                string strVersion = null;

                if (PLANTable2210 != null)
                {
                    strVersion = PLANTable2210.ARM7BootLoaderVersion.ToString(CultureInfo.CurrentCulture) + "."
                        + PLANTable2210.ARM7BootLoaderRevision.ToString(CultureInfo.CurrentCulture);
                }

                return strVersion;
            }
        }

        /// <summary>
        /// Gets the Max Transmitted Gain for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public PLANTxGain MaxTxGain
        {
            get
            {
                PLANTxGain Gain = PLANTxGain.dB0;

                if (PLANTable2211 != null)
                {
                    Gain = PLANTable2211.MaxTxGain;
                }

                return Gain;
            }
        }

        /// <summary>
        /// Gets the Max Recieved Gain for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public PLANGain MaxRxGain
        {
            get
            {
                PLANGain Gain = PLANGain.dB0;

                if (PLANTable2211 != null)
                {
                    Gain = PLANTable2211.MaxRxGain;
                }

                return Gain;
            }
        }

        /// <summary>
        /// Gets the Search Initiator Gain for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public PLANGain SearchInitiatorGain
        {
            get
            {
                PLANGain Gain = PLANGain.dB0;

                if (PLANTable2211 != null)
                {
                    Gain = PLANTable2211.SearchInitiatorGain;
                }

                return Gain;
            }
        }

        /// <summary>
        /// Gets the Repeater mode of the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public PLANRepeaterModes RepeaterMode
        {
            get
            {
                PLANRepeaterModes Mode = PLANRepeaterModes.NotRepeater;

                if (PLANTable2211 != null)
                {
                    Mode = PLANTable2211.RepeaterMode;
                }

                return Mode;
            }
        }

        /// <summary>
        /// Gets the Sync Confirm Timeout in seconds
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort SyncConfirmTimeout
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.SyncConfirmTimeout;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the Frame Not OK timeout in seconds
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort FrameNotOkTimeout
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.FrameNotOkTimeout;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the Not Addressed timeout in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort NotAddressedTimeout
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.NotAddressedTimeout;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the Not Addressed timeout in minutes
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort SearchInitiatorTimeout
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.SearchInitiatorTimeout;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the local MAC Address of the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort LocalMACAddress
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.LocalMACAddress;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the local system title for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte[] LocalSystemTitle
        {
            get
            {
                byte[] Title = null;

                if (PLANTable2211 != null)
                {
                    Title = PLANTable2211.LocalSystemTitle;
                }

                return Title;
            }
        }

        /// <summary>
        /// Gets the Search Initiator's MAC Addres for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public ushort InitiatorMACAddress
        {
            get
            {
                ushort usValue = 0;

                if (PLANTable2211 != null)
                {
                    usValue = PLANTable2211.InitiatorMACAddress;
                }

                return usValue;
            }
        }

        /// <summary>
        /// Gets the Search Initiator's system title for the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte[] InitiatorSystemTitle
        {
            get
            {
                byte[] Title = null;

                if (PLANTable2211 != null)
                {
                    Title = PLANTable2211.InitiatorSystemTitle;
                }

                return Title;
            }
        }

        /// <summary>
        /// Gets the Cell ID of the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte[] CellID
        {
            get
            {
                byte[] byCellID = null;

                if (PLANTable2215 != null)
                {
                    byCellID = PLANTable2215.CellID;
                }

                return byCellID;
            }
        }

        /// <summary>
        /// Gets the Electrical Phase Delta of the PLAN module
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public PLANElectricalPhase ElectricalPhaseDelta
        {
            get
            {
                PLANElectricalPhase Phase = PLANElectricalPhase.Unknown;

                if (PLANTable2212 != null)
                {
                    Phase = PLANTable2212.DeltaPhase;
                }

                return Phase;
            }
        }

        /// <summary>
        /// Gets the Link Attenuation (signal) for Channel 0 in dB
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte Channel0LinkAttenuation
        {
            get
            {
                byte byValue = 0;

                if (PLANTable2212 != null)
                {
                    byValue = PLANTable2212.Channel0LinkAttenuation;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Link Attenuation (signal) for Channel 1 in dB
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte Channel1LinkAttenuation
        {
            get
            {
                byte byValue = 0;

                if (PLANTable2212 != null)
                {
                    byValue = PLANTable2212.Channel1LinkAttenuation;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Noise Level for Channel 0 in dBuV
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte Channel0Noise
        {
            get
            {
                byte byValue = 0;

                if (PLANTable2212 != null)
                {
                    byValue = PLANTable2212.Channel0Noise;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Noise Level for Channel 1 in dBuV
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        public byte Channel1Noise
        {
            get
            {
                byte byValue = 0;

                if (PLANTable2212 != null)
                {
                    byValue = PLANTable2212.Channel1Noise;
                }

                return byValue;
            }
        }

        /// <summary>
        /// Gets the Credit Level
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/24/10 RCG 2.40.15		   Created

        public byte CreditLevel
        {
            get
            {
                byte byValue = 0;

                if (PLANTable2212 != null)
                {
                    byValue = PLANTable2212.CreditLevel;
                }

                return byValue;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Table 2194 Comm Module Table ID object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private OpenWayMFGTable2194 Table2194
        {
            get
            {
                if (m_Table2194 == null)
                {
                    m_Table2194 = new OpenWayMFGTable2194(m_PSEM);
                }

                return m_Table2194;
            }
        }

        /// <summary>
        /// Gets the PLAN Table 2210 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private PLANMFGTable2210 PLANTable2210
        {
            get
            {
                if (m_PLANTable2210 == null)
                {
                    m_PLANTable2210 = new PLANMFGTable2210(m_PSEM, Table2194);
                }

                return m_PLANTable2210;
            }
        }

        /// <summary>
        /// Gets the PLAN Table 2211 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private PLANMFGTable2211 PLANTable2211
        {
            get
            {
                if (m_PLANTable2211 == null)
                {
                    m_PLANTable2211 = new PLANMFGTable2211(m_PSEM, Table2194);
                }

                return m_PLANTable2211;
            }
        }

        /// <summary>
        /// Gets the PLAN Table 2212 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private PLANMFGTable2212 PLANTable2212
        {
            get
            {
                if (m_PLANTable2212 == null)
                {
                    m_PLANTable2212 = new PLANMFGTable2212(m_PSEM, Table2194);
                }

                return m_PLANTable2212;
            }
        }

        /// <summary>
        /// Gets the PLAN Table 2215 object
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/17/10 RCG 2.40.15		   Created

        private PLANMFGTable2215 PLANTable2215
        {
            get
            {
                if (m_PLANTable2215 == null)
                {
                    m_PLANTable2215 = new PLANMFGTable2215(m_PSEM, Table2194, m_AMIDevice.Table00);
                }

                return m_PLANTable2215;
            }
        }

        #endregion

        #region Member Variables

        private OpenWayMFGTable2194 m_Table2194;

        private PLANMFGTable2210 m_PLANTable2210;
        private PLANMFGTable2211 m_PLANTable2211;
        private PLANMFGTable2212 m_PLANTable2212;
        private PLANMFGTable2215 m_PLANTable2215;

        #endregion
    }
}
