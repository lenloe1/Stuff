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
//                        Copyright © 2008 - 2009
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Base class for all CRF Data Types
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/30/09 AF  2.20.10 136279 Changed "CIM" to "CRF"
    //
    public abstract class CRFDataType
    {
        #region Public Methods
        
        /// <summary>
        /// Base Constructor
        /// </summary>
        public CRFDataType()
        {

        }

        /// <summary>
        /// Base class method for wrting the XML file
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument</param>
        /// <param name="ESN">Electronic Serial Number</param>
        public virtual CreateCRFResult Write(XmlDocument xmlDoc, string ESN)
        {
            m_xmlDoc = xmlDoc;
            m_ESN = ESN;

            return m_Result;
        }

        #endregion

        
        #region Protected Methods

        /// <summary>
        /// Modifies the input string to produce a name recognized by the Collection Engine
        /// </summary>
        /// <param name="Description">The description that Field-Pro would show</param>
        /// <returns>A string that the IEE system will recognize as a valid register</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/25/09 AF  2.30.04        Created
        //  10/06/09 AF  2.30.07        Added comments suggested in code review
        //  03/22/12 jrf 2.53.51 171615 Changing "var" to "VAR"
        //  03/27/12 jrf 2.53.52 TREQ2891/3440/3447 Moved method here from derived class so 
        //                              it can be used in both the CRFRegDataType and CRFLPDataType 
        //                              classes.  Fixed to remove "Arith" keyword from anywhere 
        //                              in description.  Added removal of "Vec" keyword from
        //                              anywhere in description.  Modified to remove space before 
        //                              any left parenthesis instead of just for "Vh (".
        //                              Added uncapitalization of "Ins" in description.
        //
        protected string ModifyDescription(string Description)
        {
            string strModifiedDescription = Description;

            // Some items are shown differently in Field-Pro than the Collection Engine.
            // We will modify those strings to match the Collection Engine items
            if (strModifiedDescription.Contains("Arith"))
            {
                // Removes "Arith" from quantity description
                // For example, "VAh Arith d" becomes "VAh d" 
                strModifiedDescription = strModifiedDescription.Replace(" Arith", "");
            }
            else if (strModifiedDescription.Contains("Vec"))
            {
                // Removes "Vec" from quantity description
                // For example, "VAh Vec d" becomes "VAh d"
                strModifiedDescription = strModifiedDescription.Replace(" Vec", "");
            }
            else if (strModifiedDescription.Contains("var"))
            {
                // Make "var" always all caps.
                strModifiedDescription = strModifiedDescription.Replace("var", "VAR");
            }

            if (strModifiedDescription.Contains(" ("))
            {
                // Remove space before left parenthesis in string, ex. "Vh (a)" goes to "Vh(a)".
                strModifiedDescription = strModifiedDescription.Replace(" (", "(");
            }

            if (strModifiedDescription.Contains("Ins "))
            {
                // Make "Ins" all lower case.
                strModifiedDescription = strModifiedDescription.Replace("Ins ", "ins ");
            }

            return strModifiedDescription;
        }

        #endregion


        #region Members

        /// <summary>
        /// Storage of the XmlDocument for the CRFDataType Objects.
        /// </summary>
        protected XmlDocument m_xmlDoc = null;
        /// <summary>
        /// Storage of the Electronic Serial Number
        /// </summary>
        protected string m_ESN = "";
        /// <summary>
        /// Result Code
        /// </summary>
        protected CreateCRFResult m_Result = CreateCRFResult.FILE_CREATION_ERROR;

        #endregion
    }
}
