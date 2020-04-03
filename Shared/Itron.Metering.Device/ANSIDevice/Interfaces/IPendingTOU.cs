using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface for devices that can write to the pending TOU table.
    /// </summary>
    public interface IPendingTOU
    {
        /// <summary>
        /// Writes the TOU schedule in the specified EDL file to the TOU pending table.
        /// </summary>
        /// <param name="strFileName">The EDL file that contains the TOU schedule</param>
        /// <param name="iSeasonIndex">The index of the season to write.</param>
        /// <returns>WritePendingTOUResult code.</returns>
        WritePendingTOUResult WritePendingTOU(string strFileName, int iSeasonIndex);
    }
}
