using BPControlRoomWebAPI.Infra.Abstractions;
using System;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public static class AutomatecExecutorsFactory
    {
        public static IAutomatecExecutable CreateExecutor<T>() where T : IAutomatecExecutable, new() => new T();
        public static IAutomatecExecutable CreateExecutor<T>(string path) where T : IAutomatecExecutable, new()
        {
            return (T)Activator.CreateInstance(typeof(T), path);
        }
    }
}
