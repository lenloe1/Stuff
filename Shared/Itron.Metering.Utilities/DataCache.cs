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
//                              Copyright © 2006
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// This is the base class of all cached objects.   The class provides a common mechanism 
	/// to determine if a data item has been uploaded from a meter and cached.  It also provides 
	/// a common means to flush the cached data value and force a fresh upload of the item from 
	/// the meter.
    /// </summary>
    /// <remarks>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 mh 7.30.00    N/A Created
    /// </remarks>
    public abstract class DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        /// </remarks>
        public DataCache()
        {
            m_boolCached = false;
        }

        /// <summary>
        /// This method clears the cache.
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        /// </remarks>
        public virtual void Flush()
        {
            m_boolCached = false;
        }

        /// <summary>This property determines whether or not the data is 
        /// cached.</summary>
        /// <returns>
        /// An boolean representing whether or not the data is cached.
        /// </returns>
        /// <remarks >
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///	</remarks>
        public bool Cached
        {
            get
            {
                return m_boolCached;
            }
        }

        /// <summary>
        /// A boolean indicating whether or not the data is cached.
        /// </summary>
        protected bool m_boolCached;
    }

    /// <summary>
    /// The CachedString class represents a string and whether or not it 
    /// has been cached.
    /// </summary>
    /// <remarks>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 mh 7.30.00    N/A Created
    /// </remarks>
    public class CachedString : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public CachedString()
            : base()
        {
            m_sValue = "";
        }

        /// <summary>This property gets and sets the string value.</summary>
        /// <returns>
        /// An string representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public String Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached string value that was not initialized");
                }

                return m_sValue;
            }
            set
            {
                m_boolCached = true;
                m_sValue = value;
            }
        }

        private String m_sValue;
    }

    /// <summary>
    /// The CachedInt class represents an integer and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 mh 7.30.00    N/A Created
    ///
    public class CachedInt : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public CachedInt()
            : base()
        {
            m_nValue = 0;
        }

        /// <summary>This property gets and sets the integer value.</summary>
        /// <returns>
        /// An integer representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public int Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached int value that was not initialized");
                }

                return m_nValue;
            }
            set
            {
                m_boolCached = true;
                m_nValue = value;
            }
        }

        private int m_nValue;
    }

    /// <summary>
    /// The CachedFloat class represents a float and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 mh 7.30.00    N/A Created
    ///
    public class CachedFloat : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public CachedFloat()
            : base()
        {
            m_fValue = 0.0f;
        }

        /// <summary>This property gets and sets the float value.</summary>
        /// <returns>
        /// A float representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public float Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached floating point value that was not initialized");
                }

                return m_fValue;
            }
            set
            {
                m_boolCached = true;
                m_fValue = value;
            }
        }

        private float m_fValue;
    }

    /// <summary>
    /// The CachedFloat class represents a float and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 09/28/06 KRC 7.35.00    N/A Created
    ///
    public class CachedDouble : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/28/06 KRC 7.35.00  N/A   Created
        ///
        public CachedDouble()
            : base()
        {
            m_dblValue = 0.0f;
        }

        /// <summary>This property gets and sets the double value.</summary>
        /// <returns>
        /// A double representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/28/06 KRC 7.35.00  N/A   Created
        ///
        public double Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached double value that was not initialized");
                }

                return m_dblValue;
            }
            set
            {
                m_boolCached = true;
                m_dblValue = value;
            }
        }

        private double m_dblValue;
    }

    /// <summary>
    /// The CachedBool class represents a boolean and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 06/06/06 mh 7.30.00    N/A Created
    ///
    public class CachedBool : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        ///
        public CachedBool()
            : base()
        {
            m_bValue = false;
        }

        /// <summary>This property gets and sets the boolean value.</summary>
        /// <returns>
        /// A boolean representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been initialized.
        /// </exception>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        /// </remarks>
        public bool Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached boolean value that was not initialized");
                }

                return m_bValue;
            }
            set
            {
                m_boolCached = true;
                m_bValue = value;
            }
        }

        private bool m_bValue;
    }

    /// <summary>
    /// The CachedByte class represents a byte and whether or not it 
    /// has been cached.
    /// </summary>
    /// <remarks >
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 07/04/06 mcm 7.30.00    N/A Created
    /// </remarks>
    public class CachedByte : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 06/06/06 mh 7.30.00  N/A   Created
        /// </remarks>
        public CachedByte()
            : base()
        {
            m_Value = 0;
        }

        /// <summary>This property gets and sets the value.</summary>
        /// <returns>
        /// A byte representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 07/04/06 mcm 7.30.00  N/A   Created
        ///
        public byte Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached byte value that was not initialized");
                }

                return m_Value;
            }
            set
            {
                m_boolCached = true;
                m_Value = value;
            }
        }

        private byte m_Value;
    }

    /// <summary>
    /// The CachedDate class represents a date and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/18/06 KRC 7.35.00    N/A Created
    ///
    public class CachedDate : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 KRC 7.35.00  N/A   Created
        ///
        public CachedDate()
            : base()
        {
            m_dtValue = new DateTime(1970, 1, 1, 0, 0, 0);
        }

        /// <summary>This property gets and sets the Date value.</summary>
        /// <returns>
        /// A date representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/18/06 KRC 7.35.00  N/A   Created
        ///
        public DateTime Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached Date/Time point value that was not initialized");
                }

                return m_dtValue;
            }
            set
            {
                m_boolCached = true;
                m_dtValue = value;
            }
        }

        private DateTime m_dtValue;
    }

    /// <summary>
    /// The CachedUint class represents an unsigned nteger and whether or not it 
    /// has been cached.
    /// </summary>
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/30/06 KRC 7.35.00    N/A Created
    ///
    public class CachedUint : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/30/06 KRC 7.35.00  N/A   Created
        ///
        public CachedUint()
            : base()
        {
            m_uiValue = 0;
        }

        /// <summary>This property gets and sets the unsigned integer value.</summary>
        /// <returns>
        /// An unsigned integer representing the data value is returned.
        /// </returns>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/30/06 KRC 7.35.00  N/A   Created
        ///
        public uint Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached uint value that was not initialized");
                }

                return m_uiValue;
            }
            set
            {
                m_boolCached = true;
                m_uiValue = value;
            }
        }

        private uint m_uiValue;
    }

    /// <summary>
    /// The CachedUshort class represents an unsigned short and whether or not
    /// it has been cached
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 04/20/07 RCG 8.01.01    N/A Created

    public class CachedUshort : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/20/07 RCG 8.01.01    N/A Created

        public CachedUshort()
            : base()
        {
            m_usValue = 0;
        }

        /// <summary>
        /// Gets or sets the cached ushort
        /// </summary>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 04/20/07 RCG 8.01.01    N/A Created

        public ushort Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached uint value that was not initialized");
                }

                return m_usValue;
            }
            set
            {
                m_boolCached = true;
                m_usValue = value;
            }
        }

        private ushort m_usValue;
    }

    /// <summary>
    /// The CachedLong class represents an long and whether or not
    /// it has been cached
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 06/04/08 KRC  1.50.31    N/A Created

    public class CachedLong : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/04/08 KRC  1.50.31    N/A Created

        public CachedLong()
            : base()
        {
            m_lValue = 0;
        }

        /// <summary>
        /// Gets or sets the cached long
        /// </summary>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 06/04/08 KRC  1.50.31    N/A Created

        public long Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached long value that was not initialized");
                }

                return m_lValue;
            }
            set
            {
                m_boolCached = true;
                m_lValue = value;
            }
        }

        private long m_lValue;
    }

    /// <summary>
    /// The CachedTimeSpan class represents a TimeSpan and whether or not
    /// it has been cached
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 02/22/08 KRC 10.00

    public class CachedTimeSpan : DataCache
    {
        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/22/08 KRC 10.00

        public CachedTimeSpan()
            : base()
        {
            m_TSValue = new TimeSpan();
        }

        /// <summary>
        /// Gets or sets the cached TimeSpan
        /// </summary>
        ///<exception cref="ApplicationException">
        /// Thrown if the value has not been cached.
        /// </exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/22/08 KRC 10.00

        public TimeSpan Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
					throw new InvalidOperationException("Attempt to retrieve a cached TimeSpan value that was not initialized");
                }

                return m_TSValue;
            }
            set
            {
                m_boolCached = true;
                m_TSValue = value;
            }
        }

        private TimeSpan m_TSValue;
    }

    /// <summary>
    /// The CachedValue class represents a generic value and whether or not it 
    /// has been cached.
    /// </summary>
    // Revision History	
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 10/20/14 jrf 4.00.77 539709 Created
    public class CachedValue<T> : DataCache
    {
        #region Public Methods
        
        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/20/14 jrf 4.00.77 539709 Created
        public CachedValue()
            : base()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>This property gets and sets the value.</summary>
        /// <returns>
        /// A type T representing the data value is returned.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/20/14 jrf 4.00.77 539709 Created
        public T Value
        {
            get
            {
                // Throws an exception if the value has not been cached
                if (!Cached)
                {
                    throw new InvalidOperationException("Attempt to retrieve a cached value that was not initialized!");
                }

                return m_Value;
            }
            set
            {
                m_boolCached = true;
                m_Value = value;
            }
        }

        #endregion

        #region Members

        private T m_Value;

        #endregion
    }
}