using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Internal.ClientProxy;
using Itron.Ami.Facade.WebServices.Reporting.V200908.ClientProxy; 

namespace Itron.Ami.CEWebServiceClient.Internal
{
    /// <summary>
    /// This class handles the webservice call to download firmware from the collection engine.
    /// </summary>
    public class FirmwareDownload : IDisposable
    {
        #region Definitions

        /// <summary>
        /// Enumeration for specifying the different firmware download jobs.
        /// </summary>
        public enum FWDL_JOB_TYPE
        {
            Enter = 0,
            Download,
            Transfer,
            Activate,
            Exit, 
            ChangePhase,
        }

        #endregion

        #region Internal Methods

        internal FirmwareDownload(int endpointGroupKey, int firmwareKey)
        {
            this.m_EndpointGroupKey = new EndpointGroupKey();
            this.m_EndpointGroupKey.value = endpointGroupKey;

            this.m_FirmwareKey = new FirmwareKey();
            this.m_FirmwareKey.value = firmwareKey;

            //Keep record of when job was created ~ needed for reports.
            this.m_FWDLBegin = DateTime.Now;

            this.m_Phase = FirmwareDownloadPhase.Initial;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Property to get the type of firmware job this class represents.
        /// </summary>
        internal FWDL_JOB_TYPE JobType
        {
            get { return m_JobType; }
            set { m_JobType = value; }
        }

        /// <summary>
        /// Property to get the phase this firmware job is being sent in.
        /// </summary>
        internal FirmwareDownloadPhase Phase
        {
            get { return m_Phase; }
            set { m_Phase = value; }
        }

        /// <summary>
        /// Property to get the current firmware job being exercised.
        /// </summary>
        internal InternalFwdlAction CurrentFWDLJob
        {
            get { return m_CurrentFwdlJob; }
            set { m_CurrentFwdlJob = value; }
        }

        
        /// <summary>
        /// Property to get the time the firmware download began.
        /// </summary>
        internal DateTime FWDLBegin
        {
            get { return m_FWDLBegin; }
        }

        /// <summary>
        /// Property to get the event that will stop the firmware download thread.
        /// </summary>
        internal ManualResetEvent EventStopFWDLThread
        {
            get { return m_EventStopFWDLThread; }
        }

        /// <summary>
        /// Property to get the event signaling that the firmware download thread has stopped.
        /// </summary>
        internal ManualResetEvent EventFWDLThreadStopped
        {
            get { return m_EventFWDLThreadStopped; }
        }

        /// <summary>
        /// Property to get/set the job key for this firmware download.
        /// </summary>
        internal FirmwareJobKey FirmwareJobKey
        {
            get { return m_FirmwareJobKey; }
            set { m_FirmwareJobKey = value; }
        }
               
        /// <summary>
        /// Property to get the key associated with the config group that firmware is downloaded to.
        /// </summary>
        internal EndpointGroupKey EndpointGroupKey
        {
            get { return m_EndpointGroupKey; }
        }

        /// <summary>
        /// Property to get the key associated with the firmware that is being downloaded.
        /// </summary>
        internal FirmwareKey FirmwareKey
        {
            get { return m_FirmwareKey; }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Property to get the firmware download enter job.
        /// </summary>
        public FirmwareDownloadEnter Enter
        {
            get 
            {
                if (null == this.m_Enter)
                    this.m_Enter = new FirmwareDownloadEnter(this);
                return m_Enter; 
            }
        }

        /// <summary>
        /// Property to get the firmware download download job.
        /// </summary>
        public FirmwareDownloadDownload Download
        {
            get
            {
                if (null == this.m_Download)
                    this.m_Download = new FirmwareDownloadDownload(this);
                return m_Download;
            }
        }

        
        public FirmwareDownloadTransfer Transfer
        {
            get
            {
                if (null == this.m_Transfer)
                    this.m_Transfer = new FirmwareDownloadTransfer(this);
                return m_Transfer;
            }
        }

        private FirmwareDownloadActivate _activate;
        public FirmwareDownloadActivate Activate
        {
            get
            {
                if (null == this._activate)
                    this._activate = new FirmwareDownloadActivate(this);
                return _activate;
            }
        }

        private FirmwareDownloadExit _exit;
        public FirmwareDownloadExit Exit
        {
            get
            {
                if (null == this._exit)
                    this._exit = new FirmwareDownloadExit(this);
                return _exit;
            }
        }

        #endregion

        private FirmwareDownloadChangePhase _changePhase;
        internal InternalFwdlAction SwitchPhase
        {
            get
            {
                if (null == this._changePhase)
                    this._changePhase = new FirmwareDownloadChangePhase(this);
                return (InternalFwdlAction)_changePhase;
            }
        }


        ~FirmwareDownload()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (!m_blnDisposed)
            {
                AbortCESimulator();
                m_blnDisposed = true;
            }
        }

        public Result ChangePhase(FirmwareDownloadPhase Phase)
        {
            m_Phase = Phase;
            SwitchPhase.Execute();

            return SwitchPhase.JobResult;
            
        }

        public Result ActivateFirmware(DateTime dtActivationTime)
        {
            Activate.ActivationTime = dtActivationTime;
            Activate.Execute();

            return Activate.JobResult;
        }

        public int GetEndpointsCompletedForCurrentJob()
        {
            int iEndpointsComplete = 0;
            //TODO: Request that firmware download API accept callback parameter then wouldn't have to request a report and poll.
            Client<ReportServiceClient> c = null;
            c = InternalClientFactory.ConstructReportServiceClient(InternalLib.Endpoint.ReportEndpointName, InternalLib.Endpoint.ExternalUserName, InternalLib.Endpoint.ExternalUserPassword);
            FirmwareDownloadsReportInput fwdlReportInput = new FirmwareDownloadsReportInput();

            //Request all fwdl jobs between the begin
            fwdlReportInput.StartTime = FWDLBegin;
            fwdlReportInput.StopTime = DateTime.Now;
            FirmwareDownloadsReport _fwdlsRunningReport = c.ServiceClient.GetFirmwareDownloadsReport(fwdlReportInput);
            
            c.ServiceClient.Close();


            //TODO: FirmwareDownloadItems[0].Status is an EnumJobStatus type that is not exposed in web services, thus test status against hard string.  Request EnumJobStatus be exposed.
            if (null != _fwdlsRunningReport)
            {
                if (0 < _fwdlsRunningReport.FirmwareDownloadItems.Length)
                {
                    //firmware download running
                    foreach (FirmwareDownloadItem fwdlItem in _fwdlsRunningReport.FirmwareDownloadItems)
                    {
                        if (fwdlItem.JobId == CurrentFWDLJob.JobKey.value.ToString())
                        {
                            iEndpointsComplete = fwdlItem.CountOfEndpointsComplete;
                            break;
                        }
                    }
                }

            }

            return iEndpointsComplete;
        }

        public void CancelCurrentJob()
        {
            CurrentFWDLJob.Cancel();
        }

        internal void AbortCESimulator()
        {
            if (null != this.m_ThreadFwdlCESim && this.m_ThreadFwdlCESim.IsAlive)
            {
                // set event "Stop"
                this.m_EventStopFWDLThread.Set();

                // wait when thread  will stop or finish
                while (m_ThreadFwdlCESim.IsAlive)
                {
                    if (WaitHandle.WaitAll(
                        (new ManualResetEvent[] { this.m_EventFWDLThreadStopped }), 100))
                    {
                        break;
                    }
                }
            }
        }

        internal void StartCESimulator(InternalFwdlAction fwdlJob)
        {
            this.m_CurrentFwdlJob = fwdlJob;
            m_ThreadFwdlCESim = new Thread(new ThreadStart(this.FWDLWorkerThreadFunction));
            m_ThreadFwdlCESim.Name = "FWDL CE Simulator Thread";
            m_ThreadFwdlCESim.Start();
        }

        private void FWDLWorkerThreadFunction()
        {
            this.m_EventFWDLThreadStopped = new ManualResetEvent(false);
            this.m_EventStopFWDLThread = new ManualResetEvent(false);

            this.m_EventFWDLThreadStopped.Reset();
            this.m_EventStopFWDLThread.Reset();

            FwdlCESim CESim = new FwdlCESim(this);
            CESim.Run();
        }


        #region Members

        private Thread m_ThreadFwdlCESim = null;
        private bool m_blnDisposed = false;
        private FWDL_JOB_TYPE m_JobType;
        private FirmwareDownloadPhase m_Phase;
        private InternalFwdlAction m_CurrentFwdlJob;
        private DateTime m_FWDLBegin;
        private ManualResetEvent m_EventStopFWDLThread;
        private ManualResetEvent m_EventFWDLThreadStopped;
        private FirmwareJobKey m_FirmwareJobKey;
        private EndpointGroupKey m_EndpointGroupKey;
        private FirmwareKey m_FirmwareKey;
        private FirmwareDownloadEnter m_Enter;
        private FirmwareDownloadDownload m_Download;
        private FirmwareDownloadTransfer m_Transfer;

        #endregion

    }

    internal class FwdlCESim
    {
        private FirmwareDownloadsReport _fwdlsRunningReport;
        private FirmwareDownload _fwdl;

        internal FwdlCESim(FirmwareDownload fwdl)
        {
            _fwdl = fwdl;
        }

        internal void Run()
        {
            this._fwdlsRunningReport = null;
            Result result = null;

            while ((null == result) && !_fwdl.EventStopFWDLThread.WaitOne(3000, true))
            {
                GetFirmwareDownloadsReport();

                //TODO: FirmwareDownloadItems[0].Status is an EnumJobStatus type that is not exposed in web services, thus test status against hard string.  Request EnumJobStatus be exposed.
                if (null != _fwdlsRunningReport)
                {
                    if (0 < _fwdlsRunningReport.FirmwareDownloadItems.Length)
                    {
                        //firmware download running
                        foreach (FirmwareDownloadItem fwdlItem in _fwdlsRunningReport.FirmwareDownloadItems)
                        {
                            if ((fwdlItem.JobId == this._fwdl.CurrentFWDLJob.JobKey.value.ToString()) &&
                                (fwdlItem.Status != "Running" && fwdlItem.Status != "Initializing"))
                            {
                                result = new Result();

                                if ("Successful" == _fwdlsRunningReport.FirmwareDownloadItems[0].Status)
                                {
                                    result.ActionResultType = EnumResultCodeType.SUCCESS;
                                }
                                else
                                {
                                    result.ActionResultType = EnumResultCodeType.FAILURE;
                                }

                                InternalLib.Listener.InternalAPIController.DataArrived(this._fwdl.CurrentFWDLJob.UniqueID, result, this._fwdl.CurrentFWDLJob.JobKey.value);
                                break;
                            }
                        }
                    }

                }
            }

            _fwdl.EventFWDLThreadStopped.Set();
        }

        private void GetFirmwareDownloadsReport()
        {
            //TODO: Request that firmware download API accept callback parameter then wouldn't have to request a report and poll.
            Client<ReportServiceClient> c = null;
            c = InternalClientFactory.ConstructReportServiceClient(InternalLib.Endpoint.ReportEndpointName, InternalLib.Endpoint.ExternalUserName, InternalLib.Endpoint.ExternalUserPassword);
            FirmwareDownloadsReportInput fwdlReportInput = new FirmwareDownloadsReportInput();

            //Request all fwdl jobs between the begin
            fwdlReportInput.StartTime = _fwdl.FWDLBegin;
            fwdlReportInput.StopTime = DateTime.Now;
            this._fwdlsRunningReport = c.ServiceClient.GetFirmwareDownloadsReport(fwdlReportInput);
            c.ServiceClient.Close();
        }
    }

    public class FirmwareDownloadEnter : InternalFwdlAction
    {
        internal FirmwareDownloadEnter(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.Enter;
        }
    }

    public class FirmwareDownloadDownload : InternalFwdlAction
    {
        internal FirmwareDownloadDownload(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.Download;
        }
    }

    public class FirmwareDownloadTransfer : InternalFwdlAction
    {
        internal FirmwareDownloadTransfer(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.Transfer;
        }
    }

    public class FirmwareDownloadActivate : InternalFwdlAction
    {
        internal FirmwareDownloadActivate(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.Activate;
            _dtActivationTime = DateTime.UtcNow;
        }

        private DateTime _dtActivationTime;
        internal DateTime ActivationTime
        {
            get
            {
                return _dtActivationTime;
            }
            set
            {
                _dtActivationTime = value;
            }
        }
    }

    public class FirmwareDownloadExit : InternalFwdlAction
    {
        internal FirmwareDownloadExit(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.Exit;
        }
    }

    public class FirmwareDownloadChangePhase : InternalFwdlAction
    {
        internal FirmwareDownloadChangePhase(FirmwareDownload fwdlDaddy)
            : base(fwdlDaddy)
        {
            _fwdlDaddy.JobType = FirmwareDownload.FWDL_JOB_TYPE.ChangePhase;
        }

        public override void Cancel()
        {
            throw new NotSupportedException("The Change Firmware Download Phase Webservice cannot be canceled!");
        }

        public override InternalAPICallbackItem Execute(TimeSpan syncWaitTime)
        {
            throw new NotSupportedException("The Change Firmware Download Phase Webservice does not support callbacks!");
        }
    }


    public abstract class InternalFwdlAction : InternalCEAction
    {
        Client<InternalAPIManagerServiceClient> _client = null;

        protected FirmwareDownload _fwdlDaddy;
        internal FirmwareDownload FwdlDaddy
        {
            get { return _fwdlDaddy; }
        }

        protected JobKey _jobKey;
        internal JobKey JobKey
        {
            get { return _jobKey; }
        }

        protected Result _jobResult;
        public Result JobResult
        {
            get { return _jobResult; }
        }

        internal InternalFwdlAction(FirmwareDownload fwdlDaddy)
            : base()
        {
            _fwdlDaddy = fwdlDaddy;
        }

        public override void Cancel()
        {
            if (null != this._jobKey)
            {
                Client<InternalAPIManagerServiceClient> c = null;
                c = InternalClientFactory.ConstructInternalAPIManagerServiceClient(InternalLib.Endpoint.InternalEndpointName, InternalLib.Endpoint.InternalUserName, InternalLib.Endpoint.InternalUserPassword);
                c.ServiceClient.IInternalAPIManagerServiceCancelJob(this._jobKey);
                c.ServiceClient.Close();
            }
        }

        internal override void Invoke()
        {
            

            if (string.IsNullOrEmpty(UniqueID))
            {
                _jobResult = null;
                this._client = InternalClientFactory.ConstructInternalAPIManagerServiceClient(InternalLib.Endpoint.InternalEndpointName, InternalLib.Endpoint.InternalUserName, InternalLib.Endpoint.InternalUserPassword);

                if (FirmwareDownload.FWDL_JOB_TYPE.Enter == _fwdlDaddy.JobType)
                {
                    FirmwareJobKey fwjobKey;

                    // Restart the CE simulator to look for this job
                    RestartCESimulator(this);

                    _jobResult = this._client.ServiceClient.StartFirmwareDownload(out fwjobKey, out _jobKey, _fwdlDaddy.EndpointGroupKey, _fwdlDaddy.FirmwareKey);
                    _fwdlDaddy.FirmwareJobKey = fwjobKey;
                }
                else if (FirmwareDownload.FWDL_JOB_TYPE.Download == _fwdlDaddy.JobType)
                {
                    // Restart the CE simulator to look for this job
                    RestartCESimulator(this);

                    _jobResult = this._client.ServiceClient.FirmwareDownload(out _jobKey, _fwdlDaddy.FirmwareJobKey);
                }
                else if (FirmwareDownload.FWDL_JOB_TYPE.Transfer == _fwdlDaddy.JobType)
                {
                    // Restart the CE simulator to look for this job
                    RestartCESimulator(this);

                    //Transfer immediately
                    _jobResult = this._client.ServiceClient.TransferHanFirmware(out _jobKey, _fwdlDaddy.FirmwareJobKey, DateTime.UtcNow, true);
                }
                else if (FirmwareDownload.FWDL_JOB_TYPE.Activate == _fwdlDaddy.JobType)
                {
                    // Restart the CE simulator to look for this job
                    RestartCESimulator(this);

                    //Activate immediately
                    _jobResult = this._client.ServiceClient.FirmwareActivate(out _jobKey, _fwdlDaddy.FirmwareJobKey, ((FirmwareDownloadActivate)this).ActivationTime , true);
                }
                else if (FirmwareDownload.FWDL_JOB_TYPE.Exit == _fwdlDaddy.JobType)
                {
                    // Restart the CE simulator to look for this job
                    RestartCESimulator(this);

                    _jobResult = this._client.ServiceClient.CancelFirmwareDownload(out _jobKey, _fwdlDaddy.FirmwareJobKey);
                }
                else if (FirmwareDownload.FWDL_JOB_TYPE.ChangePhase == _fwdlDaddy.JobType)
                {
                    if (null != _fwdlDaddy.FirmwareJobKey)
                    {
                        _jobResult = this._client.ServiceClient.ChangeFirmwareDownloadPhase(_fwdlDaddy.FirmwareJobKey, _fwdlDaddy.Phase);
                        _jobKey = new JobKey();
                        _jobKey.value = 0;
                    }
                    else
                    {
                        _jobResult = new Result();
                        _jobKey = new JobKey();
                        _jobKey.value = 0;

                        if (FirmwareDownloadPhase.Initial == _fwdlDaddy.Phase)
                        {
                            JobResult.ActionResultType = EnumResultCodeType.SUCCESS;
                        }
                        else
                        {
                            JobResult.ActionResultType = EnumResultCodeType.FAILURE;
                        }
                    }
                }

                this._client.ServiceClient.Close();
                _uniqueID = _jobKey.value.ToString();
            }
        }

        internal override void Abort()
        {
            if ((null != this._client) && (null != this._client.ServiceClient) &&
                (this._client.ServiceClient.State != CommunicationState.Closed))
            {
                _client.ServiceClient.Abort();
                this._aborted = true;
            }
        }

        private void RestartCESimulator(InternalFwdlAction fwdlJob)
        {
            //Kill previous job's existing CE simulator.
            _fwdlDaddy.AbortCESimulator();

            //Start a CE simulator to support a callback for this job
            _fwdlDaddy.StartCESimulator(fwdlJob);
        }
    }
}
