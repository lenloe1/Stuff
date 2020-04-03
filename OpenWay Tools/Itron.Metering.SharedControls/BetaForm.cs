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
//                              Copyright © 2010
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Displays a Beta Splash Screen
    /// </summary>
    public partial class BetaForm : Form
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/10 RCG 2.44.06        Created

        public BetaForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the Continue button click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/10 RCG 2.44.06        Created

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
