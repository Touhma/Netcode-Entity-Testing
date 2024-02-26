using Netcode.CommandsRpc;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode
{

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientSystem : SystemBase
    {

        protected override void OnCreate()
        {
            RequireForUpdate<NetworkId>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<ServerMessageRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerMessageRpcCommand>>().WithEntityAccess())
            {
                Debug.Log(command.ValueRO.Message);
                commandBuffer.DestroyEntity(entity);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnUnitRpc(ConnectionManager.ClientWorld);
            }
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        public void SendMessageRpc(string text, World world)
        {
            if (world == null || world.IsCreated == false)
            {
                return;
            }
            Entity entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ClientMessageRpcCommand));
            world.EntityManager.SetComponentData(entity, new ClientMessageRpcCommand()
            {
                Message = text
            });
        }

        public void SpawnUnitRpc(World world)
        {
            if (world == null || world.IsCreated == false)
            {
                return;
            }
            world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(SpawnUnitRpcCommand));
        }

    }
}