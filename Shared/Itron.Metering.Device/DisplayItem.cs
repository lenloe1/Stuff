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
//                              Copyright © 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
#if (!WindowsCE)
using System.Runtime.Serialization;
#endif

namespace Itron.Metering.Device
{
    /// <summary>
    /// Display Item class - Provides access to properties associated with meter display items.
    /// </summary>
#if (!WindowsCE)
	[DataContract]
	[KnownType(typeof(ANSIDisplayItem))]
	[KnownType(typeof(OpenWayDisplayItem))]	
#endif    
	public abstract class DisplayItem
    {
        #region Public Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
        public DisplayItem()
        {
            m_strDescription = "";
			m_strValue = "";
			m_strDisplayID = "";
			m_blnEditable = false;
			m_blnNegativeAllowed = false;
        }

		/// <summary>
		/// Constructor
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//	03/06/08 mrj 1.00.00		Added to support serialization
		//
		public DisplayItem(string strDescription, string strValue, string strDisplayID)
		{
			m_strDescription = strDescription;
			m_strValue = strValue;
			m_strDisplayID = strDisplayID;
			m_blnEditable = false;
			m_blnNegativeAllowed = false;
		}
        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Description of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
#if (!WindowsCE)
		[DataMember]
#endif
        public virtual string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>
        /// Provides access to the value of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
#if (!WindowsCE)
		[DataMember]
#endif
        public virtual string Value
        {
            get
            {
                return m_strValue;
            }
            set
            {
                m_strValue = value;
            }
        }

        /// <summary>
        /// Provides access to the Display ID of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/06 KRC 7.35.00 N/A    Created
        //
#if (!WindowsCE)
		[DataMember]
#endif
        public virtual string DisplayID
        {
            get
            {
                return m_strDisplayID;
            }			
			set
			{
				m_strDisplayID = value;
			}			
        }

        /// <summary>
        /// Provides access to the Editable property of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/22/07 KRC 8.00.08 N/A    Created
        //
#if (!WindowsCE)
		[DataMember]
#endif
        public virtual bool Editable
        {
            get
            {
                return m_blnEditable;
            }			
			set
			{
				m_blnEditable = value;
			}			
        }

        /// <summary>
        /// Provides access to the Negative Allowed property of the Display Item
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  03/29/07 KRC 8.00.22 2714    Handling items that can and cannot be negative
        //
#if (!WindowsCE)
		[DataMember]
#endif
        public virtual bool NegativeAllowed
        {
            get
            {
                return m_blnNegativeAllowed;
            }			
			set
			{
				m_blnNegativeAllowed = value;
			}			
        }

        #endregion

        #region Protected Method

        /// <summary>
        /// MoveDecimal3ToTheLeft - This method effectively divides the number in the string passed in by 10^nShift
        ///  This is used so we do not have to convert to double and encounter the problems when doing double math
        /// </summary>
        /// <param name="strValue">string that contains a double that needs to have the decimal places shifted</param>
        /// <param name="nShift">number of place to shift by</param>
        /// <param name="nNumDecimalDigits">The number of deciamal digits to have in the value returned</param>
        /// <returns>The new string value with the decimal shifted nShift place to the left</returns>
        /// <remarks>
        /// Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/22/07 KRC 8.00.08 N/A    Added to support Edit Registers
        ///
        ///</remarks>		
        protected static string MoveDecimalToTheLeft(string strValue, int nShift, int nNumDecimalDigits)
        {
            string strNewValue = "";

            // We need to make sure the number has a decimal point or bad things could happen
			if (-1 == strValue.IndexOf('.'))
            {
                // Since there was no decimal point, add one plus some zeros to give us enough room to work.
                strValue += ".00000";
            }

            int nDecimalLocation = strValue.IndexOf('.');

            // We may need to add zeros in order to have enough room to shift, but if we add too many we could have a leading zero problem
            for (int iCount = nDecimalLocation; iCount < nShift + 1; iCount++)
            {
                strValue = "0" + strValue;
            }

            // Get Decimal location again, since it may have changed
            nDecimalLocation = strValue.IndexOf('.');

            strNewValue = strValue.Substring(0, nDecimalLocation - nShift) // Create the new decimal portion
                            + "."   // Add the decimal point
                            + strValue.Substring(nDecimalLocation - nShift, nShift) // Add the part between where we stopped and the old decimal point
                            + strValue.Substring(nDecimalLocation + 1, (strValue.Length - (nDecimalLocation + 1)));

            nDecimalLocation = strNewValue.IndexOf('.');

            // Now that we have new value, trim it to our target length
            if (nDecimalLocation >= 0)
            {
                int nTargetLength = nDecimalLocation + nNumDecimalDigits + nShift + 1; // add one because the decimal location is 0 based

                strNewValue = strNewValue.Substring(0, nTargetLength);
            }

            return strNewValue;
        }

        /// <summary>
        /// MoveDecimalToTheRight - This method effectively multiplies the number in the string passed in by 10^nShift
        ///  This is used so we do not have to convert to double and encounter the problems when doing double math
        /// </summary>
        /// <param name="strValue">string that contains a double that needs to have the decimal places shifted</param>
        /// <param name="nShift">number of place to shift by</param>
        /// <param name="nNumDecimalDigits">The number of deciamal digits to have in the value returned</param>
        /// <returns>The new string value with the decimal shifted nShift place to the right</returns>
        /// <remarks>
        /// Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/22/07 KRC 8.00.08 N/A    Added to support Edit Registers
        ///
        ///</remarks>
        protected static string MoveDecimalToTheRight(string strValue, int nShift, int nNumDecimalDigits)
        {
            string strNewValue = "";
            
            // We need to make sure the number has a decimal point or bad things could happen
			if (-1 == strValue.IndexOf('.'))
            {
                // Since there was no decimal point, add one
                strValue += ".";
            }

            // Add nine zeros, so we are sure that we have enough digits to do the following with. (6 places to shift and 3 decimal (Not realistic)
            strValue += "000000000";

            // Figure out the decimal location
            int nDecimalLocation = strValue.IndexOf('.');

            strNewValue = strValue.Substring(0, nDecimalLocation)  // copy the integer portion of the current value
                                        + strValue.Substring(nDecimalLocation + 1, nShift) // Then copy the next three digits (effectively multiplying by 1000)
                                        + "." // Add the decimal portion
                                        + strValue.Substring(nDecimalLocation + 4); // end with the rest of the string
            
            // Next set the correct number of decimal digits - reset the decimal location since it may have changed
            // when we scaled the 
            nDecimalLocation = strNewValue.IndexOf('.');

            // Now that we have new value, trim it to our target length
            if (nDecimalLocation >= 0)
            {
                int nTargetLength = nDecimalLocation + nNumDecimalDigits + 1; // add one because the decimal location is 0 based

                strNewValue = strNewValue.Substring(0, nTargetLength);
            }

            return strNewValue;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="nNumDecimalDigitsDesired"></param>
		/// <returns></returns>
        protected static string FormatDecimalDigits(string strValue, int nNumDecimalDigitsDesired )
        {
            string strNewValue = strValue;
			
			// Figure out the decimal location
            int nDecimalLocation = strValue.IndexOf('.');

			// Add a decimal point if none was found
			if (nDecimalLocation < 0)
			{
				strNewValue += ".";  
				nDecimalLocation = strNewValue.Length - 1;
			}

			int nNumberOfDecimalDigitsPresent = strNewValue.Length - (nDecimalLocation + 1);

			if (nNumberOfDecimalDigitsPresent > nNumDecimalDigitsDesired)
			{
				// we have too many decimal digits - strip some off
				int nDigitsToRemove = nNumberOfDecimalDigitsPresent - nNumDecimalDigitsDesired;

				strNewValue = strNewValue.Substring(0, strNewValue.Length - nDigitsToRemove);
			}
			else if (nNumberOfDecimalDigitsPresent < nNumDecimalDigitsDesired)
			{
				// We have too few decimal digits - add some more
				int nDigitsToAdd = nNumDecimalDigitsDesired - nNumberOfDecimalDigitsPresent;

				strNewValue = strNewValue.PadRight( strNewValue.Length + nDigitsToAdd, '0'); 
			}

			return strNewValue;
		}

        #endregion

        #region Members
        /// <summary>
        /// Description of Display Item
        /// </summary>
        protected string m_strDescription;
        /// <summary>
        /// Meter Value associated with Display Item
        /// </summary>
        protected string m_strValue;
        /// <summary>
        /// Display ID of Display Item
        /// </summary>
        protected string m_strDisplayID;
        /// <summary>
        /// Is this value editable
        /// </summary>
        protected bool m_blnEditable;
        /// <summary>
        /// Is the value allowed to be negative
        /// </summary>
        protected bool m_blnNegativeAllowed;

        #endregion
    }
}
