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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Globalization;
using System.ServiceModel.Security;
using C1.Win.C1Ribbon;
using C1.Win.C1Command;
using C1.Win.C1FlexGrid;
using Itron.Metering.Utilities;
using Itron.Metering.Communications;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.CustomerValidationTool.Properties;
using Itron.Metering.SharedControls;
using Itron.Metering.Progressable;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.Device;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Main form for the Customer Validation Tool
    /// </summary>
    public partial class CustomerValidationToolForm : C1RibbonForm
    {
        #region Definitions

        delegate void HandleFatalErrorDelegate(object sender, ItronErrorEventArgs e);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        public CustomerValidationToolForm()
        {
            InitializeComponent();

            // Check to see if we should upgrade the user settings from a previous version.  
            if (Settings.Default.CallUpgrade == true)
            {
                Settings.Default.Upgrade();
                Settings.Default.CallUpgrade = false;
                Settings.Default.Save();
            }

            InitializeSelectedTests();
            InitializeBaudRates();
            InitializeOpticalProbes();
            InitializePorts();

            ProgramFilePath = Settings.Default.ProgramFile;
            ResultsDirPath = Settings.Default.ResultsDir;

            UpdateStartAndCancelButtons();
            UpdateExportAndPrintButtons();

            m_RunningValidationWorkers = new List<ValidationWorker>();
            m_RunningThreads = new List<Thread>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Process key presses.
        /// </summary>
        /// <param name="msg">The key press message</param>
        /// <param name="keyData">The key press data</param>
        /// <returns>True if the key was processed. False otherwise.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bProcessed = false;

            if (ProgramEditBox.TextBox.Focused || ResultsDirEditBox.TextBox.Focused
                || cmbTest1Port.TextBox.Focused || cmbTest1Probe.TextBox.Focused
                || cmbTest1Rate.TextBox.Focused || cmbTest2Port.TextBox.Focused
                || cmbTest2Probe.TextBox.Focused || cmbTest2Rate.TextBox.Focused
                || cmbTest3Port.TextBox.Focused || cmbTest3Probe.TextBox.Focused
                || cmbTest3Rate.TextBox.Focused || cmbTest4Port.TextBox.Focused
                || cmbTest4Probe.TextBox.Focused || cmbTest4Rate.TextBox.Focused)
            {
                if (Keys.Up != keyData && Keys.Down != keyData && Keys.Tab != keyData
                    && Keys.Left != keyData && Keys.Right != keyData)
                {
                    keyData = Keys.None;
                    bProcessed = true;
                }
            }

            if (bProcessed == false)
            {
                bProcessed = base.ProcessCmdKey(ref msg, keyData);
            }

            return bProcessed;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the exit button click event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the start button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void StartTestButton_Click(object sender, EventArgs e)
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            List<ProgressControl> ProgControls = new List<ProgressControl>();
            ValidationWorker NewValidationWorker = null;
            bool bErrorDisplayed = false;

            if (SystemSettings.UseSignedAuthorization == true && (Settings.Default.AuthenticationKey == null 
                || Settings.Default.AuthenticationKey.IsValid == false))
            {
                // We need to try and update the key
                try
                {
                    SignedAuthenticator.UpdateSignedAuthorizationKey();
                }
                catch (MessageSecurityException ex)
                {
                    ErrorForm.DisplayError("Unable to update signed authorization credentials: Invalid Username/Password", ex);// Terminology changed per CQ183118
                    bErrorDisplayed = true;
                }
                catch (Exception ex)
                {
                    ErrorForm.DisplayError("The signed authorization credentials are invalid and could not be updated.", ex);
                    bErrorDisplayed = true;
                }
            }

            if (SystemSettings.UseSignedAuthorization == false || (Settings.Default.AuthenticationKey != null 
                && Settings.Default.AuthenticationKey.IsValid))
            {
                m_RunningThreads.Clear();
                m_RunningValidationWorkers.Clear();

                // Always add first test
                if (cbUseProbe1.Checked)
                {
                    NewValidationWorker = new ValidationWorker(Resources.Probe1,
                        Settings.Default.Test1PortDefault,
                        uint.Parse(Settings.Default.Test1RateDefault, CultureInfo.CurrentCulture),
                        COpticalProbes.GetOpticalProbeType(Settings.Default.Test1ProbeDefault));

                    ProgControls.Add(CreateProgressControl(NewValidationWorker));
                }

                if (cbUseProbe2.Checked)
                {
                    NewValidationWorker = new ValidationWorker(Resources.Probe2,
                        Settings.Default.Test2PortDefault,
                        uint.Parse(Settings.Default.Test2RateDefault, CultureInfo.CurrentCulture),
                        COpticalProbes.GetOpticalProbeType(Settings.Default.Test2ProbeDefault));

                    ProgControls.Add(CreateProgressControl(NewValidationWorker));
                }

                if (cbUseProbe3.Checked)
                {
                    NewValidationWorker = new ValidationWorker(Resources.Probe3,
                        Settings.Default.Test3PortDefault,
                        uint.Parse(Settings.Default.Test3RateDefault, CultureInfo.CurrentCulture),
                        COpticalProbes.GetOpticalProbeType(Settings.Default.Test3ProbeDefault));

                    ProgControls.Add(CreateProgressControl(NewValidationWorker));
                }

                if (cbUseProbe4.Checked)
                {
                    NewValidationWorker = new ValidationWorker(Resources.Probe4,
                        Settings.Default.Test4PortDefault,
                        uint.Parse(Settings.Default.Test4RateDefault, CultureInfo.CurrentCulture),
                        COpticalProbes.GetOpticalProbeType(Settings.Default.Test4ProbeDefault));

                    ProgControls.Add(CreateProgressControl(NewValidationWorker));
                }

                ProgressFlowPanel.Controls.Clear();
                ProgressFlowPanel.Controls.AddRange(ProgControls.ToArray());

                // Start the tests.
                foreach (Thread CurrentThread in m_RunningThreads)
                {
                    CurrentThread.Start();
                    // Add a little time in between starting so that the threads
                    // are staggered a bit. This will help avoid issues with the HAN
                    // test.
                    Thread.Sleep(500);
                }

                UpdateStartAndCancelButtons();
                UpdateTestSettingsGroupState();
            }
            else if ((Settings.Default.AuthenticationKey == null || Settings.Default.AuthenticationKey.IsValid == false) 
                && bErrorDisplayed == false) 
            {
                // We managed to get an Authorization key that was not valid but no exception occurred when attempting
                // to get a new one so we should display an error to the user.
                ErrorForm.DisplayError("The signed authorization credentials are invalid and could not be updated.", null);
            }
        }

        /// <summary>
        /// Creates a new progress control for the specified validation worker.
        /// </summary>
        /// <param name="newValidationWorker">The validation worker to create the progress control for.</param>
        /// <returns>Tje mew progress control.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/09 RCG 2.30.16        Created
        //  04/08/13 jrf 3.50.68 458134 Adding handler for the ValidationWorker's FatalErrorEvent.
        private ProgressControl CreateProgressControl(ValidationWorker newValidationWorker)
        {
            Thread NewThread;
            ProgressControl NewProgressControl = new ProgressControl();

            NewProgressControl.ViewDetailsClickedEvent += new EventHandler(ProgressControl_ViewDetailsClickedEvent);
            NewProgressControl.TestCompleteEvent += new EventHandler(ProgressControl_TestCompleteEvent);

            newValidationWorker.ShowProgressEvent += new ShowProgressEventHandler(NewProgressControl.ShowProgressHandler);
            newValidationWorker.HideProgressEvent += new HideProgressEventHandler(NewProgressControl.HideProgressHandler);
            newValidationWorker.StepProgressEvent += new StepProgressEventHandler(NewProgressControl.StepProgressHandler);

            newValidationWorker.FatalErrorEvent += new FatalErrorEventHandler(HandleFatalError);

            m_RunningValidationWorkers.Add(newValidationWorker);

            NewThread = new Thread(new ThreadStart(newValidationWorker.RunValidationTests));
            m_RunningThreads.Add(NewThread);

            return NewProgressControl;
        }

        /// <summary>
        /// Handles the ProgressControls Test complete event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void ProgressControl_TestCompleteEvent(object sender, EventArgs e)
        {
            ProgressControl CompletedProgressControl = sender as ProgressControl;

            if (CompletedProgressControl != null && CompletedProgressControl.ResultsFile != null)
            {
                OpenResultsFile(CompletedProgressControl.ResultsFile);
            }

            // On slower computers it is possible that the thread itself will still be
            // alive after this event has been sent so we need to give it some time to be cleared
            Thread.Sleep(750);

            UpdateStartAndCancelButtons();
            UpdateTestSettingsGroupState();
        }

        /// <summary>
        /// Handles the ProgressControl's View Details click event.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void ProgressControl_ViewDetailsClickedEvent(object sender, EventArgs e)
        {
            bool bResultsFound = false;
            ProgressControl CurrentProgressControl = sender as ProgressControl;

            if (CurrentProgressControl != null)
            {
                // First check and see if the results are already open.
                foreach (ResultDockingTabPage CurrentTabPage in ResultsDockingTab.TabPages.OfType<ResultDockingTabPage>())
                {
                    if (CurrentTabPage.FileName.Equals(CurrentProgressControl.ResultsFile)
                        && CurrentTabPage.IsDisposed == false)
                    {
                        bResultsFound = true;
                        ResultsDockingTab.SelectedTab = CurrentTabPage;
                        break;
                    }
                }

                if (bResultsFound == false)
                {
                    // We didn't find the file so we need to open it
                    OpenResultsFile(CurrentProgressControl.ResultsFile);
                }
            }
        }

        /// <summary>
        /// Initializes the check boxes for the tests selected to run.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void InitializeSelectedTests()
        {
            cbUseProbe1.Checked = Settings.Default.Test1Enabled;
            cbUseProbe2.Checked = Settings.Default.Test2Enabled;
            cbUseProbe3.Checked = Settings.Default.Test3Enabled;
            cbUseProbe4.Checked = Settings.Default.Test4Enabled;
        }

        /// <summary>
        /// Initializes the Port selection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void InitializePorts()
        {
            // Clear any prior data
            cmbTest1Port.Items.Clear();
            cmbTest2Port.Items.Clear();
            cmbTest3Port.Items.Clear();
            cmbTest4Port.Items.Clear();

            // Add none to the available selections
            cmbTest1Port.Items.Add(Resources.None);
            cmbTest2Port.Items.Add(Resources.None);
            cmbTest3Port.Items.Add(Resources.None);
            cmbTest4Port.Items.Add(Resources.None);

            // Add the available port names
            foreach (string CurrentPort in SerialPort.GetPortNames())
            {
                cmbTest1Port.Items.Add(CurrentPort);
                cmbTest2Port.Items.Add(CurrentPort);
                cmbTest3Port.Items.Add(CurrentPort);
                cmbTest4Port.Items.Add(CurrentPort);
            }

            // Select the defaults
            cmbTest1Port.Text = Settings.Default.Test1PortDefault;
            cmbTest2Port.Text = Settings.Default.Test2PortDefault;
            cmbTest3Port.Text = Settings.Default.Test3PortDefault;
            cmbTest4Port.Text = Settings.Default.Test4PortDefault;

            HideTestSettingsGroups();
            HideUsedPorts();
            EnableTestSelection();
        }

        /// <summary>
        /// Initializes the Optical Probe combo boxes.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void InitializeOpticalProbes()
        {
            COpticalProbes OpticalProbes = new COpticalProbes();

            if (null != OpticalProbes)
            {
                for (int iIndex = 0; iIndex < OpticalProbes.Length; iIndex++ )
                {
                    cmbTest1Probe.Items.Add(OpticalProbes[iIndex].m_strTitle);
                    cmbTest2Probe.Items.Add(OpticalProbes[iIndex].m_strTitle);
                    cmbTest3Probe.Items.Add(OpticalProbes[iIndex].m_strTitle);
                    cmbTest4Probe.Items.Add(OpticalProbes[iIndex].m_strTitle);
                }

                // Select the defaults
                cmbTest1Probe.Text = Settings.Default.Test1ProbeDefault;
                cmbTest2Probe.Text = Settings.Default.Test2ProbeDefault;
                cmbTest3Probe.Text = Settings.Default.Test3ProbeDefault;
                cmbTest4Probe.Text = Settings.Default.Test4ProbeDefault;
            }
        }

        /// <summary>
        /// Initializes the baud rate combo boxes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void InitializeBaudRates()
        {
            int[] iBaudRates = { 9600, 14400, 19200, 28800 };

            foreach(int CurrentRate in iBaudRates)
            {
                cmbTest1Rate.Items.Add(CurrentRate.ToString(CultureInfo.CurrentCulture));
                cmbTest2Rate.Items.Add(CurrentRate.ToString(CultureInfo.CurrentCulture));
                cmbTest3Rate.Items.Add(CurrentRate.ToString(CultureInfo.CurrentCulture));
                cmbTest4Rate.Items.Add(CurrentRate.ToString(CultureInfo.CurrentCulture));
            }

            // Select the defaults
            cmbTest1Rate.Text = Settings.Default.Test1RateDefault;
            cmbTest2Rate.Text = Settings.Default.Test2RateDefault;
            cmbTest3Rate.Text = Settings.Default.Test3RateDefault;
            cmbTest4Rate.Text = Settings.Default.Test4RateDefault;
        }

        /// <summary>
        /// Handles the selection changed event for the Port combo boxes
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void Port_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideTestSettingsGroups();
            EnableTestSelection();
            UpdatePortDefaults();
        }

        /// <summary>
        /// Handles the selection changed event for the Probe combo boxes
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void Probe_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Test1ProbeDefault = cmbTest1Probe.Text;
            Settings.Default.Test2ProbeDefault = cmbTest2Probe.Text;
            Settings.Default.Test3ProbeDefault = cmbTest3Probe.Text;
            Settings.Default.Test4ProbeDefault = cmbTest4Probe.Text;

            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the selection changed event for the Rate combo boxes
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void Rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Test1RateDefault = cmbTest1Rate.Text;
            Settings.Default.Test2RateDefault = cmbTest2Rate.Text;
            Settings.Default.Test3RateDefault = cmbTest3Rate.Text;
            Settings.Default.Test4RateDefault = cmbTest4Rate.Text;

            Settings.Default.Save();
        }

        /// <summary>
        /// Updates the default values for the selected Ports
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void UpdatePortDefaults()
        {
            Settings.Default.Test1PortDefault = cmbTest1Port.Text;
            Settings.Default.Test2PortDefault = cmbTest2Port.Text;
            Settings.Default.Test3PortDefault = cmbTest3Port.Text;
            Settings.Default.Test4PortDefault = cmbTest4Port.Text;

            Settings.Default.Save();
        }

        /// <summary>
        /// Enables or disables the test selection boxes.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void EnableTestSelection()
        {
            // Enable or disable the selection boxes
            cbUseProbe1.Enabled = cmbTest1Port.Text != Resources.None;
            cbUseProbe2.Enabled = cmbTest2Port.Text != Resources.None;
            cbUseProbe3.Enabled = cmbTest3Port.Text != Resources.None;
            cbUseProbe4.Enabled = cmbTest4Port.Text != Resources.None;

            // Clear the check from the disabled check boxes.
            if (cbUseProbe1.Enabled == false)
            {
                cbUseProbe1.Checked = false;
            }

            if (cbUseProbe2.Enabled == false)
            {
                cbUseProbe2.Checked = false;
            }

            if (cbUseProbe3.Enabled == false)
            {
                cbUseProbe3.Checked = false;
            }

            if (cbUseProbe4.Enabled == false)
            {
                cbUseProbe4.Checked = false;
            }
        }

        /// <summary>
        /// Shows or hides the test settings groups that are not being used.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void HideTestSettingsGroups()
        {
            // Make the Settings groups visible based on the other selections
            Probe2SettingsGroup.Visible = cmbTest1Port.Text != Resources.None;

            Probe3SettingsGroup.Visible = cmbTest1Port.Text != Resources.None
                && cmbTest2Port.Text != Resources.None;

            Probe4SettingsGroup.Visible = cmbTest1Port.Text != Resources.None
                && cmbTest2Port.Text != Resources.None
                && cmbTest3Port.Text != Resources.None;

            // Set the port of invisible groups to none
            if (Probe2SettingsGroup.Visible == false)
            {
                cmbTest2Port.Text = Resources.None;
            }

            if (Probe3SettingsGroup.Visible == false)
            {
                cmbTest3Port.Text = Resources.None;
            }

            if (Probe4SettingsGroup.Visible == false)
            {
                cmbTest4Port.Text = Resources.None;
            }
        }

        /// <summary>
        /// Hides ports that are currently used in other tests.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void HideUsedPorts()
        {
            string[] UsedPorts = { cmbTest1Port.Text, cmbTest2Port.Text, cmbTest3Port.Text, cmbTest4Port.Text };

            foreach (RibbonButton CurrentButton in cmbTest1Port.Items.OfType<RibbonButton>())
            {
                CurrentButton.Visible = CurrentButton.Text == Resources.None 
                    || UsedPorts.Contains(CurrentButton.Text) == false
                    || CurrentButton.Text == cmbTest1Port.Text;
            }

            foreach (RibbonButton CurrentButton in cmbTest2Port.Items.OfType<RibbonButton>())
            {
                CurrentButton.Visible = CurrentButton.Text == Resources.None
                    || UsedPorts.Contains(CurrentButton.Text) == false
                    || CurrentButton.Text == cmbTest2Port.Text;
            }

            foreach (RibbonButton CurrentButton in cmbTest3Port.Items.OfType<RibbonButton>())
            {
                CurrentButton.Visible = CurrentButton.Text == Resources.None
                    || UsedPorts.Contains(CurrentButton.Text) == false
                    || CurrentButton.Text == cmbTest3Port.Text;
            }

            foreach (RibbonButton CurrentButton in cmbTest4Port.Items.OfType<RibbonButton>())
            {
                CurrentButton.Visible = CurrentButton.Text == Resources.None
                    || UsedPorts.Contains(CurrentButton.Text) == false
                    || CurrentButton.Text == cmbTest4Port.Text;
            }
        }

        /// <summary>
        /// Prevents the user from typing in one of the combo boxes.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/26/09 RCG 2.30.00        Created

        private void ComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Just mark the event as handled.
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Browse Program click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void BrowseProgramButton_Click(object sender, EventArgs e)
        {
            OpenProgramDialog OpenProgram = new OpenProgramDialog();

            if (OpenProgram.ShowDialog() == DialogResult.OK)
            {
                ProgramFilePath = OpenProgram.SelectedFile;
            }
        }

        /// <summary>
        /// Handles the Browse Results button click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ResultsBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog SelectDirectoryDialog = new FolderBrowserDialog();

            SelectDirectoryDialog.ShowNewFolderButton = true;

            if (Directory.Exists(ResultsDirEditBox.Text))
            {
                SelectDirectoryDialog.SelectedPath = ResultsDirEditBox.Text;
            }
            else
            {
                SelectDirectoryDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (SelectDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                ResultsDirPath = SelectDirectoryDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Handles the About button click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void AboutButton_Click(object sender, EventArgs e)
        {
            AboutForm About = new AboutForm();

            About.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event for the Run Test check boxes.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void RunTestCheckBox_Click(object sender, EventArgs e)
        {
            Settings.Default.Test1Enabled = cbUseProbe1.Checked;
            Settings.Default.Test2Enabled = cbUseProbe2.Checked;
            Settings.Default.Test3Enabled = cbUseProbe3.Checked;
            Settings.Default.Test4Enabled = cbUseProbe4.Checked;

            Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Checked Changed event for the Run Test check boxes.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void RunTestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateStartAndCancelButtons();
        }

        /// <summary>
        /// Updates the enabled state of the start and cancel buttons based on the current settings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void UpdateStartAndCancelButtons()
        {
            StartTestButton.Enabled = TestRunCount > 0 && IsTestRunning == false;
            CancelTestButton.Enabled = IsTestRunning == true;
        }

        /// <summary>
        /// Updates the enabled state of the export and print buttons
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void UpdateExportAndPrintButtons()
        {
            bool bEnabled = ResultsDockingTab.TabPages.Count > 0;

            PrintMenuButton.Enabled = bEnabled;
            PrintSplitButton.Enabled = bEnabled;
            PrintPreviewButton.Enabled = bEnabled;
            PrintPreviewMenuButton.Enabled = bEnabled;

            ExportButton.Enabled = bEnabled;
            ExportMenuButton.Enabled = bEnabled;
        }

        /// <summary>
        /// Updates the enabled state of the test settings group based on whether or not tests are running
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void UpdateTestSettingsGroupState()
        {
            FileSettingsGroup.Enabled = IsTestRunning == false;
            AuthorizationGroup.Enabled = IsTestRunning == false;
            ProbeSelectionGroup.Enabled = IsTestRunning == false;
            Probe1SettingsGroup.Enabled = IsTestRunning == false;
            Probe2SettingsGroup.Enabled = IsTestRunning == false;
            Probe3SettingsGroup.Enabled = IsTestRunning == false;
            Probe4SettingsGroup.Enabled = IsTestRunning == false;
        }

        /// <summary>
        /// Handles the Cancel Test button click event
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void CancelTestButton_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        /// <summary>
        /// Cancels the currently running tests.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/22/10 RCG 2.40.27        Created

        private void CancelTests()
        {
            foreach (ValidationWorker CurrentWorker in m_RunningValidationWorkers)
            {
                CurrentWorker.CancelTests();
            }

            // It's possible that on slower computers we may not see the end of a test
            // so we should update the buttons if the user clicks this to allow them to
            // be manually reset in this case.
            UpdateStartAndCancelButtons();
            UpdateTestSettingsGroupState();
        }

        /// <summary>
        /// Handles the click event for the open results buttons
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void OpenResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.CheckFileExists = true;
            OpenDialog.Multiselect = false;
            OpenDialog.DefaultExt = "xml";
            OpenDialog.Filter = Resources.ResultsFileFilter;
            OpenDialog.Title = Resources.SelectResultsFile;
            OpenDialog.InitialDirectory = m_strResultsDirectory;

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                OpenResultsFile(OpenDialog.FileName);
            }
        }

        /// <summary>
        /// Opens the specified results file.
        /// </summary>
        /// <param name="resultsFilePath">The path to the results file to open</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void OpenResultsFile(string resultsFilePath)
        {
            ResultDockingTabPage ResultsPage = new ResultDockingTabPage();

            try
            {
                if (File.Exists(resultsFilePath))
                {
                    ResultsPage.OpenResultFile(resultsFilePath);
                    ResultsPage.TabIndex = ResultsDockingTab.TabPages.Count + 1;

                    ResultsDockingTab.TabPages.Add(ResultsPage);
                    ResultsDockingTab.SelectedTab = ResultsPage;
                }

                UpdateExportAndPrintButtons();
            }
            catch (Exception e)
            {
                ErrorForm.DisplayError(Resources.ErrorCouldNotOpenResultsFile, e);
            }
        }

        /// <summary>
        /// Handles the program clear button's click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ClearProgramButton_Click(object sender, EventArgs e)
        {
            ProgramFilePath = "";
        }

        /// <summary>
        /// Handles the Help button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void HelpButton_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguredHelp.ShowHelp(this, "Default");
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError(Resources.ErrrorHelpFile, ex);
            }
        }

        /// <summary>
        /// Handles the Help button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void HelpSearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguredHelp.ShowHelpSearch(this);
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError(Resources.ErrrorHelpFile, ex);
            }
        }

        /// <summary>
        /// Handles the Help button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void HelpContentsButton_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguredHelp.ShowHelpTableOfContents(this);
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError(Resources.ErrrorHelpFile, ex);
            }
        }

        /// <summary>
        /// Handles the Help button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void HelpIndexButton_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguredHelp.ShowHelpIndex(this);
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError(Resources.ErrrorHelpFile, ex);
            }
        }

        /// <summary>
        /// Handles the visible changed event for the ResultsDockingTab
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ResultsDockingTab_VisibleChanged(object sender, EventArgs e)
        {
            // This control likes to make itself invisible when all of the tabs have been
            // closed so we need to make sure this is always visible.
            if (ResultsDockingTab.Visible == false)
            {
                //ResultsDockingTab.Visible = true;
            }
        }

        /// <summary>
        /// Handles the Close All button click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void CloseResultsButton_Click(object sender, EventArgs e)
        {
            ResultsDockingTab.TabPages.Clear();
            UpdateExportAndPrintButtons();
        }

        /// <summary>
        /// Handles the MeasureTab event.
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ResultsDockingTab_MeasureTab(object sender, MeasureTabEventArgs e)
        {
            // Normal behavior of the tabs is to make the tab as large as the text
            // some file names can be very long so we need to limit the size of the
            // tab to a reasonable length.
            if (e.Width > 100)
            {
                e.Width = 100;
            }
        }

        /// <summary>
        /// Exports the current view to an excel spreadsheet.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ExportButton_Click(object sender, EventArgs e)
        {
            ResultDockingTabPage CurrentTab = ResultsDockingTab.SelectedTab as ResultDockingTabPage;
            SaveFileDialog SaveFileDialog = new SaveFileDialog();

            SaveFileDialog.CheckPathExists = true;
            SaveFileDialog.AddExtension = true;
            SaveFileDialog.DefaultExt = "xls";
            SaveFileDialog.Filter = Resources.ExcelFileFilter;
            SaveFileDialog.Title = Resources.SelectExportFileLocation;
            SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (CurrentTab != null)
            {
                if (SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    CurrentTab.Export(SaveFileDialog.FileName);
                }
            }

        }

        /// <summary>
        /// Handles the Print Button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void PrintButton_Click(object sender, EventArgs e)
        {
            ResultDockingTabPage CurrentTab = ResultsDockingTab.SelectedTab as ResultDockingTabPage;

            if (CurrentTab != null)
            {
                CurrentTab.Print();
            }
        }

        /// <summary>
        /// Handles the Print Preview button click event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void PrintPreviewButton_Click(object sender, EventArgs e)
        {
            ResultDockingTabPage CurrentTab = ResultsDockingTab.SelectedTab as ResultDockingTabPage;

            if (CurrentTab != null)
            {
                CurrentTab.PrintPreview();
            }
        }

        /// <summary>
        /// Handles the tab page closed event for the results docking tab
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ResultsDockingTab_TabPageClosed(object sender, TabPageEventArgs e)
        {
            UpdateExportAndPrintButtons();
        }

        /// <summary>
        /// Handles the form closing event to prevent the user from closing the application while
        /// a test is running.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void CustomerValidationToolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsTestRunning)
            {
                DialogResult Result = MessageBox.Show("Testing is not complete. Are you sure you want to cancel all tests and close the application?", 
                    "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, 0);

                if (Result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    foreach (ValidationWorker CurrentWorker in m_RunningValidationWorkers)
                    {
                        CurrentWorker.AbortTests();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Drop Down event for the port combo boxes
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void Port_DropDown(object sender, EventArgs e)
        {
            // Update the ports in case a new probe was added
            InitializePorts();
        }

        /// <summary>
        /// Populates the Signed Authorization Settings group
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 RCG 2.30.12        Created

        private void PopulateSignedAuthorization()
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            SignedAuthorizationKey CurrentKey = Settings.Default.AuthenticationKey;

            AuthorizationGroup.Visible = SystemSettings.UseSignedAuthorization;

            if (AuthorizationGroup.Visible)
            {
                if (CurrentKey != null)
                {
                    lblStartDate.Text = CurrentKey.StartDate.ToString("G", CultureInfo.CurrentCulture);
                    lblEndDate.Text = CurrentKey.EndDate.ToString("G", CultureInfo.CurrentCulture);
                    lblLevel.Text = CurrentKey.Level.ToDescription();
                    lblUserName.Text = CurrentKey.UserName;
                    lblUserID.Text = CurrentKey.UserID.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    lblStartDate.Text = "--";
                    lblEndDate.Text = "--";
                    lblLevel.Text = "--";
                    lblUserName.Text = "--";
                    lblUserID.Text = "--";
                }
            }
        }

        /// <summary>
        /// Handles the Click event for the Authorization Key update button
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 RCG 2.30.12        Created

        private void UpdateKeyButton_Click(object sender, EventArgs e)
        {
            try
            {
                SignedAuthenticator.UpdateSignedAuthorizationKey();

                PopulateSignedAuthorization();
            }
            catch (MessageSecurityException ex)
            {
                ErrorForm.DisplayError("Unable to update signed authorization credentials: Invalid Username/Password", ex);
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError("Unable to update signed authorization credentials", ex);
            }
        }

        /// <summary>
        /// Handles the Test Settings Tab select event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/21/09 RCG 2.30.12        Created

        private void TestSettingsTab_Select(object sender, EventArgs e)
        {
            PopulateSignedAuthorization();
        }

        

        /// <summary>
        /// Event handler that handles a fatal error
        /// </summary>
        /// <param name="sender">The control that sent the event</param>
        /// <param name="e">The event arguments</param>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ------------------------------------------
        // 04/08/13 jrf 3.50.68 458134 Created
        private void HandleFatalError(object sender, ItronErrorEventArgs e)
        {
            //Events will come from other threads
            if (InvokeRequired)
            {                
                HandleFatalErrorDelegate HandleFatalErrorMethod = new HandleFatalErrorDelegate(HandleFatalError);
                Invoke(HandleFatalErrorMethod, new object[] { sender, e });
            }
            else
            {
                ErrorForm frmError = new ErrorForm();

                frmError.StartPosition = FormStartPosition.CenterParent;
                frmError.IconSelection = MessageBoxIcon.Error;
                frmError.OriginatingException = e.OriginatingException;

                frmError.Message = e.Message;

                frmError.ShowDialog(this);
            }

        }//HandleFatalError

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets whether or not tests are currently running.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private bool IsTestRunning
        {
            get
            {
                bool bTestsRunning = false;

                if (m_RunningThreads != null)
                {
                    foreach (Thread CurrentThread in m_RunningThreads)
                    {
                        if (CurrentThread != null && CurrentThread.IsAlive)
                        {
                            bTestsRunning = true;
                            break;
                        }
                    }
                }

                return bTestsRunning;
            }
        }

        /// <summary>
        /// Gets the number of test runs that are currently selected.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private int TestRunCount
        {
            get
            {
                int iCount = 0;

                if (cbUseProbe1.Checked)
                {
                    iCount++;
                }

                if (cbUseProbe2.Checked)
                {
                    iCount++;
                }

                if (cbUseProbe3.Checked)
                {
                    iCount++;
                }

                if (cbUseProbe4.Checked)
                {
                    iCount++;
                }

                return iCount;
            }
        }

        /// <summary>
        /// Gets or sets the program file path
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private string ProgramFilePath
        {
            get
            {
                return m_strProgramFilePath;
            }
            set
            {
                m_strProgramFilePath = value;
                
                // Save off the settings for the new program file.
                Settings.Default.ProgramFile = m_strProgramFilePath;
                Settings.Default.Save();

                // Set the file name in the text box.
                if (String.IsNullOrEmpty(m_strProgramFilePath) == false)
                {
                    ProgramEditBox.Text = Path.GetFileNameWithoutExtension(m_strProgramFilePath);
                }
                else
                {
                    ProgramEditBox.Text = "";
                }
            }
        }

        /// <summary>
        /// Gets or sets the results directory file path.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private string ResultsDirPath
        {
            get
            {
                return m_strResultsDirectory;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    m_strResultsDirectory = value;
                }
                else
                {
                    m_strResultsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                // Save off the settings for the new program file.
                Settings.Default.ResultsDir = m_strResultsDirectory;
                Settings.Default.Save();

                // Set the file name in the text box.
                ResultsDirEditBox.Text = m_strResultsDirectory;
            }
        }

        #endregion

        #region Member Variables

        private string m_strProgramFilePath;
        private string m_strResultsDirectory;

        private List<Thread> m_RunningThreads;
        private List<ValidationWorker> m_RunningValidationWorkers;

        #endregion

    }
}
