using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace QueuesAndBuses
{
    public class BlockingConsumerProducerQueue: IDisposable
    {
        private readonly BlockingCollection<Action> actionQueue;
        private bool isDisposing;

        public BlockingConsumerProducerQueue(int workerCount)
        {
            actionQueue= new BlockingCollection<Action>();
            for (int i = 0; i < workerCount; i++)
            {
                Task.Factory.StartNew(StartConsuming);
            }
        }

        private void StartConsuming()
        {
            foreach (var action in actionQueue.GetConsumingEnumerable())
            {
                if (!isDisposing)
                {
                    action.Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            if (!isDisposing)
            {
                actionQueue.Add(action);
            }
        }
        
        public void Dispose()
        {
            if (!isDisposing)
            {
                isDisposing = true;
                actionQueue.CompleteAdding();
            }
        }
    }
}
