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
//                              Copyright © 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Itron.Metering.Communications.PSEM
{
    /// <summary>
    /// Binary Reader that will implement some special PSEM reads
    /// </summary>
    public class PSEMBinaryReader : BinaryReader
    {
        #region Constants

        /// <summary>
        /// Reference date for TM_FORMAT=3 is 1/1/1970 Local.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/15/09 jrf 2.40.01 146666 Changing LTIMEReferenceDate to be local instead of UTC.  
        //                              Times calculated using this value (i.e. history log event 
        //                              times, current time, han/lan comm log event times) are 
        //                              returned from the meter as local.  
        //
        private DateTime LTIMEReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        /// Reference date for TM_FORMAT=3 is 1/1/1970 GMT.  
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/15/09 jrf 2.40.01 146666 Created so STIMEs will still returned times in UTC. 
        //
        private DateTime STIMEReferenceDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Definitions

        /// <summary>
        /// Time Format Enumeration that matches that in table 0.
        /// </summary>
        public enum TM_FORMAT
        {
            /// <summary>
            /// No Clock type
            /// </summary>
            NO_CLOCK = 0,
            /// <summary>
            /// BCD: year, month, day, hour, minute, second
            /// </summary>
            BCD_TIME = 1,
            /// <summary>
            /// uint8: year, month, day, hour, minute, second
            /// </summary>
            UINT8_TIME = 2,
            /// <summary>
            /// UTIME: uint32 (minutes since 01/01/1970), seconds
            /// </summary>
            UINT32_TIME = 3,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// PSEMBinary Reading constructor
        /// </summary>
        /// <param name="input"></param>
        public PSEMBinaryReader(Stream input)
            : base(input)
        {
        }

        /// <summary>
        /// ReadChars - Handles the ReadChars Behavior, but using Byte
        /// </summary>
        /// <param name="iLength">Number of characters to read</param>
        /// <returns>array of characters read from bineary stream</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/15/07 KRC 78.00.12       Fixing issue with bad character reads
        //
        public override char[] ReadChars(int iLength)
        {
            // We have had trouble when "strings" inside of our streams have had invalid data.  For example
            //  Customer Serial Number started with a NULL, but then had random data.  This random data
            //  seemed to cause problems for ReadChars.  Therefore we are going to read the stream as
            //  bytes and then convert it back to characters.
            UTF8Encoding utf8 = new UTF8Encoding();  // Helps make sure we handle encoding correctly when converting
            byte[] byArray = new byte[iLength];
            char[] chArray = new char[iLength];

            // First - Get all of the bytes out of the stream
            byArray = base.ReadBytes(iLength);
            // Second - Convert the byte array to a string.
            string strValue = utf8.GetString(byArray, 0, iLength);
            
            // Now we want to get rid of any invalid characters
            // Find the index of the first null
            int index = strValue.IndexOf('\0');

            if (index >= 0)
            {
                // We found the first null, so if there are any characters after it, we don't want them.
                strValue = strValue.Substring(0, strValue.IndexOf('\0'));
            }
            
            // Finally, we have a good string, so convert it to the character array that was asked for.
            strValue.ToCharArray().CopyTo(chArray,0);

            return chArray;
        }

        /// <summary>
        /// Reads a String of a given length out of the Binary Reader
        /// </summary>
        /// <param name="iLength">The number of bytes to read from the stream.</param>
        /// <returns>string - "" if there were no valid bytes returned.</returns>
        public string ReadString(int iLength)
        {
            UTF8Encoding utf8 = new UTF8Encoding();  // Helps make sure we handle encoding correctly when converting
            int index = 0;

            // Get the bytes out of the reader into the byte[]
            byte[] byString = base.ReadBytes(iLength);
            // Now assign it to a string.  This will contain any trailing nulls
            string strString = utf8.GetString(byString, 0, iLength);

            // Now we want to get rid of any invalid characters
            // Find the index of the first null
            index = strString.IndexOf('\0');
            
            if (index >= 0)
            {
                // Get everything before the first null.  (We did not use trim, because there were
                //      some cases where there was a null followed by garbage. )
                strString = strString.Substring(0, strString.IndexOf('\0'));
            }

            return strString;
        }

        /// <summary>
        /// Reads the LTIME out of the stream.  Time Format must be set
        /// to correct value for this to work correctly.
        /// </summary>
        /// <returns>DateTime</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //                              Created
        //  03/28/07 AF  8.00.22 2791   This is really Kevin's fix.  LTIME uses a 
        //                              reference date of 1/1/1970, not 1/1/2000
        //  05/11/09 AF  2.20.04        If an LTIME field is initialized with 
        //                              0xFFFFFFFF instead of zero, we will get an 
        //                              out of bounds exception if we try to add
        //                              the minutes to the reference date.
        //  05/15/14 MDP                Added support for uint8 time, GE meters use this
        //
        public DateTime ReadLTIME(TM_FORMAT TimeFormat)
        {
            DateTime dtValue;

            switch (TimeFormat)
            {
                case TM_FORMAT.UINT32_TIME:
                {
                    UInt32 uiNumMinutes;
                    uint uiNumSeconds;

                    // Format in this case is the first four bytes indicate
                    // the number of minutes since Jan. 1st 1970.  The last
                    // byte indicates number of seconds.
                    uiNumMinutes = base.ReadUInt32();
                    uiNumSeconds = base.ReadByte();

                    dtValue = LTIMEReferenceDate;

                    // If the time has been initialized with all FFs, don't
                    // try to use it -- it's invalid
                    if (0xFFFFFFFF > uiNumMinutes)
                    {
                        dtValue = dtValue.AddMinutes((double)uiNumMinutes);
                        dtValue = dtValue.AddSeconds((double)uiNumSeconds);
                    }
                    
                    break;
                }
                //TODO: Handle other types here...
                //case TM_FORMAT.UINT8_TIME:
                //{                   
                //    //uint8: year, month, day, hour, minute, second
                //    byte byYear;
                //    byte byMonth;
                //    byte byDay;
                //    byte byNumHours;
                //    byte byNumMinutes;
                //    byte byNumSeconds;

                //    byYear = base.ReadByte();
                //    byMonth = base.ReadByte();
                //    byDay = base.ReadByte();
                //    byNumHours = base.ReadByte();
                //    byNumMinutes = base.ReadByte();
                //    byNumSeconds = base.ReadByte();

                //    dtValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
                //    dtValue = dtValue.AddYears(byYear);
                //    dtValue = dtValue.AddMonths(byMonth - 1);
                //    dtValue = dtValue.AddDays(byDay - 1);
                //    dtValue = dtValue.AddHours(byNumHours);
                //    dtValue = dtValue.AddMinutes(byNumMinutes);
                //    dtValue = dtValue.AddSeconds(byNumSeconds);
                
                //    break;
                //}
            	default :
                {
                    throw(new Exception("Selected Time Format is not implemented"));
	            }
            }
                
            return dtValue;
        }

        /// <summary>
        /// Reads the STIME out of the stream.  Time Format must be set
        /// to correct value for this to work correctly.
        /// </summary>
        /// <returns>DateTime</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/25/06 AF  7.40.00 N/A    Created
        // 11/17/06 AF  8.00.00        Corrected bug with time calculation
        // 03/28/07 AF  8.00.22 2791   This is really Kevin's fix.  LTIME uses a 
        //                             reference date of 1/1/1970, not 1/1/2000
        // 06/25/08 AF  1.50.44 116853 This is a hack.  There is a firmware bug
        //                             where an STIME field is initialized with
        //                             0xFFFFFFFF instead of zero.  We will get
        //                             an out of bounds exception if we try to 
        //                             add that to the reference date.
        //
        public DateTime ReadSTIME(TM_FORMAT TimeFormat)
        {
            DateTime dtValue;

            switch (TimeFormat)
            {
                case TM_FORMAT.UINT32_TIME:
                {
                    UInt32 uiNumMinutes;

                    // Format in this case is the first four bytes indicate
                    // the number of minutes since Jan. 1st 1970. 
                    uiNumMinutes = base.ReadUInt32();
                    dtValue = STIMEReferenceDate;

                    // If the time has been initialized with all FFs or a high number this could fail
                    try
                    {
                        dtValue = dtValue.AddMinutes((double)uiNumMinutes);
                    }
                    catch (Exception)
                    {
                        // Just go with the reference date
                    }
                    break;
                }
                //TODO: Handle other types here...
                default:
                {
                    throw (new Exception("Selected Time Format is not implemented"));
                }
            }

            return dtValue;
        }

        /// <summary>
        /// Reads the TIME out of the stream. Time Format must be set to
        /// a correct value for this to work correctly.
        /// </summary>
        /// <returns>DateTime representing the time of day</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ -------------------------------------------
        // 10/12/06 RCG 7.35.00 N/A    Created

        public TimeSpan ReadTIME(TM_FORMAT TimeFormat)
        {
            TimeSpan tsValue;

            switch (TimeFormat)
            {
                case TM_FORMAT.UINT32_TIME:
                {
                    UInt32 uiNumSeconds;
                    int iHours;
                    int iMinutes;
                    int iSeconds;

                    // Format in this case is the total number of seconds since
                    // 00:00:00
                    uiNumSeconds = base.ReadUInt32();

                    iHours = (int)(uiNumSeconds / 3600);
                    iMinutes = (int)((uiNumSeconds % 3600) / 60);
                    iSeconds = (int)(uiNumSeconds % 60);

                    tsValue = new TimeSpan(iHours, iMinutes, iSeconds);
                    break;
                }
                default:
                {
                    throw (new Exception("Selected Time Format is not implemented"));
                }                
            }

            return tsValue;
        }

        /// <summary>
        /// Reads a 48-bit integer out of the stream
        /// </summary>
        /// <returns>long</returns>
        public long ReadInt48()
        {
            long lValue = 0;
            long lValuePart1 = 0;
            long lValuePart2 = 0;

            // Read the two parts of the value.  
            // It is an INT48 so we must handle it with 
            // some special code.
            lValuePart1 = base.ReadInt32();
            lValuePart2 = base.ReadInt16();
            lValue = lValuePart1 + (lValuePart2 << 32);

            return lValue;
        }

        /// <summary>
        /// Reads a 48-bit unsigned integer out of the stream
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt48()
        {
            ulong lValue = 0;
            ulong lValuePart1 = 0;
            ulong lValuePart2 = 0;

            // Read the two parts of the value.  
            // It is an UINT48 so we must handle it with 
            // some special code.
            lValuePart1 = base.ReadUInt32();
            lValuePart2 = base.ReadUInt16();
            lValue = lValuePart1 + (lValuePart2 << 32);

            return lValue;
        }

        /// <summary>
        /// Read a 24-bit Unsigned Integer out of the stream
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt24()
        {
            uint uiValue = 0;
            uint uiValuePart1;
            uint uiValuePart2;

            // Read the two parts of the value.
            // It is a UInt24 so we must handle it with some special code
            uiValuePart1 = base.ReadUInt16();
            uiValuePart2 = base.ReadByte();
            uiValue = uiValuePart1 + (uiValuePart2 << 16);

            return uiValue;
        }

        /// <summary>
        /// Reads a 24-bit signed integer
        /// </summary>
        /// <returns>The 24-bit signed integer</returns>
        public int ReadInt24()
        {
            byte[] Data = base.ReadBytes(3);
            int Value = 0;

            // Check to see if the value is negative
            if ((Data[2] & 0x80) == 0x80)
            {
                // Set First byte to FF to make negative and because of 2's compliment
                Value |= 0xFF << 24;
            }

            Value |= (Data[2] << 16) | (Data[1] << 8) | Data[0];

            return Value;
        }

        #endregion

        #region Public Properties

        #endregion


        #region Members

        #endregion

    }
}
