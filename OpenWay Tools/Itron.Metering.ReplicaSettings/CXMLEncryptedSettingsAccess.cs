using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// XML Encrypted Settings Access Class.  This method provides access 
    /// to an encrypted settings file.
	/// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------------
    // 05/27/08 jrf 1.50.28        Created.
    //
	public class CXMLEncryptedSettingsAccess: CXMLSettingsAccess
	{
		/// <summary>
		/// CXMLSettingsAccess protected member variable
		/// </summary>
		protected CXMLEncryptedSettings m_EncryptedXMLSettings;

		/// <summary>
		/// Constructor
		/// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        public CXMLEncryptedSettingsAccess()
		{
		}

		/// <summary>
		/// Saves the current DOM to file
		/// </summary>
		/// <param name="strPath">File name to use for the .xml file.</param>
		/// <returns>Wehether or not the Save was successful</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
		public override bool SaveSettings( string strPath )
		{
            return m_EncryptedXMLSettings.SaveSettings(strPath);
		}

		/// <summary>
		/// Sets the current node to the node name passed in and returns the value of the node as a float
		/// </summary>
		/// <param name="strNode"></param>
		/// <returns></returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override float GetFloat(string strNode)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            return m_EncryptedXMLSettings.CurrentNodeFloat;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a float.
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="fltValue">Float to set the value of the node to</param>
		// Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override void SetFloat(string strNode, float fltValue)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            m_EncryptedXMLSettings.CurrentNodeFloat = fltValue;
		}

		/// <summary>
		/// Sets the current node to the node passed in and gets the value of the node as a bool
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <returns>Value of the node as a bool</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override bool GetBool(string strNode)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            return m_EncryptedXMLSettings.CurrentNodeBool;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a bool
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="blnValue">Value to set into node value</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override void SetBool(string strNode, bool blnValue)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            m_EncryptedXMLSettings.CurrentNodeBool = blnValue;
		}

		/// <summary>
		/// Sets the current node to the node passed in and sets the value of the node as a string
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
		/// <param name="strValue">Value to set into node value</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override void SetString(string strNode, string strValue)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            m_EncryptedXMLSettings.CurrentNodeString = strValue;
		}

		/// <summary>
		/// Sets the current node to the node passed in and gets the value of the node as a string
		/// </summary>
		/// <param name="strNode">Name of node to select</param>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 05/27/08 jrf 1.50.28        Created.
        //
        protected override string GetString(string strNode)
		{
            m_EncryptedXMLSettings.SetCurrentToAnchor();

            m_EncryptedXMLSettings.SelectNode(strNode, true);

            return m_EncryptedXMLSettings.CurrentNodeString;
		}
	}
}
