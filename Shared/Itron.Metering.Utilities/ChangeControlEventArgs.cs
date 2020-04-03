using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// The delegate for the change control event handler.
    /// </summary>
    /// <param name="sender">The control that sent the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ChangeControlEventHandler(object sender, ChangeControlEventArgs e);

    /// <summary>
    /// The event arguments for the ChangeControlEvent
    /// </summary>
    public class ChangeControlEventArgs : EventArgs
    {
        /// <summary>
        /// Member variable for the new control
        /// </summary>
        private Control m_NewControl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctlNewControl">The control to change to.</param>
        public ChangeControlEventArgs(Control ctlNewControl)
        {
            m_NewControl = ctlNewControl;
        }

        /// <summary>
        /// Gets the control to change to.
        /// </summary>
        public Control NewControl
        {
            get
            {
                return m_NewControl;
            }
        }
    }
}
