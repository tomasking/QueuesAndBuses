using System;

namespace QueuesAndBuses.Bus
{
    internal abstract class MessageAction : IExecute
    {
        public abstract void Execute(object parameter);
    }

    internal class MessageAction<T> : MessageAction
    {
        private readonly Action<T> action;
        public MessageAction(Action<T> action)
        {
            this.action = action;
        }

        public Action<T> Action
        {
            get { return action; }
        }

        public override void Execute(object obj)
        {
            var message = (T)obj;
            if (action != null)
            {
                action(message);
            }
        }
    }
}