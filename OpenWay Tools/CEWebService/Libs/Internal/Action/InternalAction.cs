using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;

using Itron.Ami.CEWebServiceClient.Base;
using Itron.Ami.Facade.WebServices.Internal.ClientProxy;

namespace Itron.Ami.CEWebServiceClient.Internal
{
    public interface IInternalCEAction : ICEAction
    {
        InternalAPICallbackItem Execute(TimeSpan syncWaitTime);
    }

    public abstract class InternalCEAction : IInternalCEAction
    {
        internal InternalCEAction()
        {
        }

        private Thread _threadCCL;
        protected bool _aborted = false;
        protected string _requestID;

        private InternalAPICallbackItem _internalCallbackItem;
        internal InternalAPICallbackItem InternalCallbackItem
        {
            get { return _internalCallbackItem; }
            set { _internalCallbackItem = value; }
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

        public abstract void Cancel();
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

        public virtual InternalAPICallbackItem Execute(TimeSpan syncWaitTime)
        {
            //Client will wait syncWaitTime for a result to be returned on the request thread.

            this._internalCallbackItem = null;
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

            return _internalCallbackItem;
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
        private EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>> _controllerEventHandler;
        private InternalCEAction _InternalAction;

        internal ControllerCallbackListener(InternalCEAction InternalAction)
        {
            _InternalAction = InternalAction;

            _controllerEventHandler = new EventHandler<SubscriberDataEventArgs<InternalAPICallbackItem>>(ControllerCallbackOccured);
            InternalLib.Listener.InternalAPIController.AddHandler(_controllerEventHandler);

        }

        internal void ControllerCallbackOccured(object sender, SubscriberDataEventArgs<InternalAPICallbackItem> e)
        {
            foreach (InternalAPICallbackItem item in e.DataList)
            {
                if (!string.IsNullOrEmpty(_InternalAction.UniqueID) &&
                    ((_InternalAction.UniqueID == item.RequestID) || (_InternalAction.UniqueID == item.JobKey.Value.ToString())))
                {
                    _InternalAction.InternalCallbackItem = item;
                    _InternalAction.EventListenerCallback.Set();

                    break;
                }
            }
        }

        internal void WaitForCallback()
        {
            DateTime now = DateTime.Now;
            DateTime quit = DateTime.Now.AddTicks(this._InternalAction.SyncWaitTime.Ticks);

            //while waiting for result and thread should continue to run
            while ((now.Ticks < quit.Ticks) && !this._InternalAction.EventStopCCLThread.WaitOne(100, true))
            {
                if (null != _InternalAction.InternalCallbackItem)
                {
                    break;
                }

                now = DateTime.Now;
            }

            InternalLib.Listener.InternalAPIController.RemoveHandler(_controllerEventHandler);

            this._InternalAction.EventListenerCallback.Set();
            this._InternalAction.EventCCLThreadStopped.Set();
        }
    }
}

