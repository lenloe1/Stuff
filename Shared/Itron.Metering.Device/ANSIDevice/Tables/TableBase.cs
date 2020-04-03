#region Copywrite Itron, Inc.
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
//                              Copyright © 2006 - 2013
//                                Itron, Inc.
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Threading;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Abstract base class for C12.19 standard and manufacturer defined tables
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class AnsiTable
    {
        #region Constants

        private const int MAX_PENDING_SIZE = 223;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="psem">Protocol instance being used by the device</param>
        /// <param name="TableID">C12.19 table ID with the MFG bit included as ushort</param>
        /// <param name="Size">Size in bytes of this table</param>
        /// <param name="ExpirationTimeout">The amount of time (ms) before the table expires.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        public AnsiTable(CPSEM psem, ushort TableID, uint Size, int ExpirationTimeout)
        {
            m_TableID = TableID;
            m_PSEM = psem;
            ChangeTableSize(Size);
            m_iTimeout = ExpirationTimeout;
            m_ExpirationTimer = new Timer(new TimerCallback(TableExpired), null, Timeout.Infinite, Timeout.Infinite);
            m_blnAllowAutomaticTableResizing = false;
            InitializeMembers();
        }

		/// <summary>
		/// Abstract base class for C12.19 standard and manufacturer defined tables
		/// </summary>
		/// <param name="psem">Protocol instance being used by the device</param>
		/// <param name="TableID">C12.19 table ID with the MFG bit included as 
		/// <param name="Size">Size in bytes of this table</param>
		/// needed. i.e. Std table 1 ID = 1, MFG table 1 ID = 2049</param>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/08/06 mcm 7.30.00 N/A	   Created
        // 05/16/08 RCG 1.50.26 N/A    Modified to use new constructor.

        public AnsiTable(CPSEM psem, ushort TableID, uint Size)
            : this (psem, TableID, Size, Timeout.Infinite)
		{
		}

        /// <summary>
        /// Constructor used for File bases inheritance
        /// </summary>
        /// <param name="TableID"></param>
        /// <param name="Size"></param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 RCG 1.50.26 N/A    Modified to use new constructor.

        public AnsiTable(ushort TableID, uint Size)
            : this (null, TableID, Size, Timeout.Infinite)
        {
        }

		/// <summary>
		/// Performs a full read of this table.  The table will be marked as
		/// Loaded if the read succeeds.
		/// </summary>
		/// <overloads>Read(ushort Offset, ushort Count)</overloads>
		/// <returns>protocol response</returns>
		/// 
		// Revision History	
		// MM/DD/YY who Version ID Number Description
		// -------- --- ------- -- ------ ---------------------------------------
		// 06/08/06 mcm 7.30.00    N/A	  Created
        // 03/05/08 KRC 1.50.00           Adding Exception if PSEM is not valid
        // 11/15/11 RCG 2.53.06           Adding code to display a message if we get a data size mismatch
        // 11/07/12 jrf 2.70.36 WR 240583 Adding ability to ignore a table size mismatch under special circumstances.
        // 06/12/13 AF  2.80.37           Corrected spelling error in logger entry
        // 07/18/13 jrf 2.80.54 WR 417794 Modifying to only resize table when sizes differ.
        // 
        public virtual PSEMResponse Read()
		{
            PSEMResponse Result = PSEMResponse.Iar;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {
                if (m_PSEM != null)
                {
                    byte[] Data;
                    Result = m_PSEM.FullRead(m_TableID, out Data);

                    if (PSEMResponse.Ok == Result)
                    {
                        if (true == m_blnAllowAutomaticTableResizing && 0 < Data.Length && m_Data.Length != Data.Length)
                        {
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Resizing Table " + m_TableID.ToString(CultureInfo.InvariantCulture) 
                                + " based on data received. Original Length: " + m_Size.ToString(CultureInfo.InvariantCulture)
                                + " New Length: " + Data.Length.ToString(CultureInfo.InvariantCulture));
                            //Special case where table size is not known in advance and determination
                            //of this knowledge proves to be more complex and time consuming than worth while.
                            //Tables that allow this need to take care to only read as much data as retrieved.
                            ChangeTableSize((uint)Data.Length);
                        }
                        
                        if (m_Data.Length == Data.Length)
                        {
                            Array.Copy(Data, 0, m_Data, 0, m_Data.Length);
                            m_TableState = TableState.Loaded;
                            m_ExpirationTimer.Change(m_iTimeout, 0);

                            // mcm 3/9/2007 - SCR 2553: reposition stream for cases
                            // where the table might be read mulitple times.
                            m_DataStream.Position = 0;
                        }
                        else
                        {
                            Result = PSEMResponse.Err;
                            m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Data size mismatch. Expected Length: " + m_Size.ToString(CultureInfo.InvariantCulture)
                                + " Received Length: " + Data.Length.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
                else
                {
                    throw (new NotImplementedException("This Operation is not supported with a file"));
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (Result == PSEMResponse.Bsy || Result == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }
            }

			return Result;
		}

		/// <summary>
		/// Performs an offset read of this table.  The table state is NOT
		/// changed by this method.
		/// </summary>
		/// <overloads>Read()</overloads>
		/// <param name="Offset">byte offset to start reading from</param>
		/// <param name="Count">number bytes to read</param>
		/// <returns>protocol response</returns>
		/// 
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/08/06 mcm 7.30.00 N/A	Created
        // 03/05/08 KRC 1.50.00        Adding Exception if PSEM is not valid
        // 11/15/11 RCG 2.53.06        Adding code to display a message if we get a data size mismatch
		
        public virtual PSEMResponse Read(int Offset, ushort Count)
		{
			PSEMResponse Result = PSEMResponse.Iar;
            int iReadAttempt = 0;
            bool bRetry = true;

            while (bRetry)
            {

                if (m_PSEM != null)
                {
                    if (Offset + Count <= m_Data.Length)
                    {
                        byte[] Data;

                        Result = m_PSEM.OffsetRead(m_TableID, Offset,
                                                    Count, out Data);

                        if (PSEMResponse.Ok == Result)
                        {
                            if (Count == Data.Length)
                            {
                                Array.Copy(Data, 0, m_Data, Offset, Count);
                                m_DataStream.Position = Offset;
                            }
                            else
                            {
                                Result = PSEMResponse.Err;
                                m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Data size mismatch. Expected Length: " + Count.ToString(CultureInfo.InvariantCulture)
                                    + " Received Length: " + Data.Length.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
                else
                {
                    throw (new NotImplementedException("This Operation is not supported with a file"));
                }

                iReadAttempt++;

                if (iReadAttempt < 3 && (Result == PSEMResponse.Bsy || Result == PSEMResponse.Dnr))
                {
                    bRetry = true;
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    bRetry = false;
                }
            }

			return Result;
		}

		/// <summary>
		/// Writes the table's data to the meter. If the write succeeds, the
		/// table will be marked as Loaded.
		/// </summary>
		/// <remarks>
		/// No checking is done to see if the table is Loaded or Dirty before
		/// writing it to the meter, so you can write a 0 filled, Unloaded
		/// table if you weren't careful or really wanted to.
		/// </remarks>
		/// <overloads>Write(ushort Offset, ushort Count)</overloads>
		/// <returns>protocol response</returns>
		/// 
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/08/06 mcm 7.30.00 N/A	Created
        // 03/05/08 KRC 1.50.00        Adding Exception if PSEM is not valid
        // 
		public virtual PSEMResponse Write()
		{
            PSEMResponse Result = PSEMResponse.Iar;

            if (m_PSEM != null)
            {
                Result = m_PSEM.FullWrite(m_TableID, m_Data);

                if (PSEMResponse.Ok == Result)
                {
                    m_TableState = TableState.Loaded;
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

			return Result;
		}

		/// <summary>
		/// Writes a portion of the table's data to the meter.  This method
		/// does not affect the (Unloaded/Loaded/Dirty) state of the table. 
		/// </summary>
		/// <remarks>
		/// No checking is done to see if the table is Loaded or Dirty before
		/// writing it to the meter, so you can write a 0 filed, Unloaded data
		/// if you weren't careful or really wanted to.
		/// </remarks>
		/// <param name="Offset">0 based byte offset to start writing from</param>
		/// <param name="Count">the number of bytes to write</param>
		/// <returns>protocol response</returns>
		/// <overloads>Write()</overloads>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 06/08/06 mcm 7.30.00 N/A	Created
		// 03/05/08 KRC 1.50.00        Adding Exception if PSEM is not valid
        // 
		public virtual PSEMResponse Write(ushort Offset, ushort Count)
		{
			PSEMResponse Result = PSEMResponse.Iar;

            if (m_PSEM != null)
            {
                if (Count + Offset <= m_Data.Length)
                {
                    byte[] Data = new byte[Count];

                    Array.Copy(m_Data, Offset, Data, 0, Count);
                    Result = m_PSEM.OffsetWrite(m_TableID, Offset, Data);
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

			return Result;
		}

        /// <summary>
        /// Writes the table to the meter as a pending table
        /// </summary>
        /// <param name="pendingRecord">The pending event record to use for the table write</param>
        /// <returns>The result of the write.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        
        public virtual PSEMResponse PendingTableWrite(PendingEventRecord pendingRecord)
        {
            PSEMResponse Result = PSEMResponse.Iar;
            ushort PendingTableNumber = (ushort)(m_TableID | PENDING_BIT);
            byte[] Data = new byte[m_Data.Length + pendingRecord.EntireRecord.Length];

            if (m_PSEM != null)
            {
                // There is a maximum length that a pending table may be so if the table is longer we need to do multiple offset writes
                if (m_Data.Length <= MAX_PENDING_SIZE)
                {
                    Array.Copy(pendingRecord.EntireRecord, Data, pendingRecord.EntireRecord.Length);
                    Array.Copy(m_Data, 0, Data, pendingRecord.EntireRecord.Length, m_Data.Length);

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Pending Table Full Write to " + PendingTableNumber.ToString(CultureInfo.InvariantCulture));
                    Result = m_PSEM.FullWrite(PendingTableNumber, Data);
                }
                else
                {
                    ushort CurrentOffset = 0;

                    // Write the bulk of the data
                    for (int Count = 0; Count < (m_Data.Length / MAX_PENDING_SIZE); Count++)
                    {
                        Result = PendingTableWrite(pendingRecord, CurrentOffset, (ushort)MAX_PENDING_SIZE);

                        if (Result != PSEMResponse.Ok)
                        {
                            break;
                        }

                        CurrentOffset += MAX_PENDING_SIZE;
                    }

                    // Write the remaining data
                    if (CurrentOffset < m_Data.Length && Result == PSEMResponse.Ok)
                    {
                        Result = PendingTableWrite(pendingRecord, CurrentOffset, (ushort)(m_Data.Length - CurrentOffset));
                    }
                }

                if (PSEMResponse.Ok == Result)
                {
                    m_TableState = TableState.Loaded;
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

            return Result;
        }

        /// <summary>
        /// Writes the table to the meter as a pending table using an offset write
        /// </summary>
        /// <param name="pendingRecord">The pending event record to use for the table write</param>
        /// <param name="offset">The offset of the table to write</param>
        /// <param name="count">The number of bytes to write</param>
        /// <returns>The result of the write</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  11/21/11 RCG 2.53.12        Created
        
        public virtual PSEMResponse PendingTableWrite(PendingEventRecord pendingRecord, ushort offset, ushort count)
        {
            PSEMResponse Result = PSEMResponse.Iar;
            ushort PendingTableNumber = (ushort)(m_TableID | PENDING_BIT);

            if (m_PSEM != null)
            {
                if (count + offset <= m_Data.Length)
                {
                    byte[] Data = new byte[count + pendingRecord.EntireRecord.Length];

                    Array.Copy(pendingRecord.EntireRecord, Data, pendingRecord.EntireRecord.Length);
                    Array.Copy(m_Data, offset, Data, pendingRecord.EntireRecord.Length, count);

                    m_Logger.WriteLine(Logger.LoggingLevel.Detailed, "Pending Table Offset Write to " + PendingTableNumber.ToString(CultureInfo.InvariantCulture) + " Offset: " + offset.ToString(CultureInfo.InvariantCulture) + " Count: " + count.ToString(CultureInfo.InvariantCulture));

                    Result = m_PSEM.OffsetWrite(PendingTableNumber, offset, Data);
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

            return Result;
        }

		/// <summary>
		/// Writes the m_Data contents to the logger.  The contents are written
		/// as a received protocol stream with a line before and after denoting
		/// the table.
		/// </summary>
		/// <remarks>
		/// This method is intended for debugging.  It is expected that most
		/// derived tables will override this method to dump their fields.
		/// </remarks>
		/// <exception>
		/// None.  This debugging method catches its exceptions
		/// </exception>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 mcm 7.30.00 N/A	Created
		/// 
		public virtual void Dump()
		{
			try
			{
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
									"Raw Dump of Table " + m_TableID );
				m_Logger.WriteProtocol( Logger.ProtocolDirection.Receive, 
										m_Data );
				m_Logger.WriteLine( Logger.LoggingLevel.Protocol, 
									"End Dump of Table " + m_TableID );
			}
			catch( Exception e )
			{
				try
				{
					m_Logger.WriteException( this, e );
				}
				catch
				{
					// No exceptions thrown from this debugging method
				}
			}
		}

        /// <summary>
        /// Formats a byte array into a hex string
        /// </summary>
        /// <param name="data">The data to format</param>
        /// <returns>The hex string</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/26/13 jkw 2.80.10 N/A    Created
        //  11/13/13 DLG 3.50.03 TR9505 Promoted to this class to reduce duplicate code. The same method was 
        //                              defined multiple times for many different tables.
        //
        public string FormatBytesString(byte[] data)
        {
            string FormattedValue = "";

            for (int iIndex = 0; iIndex < data.Length; iIndex++)
            {
                if (data[iIndex] > 31 && data[iIndex] < 127)
                {
                    FormattedValue += (char)data[iIndex];
                }
            }

            return FormattedValue;
        }

		#endregion public methods

		#region properties

		/// <summary>
		/// The C12.19 TABLE_IDB represented as an unsigned, 16 bit integer.
		/// Note that the MFG bit is rolled into the value so MFG table 0 is
		/// interpretted as Table 2048. 
		/// </summary>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		/// 
		public ushort TableID
		{
			get
			{
				return m_TableID;
			}
		}

		/// <summary>
		/// Tables are Unloaded, Loaded, or Dirty.  Unloaded tables have never been 
		/// read or written to.  Loaded tables are synchronized with the meter.  
		/// Dirty tables are usually in a mixed state prior to writing.
		/// </summary>
		/// <remarks>
		/// This property does not depend on the loaded/unloaded/dirty state of
		/// the table.
		/// </remarks>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/09/06 mcm 7.30.00 N/A	Created
		/// 03/09/07 mcm 8.00.17 2574   Added Set property so owners can make it dirty
		public TableState State
		{
			get
			{
				return m_TableState;
			}
            set
            {
                // Allow owners of this table to reset the state based on
                // non-table related stuff like executing procedures.
                m_TableState = value;
            }
		}

		#endregion properties

		#region public definitions

		/// <summary>
		/// Unloaded (never read), Loaded (in synch with meter), or 
		/// Dirty (unknown, probably in preparation for writing)
		/// </summary>
		public enum TableState
		{
			/// <summary>
			/// An Unloaded table has never been read or written to
			/// </summary>
			Unloaded,
			/// <summary>
			/// A Loaded table is synchronized with the meter
			/// </summary>
			Loaded,
			/// <summary>
			/// A Dirty table has been written to by its clients.  Its members
			/// may or may not have been initialized with actual meter values.
			/// This is an unknown state, usually in preparation for writing to
			/// the meter.
			/// </summary>
			Dirty,
            /// <summary>
            /// A table is marked expired after a time specified in the constructor.
            /// This will allow better performance from status tables with multiple
            /// values.
            /// </summary>
            Expired
		}

        /// <summary>
        /// Value (4096) for constructing the table id of a pending table
        /// </summary>
        public const ushort PENDING_BIT = 4096;

		#endregion public definitions

        #region Private Methods

        /// <summary>
        /// Used by constructor to initialize the Member Variables
        /// </summary>
        private void InitializeMembers()
        {
            m_Logger = Logger.TheInstance;
            m_TableState = TableState.Unloaded;
        }

        /// <summary>
        /// Timer callback that marks the table as expired.
        /// </summary>
        /// <param name="state">The current state.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  05/16/08 RCG 1.50.26 N/A    Created

        private void TableExpired(object state)
        {
            m_TableState = TableState.Expired;
            m_ExpirationTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Reads the table if the current state is not loaded.
        /// </summary>
        /// <exception cref="PSEMException">Thrown if an error occurs while reading the table.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/19/08 RCG 2.00.00 N/A    Created for OpenWay

        protected void ReadUnloadedTable()
        {
            PSEMResponse Result = PSEMResponse.Ok;

            if (State != TableState.Loaded)
            {
                Result = Read();

                if (Result != PSEMResponse.Ok)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ,
                        Result, "Error reading table " + m_TableID.ToString(CultureInfo.CurrentCulture));
                }
            }
        }

        /// <summary>
        /// Changes the size of the table to the specified value
        /// </summary>
        /// <param name="size">The new size of the table.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/27/10 RCG 2.40.43 N/A    Created

        protected void ChangeTableSize(uint size)
        {
            m_Size = size;
            
            // Changeing the size of the table means that anything that has been read will be lost
            // This operation should really only be done for special cases
            m_Data = new byte[m_Size];
            m_Data.Initialize();
            m_DataStream = new MemoryStream(m_Data);
            m_Reader = new PSEMBinaryReader(m_DataStream);
            m_Writer = new PSEMBinaryWriter(m_DataStream);

            State = TableState.Unloaded;
        }

        #endregion

        #region variable declarations

        /// <summary>Protocol object</summary>
		protected CPSEM			m_PSEM;
		/// <summary>C12.19 table ID</summary>
		protected ushort		m_TableID;
		/// <summary>The official copy of the table's data</summary>
		protected byte[] 		m_Data;
		/// <summary>Size of the table</summary>
		protected uint			m_Size;
		/// <summary>Debug logger</summary>
		protected Logger		m_Logger;
		/// <summary>MemoryStream associated with the table's data array</summary>
		protected MemoryStream	m_DataStream;
		/// <summary>Binary reader associated with the table's data array</summary>
        protected PSEMBinaryReader m_Reader;
		/// <summary>Binary writer associated with the table's data array</summary>
		protected PSEMBinaryWriter m_Writer;
		/// <summary>Unloaded (never read), Loaded (in synch with meter), or 
		/// Dirty (unknown, probably in preparation for writing)</summary>
		protected TableState	m_TableState;
        /// <summary>
        /// Timer used for marking the table as expired.
        /// </summary>
        protected Timer m_ExpirationTimer;
        /// <summary>
        /// The amount of time to wait before setting the table as expired.
        /// </summary>
        protected int m_iTimeout;
        /// <summary>
        /// Indicates whether a table should be allowed to be resized base on length of data received
        /// from a full table read. It should only be set to true under special circumstances.
        /// </summary>
        protected bool m_blnAllowAutomaticTableResizing;

		#endregion variable declarations
	}

	/// <summary>
	/// The ANSISubTable handles portions of C12.19 tables that you want to
	/// treat as tables.  This has been implemented for the configuration
	/// components that are dependent on other values in the table (2048) they
	/// are contain in.
	/// </summary>
	public abstract class ANSISubTable : AnsiTable 
	{
		#region public methods

		/// <summary> Constructor</summary>
		/// <param name="psem">instance of protocol to be used</param>
		/// <param name="TableID">C12.19 table ID including MFG bit</param>
		/// <param name="Offset">byte offset of this portion the table</param>
		/// <param name="Size">Size in bytes of this portion of the table</param>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		/// 
		public ANSISubTable(CPSEM psem, ushort TableID, int Offset, ushort Size)
						: base (psem, TableID, Size )
		{
			m_SubTableOffset = Offset;
		}

        /// <summary> Constructor</summary>
        /// <param name="psem">instance of protocol to be used</param>
        /// <param name="TableID">C12.19 table ID including MFG bit</param>
        /// <param name="Offset">byte offset of this portion the table</param>
        /// <param name="Size">Size in bytes of this portion of the table</param>
        /// <param name="ExpirationTimeout">Number of Seconds until Table needs to refresh</param>
        /// 
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 11/20/09 KRC 2.30.18 N/A	Created
        /// 
        public ANSISubTable(CPSEM psem, ushort TableID, int Offset, ushort Size, int ExpirationTimeout)
            : base(psem, TableID, Size, ExpirationTimeout)
        {
            m_SubTableOffset = Offset;
        }

        /// <summary>
        /// This constructor is used for our file based inheritance
        /// </summary>
        public ANSISubTable(ushort TableID, uint Size)
            : base(TableID, Size)
        {
        }

		/// <summary>
		/// Reads the SubTable from the meter and marks it as Loaded if 
		/// successful.
		/// </summary>
		/// <returns>protocol response</returns>
		/// <overloads>Read(ushort Offset, ushort Count)</overloads>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		/// 
		public override PSEMResponse Read()
		{
			byte[] Data;
            PSEMResponse Result = PSEMResponse.Iar;

            if (m_PSEM != null)
            {
                 Result = m_PSEM.OffsetRead(m_TableID, m_SubTableOffset,
                                                        (ushort)m_Size, out Data);

                if (PSEMResponse.Ok == Result)
                {
                    if (m_Data.Length == Data.Length)
                    {
                        Array.Copy(Data, 0, m_Data, 0, Data.Length);
                        m_TableState = TableState.Loaded;
                        m_ExpirationTimer.Change(m_iTimeout, 0);

                        m_DataStream.Position = 0;

                    }
                    else
                    {
                        Result = PSEMResponse.Err;
                    }
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

			return Result;
		}

		/// <summary>
		/// Reads a portion of the table from the meter.  The table's state is
		/// NOT updated to Loaded as a result of the read.
		/// </summary>
		/// <param name="Offset">offset WITHIN the subtable to read from</param>
		/// <param name="Count">Number of bytes to read</param>
		/// <returns>protocol response</returns>
		/// <overloads>Read()</overloads>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		/// 
		public override PSEMResponse Read(int Offset, ushort Count)
		{
			return base.Read(m_SubTableOffset + Offset, Count );
		}

		/// <summary>
		/// Writes the SubTable's data to the meter. If the write succeeds, the
		/// table will be marked as Loaded.
		/// </summary>
		/// <remarks>
		/// No checking is done to see if the table is Loaded or Dirty before
		/// writing it to the meter, so you can write a 0 filed, Unloaded table
		/// if you weren't careful or really wanted to.
		/// </remarks>
		/// <returns>protocol response</returns>
		/// <overloads>Write(ushort Offset, ushort Count)</overloads>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
		/// 
		public override PSEMResponse Write()
		{
            PSEMResponse Result = PSEMResponse.Iar;

            if (m_PSEM != null)
            {
                Result = m_PSEM.OffsetWrite(m_TableID,
                                                      m_SubTableOffset,
                                                      m_Data);
                if (PSEMResponse.Ok == Result)
                {
                    m_TableState = TableState.Loaded;
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

			return Result;
		}

		/// <summary>
		/// Writes a portion of the table's data to the meter.  This method
		/// does not affect the (Unloaded/Loaded/Dirty) state of the table. 
		/// </summary>
		/// <remarks>
		/// No checking is done to see if the table is Loaded or Dirty before
		/// writing it to the meter, so you can write a 0 filed, Unloaded data
		/// if you weren't careful or really wanted to.
		/// </remarks>
		/// <param name="Offset">Offset WITNIN the table to write to</param>
		/// <param name="Count">Number of bytes to write</param>
		/// <returns>protocol response</returns>
		/// <overloads>Write()</overloads>
		/// 
		/// Revision History	
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ ---------------------------------------
		/// 06/08/06 mcm 7.30.00 N/A	Created
        /// 10/21/09 jrf 2.30.12 N/A    Removed call to base.Write().  It would fail
        ///                             when m_SubTableOffset + Offset + Count was greater 
        ///                             than m_Data.Length.  Rewrote method to use the
        ///                             internal subtable offset when comparing against 
        ///                             m_Data.Length and the full table offset when calling
        ///                             m_PSEM.OffsetWrite().
		/// 
		public override PSEMResponse Write(ushort Offset, ushort Count)
		{
			PSEMResponse Result = PSEMResponse.Iar;

            if (m_PSEM != null)
            {
                if (Count + Offset <= m_Data.Length)
                {
                    byte[] Data = new byte[Count];

                    Array.Copy(m_Data, Offset, Data, 0, Count);
                    Result = m_PSEM.OffsetWrite(m_TableID, (ushort)(m_SubTableOffset + Offset), Data);
                }
            }
            else
            {
                throw (new NotImplementedException("This Operation is not supported with a file"));
            }

            return Result;
		}

		#endregion public methods

		#region variable declarations

		/// <summary>
		/// Offset for this subtable within its C12.19 table
		/// </summary>
		protected int m_SubTableOffset;

		#endregion variable declarations
	}

}
