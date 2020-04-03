using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.Device
{
    /// <summary>
    /// Interface for devices that are able to validate with a program.
    /// </summary>
    public interface IValidateProgram
    {
        /// <summary>
        /// Returns a list of items that are not consistent between the configuration
        /// of the program and the device.
        /// </summary>
        /// <param name="strProgramName">The name of the program to validate against.</param>
        /// <returns>
        /// A list of items that failed the validation. Returns an empty list if
        /// all items match.
        /// </returns>
        List<ProgramValidationItem> ValidateProgram(string strProgramName);
    }
}
