using System;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Enumeration to used to determine what to include in an EDL file. 
    /// Configuration and billing data is assumed to always be included.
    /// </summary>
    [Flags]
    public enum EDLSections
    {
        /// <summary>
        /// No extra tables will be included.
        /// </summary>
        None = 0,
        /// <summary>
        /// Include History and Event log tables
        /// </summary>
        HistoryLog = 1,
        /// <summary>
        /// Include Load Profile tables
        /// </summary>
        LoadProfile = 2,
        /// <summary>
        /// Include Voltage Monitoring tables
        /// </summary>
        VoltageMonitoring = 4,
        /// <summary>
        /// Include LAN and HAN network tables
        /// </summary>
        NetworkTables = 8,
        /// <summary>
        /// Include LAN and HAN log tables
        /// </summary>
        LANandHANLog = 16,
        /// <summary>
        /// Include Instrumentation Profile tables
        /// </summary>
        InstrumentationProfile = 32,
    }

    /// <summary>
    /// Enumeration to used to determine what configuration blocks
    /// to write to during a partial configuration.
    /// </summary>
    [Flags]
    public enum BlocksForPartialConfiguration : long
    {
        /// <summary>
        /// Reconfigure Non-billing Energy and Load Profile
        /// </summary>
        ExtendedEnergyAndLoadProfile = 1,
        /// <summary>
        /// Reconfigure Instrumentation Profile
        /// </summary>
        InstrumentationProfile = 2,
        /// <summary>
        /// Reconfigure Extended Voltage Monitoring
        /// </summary>
        ExtendedVoltageMonitoring = 4,
        /// <summary>
        /// Reconfigure Extended Self Read
        /// </summary>
        ExtendedSelfRead = 8,
    }

    /// <summary>
    /// Interface for devices that can create an EDL file
    /// </summary>
    public interface ICreateEDL
    {
        /// <summary>
        /// Creates an EDL file from the device tables.
        /// </summary>
        /// <param name="FileName">Path to the file where the EDL file will be written.</param>
        /// <returns>CreateEDLResult Code.</returns>
        CreateEDLResult CreateEDLFromMeter(string FileName);

        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="FileName">Path to the file where the EDL file will be written.</param>
        /// <param name="IncludedSections">The sections to include in the EDL file.</param>
        /// <returns>CreateEDLResult Code.</returns>
        CreateEDLResult CreateEDLFromMeter(string FileName, EDLSections IncludedSections);
        
        /// <summary>
        /// Creates an EDL file with the specified sections.
        /// </summary>
        /// <param name="fileName">Path to the file where the EDL file will be written.</param>
        /// <param name="includedSections">The sections to include in the EDL file.</param>
        /// <param name="logoffAndLogon">Method to logoff and log back on to meter.</param>
        /// <returns></returns>
        CreateEDLResult CreateEDLFromMeter(string fileName, EDLSections includedSections, Func<bool> logoffAndLogon);
    }
}
