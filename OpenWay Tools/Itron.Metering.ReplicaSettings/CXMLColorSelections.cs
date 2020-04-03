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
//                              Copyright © 2007 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Globalization;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// This class manages the persistent storage and retrieval of the color settings for displaying time
	/// of use data.  
	/// </summary>
	public class CXMLColorSelections : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		/// <summary>
		/// We currently allow a maximum of 8 different colors for seasons, rates, and outputs
		/// The colors can be reused if more than eight rates or outputs are defined
		/// </summary>
		public const int		MAX_COLORS = 8;
		/// <summary>
		/// A maximum of two colors are defined for holiday types
		/// </summary>
		public const int		MAX_HOLIDAY_COLORS = 2;

		private const string XML_NODE_BACKCOLOR = "BackgroundColor";
		private const string XML_NODE_TEXTCOLOR = "TextColor";
		private const string XML_NODE_HOLIDAYCOLOR = "HolidayColor";

		/// <summary>
		/// Constructor - builds the path name of the settings file and creates the
		/// base object to read the data
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public CXMLColorSelections() : base()
		{
			string strFilePath = DEFAULT_SETTINGS_DIRECTORY + "SystemSettings.xml";
			
			m_XMLSettings = new CXMLSettings(strFilePath, "", "ColorSelections");

			if (null != m_XMLSettings)
			{
				m_XMLSettings.XMLFileName = strFilePath;
			}
		}

		/// <summary>
		/// Retrieves the specified background color
		/// </summary>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		public Color GetBackgroundColor(int nIndex)
		{
			if (nIndex < 0 || nIndex >= MAX_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = GetString(XML_NODE_BACKCOLOR + nIndex.ToString(CultureInfo.InvariantCulture));

			if (strColor.Length == 0)
			{
				return m_nDefaultBackgroundColorSelections[nIndex];
			}
			else
			{
                return Color.FromArgb(Int32.Parse(strColor, CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Writes the given background color to the systems settings file
		/// </summary>
		/// <param name="nIndex"></param>
		/// <param name="nColor"></param>
		public void SetBackgroundColor(int nIndex, Color  nColor )
		{
			if (nIndex < 0 || nIndex >= MAX_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = nColor.ToArgb().ToString(CultureInfo.InvariantCulture);

            SetString(XML_NODE_BACKCOLOR + nIndex.ToString(CultureInfo.InvariantCulture), strColor);
		}

		/// <summary>
		/// Retrieves the specified text color from the system settings file
		/// </summary>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		public Color GetTextColor(int nIndex)
		{
			if (nIndex < 0 || nIndex >= MAX_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = GetString(XML_NODE_TEXTCOLOR + nIndex.ToString(CultureInfo.InvariantCulture));

			if (strColor.Length == 0)
			{
				return m_nDefaultTextColorSelections[nIndex];
			}
			else
			{
                return Color.FromArgb(Int32.Parse(strColor, CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Writes the given text color to the system settings file
		/// </summary>
		/// <param name="nIndex"></param>
		/// <param name="nColor"></param>
		public void SetTextColor(int nIndex, Color nColor)
		{
			if (nIndex < 0 || nIndex >= MAX_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = nColor.ToArgb().ToString(CultureInfo.InvariantCulture);

            SetString(XML_NODE_TEXTCOLOR + nIndex.ToString(CultureInfo.InvariantCulture), strColor);
		}


		/// <summary>
		/// Retrieves the display color for the given holiday type from the systems settings file
		/// </summary>
		/// <param name="nIndex"></param>
		/// <returns></returns>
		public Color GetHolidayColor(int nIndex)
		{
			if (nIndex < 0 || nIndex >= MAX_HOLIDAY_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = GetString(XML_NODE_HOLIDAYCOLOR + nIndex.ToString(CultureInfo.InvariantCulture));

			if (strColor.Length == 0)
			{
				return m_nDefaultHolidayColorSelections[nIndex];
			}
			else
			{
                return Color.FromArgb(Int32.Parse(strColor, CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Writes the display color for the given holiday type to the systems settings file
		/// </summary>
		/// <param name="nIndex"></param>
		/// <param name="nColor"></param>
		public void SetHolidayColor(int nIndex, Color nColor)
		{
			if (nIndex < 0 || nIndex >= MAX_HOLIDAY_COLORS)
			{
				throw (new ArgumentOutOfRangeException("Invalid color index"));
			}

            String strColor = nColor.ToArgb().ToString(CultureInfo.InvariantCulture);

            SetString(XML_NODE_HOLIDAYCOLOR + nIndex.ToString(CultureInfo.InvariantCulture), strColor);
		}

	
		/// <summary>
		/// Posts the color selections to the systems settings file
		/// </summary>
		public void SaveSettings()
		{
			SaveSettings(m_XMLSettings.XMLFileName);
		}

		#region Members

		static private Color[] m_nDefaultBackgroundColorSelections = {
                            Color.PaleGoldenrod,
                            Color.LightSalmon,
                            Color.LightSkyBlue,
                            Color.LightGreen,
                            Color.LightCoral,
                            Color.LightYellow,
                            Color.LightCyan,
                            Color.LightGray };

		static private Color[] m_nDefaultHolidayColorSelections = {
                            Color.DarkBlue,
                            Color.DarkGreen};

		static private Color[] m_nDefaultTextColorSelections = {
                            Color.Black,
                            Color.Black,
                            Color.Black,
                            Color.Black,
                            Color.Black,
                            Color.Black,
                            Color.Black,
                            Color.Black };
		
		#endregion
	}
}
