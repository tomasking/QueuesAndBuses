using System;
using System.Collections.Generic;
using System.Linq;

namespace QueuesAndBuses.SingleThreadQueues
{
    public interface ISingleThreadQueueManager
    {
        void ProcessAction(string id, Action action);
    }

    public class SingleThreadQueueManager : ISingleThreadQueueManager, IDisposable
    {
        readonly Dictionary<string, SingleThreadQueue> singleThreadQueues = 
            new Dictionary<string, SingleThreadQueue>(); 
        
        private static readonly object QueueLock = new object();
        readonly TimeSpan inactiveThreshold = new TimeSpan(0,0,30);

        private bool isDisposing;

        public void ProcessAction(string id, Action action)
        {
            lock (QueueLock)
            {
                SingleThreadQueue queue;
                if (!singleThreadQueues.TryGetValue(id, out queue))
                {
                    ClearInactiveQueues();
                    queue = new SingleThreadQueue();
                    singleThreadQueues.Add(id, queue);
                }
                queue.Enqueue(action);
            }
        }

        private void ClearInactiveQueues()
        {
            var inactiveQueues = singleThreadQueues.Where(q => q.Value.IsInActive(inactiveThreshold));
            foreach (var inactiveQueue in inactiveQueues)
            {
                inactiveQueue.Value.Dispose();
                singleThreadQueues.Remove(inactiveQueue.Key);
            }
        }

        public void Dispose()
        {
            if (isDisposing) return;
            isDisposing = true;
            lock (QueueLock)
            {
                foreach (var queue in singleThreadQueues.Values)
                {
                    queue.Dispose();
                }
                singleThreadQueues.Clear();
            }
        }
    }
}