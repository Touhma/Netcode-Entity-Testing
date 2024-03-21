using _Commons.Components;
using Unity.Entities;
using UnityEngine;

namespace _Clients.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation| WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ClientClocksSystem : ISystem, ISystemStartStop
    {
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new TickClockComponent()
            {
                CurrentTick = 0,
                CurrentPartialTick = 0
            });
            
            state.EntityManager.CreateSingleton(new TickTimerComponent()
            {
                TickDt = (uint)((1000f / 10)),
                AccumulatedTime = 0
            });
            
            state.RequireForUpdate<ConnectionEstablishedTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();
            RefRW<TickTimerComponent> clockTimer = SystemAPI.GetSingletonRW<TickTimerComponent>();
            
            for (clockTimer.ValueRW.AccumulatedTime += (uint)(SystemAPI.Time.DeltaTime * 1000); clockTimer.ValueRW.AccumulatedTime >= clockTimer.ValueRW.TickDt;)
            {
                currentTick.ValueRW.CurrentTick++;
                clockTimer.ValueRW.AccumulatedTime -= clockTimer.ValueRW.TickDt;
                currentTick.ValueRW.CurrentPartialTick = clockTimer.ValueRW.AccumulatedTime;
            }
        }

        public void OnStartRunning(ref SystemState state)
        {
            Debug.LogWarning("ClientClock Initiated");
        }

        public void OnStopRunning(ref SystemState state) { }
    }
}