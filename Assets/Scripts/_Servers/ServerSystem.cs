
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ServerSystem : SystemBase
    {

        private ComponentLookup<NetworkId> _clients;

        protected override void OnCreate()
        {
            _clients = GetComponentLookup<NetworkId>(true);
        }

        protected override void OnUpdate()
        {
            _clients.Update(this);
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                Debug.LogWarning("InitializedClient");
                commandBuffer.AddComponent<InitializedClient>(entity);
            }
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}