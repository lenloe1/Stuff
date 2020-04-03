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
using System.Text;
using System.ServiceModel;
using System.Windows.Forms;
using Itron.Metering.ReplicaSettings;
using Itron.Ami.CEWebServiceClient.External;
using Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy;
using Itron.Metering.CustomerValidationTool.Properties;
using Itron.Metering.Device;
using Itron.Metering.SharedControls;

namespace Itron.Metering.CustomerValidationTool
{
    /// <summary>
    /// Class containing public methods used to update Signed Authorization keys.
    /// </summary>
    internal static class SignedAuthenticator
    {
        #region Constants

        private const string AUTH_SEC_APPLICATION = "SignedAuthorizationUsernameClient";

        #endregion

        #region Internal Methods

        /// <summary>
        /// Updates the signed authorization key stored in user settings.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/19/09 RCG 2.30.11	    Created

        internal static void UpdateSignedAuthorizationKey()
        {
            CXMLOpenWaySystemSettings SystemSettings = new CXMLOpenWaySystemSettings("");
            bool bCanceled = false;

            // Set up the Signed Authorization endpoint
            ExternalLib.Endpoint.SecurityEndpointName = SystemSettings.AuthorizationSecurityMethod;

            // If we are not using Windows Login we need to prompt the user for a User Name and Password
            if (SystemSettings.AuthorizationSecurityMethod == AUTH_SEC_APPLICATION)
            {
                LoginDialog Login = new LoginDialog();
                Login.HelpID = "Signed Authorization Login";
                Login.Title = "Log in to the Signed Authorization Server";

                if (Login.ShowDialog() == DialogResult.OK)
                {
                    ExternalLib.Endpoint.ExternalUserName = Login.UserName;
                    ExternalLib.Endpoint.ExternalUserPassword = Login.Password;
                }
                else
                {
                    // The user hit cancel
                    bCanceled = true;
                }
            }

            // Set the location of the Authorization server using the url entered in Shop Manager
            if (bCanceled == false && true == ExternalLib.Endpoint.SetClientEndpointAddress(SystemSettings.AuthorizationSecurityMethod,
                SystemSettings.AuthorizationServer + "/ami/V2009/12/security/opticalSignedAuthorization"))
            {
                // Make the request for an Authorization key. Specifying Unknown permission level and Zero duration
                // will give us the highest level for the current user and the longest duration allowed.
                SignatureAuthorization signedAuth = ExternalLib.Request.SignatureAuthorization("", PermissionLevel.Unknown, TimeSpan.Zero);
                signedAuth.Execute();

                if (signedAuth != null)
                {
                    // Create the Authorization key object and save it to the user settings.
                    Settings.Default.AuthenticationKey = new SignedAuthorizationKey(signedAuth.Result.SignedAuthorization);
                    Settings.Default.Save();
                }
            }
        }

        #endregion

    }
}
