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

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// This class provides the Prompt-for data that should be collected in order to 
    /// do an Initialize.
    /// </summary>
    public class PromptForData
    {
        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public PromptForData()
        {
            m_strSerialNumber = "";
            m_strUnitID = "";
            m_InitialDateTime = null;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to expose the setting and retrieving of the Unit ID
        /// </summary>
        public string UnitID
        {
            get
            {
                return m_strUnitID;
            }
            set
            {
                m_strUnitID = value;
            }
        }

        /// <summary>
        /// Gets the serial number
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return m_strSerialNumber;
            }
            set
            {
                m_strSerialNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Date and Time to use for initialization. If null the PC Date and Time will be used.
        /// </summary>

        public DateTime? InitialDateTime
        {
            get
            {
                return m_InitialDateTime;
            }
            set
            {
                m_InitialDateTime = value;
            }
        }

        #endregion

        #region Members

        private string m_strUnitID;
        private string m_strSerialNumber;
        private DateTime? m_InitialDateTime;

        #endregion
    }
}
