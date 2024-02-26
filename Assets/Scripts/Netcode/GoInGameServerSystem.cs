using Netcode.CommandsRpc;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new(Allocator.Temp);
            builder.WithAll<ReceiveRpcCommandRequest, GoInGameCommand>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<GoInGameCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GoInGameCommand>>().WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
                commandBuffer.DestroyEntity(entity);
            }
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}