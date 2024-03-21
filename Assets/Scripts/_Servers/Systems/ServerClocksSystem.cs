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
    }
}