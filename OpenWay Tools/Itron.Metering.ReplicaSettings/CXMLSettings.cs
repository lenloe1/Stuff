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
//                              Copyright © 2004 - 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Base class for XML Shared
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///08/03/04 REM 7.00.11 N/A    Initial Release
	///08/22/05 REM 7.20.09 1625: Battery Carryover should default to Enable in System Manager
	/// </pre></remarks>
	public class CXMLSettings : System.Xml.XmlDocument
	{
        #region Constants
		/// <summary>
		/// CXMLSettings protected readonly string
		/// </summary>
		protected readonly string DEFAULT_XML_FILE;
		/// <summary>
		/// protected readonly string DEFAULT_XML_ROOT_NODE = "PCPRO98";
		/// </summary>
		protected readonly string DEFAULT_XML_ROOT_NODE = "PCPRO98";
		/// <summary>
		/// CXMLSettings protected readonly string
		/// </summary>
		protected readonly string DEFAULT_XML_MAIN_NODE;
		/// <summary>
		/// protected const string XML_VALUE_BOOL_TRUE = "1";
		/// </summary>
		protected const string XML_VALUE_BOOL_TRUE = "1";
		/// <summary>
		/// protected const string XML_VALUE_BOOL_FALSE = "0";
		/// </summary>
		protected const string XML_VALUE_BOOL_FALSE = "0";
		/// <summary>
		/// protected const string XML_NODE_VALUE = "Value";
		/// </summary>
		protected const string XML_NODE_VALUE = "Value";
		/// <summary>
		/// protected const int DEFAULT_INT = 0;
		/// </summary>
		protected const int DEFAULT_INT = 0;
		/// <summary>
		/// protected const float DEFAULT_FLOAT = 0.0F;
		/// </summary>
		protected const float DEFAULT_FLOAT = 0.0F;
		/// <summary>
		/// protected const string DEFAULT_STRING = "";
		/// </summary>
		protected const string DEFAULT_STRING = "";
		/// <summary>
		/// Stores the current file name to be used for getting and setting data in XML files.
		/// </summary>
		protected string m_strXMLFileName;
		/// <summary>
		/// CXMLSettings protected member variable
		/// </summary>
		protected XmlNode m_xmlnodeCurrent;
		/// <summary>
		/// CXMLSettings protected member variable
		/// </summary>
		protected XmlNode m_xmlnodeAnchor;
		/// <summary>
		/// CXMLSettings protected member variable
		/// </summary>
		protected XmlNode m_xmlnodeRoot;
        #endregion

        #region Public Methods

        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="strFile">Default File Name for .xml file</param>
		/// <param name="strRoot">Root Node in the XML file</param>
		/// <param name="strMain">Main Node in the XML file for the settings</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLSettings( string strFile, string strRoot, string strMain )
		{
			DEFAULT_XML_FILE = strFile;
			DEFAULT_XML_MAIN_NODE = strMain;

			if( "" != strRoot )
			{
				DEFAULT_XML_ROOT_NODE = strRoot;
			}
        }

        /// <summary>
        /// Saves the current XML data to the specified file. All XML data in the file will be overwritten.
        /// </summary>
        /// <param name="strFilePath"> FilePath to save to. If "" is passed in for strFilePath, then m_strXMLFileName
        ///  will be used for the path. Otherwise the default file name will be appended to the strFilePath</param>
        /// <returns>Whether or not the save was successful.</returns>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool SaveSettings(string strFilePath)
        {
            bool bReturn = false;
            XmlDocument xmldocTemp = new XmlDocument();
            string strTemp = strFilePath;

            try
            {
                if (1 > strFilePath.Length)
                {
                    strTemp = m_strXMLFileName;
                }
                else
                {
                    strTemp = strFilePath;
                }

                Save(strTemp);

                if (null != xmldocTemp)
                {
                    xmldocTemp.Load(strTemp);

                    if (null != xmldocTemp.SelectSingleNode(DEFAULT_XML_ROOT_NODE))
                    {
                        bReturn = true;
                    }

                    xmldocTemp = null;
                }
            }
            catch
            {
                bReturn = false;
            }

            return bReturn;
        }

        /// <summary>
        /// Sets the current node to the childnode of the current node named strChildName. If it does not
        /// exist and bCreate is set to true then it will be created.
        /// </summary>
        /// <param name="strChildNode">Name of child node to select</param>
        /// <param name="bCreate">bool</param>
        /// <returns></returns>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual bool SelectNode(string strChildNode, bool bCreate)
        {
            bool bReturn = false;
            XmlNode xmlnodeChild;

            if ((null != m_xmlnodeCurrent) && (0 < strChildNode.Length))
            {
                xmlnodeChild = m_xmlnodeCurrent.SelectSingleNode(strChildNode);

                if ((null == xmlnodeChild) && (false != bCreate))
                {
                    xmlnodeChild = CreateElement(strChildNode);

                    if (null != xmlnodeChild)
                    {
                        xmlnodeChild = m_xmlnodeCurrent.AppendChild(xmlnodeChild);
                    }
                }

                if (null != xmlnodeChild)
                {
                    m_xmlnodeCurrent = xmlnodeChild;
                    bReturn = true;
                }
            }

            return bReturn;
        }

        /// <summary>
        /// Sets the current node to the selected hardware type node of the 
        /// "ActiveFiles" node named strChildName. If it does not exist and bCreate
        /// is set to true then it will be created.
        /// </summary>
        /// <param name="strChildNode">The hardware type node to select</param>
        /// <param name="bCreate">
        /// whether or not the hardware type node should
        /// be created if it does not already exist
        /// </param>
        /// <returns>true if the hardware type node exists and false otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/13/09 AF  2.20.01        Created
        //
        public bool SelectHWNode(string strChildNode, bool bCreate)
        {
            bool bReturn = false;
            XmlNode xmlnodeChild;
            XmlNode xmlnodeTemp;

            if (null != m_xmlnodeRoot)
            {
                xmlnodeTemp = m_xmlnodeRoot.SelectSingleNode("ActiveFiles");

                if ((null != xmlnodeTemp) && (0 < strChildNode.Length))
                {
                    xmlnodeChild = xmlnodeTemp.SelectSingleNode(strChildNode);

                    if ((null == xmlnodeChild) && (false != bCreate))
                    {
                        xmlnodeChild = CreateElement(strChildNode);

                        if (null != xmlnodeChild)
                        {
                            xmlnodeChild = xmlnodeTemp.AppendChild(xmlnodeChild);
                        }
                    }

                    if (null != xmlnodeChild)
                    {
                        m_xmlnodeCurrent = xmlnodeChild;
                        m_xmlnodeAnchor = m_xmlnodeCurrent;
                        bReturn = true;
                    }
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Sets the current node to the childnode of the current node named strChildName. If it does not
        /// exist and bCreate is set to true then it will be created.
        /// </summary>
        /// <param name="strChildNode">Name of child node to select</param>
        /// <param name="bCreate">whether or not to create the node if it does not exist</param>
        /// <returns>true if the node was located/created; false, otherwise</returns>
        /// <remarks>The only difference between this method and SelectNode() is that
        /// this one sets the anchor node to the current node.  This is necessary for
        /// correctly traversing the active files file where there can be an active
        /// firmware file for each hardware type.</remarks>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/23/09 AF  2.20.03        Created
        //
        public bool SelectHWSubNode(string strChildNode, bool bCreate)
        {
            bool bReturn = SelectNode(strChildNode, bCreate);

            if (bReturn)
            {
                m_xmlnodeAnchor = m_xmlnodeCurrent;
            }
            return bReturn;
        }

        /// <summary>
        /// Sets the selected node to be current node 
        /// </summary>
        /// <param name="strNodeName">The node to be set</param>
       /// <returns>true if the node be set and false otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/13/09 AF  2.20.01        Created
        //
        public bool SetCurrentNode(string strNodeName)
        {
            bool bReturn = false;
            XmlNode xmlnodeTemp;

            if (null != m_xmlnodeRoot)
            {
                xmlnodeTemp = m_xmlnodeRoot.SelectSingleNode(strNodeName);
                if (null != xmlnodeTemp) 
                {
                    m_xmlnodeCurrent = xmlnodeTemp;
                    bReturn = true;
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Sets the current node to the root node
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual void SetCurrentToRoot()
        {
            m_xmlnodeCurrent = m_xmlnodeRoot;
        }

        /// <summary>
        /// Sets the current node to the anchor node
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual void SetCurrentToAnchor()
        {
            m_xmlnodeCurrent = m_xmlnodeAnchor;
        }

        /// <summary>
        /// Sets the anchor node to the current node
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual void SetAnchorToCurrent()
        {
            m_xmlnodeAnchor = m_xmlnodeCurrent;
        }

        #endregion

        #region Public Properties

        /// <summary>
		/// Returns or Sets the m_strXMLFileName. If XMLFileName is set to "" then the default XMLFileName will be used. When the filename is set, then the DOM Document is reloaded.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
        ///05/29/08 jrf 1.50.28        Refactored some code into protected methods so 
        ///                            that it could be utilized by a derived class.
		///</pre></remarks>
		public virtual string XMLFileName
		{
			get
			{
				return m_strXMLFileName;
			}
			set
			{
				try
				{
					DetermineFileName(value);

					if( File.Exists( m_strXMLFileName ) )
					{
                        SetNormalAttributes();

						Load( m_strXMLFileName );

						m_xmlnodeRoot = SelectSingleNode( DEFAULT_XML_ROOT_NODE );
					}
					else
					{
						m_xmlnodeRoot = null;
					}

                    ProcessNodes();
				}
				catch(Exception)
				{
				}
			}
		}

        /// <summary>
		/// Returns or Sets a bool value into the current node
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool CurrentNodeBool
		{
			get
			{
				bool bReturn = false;

				if( XML_VALUE_BOOL_TRUE == GetValue() )
				{
						bReturn = true;
				}

				return bReturn;
			}
			set
			{
				if( false == value )
				{
					SetValue( XML_VALUE_BOOL_FALSE );
				}
				else
				{
					SetValue( XML_VALUE_BOOL_TRUE );
				}
			}
		}

		/// <summary>
		/// Returns of Sets a string value into the current node
		/// </summary>
		///<remarks ><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/03/04 REM 7.00.11 N/A    Initial Release
		///</pre></remarks>
		public virtual string CurrentNodeString
		{
			get
			{
				return GetValue();
			}
			set
			{
				SetValue( value );
			}
		}

        /// <summary>
        /// Returns of Sets a int value into the current node
        /// </summary>
        ///<remarks ><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///08/03/04 REM 7.00.11 N/A    Initial Release
        ///</pre></remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        public virtual int CurrentNodeInt
		{
			get
			{
				int intReturn = 0;
				
				if( null != m_xmlnodeCurrent )
				{
					if( 0 != m_xmlnodeCurrent.InnerText.CompareTo( "" ) )
					{
						intReturn = Convert.ToInt32( m_xmlnodeCurrent.InnerText, 10 );
					}
				}

				return intReturn;
			}
			set
			{
                SetValue(value.ToString(CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Returns or Sets a float value into the current node
		/// </summary>
		//Revision History
		//MM/DD/YY who Version Issue# Description
		//-------- --- ------- ------ ---------------------------------------------
		//08/03/04 REM 7.00.11 N/A    Initial Release
		//07/25/08 KRC 9.50.02        Adding Try/Catch
        //
        public virtual float CurrentNodeFloat
		{
			get
			{
				double dblReturn = 0.0;

                try
                {

                    if (null != m_xmlnodeCurrent)
                    {
                        dblReturn = Convert.ToDouble(m_xmlnodeCurrent.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                }
                catch
                {
                    dblReturn = 0.0;
                }

				return (float)dblReturn;
			}
			set
			{
				string test = XmlConvert.ToString( value );
				SetValue( XmlConvert.ToString( value ) );
			}
		}

		/// <summary>
		/// Returns the value of the current node as a float. Defaults to value passed in
		/// </summary>
		//Revision History
		//MM/DD/YY who Version Issue# Description
		//-------- --- ------- ------ ---------------------------------------------
		//08/22/05 REM 7.20.09 1625   Battery Carryover should default to Enable in System Manager
        //07/25/08 KRC 9.50.02        Adding Try/Catch
        //
		public virtual float GetCurrentNodeFloat( float fltDefaultValue )
		{
			double dblReturn = fltDefaultValue;

            try
            {
                if (null != m_xmlnodeCurrent)
                {
                    dblReturn = Convert.ToDouble(m_xmlnodeCurrent.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                }
            }
            catch
            {
                dblReturn = fltDefaultValue;
            }

			return (float)dblReturn;
		}

        /// <summary>
        /// Returns the value of the current node as a int. Defaults to value passed in
        /// </summary>
        //Revision History
        //MM/DD/YY who Version Issue# Description
        //-------- --- ------- ------ ---------------------------------------------
        //08/22/05 REM 7.20.09 1625   Battery Carryover should default to Enable in System Manager
        //07/25/08 KRC 9.50.02        Adding Try/Catch
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)")]
        public virtual int GetCurrentNodeInt( int intDefaultValue )
		{
			int intReturn = intDefaultValue;

            try
            {
                if (null != m_xmlnodeCurrent)
                {
                    if (0 != m_xmlnodeCurrent.InnerText.CompareTo(""))
                    {
                        intReturn = Convert.ToInt32(m_xmlnodeCurrent.InnerText, 10);
                    }
                }
            }
            catch
            {
                intReturn = intDefaultValue;
            }

			return intReturn;
		}
		
		/// <summary>
		/// Returns the value of the current node as a bool. Defaults to value passed in
		/// </summary>
		//Revision History
		//MM/DD/YY who Version Issue# Description
		//-------- --- ------- ------ ---------------------------------------------
		//08/22/05 REM 7.20.09 1625   Battery Carryover should default to Enable in System Manager
        //07/25/08 KRC 9.50.02        Adding Try/Catch
        //
		public virtual bool GetCurrentNodeBool( bool blnDefaultValue )
		{
			bool blnReturn = blnDefaultValue;

            try
            {
                if (XML_VALUE_BOOL_TRUE == GetValue())
                {
                    blnReturn = true;
                }
                else
                {
                    if (XML_VALUE_BOOL_FALSE == GetValue())
                    {
                        blnReturn = false;
                    }
                }
            }
            catch
            {
                blnReturn = blnDefaultValue;
            }

			return blnReturn;
		}

		/// <summary>
		/// Returns of Sets an array of int values into the current node.
		/// </summary>
		//Revision History
		//MM/DD/YY who Version Issue# Description
		//-------- --- ------- ------ ---------------------------------------------
		//08/03/04 REM 7.00.11 N/A    Initial Release
        //07/25/08 KRC 9.50.02        Adding Try/Catch
        //
		public virtual int[] CurrentNodeIntArray
		{
			get
			{
				string[] astrValues = new String[ GetValues().Length ];
				int[] aintReturn = new int[ astrValues.Length ];

                try
                {

                    for (int intIndex = 0; intIndex < astrValues.Length; intIndex++)
                    {
                        aintReturn[intIndex] = Convert.ToInt32(astrValues[intIndex], CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                }

				return aintReturn;
			}
			set
			{
				string[] astrValues = new String[ value.Length ];

				for( int intIndex = 0; intIndex < value.Length; intIndex++ )
				{
                    astrValues[intIndex] = value[intIndex].ToString(CultureInfo.InvariantCulture);
					SetValues( astrValues );
				}
			}
		}

		/// <summary>
		/// Returns or Sets an array of string values into the current node
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string[] CurrentNodeStringArray
		{
			get
			{
				return GetValues();
			}
			set
			{
				SetValues( value );
			}
		}

		/// <summary>
		/// Returns the value of the Value node of the current Node.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual string GetValue()
		{
			string strReturn = "";
			XmlNode xmlnodeTemp;

			if( null != m_xmlnodeCurrent )
			{
				xmlnodeTemp = m_xmlnodeCurrent.SelectSingleNode( XML_NODE_VALUE );
				if( null != xmlnodeTemp )
				{
					strReturn = xmlnodeTemp.InnerText;
				}
			}
			
			return strReturn;
        }

        /// <summary>
        /// Returns or Sets an list of string values into the current node
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/11/08 mrj 1.00.00		Created (OpenWay Tools)
        //  
        public virtual List<string> CurrentNodeStringList
        {
            get
            {
                List<string> lstrValues = new List<string>();

                if (null != m_xmlnodeCurrent)
                {
                    if (0 < m_xmlnodeCurrent.ChildNodes.Count)
                    {
                        foreach (XmlNode xmlnodeValue in m_xmlnodeCurrent.SelectNodes(XML_NODE_VALUE))
                        {
                            lstrValues.Add(xmlnodeValue.InnerText);
                        }
                    }
                }
                return lstrValues;
            }
            set
            {
                XmlNode xmlnodeTemp;
                System.Xml.XmlNodeList xmlnodelist;

                if (null != m_xmlnodeCurrent)
                {
                    //First clear out any existing values
                    xmlnodelist = m_xmlnodeCurrent.ChildNodes;

                    if (null != xmlnodelist)
                    {
                        for (int intNode = xmlnodelist.Count - 1; intNode > -1; intNode--)
                        {
                            if (false != xmlnodelist[intNode].Name.StartsWith(XML_NODE_VALUE, StringComparison.Ordinal))
                            {
                                m_xmlnodeCurrent.RemoveChild(xmlnodelist[intNode]);
                            }
                        }
                    }

                    //Now add the new data
                    foreach (string strValue in value)
                    {
                        xmlnodeTemp = CreateElement(XML_NODE_VALUE);

                        if (null != xmlnodeTemp)
                        {
                            xmlnodeTemp.InnerText = strValue;
                            m_xmlnodeCurrent.AppendChild(xmlnodeTemp);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns or Sets an array of float values into the current node.
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///07/29/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        public virtual float[] CurrentNodeFloatArray
        {
            get
            {
                float fltValue;
                string[] astrValues = GetValues();
                float[] afltReturn = new float[astrValues.Length];

                for (int intIndex = 0; intIndex < astrValues.Length; intIndex++)
                {
                    fltValue = XmlConvert.ToSingle(astrValues[intIndex]);
                    afltReturn[intIndex] = XmlConvert.ToSingle(astrValues[intIndex]);
                }

                return afltReturn;
            }
            set
            {
                string test;
                string[] astrValues = new String[value.Length];

                for (int intIndex = 0; intIndex < value.Length; intIndex++)
                {
                    test = XmlConvert.ToString(value[intIndex]);
                    astrValues[intIndex] = XmlConvert.ToString(value[intIndex]);
                }

                SetValues(astrValues);
            }
        }

        /// <summary>
        /// Gets the currently selected node
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  09/22/09 RCG 2.30.02        Created

        public XmlNode CurrentNode
        {
            get
            {
                return m_xmlnodeCurrent;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
		/// Sets a value into the current node's Value node
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetValue( string strValue )
		{
			XmlNode xmlnodeTemp;

			if( null != m_xmlnodeCurrent )
			{
				xmlnodeTemp = m_xmlnodeCurrent.SelectSingleNode( XML_NODE_VALUE );
				
				if( null != xmlnodeTemp )
				{
					xmlnodeTemp.InnerText = strValue;
				}
				else
				{
					xmlnodeTemp = CreateElement( XML_NODE_VALUE );
					if( null != xmlnodeTemp )
					{
						xmlnodeTemp.InnerText = strValue;
						m_xmlnodeCurrent.AppendChild( xmlnodeTemp );
					}
				}
			}
        }

        /// <summary>
		/// Returns an array of strings in the Value# nodes of the the current node.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual string[] GetValues()
		{
			ArrayList alValues = new ArrayList( 0 );
			string[] astrReturn;

			if( null != m_xmlnodeCurrent )
			{
				if( 0 < m_xmlnodeCurrent.ChildNodes.Count )
				{
					foreach( XmlNode xmlnodeValue in m_xmlnodeCurrent.SelectNodes( XML_NODE_VALUE ) )
					{
						alValues.Add( xmlnodeValue.InnerText );
					}
				}
			}
			
			astrReturn = new string[ alValues.Count ];
			for( int intIndex = 0; intIndex < alValues.Count; intIndex++ )
			{
				astrReturn[ intIndex ] = alValues[ intIndex ].ToString();
			}

			return astrReturn;
		}

		/// <summary>
		/// Saves an array of strings into the Value# nodes of the current node
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetValues(string[] astrValues)
		{
			XmlNode xmlnodeTemp;
			System.Xml.XmlNodeList xmlnodelist;

			if( null != m_xmlnodeCurrent )
			{
				//First clear out any existing values
				xmlnodelist = m_xmlnodeCurrent.ChildNodes;
				
				if( null != xmlnodelist )
				{
					for( int intNode = xmlnodelist.Count - 1; intNode > -1; intNode-- )
					{
						if( false != xmlnodelist[ intNode ].Name.StartsWith( XML_NODE_VALUE, StringComparison.Ordinal ) )
						{
							m_xmlnodeCurrent.RemoveChild( xmlnodelist[ intNode ] );
						}
					}
				}

				//Now add the new data
				for( int intIndex = 0; intIndex < astrValues.Length; intIndex++ )
				{
					xmlnodeTemp = CreateElement( XML_NODE_VALUE );
					
					if( null != xmlnodeTemp )
					{
						xmlnodeTemp.InnerText = astrValues[ intIndex ];
						m_xmlnodeCurrent.AppendChild( xmlnodeTemp );
					}
				}
			}
        }

        /// <summary>
        /// This method sets the file name to the passed in value 
        /// or sets it to the default file name if the passed in 
        /// value is empty.
        /// </summary>
        /// <param name="strValue">The file name to check.</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/29/08 jrf 1.50.28        Refactored.
        protected void DetermineFileName(string strValue)
        {
            if (0 < strValue.Length)
            {
                m_strXMLFileName = strValue;
            }
            else
            {
                m_strXMLFileName = DEFAULT_XML_FILE;
            }
        }

        /// <summary>
        /// This method sets up the nodes after the xml file is loaded.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/29/08 jrf 1.50.28        Refactored.
        protected void ProcessNodes()
        {
            if (null == m_xmlnodeRoot)
            {
                //We didn't find what we were looking for so just create a blank one
                foreach (XmlNode xmlnode in ChildNodes)
                {
                    this.RemoveChild(xmlnode);
                }

                m_xmlnodeRoot = CreateElement(DEFAULT_XML_ROOT_NODE);

                if (null != m_xmlnodeRoot)
                {
                    m_xmlnodeRoot = AppendChild(m_xmlnodeRoot);
                }
            }

            if (null != m_xmlnodeRoot)
            {
                if (0 < DEFAULT_XML_MAIN_NODE.Length)
                {
                    m_xmlnodeCurrent = m_xmlnodeRoot.SelectSingleNode(DEFAULT_XML_MAIN_NODE);

                    if (null == m_xmlnodeCurrent)
                    {
                        m_xmlnodeCurrent = CreateElement(DEFAULT_XML_MAIN_NODE);

                        if (null != m_xmlnodeCurrent)
                        {
                            m_xmlnodeCurrent = m_xmlnodeRoot.AppendChild(m_xmlnodeCurrent);
                        }
                    }

                    if (null != m_xmlnodeCurrent)
                    {
                        m_xmlnodeAnchor = m_xmlnodeCurrent;
                    }
                }
                else
                {
                    m_xmlnodeCurrent = m_xmlnodeRoot;
                    m_xmlnodeAnchor = m_xmlnodeRoot;
                }
            }
        }

        /// <summary>
        /// This method sets the file attributes to normal.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/29/08 jrf 1.50.28        Refactored.
        protected void SetNormalAttributes()
        {
            //REM 10/11/04: Try setting the XML file to normal attributes.
            try
            {
                FileInfo fileTemp = new FileInfo(m_strXMLFileName);
                if (false != fileTemp.Exists)
                {
                    fileTemp.Attributes = FileAttributes.Normal;
                }
            }
            catch
            {
            }
        }

        #endregion
	}
}
