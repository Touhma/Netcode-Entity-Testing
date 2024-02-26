using Netcode.Commands;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Netcode.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class SendClientMessageSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

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