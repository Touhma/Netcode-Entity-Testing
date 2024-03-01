using Netcode.Components;
using Unity.Burst;
using Unity.Entities;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UpdateTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TestSyncComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<TestSyncComponent> tsc = SystemAPI.GetSingletonRW<TestSyncComponent>();
            tsc.ValueRW.Time += SystemAPI.Time.DeltaTime;
        }
    }
}