using System;
using System.Collections.Generic;

namespace TheClimb.Core
{
    public static class ServiceLocator    //  サービスロケーター
    {
        static readonly Dictionary<Type, object> services = new();

        public static void Register<T>(T service)    //  登録
        {
            services[typeof(T)] = service;
        }

        public static T Resolve<T>()     //  呼び出し
        {
            return (T)services[typeof(T)];
        }
    }
}