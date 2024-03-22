using _Commons.Components;
using _Commons.SystemGroups;
using Netcode.Components;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers.Systems
{
    [UpdateInGroup(typeof(UpdateOnTickGroup))]
    public partial struct ServerRTTMapping : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();

            if (currentTick.ValueRO.TickIncreaseDelta == 0) return;
            
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithAll<InitializedClientTag>().WithEntityAccess())
            {
                
            }
        }
    }
}