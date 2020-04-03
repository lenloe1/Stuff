///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
// All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
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
//                           Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// The ITRD single phase bridge meter device server class implementation of the IPreviousSeason interface.
    /// </summary>
    public partial class COpenWayITRDBridge : IPreviousSeason
    {
        #region Public Properties
        /// <summary>
        /// Gets whether the meter has any previous season data.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public bool HasPreviousSeasonData
        {
            get
            {
                return (null == PreviousSeasonRegisters) ? false : (PreviousSeasonRegisters.Count > 0);
            }
        }

        /// <summary>
        /// Gets the end date of the previous season.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public DateTime PreviousSeasonEndDate
        {
            get
            {            
                DateTime dtEndDate = DateTime.MinValue;

                if (null != m_BridgeDevice && null != m_BridgeDevice.PreviousSeasonEndDate)
                {
                    dtEndDate =m_BridgeDevice.PreviousSeasonEndDate.Value;
                }

                return dtEndDate;
            }
        }

        /// <summary>
        /// Proves access to a list of Energy Quantities from last season (Std table 24)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public List<Quantity> PreviousSeasonRegisters
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonRegisters;
            }
        }

        #region Previous Season Quantities

        /// <summary>
        /// Gets the previous season Neutral Amps from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonAmpsNeutral
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonAmpsNeutral;
            }
        }

        /// <summary>
        /// Gets the previous season Phase A Amps from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonAmpsPhaseA
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonAmpsPhaseA;
            }
        }

        /// <summary>
        /// Gets the previous season Phase B Amps from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonAmpsPhaseB
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonAmpsPhaseB;
            }
        }

        /// <summary>
        /// Gets the previous season Phase C Amps from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonAmpsPhaseC
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonAmpsPhaseC;
            }
        }

        /// <summary>
        /// Gets the previous season Amps squared from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonAmpsSquared
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonAmpsSquared;
            }
        }

        /// <summary>
        /// Gets the previous season Q Delivered from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonQDelivered
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonQDelivered;
            }
        }

        /// <summary>
        /// Gets the previous season Qh Received from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonQReceived
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonQReceived;
            }
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVADelivered
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVADelivered;
            }
        }

        /// <summary>
        /// Gets the previous season Lagging VA from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVALagging
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVALagging;
            }
        }

        /// <summary>
        /// Gets the previous season Var Delivered from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarDelivered
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarDelivered;
            }
        }

        /// <summary>
        /// Gets the previous season VA Received from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVAReceived
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVAReceived;
            }
        }

        /// <summary>
        /// Gets the previous season Var Net from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarNet
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarNet;
            }
        }

        /// <summary>
        /// Gets the previous season Var Net delivered from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarNetDelivered
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarNetDelivered;
            }
        }

        /// <summary>
        /// Gets the previous season Var Net Received from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarNetReceived
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarNetReceived;
            }
        }

        /// <summary>
        /// Gets the previous season Var Q1 from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarQuadrant1
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarQuadrant1;
            }
        }

        /// <summary>
        /// Gets the previous season Var Q2 from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarQuadrant2
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarQuadrant2;
            }
        }

        /// <summary>
        /// Gets the previous season Var Q3 from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarQuadrant3
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarQuadrant3;
            }
        }

        /// <summary>
        /// Gets the previous season Var Q4 from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarQuadrant4
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarQuadrant4;
            }
        }

        /// <summary>
        /// Gets the previous season Var Received from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVarReceived
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVarReceived;
            }
        }

        /// <summary>
        /// Gets the previous season Average Volts from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVoltsAverage
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVoltsAverage;
            }
        }

        /// <summary>
        /// Gets the previous season Phase A Volts from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVoltsPhaseA
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVoltsPhaseA;
            }
        }

        /// <summary>
        /// Gets the previous season Phase B Volts from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVoltsPhaseB
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVoltsPhaseB;
            }
        }

        /// <summary>
        /// Gets the previous season Phase C Volts from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVoltsPhaseC
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVoltsPhaseC;
            }
        }

        /// <summary>
        /// Gets the previous season Volts squared from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonVoltsSquared
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonVoltsSquared;
            }
        }

        /// <summary>
        /// Gets the previous season Watts Delivered quantity from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
       
        public Quantity PreviousSeasonWattsDelivered
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonWattsDelivered;
            }
        }

        /// <summary>
        /// Gets the previous season Watts Received quantity from the standard tables
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonWattsReceived
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonWattsReceived;
            }
        }

        /// <summary>
        /// Gets the previous season Watts Net quantity from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonWattsNet
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonWattsNet;
            }
        }

        /// <summary>
        /// Gets the previous season Unidirectional Watts from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/06/13 jrf 3.50.12 TQ9556	Created
        
        public Quantity PreviousSeasonWattsUni
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonWattsUni;
            }
        }

        /// <summary>
        /// Gets the previous season PowerFactor from the standard tables.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/17/14 jrf 3.50.26 TQ9556	Created

        public Quantity PreviousSeasonPowerFactor
        {
            get
            {
                return (null == m_BridgeDevice) ? null : m_BridgeDevice.PreviousSeasonPowerFactor;
            }
        }

        #endregion

        #endregion
    }

}

