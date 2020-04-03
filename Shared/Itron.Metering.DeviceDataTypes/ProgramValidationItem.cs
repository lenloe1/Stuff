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
//                              Copyright © 2008
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
#if (!WindowsCE)
using System.Runtime.Serialization;
#endif

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Object used for returning validated items from the meter.
    /// </summary>
#if (!WindowsCE)
	[DataContract]
#endif
    public class ProgramValidationItem : IEquatable<ProgramValidationItem>
    {

        #region Public Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        internal ProgramValidationItem()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strCategory">The name of the category this item falls under.</param>
        /// <param name="strName">The name of the item.</param>
        /// <param name="strProgramValue">The value stored in the program.</param>
        /// <param name="strMeterValue">The value stored in the meter.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created

        public ProgramValidationItem(string strCategory, string strName, string strProgramValue, string strMeterValue)
        {
            Category = strCategory;
            Name = strName;
            ProgramValue = strProgramValue;
            MeterValue = strMeterValue;
        }

        /// <summary>
        /// Compares the two ProgramValidationItems.
        /// </summary>
        /// <param name="other">The item to compare to.</param>
        /// <returns>True if the items are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/10/09 RCG	2.20.08		   Created

        public bool Equals(ProgramValidationItem other)
        {
            return Name.Equals(other.Name);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the category name for this item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
#if (!WindowsCE)
		[DataMember]
#endif
        public string Category
        {
            get
            {
                return m_strCategory;
            }
            internal set
            {
                m_strCategory = value;
            }
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
#if (!WindowsCE)
		[DataMember]
#endif
        public string Name
        {
            get
            {
                return m_strName;
            }
            internal set
            {
                m_strName = value;
            }
        }

        /// <summary>
        /// Gets the value stored in the program
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
#if (!WindowsCE)
		[DataMember]
#endif
        public string ProgramValue
        {
            get
            {
                return m_strProgramValue;
            }
            internal set
            {
                m_strProgramValue = value;
            }
        }

        /// <summary>
        /// Gets the value stored in the meter.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/17/07 RCG	8.10.05		   Created
#if (!WindowsCE)
		[DataMember]
#endif
        public string MeterValue
        {
            get
            {
                return m_strMeterValue;
            }
            internal set
            {
                m_strMeterValue = value;
            }
        }

        #endregion

        #region Member Variables

        private string m_strCategory;
        private string m_strName;
        private string m_strProgramValue;
        private string m_strMeterValue;

        #endregion
    }
}
