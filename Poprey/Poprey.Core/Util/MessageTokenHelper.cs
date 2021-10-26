using System;
using System.Collections.Generic;
using MvvmCross.Plugin.Messenger;

namespace Poprey.Core.Util
{
    public class MessageTokenHelper
    {
        private readonly IMvxMessenger _messenger;
        private readonly Dictionary<Type, MvxSubscriptionToken> _subscriptionTokens;

        public MessageTokenHelper(IMvxMessenger messenger)
        {
            _messenger = messenger;
            _subscriptionTokens = new Dictionary<Type, MvxSubscriptionToken>();
        }

        public void Subscribe<TMessage>(Action<TMessage> messageHandler, bool doNothingIfExists = false)
            where TMessage : MvxMessage
        {
            var messageType = typeof(TMessage);

            if (_subscriptionTokens.ContainsKey(messageType) && !doNothingIfExists)
            {
                throw new ArgumentException($"Already subscribed to message of type {messageType.Name}.");
            }

            _subscriptionTokens[typeof(TMessage)] = _messenger.Subscribe(messageHandler);
        }

        public void Publish(MvxMessage message)
        {
            _messenger.Publish(message);
        }

        public void Unsubscribe<TMessage>()
        {
            MvxSubscriptionToken kv;

            if (_subscriptionTokens.TryGetValue(typeof(TMessage), out kv))
            {
                _subscriptionTokens.Remove(typeof(TMessage));
                kv.Dispose();
            }
        }

        [Obsolete("Use Unsubscribe instead")]
        public void UnsubscribeSafely<TMessage>()
        {
            Unsubscribe<TMessage>();
        }

        public void UnsubscribeAll()
        {
            foreach (var token in _subscriptionTokens)
            {
                token.Value.Dispose();
            }

            _subscriptionTokens.Clear();
        }
    }
}