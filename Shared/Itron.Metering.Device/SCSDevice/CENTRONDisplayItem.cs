using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class represents a single item in a CENTRON's list of displayable items.   This class
	/// extends the display item definition from the bass class and 200 series to handle the few
	/// extra displayable items in the CENTRON's extended basepage area.
    /// </summary>
	internal class CENTRONDisplayItem : SCSDisplayItem
    {
        #region Public Methods
        /// <summary>
        /// Constructs an SCS display item from the bit mapped, 4 byte SCS display item
        /// record.  
        /// </summary>
        /// <param name="byDisplayTable"></param>
        /// <param name="nTableOffset"></param>
        /// <param name="boolTestMode"></param>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/07 KRC 8.00.10  N/A   Adding support for editable CCum
        /// </remarks>
        public CENTRONDisplayItem(ref byte[] byDisplayTable, int nTableOffset, bool boolTestMode)
            : base(ref byDisplayTable, nTableOffset, boolTestMode)
        {
        }

        #endregion

        #region Protected Properties
        /// <summary>
        /// Writes a Cum Value to the meter
        /// </summary>
        /// <param name="device">Device we are talking to</param>
        /// <param name="strValue">The value to set into the meter</param>
        /// <returns>ItronDeviceResult</returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/31/07 KRC 8.00.10  N/A   Getting Edit Registers working
        /// </remarks>
        override protected ItronDeviceResult SetCCumValue(ref SCSDevice device, string strValue)
        {
            ItronDeviceResult Result = ItronDeviceResult.SUCCESS;
         
            if (RegisterType == 0) // current season
            {
                Result = device.SetFloatingBCDValue(device.TranslateDisplayAddress(this), 4, strValue);
            }
            else if (RegisterType == 7) // last season
            {
                Result = device.SetFloatingBCDValue(device.TranslateDisplayAddress(this), 3, strValue);
            }

            return Result;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Editable property of the CENTRON Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/31/07 KRC 8.00.10 N/A    Created
        //
        public override bool Editable
        {
            get
            {
                if (RegisterClass == SCSDisplayClass.EnergyValue || RegisterClass == SCSDisplayClass.MaxDemandValue || 
                    RegisterClass == SCSDisplayClass.CumulativeValue || RegisterClass == SCSDisplayClass.TotalContinuousCumulativeValue ||
                    RegisterClass == SCSDisplayClass.TOUContinuousCumulativeValue)
                {
                    m_blnEditable = (RegisterType != 0x07);
                }
                else
                {
                    m_blnEditable = false;
                }

                return m_blnEditable;
            }
        }

        #endregion

    }
}
