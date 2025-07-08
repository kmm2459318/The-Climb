/////////////////////////////////////////////  aplha‚Ì‚½‚ßŒã‰ñ‚µ  /////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public interface IMessage { }
//public static class MessageBus
//{
//    private static readonly Dictionary<Type, List<Delegate>> Listeners = new();    //  
    
//    [RuntimeInitializeOnLoadMethod]
//    private static void Init()
//    {
//        Listeners.Clear();
//    }
//    //  ƒCƒxƒ“ƒg‚ğw“Ç
//    public static void Subscribe<T>(Action<T> handler) where T : IMessage
//    {
//        var type = typeof(T);
//        if(!Listeners.ContainsKey(type))
//        {
//            Listeners[type] = new List<Delegate>();
//        }
//        if (!Listeners[type].Contains(handler))
//        {
//            Listeners[type].Add(handler);
//#if UNITY_EDITOR
//            Debug.Log($"[MessageBus] Subscribed to {type.Name}");
//#endif
//        }
//    }
//    public static void Unsubscribe<T>(Action<T> Handler) where T : IMessage
//    {

//    }
//}