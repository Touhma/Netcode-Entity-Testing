using _Commons.Components;
using Unity.Entities;
using UnityEngine;

namespace _Clients.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ClientClocksSystem : ISystem, ISystemStartStop
    {
        public void OnCreate(ref SystemState state)
        {
            TickClockComponent tickClock = new()
            {
                TickDelta = (uint)(1000f / 10),
                TickLatest = 0,
                TickLatestPartial = 0,
                TickIncreaseDelta = 0,
                TickCurrentPartial = 0,
                TickCurrentDelta = 0,
            };
            state.EntityManager.CreateSingleton(tickClock);

            state.RequireForUpdate<ConnectionEstablishedTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();

            uint deltaTick = 0;
            uint deltaMs = (uint)(SystemAPI.Time.DeltaTime * 1000);

            currentTick.ValueRW.TickCurrentDelta = deltaMs;

            for (currentTick.ValueRW.TickCurrentPartial += deltaMs; currentTick.ValueRW.TickCurrentPartial >= currentTick.ValueRW.TickDelta;)
            {
                currentTick.ValueRW.TickLatest++;
                currentTick.ValueRW.TickIncreaseDelta = deltaTick++;
                currentTick.ValueRW.TickCurrentPartial -= currentTick.ValueRW.TickDelta;
                currentTick.ValueRW.TickLatestPartial = currentTick.ValueRW.TickCurrentPartial;
            }
        }

        public void OnStartRunning(ref SystemState state)
        {
            Debug.LogWarning("ClientClock Initiated");
        }

        public void OnStopRunning(ref SystemState state) { }
    }
}