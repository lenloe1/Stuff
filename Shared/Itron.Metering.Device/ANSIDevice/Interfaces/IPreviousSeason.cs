using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by devices that
    /// can support previous season data.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/26/13 jrf 3.50.09        Created
    //
    public interface IPreviousSeason
    {
        /// <summary>
        /// Gets whether the meter has any previous season data.
        /// </summary>
        bool HasPreviousSeasonData
        {
            get;
        }

        /// <summary>
        /// Gets the end date of the previous season.
        /// </summary>
        DateTime PreviousSeasonEndDate
        {
            get;
        }

        /// <summary>
        /// Proves access to a list of Energy Quantities from last season (Std table 24)
        /// </summary>
        List<Quantity> PreviousSeasonRegisters
        {
            get;
        }

        #region Previous Season Quantities

        /// <summary>
        /// Gets the previous season Neutral Amps from the standard tables.
        /// </summary>
        Quantity PreviousSeasonAmpsNeutral
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase A Amps from the standard tables.
        /// </summary>
        Quantity PreviousSeasonAmpsPhaseA
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase B Amps from the standard tables.
        /// </summary>
        Quantity PreviousSeasonAmpsPhaseB
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase C Amps from the standard tables.
        /// </summary>
        Quantity PreviousSeasonAmpsPhaseC
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Amps squared from the standard tables.
        /// </summary>
        Quantity PreviousSeasonAmpsSquared
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Q Delivered from the standard tables.
        /// </summary>
        Quantity PreviousSeasonQDelivered
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Qh Received from the standard tables.
        /// </summary>
        Quantity PreviousSeasonQReceived
        {
            get;
        }

        /// <summary>
        /// Gets the VA Delivered from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVADelivered
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Lagging VA from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVALagging
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Delivered from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarDelivered
        {
            get;
        }

        /// <summary>
        /// Gets the previous season VA Received from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVAReceived
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Net from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarNet
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Net delivered from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarNetDelivered
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Net Received from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarNetReceived
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Q1 from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarQuadrant1
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Q2 from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarQuadrant2
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Q3 from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarQuadrant3
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Q4 from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarQuadrant4
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Var Received from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVarReceived
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Average Volts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVoltsAverage
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase A Volts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVoltsPhaseA
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase B Volts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVoltsPhaseB
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Phase C Volts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVoltsPhaseC
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Volts squared from the standard tables.
        /// </summary>
        Quantity PreviousSeasonVoltsSquared
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Watts Delivered quantity from the standard tables.
        /// </summary>
        Quantity PreviousSeasonWattsDelivered
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Watts Received quantity from the standard tables
        /// </summary>
        Quantity PreviousSeasonWattsReceived
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Watts Net quantity from the standard tables.
        /// </summary>
        Quantity PreviousSeasonWattsNet
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Unidirectional Watts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonWattsUni
        {
            get;
        }

        /// <summary>
        /// Gets the previous season Unidirectional Watts from the standard tables.
        /// </summary>
        Quantity PreviousSeasonPowerFactor
        {
            get;
        }

        #endregion
    }
}
