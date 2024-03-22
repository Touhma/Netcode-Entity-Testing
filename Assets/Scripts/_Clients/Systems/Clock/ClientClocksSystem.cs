using _Commons.Components;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Entities;
using UnityEngine;

namespace _Clients.Systems.Clock
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(UpdateTickGroup))]
    public partial struct ClientClocksSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            ClocksHelpers.ClockInit(ref state);
            // Client Specific - start the clock after connection established
            state.RequireForUpdate<ConnectionEstablishedTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.LogWarning("ClientClock Initiated");
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();
            ClocksHelpers.ClockUpdate(ref state, ref currentTick, SystemAPI.Time.DeltaTime);
        }
    }
}