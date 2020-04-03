using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used to get information about the Calling Application
    /// </summary>
    public class CallingAppInfo
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/10 RCG 2.40.23 N/A    Created

        public CallingAppInfo()
        {
#if(!WindowsCE)
            m_CallingAssembly = Assembly.GetEntryAssembly();
#else
            // The best we can do for Windows CE is get the calling assembly
            m_CallingAssembly = Assembly.GetCallingAssembly();
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Product Name of the calling application
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/10 RCG 2.40.23 N/A    Created

        public string ProductName
        {
            get
            {
                string strProductName = "";
                object[] Attributes = m_CallingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                if (Attributes.Length > 0)
                {
                    AssemblyProductAttribute ProductAttribute = Attributes[0] as AssemblyProductAttribute;

                    if (ProductAttribute != null)
                    {
                        strProductName = ProductAttribute.Product;
                    }
                }

                return strProductName;
            }
        }

        /// <summary>
        /// Gets the Version of the calling application
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/04/10 RCG 2.40.23 N/A    Created

        public string Version
        {
            get
            {
                return m_CallingAssembly.GetName().Version.ToString();
            }
        }

        #endregion

        #region Member Variables

        private Assembly m_CallingAssembly;

        #endregion
    }
}
