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
//                              Copyright © 2011
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Itron.Metering.Zigbee
{
    /// <summary>
    /// ZCL Frame Type
    /// </summary>
    public enum ZCLFrameType : byte
    {
        /// <summary>Command acts across the entire profile</summary>
        EntireProfile = 0x00,
        /// <summary>Command is specific to a cluster</summary>
        ClusterSpecific = 0x01,
    }

    /// <summary>
    /// Direction of the ZCL Frame
    /// </summary>
    public enum ZCLDirection : byte
    {
        /// <summary>The message was sent from the client</summary>
        SentFromClient = 0x00,
        /// <summary>The message was sent from the server</summary>
        SentFromServer = 0x08,
    }

    /// <summary>
    /// ZigBee Cluster Library Frame
    /// </summary>
    public class ZCLFrame
    {
        #region Constants

        private const byte FRAME_TYPE_MASK = 0x03;
        private const byte MFG_SPECIFIC_MASK = 0x04;
        private const byte DIRECTION_MASK = 0x08;
        private const byte DISABLE_DEFAULT_RESPONSE_MASK = 0x10;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ZCLFrame()
        {
            m_FrameControl = 0;
            m_ManufacturerCode = 0;
            m_SequenceNumber = 0;
            m_CommandIdentifier = 0;
            m_Data = new byte[0];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Frame Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ZCLFrameType FrameType
        {
            get
            {
                return (ZCLFrameType)(m_FrameControl & FRAME_TYPE_MASK);
            }
            set
            {
                m_FrameControl = (byte)((m_FrameControl & ~FRAME_TYPE_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets or sets the Manufacturer Specific bit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool ManufacturerSpecific
        {
            get
            {
                return (m_FrameControl & MFG_SPECIFIC_MASK) == MFG_SPECIFIC_MASK;
            }
            set
            {
                if (value)
                {
                    m_FrameControl = (byte)(m_FrameControl | MFG_SPECIFIC_MASK);
                }
                else
                {
                    m_FrameControl = (byte)(m_FrameControl & ~MFG_SPECIFIC_MASK);
                }
            }
        }

        /// <summary>
        /// The direction of the frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public ZCLDirection Direction
        {
            get
            {
                return (ZCLDirection)(m_FrameControl & DIRECTION_MASK);
            }
            set
            {
                m_FrameControl = (byte)((m_FrameControl & ~DIRECTION_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Gets or sets whether or not the default response should be disabled.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public bool DisableDefaultResponse
        {
            get
            {
                return (m_FrameControl & DISABLE_DEFAULT_RESPONSE_MASK) == DISABLE_DEFAULT_RESPONSE_MASK;
            }
            set
            {
                if (value)
                {
                    m_FrameControl = (byte)(m_FrameControl | DISABLE_DEFAULT_RESPONSE_MASK);
                }
                else
                {
                    m_FrameControl = (byte)(m_FrameControl & ~DISABLE_DEFAULT_RESPONSE_MASK);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Manufacturer Code. This value is only used for Manufacturer Specific frames
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created

        public ushort ManufacturerCode
        {
            get
            {
                return m_ManufacturerCode;
            }
            set
            {
                m_ManufacturerCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sequence Number of the transaction
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte SequenceNumber
        {
            get
            {
                return m_SequenceNumber;
            }
            set
            {
                m_SequenceNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the Command Identifier
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte CommandID
        {
            get
            {
                return m_CommandIdentifier;
            }
            set
            {
                m_CommandIdentifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the payload data for the ZCL frame
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                if (value != null)
                {
                    m_Data = value;
                }
                else
                {
                    throw new ArgumentNullException("value", "Data value can not be null");
                }
            }
        }

        /// <summary>
        /// Gets or sets the full frame data that includes the entire message to be sent
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  08/09/11 RCG 2.52.00        Created
        
        public byte[] FrameData
        {
            get
            {
                byte[] NewFrame;
                MemoryStream FrameStream;
                BinaryWriter FrameWriter;
                int FrameLength = 3 + Data.Length;

                // Determine if we need to include the the Manufacturer Code
                if (ManufacturerSpecific)
                {
                    FrameLength += 2;
                }

                NewFrame = new byte[FrameLength];
                FrameStream = new MemoryStream(NewFrame);
                FrameWriter = new BinaryWriter(FrameStream);

                // Write the frame
                FrameWriter.Write(m_FrameControl);

                if (ManufacturerSpecific)
                {
                    FrameWriter.Write(m_ManufacturerCode);
                }

                FrameWriter.Write(m_SequenceNumber);
                FrameWriter.Write(m_CommandIdentifier);
                FrameWriter.Write(m_Data);

                return NewFrame;
            }
            set
            {
                if (value != null && value.Length >= 3)
                {
                    MemoryStream FrameStream = new MemoryStream(value);
                    BinaryReader FrameReader = new BinaryReader(FrameStream);

                    m_FrameControl = FrameReader.ReadByte();

                    if (ManufacturerSpecific)
                    {
                        m_ManufacturerCode = FrameReader.ReadUInt16();
                    }

                    m_SequenceNumber = FrameReader.ReadByte();
                    m_CommandIdentifier = FrameReader.ReadByte();

                    if (value.Length - FrameStream.Position > 0)
                    {
                        m_Data = FrameReader.ReadBytes(value.Length - (int)FrameStream.Position);
                    }
                    else
                    {
                        m_Data = new byte[0];
                    }
                }
                else
                {
                    throw new ArgumentException("The frame data can not be null and must be at least 3 bytes long");
                }
            }
        }

        #endregion

        #region Member Variables

        private byte m_FrameControl;
        private ushort m_ManufacturerCode;
        private byte m_SequenceNumber;
        private byte m_CommandIdentifier;
        private byte[] m_Data;

        #endregion
    }
}
