using Unity.Collections;
using Unity.Entities;

namespace _Servers.Components
{
    public struct NetworkMappingSingleton : IComponentData
    {
        public NativeParallelMultiHashMap<Entity, RttElement> MappingRTT;
        public NativeHashMap<uint, uint> MappingLatestTick;
    }

    public struct RttElement
    {
        public uint Tick;
        public uint Latency;
    }
}