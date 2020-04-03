using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Progressable
{
    /// <summary>
    /// Interface for a control that can display a progress bar on another form using events
    /// </summary>
    public interface IProgressable
    {
        /// <summary>
        /// Event for showing the progress bar
        /// </summary>
        event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event for hiding the progress bar
        /// </summary>
        event HideProgressEventHandler HideProgressEvent;

        /// <summary>
        /// Event for causing the progress bar to perform a step
        /// </summary>
        event StepProgressEventHandler StepProgressEvent;
    }
}
