using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Itron.Ami.CEWebServiceClient.Base
{
    public abstract class SubscriberService<TData> : IDisposable
    {
        private object _syncRoot = new object();
        private EventHandler<SubscriberDataEventArgs<TData>> _eventHandler;
        private SynchronizedCollection<TData> _list;
        private Timer _timer;
        private bool _isDisposed;

        protected SubscriberService()
        {
            this._list = new SynchronizedCollection<TData>(new object());
        }

        ~SubscriberService()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this._syncRoot)
                {
                    if (null != this._timer)
                    {
                        this._timer.Dispose();
                    }

                    if (null != this._list)
                    {
                        this._list.Clear();
                    }

                    this._timer = null;
                    this._eventHandler = null;

                    this._isDisposed = true;
                }
            }
        }

        public event EventHandler<SubscriberDataEventArgs<TData>> CallbackOccurred
        {
            add
            {
                lock (this._syncRoot)
                {
                    lock (this._list.SyncRoot)
                    {
                        this._eventHandler += value;

                        if (null == this._timer)
                        {
                            //Create the consumer timer with a 1/2 second initial pause followed by a 3 second monitoring interval.
                            this._timer = new Timer(new TimerCallback(this.TimerFired), null, 500, 3000);
                        }
                    }
                }
            }
            remove
            {
                lock (this._syncRoot)
                {
                    lock (this._list.SyncRoot)
                    {
                        this._eventHandler -= value;

                        if (null == this._eventHandler)
                        {
                            if (null != this._timer)
                            {
                                this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                                this._timer.Dispose();
                                this._timer = null;
                            }

                            this._list.Clear();
                        }
                    }
                }
            }
        }

        protected void DataArrived(TData data)
        {
            if (null != data)
            {
                lock (this._list.SyncRoot)
                {
                    if (null != this._eventHandler && !this._isDisposed)
                    {
                        this._list.Add(data);
                    }
                }
            }
        }

        private void TimerFired(object stateInfo)
        {
            if (Monitor.TryEnter(this._syncRoot, 1000))
            {
                try
                {
                    if (!this._isDisposed)
                    {
                        List<TData> list = new List<TData>();

                        lock (this._list.SyncRoot)
                        {
                            if (this._list.Count > 0)
                            {
                                for (int i = 0, j = this._list.Count; i < j; i++)
                                {
                                    list.Add(this._list[0]);
                                }

                                this._list.Clear();
                            }
                        }

                        if (null != this._eventHandler && list.Count > 0)
                        {
                            this._eventHandler(this, new SubscriberDataEventArgs<TData>(list));
                            list.Clear();
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(this._syncRoot);
                }
            }
        }
    }
}
