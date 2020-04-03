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
//                           Copyright © 2009 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using Itron.Metering.Progressable;
using Itron.Metering.CustomerValidation;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Control used for displaying the progress of a test.
    /// </summary>
    public partial class ProgressControl : UserControl
    {
        #region Definitions

        private delegate void ShowProgressDelegate(ShowProgressEventArgs e);
        private delegate void StepProgressDelegate(ProgressEventArgs e);
        private delegate void HideProgressDelegate(EventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        /// Event thrown when the View Details button is clicked.
        /// </summary>
        public event EventHandler ViewDetailsClickedEvent;
        /// <summary>
        /// Event thrown when the test completes
        /// </summary>
        public event EventHandler TestCompleteEvent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public ProgressControl()
        {
            InitializeComponent();

            m_strResultsFilePath = null;
        }

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable ShowProgress event.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void ShowProgressHandler(object sender, ShowProgressEventArgs e)
        {
            if (this.Disposing == false && this.IsDisposed == false)
            {
                this.BeginInvoke(new ShowProgressDelegate(ShowProgress), new object[] { e });
            }
        }

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable HideProgress event.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void HideProgressHandler(object sender, EventArgs e)
        {
            if (this.Disposing == false && this.IsDisposed == false)
            {
                this.BeginInvoke(new HideProgressDelegate(HideProgress), new object[] { e });
            }
        }

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable StepProgress event.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void StepProgressHandler(object sender, ProgressEventArgs e)
        {
            if (this.Disposing == false && this.IsDisposed == false)
            {
                this.BeginInvoke(new StepProgressDelegate(StepProgress), new object[] { e });
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public string Title
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the location of the results file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public string ResultsFile
        {
            get
            {
                return m_strResultsFilePath;
            }
            set
            {
                m_strResultsFilePath = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable ShowProgress event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ShowProgress(ShowProgressEventArgs e)
        {
            ProgressBar.Value = 0;
            ProgressBar.Maximum = e.NumberOfSteps;
            ProgressBar.Step = e.StepSize;

            lblStatus.Text = e.Status;
            lblTitle.Text = e.Title;

            m_iPassedCount = 0;
            m_iFailedCount = 0;
            m_iSkippedCount = 0;

            UpdateResultCounts();

            btnViewDetails.Visible = false;
        }

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable HideProgress event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void HideProgress(EventArgs e)
        {
            TestCompleteEventArgs TestCompleteArgs = e as TestCompleteEventArgs;

            ProgressBar.Value = ProgressBar.Maximum;

            if (TestCompleteArgs != null)
            {
                lblStatus.Text = TestCompleteArgs.Status;
                ResultsFile = TestCompleteArgs.ResultsFilePath;

                if (ResultsFile != null && File.Exists(ResultsFile))
                {
                    btnViewDetails.Visible = true;
                }
            }
            else
            {
                lblStatus.Text = Properties.Resources.StatusComplete;
            }

            OnTestComplete();
        }

        /// <summary>
        /// Event handler for the Itron.Metering.Progressable StepProgress event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void StepProgress(ProgressEventArgs e)
        {
            TestStepEventArgs TestStepArgs = e as TestStepEventArgs;

            ProgressBar.PerformStep();
            lblStatus.Text = e.Status;

            if (TestStepArgs != null)
            {
                if (TestStepArgs != null)
                {
                    lblTitle.Text = TestStepArgs.Title;
                }

                switch (TestStepArgs.StepResult)
                {
                    case TestStepResult.Passed:
                        {
                            m_iPassedCount++;
                            break;
                        }
                    case TestStepResult.Failed:
                        {
                            m_iFailedCount++;
                            break;
                        }
                    case TestStepResult.Skipped:
                        {
                            m_iSkippedCount++;
                            break;
                        }
                }

                UpdateResultCounts();
            }
        }

        /// <summary>
        /// Updates the displayed result counts
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void UpdateResultCounts()
        {
            lblPassedCount.Text = m_iPassedCount.ToString(CultureInfo.CurrentCulture);
            lblFailedCount.Text = m_iFailedCount.ToString(CultureInfo.CurrentCulture);
            lblSkippedCount.Text = m_iSkippedCount.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Raises the View Details Clicked event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void OnViewDetailsClicked()
        {
            if (ViewDetailsClickedEvent != null)
            {
                ViewDetailsClickedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Test Complete event
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void OnTestComplete()
        {
            if (TestCompleteEvent != null)
            {
                TestCompleteEvent(this, new EventArgs());
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the View Details button click event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            OnViewDetailsClicked();
        }

        #endregion

        #region Member Variables

        private string m_strResultsFilePath;
        private int m_iPassedCount;
        private int m_iFailedCount;
        private int m_iSkippedCount;

        #endregion
    }
}
