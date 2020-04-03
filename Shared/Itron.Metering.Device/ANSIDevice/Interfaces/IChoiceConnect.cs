using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface which needs to be implemented by devices that
    /// can operate on a ChoiceConnect network.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  11/13/13 jrf 3.50.03 N/A    Created
    //
    public interface IBridge
    {
        /// <summary>
        /// Gets whether the meter is a Bridge meter that was released 
        /// during the initial Bridge project (Phase 1).
        /// </summary>
        bool IsBridgePhase1Meter
        {
            get;
        }

        /// <summary>
        /// Gets the mode the Bridge meter was manufactured as.
        /// </summary>
        OpenWayMFGTable2428.ChoiceConnectCommMfgMode ChoiceConnectManufacturedMode
        {
            get;
        }

        /// <summary>
        /// Gets the register's current communications operating mode.
        /// </summary>
        OpenWayMFGTable2428.ChoiceConnectCommOpMode CurrentRegisterCommOpMode
        {
            get;
        }

        /// <summary>
        /// Gets the register's requested communications operating mode.
        /// </summary>
        OpenWayMFGTable2428.ChoiceConnectCommOpMode RequestedRegisterCommOpMode
        {
            get;
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware Version.Revision string
        /// </summary>
        string ChoiceConnectFWVerRev
        {
            get;
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Firmware Build string
        /// </summary>
        string ChoiceConnectFWBuild
        {
            get;
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM ERT ID as a formatted string
        /// </summary>
        string ChoiceConnectERTID
        {
            get;
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Bubble-up LID translated as a string
        /// </summary>
        string ChoiceConnectBubbleUpLIDDescription
        {
            get;
        }

        /// <summary>
        /// Gets the ChoiceConnect MSM Security State as a formatted string
        /// </summary>
        string ChoiceConnectSecurityStateDescription
        {
            get;
        }

        /// <summary>
        /// Switches the Comm Operational Mode in an ChoiceConnect capable meter.
        /// </summary>
        /// <param name="opMode">The Comm Operational Mode to which the meter should switch</param>
        /// <returns>The result of the procedure call</returns>
        ProcedureResultCodes SwitchCommOperationMode(OpenWayMFGTable2428.ChoiceConnectCommOpMode opMode);
        

        /// <summary>
        /// Method causes state and/or time sensitive ChoiceConnect table data to be refreshed
        /// when their data is next accessed.
        /// </summary>
        void RefreshChoiceConnectData();

        /// <summary>
        ///  Gets whether or not the 25 Year TOU schedule is supported.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/08/14 AF  3.50.22 TQ9484 Created
        //
        bool Supports25YearTOUSchedule
        {
            get;
        }

        /// <summary>
        /// Checks the meter's configuration to make sure that it is compatible with
        /// ChoiceConnect
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/14 AF  3.50.25 TQ9489 Created
        //
        bool IsConfigChoiceConnectCompatible
        {
            get;
        }

        /// <summary>
        /// Checks the meter's configuration to make sure that it is compatible with
        /// ChoiceConnect
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  01/14/14 AF  3.50.25 TQ9489 Created
        //
        byte OpenWayCommModuleRevision
        {
            get;
        }

    }
}
