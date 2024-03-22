using _Commons.Components;
using Unity.Entities;

namespace _Commons.Helpers
{
    public static class ClocksHelpers
    {
        public static void ClockInit(ref SystemState state)
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
        
        public static void CleanTheTick(ref SystemState state, Entity previousTick)
        {
            state.EntityManager.DestroyEntity(previousTick);
        }
        
        public static void TickTheClock(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<TickUpdateTag>();
        }
        
        public static void ClockUpdate(ref SystemState state, ref RefRW<TickClockComponent> currentTick, float delta)
        {
            uint deltaTick = 0;
            uint deltaMs = (uint)(delta * 1000);

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