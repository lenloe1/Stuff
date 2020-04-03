using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Progressable
{
    #region Public Delegates
    /// <summary>
    /// Delegate for the show progress event handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void ShowProgressEventHandler(object sender, ShowProgressEventArgs e);

    /// <summary>
    /// Delegate for the hide progress event handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void StepProgressEventHandler(object sender, ProgressEventArgs e);

    /// <summary>
    /// Delegate for the step progress event handler
    /// </summary>
    /// <param name="sender">The object that sent the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void HideProgressEventHandler(object sender, EventArgs e);

    #endregion

    /// <summary>
    /// Event arguments for a progress event
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constants

        /// <summary>
        /// The minimum number of steps that can be performed.
        /// </summary>
        private const int MIN_NUMER_OF_STEPS = 1;

        #endregion Constants

        /// <summary>
        /// The description will be displayed on the progress bar 
        /// </summary>
        protected string m_strStatus;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProgressEventArgs() 
            : base()
        {
            m_strStatus = null;
            StepsToIncrement = MIN_NUMER_OF_STEPS;
        }

        /// <summary>
        /// Constructor that sets the status
        /// </summary>
        /// <param name="strStatus">The status text for the progress event.</param>
        public ProgressEventArgs(string strStatus) 
            : base()
        {
            m_strStatus = strStatus;
            StepsToIncrement = MIN_NUMER_OF_STEPS;
        }

        /// <summary>
        /// Creates a new Progress event argument object.
        /// </summary>
        /// <param name="strStatus">The status text for the progress event.</param>
        /// <param name="stepsToIncrement">The number of steps to increment.</param>
        public ProgressEventArgs(string strStatus, int stepsToIncrement)
            : this(strStatus)
        {
            StepsToIncrement = stepsToIncrement;
        }

        /// <summary>
        /// Gets or sets the status string
        /// </summary>
        public string Status
        {
            get
            {
                return m_strStatus;
            }
            set
            {
                m_strStatus = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the number of steps to increment.
        /// </summary>
        public int StepsToIncrement { get; set; }
    }
    
    /// <summary>
    /// Event arguments for the show progress event
    /// </summary>
    public class ShowProgressEventArgs : ProgressEventArgs
    {
        /// <summary>
        /// The total number of steps in the progress bar
        /// </summary>
        protected int m_iNumberOfSteps;

        /// <summary>
        /// The number of steps to take
        /// </summary>
        protected int m_iStepSize;

        /// <summary>
        /// The title for the form
        /// </summary>
        protected string m_strTitle;

        /// <summary>
        /// Constructor that sets the step size and the number of steps
        /// </summary>
        /// <param name="iStepSize">The number of steps taken when performing a step</param>
        /// <param name="iNumberofSteps">The total number of steps in the progress bar</param>
        public ShowProgressEventArgs(int iStepSize, int iNumberofSteps) 
        {
            m_strStatus = null;
            m_strTitle = null;
            m_iStepSize = iStepSize;
            m_iNumberOfSteps = iNumberofSteps;
        }

        /// <summary>
        /// Constructor that sets the step size, the number of steps, and the title
        /// </summary>
        /// <param name="iStepSize">The number of steps taken when performing a step</param>
        /// <param name="iNumberofSteps">The total number of steps in the progress bar</param>
        /// <param name="strTitle">The title of the progress bar form</param>
        public ShowProgressEventArgs(int iStepSize, int iNumberofSteps, string strTitle)
            : this(iStepSize, iNumberofSteps)
        {
            m_strTitle = strTitle;
        }

        /// <summary>
        /// Constructor that sets the step size, the number of steps, and the title
        /// </summary>
        /// <param name="iStepSize">The number of steps taken when performing a step</param>
        /// <param name="iNumberofSteps">The total number of steps in the progress bar</param>
        /// <param name="strTitle">The title of the progress bar form</param>
        /// <param name="strStatus">The description of the current step</param>
        public ShowProgressEventArgs(int iStepSize, int iNumberofSteps, string strTitle, string strStatus)
            : this(iStepSize, iNumberofSteps, strTitle)
        {
            m_strStatus = strStatus;
        }

        /// <summary>
        /// Gets or sets the total number of steps
        /// </summary>
        public int NumberOfSteps
        {
            get
            {
                return m_iNumberOfSteps;
            }
            set
            {
                m_iNumberOfSteps = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of each step
        /// </summary>
        public int StepSize
        {
            get
            {
                return m_iStepSize;
            }
            set
            {
                m_iStepSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the string to use for the form title
        /// </summary>
        public string Title
        {
            get
            {
                return m_strTitle;
            }
            set
            {
                m_strTitle = value;
            }
        }
    }
}
