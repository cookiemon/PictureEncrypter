using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace PictureEncrypter
{
    /// <summary>
    /// Threadworker working actions from the queue on a single thread.
    /// </summary>
    class ThreadWorker
    {
        private BlockingCollection<Action> _workCollection = new BlockingCollection<Action>();
        private Thread _worker;
        private CancellationTokenSource _cancel = new CancellationTokenSource();

        public void addWork(Action workItem)
        {
            _workCollection.Add(workItem);
        }

        public int CountWorkItems { get { return _workCollection.Count; } }

        public void StartWork()
        {
            if (_worker == null)
            {
                _worker = new Thread(new ThreadStart(delegate
                    {
                        try
                        {
                            while (true)
                            {
                                _workCollection.Take(_cancel.Token)();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }));
            }
            _worker.Start();
        }

        public void StopWork()
        {
            _cancel.Cancel();
            if (!_worker.Join(100))
            {
                _worker.Abort();
            }
        }

        public bool IsExecuting()
        {
            return _worker != null && _worker.IsAlive;
        }
    }
}
