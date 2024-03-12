using _Commons.Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Clients.Systems
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(ClientTickSystem))]
    public partial struct ClientTickUpdateSystem : ISystem
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
                Debug.Log(NetworkTimeSystem.TimestampMS + " --> current Client Tick = " + tick.ValueRW.CurrentTick);
            }
        }
    }
}