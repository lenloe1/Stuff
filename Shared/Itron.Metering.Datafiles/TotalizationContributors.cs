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
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
	/// <summary>
	/// This class represents a single load profile object (contributor) that 
	/// will be used to create a totalized value.  Note that this class only allows
	/// MIF files to be used as data sources since all totalization must be  
	/// performed using primary energy values.  The class also allows for the 
	/// caller to specify whether the given contributor should be added or
	/// subtracted from the total value
	/// </summary>
	public class TotalizationDataSource : LoadProfileDataSource
	{
		#region Definitions

		/// <summary>
		/// This enumeration indicates whether a given contributor is an additive
		/// or subtractive contributor
		/// </summary>
		public enum CalculationType
		{
			/// <summary>
			///     
			/// </summary>
			Addition,
			/// <summary>
			///     
			/// </summary>
			Subtraction
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This contructor effectively restricts totalization to MIF files only   
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  10/21/08 MAH 9.50				    Added logic to use multipliers for 
		///  									primary values
		/// </remarks>
		public TotalizationDataSource(MIF mif) : base( mif )
		{
			// We always totalize energy data not pulses, so make sure that 
			// the load profile data object contains energy values
			if (LPData != null)
			{
				LPData = LPData.EnergyData;
			}

			// Energy data by itself is not good enough.  Because of the possibility of
			// differences in transformers, we need to totalize primary values not secondary
			// values.  So we need to get the correct multiplier for each channel
			if (LPData != null)
			{
				double dblTransformerRatio = mif.TransformerRatio;
				double dblCTRatio = mif.CTRatio;
				double dblVTRatio = mif.VTRatio;

				for (int nChannel = 0; nChannel < LPData.Channels.Count; nChannel++)
				{
					String strQtyName = LPData.Channels[ nChannel ].ChannelName;

					if (strQtyName.Contains("Wh") ||
						strQtyName.Contains("VAh") ||
						strQtyName.Contains("varh") ||
						strQtyName.Contains("Qh") )
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)dblTransformerRatio);
					}
					else if (strQtyName.Contains("Ah"))
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)dblCTRatio);
					}
					else if (strQtyName.Contains("A^2h"))
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)(dblCTRatio*dblCTRatio));
					}
					else if (strQtyName.Contains("Vh"))
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)dblVTRatio);
					}
					else if (strQtyName.Contains("V^2h"))
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)(dblVTRatio*dblVTRatio));
					}
					else // don't make assumptions - do not try to convert to primary
						// this could be a PF or input channel
					{
						LPData.ChangeChannelMultiplier(nChannel, (float)1.0);
					}
				}
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// This property indicates whether or not the load profile data 
		/// associated with this object should be added or removed from the total
		/// energy value during the totalization process.
		/// </summary>
		public CalculationType Calculation
		{
			get
			{
				return m_eCalculation;
			}
			set
			{
				m_eCalculation = value;
			}
		}

		#endregion

		#region Members

		private CalculationType m_eCalculation;

		#endregion
	}
	
	/// <summary>
	/// This class represents a collection of load profile objects 
	/// (contributors) that are to be totalized.  
	/// </summary>
	public class TotalizedProfileData : LoadProfileCollection
	{
		#region Definitions

		/// <summary>
		///     
		/// </summary>
		public enum TotalizationResult
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
			NoCommonIntervalData,
			/// <summary>
			///     
			/// </summary>
			DSTMismatch,
			/// <summary>
			///     
			/// </summary>
			QuantityMismatch
		};

		#endregion

		#region Public Methods

		/// <summary>
		/// This method adds the given file to the list of data sources to be         
		/// totalized. In doing so, the file will be opened and the load profile data
		/// will be extracted.
		/// </summary>
		/// <param name="strMIFFileName" type="string">
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
		///  12/22/08 MAH 9.50.28 CQ 124757     Added error handling to prevent    
		///  totaliation of more contributors with more than 16 channels of data
        ///  11/25/09 AF  2.30.22               Changed the catch block to preserve
        ///                                     the call stack and quiet compiler warning
		/// </remarks>
		public override Boolean AddContributor(String strMIFFileName)
		{
			Boolean boolSuccess = false;
			MIF hhfFile = null;

			try
			{
				if (MIF.IsMeterImageFile(strMIFFileName))
				{
					hhfFile = new MIF(strMIFFileName);

					TotalizationDataSource newContributor = new TotalizationDataSource(hhfFile);

					// By default all new contributors will be added to the resulting data
					newContributor.Calculation = TotalizationDataSource.CalculationType.Addition;

					m_lstDataSources.Add(newContributor);

					// We are successful only after we have added the file to the list of
					// contributors
					boolSuccess = true;

					// Clear any previous totalized results.  They will need to be recalculated
					FlushCachedValues();
				}
			}

			// If, for any reason we could not read the load profile data
			// simply treat it as if it doesn't exist
			catch (Exception)
			{
				throw;
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
		/// This method should be called prior to accessing the load profile property in
		/// order to see if totalization can be performed on the current set of 
		/// contributing data sources.
		/// </summary>
		/// <returns>
		/// A value of Success if totalization can be performed.
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/26/08 MAH 9.50				    Created
		/// </remarks>
		public TotalizationResult ValidateDataSources()
		{
			if (m_lstDataSources.Count == 0)
			{
				return TotalizationResult.NoContributors;
			}
			else if (NumProfileChannels == 0)
			{
				return TotalizationResult.NoContributors;
			}
			else if (ProfileStartTime == DateTime.MinValue || ProfileEndTime == DateTime.MinValue)
			{
				return TotalizationResult.NoCommonIntervalData;
			}
			else if (IntervalLength == 0)
			{
				return TotalizationResult.IntervalLengthMismatch;
			}
			else if ( !VerifyDSTConsistency() )
			{
				return TotalizationResult.DSTMismatch;
			}
			else if ( !	VerifyChannelConsistency() )
			{
				return TotalizationResult.QuantityMismatch;
			}
			else
			{
				return TotalizationResult.Success;
			}
		}


		/// <summary>
		/// This method displays a user-readable error message for the given
		/// totalization result.  
		/// </summary>
		/// <param name="Result" type="Itron.Metering.Datafiles.AggregratedProfileData.AggregationResult">
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue#        Description
		///  -------- --- ------- ------------- -----------------------------------
		///  08/28/08 MAH 9.50				    Created
		/// </remarks>
		public static String TranslateTotalizationError(TotalizationResult Result)
		{
			String strResult = "";

			switch (Result)
			{
				case TotalizationResult.DSTMismatch:
					strResult = "DST settings do not match";
					break;
				case TotalizationResult.IntervalLengthMismatch:
					strResult = "Interval lengths do not match";
					break;
				case TotalizationResult.NoContributors:
					strResult = "Some contributors do not contain load profile data";
					break;
				case TotalizationResult.QuantityMismatch:
					strResult = "Load profile channel quantities do not match";
					break;
				case TotalizationResult.NoCommonIntervalData:
					strResult = "A common date range does not exist between all contributors";
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
				if (m_lpData == null)
				{
					Totalize(out m_lpData);
				}

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
				// If the next contributor's data end's before the current start time
				// then we have nothing to totalize
				else if (dtStartTime > m_lstDataSources[nContributorIndex].ProfileEndTime)
				{
					dtStartTime = DateTime.MinValue;
				}
				// If the next contributor's data starts after the current start time
				// then use the later of the two values
				else if (dtStartTime < m_lstDataSources[nContributorIndex].ProfileStartTime)
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
				// If the next contributor's data starts after the current end time
				// then we have nothing to totalize
				else if (dtEndTime < m_lstDataSources[nContributorIndex].ProfileStartTime)
				{
					dtEndTime = DateTime.MinValue;
				}
				// If the next contributor's data ends before the current end time then use the
				// earlier of the two values
				else if (dtEndTime > m_lstDataSources[nContributorIndex].ProfileEndTime)
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
		/// <param name="lpResult" type="Itron.Metering.DeviceDataTypes.LoadProfileEnergyData">
		/// </param>
		/// <returns>
		///     A Itron.Metering.Datafiles.TotalizationContributorList.TotalizationResult value...
		/// </returns>
		private TotalizationResult Totalize(out LoadProfileEnergyData lpResult)
		{
			lpResult = null;

			TotalizationResult Result = ValidateDataSources();

			if (Result == TotalizationResult.Success)
			{
				lpResult = new LoadProfileEnergyData(IntervalLength);

				// Add the appropriate number of channels
				for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
				{
					// The channels will assume the same name as the first contributor
					lpResult.AddChannel(m_lstDataSources[0].LPData.Channels[nChannelIndex].ChannelName,
										(float)1.0,
										(float)1.0);
				}

				// Next combine the load profile data from all the contributors
				foreach (LoadProfileDataSource ProfileContributor in m_lstDataSources)
				{
					TotalizationDataSource contributor = ProfileContributor as TotalizationDataSource;

					if (contributor != null)
					{
						TotalizeIntervalData(lpResult, contributor.LPData.EnergyData, contributor.Calculation);
					}
				}
			}

			return Result;
		}


		/// <summary>
		///     
		/// </summary>
		/// <param name="lpResult" type="Itron.Metering.DeviceDataTypes.LoadProfileEnergyData">
		/// </param>
		/// <param name="lpContributor" type="Itron.Metering.DeviceDataTypes.LoadProfileEnergyData">
		/// </param>
		/// <param name="eCalculation" type="Itron.Metering.Datafiles.TotalizationContributorList.CalculationType">
		/// </param>
		private void TotalizeIntervalData(LoadProfileEnergyData lpResult,
					LoadProfileEnergyData lpContributor,
					TotalizationDataSource.CalculationType eCalculation)
		{
			if (lpResult.NumberIntervals == 0)
			{
				// This is the first contributor

				// Find the starting point
				int nIntervalIndex = FindIntervalIndex(lpContributor, ProfileStartTime);

				bool boolFinished = false;

				if (nIntervalIndex >= 0)
				{
					while (nIntervalIndex < lpContributor.NumberIntervals && !boolFinished)
					{
						if (lpContributor.Intervals[nIntervalIndex].Time <= ProfileEndTime)
						{
							lpResult.Intervals.Add(lpContributor.Intervals[nIntervalIndex]);

							if (eCalculation == TotalizationDataSource.CalculationType.Subtraction)
							{
								NegateLastInterval(lpResult);
							}

							// Move to the next interval
							nIntervalIndex++;
						}
						else
						{
							boolFinished = true;
						}
					}
				}
			}
			else
			{
				// Find the starting point
				int nResultIndex = 0;
				int nContributorIndex = FindIntervalIndex(lpContributor, ProfileStartTime);

				bool boolFinished = false;

				if (nContributorIndex >= 0)
				{
					while (nContributorIndex < lpContributor.NumberIntervals && !boolFinished)
					{
						if (lpContributor.Intervals[nContributorIndex].Time <= ProfileEndTime)
						{
							// The two time stamps must match
							CombineIntervals(lpResult.Intervals[nResultIndex],
								lpContributor.Intervals[nContributorIndex],
								eCalculation);

							// Move to the next interval
							nResultIndex++;
							nContributorIndex++;
						}
						else
						{
							boolFinished = true;
						}
					}
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="lpResult" type="Itron.Metering.DeviceDataTypes.LoadProfileEnergyData">
		///     <para>
		///         
		///     </para>
		/// </param>
		private void NegateLastInterval(LoadProfileEnergyData lpResult)
		{
			LPInterval lpiLastInterval = lpResult.Intervals[lpResult.Intervals.Count - 1];

			// Negate each interval
			for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
			{
				lpiLastInterval.Data[nChannelIndex] = -lpiLastInterval.Data[nChannelIndex];
			}
		}
		
		/// <summary>
		///     
		/// </summary>
		/// <param name="lpiResult" type="Itron.Metering.DeviceDataTypes.LPInterval">
		/// </param>
		/// <param name="lpiContributor" type="Itron.Metering.DeviceDataTypes.LPInterval">
		/// </param>
		/// <param name="eCalculation" type="Itron.Metering.Datafiles.TotalizationContributorList.CalculationType">
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		private Boolean CombineIntervals(LPInterval lpiResult,
			LPInterval lpiContributor,
			TotalizationDataSource.CalculationType eCalculation)
		{
			Boolean boolSuccessful = false;

			if (lpiContributor.Time == lpiResult.Time)
			{
				for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
				{
					if (eCalculation == TotalizationDataSource.CalculationType.Addition)
					{
						lpiResult.Data[nChannelIndex] += lpiContributor.Data[nChannelIndex];
					}
					else
					{
						lpiResult.Data[nChannelIndex] -= lpiContributor.Data[nChannelIndex];
					}
				}

				// Combine the interval statuses - assuming the new contributor has a 
				// status value...
				if (!String.IsNullOrEmpty(lpiContributor.IntervalStatus))
				{
					CombineIntervalStatuses(lpiResult, lpiContributor);
				}

				boolSuccessful = true;
			}

			return boolSuccessful;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="lpiResult" type="Itron.Metering.DeviceDataTypes.LPInterval">
		/// </param>
		/// <param name="lpiContributor" type="Itron.Metering.DeviceDataTypes.LPInterval">
		/// </param>
		private static void CombineIntervalStatuses(LPInterval lpiResult, LPInterval lpiContributor)
		{
			if (String.IsNullOrEmpty(lpiResult.IntervalStatus))
			{
				lpiResult.IntervalStatus = lpiContributor.IntervalStatus;
			}
			else // both the totalized data and the new contributor have statuses
			{
				for (int nStatusIndex = 0; nStatusIndex < lpiContributor.IntervalStatus.Length; nStatusIndex++)
				{
					if (lpiResult.IntervalStatus.IndexOf(lpiContributor.IntervalStatus[nStatusIndex]) < 0)
					{
						lpiResult.IntervalStatus += lpiContributor.IntervalStatus[nStatusIndex].ToString();
					}
				}
			}
		}

		#endregion

		#region Members

		private LoadProfileEnergyData m_lpData;

		#endregion
	}
}
