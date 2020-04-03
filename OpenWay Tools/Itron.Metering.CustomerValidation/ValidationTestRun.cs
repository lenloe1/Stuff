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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Validation Test Identifiers
    /// </summary>
    public enum ValidationTestID : uint
    {
        /// <summary>
        /// Checks the device status
        /// </summary>
        [EnumDescription("Device Status")]
        DeviceStatus = 0,
        /// <summary>
        /// Validates the program
        /// </summary>
        [EnumDescription("Program Validation Test")]
        ValidateProgram = 1,
        /// <summary>
        /// Show the normal display items
        /// </summary>
        [EnumDescription("Normal Display Items")]
        ShowNormalDisplay = 2,
        /// <summary>
        /// Makes sure that ZigBee is functioning properly
        /// </summary>
        [EnumDescription("ZigBee Radio Test")]
        ZigBeeRadioTest = 3,
        /// <summary>
        /// Validates all security keys
        /// </summary>
        [EnumDescription("Security Keys Test")]
        ValidateSecurityKeys = 4,
        /// <summary>
        /// Tests the functionality of the disconnect switch
        /// </summary>
        [EnumDescription("Remote Disconnect Test")]
        ConnectDisconnect = 5,
        /// <summary>
        /// Clears the billing registers
        /// </summary>
        [EnumDescription("Clear Billing Test")]
        ClearBilling = 6,
        /// <summary>
        /// Clears the activity status
        /// </summary>
        [EnumDescription("Clear Activity Test")]
        ClearActivityStatus = 7,
    }

    /// <summary>
    /// Class used to read and write the file describing the tests included in a test run
    /// </summary>
    public class ValidationTestRun
    {
        #region Constants

        private const string ROOT_NAME = "ValidationTestRun";
        private const string ELEM_TEST = "ValidationTest";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFilePath">The path to the test run file.</param>
        /// <summary>
        /// Loads the Test Run file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/16/09 RCG 2.30.00        Created

        public ValidationTestRun(string strFilePath)
        {
            m_strFilePath = strFilePath;
            m_ValidationIDs = new List<ValidationTestID>();

            Load();
        }

        /// <summary>
        /// Saves the Test Run file
        /// </summary>
        /// <summary>
        /// Loads the Test Run file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/16/09 RCG 2.30.00        Created

        public void Save()
        {
            XElement Root = new XElement(ROOT_NAME);

            foreach (ValidationTestID CurrentTest in m_ValidationIDs)
            {
                Root.Add(new XElement(ELEM_TEST, (uint)CurrentTest));
            }

            Root.Save(m_strFilePath);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of validation test IDs
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/16/09 RCG 2.30.00        Created

        public List<ValidationTestID> SelectedTestIDs
        {
            get
            {
                return m_ValidationIDs;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the Test Run file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/16/09 RCG 2.30.00        Created

        private void Load()
        {
            XDocument LoadedDocument = XDocument.Load(m_strFilePath);
            XElement Root = LoadedDocument.Root;

            m_ValidationIDs.Clear();

            if (Root != null && Root.Name.LocalName == ROOT_NAME)
            {
                foreach (XElement CurrentElement in Root.Descendants(ELEM_TEST))
                {
                    m_ValidationIDs.Add((ValidationTestID)uint.Parse(CurrentElement.Value, CultureInfo.CurrentCulture));
                }
            }
        }

        #endregion

        #region Member Variables

        private string m_strFilePath;
        private List<ValidationTestID> m_ValidationIDs;

        #endregion
    }
}
