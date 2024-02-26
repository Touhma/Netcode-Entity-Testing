﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Netcode.Systems
{
    public class InventoryGridData : IComponentData
    {
        public int Value = 0;
    }

    public struct InitializedClient : IComponentData { }

    public struct ServerMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class ServerSystem : SystemBase
    {
        private ComponentLookup<NetworkId> _clients;

        protected override void OnCreate()
        {
            _clients = GetComponentLookup<NetworkId>();
        }

        protected override void OnUpdate()
        {
            _clients.Update(this);

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach ((RefRO<NetworkId> networkId, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClient>(entity);
                SendMessageRpc("Client connected with id = " + networkId.ValueRO.Value, ConnectionManager.ServerWorld);
            }

            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<ClientMessageRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(entity);
                SendMessageRpc(command.ValueRO.Message + " From client index " + request.ValueRO.SourceConnection.Index + "version " + request.ValueRO.SourceConnection.Version, ConnectionManager.ServerWorld);
            }

            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<SpawnUnitRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnUnitRpcCommand>>().WithEntityAccess())
            {
                Debug.Log("Server Spawn Check");
                if (!SystemAPI.TryGetSingleton(out PrefabsComponentData prefabsComponentData) || prefabsComponentData.Prefab == Entity.Null) continue;

                Debug.Log("Server Spawn");
                Entity unit = commandBuffer.Instantiate(prefabsComponentData.Prefab);
                commandBuffer.SetComponent(unit, new LocalTransform()
                {
                    Position = new float3(UnityEngine.Random.Range(-10, 10f), 0, UnityEngine.Random.Range(-10, 10f)),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                //if we want the client to be the owner 
                /*
                commandBuffer.SetComponent(unit, new GhostOwner()
                {
                    NetworkId = _clients[request.ValueRO.SourceConnection].Value
                });
                */
                commandBuffer.DestroyEntity(entity);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        public void SendMessageRpc(string text, World world, Entity target = default)
        {
            if (world is null || (world.IsCreated == false))
            {
                return;
            }

            Entity messageEntity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ServerMessageRpcCommand));
            world.EntityManager.SetComponentData(messageEntity, new ServerMessageRpcCommand()
            {
                Message = text
            });

            if (target != Entity.Null)
            {
                world.EntityManager.SetComponentData(messageEntity, new SendRpcCommandRequest()
                {
                    TargetConnection = target
                });
            }
        }
    }
}