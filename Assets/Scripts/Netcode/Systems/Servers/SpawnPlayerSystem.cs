using Netcode.Commands;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Netcode.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class SpawnPlayerSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach ((RefRO<NetworkId> networkId, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClient>(entity);
                if (!SystemAPI.TryGetSingleton(out PrefabsComponentData prefabsComponentData) || prefabsComponentData.Player == Entity.Null) continue;
                Entity player = commandBuffer.Instantiate(prefabsComponentData.Unit);
                    
                commandBuffer.SetComponent(player, new GhostOwner()
                {
                    NetworkId = networkId.ValueRO.Value
                });
                
                commandBuffer.AppendToBuffer(entity, new LinkedEntityGroup()
                {
                    Value = player
                });
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
        
    }
}