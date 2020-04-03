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
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    class SENTINELDisplayItem : ANSIDisplayItem
    {
        #region Constants

        private const float SENTINEL_SATURN_FW_REV = 5.0F;

        #endregion
        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        public SENTINELDisplayItem()
            : this(null, "", 0, 0, 0)
        {
        }

        /// <summary>
        /// Constructor for Display Item that can be called while reading Table 2048
        /// </summary>
        /// <param name="Lid">The LID for the given Display Item</param>
        /// <param name="strDisplayID">The Display ID for the given Display Item</param>
        /// <param name="usFormat">The Format Code for the given display item</param>
        /// <param name="byDim">The Dimension of the given display item</param>
        /// <param name="FWVersion">The Firmware Version of the meter we are talking to</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/07 KRC 8.00.22 N/A    Adding support for SENTINEL specific display item behavior
        //
        internal SENTINELDisplayItem(LID Lid, string strDisplayID, ushort usFormat, byte byDim, float FWVersion)
            : base(Lid, strDisplayID, usFormat, byDim)
        {
            m_FWVersion = FWVersion;
        }
            
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether or not the display item can be edited.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/28/07 KRC 8.00.22        Adding support for SENTINEL specific behavior
        public override bool Editable
        {
            get
            {
                bool bResult = false;

                if (m_FWVersion >= SENTINEL_SATURN_FW_REV)
                {
                    // FORMS firmware (3.x) and higher can edit Demand TOU Registers
                    if (true == m_LID.IsEnergy ||
                        true == m_LID.IsMaxDemand ||
                        true == m_LID.IsMinDemand)
                    {
                        // Make sure it is not one of the types we don't support.
                        if (false == m_LID.IsTOO && false == m_LID.IsSelfRead &&
                            false == m_LID.IsSnapshot && false == m_LID.IsLastSeason)
                        {
                            bResult = true;
                        }
                    }
                }
                else
                {
                    // Previous to FORMS (3.x) we could not edit the demand TOU registers
                    if (true == m_LID.IsEnergy ||
                        (true == m_LID.IsMaxDemand && false == m_LID.IsTOURate) ||
                        (true == m_LID.IsMinDemand && false == m_LID.IsTOURate))
                    {
                        // Make sure it is not one of the types we don't support.
                        if (false == m_LID.IsTOO && false == m_LID.IsSelfRead &&
                            false == m_LID.IsSnapshot && false == m_LID.IsLastSeason)
                        {
                            bResult = true;
                        }
                    }
                }

                return bResult;
            }
        }

        #endregion
        
        #region Member Variables

        private float m_FWVersion;

        #endregion
    }
}
