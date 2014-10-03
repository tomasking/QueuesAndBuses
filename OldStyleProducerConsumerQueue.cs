using System;
using System.Collections.Generic;
using System.Threading;

namespace QueuesAndBuses
{
    public class OldStyleProducerConsumerQueue
    {
        readonly object queueLock = new object();
        readonly Thread[] workers;
        readonly Queue<Action> actionQueue = new Queue<Action>();

        public OldStyleProducerConsumerQueue(int workerCount)
        {
            workers = new Thread[workerCount];
            for (int i = 0; i < workerCount; i++)
            {
                (workers[i] = new Thread(Consume)).Start();
            }
        }

        public void Enqueue(Action item)
        {
            lock (queueLock)
            {
                actionQueue.Enqueue(item); 
                Monitor.Pulse(queueLock); // notify the waiting thread there is a change in the blocking condition.
            }
        }

        private void Consume()
        {
            while (true) // Keep consuming until a null action is sent
            {
                Action action;
                lock (queueLock)
                {
                    while (actionQueue.Count == 0)
                    {
                        Monitor.Wait(queueLock); //release lock on object and block until current thread re-aquires lock
                    }
                    action = actionQueue.Dequeue();
                }
                if (action == null) break;
                action(); // Execute item.
            }
        }

        public void Shutdown(bool waitForWorkers)
        {
            // Send each worker thread a null item to make each exit.
            foreach (Thread worker in workers)
                Enqueue(null);
            
            if (waitForWorkers)
            {
                foreach (Thread worker in workers)
                {
                    worker.Join();
                }
            }
        }
    }
}