using System;
using System.Collections.Generic;
using System.ComponentModel;
using DeployLX.Licensing.v3;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Class to be used for validating a Beta license.
    /// </summary>
    [LicenseProvider(typeof(SecureLicenseManager))]
    public class BetaLicense
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/08 RCG 2.00.00 N/A    Created

        public BetaLicense(string applicationName)
        {
            m_License = null;
            m_ApplicationName = applicationName;

            GenerateSupportInfo();

            m_LicenseInfo = new LicenseValidationRequestInfo();
            m_LicenseInfo.SupportInfo = m_LicenseSupportInfo;
        }

        /// <summary>
        /// Validates the Beta license.
        /// </summary>
        /// <returns>The license object.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/08 RCG 2.00.00 N/A    Created

        public SecureLicense Validate()
        {
            m_License = SecureLicenseManager.Validate(null, typeof(BetaLicense), m_LicenseInfo);
            return m_License;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the license object generated for the last validate.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/08 RCG 2.00.00 N/A    Created

        public SecureLicense License
        {
            get
            {
                return m_License;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates the support information for the calling application.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/13/08 RCG 2.00.00 N/A    Created

        private void GenerateSupportInfo()
        {
            m_LicenseSupportInfo = new SupportInfo();

            m_LicenseSupportInfo.Company = Application.CompanyName;
            m_LicenseSupportInfo.Product = m_ApplicationName;
            m_LicenseSupportInfo.ProductVersion = Application.ProductVersion;
            m_LicenseSupportInfo.Email = "support@itron.com";
            m_LicenseSupportInfo.Phone = "(864)718-8300";
            m_LicenseSupportInfo.Website = @"http://www.itron.com/pages/resources_support.asp";
        }

        #endregion

        #region Member Variables

        private SecureLicense m_License;
        private LicenseValidationRequestInfo m_LicenseInfo;
        private SupportInfo m_LicenseSupportInfo;
        private string m_ApplicationName;
        #endregion
    }
}
