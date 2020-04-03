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
//                           Copyright © 2006 - 2007
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Resources;
using Itron.Metering.Communications.PSEM;

namespace Itron.Metering.Device
{
	/// <summary>
	/// This class represents the CENTRON C12.19 meter (a.k.a. IMAGE Mono)
	/// As the name implies this meter is a single phase ANSI C12.19 compliant meter.
	/// </summary>
	//  Revision History	
	//  MM/DD/YY who Version Issue# Description
	//  -------- --- ------- ------ ---------------------------------------
	//  05/22/06 mrj 7.30.00 N/A    Created
    //  04/02/07 AF  8.00.23 2814   Corrected the capitalization of the meter name
    //
	public class CENTRON_MONO : CANSIDevice
    {
        #region Constants

        /// <summary>
        /// Meter type identifier
        /// </summary>
        private const string CENTRONM = "CENTRONM";
        /// <summary>
        /// Human readable name of meter
        /// </summary>
        private const string CENTRONM_NAME = "CENTRON (C12.19)";

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
		public CENTRON_MONO( Itron.Metering.Communications.ICommunications ceComm )
			: base(ceComm) 
		{
			//Use the Centron LIDs
			m_LID = new CentronMonoDefinedLIDs();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="PSEM">Protocol obj used to identify the meter</param>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 05/22/06 mrj 7.30.00 N/A    Created
		///
		public CENTRON_MONO( CPSEM PSEM )
			: base(PSEM) 
		{
			//Use the Centron LIDs
			m_LID = new CentronMonoDefinedLIDs();
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
            return STDReconfigurePasswords( Passwords );

        } // ReconfigurePasswords

        #endregion

        #region Public Property

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
                return CENTRONM_NAME;
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
        //  01/06/09 KRC  2.10			Added for OpenWay
        //
        public override ANSIEventDictionary EventDescriptions
        {
            get
            {
                if (null == m_dicEventDescriptions)
                {
                    m_dicEventDescriptions = (ANSIEventDictionary)(new CENTRON_MONO_EventDictionary());
                }

                return m_dicEventDescriptions;
            }
        }

        #endregion

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

        internal override LID CreateLID(uint uiLIDNumber)
        {
            return new CentronMonoLID(uiLIDNumber);
        }

        #endregion

        #region Internal Property
        
        /// <summary>
        /// This property returns the correct version of the 2048 table for the
        /// Centron Mono meter.  (Creates it if necessary)
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/22/06 mrj 7.30.00 N/A    Created
        // 11/20/06 KRC 8.00.00 N/A    Changed to Property
        //
        internal override CTable2048 Table2048
        {
            get
            {
                if (null == m_Table2048)
                {
                    m_Table2048 = new CTable2048_Mono(m_PSEM);
                }

                return m_Table2048;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Diagnostics are not supported on the Mono meter
        /// </summary>
        /// <returns></returns>
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/26/06 mrj 7.30.00 N/A    Created
        ///
        protected override ItronDeviceResult ResetDiagnosticCounters()
        {
            throw (new NotSupportedException());
        }

        #endregion

        #region Protected Property

        /// <summary>
        /// Gets the meter type "CENTRONM
        /// </summary>		
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 04/25/06 mrj 7.30.00 N/A    Created
        //
        protected override string DefaultMeterType
        {
            get
            {
                return CENTRONM;
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
                return 0.01f;
            }
        }

        #endregion
    }
}
