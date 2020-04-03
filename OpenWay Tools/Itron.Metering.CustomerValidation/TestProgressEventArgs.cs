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
using Itron.Metering.Progressable;

namespace Itron.Metering.CustomerValidation
{
    /// <summary>
    /// The result of a test step.
    /// </summary>
    public enum TestStepResult
    {
        /// <summary>
        /// There is no result for this step
        /// </summary>
        None = 0,
        /// <summary>
        /// The test step passed.
        /// </summary>
        Passed = 1,
        /// <summary>
        /// The test step failed.
        /// </summary>
        Failed = 2,
        /// <summary>
        /// The test step was skipped.
        /// </summary>
        Skipped = 3,
    }

    /// <summary>
    /// Event arguments for a test step progress event.
    /// </summary>
    public class TestStepEventArgs : ProgressEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestStepEventArgs()
            : this(null, null, TestStepResult.None)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strStatus">The current progress status</param>
        /// <param name="strTitle">The title of the progress status</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestStepEventArgs(string strStatus, string strTitle)
            : this(strStatus, strTitle, TestStepResult.None)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strStatus">The current progress status</param>
        /// <param name="strTitle">The title of the progress status</param>
        /// <param name="result">The result of the test step.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestStepEventArgs(string strStatus, string strTitle, TestStepResult result)
            : base(strStatus)
        {
            m_Result = result;
            m_strTitle = strTitle;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the result of the step.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestStepResult StepResult
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
                return m_strTitle;
            }
            set
            {
                m_strTitle = value;
            }
        }

        #endregion

        #region Member Variables

        private TestStepResult m_Result;
        private string m_strTitle;

        #endregion
    }

    /// <summary>
    /// Event arguments for test completion.
    /// </summary>
    public class TestCompleteEventArgs : ProgressEventArgs
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestCompleteEventArgs()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strStatus">The current progress status</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestCompleteEventArgs(string strStatus)
            : this(strStatus, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strStatus">The current progress status</param>
        /// <param name="strResultFile">The path to the results file.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public TestCompleteEventArgs(string strStatus, string strResultFile)
            : base(strStatus)
        {
            m_strResultFile = strResultFile;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the path to the results file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/28/09 RCG 2.30.00        Created

        public string ResultsFilePath
        {
            get
            {
                return m_strResultFile;
            }
            set
            {
                m_strResultFile = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_strResultFile;

        #endregion

    }
}
