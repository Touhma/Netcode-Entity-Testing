using _Commons.Commands;
using _Commons.Components;
using _Commons.SystemGroups;
using Netcode.Components;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers.Systems.Network
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

            state.EntityManager.CreateEntity();
            
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithAll<InitializedClientTag>().WithEntityAccess())
            {
                
            }
        }
    }
}