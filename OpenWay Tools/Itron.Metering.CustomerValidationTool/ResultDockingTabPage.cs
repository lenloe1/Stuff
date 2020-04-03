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
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using C1.Win.C1Command;
using C1.Win.C1FlexGrid;
using Itron.Metering.CustomerValidationTool.Properties;
using Itron.Metering.CustomerValidation;
using Itron.Metering.Device;
using Itron.Metering.Utilities;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Docking tab page control used for displaying Customer Validation result files.
    /// </summary>
    public partial class ResultDockingTabPage : C1DockingTabPage
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Allowing flex grid rows to merge.
        public ResultDockingTabPage()
            : base()
        {
            InitializeComponent();

            m_strOpenFilePath = null;

            SetupCellStyles();

            for (int i = 0; i < ResultsFlexGrid.Rows.Count; i++)
            {
                //Need to make sure all preexisting rows allow merging.
                ResultsFlexGrid.Rows[i].AllowMerging = true;
            }
        }

        /// <summary>
        /// Opens populates the tab with the results contained in the specified file.
        /// </summary>
        /// <param name="strResultFile">The file containing the results to open.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way results and result nodes are set.
        //  09/19/14 jrf 4.00.63 WR 534159 Adding the word results after each test name when displaying
        //                                 the results of each test.
        public void OpenResultFile(string strResultFile)
        {
            FileInfo CurrentFileInfo = new FileInfo(strResultFile);
            TestResults CurrentTestResults = new TestResults();

            m_strOpenFilePath = strResultFile;
            Text = CurrentFileInfo.Name;

            CurrentTestResults.Load(m_strOpenFilePath);

            if (CurrentTestResults.TestRuns != null && CurrentTestResults.TestRuns.Count > 0)
            {
                TestRun CurrentTestRun = CurrentTestResults.TestRuns[0];

                m_MeterID = CurrentTestRun.MeterID;
                m_TestDate = CurrentTestRun.TestDate;

                AddResultNode(Resources.TestInformation, "", "");
                AddResult(Resources.ElectronicSerialNumber, Resources.OK, m_MeterID, "", "");
                AddResult(Resources.TestDate, Resources.OK, m_TestDate.ToString("G", CultureInfo.CurrentCulture), "", "");
                AddResult(Resources.ProgramUsed, Resources.OK, Path.GetFileNameWithoutExtension(CurrentTestRun.ProgramName), "", "");
                AddResult(Resources.DeviceType, Resources.OK, CurrentTestRun.MeterType, "", "");
                AddResult(Resources.SoftwareVersion, Resources.OK, CurrentTestRun.SWVersion, "", "");

                // Add each of the tests that are contained in the file
                foreach(Test CurrentTest in CurrentTestRun.Tests)
                {
                    // Add the node for the test
                    AddResultNode(CurrentTest.Name + " " + Resources.Results, CurrentTest.Result, CurrentTest.Reason);

                    // Add each of the test details
                    foreach(TestDetail CurrentTestDetail in CurrentTest.TestDetails)
                    {
                        AddResult(CurrentTestDetail.Name, 
                            CurrentTestDetail.Result,
                            CurrentTestDetail.Details,
                            CurrentTestDetail.AdditionalDetails,
                            CurrentTestDetail.Reason);
                    }
                }
            }
        }

        /// <summary>
        /// Exports the current view
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void Export(string strExportFile)
        {
            ResultsFlexGrid.SaveExcel(strExportFile, m_MeterID, 
                FileFlags.IncludeFixedCells | FileFlags.AsDisplayed | FileFlags.SaveMergedRanges);
        }

        /// <summary>
        /// Prints the current view
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void Print()
        {
            ResultsFlexGrid.PrintGrid(Path.GetFileName(m_strOpenFilePath),
                PrintGridFlags.ShowPrintDialog | PrintGridFlags.FitToPageWidth | PrintGridFlags.ExtendLastCol,
                m_MeterID + Resources.ValidationResults,
                Resources.Tested + m_TestDate.ToString("G", CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Performs a Print Preview of the current view.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public void PrintPreview()
        {
            ResultsFlexGrid.PrintGrid(Path.GetFileName(m_strOpenFilePath),
                PrintGridFlags.ShowPreviewDialog | PrintGridFlags.FitToPageWidth | PrintGridFlags.ExtendLastCol, 
                m_MeterID + Resources.ValidationResults, 
                Resources.Tested + m_TestDate.ToString("G", CultureInfo.CurrentCulture));           
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the full path of the file that is currently being displayed.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public string FileName
        {
            get
            {
                return m_strOpenFilePath;
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Sets up the various cell styles for the grid.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created
        //  10/03/14 jrf 4.00.66 534160 Making a separate checked style. 
        private void SetupCellStyles()
        {
            m_ResultPassedStyle = ResultsFlexGrid.Styles.Add("ResultPassed", ResultsFlexGrid.Styles.Normal);
            m_ResultPassedStyle.ForeColor = Color.Green;

            m_ResultCheckedStyle = ResultsFlexGrid.Styles.Add("ResultChecked", ResultsFlexGrid.Styles.Normal);
            m_ResultCheckedStyle.ForeColor = Color.Green;
            m_ResultCheckedStyle.Font = new System.Drawing.Font(m_ResultCheckedStyle.Font.FontFamily, 
                                                                m_ResultCheckedStyle.Font.Size, 
                                                                FontStyle.Bold);

            m_ResultFailedStyle = ResultsFlexGrid.Styles.Add("ResultFailed", ResultsFlexGrid.Styles.Normal);
            m_ResultFailedStyle.ForeColor = Color.Red;

            m_ResultSkippedStyle = ResultsFlexGrid.Styles.Add("ResultSkipped", ResultsFlexGrid.Styles.Normal);
            m_ResultSkippedStyle.ForeColor = Color.DarkGray;
        }

        /// <summary>
        /// Adds a result node to the flex grid control
        /// </summary>
        /// <param name="strName">The name of the node to add.</param>
        /// <param name="strResult">The result of the test.</param>
        /// <param name="strReason">The result reason.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way result nodes are set to allow 
        //                                 the two details columns to merge by setting
        //                                 them to the same value.
        private void AddResultNode(string strName, string strResult, string strReason)
        {
            Row NewRow = ResultsFlexGrid.Rows.Add();
            NewRow.IsNode = true;
            NewRow.Node.Level = 0;
            NewRow.StyleNew.BackColor = Color.FromArgb(220, 220, 220);
            
            //am empty string won't allow the two details columns to merge if they are the same.
            if (string.IsNullOrEmpty(strReason))
            {
                strReason = " ";
            }

            NewRow["Item"] = strName;
            NewRow["Result"] = strResult;
            NewRow["Details"] = strReason;
            NewRow["Details2"] = strReason;
            NewRow.UserData = strReason;
            NewRow.AllowMerging = true;

            SetResultStyle(NewRow);
        }

        /// <summary>
        /// Sets the style of the results cell based on the value that it contains.
        /// </summary>
        /// <param name="NewRow">The row to set.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534160 Setting cell style appropriately for new results.
        //  10/03/14 jrf 4.00.66 WR 534160 Using a separate style for displaying a check mark. 
        private void SetResultStyle(Row NewRow)
        {
            int iResultColIndex = ResultsFlexGrid.Cols["Result"].Index;
            string strResult = NewRow["Result"] as string;

            if (strResult != null)
            {
                if (strResult.Equals(Resources.NoErrorsFound))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultPassedStyle);
                }
                else if (strResult.Equals(Resources.OK))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultCheckedStyle);
                }
                else if (strResult.Equals(Resources.Passed))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultPassedStyle);
                }
                else if (strResult.Equals(Resources.Failed))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultFailedStyle);
                }
                else if (strResult.Equals(Resources.Error)
                    || (strResult.Equals(Resources.ErrorsFound)))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultFailedStyle);
                }
                else if (strResult.Equals(Resources.Skipped))
                {
                    ResultsFlexGrid.SetCellStyle(NewRow.Index, iResultColIndex, m_ResultSkippedStyle);
                }
            }
        }

        /// <summary>
        /// Adds the result to the flex grid control
        /// </summary>
        /// <param name="name">The name of the item to add</param>
        /// <param name="result">The result</param>
        /// <param name="details">The details of the result.</param>
        /// <param name="additionalDetails">The additional details of the result.</param>
        /// <param name="reason">The reason for the result</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Modified way result rows are set to allow 
        //                                 the two details columns to merge if an additional
        //                                 details column is not needed.
        private void AddResult(string name, string result, string details, string additionalDetails, string reason)
        {
            Row NewRow = ResultsFlexGrid.Rows.Add();
            NewRow.IsNode = true;
            NewRow.Node.Level = 1;

            //am empty string won't allow the two details columns to merge if they are the same.
            if (string.IsNullOrEmpty(details))
            {
                details = " ";
            }

            //am empty string won't allow the two details columns to merge if they are the same.
            if (string.IsNullOrEmpty(additionalDetails))
            {
                additionalDetails = " ";
            }

            NewRow["Item"] = name;
            NewRow["Result"] = result;
            NewRow["Details"] = details;

            if (string.IsNullOrWhiteSpace(additionalDetails))
            {
                //if nothing there, then set details to second column so cells will be merged.
                NewRow["Details2"] = details;
            }
            else
            {
                NewRow["Details2"] = additionalDetails;
            }

            NewRow.UserData = reason;
            NewRow.AllowMerging = true;

            SetResultStyle(NewRow);
        }

        /// <summary>
        /// Clears the results from the flex grid.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ClearResults()
        {
            ResultsFlexGrid.Rows.RemoveRange(1, ResultsFlexGrid.Rows.Count - 1);
        }

        /// <summary>
        /// Shows the result reason as a tooltip
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ResultsFlexGrid_MouseEnterCell(object sender, RowColEventArgs e)
        {
            if (ResultsFlexGrid.Cols["Result"].Index == e.Col)
            {
                ReasonTooltip.SetToolTip(ResultsFlexGrid, ResultsFlexGrid.Rows[e.Row].UserData as string);
            }
            else
            {
                ReasonTooltip.SetToolTip(ResultsFlexGrid, null);
            }
        }

        #endregion

        #region Member Variables

        private string m_strOpenFilePath;
        private CellStyle m_ResultPassedStyle;
        private CellStyle m_ResultCheckedStyle;
        private CellStyle m_ResultFailedStyle;
        private CellStyle m_ResultSkippedStyle;
        private DateTime m_TestDate;
        private string m_MeterID;

        #endregion
    }
}
