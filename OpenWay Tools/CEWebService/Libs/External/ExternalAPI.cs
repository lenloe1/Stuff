using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public static class ExternalLib
    {
        #region Listener
        public static class Listener
        {
            private static RequestStatusChangedController _rscController = new RequestStatusChangedController();
            public static RequestStatusChangedController RequestStatusChangedController
            {
                get { return _rscController; }
            }

            private static EventSubscriberController _eventSubscriberController = new EventSubscriberController();
            public static EventSubscriberController EventSubscriberController
            {
                get { return _eventSubscriberController; }
            }

            private static ExceptionSubscriberController _exceptionSubscriberController = new ExceptionSubscriberController();
            public static ExceptionSubscriberController ExceptionSubscriberController
            {
                get { return _exceptionSubscriberController; }
            }

            private static DataSubscriberController _dataSubscriberController = new DataSubscriberController();
            public static DataSubscriberController DataSubscriberController
            {
                get { return _dataSubscriberController; }
            }

        } 
        #endregion

        #region Endpoint
        public static class Endpoint
        {
            public static bool SetClientEndpointAddress(string clientEndpointName, string address)
            {
                return ExternalClientFactory.SetClientEndpointAddress(clientEndpointName, address);                 
            }

            private static string _reportEndpointName;
            public static string ReportEndpointName
            {
                get { return _reportEndpointName; }
                set { _reportEndpointName = value; }
            }

            private static string _securityEndpointName;
            public static string SecurityEndpointName
            {
                get { return _securityEndpointName; }
                set { _securityEndpointName = value; }
            }

            private static string _dataEndpointName;
            public static string DataEndpointName
            {
                get { return _dataEndpointName; }
                set { _dataEndpointName = value; }
            }

            private static string _controlEndpointName;
            public static string ControlEndpointName
            {
                get { return _controlEndpointName; }
                set { _controlEndpointName = value; }
            }

            private static string _requestEndpointName;
            public static string RequestEndpointName
            {
                get { return _requestEndpointName; }
                set { _requestEndpointName = value; }
            }

            private static string _membershipEndpointName;
            public static string MembershipEndpointName
            {
                get { return _membershipEndpointName; }
                set { _membershipEndpointName = value; }
            }

            private static string _provisioningEndpointName;
            public static string ProvisioningEndpointName
            {
                get { return _provisioningEndpointName; }
                set { _provisioningEndpointName = value; }
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

            private static string _subscriptionEndpointName;
            public static string SubscriptionEndpointName
            {
                get { return _subscriptionEndpointName; }
                set { _subscriptionEndpointName = value; }
            }

            private static string _hanEnpointName;
            public static string HANEndpointName
            {
                get { return _hanEnpointName; }
                set { _hanEnpointName = value; }
            }
        } 
        #endregion

        #region Request

        public static class Request
        {
            public static SignatureAuthorization SignatureAuthorization(string serialNumber, PermissionLevel permissionLevel, TimeSpan validityPeriod)
            {
                return new SignatureAuthorization(serialNumber, permissionLevel, validityPeriod);
            }

            public static Ping Ping(string[] target)
            {
                return new Ping(target);
            }

            public static DownloadConfiguration DownloadConfiguration(string target)
            {
                return new DownloadConfiguration(target);
            }

            public static Interrogation Interrogation(
            Interrogation.TargetType targetType,
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
                return new Interrogation(targetType,
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

            public static ExternalInteractiveRead ExternalInteractiveRead(
            string strTarget,
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
                return new ExternalInteractiveRead(
                strTarget,
                readingStartTime,
                readingEndTime,
                performDemandReset,
                retrieveHomeNetworkData,
                retrieveInstantaneousData,
                retrieveLastDemandReset,
                retrieveLoadProfileData,
                retrieveLogEvents,
                retrieveNetworkStatistics,
                retrievePriorSelfRead,
                retrieveVoltageMonitorData,
                retrieveBlockStatus,
                retrieveCommLogEvents,
                retrieveDiagnosticData,
                retrieveRecentRegisterData);
            }

            public static GetExternalInteractiveReadResult GetExternalInteractiveReadResult(
                string strUniqueID,
                string strIdentifier)
            {
                return new GetExternalInteractiveReadResult(strUniqueID, strIdentifier);
            }

            public static GetDownloadConfigurationResult GetDownloadConfigurationResult(
                string strUniqueID)
            {
                return new GetDownloadConfigurationResult(strUniqueID);
            }

            public static GetFirmwareDownloadsRunningReport GetFirmwareDownloadsRunningReport()
            {
                return new GetFirmwareDownloadsRunningReport();
            }
        }
        #endregion
    }
}
