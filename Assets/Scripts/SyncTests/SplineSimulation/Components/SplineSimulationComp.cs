using Unity.Entities;
using Unity.Mathematics;

namespace Netcode.Components
{
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
}