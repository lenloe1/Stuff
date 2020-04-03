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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Itron.Metering.Communications;
using Itron.Metering.Communications.SCS;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the FULCRUM meter.
    /// </summary>
    /// <remarks>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/27/06 mrj 7.30.00  N/A	Created
    /// 05/26/06 jrf 7.30.00  N/A	Modified
    /// 12/18/06 mah 8.00.00  N/A	Added entry points for display list properties
    /// </remarks>
    public partial class FULCRUM : SCSDevice
    {

        /// <summary>
        /// Provides access to Normal Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/18/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> NormalDisplayList
        {
            get
            {
                throw( new NotImplementedException( "The normal display list property is not implemented for this device" ));
            }
        }

        /// <summary>
        /// Provides access to Alternatel Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/18/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> AlternateDisplayList
        {
            get
            {
                throw (new NotImplementedException("The alternate display list property is not implemented for this device"));
            }
        }

        /// <summary>
        /// Provides access to Alternatel Display List
        /// </summary>
        /// <returns>
        /// List of DisplayItems.  
        /// </returns> 
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  12/18/06 MAH  8.00.00			Created 
        /// </remarks>
        override public List<DisplayItem> TestDisplayList
        {
            get
            {
                throw( new NotImplementedException( "The test mode display list property is not implemented for this device" ));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayItem"></param>
        /// <returns></returns>
        override internal int TranslateDisplayAddress(SCSDisplayItem displayItem)
        {
            int nAddress;

            switch (displayItem.UpperAddress)
            {
                case 0x08: nAddress = (0x0100 | displayItem.LowerAddress);
                    break;
                case 0x09: nAddress = (0x0200 | displayItem.LowerAddress);
                    break;
                case 0x0A: nAddress = (0x0300 | displayItem.LowerAddress);
                    break;
                case 0x0B: nAddress = (0x0400 | displayItem.LowerAddress);
                    break;
                default:
                    nAddress = displayItem.LowerAddress;
                    break;
            }

            return nAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nBasepageAddress"></param>
        /// <returns></returns>
        override internal string GetDisplayItemDescription(int nBasepageAddress)
        {
            return "";
        }
        /// <summary>
        /// This method is responsible for either retrieving or calculating the continuous
        /// cummulative demand value associated with the given display item.  Note
        /// that this method is not currently implemented and will throw an exception
        /// </summary>
        /// <param name="displayItem">The display item to look up</param>
        /// <returns>A string representing the ccum value
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/03/07 mah 8.00.00  N/A   Created
        /// </remarks>
        override internal String RetrieveCCumValue(SCSDisplayItem displayItem)
        {
            throw (new NotImplementedException("Continuous cumulative display items are yet not implemented for this device"));
        }


    }
}
