using _Commons.Components;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Entities;

namespace _Commons.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PostTickGroup))]
    public partial struct ClockTickingSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.TryGetSingletonEntity<TickUpdateTag>(out Entity previousTick);
            if (previousTick != Entity.Null)
            {
                ClocksHelpers.CleanTheTick(ref state, previousTick);
            }
            
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();
            if (currentTick.ValueRW.TickIncreaseDelta != 0) return;
            
            ClocksHelpers.TickTheClock(ref state);
        }
    }
}