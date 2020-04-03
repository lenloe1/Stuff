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
//                           Copyright © 2006 - 2013
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.IO;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Class representing the SENTINEL.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------
    //  05/22/06 mrj 7.30.00 N/A    Created
    //  04/02/07 AF  8.00.23 2814    Corrected the capitalization of the meter name
    //  11/11/13 AF  3.50.02         Changed the parent class from CANSIDevice to ANSIMeter
    //
    public partial class SENTINEL : ANSIMeter, ISiteScan, IERTCheck, ICustomSchedule
    {
        #region Constants

        private const uint CLOSE_CONFIG_CONST_ERROR_MASK = 0x00000002;
        private const uint CLOSE_CONFIG_BILL_ERROR_MASK = 0x00200000;

        /// <summary>
        /// Meter type identifier
        /// </summary>
        internal const string SENTINEL_METER_TYPE = "SENTINEL";
        /// <summary>
        /// Human readable name of meter
        /// </summary>
        private const string SENTINEL_NAME = "SENTINEL";

		//R300 Option Board IDs
		private const int R300_1_ERT = 3001;
		private const int R300_2_ERT = 3002;
		private const int R300_3_ERT = 3003;

		private const byte R300_TOTAL_FORMAT_MASK		= 0x70;
		private const byte R300_DECIMAL_FORMAT_MASK		= 0x0F;
		private const byte OPT_DATE_MDY = 0;
		private const byte OPT_DATE_DMY = 3;
		private const byte OPT_DATE_YMD = 4;

		private const float SENTINEL_SATURN_FW_REV = 5.0F;
        private const int MFG_SECURITY_TABLE = 2052;
        private const byte TERTIARY_PASSWORD_LEVEL = 1;
        private const byte SECONDARY_PASSWORD_LEVEL = 2;
        private const byte LIMITED_RECONFIGURE_PASSWORD_LEVEL = 3;
        private const byte PRIMARY_PASSWORD_LEVEL = 4;

        private const int MAX_CAL_EVENTS = 44;
        private const int FIRST_TOU_CAL_INDEX = 2; // 0 & 1 are reserved for DST

        /// <summary>
        /// Modem Prefix string for no prefix
        /// </summary>
        public const string MODEM_PREFIX_NONE = "";
        /// <summary>
        /// Modem Prefix string for a prefix of 8
        /// </summary>
        public const string MODEM_PREFIX_8 = "8,";
        /// <summary>
        /// Modem prefix string for a prefix of 9
        /// </summary>
        public const string MODEM_PREFIX_9 = "9,";

        //Masks for diagnostics active
        private const byte SS_DIAG_1_MASK = 0x01;
        private const byte SS_DIAG_2_MASK = 0x02;
        private const byte SS_DIAG_3_MASK = 0x04;
        private const byte SS_DIAG_4_MASK = 0x08;
        private const byte SS_DIAG_5_MASK = 0x10;
        private const byte SS_DIAG_6_MASK = 0x20;


        #endregion

        #region Definitions

        /// <summary>
        /// Enumeration for the type of the answer delay that
        /// the meter is currently using.
        /// </summary>

        public enum ModemAnswerDelayType
        {
            /// <summary>
            /// The delay type is unavailable. This usually
            /// means that a modem is not configured.
            /// </summary>
            Unavailable = 0,
            /// <summary>
            /// The answer delays are in seconds.
            /// </summary>
            Seconds = 1,
            /// <summary>
            /// The answer delays are in rings.
            /// </summary>
            Rings = 2,
        }

        /// <summary>
        /// Enumeration for the type of I/O board installed.
        /// </summary>
        public enum IOBoardType: uint
        {
            /// <summary>
            /// No IO board installed.
            /// </summary>
            [EnumDescription("None Installed")]
            NoneInstalled = 0x0000,
            /// <summary>
            /// Solid State Contacts with 2KYZ and 1 LC.
            /// </summary>
            [EnumDescription("2KYZ, 1LC")]
            SSC_2KYZ1KY = 0x0250,
            /// <summary>
            /// Solid State Contacts with 4KYZ and 1 LC.
            /// </summary>
            [EnumDescription("4KYZ, 1LC")]
            SSC_4KYZ1KY = 0x0255,
            /// <summary>
            /// Solid State Contacts with 2KYZ, 1 LC and 2 Pulse/State Inputs.
            /// </summary>
            [EnumDescription("2KYZ, 1LC and 2 Pulse/State Inputs")]
            SSC_2KYZ1KY2I = 0x0E50,
            /// <summary>
            /// Solid State Contacts with 4KYZ, 1 LC and 2 Pulse/State Inputs.
            /// </summary>
            [EnumDescription("4KYZ, 1LC and 2 Pulse/State Inputs")]
            SSC_4KYZ1KY2I = 0x0E55,
            /// <summary>
            /// Unknown IO board type.
            /// </summary>
            [EnumDescription("Unknown IO Board")]
            UNKNOWN = 0xFFFF
        }

        #endregion

        #region Public Methods

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ceComm"></param>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00 N/A    Created
		///
		public SENTINEL( Itron.Metering.Communications.ICommunications ceComm )
			: base(ceComm) 
		{ 
			//Use the Sentinel LIDs
			m_LID = new SentinelDefinedLIDs();
			
			//Get the resource manager
			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly );
			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="PSEM">PSEM object for this session</param>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		///
		public SENTINEL( CPSEM PSEM )
			: base(PSEM) 
		{ 
			//Use the Sentinel LIDs
			m_LID = new SentinelDefinedLIDs();
			
			//Get the resource manager
			m_rmStrings = new ResourceManager( RESOURCE_FILE_PROJECT_STRINGS, this.GetType().Assembly );
			
		}

        /// <summary>
        /// The PasswordReconfigResult reconfigures passwords. 
        /// </summary>
        /// <param name="Passwords">A list of passwords to write to the meter. 
        /// The Primary password should be listed first followed by the secondary
        /// password and so on.  Use empty strings for null passwords.  Passwords
        /// will be truncated or null filled as needed to fit in the device.</param>
        /// <returns>A PasswordReconfigResult object</returns>
        /// 
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 08/21/06 mcm 7.35.00 N/A    Created
        ///	
        public override PasswordReconfigResult ReconfigurePasswords(
                            System.Collections.Generic.List<string> Passwords)
        {
            // In the Saturn project the mechanism for recofiguring passwords was
            // changed from using MFG table 4 to STD table 42. Saturn meters only
            // partially support reconfiguration via MFG table 4.

            if (SENTINEL_SATURN_FW_REV > FWRevision)
            {
                return MFGReconfigurePasswords(Passwords);
            }
            else
            {
                return STDReconfigurePasswords(Passwords);
            }

        } // ReconfigurePasswords

        /// <summary>
        /// Reconfigures the tertiary password in the meter.
        /// </summary>
        /// <param name="strTertiaryPassword">The new tertiary password.</param>
        /// <returns>The result of the reconfigure.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/22/07 RCG 8.00.13    N/A Created

        public override PasswordReconfigResult ReconfigureTertiaryPassword(string strTertiaryPassword)
        {
            // In the Saturn project the mechanism for recofiguring passwords was
            // changed from using MFG table 4 to STD table 42. Saturn meters only
            // partially support reconfiguration via MFG table 4.

            if (SENTINEL_SATURN_FW_REV > FWRevision)
            {
                return MFGReconfigureTertiaryPassword(strTertiaryPassword);
            }
            else
            {
                return STDReconfigureTertiaryPassword(strTertiaryPassword);
            }
        }

        /// <summary>
        /// Reads the first NbrEvents from table 2072 and returns them in a 
        /// list. Only rereads the table if Refresh is true;
        /// </summary>
        /// <param name="StartIndex">0 based event index</param>
        /// <param name="Count">Number of VQ events to read</param>
        /// <returns>A list of PQ events</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/12/07 mcm 8.0.11  N/A    Created
        public List<PowerQualityEvent> GetPQEvents(int StartIndex, int Count)
        {
            if (StartIndex + Count <= Table2071.NbrEvents)
            {
                return Table2072.GetEvents(StartIndex, Count);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clears all entries from the VQ Log
        /// </summary>
        /// <returns>A ItronDeviceResult indicating result</returns>
        /// 
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/14/07 mcm 8.0.11  N/A    Created
        // 03/09/07 mcm 8.00.17 SCR 2530, 2533 unhandled exception
        // 03/09/07 mcm 8.00.17 SCR 2574 Total number of events not reset on clear VQ
        public ItronDeviceResult ClearVQLoq()
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            ProcParam = new byte[0];  // No parameters for this procedure

            try
            {
                ProcResult = ExecuteProcedure(Procedures.CLEAR_VQ_LOG,
                    ProcParam, out ProcResponse);
            }
            catch
            {
                // mcm 3/9/2007 - SCRs 2530 & 2533.
                // This is a protocol or timeout error.  All C12.19 devices 
                // should support reading table 8 and writing table 7.
                return ItronDeviceResult.ERROR;
            }

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                {
                    //Success
                    Result = ItronDeviceResult.SUCCESS;

                    // Mark the VQ status table as dirty so it'll get reread on access
                    Table2071.State = AnsiTable.TableState.Dirty;
                    break;
                }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                {
                    //Isc error
                    Result = ItronDeviceResult.SECURITY_ERROR;
                    break;
                }
                default:
                {
                    //General Error
                    Result = ItronDeviceResult.ERROR;
                    break;
                }
            }
			        
			return Result;

        } // ClearVQLoq

        /// <summary>
        /// Reconfigures the modem answer delays in the meter.
        /// </summary>
        /// <param name="byInsideWindowDelay">The delay for a call insude the call window.</param>
        /// <param name="byOutsideWindowDelay">The delay for a call outside the call window.</param>
        /// <returns>The result of the reconfigure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/21/07 RCG 8.00.13        Created
        //  03/08/07 RCG 8.00.17 2480   Fixing issue where the Outside Window Delay was
        //                              being changed when Outside Window answering is disabled.

        public ItronDeviceResult ReconfigureModemAnswerDelays(byte byInsideWindowDelay, byte byOutsideWindowDelay)
        {
            PSEMResponse Response;
            ItronDeviceResult ReconfigResult = ItronDeviceResult.ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            if (OptionBoardModemConfig != null)
            {
                // Handle reconfigure for the SENTINEL Modem

                OptionBoardModemConfig.InsideWindowSeconds = byInsideWindowDelay;

                if (OptionBoardModemConfig.AnswerOutsideWindow == true)
                {
                    OptionBoardModemConfig.OutsideWindowSeconds = byOutsideWindowDelay;
                }

                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                // Open 2048 for writing
                ProcParam = new byte[0];
                ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE,
                    ProcParam, out ProcResponse);

                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                if (ProcResult == ProcedureResultCodes.COMPLETED)
                {
                    // Write the table
                    Response = OptionBoardModemConfig.Write();

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    // Clear the information in the ModemConfig table
                    m_OptionBoardModemConfig = null;

                    if (Response == PSEMResponse.Ok)
                    {
                        // Close the configuration
                        // Data reset bits - we don't want to rest any data, so 
                        // just initialize them to 0
                        ProcParam = new byte[4];
                        ProcParam.Initialize();

                        // Tell the meter we're done. It will validate and change
                        // configuration areas.
                        ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE,
                                                    ProcParam, out ProcResponse);

                        OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            ReconfigResult = ItronDeviceResult.SUCCESS;
                        }
                        else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                        }
                        else
                        {
                            ReconfigResult = ItronDeviceResult.ERROR;
                        }
                    } // if (Response == PSEMResponse.Ok)
                    else if (Response == PSEMResponse.Isc)
                    {
                        ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                    }
                    else
                    {
                        ReconfigResult = ItronDeviceResult.ERROR;
                    }
                } // if (ProcResult == ProcedureResultCodes.COMPLETED)
                else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                {
                    ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                }
                else
                {
                    ReconfigResult = ItronDeviceResult.ERROR;
                }
            }
            else if (Table91 != null && Table91.AnswerCall == true)
            {
                // Handle reconfigure through the 9's decade for C12.21 modem
                Table95.InsideWindowRings = byInsideWindowDelay;

                // Only set the Outside window delay if there are Answer windows
                // set up in the meter
                if (Table91.NumberOfAnswerWindows > 0)
                {
                    Table95.OutsideWindowRings = byOutsideWindowDelay;
                }

                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                // We can always write to the standard tables so just write the table
                Response = Table95.Write();

                // Clear the information in the table
                m_Table95 = null;

                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                // Interpret the result
                if (Response == PSEMResponse.Ok)
                {
                    ProcParam = new byte[0];
                    ProcResult = ExecuteProcedure(Procedures.SAVE_CONFIGURATION,
                        ProcParam, out ProcResponse);

                    if (ProcResult == ProcedureResultCodes.COMPLETED)
                    {
                        ReconfigResult = ItronDeviceResult.SUCCESS;
                    }
                    else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                    {
                        ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                    }
                    else
                    {
                        ReconfigResult = ItronDeviceResult.ERROR;
                    }
                }
                else if (Response == PSEMResponse.Isc)
                {
                    ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                }
                else
                {
                    ReconfigResult = ItronDeviceResult.ERROR;
                }

                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());
            }
            else
            {
                // The modem is not configured
                ReconfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
            }

            return ReconfigResult;
        }

        /// <summary>
        /// Reconfigures the prefix for the modem phone home numbers.
        /// </summary>
        /// <param name="strPrefix">The string representation of the prefix.</param>
        /// <returns>The result of the reconfigure.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/20/07 RCG 8.00.13        Created

        public ItronDeviceResult ReconfigureModemPrefix(string strPrefix)
        {
            PSEMResponse Response;
            ItronDeviceResult ReconfigResult = ItronDeviceResult.ERROR;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] ProcParam;
            byte[] ProcResponse;

            // First check to make sure we have a valid Prefix
            if (strPrefix == MODEM_PREFIX_NONE || strPrefix == MODEM_PREFIX_8
                || strPrefix == MODEM_PREFIX_9)
            {
                if (OptionBoardModemConfig != null)
                {
                    string[] strPhoneNumbers = OptionBoardModemConfig.PhoneNumbers;
                    // Handle reconfigure for the SENTINEL Modem

                    if(strPhoneNumbers != null)
                    {
                        // Set up the new phone numbers
                        for (int iIndex = 0; iIndex < strPhoneNumbers.Length; iIndex++)
                        {
                            // Do nothing if there is no phone number
                            if (strPhoneNumbers[iIndex] != String.Empty && strPhoneNumbers[iIndex] != "None")
                            {
                                // Remove any old prefixes on the original phone numbers
                                if (strPhoneNumbers[iIndex].IndexOf(MODEM_PREFIX_8, StringComparison.Ordinal) == 0)
                                {
                                    strPhoneNumbers[iIndex] = strPhoneNumbers[iIndex].Substring(MODEM_PREFIX_8.Length);
                                }
                                else if (strPhoneNumbers[iIndex].IndexOf(MODEM_PREFIX_9, StringComparison.Ordinal) == 0)
                                {
                                    strPhoneNumbers[iIndex] = strPhoneNumbers[iIndex].Substring(MODEM_PREFIX_9.Length);
                                }

                                // Add in the new prefix
                                strPhoneNumbers[iIndex] = strPrefix + strPhoneNumbers[iIndex];
                            }
                        }

                        OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                        OptionBoardModemConfig.PhoneNumbers = strPhoneNumbers;

                        // Open 2048 for writing
                        ProcParam = new byte[0];
                        ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE,
                            ProcParam, out ProcResponse);

                        OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            // Write the table
                            Response = OptionBoardModemConfig.Write();

                            OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                            // Clear the information in the ModemConfig table
                            m_OptionBoardModemConfig = null;

                            if (Response == PSEMResponse.Ok)
                            {
                                // Close the configuration
                                // Data reset bits - we don't want to rest any data, so 
                                // just initialize them to 0
                                ProcParam = new byte[4];
                                ProcParam.Initialize();

                                // Tell the meter we're done. It will validate and change
                                // configuration areas.
                                ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE,
                                                            ProcParam, out ProcResponse);

                                OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                                if (ProcResult == ProcedureResultCodes.COMPLETED)
                                {
                                    ReconfigResult = ItronDeviceResult.SUCCESS;
                                }
                                else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                                {
                                    ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                                }
                                else
                                {
                                    ReconfigResult = ItronDeviceResult.ERROR;
                                }
                            } // if (Response == PSEMResponse.Ok)
                            else if (Response == PSEMResponse.Isc)
                            {
                                ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                            }
                            else
                            {
                                ReconfigResult = ItronDeviceResult.ERROR;
                            }
                        } // if (ProcResult == ProcedureResultCodes.COMPLETED)
                        else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                        }
                        else
                        {
                            ReconfigResult = ItronDeviceResult.ERROR;
                        }

                    }
                    else
                    {
                        // We have no strings to reconfigure
                        ReconfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
                    }
                }
                else if (Table91 != null && Table91.NumberOfPhoneNumbers > 0)
                {
                    // Handle reconfigure through the 9's decade for C12.21 modem
                    Table93.Prefix = strPrefix;

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    // We can always write to the standard tables so just write the table
                    Response = Table93.Write();

                    // Clear the information in the table
                    m_Table93 = null;

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());

                    // Interpret the result
                    if (Response == PSEMResponse.Ok)
                    {
                        ProcParam = new byte[0];
                        ProcResult = ExecuteProcedure(Procedures.SAVE_CONFIGURATION,
                            ProcParam, out ProcResponse);

                        if (ProcResult == ProcedureResultCodes.COMPLETED)
                        {
                            ReconfigResult = ItronDeviceResult.SUCCESS;
                        }
                        else if (ProcResult == ProcedureResultCodes.NO_AUTHORIZATION)
                        {
                            ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                        }
                        else
                        {
                            ReconfigResult = ItronDeviceResult.ERROR;
                        }
                    }
                    else if (Response == PSEMResponse.Isc)
                    {
                        ReconfigResult = ItronDeviceResult.SECURITY_ERROR;
                    }
                    else
                    {
                        ReconfigResult = ItronDeviceResult.ERROR;
                    }

                    OnStepProgress(new Itron.Metering.Progressable.ProgressEventArgs());
                }
                else
                {
                    // The modem is not configured
                    ReconfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
                }
            }
            else
            {
                // We have been given an invalid Prefix so don't perform the reconfigure
                ReconfigResult = ItronDeviceResult.UNSUPPORTED_OPERATION;
            }

            return ReconfigResult;
        }

        /// <summary>
        /// Reconfigures TOU in the connected meter
        /// </summary>
        /// <param name="TOUFileName">The filename including path for the TOU 
        /// export</param>
        /// <param name="DSTFileName">The filename including path for the DST 
        /// file.  If this parameter is emtpy then client only wants to 
        /// reconfigure TOU, not DST. The DSTFileName MUST be included if the 
        /// meter is configured for DST. If the meter is not configured for DST
        /// and this filename is given, the operation will succeed, but it will
        /// return a conditional success code.</param>
        /// <returns>A TOUReconfigResult</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/26/06 mcm 7.30.?? N/A    Created
        //	01/24/07 mrj 8.00.08		Flushed status flags when reconfiguring
        //  11/13/13 AF  3.50.03        Moved definition from ItronDevice
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter. It is only
        //                              valid for the Sentinel and could cause confusion
        //                              with CENTRON_AMI's ReconfigureTOU(...) method.
        // 	
        public virtual TOUReconfigResult ReconfigureTOU(string TOUFileName,
                                                          string DSTFileName)
        {
            ProcedureResultCodes ProcResult =
                                ProcedureResultCodes.INVALID_PARAM;
            TOUReconfigResult Result = TOUReconfigResult.ERROR;
            CDSTSchedule DSTSchedule;
            CTOUSchedule TOUSchedule;
            byte[] ProcParam;
            byte[] ProcResponse;

            try
            {
                m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                    "Starting TOU Reconfigure");

                // Check to see if the meter's clock is running, if it is
                // configured for TOU and DST, and if the filenames are valid.
                // If possible, this call will return instances of the TOU
                // and DST servers with the files opened.
                Result = ValidateForTOUReconfigure(TOUFileName, DSTFileName,
                                            out TOUSchedule, out DSTSchedule);

                // The TOU configuration is stored in two areas. The TOUConfig
                // table has Season/Pattern definitions. The CalendarConfig
                // table has the year definitions including the DST dates,
                // holidays, and season start dates.
                if (TOUReconfigResult.SUCCESS == Result)
                {
                    OverwriteTOUConfig(TOUSchedule);
                    Result = OverwriteCalendarConfig(TOUSchedule, DSTSchedule);
                }

                // If we were able to update the two areas that hold TOU config
                // data, start the reconfigure.
                if (TOUReconfigResult.SUCCESS == Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Starting reconfiguration");

                    // Tell the meter we want to open the config table for 
                    // writing.
                    ProcParam = new byte[0];
                    ProcResult = ExecuteProcedure(Procedures.OPEN_CONFIG_FILE,
                        ProcParam, out ProcResponse);

                    // Write the two config tables to the meter.  NOTE they 
                    // have to be in sequential order. This is a forward only
                    // write to the table. There's no backing up.  To make 
                    // things a little more interesting, the relative location
                    // is device/firmware/cofiguration dependent.
                    if (ProcedureResultCodes.COMPLETED == ProcResult)
                    {
                        if (Table2048.Table2048Header.CalendarOffset <
                            Table2048.Table2048Header.TOUOffset)
                        {
                            // Write the Calendar configuration section. 
                            Result = (TOUReconfigResult)
                                GetDSTResult(Table2048.CalendarConfig.Write());

                            if (ProcedureResultCodes.COMPLETED == ProcResult)
                            {
                                // Write the TOU configuration section. 
                                Result = (TOUReconfigResult)
                                    GetDSTResult(Table2048.TOUConfig.Write());
                            }
                        }
                        else
                        {
                            // Write the TOU configuration section. 
                            Result = (TOUReconfigResult)
                                GetDSTResult(Table2048.TOUConfig.Write());

                            if (ProcedureResultCodes.COMPLETED == ProcResult)
                            {
                                // Write the Calendar configuration section. 
                                Result = (TOUReconfigResult)
                                    GetDSTResult(Table2048.CalendarConfig.Write());
                            }
                        }
                    }
                    else if (ProcedureResultCodes.NO_AUTHORIZATION == ProcResult)
                    {
                        Result = TOUReconfigResult.INSUFFICIENT_SECURITY_ERROR;
                    }
                    else
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Open config procedure failed with result = "
                                + ProcResult);
                        Result = TOUReconfigResult.ERROR;
                    }
                }
                if (TOUReconfigResult.SUCCESS == Result)
                {
                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Finishing reconfiguration");

                    // Data reset bits - we don't want to rest any data, so 
                    // just initialize them to 0
                    ProcParam = new byte[4];
                    ProcParam.Initialize();

                    // Tell the meter we're done. It will validate and change
                    // configuration areas.
                    ProcResult = ExecuteProcedure(Procedures.CLOSE_CONFIG_FILE,
                                                ProcParam, out ProcResponse);
                    if (ProcedureResultCodes.COMPLETED != ProcResult)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Close config procedure failed with result = "
                            + ProcResult);
                        Result = TOUReconfigResult.ERROR;
                    }
                }

                // Conditional success case: If everything succeeded, and we 
                // were given a DST filename, but the ValidateForTOUReconfigure
                // didn't instantiate a DST server with the filename, then the
                // meter must not have DST. The call would have failed 
                // otherwise. Change the result to show it succeeded, but
                // might not have been what they expected
                if ((TOUReconfigResult.SUCCESS == Result) &&
                   (0 < DSTFileName.Length) && (null == DSTSchedule))
                {
                    Result = TOUReconfigResult.SUCCESS_DST_NOT_SUPPORTED;
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            //Clear the TOU schedule and the cached status items
            m_TOUSchedule = null;
            m_TOUExpireDate.Flush();
            m_DayOfTheWeek.Flush();
            m_uiNumTOURates.Flush();
            m_MeterInDST.Flush();

            return Result;

        } // ReconfigureTOU

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Phone Home Numbers for the Sentinel Modem or C12.21 modem.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/20/07 RCG 8.00.13        Created

        public string[] ModemPhoneHomeNumbers
        {
            get
            {
                string[] strPhoneNumbers = null;

                // If the option board is a modem then return the phone numbers
                // from the Option board
                if (OptionBoardModemConfig != null)
                {
                    // If this is not null then that means that we have a modem
                    // as the option board.
                    strPhoneNumbers = (string[])OptionBoardModemConfig.PhoneNumbers.Clone();
                }
                else if (Table91 != null && Table91.NumberOfPhoneNumbers > 0)
                {
                    string strPrefix;

                    // The device is using a C12.21 modem. We need to add the prefix onto
                    strPhoneNumbers = (string[])Table93.PhoneNumbers.Clone();
                    strPrefix = Table93.Prefix;

                    // Add on the prefixes so that the phone numbers returned will always
                    // look the same

                    if (strPrefix != String.Empty)
                    {
                        for (int iIndex = 0; iIndex < strPhoneNumbers.Length; iIndex++)
                        {
                            // Make sure that we do not add on a prefix if the string is empty
                            if (strPhoneNumbers[iIndex] != String.Empty)
                            {
                                strPhoneNumbers[iIndex] = Table93.Prefix + strPhoneNumbers[iIndex];
                            }
                        }
                    }
                }

                return strPhoneNumbers;                
            }
        }

        /// <summary>
        /// Gets the answer delay for calls made inside a call window.
        /// </summary>
        /// <remarks>
        /// Note: The unit type if this property is determined by the
        /// ModemAnswerDelayUnits property.
        /// </remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/20/07 RCG 8.00.13        Created

        public byte ModemInsideWindowDelay
        {
            get
            {
                byte byDelay = 0;

                if (OptionBoardModemConfig != null)
                {
                    // Get the delay from the option board config
                    byDelay = OptionBoardModemConfig.InsideWindowSeconds;
                }
                else if (Table91 != null && Table91.AnswerCall == true)
                {
                    // Get the delay from decade 9
                    byDelay = Table95.InsideWindowRings;
                }

                return byDelay;
            }
        }

        /// <summary>
        /// Gets the answer delay for calls made outside a call window.
        /// </summary>
        /// <remarks>
        /// Note: The unit type if this property is determined by the
        /// ModemAnswerDelayUnits property.
        /// </remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/20/07 RCG 8.00.13        Created
        
        public byte ModemOutsideWindowDelay
        {
            get
            {
                byte byDelay = 0;

                if (OptionBoardModemConfig != null)
                {
                    // Get the delay from the option board config
                    byDelay = OptionBoardModemConfig.OutsideWindowSeconds;
                }
                else if (Table91 != null && Table91.AnswerCall == true)
                {
                    // Get the delay from decade 9
                    byDelay = Table95.OutsideWindowRings;
                }

                return byDelay;
            }
        }

		/// <summary>
		/// Get the instantaneous THD values for phases a, b, and c
		/// </summary>
		//  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/14/07 mrj 8.00.12		Created
        //  07/05/07 KRC 8.10.12 3014   Check PQ MeterKeybit
        //
		public InstantaneousQuantity INS_THD_V
		{			
			get
			{
				ANSIInstantaneousQuantity InstTHD = null;

				if (FWRevision >= SENTINEL_SATURN_FW_REV)
				{
                    //We know it is the gith firmware, but is the feture in MeterKey
                    if (MeterKeyTable.PQSupported)
                    {
                        //If this is a saturn Sentinel then attempt to get the THD registers
                        InstTHD = new ANSIInstantaneousQuantity(this);

                        InstTHD.PhaseALID = m_LID.INS_THD_V_PHASE_A;
                        InstTHD.PhaseBLID = m_LID.INS_THD_V_PHASE_B;
                        InstTHD.PhaseCLID = m_LID.INS_THD_V_PHASE_C;

                        InstTHD.ReadQuantity();
                    }
				}

				return InstTHD;
			}
		}

		/// <summary>
		/// Get the instantaneous TTD values for phases a, b, and c
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  02/14/07 mrj 8.00.12		Created
        //  07/05/07 KRC 8.10.12 3014   Check PQ MeterKeybit
        //
		public InstantaneousQuantity INS_TDD_I
		{
			get
			{
				ANSIInstantaneousQuantity InstTDD = null;

				if (FWRevision >= SENTINEL_SATURN_FW_REV)
				{
                    //We know it is the gith firmware, but is the feture in MeterKey
                    if (MeterKeyTable.PQSupported)
                    {
                        //If this is a saturn Sentinel then attempt to get the TDD registers
                        InstTDD = new ANSIInstantaneousQuantity(this);

                        InstTDD.PhaseALID = m_LID.INS_TDD_I_PHASE_A;
                        InstTDD.PhaseBLID = m_LID.INS_TDD_I_PHASE_B;
                        InstTDD.PhaseCLID = m_LID.INS_TDD_I_PHASE_C;

                        InstTDD.ReadQuantity();
                    }
				}

				return InstTDD;
			}
		}

        /// <summary>
        /// Indicates whether or not the meter is currently recording
        /// load profile data - SENTINEL needs to check Channel 1
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  12/05/06 MAH 8.00.00
        /// </remarks>
        public override bool LPRunning
        {
            get
            {
                bool bRunning = false;
                if (m_LoadProfileStatus.NumberOfChannels > 0)
                {
                    // If we have a channel, then that means that Load Profile is running
                    bRunning = true;
                }

                return bRunning;
            }
        }

        /// <summary>
        /// Checks the meter to determine whether we should allow the user to
        /// perform a reconfigure on the phone home prefix
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/20/07 RCG 8.00.13        Created
        // 03/27/07 RCG 8.00.21 2683   Fixing issue that allowed a reconfigure when there were
        //                             no phone numbers programmed into the meter.

        public bool AllowModemPrefixReconfigure
        {
            get
            {
                bool bConfigured = false;

                if (OptionBoardModemConfig != null)
                {
                    // Check the list of phone numbers to make sure atleast one is set
                    foreach (string PhoneNumber in OptionBoardModemConfig.PhoneNumbers)
                    {
                        if (PhoneNumber != String.Empty && PhoneNumber != "None")
                        {
                            bConfigured = true;
                        }
                    }
                }
                else if (Table91 != null && Table91.NumberOfPhoneNumbers > 0)
                {
                    bConfigured = true;
                }

                return bConfigured;
            }
        }

        /// <summary>
        /// Gets whether or not we will allow the Outside Window answer delay to be
        /// reconfigured.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/21/07 RCG 8.00.13        Created

        public bool AllowModemOWDelayReconfigure
        {
            get
            {
                bool bAllowReconfig = false;

                // Make sure that we can reconfigure the answer delay for the outside call window.
                // We do not allow the user to reconfigure the delay if it has been disabled ( == 0)
                if (OptionBoardModemConfig != null && OptionBoardModemConfig.AnswerOutsideWindow == true
                    && OptionBoardModemConfig.OutsideWindowSeconds > 0)
                {
                    bAllowReconfig = true;
                }
                // For the C12.21 modem we also need to make sure the field exists
                else if (Table91 != null && Table91.NumberOfAnswerWindows > 0
                    && Table95.OutsideWindowRings > 0)
                {
                    bAllowReconfig = true;
                }

                return bAllowReconfig;
            }
        }

        /// <summary>
        /// Gets whether or not a modem is enabled in the meter
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/21/07 RCG 8.00.13        Created

        public bool IsModemEnabled
        {
            get
            {
                bool bConfigured = false;

                if (OptionBoardModemConfig != null)
                {
                    bConfigured = true;
                }
                else if (Table91 != null && Table91.AnswerCall == true)
                {
                    bConfigured = true;
                }

                return bConfigured;
            }
        }

        /// <summary>
        /// Gets the unit type of the modem answer delay times
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/21/07 RCG 8.00.13        Created

        public ModemAnswerDelayType ModemAnswerDelayUnits
        {
            get
            {
                ModemAnswerDelayType DelayType = ModemAnswerDelayType.Unavailable;

                if (OptionBoardModemConfig != null)
                {
                    // Setinel modem uses seconds
                    DelayType = ModemAnswerDelayType.Seconds;
                }
                else if (Table91 != null && Table91.AnswerCall == true)
                {
                    // C12.21 modems use number of rings
                    DelayType = ModemAnswerDelayType.Rings;
                }

                return DelayType;
            }
        }

        /// <summary>
        /// This property returns Table 2069. (Creates it if needed)
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/12/07 mcm 8.00.11 N/A    Created
        // 
        internal MfgActualPowerQuality Table2069
        {
            get
            {
                if (null == m_Table2069)
                {
                    m_Table2069 = new MfgActualPowerQuality(m_PSEM, m_Table0);
                }

                return m_Table2069;
            }
        }

        /// <summary>
        /// This property returns Table 2070. (Creates it if needed)
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/12/07 mcm 8.00.11 N/A    Created
        // 
        internal MfgPowerQualityParameters Table2070
        {
            get
            {
                if (null == m_Table2070)
                {
                    m_Table2070 = new MfgPowerQualityParameters(m_PSEM, Table2069);
                }

                return m_Table2070;
            }
        }

        /// <summary>
        /// This property returns Table 2071. (Creates it if needed)
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/12/07 mcm 8.00.11 N/A    Created
        // 
        internal MfgPowerQualityStatus Table2071
        {
            get
            {
                if (null == m_Table2071)
                {
                    m_Table2071 = new MfgPowerQualityStatus(m_PSEM, Table2069);
                }

                return m_Table2071;
            }
        }

        /// <summary>
        /// This property returns Table 2072. (Creates it if needed)
        /// </summary>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/13/07 mcm 8.00.11 N/A    Created
        // 
        internal MfgPowerQualityEvents Table2072
        {
            get
            {
                if (null == m_Table2072)
                {
                    m_Table2072 = new MfgPowerQualityEvents(m_PSEM, Table2069);
                }

                return m_Table2072;
            }
        }

        /// <summary>
        /// Gets the standard table 91 object, and creates it if the table is
        /// used by the meter and it has not already been created.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/20/07 RCG 8.00.13 N/A    Created

        internal StdTable91 Table91
        {
            get
            {
                if (null == m_Table91 && Table00.IsTableUsed(91))
                {
                    m_Table91 = new StdTable91(m_PSEM);
                }

                return m_Table91;
            }
        }

        /// <summary>
        /// Gets the standard table 93 object, and creates it if the table is
        /// used by the meter and it has not already been created.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/20/07 RCG 8.00.13 N/A    Created

        internal StdTable93 Table93
        {
            get
            {
                if (null == m_Table93 && Table00.IsTableUsed(93))
                {
                    // We also need to make sure Table91 is not null
                    // since the constructor requires the use of 91
                    if (Table91 != null)
                    {
                        m_Table93 = new StdTable93(m_PSEM, Table00, Table91);
                    }
                }

                return m_Table93;
            }
        }

        /// <summary>
        /// Gets the standard table 95 object, and creates it if the table is
        /// used by the meter and it has not already been created.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/21/07 RCG 8.00.13 N/A    Created

        internal StdTable95 Table95
        {
            get
            {
                if (null == m_Table95 && Table00.IsTableUsed(95))
                {
                    // We also need to make sure Table91 is not null
                    // since the constructor requires the use of 91
                    if (Table91 != null)
                    {
                        m_Table95 = new StdTable95(m_PSEM, Table00, Table91);
                    }
                }

                return m_Table95;
            }
        }

        /// <summary>
        /// Returns the configure max number of VQ events that can be logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public int MaxVQEvents
        {
            get
            {
                return Table2069.MaxPQEvents;
            }
        }

        /// <summary>
        /// Returns the configured VQ % nearly threshold
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public int VQLogNearlyFullThreshold
        {
            get
            {
                return Table2070.LogNearFullThreshold;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether VQ is enabled in this meter
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public bool VQEnabled
        {
            get
            {
                bool Enabled = false;

                if ((Table00.IsTableUsed(2069)) && (Table2069.MaxPQEvents > 0))
                {
                    Enabled = true;
                }

                return Enabled;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether VQ is running in this meter
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public bool VQRunning
        {
            get
            {
                return Table2071.PQRunning;
            }
        }

        /// <summary>
        /// Returns the total number of VQ events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte TotalVQEvents
        {
            get
            {
                return Table2071.NbrEvents;
            }
        }

        /// <summary>
        /// Returns the current percentage of VQ event logspace used
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQPercentLogFull
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_PERCENT_LOG_FULL, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Percent Log Full LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether the VQ log has overflowed since
        /// it was last cleared.
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public bool VQLogOverflowed
        {
            get
            {
                return Table2071.Overflowed;
            }
        }

        /// <summary>
        /// Returns the total number of VQ sag events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQSagCount
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_SAG_COUNT, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Sag Count LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Returns the total number of VQ swell events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQSwellCount
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_SWELL_COUNT, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Swell Count LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Returns the total number of VQ voltage imbalance events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQVoltageImbalanceCount
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_VOLTAGE_IMBALANCE_COUNT, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Voltage Imbalance Count LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Returns the total number of VQ current imbalance events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQCurrentImbalanceCount
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_CURRENT_IMBALANCE_COUNT, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Current Imbalance Count LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Returns the total number of VQ interruption events currently logged
        /// </summary>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------
        ///  02/07/07 mcm 8.00.11			Created 
        /// </remarks>
        public byte VQInterruptionCount
        {
            get
            {
                object objValue;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.VQ_INTERRUPTION_COUNT, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading number of VQ Interruptions LID");
                }

                return (byte)objValue;
            }
        }

        /// <summary>
        /// Property to get the custom schedule name from the device.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/14/07 jrf 8.00.18 2521	Created
        //  
        public override string CustomScheduleName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Property used to get the human readable meter name 
        /// (string).  Use this property when 
        /// displaying the name of the meter to the user.  
        /// This should not be confused with the MeterType 
        /// which is used for meter determination and comparison.
        /// </summary>
        /// <returns>A string representing the human readable name of the 
        /// meter.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/16/07 jrf 8.00.19 2653   Created
        //
        public override string MeterName
        {
            get
            {
                return SENTINEL_NAME;
            }
        }

        /// <summary>
        /// Builds the list of Event descriptions and returns the dictionary 
        /// </summary>
        /// <returns>
        /// Dictionary of Event Descriptions
        /// </returns> 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/30/07 KRC  8.10.05			Added for SENTINEL
        //
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    m_dicEventDescriptions = (ANSIEventDictionary)(new SENTINEL_EventDictionary());
                }

                return m_dicEventDescriptions;
            }
        }

        /// <summary>
        /// Returns the type of IO board in the meter.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/24/11 jrf 2.50.47			Created 
        // 
        public IOBoardType IOBoard
        {
            get
            {
                IOBoardType IOBoard = IOBoardType.UNKNOWN;
                object objValue;
                uint uiIOBoardType;
                PSEMResponse Result =
                    m_lidRetriever.RetrieveLID(m_LID.IO_BOARD_CAPABILITES, out objValue);

                if (PSEMResponse.Ok != Result)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                        "Error reading IO Capabilites LID");
                }

                uiIOBoardType = Convert.ToUInt32(objValue, CultureInfo.InvariantCulture);

                if (true == Enum.IsDefined(typeof(IOBoardType), uiIOBoardType))
                {
                    IOBoard = (IOBoardType)uiIOBoardType;
                }

                return IOBoard;
            }
        }


        /// <summary>
        /// Provides access to the Pulse Input 1 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/11 jrf 2.50.49 N/A    Adding support to get pulse input 1
        //
        public virtual Quantity PulseInput1
        {
            get
            {
                ANSIQuantity PulseInput1 = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_PULSE_INPUT_1)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_PULSE_INPUT_1)))
                {
                    PulseInput1 = new ANSIQuantity("Pulse Input #1", m_PSEM, this);
                    // Set the LID Properties so the Quantity knows what type he is
                    PulseInput1.TotalEnergyLID = m_LID.ENERGY_PULSE_INPUT_1;
                    PulseInput1.TotalMaxDemandLID = m_LID.DEMAND_MAX_PULSE_INPUT_1;
                    PulseInput1.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_PULSE_INPUT_1.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    PulseInput1.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_PULSE_INPUT_1.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return PulseInput1;
            }
        }

        /// <summary>
        /// Provides access to the Pulse Input 2 Quantity
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  05/27/11 jrf 2.50.49 N/A    Adding support to get pulse input 2
        //
        public virtual Quantity PulseInput2
        {
            get
            {
                ANSIQuantity PulseInput2 = null;
                // Make sure this quantity is supported before constructing it.
                if ((true == ValidateEnergy(m_LID.ENERGY_PULSE_INPUT_2)) ||
                    (true == ValidateDemand(m_LID.DEMAND_MAX_PULSE_INPUT_2)))
                {
                    PulseInput2 = new ANSIQuantity("Pulse Input #2", m_PSEM, this);
                    // Set the LID Properties so the Quantity knows what type he is
                    PulseInput2.TotalEnergyLID = m_LID.ENERGY_PULSE_INPUT_2;
                    PulseInput2.TotalMaxDemandLID = m_LID.DEMAND_MAX_PULSE_INPUT_2;
                    PulseInput2.TOUBaseEnergyLID = CreateLID((m_LID.ENERGY_PULSE_INPUT_2.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA);
                    PulseInput2.TOUBaseMaxDemandLID = CreateLID((m_LID.DEMAND_MAX_PULSE_INPUT_2.lidValue &
                                                    (uint)DefinedLIDs.BaseLIDs.COMPONENT_MASK_OUT) |
                                                    (uint)DefinedLIDs.BaseLIDs.TOU_DATA |
                                                    (uint)DefinedLIDs.TOU_Rate_Data.TOU_DEMAND);
                }

                return PulseInput2;
            }
        }

        #endregion Public Properties

		#region Internal Methods

		/// <summary>
        /// Creates a LID object from the given 32-bit number
        /// </summary>
        /// <param name="uiLIDNumber">The 32-bit number that represents the LID</param>
        /// <returns>The LID object for the specified LID</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/07/07 RCG 8.00.11 N/A    Created

        public override LID CreateLID(uint uiLIDNumber)
        {
            return new SentinelLID(uiLIDNumber);
        }

        /// <summary>
        /// Creates a new SENTINEL DisplayItem
        /// </summary>
        /// <returns>ANSIDisplayItem</returns>
        // Revision History	
        // MM/DD/YY who Version Issue#  Description
        // -------- --- ------- ------  ---------------------------------------
        // 03/29/07 KRC 8.00.22		    Added to handle SENTINEL specific display item behavior
        //
        internal override ANSIDisplayItem CreateDisplayItem(LID lid, string strDisplayID, ushort usFormat, byte byDim)
        {
            return new SENTINELDisplayItem(lid, strDisplayID, usFormat, byDim, FWRevision);
        }

        #endregion

        #region Internal Property

        /// <summary>
        /// This method creates the correct version of the 2048 table for the
        /// Sentinel meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 06/09/06 mcm 7.30.00 N/A    Created
        //	11/20/06 KRC 8.00.00 N/A    Changed to Property
        //	
        internal override CTable2048 Table2048
        {
            get
            {
                if (null == m_Table2048)
                {
                    m_Table2048 = new CTable2048_Sentinel(m_PSEM, FWRevision);
                }

                return m_Table2048;
            }
        }

        /// <summary>
        /// Gets the Modem Configuration table from the meter if the option
        /// board currently in the meter is a SENTINEL Modem. If the option board
        /// is not a SENTINEL Modem it will always return null.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/19/07 RCG 8.00.13 N/A    Created

        internal ModemConfig OptionBoardModemConfig
        {
            get
            {
                if (null == m_OptionBoardModemConfig && null != Table2048.OptionBoardConfig)
                {
                    if (Table2048.OptionBoardConfig.OptionBoardID == (ushort)OptionBoardHeader.OptionBoardIDs.Modem)
                    {
                        m_OptionBoardModemConfig = new ModemConfig(m_PSEM, (ushort)(Table2048.Table2048Header.OptionBoardOffset
                            + OptionBoardHeader.OPTION_BOARD_HEADER_LENGTH));
                    }
                }

                return m_OptionBoardModemConfig;
            }
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Protected Property

        /// <summary>
		/// Gets the meter type "SENTINEL".
		/// </summary>
		/// <returns>string</returns>
		/// Revision History
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------------
		/// 04/25/06 mrj 7.30.00 N/A    Created
		///	
		protected override string DefaultMeterType
		{
			get
			{
				return SENTINEL_METER_TYPE;			
			}
		}

        /// <summary>
        /// Gets the multiplier used to calculate the Load Profile Pulse Weight
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/11/07 KRC 8.00.27 2864   Created
        //
        protected override float LPPulseWeightMultiplier
        {
            get
            {
                return 0.025f;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The PasswordReconfigResult reconfigures passwords.  This method implements
        /// Password reconfiguration for SENTINELs with firmware less than 5.0.  In
        /// the Saturn (5.0) project, this mechanism was diabled and the standard
        /// mechanism via table 42 was implemented.  The standard implementation 
        /// is located in the base class.
        /// </summary>
        /// <param name="Passwords">A list of passwords to write to the meter. 
        /// The Primary password should be listed first followed by the secondary
        /// password and so on.  Use empty strings for null passwords.  Passwords
        /// will be truncated or null filled as needed to fit in the device.</param>
        /// <returns>A PasswordReconfigResult object</returns>
		//  Revision History		
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/06 mcm 7.35.00 N/A    Created
		//  04/11/07 mrj 8.00.28 2873	Pre-Saturn Sentinels did not allow duplicate
		//								passwords
        //	
        private PasswordReconfigResult MFGReconfigurePasswords(
                            System.Collections.Generic.List<string> Passwords)
        {
            PasswordReconfigResult Result = PasswordReconfigResult.ERROR;
            PSEMResponse ProtocolResponse = PSEMResponse.Err;
            ProcedureResultCodes ProcResult = ProcedureResultCodes.INVALID_PARAM;
            byte[] byParameter = new byte[1];
            byte[] ProcResponse;
            byte[] byPassword = new byte[SIZE_OF_ANSI_PASSWORD];
            byte[] byTable2052 = new byte[SIZE_OF_ANSI_PASSWORD + 1];


            // We have to clear the passwords because the meter rejects duplicates,
            // so if our new set of passwords contain one of the old passwords this 
            // will still work.
            byParameter[0] = 0;
            ProcResult = ExecuteProcedure(Procedures.RESET_PASSWORDS, byParameter, 
                out ProcResponse);

            switch (ProcResult)
            {
                case ProcedureResultCodes.COMPLETED:
                    {
                        //Success
                        ProtocolResponse = PSEMResponse.Ok;
                        break;
                    }
                case ProcedureResultCodes.NO_AUTHORIZATION:
                    {
                        //Isc error
                        ProtocolResponse = PSEMResponse.Isc;
                        break;
                    }
                default:
                    {
                        //General Error
                        ProtocolResponse = PSEMResponse.Err;
                        break;
                    }
            }           


            // No need to write null passwords since we cleared them.
            if ((PSEMResponse.Ok == ProtocolResponse) &&
                (4 <= Passwords.Count) && (0 < Passwords[3].Length))
            {
                NullFillPassword(Passwords[3], SIZE_OF_ANSI_PASSWORD, ref byPassword);
                byTable2052[0] = TERTIARY_PASSWORD_LEVEL;
                Array.Copy(byPassword, 0, byTable2052, 1, SIZE_OF_ANSI_PASSWORD);
                ProtocolResponse = m_PSEM.FullWrite(MFG_SECURITY_TABLE, byTable2052);
            }
            if ((PSEMResponse.Ok == ProtocolResponse) &&
                (3 <= Passwords.Count && (0 < Passwords[2].Length)))
            {
                NullFillPassword(Passwords[2], SIZE_OF_ANSI_PASSWORD, ref byPassword);
                byTable2052[0] = SECONDARY_PASSWORD_LEVEL;
                Array.Copy(byPassword, 0, byTable2052, 1, SIZE_OF_ANSI_PASSWORD);
                ProtocolResponse = m_PSEM.FullWrite(MFG_SECURITY_TABLE, byTable2052);
            }
            if ((PSEMResponse.Ok == ProtocolResponse) &&
                 (2 <= Passwords.Count) && (0 < Passwords[1].Length))
            {
                NullFillPassword(Passwords[1], SIZE_OF_ANSI_PASSWORD, ref byPassword);
                byTable2052[0] = LIMITED_RECONFIGURE_PASSWORD_LEVEL;
                Array.Copy(byPassword, 0, byTable2052, 1, SIZE_OF_ANSI_PASSWORD);
                ProtocolResponse = m_PSEM.FullWrite(MFG_SECURITY_TABLE, byTable2052);
            }
            if ((PSEMResponse.Ok == ProtocolResponse) &&
                (1 <= Passwords.Count) && (0 < Passwords[0].Length))
            {
                NullFillPassword(Passwords[0], SIZE_OF_ANSI_PASSWORD, ref byPassword);
                byTable2052[0] = PRIMARY_PASSWORD_LEVEL;
                Array.Copy(byPassword, 0, byTable2052, 1, SIZE_OF_ANSI_PASSWORD);
                ProtocolResponse = m_PSEM.FullWrite(MFG_SECURITY_TABLE, byTable2052);
            }

            // Translate Protocol result
            if (PSEMResponse.Ok == ProtocolResponse)
            {
                Result = PasswordReconfigResult.SUCCESS;
            }
            else if (PSEMResponse.Isc == ProtocolResponse)
            {
                Result = PasswordReconfigResult.SECURITY_ERROR;
            }
			else if (PSEMResponse.Onp == ProtocolResponse)
			{
				Result = PasswordReconfigResult.DUPLICATE_SECURITY_ERROR;
			}
            else // (PSEMResponse. == ProtocolResponse)
            {
                Result = PasswordReconfigResult.PROTOCOL_ERROR;
            }

            return Result;

        } // ReconfigurePasswords

        /// <summary>
        /// Reconfigures passwords using the manufacturer table 2052. This method for
        /// changing the tertiary password is to be used only for non-Saturn SENTINEL
        /// meters.
        /// </summary>
        /// <param name="strTertiaryPassword">The new tertiary password.</param>
        /// <returns>The result of the reconfigure.</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/22/07 RCG 8.00.13    N/A Created
		//  04/11/07 mrj 8.00.28 2873	Pre-Saturn Sentinels did not allow duplicate
		//								passwords
		//	
        private PasswordReconfigResult MFGReconfigureTertiaryPassword(string strTertiaryPassword)
        {
            PasswordReconfigResult ReconfigResult = PasswordReconfigResult.ERROR;
            PSEMResponse Response = PSEMResponse.Err;
            byte[] byPassword = new byte[SIZE_OF_ANSI_PASSWORD];
            byte[] byTable2052 = new byte[SIZE_OF_ANSI_PASSWORD + 1];

            NullFillPassword(strTertiaryPassword, SIZE_OF_ANSI_PASSWORD, ref byPassword);
            byTable2052[0] = TERTIARY_PASSWORD_LEVEL;
            Array.Copy(byPassword, 0, byTable2052, 1, SIZE_OF_ANSI_PASSWORD);
            Response = m_PSEM.FullWrite(MFG_SECURITY_TABLE, byTable2052);

            // Translate Protocol result
            if (PSEMResponse.Ok == Response)
            {
                ReconfigResult = PasswordReconfigResult.SUCCESS;
            }
            else if (PSEMResponse.Isc == Response)
            {
                ReconfigResult = PasswordReconfigResult.SECURITY_ERROR;
            }
			else if (PSEMResponse.Onp == Response)
			{
				ReconfigResult = PasswordReconfigResult.DUPLICATE_SECURITY_ERROR;
			}
			else // (PSEMResponse. == Response)
			{
				ReconfigResult = PasswordReconfigResult.PROTOCOL_ERROR;
			}

            return ReconfigResult;
        }

        /// <summary>
        /// Clears Table2048.m_CalendarConfig and write the TOU and DST info
        /// to it to prepare it for writing to the meter.  The CalendarConfig
        /// contains season start dates, holdays, and DST dates.  The rest of 
        /// the TOU configuration is in the TOUConfig table.
        /// </summary>
        /// <remarks>This method ASSUMES the TOUSchedule and DSTSchedule 
        /// provided are valid for the meter. If the DSTSchedule object is be 
        /// null, the meter will not be configured with DST</remarks>
        /// <param name="TOUSchedule">TOU server with a valid TOU file open</param>
        /// <param name="DSTSchedule">DST server with a DST file open. Note 
        /// that this object can be null for meters that will not have DST</param>
        /// <returns>SUCCESS, ERROR_DST_DATA_MISSING</returns>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/26/06 mcm 7.30.00 N/A	Created
        /// 10/13/06 mcm 7.35.04 55,64,67 - Insert season start date didn't work
        /// 11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        /// 
        private TOUReconfigResult OverwriteCalendarConfig(CTOUSchedule TOUSchedule,
                                                        CDSTSchedule DSTSchedule)
        {
            TOUReconfigResult Result = TOUReconfigResult.SUCCESS;
            CalendarYear CalYear;
            TOU.CYear TOUYear;
            int YearsToConfigure;
            int StartYear = DateTime.Now.Year;

            // Are 10,000 indices really needed? We're looping through the 
            // meter's years, the year's events, the TOU server, and the DST 
            // server. 
            int TOUIndex = 0;	 // Index of TOU Years
            byte DSTStartIndex = 0;	// Index of this years' DST dates
            byte DSTIndex = 0;	 // Index of DST Years
            int YearIndex = 0;	 // Index of CalendarConfig years
            int TOUEventIndex = 0; // Index of schedule events
            int CalEventIndex = FIRST_TOU_CAL_INDEX; // Index of meter's calendar events
            bool bInsertSeasonStartDate = true;

            try
            {
                // Write the year definitions to the calendar config area.

                // Clear the table so we don't have to write over unused areas
                Table2048.CalendarConfig.Clear();

                if (null != TOUSchedule)
                {
                    Table2048.CalendarConfig.CalendarID =
                        (ushort)TOUSchedule.TOUID;

                    // Only set the offset if there's DST.
                    if (null != DSTSchedule)
                    {
                        Table2048.CalendarConfig.DSTHour =
                            (byte)(DSTSchedule.ToTime / SECONDS_PER_HOUR);
                        Table2048.CalendarConfig.DSTMinute =
                            (byte)(DSTSchedule.ToTime % SECONDS_PER_HOUR);
                        Table2048.CalendarConfig.DSTOffset =
                            (byte)DSTSchedule.JumpLength;

                        // Get the index of this year's DST dates
                        if (false == DSTSchedule.FindDSTIndex(StartYear,
                            out DSTStartIndex))
                        {
                            return TOUReconfigResult.ERROR_DST_DATA_MISSING;
                        }
                    }
                }

                // The count of years to configure is depends on the schedule's
                // start year, duration, and the meter's capacity.
                // ASSUMES the schedule starts in the past.
                YearsToConfigure = TOUSchedule.Duration + TOUSchedule.StartYear
                    - StartYear;
                if (YearsToConfigure > Table2048.CalendarConfig.MaxYears)
                {
                    YearsToConfigure = Table2048.CalendarConfig.MaxYears;
                }

                // Get the index of this year's DST dates
                // mcm 4/25/2007 - CTOUSchedule will throw an exception if the year
                // isn't in the schedule. Catch it so we can give a better error code.
                try
                {
                    TOUIndex = TOUSchedule.Years.IndexOf(new CYear(StartYear, null));
                }
                catch
                {
                    return TOUReconfigResult.ERROR_TOU_NOT_VALID;
                }

                // Calendar events aren't always sorted. Search the first year
                // for a season start date on 1/1
                TOUYear = TOUSchedule.Years[TOUIndex];
                for (TOUEventIndex = 0; TOUEventIndex < TOUYear.Events.Count;
                    TOUEventIndex++)
                {
                    CalendarEvent CalEvent =
                        GetYearEvent(TOUYear.Events[TOUEventIndex]);

                    if (CalEvent.IsSeason() &&
                         (0 == CalEvent.Month) &&
                         (0 == CalEvent.Day))
                    {
                        // We have one, stop searching
                        bInsertSeasonStartDate = false;
                        break;
                    }
                }

                for (YearIndex = 0; YearIndex < YearsToConfigure; YearIndex++)
                {
                    TOUYear = TOUSchedule.Years[TOUIndex++];
                    CalYear = Table2048.CalendarConfig.Years[YearIndex];

                    CalYear.Year = (byte)(StartYear + YearIndex -
                        CalendarConfig.CALENDAR_REFERENCE_YEAR);

                    // If we're configuring DST, it has to come first.
                    if (null != DSTSchedule)
                    {
                        DSTIndex = (byte)(DSTStartIndex + YearIndex);
                        CalYear.Events[0].Type = (byte)
                            CalendarEvent.CalendarEventType.ADD_DST;
                        CalYear.Events[0].Month = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].ToDate.Month - 1);
                        CalYear.Events[0].Day = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].ToDate.Day - 1);

                        CalYear.Events[1].Type = (byte)
                            CalendarEvent.CalendarEventType.SUB_DST;
                        CalYear.Events[1].Month = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].FromDate.Month - 1);
                        CalYear.Events[1].Day = (byte)
                            (DSTSchedule.DSTDatePairs[DSTIndex].FromDate.Day - 1);
                    }

                    // 0 & 1 are reserved for DST even if the meter is using it
                    CalEventIndex = FIRST_TOU_CAL_INDEX;

                    for (TOUEventIndex = 0;
                        TOUEventIndex < TOUYear.Events.Count;
                        TOUEventIndex++)
                    {
                        CalendarEvent CalEvent =
                            GetYearEvent(TOUYear.Events[TOUEventIndex]);

                        // 10/13/06 mcm - SCRs 55,64,67 - Insert season start 
                        // date code didn't work, and it caused the first year 
                        // to be skipped.

                        if (bInsertSeasonStartDate)
                        {
                            byte byFirstSeasonIndex;

                            // We need to insert a season start date.  Is there
                            // going to be room for it?  
                            if (MAX_CAL_EVENTS <= TOUYear.Events.Count + FIRST_TOU_CAL_INDEX)
                            {
                                // We're already maxed out. We can't add another
                                // event, so fail it.
                                return TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
                            }

                            // Find the index of the season that should start 
                            // our first year.
                            byFirstSeasonIndex = FindStartSeason(TOUSchedule, TOUIndex - 1);

                            CalendarEvent FirstSeasonEvent = new CalendarEvent();

                            FirstSeasonEvent.Month = 0;
                            FirstSeasonEvent.Day = 0;
                            FirstSeasonEvent.Type = byFirstSeasonIndex;
                            CalYear.Events[CalEventIndex++] = FirstSeasonEvent;

                            // OK, that was fun, but let's not do it again.
                            bInsertSeasonStartDate = false;
                        }

                        CalYear.Events[CalEventIndex++] = CalEvent;
                    }
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // OverwriteCalendarConfig

        /// <summary>
        /// Finds the index of the season that should be in effect at the start
        /// the new configuration. Customers usually have a Summer and Winter
        /// season. They expect Winter to be in	effect until Summer starts that
        /// first year, but it won't be unless we insert a start date.
        /// This method ASSUMES that every year has a season start date.
        /// </summary>
        /// <param name="TOUSchedule">TOU server with the schedule open</param>
        /// <param name="YearIndex">Index of the first year to configure</param>
        /// <returns>The season index that should be used on 1/1 of the 
        /// first year.</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/26/06 mcm 7.30.00 N/A	Created
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        private byte FindStartSeason(CTOUSchedule TOUSchedule, int YearIndex)
        {
            byte SeasonIndex = 0;
            TOU.CYear TOUYear;

            // If the TOUIndex given is not 0, then we have at least one year
            // defined in the schedule prior to this year. Search last year for
            // the last season start date.  If this is the first year of the 
            // calendar, we'll make the assumption that the last season of
            // this year should have started the year.
            if (0 < YearIndex)
            {
                // Decrement the index. We'll search last year.
                --YearIndex;
            }

            TOUYear = TOUSchedule.Years[YearIndex];

            // Search the schedule for season start events. We could search
            // backwards and stop at the first one we find, but there are only
            // 44 events, and I think the logic is cleaner this way. Besides
            // the XML DOM object doesn't like going backwards, so it might
            // even be faster this way.
            for (int EventIndex = 0;
                EventIndex < TOUYear.Events.Count;
                EventIndex++)
            {
                CalendarEvent CalEvent =
                    GetYearEvent(TOUYear.Events[EventIndex]);

                // If this is the first year configured and the schedule 
                // doesn't have a season starting on the first day of
                // year, insert one
                if (CalEvent.IsSeason())
                {
                    SeasonIndex = CalEvent.Type;
                }
            }
            // probably want to modify this to return the TOU server's index or a meter event object
            // depending on whether we insert a start date or use a procedure call to start it after configuration
            return SeasonIndex;

        } // FindStartSeason

        /// <summary>
        /// Builds a CaledarConfig Day Event from a TOU schedule's year event.
        /// </summary>
        /// <param name="TOUEvent">TOU server applied season start date or 
        /// applied holiday event</param>
        /// <returns>CalendarConfig CalendarEvent that represents the TOU
        /// event</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/26/06 mcm 7.30.00 N/A	Created
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        private CalendarEvent GetYearEvent(TOU.CEvent TOUEvent)
        {
            CalendarEvent Event = new CalendarEvent();

            // Translate TOU schedule event to CalendarConfig event
            if (TOU.eEventType.HOLIDAY == TOUEvent.Type)
            {
                Event.Type = (byte)CalendarEvent.CalendarEventType.HOLIDAY;
            }
            else
            {
                Event.Type = (byte)((int)CalendarEvent.CalendarEventType.SEASON1 +
                    TOUEvent.Index);
            }

            Event.Month = (byte)(TOUEvent.Date.Month - 1);
            Event.Day = (byte)(TOUEvent.Date.Day - 1);

            return Event;

        } // GetYearEvent

        /// <summary>
        /// Checks to see if the meter's clock is running and if it is 
        /// configured for DST time adjustment and TOU rates.
        /// </summary>
        /// <param name="TOUFileName">PC-PRO+ TOU schedule file name</param>
        /// <param name="DSTFileName">PC-PRO+ DST file name (DST.xml). Pass
        /// an empty string if the meter doesn't support DST.</param>
        /// <param name="TOUSchedule">Returns an instance of the TOU server
        /// with the TOUFileName file open if successful</param>
        /// <param name="DSTSchedule">Returns an instance of the DST server
        /// with the DSTFileName file open if successful. NOTE that it is valid
        /// to pass an empty DST file name for meters not configured with DST.
        /// In that case, the returned DSTSchedule will be null.</param>
        /// <returns>SUCCESS, SUCCESS_NOT_CONFIGURED_FOR_TOU, CLOCK_ERROR, 
        /// ERROR_DST_DATA_MISSING, ERROR_SCHED_NOT_SUPPORTED</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        //  10/20/06 mcm 7.35.07 SCR 99 Expired TOU schedule should ret err 30
        //  03/13/07 mrj 8.00.18		Removed wait, keep alive is now used
        //  04/05/07 mrj 8.00.25 2842	Allow Sentinel Advanced to use basic
        //								schedules.
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        // 
        private TOUReconfigResult ValidateForTOUReconfigure(string TOUFileName,
            string DSTFileName, out CTOUSchedule TOUSchedule,
            out CDSTSchedule DSTSchedule)
        {
            TOUReconfigResult Result = TOUReconfigResult.ERROR;
            bool HasDST = false;
            string DeviceType = "";
            bool bMeterSupported = true;


            m_Logger.WriteLine(Logger.LoggingLevel.Functional,
                "Validating Meter for TOU Reconfigure");

            // Initialize out parameters
            TOUSchedule = null;
            DSTSchedule = null;

            try
            {
                // The extended device type matches the TOU schedule's names
                // for supported devices.
                DeviceType = GetExtendedDeviceType();

                if (!ClockRunning)
                {
                    Result = TOUReconfigResult.CLOCK_ERROR;
                }
                else if (0 == Table2048.TOU_ID)
                {
                    Result = TOUReconfigResult.SUCCESS_NOT_CONFIGURED_FOR_TOU;
                }
                else
                {
                    HasDST = Table2048.HasDST;

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                        "Opening TOU Schedule: " + TOUFileName);

                    // Open the TOU file
                    try
                    {
                        TOUSchedule = new CTOUSchedule(TOUFileName);
                    }
                    catch
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "Error opening TOU Schedule");

                        // If the file is invalid, don't throw the 
                        // excecption. Just return the appropriate result.
                        return TOUReconfigResult.ERROR_TOU_NOT_VALID;
                    }

                    //Check to see if the meter supports this schedule.
                    if (false == TOUSchedule.IsSupported(DeviceType))
                    {
                        //The meter does not support this schedule unless
                        //this is and advanced Sentinel and this is a 
                        //basic schedule
                        if ("SENTINEL - Advanced" == DeviceType &&
                            TOUSchedule.IsSupported("SENTINEL - Basic"))
                        {
                            bMeterSupported = true;
                        }
                        else
                        {
                            bMeterSupported = false;
                        }
                    }

                    // Is our meter supported?
                    if (!bMeterSupported)
                    {
                        m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                            "This Schedule does not support " + DeviceType
                            + " Devices");
                        Result = TOUReconfigResult.ERROR_SCHED_NOT_SUPPORTED;
                    }
                    else if (HasDST &&
                        ((null == DSTFileName) || (0 == DSTFileName.Length)))
                    {
                        Result = TOUReconfigResult.ERROR_DST_DATA_MISSING;
                    }
                    else if (HasDST)
                    {
                        // Open the DST file
                        try
                        {
                            DSTSchedule = new CDSTSchedule(DSTFileName);
                        }
                        catch
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed,
                                "Error opening DST file: " + DSTFileName);

                            // If the file is invalid, don't throw the 
                            // excecption. Just return the appropriate result.
                            return TOUReconfigResult.ERROR_DST_DATA_MISSING;
                        }

                        Result = TOUReconfigResult.SUCCESS;
                    }
                    else
                    {
                        Result = TOUReconfigResult.SUCCESS;
                    }

                    // mcm 10/20/2006 - SCR 99 - Expired TOU schedule should
                    // fail with error 30 instead of causing exception.
                    if (TOUReconfigResult.SUCCESS == Result)
                    {
                        int iLastYearOfSchedule =
                            TOUSchedule.StartYear + TOUSchedule.Duration - 1;

                        if (DateTime.Now.Year > iLastYearOfSchedule)
                        {
                            Result = TOUReconfigResult.ERROR_TOU_EXPIRED;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

            return Result;

        } // ValidateForTOUReconfigure		

        /// <summary>
        /// Clears Table2048.m_TOUConfig and writes the TOU scheudule info
        /// into the table to prepare it for writing to the meter. The TOUConfig
        /// contains configuration data not related to a particular year. This
        /// table has the Typical Week and pattern definitions for all of the
        /// seasons. The per-year info (season start dates, holdays, and DST 
        /// dates) are in the CalendarConfig table.
        /// </summary>
        /// <remarks>ASSUMES the TOU schedule is valid for this meter</remarks>
        /// <param name="TOUSchedule">TOU server with a supported TOU file 
        /// open.</param>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/22/06 mcm 7.30.00 N/A	Created
        /// 10/23/06 mcm 7.35.07 105    Need to add end output switchpoints
        /// 11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        /// 
        private void OverwriteTOUConfig(CTOUSchedule TOUSchedule)
        {
            ushort TypicalWeek;
            TOUConfig.TOU_Season Season;
            CSeasonCollection SchedSeasons;
            CSeason SchedSeason;
            CPatternCollection Patterns;
            int PatternIndex;
            int TODEvent;
            bool EventsNeedSorting = false;

            try
            {
                // Write the season definitions to the TOU configuration section.

                // Clear the table so we don't have to write over unused areas
                Table2048.TOUConfig.Clear();

                if (null != TOUSchedule)
                {
                    SchedSeasons = TOUSchedule.Seasons;
                    Patterns = TOUSchedule.Patterns;

                    // The Day to DayType association is the same for all years
                    TypicalWeek = GetTypicalWeek(TOUSchedule);

                    for (int SeasonIndex = 0; SeasonIndex < SchedSeasons.Count;
                        SeasonIndex++)
                    {
                        SchedSeason = SchedSeasons[SeasonIndex];
                        Season = Table2048.TOUConfig.Seasons[SeasonIndex];
                        Season.IsProgrammed = 1;
                        Season.Daytypes = TypicalWeek;

                        // Configure the normal daytypes (0..2)
                        for (int DaytypeIndex = 0;
                            DaytypeIndex < SchedSeason.NormalDays.Count;
                            DaytypeIndex++)
                        {
                            // Get the index of the pattern in the schedule's 
                            // pattern collection that's assigned to this 
                            // daytype, so we can add its switchpoints
                            PatternIndex = Patterns.SearchID(
                                SchedSeason.NormalDays[DaytypeIndex]);

                            TODEvent = 0;
                            for (int EventIndex = 0;
                                EventIndex < Patterns[PatternIndex].SwitchPoints.Count;
                                EventIndex++)
                            {
                                // mcm 10/23/2006 - SCR 105 - Output end events not added.
                                // Outputs switchpoints are a special case. A start and end 
                                // event must be added for outputs.
                                CSwitchPoint SP = Patterns[PatternIndex].SwitchPoints[EventIndex];

                                Season.TimeOfDayEvents[DaytypeIndex, TODEvent++] =
                                    GetDayEvent(SP);

                                // Unlike rate switchpoints, outputs can overlap, so we 
                                // have to add the end point too.
                                if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                                {
                                    Season.TimeOfDayEvents[DaytypeIndex, TODEvent++] =
                                        GetOutputOffEvent(SP);

                                    // We'll need to sort the events since we added one
                                    EventsNeedSorting = true;
                                }

                            } // For each switchpoint
                        }

                        // Configure the Holiday daytype if it exists
                        if (0 != SchedSeason.Holidays.Count)
                        {
                            // Get the index of the pattern in the schedule's 
                            // pattern collection that's assigned to this 
                            // (holiday) daytype, so we can add its switchpoints
                            PatternIndex = Patterns.SearchID(SchedSeason.Holidays[0]);

                            TODEvent = 0;
                            for (int EventIndex = 0;
                                EventIndex < Patterns[PatternIndex].SwitchPoints.Count;
                                EventIndex++)
                            {
                                // mcm 10/23/2006 - SCR 105 - Output end events not added.
                                // Outputs switchpoints are a special case. A start and end 
                                // event must be added for outputs.
                                CSwitchPoint SP = Patterns[PatternIndex].SwitchPoints[EventIndex];

                                Season.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, TODEvent++] =
                                    GetDayEvent(SP);

                                // Unlike rate switchpoints, outputs can overlap, so we 
                                // have to add the end point too.
                                if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
                                {
                                    Season.TimeOfDayEvents[TOUConfig.HOLIDAY_TYPE_INDEX, TODEvent++] =
                                        GetOutputOffEvent(SP);

                                    // We'll need to sort the events since we added one
                                    EventsNeedSorting = true;
                                }

                            } // For each switchpoint
                        }

                        // Sort the events if we added any
                        if (EventsNeedSorting)
                        {
                            Season.Sort();
                        }
                    } // For each season
                } // IF the TOU server is instantiated
            }
            catch (Exception e)
            {
                // Log it and pass it up
                m_Logger.WriteException(this, e);
                throw (e);
            }

        } // OverwriteTOUConfig

        /// <summary>
        /// Returns the configuration value for the schedule's day to daytipe
        /// assignments.
        /// </summary>
        /// <param name="TOUSchedule">A TOU server instance with a TOU schedule
        /// open</param>
        /// <returns>A TOU server's typical week packed into a 2 byte value to
        /// be used in the Sentinel or Image configuration</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        private ushort GetTypicalWeek(CTOUSchedule TOUSchedule)
        {
            ushort TypicalWeek = 0;
            ushort DTIndex;


            // Day to Day Type Assignments: 
            // 2 bits for each day (Sun – Sat & Holiday)

            TypicalWeek = GetDaytypeIndex(TOUSchedule, eTypicalDay.SUNDAY);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.MONDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0004);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.TUESDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0010);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.WEDNESDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0040);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.THURSDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0100);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.FRIDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x0400);
            DTIndex = GetDaytypeIndex(TOUSchedule, eTypicalDay.SATURDAY);
            TypicalWeek = (ushort)(TypicalWeek + DTIndex * 0x1000);

            // Holiday type always gets the holiday index
            TypicalWeek = (ushort)(TypicalWeek +
                TOUConfig.HOLIDAY_TYPE_INDEX * 0x4000);

            return TypicalWeek;

        } // GetTypicalWeek

        /// <summary>
        /// Returns the schedule's Daytype Type and index into a index (1..4).
        /// Normal Daytype 1 = 1, Normal Daytype 3 = 3, Holiday type 1 = 4.
        /// </summary>
        /// <param name="TOUSchedule">A TOU server instance with a TOU schedule
        /// open</param>
        /// <param name="Day">Day of the week to translate</param>
        /// <returns>Daytype index used by GetTypicalWeek</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        private ushort GetDaytypeIndex(CTOUSchedule TOUSchedule,
                                        eTypicalDay Day)
        {
            CDayType DayType;
            ushort DaytypeIndex = 0;


            // The Image and Sentinel meters store the day of the week to 
            // daytype associations for the season as a ushort
            DayType = TOUSchedule.GetDayType(TOUSchedule.TypicalWeek[(int)Day]);
            if (eDayType.NORMAL == DayType.Type)
            {
                DaytypeIndex = (ushort)DayType.Index;
            }
            else
            {
                DaytypeIndex = TOUConfig.HOLIDAY_TYPE_INDEX;
            }

            return DaytypeIndex;

        } // GetDaytypeIndex


        /// <summary>
        /// Builds a TOUConfig time of day event from a TOU schedule's 
        /// switchpoint.  See the TIME_MAN_DESIGN.doc, WindChill document
        /// #D0209255 for more info.
        /// </summary>
        /// <param name="SP">TOU server switchpoint to translate</param>
        /// <returns>Time of Day event</returns>
        /// 
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  06/22/06 mcm 7.30.00 N/A	Created
        //  11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        private TOUConfig.DayEvent GetDayEvent(CSwitchPoint SP)
        {
            TOUConfig.DayEvent.TOUEvent EventType;
            byte Hour;
            byte Minutes;


            if (eSwitchPointType.RATE == SP.SwitchPointType)
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.RateA);
            }
            else
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.Output1);
            }

            // Translate minutes since midnight to 0-based hours and minutes
            Hour = (byte)(SP.StartTime / 60);
            Minutes = (byte)(SP.StartTime % 60);

            return new TOUConfig.DayEvent(EventType, Hour, Minutes);

        } // GetDayEvent

        /// <summary>
        /// Builds a TOUConfig time of day event from a TOU schedule's 
        /// OUTPUT switchpoint.  Calling this method with a rate switchpoint 
        /// will cause an exception to be thrown. 
        /// See the TIME_MAN_DESIGN.doc, WindChill document
        /// #D0209255 for more info.
        /// </summary>
        /// <param name="SP">TOU server output switchpoint to translate</param>
        /// <returns>Time of Day event</returns>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 10/23/06 mcm 7.35.07 105	Support for adding output off switchpoints
        /// 11/22/13 jrf 3.50.07 TQ9523 Moved definition from ANSIMeter.
        /// 
        private TOUConfig.DayEvent GetOutputOffEvent(CSwitchPoint SP)
        {
            TOUConfig.DayEvent.TOUEvent EventType;
            byte Hour;
            byte Minutes;


            if (eSwitchPointType.OUTPUT == SP.SwitchPointType)
            {
                // Translate the rate index into a TOUEvent value
                EventType = (TOUConfig.DayEvent.TOUEvent)(SP.RateOutputIndex +
                    TOUConfig.DayEvent.TOUEvent.Output1Off);
            }
            else
            {
                // This method only handles outputs! There are no rate off events.
                throw (new ApplicationException("Invalid Switchpoint Type"));
            }

            // Translate minutes since midnight to 0-based hours and minutes
            Hour = (byte)(SP.StopTime / 60);
            Minutes = (byte)(SP.StopTime % 60);

            return new TOUConfig.DayEvent(EventType, Hour, Minutes);

        } // GetOutputOffEvent	


        /// <summary>
        /// This method writes the custom schedule to the billing schedule
        /// (custom schedule) config block of table 2048.
        /// </summary>
        /// <remarks>
        /// This schedule should be validated before calling this method.
        /// </remarks>
        /// <param name="strPath">The path to the custom schedule</param>
        /// <param name="strCSName">The custom schedule name</param>
        /// <returns>CSReconfigResult</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/02/05 mrj 7.13.00 N/A    Created
        //  09/22/05 mrj 7.20.17		Added support for Image
        //  05/31/06 mrj 7.30.06        HH-Pro
        //  10/23/06 AF  7.40.xx N/A    Made number of dates dynamic to support
        //                              Open Way meters
        // 
        private CSReconfigResult WriteCustomSchedule(string strPath, string strCSName)
        {
            DateTime date;
            int iIndex = 0;
            ushort[] sScheduleArray = new ushort[Table2048.BillingSchedConfig.ScheduleLength];
            CSReconfigResult Result;


            //Validate the schedule
            Result = ValidateSchedule(strPath, strCSName);


            if (Result == CSReconfigResult.SUCCESS ||
                Result == CSReconfigResult.SUCCESS_SCHEDULE_TRUNCATED)
            {
                //Schedule is valid so write it to the config block 

                //Open the Custom Schedule
                CCustomSchedule sched = new CCustomSchedule(strPath, strCSName);

                //Get all of the dates for the current year and forward
                DateCollection dates = sched.DatesStartYear(System.DateTime.Now);


                if (0 < dates.Count)
                {
                    for (iIndex = 0;
                        iIndex < dates.Count &&
                        iIndex < Table2048.BillingSchedConfig.ScheduleLength;
                        iIndex++)
                    {
                        //Loop through the dates and add convert them to Sentinel dates
                        date = dates[iIndex];
                        sScheduleArray[iIndex] = ConvertToSentinelDays(date);
                    }

                    //The meter can only accept 300 values, but if we do not have that
                    //many we need to tell the meter we are done by assigning 0xffff 
                    //to the array of dates.
                    if (Table2048.BillingSchedConfig.ScheduleLength > iIndex)
                    {
                        sScheduleArray[iIndex] = BillingSchedConfig.END_OF_CUST_SCHED;
                    }

                    //Copy the dates to the billing schedule (custom schedule) config block
                    Table2048.BillingSchedConfig.ScheduleDates = sScheduleArray;


                    //Copy the custom schedule name to the config block, it will only
                    //write the name for the Image meters
                    Table2048.BillingSchedConfig.ScheduleName = strCSName;
                }
            }


            return Result;
        }


        /// <summary>
        /// This method validates the custom schedule
        /// </summary>
        /// <param name="strPath">The path to the custom schedule</param>
        /// <param name="strCSName">The custom schedule name</param>
        /// <returns>
        /// CSReconfigResult
        /// </returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/31/06 mrj 7.30.06        Created
        //  10/23/06 AF  7.40.xx        Made number of dates dynamic to support
        //                              Open Way meters
        // 
        private CSReconfigResult ValidateSchedule(string strPath, string strCSName)
        {
            CSReconfigResult Result = CSReconfigResult.SUCCESS;

            try
            {
                FileInfo CSFile = new FileInfo(strPath);

                if (CSFile.Exists)
                {
                    CCustomSchedule CustomSchedule = new CCustomSchedule(strPath, strCSName);

                    //Get the dates for this schedule
                    DateCollection dates = CustomSchedule.DatesStartYear(System.DateTime.Now);

                    if (dates.Count > Table2048.BillingSchedConfig.ScheduleLength)
                    {
                        //This schedule will be truncated
                        Result = CSReconfigResult.SUCCESS_SCHEDULE_TRUNCATED;
                    }
                    else if (0 == dates.Count)
                    {
                        //There are no valid dates in the schedule
                        Result = CSReconfigResult.ERROR_CS_FILE_NOT_FOUND;
                    }
                }
                else
                {
                    //File does not exist so return error;
                    Result = CSReconfigResult.ERROR_CS_FILE_NOT_FOUND;
                }
            }
            catch
            {
                Result = CSReconfigResult.ERROR_CS_FILE_NOT_FOUND;
            }


            return Result;
        }


        /// <summary>
        /// Converts from DateTime to Sentinel's date, which is number of days
        /// since 1/1/2000.
        /// </summary>
        /// <param name="date">The date to be converted to Sentinel days</param>
        /// <returns>An unsigned short representing the number of days since 1/1/2000</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/13/05 mrj 7.13.00 N/A    Created
        // 
        private ushort ConvertToSentinelDays(DateTime date)
        {
            System.TimeSpan timespan;
            System.DateTime SentinelRefDate;

            SentinelRefDate = MeterReferenceTime;

            timespan = date - SentinelRefDate;

            return (ushort)timespan.Days;
        }

        #endregion


        #region Member Variables

        private MfgActualPowerQuality m_Table2069;
        private MfgPowerQualityParameters m_Table2070;
        private MfgPowerQualityStatus m_Table2071;
        private MfgPowerQualityEvents m_Table2072;
        private ModemConfig m_OptionBoardModemConfig;
        private StdTable91 m_Table91;
        private StdTable93 m_Table93;
        private StdTable95 m_Table95;

        #endregion
    }
}
