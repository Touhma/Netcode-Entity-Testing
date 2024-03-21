using _Commons.Components;
using Unity.Entities;

namespace _Servers.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(ServerTickSyncSystem))]
    public partial struct ServerClocksSystem : ISystem
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
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();
            RefRW<TickTimerComponent> clockTimer = SystemAPI.GetSingletonRW<TickTimerComponent>();

            uint deltaTick = 0;
            for (clockTimer.ValueRW.AccumulatedTime += (uint)(SystemAPI.Time.DeltaTime * 1000); clockTimer.ValueRW.AccumulatedTime >= clockTimer.ValueRW.TickDt;)
            {
                currentTick.ValueRW.CurrentTick++;
                deltaTick++;
                clockTimer.ValueRW.AccumulatedTime -= clockTimer.ValueRW.TickDt;
                currentTick.ValueRW.CurrentPartialTick = clockTimer.ValueRW.AccumulatedTime;
              
            }
            currentTick.ValueRW.DeltaTick = deltaTick;
        }
    }
}