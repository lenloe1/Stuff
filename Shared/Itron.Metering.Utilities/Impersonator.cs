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
//                              Copyright © 2015 
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used to temporarily impersonate another user in cases where a remote service call
    /// needs to be validated by a user in a different domain
    /// </summary>
    public class Impersonator : IDisposable
    {
        #region Constants

        private const int IMPERSONATION_LEVEL = 2;
        private const int LOGON_NEW_CREDENTIALS = 9;
        private const int PROVIDER_DEFAULT = 0;

        #endregion

        #region External Methods

        /// <summary>
        /// Logs on as the specified user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="domain">User domain</param>
        /// <param name="password">User password</param>
        /// <param name="logonType">Logon Type</param>
        /// <param name="logonProvider">Logon Provider</param>
        /// <param name="token">User token</param>
        /// <returns>The result of the operation</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LogonUser(string userName, string domain, string password, int logonType, int logonProvider, ref IntPtr token);

        /// <summary>
        /// Duplicates the specified token
        /// </summary>
        /// <param name="token">The token to duplicate</param>
        /// <param name="impersonationLevel">The impersonation level</param>
        /// <param name="newToken">The new token</param>
        /// <returns>The result of the operation</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(IntPtr token, int impersonationLevel, ref IntPtr newToken);

        /// <summary>
        /// Reverts the user to self
        /// </summary>
        /// <returns>True if successful. False otherwise</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        /// <summary>
        /// Closes the specified handle
        /// </summary>
        /// <param name="handle">The handle to close</param>
        /// <returns>True if the handle was closed. False otherwise</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public Impersonator()
        {
            m_Context = null;
            m_Identity = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="domain">User domain</param>
        /// <param name="userName">User name</param>
        /// <param name="password">User password</param>
        public Impersonator(string domain, string userName, string password)
        {
            m_Context = null;
            m_Identity = null;

            ImpersonateUser(domain, userName, password);
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            RevertImpersonation();
        }

        /// <summary>
        /// Impersonates the specified user
        /// </summary>
        /// <param name="domain">User domain</param>
        /// <param name="userName">User name</param>
        /// <param name="password">User password</param>
        private void ImpersonateUser(string domain, string userName, string password)
        {
            IntPtr token = IntPtr.Zero;
            IntPtr duplicate = IntPtr.Zero;

            try
            {
                if (RevertToSelf() == true)
                {
                    if (LogonUser(userName, domain, password, LOGON_NEW_CREDENTIALS, PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, IMPERSONATION_LEVEL, ref duplicate) != 0)
                        {
                            m_Identity = new WindowsIdentity(duplicate);
                            m_Context = m_Identity.Impersonate();
                        }
                        else
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }

                if (duplicate != IntPtr.Zero)
                {
                    CloseHandle(duplicate);
                }
            }
        }

        /// <summary>
        /// Reverts the impersonation
        /// </summary>
        private void RevertImpersonation()
        {
            if (m_Context != null)
            {
                m_Context.Undo();
                m_Context.Dispose();
                m_Context = null;
            }

            if (m_Identity != null)
            {
                m_Identity.Dispose();
                m_Identity = null;
            }
        }

        #endregion

        #region Member Variables

        private WindowsIdentity m_Identity;
        private WindowsImpersonationContext m_Context;

        #endregion
    }
}
