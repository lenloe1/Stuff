using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device
{
    #region LoadProfileStatusLIDs Class
    /// <summary>
    /// This class retrieves Load Profile information using LIDs
    /// </summary>
    public class LoadProfileStatusLIDS
    {
        #region Public Methods
        /// <summary>
        /// Constructor for the Load Profile Status LID object
        /// </summary>
        /// <param name="retriever">The LIDRetriever for the device.</param>
        /// <param name="fltLPPulseWeightMultiplier">The multiplier used to convert from meter value to real value</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        internal LoadProfileStatusLIDS(LIDRetriever retriever, float fltLPPulseWeightMultiplier)
        {
            m_LIDRetriever = retriever;
            m_LIDs = new DefinedLIDs();
            m_ChannelsList = new List<ANSILoadProfileChannel>();
            m_byIntervalLength = new CachedByte();
            m_byNumberOfChannels = new CachedByte();
            m_bIsChannelStatusCached = false;
            m_fltLPPulseWeightMultiplier = fltLPPulseWeightMultiplier;
        }

        /// <summary>
        /// Gets the channel object with the selected channel number
        /// </summary>
        /// <param name="ChannelNumber">The number of the channel to get</param>
        /// <returns>
        /// The ANSILoadProfileChannel object of the selected channel if it is in use, or null 
        /// if the channel is not in use.
        /// </returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public ANSILoadProfileChannel GetChannel(int ChannelNumber)
        {
            ANSILoadProfileChannel SelectedChannel = null;

            foreach (ANSILoadProfileChannel Channel in Channels)
            {
                if (Channel.ChannelNumber == ChannelNumber)
                {
                    SelectedChannel = Channel;
                }
            }

            return SelectedChannel;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a boolean that represents whether or not Load Profile is running.
        /// </summary>
        /// <exception cref="PSEMException">Thrown when a communication error occurs.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public bool IsRunning
        {
            get
            {
                object objValue;
                PSEMResponse Result;

                Result = m_LIDRetriever.RetrieveLID(m_LIDs.LP_RUNNING, out objValue);

                if (Result != PSEMResponse.Ok)
                {
                    throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result, 
                        "Error reading Load Profile running flag.");
                }
                else
                {
                    // The data returned should be 1 byte
                    m_bRunning = (byte)objValue == (byte)1;
                }

                return m_bRunning;
            }
        }

        /// <summary>
        /// Gets the interval length in minutes
        /// </summary>
        /// <exception cref="PSEMException">Thrown when a communication error occurs.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public int IntervalLength
        {
            get
            {
                if (m_byIntervalLength.Cached == false)
                {
                    byte[] Data;
                    PSEMResponse Result;

                    Result = m_LIDRetriever.RetrieveLID(m_LIDs.LP_INTERVAL_LENGTH, out Data);

                    if (Result == PSEMResponse.Ok)
                    {
                        m_byIntervalLength.Value = Data[0];
                    }
                    else
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result, 
                            "Error reading Load Profile Interval Length LID");
                    }
                }

                return (int)m_byIntervalLength.Value;
            }
        }

        /// <summary>
        /// Gets the total number of Load Profile channels in the meter
        /// </summary>
        /// <exception cref="PSEMException">Thrown when a communication error occurs.</exception>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public int NumberOfChannels
        {
            get
            {
                if (m_byNumberOfChannels.Cached == false)
                {
                    object objValue;
                    PSEMResponse Result;

                    Result = m_LIDRetriever.RetrieveLID(m_LIDs.LP_NUM_CHANNELS, out objValue);

                    if (Result == PSEMResponse.Ok)
                    {
                        m_byNumberOfChannels.Value = (byte)objValue;
                    }
                    else
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result, 
                            "Error reading number of Load Profile channels LID");
                    }
                }

                return (int)m_byNumberOfChannels.Value;
            }
        }

        /// <summary>
        /// Gets an array of Channel objects
        /// </summary>
        public ANSILoadProfileChannel[] Channels
        {
            get
            {
                if (m_bIsChannelStatusCached == false)
                {
                    PSEMResponse Result;

                    Result = ReadChannelStatus();

                    if (Result != PSEMResponse.Ok)
                    {
                        throw new PSEMException(PSEMException.PSEMCommands.PSEM_READ, Result,
                            "Error reading Load Profile channels status");
                    }
                }

                return m_ChannelsList.ToArray();
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Reads the status LIDs from the meter and creates the channel objects
        /// </summary>
        /// <returns>The PSEM response code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
        // 04/10/07 KRC 8.00.27 2864   Fixing LID read to get Pulse Weights supported by all versions
        private PSEMResponse ReadChannelStatus()
        {
            PSEMResponse Result;
            int iTotalChannels = NumberOfChannels;
            LID[] PulseWidthLIDs = new LID[iTotalChannels];
            LID[] QuantityLIDs = new LID[iTotalChannels];
            List<object> lstObjPulseWidthData = null;
            List<object> listObjQuantityData = null;
            m_ChannelsList.Clear();

            // Set up the LIDs for reading
            for (uint uiChannel = 0; uiChannel < iTotalChannels; uiChannel++)
            {
                QuantityLIDs[uiChannel] = new LID(m_LIDs.LP_CHAN_1_QUANTITY.lidValue + uiChannel);
                PulseWidthLIDs[uiChannel] = new LID(m_LIDs.LP_CHAN_1_INT_PULSE_WT.lidValue + uiChannel);
            }

            // Read the LIDs
            Result = m_LIDRetriever.RetrieveMulitpleLIDs(QuantityLIDs, out listObjQuantityData);

            if (Result == PSEMResponse.Ok)
            {
                Result = m_LIDRetriever.RetrieveMulitpleLIDs(PulseWidthLIDs, out lstObjPulseWidthData);
            }

            if (Result == PSEMResponse.Ok)
            {
                // Create the Channel Status objects from the data
                for (int iChannel = 1; iChannel <= iTotalChannels; iChannel++)
                {
                    ANSILoadProfileChannel ChannelStatus = new ANSILoadProfileChannel();

                    ChannelStatus.ChannelNumber = iChannel;
                    ChannelStatus.QuantityLID = (UInt32)listObjQuantityData[iChannel - 1]; 
                    // We are getting the integer config value for the pulse weight, so we need to scale it to get the real value.
                    ChannelStatus.PulseWeight = Convert.ToSingle(lstObjPulseWidthData[iChannel - 1], CultureInfo.InvariantCulture) * m_fltLPPulseWeightMultiplier;

                    m_ChannelsList.Add(ChannelStatus);
                }

                m_bIsChannelStatusCached = true;
            }

            return Result;
        }

        #endregion

        #region Member Variables
        private LIDRetriever m_LIDRetriever;
        private DefinedLIDs m_LIDs;

        private List<ANSILoadProfileChannel> m_ChannelsList;
        private CachedByte m_byIntervalLength;
        private CachedByte m_byNumberOfChannels;
        private bool m_bRunning;
        private bool m_bIsChannelStatusCached;
        private float m_fltLPPulseWeightMultiplier;
        #endregion
    }
    #endregion

    #region LoadProfileChannel Class
    /// <summary>
    /// Stores information on a single channel of Load Profile
    /// </summary>
    public class ANSILoadProfileChannel
    {
        #region Public Methods
        /// <summary>
        /// Default Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

		public ANSILoadProfileChannel()
        {
            m_QuantityLID = new LID(0);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the channel number for this status object.
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created
		
        //public override int ChannelNumber
		public int ChannelNumber
        {
            get
            {
                return m_iChannelNumber;
            }
            set
            {
                if (value >= 1 && value <= 8)
                {
                    m_iChannelNumber = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ChannelNumber must be between 1 and 8 inclusive");
                }
            }
        }
		
        /// <summary>
        /// Gets the LID object for the quantity this channel is measuring
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        internal uint QuantityLID
        {
            set
            {
                m_QuantityLID.lidValue = value;
            }
        }

        /// <summary>
        /// Gets the name of the Quantity for the channel
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/02/06 RCG 7.40.00 N/A    Created

        public string QuantityName
        {
            get
            {
                return m_QuantityLID.lidDescription;
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
		public float PulseWeight
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
        #endregion

        #region Member Variables
        
        private LID m_QuantityLID;

		private float m_fPulseWeight;
		private int m_iChannelNumber;

        #endregion
    }
    #endregion
}
