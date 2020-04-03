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
using Itron.Metering.Utilities;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Datafiles
{
	/// <summary>
	/// This class represents a single load profile object that will be used 
	/// in another operation.  Note that this class differs from the normal
	/// load profile data class in that it contains properties to fully 
	/// describe the data source as well as the data itself.
	/// </summary>
	public class LoadProfileDataSource
	{
		#region Public Methods

		/// <summary>
		/// A load profile contributor starts with a data source.  In this case 
		/// the data source in an HHF file.  Therefore, in order to construct a new  
		/// contributor object you must pass in an HHF file object.  Note that it could 
		/// be either a MIF file or an MV-90 HHF file.
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		public LoadProfileDataSource(HHFFile hhf)
		{
			FileName = hhf.FileName;
			UnitID = hhf.DeviceID;

			if (hhf.ContainsLPData)
			{
				LPData = hhf.LPData;
				DSTEnabled = hhf.DSTEnabled;
				ReadTime = hhf.TimeOfRead;
			}
			else
			{
				LPData = null;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// This property represents the full path name of the data source    
		/// </summary>
		public String FileName
		{
			get
			{
				return m_strFileName;
			}
			protected set
			{
				m_strFileName = value;
			}
		}

		/// <summary>
		/// This property represents the unit id of the data source    
		/// </summary>
		public String UnitID
		{
			get
			{
				return m_strUnitID;
			}
			
			protected set
			{
				// We really should remove any unnecessary spaces surrounding
				// the unit ID.  This is primarily for display purposes but we
				// also may need to use the unit ID when creating a file name and
				// the extra padding just gets in the way.
				m_strUnitID = value.Trim(); 
			}
		}

		/// <summary>
		/// The property returns the load profile data from the contributor    
		/// </summary>
		public LoadProfileData LPData
		{
			get
			{
				return m_lpData;
			}
			protected set
			{
				m_lpData = value;
			}
		}

		/// <summary>
		/// This property returns the date/time of the first load profile 
		/// found in the contributor.  Note that if no load profile data is 
		/// available, the system's minimum date/time will be returned
		/// </summary>
		public DateTime ProfileStartTime
		{
			get
			{
				if (m_lpData != null)
				{
					return m_lpData.StartTime;
				}
				else
				{
					return DateTime.MinValue;
				}
			}
		}

		/// <summary>
		/// This property returns the date/time of the last load profile 
		/// found in the contributor.  Note that if no load profile data is 
		/// available, the system's minimum date/time will be returned
		/// </summary>
		public DateTime ProfileEndTime
		{
			get
			{
				if (m_lpData != null)
				{
					return m_lpData.EndTime;
				}
				else
				{
					return DateTime.MinValue;
				}
			}
		}

		/// <summary>
		/// This property returns the length of each load profie interval.  
		/// If no load profile data is available, a value of 0 will be returned
		/// </summary>
		public int IntervalLength
		{
			get
			{
				if (m_lpData != null)
				{
					return m_lpData.IntervalDuration;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// This property returns the number of load profile channels defined by this 
		/// contributor. If no load profile data is available, a value of 0 will be 
		/// returned
		/// </summary>
		public int NumProfileChannels
		{
			get
			{
				if (m_lpData != null)
				{
					return m_lpData.NumberOfChannels;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// This property indicates whether or not the load profile data 
		/// from this contributor follows daylight savings time or not.  If no 
		/// load profile data is available, the property will return a value 
		/// of false
		/// </summary>
		public bool DSTEnabled
		{
			get
			{
				return m_boolDSTEnabled;
			}
			protected set
			{
				m_boolDSTEnabled = value;
			}
		}

		/// <summary>
		/// This property returns the date/time that the load profile data was created    
		/// and/or read from the device.
		/// </summary>
		public DateTime ReadTime
		{
			get
			{
				return m_dtReadTime;
			}
			protected set
			{
				m_dtReadTime = value;
			}
		}

		#endregion

		#region Members

		private String m_strFileName;
		private String m_strUnitID;
		private LoadProfileData m_lpData;
		private bool m_boolDSTEnabled;
		private DateTime m_dtReadTime;

		#endregion
	}

	/// <summary>
	/// This class represents a collection of load profile objects 
	/// (contributors) that are to be acted on as a unit.  In other words,
	/// the contributors are operands for operations such as totalization 
	/// and aggregation
	/// </summary>
	public abstract class LoadProfileCollection
	{
		#region Public Methods

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		public LoadProfileCollection()
		{
			m_lstDataSources = new List<LoadProfileDataSource>();

			m_nIntervalLength = new CachedInt();
			m_nNumProfileChannels = new CachedInt();
			m_dtProfileStartTime = new CachedDate();
			m_dtProfileEndTime = new CachedDate();
			m_dtDSTStartTime = new CachedDate();
			m_dtDSTStopTime = new CachedDate();
		}

		/// <summary>
		/// This method adds the given file to the contributor list.  In doing so,
		/// the file will be opened and the load profile data will be extracted.  Note 
		/// that the file should not remain open.  
		/// </summary>
		/// <param name="strFileName" type="string">
		/// </param>
		/// <returns>
		/// A value of true is returned if the data could be successfully read from
		/// contributing file.  A value of false is returned if no load profile data
		/// is present
		/// </returns>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		public abstract Boolean AddContributor(String strFileName);

		#endregion

		#region Public Properties

		/// <summary>
		/// This property returns the common interval length of all contributing
		/// data files.  If any two contributors do not have the use the same 
		/// load profile interval length, then a value of 0 is returned.
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		public int IntervalLength
		{
			get
			{
				if (!m_nIntervalLength.Cached)
				{
					int nCommonIntervalLength = 0;

					for (int nSourceIndex = 0; nSourceIndex < m_lstDataSources.Count; nSourceIndex++)
					{
						if (nSourceIndex == 0)
						{
							nCommonIntervalLength = m_lstDataSources[nSourceIndex].IntervalLength;
						}
						else if (nCommonIntervalLength != m_lstDataSources[nSourceIndex].IntervalLength)
						{
							nCommonIntervalLength = 0;
						}
					}

					m_nIntervalLength.Value = nCommonIntervalLength;
				}

				return m_nIntervalLength.Value;
			}
		}

		/// <summary>
		/// This property returns the number of load profile channels shared by 
		/// all contributors.  In other words - it is the least common denominator
		/// of all of the contributors
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		public virtual int NumProfileChannels
		{
			get
			{
				if (!m_nNumProfileChannels.Cached)
				{
					int nNumberOfChannels = 0;

					for (int nSourceIndex = 0; nSourceIndex < m_lstDataSources.Count; nSourceIndex++)
					{
						if (nSourceIndex == 0)
						{
							nNumberOfChannels = m_lstDataSources[nSourceIndex].NumProfileChannels;
						}
						else if (nNumberOfChannels > m_lstDataSources[nSourceIndex].NumProfileChannels)
						{
							nNumberOfChannels = m_lstDataSources[nSourceIndex].NumProfileChannels;
						}
					}

					// Add yet another check to make sure that we do not exceed the 
					// maximum number of channels in an HHF file.  But instead of returning
					// an error, simply reduce the number of channels.
					if (nNumberOfChannels > MV90_HHF.MAX_CHANNELS)
					{
						nNumberOfChannels = MV90_HHF.MAX_CHANNELS;
					}

					m_nNumProfileChannels.Value = nNumberOfChannels;
				}

				return m_nNumProfileChannels.Value;
			}
		}

		/// <summary>
		/// This property represents the effective load profile start time of the
		/// contributor list.  Note that the exact meaning of the profile start time 
		/// will be different for each of the derived classes.  For example, when 
		/// totalizing data, the profile start time will be the least common start time 
		/// of all of the data sources yet, when joining data files, the start time will
		/// be the earliest start time of all of the data sources
		/// </summary>
		virtual public DateTime ProfileStartTime
		{
			get
			{
				if (!m_dtProfileStartTime.Cached)
				{
					m_dtProfileStartTime.Value = CalculateProfileStartTime();
				}

				return m_dtProfileStartTime.Value;
			}
		}

		/// <summary>
		/// This property represents the effective load profile end time of the
		/// contributor list.  Note that the exact meaning of the profile end time 
		/// will be different for each of the derived classes.  For example, when 
		/// totalizing data, the profile end time will be the greatest common end time 
		/// of all of the data sources yet, when joining data files, the end time will
		/// be the last available interval found in all of the data sources
		/// </summary>
		virtual public DateTime ProfileEndTime
		{
			get
			{
				if (!m_dtProfileEndTime.Cached)
				{
					m_dtProfileEndTime.Value = CalculateProfileEndTime();
				}

				return m_dtProfileEndTime.Value;
			}
		}


		/// <summary>
		/// The property returns the combined load profile data from all valid
		/// contributors.  This property is an abstract property so that the derived 
		/// classes can insert unique logic to calculate and/or manipulate the load 
		/// profile data sources to produce a single load profile data object.
		/// </summary>
		abstract public LoadProfileData LPData
		{
			get;
		}

		/// <summary>
		/// This property provides access to the list of contributors that 
		/// comprise this collection of load profile data.  Note that callers should NOT
		/// attempt to add or delete data sources directly from the data source list.
		/// </summary>
		public List<LoadProfileDataSource> DataSources
		{
			get
			{
				return m_lstDataSources;
			}
		}

		/// <summary>
		/// This property indicates if all of the contributors follow DST or not.  Note 
		/// that this method assumes that all of the contributors use identical DST 
		/// configurations
		/// </summary>
		public bool DSTEnabled
		{
			get
			{
				if (m_lstDataSources.Count == 0)
				{
					return false;
				}
				else
				{
					return m_lstDataSources[0].DSTEnabled;
				}
			}
		}


		#endregion

		#region Protected Methods

		/// <summary>
		/// This method returns the index of the load profile interval that ends on the 
		/// given date/time.  If the given contributor does not contain profile data for
		/// the given date/time, then a value of -1 will be returned
		/// </summary>
		/// <param name="lpContributor" type="Itron.Metering.DeviceDataTypes.LoadProfileData">
		/// </param>
		/// <param name="dtTarget" type="System.DateTime">
		/// </param>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		protected int FindIntervalIndex(LoadProfileData lpContributor, DateTime dtTarget)
		{
			int nIntervalIndex = 0;
			Boolean boolDateFound = false;

			while (nIntervalIndex < lpContributor.NumberIntervals && !boolDateFound)
			{
				if (lpContributor.Intervals[nIntervalIndex].Time == dtTarget)
				{
					boolDateFound = true;
				}
				else
				{
					nIntervalIndex++;
				}
			}

			if (!boolDateFound)
			{
				nIntervalIndex = -1;
			}

			return nIntervalIndex;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="x" type="Itron.Metering.Datafiles.LoadProfileContributor">
		/// </param>
		/// <param name="y" type="Itron.Metering.Datafiles.LoadProfileContributor">
		/// </param>
		/// <returns>
		///     A int value...
		/// </returns>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		protected static int CompareStartTime(LoadProfileDataSource x, LoadProfileDataSource y)
		{
			if (x == null)
			{
				if (y == null)
				{
					// If x is null and y is null, they're
					// equal. 
					return 0;
				}
				else
				{
					// If x is null and y is not null, y
					// is greater. 
					return -1;
				}
			}
			else
			{
				// If x is not null...
				//
				if (y == null)
				// ...and y is null, x is greater.
				{
					return 1;
				}
				else
				{
					// ...and y is not null, compare the timestampts of the two items
					//
					int retval = x.ProfileStartTime.CompareTo(y.ProfileStartTime);

					return retval;
				}
			}
		}

		/// <summary>
		/// This method returns true if every contributor in the list shares the same 
		/// DST setting or false if one or more differences are found
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		protected Boolean VerifyDSTConsistency()
		{
			// go through the list of contributors and make sure that each
			// and every one use the same DST settings - not necessarily the
			// same dates but they should all use the same setting
			Boolean boolDSTEnabled = false;
			Boolean boolDSTSettingsMatch = true;

			for (int nContributorIndex = 0; nContributorIndex < m_lstDataSources.Count; nContributorIndex++)
			{
				if (nContributorIndex == 0)
				{
					boolDSTEnabled = m_lstDataSources[nContributorIndex].DSTEnabled;
				}
				else if (boolDSTEnabled != m_lstDataSources[nContributorIndex].DSTEnabled)
				{
					boolDSTSettingsMatch = false;
					break;
				}
			}

			return boolDSTSettingsMatch;
		}

		/// <summary>
		/// This method returns true if every contributor in the list shares the same 
		/// channel definitions or false if one or more differences are found
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/25/08 mah 9.50.00		Created
		///  
		/// </remarks>
		protected Boolean VerifyChannelConsistency()
		{
			Boolean boolChannelsMatch = true;
			String[] lstReferenceNames = new String[NumProfileChannels];

			// Use the first contributor as the reference point - Note that we
			// will not necesarily look at all channels for each contributor, just 
			// the channels that will be totalized
			for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
			{
				lstReferenceNames[nChannelIndex] = m_lstDataSources[0].LPData.Channels[nChannelIndex].ChannelName;
			}

			// Next verify that all of the contributors use the same channel definition 
			// - start with the second contributor
			int nSourceIndex = 1;

			while ((nSourceIndex < m_lstDataSources.Count) && boolChannelsMatch)
			{
				for (int nChannelIndex = 0; nChannelIndex < NumProfileChannels; nChannelIndex++)
				{
					if (lstReferenceNames[nChannelIndex] != m_lstDataSources[0].LPData.Channels[nChannelIndex].ChannelName)
					{
						boolChannelsMatch = false;
					}
				}

				// If everything looks good at this point, move on to the next
				// contributor
				if (boolChannelsMatch)
				{
					nSourceIndex++;
				}
			}

			return boolChannelsMatch;
		}

		/// <summary>
		/// This method returns true if every contributor in the list shares the same  
		/// number of load profile channels or false if differences are found
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  12/22/08 mah 9.50.28		Created
		///  
		/// </remarks>
		protected Boolean VerifyChannelCount()
		{
			Boolean boolChannelsMatch = true;

			// Use the first contributor as the reference point - Note that we
			// will not necesarily look at all channels for each contributor, just 
			// the channels that will be totalized
			int nNumberOfChannels = m_lstDataSources[0].LPData.Channels.Count;

			// Next verify that all of the contributors use the same channel definition 
			// - start with the second contributor
			int nSourceIndex = 1;

			while ((nSourceIndex < m_lstDataSources.Count) && boolChannelsMatch)
			{
				if (nNumberOfChannels != m_lstDataSources[nSourceIndex].LPData.Channels.Count)
				{
					boolChannelsMatch = false;
				}

				// If everything looks good at this point, move on to the next
				// contributor
				if (boolChannelsMatch)
				{
					nSourceIndex++;
				}
			}

			return boolChannelsMatch;
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
		virtual protected void FlushCachedValues()
		{
			m_nIntervalLength.Flush();
			m_nNumProfileChannels.Flush();
			m_dtProfileStartTime.Flush();
			m_dtProfileEndTime.Flush();
			m_dtDSTStartTime.Flush();
			m_dtDSTStopTime.Flush();
		}

		/// <summary>
		/// This method will return the effective profile end date for the derived 
		/// collection.  Note that the exact meaning of the profile start time 
		/// will be different for each of the derived classes.  For example, when 
		/// totalizing data, the profile start time will be the least common start time 
		/// of all of the data sources yet, when joining data files, the start time will
		/// be the earliest start time of all of the data sources
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/26/08 mah 9.50.00		Created
		///  
		/// </remarks>
		abstract protected DateTime CalculateProfileStartTime();

		/// <summary>
		/// This method will return the effective profile end date for the derived  
		/// collection.  Note that the exact meaning of the profile end time 
		/// will be different for each of the derived classes.  For example, when 
		/// totalizing data, the profile end time will be the greatest common end time 
		/// of all of the data sources yet, when joining data files, the end time will
		/// be the last available interval found in all of the data sources
		/// </summary>
		/// <remarks>
		///  Revision History
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ --------------------------------------------- 
		///  08/26/08 mah 9.50.00		Created
		///  
		/// </remarks>
		abstract protected DateTime CalculateProfileEndTime();

		#endregion

		#region Members

		/// <summary>
		/// The internal list of contributing objects    
		/// </summary>
		protected List<LoadProfileDataSource> m_lstDataSources;

		/// <summary>
		/// This member holds the interval length common to all members of the load 
		/// profile collection
		/// </summary>
		protected CachedInt m_nIntervalLength;

		/// <summary>
		/// This cached member holds the number of profile channels that all members of 
		/// the load profile collection currently contain.
		/// </summary>
		protected CachedInt m_nNumProfileChannels;

		/// <summary>
		/// This cached member maintains the effective start date of the load profile 
		/// collection.  This value is cached so that it does not have to be calculated 
		/// each time it is accessed.
		/// </summary>
		protected CachedDate m_dtProfileStartTime;

		/// <summary>
		/// This cached member maintains the effective end date of the load profile 
		/// collection.  This value is cached so that it does not have to be calculated 
		/// each time it is accessed.
		/// </summary>
		protected CachedDate m_dtProfileEndTime;

		/// <summary>
		/// This cached member maintains common DST start date used by all members of 
		/// the load profile collection
		/// </summary>
		protected CachedDate m_dtDSTStartTime;

		/// <summary>
		/// This cached member maintains common DST start date used by all members of 
		/// the load profile collection
		/// </summary>
		protected CachedDate m_dtDSTStopTime;

		#endregion
	}

}
