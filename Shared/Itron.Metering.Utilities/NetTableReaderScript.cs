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
//                              Copyright © 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used to represent Net Table Reader scripts
    /// </summary>
    public class NetTableReaderScript
    {
        #region Constants

        private const string NTR_ROOT = "NetTableReader";
        private const string DESCRIPTION = "Description";
        private const string COMMANDS = "Commands";
        private const string EPSEM = "EPSEM";
        private const string READ = "Read";
        private const string WRITE = "Write";
        private const string OFFSET = "Offset";
        private const string RESTRICTIONS = "Restrictions";
        private const string EXPIRATION = "ExpirationDate";
        private const string HARDWARE = "Hardware";
        private const string FIRMWARE = "Firmware";
        private const string SIGNATURE = "Signature";
        private const string MIN = "min";
        private const string MAX = "max";
        private const string TABLEID = "TableID";
        private const string COMMAND = "Command";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFileName">the net table reader script file path</param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public NetTableReaderScript(string strFileName)
        {
            m_Commands = new List<NetTableReaderCommand>();
            XmlReader reader = XmlReader.Create(strFileName);

            reader.MoveToElement();

            while (false == reader.EOF)
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (reader.Name == NTR_ROOT)
                        {
                            m_NTRRoot = reader.Name;
                            m_ScriptName = reader.GetAttribute("Name");

                            if (string.IsNullOrEmpty(m_ScriptName))
                            {
                                // If the script doesn't have a name attribute, use the file name
                                m_ScriptName = Path.GetFileNameWithoutExtension(strFileName);
                            }

                            // found the root note so read the rest of the script
                            ReadNTRScript(reader);
                        }
                        break;
                    }
                    default:
                    {
                        // We haven't found the root so keep looking
                        break;
                    }
                }
            }

            reader.Close();
        }

        /// <summary>
        /// Determines whether the script has an EPSEM tag
        /// </summary>
        /// <param name="strFile">the full file path of the script</param>
        /// <returns>true if the EPSEM tag is found; false, otherwise</returns>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/28/16 AF  4.50.224 RTT587957  Created
        //
        public bool HasEPSEMTag(string strFile)
        {
            bool blnContainsPSEM = false;

            XmlDocument Document = new XmlDocument();

            // Make sure the file exists first
            if (File.Exists(strFile))
            {
                // Load the document
                Document.PreserveWhitespace = true;
                Document.Load(strFile);

                XmlNodeList xmlnodes = Document.GetElementsByTagName("EPSEM");
                if (xmlnodes.Count == 1)
                {
                    blnContainsPSEM = true;
                }
            }

            return blnContainsPSEM;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the expiration date
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public DateTime? ExpirationDate
        {
            get
            {
                return m_ExpirationDate;
            }
        }

        /// <summary>
        /// Gets the minimum hardware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string MinHWVersion
        {
            get
            {
                return m_MinHWVersion;
            }
        }

        /// <summary>
        /// Gets the maximum hardware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string MaxHWVersion
        {
            get
            {
                return m_MaxHWVersion;
            }
        }

        /// <summary>
        /// Gets the minimum firmware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string MinFWVersion
        {
            get
            {
                return m_MinFWVersion;
            }
        }

        /// <summary>
        /// Gets the maximum firmware version
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string MaxFWVersion
        {
            get
            {
                return m_MaxFWVersion;
            }
        }

        /// <summary>
        /// Gets the name of the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string ScriptName
        {
            get
            {
                return m_ScriptName;
            }
        }

        /// <summary>
        /// Gets the script description
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        /// <summary>
        /// Gets the net table reader script root, "NetTableReader"
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/01/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        public string NetTableReaderScriptRoot
        {
            get
            {
                return m_NTRRoot;
            }
        }

        /// <summary>
        /// Gets the list of commands in the script
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/03/16 AF  4.50.226 RTT 587957, 587960 Created
        //
        public List<NetTableReaderCommand> Commands
        {
            get
            {
                return m_Commands;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the net table reader script
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //
        private void ReadNTRScript(XmlReader reader)
        {
            bool bDone = false;

            reader.MoveToElement();

            while ((false == reader.EOF) && (false == bDone))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        switch (reader.Name)
                        {
                            case EXPIRATION:
                            {
                                try
                                {
                                    m_ExpirationDate = Convert.ToDateTime(reader.GetAttribute("value"), CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    m_ExpirationDate = null;
                                }
                                break;
                            }
                            case HARDWARE:
                            {
                                m_MinHWVersion = reader.GetAttribute(MIN);
                                m_MaxHWVersion = reader.GetAttribute(MAX);
                                break;
                            }
                            case FIRMWARE:
                            {
                                m_MinFWVersion = reader.GetAttribute(MIN);
                                m_MaxFWVersion = reader.GetAttribute(MAX);
                                break;
                            }
                            case COMMANDS:
                            {
                                ReadCommands(reader);
                                break;
                            }
                        }
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        if (reader.Name == NTR_ROOT)
                        {
                            bDone = true;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the commands portion of the net table reader script
        /// </summary>
        /// <param name="reader"></param>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  01/29/16 AF  4.50.225 RTT 587957, 587960 Created
        //  02/03/16 AF  4.50.226 RTT 587957, 587960 Corrected the errors
        //
        private void ReadCommands(XmlReader reader)
        {
            bool bDone = false;
            bool bContainsOffset = false;

            reader.MoveToElement();

            while (!bDone)
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                    {
                        if (reader.Name == COMMANDS)
                        {
                            bDone = true;
                        }

                        break;
                    }
                    case XmlNodeType.Element:
                    {
                        if ((WRITE == reader.Name) || (READ == reader.Name))
                        {
                            NetTableReaderCommand cmd = new NetTableReaderCommand();
                            cmd.CommandType = reader.Name;
                            while (reader.MoveToNextAttribute())
                            {
                                if (reader.Name == TABLEID)
                                {
                                    cmd.TableID = Convert.ToUInt16(reader.Value, CultureInfo.InvariantCulture);
                                }
                                else if (reader.Name == COMMAND)
                                {
                                    cmd.CommandName = reader.Value;
                                }
                                else if (reader.Name == OFFSET)
                                {
                                    cmd.Offset = Convert.ToInt32(reader.Value, CultureInfo.InvariantCulture);
                                    bContainsOffset = true;
                                    cmd.IsOffsetReadOrWrite = true;
                                }
                                else if (reader.Name == "Length")
                                {
                                    cmd.Length = Convert.ToUInt16(reader.Value, CultureInfo.InvariantCulture);
                                    cmd.IsOffsetReadOrWrite = true;
                                }
                            }

                            if (WRITE == cmd.CommandType)
                            {
                                // Get the value we are going to write to the table
                                reader.Read();
                                string temp = (reader.Value).Replace(" ", "");
                                cmd.Data = new byte[temp.Length / 2];
                                char[] tempCharArray = temp.ToCharArray();

                                for (int i = 0, j = cmd.Data.Length, k = 0; i < j; i++, k += 2)
                                {
                                    cmd.Data[i] = byte.Parse(string.Format(CultureInfo.InvariantCulture, "{0}{1}", tempCharArray[k], tempCharArray[k + 1]), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                                }

                                if (bContainsOffset)
                                {
                                    cmd.Length = (ushort)cmd.Data.Length;
                                }
                            }

                            m_Commands.Add(cmd);
                        }
                        break;
                    }
                }
            }
        }

        #endregion

        #region Members

        private string m_ScriptName = "";
        private string m_Description = "";
        private DateTime? m_ExpirationDate = null;
        private string m_MinHWVersion = "";
        private string m_MaxHWVersion = "";
        private string m_MinFWVersion = "";
        private string m_MaxFWVersion = "";
        private string m_NTRRoot = "";
        private List<NetTableReaderCommand> m_Commands;

        #endregion
    }

    /// <summary>
    /// Helper class to encapsulate a single command in a net table reader script
    /// </summary>
    public class NetTableReaderCommand
    {
        #region Public Methods

        /// <summary>
        /// Constructer
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public NetTableReaderCommand()
        {
            m_CommandName = "";
            m_CommandType = "";
            m_TableId = 0;
            m_Offset = 0;
            m_Length = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Command name as taken from the script.  Could be an empty string
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public string CommandName
        {
            get
            {
                return m_CommandName;
            }
            set
            {
                m_CommandName = value;
            }
        }

        /// <summary>
        /// Command type as read from the script.  The only types recognized are "Read" and "Write"
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public string CommandType
        {
            get
            {
                return m_CommandType;
            }
            set
            {
                m_CommandType = value;
            }
        }

        /// <summary>
        /// The table id of the table to read or write
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public ushort TableID
        {
            get
            {
                return m_TableId;
            }
            set
            {
                m_TableId = value;
            }
        }

        /// <summary>
        /// For offset reads and writes. This is the offset into the table to begin the read or write
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public int Offset
        {
            get
            {
                return m_Offset;
            }
            set
            {
                m_Offset = value;
            }
        }

        /// <summary>
        /// For offset reads and writes, this is the number of bytes to read or transmit
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public ushort Length
        {
            get
            {
                return m_Length;
            }
            set
            {
                m_Length = value;
            }
        }

        /// <summary>
        /// For table writes, this is the data to write to the table
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  12/04/15 AF  4.50.218 RTT 587959 Created
        //
        public byte[] Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                m_Data = value;
            }
        }

        /// <summary>
        /// Returns true if the script contains either the Offset tag, the Length tag or both
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version  ID Number  Description
        //  -------- --- -------  -- ------  --------------------------------------------
        //  02/04/16 AF  4.50.226 RTT 587959 Created
        //
        public bool IsOffsetReadOrWrite
        {
            get
            {
                return m_bIsOffsetReadOrWrite;
            }
            set
            {
                m_bIsOffsetReadOrWrite = value;
            }
        }

        #endregion

        #region Members

        private string m_CommandName;
        private string m_CommandType;
        private ushort m_TableId;
        private int m_Offset;
        private ushort m_Length;
        private byte[] m_Data;
        bool m_bIsOffsetReadOrWrite;

        #endregion
    }

}
