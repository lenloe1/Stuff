using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

using Itron.Ami.CEWebServiceClient.Base;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    public static class InternalLib
    {
        #region Listener
        public static class Listener
        {
            private static InternalAPIController _internalAPIController = new InternalAPIController();
            public static InternalAPIController InternalAPIController
            {
                get { return _internalAPIController; }
            }
        } 
        #endregion

        #region Endpoint
        public static class Endpoint
        {
            public static bool SetClientEndpointAddress(string clientEndpointName, string address)
            {
                return InternalClientFactory.SetClientEndpointAddress(clientEndpointName, address);
            }
            
            private static string _internalEndpointName;
            public static string InternalEndpointName
            {
                get { return _internalEndpointName; }
                set { _internalEndpointName = value; }
            }

            private static string _internalUserName;
            public static string InternalUserName
            {
                get { return _internalUserName; }
                set { _internalUserName = value; }
            }

            private static string _internalUserPassword;
            public static string InternalUserPassword
            {
                get { return _internalUserPassword; }
                set { _internalUserPassword = value; }
            }

            private static string _reportEndpointName;
            public static string ReportEndpointName
            {
                get { return _reportEndpointName; }
                set { _reportEndpointName = value; }
            }

            private static string _requestEndpointName;
            public static string RequestEndpointName
            {
                get { return _requestEndpointName; }
                set { _requestEndpointName = value; }
            }

            private static string _externalUserName;
            public static string ExternalUserName
            {
                get { return _externalUserName; }
                set { _externalUserName = value; }
            }

            private static string _externalUserPassword;
            public static string ExternalUserPassword
            {
                get { return _externalUserPassword; }
                set { _externalUserPassword = value; }
            }
        }
        #endregion

        #region Request
        public static class Request
        {
            public static InteractiveRead InteractiveRead(string esn,
                bool needsFastResponseFromMeter,
                DateTime readingStartTime,
                DateTime readingEndTime,
                bool performDemandReset,
                bool retrieveHomeNetworkData,
                bool retrieveInstantaneousData,
                bool retrieveLastDemandReset,
                bool retrieveLoadProfileData,
                bool retrieveLogEvents,
                bool retrieveNetworkStatistics,
                bool retrievePriorSelfRead,
                bool retrieveVoltageMonitorData,
                bool retrieveBlockStatus,
                bool retrieveCommLogEvents,
                bool retrieveDiagnosticData,
                bool retrieveRecentRegisterData)
            {
                InteractiveRead read = new InteractiveRead(esn, needsFastResponseFromMeter,
                        readingStartTime, readingEndTime, performDemandReset, 
                        retrieveHomeNetworkData, retrieveInstantaneousData, retrieveLastDemandReset, 
                        retrieveLoadProfileData, retrieveLogEvents, retrieveNetworkStatistics, 
                        retrievePriorSelfRead, retrieveVoltageMonitorData, retrieveBlockStatus, 
                        retrieveCommLogEvents, retrieveDiagnosticData, retrieveRecentRegisterData);

                return read;
            }

            public static FirmwareDownload FirmwareDownload(int endpointGroupKey, int firmwareKey)
            {
                FirmwareDownload fwdl = new FirmwareDownload(endpointGroupKey, firmwareKey);

                return fwdl;
            }
        }
        #endregion
    }
}
