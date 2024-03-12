using _Commons.Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Servers.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(ServerTickSystem))]
    public partial struct ServerTickUpdateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LockstepTick>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach ((RefRW<LockstepTick> tick, Entity entity) in SystemAPI.Query<RefRW<LockstepTick>>().WithEntityAccess())
            {
                tick.ValueRW.CurrentTick++;
                Debug.Log(NetworkTimeSystem.TimestampMS + " --> current Server Tick = " + tick.ValueRW.CurrentTick);
            }
        }
    }
}