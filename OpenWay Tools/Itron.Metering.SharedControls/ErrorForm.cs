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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// This Windows Form is used to display error messages and, optionally,
    /// the detailed diagnostic information provided when an exception is caught
    /// </summary>
    public partial class ErrorForm : C1.Win.C1Ribbon.C1RibbonForm
    {

        #region Definitions

        private const int ERROR_ICON = 0;
        private const int WARNING_ICON = 1;

        #endregion

        #region Contructors

        /// <summary>
        /// Contructs a form to display an error message
        /// </summary>
        public ErrorForm()
            : base()
        {
            InitializeComponent();

            btnDetails.Enabled = false;
            m_bDetailsVisible = false;

            ResizeDialog();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Use this method to display non-fatal error messages.  This method must be used 
        /// to insure consistency between all data views and to provide as much visibility as 
        /// possible to all error information. 
        /// </summary>
        /// <param name="strErrorMessage">
        /// This is the primary message that will be displayed to the user</param>
        /// <param name="e">
        /// This is the exception object that originally caused the error.  It is important to
        /// provide this information to the user when available to assist in debugging and error 
        /// resolution
        /// </param>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  09/13/07 MAH                 Created
        ///
        /// </remarks>
        public static void DisplayError(String strErrorMessage, Exception e)
        {
            ErrorForm frmErrorNotification = new ErrorForm();

            frmErrorNotification.Message = strErrorMessage;
            frmErrorNotification.IconSelection = MessageBoxIcon.Error;

            frmErrorNotification.OriginatingException = e;

            frmErrorNotification.ShowDialog();
        }

        /// <summary>
        /// Use this method to display warning messages. 
        /// </summary>
        /// <param name="strWarningMessage">
        /// This is the primary message that will be displayed to the user</param>
        /// <param name="e">
        /// This is the exception object that originally caused the warning.  
        /// </param>        
        public static void DisplayWarning(String strWarningMessage, Exception e)
        {
            ErrorForm frmErrorNotification = new ErrorForm();

            frmErrorNotification.Message = strWarningMessage;
            frmErrorNotification.IconSelection = MessageBoxIcon.Warning;
            

            frmErrorNotification.OriginatingException = e;

            if (null == e)
            {
                frmErrorNotification.btnDetails.Visible = false;
            }

            frmErrorNotification.Text = Properties.Resources.Warning;
            

            frmErrorNotification.ShowDialog();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property sets the error message displayed to the user.  This
        /// message should be easily understood and instructive to the user but
        /// not filled with debugging details
        /// </summary>
        public String Message
        {
            set
            {
                lblMessage.Text = value;
            }
        }

        /// <summary>
        /// This property exposes the member data of the exception
        /// that caused the offending error.
        /// </summary>
        public Exception OriginatingException
        {
            set
            {
                Exception e = value;

                DetailFlexGrid.Rows.RemoveRange(1, DetailFlexGrid.Rows.Count - 1);

                if (null != e)
                {
                    btnDetails.Enabled = true;

                    AddItem("Exception Type", e.GetType().Name);

                    if (e.Message != null)
                    {
                        AddItem("Message", e.Message);
                    }

                    if (null != e.Source)
                    {
                        AddItem("Source", e.Source);
                    }

                    if (null != e.TargetSite)
                    {
                        if (null != e.TargetSite.Module)
                        {
                            AddItem("Target Module", e.TargetSite.Module.Name);
                        }

                        AddItem("Target Site", e.TargetSite.Name);
                    }

                    if (null != e.StackTrace)
                    {
                        AddItem("Stack Trace", e.StackTrace);
                    }
                }
            }
        }

        /// <summary>
        /// This property sets the error image on the form.  Note that only
        /// a warning icon and error icon are supported
        /// </summary>
        public MessageBoxIcon IconSelection
        {
            set
            {
                if (value == MessageBoxIcon.Warning)
                {
                    pictureWarningIcon.BringToFront();
                }
                else
                {
                    pictureErrorIcon.BringToFront();
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a new item to the flex grid
        /// </summary>
        /// <param name="strDetailName">The name of the item</param>
        /// <param name="strValue">The item's value</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/10 RCG	2.40.27			Created

        private void AddItem(string strDetailName, string strValue)
        {
            Row NewRow = DetailFlexGrid.Rows.Add();

            NewRow["Detail"] = strDetailName;
            NewRow["Value"] = strValue;

            DetailFlexGrid.AutoSizeCols();
            DetailFlexGrid.AutoSizeRow(NewRow.Index);
        }

        /// <summary>
        /// This is the event handler for the OK button.  All that the
        /// handler does is close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The event handler for the details button expands the form
        /// to show the exception details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  09/13/07 MAH                 Created
        ///  11/06/07 MAH  9.00.23 Changed the list view's Visible default property to false and
        ///										only make it visible if the details button is clicked - SCR #3243
        ///
        /// </remarks>
        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (!m_bDetailsVisible)
            {
                // It also helps if we make the details list box visible
                DetailFlexGrid.Visible = true;

                // Only allow the user to ask for details once!
                m_bDetailsVisible = true;
                btnDetails.Enabled = false;

                // Expand the height of the window to show the list box
                // that details the exception information
                ResizeDialog();
            }
        }

        /// <summary>
        /// Resized the dialog based on whether or not the details are displayed
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/19/10 RCG	2.40.27			Created

        private void ResizeDialog()
        {
            if (m_bDetailsVisible)
            {
                Height = DetailFlexGrid.Location.Y + DetailFlexGrid.Height + 30;
            }
            else
            {
                Height = lblMessage.Location.Y + lblMessage.Height + 20;
            }
        }

        #endregion

        #region Private Members

        private Boolean m_bDetailsVisible;

        #endregion
    }
}