using _Commons.Components;
using _Commons.Helpers;
using Unity.Entities;
using UnityEngine;

namespace _Servers.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(ServerTickSyncSystem))]
    public partial struct ServerClocksSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            ClocksHelpers.ClockInit(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();
            ClocksHelpers.ClockUpdate(ref state, ref currentTick, SystemAPI.Time.DeltaTime);
        }
    }
}