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
//                              Copyright © 2006 - 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// This class provides static methods to manipulate timestamps
    /// </summary>
    public class TimeStamp
    {
        /// <summary>
        /// Returns a DateTime value that is 1 second past the start of 
        /// the interval the TimeToAdjust is in.  The value of the given
        /// DateTime instance is NOT modified by this method.
        /// </summary>
        /// <param name="TimeToAdjust">The Timestamp to be adjusted</param>
        /// <param name="IntervalLength">Interval length in minutes</param>
        /// <returns>
        /// A new DateTime that represents the first second in the interval
        /// </returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/04/07 mcm 8.00.24 2816   Support for viewing HHF files
        /// </remarks>
        static public DateTime AlignToBeggingOfInterval(DateTime TimeToAdjust, int IntervalLength)
        {
            DateTime BeggingOfInterval = TimeToAdjust;
            TimeSpan Adjustment;
            int minsOff;

            if (IntervalLength != 0)
            {
                minsOff = TimeToAdjust.Minute % IntervalLength;
                Adjustment = new TimeSpan(0, minsOff, TimeToAdjust.Second);
                BeggingOfInterval = TimeToAdjust - Adjustment;

                // now move back to the 1st second in this minute
                BeggingOfInterval = BeggingOfInterval.AddSeconds(1);
            }

            return BeggingOfInterval;
        }

        /// <summary>
        /// UNTESTED CODE! This isn't used anywhere right now.  If you use 
        /// it, test it!
        /// 
        /// Returns a DateTime value that is at the end of the interval the
        /// TimeToAdjust is in.  The value of the given DateTime instance is
        /// NOT modified by this method.
        /// </summary>
        /// <param name="TimeToAdjust">The Timestamp to be adjusted</param>
        /// <param name="IntervalLength">Interval length in minutes</param>
        /// <returns>A new DateTime that represents the EOI</returns>
        /// <remarks>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 04/04/07 mcm 8.00.24 2816   Support for viewing HHF files
        /// </remarks>
        static public DateTime AlignToEndOfInterval(DateTime TimeToAdjust, int IntervalLength)
        {
            DateTime EndOfInterval = TimeToAdjust;
            DateTime Temp;
            TimeSpan Adjustment;
            int minsOff;

            if (IntervalLength != 0)
            {
                minsOff = IntervalLength - TimeToAdjust.Minute % IntervalLength;
                Adjustment = new TimeSpan(0, minsOff, 0);
                Temp = TimeToAdjust + Adjustment;

                // Strip off the seconds and milliseconds
                EndOfInterval = new DateTime(Temp.Year, Temp.Month, 
                    Temp.Day, Temp.Hour, Temp.Minute, 0);
            }

            return EndOfInterval;
        }
    }
}
