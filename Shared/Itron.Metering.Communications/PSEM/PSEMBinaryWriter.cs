using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Itron.Metering.Communications.PSEM
{
    /// <summary>
    /// This class adds functionality on top of the BinaryWriter class for
    /// writing items to PSEM devices.
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  02/15/07 RCG 8.00.12        Created

    public sealed class PSEMBinaryWriter : BinaryWriter
    {
        #region Constants

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

        #region Public Methods

        /// <summary>
        /// Constructor. Always uses UTF-8 encoding for writing
        /// to PSEM devices.
        /// </summary>
        /// <param name="output">The stream to write the data to.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/15/07 RCG 8.00.12        Created

        public PSEMBinaryWriter(Stream output)
            : base (output, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Writes the specified string to the stream and makes sure that the
        /// string written takes up the specified number of bytes.
        /// </summary>
        /// <param name="value">The string to write to the meter.</param>
        /// <param name="iLength">The length, in bytes, of the data to write.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/15/07 RCG 8.00.12        Created

        public void Write(string value, int iLength)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] byFinalData = new byte[iLength];
            byte[] byStringData;

            // First change the string to a byte array so that
            // we can write the correct number of bytes
            byStringData = utf8.GetBytes(value);

            if (byStringData.Length > iLength)
            {
                // If the string is longer than what they want to write we
                // should throw an exception to warn the user.
                throw new ArgumentException("The string \"" + value + "\" is longer than "
                    + iLength.ToString(CultureInfo.CurrentCulture) + " bytes.", "value");
            }
            else
            {
                // Copy the string data to the byte array
                byStringData.CopyTo(byFinalData, 0);

                // Write the data to the stream
                Write(byFinalData);
            }            
        }

        /// <summary>
        /// Writes a TIME value to the stream.
        /// </summary>
        /// <param name="time">The time value to write.</param>
        /// <param name="timeFormat">The time format of the meter.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/20/08 RCG 2.00.09        Created

        public void WriteTIME(TimeSpan time, PSEMBinaryReader.TM_FORMAT timeFormat)
        {
            switch(timeFormat)
            {
                case PSEMBinaryReader.TM_FORMAT.UINT32_TIME:
                {
                    uint TotalSeconds = Convert.ToUInt32(time.TotalSeconds);
                    base.Write(TotalSeconds);
                    break;
                }
                default:
                {
                    throw new NotImplementedException("The selected time format has not been implemented");
                }
            }
        }

        /// <summary>
        /// Writes the STIME value to the stream
        /// </summary>
        /// <param name="date">The date to write</param>
        /// <param name="timeFormat">The time format to use</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        public void WriteSTIME(DateTime date, PSEMBinaryReader.TM_FORMAT timeFormat)
        {
            switch (timeFormat)
            {
                case PSEMBinaryReader.TM_FORMAT.UINT32_TIME:
                    {
                        uint uiMinutes = Convert.ToUInt32((date - STIMEReferenceDate).TotalMinutes);
                        base.Write(uiMinutes);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("The selected time format has not been implemented");
                    }
            }
        }

        /// <summary>
        /// Writes a UINT 24 value to the stream
        /// </summary>
        /// <param name="value">The value to write.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/24/12 RCG 2.60.17 N/A    Created
        
        public void WriteUInt24(uint value)
        {
            base.Write(BitConverter.GetBytes(value), 0, 3);
        }

        /// <summary>
        /// Writes a UINT 48 value to the stream
        /// </summary>
        /// <param name="value">The value to write.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/24/12 RCG 2.60.17 N/A    Created
        
        public void WriteUInt48(ulong value)
        {
            base.Write(BitConverter.GetBytes(value), 0, 6);
        }

        #endregion

    }
}
