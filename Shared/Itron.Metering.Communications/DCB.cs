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
using System.Text;
using System.Runtime.InteropServices;

namespace Itron.Metering.Communications
{

	/// <summary>
	/// The Win32 DCB structure is implemented below in a C# class. 
	/// </summary>
	/// Revision History
	/// MM/DD/YY who Version Issue# Description
	/// -------- --- ------- ------ ---------------------------------------
	/// 08/01/05 bdm 7.13.00 N/A	DCB class is sample code provided off
	///                             off the web and used as is.
	[StructLayout(LayoutKind.Sequential)]
	internal class DCB 
	{
		//Enumeration for fDtrControl bit field. Underlying type only needs
		//to be a byte since we only have 2-bits of information.
		internal enum DtrControlFlags : byte 
		{
			Disable = 0,
			Enable =1 ,
			Handshake = 2
		}

		// Enumeration for fRtsControl bit field. Underlying type only needs
		// to be a byte since we only have 2-bits of information.
		internal enum RtsControlFlags : byte 
		{
			Disable = 0,
			Enable = 1,
			Handshake = 2,
			Toggle = 3
		}

		internal DCB()
		{
			// Initialize the length of the structure. Marshal.SizeOf returns
			// the size of the unmanaged object (basically the object that
			// gets marshalled).
			this.DCBlength = (uint)Marshal.SizeOf(this);
		}

		private   UInt32 DCBlength;
		public   UInt32 BaudRate;
		internal UInt32 Control;
		internal UInt16 wReserved;
		public   UInt16 XonLim;
		public   UInt16 XoffLim;
		public   byte   ByteSize;
		public   byte   Parity;
		public   byte   StopBits;
		public   sbyte  XonChar;
		public   sbyte  XoffChar;
		public   sbyte  ErrorChar;
		public   sbyte  EofChar;
		public   sbyte  EvtChar;
		internal UInt16 wReserved1;

		// We need to have reserved fields to preserve the size of the 
		// underlying structure to match the Win32 structure when it is 
		// marshaled. Use these fields to suppress compiler warnings.
		internal void _SuppressCompilerWarnings()
		{
			wReserved +=0;
			wReserved1 +=0;
		}
        
		// Helper constants for manipulating the bit fields.
		private readonly UInt32 fBinaryMask             = 0x00000001;
		private readonly Int32  fBinaryShift            = 0;
		private readonly UInt32 fParityMask             = 0x00000002;
		private readonly Int32  fParityShift            = 1;
		private readonly UInt32 fOutxCtsFlowMask        = 0x00000004;
		private readonly Int32  fOutxCtsFlowShift       = 2;
		private readonly UInt32 fOutxDsrFlowMask        = 0x00000008;
		private readonly Int32  fOutxDsrFlowShift       = 3;
		private readonly UInt32 fDtrControlMask         = 0x00000030;
		private readonly Int32  fDtrControlShift        = 4;
		private readonly UInt32 fDsrSensitivityMask     = 0x00000040;
		private readonly Int32  fDsrSensitivityShift    = 6;
		private readonly UInt32 fTXContinueOnXoffMask   = 0x00000080;
		private readonly Int32  fTXContinueOnXoffShift  = 7;
		private readonly UInt32 fOutXMask               = 0x00000100;
		private readonly Int32  fOutXShift              = 8;
		private readonly UInt32 fInXMask                = 0x00000200;
		private readonly Int32  fInXShift               = 9;
		private readonly UInt32 fErrorCharMask          = 0x00000400;
		private readonly Int32  fErrorCharShift         = 10;
		private readonly UInt32 fNullMask               = 0x00000800;
		private readonly Int32  fNullShift              = 11;
		private readonly UInt32 fRtsControlMask         = 0x00003000;
		private readonly Int32  fRtsControlShift        = 12;
		private readonly UInt32 fAbortOnErrorMask       = 0x00004000;
		private readonly Int32  fAbortOnErrorShift      = 14;

		public bool fBinary 
		{
			get { return ((Control & fBinaryMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fBinaryShift); }
		}
		public bool fParity 
		{
			get { return ((Control & fParityMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fParityShift); }
		}
		public bool fOutxCtsFlow 
		{
			get { return ((Control & fOutxCtsFlowMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fOutxCtsFlowShift); }
		}
		public bool fOutxDsrFlow 
		{
			get { return ((Control & fOutxDsrFlowMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fOutxDsrFlowShift); }
		}
		public DtrControlFlags fDtrControl 
		{
			get { return (DtrControlFlags)((Control & fDtrControlMask) >> fDtrControlShift); }
			set { Control |= (Convert.ToUInt32(value) << fDtrControlShift); }
		}
		public bool fDsrSensitivity 
		{
			get { return ((Control & fDsrSensitivityMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fDsrSensitivityShift); }
		}
		public bool fTXContinueOnXoff 
		{
			get { return ((Control & fTXContinueOnXoffMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fTXContinueOnXoffShift); }
		}
		public bool fOutX 
		{
			get { return ((Control & fOutXMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fOutXShift); }
		}
		public bool fInX 
		{
			get { return ((Control & fInXMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fInXShift); }
		}
		public bool fErrorChar 
		{
			get { return ((Control & fErrorCharMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fErrorCharShift); }
		}
		public bool fNull 
		{
			get { return ((Control & fNullMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fNullShift); }
		}
		public RtsControlFlags fRtsControl 
		{
			get { return (RtsControlFlags)((Control & fRtsControlMask) >> fRtsControlShift); }
			set { Control |= (Convert.ToUInt32(value) << fRtsControlShift); }
		}
		public bool fAbortOnError 
		{
			get { return ((Control & fAbortOnErrorMask) != 0); }
			set { Control |= (Convert.ToUInt32(value) << fAbortOnErrorShift); }
		}
        
		// Method to dump the DCB to take a look and help debug issues.
		public override String ToString() 
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("DCB:\r\n");
			sb.AppendFormat(null, "  BaudRate:     {0}\r\n", BaudRate);
			sb.AppendFormat(null, "  Control:      0x{0:x}\r\n", Control);
			sb.AppendFormat(null, "    fBinary:           {0}\r\n", fBinary);
			sb.AppendFormat(null, "    fParity:           {0}\r\n", fParity);
			sb.AppendFormat(null, "    fOutxCtsFlow:      {0}\r\n", fOutxCtsFlow);
			sb.AppendFormat(null, "    fOutxDsrFlow:      {0}\r\n", fOutxDsrFlow);
			sb.AppendFormat(null, "    fDtrControl:       {0}\r\n", fDtrControl);
			sb.AppendFormat(null, "    fDsrSensitivity:   {0}\r\n", fDsrSensitivity);
			sb.AppendFormat(null, "    fTXContinueOnXoff: {0}\r\n", fTXContinueOnXoff);
			sb.AppendFormat(null, "    fOutX:             {0}\r\n", fOutX);
			sb.AppendFormat(null, "    fInX:              {0}\r\n", fInX);
			sb.AppendFormat(null, "    fNull:             {0}\r\n", fNull);
			sb.AppendFormat(null, "    fRtsControl:       {0}\r\n", fRtsControl);
			sb.AppendFormat(null, "    fAbortOnError:     {0}\r\n", fAbortOnError);
			sb.AppendFormat(null, "  XonLim:       {0}\r\n", XonLim);
			sb.AppendFormat(null, "  XoffLim:      {0}\r\n", XoffLim);
			sb.AppendFormat(null, "  ByteSize:     {0}\r\n", ByteSize);
			sb.AppendFormat(null, "  Parity:       {0}\r\n", Parity);
			sb.AppendFormat(null, "  StopBits:     {0}\r\n", StopBits);
			sb.AppendFormat(null, "  XonChar:      {0}\r\n", XonChar);
			sb.AppendFormat(null, "  XoffChar:     {0}\r\n", XoffChar);
			sb.AppendFormat(null, "  ErrorChar:    {0}\r\n", ErrorChar);
			sb.AppendFormat(null, "  EofChar:      {0}\r\n", EofChar);
			sb.AppendFormat(null, "  EvtChar:      {0}\r\n", EvtChar);

			return sb.ToString();
		}
	}
}
