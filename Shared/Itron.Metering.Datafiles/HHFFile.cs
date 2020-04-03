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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Itron.Metering.Utilities;
using Itron.Metering.Progressable;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// This abstract class represents the key elements of an HHF file.  Note that these
    /// elements (header records and trailer records) are used by both MV-90 (profile) files
    /// and MIF (meter image files).  
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 12/20/06 MAH 8.00.00        Created
    // 09/06/07 RCG 9.00.07        Making changes for adding MIF LP
    abstract public class HHFFile : IProgressable, IDisposable
	{
		#region Public Events

		/// <summary>
        /// Event used to display a progress bar
        /// </summary>
        public virtual event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event used to cause a progress bar to perform a step
        /// </summary>
        public virtual event StepProgressEventHandler StepProgressEvent;

        /// <summary>
        /// Event used to hide a progress bar
        /// </summary>
        public virtual event HideProgressEventHandler HideProgressEvent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFileName">The path to the HHF file.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        protected HHFFile( String strFileName )
        {
            m_bDisposed = false;

            m_strHHFFile = strFileName;
 //           m_HHFFileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			m_HeaderRecord = null;  // Will be initialized after the header has been read
        }


		/// <summary>
		/// This method provides a means for the calling application to quickly
		/// release the associated stream handle.  The 'Dispose' method will
		/// eventually do this but this allows us to clean up after ourselves
		/// so that the file can be re-opened immediately if needed
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/26/08 MAH 9.50				    Created
		/// </remarks>
		public void Close()
		{
			if (m_HHFFileStream != null)
			{
				m_HHFFileStream.Close();
			}
		}

		/// <summary>
        /// Finalizer. Disposes all used resources prior to garbage collection
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        ~HHFFile()
        {
			Dispose(false);
		}

        /// <summary>
        /// Disposes the object by cleaning up all used resources.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created
        // 11/25/09 AF  2.30.22         Added GC.SuppressFinalize to prevent unnecessary
        //                              finalization of object after it has fallen out of scope

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the path to the HHF file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        public String FileName
        {
            get
            {
                return m_strHHFFile;
            }
        }

		/// <summary>
		/// Gets the Device Type for the current HHF file.    
		/// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

		public string DeviceType
		{
			get
			{
                if (HeaderRecord != null)
				{
                    return HeaderRecord.DeviceType;
				}
				else
				{
					return "";
				}
			}
		}

		/// <summary>
		/// Returns the device ID as a string
		/// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

		public string DeviceID
		{
			get
			{
                if (HeaderRecord != null)
				{
                    return HeaderRecord.DeviceID;
				}
				else
				{
					return "";
				}
			}
		}

        /// <summary>
        /// Returns the time the HHF file was read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/20/08 jrf 1.00.00        Created
        public DateTime TimeOfRead
        {
            get
            {
                if (HeaderRecord != null)
                {
                    return HeaderRecord.TimeOfRead;
                }
                else
                {
                    return new DateTime();
                }
            }
        }

		/// <summary>
		/// Returns true if UTC time is being used or false if it is not
		/// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

		public bool UTCTime
		{
			get
			{
                if (HeaderRecord != null)
				{
                    return HeaderRecord.UTCTime;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the Load Profile Data object from the HHF file.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        virtual public LoadProfileData LPData
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets whether or not the HHF file contains Load Profile data
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        virtual public bool ContainsLPData
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a DateTime that represents the time data collection began
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

		abstract public DateTime StartTime
		{
			get;
		}

        /// <summary>
        /// Returns a DateTime that represents the time data collection stopped
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

		abstract public DateTime StopTime
		{
			get;
		}

        /// <summary>
        /// Returns true if daylight savings changes are handled automatically
        /// or false otherwise
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

		abstract public bool DSTEnabled
		{
			get;
		}

        /// <summary>
        /// Returns a DateTime that represents the time that DST starts
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

		abstract public DateTime DSTStartTime
		{
			get;
		}

        /// <summary>
        /// Returns a DateTime that represents the time that DST stops
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

		abstract public DateTime DSTStopTime
		{
			get;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads the header data from the specified file stream    
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        virtual protected void ReadHeaderData()
        {
			if (m_HHFFileStream != null)
			{
				// The Header Record should always be the first record.
				m_HHFFileStream.Seek(0, SeekOrigin.Begin);

				HHFRecord hhfRecord = new HHFRecord();

				hhfRecord.Read(m_HHFFileStream);

				//see if the block starts with "HEADER"
				if (hhfRecord.IsHeaderRecord)
				{
					HeaderRecord = new HHFHeader(hhfRecord);
				}
			}
        }

		/// <summary>
		///     
		/// </summary>
		/// <param name="hhfHeader" type="Itron.Metering.Datafiles.HHFHeader">
		/// </param>
		virtual protected void WriteHeaderData( HHFHeader hhfHeader )
		{
			if (m_HHFFileStream != null)
			{
				// The Header Record should always be the first record.
				m_HHFFileStream.Seek(0, SeekOrigin.Begin);

				hhfHeader.Write(m_HHFFileStream);

				HeaderRecord = new HHFHeader(hhfHeader);
			}
		}


        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected virtual void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected virtual void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/31/07 RCG 9.00.06        Adding progressable support	

        protected virtual void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Disposes the object and all resources used by the object.
        /// </summary>
        /// <param name="bDisposing">Whether or not the Dispose method was called.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07 RCG 9.00.07        Created	

        protected virtual void Dispose(bool bDisposing)
        {
            if (!this.m_bDisposed)
            {
				if (m_HHFFileStream != null)
				{
					m_HHFFileStream.Close();

					if (bDisposing == true)
					{
						m_HHFFileStream = null;
					}
				}

				m_bDisposed = true;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the Header Record for the HHF file.
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/07 RCG 9.00.07        Created

        virtual protected HHFHeader HeaderRecord
        {
            get
            {
                if (m_HeaderRecord == null)
                {
                    // The header has not been read yet so read it.
                    ReadHeaderData();
                }

                return m_HeaderRecord;
            }
            set
            {
                m_HeaderRecord = value;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// path to the HHF file
        /// </summary>
        protected string m_strHHFFile;

		/// <summary>
		/// HHF file header record.    
		/// </summary>
        protected HHFHeader m_HeaderRecord;

        /// <summary>
        /// File Stream used for accessing the HHF file.
        /// </summary>
        protected FileStream m_HHFFileStream;

        /// <summary>
        /// True if the object has been disposed, false otherwise.
        /// </summary>
        protected bool m_bDisposed;

        #endregion
    }
    
    /// <summary>
    /// This class represents the basic record structure of an HHF file.  It
    /// is simply a 256 byte data block.  This class provides methods to 
    /// determine the type of record that this data block represents.  All specialized
    /// HHF data records should be derived from this class.
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 12/20/06 MAH 8.00.00        Created
    // 09/06/07 RCG 9.00.07        Made internal and removed write code
    public class HHFRecord
    {
        #region Constants
        
        /// <summary>
        /// Size of a HHF record.
        /// </summary>
        public const int HHF_RECORDSIZE = 256;

        /// <summary>
        /// 
        /// </summary>
		protected const int RECORDIDENTIFIER_LENGTH = 6;

		/// <summary>
		///     
		/// </summary>
		protected const int EOF_INDICATOR_LENGTH = 8;

        /// <summary>
        /// Char used padding in HHF files.
        /// </summary>
        protected const int HHF_PADCHAR = 0x20; 

        private const int SHORTTIMESTAMPLENGTH = 10;
        private const int LONGTIMESTAMPLENGTH = 12;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        public HHFRecord()
        {
            m_Encoder = new ASCIIEncoding();  // For converting byte array to string	
            
            m_DataBuffer = new byte[HHF_RECORDSIZE];

            for (int nIndex = 0; nIndex < HHF_RECORDSIZE; nIndex++)
                m_DataBuffer[nIndex] = HHF_PADCHAR;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the record from the specified FileStream.
        /// </summary>
        /// <param name="inputStream">The FileStream to the HHF file to read.</param>
        /// <exception cref="IOException">Thrown when file IO error occurs</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        virtual public int Read(FileStream inputStream )
        {
			int nBytesRead = 0;
			
			m_lStartRecordOffset = inputStream.Position;
            nBytesRead = inputStream.Read(m_DataBuffer, 0, HHF_RECORDSIZE);
            m_lEndRecordOffset = inputStream.Position;

			return nBytesRead;
        }

		/// <summary>
		///     
		/// </summary>
		/// <param name="outputStream" type="System.IO.FileStream">
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		virtual public void Write(FileStream outputStream)
		{
			m_lStartRecordOffset = outputStream.Position;
			outputStream.Write(m_DataBuffer, 0, HHF_RECORDSIZE);

			m_lEndRecordOffset = outputStream.Position;
		}


        #endregion

        #region Public Properties

        /// <summary>
        /// This property sets/gets the type of the current HHF record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        public Boolean IsHeaderRecord
        {
            get
            {
                String strRecordType = m_Encoder.GetString(m_DataBuffer, 0, RECORDIDENTIFIER_LENGTH);

                return ( strRecordType == "HEADER" );
            }
        }

        /// <summary>
        /// Gets whether or not the current record is a termination record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        public Boolean IsTerminationRecord
        {
            get
            {
                if ( IsHeaderRecord )
                {
                    String strEOFString = m_Encoder.GetString(m_DataBuffer, 6, EOF_INDICATOR_LENGTH);

                    return ( strEOFString == "END FILE" );
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// This property determines if a given record is a register data record.
		/// </summary>
		/// // Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 03/28/07 jrf 8.00.22  2678  Created

		public Boolean IsRegisterDataRecord
		{
			get
			{
				return ("RGDATA" == RecordID );
			}
		}

        /// <summary>
        /// This property determines if a given record is a LP termination record.
        /// </summary>
        /// // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/28/07 jrf 8.00.22  2678  Created

        public Boolean IsLPTerminationRecord
        {
            get
            {
                Boolean blnIsTermRecord = false;
                Int16 iTermValue = (Int16)((m_DataBuffer[0] << 8) + m_DataBuffer[1]);
                
                // By definition the first two bytes of a 
                // pulse data termination record will be -1
                if (-1 == iTermValue)
                {
                    blnIsTermRecord = true;
                }

                return blnIsTermRecord;
            }
        }

        /// <summary>
        /// This property determines if a given record is a event data termination record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created

        public Boolean IsEventDataTerminationRecord
        {
            get
            {
                Boolean blnIsTermRecord = false;
                Int16 iTermValue = BitConverter.ToInt16(m_DataBuffer, 0);

                // By definition the first two bytes of a 
                // event data termination record will be -2
                if (-2 == iTermValue)
                {
                    blnIsTermRecord = true;
                }

                return blnIsTermRecord;
            }
        }

        /// <summary>
        /// Gets the buffer for the current record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        public byte[] DataBuffer
        {
            get
            {
                return m_DataBuffer;
            }
        }

        /// <summary>
        /// Returns the record ID as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created

        internal string RecordID
        {
            get
            {
                return m_Encoder.GetString(m_DataBuffer, 0, RECORDIDENTIFIER_LENGTH);
            }
        }//RecordID

        /// <summary>
        /// Gets the stream offset to the start of the record
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        internal long StartStreamOffset
        {
            get
            {
                return m_lStartRecordOffset;
            }
        }

        /// <summary>
        /// Gets the stream offset to the end of the record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        internal long EndStreamOffset
        {
            get
            {
                return m_lEndRecordOffset;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method extracts a Date/Time in the format "MMddyyyyHHmm".
        /// </summary>
        /// <param name="bufferOffset">The offset from the start of the data buffer.</param>
        /// <returns>The extracted Date/Time</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/28/07 jrf 8.00.22	 2768  Sometimes extracted dates will be all zeros, 
        //                             such as when meter does not use DST.  This will
        //                             cause an exception when the Date/Time is 
        //                             constructed.  In this case we will
        //                             catch the exception and return a default
        //                             Date/Time.
        // 02/26/08 jrf 10.00.00       Added culture info to Parse methods.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse methods for compatiblity
        //                             with the compact framework.
		// 03/11/08 mah 10.00.00       Addressed SCR 3591 by handling 24:00 as a representation
		//							   of midnight
        //
        protected DateTime ExtractLongDateTime( int bufferOffset )
        {
            DateTime ExtractedDate;

			try
			{
                checked
                {
                    int nMonth = Int16.Parse(m_Encoder.GetString(m_DataBuffer, bufferOffset, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int nDay = Int16.Parse(m_Encoder.GetString(m_DataBuffer, bufferOffset + 2, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int nYear = Int16.Parse(m_Encoder.GetString(m_DataBuffer, bufferOffset + 4, 4),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int nHour = Int16.Parse(m_Encoder.GetString(m_DataBuffer, bufferOffset + 8, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    int nMinute = Int16.Parse(m_Encoder.GetString(m_DataBuffer, bufferOffset + 10, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);

                    // MAH - Some older HHF Files may use 24:00 to represent midnight.  
                    // change this to 00:00 of the following date.

                    if (nHour == 24 && nMinute == 00)
                    {
                        ExtractedDate = new DateTime(nYear, nMonth, nDay, 0, 0, 0);
                        ExtractedDate = ExtractedDate.AddDays(1);
                    }
                    else
                    {
                        ExtractedDate = new DateTime(nYear, nMonth, nDay, nHour, nMinute, 0);
                    }
                }
            }
            catch (Exception)
            {
                ExtractedDate = new DateTime();
            }

            return ExtractedDate;
        }

		/// <summary>
		///     
		/// </summary>
		/// <param name="dtValue" type="System.DateTime">
		/// </param>
		/// <param name="bufferOffset" type="int">
		/// </param>
		protected void EncodeLongDateTime(DateTime dtValue, int bufferOffset)
		{
            checked
            {
                m_Encoder.GetBytes(dtValue.ToString("MM", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, bufferOffset);
                m_Encoder.GetBytes(dtValue.ToString("dd", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, bufferOffset + 2);
                m_Encoder.GetBytes(dtValue.ToString("yyyy", CultureInfo.InvariantCulture), 0, 4, m_DataBuffer, bufferOffset + 4);
                m_Encoder.GetBytes(dtValue.ToString("HH", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, bufferOffset + 8);
                m_Encoder.GetBytes(dtValue.ToString("mm", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, bufferOffset + 10);
            }
		}

        #endregion

        #region Members

        /// <summary>
		/// The data buffer for the record.    
		/// </summary>

        protected byte[] m_DataBuffer;
		
		/// <summary>
		/// The encoder for the file.    
		/// </summary>

		protected ASCIIEncoding m_Encoder;

        /// <summary>
        /// The starting offset of the Record in the FileStream
        /// </summary>
        protected long m_lStartRecordOffset;

        /// <summary>
        /// The ending offset of the Record in the FileStream
        /// </summary>
        protected long m_lEndRecordOffset;
        #endregion 
    }

    /// <summary>
    /// Represents a header record from a HHF file
    /// </summary>
    public class HHFHeader : HHFRecord 
    {
        #region Constants
		/// <summary>
		/// Defines the number of channels that each header record can represent    
		/// </summary>
		public const int MAXCHANNELSPERRECORD = 4;

        private const int MAXDEVICETYPELENGTH = 8;
        private const int MAXRECORDERIDLENTH = 14;
        private const int MAXDEVICEIDLENGTH = 20;
        private const int TIMEOFREADLENGTH = 10;

        private const int DEVICETYPEOFFSET = 0x06;
        private const int RECORDERIDOFFSET = 0x0E;
        private const int DEVICEIDOFFSET = 0x1C;
        private const int UTCFLAGOFFSET = 0xF4;
        private const int READ_TIME_MONTH_OFFSET = 0x30;
        private const int READ_TIME_DAY_OFFSET = 0x32;
        private const int READ_TIME_YEAR_OFFSET = 0x34;
        private const int READ_TIME_HOUR_OFFSET = 0x36;
        private const int READ_TIME_MINUTE_OFFSET = 0x38;

		private const int CHANNELID_1_OFFSET = 0x3A;
		private const int CHANNELID_2_OFFSET = 0x64;
		private const int CHANNELID_3_OFFSET = 0x9A;
		private const int CHANNELID_4_OFFSET = 0xC4;

        private const int BASE_YEAR = 2000;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to create an instance of the HHF Header
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/24/06 rdb N/A	 N/A	Creation of class

        internal HHFHeader( HHFRecord headerRecord ) : base()
        {
            // copy the data from the original record to the this class

            m_lStartRecordOffset = headerRecord.StartStreamOffset;
            m_lEndRecordOffset = headerRecord.EndStreamOffset;

            for (int nDataIndex = 0; nDataIndex < HHF_RECORDSIZE; nDataIndex++)
            {
                m_DataBuffer[nDataIndex] = headerRecord.DataBuffer[nDataIndex];
            }
        }


		/// <summary>
		/// This constructor should only be used for writing new 
		/// header records
		/// </summary>
		/// <param name="nStartChannel" type="int">
		/// This parameter identifies the first load profile channel that is 
		/// represented in the header record
		/// </param>
		/// <param name="nNumChannels" type="int">
		/// This parameter indicates the number of load profile channels that
		/// are represented in the header record
		/// </param>
		// Revision History
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 08/12/08 mah 9.50	N/A	   created
		internal HHFHeader(int nStartChannel, int nNumChannels)
			: base()
		{
			m_Encoder.GetBytes("HEADER", 0, 6, m_DataBuffer, 0);
			DeviceType = "PROFILE";

			int nChannelID = nStartChannel;

			m_Encoder.GetBytes(nChannelID.ToString("00", CultureInfo.InvariantCulture), 
								0, 2, m_DataBuffer, CHANNELID_1_OFFSET);

			if (nNumChannels >= 2)
			{
				nChannelID++;
				m_Encoder.GetBytes(nChannelID.ToString("00", CultureInfo.InvariantCulture), 
								0, 2, m_DataBuffer, CHANNELID_2_OFFSET);
			}

			if (nNumChannels >= 3)
			{
				nChannelID++;
				m_Encoder.GetBytes(nChannelID.ToString("00", CultureInfo.InvariantCulture), 
								0, 2, m_DataBuffer, CHANNELID_3_OFFSET);
			}

			if (nNumChannels == 4)
			{
				nChannelID++;
				m_Encoder.GetBytes(nChannelID.ToString("00", CultureInfo.InvariantCulture), 
								0, 2, m_DataBuffer, CHANNELID_4_OFFSET);
			}
		}

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Returns the device type as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public string DeviceType
        {
            get
            {
                return m_Encoder.GetString(m_DataBuffer, DEVICETYPEOFFSET, MAXDEVICETYPELENGTH);
            }
			private set
			{
				m_Encoder.GetBytes(value, 0, value.Length, m_DataBuffer, DEVICETYPEOFFSET);
			}
        }

        /// <summary>
        /// Returns the device ID as a string
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public string DeviceID
        {
            get
            {
                return m_Encoder.GetString(m_DataBuffer, DEVICEIDOFFSET, MAXDEVICEIDLENGTH);
            }
			set
			{
				if (value.Length > MAXDEVICEIDLENGTH)
				{
					m_Encoder.GetBytes(value, 0, MAXDEVICEIDLENGTH, m_DataBuffer, DEVICEIDOFFSET);
				}
				else
				{
					m_Encoder.GetBytes(value, 0, value.Length, m_DataBuffer, DEVICEIDOFFSET);
				}
			}
        }

        /// <summary>
        /// Returns true if UTC time is being used or false if it is not
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
		public bool UTCTime
        {
            get
            {
                return (DataBuffer[UTCFLAGOFFSET] == (byte)'Y');
            }
			set
			{
				if (value)
				{
					DataBuffer[UTCFLAGOFFSET] = (byte)'Y';
				}
				else
				{
					DataBuffer[UTCFLAGOFFSET] = (byte)'N';
				}
			}
        }

        /// <summary>
        /// Returns the time the HHF file was read.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/20/08 jrf 10.00.00        Created
        // 02/26/08 jrf 10.00.00       Added culture info to Parse methods.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse methods for compatiblity
        //                             with the compact framework.
        // 08/13/08 mah 9.50.00        Added 'set' accessor
        public DateTime TimeOfRead
        {
            get
            {
                DateTime dtReadTime;
                ASCIIEncoding Encoder = new ASCIIEncoding();

                try
                {
                    //Date is in the format YYYYMMDDhhmm (ascii)
                    dtReadTime = new DateTime(
                        BASE_YEAR + Int16.Parse(Encoder.GetString(m_DataBuffer, READ_TIME_YEAR_OFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture),
                        Int16.Parse(Encoder.GetString(m_DataBuffer, READ_TIME_MONTH_OFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture),
                        Int16.Parse(Encoder.GetString(m_DataBuffer, READ_TIME_DAY_OFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture),
                        Int16.Parse(Encoder.GetString(m_DataBuffer, READ_TIME_HOUR_OFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture),
                        Int16.Parse(Encoder.GetString(m_DataBuffer, READ_TIME_MINUTE_OFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture),
                        0);
                }
                catch
                {
                    dtReadTime = new DateTime();
                }

                return dtReadTime;
            }
			set
			{
				m_Encoder.GetBytes(value.ToString("yy", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, READ_TIME_YEAR_OFFSET);
				m_Encoder.GetBytes(value.ToString("MM", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, READ_TIME_MONTH_OFFSET);
				m_Encoder.GetBytes(value.ToString("dd", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, READ_TIME_DAY_OFFSET);
				m_Encoder.GetBytes(value.ToString("HH", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, READ_TIME_HOUR_OFFSET);
				m_Encoder.GetBytes(value.ToString("mm", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, READ_TIME_MINUTE_OFFSET);
			}
        }

        #endregion

    }//HHFHeader

	/// <summary>
	/// Represents a header record from a HHF file
	/// </summary>
	public class HHFTerminationRecord : HHFRecord
	{
		#region Constructors

		/// <summary>
		///     
		/// </summary>
		internal HHFTerminationRecord()
			: base()
		{
			m_Encoder.GetBytes("HEADER", 0, 6, m_DataBuffer, 0);
			m_Encoder.GetBytes("END FILE", 0, EOF_INDICATOR_LENGTH, m_DataBuffer, 6);
		}

		#endregion
	}


    /// <summary>
    /// HHF header for pulse data.
    /// </summary>
    // Revision History
    // MM/DD/YY who Version Issue# Description
    // -------- --- ------- ------ ---------------------------------------
    // 12/20/2006 MAH 8.00.00 Created
	// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
    internal class HHFPulseDataHeaderRecord : HHFRecord
    {
        #region Definitions

        /// <summary>
        /// The formatting options for the pulse data
        /// </summary>
        internal enum IntervalFormatOptions
        {
            Integer = 0,
            FloatingPoint = 1
        };

        #endregion

        #region Constants

        private const int MAXDEVICEIDLENGTH = 20;

        private const int DEVICEIDOFFSET = 0x00;
        private const int INTERVALLENGTHOFFSET = 0x23;
        private const int CHANNELCOUNTOFFSET = 0x25;
        private const int STARTTIMEOFFSET = 0x27;
        private const int STOPTIMEOFFSET = 0x33;
        private const int DSTFLAGOFFSET = 0xDF;
        private const int DSTSTARTOFFSET = 0xE0;
        private const int DSTSTOPOFFSET = 0xEC;
        private const int HEMISPHEREFLAGOFFSET = 0xF8;
        private const int EVENTSIZEOFFSET = 0xF9;
        private const int INTERVALFORMATOFFSET = 0xFB;
        private const int CHANNELSTATUSMASKOFFSET = 0xFC;

        #endregion

        #region Public Methods
        /// <summary>
        /// Constructor to create an instance of the HHF Header
        /// </summary>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/20/06 MAH N/A	 N/A	Creation of class
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
        public HHFPulseDataHeaderRecord(HHFRecord headerRecord) : base()
        {
            // copy the data from the original record to the this class

            m_lStartRecordOffset = headerRecord.StartStreamOffset;
            m_lEndRecordOffset = headerRecord.EndStreamOffset;

            for (int nDataIndex = 0; nDataIndex < HHF_RECORDSIZE; nDataIndex++)
            {
                m_DataBuffer[nDataIndex] = headerRecord.DataBuffer[nDataIndex];
            }

			m_DeviceID = new CachedString();
			m_IntervalLength = new CachedInt();
			m_ChannelCount = new CachedInt();
			m_StartTime = new CachedDate();
			m_StopTime = new CachedDate();
			m_DSTStartTime = new CachedDate();
			m_DSTStopTime = new CachedDate();
			m_EventSize = new CachedInt();

        }

		/// <summary>
		/// This constructor should be used when creating a new MV-90 HHF file.
		/// It initializes the fixed values in the record and initializes the 
		/// underlying data buffer.
		/// </summary>
		public HHFPulseDataHeaderRecord()
			: base()
		{
			m_DeviceID = new CachedString();
			m_IntervalLength = new CachedInt();
			m_ChannelCount = new CachedInt();
			m_StartTime = new CachedDate();
			m_StopTime = new CachedDate();
			m_DSTStartTime = new CachedDate();
			m_DSTStopTime = new CachedDate();
			m_EventSize = new CachedInt();

			EventSize = 14; // This is a fixed value

			// Always assume that the file was created in the northern 
			// hemisphere - unfortunately we have no way of knowing any 
			// differently
			m_DataBuffer[HEMISPHEREFLAGOFFSET] = (byte)'N';

			// No channel statuses allowed
			m_DataBuffer[CHANNELSTATUSMASKOFFSET] = 0;
			m_DataBuffer[CHANNELSTATUSMASKOFFSET + 1] = 0;

			// Only efile with interval statuses are supported
			m_DataBuffer[INTERVALFORMATOFFSET] = 0x03;
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the DeviceID from the header.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public String DeviceID
        {
			get
			{
				if (!m_DeviceID.Cached)
				{
					m_DeviceID.Value = m_Encoder.GetString(m_DataBuffer, DEVICEIDOFFSET, MAXDEVICEIDLENGTH);
				}

				return m_DeviceID.Value;
			}
			set
			{
				m_DeviceID.Value = value; // just to populate the cached item

				if (m_DeviceID.Value.Length > MAXDEVICEIDLENGTH)
				{
					m_Encoder.GetBytes(m_DeviceID.Value, 0, MAXDEVICEIDLENGTH, m_DataBuffer, DEVICEIDOFFSET);
				}
				else
				{
					m_Encoder.GetBytes(m_DeviceID.Value, 0, m_DeviceID.Value.Length, m_DataBuffer, DEVICEIDOFFSET);
				}
			}

        }

        /// <summary>
        /// Gets or sets the interval length from the header.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        // 02/26/08 jrf 10.00.00       Added culture info to Parse method.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse method for compatiblity
        //                             with the compact framework.
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		//
        public int IntervalLength
        {
            get
            {
				if (!m_IntervalLength.Cached)
				{
					// The output value is the interval length yet the header
					// data is actually the number of intervals per hour

					int intervalsPerHour =
                        Int16.Parse(m_Encoder.GetString(m_DataBuffer, INTERVALLENGTHOFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);

					if (intervalsPerHour > 0)
					{
						m_IntervalLength.Value = (int)(60 / intervalsPerHour);
					}
					else
					{
						m_IntervalLength.Value = 0;
					}
				}

				return m_IntervalLength.Value;
            }
			set
			{
				int intervalsPerHour;
				
				m_IntervalLength.Value = value;

				if (m_IntervalLength.Value > 0)
				{
					intervalsPerHour = 60 / m_IntervalLength.Value;
				}
				else
				{
					intervalsPerHour = 0;
				}

				m_Encoder.GetBytes(intervalsPerHour.ToString("00", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, INTERVALLENGTHOFFSET);
			}
        }
        
        /// <summary>
        /// Gets or sets the number of channels from the header.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
        // 02/26/08 jrf 10.00.00       Added culture info to Parse method.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse method for compatiblity
        //                             with the compact framework.
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public int ChannelCount
        {
            get
            {
				if (!m_ChannelCount.Cached)
				{
					m_ChannelCount.Value =
                        Int16.Parse(m_Encoder.GetString(m_DataBuffer, CHANNELCOUNTOFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
				}

				return m_ChannelCount.Value;
            }
			set
			{
				m_ChannelCount.Value = value;

				m_Encoder.GetBytes(m_ChannelCount.Value.ToString("00", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, CHANNELCOUNTOFFSET);
			}
        }

        /// <summary>
        /// Returns the StartTime exactly as it is in the file.  TIMs will set
        /// the start time at 1 minute past the previous interval's EOI. 
        /// PC-PRO+ will set the start time at the previous interval's EOI.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public DateTime StartTime
        {
            get
            {
				if (!m_StartTime.Cached)
				{
					m_StartTime.Value = ExtractLongDateTime(STARTTIMEOFFSET);
				}

				return m_StartTime.Value;
            }
			set
			{
				m_StartTime.Value = value;

				EncodeLongDateTime(m_StartTime.Value, STARTTIMEOFFSET);
			}
        }
        
        /// <summary>
        /// Gets or sets the stop time of the data
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public DateTime StopTime
        {
            get
            {
				if (!m_StopTime.Cached)
				{
					m_StopTime.Value = ExtractLongDateTime(STOPTIMEOFFSET);
				}

				return m_StopTime.Value;
            }
			set
			{
				m_StopTime.Value = value;

				EncodeLongDateTime(m_StopTime.Value, STOPTIMEOFFSET);
			}

        }
        
        /// <summary>
        /// Gets or sets whether DST was enabled
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public bool DSTEnabled
        {
            get
            {
				return (m_DataBuffer[DSTFLAGOFFSET] == (byte)'Y');
            }
			set
			{
				if (value)
				{
					m_DataBuffer[DSTFLAGOFFSET] = (byte)'Y';
				}
				else
				{
					m_DataBuffer[DSTFLAGOFFSET] = (byte)'N';
				}
			}

        }
        
        /// <summary>
        /// Gets or sets the DST start time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public DateTime DSTStartTime
        {
            get
            {
				if (!m_DSTStartTime.Cached)
				{
					m_DSTStartTime.Value = ExtractLongDateTime(DSTSTARTOFFSET);
				}

				return m_DSTStartTime.Value;
            }
			set
			{
				m_DSTStartTime.Value = value;

				EncodeLongDateTime(m_DSTStartTime.Value, DSTSTARTOFFSET);
			}
        }
        
        /// <summary>
        /// Gets or sets the DST stop time.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		public DateTime DSTStopTime
        {
            get
            {
				if (!m_DSTStopTime.Cached)
				{
					m_DSTStopTime.Value = ExtractLongDateTime(DSTSTOPOFFSET);
				}

				return m_DSTStopTime.Value;
            }
			set
			{
				m_DSTStopTime.Value = value;

				EncodeLongDateTime(m_DSTStopTime.Value, DSTSTOPOFFSET);
			}

        }
        
        /// <summary>
        /// Gets or sets the size of the event.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
		// 11/09/2007 MAH 9.00.24 Added cached member variables for performance improvement
        // 02/26/08 jrf 10.00.00       Added culture info to Parse methods.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse method for compatiblity
        //                             with the compact framework.
		// 08/13/08 mah 9.50.00        Added 'set' accessor
		//
		public int EventSize
        {
            get
            {
				if (!m_EventSize.Cached)
				{
					m_EventSize.Value =
                        Int16.Parse(m_Encoder.GetString(m_DataBuffer, EVENTSIZEOFFSET, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
				}

				return m_EventSize.Value;
            }
			private set // this is private because it is a fixed value for new files
			{
				m_EventSize.Value = value;

				m_Encoder.GetBytes(m_EventSize.Value.ToString( "00", CultureInfo.InvariantCulture), 0, 2, m_DataBuffer, EVENTSIZEOFFSET);
			}
        }

        /// <summary>
        /// Gets or sets the interval format
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        public IntervalFormatOptions IntervalFormat
        {
            get
            {
                if ((m_DataBuffer[INTERVALFORMATOFFSET] & 0x02) != 0)
                {
                    return IntervalFormatOptions.FloatingPoint;
                }
                else
                {
                    return IntervalFormatOptions.Integer;
                }
            }
        }

        /// <summary>
        /// Gets whether or not an interval status is present.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        public bool IntervalStatusesPresent
        {
            get
            {
				return ((m_DataBuffer[INTERVALFORMATOFFSET] & 0x01) != 0);
            }
        }

        /// <summary>
        /// Gets an array of bools that indicates whether or not a status is present
        /// for each of the channels.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 			                   Created
        public bool[] ChannelStatusPresent
        {
            get
            {
                bool[] boolStatusArray = new bool[16];

                int nBitMask = 0x01; // start at the lowest order bit

                for (int nBitIndex = 0; nBitIndex < 16; nBitIndex++)
                {
                    boolStatusArray[ nBitIndex ] = (m_DataBuffer[ CHANNELSTATUSMASKOFFSET ] & nBitMask) != 0;

                    nBitMask = nBitMask << 1;
                }

                return boolStatusArray;
            }
        }

        #endregion

		#region Private Members

		private CachedString m_DeviceID;
		private CachedInt m_IntervalLength;
		private CachedInt m_ChannelCount;
		private CachedDate m_StartTime;
		private CachedDate m_StopTime;
		private CachedDate m_DSTStartTime;
		private CachedDate m_DSTStopTime;
		private CachedInt m_EventSize;

		#endregion

	}

	/// <summary>
	/// Represents a header record from a HHF file
	/// </summary>
	public class HHFPulseTerminationRecord : HHFRecord
	{
		#region Constructors

		/// <summary>
		///     
		/// </summary>
		internal HHFPulseTerminationRecord()
			: base()
		{
			// Set the entire block to 0xFFFF

			for (int nIndex = 0; nIndex < HHF_RECORDSIZE; nIndex++)
				m_DataBuffer[nIndex] = 0xFF;
		}

		#endregion
	}

}
