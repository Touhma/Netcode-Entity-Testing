using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ServerInitializeClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClientTag>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClientTag>(entity);
            }

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}