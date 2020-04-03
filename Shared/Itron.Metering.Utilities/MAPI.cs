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
//                              Copyright © 2007
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;

namespace Itron.Metering.Utilities
{
	/// <summary></summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class MapiMessage
    {
		/// <summary></summary>
        public int reserved;
		/// <summary></summary>
        public string subject;
		/// <summary></summary>
        public string noteText;
		/// <summary></summary>
        public string messageType;
		/// <summary></summary>
        public string dateReceived;
		/// <summary></summary>
        public string conversationID;
		/// <summary></summary>
        public int flags;
		/// <summary></summary>
        public IntPtr originator;
		/// <summary></summary>
        public int recipCount;
		/// <summary></summary>
        public IntPtr recips;
		/// <summary></summary>
        public int fileCount;
		/// <summary></summary>
        public IntPtr files;
    }

	/// <summary></summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public class MapiFileDesc
    {
		/// <summary></summary>
        public int reserved;
		/// <summary></summary>
        public int flags;
		/// <summary></summary>
        public int position;
		/// <summary></summary>
        public string path;
		/// <summary></summary>
        public string name;
		/// <summary></summary>
        public IntPtr type;
    }
	/// <summary></summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public class MapiRecipDesc
    {
		/// <summary></summary>
        public int        reserved;
		/// <summary></summary>
        public int        recipClass;
		/// <summary></summary>
        public string    name;
		/// <summary></summary>
        public string    address;
		/// <summary></summary>
        public int        eIDSize;
		/// <summary></summary>
        public IntPtr    entryID;
    }

	/// <summary>
	/// This class wraps the MAPI32.dll for sending emails
	/// </summary>
    public class MAPI
    {
		/// <summary>
		/// Add to
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
        public bool AddRecipientTo(string email)
        {
            return AddRecipient(email, HowTo.MAPI_TO);
        }

		/// <summary>
		/// Adds CC
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
        public bool AddRecipientCC(string email)
        {
            return AddRecipient(email, HowTo.MAPI_TO);
        }

		/// <summary>
		/// Adds BCC
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
        public bool AddRecipientBCC(string email)
        {
            return AddRecipient(email, HowTo.MAPI_TO);
        }

		/// <summary>
		/// Adds attachment
		/// </summary>
		/// <param name="strAttachmentFileName"></param>
        public void AddAttachment(string strAttachmentFileName)
        {
            m_attachments.Add(strAttachmentFileName);
        }

		/// <summary>
		/// Puts up an email message
		/// </summary>
		/// <param name="strSubject"></param>
		/// <param name="strBody"></param>		
		/// <returns></returns>
		public int SendMailPopup(string strSubject, string strBody)
        {			
			return SendMail(strSubject, strBody, MAPI_LOGON_UI | MAPI_DIALOG);
        }

		/// <summary>
		/// Sends and email message without a dialog
		/// </summary>
		/// <param name="strSubject"></param>
		/// <param name="strBody"></param>
		/// <returns></returns>
        public int SendMailDirect(string strSubject, string strBody)
        {
            return SendMail(strSubject, strBody, MAPI_LOGON_UI);
        }
		
        [DllImport("MAPI32.DLL", SetLastError = true)]
        static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, 
            MapiMessage message, int flg, int rsv);

        int SendMail(string strSubject, string strBody, int how)
        {
			string strError = "";
            MapiMessage msg = new MapiMessage();
            msg.subject = strSubject;
            msg.noteText = strBody;

            msg.recips = GetRecipients(out msg.recipCount);
            msg.files = GetAttachments(out msg.fileCount);

            m_lastError = MAPISendMail(IntPtr.Zero, IntPtr.Zero, msg, how, 0);
						
			if (m_lastError > 1)
				strError = GetLastError();

			Cleanup(ref msg);

			if (strError.Length != 0)
			{
				throw new InvalidOperationException(strError);
			}
			
            return m_lastError;
        }

        bool AddRecipient(string email, HowTo howTo)
        {
            MapiRecipDesc recipient = new MapiRecipDesc();

            recipient.recipClass = (int)howTo;
            recipient.name = email;
            m_recipients.Add(recipient);

            return true;
        }

        IntPtr GetRecipients(out int recipCount)
        {
            recipCount = 0;
            if (m_recipients.Count == 0)
                return IntPtr.Zero;

            int size = Marshal.SizeOf(typeof(MapiRecipDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(m_recipients.Count * size);

            int ptr = (int)intPtr;
            foreach (MapiRecipDesc mapiDesc in m_recipients)
            {
                Marshal.StructureToPtr(mapiDesc, (IntPtr)ptr, false);
                ptr += size;
            }

            recipCount = m_recipients.Count;
            return intPtr;
        }

        IntPtr GetAttachments(out int fileCount)
        {
            fileCount = 0;
            if (m_attachments == null)
                return IntPtr.Zero;

            if ((m_attachments.Count <= 0)) //|| (m_attachments.Count > maxAttachments))
			{
                return IntPtr.Zero;
			}

            int size = Marshal.SizeOf(typeof(MapiFileDesc));
            IntPtr intPtr = Marshal.AllocHGlobal(m_attachments.Count * size);

            MapiFileDesc mapiFileDesc = new MapiFileDesc();
            mapiFileDesc.position = -1;
            int ptr = (int)intPtr;
            
            foreach (string strAttachment in m_attachments)
            {
                mapiFileDesc.name = Path.GetFileName(strAttachment);
                mapiFileDesc.path = strAttachment;
                Marshal.StructureToPtr(mapiFileDesc, (IntPtr)ptr, false);
                ptr += size;
            }

            fileCount = m_attachments.Count;
            return intPtr;
        }

        void Cleanup(ref MapiMessage msg)
        {
            int size = Marshal.SizeOf(typeof(MapiRecipDesc));
            int ptr = 0;

            if (msg.recips != IntPtr.Zero)
            {
                ptr = (int)msg.recips;
                for (int i = 0; i < msg.recipCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr)ptr, 
                        typeof(MapiRecipDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.recips);
            }

            if (msg.files != IntPtr.Zero)
            {
                size = Marshal.SizeOf(typeof(MapiFileDesc));

                ptr = (int)msg.files;
                for (int i = 0; i < msg.fileCount; i++)
                {
                    Marshal.DestroyStructure((IntPtr)ptr, 
                        typeof(MapiFileDesc));
                    ptr += size;
                }
                Marshal.FreeHGlobal(msg.files);
            }
            
            m_recipients.Clear();
            m_attachments.Clear();
            m_lastError = 0;
        }
        
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public string GetLastError()
        {
            if (m_lastError <= 26)
                return errors[ m_lastError ];
            return "MAPI error [" + m_lastError.ToString(CultureInfo.InvariantCulture) + "]";
        }

        readonly string[] errors = new string[] {
        "OK", "User abort", "General MAPI failure", 
                "E-Mail login failure", "Disk full", 
                "Insufficient memory", "Access denied", 
                "Unknown", "Too many sessions", 
                "Too many files were specified", 
                "Too many recipients were specified", 
                "A specified attachment was not found",
        "Attachment open failure", 
                "Attachment write failure", "Unknown recipient", 
                "Bad recipient type", "No messages", 
                "Invalid message", "Text too large", 
                "Invalid session", "Type not supported", 
                "A recipient was specified ambiguously", 
                "Message in use", "Network failure",
        "Invalid edit fields", "Invalid recipients", 
                "Not supported" 
        };


        List<MapiRecipDesc> m_recipients    = new 
            List<MapiRecipDesc>();
        List<string> m_attachments    = new List<string>();
        int m_lastError = 0;

        const int MAPI_LOGON_UI = 0x00000001;
		const int MAPI_NEW_SESSION = 0x00000002;
        const int MAPI_DIALOG = 0x00000008;
        const int maxAttachments = 20;

        enum HowTo{MAPI_ORIG=0, MAPI_TO, MAPI_CC, MAPI_BCC};
    }    
}
