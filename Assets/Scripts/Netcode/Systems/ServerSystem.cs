using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode.Systems
{
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
            
            EntityCommandBuffer commandBuffer = new (Allocator.Temp);
            
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
            
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        public void SendMessageRpc(string text, World world, Entity target = default)
        {
            if (world is null || (world.IsCreated == false)) { return; }
            
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