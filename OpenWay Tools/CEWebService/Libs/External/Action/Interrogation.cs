using System;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Itron.Ami.Facade.WebServices.Data.V200810.ClientProxy;
using Itron.Ami.CEWebServiceClient.Base;
using CommonClientProxy = Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public class Interrogation : ExternalAction
    {   
        public enum TargetType
        {
            MeterESNs = 0,
            GroupName
        }

        private Client<DataServiceClient> _client = null; 
        private InterrogateEndpointsRequest _epRequest;
        private InterrogateGroupRequest _grpRequest;
        private RequestToken _rqstToken = null;

        internal Interrogation(
            TargetType targetType,
            string[] targets,
            DateTime readingStartTime,
            DateTime readingEndTime,
            DateTime interrogationWindowStartTime,
            DateTime interrogationWindowEndTime,
            bool performAndRetrieveSelfRead,
            bool performDemandReset,
            bool retrieveHomeNetworkData,
            bool retrieveInstantaneousData,
            bool retrieveLastDemandReset,
            bool retrieveLoadProfileData,
            bool retrieveLogEvents,
            bool retrieveNetworkStatistics,
            bool retrievePriorSelfRead,
            bool retrieveVoltageMonitorData)
        {
            this.CreateRequest(
                targetType, 
                targets,
                readingStartTime,
                readingEndTime,
                interrogationWindowStartTime,
                interrogationWindowEndTime,
                performAndRetrieveSelfRead,
                performDemandReset,
                retrieveHomeNetworkData,
                retrieveInstantaneousData,
                retrieveLastDemandReset,
                retrieveLoadProfileData,
                retrieveLogEvents,
                retrieveNetworkStatistics,
                retrievePriorSelfRead,
                retrieveVoltageMonitorData);
        }

        private void CreateRequest(TargetType targetType,
            string[] target,
            DateTime readingStartTime,
            DateTime readingEndTime,
            DateTime interrogationWindowStartTime,
            DateTime interrogationWindowEndTime,
            bool performAndRetrieveSelfRead,
            bool performDemandReset,
            bool retrieveHomeNetworkData,
            bool retrieveInstantaneousData,
            bool retrieveLastDemandReset,
            bool retrieveLoadProfileData,
            bool retrieveLogEvents,
            bool retrieveNetworkStatistics,
            bool retrievePriorSelfRead,
            bool retrieveVoltageMonitorData)
        {

            RequestDetails requestDetails = null; 
            
            if (TargetType.GroupName == targetType)
            {
                this._epRequest = null;
                this._grpRequest = new InterrogateGroupRequest();
                this._grpRequest.Target = target;
                requestDetails = (RequestDetails) this._grpRequest;
            }
            else
            {
                this._grpRequest = null;
                this._epRequest = new InterrogateEndpointsRequest();
                this._epRequest.Target = target;
                requestDetails = (RequestDetails)this._epRequest;
            }

            requestDetails.RequestStatusChanged = ExternalLib.Listener.RequestStatusChangedController.HttpUri;
            requestDetails.ReadingStartTime = readingStartTime;
            requestDetails.ReadingEndTime = readingEndTime;
            requestDetails.InterrogationWindowStartTime = interrogationWindowStartTime;
            requestDetails.InterrogationWindowEndTime = interrogationWindowEndTime;
            requestDetails.PerformAndRetrieveSelfRead = performAndRetrieveSelfRead;
            requestDetails.PerformDemandReset = performDemandReset;
            requestDetails.RetrieveHomeNetworkData = retrieveHomeNetworkData;
            requestDetails.RetrieveInstantaneousData = retrieveInstantaneousData;
            requestDetails.RetrieveLastDemandReset = retrieveLastDemandReset;
            requestDetails.RetrieveLoadProfileData = retrieveLoadProfileData;
            requestDetails.RetrieveLogEvents = retrieveLogEvents;
            requestDetails.RetrieveNetworkStatistics = retrieveNetworkStatistics;
            requestDetails.RetrievePriorSelfRead = retrievePriorSelfRead;
            requestDetails.RetrieveVoltageMonitorData = retrieveVoltageMonitorData;
        }

        internal override void Invoke()
        {
            this._client = ExternalClientFactory.ConstructDataServiceClient(ExternalLib.Endpoint.DataEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);

            if (null != this._grpRequest)
            {
                this._rqstToken = this._client.ServiceClient.InterrogateByGroup(this._grpRequest.InterrogateByGroupRequest);
            }
            else
            {
                this._rqstToken = this._client.ServiceClient.InterrogateByEndpoints(this._epRequest.InterrogateByEndpointsRequest);
            }

            this._client.ServiceClient.Close();

            this._uniqueID = this._rqstToken.Id.ToString();
        }

        
        internal override void Abort()
        {
            if ((null != this._client) &&
                (null != this._client.ServiceClient) &&
                (this._client.ServiceClient.State != CommunicationState.Closed))
            {
                this._aborted = true;
                _client.ServiceClient.Abort();
            }
        }

        internal class InterrogateGroupRequest : RequestDetails
        {
            //Wrap the internal web service class
            private InterrogateByGroupRequest _grpRequest = new InterrogateByGroupRequest();

            internal InterrogateGroupRequest()
            {
                this._grpRequest = new InterrogateByGroupRequest();
                this._grpRequest.GroupRequest = new GroupRequest();
                this._grpRequest.Parameters = new InterrogationParameters();
            }

            internal InterrogateByGroupRequest InterrogateByGroupRequest
            {
                get
                {
                    return this._grpRequest;
                }
            }

            internal TargetType TargetType
            {
                get
                {
                    return TargetType.GroupName;
                }
            }

            internal override string[] Target
            {
                get
                {
                     
                    return new string[] {this._grpRequest.GroupRequest.GroupName};
                }
                set
                {
                    this._grpRequest.GroupRequest.GroupName = value[0];
                }
            }

            internal override Uri RequestStatusChanged
            {
                get
                {
                    return this._grpRequest.GroupRequest.StatusChangedService;
                }
                set
                {
                    this._grpRequest.GroupRequest.StatusChangedService = value;
                }
            }

            internal override System.DateTime ReadingStartTime
            {
                get
                {
                    return this._grpRequest.Parameters.ReadingStartTime;
                }
                set
                {
                    this._grpRequest.Parameters.ReadingStartTime = value;
                }
            }

            internal override System.DateTime ReadingEndTime
            {
                get
                {
                    return this._grpRequest.Parameters.ReadingEndTime;
                }
                set
                {
                    this._grpRequest.Parameters.ReadingEndTime = value;
                }
            }

            internal override System.DateTime InterrogationWindowStartTime
            {
                get
                {
                    return this._grpRequest.Parameters.InterrogationWindowStartTime;
                }
                set
                {
                    this._grpRequest.Parameters.InterrogationWindowStartTime = value;
                }
            }

            internal override System.DateTime InterrogationWindowEndTime
            {
                get
                {
                    return this._grpRequest.Parameters.InterrogationWindowEndTime;
                }
                set
                {
                    this._grpRequest.Parameters.InterrogationWindowEndTime = value;
                }
            }

            internal override bool PerformAndRetrieveSelfRead
            {
                get
                {
                    return this._grpRequest.Parameters.PerformAndRetrieveSelfRead;
                }
                set
                {
                    this._grpRequest.Parameters.PerformAndRetrieveSelfRead = value;
                }
            }

            internal override bool PerformDemandReset
            {
                get
                {
                    return this._grpRequest.Parameters.PerformDemandReset;
                }
                set
                {
                    this._grpRequest.Parameters.PerformDemandReset = value;
                }
            }

            internal override bool RetrieveHomeNetworkData
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveHomeNetworkData;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveHomeNetworkData = value;
                }
            }

            internal override bool RetrieveInstantaneousData
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveInstantaneousData;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveInstantaneousData = value;
                }
            }

            internal override bool RetrieveLastDemandReset
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveLastDemandReset;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveLastDemandReset = value;
                }
            }

            internal override bool RetrieveLoadProfileData
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveLoadProfileData;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveLoadProfileData = value;
                }
            }

            internal override bool RetrieveLogEvents
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveLogEvents;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveLogEvents = value;
                }
            }

            internal override bool RetrieveNetworkStatistics
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveNetworkStatistics;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveNetworkStatistics = value;
                }
            }

            internal override bool RetrievePriorSelfRead
            {
                get
                {
                    return this._grpRequest.Parameters.RetrievePriorSelfRead;
                }
                set
                {
                    this._grpRequest.Parameters.RetrievePriorSelfRead = value;
                }
            }

            internal override bool RetrieveVoltageMonitorData
            {
                get
                {
                    return this._grpRequest.Parameters.RetrieveVoltageMonitorData;
                }
                set
                {
                    this._grpRequest.Parameters.RetrieveVoltageMonitorData = value;
                }
            }
        }

        internal class InterrogateEndpointsRequest : RequestDetails
        {
            private InterrogateByEndpointsRequest _epRequest = null;

            internal InterrogateEndpointsRequest()
            {
                this._epRequest = new InterrogateByEndpointsRequest();
                this._epRequest.EndpointCollectionRequest = new EndpointCollectionRequest();
                this._epRequest.Parameters = new InterrogationParameters();
            }

            internal InterrogateByEndpointsRequest InterrogateByEndpointsRequest
            {
                get { return this._epRequest; }
            }

            internal TargetType TargetType
            {
                get
                {
                    return TargetType.MeterESNs;
                }
            }

            internal override string[] Target
            {
                get
                {
                    return this._epRequest.EndpointCollectionRequest.ElectronicSerialNumbers;
                }
                set
                {
                    this._epRequest.EndpointCollectionRequest.ElectronicSerialNumbers = value;
                }
            }

            internal override Uri RequestStatusChanged
            {
                get
                {
                    return this._epRequest.EndpointCollectionRequest.StatusChangedService;
                }
                set
                {
                    this._epRequest.EndpointCollectionRequest.StatusChangedService = value;
                }
            }

            internal override System.DateTime ReadingStartTime
            {
                get
                {
                    return this._epRequest.Parameters.ReadingStartTime;
                }
                set
                {
                    this._epRequest.Parameters.ReadingStartTime = value;
                }
            }

            internal override System.DateTime ReadingEndTime
            {
                get
                {
                    return this._epRequest.Parameters.ReadingEndTime;
                }
                set
                {
                    this._epRequest.Parameters.ReadingEndTime = value;
                }
            }

            internal override System.DateTime InterrogationWindowStartTime
            {
                get
                {
                    return this._epRequest.Parameters.InterrogationWindowStartTime;
                }
                set
                {
                    this._epRequest.Parameters.InterrogationWindowStartTime = value;
                }
            }

            internal override System.DateTime InterrogationWindowEndTime
            {
                get
                {
                    return this._epRequest.Parameters.InterrogationWindowEndTime;
                }
                set
                {
                    this._epRequest.Parameters.InterrogationWindowEndTime = value;
                }
            }

            internal override bool PerformAndRetrieveSelfRead
            {
                get
                {
                    return this._epRequest.Parameters.PerformAndRetrieveSelfRead;
                }
                set
                {
                    this._epRequest.Parameters.PerformAndRetrieveSelfRead = value;
                }
            }

            internal override bool PerformDemandReset
            {
                get
                {
                    return this._epRequest.Parameters.PerformDemandReset;
                }
                set
                {
                    this._epRequest.Parameters.PerformDemandReset = value;
                }
            }

            internal override bool RetrieveHomeNetworkData
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveHomeNetworkData;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveHomeNetworkData = value;
                }
            }

            internal override bool RetrieveInstantaneousData
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveInstantaneousData;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveInstantaneousData = value;
                }
            }

            internal override bool RetrieveLastDemandReset
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveLastDemandReset;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveLastDemandReset = value;
                }
            }

            internal override bool RetrieveLoadProfileData
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveLoadProfileData;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveLoadProfileData = value;
                }
            }

            internal override bool RetrieveLogEvents
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveLogEvents;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveLogEvents = value;
                }
            }

            internal override bool RetrieveNetworkStatistics
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveNetworkStatistics;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveNetworkStatistics = value;
                }
            }

            internal override bool RetrievePriorSelfRead
            {
                get
                {
                    return this._epRequest.Parameters.RetrievePriorSelfRead;
                }
                set
                {
                    this._epRequest.Parameters.RetrievePriorSelfRead = value;
                }
            }

            internal override bool RetrieveVoltageMonitorData
            {
                get
                {
                    return this._epRequest.Parameters.RetrieveVoltageMonitorData;
                }
                set
                {
                    this._epRequest.Parameters.RetrieveVoltageMonitorData = value;
                }
            }
        }

        #region RequestDetails
        internal abstract class RequestDetails
        {
            internal abstract string[] Target
            {
                get;
                set;
            }

            internal abstract Uri RequestStatusChanged
            {
                get;
                set;
            }

            internal abstract System.DateTime ReadingStartTime
            {
                get;
                set;
            }

            internal abstract System.DateTime ReadingEndTime
            {
                get;
                set;
            }

            internal abstract System.DateTime InterrogationWindowStartTime
            {
                get;
                set;
            }

            internal abstract System.DateTime InterrogationWindowEndTime
            {
                get;
                set;
            }

            internal abstract bool PerformAndRetrieveSelfRead
            {
                get;
                set;
            }

            internal abstract bool PerformDemandReset
            {
                get;
                set;
            }

            internal abstract bool RetrieveHomeNetworkData
            {
                get;
                set;
            }

            internal abstract bool RetrieveInstantaneousData
            {
                get;
                set;
            }

            internal abstract bool RetrieveLastDemandReset
            {
                get;
                set;
            }

            internal abstract bool RetrieveLoadProfileData
            {
                get;
                set;
            }

            internal abstract bool RetrieveLogEvents
            {
                get;
                set;
            }

            internal abstract bool RetrieveNetworkStatistics
            {
                get;
                set;
            }

            internal abstract bool RetrievePriorSelfRead
            {
                get;
                set;
            }

            internal abstract bool RetrieveVoltageMonitorData
            {
                get;
                set;
            }
        }
        #endregion

        #region class ActionParameters
        //public class ActionParameters
        //{
        //    InterrogationParameters _interrogationParams = new InterrogationParameters();

        //    internal ActionParameters(DateTime readingStartTime,
        //        DateTime readingEndTime,
        //        DateTime interrogationWindowStartTime,
        //        DateTime interrogationWindowEndTime,
        //        bool performAndRetrieveSelfRead,
        //        bool performDemandReset,
        //        bool retrieveHomeNetworkData,
        //        bool retrieveInstantaneousData,
        //        bool retrieveLastDemandReset,
        //        bool retrieveLoadProfileData,
        //        bool retrieveLogEvents,
        //        bool retrieveNetworkStatistics,
        //        bool retrievePriorSelfRead,
        //        bool retrieveVoltageMonitorData)
        //    {
        //        this._interrogationParams.ReadingStartTime = readingStartTime;
        //        this._interrogationParams.ReadingEndTime = readingEndTime;
        //        this._interrogationParams.InterrogationWindowStartTime = interrogationWindowStartTime;
        //        this._interrogationParams.InterrogationWindowEndTime = interrogationWindowEndTime;
        //        this._interrogationParams.PerformAndRetrieveSelfRead = performAndRetrieveSelfRead;
        //        this._interrogationParams.PerformDemandReset = performDemandReset;
        //        this._interrogationParams.RetrieveHomeNetworkData = retrieveHomeNetworkData;
        //        this._interrogationParams.RetrieveInstantaneousData = retrieveInstantaneousData;
        //        this._interrogationParams.RetrieveLastDemandReset = retrieveLastDemandReset;
        //        this._interrogationParams.RetrieveLoadProfileData = retrieveLoadProfileData;
        //        this._interrogationParams.RetrieveLogEvents = retrieveLogEvents;
        //        this._interrogationParams.RetrieveNetworkStatistics = retrieveNetworkStatistics;
        //        this._interrogationParams.RetrievePriorSelfRead = retrievePriorSelfRead;
        //        this._interrogationParams.RetrieveVoltageMonitorData = retrieveVoltageMonitorData;
        //    }

            

        //    public System.DateTime ReadingStartTime
        //    {
        //        get
        //        {
        //            return this._interrogationParams.ReadingStartTime;
        //        }
        //    }

        //    public System.DateTime ReadingEndTime
        //    {
        //        get
        //        {
        //            return this._interrogationParams.ReadingEndTime;
        //        }
        //    }

        //    public System.DateTime InterrogationWindowStartTime
        //    {
        //        get
        //        {
        //            return this._interrogationParams.InterrogationWindowStartTime;
        //        }
        //    }

        //    public System.DateTime InterrogationWindowEndTime
        //    {
        //        get
        //        {
        //            return this._interrogationParams.InterrogationWindowEndTime;
        //        }
        //    }

        //    public bool PerformAndRetrieveSelfRead
        //    {
        //        get
        //        {
        //            return this._interrogationParams.PerformAndRetrieveSelfRead;
        //        }
        //    }

        //    public bool PerformDemandReset
        //    {
        //        get
        //        {
        //            return this._interrogationParams.PerformDemandReset;
        //        }
        //    }

        //    public bool RetrieveHomeNetworkData
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveHomeNetworkData;
        //        }
        //    }

        //    public bool RetrieveInstantaneousData
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveInstantaneousData;
        //        }
        //    }

        //    public bool RetrieveLastDemandReset
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveLastDemandReset;
        //        }
        //    }

        //    public bool RetrieveLoadProfileData
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveLoadProfileData;
        //        }
        //    }

        //    public bool RetrieveLogEvents
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveLogEvents;
        //        }
        //    }

        //    public bool RetrieveNetworkStatistics
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveNetworkStatistics;
        //        }
        //    }

        //    public bool RetrievePriorSelfRead
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrievePriorSelfRead;
        //        }
        //    }

        //    public bool RetrieveVoltageMonitorData
        //    {
        //        get
        //        {
        //            return this._interrogationParams.RetrieveVoltageMonitorData;
        //        }
        //    }
        //}
        #endregion ActionParameters
    }

}
