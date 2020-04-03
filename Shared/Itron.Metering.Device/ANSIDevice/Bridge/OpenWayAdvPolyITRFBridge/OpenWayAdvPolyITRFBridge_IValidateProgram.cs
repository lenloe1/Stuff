///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;

using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    partial class COpenWayAdvPolyITRFBridge
    {        
        #region Protected Methods

        /// <summary>
        /// Adds TOU/Time validation items to the validation list.
        /// </summary>
        /// <param name="ValidationList">The list to add validaiton items to.</param>
        // Revision History	
        // MM/DD/YY who Version ID Number Description
        // -------- --- ------- -- ------ ---------------------------------------
        // 12/09/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override void GetTOUTimeValidationItems(List<EDLValidationItem> ValidationList)
        {
            base.GetTOUTimeValidationItems(ValidationList);

            if (null != m_BridgeDevice)
            {
                m_BridgeDevice.Get25YearTOUValidationItems(ValidationList);
            }
        }

        /// <summary>
        /// Gets the list of tables to read from the meter during program validation.
        /// </summary>
        /// <returns>The list of Table IDs</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.

        protected override List<ushort> GetValidationTablesToRead()
        {
            List<ushort> TablesToRead = base.GetValidationTablesToRead();

            if (null != m_BridgeDevice)
            {
                TablesToRead.AddRange(m_BridgeDevice.GetValidationTablesToRead());
            }

            return TablesToRead;
        }

        /// <summary>
        /// Checks to see if the item matches and then creates a ProgramValidationItem if it does not.
        /// </summary>
        /// <param name="item">The item to validate</param>
        /// <param name="meterTables">The table structure for the meter.</param>
        /// <param name="programTables">The table structure for the program.</param>
        /// <returns>Returns the ProgramValidationItem for the value if the items do not match, and null if the values match.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/12/13 jrf 3.50.16 TQ 9560   Created.
        //
        protected override ProgramValidationItem ValidateItem(EDLValidationItem item, CentronTables meterTables, CentronTables programTables)
        {
            bool blnFoundItem = false;
            ProgramValidationItem InvalidItem = null;

            if (null != m_BridgeDevice)
            {
                blnFoundItem = m_BridgeDevice.ValidateItem(item, meterTables, programTables, out InvalidItem);
            }

            if (blnFoundItem == false)
            {
                // If we didn't find item check base method.
                InvalidItem = base.ValidateItem(item, meterTables, programTables);
            }

            return InvalidItem;
        }

        /// <summary>
        /// Updates the TOU for the program file.
        /// </summary>
        /// <param name="ProgramTables">Program data.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/17/13 jrf 3.50.16 TQ 9560   Created.
        // 01/08/14 jrf 3.50.22 TQ 9560 Making sure CentronTables's Create25YearCalendarFromStandardTables(...)
        //                              gets called beforeUpdateTOUSeasonFromStandardTables(...).
        //
        //
        protected override void UpdateTOU(CentronTables ProgramTables)
        {
            if (null != m_BridgeDevice)
            {
                m_BridgeDevice.UpdateTOU(ProgramTables);
            }

            base.UpdateTOU(ProgramTables);
        }

        #endregion
    }
}
