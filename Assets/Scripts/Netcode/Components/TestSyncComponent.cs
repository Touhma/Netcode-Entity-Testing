using Unity.Entities;
using Unity.NetCode;

namespace Netcode.Components
{
    [GhostComponent(PrefabType=GhostPrefabType.All, SendTypeOptimization=GhostSendType.AllClients)]
    public struct TestSyncComponent : IComponentData
    {
        [GhostField(Quantization=1000)] public float Time;
    }

  
}