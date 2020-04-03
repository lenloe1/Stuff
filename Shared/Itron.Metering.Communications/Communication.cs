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
using Itron.Metering.Utilities;

namespace Itron.Metering.Communications
{
    #region "CommPortException class"

    /// <summary>
    /// Exception encapsulating all non-recoverable communication port
    /// failures during the open, read, write and close methods. 
    /// </summary>
    /// <example>
    /// <code>
    /// try{...}
    /// catch(CommPortException e){...}
    /// </code>
    /// </example>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/01/05 bdm 7.13.00 N/A	Created
    public class CommPortException : Exception
    {

        /// <summary>
        /// Constructor to create a CommPortException.
        /// </summary>
        /// <example>
        /// <code>
        /// CommPortException CPE = new CommPortException();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public CommPortException() : base() { }


        /// <summary>
        /// Constructor to create a CommPortException.
        /// </summary>
        /// <param name="description">
        /// Description of the CommPortException.
        /// </param>/>
        /// <example>
        /// <code>
        /// CommPortException CPE = new CommPortException("Port error.");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public CommPortException(string description) : base(description) { }


        /// <summary>
        /// Constructor to create a CommPortException.
        /// </summary>
        /// <param name="description">
        /// Description of the CommPortException.
        /// </param>/>
        /// <param name="systemException">
        /// The System.Exception caught to be nested into
        /// the CommPortException exception.
        /// </param>/>		
        /// <example>
        /// <code>
        /// try{...}
        /// catch(Exception e)
        /// {
        ///		throw(new CommPortException("Port error.", e));
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public CommPortException(string description, System.Exception systemException) : base(description, systemException) { }
    }

    #endregion

    #region "TimeOutException"
    /// <summary>
    /// Exception occurs when a communication timeout occurs with a device.
    /// </summary>
    /// <example>
    /// <code>
    /// try{...}
    /// catch(TimeOutException e){...}
    /// </code>
    /// </example>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 08/01/05 bdm 7.13.00 N/A	Created
    public class TimeOutException : Exception
    {

        /// <summary>
        /// Constructor to create a TimeOutException.
        /// </summary>
        /// <example>
        /// <code>
        /// TimeOutException TOE = new TimeOutException();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public TimeOutException() : base() { }


        /// <summary>
        /// Constructor to create a TimeOutException.
        /// </summary>
        /// <param name="description">
        /// Description of the TimeOutException.
        /// </param>
        /// <example>
        /// <code>
        /// TimeOutException TOE = new TimeOutException("Data response timeout.");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public TimeOutException(string description) : base(description) { }


        /// <summary>
        /// Constructor to create a TimeOutException.
        /// </summary>
        /// <param name="description">
        /// Description of the TimeOutException.
        /// </param>/>
        /// <param name="systemException">
        /// The System.Exception caught to be nested into
        /// the TimeOutException exception.
        /// </param>/>		
        /// <example>
        /// <code>
        /// try{...}
        /// catch(Exception e)
        /// {
        ///		throw(new TimeOutException("Data response timeout", e));
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        public TimeOutException(string description,
            System.Exception systemException)
            : base(description, systemException) { }

    }
    #endregion

    #region "CommLineDroppedException"

    /// <summary>
    /// Exception created to handle a case when the communication line is dropped and
    /// a timeout exception isn't the best way to handle it
    /// </summary>
    public class CommLineDroppedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/18/14 AF  3.70.04 WR 529537 Created
        //
        public CommLineDroppedException() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">The description of the exception</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/18/14 AF  3.70.04 WR 529537 Created
        //
        public CommLineDroppedException(string description) : base(description) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">The description of the exception</param>
        /// <param name="systemException">The System.Exception caught to be nested into
        /// the CommLineDroppedException exception.</param>
        //  Revision History	
        //  MM/DD/YY Who Version ID Issue# Description
        //  -------- --- ------- -- ------ -------------------------------------------
        //  08/18/14 AF  3.70.04 WR 529537 Created
        //
        public CommLineDroppedException(string description, System.Exception systemException) 
            : base(description, systemException) { }
    }

    #endregion

    /// <summary>
    /// Delegate to support raising a communication event.
    /// </summary>
    public delegate void CommEvent();

    /// <summary>
    /// Delegate to support raising a communication event with data.
    /// </summary>
    public delegate void CommEventData(byte[] data);

    /// <summary>
    /// Delegate to support raising a communication change event 
    /// with state. 
    /// </summary>
    public delegate void CommChangeEvent(bool newState);

    /// <summary>
    /// Delegate to support raising a communication error event 
    /// with description.
    /// </summary>
    public delegate void CommErrorEvent(string description);

    /// <summary>
    /// Communication class - This is the base class of all communication objects.
    /// </summary>    
    public interface ICommunications
    {
        #region public delegates and events

        /// <summary>
        /// Event raised when communication port input buffer has been read. 
        /// </summary>
        event CommEventData DataReceived;

        /// <summary>
        /// Event raised when communication port output buffer has sent data.
        /// </summary>
        event CommEventData DataSent;

        /// <summary>
        /// Event raised when the communication port receive buffer is overrun.
        /// </summary>
        event CommEvent RxOverrun;

        /// <summary>
        /// Event raised when the communication port character receive flag is 
        /// set.
        /// </summary>
        event CommEvent FlagCharReceived;

        #endregion

        #region public methods

        /// <summary>
        /// Opens the port passed in as a parameter.
        /// </summary>
        /// <param name="portName">
        /// The communication port to open.
        /// </param>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        void OpenPort(string portName);

        /// <summary>
        /// Closes the communication port. 
        /// </summary>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// comm.ClosePort();
        /// comm.Dispose();
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        void ClosePort();

        /// <summary>
        /// Method to send data out of the open port. 
        /// </summary>
        /// <param name="data">
        /// The data to send over the communication port.
        /// </param>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// byte[] bytData = new byte[5]{0x01, 0x02, 0x03, 0x04, 0x05};
        /// comm.Send(bytData);
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        void Send(byte[] data);
        
        /// <summary>
        /// Method to read data from the communication port into the 
        /// input buffer. 
        /// </summary>
        /// <param name="bytesToRead">
        /// Number of bytes to read. If bytesToRead equals 0, all bytes 
        /// in input buffer are read.
        /// </param>
        /// <param name="iTimeout">
        /// Unused parameter, need for desktop implementation
        /// </param>
        /// <returns>Returns number of bytes read from the communication
        /// port and stored into the input buffer.
        /// </returns>
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        int Read(uint bytesToRead, int iTimeout);
        
        #endregion

        #region public properties

        /// <summary>
        /// Whether or not the communication port is open.
        /// </summary>
        /// <returns>
        /// Boolean indicating whether or not the communication port is open.
        /// </returns>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        bool IsOpen
        {
            get;            
        }
        
        /// <summary>
        /// Property to retrieve the bytes read from the communication port 
        /// input buffer.
        /// </summary>
        /// <returns>Returns a byte[] of the data.</returns>		
        /// <exception cref="CommPortException">
        /// Thrown when a port failure occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        byte[] Input
        {
            get;            
        }

        /// <summary>
        /// Property that gets or sets the input buffer length.
        /// </summary>
        /// <returns>Returns the number of bytes in the input buffer.</returns>		
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// comm.OpenPort("COM4:");
        /// if ( 0 != comm.Read(0) )
        /// {
        ///		byte[] inputBuffer = new byte[comm.InputLen];
        ///		Array.Copy(comm.Input, 0, inputBuffer, 0, inputBuffer.Length);
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 08/01/05 bdm 7.13.00 N/A	Created
        uint InputLen
        {
            get;            
            set;            
        }

        /// <summary>
        /// Property that gets or sets the baud rate.  The baud rate can only be
        /// set to a port that is not opened.
        /// </summary>
        /// <returns>
        /// The baud rate (uint).
        /// </returns>
        /// <exception cref="CommPortException">
        /// Thrown if the port is already open.
        /// </exception>
        /// <example>
        /// <code>
        /// Communication comm = new Communication();
        /// if ( false == comm.IsOpen() )
        /// {
        ///		comm.BaudRate = 9600;
        ///		comm.OpenPort("COM4:");
        ///	}
        /// </code>
        /// </example>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	03/31/06 mrj 7.30.00 N/A    Created
        /// 
        uint BaudRate
        {
            get;            
            set;            
        }

        /// <summary>
        /// Property that gets or sets the Optical Probe Type
        /// </summary>
        /// <returns>
        /// The Optical Probe Type
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/17/06 KRC 7.35.00 N/A    Created
        /// 
        OpticalProbeTypes OpticalProbe
        {
            get;
            set;
        }

        /// <summary>
        /// Property that gets the current port name
        /// </summary>
        /// <returns>
        /// The current port name
        /// </returns>        
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        ///	08/29/06 mrj 7.35.00 N/A    Created
        /// 
        string PortName
        {
            get;            
        }

        /// <summary>
        /// Gets the Max Supported Packet Size supported by the transport protocol
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/13/12 RCG 2.70.36 N/A    Created
        
        ushort MaxSupportedPacketSize
        {
            get;
        }

        #endregion
    }
}
