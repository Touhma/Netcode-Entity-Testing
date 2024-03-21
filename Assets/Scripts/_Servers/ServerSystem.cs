
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ServerSystem : ISystem
    {
        private ComponentLookup<NetworkId> _clients;

        public void OnCreate(ref SystemState state)
        {
            _clients = SystemAPI.GetComponentLookup<NetworkId>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClientTag>().WithEntityAccess())
            {
                Debug.LogWarning("InitializedClient");
                commandBuffer.AddComponent<InitializedClientTag>(entity);
            }
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}