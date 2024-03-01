using Authorings;
using Netcode.CommandsRpc;
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
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
            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<ClientMessageRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
            {
                Debug.Log(command.ValueRO.Message + " from client index " + request.ValueRO.SourceConnection.Index + " version " + request.ValueRO.SourceConnection.Version);
                commandBuffer.DestroyEntity(entity);
            }

            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<SpawnUnitRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnUnitRpcCommand>>().WithEntityAccess())
            {
                if (SystemAPI.TryGetSingleton(out PrefabsData prefabs) && prefabs.Unit != null)
                {
                    Entity unit = commandBuffer.Instantiate(prefabs.Unit);
                    commandBuffer.SetComponent(unit, new LocalTransform()
                    {
                        Position = new float3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });

                    NetworkId networkId = _clients[request.ValueRO.SourceConnection];
                    
                    commandBuffer.SetComponent(unit, new GhostOwner()
                    {
                        NetworkId = networkId.Value
                    });

                    commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup()
                    {
                        Value = unit
                    });

                    commandBuffer.DestroyEntity(entity);
                }
            }

            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClient>(entity);
                PrefabsData prefabManager = SystemAPI.GetSingleton<PrefabsData>();
                if (prefabManager.Player != null)
                {
                    Entity player = commandBuffer.Instantiate(prefabManager.Player);
                    commandBuffer.SetComponent(player, new LocalTransform()
                    {
                        Position = new float3(UnityEngine.Random.Range(-10f, 10f), 0, UnityEngine.Random.Range(-10f, 10f)),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });
                    commandBuffer.SetComponent(player, new GhostOwner()
                    {
                        NetworkId = id.ValueRO.Value
                    });
                    commandBuffer.AppendToBuffer(entity, new LinkedEntityGroup()
                    {
                        Value = player
                    });
                }
            }
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        public void SendMessageRpc(string text, World world, Entity target = default)
        {
            if (world == null || world.IsCreated == false)
            {
                return;
            }
            Entity entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ServerMessageRpcCommand));
            world.EntityManager.SetComponentData(entity, new ServerMessageRpcCommand()
            {
                Message = text
            });
            if (target != Entity.Null)
            {
                world.EntityManager.SetComponentData(entity, new SendRpcCommandRequest()
                {
                    TargetConnection = target
                });
            }
        }

    }
}