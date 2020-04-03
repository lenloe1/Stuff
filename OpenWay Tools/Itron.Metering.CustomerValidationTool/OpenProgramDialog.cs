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
using System.IO;
using C1.Win.C1Ribbon;
using C1.Win.C1FlexGrid;
using Itron.Metering.Datafiles;
using Itron.Metering.ReplicaSettings;
using Itron.Metering.DataCollections;
using Itron.Metering.Utilities;
using Itron.Metering.SharedControls;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Open Program dialog for the customer validation tool
    /// </summary>
    public partial class OpenProgramDialog : C1RibbonForm
    {
        #region Constants

        private const string REPLICA = "OpenWay Replica";
        private const string PROGRAM_FOLDER = "Programs";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public OpenProgramDialog()
        {
            InitializeComponent();

            m_strProgramDirectory = CRegistryHelper.GetFilePath(REPLICA) + PROGRAM_FOLDER;

            PopulatePrograms();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the file that was selected.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public string SelectedFile
        {
            get
            {
                string strSelectedFilePath = null;
                EDLFile SelectedFile = null;
                int SelectedRow = ProgramFlexGrid.RowSel;

                if (SelectedRow > 0 && SelectedRow < ProgramFlexGrid.Rows.Count 
                    && ProgramFlexGrid.Rows[SelectedRow] != null)
                {
                    SelectedFile = ProgramFlexGrid.Rows[SelectedRow].UserData as EDLFile;

                    if (SelectedFile != null)
                    {
                        strSelectedFilePath = SelectedFile.FileName;
                    }
                }

                return strSelectedFilePath;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populates the list of programs.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void PopulatePrograms()
        {
            EDLFileCollection ProgramCollection = new EDLFileCollection(m_strProgramDirectory);

            foreach (EDLFile CurrentFile in ProgramCollection)
            {
                AddFile(CurrentFile);
            }
        }

        /// <summary>
        /// Adds a program file to the Flexgrid control
        /// </summary>
        /// <param name="programFile">The EDL file to add.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void AddFile(EDLFile programFile)
        {
            string strProgramName;
            string strProgramVersion;
            FileInfo Info;
            XMLOpenWayActiveFiles ActiveFiles = new XMLOpenWayActiveFiles();

            if (programFile != null)
            {
                Row NewRow = ProgramFlexGrid.Rows.Add();

                EDLFile.ParseFileName(programFile.FileName, out strProgramName, out strProgramVersion);
                Info = new FileInfo(programFile.FileName);

                NewRow.UserData = programFile;
                NewRow["Program Name"] = strProgramName;
                NewRow["Version"] = strProgramVersion;
                NewRow["Active"] = ActiveFiles.ActivePrograms.Contains(Info.Name);
                NewRow["Device Class"] = programFile.DeviceClassHumanReadable;
                NewRow["Last Modified"] = Info.LastWriteTime;
            }
        }

        /// <summary>
        /// Handles the selection changed event for the Flex Grid control.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void ProgramFlexGrid_AfterSelChange(object sender, RangeEventArgs e)
        {
            btnOK.Enabled = ProgramFlexGrid.RowSel > 0 && ProgramFlexGrid.RowSel < ProgramFlexGrid.Rows.Count;
        }

        /// <summary>
        /// Handles the help button click event.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguredHelp.ShowHelp(this, "Open Program");
            }
            catch (Exception ex)
            {
                ErrorForm.DisplayError(Properties.Resources.ErrrorHelpFile, ex);
            }
        }

        #endregion

        #region Member Variables

        private string m_strProgramDirectory;

        #endregion
    }
}
