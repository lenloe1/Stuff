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
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Device;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.DST;
using Itron.Metering.Utilities;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Represents an MV-90 HHF file.  This is a read-only implementation and has the ability to return a 
    /// load profile data object containing the data from the file
    /// </summary>
    public class MV90_HHF : HHFFile
    {
        #region Constants

		/// <summary>
		/// Defines the maximum number of load profile channels that are allowed
		/// in an MV-90 HHF file
		/// </summary>
		public const int MAX_CHANNELS = 16;

        private const string CENTRON_OPENWAY_REG_DATA = "AMI";
        private const string CENTRON_IMAGE_REG_DATA = "CENT";
        private const string SENTINEL_REG_DATA = "SNTL";
        private const string END_OF_EVENT_RECORD_CODE = "00";
        private const byte NON_FATAL_ERROR_1_MASK = 0x01;
        private const byte NON_FATAL_ERROR_2_MASK = 0x02;
        private const byte NON_FATAL_ERROR_3_MASK = 0x04;
        private const byte NON_FATAL_ERROR_4_MASK = 0x08;
        private const byte NON_FATAL_ERROR_5_MASK = 0x10;
        private const byte NON_FATAL_ERROR_6_MASK = 0x20;
        private const byte DEM_THRESHOLD_EXCEEDED_MASK = 0x40;
        private const byte FATAL_ERROR_4_MASK = 0x80;
        private const byte FATAL_ERROR_1_MASK = 0x01;
        private const byte FATAL_ERROR_2_MASK = 0x02;
        private const byte FATAL_ERROR_3_MASK = 0x04;
        private const byte FATAL_ERROR_5_MASK = 0x10;
        private const byte FATAL_ERROR_6_MASK = 0x20;
        private const byte FATAL_ERROR_7_MASK = 0x40;
        private const int REGISTER_READ_HEADER_SIZE = 4;
        private const int REGISTER_RECORD_DATA_LENGTH = 250;
        private const int REGISTER_RECORD_ID_LENGTH = 6;
        private const int FIRST_REGISTER_READ_OFFSET = 22;
        private const int REG_DATA_YEAR_OFFSET = 8;
        private const int REG_DATA_MONTH_OFFSET = 12;
        private const int REG_DATA_DAY_OFFSET = 14;
        private const int REG_DATA_HOUR_OFFSET = 16;
        private const int REG_DATA_MINUTE_OFFSET = 18;
        private const int SENT_IMAGE_BASE_YEAR = 2000;
        private const int OPENWAY_BASE_YEAR = 1970;
        private const int IMAGE_MONO_CAPABLITIES_REC_LEN = 7;
        private const int EVENT_DATE_TIME_LEN = 10;
        private const int EVENT_CODE_LEN = 2;
        private const int REG_DATA_FORMAT_OFFSET = 20;
        private const int REG_DATA_FORMAT_LEN = 8;
        private const int DEVICE_ID_LEN = 20;
        private const int EVENT_BASE_YEAR = 2000;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor to make an HHFFile object
        /// </summary>
		/// <param name="strFileName">full path to the HHF file</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/14/06 RDB				   Created
        // 02/26/08 jrf 10.00.00       Added member variable.
		public MV90_HHF(string strFileName)
			: base(strFileName)
        {
	        m_HHFFileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            ProfileHeader = null;
            m_ContainsRGData = new CachedBool();
            m_ContainsEventData = new CachedBool();
            m_RGDataFormat = new CachedString();
        }


		/// <summary>
		/// This constructor should only be used when creating a new MV-90 file.
		/// It cannot be used to read existing files.  Note that the file
		/// will be created and written during the construction of the object. Although
		/// no further actions need to be take to write the data, the file should be 
		/// closed by the calling application rather than waiting for the 
		/// garbage collector to release the file handle.
		/// </summary>
		/// <param name="strFileName" type="string">
		/// The full path name of the file to be created
		/// </param>
		/// <param name="dtReadTime">
		/// </param>
		/// <param name="strUnitID" type="string">
		/// The meter's unit ID to be written to the HHF file
		/// </param>
		/// <param name="lpData" type="Itron.Metering.DeviceDataTypes.LoadProfileData">
		/// The load profile data to be written to the file
		/// </param>
		/// <param name="boolDSTEnabled">
		/// </param>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 08/10/08 mah  9.50          Added constructor
		public MV90_HHF(string strFileName,
						DateTime dtReadTime,
						String strUnitID, 
						LoadProfileData lpData,
						Boolean boolDSTEnabled )
			: base(strFileName)
		{
			m_HHFFileStream = new FileStream(strFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

			ProfileHeader = null;

			// The current implementation does not support creating register
			// or event records
			m_ContainsRGData = new CachedBool();
			m_ContainsRGData.Value = false;

			m_ContainsEventData = new CachedBool();
			m_ContainsEventData.Value = false;

			m_RGDataFormat = new CachedString();
			m_RGDataFormat.Value = ""; // Indicates that no register data is available

			// Every MV-90 file must start with one or more header records
			WriteFileHeaders(dtReadTime, strUnitID, lpData.NumberOfChannels);

			DateTime dtLPEndTime = lpData.EndTime;
			DateTime dtDSTStart = DateTime.MinValue;
			DateTime dtDSTStop = DateTime.MinValue;
			
			// Since we can't rely on the DST dates from the data source being correct,
			// we need to retrieve them from the system calendar. 
			if (boolDSTEnabled)
			{
				GetDSTDates(dtLPEndTime, out dtDSTStart, out dtDSTStop);
			}
			
			WriteProfileHeaders(strUnitID, lpData, boolDSTEnabled, dtDSTStart, dtDSTStop );

			WriteProfileDataRecords(lpData);

			// Lastly, the HHF file must be closed out with a termination record
			HHFTerminationRecord hhfTerminator = new HHFTerminationRecord();
			hhfTerminator.Write(m_HHFFileStream);
		}




        /// <summary>
        /// Finalizer. Disposes all used resources prior to garbage collection
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created
        // 12/19/08 jrf 9.50.28 124664 HHF file stream was not being properly closed and was 
        //                             causing issues manipulating file.
        // 01/06/08 jrf 9.50.29 124664/125192 Removed unnecessary closing of the stream since it was being closed 
        //                             in the dispose method.  However closing a binary reader used in the LPData 
        //                             property prevented HHF files from being opened in the HHF viewer.  It was 
        //                             necessary to not close that binary reader there and to call GC.Collect here 
        //                             to make sure the hold on the HHF file is released.  
        //                                  
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect")]
        ~MV90_HHF()
        {
            Dispose(false);

            GC.Collect();
        }

        /// <summary>
        /// Returns true if the file is an HHF file and false otherwise
        /// </summary>
        /// <param name="FileName">path of the file</param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 12/20/06 MAH 8.00.00		   Created
        // 04/11/07 jrf 8.00.29 2881   The device type was being compared against
        //                             "MV90" instead of "MV90    "
        public static bool IsMV90HHFFile(string FileName)
        {
            bool boolIsMV90File = false; // guilty until proven innocent

			try
			{
				FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

				HHFRecord hhfRecord = new HHFRecord();

				hhfRecord.Read(file);

				if (hhfRecord.IsHeaderRecord)
				{
					HHFHeader hhfHeader;

					hhfHeader = new HHFHeader(hhfRecord);

					boolIsMV90File = (hhfHeader.DeviceType == "PROFILE ") || (hhfHeader.DeviceType == "MV90    ");
				}

				file.Close();
			}

			catch
			{
				boolIsMV90File = false; // Since we couldn't read the file we have to assume that it is not an MV90 HHF
			}
            
			return boolIsMV90File;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if the file is an HHF file and contains LP data.  It 
        /// returns false otherwise.
        /// </summary>
        /// <returns>Whether or not the given HHF file contains LP data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 03/28/07 jrf 8.00.22  2678  Created
        // 09/06/07 RCG 9.00.07        Changed to member function and promoted to HHFFile

        override public bool ContainsLPData
        {
            get
            {
                return ProfileHeader != null;
            }
        }

        /// <summary>
        /// Returns true if the file is an HHF file and contains event data.  It 
        /// returns false otherwise.
        /// </summary>
        /// <returns>Whether or not the given HHF file contains event data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/19/08 jrf 1.00.00        Created
        // 06/23/08 jrf 1.50.41 115895 Checked for case when the event data termination 
        //                             record was there but there was no event data.
        public bool ContainsEventData
        {
            get
            {
                if (!m_ContainsEventData.Cached)
                {
                    HHFRecord hhfRecord = new HHFRecord();

                    m_ContainsEventData.Value = false;

                    if (null != ProfileHeader)
                    {
                        // Seek to just after the pulse data header.
                        m_HHFFileStream.Seek(ProfileHeader.EndStreamOffset, SeekOrigin.Begin);

                        //Read until the pulse data termination record
                        while (0 < hhfRecord.Read(m_HHFFileStream) && !hhfRecord.IsLPTerminationRecord) {}

                        //read the next record after the puse data termination record
                        hhfRecord.Read(m_HHFFileStream);

                        //if we hit the event data termination record first then there are no events
                        if (hhfRecord.IsEventDataTerminationRecord || hhfRecord.IsTerminationRecord)
                        {
                            m_ContainsEventData.Value = false;
                        }
                        else
                        {
                            //Search through the records looking for an event data termination record
                            while (0 < hhfRecord.Read(m_HHFFileStream) && !hhfRecord.IsTerminationRecord)
                            {
                                if (hhfRecord.IsEventDataTerminationRecord)
                                {
                                    m_ContainsEventData.Value = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                return m_ContainsEventData.Value;
            }
        }

        /// <summary>
        /// Returns true if the file is an HHF file and contains register data.  It 
        /// returns false otherwise.
        /// </summary>
        /// <returns>Whether or not the given HHF file contains register data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/19/08 jrf 10.00.00        Created
        // 02/26/08 jrf 10.00.00        Added code to set the register data format.
        public bool ContainsRGData
        {
            get
            {
                if (!m_ContainsRGData.Cached)
                {
                    HHFRecord hhfRecord = new HHFRecord();
                    
                    m_ContainsRGData.Value = false;

                    if (null != HeaderRecord)
                    {
                        // Seek to just after the header.
                        m_HHFFileStream.Seek(HeaderRecord.EndStreamOffset, SeekOrigin.Begin);

                        //Search through the records looking for a register data record
                        while (HHFRecord.HHF_RECORDSIZE == hhfRecord.Read(m_HHFFileStream)
                            && !hhfRecord.IsLPTerminationRecord && !hhfRecord.IsTerminationRecord)
                        {                          
                            if (hhfRecord.IsRegisterDataRecord)
                            {
                                m_ContainsRGData.Value = true;
                                m_RGDataFormat.Value = GetRegisterDataFormat(hhfRecord);
                                break;
                            }
                        }
                    }
                }

                return m_ContainsRGData.Value;
            }
        }

        /// <summary>
        /// Returns the format that register data is in.
        /// </summary>
        /// <returns>The format of the register data.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/19/08 jrf 10.00.00        Created
        public string RGDataFormat
        {
            get
            {
                if (!m_RGDataFormat.Cached)
                {
                    if (!ContainsRGData)
                    {
                        m_RGDataFormat.Value = "";
                    }
                }

                return m_RGDataFormat.Value;
            }
        }

        
		/// <summary>
		/// Returns a set of load profile data based on the HHF file
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  11/14/06 RDB				   Created
		//  03/28/07 jrf 8.00.22 2768   There is a possibility that there will be 
		//                              event data records following the LP 
		//                              records and before the HHF termination record.
		//                              These records we being processed as 
		//                              pulse data records.  Code was changed 
		//                              to set and check for the end of LP so 
		//                              that these records will be skipped.
		//                              Also changed to only instantiate LP
		//                              object if we found LP data.
		//  04/11/07 jrf 8.00.29 2881   the header record comparison was changed 
		//                              to compare against "MV90    " as well.  Also
		//                              the check for LP Pulse Data Header Record was
		//                              changed to compare it's device ID against the 
		//                              device ID of header record to be sure we 
		//                              have the right record.
		//	07/30/07 mrj 9.00.00		Changed to return a new data type.
        // 12/19/08 jrf 9.50.28 124664  Binary reader was not being properly closed and was 
        //                              causing issues manipulating file.
        // 01/05/09 jrf 9.50.29 125192  The previous fix caused an error opening HHF file in the
        //                              viewer since closing binary reader closed the underlying 
        //                              file stream.  Removed close to binary reader.
		//  
		override public LoadProfileData LPData
		{
			get
			{
				LoadProfileData lpData = null;

                if (ProfileHeader != null)
                {
                    // Seek to the start of the load profile data.
                    m_HHFFileStream.Seek(ProfileHeader.EndStreamOffset, SeekOrigin.Begin);

                    // Let's make sure this is really the LP Header Record, if so 
					// the device id will match the header record's device id
                    if (ProfileHeader.DeviceID.Equals(HeaderRecord.DeviceID))
                    {
                        BinaryReader binReader = new BinaryReader(m_HHFFileStream);

                        //Read all of the interval data
                        lpData = ReadIntervalData(binReader);
                    }
                }

				return lpData;

			}//get
		}//LoadProfileData

        /// <summary>
        /// Returns the TOU register data from the HHF file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/08/08 jrf 1.00.00    	Created
        public MV90RegisterData RGData
        {
            get
            {
                MV90RegisterData rgData = null;

                if (HeaderRecord != null)
                {
                    //Read all of the register data
                    rgData = ReadRegisterData();
                   
                }
                return rgData;

            }
        }

        /// <summary>
        /// Returns the TOU register data from the HHF file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  02/08/08 jrf 1.00.00    	Created
        public List<MV90Event> MV90Events
        {
            get
            {
                List<MV90Event> Events = null;

                if (HeaderRecord != null)
                {
                    //Read all of the event data
                    Events = ReadEventData();

                }
                return Events;
            }
        }

        /// <summary>
        /// Returns true if daylight savings changes are handled automatically
        /// or false otherwise
        /// </summary>
        public override bool DSTEnabled
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.DSTEnabled;
                }
                else
                {
                    return false;
                }
            }
        }//DST

        /// <summary>
        /// Returns a DateTime that represents the time that DST starts
        /// </summary>
        public override DateTime DSTStartTime
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.DSTStartTime;
                }
                else
                {
                    return new DateTime();
                }
            }
        }//DSTStartTime

        /// <summary>
        /// Returns a DateTime that represents the time that DST stops
        /// </summary>
        public override DateTime DSTStopTime
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.DSTStopTime;
                }
                else
                {
                    return new DateTime();
                }
            }
        }//DSTStopTime

        /// <summary>
        /// Returns true if the pulse data records are in energy format
        /// </summary>
        public bool FloatingPointFormat
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return (ProfileHeader.IntervalFormat == HHFPulseDataHeaderRecord.IntervalFormatOptions.FloatingPoint);
                }
                else
                {
                    return false;
                }
            }
        }//EnergyFormat

        /// <summary>
        /// Returns the event size in bytes
        /// </summary>
        public int EventSize
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.EventSize;
                }
                else
                {
                    return 0;
                }
            }
        }//EventSize

        /// <summary>
        /// Returns an array of bools.  A value in the array is true if the
        /// channel that corresponds to that value's index has a status (index
        /// 0 = channel 1).
        /// </summary>
        public bool[] HasChannelStatus
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.ChannelStatusPresent;
                }
                else
                {
                    return null;
                }
            }
        }//HasChannelStatus

        /// <summary>
        /// Returns true if the pulse data records contain 2 byte interval
        /// statuses for each interval
        /// </summary>
        public bool HasIntervalStatus
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.IntervalStatusesPresent;
                }
                else
                {
                    return false;
                }
            }
        }//HasIntervalStatus

        /// <summary>
        /// Returns the interval length
        /// </summary>
        public int IntervalLength
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.IntervalLength;
                }
                else
                {
                    return 0;
                }
            }
        }//IntervalsPerHour

        /// <summary>
        /// Returns the number of channels
        /// </summary>
        public int NoChannels
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.ChannelCount;
                }
                else
                {
                    return 0;
                }
            }
        }//NoChannels

        /// <summary>
        /// Returns a DateTime that represents the time data collection began
        /// </summary>
        public override DateTime StartTime
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.StartTime;
                }
                else
                {
                    return new DateTime();
                }
            }
        }//StartTime

        /// <summary>
        /// Returns a DateTime that represents the time data collection stopped
        /// </summary>
        public override DateTime StopTime
        {
            get
            {
                if (ProfileHeader != null)
                {
                    return ProfileHeader.StopTime;
                }
                else
                {
                    return new DateTime();
                }
            }
        }//StopTime

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads the Load Profile Header Data from the HHF file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        protected void ReadProfileHeaderData()
        {
            HHFRecord hhfRecord = new HHFRecord();
            bool bFinished = false;

            if (HeaderRecord != null)
            {
                // Seek to the end of the header data and start reading from there.
                m_HHFFileStream.Seek(HeaderRecord.EndStreamOffset, SeekOrigin.Begin);

				int nBytesRead = 1; // Set to something non-zero so the 

                while (!bFinished & nBytesRead > 0 )
                {
                    nBytesRead = hhfRecord.Read(m_HHFFileStream);

                    //see if the block starts with "HEADER"
                    if (hhfRecord.IsTerminationRecord)
                    {
                        bFinished = true;
                    }
                    else if (hhfRecord.IsHeaderRecord == false && hhfRecord.IsRegisterDataRecord == false)
                    {
                        m_ProfileHeader = new HHFPulseDataHeaderRecord(hhfRecord);

                        // Let's make sure this is really the LP Header Record, if so 
                        // the device id will match the header record's device id
                        if (m_ProfileHeader.DeviceID.Equals(HeaderRecord.DeviceID))
                        {
                            bFinished = true;
                        }
                        else
                        {
                            m_ProfileHeader = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the Profile Header Record.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        internal HHFPulseDataHeaderRecord ProfileHeader
        {
            get
            {
                if (HeaderRecord != null && m_ProfileHeader == null)
                {
                    ReadProfileHeaderData();
                }

                return m_ProfileHeader;
            }
            set
            {
                m_ProfileHeader = value;
            }
        }

        #endregion

        #region Private Methods
		
		/// <summary>
		/// Reads interval data from a pulse data record in the .hhf file.
		/// </summary>		
		/// <param name="br"></param>		
		/// <returns>Load profile object</returns>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//  
		//  
		private LoadProfileData ReadIntervalData(BinaryReader br)
		{
			//variable declarations
			LoadProfileData LPData;
			int nIntervalSize = 0;
			double dblIntervalTime = (double)IntervalLength;

			bool[] blnChannelStatus = HasChannelStatus;
						
			//figure out the size of each interval in bytes
			foreach (Boolean bln in blnChannelStatus)
			{
				if (bln)
				{
					nIntervalSize += 2;
				}
			}

			if (HasIntervalStatus)
			{
				nIntervalSize += 2;
			}

			//Read the intervals and add them to the load profile
			if (FloatingPointFormat)
			{				
				LPData = new LoadProfileEnergyData(IntervalLength);

				//update the size of the intervals to reflect energy format
				nIntervalSize += NoChannels * 4;

				ReadFloatingPointIntervals(br, ref LPData, nIntervalSize, dblIntervalTime, blnChannelStatus);
			}
			else
			{
				LPData = new LoadProfilePulseData(IntervalLength);

				nIntervalSize += NoChannels * 2;

				ReadIntegerIntervals(br, ref LPData, nIntervalSize, dblIntervalTime, blnChannelStatus);
			}

			//Add the channels to the load profile
			for (int i = 0; i < NoChannels; i++)
			{				
				LPData.AddChannel("", 1f, 1f);
			}

			return LPData;
		}

        /// <summary>
        /// This method reads register data from the file.
        /// </summary>
        /// <returns>The register data.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 1.00.00        Created
        private MV90RegisterData ReadRegisterData()
        {
            MV90RegisterData rgData = null;
            HHFRecord hhfRecord = new HHFRecord();
            bool blnEndofRGData = false;
            List<HHFRecord> lstRGDataRecords = new List<HHFRecord>();
            ASCIIEncoding Encoder = new ASCIIEncoding();

            try
            {
                // Seek to just after the header.
                m_HHFFileStream.Seek(HeaderRecord.EndStreamOffset, SeekOrigin.Begin);

                while (0 < hhfRecord.Read(m_HHFFileStream) && !blnEndofRGData)
                {
                    if (hhfRecord.IsRegisterDataRecord)
                    {

                        lstRGDataRecords.Add(hhfRecord);

                        hhfRecord = new HHFRecord();

                        //Get the rest of the records
                        while (0 < hhfRecord.Read(m_HHFFileStream) && hhfRecord.IsRegisterDataRecord)
                        {
                            lstRGDataRecords.Add(hhfRecord);
                            hhfRecord = new HHFRecord();
                        }

                        rgData = ReadRegisterRecords(lstRGDataRecords);

                        blnEndofRGData = true;
                    }
                    else if (hhfRecord.IsLPTerminationRecord || hhfRecord.IsTerminationRecord)
                    {
                        blnEndofRGData = true;
                    }
                    else
                    {
                        string strDeviceID = Encoder.GetString(hhfRecord.DataBuffer, 0, DEVICE_ID_LEN);
                        if (DeviceID == strDeviceID)
                        {
                            //We've found the pulse data header record, so no need to look any further
                            blnEndofRGData = true;
                        }
                    }

                }
            }
            catch
            {
                //Something went wrong, return null.
                rgData = null;
            }

            return rgData;

        }

        /// <summary>
        /// This method reads event data from the file.
        /// </summary>
        /// <returns>A list of MV90 events.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 1.00.00        Created
        private List<MV90Event> ReadEventData()
        {
            List<MV90Event> Events = null;
            HHFRecord hhfRecord = new HHFRecord();
            bool blnEndofLPData = false;
            List<HHFRecord> lstEventDataRecords = new List<HHFRecord>();

            try
            {
                if (null != ProfileHeader)
                {
                    // Seek to just after the pulse data header.
                    m_HHFFileStream.Seek(ProfileHeader.EndStreamOffset, SeekOrigin.Begin);

                    while (!blnEndofLPData && HHFRecord.HHF_RECORDSIZE == hhfRecord.Read(m_HHFFileStream))
                    {
                        if (hhfRecord.IsLPTerminationRecord)
                        {
                            blnEndofLPData = true;
                        }
                    }
                    if (blnEndofLPData && HHFRecord.HHF_RECORDSIZE == hhfRecord.Read(m_HHFFileStream)
                        && !hhfRecord.IsTerminationRecord && !hhfRecord.IsEventDataTerminationRecord)
                    {
                        //This must be an event record, yippee!
                        lstEventDataRecords.Add(hhfRecord);

                        hhfRecord = new HHFRecord();

                        //Get the rest of the records
                        while (HHFRecord.HHF_RECORDSIZE == hhfRecord.Read(m_HHFFileStream)
                            && !hhfRecord.IsEventDataTerminationRecord)
                        {
                            lstEventDataRecords.Add(hhfRecord);
                            hhfRecord = new HHFRecord();
                        }

                        Events = ReadEventRecords(lstEventDataRecords);
                    }
                }
            }
            catch
            {
                //Something went wrong, return null.
                Events = null;
            }

            return Events;
        }

        /// <summary>
        /// This method reads register data records from the file.
        /// </summary>
        /// <param name="lstRGDataRecords">The list of register data records to read.</param>
        /// <returns>The register data.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 10.00.00        Created
        // 02/26/08 jrf 10.00.00        Added method to retrieve the register data format
        private MV90RegisterData ReadRegisterRecords(List<HHFRecord> lstRGDataRecords)
        {
            MV90RegisterData rgData = null;

            //Register Data Format
            if (!m_RGDataFormat.Cached)
            {
                m_RGDataFormat.Value = GetRegisterDataFormat(lstRGDataRecords[0]);
            }
            
            //We're only reading openway register data at this point
            if (CENTRON_OPENWAY_REG_DATA == m_RGDataFormat.Value)
            {
                rgData = ReadCentronOpenWayRegisterRecords(lstRGDataRecords);
            }

            return rgData;

        }

        /// <summary>
        /// This method reads event data records from the file.
        /// </summary>
        /// <param name="lstEventRecords">The list of event data records to read.</param>
        /// <returns>A list of MV90 events.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 10.00.00        Created
        // 02/26/08 jrf 10.00.00        Added culture info to parse methods.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse methods for compatiblity
        //                             with the compact framework.
        private List<MV90Event> ReadEventRecords(List<HHFRecord> lstEventRecords)
        {
            List<MV90Event> Events = new List<MV90Event>();
            byte[] bytEventRecordBuffer = new byte[HHFRecord.HHF_RECORDSIZE * lstEventRecords.Count];
            int iCurrentOffset = 0;
            int iExtraDataLength = ProfileHeader.EventSize - EVENT_CODE_LEN - EVENT_DATE_TIME_LEN;
            bool blnEndOfData = (0 == lstEventRecords.Count); //we're done if there are no records
            string strEventCode = "";
            string strExtraData = "";
            DateTime dtEventTime = new DateTime();
            int iYear = 1;
            int iMonth = 1;
            int iDay = 1;
            int iHour = 0;
            int iMinute = 0;
            ASCIIEncoding Encoder = new ASCIIEncoding();

            //Extract all the raw event data from the records into one array
            for (int i = 0; i < lstEventRecords.Count; i++)
            {
                lstEventRecords[i].DataBuffer.CopyTo(bytEventRecordBuffer, (i * HHFRecord.HHF_RECORDSIZE));
            }

            while (!blnEndOfData)
            {

                if (bytEventRecordBuffer.Length > iCurrentOffset + EVENT_CODE_LEN)
                {
                    //Get the event code
                    strEventCode = Encoder.GetString(bytEventRecordBuffer, iCurrentOffset, EVENT_CODE_LEN);
                    iCurrentOffset += EVENT_CODE_LEN;
                }
                else
                {
                    blnEndOfData = true;
                }
                
                //We could be at the end of a records data and not at the end of event data, since events
                //won't be split over a record boundary.
                while (END_OF_EVENT_RECORD_CODE == strEventCode && !blnEndOfData)
                {
                    blnEndOfData = (bytEventRecordBuffer.Length <= iCurrentOffset + EVENT_CODE_LEN);
                    if (!blnEndOfData)
                    {
                        strEventCode = Encoder.GetString(bytEventRecordBuffer, iCurrentOffset, EVENT_CODE_LEN);
                        iCurrentOffset += EVENT_CODE_LEN;
                    }
                }

                if (!blnEndOfData && bytEventRecordBuffer.Length > iCurrentOffset + EVENT_CODE_LEN + iExtraDataLength)
                {
                    iMonth = Int16.Parse(Encoder.GetString(bytEventRecordBuffer, iCurrentOffset, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    iDay = Int16.Parse(Encoder.GetString(bytEventRecordBuffer, iCurrentOffset + 2, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    iYear = EVENT_BASE_YEAR + Int16.Parse(Encoder.GetString(bytEventRecordBuffer, iCurrentOffset + 4, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    iHour = Int16.Parse(Encoder.GetString(bytEventRecordBuffer, iCurrentOffset + 6, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);
                    iMinute = Int16.Parse(Encoder.GetString(bytEventRecordBuffer, iCurrentOffset + 8, 2),
                        NumberStyles.Integer, CultureInfo.InvariantCulture);

                    //Get the event time
                    dtEventTime = new DateTime(iYear, iMonth, iDay, iHour, iMinute, 0);
                    iCurrentOffset += EVENT_DATE_TIME_LEN;

                    //Get any extra data
                    strExtraData = Encoder.GetString(bytEventRecordBuffer, iCurrentOffset, iExtraDataLength);
                    iCurrentOffset += iExtraDataLength;

                    //We got it, so add it.
                    Events.Add(new MV90Event(strEventCode, dtEventTime, strExtraData));
                }
                else
                {
                    blnEndOfData = true;
                }           
            }
            
            return Events;
        }

        /// <summary>
        /// This method reads Centron OpenWay register records from the file.
        /// </summary>
        /// <param name="lstRGDataRecords">A list of register data records.</param>
        /// <returns>The Centron OpenWay register data.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        // 05/20/08 jrf	1.50.27 114449 Added case for record 20.
        private CentronOpenWayMV90RegisterData ReadCentronOpenWayRegisterRecords(List<HHFRecord> lstRGDataRecords)
        {
            CentronOpenWayMV90RegisterData rgData =
                new CentronOpenWayMV90RegisterData(GetRegisterDataReadTime(lstRGDataRecords[0]), 
                    CENTRON_OPENWAY_REG_DATA);
            int iCurrentOffset = FIRST_REGISTER_READ_OFFSET;
            byte[] bytRGRecordBuffer = new byte[REGISTER_RECORD_DATA_LENGTH * lstRGDataRecords.Count];
            int iRecordNumber = 0;
            int iReadLength = 0;
            int iTotalRegisterDataLength = 0;
            bool blnEndOfData = (0 == lstRGDataRecords.Count); //we're done if there are no records

            //Extract all the raw register data from the records into one array
            for (int i = 0; i < lstRGDataRecords.Count; i++)
			{
                Array.Copy(lstRGDataRecords[i].DataBuffer, 
                           REGISTER_RECORD_ID_LENGTH, 
                           bytRGRecordBuffer,
                           i * REGISTER_RECORD_DATA_LENGTH, 
                           REGISTER_RECORD_DATA_LENGTH);
			}

            iTotalRegisterDataLength = BitConverter.ToInt16(bytRGRecordBuffer, 0);

            while (!blnEndOfData)
            {
                iRecordNumber = bytRGRecordBuffer[iCurrentOffset++];
                
                //Move past the number of items, we don't need it
                iCurrentOffset++; 

                iReadLength = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

				ReadRegisterRecord(rgData, iCurrentOffset, bytRGRecordBuffer, iRecordNumber, iReadLength);

                //Move to the next register read
                iCurrentOffset += iReadLength;

                //Check for the end of data 
                if (iTotalRegisterDataLength <= iCurrentOffset
                    || bytRGRecordBuffer.Length <= (iCurrentOffset + REGISTER_READ_HEADER_SIZE))
                {
                    blnEndOfData = true;
                }

            }

            return rgData;            
        }

		private void ReadRegisterRecord(CentronOpenWayMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer, int iRecordNumber, int iReadLength)
		{
			switch (iRecordNumber)
			{
				case 0:
					{

						rgData.Read0Present = ReadCurrentSeasonTotalkWh(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 1:
					{
						rgData.Read1Present = ReadConstantsData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 2:
					{
						rgData.Read2Present = ReadCapabilitiesData(rgData, iCurrentOffset, bytRGRecordBuffer, iReadLength);
						break;
					}
				case 3:
					{
						rgData.Read3Present = ReadClockRelatedData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 4:
					{
						rgData.Read4Present = ReadQuantityIdentificationData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 5:
					{
						rgData.Read5Present = ReadStateData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 6:
					{
						rgData.Read6Present = ReadEnergyData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}

				case 7:
					{
						rgData.Read7Present = ReadDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				case 8:
					{
						rgData.Read8Present = ReadCumDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				case 9:
					{
						rgData.Read9Present = ReadDemandTOOData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}

				case 10:
					{
						rgData.Read10Present = ReadLastBPStateData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 11:
					{
						rgData.Read11Present = ReadLastBPEnergyData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}

				case 12:
					{
						rgData.Read12Present = ReadLastBPDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 13:
					{
						rgData.Read13Present = ReadLastBPCumDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 14:
					{
						rgData.Read14Present = ReadLastBPDemandTOOData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}

				case 15:
					{
						rgData.Read15Present = ReadLastSRStateData(rgData, iCurrentOffset, bytRGRecordBuffer);
						break;
					}
				case 16:
					{
						rgData.Read16Present = ReadLastSREnergyData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}

				case 17:
					{
						rgData.Read17Present = ReadLastSRDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				case 18:
					{
						rgData.Read18Present = ReadLastSRCumDemandData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				case 19:
					{
						rgData.Read19Present = ReadLastSRDemandTOOData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				case 20:
					{
						rgData.Read20Present = ReadHANClientData(rgData, iCurrentOffset, bytRGRecordBuffer);

						break;
					}
				default:
					break;
			}
		}

        /// <summary>
        /// This method reads the current season total kWh record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/13/08 jrf 1.00.00        Created
        private bool ReadCurrentSeasonTotalkWh(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            try
            {
                rgData.WhDelivered = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        
        /// <summary>
        /// This method reads the constants data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        /// <remarks>The record length will be necessary when reading Sentinel and Image register data.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadConstantsData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            
            bool blnReadPresent = true;
            try
            {
                rgData.CTMultiplier = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Single);

                rgData.VTMultiplier = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Single);

                rgData.RegisterMultiplier = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Single);

                if (SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the customer serial number
                    iCurrentOffset += 10;
                }
                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the customer serial number
                    iCurrentOffset += 16;
                }

                rgData.ProgramID = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                rgData.FWVersionRevision = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Single);

                rgData.DemandIntervalLength = bytRGRecordBuffer[iCurrentOffset++];

                rgData.RateID = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the capabilities data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <param name="iRecordLength">The length of the data in the capabilites data record.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        /// <remarks>The record length will be necessary when reading Sentinel and Image register data.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadCapabilitiesData(CentronOpenWayMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer, int iRecordLength)
        {
            bool blnReadPresent = true;

            try
            {
                rgData.NumTOURates = bytRGRecordBuffer[iCurrentOffset++];
                rgData.ClockEnabled = BitConverter.ToBoolean(bytRGRecordBuffer, iCurrentOffset++);
                rgData.SelfReadDataAvailable = BitConverter.ToBoolean(bytRGRecordBuffer, iCurrentOffset++);
                rgData.NumLPChannels = bytRGRecordBuffer[iCurrentOffset++];
                rgData.NumEnergies = bytRGRecordBuffer[iCurrentOffset++];
                rgData.NumDemands = bytRGRecordBuffer[iCurrentOffset++];
                rgData.NumCumDemands = bytRGRecordBuffer[iCurrentOffset++];

                //The image mono will not have any more data
                if (IMAGE_MONO_CAPABLITIES_REC_LEN < iRecordLength)
                {
                    rgData.PFAverageAvailable = BitConverter.ToBoolean(bytRGRecordBuffer, iCurrentOffset++);
                }

                if (SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the time last programmed
                }
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the clock related data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadClockRelatedData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;

            try
            {
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;
                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }
                rgData.DaysSinceLastDR = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                rgData.DaysSinceLastTest = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int32);
                rgData.TimeLastOutage = TranslateTimeSinceYear(iBaseYear, iNumSeconds);

                iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int32);
                rgData.TimeLastInterrogation = TranslateTimeSinceYear(iBaseYear, iNumSeconds);

                rgData.DaysOnBattery = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int32);

                rgData.CurrentBatteryReading = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                rgData.GoodBatteryReading = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                rgData.DSTConfigured = BitConverter.ToBoolean(bytRGRecordBuffer, iCurrentOffset++);
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the quantity identification data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        // 08/20/09 jrf 2.21.03 139112 Making number of demand LIDs retrieved 
        //                             conditional on whether avg. PF is available.
        private bool ReadQuantityIdentificationData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            int intNumDemands = rgData.NumDemands;
            if (true == rgData.PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }
            uint[] auiEnergyLIDs = new uint[rgData.NumEnergies];
            uint[] auiDemandLIDs = new uint[intNumDemands];
            uint[] auiCumDemandLIDs = new uint[rgData.NumCumDemands];
            uint[] auiDemandTOOLIDs = new uint[rgData.NumDemands];

            try
            {
                //Energy LIDs
                for (int i = 0; i < rgData.NumEnergies; i++)
                {
                    auiEnergyLIDs[i] = BitConverter.ToUInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(UInt32);
                }
                rgData.EnergyLIDs = auiEnergyLIDs;

                //Demand LIDs
                for (int i = 0; i < intNumDemands; i++)
                {
                    auiDemandLIDs[i] = BitConverter.ToUInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(UInt32);
                }
                rgData.DemandLIDs = auiDemandLIDs;

                //Cumulative Demand LIDs
                for (int i = 0; i < rgData.NumCumDemands; i++)
                {
                    auiCumDemandLIDs[i] = BitConverter.ToUInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(UInt32);
                }
                rgData.CumDemandLIDs = auiCumDemandLIDs;

                //Demand TOO LIDs
                for (int i = 0; i < rgData.NumDemands; i++)
                {
                    auiDemandTOOLIDs[i] = BitConverter.ToUInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(UInt32);
                }
                rgData.DemandTOOLIDs = auiDemandTOOLIDs;

            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the current state data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadStateData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            try
            {
                byte bytErrors = 0;
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                rgData.DemandResetCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                //Get the non-fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.NonFatalError1 = (NON_FATAL_ERROR_1_MASK == (bytErrors & NON_FATAL_ERROR_1_MASK));
                rgData.NonFatalError2 = (NON_FATAL_ERROR_2_MASK == (bytErrors & NON_FATAL_ERROR_2_MASK));
                rgData.NonFatalError3 = (NON_FATAL_ERROR_3_MASK == (bytErrors & NON_FATAL_ERROR_3_MASK));
                rgData.NonFatalError4 = (NON_FATAL_ERROR_4_MASK == (bytErrors & NON_FATAL_ERROR_4_MASK));
                rgData.NonFatalError5 = (NON_FATAL_ERROR_5_MASK == (bytErrors & NON_FATAL_ERROR_5_MASK));
                rgData.NonFatalError6 = (NON_FATAL_ERROR_6_MASK == (bytErrors & NON_FATAL_ERROR_6_MASK));
                rgData.DemandThresholdExceeded = (DEM_THRESHOLD_EXCEEDED_MASK == (bytErrors & DEM_THRESHOLD_EXCEEDED_MASK));
                rgData.FatalError4 = (FATAL_ERROR_4_MASK == (bytErrors & FATAL_ERROR_4_MASK));

                //Get the fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.FatalError1 = (FATAL_ERROR_1_MASK == (bytErrors & FATAL_ERROR_1_MASK));
                rgData.FatalError2 = (FATAL_ERROR_2_MASK == (bytErrors & FATAL_ERROR_2_MASK));
                rgData.FatalError3 = (FATAL_ERROR_3_MASK == (bytErrors & FATAL_ERROR_3_MASK));
                rgData.FatalError5 = (FATAL_ERROR_5_MASK == (bytErrors & FATAL_ERROR_5_MASK));
                rgData.FatalError6 = (FATAL_ERROR_6_MASK == (bytErrors & FATAL_ERROR_6_MASK));
                rgData.FatalError7 = (FATAL_ERROR_7_MASK == (bytErrors & FATAL_ERROR_7_MASK));

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostics 1-5 counts
                    iCurrentOffset += 8;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                rgData.OutageCount = bytRGRecordBuffer[iCurrentOffset++];
                rgData.NumTimesProgrammed = bytRGRecordBuffer[iCurrentOffset++];
                rgData.EPFCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 3.0f)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 5.0f)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                //only available if meter has a clock
                if (rgData.ClockEnabled)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    rgData.CurrentTime = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading season
                }
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the current energy data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadEnergyData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblEnergies = new double[rgData.NumEnergies];

            try
            {
                //Total values
               
                for (int i = 0; i < rgData.NumEnergies; i++)
                {
                    adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.TotalEnergies = adblEnergies;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateAEnergies = adblEnergies;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateBEnergies = adblEnergies;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateCEnergies = adblEnergies;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateDEnergies = adblEnergies;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the current demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        // 08/20/09 jrf 2.21.03 139112 Making total demands array size 
        //                             conditional on whether avg. PF is available.
        private bool ReadDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            int intNumDemands = rgData.NumDemands;
            if (true == rgData.PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }

            double[] adblTotalDemands = new double[intNumDemands];
            double[] adblTOUDemands = new double[rgData.NumDemands];

            try
            {
                //Total values
                for (int i = 0; i < intNumDemands; i++)
                {
                    adblTotalDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Single);
                }
                rgData.TotalDemands = adblTotalDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.RateADemands = adblTOUDemands;
                }
                

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.RateBDemands = adblTOUDemands;
                }
                

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.RateCDemands = adblTOUDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.RateDDemands = adblTOUDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the current cumulative demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadCumDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblCumDemands = new double[rgData.NumCumDemands];

            try
            {
                //Total values
                for (int i = 0; i < rgData.NumCumDemands; i++)
                {
                    adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.TotalCumDemands = adblCumDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateACumDemands = adblCumDemands;
                }
                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateBCumDemands = adblCumDemands;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateCCumDemands = adblCumDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.RateDCumDemands = adblCumDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the demand time of occurence data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadDemandTOOData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            DateTime[] adtDemandTOOs = new DateTime[rgData.NumDemands];

            try
            {
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                //Total values
                for (int i = 0; i < rgData.NumDemands; i++)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }
                rgData.TotalDemandTOOs = adtDemandTOOs;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.RateADemandTOOs = adtDemandTOOs;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.RateBDemandTOOs = adtDemandTOOs;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.RateCDemandTOOs = adtDemandTOOs;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.RateDDemandTOOs = adtDemandTOOs;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last billing period state data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastBPStateData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            try
            {
                byte bytErrors = 0;
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                rgData.LastBPDemandResetCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                //Get the non-fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastBPNonFatalError1 = (NON_FATAL_ERROR_1_MASK == (bytErrors & NON_FATAL_ERROR_1_MASK));
                rgData.LastBPNonFatalError2 = (NON_FATAL_ERROR_2_MASK == (bytErrors & NON_FATAL_ERROR_2_MASK));
                rgData.LastBPNonFatalError3 = (NON_FATAL_ERROR_3_MASK == (bytErrors & NON_FATAL_ERROR_3_MASK));
                rgData.LastBPNonFatalError4 = (NON_FATAL_ERROR_4_MASK == (bytErrors & NON_FATAL_ERROR_4_MASK));
                rgData.LastBPNonFatalError5 = (NON_FATAL_ERROR_5_MASK == (bytErrors & NON_FATAL_ERROR_5_MASK));
                rgData.LastBPNonFatalError6 = (NON_FATAL_ERROR_6_MASK == (bytErrors & NON_FATAL_ERROR_6_MASK));
                rgData.LastBPDemandThresholdExceeded = (DEM_THRESHOLD_EXCEEDED_MASK == (bytErrors & DEM_THRESHOLD_EXCEEDED_MASK));
                rgData.LastBPFatalError4 = (FATAL_ERROR_4_MASK == (bytErrors & FATAL_ERROR_4_MASK));

                //Get the fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastBPFatalError1 = (FATAL_ERROR_1_MASK == (bytErrors & FATAL_ERROR_1_MASK));
                rgData.LastBPFatalError2 = (FATAL_ERROR_2_MASK == (bytErrors & FATAL_ERROR_2_MASK));
                rgData.LastBPFatalError3 = (FATAL_ERROR_3_MASK == (bytErrors & FATAL_ERROR_3_MASK));
                rgData.LastBPFatalError5 = (FATAL_ERROR_5_MASK == (bytErrors & FATAL_ERROR_5_MASK));
                rgData.LastBPFatalError6 = (FATAL_ERROR_6_MASK == (bytErrors & FATAL_ERROR_6_MASK));
                rgData.LastBPFatalError7 = (FATAL_ERROR_7_MASK == (bytErrors & FATAL_ERROR_7_MASK));

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostics 1-5 counts
                    iCurrentOffset += 8;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                rgData.LastBPOutageCount = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastBPNumTimesProgrammed = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastBPEPFCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 3.0f)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 5.0f)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                //only available if meter has a clock
                if (rgData.ClockEnabled)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    rgData.LastBPTime = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading season
                }
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last billing period energy data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastBPEnergyData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblEnergies = new double[rgData.NumEnergies];

            try
            {
                //Total values
                for (int i = 0; i < rgData.NumEnergies; i++)
                {
                    adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.LastBPTotalEnergies = adblEnergies;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        rgData.LastBPRateAEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateAEnergies = adblEnergies;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateBEnergies = adblEnergies; 
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateCEnergies = adblEnergies;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateDEnergies = adblEnergies;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last billing period demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        // 08/20/09 jrf 2.21.03 139112 Making total demands array size 
        //                             conditional on whether avg. PF is available.
        private bool ReadLastBPDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            int intNumDemands = rgData.NumDemands;
            if (true == rgData.PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }

            double[] adblTotalDemands = new double[intNumDemands];
            double[] adblTOUDemands = new double[rgData.NumDemands];

            try
            {
                //Total values
                for (int i = 0; i < intNumDemands; i++)
                {
                    adblTotalDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Single);
                }
                rgData.LastBPTotalDemands = adblTotalDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastBPRateADemands = adblTOUDemands;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastBPRateBDemands = adblTOUDemands;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastBPRateCDemands = adblTOUDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastBPRateDDemands = adblTOUDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last billing period cumulative demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastBPCumDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblCumDemands = new double[rgData.NumCumDemands];

            try
            {
                //Total values
                for (int i = 0; i < rgData.NumCumDemands; i++)
                {
                    adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.LastBPTotalCumDemands = adblCumDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateACumDemands = adblCumDemands;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateBCumDemands = adblCumDemands;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateCCumDemands = adblCumDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastBPRateDCumDemands = adblCumDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last billing period demand time of occurence data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 10.00.00       Created
        // 02/28/08 jrf 10.00.00       Corrected Reading value for rate B.
        private bool ReadLastBPDemandTOOData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            DateTime[] adtDemandTOOs = new DateTime[rgData.NumDemands];

            try
            {
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                //Total values
                for (int i = 0; i < rgData.NumDemands; i++)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }
                rgData.LastBPTotalDemandTOOs = adtDemandTOOs;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastBPRateADemandTOOs = adtDemandTOOs;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastBPRateBDemandTOOs = adtDemandTOOs;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastBPRateCDemandTOOs = adtDemandTOOs;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastBPRateDDemandTOOs = adtDemandTOOs;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last self read state data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastSRStateData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;

            try
            {
                byte bytErrors = 0;
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                rgData.LastSRDemandResetCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                //Get the non-fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastSRNonFatalError1 = (NON_FATAL_ERROR_1_MASK == (bytErrors & NON_FATAL_ERROR_1_MASK));
                rgData.LastSRNonFatalError2 = (NON_FATAL_ERROR_2_MASK == (bytErrors & NON_FATAL_ERROR_2_MASK));
                rgData.LastSRNonFatalError3 = (NON_FATAL_ERROR_3_MASK == (bytErrors & NON_FATAL_ERROR_3_MASK));
                rgData.LastSRNonFatalError4 = (NON_FATAL_ERROR_4_MASK == (bytErrors & NON_FATAL_ERROR_4_MASK));
                rgData.LastSRNonFatalError5 = (NON_FATAL_ERROR_5_MASK == (bytErrors & NON_FATAL_ERROR_5_MASK));
                rgData.LastSRNonFatalError6 = (NON_FATAL_ERROR_6_MASK == (bytErrors & NON_FATAL_ERROR_6_MASK));
                rgData.LastSRDemandThresholdExceeded = (DEM_THRESHOLD_EXCEEDED_MASK == (bytErrors & DEM_THRESHOLD_EXCEEDED_MASK));
                rgData.LastSRFatalError4 = (FATAL_ERROR_4_MASK == (bytErrors & FATAL_ERROR_4_MASK));

                //Get the fatal errors
                bytErrors = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastSRFatalError1 = (FATAL_ERROR_1_MASK == (bytErrors & FATAL_ERROR_1_MASK));
                rgData.LastSRFatalError2 = (FATAL_ERROR_2_MASK == (bytErrors & FATAL_ERROR_2_MASK));
                rgData.LastSRFatalError3 = (FATAL_ERROR_3_MASK == (bytErrors & FATAL_ERROR_3_MASK));
                rgData.LastSRFatalError5 = (FATAL_ERROR_5_MASK == (bytErrors & FATAL_ERROR_5_MASK));
                rgData.LastSRFatalError6 = (FATAL_ERROR_6_MASK == (bytErrors & FATAL_ERROR_6_MASK));
                rgData.LastSRFatalError7 = (FATAL_ERROR_7_MASK == (bytErrors & FATAL_ERROR_7_MASK));

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostics 1-5 counts
                    iCurrentOffset += 8;
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                rgData.LastSROutageCount = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastSRNumTimesProgrammed = bytRGRecordBuffer[iCurrentOffset++];
                rgData.LastSREPFCount = BitConverter.ToInt16(bytRGRecordBuffer, iCurrentOffset);
                iCurrentOffset += sizeof(Int16);

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 3.0f)
                {
                    //Add code for reading the non-fatal errors 2
                    iCurrentOffset += 1;
                }

                if (SENTINEL_REG_DATA == rgData.DataFormat && rgData.FWVersionRevision >= 5.0f)
                {
                    //Add code for reading diagnostic 6 count
                    iCurrentOffset += 1;
                }

                //only available if meter has a clock
                if (rgData.ClockEnabled)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    rgData.LastSRTime = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }

                if (CENTRON_IMAGE_REG_DATA == rgData.DataFormat || SENTINEL_REG_DATA == rgData.DataFormat)
                {
                    //Add code for reading season
                }
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last self read energy data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastSREnergyData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblEnergies = new double[rgData.NumEnergies];

            try
            {
                //Total values
                for (int i = 0; i < rgData.NumEnergies; i++)
                {
                    adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.LastSRTotalEnergies = adblEnergies;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateAEnergies = adblEnergies;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateBEnergies = adblEnergies;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateCEnergies = adblEnergies;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumEnergies; i++)
                    {
                        adblEnergies[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateDEnergies = adblEnergies;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last self read demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        // 08/20/09 jrf 2.21.03 139112 Making total demands array size 
        //                             conditional on whether avg. PF is available.
        private bool ReadLastSRDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            int intNumDemands = rgData.NumDemands;
            if (true == rgData.PFAverageAvailable)
            {
                //Add in avg PF if we've got it.
                intNumDemands++;
            }

            double[] adblTotalDemands = new double[intNumDemands];
            double[] adblTOUDemands = new double[rgData.NumDemands];

            try
            {
                //Total values
                for (int i = 0; i < intNumDemands; i++)
                {
                    adblTotalDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Single);
                }
                rgData.LastSRTotalDemands = adblTotalDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastSRRateADemands = adblTOUDemands;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastSRRateBDemands = adblTOUDemands;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastSRRateCDemands = adblTOUDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        adblTOUDemands[i] = BitConverter.ToSingle(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Single);
                    }
                    rgData.LastSRRateDDemands = adblTOUDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last self read cumulative demand data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastSRCumDemandData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            double[] adblCumDemands = new double[rgData.NumCumDemands];

            try
            {
                //Total values
                for (int i = 0; i < rgData.NumCumDemands; i++)
                {
                    adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Double);
                }
                rgData.LastSRTotalCumDemands = adblCumDemands;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateACumDemands = adblCumDemands;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateBCumDemands = adblCumDemands;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateCCumDemands = adblCumDemands;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumCumDemands; i++)
                    {
                        adblCumDemands[i] = BitConverter.ToDouble(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Double);
                    }
                    rgData.LastSRRateDCumDemands = adblCumDemands;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the last self read demand time of occurence data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private bool ReadLastSRDemandTOOData(ANSIMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;
            DateTime[] adtDemandTOOs = new DateTime[rgData.NumDemands];
            
            try
            {
                int iNumSeconds = 0;
                int iBaseYear = SENT_IMAGE_BASE_YEAR;

                if (CENTRON_OPENWAY_REG_DATA == rgData.DataFormat)
                {
                    iBaseYear = OPENWAY_BASE_YEAR;
                }

                //Total values
                for (int i = 0; i < rgData.NumDemands; i++)
                {
                    iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                    iCurrentOffset += sizeof(Int32);
                    adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                }
                rgData.LastSRTotalDemandTOOs = adtDemandTOOs;

                //Rate A
                if (0 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastSRRateADemandTOOs = adtDemandTOOs;
                }

                //Rate B
                if (1 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastSRRateBDemandTOOs = adtDemandTOOs;
                }

                //Rate C
                if (2 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastSRRateCDemandTOOs = adtDemandTOOs;
                }

                //Rate D
                if (3 < rgData.NumTOURates)
                {
                    for (int i = 0; i < rgData.NumDemands; i++)
                    {
                        iNumSeconds = BitConverter.ToInt32(bytRGRecordBuffer, iCurrentOffset);
                        iCurrentOffset += sizeof(Int32);
                        adtDemandTOOs[i] = TranslateTimeSinceYear(iBaseYear, iNumSeconds);
                    }
                    rgData.LastSRRateDDemandTOOs = adtDemandTOOs;
                }

                //For Sentinel add reading of rates E - G
            }
            catch
            {
                blnReadPresent = false;
            }

            return blnReadPresent;
        }

        /// <summary>
        /// This method reads the the HAN client data record. 
        /// </summary>
        /// <param name="rgData">The register data.</param>
        /// <param name="iCurrentOffset">The current offset into the register data buffer.</param>
        /// <param name="bytRGRecordBuffer">The buffer that contains the raw register data.</param>
        /// <returns>Whether or not the record was successfully read.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 05/16/08 jrf 1.50.27 114449 Created
        private bool ReadHANClientData(CentronOpenWayMV90RegisterData rgData, int iCurrentOffset, byte[] bytRGRecordBuffer)
        {
            bool blnReadPresent = true;

            //Supported Number of clients
            rgData.SupportedNumClients = bytRGRecordBuffer[iCurrentOffset++];

            //Supported client data size 
            rgData.SupportedClientDataSize = BitConverter.ToUInt16(bytRGRecordBuffer, iCurrentOffset);
            iCurrentOffset += sizeof(UInt16);

            MemoryStream TempStream = new MemoryStream(bytRGRecordBuffer, iCurrentOffset, bytRGRecordBuffer.Length - iCurrentOffset);
            PSEMBinaryReader BinaryReader = new PSEMBinaryReader(TempStream);

            //Read the table data from the data file
            CHANMfgTable2101 Table2101 = new CHANMfgTable2101(BinaryReader, rgData.SupportedNumClients, rgData.SupportedClientDataSize);

            //Process the HAN client data records into client meter objects
            rgData.ClientMeters = ClientMeter.ProcessClientData(Table2101.HANClientDataList);

            return blnReadPresent;

        }

        /// <summary>
        /// This method computes the the time base on the seconds since a base year.
        /// </summary>
        /// <param name="iBaseYear">The base year to compute the time from.</param>
        /// <param name="iSecs">The number of seconds that have occured since the base year.</param>
        /// <returns>The computed date/time.</returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 1.00.00        Created
        private DateTime TranslateTimeSinceYear(int iBaseYear, int iSecs)
        {
            DateTime dtTranslatedTime = new DateTime();

            try
            {
                dtTranslatedTime = new DateTime(iBaseYear, 1, 1);
                dtTranslatedTime = dtTranslatedTime.AddSeconds((double)iSecs);
            }
            catch
            {
                dtTranslatedTime = new DateTime();
            }

            return dtTranslatedTime;
        }

        /// <summary>
        /// This method extracts the register data time of reading from an HHF register
        /// data record.
        /// </summary>
        /// <param name="Record">The HHF record to extract the date from</param>
        /// <returns>The date/time of the register data read.</returns>
        /// <remarks>The HHF record must be the first HHF record of register data.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/08/08 jrf 10.00.00        Created
        // 02/26/08 jrf 10.00.00        Added culture info to parse methods.
        // 02/27/08 jrf 10.00.00       Added NumberStyles to Parse methods for compatiblity
        //                             with the compact framework.
        private DateTime GetRegisterDataReadTime(HHFRecord Record)
        {
            DateTime dtReadTime;
            ASCIIEncoding Encoder = new ASCIIEncoding();
            
            try
            {
                //Date is in the format YYYYMMDDhhmm (ascii)
                dtReadTime = new DateTime(
                    Int16.Parse(Encoder.GetString(Record.DataBuffer, REG_DATA_YEAR_OFFSET, 4),
                    NumberStyles.Integer, CultureInfo.InvariantCulture),
                    Int16.Parse(Encoder.GetString(Record.DataBuffer, REG_DATA_MONTH_OFFSET, 2),
                    NumberStyles.Integer, CultureInfo.InvariantCulture),
                    Int16.Parse(Encoder.GetString(Record.DataBuffer, REG_DATA_DAY_OFFSET, 2),
                    NumberStyles.Integer, CultureInfo.InvariantCulture),
                    Int16.Parse(Encoder.GetString(Record.DataBuffer, REG_DATA_HOUR_OFFSET, 2),
                    NumberStyles.Integer, CultureInfo.InvariantCulture),
                    Int16.Parse(Encoder.GetString(Record.DataBuffer, REG_DATA_MINUTE_OFFSET, 2),
                    NumberStyles.Integer, CultureInfo.InvariantCulture),
                    0);
            }
            catch
            {
                dtReadTime = new DateTime();
            }

            return dtReadTime;
        }

        /// <summary>
        /// This method extracts the register data format from the HHF register
        /// data record.
        /// </summary>
        /// <param name="Record">The HHF record to extract the format from</param>
        /// <returns>The format of the register data.</returns>
        /// <remarks>The HHF record must be the first HHF record of register data.</remarks>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 02/26/08 jrf 10.00.00        Created
        private string GetRegisterDataFormat(HHFRecord Record)
        {
            ASCIIEncoding Encoder = new ASCIIEncoding();
            string strRegDataFormat;

            //Register Data Format
            strRegDataFormat = Encoder.GetString(Record.DataBuffer, REG_DATA_FORMAT_OFFSET, REG_DATA_FORMAT_LEN);
            strRegDataFormat = strRegDataFormat.Trim();

            return strRegDataFormat;
        }

		/// <summary>
		/// Reads integer intervals from the HHF file.
		/// </summary>
		//  Revision History	
		//  MM/DD/YY who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------
		//  10/26/06 JRF 8.00.28 2876   intBytes was being used to track the bytes read
		//                              from br to determine when the end of record was 
		//                              reached and also the bytes remaining from br3 to 
		//                              determine when the end of the stream was reached.
		//                              I modified check for br3 end of stream to not use 
		//                              intBytes.
		//  08/24/07 mrj 9.00.00		Changed how LP data is created.
		//	04/15/08 mah 9.02.00        Fixed CQ #1 by changing DST date comparision to only
		//                              look at month, day, year, hour instead of whole 
		//								timestamp
		private void ReadIntegerIntervals(BinaryReader br, ref LoadProfileData LPData, int nIntervalSize, double dblIntervalTime, bool[] blnChannelStatus)
		{			
			DateTime dtCurTime;
			MemoryStream memStream;
			byte[] abTemp;
			string[] astrStatus;			
			string strIntervalStatus;
			BinaryReader br2;
			BinaryReader br3;
			int intBytes;			
			bool blnPassedDST = false;


			// Normalize the start time.   TIMs will set the start time at 1 minute
			// past the previous interval's EOI. PC-PRO+ will set the start time 
			// at the previous interval's EOI. This allows us to display files of
			// either origin correctly.
			dtCurTime = TimeStamp.AlignToBeggingOfInterval(StartTime, IntervalLength);

			//read the next 2 bytes and put them in a MemoryStream so
			//they can be read as a uint and then as an Int16
			abTemp = new byte[2];
			abTemp = br.ReadBytes(2);
			memStream = new MemoryStream(abTemp);
			br2 = new BinaryReader(memStream);

			//go until we hit the pulse data termination record...
			while (br2.ReadUInt16() != (uint)0xFFFF)
			{
				//set up br3 to read the 2 bytes we just read again, but
				//this time as an Int16
				memStream = new MemoryStream(abTemp);
				br3 = new BinaryReader(memStream);
				intBytes = 2;

				//do this until we hit the end of a 256 byte block...
				do
				{
					//CInterval objInterval;
					double[] aobjPulse = new double[NoChannels];
					astrStatus = new string[NoChannels];

					//for each channel...
					for (int i = 0; i < NoChannels; i++)
					{
						//if the bytes we need are in the MemoryStream...
						if (br3.BaseStream.Position + 2 <= br3.BaseStream.Length)
						{
							aobjPulse[i] = Convert.ToDouble(br3.ReadInt16());
							intBytes = 0;
						}
						//if the bytes we need are accessible from br...
						else
						{
							aobjPulse[i] = Convert.ToDouble(br.ReadInt16());
						}

						//read the channel status if present
						if (blnChannelStatus[i])
						{
							astrStatus[i] = GetChannelStatusString(br.ReadInt16());
						}
						else
						{
							astrStatus[i] = "";
						}
					}

					//read the interval status if present
					if (HasIntervalStatus)
					{
						strIntervalStatus = GetIntervalStatusString(br.ReadInt16());
					}
					else
					{
						strIntervalStatus = "";
					}

					CalculateIntervalTime(dblIntervalTime, ref dtCurTime, ref blnPassedDST);

					//Add the interval to the load profile
					LPData.AddInterval(aobjPulse, astrStatus, strIntervalStatus, dtCurTime, DisplayScaleOptions.UNITS);
					
					//increment intBytes to reflect the bytes we just read
					intBytes += nIntervalSize;

				} while (intBytes + nIntervalSize < HHFRecord.HHF_RECORDSIZE && dtCurTime < StopTime);

				//read the left-over bytes in the 256 byte block
				br.ReadBytes(HHFRecord.HHF_RECORDSIZE - intBytes);

				//set up the MemoryStream with the next 2 bytes
				abTemp = br.ReadBytes(2);
				memStream = new MemoryStream(abTemp);
				br2 = new BinaryReader(memStream);
			}

			//read the rest of the pulse data termination record
			br.ReadBytes(254);
		}

		private void CalculateIntervalTime(double dblIntervalTime, ref DateTime dtCurTime, ref bool blnPassedDST)
		{
			//calculate the timestamp and add it
			dtCurTime = dtCurTime.AddMinutes(dblIntervalTime);

			// TODO: This won't work if the data spans DST dates that 
			// weren't listed in the header.
			if (DSTEnabled)
			{
				// MAH - the current date is offset by one second from 
				// the interval boundry therefore we cannot simply compare
				// the current interval timestamp to the expected DST dates
				if ((dtCurTime.Year == DSTStartTime.Year) &&
					(dtCurTime.Month == DSTStartTime.Month) &&
					(dtCurTime.Day == DSTStartTime.Day) &&
					(dtCurTime.Hour == DSTStartTime.Hour))
				//						if (dtCurTime == DSTStartTime)
				{
					dtCurTime = dtCurTime.AddHours(1);
				}
				else if ((dtCurTime.Year == DSTStopTime.Year) &&
					(dtCurTime.Month == DSTStopTime.Month) &&
					(dtCurTime.Day == DSTStopTime.Day) &&
					(dtCurTime.Hour == DSTStopTime.Hour) &&
					(!blnPassedDST))
				//						else if (dtCurTime == DSTStopTime && !blnPassedDST)
				{
					dtCurTime = dtCurTime.AddHours(-1);
					blnPassedDST = true;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="br"></param>
		/// <param name="LPData"></param>
		/// <param name="intSize"></param>
		/// <param name="dblIntervalTime"></param>
		/// <param name="blnChannelStatus"></param>
		//  Revision History
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ ---------------------------------------------
		//								Created
		//  08/24/07 mrj 9.00.00		Changed how LP data is created.
		//  
		private void ReadFloatingPointIntervals(BinaryReader br, ref LoadProfileData LPData, int intSize, double dblIntervalTime, bool[] blnChannelStatus)
		{
			DateTime dtCurTime;
			MemoryStream memStream;
			byte[] abTemp;
			string[] astrStatus;			
			string strIntervalStatus;
			BinaryReader br2;
			BinaryReader br3;
			int intBytes;			
			bool blnPassedDST = false;


			// Normalize the start time.   TIMs will set the start time at 1 minute
			// past the previous interval's EOI. PC-PRO+ will set the start time 
			// at the previous interval's EOI. This allows us to display files of
			// either origin correctly.
			dtCurTime = TimeStamp.AlignToBeggingOfInterval(StartTime, IntervalLength);

			//read the next 4 bytes and put them into a MemoryStream so
			//they can be read seperately as a uint and a Single
			abTemp = new byte[4];
			abTemp = br.ReadBytes(4);
			memStream = new MemoryStream(abTemp);
			br2 = new BinaryReader(memStream);

			//until we hit the pulse data termination record...
			while (br2.ReadUInt32() != (uint)0xFFFFFFFF)
			{

				//setupt br3 to read the 4 bytes we just read again, this
				//time as a Single
				memStream = new MemoryStream(abTemp);
				br3 = new BinaryReader(memStream);
				intBytes = 4;

				//do this until we hit the end of a 256 byte block
				do
				{
					double[] aobjEnergy = new double[NoChannels];
					astrStatus = new string[NoChannels];

					//for each channel...
					for (int i = 0; i < NoChannels; i++)
					{
						//if we have already read the bytes we need and so
						//they are not accessible to br...
						if (intBytes == 4)
						{
							aobjEnergy[i] = (double)br3.ReadSingle();
							intBytes = 0;
						}
						//the bytes we need are accessible to br
						else
							aobjEnergy[i] = (double)br.ReadSingle();

						//read the channel status if present
						if (blnChannelStatus[i])
						{
							astrStatus[i] = GetChannelStatusString(br.ReadInt16());
						}
						else
						{
							astrStatus[i] = "";
						}
					}

					//read the interval status if present
					if (HasIntervalStatus)
					{
						strIntervalStatus = GetIntervalStatusString(br.ReadInt16());
					}
					else
					{
						strIntervalStatus = "";
					}

					//calculate the timestamp and add it
					CalculateIntervalTime(dblIntervalTime, ref dtCurTime, ref blnPassedDST);

					//Add the interval to the load profile
					LPData.AddInterval(aobjEnergy, astrStatus, strIntervalStatus, dtCurTime, DisplayScaleOptions.UNITS);
					
					//increment bytes to reflect the number of bytes we
					//just read
					intBytes += intSize;

				} while (intBytes + intSize < HHFRecord.HHF_RECORDSIZE && dtCurTime < StopTime);

				//read the left-over bytes at the end of the block
				br.ReadBytes(HHFRecord.HHF_RECORDSIZE - intBytes);

				//make a new MemoryStream with the next 4 bytes
				abTemp = br.ReadBytes(4);
				memStream = new MemoryStream(abTemp);
				br2 = new BinaryReader(memStream);
			}

			//read the rest of the pulse data termination record
			br.ReadBytes(252);
		}
  
        /// <summary>
        /// Translates a 16 bit integer into a meaningful interval status string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/06 RDB				   Created
        // 04/04/07 mcm 8.00.24 2816   Corrected statuses
        private static string GetIntervalStatusString(Int16 b)
        {

            string strStatus = "";

            if ((b & 0x01) == 0x01)
            {
                strStatus += "O";
            }
            if ((b & 0x02) == 0x02)
            {
                strStatus += "S";
            }
            if ((b & 0x04) == 0x04)
            {
                strStatus += "L";
            }
            //if ((b & 0x08) == 0x08)
            //{
            //    strStatus += "R";
            //}
            if ((b & 0x10) == 0x10)
            {
                strStatus += "R";
            }
            //if ((b & 0x20) == 0x20)
            //{
            //    strStatus += "R";
            //}
            if ((b & 0x40) == 0x40)
            {
                strStatus += "K";
            }
            if ((b & 0x80) == 0x80)
            {
                strStatus += "C";
            }
            if ((b & 0x100) == 0x100)
            {
                strStatus += "A";
            }
            //if ((b & 0x200) == 0x200)
            //{
            //    strStatus += "?";
            //}
            if ((b & 0x400) == 0x400)
            {
                strStatus += "A";
            }
            if ((b & 0x800) == 0x800)
            {
                strStatus += "T";
            }
            //if ((b & 0x1000) == 0x1000)
            //{
            //    strStatus += "?";
            //}

            return strStatus;

        }

        /// <summary>
        /// Translates a 16 bit integer into a meaningful channel status string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/15/06 RDB				   Created
        // 04/04/07 mcm 8.00.24 2816   Corrected statuses
        private static string GetChannelStatusString(Int16 b)
        {

            string strStatus = "";

            //if ((b & 0x02) == 0x02)
            //{
            //    strStatus += "?";
            //}
            //if ((b & 0x04) == 0x04)
            //{
            //    strStatus += "?";
            //}
            //if ((b & 0x08) == 0x08)
            //{
            //    strStatus += "?";
            //}
            if ((b & 0x10) == 0x10)
            {
                strStatus += "V";
            }
            //if ((b & 0x20) == 0x20)
            //{
            //    strStatus += "?";
            //}
            //if ((b & 0x40) == 0x40)
            //{
            //    strStatus += "?";
            //}
            //if ((b & 0x80) == 0x80)
            //{
            //    strStatus += "C";
            //}
            //if ((b & 0x100) == 0x100)
            //{
            //    strStatus += "B";
            //}
            if ((b & 0x200) == 0x200)
            {
                //The Vectron TIM uses this bit for SiteScan status, but there's
                //nothing in PC-PRO+ to translate it to. The SS diag number is
                //not been stored, so it can't be restored.
                strStatus += "X";
            }
            //if ((b & 0x400) == 0x400)
            //{
            //    strStatus += "?";
            //}

            return strStatus;

        }

		/// <summary>
		/// This method is responsible for writing one or more HHF header records
		/// to a newly created MV-90 HHF file.  
		/// </summary>
		/// <param name="dtReadTime">
		/// </param>
		/// <param name="strUnitID" type="string">
		/// </param>
		/// <param name="nNumChannels" type="int">
		/// </param>
		private void WriteFileHeaders( DateTime dtReadTime, String strUnitID, int nNumChannels )
		{
			int nChannelIndex = 0;

			while (nChannelIndex < nNumChannels && nChannelIndex < MV90_HHF.MAX_CHANNELS) 
			{
				// Determine how many channels are represented by this header
				int nChannelsInHeader = nNumChannels - nChannelIndex;

				if ( nChannelsInHeader > HHFHeader.MAXCHANNELSPERRECORD )
				{
					nChannelsInHeader = HHFHeader.MAXCHANNELSPERRECORD;
				}
								
				HHFHeader hhfHeader = new HHFHeader( nChannelIndex + 1, nChannelsInHeader );

				hhfHeader.DeviceID = strUnitID;
				hhfHeader.TimeOfRead = dtReadTime;
				hhfHeader.UTCTime = false;

				hhfHeader.Write(m_HHFFileStream);

				nChannelIndex += nChannelsInHeader;
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="strUnitID" type="string">
		/// </param>
		/// <param name="lpData" type="Itron.Metering.DeviceDataTypes.LoadProfileData">
		/// </param>
		/// <param name="boolDSTEnabled" type="bool">
		/// </param>
		/// <param name="dtDSTStart">
		/// </param>
		/// <param name="dtDSTStop">
		/// </param>
		private void WriteProfileHeaders(String strUnitID, 
										LoadProfileData lpData,
										Boolean boolDSTEnabled,
										DateTime dtDSTStart,
										DateTime dtDSTStop )
		{
			HHFPulseDataHeaderRecord hhfPulseDataHeader = new HHFPulseDataHeaderRecord();

			hhfPulseDataHeader.DeviceID = strUnitID;
			hhfPulseDataHeader.ChannelCount = lpData.Channels.Count;
			hhfPulseDataHeader.IntervalLength = lpData.IntervalDuration;
			hhfPulseDataHeader.DSTEnabled = boolDSTEnabled;

			if (boolDSTEnabled)
			{
				hhfPulseDataHeader.DSTStartTime = dtDSTStart;
				hhfPulseDataHeader.DSTStopTime = dtDSTStop;
			}

			// The HHF profile start time is supposed to be an interval start time
			// plus one minute to prevent the start time from being on an interval
			// boundry
			DateTime dtProfileStartTime = lpData.StartTime.AddMinutes(-(lpData.IntervalDuration - 1));
			hhfPulseDataHeader.StartTime = dtProfileStartTime;

			hhfPulseDataHeader.StopTime = lpData.EndTime;

			hhfPulseDataHeader.Write(m_HHFFileStream);
		}

		private void WriteProfileDataRecords(LoadProfileData lpData)
		{
			// Write the interval data

			int nIntervalSize = 2; // Account for the interval status - no channel statuses
			nIntervalSize += lpData.NumberOfChannels * 4;

			BinaryWriter bw = new BinaryWriter(m_HHFFileStream);

			int nIntervalsPerBlock = HHFRecord.HHF_RECORDSIZE / nIntervalSize;
			int nUnusedWordsPerBlock = (HHFRecord.HHF_RECORDSIZE - (nIntervalsPerBlock * nIntervalSize)) / 2;
			int nBlockIndex = 0;

			int nIntervalIndex = 0;

			foreach (LPInterval lpInterval in lpData.Intervals)
			{
				nIntervalIndex++;

				WriteInterval(bw, lpInterval, lpData.NumberOfChannels);
				nBlockIndex++;

				if (nBlockIndex >= nIntervalsPerBlock)
				{
					if (lpInterval.Time == lpData.EndTime)
					{
						FillPartialBlock(bw, 0xFFFF, nUnusedWordsPerBlock);
					}
					else
					{
						FillPartialBlock(bw, 0x0000, nUnusedWordsPerBlock);
					}
					// Reset the block index
					nBlockIndex = 0;
				}
			}

			// Fill the partial block
			FillPartialBlock(bw, 0xFFFF, (nIntervalsPerBlock - nBlockIndex) + nUnusedWordsPerBlock);

			// Finish off with a termination block for good measure
			HHFPulseTerminationRecord hhfPulseTerminator = new HHFPulseTerminationRecord();
			hhfPulseTerminator.Write(m_HHFFileStream);
		}
		
		private static void FillPartialBlock(BinaryWriter bw, ushort usPadding, int nPadCount)
		{
			for (int nPadIndex = 0; nPadIndex < nPadCount; nPadIndex++)
			{
				bw.Write( usPadding );
			}
		}

		private static void WriteInterval( BinaryWriter bw, LPInterval lpInterval, int nNumChannels )
		{
			ushort usIntervalStatus = TranslateIntervalStatus( lpInterval.IntervalStatus );

			for (int nChannelIndex = 0; nChannelIndex < nNumChannels; nChannelIndex++)
			{
				bw.Write( (float)lpInterval.Data[ nChannelIndex ]);
			}

			bw.Write( usIntervalStatus );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strIntervalStatus"></param>
		/// <returns></returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue#     Description
        //  -------- --- ------- ---------- ---------------------------------------
        //  08/29/08 MAH                    Created
        //  09/02/08 AF                     Switched to IndexOf() since Contains() is 
        //                                  not supported in the compact framework.
        //  09/04/08 AF                     Added StringComparison parameter to hush
        //                                  compiler warnings
        //
        private static ushort TranslateIntervalStatus(String strIntervalStatus)
		{
			ushort usIntervalStatus = 0;

			if (strIntervalStatus.Length > 0)
			{
                if (-1 != strIntervalStatus.IndexOf("O", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x01;
				}

                if (-1 != strIntervalStatus.IndexOf("S", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x02;
				}

                if (-1 != strIntervalStatus.IndexOf("L", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x04;
				}

                if (-1 != strIntervalStatus.IndexOf("R", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x10;
				}

                if (-1 != strIntervalStatus.IndexOf("K", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x40;
				}

				if (-1 != strIntervalStatus.IndexOf("C", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x80;
				}

				if (-1 != strIntervalStatus.IndexOf("A", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x100;
				}

                if (-1 != strIntervalStatus.IndexOf("T", StringComparison.OrdinalIgnoreCase))
				{
					usIntervalStatus |= 0x800;
				}
			}

			return usIntervalStatus;
		}

		private static void GetDSTDates(DateTime dtLPEndTime, out DateTime dtDSTStart, out DateTime dtDSTStop)
		{
			dtDSTStart = DateTime.MinValue;
			dtDSTStop = DateTime.MinValue;

			try
			{
				CDSTSchedule DSTSchedule = new CDSTSchedule();

				int nEndYearIndex = DSTSchedule.DSTDatePairs.FindYear(dtLPEndTime.Year);

				dtDSTStart = DSTSchedule.DSTDatePairs[nEndYearIndex].ToDate;
				dtDSTStop = DSTSchedule.DSTDatePairs[nEndYearIndex].FromDate;

				if (dtDSTStart > dtLPEndTime && nEndYearIndex > 0)
				{
					dtDSTStart = DSTSchedule.DSTDatePairs[nEndYearIndex - 1].ToDate;
				}

				if (dtDSTStop > dtLPEndTime && nEndYearIndex > 0)
				{
					dtDSTStop = DSTSchedule.DSTDatePairs[nEndYearIndex - 1].FromDate;
				}

				dtDSTStart = dtDSTStart.AddSeconds(DSTSchedule.ToTime);
				dtDSTStop = dtDSTStop.AddSeconds(DSTSchedule.FromTime);
			}

			catch
			{
			}
		}

        #endregion

        #region Members

        private HHFPulseDataHeaderRecord m_ProfileHeader;
        private CachedBool m_ContainsRGData;
        private CachedBool m_ContainsEventData;
        private CachedString m_RGDataFormat;

        #endregion
    }
}
