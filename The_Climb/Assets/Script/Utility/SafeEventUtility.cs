using System;
using UnityEngine;
namespace TheClimb.Utility
{
    public class SafeEventUtility<T>    //  サブスクライブUtility
    {
        private event Action<T> internalEvent = delegate { };

        public void Subscribe(Action<T> handler) => internalEvent += handler;
        public void Unsubscribe(Action<T> handler) => internalEvent -= handler;
        public void Invoke(T value) => internalEvent.Invoke(value);
    }
    public class SafeEventUtility    //  サブスクライブUtility(Void型関数)
    {
        private event Action internalEvent = delegate { };

        public void Subscribe(Action handler) => internalEvent += handler;
        public void Unsubscribe(Action handler) => internalEvent -= handler;
        public void Invoke() => internalEvent.Invoke();
    }
}