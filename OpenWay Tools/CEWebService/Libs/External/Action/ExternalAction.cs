using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Common.V200810.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.External
{
    public interface IExternalCEAction : ICEAction
    {
        RequestStatusChangedItem Execute(TimeSpan syncWaitTime);
    }

    public abstract class ExternalAction : IExternalCEAction //IDisposable
    {
        internal ExternalAction()
        {
        }

        private Thread _threadCCL;
        protected bool _aborted = false;
        protected string _requestID;

        private RequestStatusChangedItem _externalCallbackItem;
        internal RequestStatusChangedItem ExternalCallbackItem
        {
            get { return _externalCallbackItem; }
            set { _externalCallbackItem = value; }
        }

        private TimeSpan _syncWaitTime;
        internal TimeSpan SyncWaitTime
        {
            get { return _syncWaitTime; }
        }

        protected string _uniqueID;
        public string UniqueID
        {
            get { return _uniqueID; }
        }

        private ManualResetEvent _eventListenerCallback;
        internal ManualResetEvent EventListenerCallback
        {
            get { return _eventListenerCallback; }
        }

        private ManualResetEvent _eventStopCCLThread;
        internal ManualResetEvent EventStopCCLThread
        {
            get { return _eventStopCCLThread; }
        }

        private ManualResetEvent _eventCCLThreadStopped;
        internal ManualResetEvent EventCCLThreadStopped
        {
            get { return _eventCCLThreadStopped; }
        }

        internal abstract void Abort();
        internal abstract void Invoke();

        public virtual void Execute()
        {
            //TODO: Logging the exception would be nice.
            try
            {
                this.Invoke();
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (CommunicationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.Abort();
            }
        }

        public virtual RequestStatusChangedItem Execute(TimeSpan syncWaitTime)
        {
            //Client will wait syncWaitTime for a result to be returned on the request thread.

            this._externalCallbackItem = null;
            this._syncWaitTime = syncWaitTime;


            //Kick off a thread to listen for the controller callback
            this._threadCCL = new Thread(new ThreadStart(this.StartCCLThread));
            this._threadCCL.Name = "Internal Action Controller Callback Listener";
            this._threadCCL.Start();

            //Execute the request
            Execute();

            if (this._aborted)
                StopCCLThread();
            else
                this._eventListenerCallback.WaitOne();

            return _externalCallbackItem;
        }

        public virtual void Cancel()
        {
            if (!string.IsNullOrEmpty(this._uniqueID))
            {
                Client<RequestServiceClient> c = null;

                try
                {
                    RequestToken requestToken = new RequestToken();
                    requestToken.Id = new Guid(this._uniqueID);


                    c = ExternalClientFactory.ConstructRequestServiceClient(ExternalLib.Endpoint.RequestEndpointName, ExternalLib.Endpoint.ExternalUserName, ExternalLib.Endpoint.ExternalUserPassword);
                    c.ServiceClient.CancelRequest(requestToken);
                    c.ServiceClient.Close();
                }
                catch (TimeoutException)
                {
                    throw;
                }
                catch (CommunicationException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    c.ServiceClient.Abort();
                }
            }
        }

        private void StartCCLThread()
        {
            this._eventListenerCallback = new ManualResetEvent(false);
            this._eventListenerCallback.Reset();

            this._eventStopCCLThread = new ManualResetEvent(false);
            this._eventStopCCLThread.Reset();

            this._eventCCLThreadStopped = new ManualResetEvent(false);
            this._eventCCLThreadStopped.Reset();

            ControllerCallbackListener ccl = new ControllerCallbackListener(this);
            ccl.WaitForCallback();
        }

        internal void StopCCLThread()
        {
            if (null != this._threadCCL && this._threadCCL.IsAlive)
            {
                // set event to "Stop" the thread
                this._eventStopCCLThread.Set();

                // wait until thread will stop or finish
                while (_threadCCL.IsAlive)
                {
                    if (this._eventCCLThreadStopped.WaitOne(100, true))
                        break;

                    this._eventCCLThreadStopped.Reset();
                }
            }
        }
    }

    internal class ControllerCallbackListener
    {
        private EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>> _controllerEventHandler;
        private ExternalAction _externalAction;

        internal ControllerCallbackListener(ExternalAction externalAction)
        {
            _externalAction = externalAction;

            _controllerEventHandler = new EventHandler<SubscriberDataEventArgs<RequestStatusChangedItem>>(ControllerCallbackOccured);
            ExternalLib.Listener.RequestStatusChangedController.AddHandler(_controllerEventHandler);

        }

        internal void ControllerCallbackOccured(object sender, SubscriberDataEventArgs<RequestStatusChangedItem> e)
        {
            foreach (RequestStatusChangedItem item in e.DataList)
            {
                if (!string.IsNullOrEmpty(_externalAction.UniqueID) &&
                    ((_externalAction.UniqueID == item.Input.RequestStatus.RequestToken.Id.ToString())))
                {
                    _externalAction.ExternalCallbackItem = item;
                    _externalAction.EventListenerCallback.Set();

                    break;
                }
            }
        }

        internal void WaitForCallback()
        {
            DateTime now = DateTime.Now;
            DateTime quit = DateTime.Now.AddTicks(this._externalAction.SyncWaitTime.Ticks);

            //while waiting for result and thread should continue to run
            while ((now.Ticks < quit.Ticks) && !this._externalAction.EventStopCCLThread.WaitOne(100, true))
            {
                if (null != _externalAction.ExternalCallbackItem)
                {
                    break;
                }

                now = DateTime.Now;
            }

            ExternalLib.Listener.RequestStatusChangedController.AddHandler(_controllerEventHandler);

            this._externalAction.EventListenerCallback.Set();
            this._externalAction.EventCCLThreadStopped.Set();
        }
    }
}

