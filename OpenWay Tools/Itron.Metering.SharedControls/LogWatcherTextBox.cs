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
//                            Copyright © 2012 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Text box that displays the end of a log file and updates automatically
    /// </summary>
    public class LogWatcherTextBox : TextBox
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created
        
        public LogWatcherTextBox()
        {
            Multiline = true;
            ScrollBars = ScrollBars.Both;

            KeyPress += new KeyPressEventHandler(LogWatcherTextBox_KeyPress);

            m_FileWatcherEventHandler = new FileSystemEventHandler(m_FileWatcher_Changed);
            m_LinesToDisplay = 10;
        }

        /// <summary>
        /// Begins watching the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file to watch</param>
        /// <param name="linesToDisplay">The number of lines to display</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created

        public void WatchFile(string filePath, int linesToDisplay)
        {
            FileInfo WatchedFileInfo = new FileInfo(filePath);

            m_LinesToDisplay = linesToDisplay;

            if (m_FileWatcher != null)
            {
                // Disable monitoring of the previous file
                m_FileWatcher.EnableRaisingEvents = false;
                m_FileWatcher.Changed -= m_FileWatcherEventHandler;
                m_FileWatcher.Dispose();
            }

            m_FileWatcher = new FileSystemWatcher(WatchedFileInfo.DirectoryName, WatchedFileInfo.Name);
            m_FileWatcher.NotifyFilter = NotifyFilters.Size;
            m_FileWatcher.Changed += m_FileWatcherEventHandler;
            m_FileWatcher.EnableRaisingEvents = true;


            UpdateFileText(filePath);
        }

        /// <summary>
        /// Stops the current file from being watched
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created
        
        public void Stop()
        {
            if (m_FileWatcher != null)
            {
                m_FileWatcher.EnableRaisingEvents = false;
                m_FileWatcher.Changed -= m_FileWatcherEventHandler;
                m_FileWatcher.Dispose();
                m_FileWatcher = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the file watcher changed event
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created

        private void m_FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new FileSystemEventHandler(m_FileWatcher_Changed), new object[] {sender, e});
            }
            else
            {
                UpdateFileText(e.FullPath);
            }
        }

        /// <summary>
        /// Updates the text box with the text from the file
        /// </summary>
        /// <param name="path">The path to the file to update from</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created

        private void UpdateFileText(string path)
        {
            lock (m_FileWatcherEventHandler)
            {
                if (File.Exists(path))
                {
                    FileStream ChangedFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    int FoundLines = 0;
                    Text = "";

                    if (1 <= ChangedFileStream.Length)
                    {
                        // Seek to the end and then begin searching backwards for line endings
                        ChangedFileStream.Seek(-1, SeekOrigin.End);
                    }

                    while (ChangedFileStream.Position > 0 && FoundLines <= m_LinesToDisplay)
                    {
                        char CurrentCharacter = (char)ChangedFileStream.ReadByte();

                        if (CurrentCharacter == '\n')
                        {
                            FoundLines++;
                        }

                        // Since we read a byte we need to move back two bytes
                        ChangedFileStream.Position -= 2;
                    }

                    if (FoundLines > 0)
                    {
                        StreamReader FileReader = new StreamReader(ChangedFileStream);
                        string FoundText = FileReader.ReadToEnd();
                        string[] NewLines = FoundText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                        Array.Reverse(NewLines);
                        Lines = NewLines;

                        FileReader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Key Press event and prevents the user from typing in the text box
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/15/12 RCG 2.70.38        Created
        
        private void LogWatcherTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region Member Variables

        private FileSystemEventHandler m_FileWatcherEventHandler;
        private FileSystemWatcher m_FileWatcher;
        private int m_LinesToDisplay;

        #endregion
    }
}
