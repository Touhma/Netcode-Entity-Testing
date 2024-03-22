using Unity.Collections;
using Unity.Entities;

namespace _Servers.Components
{
    public struct NetworkMappingSingleton : IComponentData
    {
        public NativeHashMap<int, uint> MappingRTT;
        public NativeHashMap<int, uint> MappingLatestTick;

        public uint SlowestClientRTT()
        {
            uint slowest = 0;
            foreach (KVPair<int, uint> kvPair in MappingRTT)
            {
                if (kvPair.Value <= slowest) continue;
                slowest = kvPair.Value;
            }

            return slowest;
        }
    }
}