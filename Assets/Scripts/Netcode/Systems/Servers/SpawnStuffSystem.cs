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
    public partial class SpawnStuffSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

          foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<SpawnUnitRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnUnitRpcCommand>>().WithEntityAccess())
            {
                if (!SystemAPI.TryGetSingleton(out PrefabsComponentData prefabsComponentData) || prefabsComponentData.Unit == Entity.Null) continue;

                Entity unit = commandBuffer.Instantiate(prefabsComponentData.Unit);
                commandBuffer.SetComponent(unit, new LocalTransform()
                {
                    Position = new float3(UnityEngine.Random.Range(-10, 10f), 0, UnityEngine.Random.Range(-10, 10f)),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                
                commandBuffer.SetComponent(unit, new GhostOwner()
                {
                    NetworkId = -1
                });
                
                commandBuffer.DestroyEntity(entity);
            }


            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}