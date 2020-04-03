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
using System.IO;
using System.Collections.Generic;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    class LIDWriter
    {
        #region Constants
        
        private const int LID_LENGTH = 4;
        private const int LID_WRITE_TABLE = 2051;

        #endregion
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">PSEM object for communication with the meter.</param>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 02/23/07 KRC 8.00.14 N/A	Created
        /// 
        public LIDWriter(CPSEM psem)
        {
            m_PSEM = psem;
        }

        /// <summary>
        /// Write LID
        /// </summary>
        /// <param name="lid">The Lid to Write</param>
        /// <param name="dblValue">The double value to write to the meter</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 8.00.14 N/A    Created
        //
        public PSEMResponse WriteLID(LID lid, Double dblValue)
        {

            byte[] Data = new byte[LID_LENGTH + sizeof(double)];
            Data.Initialize();
            MemoryStream DataStream = new MemoryStream(Data);
            PSEMBinaryWriter DataWriter = new PSEMBinaryWriter(DataStream);
            DataStream.Position = 0;
            DataWriter.Write(lid.lidValue);
            DataWriter.Write(dblValue);

            PSEMResponse Result = m_PSEM.FullWrite(LID_WRITE_TABLE, DataStream.ToArray());

            return Result;
        }

        /// <summary>
        /// Write LID
        /// </summary>
        /// <param name="lid">The Lid to Write</param>
        /// <param name="fltValue">The float value to write to the meter</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 8.00.14 N/A    Created
        //
        public PSEMResponse WriteLID(LID lid, Single fltValue)
        {

            byte[] Data = new byte[LID_LENGTH + sizeof(float)];
            Data.Initialize();
            MemoryStream DataStream = new MemoryStream(Data);
            PSEMBinaryWriter DataWriter = new PSEMBinaryWriter(DataStream);
            DataWriter.Write(lid.lidValue);
            DataWriter.Write(fltValue);

            PSEMResponse Result = m_PSEM.FullWrite(LID_WRITE_TABLE, Data);

            return Result;
        }

        /// <summary>
        /// Write LID
        /// </summary>
        /// <param name="lid">The Lid to Write</param>
        /// <param name="uiValue">The uint value to write to the meter</param>
        /// <returns>PSEMResponse</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/06 KRC 8.00.14 N/A    Created
        //
        public PSEMResponse WriteLID(LID lid, uint uiValue)
        {

            byte[] Data = new byte[LID_LENGTH + sizeof(uint)];
            Data.Initialize();
            MemoryStream DataStream = new MemoryStream(Data);
            PSEMBinaryWriter DataWriter = new PSEMBinaryWriter(DataStream);
            DataWriter.Write(lid.lidValue);
            DataWriter.Write(uiValue);

            PSEMResponse Result = m_PSEM.FullWrite(LID_WRITE_TABLE, Data);

            return Result;
        }
        #endregion

        #region Members

        protected CPSEM m_PSEM = null;

        #endregion
    }
}
