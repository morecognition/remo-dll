using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace morecognition
{
    internal sealed class MyTaskScheduler : TaskScheduler
    {
        private bool isExecuting;
        private Thread _thread;
        private BlockingCollection<Task> _tasks;
        private CancellationToken _token;

        public MyTaskScheduler(CancellationToken token)
        {
            _token = token;
            _tasks = new BlockingCollection<Task>();
            _thread = new Thread(RunOnCurrentThread)
            {
                IsBackground = true,
                Name = "MoreScheduler"
            };
        }

        private void RunOnCurrentThread()
        {
            isExecuting = true;

            try
            {
                foreach (Task t in _tasks.GetConsumingEnumerable(_token))
                {
                    TryExecuteTask(t);
                }
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                isExecuting = false;
            }
        }

        public void Start()
        {
            _thread.Start();
        }

        public Task Schedule(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this);
        }

        public void Complete()
        {
            _tasks.CompleteAdding();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return null;
        }

        protected override void QueueTask(Task t)
        {
            try
            {
                _tasks.Add(t, _token);
            }
            catch (OperationCanceledException)
            { }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (taskWasPreviouslyQueued)
                return false;

            return isExecuting && TryExecuteTask(task);
        }

        public override int MaximumConcurrencyLevel
        {
            get
            {
                return 1;
            }
        }
    }
}