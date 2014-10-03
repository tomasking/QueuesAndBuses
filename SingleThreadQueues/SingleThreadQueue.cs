using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace QueuesAndBuses.SingleThreadQueues
{
    public interface ISingleThreadActionQueue
    {
        void Enqueue(Action action);
    }

    public class SingleThreadQueue : ISingleThreadActionQueue, IDisposable
    {
        readonly BlockingCollection<Action> blockingQueue = new BlockingCollection<Action>();
        private bool isDisposing;
        private DateTime lastActive;

        public SingleThreadQueue()
        {
            lastActive = DateTime.UtcNow;
            StartProcessingActions();
        }

        public void Enqueue(Action action)
        {
            if (!isDisposing)
            {
                blockingQueue.Add(action);
            }
        }
        
        private void StartProcessingActions()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var action in blockingQueue.GetConsumingEnumerable())
                {
                    if (!isDisposing)
                    {
                        try
                        {
                            lastActive = DateTime.UtcNow;
                            action.Invoke();
                        }
                        catch (Exception e)
                        {
                            //TODO: Log
                        }
                    }
                }

            },TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                blockingQueue.CompleteAdding();
            }
        }

        public bool IsInActive(TimeSpan inactiveThreshold)
        {
            return lastActive > DateTime.UtcNow - inactiveThreshold;
        }
    }
}