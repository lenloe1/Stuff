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
//                              Copyright © 2008
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Scale options for displaying data
    /// </summary>
    public enum DisplayScaleOptions
    {
        /// <summary>
        /// Units
        /// </summary>
        UNITS,
        /// <summary>
        /// Kilo
        /// </summary>
        KILO,
        /// <summary>
        /// Mega
        /// </summary>
        MEGA,
    }

    /// <summary>
    /// This class represent a set of Load Profile data.  It is the abstract base class
    /// for the pulse, energy, or demand load profile data objects.
    /// </summary>
    public abstract class LoadProfileData
    {
        #region Public Methods

        /// <summary>
        /// Constructor to instantiate a LoadProfile object
        /// </summary>
        /// <param name="IntervalDuration"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public LoadProfileData(int IntervalDuration)
        {
            m_iIntervalDuration = IntervalDuration;
            m_liIntervals = new List<LPInterval>();
            m_llpcChannels = new List<LPChannel>();

            m_bCalculateVar = false;
            m_bCalculateVA = false;
            m_bCalculatePF = false;
            m_iWattIndex = -1;
            m_iVarIndex = -1;
            m_iVAIndex = -1;
            m_iQIndex = -1;

            m_DataSetName = null;
        }

        /// <summary>
        /// Adds an interval to the load profile object
        /// </summary>
        /// <param name="Data">Interval data</param>
        /// <param name="ChanStatus">Channel statuses</param>
        /// <param name="IntStatus">Interval statuses</param>
        /// <param name="Time">Time of the interval</param>
        /// <param name="Scale">The current scale of the data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public void AddInterval(double[] Data, string[] ChanStatus, string IntStatus, DateTime Time, DisplayScaleOptions Scale)
        {
            LPInterval NewInterval = new LPInterval(m_liIntervals.Count, Data, ChanStatus, IntStatus, Time, Scale);
            m_liIntervals.Add(NewInterval);
        }

        /// <summary>
        /// Adds a channel to load profile.
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="fPulseWeight">Pulse weight of the channel</param>
        /// <param name="fMultiplier">Multiplier for the channel</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public void AddChannel(string strName, float fPulseWeight, float fMultiplier)
        {
            LPChannel NewChannel = new LPChannel(strName, m_llpcChannels.Count, fPulseWeight, fMultiplier, ref m_liIntervals);
            m_llpcChannels.Add(NewChannel);
        }

        /// <summary>
        /// Adds a channel to the load profile object.
        /// </summary>
        /// <param name="Channel">New channel to add.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public virtual void AddChannel(LPChannel Channel)
        {
            LPChannel NewChannel = Channel.CreateChannel(Channel.ChannelName, m_llpcChannels.Count, Channel.PulseWeight, Channel.Multiplier, ref m_liIntervals);
            m_llpcChannels.Add(NewChannel);
        }

        /// <summary>
        /// Returns a set of load profile data that encompasses the given time range
        /// </summary>
        /// <param name="dtStart">start time for the time range</param>
        /// <param name="dtEnd">end time for the time range</param>
        /// <returns>Load profile data in the new range.</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public LoadProfileData GetRange(DateTime dtStart, DateTime dtEnd)
        {
            //Some interval data has a second added, so adjust the stop time to 
            DateTime dtLPStop = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour, dtEnd.Minute, 1);

            //Set the start time to the end time of the interval
            DateTime dtLPStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, dtStart.Minute, 0);
            dtLPStart = dtLPStart.AddMinutes(m_iIntervalDuration);

            LoadProfileData LPRange = GetNewLPData(m_iIntervalDuration);

            //Add the channels to the new load profile
            for (int i = 0; i < m_llpcChannels.Count; i++)
            {
                LPRange.AddChannel(m_llpcChannels[i]);
            }

            //Add every interval that falls within the given range
            for (int i = 0; i < m_liIntervals.Count; i++)
            {
                if (m_liIntervals[i].Time.CompareTo(dtLPStart) >= 0 &&
                   m_liIntervals[i].Time.CompareTo(dtLPStop) <= 0)
                {
                    LPRange.AddInterval(m_liIntervals[i]);
                }
            }

            return LPRange;
        }

        /// <summary>
        /// This method returns the new combined load profile data.  If it is unable
        /// to combine the intervals then it will just return the data as is.
        /// </summary>
        /// <param name="iNewDuration">
        /// The new interval length duration for the load profile data.
        /// </param>
        /// <returns>
        /// The new combined load profile data.
        /// </returns>
        /// <exception cref="NotSupportedException">This exception is thrown if the 
        /// data is demand.  We would have to convert to energy, combine intervals,
        /// and then convert back to demand.  So this is currently not supported.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if the new interval length is either smaller than
        /// the current interval length or it is not a multiple of the current interval
        /// length.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/17/07 mrj 9.00.00		Created
        //  
        public LoadProfileData CombineIntervals(int iNewDuration)
        {
            LoadProfileData LPCombined;
            int iIndex = 0;
            bool bFirstIntervalFound = false;
            int iNumToCombine = iNewDuration / m_iIntervalDuration;

            if (this is LoadProfileDemandData)
            {
                //We don't support combining demand intervals.  Data must be in pulse
                //or energy, combined, and then converted to demand.
                throw new NotSupportedException("Unable to combine demand intervals");
            }
            else if (m_liIntervals.Count == 0)
            {
                throw new ArgumentException("Unable to combine intervals.  There are no intervals present.");
            }
            else if (iNewDuration < m_iIntervalDuration)
            {
                //We cannot combine these intervals because the new duration
                //is too small.
                throw new ArgumentException("Unable to combine intervals.  The " +
                                            "new interval length is smaller than " +
                                            "the current interval length.");
            }
            else if ((iNewDuration % m_iIntervalDuration) != 0)
            {
                //We cannot combine these intervals because the new duration is 
                //not a multiple of the current interval legth.
                throw new ArgumentException("Unable to combine intervals.  The " +
                                            "new interval length is not a " +
                                            "multiple of the current interval length.");
            }
            else
            {
                //Create a new load profile object with the new duration
                LPCombined = GetNewLPData(iNewDuration);

                //Add the channels to the new load profile
                for (int i = 0; i < m_llpcChannels.Count; i++)
                {
                    LPCombined.AddChannel(m_llpcChannels[i]);
                }

                //Get the first interval
                if (m_liIntervals[0].Time.Minute == 0)
                {
                    //This is the first interval since it ends an hour, so just add this interval
                    LPCombined.AddInterval(m_liIntervals[0]);
                    LPCombined.Intervals[0].IntervalStatus = ConcatenateStatuses(m_liIntervals[0].IntervalStatus, "S");
                    iIndex++;
                }
                else
                {
                    //This is not the first interval in the hour so we need to combine intervals
                    //to get the first interval
                    DateTime IntervalTime = new DateTime(m_liIntervals[0].Time.Year,
                                                     m_liIntervals[0].Time.Month,
                                                     m_liIntervals[0].Time.Day,
                                                     m_liIntervals[0].Time.Hour,
                                                     0,
                                                     m_liIntervals[0].Time.Second);

                    //Loop through the hour until we get a combined interval that includes the 
                    //first interval of data.
                    while (iIndex < m_liIntervals.Count &&
                           !bFirstIntervalFound)
                    {
                        double[] data = new double[m_liIntervals[0].Data.Length];
                        string[] statuses = new string[m_liIntervals[0].Data.Length];
                        string strIntervalStatus = "";
                        int TempIntervalLength = 0;
                        int iCurrentNumCombine = 0;

                        while (TempIntervalLength != iNewDuration)
                        {
                            IntervalTime = IntervalTime.AddMinutes((double)m_iIntervalDuration);

                            if (m_liIntervals[iIndex].Time.Minute == IntervalTime.Minute)
                            {
                                //We found an interval to combine
                                for (int iChannel = 0; iChannel < m_liIntervals[iIndex].Data.Length; iChannel++)
                                {
                                    data[iChannel] += m_liIntervals[iIndex].Data[iChannel];
                                    statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex].ChannelStatuses[iChannel]);
                                }
                                strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex].IntervalStatus);

                                iIndex++;
                                iCurrentNumCombine++;
                                bFirstIntervalFound = true;
                            }

                            TempIntervalLength += m_iIntervalDuration;
                        }

                        if (bFirstIntervalFound)
                        {
                            if (iNumToCombine != iCurrentNumCombine)
                            {
                                //This combined interval does not contain all intervals so mark it as short
                                strIntervalStatus = ConcatenateStatuses(strIntervalStatus, "S");
                            }

                            //Add the first interval
                            LPCombined.AddInterval(data, statuses, strIntervalStatus, IntervalTime, m_liIntervals[0].DisplayScale);
                        }
                    }
                }

                //Get the rest of the intervals				
                while (iIndex < m_liIntervals.Count)
                {
                    DateTime IntervalTime = m_liIntervals[iIndex].Time;
                    double[] data = new double[m_liIntervals[0].Data.Length];
                    string[] statuses = new string[m_liIntervals[0].Data.Length];
                    string strIntervalStatus = "";
                    int iCurrentNumCombine = 0;

                    //Loop through the number of intervals to combine
                    for (int i = 0; i < iNumToCombine && iIndex < m_liIntervals.Count; i++)
                    {
                        //Combine the intervals
                        for (int iChannel = 0; iChannel < m_liIntervals[iIndex].Data.Length; iChannel++)
                        {
                            data[iChannel] += m_liIntervals[iIndex].Data[iChannel];
                            statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex].ChannelStatuses[iChannel]);
                        }
                        strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex].IntervalStatus);

                        IntervalTime = m_liIntervals[iIndex].Time;
                        iIndex++;
                        iCurrentNumCombine++;
                    }

                    if (iNumToCombine == iCurrentNumCombine)
                    {
                        //This is a full interval so add it
                        LPCombined.AddInterval(data, statuses, strIntervalStatus, IntervalTime, m_liIntervals[0].DisplayScale);
                    }
                }
            }

            //Calculated channels should not have been added yet, but if they were then re-calculate
            LPCombined.ReCalculateChannels();

            return LPCombined;
        }

        /// <summary>
        /// Method used to get rolling demand data.
        /// </summary>
        /// <param name="iNumIntervalsToCombine">
        /// Number of intervals to combine when doing rolling demand.
        /// </param>
        /// <returns>
        /// Load profile demand data rolled based on the supplied demand length.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if the demand length is larger than an hour or
        /// it is not divible evenly in an hour.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/29/07 mrj 9.00.00		Created
        //  
        public abstract LoadProfileDemandData CalculateRollingDemand(int iNumIntervalsToCombine);

        /// <summary>
        /// Return the number of intervals that had a status, channel or interval status.
        /// </summary>
        /// <param name="iChannel">
        /// The requesetd channel number to get the status count.
        /// </param>
        /// <returns>
        /// Returns the number of intervals that had a status.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/26/07 mrj 9.00.00		Created
        //  
        public int GetNumberOfStatusedChannels(int iChannel)
        {
            int iNumStatused = 0;

            foreach (LPInterval lpInterval in m_liIntervals)
            {
                if (lpInterval.HasChannelStatuses.Length > iChannel)
                {
                    if (lpInterval.HasChannelStatuses[iChannel] ||
                        lpInterval.HasIntervalStatus)
                    {
                        iNumStatused++;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("iChannel", "Invalid channel requested in GetNumberOfStatusedChannels");
                }
            }

            return iNumStatused;
        }

        /// <summary>
        /// Return the number of intervals that had a status, channel or interval status.
        /// The "D" status is filtered out in this method
        /// </summary>
        /// <param name="iChannel">
        /// The requesetd channel number to get the status count.
        /// </param>
        /// <returns>
        /// Returns the number of intervals that had a status, but not the "D" status.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/13/07 mrj 9.00.26 3415	Created
        //  
        public int GetNumberOfFilteredStatusedChannels(int iChannel)
        {
            int iNumStatused = 0;

            foreach (LPInterval lpInterval in m_liIntervals)
            {
                if (lpInterval.HasChannelStatuses.Length > iChannel)
                {
                    if (lpInterval.HasFilteredChannelStatuses[iChannel] ||
                        lpInterval.HasFilteredIntervalStatus)
                    {
                        iNumStatused++;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("iChannel", "Invalid channel requested in GetNumberOfStatusedChannels");
                }
            }

            return iNumStatused;
        }

        /// <summary>
        /// This method changes the pulse weight for the given channel.
        /// </summary>
        /// <param name="iChannel">
        /// The channel to change the pulse weight of.
        /// </param>
        /// <param name="fPulseWeight">
        /// The new pulse weight for the supplied channel.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/27/07 mrj 9.00.00		Created
        //  
        public void ChangeChannelPulseWeight(int iChannel, float fPulseWeight)
        {
            m_llpcChannels[iChannel].PulseWeight = fPulseWeight;
        }

        /// <summary>
        /// This method changes the channel's name.
        /// </summary>
        /// <param name="iChannel">Index for the channel.</param>
        /// <param name="strName">Name for the channel.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/07 mrj 9.00.00		Created
        //  
        public void ChangeChannelName(int iChannel, string strName)
        {
            m_llpcChannels[iChannel].ChannelName = strName;
        }

        /// <summary>
        /// This method changes the multiplier for the given channel.
        /// </summary>
        /// <param name="iChannel">
        /// The channel to change the multiplier of.
        /// </param>
        /// <param name="fMultiplier">
        /// The new multiplier for the supplied channel.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/07 mrj 9.00.00		Created
        //  
        public virtual void ChangeChannelMultiplier(int iChannel, float fMultiplier)
        {
            m_llpcChannels[iChannel].Multiplier = fMultiplier;
        }

        /// <summary>
        /// This method stores off information so that vars can be calculated.
        /// </summary>
        /// <param name="iWattIndex">Index to the watt channel.</param>
        /// <param name="iVAIndex">Index to the VA channel, -1 if not used.</param>
        /// <param name="iQIndex">Index to the Q channel, -1 if not used.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if any of the indexes are out of range.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        public virtual void AddCalculatedVar(int iWattIndex, int iVAIndex, int iQIndex)
        {
            if (iWattIndex >= m_llpcChannels.Count ||
                iVAIndex >= m_llpcChannels.Count ||
                iQIndex >= m_llpcChannels.Count)
            {
                throw new ArgumentException("Vars cannot be calculated with the given channel indexes.");
            }

            m_bCalculateVar = true;

            m_iWattIndex = iWattIndex;

            if (iVAIndex != -1)
            {
                m_iVAIndex = iVAIndex;
            }
            else
            {
                m_iQIndex = iQIndex;
            }

            //If this is not pulse data then go ahead and calculate the vars
            if (!(this is LoadProfilePulseData))
            {
                if (m_iWattIndex != -1 && (m_iVAIndex != -1 || m_iQIndex != -1))
                {
                    int iVarChannel = m_llpcChannels.Count;
                    CalculatedVarChannel NewChannel = new CalculatedVarChannel(iVarChannel, ref m_liIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);

                    if (this is LoadProfileDemandData)
                    {
                        NewChannel.ChangeNameToDemand();
                    }

                    m_llpcChannels.Add(NewChannel);

                    CalculateVarChannel(iVarChannel);
                }

                //We are done so mark it as false so we don't do it again.
                m_bCalculateVar = false;
            }
        }

        /// <summary>
        /// This method stores off information so that VA can be calculated.
        /// </summary>
        /// <param name="iWattIndex">Index to the watt channel.</param>
        /// <param name="iVarIndex">Index to the var channel, -1 if not used.</param>
        /// <param name="iQIndex">Index to the Q channel, -1 if not used.</param>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if any of the indexes are out of range.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        public virtual void AddCalculatedVA(int iWattIndex, int iVarIndex, int iQIndex)
        {
            if (iWattIndex >= m_llpcChannels.Count ||
                iVarIndex >= m_llpcChannels.Count ||
                iQIndex >= m_llpcChannels.Count)
            {
                throw new ArgumentException("VA cannot be calculated with the given channel indexes.");
            }

            m_bCalculateVA = true;

            m_iWattIndex = iWattIndex;

            if (iVarIndex != -1)
            {
                m_iVarIndex = iVarIndex;
            }
            else
            {
                m_iQIndex = iQIndex;
            }

            //If this is not pulse data then go ahead and calculate the VA
            if (!(this is LoadProfilePulseData))
            {
                if (m_iWattIndex != -1 && (m_iVarIndex != -1 || m_iQIndex != -1))
                {
                    int iVAChannel = m_llpcChannels.Count;
                    CalculatedVAChannel NewChannel = new CalculatedVAChannel(iVAChannel, ref m_liIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);

                    if (this is LoadProfileDemandData)
                    {
                        NewChannel.ChangeNameToDemand();
                    }

                    m_llpcChannels.Add(NewChannel);

                    CalculateVAChannel(iVAChannel);
                }

                //We are done so mark it as false so we don't do it again.
                m_bCalculateVA = false;
            }
        }

        /// <summary>
        /// This method stores off information so that PF can be calculated.
        /// </summary>
        /// <param name="iWattIndex">Index to the watt channel.</param>
        /// <param name="iVAIndex">Index to the VA channel, -1 if not used.</param>
        /// <param name="iVarIndex">Index to the var channel, -1 if not used.</param>
        /// <param name="iQIndex">Index to the Q channel, -1 if not used.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if any of the indexes are out of range.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        public virtual void AddCalculatedPF(int iWattIndex, int iVAIndex, int iVarIndex, int iQIndex)
        {
            if (iWattIndex >= m_llpcChannels.Count ||
                iVAIndex >= m_llpcChannels.Count ||
                iVarIndex >= m_llpcChannels.Count ||
                iQIndex >= m_llpcChannels.Count)
            {
                throw new ArgumentException("PF cannot be calculated with the given channel indexes.");
            }

            m_bCalculatePF = true;

            m_iWattIndex = iWattIndex;

            if (iVAIndex != -1)
            {
                m_iVAIndex = iVAIndex;
            }
            else if (iVarIndex != -1)
            {
                m_iVarIndex = iVarIndex;
            }
            else
            {
                m_iQIndex = iQIndex;
            }

            //If this is not pulse data then go ahead and calculate the PF
            if (!(this is LoadProfilePulseData))
            {
                if (m_iWattIndex != -1 &&
                    (m_iVarIndex != -1 || m_iQIndex != -1 || m_iVAIndex != -1))
                {
                    int iPFChannel = m_llpcChannels.Count;
                    CalculatedPFChannel NewChannel = new CalculatedPFChannel(iPFChannel, ref m_liIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);
                    m_llpcChannels.Add(NewChannel);

                    CalculatePFChannel(iPFChannel);
                }

                //We are done so mark it as false so we don't do it again.
                m_bCalculatePF = false;
            }
        }

        /// <summary>
        /// Updates the given interval for the given channel with the supplied data
        /// </summary>
        /// <param name="Index">Interval Index to update</param>
        /// <param name="Channel">Channel data to update</param>
        /// <param name="NewData">New data</param>
        /// <param name="NewChanStatus">new Channel Status</param>
        /// <param name="NewIntervalStatus">New Interval Status</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 KRC 9.00.09          Created
        //  
        public void UpdateInterval(int Index, int Channel, double NewData, string NewChanStatus, string NewIntervalStatus)
        {
            m_liIntervals[Index].Data[Channel] = NewData;
            m_liIntervals[Index].ChannelStatuses[Channel] = NewChanStatus;
            m_liIntervals[Index].IntervalStatus = ConcatenateStatuses(m_liIntervals[Index].IntervalStatus, NewIntervalStatus);
        }

        /// <summary>
        /// Returns the Interval Index for the given time.
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 KRC 9.00.09          Created
        //  
        public int GetIntervalIndexAt(DateTime Time)
        {
            int Index = 0;
            bool bFound = false;

            while (false == bFound && Index < m_liIntervals.Count)
            {
                if (Time == m_liIntervals[Index].Time)
                {
                    bFound = true;
                }
                else
                {
                    Index++;
                }
            }

            return Index;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property to get the number of channels of load profile data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public int NumberOfChannels
        {
            get
            {
                return m_llpcChannels.Count;
            }
        }

        /// <summary>
        /// Property to get the number of intervals.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public int NumberIntervals
        {
            get
            {
                return m_liIntervals.Count;
            }
        }

        /// <summary>
        /// Property to get the start time of the load profile data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public DateTime StartTime
        {
            get
            {
                return m_liIntervals[0].Time;
            }
        }

        /// <summary>
        /// Property to get the end time of the load profile data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public DateTime EndTime
        {
            get
            {
                return m_liIntervals[m_liIntervals.Count - 1].Time;
            }
        }

        /// <summary>
        /// Returns a list of intervals in the Load Profile
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public List<LPInterval> Intervals
        {
            get
            {
                return m_liIntervals;
            }
        }

        /// <summary>
        /// Property to get the list of channels in the load profile object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public List<LPChannel> Channels
        {
            get
            {
                return m_llpcChannels;
            }
        }

        /// <summary>
        /// Property used to get or set the scale for the load profile data.  The set
        /// will adjust the data based on the new scale.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/27/07 mrj 9.00.00		Created
        //  
        public DisplayScaleOptions DisplayScale
        {
            get
            {
                //All of the intervals are the same scale so just get the first one
                return m_liIntervals[0].DisplayScale;
            }
            set
            {
                if (value != m_liIntervals[0].DisplayScale)
                {
                    //Loop through the intervals and set the display scale, this will also
                    //adjust the data.
                    foreach (LPInterval lpInterval in m_liIntervals)
                    {
                        lpInterval.DisplayScale = value;
                    }

                    //Loop through the channels and change their names
                    foreach (LPChannel lpChannel in m_llpcChannels)
                    {
                        lpChannel.ChangeScale(value);
                    }
                }
            }
        }

        /// <summary>
        /// Abstract method overridden in the derived classes.  Used to get Energy data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public abstract LoadProfileEnergyData EnergyData
        {
            get;
        }

        /// <summary>
        /// Abstract method overridden in the derived classes.  Used to get Demand data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public abstract LoadProfileDemandData DemandData
        {
            get;
        }

        /// <summary>
        /// Property to get the interval duration in minutes for each interval. 
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/05/07 mrj 9.00.00		Created
        //  
        public int IntervalDuration
        {
            get
            {
                return m_iIntervalDuration;
            }
        }

        /// <summary>
        /// Gets or sets the name of the data set
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  12/19/11 RCG 2.53.20		Created

        public string DataSetName
        {
            get
            {
                return m_DataSetName;
            }
            set
            {
                m_DataSetName = value;
            }
        }

        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// This method re-calculates the channels.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        internal void ReCalculateChannels()
        {
            for (int i = 0; i < m_llpcChannels.Count; i++)
            {
                if (m_llpcChannels[i] is CalculatedChannel)
                {
                    //Get the indexes since they may not be valid anymore
                    m_iWattIndex = ((CalculatedChannel)m_llpcChannels[i]).WattIndex;
                    m_iVarIndex = ((CalculatedChannel)m_llpcChannels[i]).VarIndex;
                    m_iVAIndex = ((CalculatedChannel)m_llpcChannels[i]).VAIndex;
                    m_iQIndex = ((CalculatedChannel)m_llpcChannels[i]).QIndex;
                }

                if (m_llpcChannels[i] is CalculatedVarChannel)
                {
                    CalculateVarChannel(i);
                }

                if (m_llpcChannels[i] is CalculatedVAChannel)
                {
                    CalculateVAChannel(i);
                }

                if (m_llpcChannels[i] is CalculatedPFChannel)
                {
                    CalculatePFChannel(i);
                }
            }
        }

        /// <summary>
        /// Adds an interval to the load profile object
        /// </summary>
        /// <param name="NewInterval">the new interval to be added</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        internal void AddInterval(LPInterval NewInterval)
        {
            NewInterval.Index = m_liIntervals.Count;
            m_liIntervals.Add(NewInterval);
        }

        #endregion Internal Methods

        #region Protected Methods

        /// <summary>
        /// Calculates var for the given Watts and VA.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dVAChan">VA value</param>
        /// <returns>The calculated var value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculateVar(double dWattChan, double dVAChan)
        {
            double dVar = 0.0;

            if (dWattChan >= dVAChan)
            {
                dVar = 0.0;
            }
            else
            {
                try
                {
                    //Sqrt(VA^2-Watt^2)			
                    dVar = (dVAChan * dVAChan) - (dWattChan * dWattChan);
                    dVar = Math.Sqrt(dVar);
                }
                catch
                {
                    dVar = 0.0;
                }
            }

            return dVar;
        }

        /// <summary>
        /// Calculates var for the given Watts and Q.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dQChan">Q value</param>
        /// <returns>The calculated var value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculateVarWithQ(double dWattChan, double dQChan)
        {
            double dVar = 0.0;

            //(2Q - Watt) / sqrt(3)
            dVar = (2 * dQChan) - dWattChan;
            dVar = dVar / (Math.Sqrt(3));

            return dVar;
        }

        /// <summary>
        /// Calculates VA for the given Watts and var.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dVarChan">Var value</param>
        /// <returns>The calculated VA value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculateVA(double dWattChan, double dVarChan)
        {
            double dVA = 0.0;

            if (0.0 != dWattChan ||
                0.0 != dVarChan)
            {
                //Sqrt(Watt^2 + Var^2)
                dVA = (dWattChan * dWattChan) + (dVarChan * dVarChan);
                dVA = Math.Sqrt(dVA);
            }

            return dVA;
        }

        /// <summary>
        /// Calculates VA for the given Watts and Q.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dQChan">Q value</param>
        /// <returns>The calculated VA value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculateVAWithQ(double dWattChan, double dQChan)
        {
            double dVA = 0.0;
            double dVarChan = 0.0;

            //Calculate Var
            dVarChan = CalculateVarWithQ(dWattChan, dQChan);

            //Calculate VA
            dVA = CalculateVA(dWattChan, dVarChan);

            return dVA;
        }

        /// <summary>
        /// Calculates PF for the given Watts and VA.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dVAChan">VA value</param>
        /// <returns>The calculated PF value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculatePF(double dWattChan, double dVAChan)
        {
            double dPF = 0.0;

            if (0.0 == dWattChan && 0.0 == dVAChan)
            {
                dPF = 0.0;
            }
            else if (0.0 != dWattChan && 0.0 == dVAChan)
            {
                dPF = 1.0;
            }
            else
            {
                //Watt/VA
                dPF = dWattChan / dVAChan;
            }

            //PF cannot be greater than 1.0.
            if (1.0 < dPF)
            {
                dPF = 1.0;
            }

            return dPF;
        }

        /// <summary>
        /// Calculates PF for the given Watts and var.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dVarChan">Var value</param>
        /// <returns>The calculated PF value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculatePFWithVar(double dWattChan, double dVarChan)
        {
            double dPF = 0.0;
            double dVAChan = 0.0;

            //Calculate VA
            dVAChan = CalculateVA(dWattChan, dVarChan);

            //Calculate PF with Watts and VA
            dPF = CalculatePF(dWattChan, dVAChan);

            return dPF;
        }

        /// <summary>
        /// Calculates PF for the given Watts and Q.
        /// </summary>
        /// <param name="dWattChan">Watt value</param>
        /// <param name="dQChan">Q value</param>
        /// <returns>The calculated PF value</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected double CalculatePFWithQ(double dWattChan, double dQChan)
        {
            double dPF = 0.0;
            double dVAChan = 0.0;

            //Calculated VA
            dVAChan = CalculateVAWithQ(dWattChan, dQChan);

            //Calculated PF with Watts and VA
            dPF = CalculatePF(dWattChan, dVAChan);

            return dPF;
        }

        /// <summary>
        /// Concatenates statuses
        /// </summary>
        /// <param name="str1Status">First status</param>
        /// <param name="str2Status">Second status</param>
        /// <returns>Concatenated statuses</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        protected string ConcatenateStatuses(string str1Status, string str2Status)
        {
            string strReturn = str1Status;

            if (strReturn == null)
            {
                strReturn = "";
            }

            if (str2Status != null)
            {
                for (int i = 0; i < str2Status.Length; i++)
                {
                    if (-1 == strReturn.IndexOf(str2Status[i]))
                    {
                        //The status was not found so add it
                        strReturn += str2Status[i];
                    }
                }
            }

            return strReturn;
        }

        /// <summary>
        /// This method returns a new object of the give load profile type.
        /// </summary>
        /// <param name="iIntervalDuration">Duration for the new load profile.</param>
        /// <returns>LoadProfileData object</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 mrj 9.00.00		Created
        //  
        protected abstract LoadProfileData GetNewLPData(int iIntervalDuration);

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Calculates var on the supplied channel.
        /// </summary>
        /// <param name="iVarChannelIndex">Index of the calculated var channel</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        private void CalculateVarChannel(int iVarChannelIndex)
        {
            double[] data;
            string[] statuses;

            //Loop through the intervals, calculate the channel, and set the new interval back
            for (int iIndex = 0; iIndex < m_liIntervals.Count; iIndex++)
            {
                if (iVarChannelIndex == m_liIntervals[iIndex].Data.Length)
                {
                    //This is a new channel
                    data = new double[m_liIntervals[iIndex].Data.Length + 1];
                    statuses = new string[m_liIntervals[iIndex].Data.Length + 1];
                }
                else
                {
                    //We are recalculating var
                    data = new double[m_liIntervals[iIndex].Data.Length];
                    statuses = new string[m_liIntervals[iIndex].Data.Length];
                }

                //Loop through the channels and save off the original data
                for (int iChannel = 0; iChannel < m_liIntervals[iIndex].Data.Length; iChannel++)
                {
                    data[iChannel] = m_liIntervals[iIndex].Data[iChannel];
                    statuses[iChannel] = m_liIntervals[iIndex].ChannelStatuses[iChannel];
                }

                //Calculate Vars
                if (m_iVAIndex != -1)
                {
                    //Calculate using Watts and VA
                    data[iVarChannelIndex] = CalculateVar(data[m_iWattIndex], data[m_iVAIndex]);
                    statuses[iVarChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iVAIndex]);
                }
                else
                {
                    //Calculate using Watts and Q
                    data[iVarChannelIndex] = CalculateVarWithQ(data[m_iWattIndex], data[m_iQIndex]);
                    statuses[iVarChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iQIndex]);
                }

                //Change this interval to the new interval with the calculated channel
                LPInterval NewLPInterval = new LPInterval(m_liIntervals[iIndex].Index, data, statuses, m_liIntervals[iIndex].IntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                m_liIntervals[iIndex] = NewLPInterval;
            }
        }

        /// <summary>
        /// Calculates VA on the supplied channel.
        /// </summary>
        /// <param name="iVAChannelIndex">Index of the calculated var channel</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        private void CalculateVAChannel(int iVAChannelIndex)
        {
            double[] data;
            string[] statuses;

            //Loop through the intervals, calculate the channel, and set the new interval back
            for (int iIndex = 0; iIndex < m_liIntervals.Count; iIndex++)
            {
                if (iVAChannelIndex == m_liIntervals[iIndex].Data.Length)
                {
                    //This is a new channel
                    data = new double[m_liIntervals[iIndex].Data.Length + 1];
                    statuses = new string[m_liIntervals[iIndex].Data.Length + 1];
                }
                else
                {
                    //We are recalculating VA
                    data = new double[m_liIntervals[iIndex].Data.Length];
                    statuses = new string[m_liIntervals[iIndex].Data.Length];
                }

                //Loop through the channels and save off the original data
                for (int iChannel = 0; iChannel < m_liIntervals[iIndex].Data.Length; iChannel++)
                {
                    data[iChannel] = m_liIntervals[iIndex].Data[iChannel];
                    statuses[iChannel] = m_liIntervals[iIndex].ChannelStatuses[iChannel];
                }

                //Calculate VA
                if (m_iVarIndex != -1)
                {
                    //Calculate using Watts and Var
                    data[iVAChannelIndex] = CalculateVA(data[m_iWattIndex], data[m_iVarIndex]);
                    statuses[iVAChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iVarIndex]);
                }
                else
                {
                    //Calculate using Watts and Q
                    data[iVAChannelIndex] = CalculateVAWithQ(data[m_iWattIndex], data[m_iQIndex]);
                    statuses[iVAChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iQIndex]);
                }

                //Change this interval to the new interval with the calculated channel
                LPInterval NewLPInterval = new LPInterval(m_liIntervals[iIndex].Index, data, statuses, m_liIntervals[iIndex].IntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                m_liIntervals[iIndex] = NewLPInterval;
            }
        }

        /// <summary>
        /// Calculates PF on the supplied channel.
        /// </summary>
        /// <param name="iPFChannelIndex">Index of the calculated var channel</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        private void CalculatePFChannel(int iPFChannelIndex)
        {
            double[] data;
            string[] statuses;

            //Loop through the intervals, calculate the channel, and set the new interval back
            for (int iIndex = 0; iIndex < m_liIntervals.Count; iIndex++)
            {
                if (iPFChannelIndex == m_liIntervals[iIndex].Data.Length)
                {
                    //This is a new channel
                    data = new double[m_liIntervals[iIndex].Data.Length + 1];
                    statuses = new string[m_liIntervals[iIndex].Data.Length + 1];
                }
                else
                {
                    //We are recalculating PF
                    data = new double[m_liIntervals[iIndex].Data.Length];
                    statuses = new string[m_liIntervals[iIndex].Data.Length];
                }

                //Loop through the channels and save off the original data
                for (int iChannel = 0; iChannel < m_liIntervals[iIndex].Data.Length; iChannel++)
                {
                    data[iChannel] = m_liIntervals[iIndex].Data[iChannel];
                    statuses[iChannel] = m_liIntervals[iIndex].ChannelStatuses[iChannel];
                }

                //Calculate PF
                if (m_iVAIndex != -1)
                {
                    //Calculate using Watts and VA
                    data[iPFChannelIndex] = CalculatePF(data[m_iWattIndex], data[m_iVAIndex]);
                    statuses[iPFChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iVAIndex]);
                }
                else if (m_iVarIndex != -1)
                {
                    //Calculate using Watts and Var
                    data[iPFChannelIndex] = CalculatePFWithVar(data[m_iWattIndex], data[m_iVarIndex]);
                    statuses[iPFChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iVarIndex]);
                }
                else
                {
                    //Calculate using Watts and Q
                    data[iPFChannelIndex] = CalculatePFWithQ(data[m_iWattIndex], data[m_iQIndex]);
                    statuses[iPFChannelIndex] = ConcatenateStatuses(m_liIntervals[iIndex].ChannelStatuses[m_iWattIndex], m_liIntervals[iIndex].ChannelStatuses[m_iQIndex]);
                }

                //Change this interval to the new interval with the calculated channel
                LPInterval NewLPInterval = new LPInterval(m_liIntervals[iIndex].Index, data, statuses, m_liIntervals[iIndex].IntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                m_liIntervals[iIndex] = NewLPInterval;
            }
        }

        #endregion Private Methods

        #region Members

        /// <summary>
        /// List containing all of the intervals in the set of load profile data
        /// </summary>		
        protected List<LPInterval> m_liIntervals;

        /// <summary>
        /// List containing all of the channels in the load profile data
        /// </summary>
        protected List<LPChannel> m_llpcChannels;

        /// <summary>
        /// Duration of each interval in minutes
        /// </summary>
        protected int m_iIntervalDuration;

        /// <summary>
        /// Index to watts that will be used in calculated channels
        /// </summary>
        protected int m_iWattIndex;
        /// <summary>
        /// Index to vars that will be used in calculated channels
        /// </summary>
        protected int m_iVarIndex;
        /// <summary>
        /// Index to VA that will be used in calculated channels
        /// </summary>
        protected int m_iVAIndex;
        /// <summary>
        /// Index to Q that will be used in calculated channels
        /// </summary>
        protected int m_iQIndex;
        /// <summary>
        /// Calculate var channel
        /// </summary>
        protected bool m_bCalculateVar;
        /// <summary>
        /// Calculate VA channel
        /// </summary>
        protected bool m_bCalculateVA;
        /// <summary>
        /// Calculate PF channel
        /// </summary>
        protected bool m_bCalculatePF;
        /// <summary>
        /// The name of the Data Set
        /// </summary>
        protected string m_DataSetName;

        #endregion
    }

    /// <summary>
    /// This class represent load profile data as pulse data.
    /// </summary>
    public class LoadProfilePulseData : LoadProfileData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iDuration"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public LoadProfilePulseData(int iDuration)
            : base(iDuration)
        {
        }

        /// <summary>
        /// This method is used to get rolling demand data from pulse data.
        /// </summary>
        /// <param name="iNumIntervalsToCombine">
        /// Number of intervals to combine when doing rolling demand.
        /// </param>
        /// <returns>
        /// Load profile demand data rolled based on the supplied demand length.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if the demand length is larger than an hour or
        /// it is not divible evenly in an hour.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/29/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData CalculateRollingDemand(int iNumIntervalsToCombine)
        {
            LoadProfileDemandData LPRollingDemand = new LoadProfileDemandData(m_iIntervalDuration);
            int iIndex = 0;
            int iDemandIntervalLength = m_iIntervalDuration * iNumIntervalsToCombine;


            if (iDemandIntervalLength > 60 ||
                (60 % iDemandIntervalLength) != 0 ||
                m_liIntervals.Count == 0)
            {
                //We cannot do rolling demand since the demand interval length is not valid.
                throw new ArgumentException("Unable to calculate rolling demand.  The demand " +
                                            "interval length is not valid.");
            }

            //Add the channels to the new load profile
            for (int i = 0; i < m_llpcChannels.Count; i++)
            {
                LPRollingDemand.AddChannel(m_llpcChannels[i]);
            }

            //Get the start of load profile.  The first interval is the same.  The second interval is the
            //first and second interval.  The third interval is the first three intervals.  This goes on
            //until the number of intervals being used to compute demand matches the number of requested
            //intervals to use for demand.
            for (int iNumberIntervals = 1; iNumberIntervals <= iNumIntervalsToCombine && iIndex < m_liIntervals.Count; iNumberIntervals++)
            {
                double[] data = new double[m_liIntervals[0].Data.Length];
                string[] statuses = new string[m_liIntervals[0].Data.Length];
                string strIntervalStatus = "";

                for (int i = 0; i < iNumberIntervals && (iIndex - i) >= 0; i++)
                {
                    //Add up the interval data
                    for (int iChannel = 0; iChannel < m_liIntervals[iIndex - i].Data.Length; iChannel++)
                    {
                        data[iChannel] += m_liIntervals[iIndex - i].Data[iChannel];
                        statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex - i].ChannelStatuses[iChannel]);
                    }
                    strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex - i].IntervalStatus);
                }

                //Calculate demand
                for (int iChannel = 0; iChannel < data.Length; iChannel++)
                {
                    //Demand = (Pulse * Pulse Weight * Multiplier) / Interval length in hours
                    data[iChannel] = (double)((data[iChannel] * m_llpcChannels[iChannel].PulseWeight * m_llpcChannels[iChannel].Multiplier) / ((m_iIntervalDuration * iNumberIntervals) / 60.0));
                }

                //Add the new interval
                LPRollingDemand.AddInterval(data, statuses, strIntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                iIndex++;
            }

            //Now compute rollind demand on the rest of the data
            while (iIndex < m_liIntervals.Count)
            {
                double[] data = new double[m_liIntervals[0].Data.Length];
                string[] statuses = new string[m_liIntervals[0].Data.Length];
                string strIntervalStatus = "";

                for (int i = 0; i < iNumIntervalsToCombine; i++)
                {
                    //Add up the interval data
                    for (int iChannel = 0; iChannel < m_liIntervals[iIndex - i].Data.Length; iChannel++)
                    {
                        data[iChannel] += m_liIntervals[iIndex - i].Data[iChannel];
                        statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex - i].ChannelStatuses[iChannel]);
                    }
                    strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex - i].IntervalStatus);
                }

                //Calculate demand
                for (int iChannel = 0; iChannel < data.Length; iChannel++)
                {
                    //Demand = (Pulse * Pulse Weight * Multiplier) / Interval length in hours
                    data[iChannel] = (double)((data[iChannel] * m_llpcChannels[iChannel].PulseWeight * m_llpcChannels[iChannel].Multiplier) / ((m_iIntervalDuration * iNumIntervalsToCombine) / 60.0));
                }

                //Add the new interval
                LPRollingDemand.AddInterval(data, statuses, strIntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                iIndex++;
            }

            //Calculate var if it was requested since it is not added to pulse data
            if (m_bCalculateVar)
            {
                LPRollingDemand.AddCalculatedVar(m_iWattIndex, m_iVAIndex, m_iQIndex);
            }

            //Calculate VA if it was requested since it is not added to pulse data
            if (m_bCalculateVA)
            {
                LPRollingDemand.AddCalculatedVA(m_iWattIndex, m_iVarIndex, m_iQIndex);
            }

            //Calculate PF if it was requested since it is not added to pulse data
            if (m_bCalculatePF)
            {
                LPRollingDemand.AddCalculatedPF(m_iWattIndex, m_iVAIndex, m_iVarIndex, m_iQIndex);
            }

            return LPRollingDemand;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property to get energy data from this pulse data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileEnergyData EnergyData
        {
            get
            {
                LoadProfileEnergyData NewEnergyData = new LoadProfileEnergyData(m_iIntervalDuration);

                //Add the channel info to the new load profile
                for (int iChannel = 0; iChannel < m_llpcChannels.Count; iChannel++)
                {
                    NewEnergyData.AddChannel(m_llpcChannels[iChannel]);
                }

                //Loop through the intervals and convert to energy
                foreach (LPInterval lpInterval in m_liIntervals)
                {
                    double[] data = new double[lpInterval.Data.Length];
                    string[] statuses = new string[lpInterval.Data.Length];

                    for (int iChannel = 0; iChannel < lpInterval.Data.Length; iChannel++)
                    {
                        //Energy = Pulse * Pulse Weight * Multiplier
                        data[iChannel] = (lpInterval.Data[iChannel] * m_llpcChannels[iChannel].PulseWeight * m_llpcChannels[iChannel].Multiplier);
                        statuses[iChannel] = lpInterval.ChannelStatuses[iChannel];
                    }

                    //Add the new energy interval					
                    NewEnergyData.AddInterval(data, statuses, lpInterval.IntervalStatus, lpInterval.Time, lpInterval.DisplayScale);
                }

                //Calculate var if it was requested since it is not added to pulse data
                if (m_bCalculateVar)
                {
                    NewEnergyData.AddCalculatedVar(m_iWattIndex, m_iVAIndex, m_iQIndex);
                }

                //Calculate VA if it was requested since it is not added to pulse data
                if (m_bCalculateVA)
                {
                    NewEnergyData.AddCalculatedVA(m_iWattIndex, m_iVarIndex, m_iQIndex);
                }

                //Calculate PF if it was requested since it is not added to pulse data
                if (m_bCalculatePF)
                {
                    NewEnergyData.AddCalculatedPF(m_iWattIndex, m_iVAIndex, m_iVarIndex, m_iQIndex);
                }

                return NewEnergyData;
            }
        }

        /// <summary>
        /// Property to get demand data from this pulse data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData DemandData
        {
            get
            {
                LoadProfileDemandData NewDemandData = new LoadProfileDemandData(m_iIntervalDuration);

                //Add the channel info to the new load profile
                for (int iChannel = 0; iChannel < m_llpcChannels.Count; iChannel++)
                {
                    NewDemandData.AddChannel(m_llpcChannels[iChannel]);
                }

                //Loop through the intervals and convert to demand
                foreach (LPInterval lpInterval in m_liIntervals)
                {
                    double[] data = new double[lpInterval.Data.Length];
                    string[] statuses = new string[lpInterval.Data.Length];

                    for (int iChannel = 0; iChannel < lpInterval.Data.Length; iChannel++)
                    {
                        //Demand = (Pulse * Pulse Weight * Multiplier) / Interval length in hours
                        data[iChannel] = (double)((lpInterval.Data[iChannel] * m_llpcChannels[iChannel].PulseWeight * m_llpcChannels[iChannel].Multiplier) / (m_iIntervalDuration / 60.0));
                        statuses[iChannel] = lpInterval.ChannelStatuses[iChannel];
                    }

                    //Add the new demand interval					
                    NewDemandData.AddInterval(data, statuses, lpInterval.IntervalStatus, lpInterval.Time, lpInterval.DisplayScale);
                }

                //Calculate var if it was requested since it is not added to pulse data
                if (m_bCalculateVar)
                {
                    NewDemandData.AddCalculatedVar(m_iWattIndex, m_iVAIndex, m_iQIndex);
                }

                //Calculate VA if it was requested since it is not added to pulse data
                if (m_bCalculateVA)
                {
                    NewDemandData.AddCalculatedVA(m_iWattIndex, m_iVarIndex, m_iQIndex);
                }

                //Calculate PF if it was requested since it is not added to pulse data
                if (m_bCalculatePF)
                {
                    NewDemandData.AddCalculatedPF(m_iWattIndex, m_iVAIndex, m_iVarIndex, m_iQIndex);
                }


                return NewDemandData;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// This method returns a new object of the give load profile type.
        /// </summary>
        /// <param name="iIntervalDuration">Duration for the new load profile.</param>
        /// <returns>LoadProfileData object</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 mrj 9.00.00		Created
        //  
        protected override LoadProfileData GetNewLPData(int iIntervalDuration)
        {
            LoadProfileData lpData = new LoadProfilePulseData(iIntervalDuration);
            return lpData;
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// This class represent load profile data as energy data.
    /// </summary>
    public class LoadProfileEnergyData : LoadProfileData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iDuration"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public LoadProfileEnergyData(int iDuration)
            : base(iDuration)
        {
        }

        /// <summary>
        /// This method changes the multiplier for the given energy channel.
        /// </summary>
        /// <param name="iChannel">
        /// The channel to change the multiplier of.
        /// </param>
        /// <param name="fMultiplier">
        /// The new multiplier for the supplied channel.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        /// <remarks>
        /// This really will only be used when the data file has the data in Energy
        /// and a multiplier needs to be applied to get the data to primary.
        /// </remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/27/07 mrj 9.00.00		Created
        //  
        public override void ChangeChannelMultiplier(int iChannel, float fMultiplier)
        {
            //Loop through the intervals, remove the old multiplier and apply 
            //the new one.
            foreach (LPInterval lpInterval in m_liIntervals)
            {
                lpInterval.Data[iChannel] = (lpInterval.Data[iChannel] / m_llpcChannels[iChannel].Multiplier) * fMultiplier;
            }

            //Save off the new multiplier
            m_llpcChannels[iChannel].Multiplier = fMultiplier;
        }

        /// <summary>
        /// Method used to get rolling demand data from energy data.  Energy data could
        /// have calculated channels so the new rolling demand is generated and then the
        /// calculated channels will be calculated for this new demand.
        /// </summary>
        /// <param name="iNumIntervalsToCombine">
        /// Number of intervals to combine when doing rolling demand.
        /// </param>
        /// <returns>
        /// Load profile demand data rolled based on the supplied demand length.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if the demand length is larger than an hour or
        /// it is not divible evenly in an hour.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/29/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData CalculateRollingDemand(int iNumIntervalsToCombine)
        {
            LoadProfileDemandData LPRollingDemand = new LoadProfileDemandData(m_iIntervalDuration);
            int iIndex = 0;
            int iDemandIntervalLength = m_iIntervalDuration * iNumIntervalsToCombine;


            if (iDemandIntervalLength > 60 ||
                (60 % iDemandIntervalLength) != 0 ||
                m_liIntervals.Count == 0)
            {
                //We cannot do rolling demand since the demand interval length is not valid.
                throw new ArgumentException("Unable to calculate rolling demand.  The demand " +
                                            "interval length is not valid.");
            }

            //Add the channels to the new load profile
            for (int i = 0; i < m_llpcChannels.Count; i++)
            {
                LPRollingDemand.AddChannel(m_llpcChannels[i]);
            }

            //Get the start of load profile.  The first interval is the same.  The second interval is the
            //first and second interval.  The third interval is the first three intervals.  This goes on
            //until the number of intervals being used to compute demand matches the number of requested
            //intervals to use for demand.
            for (int iNumberIntervals = 1; iNumberIntervals <= iNumIntervalsToCombine && iIndex < m_liIntervals.Count; iNumberIntervals++)
            {
                double[] data = new double[m_liIntervals[0].Data.Length];
                string[] statuses = new string[m_liIntervals[0].Data.Length];
                string strIntervalStatus = "";

                for (int i = 0; i < iNumberIntervals && (iIndex - i) >= 0; i++)
                {
                    //Add up the interval data
                    for (int iChannel = 0; iChannel < m_liIntervals[iIndex - i].Data.Length; iChannel++)
                    {
                        data[iChannel] += m_liIntervals[iIndex - i].Data[iChannel];
                        statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex - i].ChannelStatuses[iChannel]);
                    }
                    strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex - i].IntervalStatus);
                }

                //Calculate demand
                for (int iChannel = 0; iChannel < data.Length; iChannel++)
                {
                    //Demand = Energy / Interval length in hours
                    data[iChannel] = (double)((data[iChannel]) / ((m_iIntervalDuration * iNumberIntervals) / 60.0));
                }

                //Add the new interval
                LPRollingDemand.AddInterval(data, statuses, strIntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                iIndex++;
            }

            //Now compute rollind demand on the rest of the data
            while (iIndex < m_liIntervals.Count)
            {
                double[] data = new double[m_liIntervals[0].Data.Length];
                string[] statuses = new string[m_liIntervals[0].Data.Length];
                string strIntervalStatus = "";

                for (int i = 0; i < iNumIntervalsToCombine; i++)
                {
                    //Add up the interval data
                    for (int iChannel = 0; iChannel < m_liIntervals[iIndex - i].Data.Length; iChannel++)
                    {
                        data[iChannel] += m_liIntervals[iIndex - i].Data[iChannel];
                        statuses[iChannel] = ConcatenateStatuses(statuses[iChannel], m_liIntervals[iIndex - i].ChannelStatuses[iChannel]);
                    }
                    strIntervalStatus = ConcatenateStatuses(strIntervalStatus, m_liIntervals[iIndex - i].IntervalStatus);
                }

                //Calculate demand
                for (int iChannel = 0; iChannel < data.Length; iChannel++)
                {
                    //Demand = Eneryg / Interval length in hours
                    data[iChannel] = (double)((data[iChannel]) / ((m_iIntervalDuration * iNumIntervalsToCombine) / 60.0));
                }

                //Add the new interval
                LPRollingDemand.AddInterval(data, statuses, strIntervalStatus, m_liIntervals[iIndex].Time, m_liIntervals[0].DisplayScale);
                iIndex++;
            }


            //Re-calculate channels with the new demand data
            LPRollingDemand.ReCalculateChannels();

            return LPRollingDemand;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property to get energy data
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileEnergyData EnergyData
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Property to get demand data from this energy data
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData DemandData
        {
            get
            {
                LoadProfileDemandData NewDemandData = new LoadProfileDemandData(m_iIntervalDuration);

                //Add the channel info to the new load profile
                for (int iChannel = 0; iChannel < m_llpcChannels.Count; iChannel++)
                {
                    NewDemandData.AddChannel(m_llpcChannels[iChannel]);
                }

                //Loop through the intervals and convert to demand
                foreach (LPInterval lpInterval in m_liIntervals)
                {
                    double[] data = new double[lpInterval.Data.Length];

                    for (int iChannel = 0; iChannel < lpInterval.Data.Length; iChannel++)
                    {
                        if (m_llpcChannels[iChannel] is CalculatedPFChannel)
                        {
                            //Do not convert PF channels to demand
                            data[iChannel] = lpInterval.Data[iChannel];
                        }
                        else
                        {
                            //Demand = Energy / Interval length in hours
                            data[iChannel] = (double)((lpInterval.Data[iChannel]) / (m_iIntervalDuration / 60.0));
                        }
                    }

                    //Add the new demand interval										
                    NewDemandData.AddInterval(data, lpInterval.ChannelStatuses, lpInterval.IntervalStatus, lpInterval.Time, lpInterval.DisplayScale);
                }

                return NewDemandData;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// This method returns a new object of the give load profile type.
        /// </summary>
        /// <param name="iIntervalDuration">Duration for the new load profile.</param>
        /// <returns>LoadProfileData object</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 mrj 9.00.00		Created
        //  
        protected override LoadProfileData GetNewLPData(int iIntervalDuration)
        {
            LoadProfileData lpData = new LoadProfileEnergyData(iIntervalDuration);
            return lpData;
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// This class represent load profile data as demand data.
    /// </summary>
    public class LoadProfileDemandData : LoadProfileData
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iDuration"></param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public LoadProfileDemandData(int iDuration)
            : base(iDuration)
        {
        }

        /// <summary>
        /// This method changes the multiplier for the given demand channel.
        /// </summary>
        /// <param name="iChannel">
        /// The channel to change the multiplier of.
        /// </param>
        /// <param name="fMultiplier">
        /// The new mutliplier for the supplied channel.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if a channel is requested that is not valid.
        /// </exception>
        /// <remarks>
        /// This really will only be used when the data file has the data in Energy
        /// and a multiplier needs to be applied to get the data to primary.
        /// </remarks>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/27/07 mrj 9.00.00		Created
        //  
        public override void ChangeChannelMultiplier(int iChannel, float fMultiplier)
        {
            //Loop through the intervals, remove the old multiplier and apply 
            //the new one.
            foreach (LPInterval lpInterval in m_liIntervals)
            {
                lpInterval.Data[iChannel] = (lpInterval.Data[iChannel] / m_llpcChannels[iChannel].Multiplier) * fMultiplier;
            }

            //Save off the new multiplier
            m_llpcChannels[iChannel].Multiplier = fMultiplier;
        }

        /// <summary>
        /// This method calculates rolling demand.  It is currently not supported by the 
        /// load profile demand data object
        /// </summary>
        /// <param name="iNumIntervalsToCombine">Number of intervals to combine</param>
        /// <returns>Load profile rolling demand data</returns>
        /// <exception cref="NotImplementedException">
        /// This method is currently not supported since we would have to convert back
        /// to energy and then calculate rolling demand.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/29/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData CalculateRollingDemand(int iNumIntervalsToCombine)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a channel to the demand load profile object and changes the name to
        /// a demand quantity.
        /// </summary>
        /// <param name="Channel">New channel to add.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/07 mrj 9.00.00		Created
        //  
        public override void AddChannel(LPChannel Channel)
        {
            LPChannel NewChannel = Channel.CreateChannel(Channel.ChannelName, m_llpcChannels.Count, Channel.PulseWeight, Channel.Multiplier, ref m_liIntervals);
            NewChannel.ChangeNameToDemand();
            m_llpcChannels.Add(NewChannel);
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property to get energy data
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// This is thrown because we currently don't allow converting demand data back
        /// to energy.
        /// </exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileEnergyData EnergyData
        {
            get
            {
                throw new NotImplementedException("Unable to convert demand data to energy.");
            }
        }

        /// <summary>
        /// Property to get demand data
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override LoadProfileDemandData DemandData
        {
            get
            {
                return this;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// This method returns a new object of the give load profile type.
        /// </summary>
        /// <param name="iIntervalDuration">Duration for the new load profile.</param>
        /// <returns>LoadProfileData object</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/13/07 mrj 9.00.00		Created
        //  
        protected override LoadProfileData GetNewLPData(int iIntervalDuration)
        {
            LoadProfileData lpData = new LoadProfileDemandData(iIntervalDuration);
            return lpData;
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// This class represent an interval of load profile data.
    /// </summary>
    public class LPInterval
    {
        #region Public Methods

        /// <summary>
        /// Constructor to instantiate a LoadProfileInterval object
        /// </summary>
        /// <param name="index">Index of the load profile interval</param>
        /// <param name="data">Data associated with this interval</param>					 
        /// <param name="channelStatus">Channel statuses for this interval</param>
        /// <param name="intervalStatus">Interval status for thie interval</param>
        /// <param name="dtEndTime">Time for this interval</param>
        /// <param name="Scale">The current scale of the data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	07/25/07 mrj 9.00.00		Changed pulse to be an array of doubles
        //  
        internal LPInterval(int index,
                          double[] data,
                          string[] channelStatus,
                          string intervalStatus,
                          DateTime dtEndTime,
                          DisplayScaleOptions Scale)
        {
            m_iIndex = index;
            m_ChannelData = data;
            m_astrChannelStatuses = channelStatus;
            m_strIntervalStatus = intervalStatus;
            m_dtEndTime = dtEndTime;
            m_DisplayScale = Scale;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Propert to get the data for this interval.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public double[] Data
        {
            get
            {
                return m_ChannelData;
            }
        }

        /// <summary>
        /// Property to get an array of ints that represents the status of each
        /// channel.  Status[0] gives the status of channel 1 and so on.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public string[] ChannelStatuses
        {
            get
            {
                return m_astrChannelStatuses;
            }
        }

        /// <summary>
        /// Property to get an array of ints that represents the status of each
        /// channel.  Status[0] gives the status of channel 1 and so on.  The "D"
        /// status is filtered out of this property.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	11/13/07 mrj 9.00.26 3415	Do not show the "D" status.
        //  
        public string[] ChannelStatusesFiltered
        {
            get
            {
                string[] strChannelStatuesFiltered = new string[m_astrChannelStatuses.Length];

                for (int i = 0; i < m_astrChannelStatuses.Length; i++)
                {
                    string strStatus = m_astrChannelStatuses[i];

                    if (strStatus != null)
                    {
                        int iIndex = strStatus.IndexOf('D');
                        if (iIndex != -1)
                        {
                            strStatus = strStatus.Remove(iIndex, 1);
                        }

                        strChannelStatuesFiltered[i] = strStatus;
                    }
                }

                return strChannelStatuesFiltered;
            }
        }

        /// <summary>
        /// Property to get the interval statuses.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public string IntervalStatus
        {
            get
            {
                return m_strIntervalStatus;
            }
            set
            {
                m_strIntervalStatus = value;
            }
        }

        /// <summary>
        /// Property to get the interval statuses.  This "D" status is filtered out
        /// of this property.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //	11/13/07 mrj 9.00.26 3415	Do not show the "D" status.
        //  
        public string IntervalStatusFiltered
        {
            get
            {
                string strStatus = m_strIntervalStatus;

                int iIndex = strStatus.IndexOf('D');
                if (iIndex != -1)
                {
                    strStatus = strStatus.Remove(iIndex, 1);
                }

                return strStatus;
            }
        }

        /// <summary>
        /// Property to get the time that the interval ended as a DateTime object.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public DateTime Time
        {
            get
            {
                return m_dtEndTime;
            }
        }

        /// <summary>
        /// Property to get whether or not the interval has statuses.  True if the 
        /// interval has a status or false if it does not.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public bool HasIntervalStatus
        {
            get
            {
                if (m_strIntervalStatus != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Property to get an array of bools indicating whether or not a channel has
        /// a status.  The array has one entry for each channel.  If the channel has
        /// a status then the entry is true, otherwise it is false.  Channel 1 
        /// corresponds to index 0 in the array and so on.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        public bool[] HasChannelStatuses
        {
            get
            {
                bool[] blnHasStatus = new bool[m_astrChannelStatuses.GetLength(0)];
                for (int i = 0; i < m_astrChannelStatuses.GetLength(0); i++)
                {
                    if (m_astrChannelStatuses[i] != "")
                    {
                        blnHasStatus[i] = true;
                    }
                    else
                    {
                        blnHasStatus[i] = false;
                    }
                }

                return blnHasStatus;
            }
        }

        /// <summary>
        /// Property to get whether or not the interval has statuses.  True if the 
        /// interval has a status or false if it does not.  This method does not
        /// count the "D" status.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/13/07 mrj 9.00.26 3415	Created
        //  
        public bool HasFilteredIntervalStatus
        {
            get
            {
                if (m_strIntervalStatus != "")
                {
                    if (m_strIntervalStatus.Length == 1 &&
                        m_strIntervalStatus[0] == 'D')
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Property to get an array of bools indicating whether or not a channel has
        /// a status.  The array has one entry for each channel.  If the channel has
        /// a status then the entry is true, otherwise it is false.  Channel 1 
        /// corresponds to index 0 in the array and so on.  This method does not 
        /// count the "D" status.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  11/13/07 mrj 9.00.26 3415	Created
        //  
        public bool[] HasFilteredChannelStatuses
        {
            get
            {
                bool[] blnHasStatus = new bool[m_astrChannelStatuses.GetLength(0)];
                for (int i = 0; i < m_astrChannelStatuses.GetLength(0); i++)
                {
                    if (m_astrChannelStatuses[i] != "" && m_astrChannelStatuses[i] != null)
                    {
                        if (m_astrChannelStatuses[i].Length == 1 &&
                            m_astrChannelStatuses[i][0] == 'D')
                        {
                            blnHasStatus[i] = false;
                        }
                        else
                        {
                            blnHasStatus[i] = true;
                        }
                    }
                    else
                    {
                        blnHasStatus[i] = false;
                    }
                }

                return blnHasStatus;
            }
        }

        /// <summary>
        /// Property for the display scale.  During the set, the interval data
        /// will be adjusted based on the scale provided.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/31/07 mrj 9.00.00		Created
        //  
        public DisplayScaleOptions DisplayScale
        {
            get
            {
                return m_DisplayScale;
            }
            set
            {
                //Convert the data based on the new scale
                switch (m_DisplayScale)
                {
                    case DisplayScaleOptions.KILO:
                    {
                        if (value == DisplayScaleOptions.UNITS)
                        {
                            //Going from kilo to units so multiply by 1000
                            for (int iIndex = 0; iIndex < Data.Length; iIndex++)
                            {
                                Data[iIndex] = Data[iIndex] * 1000.0;
                            }
                        }
                        else if (value == DisplayScaleOptions.MEGA)
                        {
                            //Going from kilo to mega so divide by 1000
                            for (int iIndex = 0; iIndex < Data.Length; iIndex++)
                            {
                                Data[iIndex] = Data[iIndex] / 1000.0;
                            }
                        }
                        break;
                    }
                    case DisplayScaleOptions.MEGA:
                    {
                        double dMultiplier = 1.0;
                        if (value == DisplayScaleOptions.KILO)
                        {
                            //Going from mega to kilo so multiply by 1000
                            dMultiplier = 1000.0;
                        }
                        else if (value == DisplayScaleOptions.UNITS)
                        {
                            //Going from mega to units so multiply by 1000000
                            dMultiplier = 1000000.0;
                        }

                        for (int iIndex = 0; iIndex < Data.Length; iIndex++)
                        {
                            Data[iIndex] = Data[iIndex] * dMultiplier;
                        }
                        break;
                    }
                    case DisplayScaleOptions.UNITS:
                    default:
                    {
                        double dDivisor = 1.0;
                        if (value == DisplayScaleOptions.KILO)
                        {
                            //Going from units to kilo so divide by 1000
                            dDivisor = 1000.0;
                        }
                        else if (value == DisplayScaleOptions.MEGA)
                        {
                            //Going from units to mega so divide by 1000000
                            dDivisor = 1000000.0;
                        }

                        for (int iIndex = 0; iIndex < Data.Length; iIndex++)
                        {
                            Data[iIndex] = Data[iIndex] / dDivisor;
                        }
                        break;
                    }
                }

                //Set the new scale
                m_DisplayScale = value;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Property for the interval index used to keep the intervals in order in the 
        /// sorted list.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/25/07 mrj 9.00.00		Created
        //  
        internal int Index
        {
            get
            {
                return m_iIndex;
            }
            set
            {
                m_iIndex = value;
            }
        }

        #endregion Internal Properties

        #region Members

        /// <summary>
        /// Index of the interval.  Used to sort the sorted list of intervals
        /// and to make sure the intervals can never get out of time order.
        /// </summary>
        protected int m_iIndex;

        /// <summary>
        /// Data for each channel
        /// </summary>        
        protected double[] m_ChannelData;

        /// <summary>
        /// Array of strings that holds the status of each channel.
        /// </summary>
        protected string[] m_astrChannelStatuses;

        /// <summary>
        /// Integer value that represents the status of the interval.
        /// </summary>
        protected string m_strIntervalStatus;

        /// <summary>
        /// Holds the interval's timestamp as a DateTime object.  The timestamp
        /// represents the time the interval ended.
        /// </summary>
        protected DateTime m_dtEndTime;

        /// <summary>
        /// The scale that the data is in.
        /// </summary>
        protected DisplayScaleOptions m_DisplayScale;

        #endregion Members

    }

    /// <summary>
    /// This class represent single channel of Load Profile.
    /// </summary>
    public class LPChannel
    {
        #region Constants

        //Energy
        private const string A2H = "A^2h";
        private const string AH_A = "Ah(a)";
        private const string AH_B = "Ah(b)";
        private const string AH_C = "Ah(c)";
        private const string NAH = "NAh";
        private const string QH = "Qh";
        private const string QH_R = "Qh r";
        private const string V2H = "V^2h";
        private const string VAH = "VAh";
        private const string VAH_CALCULATED = "VAh (Calculated)";
        private const string VAH_ARITH_D = "VAh Arith d";
        private const string VAH_ARITH_R = "VAh Arith r";
        private const string VAH_LAG = "VAh Lag";
        private const string VAH_VEC_D = "VAh Vec d";
        private const string VAH_VEC_R = "VAh Vec r";
        private const string VARH = "VARh";
        private const string VARH_CALCULATED = "VARh (Calculated)";
        private const string VARH_D = "VARh d";
        private const string VARH_LAG = "VARh Lag";
        private const string VARH_NET = "VARh net";
        private const string VARH_NET_D = "VARh net del (Q1-Q4)";
        private const string VARH_NET_R = "VARh net rec (Q2-Q3)";
        private const string VARH_Q1 = "VARh Q1";
        private const string VARH_Q2 = "VARh Q2";
        private const string VARH_Q3 = "VARh Q3";
        private const string VARH_Q4 = "VARh Q4";
        private const string VARH_R = "VARh r";
        private const string VH_A = "Vh(a)";
        private const string VH_AVG = "Vh avg";
        private const string VH_B = "Vh(b)";
        private const string VH_C = "Vh(c)";
        private const string WH = "Wh";
        private const string WH_D = "Wh d";
        private const string WH_NET = "Wh net";
        private const string WH_R = "Wh r";

        //Demand
        private const string A2 = "A^2";
        private const string A_A = "A(a)";
        private const string A_B = "A(b)";
        private const string A_C = "A(c)";
        private const string NA = "NA";
        private const string PF = "PF";
        private const string PF_CALCULATED = "PF (Calculated)";
        private const string Q = "Q";
        private const string Q_R = "Q r";
        private const string V2 = "V^2";
        private const string VA = "VA";
        private const string VA_CALCULATED = "VA (Calculated)";
        private const string VA_ARITH_D = "VA Arith d";
        private const string VA_ARITH_R = "VA Arith r";
        private const string VA_LAG = "VA Lag";
        private const string VA_VEC_D = "VA Vec d";
        private const string VA_VEC_R = "VA Vec r";
        private const string VAR = "VAR";
        private const string VAR_LAG = "VAR Lag";
        private const string VAR_CALCULATED = "VAR (Calculated)";
        private const string VAR_D = "VAR d";
        private const string VAR_NET = "VAR net";
        private const string VAR_NET_D = "VAR net del (Q1-Q4)";
        private const string VAR_NET_R = "VAR net rec (Q2-Q3)";
        private const string VAR_Q1 = "VAR Q1";
        private const string VAR_Q2 = "VAR Q2";
        private const string VAR_Q3 = "VAR Q3";
        private const string VAR_Q4 = "VAR Q4";
        private const string VAR_R = "VAR r";
        private const string V_A = "V(a)";
        private const string V_AVG = "V avg";
        private const string V_B = "V(b)";
        private const string V_C = "V(c)";
        private const string W = "W";
        private const string W_D = "W d";
        private const string W_NET = "W net";
        private const string W_R = "W r";

        #endregion Constants

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="iChannel">Channel number</param>
        /// <param name="fPulseWeight">Pulse weight for the channel</param>		
        /// <param name="fMultiplier">Multiplier for the channel</param>
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        internal LPChannel(string strName, int iChannel, float fPulseWeight, float fMultiplier, ref List<LPInterval> lpIntervals)
        {
            m_iChannelNumber = iChannel;
            m_fPulseWeight = fPulseWeight;
            m_fMultiplier = fMultiplier;
            m_strName = strName;

            m_liIntervals = lpIntervals;
        }

        /// <summary>
        /// Creates a channel of the correct type.
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="iChannel">Channel number</param>
        /// <param name="fPulseWeight">Pulse weight for the channel</param>		
        /// <param name="fMultiplier">Multiplier for the channel</param>
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/07 mrj 9.00.00		Created
        //  
        public virtual LPChannel CreateChannel(string strName, int iChannel, float fPulseWeight, float fMultiplier, ref List<LPInterval> lpIntervals)
        {
            LPChannel channel = new LPChannel(strName, iChannel, fPulseWeight, fMultiplier, ref lpIntervals);
            return channel;
        }

        /// <summary>
        /// Returns the specified number of peaks for the channel in a list
        /// in descending order from largest peak to smallest
        /// </summary>		
        /// <param name="iNumberOfPeaks">number of peaks to get</param>
        /// <returns>list of peaks from largest peak (index 0) to smallest peak
        /// (index iNumberOfPeaks -1)</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public virtual List<LPInterval> GetPeaks(int iNumberOfPeaks)
        {
            //If we are trying to ask for more peaks than there are intervals then change
            //the expected number of peaks to the number of intervals
            if (iNumberOfPeaks > m_liIntervals.Count)
            {
                iNumberOfPeaks = m_liIntervals.Count;
            }

            List<LPInterval> liPeaks = new List<LPInterval>(iNumberOfPeaks);

            //Use the max for the first peak
            if (iNumberOfPeaks > 0)
            {
                liPeaks.Add(GetMaxValue());
            }

            //For each other peak we need...
            for (int i = 1; i < iNumberOfPeaks; i++)
            {
                int iMaxIndex = 0;

                //Set the index of the current maximum pulse to the index of the first
                //interval that is not already in the peak list
                while (liPeaks.Contains(m_liIntervals[iMaxIndex]))
                {
                    iMaxIndex++;
                }

                //Go through each other interval and see if there are any with a larger
                //pulse value than the current max that are not already in the peak list
                for (int iCurrentIndex = 0; iCurrentIndex < m_liIntervals.Count; iCurrentIndex++)
                {
                    if (m_liIntervals[iCurrentIndex].Data[m_iChannelNumber] >
                        m_liIntervals[iMaxIndex].Data[m_iChannelNumber] &&
                        !liPeaks.Contains(m_liIntervals[iCurrentIndex]))
                    {
                        iMaxIndex = iCurrentIndex;
                    }
                }
                liPeaks.Add(m_liIntervals[iMaxIndex]);
            }

            return liPeaks;
        }

        /// <summary>
        /// Returns the load profile interval containing the maximum value for this
        /// channel.
        /// </summary>		
        /// <returns>
        /// The load profile interval containing the maximum pulse value for the 
        /// given channel.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  09/16/08 KRC 2.00.00        Make it so we return last max
        //  01/09/09 jrf 9.50.30 120274 Reverted this method back to retrieve the 
        //                              first occurence of the max value and added a 
        //                              new method to get the most recent max value.
        public virtual LPInterval GetMaxValue()
        {
            int iMaxIndex = -1;

            //If there are intervals
            if (m_liIntervals.Count > 0)
            {
                //Set the first interval as the max so far
                iMaxIndex = 0;

                //Go through each interval and each time a new max is found save its
                //index
                for (int iCurrentIndex = 1; iCurrentIndex < m_liIntervals.Count; iCurrentIndex++)
                {
                    if (m_liIntervals[iCurrentIndex].Data[m_iChannelNumber] >
                        m_liIntervals[iMaxIndex].Data[m_iChannelNumber])
                    {
                        iMaxIndex = iCurrentIndex;
                    }
                }
            }
            //Return null if there were no intervals
            if (iMaxIndex == -1)
            {
                return null;
            }
            return m_liIntervals[iMaxIndex];
        }

        /// <summary>
        /// Returns the load profile interval containing the most recent maximum value for this
        /// channel.
        /// </summary>		
        /// <returns>
        /// The load profile interval containing the maximum pulse value for the 
        /// given channel.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  01/09/09 jrf 9.00.30 120274 Created
        public virtual LPInterval GetMostRecentMaxValue()
        {
            int iMaxIndex = -1;

            //If there are intervals
            if (m_liIntervals.Count > 0)
            {
                //Set the first interval as the max so far
                iMaxIndex = 0;

                //Go through each interval and each time a new max is found save its
                //index
                for (int iCurrentIndex = 1; iCurrentIndex < m_liIntervals.Count; iCurrentIndex++)
                {
                    if (m_liIntervals[iCurrentIndex].Data[m_iChannelNumber] >=
                        m_liIntervals[iMaxIndex].Data[m_iChannelNumber])
                    {
                        iMaxIndex = iCurrentIndex;
                    }
                }
            }
            //Return null if there were no intervals
            if (iMaxIndex == -1)
            {
                return null;
            }
            return m_liIntervals[iMaxIndex];
        }

        /// <summary>
        /// Returns the load profile interval containing the minimum value for this
        /// channel.
        /// </summary>		
        /// <returns>
        /// The load profile interval containing the minimum value for the given channel
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public virtual LPInterval GetMinValue()
        {
            //Start the min index at -1...later return null if it is still -1
            int iMinIndex = -1;

            //If there are any intervals...
            if (m_liIntervals.Count > 0)
            {
                //So far the first interval is the minimum
                iMinIndex = 0;

                //Go through each other interval and each time a new minimum is found
                //save its index
                for (int iMaxIndex = 1; iMaxIndex < m_liIntervals.Count; iMaxIndex++)
                {
                    if (m_liIntervals[iMaxIndex].Data[m_iChannelNumber] <
                        m_liIntervals[iMinIndex].Data[m_iChannelNumber])
                    {
                        iMinIndex = iMaxIndex;
                    }
                }
            }

            //Return null if there were no intervals
            if (iMinIndex == -1)
            {
                return null;
            }

            return m_liIntervals[iMinIndex];
        }

        /// <summary>
        /// This method returns the sum of all the values for this channel.
        /// </summary>		
        /// <returns>
        /// Total value for the given channel.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public virtual double GetTotal()
        {
            double dTotal = 0;

            foreach (LPInterval lpInt in m_liIntervals)
            {
                dTotal += lpInt.Data[m_iChannelNumber];
            }

            return dTotal;
        }

        /// <summary>
        /// This method returns average value for this channel.
        /// </summary>		
        /// <returns>
        /// The average for the give channel.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public virtual double GetAverage()
        {
            return GetTotal() / (double)m_liIntervals.Count;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property for the channel number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if the channel number is not valid.
        /// </exception>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/07 mrj 9.00.00		Created
        //  
        public virtual int ChannelNumber
        {
            get
            {
                return m_iChannelNumber;
            }
            set
            {
                if (value >= 1 && value <= 19)  //16 channels + 3 calculated channels
                {
                    m_iChannelNumber = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "ChannelNumber must be between 1 and 19 inclusive");
                }
            }
        }

        /// <summary>
        /// Property for the pulse weight for the channel.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  07/24/07 mrj 9.00.00		Created
        //  
        public virtual float PulseWeight
        {
            get
            {
                return m_fPulseWeight;
            }
            set
            {
                m_fPulseWeight = value;
            }
        }

        /// <summary>
        /// Property to get the multiplier for the channel.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/07 mrj 9.00.00		Created
        //  
        public virtual float Multiplier
        {
            get
            {
                return m_fMultiplier;
            }
            set
            {
                m_fMultiplier = value;
            }
        }

        /// <summary>
        /// Property to get/set the channel name.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/24/07 mrj 9.00.00		Created
        //  
        public virtual string ChannelName
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }

        #endregion Public Properties

        #region Internal Methods

        /// <summary>
        /// Sets the intervals to the channel
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/31/07 mrj 9.00.00		Created
        //  
        internal virtual void SetIntervals(ref List<LPInterval> lpIntervals)
        {
            m_liIntervals = lpIntervals;
        }

        /// <summary>
        /// This method changes the name based on the display scale.
        /// </summary>
        /// <param name="DisplayScale">Display scale for the data</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/07 mrj 9.00.00		Created
        //	11/12/07 mrj 9.00.25 3276   Added support for varh lag
        //  
        internal void ChangeScale(DisplayScaleOptions DisplayScale)
        {
            bool bNameChanged = false;
            string strTemp = m_strName;

            if (m_strName != null && m_strName.Length != 0)
            {
                string[] Quantities = new string[] { WH, WH_D, WH_R, WH_NET, 
                VARH, VARH_D, VARH_LAG, VARH_R, VARH_NET, VARH_NET_D, VARH_NET_R, VAH,
                VAH_VEC_D, VAH_ARITH_D, VAH_VEC_R, VAH_ARITH_R, VAH_LAG, QH, QH_R, 
                VARH_Q1, VARH_Q2, VARH_Q3, VARH_Q4, AH_A, AH_B, AH_C, NAH, 
                A2H, VH_A, VH_B, VH_C, VH_AVG, V2H, W, W_D, W_R, W_NET, 
                VAR, VAR_LAG, VAR_D, VAR_R, VAR_NET, VAR_NET_D, VAR_NET_R, VA,
                VA_VEC_D, VA_ARITH_D, VA_VEC_R, VA_ARITH_R, VA_LAG, Q, Q_R, 
                VAR_Q1, VAR_Q2, VAR_Q3, VAR_Q4, A_A, A_B, A_C, NA, 
                A2, V_A, V_B, V_C, V_AVG, V2, VAH_CALCULATED, VARH_CALCULATED,
			    VA_CALCULATED, VAR_CALCULATED};

                //Remove any scale for the temporary string
                if (strTemp[0] == 'k' ||
                    strTemp[0] == 'M')
                {
                    //Remove the prefix
                    strTemp = strTemp.Substring(1);
                }

                for (int iIndex = 0; iIndex < Quantities.Length; iIndex++)
                {
                    if (string.Compare(Quantities[iIndex], strTemp, StringComparison.Ordinal) == 0)
                    {
                        //We found a matching quantity so change the scale
                        switch (DisplayScale)
                        {
                            case DisplayScaleOptions.KILO:
                            {
                                m_strName = "k" + strTemp;
                                break;
                            }
                            case DisplayScaleOptions.MEGA:
                            {
                                m_strName = "M" + strTemp;
                                break;
                            }
                            case DisplayScaleOptions.UNITS:
                            default:
                            {
                                m_strName = strTemp;
                                break;
                            }
                        }
                        bNameChanged = true;
                        break;
                    }
                }

                if (string.Compare(PF, strTemp, StringComparison.Ordinal) == 0 ||
                    string.Compare(PF_CALCULATED, strTemp, StringComparison.Ordinal) == 0)
                {
                    //PF does not get converted				
                    bNameChanged = true;
                }

                //If we still did not find the quantity then add the scale to the
                //end of the quantity name
                if (!bNameChanged)
                {
                    //Check to see if the quantity already has a scale, if so remove it
                    int iKiloIndex = m_strName.IndexOf(" (Kilo)", StringComparison.Ordinal);
                    int iMegaIndex = m_strName.IndexOf(" (Mega)", StringComparison.Ordinal);
                    if (iKiloIndex != -1)
                    {
                        m_strName = m_strName.Substring(iKiloIndex);
                    }
                    else if (iMegaIndex != -1)
                    {
                        m_strName = m_strName.Substring(iMegaIndex);
                    }

                    //Add the scale
                    switch (DisplayScale)
                    {
                        case DisplayScaleOptions.KILO:
                        {
                            m_strName = m_strName + " (Kilo)";
                            break;
                        }
                        case DisplayScaleOptions.MEGA:
                        {
                            m_strName = m_strName + " (Mega)";
                            break;
                        }
                        case DisplayScaleOptions.UNITS:
                        default:
                        {
                            //The name stays the same
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes a known quantity to demand
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/02/07 mrj 9.00.00		Created
        //	11/12/07 mrj 9.00.25 3276   Added support for varh lag.
        //  
        internal void ChangeNameToDemand()
        {
            string strTemp = m_strName;
            string strPrefix = "";

            if (m_strName != null && m_strName.Length != 0)
            {
                //These arrays contain the known quantities in a known order
                string[] EnergyQuantities = new string[] { WH, WH_D, WH_R, WH_NET, 
                VARH, VARH_D, VARH_LAG, VARH_R, VARH_NET, VARH_NET_D, VARH_NET_R, VAH,
                VAH_VEC_D, VAH_ARITH_D, VAH_VEC_R, VAH_ARITH_R, VAH_LAG, QH, QH_R, 
                VARH_Q1, VARH_Q2, VARH_Q3, VARH_Q4, AH_A, AH_B, AH_C, NAH, 
                A2H, VH_A, VH_B, VH_C, VH_AVG, V2H, VAH_CALCULATED, VARH_CALCULATED};
                string[] DemandQuantities = new string[] { W, W_D, W_R, W_NET, 
                VAR, VAR_D, VAR_LAG, VAR_R, VAR_NET, VAR_NET_D, VAR_NET_R, VA,
                VA_VEC_D, VA_ARITH_D, VA_VEC_R, VA_ARITH_R, VA_LAG, Q, Q_R, 
                VAR_Q1, VAR_Q2, VAR_Q3, VAR_Q4, A_A, A_B, A_C, NA, 
                A2, V_A, V_B, V_C, V_AVG, V2, VA_CALCULATED, VAR_CALCULATED};

                //Remove any scale for the temporary string
                if (strTemp[0] == 'k')
                {
                    strTemp = strTemp.Substring(1);
                    strPrefix = "k";
                }
                else if (strTemp[0] == 'M')
                {
                    strTemp = strTemp.Substring(1);
                    strPrefix = "M";
                }


                for (int iIndex = 0; iIndex < EnergyQuantities.Length; iIndex++)
                {
                    if (string.Compare(strTemp, EnergyQuantities[iIndex], StringComparison.Ordinal) == 0)
                    {
                        //We found a quantity that needs to change
                        m_strName = strPrefix + DemandQuantities[iIndex];
                        break;
                    }
                }
            }
        }

        #endregion Internal Methods

        #region Members

        /// <summary>
        /// Channel number
        /// </summary>
        protected int m_iChannelNumber;
        /// <summary>
        /// Pulse weight for the channel
        /// </summary>
        protected float m_fPulseWeight;
        /// <summary>
        /// Multiplier for the channel
        /// </summary>
        protected float m_fMultiplier;
        /// <summary>
        /// Name of the channel
        /// </summary>
        protected string m_strName;

        /// <summary>
        /// Reference list of intervals
        /// </summary>		
        protected List<LPInterval> m_liIntervals;

        #endregion Members
    }

    /// <summary>
    /// This class represents a single calculated channel of Load Profile.
    /// </summary>
    public abstract class CalculatedChannel : LPChannel
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName">Name of the calculated channel</param>
        /// <param name="iChannel">Channel number</param>		
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        /// <param name="iWattIndex">index</param>
        /// <param name="iVarIndex">index</param>
        /// <param name="iVAIndex">index</param>
        /// <param name="iQIndex">index</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        internal CalculatedChannel(string strName, int iChannel, ref List<LPInterval> lpIntervals,
                                   int iWattIndex, int iVarIndex, int iVAIndex, int iQIndex)
            : base(strName, iChannel, 1.0f, 1.0f, ref lpIntervals)
        {
            m_iWattIndex = iWattIndex;
            m_iVarIndex = iVarIndex;
            m_iVAIndex = iVAIndex;
            m_iQIndex = iQIndex;
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Property to get the index
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        public int WattIndex
        {
            get
            {
                return m_iWattIndex;
            }
        }

        /// <summary>
        /// Property to get the index
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        public int VarIndex
        {
            get
            {
                return m_iVarIndex;
            }
        }

        /// <summary>
        /// Property to get the index
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        public int VAIndex
        {
            get
            {
                return m_iVAIndex;
            }
        }

        /// <summary>
        /// Property to get the index
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/30/07 mrj 9.00.00		Created
        //  
        public int QIndex
        {
            get
            {
                return m_iQIndex;
            }
        }

        #endregion Public Properties

        #region Members

        /// <summary>
        /// Index to watts that will be used in calculated channel
        /// </summary>
        protected int m_iWattIndex;
        /// <summary>
        /// Index to vars that will be used in calculated channel
        /// </summary>
        protected int m_iVarIndex;
        /// <summary>
        /// Index to VA that will be used in calculated channel
        /// </summary>
        protected int m_iVAIndex;
        /// <summary>
        /// Index to Q that will be used in calculated channel
        /// </summary>
        protected int m_iQIndex;

        #endregion Members
    }

    /// <summary>
    /// This class represent single calculated var channel of Load Profile.
    /// </summary>
    public class CalculatedVarChannel : CalculatedChannel
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>		
        /// <param name="iChannel">Channel number</param>		
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        /// <param name="iWattIndex">index</param>
        /// <param name="iVarIndex">index</param>
        /// <param name="iVAIndex">index</param>
        /// <param name="iQIndex">index</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        internal CalculatedVarChannel(int iChannel, ref List<LPInterval> lpIntervals,
                                      int iWattIndex, int iVarIndex, int iVAIndex, int iQIndex)
            : base("varh (Calculated)", iChannel, ref lpIntervals, iWattIndex, iVarIndex, iVAIndex, iQIndex)
        {
            if (m_liIntervals.Count != 0)
            {
                switch (m_liIntervals[0].DisplayScale)
                {
                    case DisplayScaleOptions.KILO:
                    {
                        m_strName = "k" + m_strName;
                        break;
                    }
                    case DisplayScaleOptions.MEGA:
                    {
                        m_strName = "M" + m_strName;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a channel of the correct type.
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="iChannel">Channel number</param>
        /// <param name="fPulseWeight">Pulse weight for the channel</param>		
        /// <param name="fMultiplier">Multiplier for the channel</param>
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/07 mrj 9.00.00		Created
        //  
        public override LPChannel CreateChannel(string strName, int iChannel, float fPulseWeight, float fMultiplier, ref List<LPInterval> lpIntervals)
        {
            LPChannel channel = new CalculatedVarChannel(iChannel, ref lpIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);
            channel.ChannelName = strName;
            return channel;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// This class represent single calculated VA channel of Load Profile.
    /// </summary>
    public class CalculatedVAChannel : CalculatedChannel
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>			
        /// <param name="iChannel">Channel number</param>		
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        /// <param name="iWattIndex">index</param>
        /// <param name="iVarIndex">index</param>
        /// <param name="iVAIndex">index</param>
        /// <param name="iQIndex">index</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        internal CalculatedVAChannel(int iChannel, ref List<LPInterval> lpIntervals,
                                     int iWattIndex, int iVarIndex, int iVAIndex, int iQIndex)
            : base("VAh (Calculated)", iChannel, ref lpIntervals, iWattIndex, iVarIndex, iVAIndex, iQIndex)
        {
            if (m_liIntervals.Count != 0)
            {
                switch (m_liIntervals[0].DisplayScale)
                {
                    case DisplayScaleOptions.KILO:
                    {
                        m_strName = "k" + m_strName;
                        break;
                    }
                    case DisplayScaleOptions.MEGA:
                    {
                        m_strName = "M" + m_strName;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a channel of the correct type.
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="iChannel">Channel number</param>
        /// <param name="fPulseWeight">Pulse weight for the channel</param>		
        /// <param name="fMultiplier">Multiplier for the channel</param>
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/07 mrj 9.00.00		Created
        //  
        public override LPChannel CreateChannel(string strName, int iChannel, float fPulseWeight, float fMultiplier, ref List<LPInterval> lpIntervals)
        {
            LPChannel channel = new CalculatedVAChannel(iChannel, ref lpIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);
            channel.ChannelName = strName;
            return channel;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// This class represent single power factor channel of Load Profile.
    /// </summary>
    public class CalculatedPFChannel : CalculatedChannel
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>		
        /// <param name="iChannel">Channel number</param>		
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        /// <param name="iWattIndex">index</param>
        /// <param name="iVarIndex">index</param>
        /// <param name="iVAIndex">index</param>
        /// <param name="iQIndex">index</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        internal CalculatedPFChannel(int iChannel, ref List<LPInterval> lpIntervals,
                                     int iWattIndex, int iVarIndex, int iVAIndex, int iQIndex)
            : base("PF (Calculated)", iChannel, ref lpIntervals, iWattIndex, iVarIndex, iVAIndex, iQIndex)
        {
        }

        /// <summary>
        /// Creates a channel of the correct type.
        /// </summary>
        /// <param name="strName">Name of the channel</param>
        /// <param name="iChannel">Channel number</param>
        /// <param name="fPulseWeight">Pulse weight for the channel</param>		
        /// <param name="fMultiplier">Multiplier for the channel</param>
        /// <param name="lpIntervals">Reference pointer to the intervals</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/07 mrj 9.00.00		Created
        //  
        public override LPChannel CreateChannel(string strName, int iChannel, float fPulseWeight, float fMultiplier, ref List<LPInterval> lpIntervals)
        {
            LPChannel channel = new CalculatedPFChannel(iChannel, ref lpIntervals, m_iWattIndex, m_iVarIndex, m_iVAIndex, m_iQIndex);
            channel.ChannelName = strName;
            return channel;
        }

        /// <summary>
        /// PF overrides this method to return min peaks.
        /// 
        /// Returns the specified number of peaks for this channel in a list
        /// in descending order from smallest peak to largest.
        /// </summary>		
        /// <param name="iNumberOfPeaks">number of peaks to get</param>
        /// <returns>list of peaks from smallest peak (index 0) to largest peak
        /// (index iNumberOfPeaks -1)</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  07/30/07 mrj 9.00.00		Created
        //  
        public override List<LPInterval> GetPeaks(int iNumberOfPeaks)
        {
            //If we are trying to ask for more peaks than there are intervals then change
            //the expected number of peaks to the number of intervals
            if (iNumberOfPeaks > m_liIntervals.Count)
            {
                iNumberOfPeaks = m_liIntervals.Count;
            }

            List<LPInterval> liPeaks = new List<LPInterval>(iNumberOfPeaks);

            //Use the min for the first peak
            if (iNumberOfPeaks > 0)
            {
                liPeaks.Add(GetMinValue());
            }

            //For each other peak we need...
            for (int i = 1; i < iNumberOfPeaks; i++)
            {
                int iMinIndex = 0;

                //Set the index of the current min to the index of the first
                //interval that is not already in the peak list
                while (liPeaks.Contains(m_liIntervals[iMinIndex]))
                {
                    iMinIndex++;
                }

                //Go through each other interval and see if there are any with a smaller
                //value than the current min that are not already in the peak list
                for (int iCurrentIndex = 0; iCurrentIndex < m_liIntervals.Count; iCurrentIndex++)
                {
                    if (m_liIntervals[iCurrentIndex].Data[m_iChannelNumber] <
                        m_liIntervals[iMinIndex].Data[m_iChannelNumber] &&
                        !liPeaks.Contains(m_liIntervals[iCurrentIndex]))
                    {
                        iMinIndex = iCurrentIndex;
                    }
                }
                liPeaks.Add(m_liIntervals[iMinIndex]);
            }

            return liPeaks;
        }

        /// <summary>
        /// PF does not support total.
        /// </summary>		
        /// <returns>
        /// Total value for the given channel.
        /// </returns>
        /// 
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/21/07 mrj 9.00.00		Created
        //  
        public override double GetTotal()
        {
            throw new NotSupportedException("PF does not support total");
        }

        /// <summary>
        /// This method returns average value for this channel.
        /// </summary>		
        /// <returns>
        /// The average for the give channel.
        /// </returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/04/07 mrj 9.00.00		Created
        //  
        public override double GetAverage()
        {
            double dTotal = 0;

            foreach (LPInterval lpInt in m_liIntervals)
            {
                dTotal += lpInt.Data[m_iChannelNumber];
            }

            return dTotal / (double)m_liIntervals.Count;
        }

        #endregion Public Methods
    }
}
