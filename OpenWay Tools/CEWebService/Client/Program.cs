using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Configuration;

using Itron.Ami.CEWebServiceClient.External;
using Itron.Ami.CEWebServiceClient.Internal;
using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.CEWebServiceClientLibClient.Properties;
using Itron.Ami.Facade.WebServices.Security.V200912.ClientProxy;
using Itron.Ami.Facade.WebServices.Subscriptions.V200908.ClientProxy;

namespace CEWebServiceClientLibClient
{
    class Program
    {
        private static string _internalUserName = string.Empty;
        private static string _internalUserPassword = string.Empty;
        private static string _externalUserName = string.Empty;
        private static string _externalUserPassword = string.Empty;
        private static string _internalEndpointName = "InternalNetTcp";
        private static string _dataEndpointName = "DataUnsecuredClient";
        private static string _requestEndpointName = "RequestUnsecuredClient";
        private static string _reportEndpointName = "ReportingUnsecuredClient";
        private static string _controlEndpointName = "ControlUnsecuredClient";
        private static string _securityEndpointName = "SignedAuthorizationKerberosClient";
        private static string _dataSubscriberEndpointName = string.Empty;
        private static string _eventSubscriberEndpointName = string.Empty;
        private static string _exceptionSubscriberName = string.Empty;

        private static string _subscriptionEndpointName = "SubscriptionUnsecuredClient";
        private static string _provisioningEndpointName = "ProvisioningUnsecuredClient";
        private static string _hanEndpointName = "HanUnsecuredClient";
        private static string _membershipEndpointName = "MembershipUnsecuredClient";

        private static EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>> _requestStatusChangedCallback;
        private static EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>> _internalCallback;
        private static EventHandler<SubscriberDataEventArgs<EventSubscriberItem>> _eventCallback;
        private static EventHandler<SubscriberDataEventArgs<ExceptionSubscriberItem>> _exceptionCallback;
                    
        private static InternalAPICallbackItem _internalCallbackItem;
        private static RequestStatusChangedItem _requestStatusChangedItem;

        private static InteractiveRead _read1;
        private static FirmwareDownload _fwdl;
        private static Ping _ping1;
        private static Interrogation _interrogation1;
        private static SignatureAuthorization _signedAuth1;

        static void Main(string[] args)
        {

#if (USERNAME_CLIENT)
            _securityEndpointName = "SignedAuthorizationUsernameClient";     
            _externalUserName = "OpenWayTool";
            _externalUserPassword = "";
            //_dataEndpointName = "DataUserNameClient";
            //_requestEndpointName = "RequestUserNameClient";
            //_reportEndpointName = "ReportingUserNameClient";
            //_subscriptionEndpointName = "SubscriptionUsernameClient";
            //_controlEndpointName = "ControlUserNameClient";
            //_provisioningEndpointName = "ProvisioningUserNameClient";
            //_hanEndpointName = "HanUserNameClient";
            //_membershipEndpointName = "MembershipUserNameClient";
#endif

            try
            {   
                _internalCallback = new EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>>(InternalCallbackOccured);
                _requestStatusChangedCallback = new EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>>(RequestStatusChangedCallbackOccured);
                _eventCallback = new EventHandler<SubscriberDataEventArgs<EventSubscriberItem>>(EventCallbackOccured);
                _exceptionCallback = new EventHandler<SubscriberDataEventArgs<ExceptionSubscriberItem>>(ExceptionCallbackOccured);

                ConfigureInternalLib();
                ConfigureExternalLib();

                ExternalLib.Listener.RequestStatusChangedController.StartService();
                ExternalLib.Listener.RequestStatusChangedController.AddHandler(_requestStatusChangedCallback);

                ExternalLib.Listener.EventSubscriberController.StartService();
                ExternalLib.Listener.EventSubscriberController.AddHandler(_eventCallback);

                ExternalLib.Listener.ExceptionSubscriberController.StartService();
                ExternalLib.Listener.ExceptionSubscriberController.AddHandler(_exceptionCallback);

                InternalLib.Listener.InternalAPIController.StartService();
                InternalLib.Listener.InternalAPIController.AddHandler(_internalCallback);
                
                //RunInteractiveRead();

                //RunSignedAuthorization();

                //RunPing();

                //RunInterrogation();                       

                //RunFirmwareDownload();

                Console.ReadLine();                
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}: {1}.", e.GetType(), e.Message));
                Console.ReadLine();
            }
            finally
            {
                InternalLib.Listener.InternalAPIController.RemoveHandler(_internalCallback);
                InternalLib.Listener.InternalAPIController.StopService();

                ExternalLib.Listener.EventSubscriberController.RemoveHandler(_eventCallback);
                ExternalLib.Listener.EventSubscriberController.StopService();

                ExternalLib.Listener.ExceptionSubscriberController.RemoveHandler(_exceptionCallback);
                ExternalLib.Listener.ExceptionSubscriberController.StopService();

                ExternalLib.Listener.RequestStatusChangedController.RemoveHandler(_requestStatusChangedCallback);
                ExternalLib.Listener.RequestStatusChangedController.StopService();
            }
        }

        private static void ConfigureInternalLib()
        {
            InternalLib.Endpoint.ExternalUserName = _externalUserName;
            InternalLib.Endpoint.ExternalUserPassword = _externalUserPassword; 
            InternalLib.Endpoint.InternalEndpointName = _internalEndpointName; 
            InternalLib.Endpoint.InternalUserName = _internalUserName; 
            InternalLib.Endpoint.InternalUserPassword = _internalUserPassword; 
            InternalLib.Endpoint.ReportEndpointName = _reportEndpointName; 
            InternalLib.Endpoint.RequestEndpointName = _requestEndpointName; 
        }

        private static void ConfigureExternalLib()
        {
            ExternalLib.Endpoint.ExternalUserName = _externalUserName;
            ExternalLib.Endpoint.ExternalUserPassword = _externalUserPassword;
            ExternalLib.Endpoint.SecurityEndpointName = _securityEndpointName; 
            
            ExternalLib.Endpoint.DataEndpointName = _dataEndpointName;
            ExternalLib.Endpoint.ReportEndpointName = _reportEndpointName;
            ExternalLib.Endpoint.RequestEndpointName = _requestEndpointName;
            ExternalLib.Endpoint.ControlEndpointName = _controlEndpointName;
            ExternalLib.Endpoint.SubscriptionEndpointName = _subscriptionEndpointName;
            
        }

        private static void RunSignedAuthorization()
        {
            try
            {
                string serialNumber = null; // "60858769";
                _signedAuth1 = ExternalLib.Request.SignatureAuthorization(serialNumber, PermissionLevel.Level4, TimeSpan.FromDays(1));
                _signedAuth1.Execute();
                byte[] sa = _signedAuth1.Result.SignedAuthorization;
            }
            catch (Exception ex)
            {
                Console.WriteLine("RunSignedAuthorization: " + ex.Message);
            }
        }


        private static void RunInterrogation()
        {
            try
            {
                _interrogation1 = ExternalLib.Request.Interrogation(Interrogation.TargetType.GroupName,
                        new string[] { "Simple" },
                        new DateTime(DateTime.UtcNow.AddHours(-8).Ticks),
                        new DateTime(DateTime.UtcNow.Ticks),
                        new DateTime(DateTime.UtcNow.Ticks),
                        new DateTime(DateTime.UtcNow.AddMinutes(15).Ticks),
                        false, false, false, false, false, false, true, false, false, false);

                RequestStatusChangedItem item  = _interrogation1.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                {
                    //Running longer than wanted.  Cancel it.
                    _interrogation1.Cancel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RunInterrogation: " + ex.Message);
            }
        }

        private static void RunPing()
        {
            try
            {
                _ping1 = ExternalLib.Request.Ping( new string[] {"2.16.840.1.114416.0.57513008"});
                RequestStatusChangedItem item = _ping1.Execute(TimeSpan.FromSeconds(30));
                if (null == item)
                    _ping1.Cancel();

            }
            catch (Exception ex)
            {
                Console.WriteLine("RunPing: " + ex.Message);
            }
        }


        private static void RunInteractiveRead()
        {
            try
            {
                _read1 = InternalLib.Request.InteractiveRead("2.16.840.1.114416.0.57513008", 
                    false, 
                    new DateTime(DateTime.UtcNow.AddHours(-8).Ticks),
                    new DateTime(DateTime.UtcNow.Ticks),
                    false, false, true, false, false, true, false, false, false, false, false, false, false);
                
                //Sync call that will return after job completion or waiting Timespan. Callback when completed will occur.
                InternalAPICallbackItem item = _read1.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                {
                    //Running longer than wanted.  Cancel it.
                    _read1.Cancel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RunInteractiveRead: " + ex.Message);
            }
        }

        private static void RunFirmwareDownload()
        {
            try
            {
                _fwdl = InternalLib.Request.FirmwareDownload(1004, 1101);
            
                //_fwdl.Enter.Execute();
                //_fwdl.Enter.Execute(_internalCallback);
                InternalAPICallbackItem item = _fwdl.Enter.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                    _fwdl.Enter.Cancel();

                item = _fwdl.Download.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                    _fwdl.Download.Cancel();

                //Transfer only makes sense if firmware type is third party over zigbee.
                //item = _fwdl.Transfer.Execute(_internalCallback, TimeSpan.FromMinutes(3), TimeSpan.FromSeconds(15));
                //if (null == item)
                //    _fwdl.Transfer.Cancel();

                item = _fwdl.Activate.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                    _fwdl.Activate.Cancel();

                item = _fwdl.Exit.Execute(TimeSpan.FromSeconds(10));
                if (null == item)
                    _fwdl.Exit.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("RunFirmwareDownload: " + ex.Message);
            }

        }

        private static void RequestStatusChangedCallbackOccured(object sender, SubscriberDataEventArgs<RequestStatusChangedItem> e)
        {
            //Capture the item and do with it what you will.
            _requestStatusChangedItem = e.DataList[0];
        }

        private static void InternalCallbackOccured(object sender, SubscriberDataEventArgs<InternalAPICallbackItem> e)
        {
            //Capture the item and do with it what you will.
            _internalCallbackItem = e.DataList[0];
        }


        private static void EventCallbackOccured(object sender, SubscriberDataEventArgs<EventSubscriberItem> e)
        {
            //Capture the item and do with it what you will.

            if ((null != e) &&
                (0 < e.DataList.Count) &&
                (null != e.DataList[0].EventsArrivedInput) &&
                (null != e.DataList[0].EventsArrivedInput.EventLog) &&
                (e.DataList[0].EventsArrivedInput.EventLog.MeterEvents.Length > 0))
            {
                foreach (MeterEvent me in e.DataList[0].EventsArrivedInput.EventLog.MeterEvents)
                {
                    //Do what you will with each event
                }
            }
        }

        private static void ExceptionCallbackOccured(object sender, SubscriberDataEventArgs<ExceptionSubscriberItem> e)
        {
            //Capture the item and do with it what you will.

            if ((null != e) &&
                (0 < e.DataList.Count) &&
                (null != e.DataList[0].ExceptionsArrivedInput) &&
                (null != e.DataList[0].ExceptionsArrivedInput.MeterExceptionCollection) &&
                (e.DataList[0].ExceptionsArrivedInput.MeterExceptionCollection.Length > 0))
            {
                foreach (MeterException me in e.DataList[0].ExceptionsArrivedInput.MeterExceptionCollection)
                {
                    //Do what you will with each event
                }
            }
        }
    }
}
