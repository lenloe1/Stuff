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
using System.Xml;
using Itron.Metering.CustomSchedule;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Display;
using Itron.Metering.Progressable;
using Itron.Metering.TOU;
using Itron.Metering.Utilities;
using VirtDevLib;

namespace Itron.Metering.Datafiles
{

    /// <summary>
    /// This class represents a meter image file (MIF).  Meter Image files are vendor
    /// specific HHF files - a predessor to the the generic MV-90 Load Profile HHF file.
    /// </summary>
    /// <remarks >
    /// Revision History	
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------
    /// 01/09/07 MAH 8.00.00			   Created
    /// 07/28/08 MAH 9.50.02        Retrieved pulse multipliers and channel names
    /// </remarks>
    public class MIF : HHFFile
    {
        #region Constants

        // PROG IDs
        private const string CENTRONM_PROG_ID = "CentronImgDS.CentronImg";
        private const string CENTRONP_PROG_ID = "CentronImgVIDS.CentronImg";
        private const string SENTINEL_PROG_ID = "MAPSSentinelDS.Sentinel";
        private const string CENTRON_PROG_ID = "Centron32.Server";
        private const string MT200_PROG_ID = "MT200Series32.Server";
        private const string VECTRON_PROG_ID = "Vectron32.Server";
        private const string FULCRUM_PROG_ID = "FULCRUM32.Server";
        private const string Q1000_PROG_ID = "Q1000.Server";
        private const string QUANTUM_PROG_ID = "Quantum32.Server";
        private const string SQ400_PROG_ID = "SQ4SRV32.Server";

        // HHF Device Types
        private const string CENTRONM_DEV_TYPE = "CENTRONM";
        private const string CENTRONP_DEV_TYPE = "CENTRONP";
        private const string SENTINEL_DEV_TYPE = "SENTINEL";
        private const string Sentinel_DEV_TYPE = "Sentinel";
        private const string CENTRON_DEV_TYPE = "CENTRON";
        private const string MT200_DEV_TYPE = "MT200";
        private const string MT2_DEV_TYPE = "MT2";
        private const string DEM_DEV_TYPE = "DEM";
        private const string TOU_DEV_TYPE = "TOU";
        private const string VECTRON_DEV_TYPE = "VECTRON";
        private const string VEC_DEV_TYPE = "VEC";
        private const string FULCRUM_DEV_TYPE = "FULCRUM";
        private const string X20_DEV_TYPE = "X20";
        private const string X200_DEV_TYPE = "X200";
        private const string Q1000_DEV_TYPE = "Q1000";
        private const string QTM_DEV_TYPE = "QTM";
        private const string Q101_DEV_TYPE = "Q101";
        private const string Q200_DEV_TYPE = "Q200";
        private const string SQ400_DEV_TYPE = "SQ400";

        private const int NUM_LP_BANKS = 10;
        private const short PROFILE_BANK = 1;
        private const short SUCCESS = 0;
        
        private const int HOLIDAY_EVENT_TYPE = 2;
        private const int SEASON_EVENT_TYPE = 0;
        private const int REG_OP_TOU_TABLE_ID = 569;
        //date oldest meter we reverse engineer (Vectron) was first produced
        private const int EARLIEST_TOU_START_YEAR = 1993;
        private const int MINUTES_PER_DAY = 1440;
        private const int HOLIDAY_TYPE_1_INDEX = 0;
        private const int HOLIDAY_TYPE_2_INDEX = 1;

        private const string PATTERN = "Pattern";
        private const string SEASON = "Season";
        private const string RATE_A = "Rate A";
        private const string RATE_B = "Rate B";
        private const string RATE_C = "Rate C";
        private const string RATE_D = "Rate D";
        private const string RATE_E = "Rate E";
        private const string RATE_F = "Rate F";
        private const string RATE_G = "Rate G";
        private const string OUTPUT = "Output";
        private const String TOU_SCHEDULE = "Calendar Editor";
        private const String DEFAULT_TOU_FILE = "TOU_Def.Dat";
        
        // Server Items
        private const int PROFILE_CHANNEL_COUNT = 804;
        private const int PROFILE_BANK_SELECT = 2384;
        private const int PROFILE_START_END_TIME = 814;
        private const int PROFILE_STOP_END_TIME = 815;
        private const int PROFILE_INTERVAL_COUNT = 816;
        private const int CLOCK_USE_DST = 201;
        private const int CLOCK_TO_DST = 202;
        private const int CLOCK_FROM_DST = 203;
        private const int PROFILE_1_LENGTH = 2100;
        private const int FIRMWARE_VERSION = 307;
        private const int PROGRAM_ID = 301;
        private const int TOU_ID = 1806;
        private const int REGOP_ENABLE_TOU = 17403;
        private const int ENABLE_TOU = 25203;
        private const int CUSTOM_SCHED_NAME = 14438;
        private const int REG_OP_TRANSFORMER_RATIO = 539;
		private const int REG_OP_VOLT_RATIO = 509;
        private const int REG_OP_CURRENT_RATIO = 510;
        private const int PROFILE_1_QTY                   = 830;      //# Qty handle
        private const int PROFILE_1_MULT                  = 831;      //# PW
        private const int PROFILE_2_QTY                   = 832;      //# Qty handle
        private const int PROFILE_2_MULT                  = 833;      //# PW
        private const int PROFILE_3_QTY                   = 834;      //# Qty handle
        private const int PROFILE_3_MULT                  = 835;      //# PW
        private const int PROFILE_4_QTY                   = 836;      //# Qty handle
        private const int PROFILE_4_MULT                  = 837;      //# PW
        private const int PROFILE_5_QTY                   = 838;      //# Qty handle
        private const int PROFILE_5_MULT                  = 839;      //# PW
        private const int PROFILE_6_QTY                   = 840;      //# Qty handle
        private const int PROFILE_6_MULT                  = 841;      //# PW
        private const int PROFILE_7_QTY                   = 842;      //# Qty handle
        private const int PROFILE_7_MULT                  = 843;      //# PW
        private const int PROFILE_8_QTY                   = 844;      //# Qty handle
        private const int PROFILE_8_MULT                  = 845;      //# PW
        private const int PROFILE_9_QTY                   = 846;      //# Qty handle
        private const int PROFILE_9_MULT                  = 847;      //# PW
        private const int PROFILE_10_QTY                  = 848;      //# Qty handle
		private const int PROFILE_10_MULT = 849;      //# PW
        private const int PROFILE_11_QTY                  = 850;      //# Qty handle
        private const int PROFILE_11_MULT                 = 851;      //# PW
        private const int PROFILE_12_QTY                  = 852;      //# Qty handle
        private const int PROFILE_12_MULT                 = 853;      //# PW
        private const int PROFILE_13_QTY                  = 854;      //# Qty handle
        private const int PROFILE_13_MULT                 = 855;      //# PW
        private const int PROFILE_14_QTY                  = 856;      //# Qty handle
        private const int PROFILE_14_MULT                 = 857;      //# PW
        private const int PROFILE_15_QTY                  = 858;      //# Qty handle
        private const int PROFILE_15_MULT                 = 859;      //# PW
        private const int PROFILE_16_QTY                  = 860;      //# Qty handle
        private const int PROFILE_16_MULT                 = 861;      //# PW

		private int [] PULSE_MULTIPLIERS = {
									PROFILE_1_MULT,
									PROFILE_2_MULT,
									PROFILE_3_MULT,
									PROFILE_4_MULT,
									PROFILE_5_MULT,
									PROFILE_6_MULT,
									PROFILE_7_MULT,
									PROFILE_8_MULT,
									PROFILE_9_MULT,
									PROFILE_10_MULT,
									PROFILE_11_MULT,
									PROFILE_12_MULT,
									PROFILE_13_MULT,
									PROFILE_14_MULT,
									PROFILE_15_MULT,
									PROFILE_16_MULT };

		private int[] CHANNEL_QTYS = {
									PROFILE_1_QTY,
									PROFILE_2_QTY,
									PROFILE_3_QTY,
									PROFILE_4_QTY,
									PROFILE_5_QTY,
									PROFILE_6_QTY,
									PROFILE_7_QTY,
									PROFILE_8_QTY,
									PROFILE_9_QTY,
									PROFILE_10_QTY,
									PROFILE_11_QTY,
									PROFILE_12_QTY,
									PROFILE_13_QTY,
									PROFILE_14_QTY,
									PROFILE_15_QTY,
									PROFILE_16_QTY };

        /// <summary>
        /// The success result for a device server operation.
        /// </summary>
        protected const short DEVICE_SERVER_SUCCESS = 0;

        /// <summary>
        /// The extension for an xml file.
        /// </summary>
        protected const string XML_EXT = ".xml";

        /// <summary>
        /// The location of the resource strings.
        /// </summary>
        protected const string RESOURCE_FILE_PROJECT_STRINGS =
                                    "Itron.Metering.Datafiles.Properties.Resources";

        #endregion

        #region Definitions

        /// <summary>
        /// The Daytypes used in a TOU Schedule.
        /// </summary>
        private enum DayTypes
        {
            DayType1 = 1,
            DayType2 = 2,
            DayType3 = 3,
            DayType4 = 4,
            HolidayType1 = 5,
            HolidayType2 = 6,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This constructor establishes the association between the MIF
        /// object and the file name.  Note that the MIF file is not created
        /// nor opened when the constructor is called
        /// </summary>
        /// <param name="FileName">The full directory and name of the
        /// MIF file
        /// </param>
        /// <remarks >
        /// Revision History	
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 01/09/07 MAH 8.00.00			   Created
        /// </remarks>
        public MIF(string FileName)
            : base(FileName)
        {
            m_VirtualDevice = null;
            m_bHasHHFBeenRead = false;
            m_bStartTimeRead = false;
            m_bStopTimeRead = false;
            m_bDSTStartTimeRead = false;
            m_bDSTEndTimeRead = false;
            m_bDSTEnabled = new CachedBool();
            m_LoadProfile = null;
            m_usProgramID = new CachedUshort();
            m_usTOUID = new CachedUshort();
            m_blnTOUEnabled = new CachedBool();
            
            m_dblFWVersion = new CachedDouble();
			m_dblTransformerRatio = new CachedDouble();
			m_dblCTRatio = new CachedDouble();
			m_dblVTRatio = new CachedDouble();
            m_TOUSchedule = null;
            m_rmStrings = new System.Resources.ResourceManager(RESOURCE_FILE_PROJECT_STRINGS, 
                this.GetType().Assembly);
		}

        /// <summary>
        /// Returns true if the file is a meter image file and false otherwise
        /// </summary>
        /// <param name="FileName">The full directory and name of the
        /// MIF file
        /// </param>
        /// <returns>True if the file in question is a MIF file, False
        /// if the file either does not exist or is not a MIF file
        /// </returns>        
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  01/09/07 MAH 8.00.00			   Created
		//  12/07/07 MAH 9.00.31 3532 Trimmed spaces from end of device type name
		//	12/11/07 mrj 9.00.32 3545	Fixed method so HHF's are not assumed to
		//								be MIF's
		//
        public static bool IsMeterImageFile(string FileName)
        {
            bool boolIsMeterImageFile = false; // guilty until proven innocent

            try
            {
                FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                HHFRecord hhfRecord = new HHFRecord();

                hhfRecord.Read(file);

                if (hhfRecord.IsHeaderRecord)
                {
                    HHFHeader hhfHeader;

                    hhfHeader = new HHFHeader(hhfRecord);

                    // There are two types of HHF files - MV-90 load profile HHFs and vendor specific HHFs (which we
                    // call MIFs).  At this point we know that we have an HHF file in hand since we found the standard
                    // HHF record.  Now, check to see if this is an MV-90 load profile HHF.  If it is not then assume that
                    // we have our hands on a MIF 

					String strMeterType = hhfHeader.DeviceType;

					if (!String.IsNullOrEmpty(strMeterType))
					{
						strMeterType = strMeterType.TrimEnd(' '); // Added since some MV created files append spaces 
																					 // to the file type

						boolIsMeterImageFile = (strMeterType != "PROFILE") && (strMeterType != "MV90");
					}
                }

				file.Close(); 
            }

            catch
            {
                // Note that since all we are being asked to do is to determine if the given file is a valid MIF file 
                // or not, we should not throw an error if something unexpected occurs.  We should simply assume
                // that this file isn't a usable MIF file and return control to the caller

                boolIsMeterImageFile = false;
            }

            return boolIsMeterImageFile;
        }

        /// <summary>
        /// Finalizer. Disposes all used resources prior to garbage collection
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created

        ~MIF()
        {
			Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Load Profile data from the MIF file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/07	RCG	9.00.07        Created
        // 02/28/08 AF  1.01.06        Changed Convert for code analysis globalization warning
        //

        public override LoadProfileData LPData
        {
            get
            {
                MAPSServerResponses sResponse = MAPSServerResponses.SUCCESS;
                DateTime dtCurrentTime;
                object objValue = null;
                object objIntervalTime = null;
                object objR1 = null;
                object objR2 = null;
                object objR3 = null;
                object objR4 = null;
                object objR5 = null;
                byte byChannels = 0;
                byte byIntervalLength = 0;
                string strStatus = "";
                string strChannelStatus = "";
                string strIntervalstatus = "";
                int iIntervals = 0;
                double[] adblData;
                string[] astrStatuses;
                //int iPulses = 0;

                if (m_LoadProfile == null)
                {

                    if (m_bHasHHFBeenRead == false)
                    {
                        // Read the HHF file
                        if (VirtualDevice != null)
                        {
                            sResponse = OpenHHF();
                        }
                    }

                    // Set the Load Profile Bank
                    // TODO: Find a way to handle multiple banks for Q1000
                    if (sResponse == MAPSServerResponses.SUCCESS)
                    {
                        // Set the profile bank
                        sResponse = (MAPSServerResponses)VirtualDevice.SetValue("", PROFILE_BANK_SELECT, PROFILE_BANK, 0);
                        
                        // We are specifically not checking the value of the response for the above SetValue, because not all devices
                        //  support PROFILE_BANK_SELECT.  (I left it there just in case you want to see it while debugging.
                        sResponse = (MAPSServerResponses)VirtualDevice.GetValue(PROFILE_1_LENGTH, ref objValue, 0);
                    }

                    if (sResponse == MAPSServerResponses.SUCCESS)
                    {
                        // The Q1000 returns it interval time in seconds
                        if (Utilities.DeviceType.eDeviceTypes.Q1000 == DeviceType)
                        {
                            long lIntervalLengthSec = (long)Convert.ChangeType(objValue, typeof(long), CultureInfo.InvariantCulture);
                            if (lIntervalLengthSec >= 60)
                            {
                                byIntervalLength = (byte)(lIntervalLengthSec / 60);
                            }
                            else
                            {
                                // The interval length is less that a minute, which we cannot support.
                                throw new NotSupportedException("Sub Minute Intervals for Q1000 are not supported in reports.");
                            }
                        }
                        else
                        {
                            byIntervalLength = (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture);
                        }
                        // Determine the number of intervals
                        TimeSpan LPTimeSpan = StopTime - StartTime;
                        iIntervals = (int)LPTimeSpan.TotalMinutes / byIntervalLength;

                        OnShowProgress(new ShowProgressEventArgs(1, iIntervals, "Reading Load Profile...", "Reading Load Profile..."));

                        m_LoadProfile = new LoadProfilePulseData((int)byIntervalLength);

                        // Get the channel count
                        sResponse = (MAPSServerResponses)VirtualDevice.GetValue(PROFILE_CHANNEL_COUNT, ref objValue, 0);
                    }

                    // Get the first channel
                    if (sResponse == MAPSServerResponses.SUCCESS)
                    {
                        byChannels = (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture);
                        objIntervalTime = StartTime;

						float fPulseMultiplier;
						String strChannelName;

                        //Add the channels to the load profile
                        for (int i = 0; i < byChannels; i++)
                        {
							// Get the pulse multiplier
							sResponse = (MAPSServerResponses)VirtualDevice.GetValue(PULSE_MULTIPLIERS[ i ], ref objValue, 0);

							if (sResponse == MAPSServerResponses.SUCCESS)
							{
								fPulseMultiplier = (float)Convert.ChangeType(objValue, typeof(float), CultureInfo.InvariantCulture);
							}
							else
							{
								fPulseMultiplier = 1f;
							}

							// Get the channel name
							sResponse = (MAPSServerResponses)VirtualDevice.GetTranslatedValue( CHANNEL_QTYS[i], ref objValue, 0);

							if (sResponse == MAPSServerResponses.SUCCESS)
							{
								strChannelName = (string)Convert.ChangeType(objValue, typeof(string), CultureInfo.InvariantCulture);
							}
							else
							{
								strChannelName = "";
							}

							m_LoadProfile.AddChannel( strChannelName, fPulseMultiplier, 1f);
                        }

                        // We are going to create the Array for all the channels, but we need to read one channel at a time.
                        adblData = new double[byChannels];
                        astrStatuses = new string[byChannels];
                    
                        sResponse = (MAPSServerResponses)VirtualDevice.GetIntervalAtEx(PROFILE_BANK, 1, (short)MAPSProfileDataTypesEx.PulseDataEx,
                                                                               ref objIntervalTime, ref objValue, ref strStatus, 32, ref objR1,
                                                                               ref objR2, ref objR3, ref objR4, ref objR5, 0);
                        
                        if (sResponse == MAPSServerResponses.SUCCESS)
                        {
                            // We now have a value to add to the interval's channel list
                            adblData[0] = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.CurrentCulture);
                            GetStatusString(strStatus, ref strChannelStatus, ref strIntervalstatus);
                            astrStatuses[0] = strChannelStatus;

                            // Fill in the remaining intervals will valid data.
                            for(int Index = 1; Index < byChannels; Index++)
                            {
                                adblData[Index] = (int) 0;
                                astrStatuses[Index] = "";
                            }

                            // Now add the first interval
                            dtCurrentTime = (DateTime)objIntervalTime;
                            m_LoadProfile.AddInterval(adblData, astrStatuses, strIntervalstatus, dtCurrentTime, DisplayScaleOptions.UNITS);

                            // Done with the first interval
                            OnStepProgress(new ProgressEventArgs());
  
                            // Now get the remaining intervals for the first channel
                            while (dtCurrentTime < StopTime && sResponse == MAPSServerResponses.SUCCESS)
                            {
                                adblData = new double[byChannels];
                                astrStatuses = new string[byChannels];

                                sResponse = (MAPSServerResponses)VirtualDevice.GetNextIntervalEx(PROFILE_BANK, 1, (short)MAPSProfileDataTypesEx.PulseDataEx,
                                                                                           ref objIntervalTime, ref objValue, ref strStatus, 32, ref objR1,
                                                                                           ref objR2, ref objR3, ref objR4, ref objR5);

                                if (sResponse == MAPSServerResponses.SUCCESS)
                                {

                                    // We now have a value to add to the interval list
                                    adblData[0] = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.CurrentCulture);
                                    GetStatusString(strStatus, ref strChannelStatus, ref strIntervalstatus);
                                    astrStatuses[0] = strChannelStatus;

                                    // Fill in the remaining intervals will valid data.
                                    for (int Index = 1; Index < byChannels; Index++)
                                    {
                                        adblData[Index] = (int)0;
                                        astrStatuses[Index] = "";
                                    }

                                    // Now add the first interval
                                    dtCurrentTime = (DateTime)objIntervalTime;
                                    m_LoadProfile.AddInterval(adblData, astrStatuses, strIntervalstatus, dtCurrentTime, DisplayScaleOptions.UNITS);

                                    // Done with the first interval
                                    OnStepProgress(new ProgressEventArgs());
                                }
                            }
                        }
                    }

                    // Now we have retrieved all of the first Channels Data.  Now cycle throught the rest of the channels and
                    //  update the created intervals with the new channel data.
                    if (sResponse == MAPSServerResponses.SUCCESS)
                    {
                        for (short ChannelIndex = 2; ChannelIndex <= byChannels; ChannelIndex++)
                        {
                            int IntervalIndex = 0;
                            objIntervalTime = StartTime;

                            sResponse = (MAPSServerResponses)VirtualDevice.GetIntervalAtEx(PROFILE_BANK, ChannelIndex, (short)MAPSProfileDataTypesEx.PulseDataEx,
                                                                                   ref objIntervalTime, ref objValue, ref strStatus, 32, ref objR1,
                                                                                   ref objR2, ref objR3, ref objR4, ref objR5, 0);

                            if (sResponse == MAPSServerResponses.SUCCESS)
                            {
                                dtCurrentTime = (DateTime)objIntervalTime;
                                IntervalIndex = m_LoadProfile.GetIntervalIndexAt(dtCurrentTime);
                                GetStatusString(strStatus, ref strChannelStatus, ref strIntervalstatus);

                                m_LoadProfile.UpdateInterval(IntervalIndex, ChannelIndex - 1, 
                                                             (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture), 
                                                             strChannelStatus, strIntervalstatus);

                                // Done with the first interval
                                OnStepProgress(new ProgressEventArgs());

                                // Now get the remaining intervals for this Channel
                                while (dtCurrentTime < StopTime && sResponse == MAPSServerResponses.SUCCESS)
                                {
                                    // Increment the Interval Index
                                    IntervalIndex++;

                                    sResponse = (MAPSServerResponses)VirtualDevice.GetNextIntervalEx(PROFILE_BANK, ChannelIndex, (short)MAPSProfileDataTypesEx.PulseDataEx,
                                                                                               ref objIntervalTime, ref objValue, ref strStatus, 32, ref objR1,
                                                                                               ref objR2, ref objR3, ref objR4, ref objR5);

                                    if (sResponse == MAPSServerResponses.SUCCESS)
                                    {
                                        GetStatusString(strStatus, ref strChannelStatus, ref strIntervalstatus);
                                        m_LoadProfile.UpdateInterval(IntervalIndex, ChannelIndex - 1, 
                                                                    (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture), 
                                                                     strChannelStatus, strIntervalstatus);
                                    }

                                    // Done with this interval
                                    dtCurrentTime = (DateTime)objIntervalTime;
                                    OnStepProgress(new ProgressEventArgs());
                                }
                            }
                        }
                    }

                    OnHideProgress(new EventArgs());
                }

                // If we encountered a problem during this we should throw an exception
                if(sResponse != MAPSServerResponses.SUCCESS)
                {
                    throw new IOException("Error Reading Load Profile Data from MIF");
                }

                return m_LoadProfile;
            }
        }

        /// <summary>
        /// Gets whether or not the MIF file contains Load Profile data.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/07	RCG	9.00.07        Created
        // 02/28/08 AF  1.01.06        Changed Convert() for code analysis globalization warning

        public override bool ContainsLPData
        {
            get
            {
                MAPSServerResponses sResponse = MAPSServerResponses.SUCCESS;
                bool bContainsLPData = false;
                object objValue = null;

                if (m_bHasHHFBeenRead == false)
                {
                    // Read the HHF file
                    if (VirtualDevice != null)
                    {
                        sResponse = OpenHHF(); 
                    }
                }

                // Result code of 0 means success
                if (sResponse == MAPSServerResponses.SUCCESS)
                {
                    if (Utilities.DeviceType.eDeviceTypes.Q1000 == DeviceType)
                    {
                        // We are only going to check the first profile bank for now.
                        short sIndex = 0;
                    
                        // Set the profile bank
						// We are specifically not checking the value of the response for the above SetValue, because not all devices
						//  support PROFILE_BANK_SELECT.  (I left it there just in case you want to see it while debugging.
                        sResponse = (MAPSServerResponses)VirtualDevice.SetValue("", PROFILE_BANK_SELECT, (object)sIndex, 0);

                        // Get the channel count
                        sResponse = (MAPSServerResponses)VirtualDevice.GetValue(PROFILE_CHANNEL_COUNT, ref objValue, 0);
						
						if (sResponse == MAPSServerResponses.SUCCESS && 
                            (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture) > 0)
						{
							bContainsLPData = true;
						}                            
                    }
                    else
                    {
						// We are specifically not checking the value of the response for the above SetValue, because not all devices
						// support PROFILE_BANK_SELECT.  (I left it there just in case you want to see it while debugging.
                        sResponse = (MAPSServerResponses)VirtualDevice.SetValue("", PROFILE_BANK_SELECT, PROFILE_BANK, 0);
						                        
                        // Get the channel count
                        sResponse = (MAPSServerResponses)VirtualDevice.GetValue(PROFILE_CHANNEL_COUNT, ref objValue, 0);

                        if (sResponse == MAPSServerResponses.SUCCESS && 
                            (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture) > 0)
						{
							bContainsLPData = true;
						}                        
                    }
                }

                return (sResponse == MAPSServerResponses.SUCCESS && bContainsLPData == true);
            }
        }

        /// <summary>
        /// Gets the start time of the Load Profile data in the MIF
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/07	RCG	9.00.07        Created
        // 08/22/08 jrf 9.50.05        Restructured to use RetrieveItem method.
        //
        public override DateTime StartTime
        {
            get 
            {
                object objValue = new DateTime();

                // Check to see if we have already retrieved the value.
                if (m_bStartTimeRead == false)
                {
                    if (RetrieveItem(PROFILE_START_END_TIME, ref objValue))
                    {
                        if (objValue is DateTime)
                        {
                            m_dtStartTime = (DateTime)objValue;

                            m_bStartTimeRead = true;
                        }
                    }
                }

                return m_dtStartTime;
            }
        }

        /// <summary>
        /// Gets the StopTime of the Load Profile data in the MIF
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/11/07	RCG	9.00.07        Created
        // 08/22/08 jrf 9.50.05        Restructured to use RetrieveItem method.
        //
        public override DateTime StopTime
        {
            get 
            {
                object objValue = new DateTime();

                // Check to see if we have already retrieved the value.
                if (false == m_bStopTimeRead)
                {
                    if (RetrieveItem(PROFILE_STOP_END_TIME, ref objValue))
                    {
                        if (objValue is DateTime)
                        {
                            m_dtStopTime = (DateTime)objValue;

                            // Sometimes the Stop Time has some random second value applied, which we need to remove
                            m_dtStopTime = m_dtStopTime.AddSeconds(-1 * m_dtStopTime.Second);

                            m_bStopTimeRead = true;
                        }
                    }
                }

                return m_dtStopTime;
            }
        }

        /// <summary>
        /// Returns true if daylight savings changes are handled automatically
        /// or false otherwise
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created
        // 02/28/08 AF  1.01.06        Changed Convert() for code analysis globalization warning
        // 08/22/08 jrf 9.50.05        Restructured to use RetrieveItem method.
        //
        public override bool DSTEnabled
        {
            get 
            {
                object objValue = null;

                // Check to see if we have already retrieved the value.
                if (false == m_bDSTEnabled.Cached)
                {
                    if (RetrieveItem(CLOCK_USE_DST, ref objValue))
                    {
                        byte byValue = (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture);

                        if (byValue > 0)
                        {
                            m_bDSTEnabled.Value = true;
                        }
                        else
                        {
                            m_bDSTEnabled.Value = false;
                        }
                       
                    }
                    else
                    {
                        // An error occurred
                        m_bDSTEnabled.Value = false;
                    }
                }

                return m_bDSTEnabled.Value;
            }
        }

        /// <summary>
        /// Returns a DateTime that represents the time that DST starts
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created
        // 08/22/08 jrf 9.50.05        Restructured to use RetrieveItem method.
        //
        public override DateTime DSTStartTime
        {
            get 
            {
                object objValue = new DateTime();

                // Check to see if we have already retrieved the value.
                if (false == m_bDSTStartTimeRead)
                {
                    if (RetrieveItem(CLOCK_TO_DST, ref objValue))
                    {
                        if (objValue is DateTime)
                        {
                            m_dtDSTStartTime = (DateTime)objValue;

                            m_bDSTStartTimeRead = true;
                        }
                    }
                }

                return m_dtDSTStartTime;
            }
        }

        /// <summary>
        /// Returns a DateTime that represents the time that DST stops
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07	RCG	9.00.07        Created
        // 08/22/08 jrf 9.50.05        Restructured to use RetrieveItem method.
        //
        public override DateTime DSTStopTime
        {
            get 
            {
                object objValue = new DateTime();

                // Check to see if we have already retrieved the value.
                if (false == m_bDSTEndTimeRead)
                {
                    if (RetrieveItem(CLOCK_FROM_DST, ref objValue))
                    {
                        if (objValue is DateTime)
                        {
                            m_dtDSTEndTime = (DateTime)objValue;

                            m_bDSTEndTimeRead = true;
                        }
                    }
                }

                return m_dtDSTEndTime;
            }
        }

        /// <summary>
        /// Returns an short that represents the Program ID.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created

        public ushort ProgramID
        {
            get
            {
                object objValue = 0;

                // Check to see if we have already retrieved the value.
                if (m_usProgramID.Cached == false)
                {
                    if (RetrieveItem(PROGRAM_ID, ref objValue))
                    {
                        m_usProgramID.Value = (ushort)Convert.ChangeType(objValue, typeof(ushort), CultureInfo.InvariantCulture);
                    }
                }

                return m_usProgramID.Value;
            }
        }

        /// <summary>
        /// Returns an short that represents the TOU schedule ID.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created
        //
        public ushort TOUID
        {
            get
            {
                object objValue = 0;

                // Check to see if we have already retrieved the value.
                if (m_usTOUID.Cached == false)
                {
                    if (RetrieveItem(TOU_ID, ref objValue))
                    {
                        if (null == objValue)
                        {
                            m_usTOUID.Value = 0;
                        }
                        else
                        {
                            m_usTOUID.Value = (ushort)Convert.ChangeType(objValue, typeof(ushort), CultureInfo.InvariantCulture);
                        }
                    }
                    else  //Couldn't retreive the ID, so let's set to zero to indicate no ID
                    {
                        m_usTOUID.Value = 0;
                    }
                }

                return m_usTOUID.Value;
            }
        }

        /// <summary>
        /// Returns an bool that indicates if TOU is programmed in the MIF.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created
        // 10/14/08 jrf 9.50.16        Changed property to indicate if TOU was 
        //                             programmed in the MIF.
        public bool TOUProgrammed
        {
            get
            {
                object objValue = 0;
                bool blnItemRetrieved = false;

                // Check to see if we have already retrieved the value.
                if (m_blnTOUEnabled.Cached == false)
                {
                    blnItemRetrieved = RetrieveItem(REGOP_ENABLE_TOU, ref objValue);

                    //Need to try a different server item for sentinel and image
                    if (false == blnItemRetrieved)
                    {
                        blnItemRetrieved = RetrieveItem(ENABLE_TOU, ref objValue);
                    }

                    if (true == blnItemRetrieved)
                    {
                        byte byValue = (byte)Convert.ChangeType(objValue, typeof(byte), CultureInfo.InvariantCulture);

                        if (byValue > 0)
                        {
                            m_blnTOUEnabled.Value = true;
                        }
                        else
                        {
                            m_blnTOUEnabled.Value = false;
                        }

                        //We could have the situation where the TOU was not programmed 
                        //but the TOU calendar has DST dates and thus is enabled.
                        //We will verify that a season in the current year has switchpoints
                        //to confirm that TOU is actually programmed.
                        if (true == m_blnTOUEnabled.Value)
                        {
                            short sYear = (short)DateTime.Now.Year;
                            object objNumEvents = null;
                            object objDateArray = null;
                            object objEventArray = null;
                            object objIDArray = null;
                            object objNumSwitchPoints = null;
                            short sNumEvents = 0;
                            short sNumSwitchPoints = 0;
                            short[] asCalendarEvents;
                            short[] asCalendarEventIDs;
                            short sResponse = SUCCESS;

                            sResponse = VirtualDevice.GetTOUCalendarLength(sYear, ref objNumEvents, 0);
                            if (SUCCESS == sResponse)
                            {
                                sNumEvents = (short)objNumEvents;

                                objEventArray = new short[sNumEvents];
                                objIDArray = new short[sNumEvents];

                                sResponse = VirtualDevice.GetTOUCalendar(sYear, ref objDateArray, ref objEventArray, ref objIDArray, 0);
                            }

                            if (SUCCESS == sResponse)
                            {
                                asCalendarEvents = (short[])objEventArray;
                                asCalendarEventIDs = (short[])objIDArray;

                                for (int i = 0; i < asCalendarEvents.Length; i++)
                                {
                                    if (SEASON_EVENT_TYPE == asCalendarEvents[i])
                                    {
                                        sResponse = VirtualDevice.GetTOUSeasonLength(asCalendarEventIDs[i], ref objNumSwitchPoints, 0);
                                        if (SUCCESS == sResponse)
                                        {
                                            sNumSwitchPoints = (short)objNumSwitchPoints;
                                            break;
                                        }
                                    }
                                }

                                if (SUCCESS == sResponse && 0 == sNumSwitchPoints)
                                {
                                    m_blnTOUEnabled.Value = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // An error occurred
                        m_blnTOUEnabled.Value = false;
                    }
                }

                return m_blnTOUEnabled.Value;
            }
        }

        /// <summary>
        /// Returns the TOU schedule from the MIF.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/30/08	jrf	9.50           Created
        //
        public virtual CTOUSchedule TOUSchedule
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the device type for the current MIF file
        /// </summary>
        /// <remarks>This property hides the inherited device type property and 
        /// converts the inherited device type string into a more meaningful enum value.</remarks>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created
        // 10/27/08 jrf 9.50.18        Hiding inherited base property.
        //
        new public Utilities.DeviceType.eDeviceTypes DeviceType
        {
            get
            {
                string strDeviceTypeString = HeaderRecord.DeviceType.Trim();
                Utilities.DeviceType.eDeviceTypes eDeviceType = Utilities.DeviceType.eDeviceTypes.UNKNOWN;

                strDeviceTypeString = strDeviceTypeString.ToUpper(CultureInfo.InvariantCulture);

                if (HeaderRecord != null)
                {
                    // Determine the device type string from the MIF file Device Type.
                    switch (strDeviceTypeString)
                    {
                        case CENTRONM_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.CENTRON_C12_19;
                                break;
                            }
                        case CENTRONP_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.CENTRON_V_AND_I;
                                break;
                            }
                        case SENTINEL_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.SENTINEL;
                                break;
                            }
                        case CENTRON_DEV_TYPE:
                        case MT200_DEV_TYPE:
                        case MT2_DEV_TYPE:
                        case DEM_DEV_TYPE:
                        case TOU_DEV_TYPE:
                            {
                                //FW less than 10 means MT200
                                if (FWVersion < 10.0)
                                {
                                    eDeviceType = Utilities.DeviceType.eDeviceTypes.TWO_HUNDRED_SERIES;
                                }
                                else
                                {
                                    eDeviceType = Utilities.DeviceType.eDeviceTypes.CENTRON;
                                }
                                break;
                            }
                        case FULCRUM_DEV_TYPE:
                        case X20_DEV_TYPE:
                        case X200_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.FULCRUM;
                                break;
                            }
                        case QTM_DEV_TYPE:
                        case Q101_DEV_TYPE:
                        case Q200_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.QUANTUM;
                                break;
                            }
                        case SQ400_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.SQ400;
                                break;
                            }
                        case VECTRON_DEV_TYPE:
                        case VEC_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.VECTRON;
                                break;
                            }
                        case Q1000_DEV_TYPE:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.Q1000;
                                break;
                            }
                        default:
                            {
                                eDeviceType = Utilities.DeviceType.eDeviceTypes.UNKNOWN;
                                break;
                            }
                    }
                }

                return eDeviceType;
            }
        }

        /// <summary>
        /// Returns an double that represents the firmware version.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/08	jrf	9.50.05        Created
        //
        public double FWVersion
        {
            get
            {
                object objValue = 0.0;

                // Check to see if we have already retrieved the value.
                if (m_dblFWVersion.Cached == false)
                {
                    if (RetrieveItem(FIRMWARE_VERSION, ref objValue))
                    {
                        m_dblFWVersion.Value = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture);
                    }
                    else //an error occured, just set to 0
                    {
                        m_dblFWVersion.Value = 0.0;
                    }
                }
                
                return m_dblFWVersion.Value;
            }
        }

		/// <summary>
		/// Returns an double that represents the transformer ratio.
		/// </summary>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ --------------------------------------- 
		/// 10/21/08 mah 9.50.05        Created
		///
		/// </remarks>
		public double TransformerRatio
		{
			get
			{
				object objValue = 0.0;

				// Check to see if we have already retrieved the value.
				if (m_dblTransformerRatio.Cached == false)
				{
					if (RetrieveItem(REG_OP_TRANSFORMER_RATIO, ref objValue))
					{
						m_dblTransformerRatio.Value = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture);
					}
					else //an error occured, just set to 1
					{
						m_dblTransformerRatio.Value = 1.0;
					}
				}

				return m_dblTransformerRatio.Value;
			}
		}

		/// <summary>
		/// Returns an double that represents the CT ratio.
		/// </summary>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ --------------------------------------- 
		/// 10/21/08 mah 9.50.05        Created
		///
		/// </remarks>
		public double CTRatio
		{
			get
			{
				object objValue = 0.0;

				// Check to see if we have already retrieved the value.
				if (m_dblCTRatio.Cached == false)
				{
					if (RetrieveItem(REG_OP_CURRENT_RATIO, ref objValue))
					{
						m_dblCTRatio.Value = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture);
					}
					else //an error occured, just set to 1
					{
						m_dblCTRatio.Value = 1.0;
					}
				}

				return m_dblCTRatio.Value;
			}
		}

		/// <summary>
		/// Returns an double that represents the VT ratio.
		/// </summary>
		/// <remarks>
		/// MM/DD/YY who Version Issue# Description
		/// -------- --- ------- ------ --------------------------------------- 
		/// 10/21/08 mah 9.50.05        Created
		///
		/// </remarks>
		public double VTRatio
		{
			get
			{
				object objValue = 0.0;

				// Check to see if we have already retrieved the value.
				if (m_dblVTRatio.Cached == false)
				{
					if (RetrieveItem(REG_OP_VOLT_RATIO, ref objValue))
					{
						m_dblVTRatio.Value = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture);
					}
					else //an error occured, just set to 1
					{
						m_dblVTRatio.Value = 1.0;
					}
				}

				return m_dblVTRatio.Value;
			}
		}


		#endregion

		#region Protected Methods

		/// <summary>
		/// Reads the header data from the specified file stream.  This method is 
		/// overridden in order to minimize the amount of time the file stream is actually
		/// open.  The MV-90 class keeps the stream open for the lifetime of the object,
		/// we don't need to.
		/// </summary>
		// Revision History	
		// MM/DD/YY who Version Issue# Description
		// -------- --- ------- ------ ---------------------------------------
		// 			                   Created
		override protected void ReadHeaderData()
		{
			m_HHFFileStream = new FileStream(m_strHHFFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			base.ReadHeaderData();

			m_HHFFileStream.Close();

			m_HHFFileStream = null;
		}

        /// <summary>
        /// This method retrieves a server item from the MIF file.
        /// </summary>
        /// <param name="objValue">The server item's value.</param>
        /// <param name="iServerItem">The server item's ID.</param>
        /// <returns>It returns a bool which indicates whether the item was retrieved.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/08/07	RCG	9.50.05        Created.
        //
        protected bool RetrieveItem(int iServerItem, ref object objValue)
        {
            bool blnReturn = false;
            MAPSServerResponses sResponse = MAPSServerResponses.SUCCESS;

            if (null != VirtualDevice)
            {
                if (false == m_bHasHHFBeenRead)
                {
                    sResponse = OpenHHF();
                }

                if (MAPSServerResponses.SUCCESS == sResponse)
                {
                    sResponse = (MAPSServerResponses)VirtualDevice.GetValue(iServerItem, ref objValue, 0);
                }

                if (MAPSServerResponses.SUCCESS == sResponse && null != objValue)
                {
                    blnReturn = true;
                }
            }

            return blnReturn;
        }

        /// <summary>
        /// This method reverse engineers the TOU schedule from the MIF.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/17/08 jrf 9.50.18        Created.
        //
        protected void ReverseEngineerTOUSchedule()
        {
            DateTime[] adtCalendarDates = null;
            short[] asCalendarEvents = null;
            short[] asCalendarEventIDs = null;
            object objDates = null;
            object objEvents = null;
            object objEventIDs = null;
            object objValue = null;
            short sDisplayProgress = 0;
            short sResponse = DEVICE_SERVER_SUCCESS;
            short sMaxRate = -1;
            short sMaxOutput = -1;
            short sNewPatternID = 1;
            int iYearIndex = 0;
            int iNumEvents = 0;
            int iEventIndex = 0;
            CEvent NewEvent = null;

            try
            {
                RetreiveGeneralTOUInfo();

                OnShowProgress(new ShowProgressEventArgs(1, TOUSchedule.Duration, "", "Reverse Engineering TOU Schedule..."));

                //Clear out the events in each year
                foreach (CYear Year in TOUSchedule.Years)
                {
                    Year.Events.Clear();
                }

                //Now get the tou events from each year in the device server
                for (int iYear = TOUSchedule.StartYear; iYear < TOUSchedule.StartYear + TOUSchedule.Duration; iYear++)
                {
                    sResponse = VirtualDevice.GetTOUCalendarLength((short)iYear, ref objValue, 0);

                    if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
                    {
                        iNumEvents = (int)Convert.ChangeType(objValue, typeof(int), CultureInfo.InvariantCulture);

                        objDates = new DateTime[iNumEvents];
                        objEvents = new short[iNumEvents];
                        objEventIDs = new short[iNumEvents];

                        sResponse = VirtualDevice.GetTOUCalendar((short)iYear, ref objDates, ref objEvents, ref objEventIDs, sDisplayProgress);
                    }

                    if (DEVICE_SERVER_SUCCESS == sResponse && null != objDates
                        && null != objEvents && null != objEventIDs)
                    {
                        adtCalendarDates = (DateTime[])objDates;
                        asCalendarEvents = (short[])objEvents;
                        asCalendarEventIDs = (short[])objEventIDs;

                        iYearIndex = TOUSchedule.Years.SearchYear(iYear);

                        for (short sIndex = 0; sIndex < asCalendarEvents.Length; sIndex++)
                        {
                            if (HOLIDAY_EVENT_TYPE == asCalendarEvents[sIndex])
                            {
                                if ((short)DayTypes.HolidayType1 == (asCalendarEventIDs[sIndex]))
                                {
                                    iEventIndex = HOLIDAY_TYPE_1_INDEX;
                                }
                                else
                                {
                                    iEventIndex = HOLIDAY_TYPE_2_INDEX;
                                }

                                NewEvent = new CEvent(adtCalendarDates[sIndex],
                                       eEventType.HOLIDAY, iEventIndex, "");

                                TOUSchedule.Years[iYearIndex].Events.Add(NewEvent);

                            }
                            else if (SEASON_EVENT_TYPE == asCalendarEvents[sIndex])
                            {
                                //The season event index is the season ID.  Need to 
                                //subtract 1 to make it a zero-based index.
                                iEventIndex = (int)asCalendarEventIDs[sIndex] - 1;

                                NewEvent = new CEvent(adtCalendarDates[sIndex],
                                    eEventType.SEASON, iEventIndex, "");

                                TOUSchedule.Years[iYearIndex].Events.Add(NewEvent);

                                RetrieveSeason(asCalendarEventIDs[sIndex], ref sNewPatternID, ref sMaxRate, ref sMaxOutput);
                            }
                        }
                    }

                    //Increment progress after each year completes.
                    OnStepProgress(new ProgressEventArgs());
                }

                SetRates(sMaxRate);

                SetOutputs(sMaxOutput);

                SetSupportedDevices();

            }
            catch
            {
                //catching and throwing this exception to make sure the code in the finally
                //block gets executed
                throw;
            }
            finally
            {
                OnHideProgress(new EventArgs());
            }

        }

        /// <summary>
        /// Opens the HHF file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/22/07	KRC	9.00.21        Created
        // 11/14/07 KRC 9.00.27 3370   Do not show Progress Indicator.
        //
        protected MAPSServerResponses OpenHHF()
        {
            MAPSServerResponses sResponse = (MAPSServerResponses)VirtualDevice.ReadHHF(m_strHHFFile, 0);
            m_bHasHHFBeenRead = true;

            if (CheckDevice == false)
            {
                // We were using the wrong device, Read the HHF again with the correct device.
                sResponse = (MAPSServerResponses)VirtualDevice.ReadHHF(m_strHHFFile, 0);
                m_bHasHHFBeenRead = true;
            }
            return sResponse;
        }

        #endregion

		#region Private Properties

		/// <summary>
        /// Disposes the object and all resources used by the object.
        /// </summary>
        /// <param name="bDisposing">Whether or not the Dispose method was called.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/06/07 RCG 9.00.07        Created	

        protected override void Dispose(bool bDisposing)
        {
            if (!this.m_bDisposed)
            {
                if (m_VirtualDevice != null && bDisposing)
                {
                    m_VirtualDevice.ReleaseDeviceServer();
                    m_VirtualDevice = null;
                }
            }

            base.Dispose(bDisposing);
        }

        /// <summary>
        /// Gets the Virtual Device object for the current MIF file.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/07	RCG	9.00.07        Created

        protected VirtualDeviceClass VirtualDevice
        {
            get
            {
                if (m_VirtualDevice == null)
                {
                    // The virtual device server has not been created so we need to 
                    // create it and then figure out which device server to create
                    m_VirtualDevice = new VirtualDeviceClass();

                    m_VirtualDevice.SetDeviceServer(DeviceServerProgID);
                }

                return m_VirtualDevice;
            }
        }

        /// <summary>
        /// Determines if the correct Device is loaded
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/22/07	KRC	9.00.21        Created
        // 02/28/08 AF  1.01.06        Changed Convert() for code analysis globalization warning
        //
        private bool CheckDevice
        {
            get
            {
                object objValue = null;
                bool bResult = true;

                // If this is a CENTRON, we are not sure if it really is a CENTRON,
                //  or if it is actually a MT200
                if (DeviceServerProgID == CENTRON_PROG_ID)
                {
                    m_VirtualDevice.GetValue(FIRMWARE_VERSION, ref objValue, 0);

                    double dblFWVersion = (double)Convert.ChangeType(objValue, typeof(double), CultureInfo.InvariantCulture);

                    if (dblFWVersion < 10.0)
                    {
                        // This is a MT200, so we need to make things right
                        m_VirtualDevice.ReleaseDeviceServer();
                        m_VirtualDevice = null;

                        m_VirtualDevice = new VirtualDeviceClass();

                        m_VirtualDevice.SetDeviceServer(MT200_PROG_ID);
                        
                        m_bHasHHFBeenRead = false;
                        bResult = false;
                    }
                }

                return bResult;
            }
        }
        /// <summary>
        /// Takes the status from the Device Server and break it into Channel status and Interval status
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/14/07	KRC	9.00.09        Created
        // 02/28/08 AF  1.01.06        Changed string compare for code analysis globalization warning
        //
        private void GetStatusString(string strStatuses, ref string strChannelStatuses, ref string strIntervalStatusus)
        {
            strChannelStatuses = "";
            strIntervalStatusus = "";

            for (int Index = 0; Index < strStatuses.Length; Index++)
            {
                if (0 == String.Compare(strStatuses[Index].ToString(), "V", StringComparison.OrdinalIgnoreCase) ||
                   0 == String.Compare(strStatuses[Index].ToString(), "X", StringComparison.OrdinalIgnoreCase))
                {
                    strChannelStatuses += strStatuses[Index].ToString();
                }
                else
                {
                    strIntervalStatusus += strStatuses[Index].ToString();
                }
            }
        }

        /// <summary>
        /// Gets the Prog ID string for the current MIF file
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/07/07	RCG	9.00.07        Created
        //
        private string DeviceServerProgID
        {
            get
            {
                string strProgID = null;

                if (HeaderRecord != null)
                {
                    // Determine the Prog ID from the MIF file Device Type.
                    string strTemp = HeaderRecord.DeviceType.Trim();
                    switch (HeaderRecord.DeviceType.Trim())
                    {
                        case CENTRON_DEV_TYPE:
                        {
                            strProgID = CENTRON_PROG_ID;
                            break;
                        }
                        case CENTRONM_DEV_TYPE:
                        {
                            strProgID = CENTRONM_PROG_ID;
                            break;
                        }
                        case CENTRONP_DEV_TYPE:
                        {
                            strProgID = CENTRONP_PROG_ID;
                            break;
                        }
                        case SENTINEL_DEV_TYPE:
                        case Sentinel_DEV_TYPE:
                        {
                            strProgID = SENTINEL_PROG_ID;
                            break;
                        }
                        case MT200_DEV_TYPE:
                        case MT2_DEV_TYPE:
                        case DEM_DEV_TYPE:
                        case TOU_DEV_TYPE:
                        {
                            // Start out with the CENTRON and then switch to the 200 if we determine that is the case
                            strProgID = CENTRON_PROG_ID;
                            break;
                        }
                        case FULCRUM_DEV_TYPE:
                        case X20_DEV_TYPE:
                        case X200_DEV_TYPE:
                        {
                            strProgID = FULCRUM_PROG_ID;
                            break;
                        }
                        case QTM_DEV_TYPE:
                        case Q101_DEV_TYPE:
                        case Q200_DEV_TYPE:
                        {
                            strProgID = QUANTUM_PROG_ID;
                            break;
                        }
                        case SQ400_DEV_TYPE:
                        {
                            strProgID = SQ400_PROG_ID;
                            break;
                        }
                        case VECTRON_DEV_TYPE:
                        case VEC_DEV_TYPE:
                        {
                            strProgID = VECTRON_PROG_ID;
                            break;
                        }
                        case Q1000_DEV_TYPE:
                        {
                            strProgID = Q1000_PROG_ID;
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException(HeaderRecord.DeviceType + " is not a supported Device Type");
                        }
                    }
                }

                return strProgID;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves the season from the device server. 
        /// </summary>
        /// <param name="sSeasonID">The season's ID number.</param>
        /// <param name="sNewPatternID">The next pattern ID.</param>
        /// <param name="sMaxOutput">The maximum rate.</param>
        /// <param name="sMaxRate">The maximum output.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private void RetrieveSeason(short sSeasonID, ref short sNewPatternID, ref short sMaxRate, ref short sMaxOutput)
        {
            DateTime[] adtSeasonTimes;
            object objNumSwitchPoints = 0;
            object objDayTypeArray = null;
            object objTimeArray = null;
            object objEventArray = null;
            object objEventIDArray = null;
            short[] asDayTypes;
            short[] asSeasonEvents;
            short[] asSeasonEventIDs;
            short sNumSwitchPoints = 0;
            short sDisplayProgress = 0;
            short sResponse = DEVICE_SERVER_SUCCESS;
            Int16Collection NormalDays = null;
            Int16Collection Holidays = null;

            //Get the season info
            sResponse = VirtualDevice.GetTOUSeasonLength(sSeasonID, ref objNumSwitchPoints, sDisplayProgress);

            if (DEVICE_SERVER_SUCCESS == sResponse)
            {
                sNumSwitchPoints = (short)objNumSwitchPoints;

                objDayTypeArray = new short[sNumSwitchPoints];
                objTimeArray = new DateTime[sNumSwitchPoints];
                objEventArray = new short[sNumSwitchPoints];
                objEventIDArray = new short[sNumSwitchPoints];

                sResponse = VirtualDevice.GetTOUSeason(sSeasonID, ref objDayTypeArray, ref objTimeArray,
                    ref objEventArray, ref objEventIDArray, sDisplayProgress);
            }

            if (DEVICE_SERVER_SUCCESS == sResponse && null != objDayTypeArray
                && null != objTimeArray && null != objEventArray && null != objEventIDArray)
            {
                CSwitchPointCollection[] aNormalDaysCollections;
                CSwitchPointCollection[] aHolidaysCollections;

                asDayTypes = (short[])objDayTypeArray;
                adtSeasonTimes = (DateTime[])objTimeArray;
                asSeasonEvents = (short[])objEventArray;
                asSeasonEventIDs = (short[])objEventIDArray;

                //the season rate and output IDs are all 1 greater than 
                //their index number which we need for the switchpoint
                for (int i = 0; i < asSeasonEventIDs.Length; i++)
                {
                    asSeasonEventIDs[i] = (short)(asSeasonEventIDs[i] - 1);
                }

                ExtractSwitchPoints(asDayTypes, adtSeasonTimes, asSeasonEvents, asSeasonEventIDs,
                    out aNormalDaysCollections, out aHolidaysCollections, ref sMaxRate, ref sMaxOutput);

                //Now prepare the patterns to be added to the season
                NormalDays = AddPatterns(ref sNewPatternID, aNormalDaysCollections);
                Holidays = AddPatterns(ref sNewPatternID, aHolidaysCollections);

                //Now we should be set up to add the season
                try
                {
                    TOUSchedule.Seasons.SearchID((int)sSeasonID);
                }
                //The search throws an argument exception when it can't find the 
                //specified season ID.  Since we should only add when the season
                //ID isn't in the collection, we will catch this exception.
                catch (ArgumentException)
                {
                    TOUSchedule.Seasons.Add(new CSeason((int)sSeasonID,
                        SEASON_EVENT_TYPE + sSeasonID.ToString(CultureInfo.InvariantCulture),
                        NormalDays, Holidays));
                }


            }
        }

        /// <summary>
        /// This method extracts switchpoints from the given season events.
        /// </summary>
        /// <param name="asDayTypes">The daytypes for each event in the season.</param>
        /// <param name="adtSeasonTimes">The times for each event in the season.</param>
        /// <param name="asSeasonEvents">The season's events.</param>
        /// <param name="asSeasonEventIDs">The ID for each event in the season.</param>
        /// <param name="aNormalDaysCollections">The normal day switchpoint collections.</param>
        /// <param name="aHolidaysCollections">The holiday switchpoint collections.</param>
        /// <param name="sMaxRate">The maximum rate.</param>
        /// <param name="sMaxOutput">The maximum output.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private static void ExtractSwitchPoints(short[] asDayTypes, DateTime[] adtSeasonTimes, short[] asSeasonEvents,
            short[] asSeasonEventIDs, out CSwitchPointCollection[] aNormalDaysCollections, out CSwitchPointCollection[]
            aHolidaysCollections, ref short sMaxRate, ref short sMaxOutput)
        {
            CSwitchPointCollection DayType1 = new CSwitchPointCollection();
            CSwitchPointCollection DayType2 = new CSwitchPointCollection();
            CSwitchPointCollection DayType3 = new CSwitchPointCollection();
            CSwitchPointCollection DayType4 = new CSwitchPointCollection();
            CSwitchPointCollection HolidayType1 = new CSwitchPointCollection();
            CSwitchPointCollection[] aAllSwitchPointCollections =
                new CSwitchPointCollection[] { DayType1, DayType2, DayType3, DayType4, HolidayType1 };
            DateTime dtMidnight;
            DateTime dtSwitchTime;
            TimeSpan Span;
            eSwitchPointType eType = eSwitchPointType.RATE;

            aNormalDaysCollections = new CSwitchPointCollection[] { DayType1, DayType2, DayType3, DayType4 };
            aHolidaysCollections = new CSwitchPointCollection[] { HolidayType1 };

            //Process each event in the season.
            for (int i = 0; i < asSeasonEvents.Length; i++)
            {
                //The switchpoint start and stop times are stored as minutes since midnight.
                dtMidnight = new DateTime(adtSeasonTimes[i].Year, adtSeasonTimes[i].Month, adtSeasonTimes[i].Day, 0, 0, 0);
                dtSwitchTime = adtSeasonTimes[i];
                Span = dtSwitchTime - dtMidnight;

                //Set a new max rate or output if appropriate.
                eType = (eSwitchPointType)asSeasonEvents[i];

                if (eSwitchPointType.RATE == eType)
                {
                    if (asSeasonEventIDs[i] > sMaxRate)
                    {
                        sMaxRate = asSeasonEventIDs[i];
                    }
                }
                //we've got an output
                else if (asSeasonEventIDs[i] > sMaxOutput)
                {
                    sMaxOutput = asSeasonEventIDs[i];
                }


                if (0 <= asSeasonEventIDs[i])
                {
                    switch (asDayTypes[i])
                    {
                        case (short)DayTypes.DayType1:
                            {
                                DayType1.Add(new CSwitchPoint((int)Span.TotalMinutes, 0, asSeasonEventIDs[i], eType));
                                break;
                            }
                        case (short)DayTypes.DayType2:
                            {
                                DayType2.Add(new CSwitchPoint((int)Span.TotalMinutes, 0, asSeasonEventIDs[i], eType));
                                break;
                            }
                        case (short)DayTypes.DayType3:
                            {
                                DayType3.Add(new CSwitchPoint((int)Span.TotalMinutes, 0, asSeasonEventIDs[i], eType));
                                break;
                            }
                        case (short)DayTypes.DayType4:
                            {
                                DayType4.Add(new CSwitchPoint((int)Span.TotalMinutes, 0, asSeasonEventIDs[i], eType));
                                break;
                            }
                        case (short)DayTypes.HolidayType1:
                            {
                                HolidayType1.Add(new CSwitchPoint((int)Span.TotalMinutes, 0, asSeasonEventIDs[i], eType));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

            }

            //Now sort the switchpoint collections and assign stop times.
            //The stop time will be the start time of the next switchpoint except 
            //for the last which will be the end of the day.
            foreach (CSwitchPointCollection Collection in aAllSwitchPointCollections)
            {
                int iLastSwitchPoint = Collection.Count - 1;

                if (0 < Collection.Count)
                {
                    Collection.Sort();

                    //Set stop times for all but the last switchpoint.
                    for (int i = 0; i < iLastSwitchPoint; i++)
                    {
                        Collection[i].StopTime = Collection[i + 1].StartTime;
                    }

                    //The last switchpoint always has the same stop time.
                    Collection[iLastSwitchPoint].StopTime = MINUTES_PER_DAY;
                }
            }
        }

        /// <summary>
        /// This method sets the rates and outputs in the TOU schedule.
        /// </summary>
        /// <param name="sMaxRate">The maximum rate.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private void SetRates(short sMaxRate)
        {
            m_TOUSchedule.Rates.Clear();

            for (short s = 0; s <= sMaxRate; s++)
            {
                switch (s)
                {
                    case 0:
                        {
                            m_TOUSchedule.Rates.Add(RATE_A);
                            break;
                        }
                    case 1:
                        {
                            m_TOUSchedule.Rates.Add(RATE_B);
                            break;
                        }
                    case 2:
                        {
                            m_TOUSchedule.Rates.Add(RATE_C);
                            break;
                        }
                    case 3:
                        {
                            m_TOUSchedule.Rates.Add(RATE_D);
                            break;
                        }
                    case 4:
                        {
                            m_TOUSchedule.Rates.Add(RATE_E);
                            break;
                        }
                    case 5:
                        {
                            m_TOUSchedule.Rates.Add(RATE_F);
                            break;
                        }
                    case 6:
                        {
                            m_TOUSchedule.Rates.Add(RATE_G);
                            break;
                        }
                    default:
                        break;
                }
            }


        }

        /// <summary>
        /// This method sets the rates and outputs in the TOU schedule.
        /// </summary>
        /// <param name="sMaxOutput">The maximum output.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private void SetOutputs(short sMaxOutput)
        {
            TOUSchedule.Outputs.Clear();

            for (short s = 0; s <= sMaxOutput; s++)
            {
                TOUSchedule.Outputs.Add(OUTPUT + s.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// This method determines the supported devices for the TOU schedule.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the device is not supported.</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private void SetSupportedDevices()
        {
            //Set the supported devices
            TOUSchedule.SupportedDevices.Clear();

            switch (DeviceType)
            {
                case Utilities.DeviceType.eDeviceTypes.CENTRON:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.CENTRON_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.VECTRON:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.VECTRON_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.SENTINEL:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.SENTINEL_BAS_DEVICE);

                        if (TOUSchedule.Rates.Count > 4)
                        {
                            TOUSchedule.SupportedDevices.Add(CTOUSchedule.SENTINEL_ADV_DEVICE);
                        }
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.Q1000:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.Q1000_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.QUANTUM:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.QUANTUM_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.FULCRUM:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.FULCRUM_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.CENTRON_C12_19:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.CENTRON_MONO_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.CENTRON_V_AND_I:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.CENTRON_POLY_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.TWO_HUNDRED_SERIES:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.DMTMTR200_DEVICE);
                        break;
                    }
                case Utilities.DeviceType.eDeviceTypes.CENTRONII_C12_19:
                    {
                        TOUSchedule.SupportedDevices.Add(CTOUSchedule.CENTRONII_MONO_DEVICE);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("The following device type is not supported: "
                            + Utilities.DeviceType.GetDeviceTypeString(DeviceType));
                    }
            }
        }

        /// <summary>
        /// This method extracts the general TOU information for the TOU schedule.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/29/08 jrf 9.50           Created.
        //
        private void RetreiveGeneralTOUInfo()
        {
            int iNumEvents = 0;
            object objValue = null;
            short sDisplayProgress = 0;
            short sTOUStartYear = EARLIEST_TOU_START_YEAR;
            short sTOUStopYear = sTOUStartYear;
            short sTOUMaxYear = (short)(DateTime.Now.Year + 100);
            short sResponse = DEVICE_SERVER_SUCCESS;
            string strDirectoryName = CRegistryHelper.GetDataDirectory(TOU_SCHEDULE);

            m_TOUSchedule = new CTOUSchedule(strDirectoryName + DEFAULT_TOU_FILE);
            m_TOUSchedule.TOUName = Path.GetFileNameWithoutExtension(FileName);
            m_TOUSchedule.TOUID = CTOUSchedule.GetNextTOUScheduleID();
            m_TOUSchedule.FileName = strDirectoryName + TOUSchedule.TOUID.ToString("D4", CultureInfo.InvariantCulture) + TOUSchedule.TOUName + XML_EXT;
            m_TOUSchedule.DateCreated = DateTime.Now;
            m_TOUSchedule.Description = m_rmStrings.GetString("REVERSE_ENGINEERED_FROM") + Path.GetFileName(FileName);

            //Find the start year
            sResponse = VirtualDevice.GetTOUCalendarLength(sTOUStartYear, ref objValue, sDisplayProgress);

            if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
            {
                iNumEvents = (short)Convert.ChangeType(objValue, typeof(short), CultureInfo.InvariantCulture);
            }

            //Search forward until we find some events
            while (DEVICE_SERVER_SUCCESS == sResponse && 0 == iNumEvents && sTOUMaxYear > sTOUStartYear)
            {
                sTOUStartYear++;
                sResponse = VirtualDevice.GetTOUCalendarLength(sTOUStartYear, ref objValue, sDisplayProgress);
                if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
                {
                    iNumEvents = (short)Convert.ChangeType(objValue, typeof(short), CultureInfo.InvariantCulture);
                }
            }

            if (sTOUStartYear == sTOUMaxYear)
            {
                //We found no events before reaching the max year.
                throw new InvalidOperationException("No events were found for the TOU schedule.");
            }
            else
            {
                TOUSchedule.StartYear = sTOUStartYear;

                //Find the stop year
                sTOUStopYear = sTOUStartYear;

                //Search forward until we don't find any more events
                while (DEVICE_SERVER_SUCCESS == sResponse && 0 != iNumEvents)
                {
                    sTOUStopYear++;
                    sResponse = VirtualDevice.GetTOUCalendarLength(sTOUStopYear, ref objValue, sDisplayProgress);
                    if (DEVICE_SERVER_SUCCESS == sResponse && null != objValue)
                    {
                        iNumEvents = (short)Convert.ChangeType(objValue, typeof(short), CultureInfo.InvariantCulture);
                    }
                }

                TOUSchedule.Duration = sTOUStopYear - sTOUStartYear;
            }
        }

        /// <summary>
        /// This method converts the array of switch point colletions into patterns and 
        /// adds them to the TOU schedule.
        /// </summary>
        /// <param name="sNewPatternID">The next new pattern ID to use.</param>
        /// <param name="aPatterns">The array of switchpoint colletions.</param>
        /// <returns>An Int16Collection representing each of the patterns added.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/17/08 jrf 9.50.18        Created.
        //
        private Int16Collection AddPatterns(ref short sNewPatternID,
            CSwitchPointCollection[] aPatterns)
        {
            Int16Collection Days = new Int16Collection();
            CPattern Pattern = null;
            short sExistingPatternID = 0;

            foreach (CSwitchPointCollection Collection in aPatterns)
            {
                if (0 < Collection.Count)
                {
                    Pattern = new CPattern((int)sNewPatternID, PATTERN + sNewPatternID.ToString(CultureInfo.InvariantCulture), Collection);

                    if (false == m_TOUSchedule.Patterns.Contains(Pattern))
                    {
                        //Only add the pattern if there is not an equivalent pattern in 
                        //the list.
                        m_TOUSchedule.Patterns.Add(Pattern);
                        Days.Add(sNewPatternID);
                        sNewPatternID++;
                    }
                    else
                    {
                        sExistingPatternID = (short)m_TOUSchedule.Patterns.FindPatternID(Collection);
                        Days.Add(sExistingPatternID);
                    }
                }
            }

            return Days;
        }

        #endregion

        #region Member Variables

        private VirtualDeviceClass m_VirtualDevice;
        

        private DateTime m_dtStartTime;
        private bool m_bStartTimeRead;

        private DateTime m_dtStopTime;
        private bool m_bStopTimeRead;

        private CachedBool m_bDSTEnabled;

        private DateTime m_dtDSTStartTime;
        private bool m_bDSTStartTimeRead;

        private DateTime m_dtDSTEndTime;
        private bool m_bDSTEndTimeRead;

        private LoadProfileData m_LoadProfile;

        private CachedUshort m_usProgramID;
        private CachedUshort m_usTOUID;
        private CachedBool m_blnTOUEnabled;
        private CachedDouble m_dblFWVersion;
		private CachedDouble m_dblTransformerRatio;
		private CachedDouble m_dblCTRatio;
		private CachedDouble m_dblVTRatio;

        /// <summary>
        /// Indicates whether or not the HHF has been read.
        /// </summary>
        protected bool m_bHasHHFBeenRead;

        /// <summary>
        /// The MIF's TOU schedule.
        /// </summary>
        protected CTOUSchedule m_TOUSchedule;

        /// <summary>
        /// Used to retrieve strings from the resource file.
        /// </summary>
        protected readonly System.Resources.ResourceManager m_rmStrings;
        

        #endregion
    }
}
