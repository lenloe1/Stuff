using System;
using C1.Win.C1Chart;
using Itron.Metering.DeviceDataTypes;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Class used for generating the chart object for the Toolbox
    /// </summary>
    public class ToolBoxChart
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="toolbox">The object to use for toolbox generation.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/07/08 RCG 9.50.04		Modified from ctrlItronToolboxSubControl

        public ToolBoxChart(Toolbox toolbox)
        {
            m_toolboxData = toolbox;
            m_ToolBoxChart = new C1Chart();
            m_PowerCircleChart = new C1Chart();

            ((System.ComponentModel.ISupportInitialize)(m_ToolBoxChart)).BeginInit();
            m_ToolBoxChart.BackColor = System.Drawing.Color.White;
            m_ToolBoxChart.ForeColor = System.Drawing.Color.White;
            m_ToolBoxChart.Location = new System.Drawing.Point(3, 3);
            m_ToolBoxChart.Name = "m_ToolBoxChart";
            m_ToolBoxChart.PropBag = ToolBoxChartResources.ToolBoxChartPropBag;
            m_ToolBoxChart.Size = new System.Drawing.Size(300, 300);
            ((System.ComponentModel.ISupportInitialize)(m_ToolBoxChart)).EndInit();

            ((System.ComponentModel.ISupportInitialize)(m_PowerCircleChart)).BeginInit();
            m_PowerCircleChart.BackColor = System.Drawing.Color.White;
            m_PowerCircleChart.ForeColor = System.Drawing.Color.White;
            m_PowerCircleChart.Location = new System.Drawing.Point(3, 3);
            m_PowerCircleChart.Name = "m_PowerCircleChart";
            m_PowerCircleChart.PropBag = ToolBoxChartResources.PowerCirclePropBag;
            m_PowerCircleChart.Size = new System.Drawing.Size(300, 300);
            ((System.ComponentModel.ISupportInitialize)(m_PowerCircleChart)).EndInit();

        }

        /// <summary>
        /// Generates the ToolBox chart
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/07/08 RCG 9.50.04		Created

        public void GenerateToolBox()
        {
            m_ToolBoxChart.Header.Text = "Toolbox";

            ChartDataSeriesCollection series = m_ToolBoxChart.ChartGroups[0].ChartData.SeriesList;

            //Create arrays for the lines, the second index value is zero 
            int[] x = new int[] { 0, 0 };
            float[] y0 = new float[] { 0.0f, 0.0f };
            float fMagnitudeA = 0.0f;
            float fMagnitudeB = 0.0f;
            float fMagnitudeC = 0.0f;

            //Determine the magnitude of the voltage vectors
            DetermineMagnitudes(ref fMagnitudeA, ref fMagnitudeB, ref fMagnitudeC, true);

            //Va
            x[0] = (int)m_toolboxData.m_fVAngleA;
            y0[0] = fMagnitudeA;
            ChartDataSeries s = series[0];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            //Vb
            x[0] = (int)m_toolboxData.m_fVAngleB;
            y0[0] = fMagnitudeB;
            s = series[1];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            //Vc
            x[0] = (int)m_toolboxData.m_fVAngleC;
            y0[0] = fMagnitudeC;
            s = series[2];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);


            //Determine the magnitude of the current vectors
            DetermineMagnitudes(ref fMagnitudeA, ref fMagnitudeB, ref fMagnitudeC, false);

            //Ia
            x[0] = (int)m_toolboxData.m_fIAngleA;
            y0[0] = fMagnitudeA;
            s = series[3];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            //Ib
            x[0] = (int)m_toolboxData.m_fIAngleB;
            y0[0] = fMagnitudeB;
            s = series[4];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            //Ic
            x[0] = (int)m_toolboxData.m_fIAngleC;
            y0[0] = fMagnitudeC;
            s = series[5];
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            //Setup the rotation
            SetupRotation();
        }

        /// <summary>
        /// Generates the Power Circle chart
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/07/08 RCG 9.50.04		Created

        public void GeneratePowerCircle()
        {
            m_PowerCircleChart.Header.Text = "Power Circle";
            m_PowerCircleChart.Footer.Visible = false;

            // Draw the Lines
            ChartDataSeriesCollection series = m_PowerCircleChart.ChartGroups[0].ChartData.SeriesList;
            ChartDataSeries s;
            int[] x = { 0, 0 };
            float[] y0 = { 0.0f, 0.0f };

            // Line for kW
            s = series[0];

            if (m_toolboxData.m_dInsKW > 0.0)
            {
                x[0] = 0;
            }
            else
            {
                x[0] = 180;
            }

            y0[0] = (float)Math.Abs(m_toolboxData.m_dInsKW);
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // Line for kVAR
            s = series[1];

            if (m_toolboxData.m_dInsKVar > 0.0)
            {
                x[0] = 90;
            }
            else
            {
                x[0] = 270;
            }

            y0[0] = (float)Math.Abs(m_toolboxData.m_dInsKVar);
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // Line for kVA
            s = series[2];
            x[0] = DetermineVAAngle();
            y0[0] = (float)Math.Abs(m_toolboxData.m_dInsKVA);
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // Draw Axes lines and label them to label the graph
            // Note that by setting the line lengths to 0 and setting the
            // offset we can keep the labels at a dynamic distance from the
            // center of the graph depending on the size of the application.

            // W Del
            s = series[3];
            s.DataLabel.Offset = 100;
            x[0] = 0;
            y0[0] = 0;
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // W Rec
            s = series[4];
            s.DataLabel.Offset = 100;
            x[0] = 180;
            y0[0] = 0;
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // var Rec
            s = series[5];
            s.DataLabel.Offset = 100;
            x[0] = 270;
            y0[0] = 0;
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);

            // var Del
            s = series[6];
            s.DataLabel.Offset = 100;
            x[0] = 90;
            y0[0] = 0;
            s.X.CopyDataIn(x);
            s.Y.CopyDataIn(y0);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the angle for the VA vector.
        /// </summary>
        /// <returns>The integer representation of the VA angle.</returns>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 05/02/07 ach 8.10    N/A    Added helper method.
        private int DetermineVAAngle()
        {
            int nAngle = 0;

            // Calculate the Angle
            // NOTE: here we have determined it is OK to compare 
            // the m_kW value to 0 since we want to avoid a divide by
            // 0 exception that could potentially be thrown when 
            // calculating the Angle.  If we decide against this the
            // logic here may need to be changed and this note should
            // be removed.
            if (0 == m_toolboxData.m_dInsKW)
            {
                nAngle = -90;
            }
            else
            {
                nAngle = (int)((-1) * Math.Atan(Math.Abs(m_toolboxData.m_dInsKVar / m_toolboxData.m_dInsKW)) * 180 / Math.PI);
            }

            // Determine if the Angle is in Left or Right Hemisphere
            if ((double)Math.Abs(m_toolboxData.m_dInsKW) != m_toolboxData.m_dInsKW)
            {
                nAngle = 180 - nAngle;
            }

            // Determine if the Angle is in the North or South Hemisphere
            if ((double)Math.Abs(m_toolboxData.m_dInsKVar) == m_toolboxData.m_dInsKVar)
            {
                nAngle = -1 * nAngle;
            }

            return nAngle;
        }

        /// <summary>
        /// Determines the magnitudes of the vectors for the graph
        /// </summary>
        /// <param name="fMagnitudeA">The magnitude for the A phase vector.</param>
        /// <param name="fMagnitudeB">The magnitude for the B phase vector.</param>
        /// <param name="fMagnitudeC">The magnitude for the C phase vector.</param>
        /// <param name="bDetermineVoltages">Boolean to indicate whether to determine
        /// magnitudes for voltages or currents.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  02/05/07 mrj 8.00.10		Created
        //
        private void DetermineMagnitudes(ref float fMagnitudeA,
                                  ref float fMagnitudeB,
                                  ref float fMagnitudeC,
                                  bool bDetermineVoltages)
        {
            if (bDetermineVoltages)
            {
                //Figure or the scale for the voltage vectors
                if (m_toolboxData.m_fVoltsA >= m_toolboxData.m_fVoltsB &&
                    m_toolboxData.m_fVoltsA >= m_toolboxData.m_fVoltsC)
                {
                    //A is the largest
                    fMagnitudeA = 20.0f;

                    if (m_toolboxData.m_fVoltsA != 0)
                    {
                        fMagnitudeB = (float)(m_toolboxData.m_fVoltsB / m_toolboxData.m_fVoltsA) * fMagnitudeA;
                        fMagnitudeC = (float)(m_toolboxData.m_fVoltsC / m_toolboxData.m_fVoltsA) * fMagnitudeA;
                    }
                }
                else if (m_toolboxData.m_fVoltsB >= m_toolboxData.m_fVoltsA &&
                         m_toolboxData.m_fVoltsB >= m_toolboxData.m_fVoltsC)
                {
                    //B is the largest				
                    fMagnitudeB = 20.0f;

                    if (m_toolboxData.m_fVoltsB != 0)
                    {
                        fMagnitudeA = (float)(m_toolboxData.m_fVoltsA / m_toolboxData.m_fVoltsB) * fMagnitudeB;
                        fMagnitudeC = (float)(m_toolboxData.m_fVoltsC / m_toolboxData.m_fVoltsB) * fMagnitudeB;
                    }
                }
                else if (m_toolboxData.m_fVoltsC >= m_toolboxData.m_fVoltsA &&
                         m_toolboxData.m_fVoltsC >= m_toolboxData.m_fVoltsB)
                {
                    //C is the largest				
                    fMagnitudeC = 20.0f;

                    if (m_toolboxData.m_fVoltsC != 0)
                    {
                        fMagnitudeA = (float)(m_toolboxData.m_fVoltsA / m_toolboxData.m_fVoltsC) * fMagnitudeC;
                        fMagnitudeB = (float)(m_toolboxData.m_fVoltsB / m_toolboxData.m_fVoltsC) * fMagnitudeC;
                    }
                }
            }
            else
            {
                //Figure or the scale for the current vectors
                if (m_toolboxData.m_fCurrentA >= m_toolboxData.m_fCurrentB &&
                    m_toolboxData.m_fCurrentA >= m_toolboxData.m_fCurrentC)
                {
                    //A is the largest
                    fMagnitudeA = 20.0f;

                    if (m_toolboxData.m_fCurrentA != 0)
                    {
                        fMagnitudeB = (float)(m_toolboxData.m_fCurrentB / m_toolboxData.m_fCurrentA) * fMagnitudeA;
                        fMagnitudeC = (float)(m_toolboxData.m_fCurrentC / m_toolboxData.m_fCurrentA) * fMagnitudeA;
                    }
                }
                else if (m_toolboxData.m_fCurrentB >= m_toolboxData.m_fCurrentA &&
                         m_toolboxData.m_fCurrentB >= m_toolboxData.m_fCurrentC)
                {
                    //B is the largest				
                    fMagnitudeB = 20.0f;

                    if (m_toolboxData.m_fCurrentB != 0)
                    {
                        fMagnitudeA = (float)(m_toolboxData.m_fCurrentA / m_toolboxData.m_fCurrentB) * fMagnitudeB;
                        fMagnitudeC = (float)(m_toolboxData.m_fCurrentC / m_toolboxData.m_fCurrentB) * fMagnitudeB;
                    }
                }
                else if (m_toolboxData.m_fCurrentC >= m_toolboxData.m_fCurrentA &&
                         m_toolboxData.m_fCurrentC >= m_toolboxData.m_fCurrentB)
                {
                    //C is the largest				
                    fMagnitudeC = 20.0f;

                    if (m_toolboxData.m_fCurrentC != 0)
                    {
                        fMagnitudeA = (float)(m_toolboxData.m_fCurrentA / m_toolboxData.m_fCurrentC) * fMagnitudeC;
                        fMagnitudeB = (float)(m_toolboxData.m_fCurrentB / m_toolboxData.m_fCurrentC) * fMagnitudeC;
                    }
                }
            }

            //The control has a problem when the magnitude is zero
            if (0.0f == fMagnitudeA)
            {
                fMagnitudeA = 0.1f;
            }
            if (0.0f == fMagnitudeB)
            {
                fMagnitudeB = 0.1f;
            }
            if (0.0f == fMagnitudeC)
            {
                fMagnitudeC = 0.1f;
            }
        }

        /// <summary>
        ///	This method sets up the meter's rotation 
        /// </summary>

        private void SetupRotation()
        {
            double dVoltDelta;


            dVoltDelta = Math.Abs(m_toolboxData.m_fVoltsC - m_toolboxData.m_fVoltsA);

            if ((dVoltDelta > 165.0) &&
                (dVoltDelta < 195.0) &&
                (m_toolboxData.m_fVAngleB == 0.0))
            {
                m_ToolBoxChart.ChartArea.AxisX.Text = "Rotation: Unknown";
            }
            else if ((m_toolboxData.m_fVAngleC >= 0.0) &&
                     (m_toolboxData.m_fVAngleC < 180.0))
            {
                m_ToolBoxChart.ChartArea.AxisX.Text = "Rotation: CBA";
            }
            else
            {
                m_ToolBoxChart.ChartArea.AxisX.Text = "Rotation: ABC";
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the generated ToolBox Chart object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/07/08 RCG 9.50.04		Created

        public C1Chart ChartToolBox
        {
            get
            {
                return m_ToolBoxChart;
            }
        }

        /// <summary>
        /// Gets the generated Power Circle Chart object
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  08/07/08 RCG 9.50.04		Created

        public C1Chart ChartPowerCircle
        {
            get
            {
                return m_PowerCircleChart;
            }
        }

        #endregion

        #region Member Variables

        private Toolbox m_toolboxData;
        private C1Chart m_ToolBoxChart;
        private C1Chart m_PowerCircleChart;

        #endregion
    }
}
