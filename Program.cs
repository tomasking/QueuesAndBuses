using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueuesAndBuses
{
    class Program
    {
        private static void Main(string[] args)
        {
            OldStyleProducerConsumerQueueExample();
            //BlockingConsumerProducerQueueExample();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void OldStyleProducerConsumerQueueExample()
        {
            var queue = new OldStyleProducerConsumerQueue(5);
            Console.WriteLine("Started all tasks");

            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                Action action = () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(i1);
                };
                queue.Enqueue(action);
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Disposing");
            queue.Shutdown(false);

            Action action2 = () => Console.WriteLine("Shouldn't do this one");
            queue.Enqueue(action2);
        }

        private static void BlockingConsumerProducerQueueExample()
        {
            var queue = new BlockingConsumerProducerQueue(5);
            Console.WriteLine("Started all tasks");

            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                Action action = () =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(i1);
                };
                queue.Enqueue(action);
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Disposing");
            queue.Dispose();
            
            Action action2 = () => Console.WriteLine("Shouldn't do this one");
            queue.Enqueue(action2);
        }
    }
}
