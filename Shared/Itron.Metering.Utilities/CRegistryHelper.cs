using System;
using Microsoft.Win32;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Generic Registry Helper Class
    /// </summary>
    /// <remarks><pre>
    ///Revision History
    ///MM/DD/YY who Version Issue# Description
    ///-------- --- ------- ------ ---------------------------------------------
    ///07/29/04 REM 7.00.15 N/A    Initial Release
    ///</pre></remarks>
    public class CRegistryHelper
    {
        #region Constants

        /// <summary>
        /// Base Registry path for 32-bit OS
        /// </summary>
        protected const string REG_KEY_BASE_32 = "SOFTWARE\\";
        /// <summary>
        /// Base Registry path for 64-bit OS
        /// </summary>
        protected const string REG_KEY_BASE_64 = "SOFTWARE\\Wow6432Node\\";

        /// <summary>
        /// protected const string REG_KEY_FILE_PATHS = "SOFTWARE\\Itron\\Metering\\FilePaths";
        /// </summary>
        protected const string REG_KEY_FILE_PATHS = "Itron\\Metering\\FilePaths";
        /// <summary>
        /// protected const string REG_KEY_APPLICATIONS = "SOFTWARE\\Itron\\Metering\\Applications";
        /// </summary>
        protected const string REG_KEY_APPLICATIONS = "Itron\\Metering\\Applications";
        /// <summary>
        /// protected const string REG_VALUE_DATA_DIRECTORY = "DataDirectory";
        /// </summary>
        protected const string REG_VALUE_DATA_DIRECTORY = "DataDirectory";

        /// <summary>
        /// protected const string REG_KEY_LOGON_OPTIONS = "SOFTWARE\\Itron\\Metering\\Defaults\\Logon";
        /// </summary>
        protected const string REG_KEY_LOGON_OPTIONS = "Itron\\Metering\\Defaults\\Logon";

        //Strings used in HH-Pro to get the handheld name
        private const string REG_KEY_IDENT = "Ident";
        private const string REG_DEVICE_NAME = "Name";

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the path found in the registry for the File Path requested
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------------
        // 07/29/04 REM 7.00.15 N/A    Initial Release
        // 06/01/07 RCG 8.10.06        Removed code that added \\ to the end of the string
        //                             since we now include paths to files in the registry

        static public string GetFilePath( string strFilePathKey )
        {
            string strReturn = "";

            try
            {
                RegistryKey regRoot = Registry.LocalMachine;
                RegistryKey regFilePath = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_FILE_PATHS );

                if( null != regFilePath )
                {
                    strReturn = regFilePath.GetValue( strFilePathKey ).ToString();
                }
            }
            catch
            {
                strReturn = "";
            }

            return strReturn;
        }

        /// <summary>
        /// Returns the Data Directory found for the Data Directory requested
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///08/30/04 REM 7.00.15 N/A    Initial Release
        ///</pre></remarks>
        static public string GetDataDirectory( string strApplication )
        {
            string strReturn = "";
            object objTemp = null;
            
            RegistryKey regRoot = Registry.LocalMachine;
            RegistryKey regDataDirectory = null;

            try
            {
                regDataDirectory = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_APPLICATIONS + "\\" + strApplication);
                if( null != regDataDirectory )
                {
                    objTemp = regDataDirectory.GetValue( REG_VALUE_DATA_DIRECTORY );
                        
                    if( null != objTemp )
                    {
                        strReturn = objTemp.ToString();

                        if( 2 < strReturn.Length )
                        {
                            if( 0 != String.Compare( strReturn.Substring( strReturn.Length - 1, 1 ), "\\", StringComparison.Ordinal ) )
                            {
                                strReturn += "\\";
                            }
                        }
                    }
                }
            }
            catch
            {
                strReturn = "";
            }

            regRoot.Close();
            if (null != regDataDirectory)
            {
                regDataDirectory.Close();
            }
            return strReturn;
        }

        /// <summary>
        /// Returns the requested string from the registry from the requested application
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///10/05/04 REM 7.00.22 N/A    Initial Release
        ///</pre></remarks>
        static public string GetProgramString( string strApplication, string strString )
        {
            string strReturn = "";
            object objTemp = null;
            
            RegistryKey regRoot = Registry.LocalMachine;
            RegistryKey regDataDirectory = null;

            try
            {
                regDataDirectory = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_APPLICATIONS + "\\" + strApplication);
                if( null != regDataDirectory )
                {
                    objTemp = regDataDirectory.GetValue( strString );
                        
                    if( null != objTemp )
                    {
                        strReturn = objTemp.ToString();

                        if( 2 < strReturn.Length )
                        {
                            if (0 != String.Compare(strReturn.Substring(strReturn.Length - 1, 1), "\\", StringComparison.Ordinal))
                            {
                                strReturn += "\\";
                            }
                        }
                    }
                }
            }
            catch
            {
                strReturn = "";
            }

            regRoot.Close();
            regDataDirectory.Close();
            return strReturn;
        }

        /// <summary>
        /// Gets a value out of the registry from the given application key 
        /// </summary>
        /// <param name="strApplication">The application to get the value for</param>
        /// <param name="strValue">The value to get out of the registry</param>
        /// <returns>Returns an object containing the value requested</returns>		
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/22/05 mrj 7.13.00 N/A	Created
        /// 		
        static public object GetApplicationValue(string strApplication, string strValue)
        {
            object objValue = null;
            RegistryKey regDataDirectory = null;
            RegistryKey regRoot = Registry.LocalMachine;


            //Open the key for the given application
            regDataDirectory = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_APPLICATIONS + "\\" + strApplication);

            if (null != regDataDirectory)
            {
                //Get the requested value
                objValue = regDataDirectory.GetValue(strValue);
            }

            if (null != regRoot)
            {
                regRoot.Close();
            }
            if (null != regDataDirectory)
            {
                regDataDirectory.Close();
            }


            return objValue;
        }

        /// <summary>
        /// Sets a value to the registry from the given application key 
        /// </summary>
        /// <param name="strApplication">The application to get the value for</param>
        /// <param name="strValue">The value to set to the registry</param>
        /// <param name="objData">The data to set to the registry</param>		 
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 07/26/05 mrj 7.13.00 N/A	Created

        static public void SetApplicationValue(string strApplication,
            string strValue,
            object objData)
        {
            RegistryKey regDataDirectory = null;
            RegistryKey regRoot = Registry.LocalMachine;


            //Open the key for the given application
            regDataDirectory = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_APPLICATIONS + "\\" + strApplication, true);

            if (null != regDataDirectory)
            {
                //Set the requested value
                regDataDirectory.SetValue(strValue, objData);
            }

            if (null != regRoot)
            {
                regRoot.Close();
            }
            if (null != regDataDirectory)
            {
                regDataDirectory.Close();
            }
        }

        /// <summary>
        /// Sets a value to the registry for Logon Options
        /// </summary>
        /// <param name="strValue">The value to set to the registry</param>
        /// <param name="objData">The data to set to the registry</param>		 
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/21/07 KRC 9.00.03 N/A	Need to write Logon options

        static public void SetLogonValue(string strValue, object objData)
        {
            RegistryKey regDataDirectory = null;
            RegistryKey regRoot = Registry.CurrentUser;

            // Open the Key for Logon Options
            regDataDirectory = regRoot.OpenSubKey( GetBaseRegistryKey() + REG_KEY_LOGON_OPTIONS, true);

            if (null != regDataDirectory)
            {
                // Set the requested value
                regDataDirectory.SetValue(strValue, objData);
            }

            if (null != regRoot)
            {
                regRoot.Close();
            }
            if (null != regDataDirectory)
            {
                regDataDirectory.Close();
            }
        }

        /// <summary>
        /// Determines the path to the base registry keys
        /// </summary>
        /// <returns>The path to the keys</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/03/10 RCG 2.45.14 153374 Created
        // 02/15/11 RCG 2.50.04 167806 Fixing issue where 32-bit machine could have Wow6432Node
        //                             registry key causing us to look in the wrong location

        public static string GetBaseRegistryKey()
        {
            string strBase = REG_KEY_BASE_32;

            if(OSVersionChecker.Is64BitOperatingSystem())
            {
                strBase = REG_KEY_BASE_64;
            }

            return strBase;
        }

        /// <summary>
        /// Gets the path to the Default Web Browser
        /// </summary>
        /// <returns>The path to the default browser or null if none found</returns>
        public static string GetDefaultBrowserPath()
        {
            string Path = null;

            try
            {
                RegistryKey Key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                Path = Key.GetValue(null, null) as string;

                if (string.IsNullOrEmpty(Path) == false)
                {
                    // If we split on the " character the path to the browser .exe will always be the second item
                    Path = "\"" + Path.Split('"')[1] +"\"";
                }

                Key.Close();
            }
            catch (Exception)
            {
            }

            return Path;
        }

#if (WindowsCE)
        /// <summary>
        /// Gets the name of the handheld device (FC200)
        /// </summary>		
        /// <returns>
        /// string - Name of the handheld
        /// </returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/13/06 mrj 7.30.00        Created for HH-Pro
        /// 
        static public string GetHandheldName()
        {
            string strReturn = "";
            object objTemp = null;
            RegistryKey regIdent = null;
            RegistryKey regRoot = Registry.LocalMachine;


            regIdent = regRoot.OpenSubKey(REG_KEY_IDENT);
            if (null != regIdent)
            {
                objTemp = regIdent.GetValue(REG_DEVICE_NAME);

                if (null != objTemp)
                {
                    strReturn = objTemp.ToString();
                }
            }

            regRoot.Close();
            regIdent.Close();
            return strReturn;
        }
#endif

        #endregion
    }
}
