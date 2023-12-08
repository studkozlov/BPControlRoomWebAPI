using BPControlRoomWebAPI.Infra.Abstractions;

namespace BPControlRoomWebAPI.Infra.Classes
{
    public static class BpHttpExecutorsFactory
    {
        public static IBpHttpExecutable CreateExecutor<T>() where T : IBpHttpExecutable, new() => new T();
    }
}
