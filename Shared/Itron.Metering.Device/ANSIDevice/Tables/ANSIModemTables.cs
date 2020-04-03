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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region Standard Table 91

    /// <summary>
    /// Table 91 - Actual Telephone Limiting 
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  02/14/07 RCG 8.00.12        Created

    internal class StdTable91 : AnsiTable
    {
        #region Constants

        private const int TABLE91_SIZE = 14;
        private const byte BIT_RATE_MASK = 0x18;

        // Telephone Flag masks
        private const byte ANSWER_CALL_MASK = 0x01;
        private const byte NO_LOCKOUT_MASK = 0x40;

        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration for the options for changing the 
        /// bit rate
        /// </summary>
        public enum BitRateTypes : byte
        {
            /// <summary>
            /// The bit rate can not be controlled
            /// </summary>
            NotControlled = 0x00,
            /// <summary>
            /// The bit rate is controlled globally. The setting for this
            /// is stored in Standard Table 92
            /// </summary>
            GloballyControlled = 0x08,
            /// <summary>
            /// The bit rate can be controlled seperately. These settings are
            /// stored in Standard Table 93
            /// </summary>
            SeperatelyControlled = 0x10,
            /// <summary>
            /// Reserved
            /// </summary>
            Reserved = 0x18,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM object for the current session</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public StdTable91(CPSEM psem)
            : base(psem, 91, TABLE91_SIZE)
        {
        }

        /// <summary>
        /// Reads the table from the meter and populates the data
        /// </summary>
        /// <returns>The PSEM response for the read of the table.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable91.Read");

            // Read the table
            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                // Parse the data that has been read.
                
                m_byTelephoneFlags = m_Reader.ReadByte();
                m_byNumOriginateWindows = m_Reader.ReadByte();
                m_byNumSetupStrings = m_Reader.ReadByte();
                m_bySetupStringLength = m_Reader.ReadByte();
                m_byPrefixLength = m_Reader.ReadByte();
                m_byNumOriginateNumbers = m_Reader.ReadByte();
                m_byPhoneNumberLength = m_Reader.ReadByte();
                m_byNumRecurringDates = m_Reader.ReadByte();
                m_byNumNonRecurringDates = m_Reader.ReadByte();
                m_byNumEvents = m_Reader.ReadByte();
                m_byNumWeeklySchedules = m_Reader.ReadByte();
                m_byNumAnswerWindows = m_Reader.ReadByte();
                m_byNumCallerIDs = m_Reader.ReadByte();
                m_byCallerIDLength = m_Reader.ReadByte();

            }

            return Response;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of originating phone numbers programmed
        /// into the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public int NumberOfPhoneNumbers
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byNumOriginateNumbers;
            }
        }

        /// <summary>
        /// Gets the length, in number of octets, of the phone numbers stored
        /// in Table 93
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public int PhoneNumberLength
        {
            get
            {
                if(State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if(Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response, 
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byPhoneNumberLength;
            }
        }

        /// <summary>
        /// Gets the length of the Prefix, in number of octets
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public int PrefixLength
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byPrefixLength;
            }
        }

        /// <summary>
        /// Gets the Bit Rate settings for the ANSI Modem
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public BitRateTypes BitRate
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                // Extract the bit rate information from the telephone flags

                return (BitRateTypes)(m_byTelephoneFlags & BIT_RATE_MASK);
            }
        }

        /// <summary>
        /// Gets the number of Originate Windows for the modem configuration
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public int NumberOfOriginateWindows
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byNumOriginateWindows;
            }
        }

        /// <summary>
        /// Gets whether or not the device is set up to answer phone calls
        /// through the modem.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public bool AnswerCall
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                // Determine if the Answer flag is set
                return (m_byTelephoneFlags & ANSWER_CALL_MASK) == ANSWER_CALL_MASK;
            }
        }

        /// <summary>
        /// Returns true if the meter does not support lockout parameters, and false
        /// if the meter does support lockout parameters
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public bool NoLockoutParameters
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                // Determine if the No Lockout flag is set
                return (m_byTelephoneFlags & NO_LOCKOUT_MASK) == NO_LOCKOUT_MASK;
            }
        }

        /// <summary>
        /// Gets the number of answer call windows stored in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public int NumberOfAnswerWindows
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byNumAnswerWindows;
            }
        }

        /// <summary>
        /// Gets the number of caller IDs stored in the meter
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public int NumberOfCallerIDs
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byNumCallerIDs;
            }
        }

        /// <summary>
        /// Gets the length in bytes of a caller ID
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public int CallerIDLength
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 91");
                    }
                }

                return (int)m_byCallerIDLength;
            }
        }

        #endregion

        #region Member Variables

        private byte m_byTelephoneFlags;
        private byte m_byNumOriginateWindows;
        private byte m_byNumSetupStrings;
        private byte m_bySetupStringLength;
        private byte m_byPrefixLength;
        private byte m_byNumOriginateNumbers;
        private byte m_byPhoneNumberLength;
        private byte m_byNumRecurringDates;
        private byte m_byNumNonRecurringDates;
        private byte m_byNumEvents;
        private byte m_byNumWeeklySchedules;
        private byte m_byNumAnswerWindows;
        private byte m_byNumCallerIDs;
        private byte m_byCallerIDLength;

        #endregion
    }

    #endregion

    #region Standard Table 93

    /// <summary>
    /// Standard Table 93 - Originate Communication Parameters
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  02/14/07 RCG 8.00.12        Created

    internal class StdTable93 : AnsiTable
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM object for the current session.</param>
        /// <param name="table00">The Table 0 object for the current device.</param>
        /// <param name="table91">The Table 91 object for the current device.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public StdTable93(CPSEM psem, CTable00 table00, StdTable91 table91)
            : base(psem, 93, StdTable93.DetermineTableSize(table00, table91))
        {
            m_Table00 = table00;
            m_Table91 = table91;

            // Determine the size of the WINDOW_RCD record
            m_uiWindowRcdSize = table00.STIMESize * 2 + 1;
        }

        /// <summary>
        /// Reads the table from the meter.
        /// </summary>
        /// <returns>The PSEM Reponse as a result of the read</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable93.Read");

            // Read the table
            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                // Parse the data that has been read.
                if (m_Table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
                {
                    m_uiOriginateBitRate = m_Reader.ReadUInt32();
                }

                m_byDialDelay = m_Reader.ReadByte();
                m_strPrefix = m_Reader.ReadString(m_Table91.PrefixLength);

                // Read each of the phone numbers
                m_astrPhoneNumbers = new string[m_Table91.NumberOfPhoneNumbers];

                for (int iIndex = 0; iIndex < m_Table91.NumberOfPhoneNumbers; iIndex++)
                {
                    m_astrPhoneNumbers[iIndex] = m_Reader.ReadString(m_Table91.PhoneNumberLength);
                }

                // Currently we do not need the Originate Windows items so we can just read them as
                // filler for now
                m_Filler = m_Reader.ReadBytes((int)(m_Table91.NumberOfOriginateWindows * m_uiWindowRcdSize));
            }

            return Response;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The PSEM response code for the write.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StdTable93.Write");

            // Write all of the data back to the stream so we
            // write the data that we have stored.

            m_DataStream.Position = 0;

            if (m_Table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
            {
                m_Writer.Write(m_uiOriginateBitRate);
            }

            m_Writer.Write(m_byDialDelay);
            m_Writer.Write(m_strPrefix, m_Table91.PrefixLength);

            // Write each of the phone numbers
            for (int iIndex = 0; iIndex < m_Table91.NumberOfPhoneNumbers; iIndex++)
            {
                m_Writer.Write(m_astrPhoneNumbers[iIndex], m_Table91.PhoneNumberLength);
            }

            // Write the remaining data
            m_Writer.Write(m_Filler);

            return base.Write();
        }

        /// <summary>
        /// Determines the size of a Window record field
        /// </summary>
        /// <param name="table00">The table 0 object for the current device.</param>
        /// <returns>The size in bytes of the Window record.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public static uint WindowRcdSize(CTable00 table00)
        {
            uint uiWindowRcdSize = 0;
            // Determine the size of the WINDOW_RCD

            // BEGIN_WINDOW_TIME : STIME
            uiWindowRcdSize += table00.STIMESize;

            // WINDOW_DURATION : STIME
            uiWindowRcdSize += table00.STIMESize;

            // WINDOW_DAYS : DAYS_BFLD (UINT8)
            uiWindowRcdSize += 1;

            return uiWindowRcdSize;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Phone Number Prefix
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        public string Prefix
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 93");
                    }
                }

                return m_strPrefix;
            }
            set
            {
                // Make sure that we have the other information so that we do not clear out
                // information when we go to write the data
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 93");
                    }
                }

                m_strPrefix = value;
            }
        }

        /// <summary>
        /// Gets the list of phone numbers from the meter.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/20/07 RCG 8.00.13        Created

        public string[] PhoneNumbers
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 93");
                    }
                }

                return m_astrPhoneNumbers;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the size of table 93
        /// </summary>
        /// <param name="table00">Standard Table 0 object for the current device.</param>
        /// <param name="table91">Standard Table 91 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 RCG 8.00.12        Created

        private static uint DetermineTableSize(CTable00 table00, StdTable91 table91)
        {
            uint uiTableSize = 0;
            uint uiWindowRcdSize = 0;

            // If the Bit Rate is set up to be seperately controlled this table will
            // contain the originate bit rate
            if (table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
            {
                // ORIGINATE_BIT_RATE : UINT32
                uiTableSize += 4; 
            }

            // DIAL_DELAY : UINT8
            uiTableSize += 1; 

            // Add in the Originate Phone Number record

            // PHONE_NUMBERS_RCD.PREFIX : ARRAY of CHAR
            uiTableSize += (uint)table91.PrefixLength;

            // PHONE_NUMBERS_RCD.NBR_ORIGINATE_NUMBERS : ARRAY of PHONE_NUMBER_RCD
            uiTableSize += (uint)(table91.NumberOfPhoneNumbers * table91.PhoneNumberLength);

            uiWindowRcdSize = WindowRcdSize(table00);

            // Add in the Originate Windows records

            // WINDOWS : ARRAY of WINDOW_RCD
            uiTableSize += (uint)table91.NumberOfOriginateWindows * uiWindowRcdSize;

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table00;
        private StdTable91 m_Table91;

        private uint m_uiWindowRcdSize;

        private uint m_uiOriginateBitRate;
        private byte m_byDialDelay;
        private string m_strPrefix;
        private string[] m_astrPhoneNumbers;
        private byte[] m_Filler;

        #endregion
    }

    #endregion

    /// <summary>
    /// Standard Table 95 - Answer Communication Parameters
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  02/21/07 RCG 8.00.12        Created

    internal class StdTable95 : AnsiTable
    {

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">The PSEM communications object for the current session.</param>
        /// <param name="table00">The table 0 object for the current device.</param>
        /// <param name="table91">the table 91 object for the current device.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public StdTable95(CPSEM psem, CTable00 table00, StdTable91 table91)
            : base(psem, 95, StdTable95.DetermineTableSize(table00, table91))
        {
            m_Table00 = table00;
            m_Table91 = table91;
        }

        /// <summary>
        /// Reads the table from the meter and populates the data fields.
        /// </summary>
        /// <returns>The PSEM response code for the read request.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public override PSEMResponse Read()
        {
            PSEMResponse Response = PSEMResponse.Ok;
            uint uiFillerBytes = 0;

            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "StdTable95.Read");

            // Read the table
            Response = base.Read();

            if (Response == PSEMResponse.Ok)
            {
                m_DataStream.Position = 0;

                // Parse the data that has been read.
                if (m_Table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
                {
                    // Read the Answer bit rate only if the bit rate is seperately controlled
                    m_uiAnswerBitRate = m_Reader.ReadUInt32();
                }

                if (m_Table91.NoLockoutParameters == false)
                {
                    // Read the lockout parameters only if the lockout parameters are available
                    m_byLockoutDelay = m_Reader.ReadByte();
                    m_byRetryAttempts = m_Reader.ReadByte();
                    m_byRetryLockoutTime = m_Reader.ReadByte();
                }

                m_byNumberOfRings = m_Reader.ReadByte();

                if (m_Table91.NumberOfAnswerWindows > 0)
                {
                    // Only read the Outside call window rings if answer windows are used
                    m_byNumberOfOutsideRings = m_Reader.ReadByte();
                }

                // Since we currently do not need the remaining data we are going to read this
                // information in as filler

                // Caller ID information
                uiFillerBytes += (uint)(m_Table91.NumberOfCallerIDs * m_Table91.CallerIDLength);

                // Windows information
                uiFillerBytes += (uint)(m_Table91.NumberOfAnswerWindows * StdTable93.WindowRcdSize(m_Table00));

                m_byFiller = m_Reader.ReadBytes((int)uiFillerBytes);
            }

            return Response;
        }

        /// <summary>
        /// Writes the table to the meter.
        /// </summary>
        /// <returns>The PSEM response code for the write request.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public override PSEMResponse Write()
        {
            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                "StdTable95.Write");

            // Write all of the data back to the stream so we
            // write the data that we have stored.

            m_DataStream.Position = 0;

            if (m_Table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
            {
                // Write the Answer bit rate only if the bit rate is seperately controlled
                m_Writer.Write(m_uiAnswerBitRate);
            }

            if (m_Table91.NoLockoutParameters == false)
            {
                // Write the lockout parameters only if the lockout parameters are available
                m_Writer.Write(m_byLockoutDelay);
                m_Writer.Write(m_byRetryAttempts);
                m_Writer.Write(m_byRetryLockoutTime);
            }

            m_Writer.Write(m_byNumberOfRings);

            if (m_Table91.NumberOfAnswerWindows > 0)
            {
                // Only write the Outside call window rings if answer windows are used
                m_Writer.Write(m_byNumberOfOutsideRings);
            }

            // Write the remaining data
            m_Writer.Write(m_byFiller);

            return base.Write();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of rings to wait before answering when inside the call
        /// window.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public byte InsideWindowRings
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 95");
                    }
                }

                return m_byNumberOfRings;
            }
            set
            {
                if (State == TableState.Unloaded)
                {
                    // Make sure that we have read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 95");
                    }
                }

                m_byNumberOfRings = value;
            }
        }

        /// <summary>
        /// Gets the number of rings to wait before answering when outside of the 
        /// call window. This property is only valid if the number of call windows 
        /// is greater than 0.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.12        Created

        public byte OutsideWindowRings
        {
            get
            {
                if (State == TableState.Unloaded)
                {
                    // Read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 95");
                    }
                }

                return m_byNumberOfOutsideRings;
            }
            set
            {
                if (State == TableState.Unloaded)
                {
                    // Make sure that we have read the table first
                    PSEMResponse Response = Read();

                    if (Response != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Response,
                            "Error reading Standard Table 95");
                    }
                }

                m_byNumberOfOutsideRings = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the size in bytes of standard table 95
        /// </summary>
        /// <param name="table00">The table 0 object for the current device.</param>
        /// <param name="table91">The table 91 object for the current device.</param>
        /// <returns>The size of the table in bytes.</returns>

        private static uint DetermineTableSize(CTable00 table00, StdTable91 table91)
        {
            uint uiTableSize = 0;

            // If the bit rate is set up to be seperately controlled then this
            // table will contain the answer baud rate
            if (table91.BitRate == StdTable91.BitRateTypes.SeperatelyControlled)
            {
                // ANSWER_BIT_RATE : UINT32
                uiTableSize += 4;
            }

            // If the meter supports lockout parameters we need to include the lockout
            // parameters. Notice the flag is true if there are no parameters and false
            // if there are lockout parameters.
            if (table91.NoLockoutParameters == false)
            {
                // LOCKOUT_DELAY        : UINT8
                // RETRY_ATTEMPTS       : UINT8
                // RETRY_LOCKOUT_TIME   : UINT8
                uiTableSize += 3;
            }

            // NBR_RINGS : UINT8
            uiTableSize += 1;

            // If the number of answer windows is greater than zero then the meter contains
            // the field for rings outside of the call window
            if (table91.NumberOfAnswerWindows > 0)
            {
                // NBR_RINGS_OUTSIDE : UINT8
                uiTableSize += 1;
            }

            // CALLER_IDS : ARRAY of CALLER_ID_RCD
            uiTableSize += (uint)(table91.NumberOfCallerIDs * table91.CallerIDLength);

            // WINDOWS : ARRAY of WINDOW_RCD
            uiTableSize += (uint)(table91.NumberOfAnswerWindows * StdTable93.WindowRcdSize(table00));

            return uiTableSize;
        }

        #endregion

        #region Member Variables

        private CTable00 m_Table00;
        private StdTable91 m_Table91;

        private uint m_uiAnswerBitRate;
        private byte m_byLockoutDelay;
        private byte m_byRetryAttempts;
        private byte m_byRetryLockoutTime;
        private byte m_byNumberOfRings;
        private byte m_byNumberOfOutsideRings;
        private byte[] m_byFiller;

        #endregion
    }
}
