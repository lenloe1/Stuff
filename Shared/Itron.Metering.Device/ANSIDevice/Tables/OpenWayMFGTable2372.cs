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
//                           Copyright © 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Sub table into the 2372 factory data table to read/write Energy 1 Meter key Bit
    /// </summary>
    public class MFGTable2372Energy1MeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 0;
        private const ushort TABLE_LENGTH = 4;
        private const ushort ENERGY_1_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2372Energy1MeterKey(CPSEM psem)
            : base(psem, 2372, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Energy 1 Meter key.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Energy1MeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes until 
        /// this table is fully implemented.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the energy 1 meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 Energy1MeterKey
        {
            get
            {
                Read();
                return m_Energy1MeterKey;

            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_Energy1MeterKey;

        #endregion

    }

    /// <summary>
    /// Sub table into the 2372 factory data table to read Energy 2 Meter key Bit
    /// </summary>
    public class MFGTable2372Energy2MeterKey : ANSISubTable
    {
        #region Constants

        private const int TABLE_OFFSET = 4;
        private const ushort TABLE_LENGTH = 4;
        private const ushort ENERGY_2_KEY_OFFSET = 0;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psem">The current PSEM communications object.</param>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public MFGTable2372Energy2MeterKey(CPSEM psem)
            : base(psem, 2372, TABLE_OFFSET, TABLE_LENGTH)
        {
        }

        /// <summary>
        /// Reads the table from the meter.  Right now it just reads the Energy 2 Meter key.
        /// </summary>
        /// <returns>The result of the read.</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Read()
        {
            PSEMResponse Result;

            Result = base.Read();

            if (Result == PSEMResponse.Ok)
            {
                m_Energy2MeterKey = m_Reader.ReadUInt32();
            }

            return Result;
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting full writes.
        /// </summary>
        /// <returns>protocol response</returns>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write()
        {
            throw (new NotSupportedException("Write Not Supported!"));
        }

        /// <summary>
        /// Overrides the base class and returns an exception.  Not supporting offset writes.
        /// </summary>
        /// <param name="Offset">0 based byte offset to start writing from</param>
        /// <param name="Count">the number of bytes to write</param>
        /// <returns>protocol response</returns>
        /// <overloads>Write()</overloads>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public override PSEMResponse Write(ushort Offset, ushort Count)
        {
            throw (new NotSupportedException("Offset Write Not Supported!"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property gets and sets the energy 2 meter key bit.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who ID Version Issue# Description
        //  -------- --- -- ------- ------ -------------------------------------------
        //  08/15/16 jrf TC 4.70.14 63266  Created
        public UInt32 Energy2MeterKey
        {
            get
            {
                Read();
                return m_Energy2MeterKey;

            }
        }

        #endregion

        #region Member Variables

        private UInt32 m_Energy2MeterKey;

        #endregion

    }
}
