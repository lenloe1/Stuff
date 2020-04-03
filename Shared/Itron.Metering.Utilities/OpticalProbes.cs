///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and 
//                                trade secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential  
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or  
//  otherwise. Including photocopying and recording or in connection with any 
//  information storage or retrieval system without the permission in writing 
//  from Itron, Inc.
//
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Microsoft.Win32;
using System.Globalization;

namespace Itron.Metering.Utilities
{
        /// <summary>
    /// The bits indicating which item to reset via MFG procedure 6
    /// </summary>
    public enum OpticalProbeTypes
    {
        /// <summary>
        /// Schlumberger
        /// </summary>
        SCHLUMBERGER,
        /// <summary>
        /// Sclumberger (France)
        /// </summary>
        SCHLUMBERGER_FRANCE,
        /// <summary>
        /// Schlumberger (Spain)
        /// </summary>
        SCHLUMBERGER_SPAIN,
        /// <summary>
        /// US Microtel PM-300
        /// </summary>
        US_MICROTEL_PM_300,
        /// <summary>
        /// US Microtel PM-500
        /// </summary>
        US_MICROTEL_PM_500,
        /// <summary>
        /// US Microtel PM-600
        /// </summary>
        US_MICROTEL_PM_600,
        /// <summary>
        /// GE Smartcoupler SC-1
        /// </summary>
        GE_SMARTCOUPLER_SC1,
        /// <summary>
        /// Generic 1 (DTR Not Set)
        /// </summary>
        GENERIC_1_NO_DTR,
        /// <summary>
        /// Generic 2 (DTR Set)
        /// </summary>
        GENERIC_2_DTR
    }

	/// <summary>
	/// Summary description for OpticalProbes.
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class COpticalProbes
    {
        #region Definitions
        /// <summary>
		/// Structure for storing Optical Probe information
		/// </summary>
		public struct OpticalProbeInfo
		{
			/// <summary>
			/// OpticalProbeInfo public member variable
			/// </summary>
			public bool m_blnDTR;
			/// <summary>
			/// OpticalProbeInfo public member variable
			/// </summary>
			public bool m_blnRTS;
			/// <summary>
			/// OpticalProbeInfo public member variable
			/// </summary>
			public string m_strTitle;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="blnDTR"></param>
			/// <param name="blnRTS"></param>
			/// <param name="strTitle"></param>
			public OpticalProbeInfo( bool blnDTR, bool blnRTS, string strTitle )
			{
				m_blnDTR = blnDTR;
				m_blnRTS = blnRTS;
				m_strTitle = strTitle;
			}
        }

        #endregion

        #region Constants

        /// <summary>
		/// protected const string REG_KEY = "Software\\Itron\\Metering\\Data\\Optical Probes";
		/// </summary>
		protected const string REG_KEY = "Itron\\Metering\\Data\\Optical Probes";
		/// <summary>
		/// protected const string REG_KEY_PROBE_BASE = "Probe";
		/// </summary>
		protected const string REG_KEY_PROBE_BASE = "Probe";
		/// <summary>
		/// protected const string REG_VALUE_DTR = "DTR";
		/// </summary>
		protected const string REG_VALUE_DTR = "DTR";
		/// <summary>
		/// protected const string REG_VALUE_RTS = "RTS";
		/// </summary>
		protected const string REG_VALUE_RTS = "RTS";
		/// <summary>
		/// protected const string REG_VALUE_TITLE = "Title";
		/// </summary>
		protected const string REG_VALUE_TITLE = "Title";

        private const string PROBE_SLB = "Schlumberger";
        private const string PROBE_SLB_FRANCE = "Schlumberger (France)";
        private const string PROBE_SLB_SPAIN = "Schlumberger (Spain)";
        private const string PROBE_USM_300 = "US Microtel PM-300";
        private const string PROBE_USM_500 = "US Microtel PM-500";
        private const string PROBE_USM_600 = "US Microtel PM-600";
        private const string PROBE_GE = "GE Smartcoupler SC-1";
        private const string PROBE_GENERIC_1 = "Generic 1";
        private const string PROBE_GENERIC_2 = "Generic 2";

        #endregion

        #region Public Methods

        /// <summary>
		/// Constructor
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public COpticalProbes()
		{
			try
			{
				RegistryKey regRoot = Registry.LocalMachine;
				RegistryKey regOpticalProbes;
				RegistryKey regOpticalProbe;

				if( null != regRoot )
				{
                    regOpticalProbes = regRoot.OpenSubKey(CRegistryHelper.GetBaseRegistryKey() + REG_KEY);

					if( null != regOpticalProbes )
					{
						m_structOpticalProbes = new OpticalProbeInfo[ regOpticalProbes.SubKeyCount ];

						for( int intOpticalProbe = 0; intOpticalProbe < regOpticalProbes.SubKeyCount; intOpticalProbe++ )
						{
							m_structOpticalProbes[ intOpticalProbe ] = new OpticalProbeInfo( false, false, "" );
							regOpticalProbe = regOpticalProbes.OpenSubKey( REG_KEY_PROBE_BASE + ( intOpticalProbe + 1 ).ToString( CultureInfo.InvariantCulture ) );
							
							if( null != regOpticalProbe )
							{
								if( 0 == String.Compare( "0", regOpticalProbe.GetValue( REG_VALUE_DTR ).ToString(), StringComparison.Ordinal  ) )
								{
									m_structOpticalProbes[ intOpticalProbe ].m_blnDTR = false;
								}
								else
								{
									m_structOpticalProbes[ intOpticalProbe ].m_blnDTR = true;
								}

								if (0 == String.Compare("0", regOpticalProbe.GetValue(REG_VALUE_RTS).ToString(), StringComparison.Ordinal))
								{
									m_structOpticalProbes[ intOpticalProbe ].m_blnRTS = false;
								}
								else
								{
									m_structOpticalProbes[ intOpticalProbe ].m_blnRTS = true;
								}

								m_structOpticalProbes[ intOpticalProbe ].m_strTitle = regOpticalProbe.GetValue( REG_VALUE_TITLE ).ToString();

								regOpticalProbe.Close();
							}
						}

						regOpticalProbes.Close();
					}

					regRoot.Close();
				}
			}
			catch
			{
				//If we couldn't get optical probes then the list will be empty
			}
        }

        /// <summary>
        /// Gets the optical probe type from the name of the optical probe.
        /// </summary>
        /// <param name="strOpticalProbeName">The name of the optical probe to get</param>
        /// <returns>The Optical probe type as an enum</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/03/09 RCG 2.30.16        Created

        public static OpticalProbeTypes GetOpticalProbeType(string strOpticalProbeName)
        {
            OpticalProbeTypes SelectedType = OpticalProbeTypes.GENERIC_1_NO_DTR;

            switch(strOpticalProbeName)
            {
                case PROBE_SLB:
                {
                    SelectedType = OpticalProbeTypes.SCHLUMBERGER;
                    break;
                }
                case PROBE_SLB_FRANCE:
                {
                    SelectedType = OpticalProbeTypes.SCHLUMBERGER_FRANCE;
                    break;
                }
                case PROBE_SLB_SPAIN:
                {
                    SelectedType = OpticalProbeTypes.SCHLUMBERGER_SPAIN;
                    break;
                }
                case PROBE_GE:
                {
                    SelectedType = OpticalProbeTypes.GE_SMARTCOUPLER_SC1;
                    break;
                }
                case PROBE_USM_300:
                {
                    SelectedType = OpticalProbeTypes.US_MICROTEL_PM_300;
                    break;
                }
                case PROBE_USM_500:
                {
                    SelectedType = OpticalProbeTypes.US_MICROTEL_PM_500;
                    break;
                }
                case PROBE_USM_600:
                {
                    SelectedType = OpticalProbeTypes.US_MICROTEL_PM_600;
                    break;
                }
                case PROBE_GENERIC_1:
                {
                    SelectedType = OpticalProbeTypes.GENERIC_1_NO_DTR;
                    break;
                }
                case PROBE_GENERIC_2:
                {
                    SelectedType = OpticalProbeTypes.GENERIC_2_DTR;
                    break;
                }
            }

            return SelectedType;
        }

        #endregion

        #region Public Properties

        /// <summary>
		/// Returns optical probe information for the index requested
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public OpticalProbeInfo this[ int intProbe ]
		{
			get
			{
				if( intProbe < m_structOpticalProbes.Length )
				{
					return m_structOpticalProbes[ intProbe ];
				}
				else
				{
					return new OpticalProbeInfo( false, false, "" );
				}
			}
		}
		
		/// <summary>
		/// Returns the number of optical probes
		/// </summary>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  01/04/07 mrj 8.00.04		Created
		//
		public int Length
		{
			get
			{
				return m_structOpticalProbes.Length;
			}
        }

        #endregion

        #region Member Variables

        private OpticalProbeInfo[] m_structOpticalProbes = new OpticalProbeInfo[0];

        #endregion
    }
}
