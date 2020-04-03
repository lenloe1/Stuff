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
//                              Copyright © 2005 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications.PSEM
{

	/// <summary>
	/// ResultCodes enumeration encapsulates all possible results or errors
	/// that could be returned from the PSEM application layer. However, the
	/// possible errors returned are not necessarily restricted to  
	/// the result codes defined within the ANSI application layer 7.
	/// </summary>
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	Created
	public enum PSEMResponse : byte
	{	
		/// <summary>
		/// Acknowledge—No problems, request accepted.
		/// </summary>
        [EnumDescription("Success")]
		Ok  = 0, 	
		/// <summary>
		/// Rejection of the received service request.
		/// </summary>
        [EnumDescription("Error")]
        Err,
		/// <summary>
		/// Service not supported.
		/// </summary>
        [EnumDescription("Service Not Supported")]
        Sns,		
		/// <summary>
		/// Insufficient security clearance.
		/// </summary>
        [EnumDescription("Insufficient Security Clearance")]
        Isc,		
		/// <summary>
		/// Operation not possible.
		/// </summary>
        [EnumDescription("Operation Not Possible")]
        Onp,		
		/// <summary>
		/// Inappropriate action requested.
		/// </summary>
        [EnumDescription("Inappropriate Action Requested")]
        Iar,		
		/// <summary>
		/// Device busy.
		/// </summary>
        [EnumDescription("Device Busy")]
        Bsy,		
		/// <summary>
		/// Data not ready.
		/// </summary>
        [EnumDescription("Data Not Ready")]
        Dnr,		
		/// <summary>
		/// Data locked.
		/// </summary>
        [EnumDescription("Data Locked")]
        Dlk,		
		/// <summary>
		/// Renegotiate request.
		/// </summary>
        [EnumDescription("Renegotiate Request")]
        Rno,		
		/// <summary>
		/// Invalid service sequence state.
		/// </summary>
        [EnumDescription("Invalid Service Sequence State")]
        Isss,
	}

    /// <summary>
    /// Singleton class used to keep communication statistics
    /// </summary>
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 03/24/08 rdb            	Created
    public class PSEMCommunicationsStatistics
    {

        private static PSEMCommunicationsStatistics instance;

        #region Public Methods

        /// <summary>
        /// Clear the counters
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/24/08 rdb            	Created
        public void Clear()
        {
            m_iOk = 0;
            m_iErr = 0;
            m_iSns = 0;
            m_iIsc = 0;
            m_iOnp = 0;
            m_iIar = 0;
            m_iBsy = 0;
            m_iDnr = 0;
            m_iDlk = 0;
            m_iRno = 0;
            m_iIsss = 0;
            m_iTo = 0;
            m_iAckSent = 0;
            m_iNakSent = 0;
        }//Clear

        /// <summary>
        /// Increment the counter that matches the code
        /// </summary>
        /// <param name="iCode"></param>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/24/08 rdb            	Created
        public void Increment(int iCode)
        {
            switch (iCode)
            {
                case (int)PSEMResponse.Bsy:
                    Bsy++;
                    break;
                case (int)PSEMResponse.Dlk:
                    Dlk++;
                    break;
                case (int)PSEMResponse.Dnr:
                    Dnr++;
                    break;
                case (int)PSEMResponse.Err:
                    Err++;
                    break;
                case (int)PSEMResponse.Iar:
                    Iar++;
                    break;
                case (int)PSEMResponse.Isss:
                    Isss++;
                    break;
                case (int)PSEMResponse.Isc:
                    Isc++;
                    break;
                case (int)PSEMResponse.Ok:
                    Ok++;
                    break;
                case (int)PSEMResponse.Onp:
                    Onp++;
                    break;
                case (int)PSEMResponse.Rno:
                    Rno++;
                    break;
                case (int)PSEMResponse.Sns:
                    Sns++;
                    break;
                default:
                    break;
            }
        }//Increment

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the instance of the singleton
        /// </summary>
        public static PSEMCommunicationsStatistics CommStats
        {
            get
            {
                if (instance == null)
                {
                    instance = new PSEMCommunicationsStatistics();
                }
                return instance;
            }
        }//CommStats

        /// <summary>
        /// Gets or sets number of Ok's
        /// </summary>
        public int Ok
        {
            get
            {
                return m_iOk;
            }
            set
            {
                m_iOk = value;
            }
        }//Ok

        /// <summary>
        /// Gets or sets number of Errors
        /// </summary>
        public int Err
        {
            get
            {
                return m_iErr;
            }
            set
            {
                m_iErr = value;
            }
        }//Err

        /// <summary>
        /// Gets or sets number of Service Not Supporteds
        /// </summary>
        public int Sns
        {
            get
            {
                return m_iSns;
            }
            set
            {
                m_iSns = value;
            }
        }//Sns

        /// <summary>
        /// Gets or sets number of Insufficient Security Clearances
        /// </summary>
        public int Isc
        {
            get
            {
                return m_iIsc;
            }
            set
            {
                m_iIsc = value;
            }
        }//Isc

        /// <summary>
        /// Gets or sets the number of Operation Not Possibles
        /// </summary>
        public int Onp
        {
            get
            {
                return m_iOnp;
            }
            set
            {
                m_iOnp = value;
            }
        }//Onp

        /// <summary>
        /// Gets or sets the number of Inappropriate Action Requesteds
        /// </summary>
        public int Iar
        {
            get
            {
                return m_iIar;
            }
            set
            {
                m_iIar = value;
            }
        }//Iar

        /// <summary>
        /// Gets or sets the number of Device Busies
        /// </summary>
        public int Bsy
        {
            get
            {
                return m_iBsy;
            }
            set
            {
                m_iBsy = value;
            }
        }//Bsy

        /// <summary>
        /// Gets or sets the number of Data Not Readies
        /// </summary>
        public int Dnr
        {
            get
            {
                return m_iDnr;
            }
            set
            {
                m_iDnr = value;
            }
        }//Dnr

        /// <summary>
        /// Gets or sets the number of Data Lockeds
        /// </summary>
        public int Dlk
        {
            get
            {
                return m_iDlk;
            }
            set
            {
                m_iDlk = value;
            }
        }//Dlk

        /// <summary>
        /// Gets or sets the number of Renegotiate Requests
        /// </summary>
        public int Rno
        {
            get
            {
                return m_iRno;
            }
            set
            {
                m_iRno = value;
            }
        }//Rno

        /// <summary>
        /// Gets or sets the number of Invalid Service Sequence States
        /// </summary>
        public int Isss
        {
            get
            {
                return m_iIsss;
            }
            set
            {
                m_iIsss = value;
            }
        }//Isss

        /// <summary>
        /// Gets or sets the number of Timeouts
        /// </summary>
        public int To
        {
            get
            {
                return m_iTo;
            }
            set
            {
                m_iTo = value;
            }
        }//Isss

        /// <summary>
        /// Gets or sets number of Acks
        /// </summary>
        public int AckSent
        {
            get
            {
                return m_iAckSent;
            }
            set
            {
                m_iAckSent = value;
            }
        }//Ack

        /// <summary>
        /// Gets or sets number of Nacks
        /// </summary>
        public int NakSent
        {
            get
            {
                return m_iNakSent;
            }
            set
            {
                m_iNakSent = value;
            }
        }//Nack

        /// <summary>
        /// Gets or sets number of acks received
        /// </summary>
        public int Ack
        {
            get
            {
                return m_iAck;
            }
            set
            {
                m_iAck = value;
            }
        }//Ack

        /// <summary>
        /// Gets or sets the number of naks received
        /// </summary>
        public int Nak
        {
            get
            {
                return m_iNak;
            }
            set
            {
                m_iNak = value;
            }
        }//Nak

        #endregion

        #region Private Methods

        /// <summary>
        /// Constructor
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/24/08 rdb            	Created
        private PSEMCommunicationsStatistics()
        {
            Clear();
        }//Singleton

        #endregion

        #region Private Members

        private int m_iOk;
        private int m_iErr;
        private int m_iSns;
        private int m_iIsc;
        private int m_iOnp;
        private int m_iIar;
        private int m_iBsy;
        private int m_iDnr;
        private int m_iDlk;
        private int m_iRno;
        private int m_iIsss;
        private int m_iTo;
        private int m_iAckSent;
        private int m_iNakSent;
        private int m_iAck;
        private int m_iNak;

        #endregion

    }//Singleton

}
