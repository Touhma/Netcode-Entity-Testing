using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Netcode.Components
{
    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct TestSyncComponent : IComponentData
    {
        [GhostField(Quantization = 1000)] public float Time;
    }

    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct TestEntitySyncComponent : IComponentData
    {
        [GhostField(Quantization = 1000)] public float Time;
    }

    [GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
    public struct TestSyncBuffer : IBufferElementData
    {
        [GhostField(Quantization = 1000)] public float Progress;
    }

    public struct SplinePositions : IComponentData
    {
        public float3 Start;
        public float3 End ;
    }

    public struct SplineProgress : IComponentData
    {
        public Entity Spline;
        public float Position;
    }
    
    public struct TestInstTag: IComponentData{}
}