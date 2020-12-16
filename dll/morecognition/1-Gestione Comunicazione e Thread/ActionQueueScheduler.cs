using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace morecognition
{
    internal class ActionQueueScheduler
    {
        private BlockingCollection<Action> _actionQueue = new BlockingCollection<Action>();
        private Thread _queueThread;
        private CancellationTokenSource _tokenSource;

        public ActionQueueScheduler()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _queueThread = new Thread(RunOnCurrentThread)
            {
                Name = $"{GetType().Name}",
                IsBackground = true
            };
            _queueThread.Start();
        }

        public void Schedule(Action action)
        {
            _actionQueue.TryAdd(action);
        }

        public void Stop(bool completeQueue)
        {
            if (completeQueue)
                _actionQueue.CompleteAdding();
            else
                _tokenSource.Cancel();

            _queueThread.Join();
        }

        private void RunOnCurrentThread()
        {
            try
            {
                foreach (var action in _actionQueue.GetConsumingEnumerable(_tokenSource.Token))
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{GetType().Name} exception: {ex.Message} {ex.StackTrace}");
                    }
                }
            }
            catch (OperationCanceledException)
            { }
        }

        public void Dispose()
        { }
    }
}
