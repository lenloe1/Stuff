///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//  All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//  embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//  transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//  storage or retrieval system without the permission in writing from Itron, Inc.
//
//                            Copyright © 2009 - 2014
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Quantity object for LED configuration
    /// </summary>
    public class LEDQuantity : IEquatable<LEDQuantity>
    {
        #region Definitions

        /// <summary>
        /// LED Quantity Direction
        /// </summary>
        [Flags]
        public enum DirectionType : uint
        {
            /// <summary>
            /// Mask
            /// </summary>
            Mask = 0x000000FF,
            /// <summary>
            /// Phase A Delivered
            /// </summary>
            PhaseADelivered = 0x00000001,
            /// <summary>
            /// Phase B Delivered
            /// </summary>
            PhaseBDelivered = 0x00000002,
            /// <summary>
            /// Phase C Delivered
            /// </summary>
            PhaseCDelivered = 0x00000004,
            /// <summary>
            /// Aggregate Delivered
            /// </summary>
            AggregateDelivered = 0x00000008,
            /// <summary>
            /// Phase A Received
            /// </summary>
            PhaseAReceived = 0x00000010,
            /// <summary>
            /// Phase B Received
            /// </summary>
            PhaseBReceived = 0x00000020,
            /// <summary>
            /// Phase C Received
            /// </summary>
            PhaseCReceived = 0x00000040,
            /// <summary>
            /// Aggregate Received
            /// </summary>
            AggregateReceived = 0x00000080,
            /// <summary>
            /// Aggregate Delivered and Received
            /// </summary>
            AggDeliveredAndReceived = AggregateDelivered | AggregateReceived,
        }

        /// <summary>
        /// LED Quantity Type
        /// </summary>
        public enum QuantityType : uint
        {
            /// <summary>
            /// Mask
            /// </summary>
            Mask = 0xFFFFFF00,
            /// <summary>
            /// Wh
            /// </summary>
            Wh = 0x00000100,
            /// <summary>
            /// varh
            /// </summary>
            Varh = 0x00000200,
            /// <summary>
            /// VAh Arithm
            /// </summary>
            VAhArith = 0x00000400,
            /// <summary>
            /// VAh Vec
            /// </summary>
            VAhVect = 0x00000800,
            /// <summary>
            /// VAh Lag
            /// </summary>
            VAhLag = 0x00001000,
            /// <summary>
            /// NAh
            /// </summary>
            NAh = 0x00004000,
            /// <summary>
            /// Ah
            /// </summary>
            Ah = 0x00008000,
            /// <summary>
            /// Vh
            /// </summary>
            Vh = 0x00010000,
            /// <summary>
            /// A^2h
            /// </summary>
            A2h = 0x000020000,
            /// <summary>
            /// v^2h
            /// </summary>
            V2h = 0x00040000,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="quantityID">The LED quantity ID number</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        public LEDQuantity(uint quantityID)
        {
            m_uiQuantityID = quantityID;
            DetermineDescription();
        }

        /// <summary>
        /// Determines whether or not the LED Quantities are equal.
        /// </summary>
        /// <param name="other">The LED Quantity to compare to.</param>
        /// <returns>True if the LED Quantities are equal. False otherwise.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        public bool Equals(LEDQuantity other)
        {
            return QuantityID == other.QuantityID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the LED Quantity ID.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        public uint QuantityID
        {
            get
            {
                return m_uiQuantityID;
            }
        }

        /// <summary>
        /// Gets the description of the LED quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        public string Description
        {
            get
            {
                return m_strDescription;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the description of the LED quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        private void DetermineDescription()
        {
            m_strDescription = GetQuantityDescription();
            m_strDescription += GetDirectionDescription();
        }

        /// <summary>
        /// Gets the direction portion of the quantity description
        /// </summary>
        /// <returns>The direction as a string</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created
        // 12/03/14 AF  4.00.90 550154 Shorten the aggregate delivered and received
        //                             description for VAh vec and arith
        //
        private string GetDirectionDescription()
        {
            string strDirectionDescription = "";

            switch (Direction)
            {
                case DirectionType.AggregateDelivered:
                {
                    strDirectionDescription = " d";
                    break;
                }
                case DirectionType.AggregateReceived:
                {
                    strDirectionDescription = " r";
                    break;
                }
                case DirectionType.PhaseADelivered:
                case DirectionType.PhaseAReceived:
                {
                    strDirectionDescription = " (a)";
                    break;
                }
                case DirectionType.PhaseBDelivered:
                case DirectionType.PhaseBReceived:
                {
                    strDirectionDescription = " (b)";
                    break;
                }
                case DirectionType.PhaseCDelivered:
                case DirectionType.PhaseCReceived:
                {
                    strDirectionDescription = " (c)";
                    break;
                }
                case DirectionType.AggDeliveredAndReceived:
                {
                    string strQuantity = GetQuantityDescription(true);

                    strDirectionDescription = " (" + strQuantity + " d & " + strQuantity + " r)";
                    break;
                }
            }

            return strDirectionDescription;
        }

        /// <summary>
        /// Gets the description of the quantity portion of the LED quantity
        /// </summary>
        /// <returns>The quantity as a string.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created
        // 12/03/14 AF  4.00.90 550154 Added an optional parameter so that we can shorten
        //                             VAh Vec and VAh Arith for the reconfigure LED feature
        //
        private string GetQuantityDescription(bool Shorten = false)
        {
            string strQuantityDescription = "";

            switch (Quantity)
            {
                case QuantityType.A2h:
                {
                    strQuantityDescription = "A^2h";
                    break;
                }
                case QuantityType.Ah:
                {
                    strQuantityDescription = "Ah";
                    break;
                }
                case QuantityType.NAh:
                {
                    strQuantityDescription = "NAh";
                    break;
                }
                case QuantityType.V2h:
                {
                    strQuantityDescription = "V^2h";
                    break;
                }
                case QuantityType.VAhArith:
                {
                    if (Shorten)
                    {
                        strQuantityDescription = "VAh";
                    }
                    else
                    {
                        strQuantityDescription = "VAh Arith";
                    }
                    break;
                }
                case QuantityType.VAhLag:
                {
                    if (Shorten)
                    {
                        strQuantityDescription = "VAh";
                    }
                    else
                    {
                        strQuantityDescription = "VAh lag";
                    }
                    break;
                }
                case QuantityType.VAhVect:
                {
                    if (Shorten)
                    {
                        strQuantityDescription = "VAh";
                    }
                    else
                    {
                        strQuantityDescription = "VAh Vec";
                    }
                    break;
                }
                case QuantityType.Varh:
                {
                    strQuantityDescription = "varh";
                    break;
                }
                case QuantityType.Vh:
                {
                    strQuantityDescription = "Vh";
                    break;
                }
                case QuantityType.Wh:
                {
                    strQuantityDescription = "Wh";
                    break;
                }
            }

            return strQuantityDescription;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Direction of the quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        private DirectionType Direction
        {
            get
            {
                return (DirectionType)(m_uiQuantityID & (uint)DirectionType.Mask);
            }
        }

        /// <summary>
        /// Gets the base quantity
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/08/09 RCG	2.20.04		   Created

        private QuantityType Quantity
        {
            get
            {
                return (QuantityType)(m_uiQuantityID & (uint)QuantityType.Mask);
            }
        }

        #endregion

        #region Member Variables

        private uint m_uiQuantityID;
        private string m_strDescription;

        #endregion
    }
}
