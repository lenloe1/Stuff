using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// This class overrides The TableLayoutPanel so that Double Bufferring can be enabled
    /// </summary>
    public partial class DoubleBufferedPanel : UserControl
    {
        /// <summary>
        /// Default Constructor. Enables Double Buffereing
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/28/06 RCG	7.35.00			Created
        public DoubleBufferedPanel()
        {
            InitializeComponent();
            base.DoubleBuffered = true;
        }
    }
}
