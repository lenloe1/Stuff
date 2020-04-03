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
using System.Runtime.InteropServices;

namespace Itron.Metering.Communications
{

	/// <summary>
	/// PortAPI contains the classes, structures, enumerations and interfaces
	/// to support comm port communications on the compact framework.  
	/// All code within PortAPI is sample code off the web and used as is. 
	/// </summary>
	/// <remarks>Although the PortAPI code has links to a desktop libraries,
	/// a desktop implementation has NOT been tested in any way.
	/// </remarks>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	PortAPI is sample code off the web and
	///                             used as is.
	
	[StructLayout(LayoutKind.Sequential)]
	internal class CommTimeouts 
	{
		//Port timeouts.
		public UInt32 ReadIntervalTimeout;
		public UInt32 ReadTotalTimeoutMultiplier;
		public UInt32 ReadTotalTimeoutConstant;
		public UInt32 WriteTotalTimeoutMultiplier;
		public UInt32 WriteTotalTimeoutConstant;
	}

	#region API structs and enums
	[Flags]
	internal enum CommEventFlags : uint
	{
		NONE        = 0x0000, //
		RXCHAR      = 0x0001, // Any Character received
		RXFLAG      = 0x0002, // Received specified flag character
		TXEMPTY     = 0x0004, // Tx buffer Empty
		CTS         = 0x0008, // CTS changed
		DSR         = 0x0010, // DSR changed
		RLSD        = 0x0020, // RLSD changed
		BREAK       = 0x0040, // BREAK received
		ERR         = 0x0080, // Line status error
		RING        = 0x0100, // ring detected
		PERR        = 0x0200, // printer error
		RX80FULL    = 0x0400, // rx buffer is at 80%
		EVENT1      = 0x0800, // provider event
		EVENT2      = 0x1000, // provider event
		POWER       = 0x2000, // win ce power notification
		ALL			= 0x1FFF,
		ALL_CE		= 0x3FFF  // mask of all flags 
	}

	internal enum EventFlags
	{
		EVENT_PULSE     = 1,
		EVENT_RESET     = 2,
		EVENT_SET       = 3
	}

	[Flags]
	internal enum CommErrorFlags : uint
	{
		RXOVER = 0x0001,
		OVERRUN = 0x0002,
		RXPARITY = 0x0004,
		FRAME = 0x0008,
		BREAK = 0x0010,
		TXFULL = 0x0100,
		PTO = 0x0200,
		IOE = 0x0400,
		DNS = 0x0800,
		OOP = 0x1000,
		MODE = 0x8000
	}

	[Flags]
	internal enum CommModemStatusFlags : uint
	{		
		MS_CTS_ON	= 0x0010,	// The CTS (Clear To Send) signal is on. 
		MS_DSR_ON	= 0x0020,	// The DSR (Data Set Ready) signal is on. 
		MS_RING_ON	= 0x0040,	// The ring indicator signal is on. 
		MS_RLSD_ON	= 0x0080	// The RLSD (Receive Line Signal Detect) signal is on. 
	}

    [Flags]
    internal enum PurgeCommFlags : uint
    {
        PURGE_TXABORT = 0x0001,  // Kill the pending/current writes to the comm port.
        PURGE_RXABORT = 0x0002,  // Kill the pending/current reads to the comm port.
        PURGE_TXCLEAR = 0x0004,  // Kill the transmit queue if there.
        PURGE_RXCLEAR = 0x0008   // Kill the typeahead buffer if there.
    }

	internal enum CommEscapes : uint
	{
		SETXOFF		= 1,
		SETXON		= 2,
		SETRTS		= 3,
		CLRRTS		= 4,
		SETDTR		= 5,
		CLRDTR		= 6,
		RESETDEV	= 7,
		SETBREAK	= 8,
		CLRBREAK	= 9
	}

	internal enum APIErrors : int
	{
		ERROR_FILE_NOT_FOUND	= 2,
		ERROR_INVALID_NAME		= 123,
		ERROR_ACCESS_DENIED		= 5,
		ERROR_INVALID_HANDLE	= 6,
		ERROR_IO_PENDING		= 997
	}

	internal enum APIConstants : uint
	{
		WAIT_OBJECT_0   	= 0x00000000,
		WAIT_ABANDONED  	= 0x00000080,
		WAIT_ABANDONED_0	= 0x00000080,
		WAIT_FAILED         = 0xffffffff,
		INFINITE            = 0xffffffff	
	}
	#endregion

	[StructLayout(LayoutKind.Sequential)]
	internal class CommStat 
	{
		private UInt32 bitfield;
		public UInt32 cbInQue	= 0;
		public UInt32 cbOutQue	= 0;

		// Helper constants for manipulating the bit fields.
		private readonly UInt32 fCtsHoldMask    = 0x00000001;
		private readonly Int32 fCtsHoldShift    = 0;
		private readonly UInt32 fDsrHoldMask    = 0x00000002;
		private readonly Int32 fDsrHoldShift    = 1;
		private readonly UInt32 fRlsdHoldMask   = 0x00000004;
		private readonly Int32 fRlsdHoldShift   = 2;
		private readonly UInt32 fXoffHoldMask   = 0x00000008;
		private readonly Int32 fXoffHoldShift   = 3;
		private readonly UInt32 fXoffSentMask   = 0x00000010;
		private readonly Int32 fXoffSentShift   = 4;
		private readonly UInt32 fEofMask        = 0x00000020;
		private readonly Int32 fEofShift        = 5;        
		private readonly UInt32 fTximMask       = 0x00000040;
		private readonly Int32 fTximShift       = 6;

		public bool fCtsHold 
		{
			get { return ((bitfield & fCtsHoldMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fCtsHoldShift); }
		}
		public bool fDsrHold 
		{
			get { return ((bitfield & fDsrHoldMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fDsrHoldShift); }
		}
		public bool fRlsdHold 
		{
			get { return ((bitfield & fRlsdHoldMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fRlsdHoldShift); }
		}
		public bool fXoffHold 
		{
			get { return ((bitfield & fXoffHoldMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fXoffHoldShift); }
		}
		public bool fXoffSent 
		{
			get { return ((bitfield & fXoffSentMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fXoffSentShift); }
		}
		public bool fEof 
		{
			get { return ((bitfield & fEofMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fEofShift); }
		}
		public bool fTxim 
		{
			get { return ((bitfield & fTximMask) != 0); }
			set { bitfield |= (Convert.ToUInt32(value) << fTximShift); }
		}
	}

	internal class PortAPI
	{
	
		#region API Constants
		
		internal const Int32 INVALID_HANDLE_VALUE = -1;
		internal const UInt32 OPEN_EXISTING = 3;
		internal const UInt32 GENERIC_READ = 0x80000000;
		internal const UInt32 GENERIC_WRITE = 0x40000000;
		
		#endregion

		#region Helper methods

		internal static bool FullFramework
		{
			get{return Environment.OSVersion.Platform != PlatformID.WinCE;}
		}
		
		#endregion

		#region Windows CE API imports

		[DllImport("coredll.dll", EntryPoint="WaitForSingleObject", SetLastError = true)]
		private static extern int CEWaitForSingleObject(IntPtr hHandle, uint dwMilliseconds); 

		[DllImport("coredll.dll", EntryPoint="EventModify", SetLastError = true)]
		private static extern int CEEventModify(IntPtr hEvent, uint function); 

		[DllImport("coredll.dll", EntryPoint="CreateEvent", SetLastError = true)]
		private static extern IntPtr CECreateEvent(IntPtr lpEventAttributes, int bManualReset, int bInitialState, string lpName); 

		[DllImport("coredll.dll", EntryPoint="EscapeCommFunction", SetLastError = true)]
		private static extern int CEEscapeCommFunction(IntPtr hFile, UInt32 dwFunc);

		[DllImport("coredll.dll", EntryPoint="SetCommTimeouts", SetLastError = true)]
		private static extern int CESetCommTimeouts(IntPtr hFile, CommTimeouts timeouts);

		[DllImport("coredll.dll", EntryPoint="GetCommState", SetLastError = true)]
		private static extern int CEGetCommState(IntPtr hFile, DCB dcb);

		[DllImport("coredll.dll", EntryPoint="SetCommState", SetLastError = true)]
		private static extern int CESetCommState(IntPtr hFile, DCB dcb);

		[DllImport("coredll.dll", EntryPoint="SetupComm", SetLastError = true)]
		private static extern int CESetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);

		[DllImport("coredll.dll", EntryPoint="CloseHandle", SetLastError = true)]
		private static extern int CECloseHandle(IntPtr hObject);

		[DllImport("coredll.dll", EntryPoint="WriteFile", SetLastError = true)]
		private static extern int CEWriteFile(IntPtr hFile, byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref Int32 lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("coredll.dll", EntryPoint="ReadFile", SetLastError = true)]
		private static extern int CEReadFile(IntPtr hFile, byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref Int32 lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("coredll.dll", EntryPoint="SetCommMask", SetLastError = true)]
		private static extern int CESetCommMask(IntPtr handle, CommEventFlags dwEvtMask);

		[DllImport("coredll.dll", EntryPoint="GetCommModemStatus", SetLastError = true)]
		extern private static int CEGetCommModemStatus(IntPtr hFile, ref uint lpModemStat);

		[DllImport("coredll.dll", EntryPoint="ClearCommError", SetLastError = true)]
		extern private static int CEClearCommError(IntPtr hFile, ref CommErrorFlags lpErrors, CommStat lpStat);

		[DllImport("coredll.dll", EntryPoint="WaitCommEvent", SetLastError = true)]
		private static extern int CEWaitCommEvent(IntPtr hFile, ref CommEventFlags lpEvtMask, IntPtr lpOverlapped);
		
		[DllImport("coredll.dll", EntryPoint="CreateFileW", SetLastError = true)]
		private static extern IntPtr CECreateFileW(
			String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
			IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile);

        [DllImport("coredll.dll", EntryPoint = "PurgeComm", SetLastError = true)]
        private static extern int CEPurgeComm(IntPtr hFile, PurgeCommFlags lpPurgeFlags);

		#endregion

		#region Desktop Windows API imports

		[DllImport("kernel32.dll", EntryPoint="WaitForSingleObject", SetLastError = true)]
		private static extern int WinWaitForSingleObject(IntPtr hHandle, uint dwMilliseconds); 

		[DllImport("kernel32.dll", EntryPoint="EventModify", SetLastError = true)]
		private static extern int WinEventModify(IntPtr hEvent, uint function); 

		[DllImport("kernel32.dll", EntryPoint="CreateEvent", SetLastError = true)]
		private static extern IntPtr WinCreateEvent(IntPtr lpEventAttributes, int bManualReset, int bInitialState, string lpName); 

		[DllImport("kernel32.dll", EntryPoint="EscapeCommFunction", SetLastError = true)]
		private static extern int WinEscapeCommFunction(IntPtr hFile, UInt32 dwFunc);

		[DllImport("kernel32.dll", EntryPoint="SetCommTimeouts", SetLastError = true)]
		private static extern int WinSetCommTimeouts(IntPtr hFile, CommTimeouts timeouts);

		[DllImport("kernel32.dll", EntryPoint="GetCommState", SetLastError = true)]
		private static extern int WinGetCommState(IntPtr hFile, DCB dcb);

		[DllImport("kernel32.dll", EntryPoint="SetCommState", SetLastError = true)]
		private static extern int WinSetCommState(IntPtr hFile, DCB dcb);

		[DllImport("kernel32.dll", EntryPoint="SetupComm", SetLastError = true)]
		private static extern int WinSetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);

		[DllImport("kernel32.dll", EntryPoint="CloseHandle", SetLastError = true)]
		private static extern int WinCloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", EntryPoint="WriteFile", SetLastError = true)]
		extern private static int WinWriteFile(IntPtr hFile, byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref Int32 lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", EntryPoint="ReadFile", SetLastError = true)]
		private static extern int WinReadFile(IntPtr hFile, byte[] lpBuffer, UInt32 nNumberOfBytesToRead, ref Int32 lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", EntryPoint="SetCommMask", SetLastError = true)]
		private static extern int WinSetCommMask(IntPtr handle, CommEventFlags dwEvtMask);

		[DllImport("kernel32.dll")]
		extern private static int WinGetCommModemStatus(IntPtr hFile, ref uint lpModemStat);

		[DllImport("kernel32.dll", EntryPoint="ClearCommError", SetLastError = true)]
		extern private static int WinClearCommError(IntPtr hFile, ref CommErrorFlags lpErrors, CommStat lpStat);

		[DllImport("kernel32.dll", EntryPoint="CreateFileW", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr WinCreateFileW(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
			IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll", EntryPoint="WaitCommEvent", SetLastError = true)]
		private static extern int WinWaitCommEvent(IntPtr hFile, ref CommEventFlags lpEvtMask, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", EntryPoint = "PurgeComm", SetLastError = true)]
        private static extern int WinPurgeComm(IntPtr hFile, PurgeCommFlags lpPurgeFlags);

		#endregion
		
		//These functions wrap the P/Invoked API calls and:
		// - make the correct call based on whether we're running under the full or compact framework
		// - eliminate empty parameters and defaults
		//		
		internal static IntPtr CreateFile(string FileName)
		{
			uint access = GENERIC_WRITE | GENERIC_READ;

			if(FullFramework)
			{
				return WinCreateFileW(FileName, access, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
			}
			else
			{
				return CECreateFileW(FileName, access, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
			}
		}

		internal static bool WaitCommEvent(IntPtr hPort, ref CommEventFlags flags) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinWaitCommEvent(hPort, ref flags, IntPtr.Zero));				
			} 
			else 
			{
				return Convert.ToBoolean(CEWaitCommEvent(hPort, ref flags, IntPtr.Zero));
			}
		}

		internal static bool ClearCommError(IntPtr hPort, ref CommErrorFlags flags, CommStat stat) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinClearCommError(hPort, ref flags, stat));
			} 
			else 
			{
				return Convert.ToBoolean(CEClearCommError(hPort, ref flags, stat));
			}
		}

		internal static bool GetCommModemStatus(IntPtr hPort, ref uint lpModemStat)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinGetCommModemStatus(hPort, ref lpModemStat));
			} 
			else 
			{
				return Convert.ToBoolean(CEGetCommModemStatus(hPort, ref lpModemStat));
			}
		}

		internal static bool SetCommMask(IntPtr hPort, CommEventFlags dwEvtMask) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinSetCommMask(hPort, dwEvtMask));
			} 
			else 
			{
				if( CommEventFlags.ALL == dwEvtMask )
				{
					//This is a CE device so include the POWER event
					dwEvtMask = CommEventFlags.ALL_CE;
				}
				return Convert.ToBoolean(CESetCommMask(hPort, dwEvtMask));
			}
		}	

		internal static bool ReadFile(IntPtr hPort, byte[] buffer, uint cbToRead, ref Int32 cbRead) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinReadFile(hPort, buffer, cbToRead, ref cbRead, IntPtr.Zero));
			} 
			else 
			{
				return Convert.ToBoolean(CEReadFile(hPort, buffer, cbToRead, ref cbRead, IntPtr.Zero));
			}
		}		

		internal static bool WriteFile(IntPtr hPort, byte[] buffer, UInt32 cbToWrite, ref Int32 cbWritten) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinWriteFile(hPort, buffer, cbToWrite, ref cbWritten, IntPtr.Zero));
			} 
			else 
			{
				return Convert.ToBoolean(CEWriteFile(hPort, buffer, cbToWrite, ref cbWritten, IntPtr.Zero));
			}
		}

		internal static bool CloseHandle(IntPtr hPort) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinCloseHandle(hPort));
			} 
			else 
			{
				return Convert.ToBoolean(CECloseHandle(hPort));
			}
		}

		internal static bool SetupComm(IntPtr hPort, UInt32 dwInQueue, UInt32 dwOutQueue)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinSetupComm(hPort, dwInQueue, dwOutQueue));
			} 
			else 
			{
				return Convert.ToBoolean(CESetupComm(hPort, dwInQueue, dwOutQueue));
			}
		}

		internal static bool SetCommState(IntPtr hPort, DCB dcb) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinSetCommState(hPort, dcb));
			} 
			else 
			{
				return Convert.ToBoolean(CESetCommState(hPort, dcb));
			}
		}

		internal static bool GetCommState(IntPtr hPort, DCB dcb) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinGetCommState(hPort, dcb));
			} 
			else 
			{
				return Convert.ToBoolean(CEGetCommState(hPort, dcb));
			}
		}

		internal static bool SetCommTimeouts(IntPtr hPort, CommTimeouts timeouts) 
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinSetCommTimeouts(hPort, timeouts));
			} 
			else 
			{
				return Convert.ToBoolean(CESetCommTimeouts(hPort, timeouts));
			}
		}
		
		internal static bool EscapeCommFunction(IntPtr hPort, CommEscapes escape)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinEscapeCommFunction(hPort, (uint)escape));
			} 
			else 
			{
				return Convert.ToBoolean(CEEscapeCommFunction(hPort, (uint)escape));
			}
		}

		internal static IntPtr CreateEvent(bool bManualReset, bool bInitialState, string lpName)
		{
			if (FullFramework) 
			{
				return WinCreateEvent(IntPtr.Zero, Convert.ToInt32(bManualReset), Convert.ToInt32(bInitialState), lpName);
			} 
			else 
			{
				return CECreateEvent(IntPtr.Zero, Convert.ToInt32(bManualReset), Convert.ToInt32(bInitialState), lpName);
			}
		}

		internal static bool SetEvent(IntPtr hEvent)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinEventModify(hEvent, (uint)EventFlags.EVENT_SET));
			} 
			else 
			{
				return Convert.ToBoolean(CEEventModify(hEvent, (uint)EventFlags.EVENT_SET));
			}
		}

		internal static bool ResetEvent(IntPtr hEvent)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinEventModify(hEvent, (uint)EventFlags.EVENT_RESET));
			} 
			else 
			{
				return Convert.ToBoolean(CEEventModify(hEvent, (uint)EventFlags.EVENT_RESET));
			}
		}

		internal static bool PulseEvent(IntPtr hEvent)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean(WinEventModify(hEvent, (uint)EventFlags.EVENT_PULSE));
			} 
			else 
			{
				return Convert.ToBoolean(CEEventModify(hEvent, (uint)EventFlags.EVENT_PULSE));
			}
		}

		internal static int WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds)
		{
			if (FullFramework) 
			{
				return WinWaitForSingleObject(hHandle, dwMilliseconds);
			} 
			else 
			{
				return CEWaitForSingleObject(hHandle, dwMilliseconds);
			}
		}

        internal static bool PurgeComm(IntPtr hHandle, PurgeCommFlags commflags)
		{
			if (FullFramework) 
			{
				return Convert.ToBoolean( WinPurgeComm(hHandle, commflags));
			} 
			else 
			{
				return Convert.ToBoolean( CEPurgeComm(hHandle, commflags));
			}
		}       
	}
}


