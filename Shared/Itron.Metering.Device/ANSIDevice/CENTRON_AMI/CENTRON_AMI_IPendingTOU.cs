using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Itron.Common.C1219Tables.ANSIStandard;
using Itron.Common.C1219Tables.Centron;
using Itron.Metering.Communications.PSEM;
using Itron.Metering.Progressable;

namespace Itron.Metering.Device
{
    public partial class CENTRON_AMI : IPendingTOU
    {
        #region Definitions
        private const ushort MFG_CONFIG_TABLE = 2048;
        private const ushort TOU_MFG_TABLE = 2090;

        #endregion

        #region Public Methods
        /// <summary>
        /// Reconfigures TOU using the specified EDL file
        /// </summary>
        /// <param name="strFileName">The EDL file that contains the TOU data.</param>
        /// <param name="iSeasonIndex">The number of seasons from the current season to write</param>
        /// <returns>TOUReconfigResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 10/24/06 RCG 7.40.00 N/A    Created

        public WritePendingTOUResult WritePendingTOU(string strFileName, int iSeasonIndex)
        {
            WritePendingTOUResult Result = WritePendingTOUResult.SUCCESS;
            CentronTables DeviceTables = new CentronTables();
            TableData[] PSEMData = null;

            DateTime dtSeasonStart = new DateTime();
            DateTime dtNextSeasonStart = new DateTime();
            bool bDemandReset = false;
            bool bSelfRead = false;

            // Load the file into the tables
            if (File.Exists(strFileName) == true)
            {
                try
                {
                    OnStepProgress(new ProgressEventArgs("Loading EDL File..."));

                    DeviceTables.LoadEDLFile(XmlReader.Create(strFileName));

                    OnStepProgress(new ProgressEventArgs("Selecting Season..."));

                    DeviceTables.UpdateTOUSeasonFromStandardTables(DateTime.Now, iSeasonIndex, 
                        out dtSeasonStart, out bDemandReset, out bSelfRead, out dtNextSeasonStart);
                }
                catch (Exception)
                {
                    Result = WritePendingTOUResult.INVALID_EDL_FILE;
                }
            }
            else
            {
                Result = WritePendingTOUResult.FILE_NOT_FOUND;
            }

            if (Result == WritePendingTOUResult.SUCCESS)
            {

                OnStepProgress(new ProgressEventArgs("Building Pending Table..."));

                // Build the streams for the pending table
                PSEMData = DeviceTables.BuildPSEMStreams(TOU_MFG_TABLE);

                OnStepProgress(new ProgressEventArgs("Checking for Available Pending Buffers..."));

                // Check to make sure there are enough Pending Table Buffers available
                if (Table04.NumberPendingTables + PSEMData.Length > Table04.NumberPendingSupported)
                {
                    Result = WritePendingTOUResult.PENDING_BUFFERS_FULL;
                }
            }

            if (Result == WritePendingTOUResult.SUCCESS)
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;

                // Write the TOU to the meter
                foreach (TableData DataBlock in PSEMData)
                {
                    OnStepProgress(new ProgressEventArgs("Writing Pending Table..."));

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        MemoryStream PendingHeader = new MemoryStream();

                        if(PSEMData[0] == DataBlock)
                        {
                            // This is the first stream so set the Demand Reset and Self Read
                            DeviceTables.BuildPendingHeader(PendingHeader, dtSeasonStart, bSelfRead, bDemandReset);
                        }
                        else
                        {
                            // Ignore the Self Read and Demand Reset since it was already set in the
                            // first stream.
                            DeviceTables.BuildPendingHeader(PendingHeader, dtSeasonStart, false, false);
                        }

                        DataBlock.AddPendingHeader(PendingHeader);

                        if (DataBlock.FullTable == true)
                        {
                            PSEMResult = m_PSEM.FullWrite(DataBlock.TableID, DataBlock.PSEM.ToArray());

                        }
                        else
                        {
                            PSEMResult = m_PSEM.OffsetWrite(DataBlock.TableID, (int)DataBlock.Offset,
                                                            DataBlock.PSEM.ToArray());
                        }                       
                    }
                }

                OnStepProgress(new ProgressEventArgs());

                // Check the PSEM Result
                if (PSEMResult == PSEMResponse.Isc)
                {
                    Result = WritePendingTOUResult.INSUFFICIENT_SECURITY_ERROR;
                }
                else if (PSEMResult != PSEMResponse.Ok)
                {
                    Result = WritePendingTOUResult.PROTOCOL_ERROR;
                }

                // Clear the TOU information if we changed the current season.
                if (iSeasonIndex == 0)
                {
                    m_TOUSchedule = null;
                    Table2048.TOUConfig.State = AnsiTable.TableState.Unloaded;
                    Table2048.CalendarConfig.State = AnsiTable.TableState.Unloaded;
                    Table2048.BillingSchedConfig.State = AnsiTable.TableState.Unloaded;
                    m_TOUExpireDate.Flush();
                    m_DayOfTheWeek.Flush();
                    m_uiNumTOURates.Flush();
                    m_MeterInDST.Flush();
                }
            }

            return Result;
        }

        /// <summary>
        /// Reconfigures TOU using the specified EDL file - USED IN TEST APPLICATIONS ONLY!
        /// </summary>
        /// <param name="strFileName">The EDL file that contains the TOU data.</param>
        /// <param name="iSeasonIndex">The number of seasons from the current season to write</param>
        /// <param name="MeterTime">This it the Current Meter Time so we are sure to pick the correct season</param>
        /// <returns>TOUReconfigResult code.</returns>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- --------- ------ ---------------------------------------
        // 03/18/09 KRC OW2.10.XX  N/A    Created - For Teseting

        public WritePendingTOUResult WritePendingTOU(string strFileName, int iSeasonIndex, DateTime MeterTime)
        {
            WritePendingTOUResult Result = WritePendingTOUResult.SUCCESS;
            CentronTables DeviceTables = new CentronTables();
            TableData[] PSEMData = null;

            DateTime dtSeasonStart = new DateTime();
            DateTime dtNextSeasonStart = new DateTime();
            bool bDemandReset = false;
            bool bSelfRead = false;

            // Load the file into the tables
            if (File.Exists(strFileName) == true)
            {
                try
                {
                    OnStepProgress(new ProgressEventArgs("Loading EDL File..."));

                    DeviceTables.LoadEDLFile(XmlReader.Create(strFileName));

                    OnStepProgress(new ProgressEventArgs("Selecting Season..."));

                    DeviceTables.UpdateTOUSeasonFromStandardTables(MeterTime, iSeasonIndex,
                        out dtSeasonStart, out bDemandReset, out bSelfRead, out dtNextSeasonStart);
                }
                catch (Exception)
                {
                    Result = WritePendingTOUResult.INVALID_EDL_FILE;
                }
            }
            else
            {
                Result = WritePendingTOUResult.FILE_NOT_FOUND;
            }

            if (Result == WritePendingTOUResult.SUCCESS)
            {

                OnStepProgress(new ProgressEventArgs("Building Pending Table..."));

                // Build the streams for the pending table
                PSEMData = DeviceTables.BuildPSEMStreams(TOU_MFG_TABLE);

                OnStepProgress(new ProgressEventArgs("Checking for Available Pending Buffers..."));

                // Check to make sure there are enough Pending Table Buffers available
                if (Table04.NumberPendingTables + PSEMData.Length > Table04.NumberPendingSupported)
                {
                    Result = WritePendingTOUResult.PENDING_BUFFERS_FULL;
                }
            }

            if (Result == WritePendingTOUResult.SUCCESS)
            {
                PSEMResponse PSEMResult = PSEMResponse.Ok;

                // Write the TOU to the meter
                foreach (TableData DataBlock in PSEMData)
                {
                    OnStepProgress(new ProgressEventArgs("Writing Pending Table..."));

                    if (PSEMResult == PSEMResponse.Ok)
                    {
                        MemoryStream PendingHeader = new MemoryStream();

                        if (PSEMData[0] == DataBlock)
                        {
                            // This is the first stream so set the Demand Reset and Self Read
                            DeviceTables.BuildPendingHeader(PendingHeader, dtSeasonStart, bSelfRead, bDemandReset);
                        }
                        else
                        {
                            // Ignore the Self Read and Demand Reset since it was already set in the
                            // first stream.
                            DeviceTables.BuildPendingHeader(PendingHeader, dtSeasonStart, false, false);
                        }

                        DataBlock.AddPendingHeader(PendingHeader);

                        if (DataBlock.FullTable == true)
                        {
                            PSEMResult = m_PSEM.FullWrite(DataBlock.TableID, DataBlock.PSEM.ToArray());

                        }
                        else
                        {
                            PSEMResult = m_PSEM.OffsetWrite(DataBlock.TableID, (int)DataBlock.Offset,
                                                            DataBlock.PSEM.ToArray());
                        }
                    }
                }

                OnStepProgress(new ProgressEventArgs());

                // Check the PSEM Result
                if (PSEMResult == PSEMResponse.Isc)
                {
                    Result = WritePendingTOUResult.INSUFFICIENT_SECURITY_ERROR;
                }
                else if (PSEMResult != PSEMResponse.Ok)
                {
                    Result = WritePendingTOUResult.PROTOCOL_ERROR;
                }

                // Clear the TOU information if we changed the current season.
                if (iSeasonIndex == 0)
                {
                    m_TOUSchedule = null;
                    Table2048.TOUConfig.State = AnsiTable.TableState.Unloaded;
                    Table2048.CalendarConfig.State = AnsiTable.TableState.Unloaded;
                    Table2048.BillingSchedConfig.State = AnsiTable.TableState.Unloaded;
                    m_TOUExpireDate.Flush();
                    m_DayOfTheWeek.Flush();
                    m_uiNumTOURates.Flush();
                    m_MeterInDST.Flush();
                }
            }

            return Result;
        }

        #endregion

    }
}
