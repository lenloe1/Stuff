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
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// Class used for reading a writing test results files
    /// </summary>
    public class TestResults
    {
        #region Constants

        internal const string ELEMENT_NAME = "TestResults";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public TestResults()
        {
            m_strFileName = null;
            m_TestRuns = new List<TestRun>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public void Load(string strFileName)
        {
            m_strFileName = strFileName;
            XDocument NewDocument = XDocument.Load(strFileName);

            m_TestRuns.Clear();

            if (NewDocument.Root != null && NewDocument.Root.Name.LocalName == ELEMENT_NAME)
            {
                m_TestRuns = new List<TestRun>();

                foreach (XElement TestRunElement in NewDocument.Descendants(TestRun.ELEMENT_NAME))
                {
                    m_TestRuns.Add(new TestRun(TestRunElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Test Results file", "strFileName");
            }
        }

        /// <summary>
        /// Saves the result file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public void Save()
        {
            XElement NewElement = new XElement(ELEMENT_NAME);

            foreach (TestRun CurrentTestRun in TestRuns)
            {
                NewElement.Add(CurrentTestRun.XElement);
            }

            NewElement.Save(m_strFileName);
        }

        /// <summary>
        /// Saves the results file to the specified location.
        /// </summary>
        /// <param name="strFilePath">The new file to save to.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public void SaveAs(string strFilePath)
        {
            m_strFileName = strFilePath;
            Save();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of test runs in the file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public List<TestRun> TestRuns
        {
            get
            {
                return m_TestRuns;
            }
        }

        #endregion

        #region Member Variables

        private string m_strFileName;
        private List<TestRun> m_TestRuns;

        #endregion
    }

    /// <summary>
    /// Class used for reading and writing a test run
    /// </summary>
    public class TestRun
    {
        #region Constants

        internal const string ELEMENT_NAME = "TestRun";

        private const string ATTRIB_MFGSERIAL = "MeterID";
        private const string ATTRIB_TEST_DATE = "TestDate";
        private const string ATTRIB_METER_TYPE = "MeterType";
        private const string ATTRIB_PROGRAM = "Program";
        private const string ATTRIB_VERSION = "Version";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/30/14 jrf 4.00.63 WR 534158 Storing meter type rather than device class.
        public TestRun()
        {
            m_strMeterID = "";
            m_TestDate = DateTime.MinValue;
            m_MeterType = "";
            m_strProgramName = "";
            m_strSWVersion = "";
            m_Tests = new List<Test>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testRunElement">The XElement representing the test run</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/30/14 jrf 4.00.63 WR 534158 Storing meter type rather than device class.
        public TestRun(XElement testRunElement)
        {
            if (testRunElement != null && testRunElement.Name.LocalName == ELEMENT_NAME)
            {
                if (testRunElement.Attribute(ATTRIB_MFGSERIAL) != null)
                {
                    m_strMeterID = testRunElement.Attribute(ATTRIB_MFGSERIAL).Value;
                }

                if (testRunElement.Attribute(ATTRIB_TEST_DATE) != null)
                {
                    m_TestDate = DateTime.Parse(testRunElement.Attribute(ATTRIB_TEST_DATE).Value, CultureInfo.CurrentCulture);
                }

                if (testRunElement.Attribute(ATTRIB_METER_TYPE) != null)
                {
                    m_MeterType = testRunElement.Attribute(ATTRIB_METER_TYPE).Value;
                }

                if (testRunElement.Attribute(ATTRIB_PROGRAM) != null)
                {
                    m_strProgramName = testRunElement.Attribute(ATTRIB_PROGRAM).Value;
                }

                if (testRunElement.Attribute(ATTRIB_VERSION) != null)
                {
                    m_strSWVersion = testRunElement.Attribute(ATTRIB_VERSION).Value;
                }

                m_Tests = new List<Test>();

                // Add the test details
                foreach (XElement TestElement in testRunElement.Descendants(Test.ELEMENT_NAME))
                {
                    m_Tests.Add(new Test(TestElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid TestRun element", "testElement");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Manufacturer Serial Number
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string MeterID
        {
            get
            {
                return m_strMeterID;
            }
            set
            {
                m_strMeterID = value;
            }
        }

        /// <summary>
        /// Gets or sets the date of the test.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public DateTime TestDate
        {
            get
            {
                return m_TestDate;
            }
            set
            {
                m_TestDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the meter tested
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/30/14 jrf 4.00.63 WR 534158 Created.
        public string MeterType
        {
            get
            {
                return m_MeterType;
            }
            set
            {
                m_MeterType = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the program used to test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string ProgramName
        {
            get
            {
                return m_strProgramName;
            }
            set
            {
                m_strProgramName = value;
            }
        }

        /// <summary>
        /// Gets or sets the software version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string SWVersion
        {
            get
            {
                return m_strSWVersion;
            }
            set
            {
                m_strSWVersion = value;
            }
        }

        /// <summary>
        /// Gets the list of tests that were run
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public List<Test> Tests
        {
            get
            {
                return m_Tests;
            }
        }

        /// <summary>
        /// Gets the TestRun as a XElement node.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/30/14 jrf 4.00.63 WR 534158 Storing meter type rather than device class.
        public XElement XElement
        {
            get
            {
                XElement NewElement = new XElement(ELEMENT_NAME);

                if (String.IsNullOrEmpty(m_strMeterID) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_MFGSERIAL, m_strMeterID));
                }

                NewElement.Add(new XAttribute(ATTRIB_TEST_DATE, m_TestDate));

                if (String.IsNullOrEmpty(m_MeterType) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_METER_TYPE, m_MeterType));
                }

                if (String.IsNullOrEmpty(m_strProgramName) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_PROGRAM, m_strProgramName));
                }

                if (String.IsNullOrEmpty(m_strSWVersion) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_VERSION, m_strSWVersion));
                }


                // Add each of the Tests
                foreach (Test CurrentTest in m_Tests)
                {
                    NewElement.Add(CurrentTest.XElement);
                }

                return NewElement;
            }
        }

        #endregion

        #region Member Variables

        private string m_strMeterID;
        private DateTime m_TestDate;
        private string m_MeterType;
        private string m_strProgramName;
        private string m_strSWVersion;
        private List<Test> m_Tests;

        #endregion
    }

    /// <summary>
    /// Class used for reading and writing a test node
    /// </summary>
    public class Test
    {
        #region Constants

        internal const string ELEMENT_NAME = "Test";

        private const string ATTRIB_NAME = "Name";
        private const string ATTRIB_RESULT = "Result";
        private const string ATTRIB_REASON = "Reason";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public Test()
        {
            m_strName = "";
            m_strResult = "";
            m_TestDetails = new List<TestDetail>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="testElement">The XElement node containing the test</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public Test(XElement testElement)
        {
            if (testElement != null && testElement.Name.LocalName == ELEMENT_NAME)
            {
                if (testElement.Attribute(ATTRIB_NAME) != null)
                {
                    m_strName = testElement.Attribute(ATTRIB_NAME).Value;
                }

                if (testElement.Attribute(ATTRIB_RESULT) != null)
                {
                    m_strResult = testElement.Attribute(ATTRIB_RESULT).Value;
                }

                if (testElement.Attribute(ATTRIB_REASON) != null)
                {
                    m_strReason = testElement.Attribute(ATTRIB_REASON).Value;
                }

                m_TestDetails = new List<TestDetail>();

                // Add the test details
                foreach(XElement TestDetailElement in testElement.Descendants(TestDetail.ELEMENT_NAME))
                {
                    m_TestDetails.Add(new TestDetail(TestDetailElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Test element", "testElement");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }

        /// <summary>
        /// Gets or sets the result of the test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Result
        {
            get
            {
                return m_strResult;
            }
            set
            {
                m_strResult = value;
            }
        }

        /// <summary>
        /// Gets or sets the reason for the result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Reason
        {
            get
            {
                return m_strReason;
            }
            set
            {
                m_strReason = value;
            }
        }

        /// <summary>
        /// Gets the list of test details for the test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public List<TestDetail> TestDetails
        {
            get
            {
                return m_TestDetails;
            }
        }

        /// <summary>
        /// Gets the XElement representation for the test node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public XElement XElement
        {
            get
            {
                XElement NewElement = new XElement(ELEMENT_NAME);

                if (String.IsNullOrEmpty(m_strName) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_NAME, m_strName));
                }

                if (String.IsNullOrEmpty(m_strResult) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_RESULT, m_strResult));
                }

                if (String.IsNullOrEmpty(m_strReason) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_REASON, m_strReason));
                }

                foreach (TestDetail CurrentDetail in m_TestDetails)
                {
                    NewElement.Add(CurrentDetail.XElement);
                }

                return NewElement;
            }
        }

        #endregion

        #region Member Variables

        private string m_strName;
        private string m_strResult;
        private string m_strReason;
        private List<TestDetail> m_TestDetails;

        #endregion
    }

    /// <summary>
    /// Class used for reading and writing a test detail node
    /// </summary>
    public class TestDetail
    {
        #region Constants

        internal const string ELEMENT_NAME = "TestDetail";

        private const string ATTRIB_NAME = "Name";
        private const string ATTRIB_DETAIL = "Detail";
        private const string ATTRIB_DETAIL2 = "Detail2";
        private const string ATTRIB_RESULT = "Result";
        private const string ATTRIB_REASON = "Reason";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                  value elements.
        public TestDetail()
        {
            m_Name = "";
            m_Result = "";
            m_Details = "";
            m_AdditionalDetails = "";
            m_Reason = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the detail item.</param>
        /// <param name="result">The result of the test detail</param>
        /// <param name="details">The expected value</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                  value elements.
        public TestDetail(string name, string result, string details)
            : this(name, result, details, "")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the detail item.</param>
        /// <param name="result">The result of the test detail</param>
        /// <param name="details">The expected value</param>
        /// <param name="reason">The reason for the result</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                  value elements.
        public TestDetail(string name, string result, string details, string reason)
            : this(name, result, details, reason, "")
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the detail item.</param>
        /// <param name="result">The result of the test detail</param>
        /// <param name="details">The details for the result.</param>
        /// <param name="reason">The reason for the result.</param>
        /// <param name="additionalDetails">The additional details for the result.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                  value elements.
        public TestDetail(string name, string result, string details, string reason, string additionalDetails)
        {
            m_Name = name;
            m_Result = result;
            m_Details = details;
            m_Reason = reason;
            m_AdditionalDetails = additionalDetails;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="testDetailElement">The xml element to create it from.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                 value elements.
        public TestDetail(XElement testDetailElement)
            : this()
        {
            if (testDetailElement != null && testDetailElement.Name.LocalName == ELEMENT_NAME)
            {
                if (testDetailElement.Attribute(ATTRIB_NAME) != null)
                {
                    m_Name = testDetailElement.Attribute(ATTRIB_NAME).Value;
                }

                if (testDetailElement.Attribute(ATTRIB_RESULT) != null)
                {
                    m_Result = testDetailElement.Attribute(ATTRIB_RESULT).Value;
                }

                if (testDetailElement.Attribute(ATTRIB_DETAIL) != null)
                {
                    m_Details = testDetailElement.Attribute(ATTRIB_DETAIL).Value;
                }

                if (testDetailElement.Attribute(ATTRIB_DETAIL2) != null)
                {
                    m_AdditionalDetails = testDetailElement.Attribute(ATTRIB_DETAIL2).Value;
                }

                if (testDetailElement.Attribute(ATTRIB_REASON) != null)
                {
                    m_Reason = testDetailElement.Attribute(ATTRIB_REASON).Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid TestDetail element", "testDetailElement");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the test detail
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the details value of the test detail
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/19/14 jrf 4.00.63 WR 534158 Created
        public string Details
        {
            get
            {
                return m_Details;
            }
            set
            {
                m_Details = value;
            }
        }

        /// <summary>
        /// Gets or sets the additional details value of the test detail
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/24/14 jrf 4.00.63 WR 534158 Created
        public string AdditionalDetails
        {
            get
            {
                return m_AdditionalDetails;
            }
            set
            {
                m_AdditionalDetails = value;
            }
        }

        /// <summary>
        /// Gets or sets the result of the test
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Result
        {
            get
            {
                return m_Result;
            }
            set
            {
                m_Result = value;
            }
        }

        /// <summary>
        /// Gets or sets the reason for the result
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00        Created

        public string Reason
        {
            get
            {
                return m_Reason;
            }
            set
            {
                m_Reason = value;
            }
        }

        /// <summary>
        /// Gets the XElement representation of the xml node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  09/14/09 RCG 2.30.00           Created
        //  09/19/14 jrf 4.00.63 WR 534158 Added new detail element and removed meter value/expected
        //                                 value elements.
        public XElement XElement
        {
            get
            {
                XElement NewElement = new XElement(ELEMENT_NAME);

                if (String.IsNullOrEmpty(m_Name) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_NAME, m_Name));
                }

                if (String.IsNullOrEmpty(m_Result) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_RESULT, m_Result));
                }

                if (String.IsNullOrEmpty(m_Details) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_DETAIL, m_Details));
                }

                if (String.IsNullOrEmpty(m_AdditionalDetails) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_DETAIL2, m_AdditionalDetails));
                }

                if (String.IsNullOrEmpty(m_Reason) == false)
                {
                    NewElement.Add(new XAttribute(ATTRIB_REASON, m_Reason));
                }

                return NewElement;
            }
        }

        #endregion

        #region Member Variables

        private string m_Name;
        private string m_Details;
        private string m_AdditionalDetails;
        private string m_Result;
        private string m_Reason;

        #endregion
    }
}
