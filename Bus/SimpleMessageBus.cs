using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QueuesAndBuses.Bus
{
    public interface ISimpleMessageBus
    {
        void Subscribe<T>(Action<T> action);
        void Publish<T>(T message);
    }

    public class SimpleMessageBus : ISimpleMessageBus
    {
        private readonly ConcurrentDictionary<Type, ISet<MessageAction>> actions =
            new ConcurrentDictionary<Type, ISet<MessageAction>>();

        public void Subscribe<T>(Action<T> action)
        {
            MessageAction messageAction = new MessageAction<T>(action);
            actions.AddOrUpdate(typeof(T), new HashSet<MessageAction>() { messageAction },
                (key, existingVal) =>
                {
                    existingVal.Add(messageAction);
                    return existingVal;
                });
        }

        public void Publish<T>(T message)
        {
            var types = GetBaseTypes(message.GetType());

            foreach (var typeToTest in types)
            {
                ISet<MessageAction> messageActionsForType;
                if (actions.TryGetValue(typeToTest, out messageActionsForType))
                {
                    foreach (var messageActionForType in messageActionsForType)
                    {
                        messageActionForType.Execute(message);
                    }
                }
            }
        }

        private IEnumerable<Type> GetBaseTypes(Type messageType)
        {
            var types = new List<Type>() { messageType };
            var baseType = messageType.BaseType;
            if (baseType != null)
            {
                types.AddRange(GetBaseTypes(baseType));
            }
            return types;
        }
    }
}
