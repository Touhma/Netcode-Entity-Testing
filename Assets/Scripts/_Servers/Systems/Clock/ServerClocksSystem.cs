using _Commons.Components;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Entities;

namespace _Servers.Systems.Clock
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(UpdateTickGroup))]
    [UpdateBefore(typeof(ServerTickSyncReceiveSystem))]
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