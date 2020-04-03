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
//                              Copyright © 2006 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using Itron.Metering.Progressable;

namespace Itron.Metering.Device
{
    /// <summary>
    /// ItronDevice class - This is the base class of all meter objects.
    /// </summary>
    /// <remarks>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 04/12/06 mrj 7.30.00    N/A Created
    /// 12/05/06 MAH 8.00.00    Added load profile status properties
    /// 01/16/14 DLG 3.50.26        Moved the DisplayMode Enum to the CANSIDevice object.
    /// </remarks>
    public abstract class ItronDevice : IProgressable
    {
        #region Public Events

        /// <summary>
        /// Event used to display a progress bar
        /// </summary>
        public virtual event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event used to cause a progress bar to perform a step
        /// </summary>
        public virtual event StepProgressEventHandler StepProgressEvent;

        /// <summary>
        /// Event used to hide a progress bar
        /// </summary>
        public virtual event HideProgressEventHandler HideProgressEvent;

        #endregion Public Events

        #region Public Methods

        /// <summary>Constructor.</summary>
		/// <example>
		/// <code>
		/// VECTRON m_Vectron;
		/// ItronDevice m_ItronDevice;
		///
		/// m_Vectron = new VECTRON( m_CommPort );
		/// m_ItronDevice = m_Vectron;
		///	
		///	//Logon to the meter
		/// m_ItronDevice.Logon();
		/// </code>
		/// </example>
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 04/12/06 mrj 7.30.00    N/A Created
        /// 02/05/07 jrf 8.00.10        Added intitialization for new member var
		///
		public ItronDevice()
		{}

		/// <summary>
		/// The abstract logon method which must be implemented by the derived
		/// class.
		/// </summary>
		/// <returns>A ItronDeviceResult object</returns>
		public abstract ItronDeviceResult Logon();

		/// <summary>
		/// The abstract logoff method which must be implemented by the derived
		/// class.
		/// </summary>
		public abstract void Logoff();
		
        #endregion Protected Methods

        #region Internal Properties

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal virtual void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal virtual void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/06 RCG 7.35.00    N/A Created
        // 01/06/14 DLG 3.50.19        Changed from protected to internal.
        //
        internal virtual void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        #endregion Internal Properties
    }
}