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
//                              Copyright © 2005-2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Globalization;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Shared methods and member variables
	/// </summary>
	internal class CShared
	{
		//Device Type name constants
		public static readonly string METER_TYPE_CENTRON = "CENTRON";
		public static readonly string METER_TYPE_CENTRON_MONO = "IMAGE";
		public static readonly string METER_TYPE_CENTRON_POLY = "IMAGE_VI";
		public static readonly string METER_TYPE_Q1000 = "Q1000";
		public static readonly string METER_TYPE_SENTINEL = "SENTINEL";
		public static readonly string METER_TYPE_VECTRON = "VECTRON";
		//REM 05/13/05: Adding support for FULCRUM, QUANTUM, DATASTAR, and D/MT/MTR 200
		public static readonly string METER_TYPE_FULCRUM = "FULCRUM";
		public static readonly string METER_TYPE_QUANTUM = "QUANTUM";
		public static readonly string METER_TYPE_DATASTAR = "DATASTAR";
		public static readonly string METER_TYPE_DMTMTR200 = "D_MT_MTR_200";
		public static readonly string METER_TYPE_CENTRON_OPENWAY = "CENTRON_OPENWAY";

		internal const string KEY = "!4^*|zQ?90!@#$%^&*(-)1234567890=";

		#region Internal Methods

		/// <summary>
		/// Encodes the passed in string
		/// </summary>
		/// <param name="strTemp">String to Encrypt</param>
		/// <returns>Encrypted version of string</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//	02/08/08 mrj 1.00.00		Changed method to be static (OpenWay Tools)
		//  
		internal static string EncodeString(string strTemp)
		{
			string strReturn = "";
			string strEncode = EncryptPassword(strTemp);
			string strNextCharacter = "";

			for (int intPosition = 0; intPosition < strEncode.Length; intPosition++)
			{
				strNextCharacter = Convert.ToByte(strEncode[intPosition]).ToString("X", CultureInfo.InvariantCulture);


				if (1 == strNextCharacter.Length)
				{
					strReturn += '0';
				}

				strReturn += strNextCharacter;
			}

			return strReturn;
		}

		/// <summary>
		/// Decodes the passed in string
		/// </summary>
		/// <param name="strTemp">String to descrypt</param>
		/// <returns>Decrypted string</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//	02/08/08 mrj 1.00.00		Changed method to be static (OpenWay Tools)
		//  
		internal static string DecodeString(string strTemp)
		{
			string strReturn = "";
			int intNumber;

			for (int intPosition = 0; intPosition < strTemp.Length; intPosition = intPosition + 2)
			{
				//Determine the actual character number
				if (strTemp[intPosition] < 'A')
				{
					intNumber = strTemp[intPosition] - '0';
				}
				else
				{
					intNumber = strTemp[intPosition] - 'A' + 10;
				}
				intNumber *= 16;

				if (strTemp[intPosition + 1] < 'A')
				{
					intNumber += strTemp[intPosition + 1] - '0';
				}
				else
				{
					intNumber += strTemp[intPosition + 1] - 'A' + 10;
				}

				strReturn += (char)intNumber;
			}

			return EncryptPassword(strReturn);
		}

		/// <summary>
		/// Encrypts the password passed in
		/// </summary>
		/// <param name="strTemp">Password to encrypt</param>
		/// <returns>Encrypted password</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//	02/08/08 mrj 1.00.00		Changed method to be static (OpenWay Tools)
		//  
		internal static string EncryptPassword(string strTemp)
		{
			string strReturn = "";
			char cByte;

			for (int intPosition = 0; intPosition < strTemp.Length; intPosition++)
			{
				cByte = strTemp[intPosition];
				cByte ^= KEY[intPosition];
				strReturn += cByte;
			}

			return strReturn;
		}

		#endregion Internal Methods
	}
}
