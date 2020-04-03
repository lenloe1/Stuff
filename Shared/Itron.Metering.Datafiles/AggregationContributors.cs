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
using System.Text;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
	/// <summary>
	/// This class represents a collection of load profile objects 
	/// (contributors) that are to be joined into a single load profile
	/// object.
	/// </summary>
	public class AggregratedProfileData : LoadProfileCollection
	{
		#region Definitions

		/// <summary>
		///     
		/// </summary>
		public enum AggregationResult
		{
			/// <summary>
			///     
			/// </summary>
			Success,
			/// <summary>
			///     
			/// </summary>
			NoContributors,
			/// <summary>
			///     
			/// </summary>
			IntervalLengthMismatch,
			/// <summary>
			///     
			/// </summary>
			DSTMismatch,
			/// <summary>
			///     
			/// </summary>
			DeviceIDMismatch,
			/// <summary>
			///     
			/// </summary>
			QuantityMismatch,
			/// <summary>
			/// Added 12/22/08 to flag contributors that do not have the same number
			/// of channels
			/// </summary>
			ChannelMismatch
		};

		#endregion

		#region Public Methods

		/// <summary>
		/// This method adds the given file to the list of data sources to be joined     
		/// together. In doing so, the file will be opened and the load profile data 
		/// will be extracted.   
		/// </summary>
		/// <param name="strFileName" type="string">
		/// </param>
		/// <returns>
		/// True if the given file was successfully opened, the load profile data was   
		/// extracted, and the file was added to the list of data sources.  False is  
		/// returned if the file could not be added to the data source list.
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/26/08 MAH 9.50				    Created
        ///  11/25/09 AF  2.30.22               Changed the catch to quiet a compiler warning
		/// </remarks>
		public override Boolean AddContributor(String strFileName)
		{
			Boolean boolSuccess = false;
			HHFFile hhfFile = null;

			try
			{
				if (MIF.IsMeterImageFile(strFileName))
				{
					hhfFile = new MIF(strFileName);
				}
				else if (MV90_HHF.IsMV90HHFFile(strFileName))
				{
					hhfFile = new MV90_HHF(strFileName);
				}

				if (hhfFile != null)
				{
					LoadProfileDataSource newContributor = new LoadProfileDataSource(hhfFile);

					m_lstDataSources.Add(newContributor);

					// We are successful only after we have added the file to the list of
					// contributors
					boolSuccess = true;

					// Everytime a new data source is added we need to clear all of the
					// cached properties so that they can be recalculated when requested
					FlushCachedValues();
				}
			}

			// If, for any reason we could not read the load profile data simply throw the 
			// error so that the caller can catch it but also close the HHF file so that we
			// do not leave open file handles laying around.
			catch ( Exception )
			{
				throw ; 
			}

			finally
			{
				if (hhfFile != null)
				{
					hhfFile.Close();
				}
			}

			return boolSuccess;
		}

		/// <summary>
		/// This method determines if the current data sources within the collection can
		/// be successfully joined.  
		/// </summary>
		/// <returns>
		/// A value of Success is returned if the data sources can be joined.  
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/26/08 MAH 9.50				    Created
		///  12/22/08 MAH 9.50.28 CQ 124459     Added check for number of channels
		/// </remarks>
		public AggregationResult Validate()
		{
			AggregationResult ValidationResult = ValidateDeviceID();

			if (ValidationResult == AggregationResult.Success)
			{
				if (m_lstDataSources.Count == 0)
				{
					ValidationResult = AggregationResult.NoContributors;
				}
			}

			if (ValidationResult == AggregationResult.Success)
			{
				if (NumProfileChannels == 0)
				{
					ValidationResult = AggregationResult.NoContributors;
				}
			}

			if (ValidationResult == AggregationResult.Success)
			{
				if (IntervalLength == 0)
				{
					ValidationResult = AggregationResult.IntervalLengthMismatch;
				}
			}

			if (ValidationResult == AggregationResult.Success)
			{
				if (!VerifyDSTConsistency())
				{
					ValidationResult = AggregationResult.DSTMismatch;
				}
			}

			if (ValidationResult == AggregationResult.Success)
			{
				if (!VerifyChannelCount())
				{
					ValidationResult = AggregationResult.ChannelMismatch;
				}
			}

			if (ValidationResult == AggregationResult.Success)
			{
				if (!VerifyChannelConsistency())
				{
					ValidationResult = AggregationResult.QuantityMismatch;
				}
			}

			return ValidationResult;
		}

		/// <summary>
		/// This method displays a user-readable error message for the given
		/// aggregation result.  Note that this method should only be called in the
		/// case of an error and only on behalf of a user-interactive application.
		/// </summary>
		/// <param name="Result" type="Itron.Metering.Datafiles.AggregratedProfileData.AggregationResult">
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/26/08 MAH 9.50				    Created
		/// </remarks>
		public static String TranslateAggregationError(AggregationResult Result)
		{
			String strResult = "";

			switch (Result)
			{
				case AggregratedProfileData.AggregationResult.DeviceIDMismatch:
					strResult = "Unit IDs do not match";
					break;
				case AggregratedProfileData.AggregationResult.DSTMismatch:
					strResult = "DST settings do not match";
					break;
				case AggregratedProfileData.AggregationResult.IntervalLengthMismatch:
					strResult = "Interval lengths do not match";
					break;
				case AggregratedProfileData.AggregationResult.NoContributors:
					strResult = "Some contributors do not contain load profile data";
					break;
				case AggregratedProfileData.AggregationResult.QuantityMismatch:
					strResult = "Load profile channel quantities do not match";
					break;
				case AggregratedProfileData.AggregationResult.ChannelMismatch:
					strResult = "All contributors do not contain the same number of load profile channels";
					break;
				default:
					strResult = Result.ToString();
					break;
			}

			return strResult;
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// The property returns the combined load profile data from all valid
		/// contributors
		/// </summary>
		public override LoadProfileData LPData
		{
			get
			{
				//if (m_lpData == null)
				//{
					Join(out m_lpData);
				//}

				return m_lpData;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		///     
		/// </summary>
		/// <value>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		protected override DateTime CalculateProfileStartTime()
		{
			DateTime dtStartTime = DateTime.MinValue;

			for (int nContributorIndex = 0; nContributorIndex < m_lstDataSources.Count; nContributorIndex++)
			{
				if (nContributorIndex == 0)
				{
					dtStartTime = m_lstDataSources[nContributorIndex].ProfileStartTime;
				}
				// Find the smallest start time
				else if (dtStartTime > m_lstDataSources[nContributorIndex].ProfileStartTime)
				{
					dtStartTime = m_lstDataSources[nContributorIndex].ProfileStartTime;
				}
			}

			return dtStartTime;
		}

		/// <summary>
		///     
		/// </summary>
		/// <value>
		/// </value>
		/// <remarks>
		///     
		/// </remarks>
		protected override DateTime CalculateProfileEndTime()
		{
			DateTime dtEndTime = DateTime.MinValue;

			for (int nContributorIndex = 0; nContributorIndex < m_lstDataSources.Count; nContributorIndex++)
			{
				if (nContributorIndex == 0)
				{
					dtEndTime = m_lstDataSources[nContributorIndex].ProfileEndTime;
				}
				else if (dtEndTime < m_lstDataSources[nContributorIndex].ProfileEndTime)
				{
					dtEndTime = m_lstDataSources[nContributorIndex].ProfileEndTime;
				}
			}

			return dtEndTime;
		}

		/// <summary>
		/// This help method clears all of the cached values in the data source 
		/// collection.  This method should be called whenever the contents of the 
		/// collection changes.
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/26/08 mah 9.50.00		Created
		///  
		/// </remarks>
		protected override void FlushCachedValues()
		{
			base.FlushCachedValues();

			m_lpData = null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		///     
		/// </summary>
		/// <param name="lpResult" type="Itron.Metering.DeviceDataTypes.LoadProfilePulseData">
		/// </param>
		/// <returns>
		///     A Itron.Metering.Datafiles.AggregrateContributors.AggregationResult value...
		/// </returns>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  12/19/08 mah 9.50.00 CQ142242 Added conditional to prevent infinite loop	
		/// </remarks>
		private AggregationResult Join(out LoadProfilePulseData lpResult)
		{
			lpResult = null;

			AggregationResult Result = Validate();

			if (Result == AggregationResult.Success)
			{
				lpResult = new LoadProfilePulseData(IntervalLength);

				// Add the appropriate number of channels
				for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
				{
					// The channels will assume the same name as the first contributor
					lpResult.AddChannel(m_lstDataSources[0].LPData.Channels[nChannelIndex].ChannelName,
										(float)1.0,
										(float)1.0);
				}

				DateTime dtCurrentInterval = ProfileStartTime;
				DateTime dtLastInterval = ProfileEndTime;

				// It is most efficient if we start by sorting the contributors
				// in order of their start times.  This allows us to easily determine
				// where gaps exist in the data and fill them as we go
				m_lstDataSources.Sort(CompareStartTime);
				int nContributorIndex = 0;

				// We may or may not need all contributors - the best way to know when 
				// we are done is by looking at the last interval in the result set.
				// Once we have the last interval, we can stop 

				while ((dtCurrentInterval < dtLastInterval) && 
					(nContributorIndex < m_lstDataSources.Count ))
				{
					// Start by trying to find a data source that contains the interval we
					// are looking for.  Note that the call to GetIntervalIndexAt will return an 
					// index greater than number of intervals in the data set if the interval is not
					// found
					int nIntervalIndex = m_lstDataSources[nContributorIndex].LPData.GetIntervalIndexAt(dtCurrentInterval);

					if (nIntervalIndex < m_lstDataSources[nContributorIndex].LPData.NumberIntervals)
					{
						// OK, we found the interval we were looking for.  Since we are just joining
						// data we should be joining pulse data so try to get a reference to a pulse 
						// data object.  If we can't get one just use what we already have.
						LoadProfilePulseData LPPulseData = m_lstDataSources[nContributorIndex].LPData as LoadProfilePulseData;

						if (LPPulseData != null)
						{
							MergeContributorData(lpResult, LPPulseData, ref dtCurrentInterval);
						}
						else
						{
							MergeContributorData(lpResult, m_lstDataSources[nContributorIndex].LPData, ref dtCurrentInterval);
						}

						// Move on to the next contributor - at this point we should have retrieved
						// all of the information that we could from the current data source.  The next
						// set of data will either come from the next contributor or it will be a data 
						// gap
						nContributorIndex++;
					}
					else if ( dtCurrentInterval > m_lstDataSources[nContributorIndex].LPData.StartTime )
					{
						// No gap here - just go to the next contributor - MAH 12/19/08
						nContributorIndex++;
					}
					else // we found a gap in the data!  We need to add filler
					{
						FillDataGap(lpResult, ref dtCurrentInterval, m_lstDataSources[nContributorIndex].LPData.StartTime);
					}

				}
			}

			return Result;
		}


		/// <summary>
		///     
		/// </summary>
		/// <returns>
		///     A Itron.Metering.Datafiles.AggregrationContributors.AggregationResult value...
		/// </returns>
		private AggregationResult ValidateDeviceID()
		{
			AggregationResult DeviceIDResults = AggregationResult.Success;

			String strUnitID = "";

			for (int nContributorIndex = 0; nContributorIndex < m_lstDataSources.Count; nContributorIndex++)
			{
				if (nContributorIndex == 0)
				{
					strUnitID = m_lstDataSources[nContributorIndex].UnitID;
				}
				else if (strUnitID != m_lstDataSources[nContributorIndex].UnitID)
				{
					DeviceIDResults = AggregationResult.DeviceIDMismatch;
					break;
				}
			}
			return DeviceIDResults;
		}


		/// <summary>
		///     
		/// </summary>
		/// <param name="lpResult" type="Itron.Metering.DeviceDataTypes.LoadProfilePulseData">
		/// </param>
		/// <param name="dtCurrentInterval" type="System.DateTime">
		/// </param>
		/// <param name="dtGapEndTime" type="System.DateTime">
		/// </param>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/26/08 mah 9.50.00		Created
		///  
		/// </remarks>
		private void FillDataGap(LoadProfilePulseData lpResult, ref DateTime dtCurrentInterval, DateTime dtGapEndTime)
		{
			double[] dblIntervalData = new double[NumProfileChannels];
			String[] strChannelStatus = new String[NumProfileChannels];
			String strIntervalStatus = "K";

			// Every interval in the gap will have a value of 0 with an empty channel 
			// status
			for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++ )
			{
				dblIntervalData[ nChannelIndex ] = 0.0;
				strChannelStatus[ nChannelIndex ] = "";
			}

			// Now simply walk through the time period defined by the gap and adding 
			// one interval at a time
			while (dtCurrentInterval < dtGapEndTime)
			{
				lpResult.AddInterval( dblIntervalData,
								strChannelStatus,
								strIntervalStatus,
								dtCurrentInterval,
								DisplayScaleOptions.UNITS);

				// Advance the clock one interval at a time.
				dtCurrentInterval = dtCurrentInterval.AddMinutes( (double)IntervalLength );
			}
		}

		/// <summary>
		/// Merge the data from the given source to the destination structure    
		/// </summary>
		/// <param name="lpDestination" type="Itron.Metering.DeviceDataTypes.LoadProfilePulseData">
		/// </param>
		/// <param name="lpSource" type="Itron.Metering.DeviceDataTypes.LoadProfileData">
		/// </param>
		/// <param name="dtCurrentInterval" type="System.DateTime">
		/// </param>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  12/15/08 mah 9.50.26 CQ124360	Incremented the interval end time to advance
		///  								to the next interval
		///  
		/// </remarks>
		private void MergeContributorData(LoadProfilePulseData lpDestination, LoadProfileData lpSource, ref DateTime dtCurrentInterval )
		{
			int nIntervalIndex = lpSource.GetIntervalIndexAt( dtCurrentInterval );

			while (nIntervalIndex < lpSource.NumberIntervals)
			{
				// We found useful data - now add all that we can to the result data.
				// We don't have to worry about the end date since the whole purpose of
				// aggregation is to join as much data as possible
				LPInterval nextInterval = lpSource.Intervals[nIntervalIndex];

				lpDestination.AddInterval(
								nextInterval.Data,
								nextInterval.ChannelStatuses,
								nextInterval.IntervalStatus,
								nextInterval.Time,
								DisplayScaleOptions.UNITS);

				dtCurrentInterval = lpDestination.EndTime;
				nIntervalIndex++;
			}

			// Advance the clock to the next interval
			dtCurrentInterval = dtCurrentInterval.AddMinutes((double)IntervalLength);

		}

		#endregion

		#region Members

		private LoadProfilePulseData m_lpData;

		#endregion
	}
}
