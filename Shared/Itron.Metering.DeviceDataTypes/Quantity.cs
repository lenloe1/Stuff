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
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Itron.Metering.DeviceDataTypes
{
    /// <summary>
    /// Quantity Base Class - Represents all meausrement values associated
    /// 	with a given quantity.
    /// </summary>
    public class Quantity
    {
        #region Public Methods

      
        /// <summary>
        /// Creates a quantity object
        /// </summary>
        /// <param name="strDescription"> A user viewable description of the new quantity</param>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  11/21/06 MAH 8.00			Created
        /// </remarks>
        public Quantity( String strDescription ) 
        {
            Description = strDescription;

            m_EnergyMeasurement = null;
            m_MaxDemandMeasurement = null;
            m_CummulativeDemandMeasurement = null;
            m_ContinuousCummulativeMeasurement = null;
            m_TOUEnergyMeasurements = null;
            m_TOUMaxDemandMeasurements = null;
            m_TOUCumMeasurements = null;
            m_TOUCCumMeasurements = null;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// A user viewable description of the quantity object
        /// </summary>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  11/21/06 MAH 8.00			Created
        /// </remarks>
        public String Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        /// <summary>
        /// Provides read access to the Energy Data
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/06 KRC 7.36			Created
        //  11/21/06 jrf 8.00           Changed from abstract to virtual property
        //
        public virtual Measurement TotalEnergy
        {
            get
            {
                return m_EnergyMeasurement;
            }
            set
            {
                m_EnergyMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the cummulative demand reading
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/03/07 MAH 8.00			Created
        /// </remarks>
        public virtual Measurement CummulativeDemand
        {
            get
            {
                return m_CummulativeDemandMeasurement;
            }
            set
            {
                m_CummulativeDemandMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the continuous cummulative demand reading
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/03/07 MAH 8.00			Created
        /// </remarks>
        public virtual Measurement ContinuousCummulativeDemand
        {
            get
            {
                return m_ContinuousCummulativeMeasurement;
            }
            set
            {
                m_ContinuousCummulativeMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the Max Demand data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/06 KRC 7.36			Created
        //  11/21/06 jrf 8.00           Changed from abstract to virtual property
        //
        public virtual DemandMeasurement TotalMaxDemand
        {
            get
            {
                return m_MaxDemandMeasurement;
            }
            set
            {
                m_MaxDemandMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the TOU Energy Data
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/06 KRC 7.36			Created
        //  11/21/06 jrf 8.00           Changed from abstract to virtual property
        //
        public virtual List<Measurement> TOUEnergy
        {
            get
            {
                return m_TOUEnergyMeasurements;
            }
            set
            {
                m_TOUEnergyMeasurements = value;
            }
        }

        /// <summary>
        /// Provides read access to the TOU Max Demand data.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/06 KRC 7.36			Created
        //  11/21/06 jrf 8.00           Changed from abstract to virtual property
        //
        public virtual List<DemandMeasurement> TOUMaxDemand
        {
            get
            {
                return m_TOUMaxDemandMeasurements;
            }
            set
            {
                m_TOUMaxDemandMeasurements = value;
            }
        }

        /// <summary>
        /// Provides read access to the TOU cummulative demand readings
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/03/07 MAH 8.00			Created
        /// </remarks>
        public virtual List<Measurement> TOUCummulativeDemand
        {
            get
            {
                return m_TOUCumMeasurements;
            }
            set
            {
                m_TOUCumMeasurements = value;
            }
        }

        /// <summary>
        /// Provides read access to the TOU continuous cummulative demand readings
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        /// <remarks>
        ///  Revision History
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ ---------------------------------------------
        ///  01/03/07 MAH 8.00			Created
        /// </remarks>
        public virtual List<Measurement> TOUCCummulativeDemand
        {
            get
            {
                return m_TOUCCumMeasurements;
            }
            set
            {
                m_TOUCCumMeasurements = value;
            }
        }

        /// <summary>
        /// Refresh all of our Measurment objects so they will be reread.
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/28/06 KRC 7.35.00 N/A    Created
        //  03/29/11 jrf 2.50.16        Corrected error that prevented m_TOUMaxDemandMeasurements from
        //                              getting reset.
        //
        public void Refresh()
        {
            if (null != m_EnergyMeasurement)
            {
                m_EnergyMeasurement = null;
            }
            if (null != m_MaxDemandMeasurement)
            {
                m_MaxDemandMeasurement = null;
            }
            if (null != m_CummulativeDemandMeasurement)
            {
                m_CummulativeDemandMeasurement = null;
            }
            if (null != m_ContinuousCummulativeMeasurement)
            {
                m_ContinuousCummulativeMeasurement = null;
            }
            if (null != m_TOUEnergyMeasurements)
            {
                m_TOUEnergyMeasurements = null;
            }
            if (null != m_TOUMaxDemandMeasurements)
            {
                m_TOUMaxDemandMeasurements = null;
            }
        }
        #endregion

        #region Members

        /// <summary>
        /// Total Energy Measurment data
        /// </summary>
        protected Measurement m_EnergyMeasurement;

        /// <summary>
        /// Total Demand Measurement data
        /// </summary>
        protected DemandMeasurement m_MaxDemandMeasurement;

        /// <summary>
        /// Cummulative demand data
        /// </summary>
        protected Measurement m_CummulativeDemandMeasurement;

        /// <summary>
        /// Continuous cummulative demand data
        /// </summary>
        protected Measurement m_ContinuousCummulativeMeasurement;

        /// <summary>
        /// List of TOU Energies
        /// </summary>
        protected List<Measurement> m_TOUEnergyMeasurements;

        /// <summary>
        /// List of TOU cummulative demand values
        /// </summary>
        protected List<Measurement> m_TOUCumMeasurements;

        /// <summary>
        /// List of TOU continuous cummulative demand values
        /// </summary>
        protected List<Measurement> m_TOUCCumMeasurements;

        /// <summary>
        /// List of TOU Max Demands
        /// </summary>
        protected List<DemandMeasurement> m_TOUMaxDemandMeasurements;
        
        /// <summary>
        /// A user viewable description of the quantity object 
        /// </summary>
        protected String m_strDescription;

        #endregion

    }

    /// <summary>
    /// This class represents a collection of Quantities in the meter.
    /// </summary>
    /// Revision History
    /// MM/DD/YY who Version Issue# Description
    /// -------- --- ------- ------ ---------------------------------------------
    /// 09/29/06 KRC 7.35.00        Created
    ///
    public class QuantityCollection
    {
        #region Constants
        #endregion

        #region Definitions
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor, this method sizes the diagnostics and sets
        /// their names.
        /// </summary>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------------
        /// 09/28/06 KRC 7.35.00        Created
        ///
        public QuantityCollection()
        {
            m_Quantities = new List<Quantity>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the Quantity List
        /// </summary>
        /// <returns>
        /// A List of Quantities
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/06 KRC 7.35.00	 N/A	Creation of class  
        public List<Quantity> Quantities
        {
            get
            {
                return m_Quantities;
            }
        }

        /// <summary>
        /// Provides access to the Date/Time of Reading
        /// </summary>
        /// <returns>
        /// A List of DateTime
        /// </returns>
        // Revision History
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/29/06 KRC 7.35.00	 N/A	Creation of class  
        public DateTime DateTimeOfReading
        {
            get
            {
                return m_dtTimeOfRead;
            }
            set
            {
                m_dtTimeOfRead = value;
            }
        }

        #endregion

        #region Internal Methods
        #endregion

        #region Internal Properties
        #endregion

        #region Protected Methods
        #endregion

        #region Protected Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Members
        
        List<Quantity> m_Quantities;
        DateTime m_dtTimeOfRead;

        #endregion
    }

    /// <summary>
    /// This class contains all of the information that goes along with a
    /// Energy measurement.
    /// </summary>
    public class Measurement
    {
        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.36			Created
        //
        public Measurement()
        {
            // Make sure each quantity starts out empty and not enabled.
            m_dblValue = 0.0;
            m_strDescription = "";
        }

        /// <summary>
        /// Constructor - Include Value and Description
        /// </summary>
        /// <param name="dblValue">The Value of this Measurement</param>
        /// <param name="strDescription">Description of this Measurement</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.36			Created
        //
        public Measurement(double dblValue, string strDescription)
        {
            m_dblValue = dblValue;
            m_strDescription = strDescription;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Provides access to the Measurmeent value
        /// </summary>
        /// <exception cref="Exception">Thrown when value not available.</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.35			Created
        //
        public double Value
        {
            get
            {
                return m_dblValue;
            }
            set
            {
                m_dblValue = value;
            }
        }

        /// <summary>
        /// Provies access to the Quantities Description
        /// </summary>
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        #endregion

        #region Members

        private double m_dblValue;
        private string m_strDescription;
       
        #endregion

    }

    /// <summary>
    /// This class contains all of the information that goes along with a
    /// Demand measurement.
    /// </summary>
    public class DemandMeasurement : Measurement
    {
        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.36			Created
        //
        public DemandMeasurement()
            : base()
        {
        }
                
        /// <summary>
        /// Constructor - Include Value and Description
        /// </summary>
        /// <param name="dblValue">The Value of this Measurement</param>
        /// <param name="strDescription">Description of this Measurement</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/22/06 KRC 7.36			Created
        //
        public DemandMeasurement(double dblValue, string strDescription)
            : base(dblValue, strDescription)
        {
        }

        /// <summary>
        /// Constructor - Include Value and Description
        /// </summary>
        /// <param name="dblValue">The Value of this Measurement</param>
        /// <param name="dtTimeOfOccurrence"></param>
        /// <param name="strDescription">Description of this Measurement</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public DemandMeasurement(double dblValue, DateTime dtTimeOfOccurrence, string strDescription)
            : base(dblValue, strDescription)
        {
            m_TimeOfOccurrence = dtTimeOfOccurrence;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Provides access to the Time of Occurrence of a Demand Quantity
        /// </summary>
        /// <exception cref="Exception">Thrown when value not available.</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  09/25/06 KRC 7.35			Created
        //
        public DateTime TimeOfOccurrence
        {
            get
            {
                return m_TimeOfOccurrence;
            }
            set
            {
                m_TimeOfOccurrence = value;
            }
        }
      
        #endregion

        #region Members

        private DateTime m_TimeOfOccurrence;
       
        #endregion

    }

    /// <summary>
    /// Represents all meausrement values associated with a given instantaneous quantity.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/07/11 jrf 2.51.10 173353 Rewrote this class to handle current, minimum, maximum and average 
    //                              instantaneous values returned from the power monitoring tables.
    //
    public class InstantaneousQuantity
    {
        #region Public Methods


        /// <summary>
        /// Creates an instantaneous quantity object
        /// </summary>
        /// <param name="strDescription"> A user viewable description of the new quantity</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public InstantaneousQuantity(String strDescription)
        {
            m_strDescription = strDescription;

            m_MostRecentMeasurement = null;
            m_MaxMeasurement = null;
            m_MinMeasurement = null;
            m_AvgMeasurement = null;
            m_MostRecentMeasurementsPerPhase = null;
            m_AvgMeasurementsPerPhase = null;
            m_MaxMeasurementsPerPhase = null;
            m_MinMeasurementsPerPhase = null;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// A user viewable description of the quantity object
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public String Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        /// <summary>
        /// Provides read access to the most recent instantaneous reading.  For polyphase meters
        /// this will be the aggregate of all phases.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual DemandMeasurement MostRecentMeasurement
        {
            get
            {
                return m_MostRecentMeasurement;
            }
            set
            {
                m_MostRecentMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the minimum instantaneous reading.  For polyphase meters
        /// this will be the aggregate of all phases.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual DemandMeasurement Minimum
        {
            get
            {
                return m_MinMeasurement;
            }
            set
            {
                m_MinMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the average instantaneous reading.  For polyphase meters
        /// this will be the aggregate of all phases.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual DemandMeasurement Average
        {
            get
            {
                return m_AvgMeasurement;
            }
            set
            {
                m_AvgMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the maximum instantaneous reading. For polyphase meters
        /// this will be the aggregate of all phases.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual DemandMeasurement Maximum
        {
            get
            {
                return m_MaxMeasurement;
            }
            set
            {
                m_MaxMeasurement = value;
            }
        }

        /// <summary>
        /// Provides read access to the most recent instantaneous measurements for each phase
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual List<DemandMeasurement> MostRecentMeasurementsPerPhase
        {
            get
            {
                return m_MostRecentMeasurementsPerPhase;
            }
            set
            {
                m_MostRecentMeasurementsPerPhase = value;
            }
        }

        /// <summary>
        /// Provides read access to the average instantaneous measurements for each phase
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual List<DemandMeasurement> AveragePerPhase
        {
            get
            {
                return m_AvgMeasurementsPerPhase;
            }
            set
            {
                m_AvgMeasurementsPerPhase = value;
            }
        }

        /// <summary>
        /// Provides read access to the maximum instantaneous measurements for each phase
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual List<DemandMeasurement> MaximumPerPhase
        {
            get
            {
                return m_MaxMeasurementsPerPhase;
            }
            set
            {
                m_MaxMeasurementsPerPhase = value;
            }
        }

        /// <summary>
        /// Provides read access to the minimum instantaneous measurements for each phase
        /// </summary>
        /// <exception cref="Exception">Thrown if Read fails</exception>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public virtual List<DemandMeasurement> MinimumPerPhase
        {
            get
            {
                return m_MinMeasurementsPerPhase;
            }
            set
            {
                m_MinMeasurementsPerPhase = value;
            }
        }

        /// <summary>
        /// Refresh all of our Measurment objects so they will be reread.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/07/11 jrf 2.51.10 173353 Created
        //
        public void Refresh()
        {
            if (null != m_MostRecentMeasurement)
            {
                m_MostRecentMeasurement = null;
            }
            if (null != m_MaxMeasurement)
            {
                m_MaxMeasurement = null;
            }
            if (null != m_MinMeasurement)
            {
                m_MinMeasurement = null;
            }
            if (null != m_AvgMeasurement)
            {
                m_AvgMeasurement = null;
            }
            if (null != m_MostRecentMeasurementsPerPhase)
            {
                m_MostRecentMeasurementsPerPhase = null;
            }
            if (null != m_MinMeasurementsPerPhase)
            {
                m_MinMeasurementsPerPhase = null;
            }
            if (null != m_MaxMeasurementsPerPhase)
            {
                m_MaxMeasurementsPerPhase = null;
            }
            if (null != m_AvgMeasurementsPerPhase)
            {
                m_AvgMeasurementsPerPhase = null;
            }
        }
        #endregion

        #region Members

        /// <summary>
        /// Most recent instantaneous measurment data
        /// </summary>
        protected DemandMeasurement m_MostRecentMeasurement;

        /// <summary>
        /// Maximum instantaneous measurment data
        /// </summary>
        protected DemandMeasurement m_MaxMeasurement;

        /// <summary>
        /// Minimum instantaneous measurment data
        /// </summary>
        protected DemandMeasurement m_MinMeasurement;

        /// <summary>
        /// Average instantaneous measurment data
        /// </summary>
        protected DemandMeasurement m_AvgMeasurement;

        /// <summary>
        /// List of the most recent instantaneous measurment data for each phase
        /// </summary>
        protected List<DemandMeasurement> m_MostRecentMeasurementsPerPhase;

        /// <summary>
        /// List of the maximum instantaneous measurement data for each phase
        /// </summary>
        protected List<DemandMeasurement> m_MaxMeasurementsPerPhase;

        /// <summary>
        /// List of the minimum instantaneous measurement data for each phase
        /// </summary>
        protected List<DemandMeasurement> m_MinMeasurementsPerPhase;

        /// <summary>
        /// List of the average instantaneous measurement data for each phase
        /// </summary>
        protected List<DemandMeasurement> m_AvgMeasurementsPerPhase;

        /// <summary>
        /// A user viewable description of the quantity object 
        /// </summary>
        protected String m_strDescription;

        #endregion

    }
}
