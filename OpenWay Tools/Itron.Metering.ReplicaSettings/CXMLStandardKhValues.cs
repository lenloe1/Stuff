using System;

namespace Itron.Metering.ReplicaSettings
{
	/// <summary>
	/// Standard Kh Values XML Setting class
	/// </summary>
	/// <remarks><pre>
	///Revision History
	///MM/DD/YY who Version Issue# Description
	///-------- --- ------- ------ ---------------------------------------------
	///07/29/04 REM 7.00.15 N/A    Initial Release
	///</pre></remarks>
	public class CXMLStandardKhValues : Itron.Metering.ReplicaSettings.CXMLSettingsAccess
	{
		//Constants
        /// <summary>
        /// protected const string XML_NODE_FORM_2 = "Form2";
        /// </summary>
        protected const string XML_NODE_FORM_2 = "Form2";
        /// <summary>
        /// protected const strong XML_NODE_FORM_2_CL320 = "Form2CL320";
        /// </summary>
        protected const string XML_NODE_FORM_2_CL320 = "Form2CL320";
        /// <summary>
        /// protected const string XML_NODE_FORM_3 = "Form3";
        /// </summary>
        protected const string XML_NODE_FORM_3 = "Form3";
        /// <summary>
        /// protected const string XML_NODE_FORM_4 = "Form4";
        /// </summary>
        protected const string XML_NODE_FORM_4 = "Form4";
        /// <summary>
        /// protected const string XML_NODE_FORM_9 = "Form9";
        /// </summary>
        protected const string XML_NODE_FORM_9 = "Form9";
        /// <summary>
        /// protected const string XML_NODE_FORM_9_36 = "Form9_36";
        /// </summary>
        protected const string XML_NODE_FORM_9_36 = "Form9_36";
        /// <summary>
        /// protected const string XML_NODE_FORM_10 = "Form10";
        /// </summary>
        protected const string XML_NODE_FORM_10 = "Form10";
        /// <summary>
        /// protected const string XML_NODE_FORM_12 = "Form12";
        /// </summary>
        protected const string XML_NODE_FORM_12 = "Form12";
        /// <summary>
        /// protected const strong XML_NODE_FORM_12_CL320 = "Form12CL320";
        /// </summary>
        protected const string XML_NODE_FORM_12_CL320 = "Form12CL320";
        /// <summary>
        /// protected const string XML_NODE_FORM_16 = "Form16";
        /// </summary>
        protected const string XML_NODE_FORM_16 = "Form16";
        /// <summary>
        /// protected const string XML_NODE_FORM_16_CL320 = "Form16CL320";
        /// </summary>
        protected const string XML_NODE_FORM_16_CL320 = "Form16CL320";
        /// <summary>
        /// protectec const string XML_NODE_FORM_36 = "Form36";
        /// </summary>
        protected const string XML_NODE_FORM_36 = "Form36";
        /// <summary>
        /// protected const string XML_NODE_FORM_45 = "Form45";
        /// </summary>
        protected const string XML_NODE_FORM_45 = "Form45";
        /// <summary>
        /// protected const string XML_NODE_FORM_46 = "Form46";
        /// </summary>
        protected const string XML_NODE_FORM_46 = "Form46";
        /// <summary>
        /// protected const string XML_NODE_FORM_48 = "Form48";
        /// </summary>
        protected const string XML_NODE_FORM_48 = "Form48";
        /// <summary>
        /// protected const string XML_NODE_FORM_56 = "Form56";
        /// </summary>
        protected const string XML_NODE_FORM_56 = "Form56";
        /// <summary>
        /// protected const string XML_NODE_FORM_66 = "Form66";
        /// </summary>
        protected const string XML_NODE_FORM_66 = "Form66";
        /// <summary>
        /// protected const string XML_NODE_FORM_OTHER = "FormOther";
        /// </summary>
        protected const string XML_NODE_FORM_OTHER = "FormOther";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="XMLSettings">CXMLSettings</param>
		public CXMLStandardKhValues( CXMLSettings XMLSettings )
		{
			m_XMLSettings = XMLSettings;
		}

		/// <summary>
		/// Form 2 Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form2
		{
			get
			{
				return GetFloat( XML_NODE_FORM_2 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_2, value );
			}
		}

        /// <summary>
        /// Form 2 Standard Kh Value (Class 320)
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///02/12/07 KRC 8.00.11         Adding Standard Kh for Class 320
        ///</pre></remarks>
        public virtual float Form2CL320
        {
            get
            {
                float fltValue;
                // If the value cannot be found it will be returned as 0
                fltValue = GetFloat(XML_NODE_FORM_2_CL320);

                if (0 == fltValue)
                {
                    // The Class 320 value could not be found, so just get the regular value.
                    fltValue = GetFloat(XML_NODE_FORM_2);
                }

                return fltValue;
            }
            set
            {
                SetFloat(XML_NODE_FORM_2_CL320, value);
            }
        }

		/// <summary>
		/// Form 3 Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form3
		{
			get
			{
				return GetFloat( XML_NODE_FORM_3 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_3, value );
			}
		}

		/// <summary>
		/// Form 4 Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form4
		{
			get
			{
				return GetFloat( XML_NODE_FORM_4 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_4, value );
			}
		}

		/// <summary>
		/// Form 9(8) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form9
		{
			get
			{
				return GetFloat( XML_NODE_FORM_9 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_9, value );
			}
		}

        /// <summary>
        /// Form 9(36) Standard Kh Value
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///09/17/07 KRC 9.00.09 N/A    Adding Form 36
        ///</pre></remarks>
        public virtual float Form9_36
        {
            get
            {
                float fltValue;
                // If the value cannot be found it will be returned as 0
                fltValue = GetFloat(XML_NODE_FORM_9_36);

                if (0 == fltValue)
                {
                    // The Form 9/36S value could not be found, so just get the Form 9S value.
                    fltValue = GetFloat(XML_NODE_FORM_9);
                }

                return fltValue;
            }
            set
            {
                SetFloat(XML_NODE_FORM_9_36, value);
            }
        }
        
        /// <summary>
        /// Form 9(36) Standard Kh Value
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///04/15/08 KRC 9.02.06 N/A    Adding Form 36
        ///</pre></remarks>
        public virtual float Form36
        {
            get
            {
                float fltValue;
                // If the value cannot be found it will be returned as 0
                fltValue = GetFloat(XML_NODE_FORM_36);

                if (0 == fltValue)
                {
                    // The Form 36S value could not be found, so just get the Form 9S Value.
                    fltValue = GetFloat(XML_NODE_FORM_9);
                }

                return fltValue;
            }
            set
            {
                SetFloat(XML_NODE_FORM_36, value);
            }
        }

		/// <summary>
		/// Form 10(9) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form10
		{
			get
			{
				return GetFloat( XML_NODE_FORM_10 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_10, value );
			}
		}

		/// <summary>
		/// Form 12 Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form12
		{
			get
			{
				return GetFloat( XML_NODE_FORM_12 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_12, value );
			}
		}

        /// <summary>
        /// Form 12 Standard Kh Value (Class 320)
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///02/12/07 KRC 8.00.11         Adding Class 320 Standard Kh
        ///</pre></remarks>
        public virtual float Form12CL320
        {
            get
            {
                float fltValue;
                // If the value cannot be found it will be returned as 0
                fltValue = GetFloat(XML_NODE_FORM_12_CL320);

                if (0 == fltValue)
                {
                    // The Class 320 value could not be found, so just get the regular value.
                    fltValue = GetFloat(XML_NODE_FORM_12);
                }

                return fltValue;
            }
            set
            {
                SetFloat(XML_NODE_FORM_12_CL320, value);
            }
        }

		/// <summary>
		/// Form 16( 14, 15, 16 ) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form16
		{
			get
			{
				return GetFloat( XML_NODE_FORM_16 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_16, value );
			}
		}

        /// <summary>
        /// Form 16 (14, 15, 16 ) Standard Kh Value (Class 320)
        /// </summary>
        /// <remarks><pre>
        ///Revision History
        ///MM/DD/YY who Version Issue# Description
        ///-------- --- ------- ------ ---------------------------------------------
        ///02/12/07 KRC 8.00.11         Adding Class 320 Standard Kh
        ///</pre></remarks>
        public virtual float Form16CL320
        {
            get
            {
                float fltValue;
                // If the value cannot be found it will be returned as 0
                fltValue = GetFloat(XML_NODE_FORM_16_CL320);

                if (0 == fltValue)
                {
                    // The Class 320 value could not be found, so just get the regular value.
                    fltValue = GetFloat(XML_NODE_FORM_16);
                }

                return fltValue;
            }
            set
            {
                SetFloat(XML_NODE_FORM_16_CL320, value);
            }
        }

		/// <summary>
		/// Form 45(5) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form45
		{
			get
			{
				return GetFloat( XML_NODE_FORM_45 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_45, value );
			}
		}

		/// <summary>
		/// Form 46(6) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form46
		{
			get
			{
				return GetFloat( XML_NODE_FORM_46 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_46, value );
			}
		}

		/// <summary>
		/// Form 48 Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form48
		{
			get
			{
				return GetFloat( XML_NODE_FORM_48 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_48, value );
			}
		}

		/// <summary>
		/// Form 56(26) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form56
		{
			get
			{
				return GetFloat( XML_NODE_FORM_56 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_56, value );
			}
		}

		/// <summary>
		/// Form 66(26) Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float Form66
		{
			get
			{
				return GetFloat( XML_NODE_FORM_66 );
			}
			set
			{
				SetFloat( XML_NODE_FORM_66, value );
			}
		}

		/// <summary>
		/// Form Other Forms Standard Kh Value
		/// </summary>
		/// <remarks><pre>
		///Revision History
		///MM/DD/YY who Version Issue# Description
		///-------- --- ------- ------ ---------------------------------------------
		///07/29/04 REM 7.00.15 N/A    Initial Release
		///</pre></remarks>
		public virtual float FormOther
		{
			get
			{
				return GetFloat( XML_NODE_FORM_OTHER );
			}
			set
			{
				SetFloat( XML_NODE_FORM_OTHER, value );
			}
		}
	}
}
