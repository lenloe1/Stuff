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
//                            Copyright © 2010 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;

namespace Itron.Metering.SharedControls
{
	/// <summary>
	/// Helper class that contains a control hosted within a C1FlexGrid.
	/// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  09/22/10 jrf 2.45.03        Example class from component one sample code.
    // 
	public class HostedControl
    {
        #region Public Methods

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="flex">Flexgrid that the control resides in.</param>
		/// <param name="hosted">The control to be hosted in the grid</param>
		/// <param name="row">The row the control resides in.</param>
		/// <param name="col">The column that the control resides in.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 jrf 2.45.03        Created.
        // 
        public HostedControl(C1FlexGrid flex, Control hosted, int row, int col)
		{
			// save info
			m_c1FlexGrid = flex;
			m_ctlHosted  = hosted;
            m_iRow = row;
            m_iCol = col;

			// insert hosted control into grid
			m_c1FlexGrid.Controls.Add(m_ctlHosted);
		}

		/// <summary>
		/// This method updates the position of the control in the flexgrid.  It must 
        /// be called in the flexgrid's paint event handler.
		/// </summary>
		/// <returns>Whether or not the position was updated.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 jrf 2.45.03        Created.
        //
        public bool UpdatePosition()
		{
			// get row/col indices
			int r = m_iRow;
			int c = m_iCol;
            if (r < 0 || c < 0 || r >= m_c1FlexGrid.Rows.Count || c >= m_c1FlexGrid.Cols.Count)
            {
                return false;
            }

			// get cell rect
			Rectangle rc = m_c1FlexGrid.GetCellRect(r, c, false);

			// hide control if out of range
			if (rc.Width <= 0 || rc.Height <= 0 || !rc.IntersectsWith(m_c1FlexGrid.ClientRectangle))
			{
				m_ctlHosted.Visible = false;
				return true;
			}

			// move the control and show it
			m_ctlHosted.Bounds = rc;
            m_ctlHosted.Visible = true;

			// done
			return true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The control being hosted.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/10 jrf 2.45.03        Created.
        //
        public Control RootControl
        {
            get
            {
                return m_ctlHosted;
            }
            set
            {
                m_ctlHosted = value;
            }
        }

        /// <summary>
        /// The row the control is in.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/10 jrf 9.70.16        Created.
        //
        public int Row
        {
            get
            {
                return m_iRow;
            }
            set
            {
                m_iRow = value;
            }
        }

        /// <summary>
        /// The row the control is in.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  12/07/10 jrf 9.70.16        Created.
        //
        public int Column
        {
            get
            {
                return m_iCol;
            }
            set
            {
                m_iCol = value;
            }
        }

        #endregion

        #region Members

        internal C1FlexGrid m_c1FlexGrid;
        internal Control m_ctlHosted;
        internal int m_iRow;
        internal int m_iCol;

        #endregion
    }//end HostedControl


}
