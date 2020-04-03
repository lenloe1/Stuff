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
//                              Copyright © 2004 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
	/// <summary>
	/// Shared Controls global enumeration
	/// </summary>
	public enum Types
	{
		/// <summary>
		/// Float = 0
		/// </summary>
		Float,
		/// <summary>
		/// PhoneNumber = 1
		/// </summary>
		PhoneNumber
	}

	/// <summary>
	/// Summary description for FilteredTextBox.
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class FilteredTextBox : System.Windows.Forms.TextBox
	{
		/// <summary>
		/// public static string CHARACTER_DECIMAL = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
		/// </summary>
		public static string CHARACTER_DECIMAL = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
		/// <summary>
		/// public static string CHARACTER_NEGATIVE = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign; 
		/// </summary>
		public static string CHARACTER_NEGATIVE = System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign; 
		/// <summary>
		/// private const int WM_PASTE = 0x0302;
		/// </summary>
		private const int WM_PASTE = 0x0302;
		/// <summary>
		/// private const int WM_CONTEXTMENU = 0x7b;
		/// </summary>
		private const int WM_CONTEXTMENU = 0x7b;
		/// <summary>
		/// protected const string PHONE_NUMBER_SPECIAL_CHARACTERS = "*,.# -";
		/// </summary>
		protected const string PHONE_NUMBER_SPECIAL_CHARACTERS = "*,.# -";
		

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected Types m_enumType = Types.Float;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected uint m_uintMaximumCharacters = 0;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected uint m_uintMaximumDecimals = 0;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected uint m_uintMaximumLeftDecimals = 0;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected double m_dblMinimumValue = 0.0;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected double m_dblMaximumValue = 0.0;
		/// <summary>
		/// FilteredTextBox protected member variable
		/// </summary>
		protected string m_strLastValidValue = "";
        
		/// <summary>
		/// Handles windows messages
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case WM_PASTE:
				{
					if( Types.Float != m_enumType ) //If you want to allow paste
					{
						base.WndProc(ref m);
					}
					break;
				}
				case WM_CONTEXTMENU:
				{
					break;
				}
				default:
				{
					base.WndProc(ref m);
					break;
				}
			}
		}

		/// <summary>
		/// Returns a double from a string
		/// </summary>
		/// <param name="strValue">String to convert</param>
		/// <param name="dblValue">Varible to store the double in</param>
		/// <returns>Whether or not the conversion was successful</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public bool ParseDouble( string strValue, out double dblValue )
		{
			return Double.TryParse( strValue, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.CurrentInfo, out dblValue );
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public FilteredTextBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.KeyPress += new KeyPressEventHandler(HandleKeyPress); 
		}

		/// <summary>
		/// Stores the current text as the last valid value
		/// </summary>
		/// <param name="e"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected override void OnGotFocus(EventArgs e)
		{
			m_strLastValidValue = Text;
			base.OnGotFocus( e );
		}

		/// <summary>
		/// Filters keyboard input based upon type of filteredtextbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		private void HandleKeyPress(object sender, KeyPressEventArgs e) 
		{ 
			int intPosition = -1;
			int intDecimalPosition = -1;
			int intDigits = 0;
			string strNewNumber = "";
			string strChar = e.KeyChar.ToString();
			string strTemp = "";
			bool blnHandled = false;

			if( !char.IsControl( e.KeyChar ) )
			{
				if( false == char.IsDigit( e.KeyChar ) )
				{
					if( Types.Float == m_enumType )
					{
						//Check to see if the user entered a decimal
						if( 0 != CHARACTER_DECIMAL.CompareTo( strChar ) )
						{
							//The user didn't enter a decimal, so check to see if we support negative numbers
							if( 0 > m_dblMinimumValue )
							{
								//We support negative numbers, so check to see if the user entered a negative sign
								if( 0 != CHARACTER_NEGATIVE.CompareTo( strChar ) )
								{
									blnHandled = true;
								}
							}
							else
							{
								blnHandled = true;
							}
						}
					}
					else
					{
						//Check for other supported phone number characters
					}
				}
				
				if( false == blnHandled )
				{
					//Determine the potential new string
					strNewNumber = Text.Remove( this.SelectionStart, this.SelectionLength );
					strNewNumber = strNewNumber.Insert( this.SelectionStart, e.KeyChar.ToString() );

					intDigits = strNewNumber.Length;

					//Now check to see if the new string is valid
					if( Types.Float == m_enumType )
					{
						//Check for multiple decimal places and negative signs
						intDecimalPosition = strNewNumber.IndexOf( CHARACTER_DECIMAL );
						if( -1 < intDecimalPosition )
						{
							if( intDecimalPosition < strNewNumber.IndexOf( CHARACTER_DECIMAL, intDecimalPosition + 1 ) )
							{
								//A second decimal has been entered. Do not allow it
								blnHandled = true;
							}
							else
							{
								//Take away one from the number of digits since the decimal doesn't count as one
								intDigits--;
							}
						}
						else
						{
							intDecimalPosition = strNewNumber.Length;
						}

						//Check to make sure the negative sign is in the right place and doesn't show up twice
						if( ( false == blnHandled ) && ( 0 > m_dblMinimumValue ) )
						{
							intPosition = strNewNumber.IndexOf( CHARACTER_NEGATIVE );
							if( -1 < intPosition )
							{
								if( ( 0 < intPosition ) || 
									(intPosition < strNewNumber.IndexOf( CHARACTER_NEGATIVE, intPosition + 1 ) ) )
								{
									//A second negative sign has been entered. Do not allow it
									blnHandled = true;
								}
								else
								{
									//Take away one from the number of digits since the negative sign doesn't count as one
									intDigits--;
								}
							}	
						}
						
						if( false == blnHandled )
						{
							if( m_uintMaximumCharacters < intDigits )
							{
								blnHandled = true;
							}
							else
							{
								//Now check to make sure we have not exceeded the maximum number of decimal digits
								if( m_uintMaximumDecimals < ( intDigits - intDecimalPosition ) )
								{
									blnHandled = true;
								}

								//Lastly check to make sure that we have not exceeded the maximum number of digits
								//to the left of the decimal place
								if( false == blnHandled )
								{
									if( -1 < intPosition )
									{
										strTemp = strNewNumber.Substring( 1, intDecimalPosition - 1 );
										if( strTemp.Length > Convert.ToInt32( -1 * m_dblMinimumValue ).ToString().Length )
										{
											blnHandled = true;
										}
									}
									else
									{
										strTemp = strNewNumber.Substring( 0, intDecimalPosition );
										if( strTemp.Length > Convert.ToInt32( m_dblMaximumValue ).ToString().Length )
										{
											blnHandled = true;
										}
									}
								}
							}
						}
					}
					else
					{
						//We are a phone number, so make sure we don't exceed the maximum length
						if( m_uintMaximumCharacters < intDigits )
						{
							blnHandled = true;	
						}
						else
						{
							//if( m_strPhoneNumberSpecialCharacters
							//Check for digit or number
							if( !Char.IsLetterOrDigit( strChar[0] ) )
							{
								if( 0 > PHONE_NUMBER_SPECIAL_CHARACTERS.IndexOf( strChar ) )
								{
									blnHandled = true;
								}
							}
						}
					}
				}
				
				e.Handled = blnHandled; 
			}
		} 

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		/// <summary>
		/// Sets the type of Filtered Text Box. Float, Phone Number, etc.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual Types Type
		{
			get
			{
				return m_enumType;
			}
			set
			{
				m_enumType = value;
			}
		}
		
		/// <summary>
		/// Sets the Minimum Value of the textbox. Used for Float types.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual double MinimumValue
		{
			set
			{
				m_dblMinimumValue = value;
			}
		}
		/// <summary>
		/// Sets the Maximum Value of the textbox. Used for Float types.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual double MaximumValue
		{
			set
			{
				m_dblMaximumValue = value;
			}
		}
		
		/// <summary>
		/// Sets the Maximum number of actual characters. Does not include decimal place or negative symbol
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual uint MaximumCharacters
		{
			set
			{
				m_uintMaximumCharacters = value;

				if( value < m_uintMaximumDecimals )
				{
					m_uintMaximumDecimals = value;
				}
			}
		}
		/// <summary>
		/// Sets the Maximum number of decimal places. This number should not exceed the maximum number of characters.
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual uint MaximumDecimal
		{
			set
			{
				if( m_uintMaximumCharacters < value )
				{
					m_uintMaximumDecimals = m_uintMaximumCharacters;
				}
				else
				{
					m_uintMaximumDecimals = value;
				}	
			}
		}

		/// <summary>
		/// Checks to see if the value entered is a valid value
		/// </summary>
		/// <returns>Whether or not the current value is valid</returns>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual bool IsValidValue()
		{
			double dblValue;
			bool blnValid = true;

			if( Types.Float == m_enumType )
			{
				if( false != ParseDouble( Text, out dblValue ) )
				{
					if( ( m_dblMinimumValue > dblValue ) || ( m_dblMaximumValue < dblValue ) )
					{
						blnValid = false;
					}
				}
				else
				{
					blnValid = false;
				}
			}
			
			return blnValid;
		}
	}
}
