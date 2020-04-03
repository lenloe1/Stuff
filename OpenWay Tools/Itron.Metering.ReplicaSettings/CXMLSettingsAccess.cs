using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// XML Settings Access Base Class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLSettingsAccess
	{
		//Constants
		/// <summary>
		/// protected const string FILE_PATH_REPLICA = "Replica";
		/// </summary>
		protected const string FILE_PATH_REPLICA = "Replica";
		/// <summary>
		/// public readonly string DEFAULT_SETTINGS_DIRECTORY = CRegistryHelper.GetFilePath( FILE_PATH_REPLICA );
		/// </summary>
		public readonly string DEFAULT_SETTINGS_DIRECTORY = CRegistryHelper.GetFilePath( FILE_PATH_REPLICA );
		/// <summary>
		/// CXMLSettingsAccess protected member variable
		/// </summary>
		protected CXMLSettings m_XMLSettings;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLSettingsAccess( )
		{
		}

		/// <summary>
		/// Saves the current DOM to file
		/// </summary>
		/// <param name="strPath">File name to use for the .xml file.</param>
		/// <returns>Wehether or not the Save was successful</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool SaveSettings( string strPath )
		{
			return m_XMLSettings.SaveSettings( strPath );
		}

		/// <summary>
		/// Sets the current node to the node name passed in and returns the value of the node as a float
		/// </summary>
		/// <param name="strNode"></param>
		/// <returns></returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual float GetFloat( string strNode )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );
				
			return m_XMLSettings.CurrentNodeFloat;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a float.
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="fltValue">Float to set the value of the node to</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetFloat( string strNode, float fltValue )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );

			m_XMLSettings.CurrentNodeFloat = fltValue;
		}

        /// <summary>
        /// Gets the value of the specified node as an int.
        /// </summary>
        /// <param name="strNode">The node to get.</param>
        /// <returns>The value of the node as an int.</returns>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/17/08 RCG 1.51.05		Created

        protected virtual int GetInt( string strNode)
        {
            m_XMLSettings.SetCurrentToAnchor();

            m_XMLSettings.SelectNode( strNode, true);

            return m_XMLSettings.CurrentNodeInt;
        }

        /// <summary>
        /// Sets the value of the specified node as an int.
        /// </summary>
        /// <param name="strNode">The node to set.</param>
        /// <param name="iValue">The value of the node.</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/17/08 RCG 1.51.05		Created

        protected virtual void SetInt(string strNode, int iValue)
        {
            m_XMLSettings.SetCurrentToAnchor();

            m_XMLSettings.SelectNode(strNode, true);

            m_XMLSettings.CurrentNodeInt = iValue;
        }

		/// <summary>
		/// Sets the current node to the node passed in and gets the value of the node as a bool
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <returns>Value of the node as a bool</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual bool GetBool( string strNode )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );
				
			return m_XMLSettings.CurrentNodeBool;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a bool
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="blnValue">Value to set into node value</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetBool( string strNode, bool blnValue )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );

			m_XMLSettings.CurrentNodeBool = blnValue;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a string
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="strValue">Value to set into node value</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/27/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual void SetString( string strNode, string strValue )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );

			m_XMLSettings.CurrentNodeString = strValue;
		}

		/// <summary>
		/// Sets the current node to the node passed in and gets the value of the node as a string
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///08/27/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected virtual string GetString( string strNode )
		{
			m_XMLSettings.SetCurrentToAnchor();

			m_XMLSettings.SelectNode( strNode, true );

			return m_XMLSettings.CurrentNodeString;
		}

        /// <summary>
        /// Determines if the passed in node exists.
        /// </summary>
        /// <param name="strNode">Name of node to check</param>
        //  Revision History
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  04/24/13 jrf 2.80.23 TQ8285 Created.
        //
        protected virtual bool NodeExists(string strNode)
        {
            m_XMLSettings.SetCurrentToAnchor();

            return m_XMLSettings.SelectNode(strNode, false);
        }
	}
}
