using _Commons.Helpers;
using Gameplay.Commons.Architectures.SystemGroups;
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers.Systems.Network
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(GameLoopGameplayGroup))]
    public partial struct ServerInitializeClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClientTag>().WithEntityAccess())
            {
                NetworkHelper.InitializeClient(ref commandBuffer, entity);
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}